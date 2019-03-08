/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Weather
{
    public class CityButton : Button
    {
        private static Random rand = new Random();
        private Label cityNameLabel;
        private LiveSpringPanel imagePanel;

        public CityInfo cityInfo;

        public CityButton (CityInfo info)
        {
            this.SetSize(150, 150);
            cityInfo = info;
            
            // Init Label
            cityNameLabel = new Label();
            cityNameLabel.SetPosition(10, 5);
            cityNameLabel.SetSize(130, 25);
            cityNameLabel.BackgroundColor = new UIColor(232f / 255f, 192f / 255f, 120f / 255f, 192f / 255f);
            cityNameLabel.TextColor = new UIColor(72f / 255f, 40f / 255f, 16f / 255f, 255f / 255f);
            cityNameLabel.Font = new UIFont( FontAlias.System, 24, FontStyle.Regular);
            cityNameLabel.LineBreak = LineBreak.Character;
            cityNameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            cityNameLabel.Text = info.cityName;
            cityNameLabel.TouchResponse = false;
            TextShadowSettings textShadow_dateLabel = new TextShadowSettings();
            textShadow_dateLabel.Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f);
            textShadow_dateLabel.HorizontalOffset = 2;
            textShadow_dateLabel.VerticalOffset = 2;
            cityNameLabel.TextShadow = textShadow_dateLabel;
            this.AddChildLast(cityNameLabel);

            // Init Image box for weather icon
            ImageBox weatherIcon = new ImageBox();
            weatherIcon.PivotType = PivotType.MiddleCenter;
            weatherIcon.SetSize(100, 100);
            weatherIcon.SetPosition(50, 50);
            weatherIcon.Image = new ImageAsset("/Application/assets/" + info.weatherId + ".png");
            weatherIcon.TouchResponse = false;
            this.Style = ButtonStyle.Custom;
            CustomButtonImageSettings customImage = new CustomButtonImageSettings();
            customImage.BackgroundNormalImage = new ImageAsset("/Application/assets/city_normal.png");
            customImage.BackgroundPressedImage = new ImageAsset("/Application/assets/city_pressed.png");
            customImage.BackgroundDisabledImage = new ImageAsset("/Application/assets/city_normal.png");
            customImage.BackgroundNinePatchMargin = new NinePatchMargin(15, 15, 15, 15);
            this.CustomImage = customImage;

            // Init LiveSpring Panel
            imagePanel = new LiveSpringPanel();
            imagePanel.SetPosition(30, 40);
            imagePanel.SetSize(100, 100);
            imagePanel.TouchResponse = false;
            imagePanel.AddChildLast(weatherIcon);
            float springConst = (float)rand.NextDouble() / 10.0f + 0.8f;
            imagePanel.SetSpringConstant(weatherIcon, SpringType.PositionX, springConst);
            imagePanel.SetSpringConstant(weatherIcon, SpringType.PositionY, springConst);
            imagePanel.SetSpringConstant(weatherIcon, SpringType.PositionZ, 1.0f);
            springConst = (float)rand.NextDouble() / 10.0f + 0.725f;
            imagePanel.SetSpringConstant(weatherIcon, SpringType.AngleAxisX, springConst);
            imagePanel.SetSpringConstant(weatherIcon, SpringType.AngleAxisY, springConst);
            imagePanel.SetSpringConstant(weatherIcon, SpringType.AngleAxisZ, 0.6f);
            imagePanel.SetDampingConstant(null, SpringType.All, 0.005f);
            this.AddChildLast(imagePanel);
        }

        protected override void OnMotionEvent(MotionEvent motionEvent)
        {
            base.OnMotionEvent(motionEvent);

            if (imagePanel != null && imagePanel.ReflectSensorAcceleration)
            {
                float accelerationY = Motion.GetData(0).Acceleration.Y;
                imagePanel.AddAcceleraton(0.0f, FMath.Clamp(accelerationY, -1.0f, 1.0f), 0.0f);
            }
        }
    }
}

