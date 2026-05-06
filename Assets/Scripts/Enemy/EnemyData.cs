using UnityEngine;

/// <summary>
/// 敵特有のステータス
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : BaseCharacterData
{
    [field: Header("移動速度")]
    [field: SerializeField] public float PatrolSpeed { get; private set; }
    [field: SerializeField] public float ChaseSpeed { get; private set; }

    [field: Header("移動速度の乱数調整")]
    [field: SerializeField] public float ChaseSpeedMinMultiplier { get; private set; } = 0.75f;
    [field: SerializeField] public float ChaseSpeedMaxMultiplier { get; private set; } = 1.25f;
    [field: SerializeField] public float PatrolSpeedMinMultiplier { get; private set; } = 0.9f;
    [field: SerializeField] public float PatrolSpeedMaxMultiplier { get; private set; } = 1.1f;

    [field: Header("追いかける対象を検知する距離")]
    [field: SerializeField] public float DetectDistance { get; private set; } = 5.0f;

    [field: Header("追いかける対象との最小距離")]
    [field: SerializeField] public float MinDistance { get; private set; } = 0.5f;

    [field: Header("攻撃開始する距離")]
    [field: SerializeField] public float AttackDistance { get; private set; } = 1.0f;

    [field: Header("攻撃開始する前の待機時間")]
    [field: SerializeField] public float AttackSignTime { get; private set; } = 1.0f;

    [field: Header("ドロップアイテム")]
    [field: SerializeField] public GameObject DropItem { get; private set; }
}