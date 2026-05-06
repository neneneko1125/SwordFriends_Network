using Fusion;
using UnityEngine;

public class EnemyAttackHandler_Dash : EnemyAttackHandler
{
    private Rigidbody2D _rb;

    public override void Spawned()
    {
        base.Spawned();
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (IsAttacking)
        {
            if (_enemyMovement.IsFacingRight)
            {
                _rb.linearVelocityX = _attackData.DashSpeed;
            }
            else
            {
                _rb.linearVelocityX = -_attackData.DashSpeed;
            }
        }
    }
}
