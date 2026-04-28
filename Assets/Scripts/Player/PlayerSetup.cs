using UnityEngine;
using Fusion;

public class PlayerSetup : NetworkBehaviour
{
    // [Networked]をつけると、Aさんが1に書き換えた瞬間、自動的にBさんの画面でも1になる
    [Networked] public int CharacterIndex { get; set; }

    [Tooltip("プレイヤーの子オブジェクトとしてついてるスキンたち")]
    [SerializeField] private GameObject[] _characterSkins;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            int id = Object.InputAuthority.PlayerId;

            CharacterIndex = (id - 1) % _characterSkins.Length;

            // メインカメラを探して、ターゲットに自分をセットする
            Camera.main.GetComponent<CameraManager>().SetTarget(this.transform);
        }
    }

    public override void Render()
    {
        if (_characterSkins.Length == 0)
        {
            return;
        }

        // スキンを設定(全員の画面に反映)
        for (int i = 0; i < _characterSkins.Length; i++)
        {
            if (_characterSkins[i] != null)
            {
                _characterSkins[i].SetActive(i == CharacterIndex);
            }
        }
    }
}