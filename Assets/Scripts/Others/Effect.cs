using UnityEngine;
using Fusion;

public class Effect : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }


    //[Networked] private TickTimer LifeTimer { get; set; }
    //public override void Spawned()
    //{
    //    if (HasStateAuthority)
    //    {
    //        //タイマー開始
    //        LifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
    //    }
    //}

    //public override void FixedUpdateNetwork()
    //{
    //    if (!HasStateAuthority)
    //    {
    //        return;
    //    }

    //    if (LifeTimer.Expired(Runner))
    //    {
    //        Runner.Despawn(Object);
    //    }
    //}
}
