using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;


public class NetworkStarter : MonoBehaviour
{
    private NetworkRunner _runner;

    [SerializeField] private GameObject _button;

    public async void OnClickJoinGame()
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>(); 
        }

        // このパソコンからの入力を読み取れるように
        _runner.ProvideInput = true;

        //Fusionでは単なるシーン名を渡すのではなく、NetworkSceneInfoに情報を入れてから渡すルール
        var sceneInfo = new NetworkSceneInfo();
        //FromIndex:シーンリストのどのインデックスか　GetActiveScene:現在のシーン buildInde:そのシーンのインデックスを取得
        sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));

        StartGameResult result;

        //通信開始
        result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = null,   //部屋の名前　nullなら空いてる部屋を探す
            Scene = sceneInfo,  //シーン
            PlayerCount = 4,    //人数
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            Debug.Log("接続成功: " + result.ShutdownReason + "  " + _runner.SessionInfo.Name);
            _button.SetActive(false);
        }
        else
        {
            Debug.Log("接続失敗: " + result.ShutdownReason);
        }
    }
}