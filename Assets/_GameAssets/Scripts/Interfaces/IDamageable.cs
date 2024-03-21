public interface IDamageable
{
    public float MaxHealth { get; }
    public float Health { get; }
     void TakeDamage(float damageValue);
}