using Fusion;
using UnityEngine;

public class EnemyAttack_Dash : EnemyAttack
{
    [SerializeField] private float _dashSpeed = 15f;
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
            if (_enemyMovement.IsFacingRightNet)
            {
                _rb.linearVelocityX = _dashSpeed;
            }
            else
            {
                _rb.linearVelocityX = -_dashSpeed;
            }
        }
    }
}
