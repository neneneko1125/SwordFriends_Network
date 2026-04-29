using UnityEngine;
using Fusion;


public class PlayerMovement : NetworkBehaviour
{
    // 横方向の移動速度
    [SerializeField] private float _moveSpeed = 5f;

    // ジャンプ関連
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private float _ladderSpeed = 5f;
    [SerializeField] private Vector2 _hitBoxSize = new Vector2(0.75f, 5.0f);
    [SerializeField] private LayerMask _ladderLayer;

    [SerializeField] private Animator _animator1;
    [SerializeField] private Animator _animator2;
    [SerializeField] private Animator _animator3;
    [SerializeField] private Animator _animator4;

    [Networked] public bool IsGrounded { get; set; }
    [Networked] public bool IsWalking { get; set; }

    // 右(正の方向)を向いていればtrue
    [Networked] public bool IsFacingRightNet { get; set; } = true;
    [Networked] public bool IsLadderMode { get; set; } = false;

    // プレイヤーの向きを変更するのに使用する
    [SerializeField] private Transform _parentTransform;
    private Vector2 _playerDefaultScale;
    private float _defaultGravityScale;

    private Rigidbody2D _rb;
    private PlayerAttackHandler _playerAttack;

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAttack = GetComponent<PlayerAttackHandler>();
        _playerDefaultScale = _parentTransform.localScale;  // デフォルトのScaleを保存
        _defaultGravityScale = _rb.gravityScale;    // デフォルトの重力を保存
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
        if (IsFacingRightNet)
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

        _rb.linearVelocityX = direction * _moveSpeed;

        // 固定更新の中で状態を確定させる
        IsWalking = Mathf.Abs(direction) > 0.1f;

        // 進んでいる方向によってプレイヤーの向きを決定
        if (direction > 0.1f)
        {
            IsFacingRightNet = true;
        }
        else if (direction < -0.1f)
        {
            IsFacingRightNet = false;
        }
    }

    private void Jump()
    {
        if (IsGrounded)
        {
            _rb.linearVelocityY = _jumpForce;
        }
    }

    private void CheckLadder()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, _hitBoxSize, 0, _ladderLayer);    // はしごチェック

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
        _rb.linearVelocityY = _ladderSpeed;
    }

    private void CancelLadder()
    {
        IsLadderMode = false;
        _rb.gravityScale = _defaultGravityScale;
    }

    private void ChangeWalkAnimation()
    {
        // どのAnimatorがアクティブ状態かをみる
        if (_animator1 != null && _animator1.gameObject.activeInHierarchy)
        {
            _animator1.SetBool("Walk", IsWalking);
        }
        else if (_animator2 != null && _animator2.gameObject.activeInHierarchy)
        {
            _animator2.SetBool("Walk", IsWalking);
        }
        else if (_animator3 != null && _animator3.gameObject.activeInHierarchy)
        {
            _animator3.SetBool("Walk", IsWalking);
        }
        else if (_animator4 != null && _animator4.gameObject.activeInHierarchy)
        {
            _animator4.SetBool("Walk", IsWalking);
        }
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _hitBoxSize);
    }
}


