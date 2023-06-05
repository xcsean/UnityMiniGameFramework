namespace UnityMiniGameFramework.Addressable
{
    public interface IPooledActive
    {
        int priority { get; }

        void OnActive();

        void OnInactive();
    }
}