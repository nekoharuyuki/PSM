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

namespace Weather
{
    public partial class TemperaturePanel : Panel
    {
        public TemperaturePanel()
        {
            InitializeWidget();
        }
        
        public TemperaturePanel(CityInfo info, EventHandler<TouchEventArgs> hideDetailAction, EventHandler<TapEventArgs> flipAction)
        {
            InitializeWidget();
            
            // Init city information
            cityName.Text = info.cityName;
            weatherImage.Image = new ImageAsset("/Application/assets/" + info.weatherId + ".png");
            lowTemperature.Text = "" + info.lowTemp + "℃";
            highTemperature.Text = "" + info.highTemp + "℃";

            // Add close event
            closeButton.ButtonAction += new EventHandler<TouchEventArgs>(hideDetailAction);

            // Add jump flip Action
            TapGestureDetector singleTapForFlip = new TapGestureDetector();
            singleTapForFlip.TapDetected += flipAction;
            this.BgImage.AddGestureDetector(singleTapForFlip);
            
            // Set each widget's Touchresponse
            weatherImage.TouchResponse = false;
            cityName.TouchResponse = false;
            curDate.TouchResponse = false;
            lowTemperature.TouchResponse = false;
            highTemperature.TouchResponse = false;
            diagonal.TouchResponse = false;
        }
        
        public string Date
        {
            set
            {
                curDate.Text = value;
            }
        }
    }
}
