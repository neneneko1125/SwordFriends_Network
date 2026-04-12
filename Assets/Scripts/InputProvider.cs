//using UnityEngine;
//using Fusion;
//using Fusion.Sockets;

//public class InputProvider : MonoBehaviour
//{
//    private NetworkRunner _runner;

//    void Start()
//    {
//        _runner = FindObjectOfType<NetworkRunner>();
//        _runner.AddCallbacks(new MyCallbacks());
//    }

//    class MyCallbacks : INetworkRunnerCallbacks
//    {
//        public void OnInput(NetworkRunner runner, NetworkInput input)
//        {
//            NetworkInputData data = new NetworkInputData();

//            data.move = new Vector2(
//                Input.GetAxis("Horizontal"),
//                Input.GetAxis("Vertical")
//            );

//            input.Set(data);
//        }

//        // ‘¼‚Í‹ó‚ÅOK
//        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
//        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
//        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
//        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
//        public void OnConnectedToServer(NetworkRunner runner) { }
//        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
//        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
//        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
//        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
//        public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
//        public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
//        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
//        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
//        public void OnSceneLoadDone(NetworkRunner runner) { }
//        public void OnSceneLoadStart(NetworkRunner runner) { }
//    }
//}