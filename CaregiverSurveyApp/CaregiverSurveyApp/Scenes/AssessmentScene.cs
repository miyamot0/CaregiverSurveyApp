using CocosSharp;
using CaregiverSurveyApp.Layers;

namespace CaregiverSurveyApp.Scenes
{
    public class AssessmentScene : CCScene
    {
        CCLayer assessmentLayer;
        public CCScene HomeScene { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameView"></param>
        /// <param name="homeScene"></param>
        public AssessmentScene(CCGameView gameView, CCScene homeScene) : base(gameView)
        {
            HomeScene = homeScene;
            assessmentLayer = new AssessmentLayer(gameView.DesignResolution.Width, gameView.DesignResolution.Height);

            AddLayer(assessmentLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PopBackHome()
        {
            GameView.Director.PopScene(1.5f, new CCTransitionFade(1.5f, HomeScene));
        }
    }
}
