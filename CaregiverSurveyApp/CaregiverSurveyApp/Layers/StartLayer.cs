using CocosSharp;
using CaregiverSurveyApp.Scenes;
using CaregiverSurveyApp.Values;
using System;
using Xamarin.Forms;
using CaregiverSurveyApp.Views;
using System.Threading.Tasks;
using Xamarin.Auth;
using System.Linq;
using Acr.UserDialogs;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;
using System.Diagnostics;
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

        int paddingTop = 0;

        private float Height,
                      Width;

        private bool Ready = false;

        private CCSpriteSheet spriteSheet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public StartLayer(int width, int height) : base(CCColor4B.AliceBlue)
        {
            Color = Constants.midBlue;
            Height = height;
            Width = width;

            spriteSheet = new CCSpriteSheet("static.plist");

#if (__IOS__)
            paddingTop = Constants.PaddingTop;
#endif

            statusButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Green")));
            statusButton.ContentSize = new CCSize(Width / 4, Height / 8);

            statusLabel = new CCLabel("Settings", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            statusLabel.Color = CCColor3B.Black;

            statusControlButton = new CCControlButton(statusLabel, statusButton);
            statusControlButton.PositionX = Width - statusControlButton.ContentSize.Width / 2 - Constants.hOffset;
            statusControlButton.PositionY = Height - statusControlButton.ContentSize.Height / 2 - Constants.vOffset - paddingTop;
            statusControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            statusControlButton.Clicked += StatusControlButton_Clicked;

            AddChild(statusControlButton);

            startButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blue")), new CCRect(10, 10, 10, 10));
            startButton.ContentSize = new CCSize(Width / 2, Height / 4);

            startLabel = new CCLabel("Begin Survey", Constants.LabelFont, Constants.ButtonStart, CCLabelFormat.SystemFont);
            startLabel.Color = CCColor3B.Black;

            startControlButton = new CCControlButton(startLabel, startButton);
            startControlButton.PositionX = Width / 2 - Constants.hOffset;
            startControlButton.PositionY = startControlButton.ContentSize.Height / 2 + (Constants.vOffset * 3);
            startControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            startControlButton.Clicked += StartControlButton_Clicked;

            AddChild(startControlButton);

            credentialLabel = new CCLabel("Status: Checking...", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            credentialLabel.PositionX = credentialLabel.ContentSize.Width / 2 + Constants.hOffset;
            credentialLabel.Color = CCColor3B.Red;
            credentialLabel.PositionY = Height - credentialLabel.ContentSize.Height / 2 - Constants.vOffset - paddingTop;
            credentialLabel.AnchorPoint = CCPoint.AnchorMiddle;

            AddChild(credentialLabel);

            // Begin startup check
            UpdateStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        private async void UpdateStatus()
        {
            string statusText = await ChallengeCredentials();

            UpdateLabelText(statusText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        private void UpdateLabelText(string text)
        {
            ScheduleOnce((dt) =>
            {
                Ready = (text.Trim().Equals("Status: Ready."));

                credentialLabel.Text = text;
                credentialLabel.PositionX = credentialLabel.ContentSize.Width / 2 + Constants.hOffset;
                credentialLabel.Color = (Ready) ? CCColor3B.Black : CCColor3B.Red;
            }, 0f);
        }

        /// <summary>
        /// 
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
                //UserDialogs.Instance.ShowLoading("Checking credentials ...", MaskType.Black);
            });

            return tcs.Task;
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
                    var action = await App.Current.MainPage.DisplayActionSheet("Existing credentials found", "Cancel", null, "Modify Credentials", "Delete Credentials");

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
        /// <param name="Server"></param>
        /// <param name="Key"></param>
        private async void SaveCredentials(string Server, string Key, string Id)
        {
            if (!string.IsNullOrWhiteSpace(Server) && !string.IsNullOrWhiteSpace(Key))
            {
                string ans = await ValidateSuppliedCredentials(Server, Key, Id);

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

            Device.BeginInvokeOnMainThread(() =>
            {
                var popup = new PopUpWindow(query, string.Empty, "OK", "Cancel");
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                    {
                        tcs.SetResult(new string[] { closedArgs.Text.Trim(), closedArgs.Text2.Trim() });
                    }
                    else
                    {
                        tcs.SetResult(new string[] { "", "" });
                    }
                };

                popup.Show();
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> ValidateSuppliedCredentials(string _address, string _key, string _id)
        {
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Checking credentials ...", MaskType.Black));

            string result = await await TestServerCredentials(_address, _key, _id)
                .ContinueWith(async t => await CloseUserDialog(t.Result));

            return result;
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
        private void StartControlButton_Clicked(object sender, EventArgs e)
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

            if (account != null)
            {
                if (CheckAccountContent())
                {                
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Invalid credentials found", "Cancel");
                    });

                    return;
                }
                else
                {
                    App.Token = account.Properties["Key"];
                    App.ApiAddress = account.Properties["Server"];
                    App.DeviceName = account.Properties["DeviceName"];

                    GameView.Director.PushScene(new CCTransitionFade(1.5f, new DemoScene(GameView, this.Scene)));
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No credentials found", "Cancel");
                });
            }
        }
    }
}
