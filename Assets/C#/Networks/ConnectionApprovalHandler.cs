using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Approval process for new connecting clients.
/// Currently we are just making sure the amount of players does not exceed the MaxPlayers count.
/// Here you can do all sorts of pre-processing such as selecting different prefabs for the player to spawn with depending on some condition.
/// https://docs-multiplayer.unity3d.com/netcode/current/basics/connection-approval/
/// </summary>
public class ConnectionApprovalHandler
{
    public static int MaxPlayers = 3;

    public void Init()
    {
        //Debug.Log("4: ConnectionApprovalHandler Init");
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Server is Full";
        }

        response.Pending = false;
    }
}
