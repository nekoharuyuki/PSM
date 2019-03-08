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
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class LiveListPanelScene : ContentsScene
    {
        static private int itemCount = 50;
        static private int imageAssetCount = 10;
        static private ImageAsset[] listItemImageAssets;

        public LiveListPanelScene()
        {
            InitializeWidget();

            this.Name = "LiveListPanel";

            // Setup ImageAsset
            listItemImageAssets = new ImageAsset[imageAssetCount];
            for (int i = 0; i < imageAssetCount; i++)
            {
                listItemImageAssets[i] = new ImageAsset("/Application/assets/list" + (i + 1).ToString("00") + ".png", true);
            }

            // LiveList
            liveListPanel.ItemCount = itemCount;
            liveListPanel.BackgroundColor = new UIColor(0.05f, 0.05f, 0.05f, 1.0f);
            liveListPanel.SetListItemCreator(ListItemCreator);
            liveListPanel.SetListItemUpdater(ListItemUpdator);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            liveListPanel.ItemHeight = liveListPanel.Height / 8;
            liveListPanel.ItemVerticalGap = liveListPanel.Height / 20;
        }

        private static ListPanelItem ListItemCreator()
        {
            return new MyLiveListPanelItem();
        }

        private static void ListItemUpdator(ListPanelItem item)
        {
            MyLiveListPanelItem myItem = (item as MyLiveListPanelItem);
            if (myItem != null)
            {
                myItem.Image = listItemImageAssets[myItem.Index % imageAssetCount];
            }
        }

        private class MyLiveListPanelItem : ListPanelItem
        {
            public MyLiveListPanelItem()
            {
                this.image = new ImageBox();
                this.AddChildLast(image);
            }

            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;

                    if (this.image != null)
                    {
                        this.image.Width = value;
                    }
                }
            }

            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;

                    if (this.image != null)
                    {
                        this.image.Height = value;
                    }
                }
            }

            public ImageAsset Image
            {
                get
                {
                    return this.image.Image;
                }
                set
                {
                    this.image.Image = value;
                }
            }

            private ImageBox image;
        }
    }
}
