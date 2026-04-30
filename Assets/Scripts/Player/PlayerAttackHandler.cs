using DG.Tweening.Core.Easing;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHandler : NetworkBehaviour
{
    [SerializeField] private float _dashMaxSpeed = 10f;
    [SerializeField] private float _dashMinSpeed = 1f;
    [SerializeField] private float _downSpeed = 15f;
    [SerializeField] private float _upSpeed = 15f;

    [SerializeField] private Animator _animator1;
    [SerializeField] private Animator _animator2;
    [SerializeField] private Animator _animator3;
    [SerializeField] private Animator _animator4;

    [SerializeField] private string _attackAnimationName = "Attack";
    [SerializeField] private string _dashAttackAnimationName = "DashAttack";
    [SerializeField] private string _downAttackAnimationName = "DownAttack";
    [SerializeField] private string _upAttackAnimationName = "UpAttack";

    [SerializeField] private float _attackIntervalTime = 0.25f;
    [SerializeField] private float _dashAttackIntervalTime = 0.5f;
    [SerializeField] private float _downAttackIntervalTime = 0.5f;
    [SerializeField] private float _upAttackIntervalTime = 0.5f;

    [SerializeField] private float _hitboxActiveDuration = 0.1f;
    [SerializeField] private float _dashHitboxActiveDuration = 0.5f;
    [SerializeField] private float _downHitboxActiveDuration = 0.5f;
    [SerializeField] private float _upHitboxActiveDuration = 0.2f;

    [Networked] private TickTimer DashTimer {  get; set; }
    [Networked] private Vector2 DashDirection { get; set; }

    //攻撃した回数を同期する boolでやると、ネットワークで見逃される可能性が高い
    [Networked] private int AttackCount {  get; set; }

    // 判定をOFFにするタイミングを管理するタイマー
    [Networked] private TickTimer HitboxDisableTimer { get; set; }

    [Networked] public bool IsNormalAttacking {  get; set; }
    [Networked] public bool IsDashAttacking {  get; set; }
    [Networked] public bool IsDownAttacking {  get; set; }
    [Networked] public bool IsUpAttacking {  get; set; }

    [Networked] private string AnimationName { get; set; }

    //前回の攻撃回数
    private int _lastAttackCount;

    //連打防止
    private float _nextAttackTime;

    private Rigidbody2D _rb;
    private PlayerMovement _playerMovement;
    private PlayerHitboxController _playerAttackObject;


    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAttackObject = GetComponent<PlayerHitboxController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        //攻撃判定を消す
        if (HitboxDisableTimer.Expired(Runner))
        {
            IsNormalAttacking = false;
            IsDashAttacking = false;
            IsDownAttacking = false;
            IsUpAttacking = false;
            _playerAttackObject.IsAttacking = false;
            HitboxDisableTimer = TickTimer.None;
        }

        DoDashAttack();

        // 入力があった かつインターバルが終了していれば
        if (GetInput(out MyInputData inputData) && Runner.SimulationTime >= _nextAttackTime)
        {
            //攻撃ボタンなら
            if (inputData.AttackPressed)
            {
                AttackSetup(_attackIntervalTime, _attackAnimationName);
                DoAttack();
            }
            else if (inputData.RightDashAttackPressed && _playerMovement.IsFacingRight)
            {
                AttackSetup(_dashAttackIntervalTime, _dashAttackAnimationName);
                PreparationDashAttack(Vector2.right);

            }
            else if (inputData.LeftDashAttackPressed && !_playerMovement.IsFacingRight)
            {
                AttackSetup(_dashAttackIntervalTime, _dashAttackAnimationName);
                PreparationDashAttack(Vector2.left);

            }
            else if (inputData.DownAttackPressed)
            {
                AttackSetup(_downAttackIntervalTime, _downAttackAnimationName);
                DoDownAttack();
            }
            else if (inputData.UpAttackPressed && _playerMovement.IsGrounded)
            {
                AttackSetup(_upAttackIntervalTime, _upAttackAnimationName);
                DoUpAttack();
            }
        }
    }

    private void AttackSetup(float intervalTime, string animationName)
    {
        _playerAttackObject.IsAttacking = true;
        AttackCount++;
        _nextAttackTime = Runner.SimulationTime + intervalTime;
        AnimationName = animationName;
    }

    public override void Render()
    {
        if(AttackCount > _lastAttackCount)
        {
            _lastAttackCount = AttackCount;
            PlayAttackAnimation();
        }
    }

    private void PlayAttackAnimation()
    {
        // アクティブな方のアニメーターにトリガーを送る
        if (_animator1 != null && _animator1.gameObject.activeInHierarchy)
        {
            _animator1.SetTrigger(AnimationName);
        }
        else if (_animator2 != null && _animator2.gameObject.activeInHierarchy)
        {
            _animator2.SetTrigger(AnimationName);
        }
        else if (_animator3 != null && _animator3.gameObject.activeInHierarchy)
        {
            _animator3.SetTrigger(AnimationName);
        }
        else if (_animator4 != null && _animator4.gameObject.activeInHierarchy)
        {
            _animator4.SetTrigger(AnimationName);
        }
    }


    private void DoAttack()
    {
        IsNormalAttacking = true;

        int rnd = Random.Range(0, 2);
        if(rnd == 0)
        {
            SEManager.Instance.SEAttack();
        }
        else
        {
            SEManager.Instance.SEAttack2();
        }

        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, _hitboxActiveDuration);
    }
    private void PreparationDashAttack(Vector2 direction)
    {
        IsDashAttacking = true;

        SEManager.Instance.SEDashAttack();

        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, _dashHitboxActiveDuration);
        //ダッシュの終わりを検知するために、もう一つ同じタイマーを用意する必要がある
        DashTimer = TickTimer.CreateFromSeconds(Runner, _dashHitboxActiveDuration);

        DashDirection = direction;
    }
    private void DoDashAttack()
    {
        if (!DashTimer.ExpiredOrNotRunning(Runner))
        {
            float remaining = DashTimer.RemainingTime(Runner) ?? 0;     //タイマーがnullなら0を使用
            float progress = 1.0f - remaining / _dashHitboxActiveDuration;

            float currentSpeed = Mathf.Lerp(_dashMaxSpeed, _dashMinSpeed, progress);

            _rb.linearVelocity = DashDirection * currentSpeed;
        }
        else if (IsDashAttacking)
        {
            IsDashAttacking = false;
            DashTimer = TickTimer.None;
            _rb.linearVelocity = Vector2.zero;
        }
    }
    private void DoDownAttack()
    {
        IsDownAttacking = true;

        SEManager.Instance.SEDownAttack();

        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, _downHitboxActiveDuration);

        _rb.AddForce(Vector2.down * _downSpeed, ForceMode2D.Impulse);
    }
    private void DoUpAttack()
    {
        IsUpAttacking = true;

        int rnd = Random.Range(0, 2);
        if (rnd == 0)
        {
            SEManager.Instance.SEAttack();
        }
        else
        {
            SEManager.Instance.SEAttack2();
        }

        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, _upHitboxActiveDuration);

        _rb.linearVelocityY = _upSpeed;
    }

}