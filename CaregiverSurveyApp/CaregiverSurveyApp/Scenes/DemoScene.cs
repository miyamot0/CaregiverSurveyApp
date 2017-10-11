using CocosSharp;
using CaregiverSurveyApp.Layers;

namespace CaregiverSurveyApp.Scenes
{
    public class DemoScene : CCScene
    {
        CCLayer demoLayer;
        public CCScene HomeScene { get; private set; }

        public DemoScene(CCGameView gameView, CCScene homeScene) : base(gameView)
        {
            HomeScene = homeScene;
            demoLayer = new DemoLayer(gameView.DesignResolution.Width, gameView.DesignResolution.Height);

            AddLayer(demoLayer);
        }

        public void PopBackHome()
        {
            GameView.Director.PopScene(1.5f, new CCTransitionFade(1.5f, HomeScene));
        }
    }
}
