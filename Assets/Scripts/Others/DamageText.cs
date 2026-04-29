using DG.Tweening;
using Fusion;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 1.0f;
    [SerializeField] private float _upAmount;

    private RectTransform _rect;
    private TextMeshProUGUI _damageText;
    private Vector2 _startPosition;

    [Networked] public int DamageAmount { get; set; }
    [Networked] public Vector3 TargetWorldPos { get; set; }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _damageText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (DamageTextSpawnManager.Instance != null && DamageTextSpawnManager.Instance.MainCanvas != null)
        {
            transform.SetParent(DamageTextSpawnManager.Instance.MainCanvas.transform, false);
        }

        //DamageTextSpawnManagerでもテキスト変更処理をしているが、安定させるためにこっちでもやる
        if (_damageText != null)
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

        Destroy(gameObject, _lifeTime);
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