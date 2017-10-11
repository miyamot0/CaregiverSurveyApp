using CocosSharp;
using CaregiverSurveyApp.Layers;

namespace CaregiverSurveyApp.Scenes
{
    /// <summary>
    /// 
    /// </summary>
    public class StartScene : CCScene
    {
        CCLayer startLayer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameView"></param>
        public StartScene(CCGameView gameView) : base(gameView)
        {
            startLayer = new StartLayer(gameView.DesignResolution.Width, gameView.DesignResolution.Height);

            AddLayer(startLayer);
        }
    }
}
