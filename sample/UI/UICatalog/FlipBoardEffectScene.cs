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
    public partial class FlipBoardEffectScene : ContentsScene
    {
        ImageBox[] images = new ImageBox[2];
        ImageAsset[] assets = new ImageAsset[6];
        int assetIndex = 0;

        public FlipBoardEffectScene()
        {
            InitializeWidget();

            this.Name = "FlipBoardEffect";

            // Button
            buttonFlip.ButtonAction += new EventHandler<TouchEventArgs>(FlipButtonAction);

            // Asset
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = new ImageAsset(string.Format("/Application/assets/photo{0:00}.png", i + 1));
            }
            
            images[0] = image1;
            images[1] = image2;
        }

        private void FlipButtonAction(object sender, TouchEventArgs e)
        {
            var img = images[1];
            images[1] = images[0];
            images[0] = img;

            if (++assetIndex >= assets.Length)
            {
                assetIndex = 0;
            }
            images[1].Image = assets[assetIndex];

            images[0].Visible = true;
            images[1].Visible = true;
            FlipBoardEffect effect = new FlipBoardEffect(images[0], images[1]);
            effect.EffectStopped += new EventHandler<EventArgs>(OnStopEffect);
            effect.Start();
        }

        private void ResetButtonAction(object sender, TouchEventArgs e)
        {
            assetIndex = 0;
            images[0].Image = assets[assetIndex];
            images[1].Image = assets[assetIndex + 1];
            images[1].Visible = true;
            images[0].Visible = true;
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            buttonFlip.Enabled = true;
        }
    }
}
