
using UnityEngine;

/// <summary>
/// 敵特有のステータス
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : BaseCharacterData
{
    [Header("巡回速度")]
    [SerializeField] private float _patrolSpeed;

    [Header("追跡速度")]
    [SerializeField] private float _chaseSpeed;

    [Header("ドロップアイテム")]
    [SerializeField] private GameObject _dropItem;

    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public GameObject DropItem => _dropItem;
}