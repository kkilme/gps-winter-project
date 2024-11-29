using System;
using System.Collections;
using System.Collections.Generic;

public class ServerManager
{
    private TargetFPS _targetFPS;
    private MatchmakerClient _matchmakerClient;
    private ServerStartUp _serverStartUp;
    private ConnectionApprovalHandler _connectionApprovalHandler;
    
    public void Init()
    {
        _targetFPS = new TargetFPS();
        _matchmakerClient = new MatchmakerClient();
        _serverStartUp = new ServerStartUp();
        _connectionApprovalHandler = new ConnectionApprovalHandler();
        
        _serverStartUp.InitClientInstance();
        _targetFPS.Init();
        _matchmakerClient.Init();
        _serverStartUp.Init();
        _connectionApprovalHandler.Init();
    }

    public void StartClient()
    {
        _matchmakerClient.StartClient();
    }
}
