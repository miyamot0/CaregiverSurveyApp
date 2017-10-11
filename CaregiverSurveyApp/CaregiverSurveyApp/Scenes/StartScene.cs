using CocosSharp;
using CaregiverSurveyApp.Layers;

namespace CaregiverSurveyApp.Scenes
{
    public class StartScene : CCScene
    {
        CCLayer startLayer;

        public StartScene(CCGameView gameView) : base(gameView)
        {
            startLayer = new StartLayer(gameView.DesignResolution.Width, gameView.DesignResolution.Height);

            AddLayer(startLayer);
        }
    }
}
