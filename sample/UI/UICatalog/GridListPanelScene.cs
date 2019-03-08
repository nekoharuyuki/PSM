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
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class GridListPanelScene : ContentsScene
    {
        static private int itemCount = 102;
        static private int imageAssetCount = 10;
        static private ImageAsset[] imageAssets;
        private bool isInitialized = false;
        
        public GridListPanelScene()
        {
            InitializeWidget();

            this.Name = "GridListPanel";

            // GridListPanel
            gridListPanel.ItemHorizontalGap = 10.0f;
            gridListPanel.ItemVerticalGap = 10.0f;
            gridListPanel.ItemCount = itemCount;
            gridListPanel.SnapScroll = true;
            gridListPanel.SetListItemUpdater(GridListItemUpdater);
            gridListPanel.BackgroundColor = new UIColor(0.0f, 0.0f, 0.3f, 1.0f);
        }
        
        static GridListPanelScene()
        {
            // ImageAsset
            imageAssets = new ImageAsset[imageAssetCount];
            for (int i = 0; i < imageAssetCount; i++)
            {
                string filename = "/Application/assets/photo" + (i + 1).ToString("00") + "_s.png";
                imageAssets[i] = new ImageAsset(filename);
            }
        }
        
        protected override void OnShowing()
        {
            base.OnShowing();

            if (!isInitialized)
            {
                gridListPanel.ItemHeight = (gridListPanel.Height - gridListPanel.ItemVerticalGap) / 6.0f - gridListPanel.ItemVerticalGap;
                gridListPanel.ItemWidth = gridListPanel.ItemHeight * 3 / 2;
                gridListPanel.StartItemRequest();
                isInitialized = true;
            }
        }
        
        static private void GridListItemUpdater(ListPanelItem item)
        {
            (item as SampleGridListPanelItem).Image = imageAssets[item.Index % imageAssetCount];
        }
    }
}
