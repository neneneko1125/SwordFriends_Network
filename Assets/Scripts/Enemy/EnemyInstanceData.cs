
/// <summary>
/// ここでは計算だけ　同期はそれぞれのクラスで
/// </summary>
public class EnemyInstanceData : ICharacterInstance
{
    public EnemyData EnemyData;

    public float PatrolSpeed;
    public float ChaseSpeed;
    public int BaseAttackPower;

    public int CurrentHP { get; set; }
    public BaseCharacterData BaseData => EnemyData;      //マスターデータがほしければこれを使う
    public bool IsDead => CurrentHP <= 0;

    // マスターデータをもらって、初期値を設定
    public EnemyInstanceData(EnemyData enemyData)
    {
        EnemyData = enemyData;

        CurrentHP = enemyData.MaxHP;
        PatrolSpeed = enemyData.PatrolSpeed;
        ChaseSpeed = enemyData.ChaseSpeed;
        BaseAttackPower = enemyData.BaseAttackPower;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
        }
    }
}
