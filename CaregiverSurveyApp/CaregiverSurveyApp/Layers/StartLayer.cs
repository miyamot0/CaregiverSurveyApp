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
using CaregiverSurveyApp.Scenes;
using CaregiverSurveyApp.Values;
using System;
using Xamarin.Forms;
using Xamarin.Auth;
using System.Linq;
using CaregiverSurveyApp.Utilities;

namespace CaregiverSurveyApp.Layers
{
    public class StartLayer : CCLayerColor
    {
        CCControlButton statusControlButton,
                        startControlButton;

        CCScale9Sprite statusButton,
                       startButton;

        CCLabel statusLabel,
                startLabel,
                credentialLabel;

        private bool Ready = false;

        private CCSpriteSheet spriteSheet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public StartLayer() : base(CCColor4B.AliceBlue)
        {
            Color = Constants.midBlue;

            spriteSheet = new CCSpriteSheet("static.plist");

            statusButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Green")));
            statusButton.ContentSize = new CCSize(App.Width / 4, App.Height / 8);

            statusLabel = new CCLabel("Settings", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            statusLabel.Color = CCColor3B.Black;

            statusControlButton = new CCControlButton(statusLabel, statusButton);
            statusControlButton.PositionX = App.Width - statusControlButton.ContentSize.Width / 2 - Constants.hOffset;
            statusControlButton.PositionY = App.Height - statusControlButton.ContentSize.Height / 2 - Constants.vOffset;
            statusControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            statusControlButton.Clicked += StatusControlButton_Clicked;

            AddChild(statusControlButton);

            startButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blue")), new CCRect(10, 10, 10, 10));
            startButton.ContentSize = new CCSize(App.Width / 2, App.Height / 4);

            startLabel = new CCLabel("Begin Survey", Constants.LabelFont, Constants.ButtonStart, CCLabelFormat.SystemFont);
            startLabel.Color = CCColor3B.Black;

            startControlButton = new CCControlButton(startLabel, startButton);
            startControlButton.PositionX = App.Width / 2 - Constants.hOffset;
            startControlButton.PositionY = startControlButton.ContentSize.Height / 2 + (Constants.vOffset * 3);
            startControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            startControlButton.Clicked += StartControlButton_Clicked;

            AddChild(startControlButton);

            credentialLabel = new CCLabel("Status: Checking...", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            credentialLabel.PositionX = credentialLabel.ContentSize.Width / 2 + Constants.hOffset;
            credentialLabel.Color = CCColor3B.Red;
            credentialLabel.PositionY = App.Height - credentialLabel.ContentSize.Height / 2 - Constants.vOffset;
            credentialLabel.AnchorPoint = CCPoint.AnchorMiddle;

            AddChild(credentialLabel);

            // Begin startup check
            UpdateStatus();

            Schedule(t =>
            {
                if (App.UpdateValue)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.UpdateValue = false;
                    });

                    UpdateStatus();
                }
            }, 3f);
        }

        /// <summary>
        /// Awaitable call to update the text
        /// </summary>
        public async void UpdateStatus()
        {
            string statusText = await ServerTools.ChallengeCredentials();

            UpdateLabelText(statusText);
        }

        /// <summary>
        /// Update label based on server response
        /// </summary>
        /// <param name="text"></param>
        private void UpdateLabelText(string text)
        {
            ScheduleOnce((dt) =>
            {
                Ready = (text.Trim().Equals("Status: Ready."));

                credentialLabel.Text = string.Format("{0} ({1})", 
                    text, 
                    App.SubmissionCounter);

                credentialLabel.PositionX = credentialLabel.ContentSize.Width / 2 + Constants.hOffset;
                credentialLabel.Color = (Ready) ? CCColor3B.Black : CCColor3B.Red;
            }, 0.1f);
        }
                        
        /// <summary>
        /// Save credentials if appropriate
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Key"></param>
        private async void SaveCredentials(string Server, string Key, string Id)
        {
            if (!string.IsNullOrWhiteSpace(Server) && !string.IsNullOrWhiteSpace(Key))
            {
                string ans = await ServerTools.ChallengeCredentials(Server, Key, Id);

                if (ServerTools.ValidateResponseString(ans))
                {
                    Account account = new Account
                    {
                        Username = App.AppName
                    };

                    account.Properties.Add("Server", Server);
                    account.Properties.Add("Key", Key);
                    account.Properties.Add("DeviceName", Id);

                    AccountStore.Create().Save(account, App.AppName);

                    UpdateLabelText("Status: Ready.");

                    await App.Current.MainPage.DisplayAlert("Credentials accepted", "Credentials are stored and ready", "Close");
                }
                else
                {
                    UpdateLabelText("Status: Credentials failed.");

                    await App.Current.MainPage.DisplayAlert("Incorrect credentials", "Credentials were not correct", "Close");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusControlButton_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

                string id = Guid.NewGuid().ToString();

                if (account != null)
                {
                    var action = await App.Current.MainPage.DisplayActionSheet("Existing credentials found",
                        "Cancel",
                        null,
                        "Modify Credentials",
                        "Delete Credentials");

                    if (action != null && action == "Modify Credentials")
                    {
                        AccountStore.Create().Delete(account, App.AppName);

                        var result = await ServerTools.TextInputWindow("What are your credentials?");

                        SaveCredentials(result[0], result[1], id);

                        UpdateLabelText(await ServerTools.ChallengeCredentials());
                    }
                    else if (action != null && action == "Delete Credentials")
                    {
                        AccountStore.Create().Delete(account, App.AppName);

                        App.SubmissionCounter = 0;

                        UpdateLabelText("Status: Credentials Deleted");
                    }
                }
                else
                {
                    var result = await ServerTools.TextInputWindow("What are your credentials?");

                    SaveCredentials(result[0], result[1], id);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartControlButton_Clicked(object sender, EventArgs e)
        {
            if (Ready)
            {
                App.DemonstrationScene = new DemoScene();

                App.GameView.Director.PushScene(new CCTransitionFade(1.5f, App.DemonstrationScene));
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Not fully set initialized yet.", "OK");
                });
            }
        }
    }
}
