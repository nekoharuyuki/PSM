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
    public partial class UITopMenu : Scene
    {
        double totalTime = 0.0;

        TapGestureDetector menuStartTapGesture;
        TapGestureDetector menuRankingTapGesture;
        TapGestureDetector menuHelpTapGesture;

        public UITopMenu()
        {
            InitializeWidget();

            menuStartTapGesture = new TapGestureDetector();
            menuStartTapGesture.TapDetected += menuStartTapEventHandler;
            menuStartBgTop.AddGestureDetector(menuStartTapGesture);
            menuStartBgTop.PriorityHit = true;
            menuStartTop.TouchResponse = false;

            menuRankingTapGesture = new TapGestureDetector();
            menuRankingTapGesture.TapDetected += menuRankingTapEventHandler;
            menuRankingBgTop.AddGestureDetector(menuRankingTapGesture);
            menuRankingBgTop.PriorityHit = true;
            menuRankingTop.TouchResponse = false;

            menuHelpTapGesture = new TapGestureDetector();
            menuHelpTapGesture.TapDetected += menuHelpTapEventHandler;
            menuHelpBgTop.AddGestureDetector(menuHelpTapGesture);
            menuRankingBgTop.PriorityHit = true;
            menuHelpTop.TouchResponse = false;
        }

        event EventHandler<TapEventArgs> MenuStartTap
        {
            add
            {
                menuStartTapGesture.TapDetected += value;
            }
            remove
            {
                menuStartTapGesture.TapDetected -= value;
            }
        }
        event EventHandler<TapEventArgs> MenuRankingTap
        {
            add
            {
                menuRankingTapGesture.TapDetected += value;
            }
            remove
            {
                menuRankingTapGesture.TapDetected -= value;
            }
        }
        event EventHandler<TapEventArgs> MenuHelpTap
        {
            add
            {
                menuHelpTapGesture.TapDetected += value;
            }
            remove
            {
                menuHelpTapGesture.TapDetected -= value;
            }
        }

        void menuStartTapEventHandler(object sender, TapEventArgs e)
        {
            Globals.GameState = GameState.Play;

            var next = new UIOverlayScene();

            var trans = new CrossFadeTransition(1000, CrossFadeTransitionInterpolator.EaseOutQuad);

            trans.NextSceneFront = false;

            UISystem.SetScene(next, trans);
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

        protected override void OnShown()
        {
            new SlideInEffect()
            {
                Widget = ImageBox_title2,
                MoveDirection = FourWayDirection.Left,
                Interpolator = SlideInEffectInterpolator.Elastic,
            }
            .Start();
            new SlideInEffect()
            {
                Widget = ImageBox_title1,
                MoveDirection = FourWayDirection.Right,
                Interpolator = SlideInEffectInterpolator.Elastic,
            }
            .Start();
        }

        protected override void OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            totalTime += elapsedTime;

            ImageBox_animal1.PivotType = PivotType.TopCenter;
            var mat = Matrix4.RotationZ((float)Math.Sin(totalTime / 300)/10f);
            mat.ColumnW = ImageBox_animal1.Transform3D.ColumnW;
            ImageBox_animal1.Transform3D = mat;


            ImageBox_animal2.PivotType = PivotType.BottomCenter;
            mat = Matrix4.RotationZ((float)Math.Sin(totalTime / 200) / 10f);
            mat.ColumnW = ImageBox_animal2.Transform3D.ColumnW;
            ImageBox_animal2.Transform3D = mat;

        }
    }
}
