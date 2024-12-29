using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

/// <summary>
/// Server side script that handles starting the server and connecting to Unity Gaming Services.
/// It is important to note that you cannot call a ServerRPC from a server.
/// </summary>
public class ServerStartUp
{
    // This event is invoked when a client instance is detected
    public static event Action ClientInstance;
    
    #region Private Variables
    
    private const string InternalServerIP = "0.0.0.0";
    private string _externalServerIP = "0.0.0.0";
    private ushort _serverPort = 7777;
    private string _externalConnectionString => $"{_externalServerIP}:{_serverPort}";

    private IMultiplayService _multiplayService;
    private const int _multiplayServiceTimeout = 20000;
    private string _allocationId;
    private MultiplayEventCallbacks _serverCallbacks;
    private IServerEvents _serverEvents;
    private BackfillTicket _localBackfillTicket;
    private CreateBackfillTicketOptions _createBackfillTicketOptions;
    private const int _ticketCheckMs = 1000;
    private MatchmakingResults _matchmakingPayload;
    private bool _backfilling = false;
    
    #endregion Private Variables

    /// <summary>
    /// Reads the command line arguments and starts the server if the dedicated server flag is set.
    /// Else, it starts the client.
    /// </summary>
    public async void Init()
    {
        bool server = false;
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
            {
                server = true;
            }

            if (args[i] == "-port" && (i + 1 < args.Length))
            {
                _serverPort = (ushort)int.Parse(args[i + 1]);
            }

            if (args[i] == "-ip" && (i + 1 < args.Length))
            {
                _externalServerIP = args[i + 1];
            }
        }

