using Fusion;
using UnityEngine;

public class EnemyAttackHandler_Dash : EnemyAttackHandler
{
    private Rigidbody2D _rb;
    private AttackData data;

    public override void Spawned()
    {
        base.Spawned();
        _rb = GetComponent<Rigidbody2D>();
        data = _instance.BaseData.AttackPatterns[0];
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (IsAttacking)
        {
            if (_enemyMovement.IsFacingRight)
            {
                _rb.linearVelocityX = data.DashSpeed;
            }
            else
            {
                _rb.linearVelocityX = -data.DashSpeed;
            }
        }
    }
}
