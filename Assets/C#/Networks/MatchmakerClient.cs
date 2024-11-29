using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using StatusOptions = Unity.Services.Matchmaker.Models.MultiplayAssignment.StatusOptions;
using UnityEngine;
#if UNITY_EDITOR
using ParrelSync;
#endif

/// <summary>
/// Client-side script that signs the client in anonymously, connects to the matchmaker service, creates a ticket, and attempts to find a match given the passed in properties.
/// </summary>
public class MatchmakerClient
{
    private string _ticketId;
    
    public void Init()
    {
        //Debug.Log("1: MatchmakerClient Init");
        ServerStartUp.ClientInstance += SignIn;
    }

    private void OnDisable()
    {
        ServerStartUp.ClientInstance -= SignIn;
    }

    /// <summary>
    /// Initializes UnityServices and Authenticates the client anonymously. 
    /// </summary>
    private async void SignIn()
    {
        //Debug.Log("3: SignIn");
        await ClientSignIn("HeroPlayer");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    }
    /// <summary>
    /// Initializes UnityServices. If the client is running on a local build with Parallel Sync, gets the current clone number and adds that to the service profile name to avoid testing issues.
    /// </summary>
    /// <param name="serviceProfileName">Service Profile Name to be passed in with the player if any.</param>
    private async Task ClientSignIn(string serviceProfileName = null)
    {
        if (serviceProfileName != null)
        {
            #if UNITY_EDITOR
            serviceProfileName = $"{serviceProfileName}{GetCloneNumberSuffix()}";
            #endif
            var initOptions = new InitializationOptions();
            initOptions.SetProfile(serviceProfileName);
            await UnityServices.InitializeAsync(initOptions);
        }
        else
        {
            await UnityServices.InitializeAsync();
        }
        
        Debug.Log($"Signed In Anonymously as {serviceProfileName}({PlayerID()})");
    }

    /// <summary>
    /// Helped function to get the ID of the player connected to Unity Services.
    /// </summary>
    private string PlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }
    
    /// <summary>
    /// If we are running ParallelSync, determines the current clone number. This is to avoid issues testing locally with UGS.
    /// </summary>
    /// <returns>Current clone number as a string.</returns>
    #if UNITY_EDITOR
    private string GetCloneNumberSuffix()
    {
        {
            string projectPath = ClonesManager.GetCurrentProjectPath();
            int lastUnderscore = projectPath.LastIndexOf("_", StringComparison.Ordinal);
            string projectCloneSuffix = projectPath.Substring(lastUnderscore + 1);
            if (projectCloneSuffix.Length != 1)
                projectCloneSuffix = "";
            return projectCloneSuffix;
        }
    }
    #endif

    /// <summary>
    /// Called when the client clicks the UI Start button. Starts the matchmaking process.
    /// </summary>
    public void StartClient()
    {
        CreateATicket();
    }

    /// <summary>
    /// Creates a matchmaking ticket with the passed in player properties.
    /// </summary>
    private async void CreateATicket()
    {
        var options = new CreateTicketOptions("HeroMode");

        var players = new List<Player>
        {
            new Player(
                PlayerID(),
                new MatchmakingPlayerData
                {
                    skill = 100,
                }
            )
        };

        var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
        _ticketId = ticketResponse.Id;
        Debug.Log($"Ticket ID: {_ticketId}");
        PollTicketStatus();
    }

    /// <summary>
    /// Loops until the ticket status is found, failed, or timed out. If the ticket is found, calls TicketAssigned.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private async void PollTicketStatus()
    {
        MultiplayAssignment multiplayAssignment = null;
        bool gotAssignment = false;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(_ticketId);
            if (ticketStatus == null) continue;
            if (ticketStatus.Type == typeof(MultiplayAssignment))
            {
                multiplayAssignment = ticketStatus.Value as MultiplayAssignment;
                ;
            }

            switch (multiplayAssignment.Status)
            {
                case StatusOptions.Found:
                    gotAssignment = true;
                    TicketAssigned(multiplayAssignment);
                    break;
                case StatusOptions.InProgress:
                    break;
                case StatusOptions.Failed:
                    gotAssignment = true;
                    Debug.LogError($"Failed to get ticket status. Error: {multiplayAssignment.Message}");
                    break;
                case StatusOptions.Timeout:
                    gotAssignment = true;
                    Debug.LogError("Failed to get ticket status. Ticket timed out.");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        } while (!gotAssignment);
    }

    /// <summary>
    /// Called when the ticket is found. Sets the connection data and starts the client.
    /// </summary>
    /// <param name="assignment">The server assignment that was previously found.</param>
    private void TicketAssigned(MultiplayAssignment assignment)
    {
        Debug.Log($"Ticket Assigned: {assignment.Ip}:{assignment.Port}");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(assignment.Ip, (ushort)assignment.Port);
        NetworkManager.Singleton.StartClient();
    }
    
    /// <summary>
    /// Example serializable player data to be passed in with the player for matchmaker.
    /// </summary>
    [Serializable]
    public class MatchmakingPlayerData
    {
        public int skill;
    }
}
