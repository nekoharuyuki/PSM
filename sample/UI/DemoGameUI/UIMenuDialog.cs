/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    public partial class UIMenuDialog : Dialog
    {
        public UIMenuDialog()
        {
            InitializeWidget();

            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = null;

            this.ImageBox_background.Alpha = 0.8f;

            var tapGesture = new TapGestureDetector();
            tapGesture.TapDetected += menuContinueTapEventHandler;
            this.menuContinue.AddGestureDetector(tapGesture);
            this.menuContinue.Visible = false;

            tapGesture = new TapGestureDetector();
            tapGesture.TapDetected += menuReturnTapEventHandler;
            this.menuReturn.AddGestureDetector(tapGesture);
            this.menuReturn.Visible = false;

            tapGesture = new TapGestureDetector();
            tapGesture.TapDetected += menuRankingTapEventHandler;
            this.menuRanking.AddGestureDetector(tapGesture);
            this.menuRanking.Visible = false;

            tapGesture = new TapGestureDetector();
            tapGesture.TapDetected += menuHelpTapEventHandler;
            this.menuHelp.AddGestureDetector(tapGesture);
            this.menuHelp.Visible = false;

            this.Shown += new EventHandler(UIMenuDialog_Shown);
        }

        void menuContinueTapEventHandler(object sender, TapEventArgs e)
        {
            Globals.GameState = GameState.Play;
            this.Hide();
        }

        void menuRankingTapEventHandler(object sender, TapEventArgs e)
        {
            var dialog = new Dialog();
            dialog.BackgroundStyle = DialogBackgroundStyle.Custom;

            var panel = new UIRankingPanel();
            panel.SetSize(UISystem.FramebufferWidth, UISystem.FramebufferHeight);

            panel.ReturnButton.ButtonAction += (s, ea) =>
            {
                dialog.Hide();
            };

            dialog.SetSize(UISystem.FramebufferWidth, UISystem.FramebufferHeight);
            dialog.CustomBackgroundColor = new UIColor(1.0f, 1f, 1f, 0.6f);
            dialog.AddChildLast(panel);
            dialog.Show();
        }

        void menuHelpTapEventHandler(object sender, TapEventArgs e)
        {
            var dialog = new Dialog();
            dialog.BackgroundStyle = DialogBackgroundStyle.Custom;

            var panel = new UIHelpPanel();
            panel.SetSize(UISystem.FramebufferWidth, UISystem.FramebufferHeight);

            panel.ReturnButton.ButtonAction += (s, ea) =>
            {
                dialog.Hide();
            };


            dialog.SetSize(UISystem.FramebufferWidth, UISystem.FramebufferHeight);
            dialog.CustomBackgroundColor = new UIColor(1.0f, 1f, 1f, 0.6f);
            dialog.AddChildLast(panel);
            dialog.Show();
        }

        void menuReturnTapEventHandler(object sender, TapEventArgs e)
        {
            Globals.GameState = GameState.Stop;
            this.Hide();
            UISystem.SetScene(new UITopMenu());
        }

        void UIMenuDialog_Shown(object sender, EventArgs e)
        {
//            var delay = 100.0f;
//            DelayEffect.CreateAndStart(0, () =>
//            {
                SlideInEffect.CreateAndStart(menuContinue, 500, FourWayDirection.Right, SlideInEffectInterpolator.Elastic);
//            });

//            DelayEffect.CreateAndStart(delay, () =>
//            {
                SlideInEffect.CreateAndStart(menuReturn, 500, FourWayDirection.Left, SlideInEffectInterpolator.Elastic);
//            });

//            DelayEffect.CreateAndStart(delay * 2, () =>
//            {
                SlideInEffect.CreateAndStart(menuRanking, 500, FourWayDirection.Right, SlideInEffectInterpolator.Elastic);
//            });

//            DelayEffect.CreateAndStart(delay * 3, () =>
//            {
                SlideInEffect.CreateAndStart(menuHelp, 500, FourWayDirection.Left, SlideInEffectInterpolator.Elastic);
//            });
        }

        //public void PreShow()
        //{
        //    menuContinue.Visible = false;
        //    menuReturen.Visible = false;
        //    menuRnking.Visible = false;
        //    menuHelp.Visible = false;
        //}
    }
}
