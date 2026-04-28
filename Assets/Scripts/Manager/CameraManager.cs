using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //カメラが追いかける対象
    private Transform _target;

    //滑らかに動く時間　短いほど俊敏に動く
    [SerializeField] private float _smoothTime = 0.2f;

    //smoothDampのvarで使うときはfloatfloatじゃなくてVector3
    private Vector3 _velocity = Vector3.zero;

    //カメラの移動範囲　最小値と最大値
    [SerializeField] private float _minX, _maxX;
    [SerializeField] private float _minY, _maxY;

    void LateUpdate()
    {
        FollowTarget();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    /// <summary>
    /// ターゲットを追いかける
    /// </summary>
    void FollowTarget()
    {
        if (_target == null) return;

        // 目標位置
        Vector3 targetPos = new Vector3(
            Mathf.Clamp(_target.position.x, _minX, _maxX),
            Mathf.Clamp(_target.position.y, _minY, _maxY),
            transform.position.z
        );

        // 滑らかに追従
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref _velocity,
            _smoothTime
        );
    }
}
