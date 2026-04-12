using UnityEngine;
using Fusion;

public class PlayerSetup : NetworkBehaviour
{
    // [Networked]をつけると、Aさんが1に書き換えた瞬間、自動的にBさんの画面でも1になる
    [Networked] public int CharacterIndex { get; set; }

    [SerializeField] private GameObject[] characterSkins;

    public override void Spawned()
    {
        // 操作権限がある場合のみ、同期変数を上書き
        if (HasStateAuthority)
        {
            // InputAuthority(プレイヤー番号)を元にスキン決定
            int id = Object.InputAuthority.RawEncoded;
            CharacterIndex = id % characterSkins.Length;
        }
    }

    /// <summary>
    /// 連続で実行　Updateメソッドと似てる
    /// </summary>
    public override void Render()
    {
        if (characterSkins.Length == 0)
        {
            return;
        }

        // 全員の画面で、同期されたCharacterIndexに基づいて見た目を変える
        for (int i = 0; i < characterSkins.Length; i++)
        {
            if (characterSkins[i] != null)
            {
                characterSkins[i].SetActive(i == CharacterIndex);
            }
        }
    }
}