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

using CocosSharp;

namespace CaregiverSurveyApp.Values
{
    /// <summary>
    /// 
    /// </summary>
    public static class Constants
    {
        public static CCColor3B lightBlue = new CCColor3B(222, 235, 247);
        public static CCColor3B midBlue = new CCColor3B(158, 202, 225);
        public static CCColor3B darkBlue = new CCColor3B(49, 130, 189);

        public static int hOffset = 10;
        public static int vOffset = 10;

        public static string demoInstruction = "The purpose of this application is find out what type of things that caregivers are motivated by. " +
            "For example, some are looking to make things perfect in the long-term and others are more motivated by smaller differences right away. " + 
            "In this program, we ask that you make choices by dragging options you’d prefer into the bottom portion of the screen. For practice, please put the choice you want in the box below.";

        public static string demoTextSsr = "Choice I Want";
        public static string demoTextLlr = "Choice I Do notWant";

        public static string assessmentInstruction = "Now or in {0}";

        public static string assessmentTextSsr = "I'd prefer to have {0}% less issues immediately.";
        public static string assessmentTextLlr = "I'd prefer to have 100% fewer issues in {0}";

        /// <summary>
        /// 
        /// </summary>
        public static string[] delayStrings = 
        {
            "1 month",
            "3 months",
            "9 months",
            "2 years",
            "5 years",
            "10 years",
            "20 years"
        };

        /// <summary>
        /// 
        /// </summary>
        public enum SpriteTags
        {
            None = 0,
            SSR = 1,
            LLR = 2,
            Text = 3
        }

        public static int PaddingTop = 25;

        public static float ButtonNormal = 40f;
        public static float ButtonStart = 96f;

        public static string LabelFont = "Arial";
        public static float LabelTitleSize = 72f;

        public static float TextReadingSize = 26f;
    }
}
