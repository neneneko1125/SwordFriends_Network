using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class EnemyAttackHandler : NetworkBehaviour
{
    [SerializeField] protected Animator _animator;
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
    protected CharacterHitboxController _hitboxController;

    protected EnemyInstanceData _instance;
    protected AttackData _attackData;

    public override void Spawned()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _hitboxController = GetComponent<CharacterHitboxController>();
    }

    public void Setup(EnemyInstanceData instance)
    {
        _instance = instance;
        _attackData = _instance.BaseData.AttackPatterns[0];
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
            _hitboxController.IsAttacking = false;
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
            _hitboxController.IsAttacking = false;
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
        if (!IsAttackSigning && Runner.SimulationTime >= NextAttackStartTime && distance < _instance.EnemyData.AttackDistance)
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
        AttackSignTimer = TickTimer.CreateFromSeconds(Runner, _instance.EnemyData.AttackSignTime);
    }

    private void CheckAttackSignTimeout()
    {
        if (IsAttackSigning && AttackSignTimer.Expired(Runner))
        {
            AttackSignTimer = TickTimer.None;
            IsAttackSigning = false;
            ExecuteAttack();
        }
    }

    protected virtual void ExecuteAttack()
    {
        AttackData data = _instance.BaseData.AttackPatterns[0];     // 基本的に敵の攻撃は1種類　そうじゃないやつはこのクラスを継承してメソッドを追加なりする
        _hitboxController.SetupAttack(data, _enemyMovement.IsFacingRight, _instance.BaseAttackPower);

        IsAttacking = true;
        _hitboxController.IsAttacking = true;
        AttackCount++;
        NextAttackStartTime = Runner.SimulationTime + data.IntervalTime;

        //ここで攻撃ON 攻撃OFFタイマーも起動
        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, data.AttackDuration);
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
