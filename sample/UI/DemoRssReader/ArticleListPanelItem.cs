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
    public partial class ArticleListPanelItem : ListPanelItem
    {
        public ArticleListPanelItem()
        {
            InitializeWidget();

            this.HookChildTouchEvent = true;
            articleLabel.TouchResponse = true;
        }

        public string Text
        {
            get
            {
                return articleLabel.Text;
            }
            set
            {
                articleLabel.Text = value;
            }
        }

        public UIFont Font
        {
            get
            {
                return articleLabel.Font;
            }
            set
            {
                articleLabel.Font = value;
            }
        }

        public ImageAsset BackImage
        {
            get
            {
                return this.articleImageBox.Image;
            }
            set
            {
                this.articleImageBox.Image = value;
            }
        }
    }
}
