using DG.Tweening;
using Fusion;
using UnityEngine;
using TMPro;

public class DamageText : NetworkBehaviour
{
    [SerializeField] private float _lifeTime = 1.0f;
    [SerializeField] private float _upAmount;

    // public に変更して、外部（マネージャー）から書き込めるようにする
    [Networked] public Vector3 TargetWorldPos { get; set; }

    private RectTransform _rect;
    private TextMeshProUGUI _damageText;
    private Vector2 _startPosition;

    [Networked] private TickTimer LifeTimer { get; set; }
    [Networked] public int DamageAmount { get; set; }
    public override void Spawned()
    {
        _rect = GetComponent<RectTransform>();
        _damageText = GetComponent<TextMeshProUGUI>();

        if (DamageTextSpawnManager.Instance != null && DamageTextSpawnManager.Instance.MainCanvas != null)
        {
            transform.SetParent(DamageTextSpawnManager.Instance.MainCanvas.transform, false);
        }

        //DamageTextSpawnManagerでもテキスト変更処理をしているが、安定させるためにこっちでもやる
        if(_damageText != null)
        {
            _damageText.text = DamageAmount.ToString();
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(TargetWorldPos);

        if (_rect != null)
        {
            _rect.position = screenPos;
            _startPosition = _rect.anchoredPosition;
        }

        DamageTextAnimation();

        if (HasStateAuthority)
        {
            //タイマー開始
            LifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (LifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    private void DamageTextAnimation()
    {
        if (_rect == null)
        {
            return;
        }

        _rect.DOAnchorPosY(_startPosition.y + _upAmount, _lifeTime).SetEase(Ease.OutQuad);
    }
}