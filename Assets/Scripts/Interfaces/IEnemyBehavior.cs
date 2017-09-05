namespace Assets.Scripts.Interfaces
{
    public interface IEnemyBehavior
    {
        void TakeDamage(IGunBehavior gun);

        void OnDestroyed();

    }
}
