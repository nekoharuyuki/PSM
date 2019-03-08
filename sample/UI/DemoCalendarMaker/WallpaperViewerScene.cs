/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using System.IO;
using System.Diagnostics;


namespace CalendarMaker
{
    public partial class WallpaperViewerScene : Scene
    {
        FullScreenPanel fullScreenPanel;
        List<ImageAsset> images;
       
        public WallpaperViewerScene()
        {
            InitializeWidget();

            GridListPanel_1.BackgroundColor = new UIColor(0f, 0f, 0f, 0f);

            backButton.ButtonAction += new EventHandler<TouchEventArgs>(backButtonExecuteAction);
            
            // init full screen wallpaper panel
            fullScreenPanel = new FullScreenPanel();
            fullScreenPanel.Visible = false;
            Matrix4 scaleMat = Matrix4.Scale(new Vector3(0.01f, 0.01f, 0.01f));
            Matrix4 baseMat = fullScreenPanel.Transform3D;
            Matrix4 mat;
            Matrix4.Multiply(ref baseMat, ref scaleMat, out mat);
            fullScreenPanel.Transform3D = mat;
            
            this.RootWidget.AddChildLast(fullScreenPanel);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            int height = (int)(GridListPanel_1.Height / 2.5f);
            int gap = height / 10;

            images = ImageManager.LoadCreatedImages();
            GridListPanel_1.ItemCount = images.Count;

            GridListPanel_1.ItemWidth = (height - 14) * UISystem.FramebufferWidth / UISystem.FramebufferHeight + 14;
            GridListPanel_1.ItemHeight = height;
            GridListPanel_1.ItemHorizontalGap = gap;
            GridListPanel_1.ItemVerticalGap = gap;

            GridListPanel_1.ItemCount = images.Count;
            GridListPanel_1.SetListItemCreator(GridListItemCreator);
            GridListPanel_1.SetListItemUpdater(GridListItemUpdater);
            GridListPanel_1.StartItemRequest();
        }

        protected override void OnHidden()
        {
            base.OnHidden();

            foreach (var image in images)
            {
                if (image != null) image.Dispose();
            }
        }
        
        private void GridListItemUpdater(ListPanelItem item)
        {
            Debug.Assert(item is WallpaperGalleryItem);
            var wgItem = item as WallpaperGalleryItem;
            if (wgItem != null && images != null && images.Count > 0)
            {
                wgItem.Image = images[item.Index];
            }
        }

        private ListPanelItem GridListItemCreator()
        {
            var item = new WallpaperGalleryItem();
            item.TouchEventReceived += new EventHandler<TouchEventArgs>(item_TouchEventReceived);
            return item;
        }

        void item_TouchEventReceived(object sender, TouchEventArgs e)
        {
            Debug.Assert(sender is WallpaperGalleryItem);

            if (e.TouchEvents.PrimaryTouchEvent.Type == TouchEventType.Up)
            {
                var item = sender as WallpaperGalleryItem;
                if (item != null)
                {
                    fullScreenPanel.WallpaperImage = item.Image;
                    fullScreenPanel.Visible = true;
                    fullScreenPanel.Show();
                }
            }
        }
        
        public Action ReturnMainScene;

        private void backButtonExecuteAction(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Debug.Assert(ReturnMainScene != null);
            ReturnMainScene();
        }
        
    }
}
