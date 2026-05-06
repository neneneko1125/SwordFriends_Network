using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1種類の攻撃ごとの設計図
/// </summary>
[System.Serializable]
public class AttackData
{
    [field: Header("攻撃設定")]
    [field: SerializeField] public string Name { get; private set; } = "Normal";
    [field: SerializeField] public string AnimationTriggerName { get; private set; } = "Attack";
    [field: SerializeField] public float IntervalTime { get; private set; } = 2f;
    [field: SerializeField] public float AttackDuration { get; private set; } = 0.25f;

    [field: Header("威力・判定")]
    [field: SerializeField] public float AttackPowerMultiplier { get; private set; } = 1.0f;
    [field: SerializeField] public float HitboxDuration { get; private set; } = 0.2f;
    [field: SerializeField] public Vector2 HitboxSize { get; private set; }
    [field: SerializeField] public Vector2 HitboxOffset { get; private set; }

    [field: Header("物理挙動")]
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float MinDashSpeed { get; private set; }
    [Tooltip("1フレームごとのダッシュスピード減少率")]
    [field: SerializeField] public float BrakeForce { get; private set; }
    [field: SerializeField] public float UpSpeed { get; private set; }
    [field: SerializeField] public float DownSpeed { get; private set; }

}

/// <summary>
/// 全キャラクター共通のステータス
/// </summary>
public class BaseCharacterData : ScriptableObject
{
    [field: Header("基本情報")]
    [field: SerializeField] public string CharacterName { get; private set; } = "Default";

    [field: Header("ステータス")]
    [field: SerializeField] public int MaxHP { get; private set; } = 10;
    [field: SerializeField] public float InvincibleTime { get; private set; } = 0.15f;
    [field: SerializeField] public float BlinkingSpeed { get; private set; } = 0.05f;
    [field: SerializeField] public int BaseAttackPower { get; private set; } = 1;

    [field: Header("攻撃パターン")]
    [field: SerializeField] public List<AttackData> AttackPatterns { get; private set; } = new List<AttackData>();
}