using UnityEngine;
using Fusion;
using System.Collections.Generic;


public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private Vector2 _ladderHitBoxSize = new Vector2(0.75f, 5.0f);
    [SerializeField] private LayerMask _ladderLayer;

    [SerializeField] private List<Animator> _animators = new List<Animator>();
    private Animator _anim;

    [Networked] protected float SyncMoveSpeed { get; set; }
    [Networked] protected float SyncJumpForce { get; set; }
    [Networked] protected float SyncLadderSpeed { get; set; }

    [Networked] public bool IsGrounded { get; set; }
    [Networked] public bool IsWalking { get; set; }
    [Networked] public bool IsFacingRight { get; set; } = true;
    [Networked] public bool IsLadderMode { get; set; } = false;

    // プレイヤーの向きを変更するのに使用する
    [SerializeField] private Transform _parentTransform;
    private Vector2 _playerDefaultScale;
    private float _defaultGravityScale;

    private Rigidbody2D _rb;
    private PlayerSetup _playerSetup;
    private PlayerAttackHandler _playerAttack;

    private PlayerInstanceData _instance;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerSetup = GetComponent<PlayerSetup>();
        _playerAttack = GetComponent<PlayerAttackHandler>();
        _playerDefaultScale = _parentTransform.localScale;  // デフォルトのScaleを保存
        _defaultGravityScale = _rb.gravityScale;    // デフォルトの重力を保存
    }

    public void Setup(PlayerInstanceData instance)
    {
        _instance = instance;

        if (HasStateAuthority)
        {
            SyncMoveSpeed = _instance.MoveSpeed;
            SyncJumpForce = _instance.JumpForce;
            SyncLadderSpeed = _instance.LadderSpeed;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 自分以外のキャラクターを動かさないようにする
        if (!HasStateAuthority)
        {
            return;
        }

        IsGrounded = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _groundLayer);    // 地面チェック
        CheckLadder();

        // PlayerInputのMyInputDataを取得
        if (GetInput(out MyInputData inputData))
        {
            Walk(inputData.Horizontal);

            if (inputData.JumpPressed && !IsLadderMode)
            {
                Jump();
            }
            if (inputData.LadderPressed && IsLadderMode)
            {
                Ladder();
            }
            if (inputData.LadderReleased)
            {
                CancelLadder();     // はしごを上ってるとき、はしごボタンを離すとはしごモードOFF
            }
        }

    }

    public override void Render()
    {
        ChangeWalkAnimation();

        // 向きの反映
        if (IsFacingRight)
        {
            _parentTransform.localScale = _playerDefaultScale;
        }
        else
        {
            _parentTransform.localScale = new Vector2(-_playerDefaultScale.x, _playerDefaultScale.y);
        }
    }

    private void Walk(float direction)
    {
        // ダッシュ時のスピードと競合しないように
        if (_playerAttack.IsDashAttacking)
        {
            return;
        }

        _rb.linearVelocityX = direction * SyncMoveSpeed;

        // 固定更新の中で状態を確定させる
        IsWalking = Mathf.Abs(direction) > 0.1f;

        // 進んでいる方向によってプレイヤーの向きを決定
        if (direction > 0.1f)
        {
            IsFacingRight = true;
        }
        else if (direction < -0.1f)
        {
            IsFacingRight = false;
        }
    }

    private void Jump()
    {
        if (IsGrounded)
        {
            _rb.linearVelocityY = SyncJumpForce;
        }
    }

    private void CheckLadder()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, _ladderHitBoxSize, 0, _ladderLayer);    // はしごチェック

        if (hit != null)
        {
            IsLadderMode = true;
        }
        else
        {
            CancelLadder();     // はしごから離れたらはしごモードOFF
        }
    }

    private void Ladder()
    {
        _rb.gravityScale = 0;
        _rb.linearVelocityY = SyncLadderSpeed;
    }

    private void CancelLadder()
    {
        IsLadderMode = false;
        _rb.gravityScale = _defaultGravityScale;
    }

    private void ChangeWalkAnimation()
    {
        int index = _playerSetup.CharacterIndex;

        if (index < _animators.Count && _anim == null)
        {
            _anim = _animators[index];
        }

        if (_anim != null)
        {
            _anim.SetBool("Walk", IsWalking);
        }
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _ladderHitBoxSize);
    }
}


