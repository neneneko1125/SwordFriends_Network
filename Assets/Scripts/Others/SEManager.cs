using UnityEngine;
using Fusion;

public class SEManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sourceAttack;
    [SerializeField] private AudioSource _sourceAttack2;
    [SerializeField] private AudioSource _sourceDashAttack;
    [SerializeField] private AudioSource _sourceDownAttack;
    [SerializeField] private AudioSource _sourceLvUp;
    [SerializeField] private AudioSource _sourceCoin;
    [SerializeField] private AudioSource _sourceDamage;
    [SerializeField] private AudioSource _sourceHeal;
    [SerializeField] private AudioSource _sourceBuff;
    [SerializeField] private AudioSource _sourceButton;

    //[SerializeField] private AudioSource _source; 

    public static SEManager Instance { get; private set; } 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //‚±‚ê‚ðŒÄ‚Ño‚¹‚ÎSE‚ð–Â‚ç‚·‚±‚Æ‚ª‚Å‚«‚é
    public void SEDamage() => _sourceDamage.Play();
    public void SEAttack() => _sourceAttack.Play();
    public void SEAttack2() => _sourceAttack2.Play();
    public void SEDashAttack() => _sourceDashAttack.Play();
    public void SEDownAttack() => _sourceDownAttack.Play();
    public void SELvUp() => _sourceLvUp.Play();
    public void SECoin() => _sourceCoin.Play();
    public void SEHeal() => _sourceHeal.Play();
    public void SEBuff() => _sourceBuff.Play();
    public void SEButton() => _sourceButton.Play();
}