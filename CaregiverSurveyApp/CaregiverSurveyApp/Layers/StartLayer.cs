using CocosSharp;
using CaregiverSurveyApp.Scenes;
using CaregiverSurveyApp.Values;
using System;
using Xamarin.Forms;
using CaregiverSurveyApp.Views;
using System.Threading.Tasks;
using System.Diagnostics;

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
                var action = await App.Current.MainPage.DisplayActionSheet("Edit Settings",
                    "Cancel",
                    null,
                    "Edit Key",
                    "Edit Server Address");

                if (action != null)
                {
                    if (action == "Edit Key")
                    {
                        var result = await TextInputWindow("What is your key?");

                        Debug.WriteLine(result);
                    }
                    else if (action == "Edit Server Address")
                    {
                        var result = await TextInputWindow("What is your server address?");

                        Debug.WriteLine(result);
                    }
                }
            });
        }

        /// <summary>
        /// Await-able window for assigning icon label
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Task<string> TextInputWindow(string query)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(() =>
            {
                var popup = new PopUpWindow(query, string.Empty, "OK", "Cancel");
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == "OK" && closedArgs.Text.Trim().Length > 0)
                    {
                        tcs.SetResult(closedArgs.Text.Trim());
                    }
                    else
                    {
                        tcs.SetResult("");
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
            GameView.Director.PushScene(new CCTransitionFade(1.5f, new DemoScene(GameView, this.Scene)));
        }
    }
}
