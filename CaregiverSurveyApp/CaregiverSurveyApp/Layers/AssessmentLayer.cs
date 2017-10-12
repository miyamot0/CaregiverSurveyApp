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
using ModernHttpClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Diagnostics;

namespace CaregiverSurveyApp.Layers
{
    /// <summary>
    /// 
    /// </summary>
    public class AssessmentLayer : CCLayerColor
    {
        CCControlButton backControlButton;

        CCScale9Sprite backButton,
                       bottomContainer,
                       ssrCard,
                       llrCard;

        CCLabel titleLabel,
                backLabel,
                instructionLabel,
                ssrText,
                llrText;

        CCScale9Sprite CurrentSpriteTouched = null;
        CCSpriteSheet spriteSheet;

        CCEventListenerTouchOneByOne mListener;

        double[] sendArray = new double[8];

        int Width,
            Height,
            CurrentSpriteType = -1,
            SSRValue = 50,
            LLRValue = 100,
            delayIncrement = 0,
            valueIncrement = 0,
            priorAdjustment = 0,
            paddingTop = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public AssessmentLayer(int width, int height) : base(CCColor4B.AliceBlue)
        {
            Color = Constants.midBlue;

            spriteSheet = new CCSpriteSheet("static.plist");

#if (__IOS__)
            paddingTop = Constants.PaddingTop;
#endif

            Width = width;
            Height = height;

            // Top Bar
            backButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blue")));
            backButton.ContentSize = new CCSize(width / 4, height / 8);
            backButton.Tag = (int)Constants.SpriteTags.None;

            backLabel = new CCLabel("Back", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            backLabel.Color = CCColor3B.Black;
            backLabel.Tag = (int)Constants.SpriteTags.None;

            backControlButton = new CCControlButton(backLabel, backButton);
            backControlButton.PositionX = Constants.hOffset;
            backControlButton.PositionY = height - backControlButton.ContentSize.Height / 2 - Constants.vOffset - paddingTop;
            backControlButton.AnchorPoint = CCPoint.AnchorMiddleLeft;
            backControlButton.Clicked += BackControlButton_Clicked;

            AddChild(backControlButton, 0, (int)Constants.SpriteTags.None);

            titleLabel = new CCLabel(String.Format(Constants.assessmentInstruction, Constants.delayStrings[delayIncrement]),
                Constants.LabelFont,
                Constants.LabelTitleSize,
                CCLabelFormat.SystemFont);
            titleLabel.Color = CCColor3B.Black;
            titleLabel.PositionX = width / 2;
            titleLabel.PositionY = height - paddingTop - Constants.vOffset * 2 - backControlButton.ContentSize.Height;
            titleLabel.AnchorPoint = CCPoint.AnchorMiddleTop;
            titleLabel.Tag = (int)Constants.SpriteTags.None;

            AddChild(titleLabel, 0, (int)Constants.SpriteTags.None);

            // White panel
            bottomContainer = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
            bottomContainer.CapInsets = new CCRect(20, 20, 20, 20);
            bottomContainer.ContentSize = new CCSize(width - Constants.hOffset * 2, (height / 3) - Constants.vOffset * 2);
            bottomContainer.PositionX = Constants.hOffset;
            bottomContainer.PositionY = Constants.vOffset;
            bottomContainer.AnchorPoint = CCPoint.AnchorLowerLeft;
            bottomContainer.Tag = (int)Constants.SpriteTags.None;

            AddChild(bottomContainer, 0, (int)Constants.SpriteTags.None);

            // Instructions
            instructionLabel = new CCLabel(Constants.assessmentInstruction, Constants.LabelFont, 26, CCLabelFormat.SystemFont);
            instructionLabel.Color = CCColor3B.Black;
            instructionLabel.Dimensions = new CCSize(width - Constants.hOffset * 2, height / 4);
            instructionLabel.ContentSize = new CCSize(width - Constants.hOffset * 2, height / 4);
            instructionLabel.LineBreak = CCLabelLineBreak.Word;
            instructionLabel.PositionX = Constants.hOffset;
            instructionLabel.PositionY = height - paddingTop - backButton.ContentSize.Height - titleLabel.ContentSize.Height - Constants.vOffset * 3;
            instructionLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            instructionLabel.Tag = (int)Constants.SpriteTags.None;

            AddChild(instructionLabel, 0, (int)Constants.SpriteTags.None);

            // Listener
            mListener = new CCEventListenerTouchOneByOne();
            mListener.IsSwallowTouches = true;
            mListener.OnTouchBegan = OnTouchBegan;
            mListener.OnTouchEnded = OnTouchesEnded;
            mListener.OnTouchMoved = HandleTouchesMoved;

            // Drag card
            llrCard = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
            llrCard.CapInsets = new CCRect(20, 20, 20, 20);
            llrCard.ContentSize = new CCSize(width / 3 - Constants.hOffset * 2, width / 3 - Constants.hOffset * 2);
            llrCard.PositionX = width - Constants.hOffset * 3 - (width / 3 - Constants.hOffset * 2) / 2;
            llrCard.PositionY = height -
                                paddingTop -
                                backButton.ContentSize.Height -
                                titleLabel.ContentSize.Height -
                                instructionLabel.ContentSize.Height -
                                llrCard.ContentSize.Height / 2 -
                                Constants.vOffset * 3;
            llrCard.AnchorPoint = CCPoint.AnchorMiddle;
            llrCard.Tag = (int)Constants.SpriteTags.LLR;

            llrText = new CCLabel(String.Format(Constants.assessmentTextLlr, Constants.delayStrings[delayIncrement]), Constants.LabelFont, 26, CCLabelFormat.SystemFont);
            llrText.Color = CCColor3B.Black;
            llrText.Dimensions = new CCSize(llrCard.ContentSize.Width - Constants.hOffset * 2, llrCard.ContentSize.Height - Constants.vOffset * 2);
            llrText.ContentSize = new CCSize(llrCard.ContentSize.Width - Constants.hOffset * 2, llrCard.ContentSize.Height - Constants.vOffset * 2);
            llrText.LineBreak = CCLabelLineBreak.Word;
            llrText.PositionX = Constants.hOffset;
            llrText.PositionY = Constants.vOffset;
            llrText.AnchorPoint = CCPoint.AnchorLowerLeft;
            llrText.HorizontalAlignment = CCTextAlignment.Center;
            llrText.VerticalAlignment = CCVerticalTextAlignment.Center;
            llrText.Tag = (int)Constants.SpriteTags.Text;

            llrCard.AddChild(llrText, 0, (int)Constants.SpriteTags.Text);

            AddEventListener(mListener.Copy(), llrCard);
            AddChild(llrCard, 1, (int)Constants.SpriteTags.LLR);

            ssrCard = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
            ssrCard.CapInsets = new CCRect(20, 20, 20, 20);
            ssrCard.ContentSize = new CCSize(width / 3 - Constants.hOffset * 2, width / 3 - Constants.hOffset * 2);
            ssrCard.PositionX = Constants.hOffset * 3 + (width / 3 - Constants.hOffset * 2) / 2;
            ssrCard.PositionY = height -
                                paddingTop -
                                backButton.ContentSize.Height -
                                titleLabel.ContentSize.Height -
                                instructionLabel.ContentSize.Height -
                                ssrCard.ContentSize.Height / 2 -
                                Constants.vOffset * 3;
            ssrCard.AnchorPoint = CCPoint.AnchorMiddle;
            ssrCard.Tag = (int)Constants.SpriteTags.SSR;

            ssrText = new CCLabel(String.Format(Constants.assessmentTextSsr, SSRValue), Constants.LabelFont, 26, CCLabelFormat.SystemFont);
            ssrText.Color = CCColor3B.Black;
            ssrText.Dimensions = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
            ssrText.ContentSize = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
            ssrText.LineBreak = CCLabelLineBreak.Word;
            ssrText.PositionX = Constants.hOffset;
            ssrText.PositionY = Constants.vOffset;
            ssrText.AnchorPoint = CCPoint.AnchorLowerLeft;
            ssrText.HorizontalAlignment = CCTextAlignment.Center;
            ssrText.VerticalAlignment = CCVerticalTextAlignment.Center;
            ssrText.Tag = (int)Constants.SpriteTags.Text;

            ssrCard.AddChild(ssrText, 0, (int)Constants.SpriteTags.Text);

            AddEventListener(mListener.Copy(), ssrCard);
            AddChild(ssrCard, 1, (int)Constants.SpriteTags.SSR);
        }

        /// <summary>
        /// Listener for active touch events
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        /// <returns></returns>
        bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            CurrentSpriteTouched = null;
            CurrentSpriteType = -1;

            CCScale9Sprite caller = touchEvent.CurrentTarget as CCScale9Sprite;

            if (caller != null)
            {
                if (caller.Tag == (int)Constants.SpriteTags.SSR && ssrCard.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    CurrentSpriteTouched = ssrCard;
                    CurrentSpriteType = caller.Tag;

                    CurrentSpriteTouched.Opacity = 100;

                    return true;
                }
                else if (caller.Tag == (int)Constants.SpriteTags.LLR && llrCard.BoundingBoxTransformedToWorld.ContainsPoint(touch.Location))
                {
                    CurrentSpriteTouched = llrCard;
                    CurrentSpriteType = caller.Tag;

                    CurrentSpriteTouched.Opacity = 100;

                    return true;
                }

                return false;

            }

            return false;
        }

