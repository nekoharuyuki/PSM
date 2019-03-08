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
    public partial class PrecipitationPanel : Panel
    {
        public PrecipitationPanel()
        {
            InitializeWidget();
        }
        
        public PrecipitationPanel(CityInfo info, EventHandler<TouchEventArgs> closeDetailAction, EventHandler<TapEventArgs> flipAction)
        {
            InitializeWidget();
            
            // Init city information
            cityName.Text = info.cityName;
            weatherImage.Image = new ImageAsset("/Application/assets/" + info.weatherId + ".png");
            precipitation.Text = info.precipitation + "％";

            // Add close event
            closeButton.ButtonAction += new EventHandler<TouchEventArgs>(closeDetailAction);

            // Add jump flip Action
            TapGestureDetector singleTapForFlip = new TapGestureDetector();
            singleTapForFlip.TapDetected += flipAction;
            this.BgImage.AddGestureDetector(singleTapForFlip);
            
            weatherImage.TouchResponse = false;
            cityName.TouchResponse = false;
            curDate.TouchResponse = false;
            precipitation.TouchResponse = false;
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
