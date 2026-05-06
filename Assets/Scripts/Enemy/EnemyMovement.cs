using Fusion;
using UnityEngine;


public class EnemyMovement : NetworkBehaviour
{
    private enum State
    {
        Patrol,
        Chase
    }

    [Header("追いかける対象のレイヤー")]
    [SerializeField] private LayerMask _playerLayer;

    [Header("壁や崖を検知する関連")]
    [SerializeField] private float _wallCheckerRayLength = 0.5f;
    [SerializeField] private float _cliffCheckerRayLength = 0.5f;
    [SerializeField] private float _cliffCheckerRayOffset = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private Vector2 _defaultScale;
    public Transform _currentTarget;
    private EnemyAttackHandler _enemyAttack;


    // 実際に使用する同期された速度
    [Networked] private float SyncCurrentPatrolSpeed { get; set; }
    [Networked] private float SyncCurrentChaseSpeed { get; set; }

    // ターゲットの NetworkObject 参照
    [Networked] public NetworkObject TargetObject { get; set; }
    [Networked] private State CurrentState { get; set; }
    [Networked] public NetworkBool IsFacingRight {  get; set; }

    private EnemyInstanceData _instance; 

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyAttack = GetComponent<EnemyAttackHandler>();
        _defaultScale = transform.localScale;

        if (HasStateAuthority)
        {
            CurrentState = State.Patrol;
            IsFacingRight = transform.localScale.x > 0;
        }
    }

    public void Setup(EnemyInstanceData instance)
    {
        _instance = instance;

        if (HasStateAuthority)
        {
            // 権限者だけが乱数を振り、[Networked]変数に保存する
            SyncCurrentChaseSpeed = Random.Range(_instance.ChaseSpeed *_instance.EnemyData.ChaseSpeedMinMultiplier, _instance.ChaseSpeed * _instance.EnemyData.ChaseSpeedMaxMultiplier);
            SyncCurrentPatrolSpeed = Random.Range(_instance.PatrolSpeed *_instance.EnemyData.PatrolSpeedMinMultiplier, _instance.ChaseSpeed * _instance.EnemyData.PatrolSpeedMaxMultiplier);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (_enemyAttack.IsAttackSigning || _enemyAttack.IsAttacking || _instance == null)
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
        float scaleX = IsFacingRight ? Mathf.Abs(_defaultScale.x) : -Mathf.Abs(_defaultScale.x);
        transform.localScale = new Vector2(scaleX, _defaultScale.y);
    }

    /// <summary>
    /// 状態を変更する
    /// </summary>
    private void ChangeState()
    {
        bool isdetecting = Physics2D.OverlapCircle(transform.position, _instance.EnemyData.DetectDistance, _playerLayer);

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
        int direction = IsFacingRight ? 1 : -1;  // 現在の方向
        CheckObstaclesAndTurn(direction);       // 壁と崖を検知して左右反転させる

        int finalDirection = transform.localScale.x > 0 ? 1 : -1;   // 新しい方向
        IsFacingRight = finalDirection > 0;

        _rb.linearVelocityX = finalDirection * SyncCurrentPatrolSpeed;   
    }

    /// <summary>
    /// パトロール状態のみ使用　壁と崖を検知して左右反転させる
    /// </summary>
    /// <param name="direction"></param>
    private void CheckObstaclesAndTurn(int direction)
    {
        bool wallchecked = Physics2D.Raycast(transform.position, Vector2.right * direction, _wallCheckerRayLength, _groundLayer);   //壁チェックの光線

        Vector2 cliffCheckerPos = new Vector2(transform.position.x + direction * _cliffCheckerRayOffset, transform.position.y);     
        bool cliffchecked = !Physics2D.Raycast(cliffCheckerPos, Vector2.down, _cliffCheckerRayLength, _groundLayer);        //崖チェックの光線　崖が検知できなくなったら切り返す

        // チェックに引っかかったら左右反転
        if (wallchecked || cliffchecked)
        {
            transform.localScale = new Vector2(-direction * _defaultScale.x, _defaultScale.y);
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
            IsFacingRight = true;
            transform.localScale = _defaultScale;
        }
        else if(diff < 0)
        {
            IsFacingRight = false;
            transform.localScale = new Vector2(-_defaultScale.x, _defaultScale.y);
        }

        // プレイヤーとの距離が一定より近いならストップ
        if(Mathf.Abs(diff) < _instance.EnemyData.MinDistance)
        {
            return;
        }

        _rb.linearVelocityX = direction * SyncCurrentChaseSpeed;
    }

    /// <summary>
    /// ターゲット探し
    /// </summary>
    private void SearchTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _instance.EnemyData.DetectDistance, _playerLayer);

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

    //デバッグ----------------------------------------------------------------------------------//

    private void OnDrawGizmos()
    {
        // 実行中でなくてもlocalScaleから向きを判定
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector3 pos = transform.position;

        // --- 壁チェックの描画 (赤色) ---
        Gizmos.color = Color.red;
        Vector3 wallDest = pos + new Vector3(direction * _wallCheckerRayLength, 0, 0);
        Gizmos.DrawLine(pos, wallDest);

        // --- 崖チェックの描画 (青色) ---
        Gizmos.color = Color.blue;
        // 実際に計算で使っているオフセットを反映
        Vector3 cliffOrigin = pos + new Vector3(direction * _cliffCheckerRayOffset, 0, 0);
        Vector3 cliffDest = cliffOrigin + Vector3.down * _cliffCheckerRayLength;
        Gizmos.DrawLine(cliffOrigin, cliffDest);

        // レイの開始地点に小さな球を表示して視認性を上げる
        Gizmos.DrawSphere(cliffOrigin, 0.05f);

        // --- 検知範囲の描画 (黄色) ---
        if (_instance != null && _instance.EnemyData != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(pos, _instance.EnemyData.DetectDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(pos, _instance.EnemyData.MinDistance);
        }
    }
}

