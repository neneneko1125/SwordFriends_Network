using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("最大HP")]
    [SerializeField] private int _maxHP;

    [Header("移動速度")]
    [SerializeField] private int _moveSpeed;

    [Header("基礎攻撃力")]
    [SerializeField] private int _baseAttackPower;

    [Header("会心率")]
    [SerializeField] private float _criticalProbability;

    //プロパティ(外部から読み取る用)
    public int MaxHP => _maxHP;
    public int MoveSpeed => _moveSpeed;
    public int BaseAttackPower => _baseAttackPower;
    public float CriticalProbability => _criticalProbability;

}