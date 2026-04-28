using UnityEngine.UI;

public class PlayerHP : BaseHP
{
    private PlayerMovement _playerMovement;

    public override void Spawned()
    {
        base.Spawned();
        _playerMovement = GetComponent<PlayerMovement>();
    }


    protected override void UpdateBarDirection()
    {
        if (_playerMovement == null)
        {
            return;
        }

        //HPÉoÅ[ÇÃç∂âEîΩì]
        if (_playerMovement.IsFacingRightNet)
        {
            _hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            _hpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    protected override void PlaySound()
    {
        SEManager.Instance.SEDamage();
    }

    protected override void SpawnDamageText(int damage)
    {
        DamageTextSpawnManager.Instance.SpawnDamageOfPlayerText(transform, damage);
    }
}
