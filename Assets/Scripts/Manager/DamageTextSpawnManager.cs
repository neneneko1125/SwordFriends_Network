using Fusion;
using TMPro;
using UnityEngine;

public class DamageTextSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _damageOfPlayerTextPrefab;
    [SerializeField] private GameObject _damageOfEnemyTextPrefab;

    public Canvas MainCanvas;

    public static DamageTextSpawnManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnDamageOfPlayerText(Transform targetTransform, int damage)
    {
        SpawnText(_damageOfPlayerTextPrefab, targetTransform, damage);
    }

    public void SpawnDamageOfEnemyText(Transform targetTransform, int damage)
    {
        SpawnText(_damageOfEnemyTextPrefab, targetTransform, damage);
    }

    private void SpawnText(GameObject prefab, Transform targetTransform, int damage)
    {
        if (prefab == null)
        {
            return;
        }

        GameObject damageText = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        // 座標のセット
        DamageText damageScript = damageText.GetComponent<DamageText>();
        if (damageScript != null)
        {
            damageScript.TargetWorldPos = targetTransform.position;
            damageScript.DamageAmount = damage;
        }

        // //DamageTextクラスでもテキスト変更処理をしているが、安定させるためにこっちでもやる
        TextMeshProUGUI text = damageText.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = damage.ToString();
        }
    }
}
