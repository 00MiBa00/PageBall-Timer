using Enums;
using Values;

namespace Models
{
    public class OnboardingModel
    {
        public string GetNameNextScene()
        {
            if (Subscription.HasSubscription())
            {
                return BooksInfo.HaveBooks ? SceneName.GameScene.ToString() : SceneName.CreateScene.ToString();
            }
            else
            {
                return SceneName.Subscription.ToString();
            }
        }
    }
}