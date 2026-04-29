using Fusion;
using UnityEngine;
using System.Collections;

public class EnemyAttackHandler : NetworkBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected float _attackIntervalTime = 0.25f;
    [SerializeField] protected float _attackDistance = 1.0f;
    [SerializeField] protected float _hitboxActiveDuration = 0.2f;

    [SerializeField] protected float _attackSignTime = 1.0f;
    [SerializeField] protected GameObject _attackSign;

    //攻撃した回数を同期する  boolでやるとネットワークで見逃される可能性が高い
    //この値が変化したら、OnAttackCountChangedメソッドが呼び出される(Fusionの機能)
    [Networked]
    [OnChangedRender(nameof(OnAttackCountChanged))] protected int AttackCount { get; set; }
    //当たり判定を消すタイミングを管理するタイマー(Fusionの機能)
    [Networked] protected TickTimer HitboxDisableTimer { get; set; }
    [Networked] protected TickTimer AttackSignTimer { get; set; }
    //アタックサイン中ならtrue
    [Networked] public bool IsAttackSigning { get; set; }
    //攻撃中はtrue これはEnemyMovementで使う
    [Networked] public bool IsAttacking { get; set; }

    [Networked] protected float NextAttackStartTime { get; set; }

    protected EnemyMovement _enemyMovement;
    protected EnemyHitboxController _enemyAttackObject;

    public override void Spawned()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyAttackObject = GetComponent<EnemyHitboxController>();
    }

    public override void Render()
    {
        _attackSign.SetActive(IsAttackSigning);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        //時間切れで攻撃OFF
        if (HitboxDisableTimer.Expired(Runner))
        {
            IsAttacking = false;
            _enemyAttackObject.IsAttacking = false;
            HitboxDisableTimer = TickTimer.None;    //念のため
        } 

        if (_enemyMovement.TargetObject == null)
        {
            // 予備動作中なら中断する
            if (IsAttackSigning)
            {
                IsAttackSigning = false;
                AttackSignTimer = TickTimer.None;
            }

            // 攻撃フラグも念のためリセット
            IsAttacking = false;
            _enemyAttackObject.IsAttacking = false;
            return;
        }

        if (CanStartAttack())
        {
            AttackSign();
        }

        CheckAttackSignTimeout();
    }
    private bool CanStartAttack()
    {
        float distance = Mathf.Abs(_enemyMovement.TargetObject.transform.position.x - transform.position.x);

        //攻撃サイン中じゃない かつ インターバルが終了している かつ ターゲットとの距離が一定より小さいならば
        if (!IsAttackSigning && Runner.SimulationTime >= NextAttackStartTime && distance < _attackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void AttackSign()
    {
        IsAttackSigning = true;
        AttackSignTimer = TickTimer.CreateFromSeconds(Runner, _attackSignTime);
    }

    private void CheckAttackSignTimeout()
    {
        if (IsAttackSigning && AttackSignTimer.Expired(Runner))
        {
            AttackSignTimer = TickTimer.None;
            IsAttackSigning = false;
            DoAttack();
        }
    }

    protected virtual void DoAttack()
    {
        IsAttacking = true;
        _enemyAttackObject.IsAttacking = true;
        AttackCount++;
        NextAttackStartTime = Runner.SimulationTime + _attackIntervalTime;

        //ここで攻撃ON 攻撃OFFタイマーも起動
        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, _hitboxActiveDuration);
    }

    protected void OnAttackCountChanged()
    {
        PlayAttackAnimation();
    }

    protected void PlayAttackAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
    }
}
