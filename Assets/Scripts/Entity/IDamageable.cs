
namespace Entity
{
    // Implement IDamageable to be able to take damage.
    public interface IDamageable
    {
        float Health { get; }

        void die();

        void takeDamage(float damage);
    }
}
