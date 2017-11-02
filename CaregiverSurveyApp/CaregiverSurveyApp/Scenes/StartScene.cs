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

using CocosSharp;
using CaregiverSurveyApp.Layers;

namespace CaregiverSurveyApp.Scenes
{
    /// <summary>
    /// Staring Scene page
    /// </summary>
    public class StartScene : CCScene
    {
        CCLayer startLayer;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="gameView"></param>
        public StartScene() : base(App.GameView)
        {
            App.Height = App.GameView.DesignResolution.Height;
            App.Width = App.GameView.DesignResolution.Width;

            startLayer = new StartLayer();

            AddLayer(startLayer);
        }
    }
}
