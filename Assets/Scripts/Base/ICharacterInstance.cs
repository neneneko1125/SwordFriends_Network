
public interface ICharacterInstance
{
    BaseCharacterData MasterData { get; }
    int CurrentHP { get; set; }
    void TakeDamage(int damage);
    bool IsDead { get; }
}