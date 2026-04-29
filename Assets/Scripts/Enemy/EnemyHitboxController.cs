using UnityEngine;

public class EnemyHitboxController : BaseHitboxController
{
    private EnemyMovement _enemyMovement;
    private EnemyAttackHandler _enemyAttack;

    public override void Spawned()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyAttack = GetComponent<EnemyAttackHandler>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (_enemyAttack != null)
        {
            _currentAttackData = _attackProfile.AttackPatterns[0];
        }

        base.FixedUpdateNetwork();
    }

    protected override Vector2 CalculateHitboxPos()
    {
        float direction = (_enemyMovement != null && !_enemyMovement.IsFacingRightNet) ? -1 : 1;
        return (Vector2)transform.position + new Vector2(_currentAttackData.HitboxOffset.x * direction, _currentAttackData.HitboxOffset.y);
    }




    //==========================================================================================================================================//



    private void OnDrawGizmos()
    {
        // プロフィールがセットされていないなら何もしない（エラー防止）
        if (_attackProfile == null || _attackProfile.AttackPatterns.Count == 0) return;

        // --- ここからエディタ・再生時共通のロジック ---

        AttackData dataToDraw = null;

        if (!Application.isPlaying)
        {
            // 【非再生時】インスペクターで数値をいじっている時は、
            // とりあえず「0番目（通常攻撃）」を黄色で表示して確認できるようにする
            dataToDraw = _attackProfile.AttackPatterns[0];
            Gizmos.color = Color.yellow;
        }
        else
        {
            // 【再生中】現在の攻撃データがあれば、攻撃中かどうかに合わせて表示
            if (_currentAttackData == null) return;
            dataToDraw = _currentAttackData;
            Gizmos.color = IsAttacking ? Color.red : new Color(1, 0, 0, 0.2f);
        }

        // 共通の計算式で描画
        if (dataToDraw != null)
        {
            // エディタ時は _playerMovement が null なので、右向き（1）固定で表示
            float dir = (Application.isPlaying && _enemyMovement != null && !_enemyMovement.IsFacingRightNet) ? -1f : 1f;

            Vector3 pos = transform.position + new Vector3(dataToDraw.HitboxOffset.x * dir, dataToDraw.HitboxOffset.y, 0);
            Gizmos.DrawWireCube(pos, dataToDraw.HitboxSize);
        }
    }
}