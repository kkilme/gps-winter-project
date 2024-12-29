using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetworkManagerEx
{
    private NetworkManager _networkManager;
    private UnityTransport _unityTransport;
    
    public void Init()
    {
        if (_networkManager == null)
        {
            GameObject go = GameObject.Find("@NetworkManager");
            if (go == null)
            {
                go = new GameObject { name = "@NetworkManager" };
            }
            
            _networkManager = go.GetOrAddComponent<NetworkManager>();
            _unityTransport = go.GetOrAddComponent<UnityTransport>();
        }
    }
}
