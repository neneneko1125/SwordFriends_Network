using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody2D _rb;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        // Ž©•ª‚ÌƒLƒƒƒ‰‚¾‚¯“ü—Í‚ðŽó‚¯•t‚¯‚é
        if (!HasStateAuthority)
        {
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(x, y).normalized;

        _rb.linearVelocity = moveDirection * _moveSpeed;
    }
}