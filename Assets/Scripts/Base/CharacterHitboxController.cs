using UnityEngine;
using Fusion;
using System.Collections.Generic;


public class CharacterHitboxController : NetworkBehaviour
{
    [Header("Gizmosでのデバッグ用")]
    [SerializeField] private BaseCharacterData _baseData;

    [SerializeField] private LayerMask _targetLayer;

    // 他のクラスで既に同期されたデータが渡されるからNetworkedは不要
    private AttackData _currentAttackData;
    private bool _isFacingRight = true;
    private int _baseAttackPower;

    private ContactFilter2D _attackFilter;

    [Networked] public bool IsAttacking { get; set; }

    //private List<Collider2D> _hitResults = new List<Collider2D>();
    private List<LagCompensatedHit> _hitResults = new List<LagCompensatedHit>();    // 当たった対象を保存するリスト
    private HashSet<Collider2D> _colliderHashSet = new HashSet<Collider2D>();   //一度の攻撃で同じ対象に当たらないようにするためのHashSet

    public override void Spawned()
    {
        // OverlapBoxに必要
        _attackFilter = new ContactFilter2D();
        _attackFilter.useLayerMask = true;
        _attackFilter.layerMask = _targetLayer;
        _attackFilter.useTriggers = true;
    }

    // Handlerからデータをもらう
    public void SetupAttack(AttackData data, bool isFacingRIght, int basePower)
    {
        _currentAttackData = data;
        _isFacingRight = isFacingRIght;
        _baseAttackPower = basePower;
        IsAttacking = true;
    }
    

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (!IsAttacking)
        {
            _colliderHashSet.Clear();
            return;
        }
        if (_currentAttackData == null)
        {
            return;
        }

        ExcuteHitDetection();
    }

    private void ExcuteHitDetection()
    {
        float dir = _isFacingRight? 1 : -1;

        // 自身の座標に、ずらしの分と今向いている方向を反映
        Vector2 spawnPos = (Vector2)transform.position + new Vector2(_currentAttackData.HitboxOffset.x * dir, _currentAttackData.HitboxOffset.y);

        // FhotonならPhysics2DじゃなくてRunner.GetPhysicsScene2D()を使う
        //int hitCount = Runner.GetPhysicsScene2D().OverlapBox(spawnPos, _currentAttackData.HitboxSize, 0, _attackFilter, _hitResults);

        int hitCount = Runner.LagCompensation.OverlapBox(
            spawnPos,
            _currentAttackData.HitboxSize,
            Quaternion.identity,
            Object.InputAuthority,       // 誰の入力タイミングに巻き戻すか
            _hitResults,
            _targetLayer,
            HitOptions.IncludePhysX      // Unityの通常のColliderも対象に含める
        );

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hitResults[i];
            var col = hit.GameObject.GetComponent<Collider2D>();

            if (col != null && !_colliderHashSet.Contains(col))
            {
                _colliderHashSet.Add(col);

                int finalDamage = Mathf.RoundToInt(_baseAttackPower * _currentAttackData.AttackPowerMultiplier);
                if (col.TryGetComponent<BaseHP>(out var hp))
                {
                    hp.Rpc_TakeDamage(finalDamage);
                }
            }
        }
    }

    //________________________________________________________________________________________________________________________________________________//

    private void OnDrawGizmos()
    {
        // テストプレイ中（再生中）は何もしない
        if (Application.isPlaying) return;

        // データの確認
        if (_baseData == null || _baseData.AttackPatterns == null || _baseData.AttackPatterns.Count == 0)
        {
            return;
        }

        // エディタ上での確認用として、リストの最初（通常攻撃など）を表示する
        AttackData dataToDraw = _baseData.AttackPatterns[0];

        if (dataToDraw != null)
        {
            // インスペクター上のlocalScaleから向きを判定
            float dir = transform.localScale.x > 0 ? 1f : -1f;

            // 描画位置の計算
            Vector3 pos = transform.position + new Vector3(dataToDraw.HitboxOffset.x * dir, dataToDraw.HitboxOffset.y, 0);

            // ギズモの色設定（エディタ時は見やすいように黄色など）
            Gizmos.color = Color.yellow;

            // 箱の枠線を描画
            Gizmos.DrawWireCube(pos, dataToDraw.HitboxSize);

            // 中心点に小さな印
            Gizmos.DrawSphere(pos, 0.05f);
        }
    }
}
