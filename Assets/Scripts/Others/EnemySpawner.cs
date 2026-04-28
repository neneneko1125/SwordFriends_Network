using UnityEngine;
using Fusion;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _enemyPrefab;
    [SerializeField] private float _enemySpawnIntervalTime = 10f;

    [Networked] private TickTimer SpawnTimer {  get; set; }

    [Networked] private NetworkObject SpawnedEnemy {  get; set; }

    public override void Spawned()
    {
        // 最初にタイマーをセット
        if (HasStateAuthority)
        {
            if(_enemyPrefab != null)
            {
                SpawnedEnemy = Runner.Spawn(_enemyPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        TrySpawn();
    }

    private void TrySpawn()
    {
        // 敵がまだいる場合は生成処理は行わない
        if (SpawnedEnemy != null && Runner.Exists(SpawnedEnemy))
        {
            return;
        }

        // タイマーが動いていないならインターバルのカウント開始
        if(!SpawnTimer.IsRunning)
        {
            SpawnTimer = TickTimer.CreateFromSeconds(Runner, _enemySpawnIntervalTime);
            return;     //同時に2匹でる事故を防ぐ
        }

        // インターバル時間が過ぎていれば生成
        if (_enemyPrefab != null && SpawnTimer.Expired(Runner))
        {
            SpawnedEnemy = Runner.Spawn(_enemyPrefab, transform.position, Quaternion.identity);
            SpawnTimer = TickTimer.None;    //敵がいなくカウントしない
        }
    }
}
