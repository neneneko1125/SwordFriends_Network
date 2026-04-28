using UnityEngine;
using Fusion;

public class LadderBox : NetworkBehaviour
{
    [SerializeField] private Vector2 _hitBoxSize = new Vector2 (0.75f, 5.0f);
    [SerializeField] private LayerMask _playerLayer;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, _hitBoxSize, 0, _playerLayer);

        foreach (Collider2D h in hits)
        {
            if (h != null)
            {
                PlayerMovement playerMovement = h.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.IsLadderMode = true;
                }
            }
        }
    }

  
}
