using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CocosSharp;
using CaregiverSurveyApp.Scenes;

namespace CaregiverSurveyApp.Pages
{
    public class GamePage : ContentPage
	{
        /// <summary>
        /// Init
        /// </summary>
		public GamePage ()
		{
            var grid = new Grid();

            Content = grid;

            grid.RowDefinitions = new RowDefinitionCollection {
                new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                }
            };

            CreateGameView(grid);
        }

        /// <summary>
        /// Make view
        /// </summary>
        /// <param name="grid"></param>
        void CreateGameView(Grid grid)
        {
            var gameView = new CocosSharpView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ViewCreated = HandleViewCreated
            };

            grid.Children.Add(gameView, 0, 0);
        }

        /// <summary>
        /// After CC view is made, get down to business
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HandleViewCreated(object sender, EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;

            if (gameView != null)
            {
                gameView.DesignResolution = gameView.ViewSize;

                var contentSearchPaths = new List<string>()
                {
                    //"Fonts",
                    "Sounds",
                    "Images"
                };

                gameView.ContentManager.SearchPaths = contentSearchPaths;

                CCSprite.DefaultTexelToContentSizeRatio = 1.0f;
                CCAudioEngine.SharedEngine.PreloadEffect("Sounds/Success");

                StartScene mScene = new StartScene(gameView);
                gameView.RunWithScene(mScene);
            }
        }
    }
}