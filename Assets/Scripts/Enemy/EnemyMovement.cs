using Fusion;
using UnityEngine;


public class EnemyMovement : NetworkBehaviour
{
    private enum State
    {
        Patrol,
        Chase
    }

    [Header("移動速度")]
    [SerializeField] private float _basePatrolSpeed = 3.0f;
    [SerializeField] private float _baseChaseSpeed = 6.0f;

    [Header("移動速度の乱数調整")]
    [SerializeField] private float _chaseSpeedMinMultiplier = 0.75f;
    [SerializeField] private float _chaseSpeedMaxMultiplier = 1.25f;

    [SerializeField] private float _patrolSpeedMinMultiplier = 0.9f;
    [SerializeField] private float _patrolSpeedMaxMultiplier = 1.1f;

    [Header("追いかける対象を検知する距離")]
    [SerializeField] private float _detectDistance = 5.0f;
    [Header("追いかける対象との最小距離")]
    [SerializeField] private float _minDistance = 0.5f;


    [Header("追いかける対象のレイヤー")]
    [SerializeField] private LayerMask _playerLayer;

    [Header("壁や崖を検知する関連")]
    [SerializeField] private float _wallCheckerRayLength = 0.5f;
    [SerializeField] private float _cliffCheckerRayLength = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private Vector2 _defaultScale;
    public Transform _currentTarget;
    private EnemyAttackHandler _enemyAttack;


    // 実際に使用する同期された速度
    [Networked] private float CurrentPatrolSpeed { get; set; }
    [Networked] private float CurrentChaseSpeed { get; set; }

    // ターゲットの NetworkObject 参照
    [Networked] public NetworkObject TargetObject { get; set; }
    [Networked] private State CurrentState { get; set; }
    [Networked] public NetworkBool IsFacingRightNet {  get; set; }


    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyAttack = GetComponent<EnemyAttackHandler>();
        _defaultScale = transform.localScale;

        if (HasStateAuthority)
        {
            CurrentState = State.Patrol;
            IsFacingRightNet = transform.localScale.x > 0;

            // 権限者だけが乱数を振り、[Networked]変数に保存する
            CurrentChaseSpeed = Random.Range(_baseChaseSpeed * _chaseSpeedMinMultiplier, _baseChaseSpeed * _chaseSpeedMaxMultiplier);
            CurrentPatrolSpeed = Random.Range(_basePatrolSpeed * _patrolSpeedMinMultiplier, _basePatrolSpeed * _patrolSpeedMaxMultiplier);
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (_enemyAttack.IsAttackSigning || _enemyAttack.IsAttacking)
        {
            return;
        }

        if (_currentTarget == null)
        {
            SearchTarget();
        }
        
        if (CurrentState == State.Patrol)
        {
            MovePatrol();
        }
        else
        {
            MoveChase();
        }

        ChangeState();
    }

    public override void Render()
    {
        float scaleX = IsFacingRightNet ? Mathf.Abs(_defaultScale.x) : -Mathf.Abs(_defaultScale.x);
        transform.localScale = new Vector2(scaleX, _defaultScale.y);
    }

    /// <summary>
    /// パトロール状態のときの、壁のチェックと方向転換
    /// </summary>
    private void WallAndCliffChecker_ChangeDirection()
    {
        if (CurrentState == State.Chase)
        {
            return;
        }

        bool wallchecked;
        bool cliffchecked;

        if (IsFacingRightNet)
        {
            wallchecked = Physics2D.Raycast(transform.position, Vector2.right, _wallCheckerRayLength, _groundLayer);
            cliffchecked = !Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.down, _cliffCheckerRayLength, _groundLayer);
            if (wallchecked || cliffchecked)
            {
                transform.localScale = new Vector2(-_defaultScale.x, _defaultScale.y);
            }
        }
        else
        {
            wallchecked = Physics2D.Raycast(transform.position, Vector2.left, _wallCheckerRayLength, _groundLayer);
            cliffchecked = !Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.down, _cliffCheckerRayLength, _groundLayer);

            if (wallchecked || cliffchecked)
            {
                transform.localScale = new Vector2(_defaultScale.x, _defaultScale.y);
            }
        }

    }


    /// <summary>
    /// 状態を変更する
    /// </summary>
    private void ChangeState()
    {
        bool isdetecting = Physics2D.OverlapCircle(transform.position, _detectDistance, _playerLayer);

        if (isdetecting)
        {
            CurrentState = State.Chase;
        }
        else
        {
            CurrentState = State.Patrol;
            _currentTarget = null;
        }
    }

    /// <summary>
    /// パトロールモードのときの動き
    /// </summary>
    private void MovePatrol()
    {
        WallAndCliffChecker_ChangeDirection();

        if (transform.localScale.x > 0)
        {
            IsFacingRightNet = true;
        }
        else
        {
            IsFacingRightNet = false;
        }

        if (IsFacingRightNet)
        {
            _rb.linearVelocityX = _basePatrolSpeed;
        }
        else
        {
            _rb.linearVelocityX = -_basePatrolSpeed;
        }
    }
    /// <summary>
    /// 追跡モードのときの動き
    /// </summary>
    private void MoveChase()
    {
        if (TargetObject == null)
        {
            return;
        }

        Vector3 targetPos = TargetObject.transform.position;
        float diff = targetPos.x - transform.position.x;
        float direction = Mathf.Sign(diff);

        //どちらを向いているか確認＋向きの変更
        if (diff > 0)
        {
            IsFacingRightNet = true;
            transform.localScale = _defaultScale;
        }
        else if(diff < 0)
        {
            IsFacingRightNet = false;
            transform.localScale = new Vector2(-_defaultScale.x, _defaultScale.y);
        }

        if(Mathf.Abs(diff) < _minDistance)
        {
            return;
        }

        _rb.linearVelocityX = direction * CurrentChaseSpeed;
    }

    /// <summary>
    /// ターゲット探し
    /// </summary>
    private void SearchTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectDistance, _playerLayer);

        float minDistance = float.MaxValue;
        Transform target = null;
        foreach (var h in hits)
        {
            float distance = Vector2.Distance(transform.position, h.transform.position);

            if(minDistance > distance)
            {
                target = h.transform;

                // 権限者が NetworkObject を保存する
                TargetObject = h.GetComponent<NetworkObject>();
            }            
        }
        _currentTarget = target;
    }

    private void OnDrawGizmos()
    {
        // 実行中でなくても計算できるように向きを判定
        bool right = transform.localScale.x > 0;
        Vector2 rayDir = right ? Vector2.right : Vector2.left;
        Vector2 cliffPos = right ? new Vector2(transform.position.x + 0.5f, transform.position.y)
                                : new Vector2(transform.position.x - 0.5f, transform.position.y);

        // 壁チェックの描画
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rayDir * _wallCheckerRayLength);

        // 崖チェックの描画
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(cliffPos, cliffPos + Vector2.down * _cliffCheckerRayLength);
    }
}

