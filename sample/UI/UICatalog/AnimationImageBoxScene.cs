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
    public partial class AnimationImageBoxScene : ContentsScene
    {
        private static AnimationImageBox animImageBox;
        private const float sliderValueCoefficient = 10.0f;

        public AnimationImageBoxScene()
        {
            InitializeWidget();

            this.Name = "AnimationImage";

            // Start Button
            startButton.ButtonAction += new EventHandler<TouchEventArgs>(OnStartButtonExecuteAction);
            startButton.Enabled = false;

            // Stop Button
            stopButton.ButtonAction += new EventHandler<TouchEventArgs>(OnStopButtonExecuteAction);
            stopButton.Enabled = true;

            // Slider
            slider.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderValueChangeAction);
            slider.ValueChangeEventEnabled = true;

            // AnimationImageBox
            animImageBox = new AnimationImageBox();
            animImageBox.SetPosition(540, 250);
            animImageBox.SetSize(64, 64);
            animImageBox.Anchors = Anchors.Height | Anchors.Width;
            animImageBox.Visible = true;
            animImageBox.Image = new ImageAsset("/Application/assets/square_animation.png");
            animImageBox.FrameWidth = 64;
            animImageBox.FrameHeight = 64;
            animImageBox.FrameCount = 12;
            animImageBox.FrameInterval = (float)slider.Value / sliderValueCoefficient;
            animImageBox.Start();
            this.RootWidget.AddChildLast(animImageBox);

            labelInterval.Text = FormatValue();
        }

        private void OnStartButtonExecuteAction(object sender, TouchEventArgs e)
        {
            startButton.Enabled = false;
            animImageBox.Start();
            stopButton.Enabled = true;
        }

        private void OnStopButtonExecuteAction(object sender, TouchEventArgs e)
        {
            stopButton.Enabled = false;
            animImageBox.Stop();
            startButton.Enabled = true;
        }

        private void OnSliderValueChangeAction(object sender, SliderValueChangeEventArgs e)
        {
            Slider slider = (Slider)sender;

            if (startButton.Enabled == false)
            {
                animImageBox.Stop();
            }

            animImageBox.FrameInterval = (float)slider.Value / sliderValueCoefficient;
            labelInterval.Text = FormatValue();

            if (startButton.Enabled == false)
            {
                animImageBox.Start();
            }
        }

        private static string FormatValue()
        {
            return string.Format("Interval = {0:0.0} ms", animImageBox.FrameInterval);
        }
    }
}