        if (server)
        {
            StartServer();
            await StartServerServices();
        }
        else
        {
            //Debug.Log("2: ServerStartUp Init");
            ClientInstance?.Invoke();
        }
    }

    public void InitClientInstance()
    {
        ClientInstance = null;
    }

    /// <summary>
    /// Starts the server and sets the connection data.
    /// </summary>
    private void StartServer()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(InternalServerIP, _serverPort);
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }

    /// <summary>
    /// Connects to Unity Gaming Services and starts the SQP Service. Then connects to the Matchmaker Service to get the Matchmaker Payload, and starts the Backfill process (backfilling is when players can join the match after it has already started).
    /// </summary>
    async Task StartServerServices()
    {
        await UnityServices.InitializeAsync();
        try
        {
            _multiplayService = MultiplayService.Instance;
            await _multiplayService.StartServerQueryHandlerAsync((ushort)ConnectionApprovalHandler.MaxPlayers, "n/a", "n/a", "0", "n/a");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Something went wrong trying to set up the SQP Service:\n{ex}");
        }

        try
        {
            _matchmakingPayload = await GetMatchmakerPayload(_multiplayServiceTimeout);
            if (_matchmakingPayload != null)
            {
                Debug.Log($"Got payload: {_matchmakingPayload}");
                await StartBackfill(_matchmakingPayload);
            }
            else
            {
                Debug.LogWarning("Getting the Matchmaker Payload timed out, starting with defaults.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Something went wrong trying to set up the Allocation & Backfill Services:\n{ex}");
        }
    }

    /// <summary>
    /// Attempts to get the Matchmaker Payload from the Matchmaker Service. If it doesn't find the allocation after the timeout period, it returns null.
    /// </summary>
    /// <param name="timeout">How long until server timeout  if a matchmaker allocation is not found.</param>
    /// <returns>Matchmaker Payload if found.</returns>
    private async Task<MatchmakingResults> GetMatchmakerPayload(int timeout)
    {
        var matchmakerPayloadTask = SubscribeAndAwaitMatchmakerAllocation();
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    /// <summary>
    /// Subscribes to Matchmaker Events and awaits the allocation ID.
    /// </summary>
    /// <returns>Matchmaker Payload if found.</returns>
    private async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
        if (_multiplayService == null) return null;
        _allocationId = null;
        _serverCallbacks = new MultiplayEventCallbacks();
        _serverCallbacks.Allocate += OnMultiplayAllocation;
        _serverEvents = await _multiplayService.SubscribeToServerEventsAsync(_serverCallbacks);

        _allocationId = await AwaitAllocationID();
        var mmPayload = await GetMatchmakerAllocationPayloadAsync();
        return mmPayload;
    }

    /// <summary>
    /// Is called when a Multiplay allocation has been found. Sets the allocation ID.
    /// </summary>
    /// <param name="allocation">The Multiplay allocation that was found.</param>
    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {
        Debug.Log($"OnAllocation: {allocation.AllocationId}");
        if (string.IsNullOrEmpty(allocation.AllocationId)) return;
        _allocationId = allocation.AllocationId;
    }

    /// <summary>
    /// Awaits the allocation ID from the Multiplay Service. Loops until the allocation ID is found.
    /// </summary>
    /// <returns>The allocation ID.</returns>
    private async Task<string> AwaitAllocationID()
    {
        var config = _multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is:\n" +
                  $"-ServerID: {config.ServerId}\n" +
                  $"-AllocationID: {config.AllocationId}\n" +
                  $"-Port: {config.Port}\n" +
                  $"-QPort: {config.QueryPort}\n" +
                  $"-logs: {config.ServerLogDirectory}");
        while (string.IsNullOrEmpty(_allocationId))
        {
            var configId = config.AllocationId;
            if (!string.IsNullOrEmpty(configId) && string.IsNullOrEmpty(_allocationId))
            {
                _allocationId = configId;
                break;
            }

            await Task.Delay(100);
        }

        return _allocationId;
    }

    /// <summary>
    /// Parses the Matchmaker Payload from the Multiplay Service and returns it.
    /// </summary>
    /// <returns>Matchmaker Payload.</returns>
    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
        try
        {
            var payloadAllocation =
                await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            var modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            Debug.Log($"{nameof(GetMatchmakerAllocationPayloadAsync)}:\n{modelAsJson}");
            return payloadAllocation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Something went wrong trying to get the Matchmaker Payload in GetMatchmakerAllocationPayloadAsync:\n{ex}");
        }

        return null;
    }

    /// <summary>
    /// Backfill is when players can join the match after it has already started. This method starts the backfill process by creating a Backfill ticket.
    /// </summary>
    /// <param name="payload">The matchmaker payload that was previously found.</param>
    private async Task StartBackfill(MatchmakingResults payload)
    {
        var backfillProperties = new BackfillTicketProperties(payload.MatchProperties);
        _localBackfillTicket = new BackfillTicket { Id = payload.MatchProperties.BackfillTicketId, Properties = backfillProperties };
        await BeginBackfilling(payload);
    }

    /// <summary>
    /// This method begins the Backfill loop. If no local backfill ticket is already set, then a new one is created with the match properties.
    /// </summary>
    /// <param name="payload"></param>
    private async Task BeginBackfilling(MatchmakingResults payload)
    {
        if (string.IsNullOrEmpty(_localBackfillTicket.Id))
        {
            var matchProperties = payload.MatchProperties;
            
            _createBackfillTicketOptions = new CreateBackfillTicketOptions
            {
                Connection = _externalConnectionString,
                QueueName = payload.QueueName,
                Properties = new BackfillTicketProperties(matchProperties)
            };

            _localBackfillTicket.Id =
                await MatchmakerService.Instance.CreateBackfillTicketAsync(_createBackfillTicketOptions);
        }

        _backfilling = true;
        # pragma warning disable 4014
        BackfillLoop();
        # pragma warning restore 4014
    }
    
    /// <summary>
    /// This method loops until the backfill ticket is approved or until the match is full.
    /// </summary>
    private async Task BackfillLoop()
    {
        while (_backfilling && NeedsPlayers())
        {
            _localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_localBackfillTicket.Id);
            if (!NeedsPlayers())
            {
                await MatchmakerService.Instance.DeleteBackfillTicketAsync(_localBackfillTicket.Id);
                _localBackfillTicket.Id = null;
                _backfilling = false;
                return;
            }

            await Task.Delay(_ticketCheckMs);
        }

        _backfilling = false;
    }

    /// <summary>
    /// Called when a client disconnects. If the match is not currently full, then backfilling is started.
    /// </summary>
    /// <param name="clientId"></param>
    private void ClientDisconnected(ulong clientId)
    {
        if (!_backfilling && NetworkManager.Singleton.ConnectedClients.Count > 0 && NeedsPlayers())
        {
            # pragma warning disable 4014
            BeginBackfilling(_matchmakingPayload);
            # pragma warning restore 4014
        }
    }
    
    /// <summary>
    /// Determines if the match is full or not.
    /// </summary>
    /// <returns>True if the match is not full, false if the match is full.</returns>
    private bool NeedsPlayers()
    {
        return NetworkManager.Singleton.ConnectedClients.Count < ConnectionApprovalHandler.MaxPlayers;
    }

    /// <summary>
    /// Should be called when processing is finished, this deallocates the previously subscribed to server events.
    /// </summary>
    private void Dispose()
    {
        _serverCallbacks.Allocate -= OnMultiplayAllocation;
        _serverEvents?.UnsubscribeAsync();
    }
}
