
public interface ICharacterInstance
{
    BaseCharacterData BaseData { get; }
    int CurrentHP { get; set; }
    void TakeDamage(int damage);
    bool IsDead { get; }
}