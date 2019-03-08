﻿/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    public partial class SampleGridListPanelItem : ListPanelItem
    {
        public SampleGridListPanelItem()
        {
            InitializeWidget();
        }
        
        public ImageAsset Image
        {
            get
            {
                return this.imageBox.Image;
            }
            set
            {
                this.imageBox.Image = value;
                this.imageBox.Visible = true;
            }
        }
    }
}
