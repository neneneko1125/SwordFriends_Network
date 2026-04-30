
using UnityEngine;

/// <summary>
/// プレイヤー特有のステータス
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : BaseCharacterData
{
    [Header("移動速度")]
    [SerializeField] private float _moveSpeed;

    [Header("ジャンプ")]
    [SerializeField] private float _jumpForce;

    [Header("はしごを登る速度")]
    [SerializeField] private float _ladderSpeed;
    [Header("会心率")]
    [SerializeField] private float _criticalProbability;

    //プロパティ(外部から読み取る用)
    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;
    public float LadderSpeed => _ladderSpeed;
    public float CriticalProbability => _criticalProbability;
}