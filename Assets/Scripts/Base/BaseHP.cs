using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class BaseHP : NetworkBehaviour
{
    [SerializeField] protected int _maxHP = 5;
    [SerializeField] protected float _invincibleTime = 1.0f;
    [SerializeField] protected float _blinkingSpeed = 1.0f;

    [SerializeField] protected Image _hpBar;

    [Networked] protected int CurrentHP { get; set; }
    [Networked] protected bool IsInvincible { get; set; }
    [Networked] protected TickTimer InvincibilityTimer { get; set; }

    public override void Spawned()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        CurrentHP = _maxHP;
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
        if (IsInvincible)
        {
            // 時間を点滅速度で割った余りが 0.5 未満かどうかで 0 か 1 を決める
            float alpha = (Time.time * _blinkingSpeed % 1.0f) < 0.5f ? 0f : 1f;
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
        if (IsInvincible)
        {
            return;
        }

        CurrentHP -= damage;
        Rpc_AllEffects(damage);

        if (CurrentHP <= 0)
        {
            //死亡処理
            CurrentHP = 0;
            Runner.Despawn(Object);
        }
        else
        {
            //無敵処理
            IsInvincible = true;
            InvincibilityTimer = TickTimer.CreateFromSeconds(Runner, _invincibleTime);
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
            _hpBar.fillAmount = (float)CurrentHP / _maxHP;
        }

        UpdateBarDirection();
    }

    protected virtual void PlaySound() { }
    protected virtual void PlayEffect() { }
    protected virtual void UpdateBarDirection() { }
    protected virtual void SpawnDamageText(int damage) { }
}
