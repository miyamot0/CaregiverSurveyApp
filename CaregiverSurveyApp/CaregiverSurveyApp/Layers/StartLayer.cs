using CocosSharp;
using CaregiverSurveyApp.Scenes;
using CaregiverSurveyApp.Values;
using System;
using Xamarin.Forms;
using CaregiverSurveyApp.Views;
using System.Threading.Tasks;
using Xamarin.Auth;
using System.Linq;

namespace CaregiverSurveyApp.Layers
{
    public class StartLayer : CCLayerColor
    {
        CCControlButton statusControlButton,
                        startControlButton;

        CCScale9Sprite statusButton,
                       startButton;

        CCLabel statusLabel,
                startLabel;

        int paddingTop = 0;

        private CCSpriteSheet spriteSheet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public StartLayer(int width, int height) : base(CCColor4B.AliceBlue)
        {
            Color = Constants.midBlue;

            spriteSheet = new CCSpriteSheet("static.plist");

#if (__IOS__)
            paddingTop = Constants.PaddingTop;
#endif

            statusButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Green")));
            statusButton.ContentSize = new CCSize(width / 4, height / 8);

            statusLabel = new CCLabel("Settings", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            statusLabel.Color = CCColor3B.Black;

            statusControlButton = new CCControlButton(statusLabel, statusButton);
            statusControlButton.PositionX = width - statusControlButton.ContentSize.Width / 2 - Constants.hOffset;
            statusControlButton.PositionY = height - statusControlButton.ContentSize.Height / 2 - Constants.vOffset - paddingTop;
            statusControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            statusControlButton.Clicked += StatusControlButton_Clicked;

            AddChild(statusControlButton);

            startButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blue")), new CCRect(10, 10, 10, 10));
            startButton.ContentSize = new CCSize(width / 2, height / 4);

            startLabel = new CCLabel("Begin Survey", Constants.LabelFont, Constants.ButtonStart, CCLabelFormat.SystemFont);
            startLabel.Color = CCColor3B.Black;

            startControlButton = new CCControlButton(startLabel, startButton);
            startControlButton.PositionX = width / 2 - Constants.hOffset;
            startControlButton.PositionY = startControlButton.ContentSize.Height / 2 + (Constants.vOffset * 3);
            startControlButton.AnchorPoint = CCPoint.AnchorMiddle;
            startControlButton.Clicked += StartControlButton_Clicked;

            AddChild(startControlButton);
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

                if (account != null)
                {
                    var action = await App.Current.MainPage.DisplayActionSheet("Existing credentials found", "Cancel", null, "Modify Credentials", "Delete Credentials");
                    
                    if (action != null && action == "Modify Credentials")
                    {
                        AccountStore.Create().Delete(account, App.AppName);

                        var result = await TextInputWindow("What are your credentials?");

                        SaveCredentials(result[0], result[1]);
                    }
                    else if (action != null && action == "Delete Credentials")
                    {
                        AccountStore.Create().Delete(account, App.AppName);
                    }
                }
                else
                {
                    var result = await TextInputWindow("What are your credentials?");

                    SaveCredentials(result[0], result[1]);
                }

            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Key"></param>
        private void SaveCredentials(string Server, string Key)
        {
            if (!string.IsNullOrWhiteSpace(Server) && !string.IsNullOrWhiteSpace(Key))
            {
                Account account = new Account
                {
                    Username = App.AppName
                };
                account.Properties.Add("Server", Server);
                account.Properties.Add("Key", Key);
                account.Properties.Add("DeviceName", Guid.NewGuid().ToString());
                AccountStore.Create().Save(account, App.AppName);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartControlButton_Clicked(object sender, EventArgs e)
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

            if (account != null)
            {
                if (string.IsNullOrWhiteSpace(account.Properties["Key"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["Server"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["DeviceName"]))
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

        private void CheckCredentials()
        {

        }
    }
}
