using Fusion.Sockets;
using Fusion;
using System;
using UnityEngine;

public struct MyInputData : INetworkInput
{
    public float Horizontal;
    public bool JumpPressed;
    public bool AttackPressed;
    public bool RightDashAttackPressed;
    public bool LeftDashAttackPressed;
    public bool DownAttackPressed;
    public bool UpAttackPressed;
    public bool LadderPressed;
    public bool LadderReleased;
}


public class PlayerInput : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private bool _jumpRequested;
    private bool _attackRequested;
    private bool _rightDashAttackRequested;
    private bool _leftDashAttackRequested;
    private bool _downAttackRequested;
    private bool _upAttackRequested;
    private bool _ladderRequested;
    private bool _ladderReleasedRequested;

    public void OnEnable()
    {
        if (_runner == null)
        {
            _runner = FindFirstObjectByType<NetworkRunner>();
        }

        if (_runner != null)
        {
            _runner.AddCallbacks(this);
        }
    }

    public void OnDisable()
    {
        if (_runner != null)
        {
            _runner.RemoveCallbacks(this);
        }
    }

    private void Update()
    {
        float direction = Input.GetAxisRaw("Horizontal");

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.S))
            {
                if(direction > 0)
                {
                    _rightDashAttackRequested = true;
                }
                else if(direction < 0)
                {
                    _leftDashAttackRequested = true;
                }
                else
                {
                    _downAttackRequested = true;
                }
            }
            else
            {
                _attackRequested = true;
            }
        }
        //左クリックしたままジャンプすれば上攻撃
        else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (Input.GetMouseButton(0))
            {
                _upAttackRequested = true;
            }
            else
            {
                _jumpRequested = true;
            }
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            _ladderRequested = true;
        }
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
        {
            _ladderReleasedRequested = true;
        }
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var myData = new MyInputData();
        myData.Horizontal = Input.GetAxisRaw("Horizontal");
        myData.JumpPressed = _jumpRequested;
        myData.AttackPressed = _attackRequested;
        myData.RightDashAttackPressed = _rightDashAttackRequested;
        myData.LeftDashAttackPressed = _leftDashAttackRequested;
        myData.DownAttackPressed = _downAttackRequested;
        myData.UpAttackPressed = _upAttackRequested;
        myData.LadderPressed = _ladderRequested;
        myData.LadderReleased = _ladderReleasedRequested;

        // どの瞬間に押したかを正確に固定する
        input.Set(myData); 

        _jumpRequested = false;
        _attackRequested = false;
        _rightDashAttackRequested = false;
        _leftDashAttackRequested = false;
        _downAttackRequested = false;
        _upAttackRequested = false;
        _ladderRequested = false;
        _ladderReleasedRequested = false;
    }


    // これを書かないとエラー
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress address, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
