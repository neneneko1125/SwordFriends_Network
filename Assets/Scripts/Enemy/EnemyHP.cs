using UnityEngine.UI;
using UnityEngine;

public class EnemyHP : BaseHP
{
    [SerializeField] private GameObject _effectPrefab;
    private EnemyMovement _enemyMovement;

    public override void Spawned()
    {
        base.Spawned();
        _enemyMovement = GetComponent<EnemyMovement>();
    }


    protected override void UpdateBarDirection()
    {
        if (_enemyMovement == null)
        {
            return;
        }

        //HPÉoÅ[ÇÃç∂âEîΩì]
        if (_enemyMovement.IsFacingRightNet)
        {
            _hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            _hpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    protected override void PlayEffect()
    {
        int rnd = Random.Range(0, 360);
        Quaternion randomRotation = Quaternion.Euler(0, 0, rnd);
        Instantiate(_effectPrefab, transform.position, randomRotation);
    }

    protected override void SpawnDamageText(int damage)
    {
        DamageTextSpawnManager.Instance.SpawnDamageOfEnemyText(transform, damage);
    }
}
