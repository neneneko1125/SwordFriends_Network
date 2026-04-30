using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1種類の攻撃ごとの設計図
/// </summary>
[System.Serializable]
public class AttackData
{
    [Header("攻撃名")]
    public string Name;
    [Header("攻撃力倍率 (1なら1倍、2なら2倍...)")]
    public float _attackPowerMultiplier;
    [Header("攻撃判定の大きさ")]
    public Vector2 HitboxSize;
    [Header("中心からどれだけ離れているか")]
    public Vector2 HitboxOffset;
}

/// <summary>
/// 全キャラクター共通のステータス
/// </summary>
public class BaseCharacterData : ScriptableObject
{
    [Header("名前")]
    [SerializeField] string _characterName;

    [Header("最大HP")]
    [SerializeField] private int _maxHP;

    [Header("無敵時間")]
    [SerializeField] private float _invincibleTime;

    [Header("点滅速度")]
    [SerializeField] private float _blinkingSpeed;

    [Header("基礎攻撃力")]
    [SerializeField] private int _baseAttackPower;

    [Header("1キャラがもつ攻撃パターンたち")]
    [SerializeField] private List<AttackData> _attackPatterns = new List<AttackData>();

    //プロパティ(外部から読み取る用)
    public string CharacterName => _characterName;
    public int MaxHP => _maxHP;
    public float InvincibleTime => _invincibleTime;
    public float BlinkingSpeed => _blinkingSpeed;
    public int BaseAttackPower => _baseAttackPower;
    public List<AttackData> AttackPatterns => _attackPatterns;
}

