//----------------------------------------------------------------------------------------------
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

using CaregiverSurveyApp.Pages;
using CaregiverSurveyApp.Scenes;
using CocosSharp;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace CaregiverSurveyApp
{
    public class App : Application
    {
        /// <summary>
        /// App Settings
        /// </summary>
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        /// <summary>
        /// App Settings Key (submission count)
        /// </summary>
        private const string IteratorKey = "iteration_count";
        private static readonly int IteratorDefault = 0;

        /// <summary>
        /// Submission Counter
        /// </summary>
        public static int SubmissionCounter
        {
            get
            {
                return AppSettings.GetValueOrDefault(IteratorKey, IteratorDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IteratorKey, value);
            }
        }

        public static string AppName = "com.smallnstats.caregiversurveyapp";
        public static string Token = "";
        public static string ApiAddress = "";
        public static string DeviceName = "";

        public static bool UpdateValue = false;
        public static bool Debugging = true;

        public static CCGameView GameView;
        public static StartScene StartingScene;

        public static AssessmentScene AssessScene;

        public static int Height;
        public static int Width;

        // This is insane
        public static int RetrySendCount = 100;

        /// <summary>
        /// Main App
        /// </summary>
        public App()
        {
            MainPage = new GamePage();
        }
    }
}
