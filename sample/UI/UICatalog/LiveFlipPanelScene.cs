/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class LiveFlipPanelScene : ContentsScene
    {

        public LiveFlipPanelScene()
        {
            InitializeWidget();

            this.Name = "LiveFlipPanel";

            // Button
            startButton.ButtonAction += new EventHandler<TouchEventArgs>(Button1ButtonAction);
            stopButton.ButtonAction += new EventHandler<TouchEventArgs>(Button2ButtonAction);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            if (liveFlipPanel.FrontPanel.Children.GetEnumerator().Current == null)
            {
                Panel panel1 = new Panel();
                panel1.SetPosition(0.0f, 0.0f);
                panel1.SetSize(liveFlipPanel.Width, liveFlipPanel.Height);
                panel1.BackgroundColor = new UIColor(0.0f, 0.0f, 0.0f ,1.0f);
                liveFlipPanel.FrontPanel.AddChildLast(panel1);

                ImageBox image1 = new ImageBox();
                image1.SetSize(liveFlipPanel.Width, liveFlipPanel.Height);
                image1.SetPosition(0.0f, 0.0f);
                image1.Image = new ImageAsset("/Application/assets/photo05.png");
                panel1.AddChildLast(image1);
                
                Label label1 = new Label();
                label1.SetPosition(10.0f, 10.0f);
                label1.SetSize(liveFlipPanel.Width, 50.0f);
                label1.Text = "Front Panel";
                label1.TextColor = new UIColor(1f, 1f, 0f, 1f);
                label1.HorizontalAlignment = HorizontalAlignment.Center;
                panel1.AddChildLast(label1);
            }

            if (liveFlipPanel.BackPanel.Children.GetEnumerator().Current == null)
            {
                Panel panel2 = new Panel();
                panel2.SetPosition(0.0f, 0.0f);
                panel2.SetSize(liveFlipPanel.Width, liveFlipPanel.Height);
                panel2.BackgroundColor = new UIColor(0.0f, 0.0f, 0.6f, 1.0f);
                liveFlipPanel.BackPanel.AddChildLast(panel2);

                Label label2 = new Label();
                label2.SetPosition(10.0f, 10.0f);
                label2.SetSize(liveFlipPanel.Width, 50.0f);
                label2.Text = "Back Panel";
                label2.TextColor = new UIColor(0f, 1f, 1f, 1f);
                label2.HorizontalAlignment = HorizontalAlignment.Center;
                panel2.AddChildLast(label2);
            }
        }

        private void Button1ButtonAction(object sender, TouchEventArgs e)
        {
            liveFlipPanel.Start();
        }

        private void Button2ButtonAction(object sender, TouchEventArgs e)
        {
            liveFlipPanel.Stop(true);
        }
    }
}
