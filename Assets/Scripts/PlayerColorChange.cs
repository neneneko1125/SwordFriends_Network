using UnityEngine;
using Fusion;

public class PlayerColorChange : NetworkBehaviour
{
    // [Networked] をつけることで、この変数は全プレイヤー間で同期されます
    // 変化があった時に ColorChanged メソッドを呼び出す設定です
    [Networked]
    [OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }

    private SpriteRenderer _renderer;


    public override void Spawned()
    {
        _renderer = GetComponent<SpriteRenderer>();
        // 生成された瞬間に、現在のネットワーク上の値を色に反映させる
       // ColorChanged();
    }

    public override void FixedUpdateNetwork()
    {
        // 操作権（Authority）を持っている自分だけが色を変更できる
        if (HasStateAuthority == false) return;

        // スペースキーを押したらランダムな色に変更
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NetworkedColor = Random.ColorHSV();
        }
    }

    // 値が変わったときに、全クライアントで実行される見た目の反映処理
    void ColorChanged()
    {
        if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = NetworkedColor;
    }

}