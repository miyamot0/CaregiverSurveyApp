﻿using System;
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
                Message = popup.Text,
                AlertViewStyle = UIAlertViewStyle.PlainTextInput,
            };

            alert.GetTextField(0).AutocorrectionType = UITextAutocorrectionType.Default;
            alert.GetTextField(0).AutocapitalizationType = UITextAutocapitalizationType.Words;

            foreach (var b in popup.Buttons)
                alert.AddButton(b);

            alert.Clicked += (s, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = popup.Buttons.ElementAt(Convert.ToInt32(args.ButtonIndex)),
                    Text = alert.GetTextField(0).Text
                });
            };
            alert.Show();
        }
    }
}