using CocosSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaregiverSurveyApp.Scenes
{
    public class TestScene : CCScene
    {
        CCDrawNode circle;

        public TestScene(CCGameView gameView) : base(gameView)
        {
            var layer = new CCLayer();
            this.AddLayer(layer);
            circle = new CCDrawNode();
            layer.AddChild(circle);
            circle.DrawCircle(
                // The center to use when drawing the circle,
                // relative to the CCDrawNode:
                new CCPoint(0, 0),
                radius: 15,
                color: CCColor4B.White);
            circle.PositionX = 20;
            circle.PositionY = 50;
        }
    }
}
