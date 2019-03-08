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


namespace UICatalog
{
    public partial class ImageBoxScene : ContentsScene
    {
        static Panel[] panel = new Panel[(int)Enum.GetValues(typeof(ImageScaleType)).Length];
        static ImageBox[] image = new ImageBox[(int)Enum.GetValues(typeof(ImageScaleType)).Length];
        private bool isSliderInitialized = false;

        public ImageBoxScene()
        {
            InitializeWidget();

            this.Name = "ImageBox";

            // ImageBox2
            alphaImage.Alpha = 0.5f;

            panel[0] = panel1;
            image[0] = image1;
            panel[1] = panel2;
            image[1] = image2;
            panel[2] = panel3;
            image[2] = image3;
            panel[3] = panel4;
            image[3] = image4;
            panel[4] = panel5;
            image[4] = image5;

            // Slider Horizontal
            sliderH.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderHValueChangeAction);

            // Slider Vartical
            sliderV.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderVValueChangeAction);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            if (!isSliderInitialized)
            {
                sliderH.MaxValue = panel1.Width;
                sliderH.Value = panel1.Width;
                sliderV.MaxValue = panel1.Height;
                sliderV.Value = panel1.Height;
                isSliderInitialized = true;
            }
        }

        private static void OnSliderHValueChangeAction(object sender, SliderValueChangeEventArgs e)
        {
            for (int i = 0; i < Enum.GetValues(typeof(ImageScaleType)).Length; i++)
            {
                panel[i].Width = (float)((Slider)sender).Value;
                image[i].Width = (float)((Slider)sender).Value;
            }
        }

        private static void OnSliderVValueChangeAction(object sender, SliderValueChangeEventArgs e)
        {
            for (int i = 0; i < Enum.GetValues(typeof(ImageScaleType)).Length; i++)
            {
                panel[i].Height = (float)((Slider)sender).Value;
                image[i].Height = (float)((Slider)sender).Value;
            }
        }
    }
}
