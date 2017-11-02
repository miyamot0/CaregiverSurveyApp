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
using System.Threading.Tasks;
using Xamarin.Auth;
using System.Linq;
using Acr.UserDialogs;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;
using System.Text;

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
            string statusText = await ChallengeCredentials();

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

                credentialLabel.Text = string.Format("{0} ({1})", text, App.SubmissionCounter);
                credentialLabel.PositionX = credentialLabel.ContentSize.Width / 2 + Constants.hOffset;
                credentialLabel.Color = (Ready) ? CCColor3B.Black : CCColor3B.Red;
            }, 0.1f);
        }

        /// <summary>
        /// Submit credentials to observe response
        /// </summary>
        /// <returns></returns>
        private Task<string> ChallengeCredentials()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                using (var progress = UserDialogs.Instance.Loading("Checking credentials ...", null, null, true, MaskType.Black))
                {
                    progress.Title = "Checking for credentials...";

                    var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

                    if (account != null)
                    {
                        if (CheckAccountContent())
                        {
                            string result = await await TestServerCredentials(account.Properties["Server"],
                                            account.Properties["Key"],
                                            account.Properties["DeviceName"]).ContinueWith(async t => await CloseUserDialog(t.Result));

                            if (ValidateResponseString(result))
                            {
                                App.Token = account.Properties["Key"];
                                App.ApiAddress = account.Properties["Server"];
                                App.DeviceName = account.Properties["DeviceName"];

                                tcs.SetResult("Status: Ready.");
                            }
                            else
                            {
                                tcs.SetResult("Status: Auth failed.");
                            }
                        }
                        else
                        {
                            tcs.SetResult("Status: Credentials Empty.");
                        }
                    }
                    else
                    {
                        tcs.SetResult("Status: No Credentials Found.");
                    }
                }
            });

            return tcs.Task;
        }
        
        /// <summary>
        /// Challenge specific credentials
        /// </summary>
        /// <returns></returns>
        private async Task<string> ChallengeCredentials(string _address, string _key, string _id)
        {
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Checking credentials ...", MaskType.Black));

            string result = await await TestServerCredentials(_address, _key, _id)
                .ContinueWith(async t => await CloseUserDialog(t.Result));

            return result;
        }

        /// <summary>
        /// Check if anything is there
        /// </summary>
        /// <returns></returns>
        private bool CheckAccountContent()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

            if (account != null)
            {
                if (string.IsNullOrWhiteSpace(account.Properties["Key"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["Server"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["DeviceName"]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
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
                string ans = await ChallengeCredentials(Server, Key, Id);

                if (ValidateResponseString(ans))
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
        /// Await-able window for assigning icon label
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Task<string[]> TextInputWindow(string query)
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var login = await UserDialogs.Instance.LoginAsync(new LoginConfig
                {
                    Message = "Enter your credentials",
                    OkText = "Ok",
                    CancelText = "Cancel",
                    LoginPlaceholder = "Server",
                    PasswordPlaceholder = "Key",
                });

                if (login.Ok)
                {
                    tcs.SetResult(new string[] { login.LoginText, login.Password });
                }
                else
                {
                    tcs.SetResult(new string[] { "", "" });
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Task<string> CloseUserDialog(string b)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.HideLoading();

                tcs.SetResult(b);
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> TestServerCredentials(string _address, string _key, string _id)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            try
            {
                // Throw if MITM over captive net
                // Throw if SSL issue
                var httpClient = new HttpClient(new NativeMessageHandler(
                    throwOnCaptiveNetwork: true,
                    customSSLVerification: true
                ));

                var mId = String.Format("{0}-{1}",
                    _id,
                    "TestWrite");

                double[] values = { -1, -1, -1, -1, -1, -1, -1, -1 };

                var parameters = new Dictionary<string, string>
                        {
                            { "token", _key },
                            { "content", "record"},
                            { "format", "xml" },
                            { "type", "flat"},
                            { "overwriteBehavior", "overwrite" },
                            { "data", ConstructResponse(mId, values) },
                            { "returnContent", "count" },
                            { "returnFormat", "json" }
                        };

                var encodedContent = new FormUrlEncodedContent(parameters);

                var resp = await httpClient.PostAsync(new Uri(_address), encodedContent);

                string returnStatment = "";

                if (resp.IsSuccessStatusCode)
                {
                    returnStatment = await resp.Content.ReadAsStringAsync();

                    tcs.SetResult(returnStatment);
                }
                else
                {
                    tcs.SetResult(returnStatment);
                }
            }
            catch
            {
                tcs.SetResult("");
            }

            return await tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string ConstructResponse(string id, double[] values)
        {
            string temp;
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<records>");
            sb.Append("<item>");
            sb.Append("<record_id>");
            sb.Append(id);
            sb.Append("</record_id>");

            for (int i = 0; i < 8; i++)
            {
                temp = string.Format("<{0}>{1}</{0}>",
                    "delay_" + (i + 1),
                    values[i]);

                sb.Append(temp);
            }


            sb.Append("</item>");
            sb.Append("</records>");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        private bool ValidateResponseString(string resp)
        {
            return (resp.Trim().Equals("{\"count\": 1}"));
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

                        var result = await TextInputWindow("What are your credentials?");

                        SaveCredentials(result[0], result[1], id);

                        UpdateLabelText(await ChallengeCredentials());
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
                    var result = await TextInputWindow("What are your credentials?");

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
                App.GameView.Director.PushScene(new CCTransitionFade(1.5f, new DemoScene()));
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
