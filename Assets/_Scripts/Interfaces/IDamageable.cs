public interface IDamageable
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    void TakeDamage(int damageAmount);
}