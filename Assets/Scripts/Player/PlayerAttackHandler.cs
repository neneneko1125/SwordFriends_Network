using DG.Tweening.Core.Easing;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAttackHandler : NetworkBehaviour
{
    [SerializeField] private List<Animator> _animators = new List<Animator>();
    private Animator _anim;

    [Networked] private int CurrentAttackIndex {  get; set; }

    [Networked] private TickTimer AttackTimer {  get; set; }

    //攻撃した回数を同期する boolでやると、ネットワークで見逃される可能性が高い
    [Networked] private int AttackCount {  get; set; }

    // 判定をOFFにするタイミングを管理するタイマー
    [Networked] private TickTimer HitboxDisableTimer { get; set; }

    [Networked] public bool IsNormalAttacking {  get; set; }
    [Networked] public bool IsDashAttacking {  get; set; }
    [Networked] public bool IsDownAttacking {  get; set; }
    [Networked] public bool IsUpAttacking {  get; set; }

    private enum AttackType { Normal, Dash, Down, Up }
    [Networked] private AttackType CurrentType {  get; set; }

    //前回の攻撃回数
    private int _lastAttackCount;

    //連打防止
    private float _nextAttackTime;

    private Rigidbody2D _rb;
    private PlayerSetup _playerSetup;
    private PlayerMovement _playerMovement;
    private CharacterHitboxController _hitboxController;

    private PlayerInstanceData _instance;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerSetup = GetComponent<PlayerSetup>();
        _playerMovement = GetComponent<PlayerMovement>();
        _hitboxController = GetComponent<CharacterHitboxController>();
    }

    public void Setup(PlayerInstanceData instance)
    {
        _instance = instance;
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
            _hitboxController.IsAttacking = false;
            HitboxDisableTimer = TickTimer.None;
        }

        // 入力があった かつインターバルが終了していれば
        if (GetInput(out MyInputData inputData) && Runner.SimulationTime >= _nextAttackTime)
        {
            if (inputData.AttackPressed)
            {
                ExcuteAttack(0, AttackType.Normal);
                IsNormalAttacking = true;
                SEManager.Instance.SEAttack();
            }
            else if (inputData.RightDashAttackPressed || inputData.LeftDashAttackPressed)
            {
                ExcuteAttack(1, AttackType.Dash);
                IsDashAttacking = true;
                SEManager.Instance.SEDashAttack();
            }
            else if (inputData.DownAttackPressed)
            {
                ExcuteAttack(2, AttackType.Down);
                IsDownAttacking = true;
                SEManager.Instance.SEDownAttack();
            }
            else if (inputData.UpAttackPressed && _playerMovement.IsGrounded)
            {
                ExcuteAttack(3, AttackType.Up);
                IsUpAttacking = true;
                SEManager.Instance.SEAttack2();
            }
        }
        ApplyAttackMotion();
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
        int index = _playerSetup.CharacterIndex;

        if (index < _animators.Count && _anim == null)
        {
            _anim = _animators[index];
        }

        if (_anim != null)
        {
            string trigger = _instance.PlayerData.AttackPatterns[CurrentAttackIndex].AnimationTriggerName;
            _anim.SetTrigger(trigger);
        }
    }


    private void ExcuteAttack(int index, AttackType type)
    {
        AttackData data = _instance.BaseData.AttackPatterns[index];
        _hitboxController.SetupAttack(data, _playerMovement.IsFacingRight, _instance.BaseAttackPower);

        CurrentAttackIndex = index;
        CurrentType = type;
        AttackCount++;

        _nextAttackTime = Runner.SimulationTime + data.IntervalTime;

        // 攻撃時間分だけタイマー起動
        HitboxDisableTimer = TickTimer.CreateFromSeconds(Runner, data.HitboxDuration);
        AttackTimer = TickTimer.CreateFromSeconds(Runner, data.AttackDuration);

        // 現在の攻撃タイプをみてフラグ管理
        IsNormalAttacking = (type == AttackType.Normal);
        IsDashAttacking = (type == AttackType.Dash);
        IsDownAttacking = (type == AttackType.Down);
        IsUpAttacking = (type == AttackType.Up);

        PlayAttackSE(type);
    }

    private void ApplyAttackMotion()
    {
        if (AttackTimer.ExpiredOrNotRunning(Runner))
        {
            return;
        }

        AttackData data = _instance.BaseData.AttackPatterns[CurrentAttackIndex];

        Vector2 dir = _playerMovement.IsFacingRight ? Vector2.right : Vector2.left;

        switch (CurrentType)
        {
            case AttackType.Dash:
                float currentSpeed = AttackTimer.RemainingTime(Runner).Value / data.AttackDuration;
                _rb.linearVelocity = dir * data.DashSpeed * currentSpeed;
                break;
            case AttackType.Down:
                _rb.linearVelocity = Vector2.down * data.DownSpeed;
                break;
            case AttackType.Up:
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, data.UpSpeed);
                break;
        }
    }

    private void PlayAttackSE(AttackType type)
    {
        switch (type)
        {
            case AttackType.Normal:
                SEManager.Instance.SEAttack();
                break;
            case AttackType.Dash:
                SEManager.Instance.SEDashAttack();
                break;
            case AttackType.Down:
                SEManager.Instance.SEDownAttack();
                break;
            case AttackType.Up:
                SEManager.Instance.SEAttack2();
                break;
        }
    }
}