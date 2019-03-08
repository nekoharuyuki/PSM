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
    public partial class WorldPanel : Panel
    {
        public WorldPanel()
        {
        }
        
        public WorldPanel(float width, float height, List<CityInfo> cityInfoList, EventHandler<TouchEventArgs> openDetailAction)
        {
            InitializeWidget();
            
            // Set Size
            this.SetSize(width, height);
            worldMap.SetSize(width, height);

            // Add city on live jump panel
            foreach(CityInfo info in cityInfoList)
            {
                CityButton city = new CityButton(info);
                city.X = info.locationX;
                city.Y = info.locationY;
                city.ButtonAction += new EventHandler<TouchEventArgs>(openDetailAction);
                this.AddChildLast(city);
            }
        }
    }
}
