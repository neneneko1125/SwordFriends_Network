using UnityEngine;
using Fusion;

/// <summary>
/// SOデータを配る　
/// InstanceDataを生成し、それぞれのクラスのSetupを実行する(InstanceDataを配る)
/// </summary>
public class EnemyCore : NetworkBehaviour
{
    [SerializeField] private EnemyData _enemyData;


    public override void Spawned()
    {
        var instanceData = new EnemyInstanceData(_enemyData);

        // InstanceDataをコンポーネントたちに配る
        if (TryGetComponent<BaseHP>(out var hp))
        {
            hp.Setup(instanceData);
        }
        if (TryGetComponent<EnemyAttackHandler>(out var attackHandler))
        {
            attackHandler.Setup(instanceData);
        }
        if (TryGetComponent<EnemyMovement>(out var movement))
        {
            movement.Setup(instanceData);
        }
    }
}
