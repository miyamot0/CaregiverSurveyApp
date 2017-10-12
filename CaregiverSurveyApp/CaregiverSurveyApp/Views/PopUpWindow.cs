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

using CaregiverSurveyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CaregiverSurveyApp.Views
{
    /// <summary>
    /// Assigned/returned args to cross-platform window
    /// </summary>
    public class PopUpWindowArgs : EventArgs
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Button { get; set; }
    }

    /// <summary>
    /// Pop-up window interface
    /// </summary>
    public class PopUpWindow
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Text2 { get; set; }
        public List<string> Buttons { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        public PopUpWindow(string title, string text, params string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons.ToList();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public PopUpWindow(string title, string text) : this(title, text, "OK", "Cancel") { }

        /// <summary>
        /// Event call following the close of the window, drawing args
        /// </summary>
        public event EventHandler<PopUpWindowArgs> PopupClosed;
        public void OnPopupClosed(PopUpWindowArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        /// <summary>
        /// Dependency Service call to individual implementation
        /// </summary>
        public void Show()
        {
            DependencyService.Get<IPopUpWindow>().ShowPopup(this);
        }
    }
}