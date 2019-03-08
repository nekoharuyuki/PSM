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
using System.Diagnostics;

namespace CalendarMaker
{
    public partial class WallpaperSelectionPanel : Panel
    {
        
        CompleteModeAction itemClickAction;

        ImageAsset selectedImage;
        List<ImageAsset> images;
        
        public WallpaperSelectionPanel(CompleteModeAction action)
        {
            InitializeWidget();
            GridListPanel_1.BackgroundColor = new UIColor(0f, 0f, 0f, 0f);

            itemClickAction = action;

            GridListPanel_1.SetListItemCreator(GridListItemCreator);
            GridListPanel_1.SetListItemUpdater(GridListItemUpdater);
        }

        public void Init()
        {
            images = ImageManager.LoadMaterialImages();

            int size = (int)(GridListPanel_1.Width / 3.5f);
            int gap = size / 10;

            GridListPanel_1.ItemWidth = size;
            GridListPanel_1.ItemHeight = size;
            GridListPanel_1.ItemHorizontalGap = gap;
            GridListPanel_1.ItemVerticalGap = gap;

            GridListPanel_1.ItemCount = images.Count;
            GridListPanel_1.StartItemRequest();
        }

        protected override void DisposeSelf()
        {
            base.DisposeSelf();

            foreach (var img in images)
            {
                if (img != null)
                {
                    img.Dispose();
                }
            }
        }

        private void GridListItemUpdater(ListPanelItem item)
        {
            Debug.Assert(item is WallpaperGridListItem);
            var wglitem = item as WallpaperGridListItem;
            if (wglitem != null)
            {
                wglitem.Image = images[item.Index];
            }
        }

        public GridListPanel WallpaperGrid
        {
            get{return this.GridListPanel_1;}
        }

        private ListPanelItem GridListItemCreator()
        {
            var item = new WallpaperGridListItem();
            item.TouchEventReceived += GridListItem_TouchEventReceived;
            return item;
        }

        void GridListItem_TouchEventReceived(object sender, TouchEventArgs e)
        {
            Debug.Assert(sender is WallpaperGridListItem);
            Debug.Assert(itemClickAction != null);

            if (e.TouchEvents.PrimaryTouchEvent.Type == TouchEventType.Up)
            {
                var item = sender as WallpaperGridListItem;
                if (item != null)
                {
                    selectedImage = item.Image;
                    if (itemClickAction != null)
                    {
                        itemClickAction();
                    }
                }
            }
        }
  
        public ImageAsset SelectedImage
        {
            get{return selectedImage;}
        }
        
    }
}
