using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BaseHP : NetworkBehaviour
{
    [SerializeField] protected Image _hpBar;

    //Sync:同期　値の変更はInstanceDataがやってくれたので、ここでは同期処理をする
    [Networked] protected int SyncMaxHP { get; set; }
    [Networked] protected int SyncCurrentHP { get; set; }
    [Networked] protected float SyncInvincibleTime { get; set; }
    [Networked] protected bool IsInvincible { get; set; }
    [Networked] protected TickTimer InvincibilityTimer { get; set; }

    //インタフェースのおかげでプレイヤーでも敵でも対応可能
    protected ICharacterInstance _instance;

    public void Setup(ICharacterInstance instance)
    {
        //PlayerData,もしくはEnemyData
        _instance = instance;

        if (HasStateAuthority)
        {
            SyncMaxHP = _instance.MasterData.MaxHP;
            SyncCurrentHP = SyncMaxHP;
            SyncInvincibleTime = _instance.MasterData.InvincibleTime;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsInvincible && InvincibilityTimer.Expired(Runner))
        {
            IsInvincible = false;
        }
    }

    public override void Render()
    {
        if (_instance == null)
        {
            return;
        }

        if (IsInvincible)
        {
            // 時間を点滅速度で割った余りが 0.5 未満かどうかで 0 か 1 を決める
            float alpha = (Time.time * _instance.MasterData.BlinkingSpeed % 1.0f) < 0.5f ? 0f : 1f;
            SetAlpha(alpha);
        }
        else
        {
            //白色(元々の色)に戻す
            SetAlpha(1.0f);
        }

        UpdateUI();
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]    //誰でもRPCを呼べる　そのオブジェクトの管理権限を持っている人の画面だけで実行
    public void Rpc_TakeDamage(int damage)
    {
        if (IsInvincible || _instance == null)
        {
            return;
        }

        _instance.TakeDamage(damage);
        SyncCurrentHP = _instance.CurrentHP;    //ダメージ処理の直後も同期

        Rpc_AllEffects(damage);

        if (_instance.IsDead)
        {
            Runner.Despawn(Object);
        }
        else
        {
            //無敵処理
            IsInvincible = true;
            InvincibilityTimer = TickTimer.CreateFromSeconds(Runner, _instance.MasterData.InvincibleTime);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]    //そのオブジェクトの管理権限を持っている人だけ呼べる　全員が実行する
    private void Rpc_AllEffects(int damage)
    {
        PlaySound();        // 音を鳴らす
        PlayEffect();       // エフェクトを出す
        SpawnDamageText(damage); // ダメージテキスト
    }

    protected virtual void SetAlpha(float alpha)
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;
        }
    }

    protected virtual void UpdateUI()
    {
        if (_hpBar != null)
        {
            _hpBar.fillAmount = (float)SyncCurrentHP / SyncMaxHP;
        }

        UpdateBarDirection();
    }

    protected virtual void PlaySound() { }
    protected virtual void PlayEffect() { }
    protected virtual void UpdateBarDirection() { }
    protected virtual void SpawnDamageText(int damage) { }
}
