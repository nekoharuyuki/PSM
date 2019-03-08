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
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Imaging;


namespace UICatalog
{
    public partial class SliderScene : ContentsScene
    {
        public SliderScene()
        {
            InitializeWidget();

            this.Name = "Slider";

            // Slider1(Red)
            slider1.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderValueChangeAction1);
            slider1.ValueChangeEventEnabled = true;
            label1.Text = "R: " + slider1.Value.ToString();

            // Slider2(Green)
            slider2.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderValueChangeAction2);
            slider2.ValueChangeEventEnabled = true;
            label2.Text = "G: " + slider2.Value.ToString();

            // Slider3(Blue)
            slider3.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderValueChangeAction3);
            slider3.ValueChangeEventEnabled = true;
            label3.Text = "B: " + slider3.Value.ToString();

            // Slider4(Alpha)
            slider4.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderValueChangeAction4);
            slider4.ValueChangeEventEnabled = true;
            label4.Text = "A: " + slider4.Value.ToString();

            panel.BackgroundColor = new UIColor(
                slider1.Value / (float)slider1.MaxValue,
                slider2.Value / (float)slider2.MaxValue,
                slider3.Value / (float)slider3.MaxValue,
                slider4.Value / (float)slider4.MaxValue);
        }

        private void OnSliderValueChangeAction1(object sender, SliderValueChangeEventArgs e)
        {
            Slider slider = (sender as Slider);
            if (slider != null)
            {
                label1.Text = "R: " + slider.Value.ToString("F0");
                panel.BackgroundColor = new UIColor(
                    (float)slider.Value / (float)slider.MaxValue,
                    panel.BackgroundColor.G,
                    panel.BackgroundColor.B,
                    panel.BackgroundColor.A);
            }
        }

        private void OnSliderValueChangeAction2(object sender, SliderValueChangeEventArgs e)
        {
            Slider slider = (sender as Slider);
            if (slider != null)
            {
                label2.Text = "G: " + slider.Value.ToString("F0");
                panel.BackgroundColor = new UIColor(
                    panel.BackgroundColor.R,
                    (float)slider.Value / (float)slider.MaxValue,
                    panel.BackgroundColor.B,
                    panel.BackgroundColor.A);
            }
        }

        private void OnSliderValueChangeAction3(object sender, SliderValueChangeEventArgs e)
        {
            Slider slider = (sender as Slider);
            if (slider != null)
            {
                label3.Text = "B: " + slider.Value.ToString("F0");
                panel.BackgroundColor = new UIColor(
                    panel.BackgroundColor.R,
                    panel.BackgroundColor.G,
                    (float)slider.Value / (float)slider.MaxValue,
                    panel.BackgroundColor.A);
            }
        }

        private void OnSliderValueChangeAction4(object sender, SliderValueChangeEventArgs e)
        {
            Slider slider = (sender as Slider);
            if (slider != null)
            {
                label4.Text = "A: " + slider.Value.ToString("F0");
                panel.BackgroundColor = new UIColor(
                    panel.BackgroundColor.R,
                    panel.BackgroundColor.G,
                    panel.BackgroundColor.B,
                    (float)slider.Value / (float)slider.MaxValue);
            }
        }

    }
}
