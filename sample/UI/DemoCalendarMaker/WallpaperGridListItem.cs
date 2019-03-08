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

namespace CalendarMaker
{
    public delegate void CompleteModeAction();
    
    public partial class WallpaperGridListItem : ListPanelItem
    {
        public WallpaperGridListItem()
        {
            InitializeWidget();
            this.Clip = false;
            foreach (var item in this.Children)
            {
                item.TouchResponse = false;
            }
        }

        public ImageAsset Image
        {
            get
            {
                return this.ImageBox_1.Image;
            }
            
            set
            {
                this.ImageBox_1.Image = value;
            }
        }
    }
}