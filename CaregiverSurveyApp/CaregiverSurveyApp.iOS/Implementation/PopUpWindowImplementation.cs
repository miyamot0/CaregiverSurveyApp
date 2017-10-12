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

using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using CaregiverSurveyApp.iOS.Implementation;
using CaregiverSurveyApp.Interfaces;
using CaregiverSurveyApp.Views;

[assembly: Dependency(typeof(PopUpWindowImplementation))]
namespace CaregiverSurveyApp.iOS.Implementation
{
    public class PopUpWindowImplementation : IPopUpWindow
    {
        public void ShowPopup(PopUpWindow popup)
        {
            var alert = new UIAlertView
            {
                Title = popup.Title,
                Message = "",
                AlertViewStyle = UIAlertViewStyle.LoginAndPasswordInput,
            };

            alert.GetTextField(0).AutocorrectionType = UITextAutocorrectionType.Default;
            alert.GetTextField(0).AutocapitalizationType = UITextAutocapitalizationType.Words;

            alert.GetTextField(1).AutocorrectionType = UITextAutocorrectionType.Default;
            alert.GetTextField(1).SecureTextEntry = false;
            alert.GetTextField(1).AutocapitalizationType = UITextAutocapitalizationType.Words;

            foreach (var b in popup.Buttons)
            {
                alert.AddButton(b);
            }

            alert.Clicked += (s, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = popup.Buttons.ElementAt(Convert.ToInt32(args.ButtonIndex)),
                    Text = alert.GetTextField(0).Text,
                    Text2 = alert.GetTextField(1).Text
                });
            };
            alert.Show();
        }
    }
}