        /// <summary>
        /// Listener for active touch events (end)
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        void OnTouchesEnded(CCTouch touch, CCEvent touchEvent)
        {
            if (CurrentSpriteTouched != null)
            {
                CurrentSpriteTouched.Opacity = 255;

                ssrCard.Opacity = 255;
                llrCard.Opacity = 255;
            }

            if (CurrentSpriteTouched != null && CheckIfInDropBox(CurrentSpriteTouched))
            {
                if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.SSR)
                {
                    CCSequence iconAnimationFocus = new CCSequence(
                        new CCDelayTime(0.1f),
                        new CCMoveTo(0.5f, GetLeftPosition()),
                        new CCCallFunc(() => {

                            CCLabel mContent;

                            // Update delay
                            if (valueIncrement >= 4)
                            {
                                sendArray[delayIncrement] = SSRValue - (priorAdjustment / 2);

                                delayIncrement++;

                                if (delayIncrement >= Constants.delayStrings.Length)
                                {
                                    SendData();

                                    return;
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        await App.Current.MainPage.DisplayAlert("The Wait Time Has Changed",
                                            "Right now you have an immediate choice and a choice in " + Constants.delayStrings[delayIncrement],
                                            "Okay");
                                    });
                                }

                                valueIncrement = 0;
                                SSRValue = 50;

                                mContent = ssrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;
                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);

                                CCLabel mContent2 = llrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;
                                mContent2.Text = String.Format(Constants.assessmentTextLlr, Constants.delayStrings[delayIncrement]);

                                titleLabel.Text = String.Format(Constants.assessmentInstruction, Constants.delayStrings[delayIncrement]);

                                CurrentSpriteTouched = null;

                                return;
                            }

                            mContent = ssrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;

                            if (valueIncrement == 0)
                            {
                                valueIncrement++;
                                priorAdjustment = LLRValue / 2;

                                SSRValue = SSRValue - priorAdjustment;

                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);
                            }
                            else
                            {
                                valueIncrement++;
                                priorAdjustment = priorAdjustment / 2;

                                SSRValue = SSRValue - priorAdjustment;

                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);
                            }

