using UnityEngine;
using Fusion;
using System.Collections.Generic;


public class BaseHitboxController : NetworkBehaviour
{
    [SerializeField] protected LayerMask _targetLayer;

    [SerializeField] protected CharacterAttackProfile _attackProfile;
    protected AttackData _currentAttackData;

    [Networked] public bool IsAttacking { get; set; }

    private List<Collider2D> _hitResults = new List<Collider2D>();
    //ˆê“x‚ÌUŒ‚‚Å“¯‚¶‘ÎÛ‚É“–‚½‚ç‚È‚¢‚æ‚¤‚É‚·‚é‚½‚ß‚ÌHashSet
    private HashSet<Collider2D> _colliderHashSet = new HashSet<Collider2D>();
   

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        if (!IsAttacking)
        {
            if (_colliderHashSet != null)
            {
                _colliderHashSet.Clear();
            }
            return;
        }

        CreateHitbox();
    }

    protected virtual void CreateHitbox() 
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = _targetLayer;

        Vector2 spawnPos = CalculateHitboxPos();
        int hitCount = Physics2D.OverlapBox(spawnPos, _currentAttackData.HitboxSize, 0, filter ,_hitResults);

        if(hitCount > 0)
        {
            UpdateHitDetection(_currentAttackData.Damage);
        }
    }

    /// <summary>
    /// UŒ‚”»’è‚Ì”ÍˆÍ‚ğŒvZ‚·‚é
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 CalculateHitboxPos() {  return Vector2.zero; }

    protected virtual void UpdateHitDetection(int damage) 
    {
        foreach(var h in _hitResults)
        {
            //‚Ü‚¾‚±‚ÌUŒ‚‚É“–‚½‚Á‚Ä‚È‚¯‚ê‚Î
            if (!_colliderHashSet.Contains(h))
            {
                //Šù‚É“–‚½‚Á‚½‘ÎÛ‚Æ‚µ‚Ä“o˜^
                _colliderHashSet.Add(h);

                //ƒ_ƒ[ƒWˆ—
                var target = h.GetComponent<BaseHP>();
                if (target != null)
                {
                    target.Rpc_TakeDamage(damage);
                }
            }
        }
        _hitResults.Clear();
    }

}
