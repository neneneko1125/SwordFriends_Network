using UnityEngine;

/// <summary>
/// プレイヤー特有のステータス
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : BaseCharacterData
{
    [field: Header("基本アクション")]
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float LadderSpeed { get; private set; }

    [field: Header("クリティカルヒット率")]
    [field: SerializeField] public float CriticalProbability { get; private set; }
}