                            CurrentSpriteTouched = null;
                        }));

                    CurrentSpriteTouched.AddAction(iconAnimationFocus);
                }
                else if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.LLR)
                {

                    CCSequence iconAnimationFocus = new CCSequence(
                        new CCDelayTime(0.1f),
                        new CCMoveTo(0.5f, GetRightPosition()),
                        new CCCallFunc(() => {

                            CCLabel mContent;

                            // Has responded 5x at this delay, advance or submit
                            if (valueIncrement >= 4)
                            {
                                sendArray[delayIncrement] = SSRValue + (priorAdjustment / 2);

                                delayIncrement++;

                                if (delayIncrement >= Constants.delayStrings.Length)
                                {
                                    SendData();

                                    return;
                                }
                                else
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        await App.Current.MainPage.DisplayAlert("Wait time changed",
                                            "Right now you have an immediate choice and a choice in " + Constants.delayStrings[delayIncrement],
                                            "Okay");
                                    });
                                }

                                valueIncrement = 0;

                                SSRValue = 50;

                                mContent = ssrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;
                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);

                                CCLabel mContent2 = llrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;
                                mContent2.Text = String.Format(Constants.assessmentTextLlr, Constants.delayStrings[delayIncrement]);

                                titleLabel.Text = String.Format(Constants.assessmentInstruction, Constants.delayStrings[delayIncrement]);

                                CurrentSpriteTouched = null;

                                return;
                            }

                            mContent = ssrCard.GetChildByTag((int)Constants.SpriteTags.Text) as CCLabel;

                            if (valueIncrement == 0)
                            {
                                valueIncrement++;
                                priorAdjustment = LLRValue / 2;

                                SSRValue = SSRValue + priorAdjustment;

                                SSRValue = (SSRValue > 100) ? 100 : SSRValue;

                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);
                            }
                            else
                            {
                                valueIncrement++;
                                priorAdjustment = priorAdjustment / 2;

                                SSRValue = SSRValue + priorAdjustment;

                                SSRValue = (SSRValue > 100) ? 100 : SSRValue;

                                mContent.Text = String.Format(Constants.assessmentTextSsr, SSRValue);
                            }

                            CurrentSpriteTouched = null;
                        }));

                    CurrentSpriteTouched.AddAction(iconAnimationFocus);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<bool> SendDataToServer()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var httpClient = new HttpClient(new NativeMessageHandler(
                        throwOnCaptiveNetwork: true,
                        customSSLVerification: true
                    ));                                     

                    var mId = string.Format("{0}-{1}",
                        App.DeviceName,
                        App.SubmissionCounter.ToString("0000000000"));

                    var parameters = new Dictionary<string, string>
                    {
                        { "token", App.Token },
                        { "content", "record"},
                        { "format", "xml" },
                        { "type", "flat"},
                        { "overwriteBehavior", "normal" },
                        { "data", ConstructResponse(mId, sendArray) },
                        { "returnContent", "count" },
                        { "returnFormat", "json" }
                    };

                    var encodedContent = new FormUrlEncodedContent(parameters);

                    var resp = await httpClient.PostAsync(new Uri(App.ApiAddress), encodedContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        App.SubmissionCounter = App.SubmissionCounter + 1;

                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetResult(false);
                    }
                }
                catch
                {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
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

            System.Diagnostics.Debug.WriteLine(sb.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Upload task to REDcap
        /// </summary>
        /// <returns></returns>
        private Task<bool> UploadData()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                using (var progress = UserDialogs.Instance.Loading("Saving data ...", null, null, true, MaskType.Black))
                {
                    bool result = await SendDataToServer();

                    // Double check for sending
                    if (!result)
                    {
                        result = await SendDataToServer();
                    }

                    // Triple check for sending
                    if (!result)
                    {
                        result = await SendDataToServer();
                    }

                    await Task.Delay(2000);

                    tcs.SetResult(result);
                }
            });


            return tcs.Task;
        }

        /// <summary>
        /// Send data, return back to thread
        /// </summary>
        private async void SendData()
        {
            bool finalResult = await UploadData();

            if (finalResult && GameView.Director.CanPopScene)
            {
                if ((Scene as AssessmentScene) != null)
                {
                    (Scene as AssessmentScene).PopBackHome();
                }
            }
            else
            {
                Debug.WriteLine("Failed");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        CCPoint GetLeftPosition()
        {
            return new CCPoint(Constants.hOffset * 3 + (Width / 3 - Constants.hOffset * 2) / 2,
                    Height - paddingTop - backButton.ContentSize.Height - titleLabel.ContentSize.Height - instructionLabel.ContentSize.Height - ssrCard.ContentSize.Height / 2 - Constants.vOffset * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        CCPoint GetRightPosition()
        {
            return new CCPoint(Width - Constants.hOffset * 3 - (Width / 3 - Constants.hOffset * 2) / 2,
                    Height - paddingTop - backButton.ContentSize.Height - titleLabel.ContentSize.Height - instructionLabel.ContentSize.Height - ssrCard.ContentSize.Height / 2 - Constants.vOffset * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        void HandleTouchesMoved(CCTouch touch, CCEvent touchEvent)
        {
            if (CurrentSpriteTouched != null)
            {
                var pos = touch.Location;

                pos.X = (pos.X < 0 + CurrentSpriteTouched.ContentSize.Width / 2) ?
                    0 + CurrentSpriteTouched.ContentSize.Width / 2 :
                    pos.X;

                pos.Y = (pos.Y < 0 + CurrentSpriteTouched.ContentSize.Height / 2) ?
                    0 + CurrentSpriteTouched.ContentSize.Height / 2 :
                    pos.Y;

                pos.X = (pos.X > Width - CurrentSpriteTouched.ContentSize.Width / 2) ?
                    Width - CurrentSpriteTouched.ContentSize.Width / 2 :
                    pos.X;

                pos.Y = (pos.Y > Height - CurrentSpriteTouched.ContentSize.Height / 2) ?
                    Height - CurrentSpriteTouched.ContentSize.Height / 2 :
                    pos.Y;

                CurrentSpriteTouched.Position = pos;                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="CurrentSpriteTouched"></param>
        /// <returns></returns>
        bool CheckIfInDropBox(CCScale9Sprite CurrentSpriteTouched)
        {
            if (CurrentSpriteTouched.Position.Y > 0 &&
                CurrentSpriteTouched.Position.Y < (((Height / 3) + Constants.vOffset) - CurrentSpriteTouched.ContentSize.Height / 2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackControlButton_Clicked(object sender, EventArgs e)
        {
            if (GameView.Director.CanPopScene)
            {
                if ((Scene as AssessmentScene) != null)
                {
                    (Scene as AssessmentScene).PopBackHome();
                }
            }
        }
    }
}
