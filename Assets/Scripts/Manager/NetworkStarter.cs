using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class NetworkLauncher : MonoBehaviour
{
    private NetworkRunner _runner;

    public async void OnClickJoinGame()
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }

        _runner.ProvideInput = true;

        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "PublicRoom",
            Scene = sceneInfo,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            
        });

        if (result.Ok)
        {
            Debug.Log("ê⁄ë±ê¨å˜");
        }
        else
        {
            Debug.LogError("ê⁄ë±é∏îs: " + result.ShutdownReason);
        }
    }
}