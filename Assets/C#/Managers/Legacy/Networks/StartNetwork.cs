using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Helper functions to start the server, host, or client on the NetworkManager.
/// </summary>
public class StartNetwork : MonoBehaviour
{
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
