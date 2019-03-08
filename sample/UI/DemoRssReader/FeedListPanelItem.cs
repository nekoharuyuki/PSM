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

namespace RssApp
{
    public partial class FeedListPanelItem : ListPanelItem
    {
        public FeedListPanelItem()
        {
            InitializeWidget();

            this.HookChildTouchEvent = true;
            feedLabel.TouchResponse = true;
        }

        public string Text
        {
            get
            {
                return feedLabel.Text;
            }
            set
            {
                feedLabel.Text = value;
            }
        }

        public UIFont Font
        {
            get
            {
                return feedLabel.Font;
            }
            set
            {
                feedLabel.Font = value;
            }
        }

        public ImageAsset BackImage
        {
            set
            {
                this.feedImageBox.Image = value;
            }
        }
    }
}
