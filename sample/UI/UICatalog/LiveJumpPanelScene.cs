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
    public partial class LiveJumpPanelScene : ContentsScene
    {
        private ImageAsset[] assets;
        
        public LiveJumpPanelScene()
        {
            InitializeWidget();

            this.Name = "LiveJumpPanel";

            // live jump panel
            liveJumpPanel.TouchEventReceived += new EventHandler<TouchEventArgs>(JPanelTouchEventReceived);

            assets = new ImageAsset[6];
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = new ImageAsset("/Application/assets/photo" + (i + 1).ToString("00") + "_s.png");
            }

        }

        protected override void OnShowing()
        {
            base.OnShowing();

            if (liveJumpPanel.Children.GetEnumerator().Current == null)
            {
                float hOffset = 30f;
                float vOffset = 20f;
                float width = (liveJumpPanel.Width - hOffset) / assets.Length - hOffset;
                float height = (liveJumpPanel.Height - vOffset) / 5 - vOffset;
    
                for (int i = 0; i < 30; i++)
                {
                    ImageBox image = new ImageBox();
                    image.X = hOffset + ((width + hOffset) * (i % assets.Length));
                    image.Y = vOffset + ((height + vOffset) * (int)(i / assets.Length));
                    image.Width = width;
                    image.Height = height;
                    image.Image = assets[i % assets.Length];
                    image.TouchResponse = false;
                    liveJumpPanel.AddChildLast(image);
                }
            }
        }

        private void JPanelTouchEventReceived(object sender, TouchEventArgs e)
        {
            liveJumpPanel.Jump(e.TouchEvents.PrimaryTouchEvent.LocalPosition.X, e.TouchEvents.PrimaryTouchEvent.LocalPosition.Y);
        }

    }
}
