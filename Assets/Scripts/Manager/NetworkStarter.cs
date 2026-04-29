using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour
{
    [SerializeField] private GameObject _connectButton;
    [SerializeField] private GameObject _preparationCompletebutton;
    [SerializeField] private PlayerData _playerDataPrefab;

    private bool _isSceneLoading = false;

    private NetworkRunner _runner;

    [Networked] public bool IsPreparationCompleted { get; private set; } = false;

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
            SessionName = null,   // 部屋の名前 ここがnullのとき、空いてる部屋があればそこに入り、なければ自分で作る
            Scene = sceneInfo,  //シーン
            PlayerCount = 4,    //人数
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            Debug.Log("接続成功: " + result.ShutdownReason + "  " + _runner.SessionInfo.Name);
            _runner.Spawn(_playerDataPrefab, Vector3.zero, Quaternion.identity, _runner.LocalPlayer);
            _connectButton.SetActive(false);
            _preparationCompletebutton.SetActive(true);
        }
        else
        {
            Debug.Log("接続失敗: " + result.ShutdownReason);
        }
    }

    public void OnClickPreparationComplete()
    {
        if(PlayerData.Local != null)
        {
            PlayerData.Local.Rpc_SetReady(true);
            _preparationCompletebutton.SetActive(false);
        }
    }

    private void Update()
    {
        if (_runner == null || !_runner.IsSceneAuthority || _isSceneLoading)
        {
            return;
        }

        // 結果(ready, total)を取得
        var status = GetReadyStatus();

        // 準備が整ったか判定
        if (status.total > 0 && status.ready == status.total)
        {
            _isSceneLoading = true;
            _runner.LoadScene("MainScene");
        }
    }

    // タプルを使って、準備数と総人数を同時に受け取る
    private (int ready, int total) GetReadyStatus()
    {
        int ready = 0;
        int total = 0;

        foreach (var obj in _runner.GetAllNetworkObjects())
        {
            if (obj.TryGetComponent<PlayerData>(out var data))
            {
                total++;
                if (data.IsReady) ready++;
            }
        }
        return (ready, total);
    }

}