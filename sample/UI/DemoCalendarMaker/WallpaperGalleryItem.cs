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
    public partial class WallpaperGalleryItem : ListPanelItem
    {
        public WallpaperGalleryItem()
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
                return this.wallpaper.Image;
            }
            
            set
            {
                this.wallpaper.Image = value;
            }
        }
    }
}
