﻿//----------------------------------------------------------------------------------------------
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

namespace CaregiverSurveyApp.Layers
{
    /// <summary>
    /// 
    /// </summary>
    public class DemoLayer : CCLayerColor
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

        CCTextureCache Cache = CCTextureCache.SharedTextureCache;

        CCScale9Sprite CurrentSpriteTouched = null;

        CCEventListenerTouchOneByOne mListener;

        int CurrentSpriteType = -1,
            choiceTrial = 1;

        private CCSpriteSheet spriteSheet;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public DemoLayer(int width, int height) : base(CCColor4B.AliceBlue)
        {
            App.AssessScene = new AssessmentScene(App.GameView);
            spriteSheet = new CCSpriteSheet("static.plist");

            CCTextureCache.SharedTextureCache.AddImage("static.png");
            CCSprite starLoad = new CCSprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("HappyStar")).Texture);

            Color = Constants.midBlue;

            // Top Bar
            backButton = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("Blue")));
            backButton.ContentSize = new CCSize(width / 4, height / 8);
            backButton.Tag = (int)Constants.SpriteTags.None;

            backLabel = new CCLabel("Back", Constants.LabelFont, Constants.ButtonNormal, CCLabelFormat.SystemFont);
            backLabel.Color = CCColor3B.Black;
            backLabel.Tag = (int)Constants.SpriteTags.None;

            backControlButton = new CCControlButton(backLabel, backButton);
            backControlButton.PositionX = Constants.hOffset;
            backControlButton.PositionY = height - backControlButton.ContentSize.Height / 2 - Constants.vOffset;
            backControlButton.AnchorPoint = CCPoint.AnchorMiddleLeft;
            backControlButton.Clicked += BackControlButton_Clicked;

            AddChild(backControlButton, 0, (int)Constants.SpriteTags.None);

            titleLabel = new CCLabel("Practice Trial (" + choiceTrial + "/5)",
                Constants.LabelFont,
                Constants.LabelTitleSize,
                CCLabelFormat.SystemFont);
            titleLabel.Color = CCColor3B.Black;
            titleLabel.PositionX = width / 2;
            titleLabel.PositionY = height - Constants.vOffset * 2 - backControlButton.ContentSize.Height;
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
            instructionLabel = new CCLabel(Constants.demoInstruction, Constants.LabelFont, Constants.InstructionTextSize, CCLabelFormat.SystemFont);
            instructionLabel.Color = CCColor3B.Black;
            instructionLabel.Dimensions = new CCSize(width - Constants.hOffset * 2, height / 4);
            instructionLabel.ContentSize = new CCSize(width - Constants.hOffset * 2, height / 4);
            instructionLabel.LineBreak = CCLabelLineBreak.Word;
            instructionLabel.PositionX = Constants.hOffset;
            instructionLabel.PositionY = height - backButton.ContentSize.Height - titleLabel.ContentSize.Height - Constants.vOffset * 3;
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
                                backButton.ContentSize.Height -
                                titleLabel.ContentSize.Height -
                                instructionLabel.ContentSize.Height -
                                llrCard.ContentSize.Height / 2 -
                                Constants.vOffset * 3;
            llrCard.AnchorPoint = CCPoint.AnchorMiddle;
            llrCard.Tag = (int)Constants.SpriteTags.LLR;

            llrText = new CCLabel(Constants.demoTextLlr, Constants.LabelFont, Constants.TextReadingSize, CCLabelFormat.SystemFont);
            llrText.Color = CCColor3B.Black;
            llrText.Dimensions = new CCSize(llrCard.ContentSize.Width - Constants.hOffset * 2, llrCard.ContentSize.Height - Constants.vOffset * 2);
            llrText.ContentSize = new CCSize(llrCard.ContentSize.Width - Constants.hOffset * 2, llrCard.ContentSize.Height - Constants.vOffset * 2);
            llrText.LineBreak = CCLabelLineBreak.Word;
            llrText.PositionX = Constants.hOffset;
            llrText.PositionY = Constants.vOffset;
            llrText.AnchorPoint = CCPoint.AnchorLowerLeft;
            llrText.HorizontalAlignment = CCTextAlignment.Center;
            llrText.VerticalAlignment = CCVerticalTextAlignment.Center;
            llrText.Tag = (int)Constants.SpriteTags.None;

            llrCard.AddChild(llrText, 0, (int)Constants.SpriteTags.None);

            AddEventListener(mListener.Copy(), llrCard);
            AddChild(llrCard, 1, (int)Constants.SpriteTags.LLR);

            ssrCard = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
            ssrCard.CapInsets = new CCRect(20, 20, 20, 20);
            ssrCard.ContentSize = new CCSize(width / 3 - Constants.hOffset * 2, width / 3 - Constants.hOffset * 2);
            ssrCard.PositionX = Constants.hOffset * 3 + (width / 3 - Constants.hOffset * 2) / 2;
            ssrCard.PositionY = height -
                                backButton.ContentSize.Height -
                                titleLabel.ContentSize.Height -
                                instructionLabel.ContentSize.Height -
                                ssrCard.ContentSize.Height / 2 -
                                Constants.vOffset * 3;
            ssrCard.AnchorPoint = CCPoint.AnchorMiddle;
            ssrCard.Tag = (int)Constants.SpriteTags.SSR;

            ssrText = new CCLabel(Constants.demoTextSsr, Constants.LabelFont, Constants.TextReadingSize, CCLabelFormat.SystemFont);
            ssrText.Color = CCColor3B.Black;
            ssrText.Dimensions = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
            ssrText.ContentSize = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
            ssrText.LineBreak = CCLabelLineBreak.Word;
            ssrText.PositionX = Constants.hOffset;
            ssrText.PositionY = Constants.vOffset;
            ssrText.AnchorPoint = CCPoint.AnchorLowerLeft;
            ssrText.HorizontalAlignment = CCTextAlignment.Center;
            ssrText.VerticalAlignment = CCVerticalTextAlignment.Center;
            ssrText.Tag = (int)Constants.SpriteTags.None;

            ssrCard.AddChild(ssrText, 0, (int)Constants.SpriteTags.None);

            AddEventListener(mListener.Copy(), ssrCard);
            AddChild(ssrCard, 1, (int)Constants.SpriteTags.SSR);

            VisualEffect(new CCPoint(-App.Width, -App.Height));
        }

        /// <summary>
        /// Active interaction listener
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
        /// Active interaction (end) listener
        /// </summary>
        /// <param name="touch"></param>
        /// <param name="touchEvent"></param>
        void OnTouchesEnded(CCTouch touch, CCEvent touchEvent)
        {
            if (CurrentSpriteTouched != null)
            {
                CurrentSpriteTouched.Opacity = 255;

                // In case both were activated
                ssrCard.Opacity = 255;
                llrCard.Opacity = 255;
            }

            if (CurrentSpriteTouched != null && CheckIfInDropBox(CurrentSpriteTouched))
            {
                if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.SSR)
                {
                    CCSequence iconAnimationFocus = new CCSequence(
                        new CCDelayTime(0.1f),
                        new CCScaleTo(0.5f, 0.1f),
                        new CCCallFunc(() => {
                            CCAudioEngine.SharedEngine.PlayEffect("Sounds/Success");

                            VisualEffect(CurrentSpriteTouched.Position);

                            CurrentSpriteTouched.RemoveEventListener(mListener);
                            CurrentSpriteTouched.RemoveFromParent(true);

                            CurrentSpriteTouched = null;

                            PrepForNextTrial();
                        }));

                    CurrentSpriteTouched.AddAction(iconAnimationFocus);

                }
                else if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.LLR)
                {
                    if (choiceTrial % 2 == 0)
                    {
                        CCSequence iconAnimationFocus = new CCSequence(
                            new CCDelayTime(0.1f),
                            new CCMoveTo(0.5f, GetLeftPosition()),
                            new CCCallFunc(() => {
                                CurrentSpriteTouched.Color = CCColor3B.White;

                                CurrentSpriteTouched = null;
                            }));

                        CurrentSpriteTouched.AddAction(iconAnimationFocus);
                    }
                    else
                    {
                        CCSequence iconAnimationFocus = new CCSequence(
                            new CCDelayTime(0.1f),
                            new CCMoveTo(0.5f, GetRightPosition()),
                            new CCCallFunc(() => {
                                CurrentSpriteTouched.Color = CCColor3B.White;

                                CurrentSpriteTouched = null;
                            }));

                        CurrentSpriteTouched.AddAction(iconAnimationFocus);
                    }
                }
            }
        }
        
        /// <summary>
        /// Prepare materials for the following trial
        /// </summary>
        void PrepForNextTrial()
        {
            choiceTrial++;

            if (choiceTrial > 5)
            {
                AddActions(false, new CCSequence(
                    new CCDelayTime(2f),
                    new CCCallFuncO((dt) => 
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Current.MainPage.DisplayAlert("Practice Tests Complete", "In the next part, you will be asked to choose between one outcome right away and another that would come later", "I'm Ready");
                        });
                    }, App.GameView),
                    new CCCallFuncO((dt) =>
                    {
                        GameView.Director.ReplaceScene(new CCTransitionFade(1.5f, App.AssessScene));
                    }, App.GameView)));
            }
            else
            {
                titleLabel.Text = "Practice Trial (" + choiceTrial + "/5)";

                if (choiceTrial % 2 == 0)
                {
                    CCSequence iconAnimationFocus = new CCSequence(
                        new CCDelayTime(0.1f),
                        new CCMoveTo(0.5f, GetLeftPosition()),
                        new CCCallFunc(() =>
                        {
                            CCPoint mNewLocation = GetRightPosition();

                            ssrCard = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
                            ssrCard.CapInsets = new CCRect(20, 20, 20, 20);
                            ssrCard.ContentSize = new CCSize(App.Width / 3 - Constants.hOffset * 2, App.Width / 3 - Constants.hOffset * 2);
                            ssrCard.PositionX = mNewLocation.X;
                            ssrCard.PositionY = mNewLocation.Y;
                            ssrCard.AnchorPoint = CCPoint.AnchorMiddle;
                            ssrCard.Tag = (int)Constants.SpriteTags.SSR;

                            ssrText = new CCLabel(Constants.demoTextSsr, Constants.LabelFont, Constants.TextReadingSize, CCLabelFormat.SystemFont);
                            ssrText.Color = CCColor3B.Black;
                            ssrText.Dimensions = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
                            ssrText.ContentSize = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
                            ssrText.LineBreak = CCLabelLineBreak.Word;
                            ssrText.PositionX = Constants.hOffset;
                            ssrText.PositionY = Constants.vOffset;
                            ssrText.AnchorPoint = CCPoint.AnchorLowerLeft;
                            ssrText.HorizontalAlignment = CCTextAlignment.Center;
                            ssrText.VerticalAlignment = CCVerticalTextAlignment.Center;
                            ssrText.Tag = (int)Constants.SpriteTags.None;

                            ssrCard.AddChild(ssrText, 0, (int)Constants.SpriteTags.None);

                            AddEventListener(mListener.Copy(), ssrCard);
                            AddChild(ssrCard, 1, (int)Constants.SpriteTags.SSR);
                        }));

                    llrCard.AddAction(iconAnimationFocus);
                }
                else
                {
                    CCSequence iconAnimationFocus = new CCSequence(
                        new CCDelayTime(0.1f),
                        new CCMoveTo(0.5f, GetRightPosition()),
                        new CCCallFunc(() =>
                        {
                            CCPoint mNewLocation = GetLeftPosition();

                            ssrCard = new CCScale9Sprite(spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("White")));
                            ssrCard.CapInsets = new CCRect(20, 20, 20, 20);
                            ssrCard.ContentSize = new CCSize(App.Width / 3 - Constants.hOffset * 2, App.Width / 3 - Constants.hOffset * 2);
                            ssrCard.PositionX = mNewLocation.X;
                            ssrCard.PositionY = mNewLocation.Y;
                            ssrCard.AnchorPoint = CCPoint.AnchorMiddle;
                            ssrCard.Tag = (int)Constants.SpriteTags.SSR;

                            ssrText = new CCLabel(Constants.demoTextSsr, Constants.LabelFont, Constants.TextReadingSize, CCLabelFormat.SystemFont);
                            ssrText.Color = CCColor3B.Black;
                            ssrText.Dimensions = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
                            ssrText.ContentSize = new CCSize(ssrCard.ContentSize.Width - Constants.hOffset * 2, ssrCard.ContentSize.Height - Constants.vOffset * 2);
                            ssrText.LineBreak = CCLabelLineBreak.Word;
                            ssrText.PositionX = Constants.hOffset;
                            ssrText.PositionY = Constants.vOffset;
                            ssrText.AnchorPoint = CCPoint.AnchorLowerLeft;
                            ssrText.HorizontalAlignment = CCTextAlignment.Center;
                            ssrText.VerticalAlignment = CCVerticalTextAlignment.Center;
                            ssrText.Tag = (int)Constants.SpriteTags.None;

                            ssrCard.AddChild(ssrText, 0, (int)Constants.SpriteTags.None);

                            AddEventListener(mListener.Copy(), ssrCard);
                            AddChild(ssrCard, 1, (int)Constants.SpriteTags.SSR);
                        }));

                    llrCard.AddAction(iconAnimationFocus);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        CCPoint GetLeftPosition()
        {
            return new CCPoint(Constants.hOffset * 3 + (App.Width / 3 - Constants.hOffset * 2) / 2,
                    App.Height - backButton.ContentSize.Height - titleLabel.ContentSize.Height - instructionLabel.ContentSize.Height -
                                ssrCard.ContentSize.Height / 2 - Constants.vOffset * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        CCPoint GetRightPosition()
        {
            return new CCPoint(App.Width - Constants.hOffset * 3 - (App.Width / 3 - Constants.hOffset * 2) / 2,
                    App.Height - backButton.ContentSize.Height - titleLabel.ContentSize.Height - instructionLabel.ContentSize.Height -
                                ssrCard.ContentSize.Height / 2 - Constants.vOffset * 3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        void VisualEffect(CCPoint pos)
        {
            CCParticleExplosion explosion = new CCParticleExplosion(pos);
            explosion.TotalParticles = 6;
            explosion.AutoRemoveOnFinish = true;

            explosion.Texture = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("HappyStar")).Texture;
            explosion.TextureRect = spriteSheet.Frames.Find((x) => x.TextureFilename.Contains("HappyStar")).TextureRectInPixels;

            explosion.StartColor = new CCColor4F(CCColor4B.Yellow);
            explosion.EndColor = new CCColor4F(CCColor4B.Yellow);

            explosion.StartSpin = 0f;
            explosion.EndSpin = 360f;

            explosion.StartSize = 100f;
            explosion.EndSize = 20f;

            explosion.Gravity = new CCPoint(0, -150);

            explosion.Speed = 200f;

            AddChild(explosion);
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

                pos.X = (pos.X > App.Width - CurrentSpriteTouched.ContentSize.Width / 2) ?
                    App.Width - CurrentSpriteTouched.ContentSize.Width / 2 :
                    pos.X;

                pos.Y = (pos.Y > App.Height - CurrentSpriteTouched.ContentSize.Height / 2) ?
                    App.Height - CurrentSpriteTouched.ContentSize.Height / 2 :
                    pos.Y;

                CurrentSpriteTouched.Position = pos;

                if (CheckIfInDropBox(CurrentSpriteTouched))
                {
                    if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.SSR)
                    {
                        CurrentSpriteTouched.Color = CCColor3B.Green;
                    }
                    else if (CurrentSpriteTouched.Tag == (int)Constants.SpriteTags.LLR)
                    {
                        CurrentSpriteTouched.Color = CCColor3B.Red;
                    }
                }
                else
                {
                    CurrentSpriteTouched.Color = CCColor3B.White;
                }
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
                CurrentSpriteTouched.Position.Y < (((App.Height / 3) + Constants.vOffset) - CurrentSpriteTouched.ContentSize.Height / 2))
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
            if ((Scene as DemoScene) != null)
            {
                (Scene as DemoScene).PopBackHome();

                CCTextureCache.SharedTextureCache.RemoveAllTextures();
            }
        }
    }
}
