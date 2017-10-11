using CocosSharp;
using CaregiverSurveyApp.Layers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaregiverSurveyApp.Scenes
{
    public class AssessmentScene : CCScene
    {
        CCLayer assessmentLayer;
        public CCScene HomeScene { get; private set; }

        public AssessmentScene(CCGameView gameView, CCScene homeScene) : base(gameView)
        {
            HomeScene = homeScene;
            assessmentLayer = new AssessmentLayer(gameView.DesignResolution.Width, gameView.DesignResolution.Height);

            AddLayer(assessmentLayer);
        }

        public void PopBackHome()
        {
            GameView.Director.PopScene(1.5f, new CCTransitionFade(1.5f, HomeScene));
        }
    }
}
