namespace SGoap
{
    public interface IDamagable
    {
        void TakeDamage(int damage = 1, IAttacker attacker = null);
    }
}