using Fusion;
using TMPro;
using UnityEngine;

public class DamageTextSpawnManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject _damageOfPlayerTextPrefab;
    [SerializeField] private NetworkObject _damageOfEnemyTextPrefab;

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

    private void SpawnText(NetworkObject prefab, Transform targetTransform, int damage)
    {
        if (prefab == null || Runner == null)
        {
            return;
        }

        // 第5引数のラムダ式で、ネットワーク同期が始まる前に値をセットする
        Runner.Spawn(prefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer, (runner, obj) =>
        {
            // //DamageTextクラスでもテキスト変更処理をしているが、安定させるためにこっちでもやる
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = damage.ToString();
            }

            // 座標のセット
            DamageText damageScript = obj.GetComponent<DamageText>();
            if (damageScript != null)
            {
                damageScript.TargetWorldPos = targetTransform.position;
                damageScript.DamageAmount = damage;
            }
        });
    }

}
