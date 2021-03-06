﻿//----------------------------------------------------------------------------------------------
// <copyright file="StoredJson.cs" 
// Copyright November 6, 2016 Shawn Gilroy
//
// This file is part of CaregiverStudyApp
//
// CaregiverStudyApp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// CaregiverStudyApp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CaregiverStudyApp.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The CaregiverStudyApp is a tool to assist clinicans and researchers in the treatment of communication disorders.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

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

            double padding = 0.0;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    padding = 20;
                    break;

                case Device.Android:
                    padding = 0;
                    break;
            }

            Padding = new Thickness(0, padding, 0, 0);

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
            App.GameView = sender as CCGameView;

            if (App.GameView != null)
            {
                App.GameView.DesignResolution = App.GameView.ViewSize;
                App.GameView.ContentManager.SearchPaths = new List<string>()
                {
                    "Sounds",
                    "Images"
                };

                CCSprite.DefaultTexelToContentSizeRatio = 1.0f;

                CCAudioEngine.SharedEngine.PreloadEffect("Sounds/Success");
                // TODO: load stars?

                App.StartingScene = new StartScene();
                App.GameView.RunWithScene(App.StartingScene);
            }
        }
    }
}