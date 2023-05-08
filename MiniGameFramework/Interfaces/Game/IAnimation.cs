
namespace MiniGameFramework
{
    public interface IAnimation : IGameObjectComponent
    {
        //Animation currBaseAnimation

        float playSpeed { get; set; }

        bool isCurrBaseAnimation(string aniName);

        void playAnimation(string aniName, float speed = 1.0f);

        void stopAnimation(string aniName);

        float getAnimatorStateInfoNormalizedTime();
        
    }
}