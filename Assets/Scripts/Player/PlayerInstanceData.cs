using UnityEngine;

/// <summary>
/// ここでは計算だけ　同期はそれぞれのクラスで
/// </summary>
public class PlayerInstanceData : ICharacterInstance
{
    public PlayerData PlayerData;

    public float MoveSpeed;
    public float JumpForce;
    public float LadderSpeed;
    public int BaseAttackPower;
    public float CriticalProbability;

    public int CurrentHP { get; set; }
    public BaseCharacterData MasterData => PlayerData;
    public bool IsDead => CurrentHP <= 0;

    public PlayerInstanceData(PlayerData playerData)
    {
        PlayerData = playerData;

        CurrentHP = playerData.MaxHP;
        MoveSpeed = playerData.MoveSpeed;
        JumpForce = playerData.JumpForce;
        LadderSpeed = playerData.LadderSpeed;
        BaseAttackPower = playerData.BaseAttackPower;
        CriticalProbability = playerData.CriticalProbability;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
        }
    }
}
