using UnityEngine;
using Fusion;

public class SimpleSpawner : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private bool _spawned = false;

    /// <summary>
    /// 自分がネットワークに接続した時に実行
    /// </summary>
    public override void Spawned()
    {
        // この時点でネットワークに繋がってる場合はPlayerを生成
        if (Runner.IsRunning)
        {
            TrySpawn(Runner.LocalPlayer);
        }
    }

    /// <summary>
    /// 誰かが参加した時に実行
    /// </summary>
    /// <param name="player"></param>
    public void PlayerJoined(PlayerRef player)
    {
        //その人を生成
        TrySpawn(player);
    }

    private void TrySpawn(PlayerRef player)
    {
        // 生成するのは「自分」の分だけ かつ まだ生成していない場合だけ実行
        if (player == Runner.LocalPlayer && !_spawned)
        {
            Runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
            _spawned = true;
        }
    }
}