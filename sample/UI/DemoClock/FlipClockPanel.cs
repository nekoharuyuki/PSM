/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    public partial class FlipClockPanel : Panel
    {
        FlipPanelAnimation flipPanelAnimation;
        
        public FlipClockPanel(String placeName, TimeSpan timeSpan, String bgImage)
        {
            InitializeWidget();
            
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            
            Label_1.Text = placeName;

            flipPanelAnimation = new FlipPanelAnimation(this,
                                                        ImageBox_1.X, ImageBox_2.X, ImageBox_3.X, ImageBox_1.Y,
                                                        ImageBox_1.Width, ImageBox_1.Height,
                                                        ImageBox_4.X - ImageBox_1.X, ImageBox_5.X - ImageBox_1.X, ImageBox_4.Y - ImageBox_1.Y,
                                                        ImageBox_4.Width, ImageBox_4.Height,
                                                        ImageBox_1.Image, timeSpan, ImageBox_1.Anchors);
            
            ImageBox_1.Visible = false;
            ImageBox_2.Visible = false;
            ImageBox_3.Visible = false;
            ImageBox_4.Visible = false;
            ImageBox_5.Visible = false;
            ImageBox_6.Visible = false;
            ImageBox_7.Visible = false;
            ImageBox_8.Visible = false;
            ImageBox_9.Visible = false;

            if( bgImage != null )
            {
                ImageBox_10.Image = new ImageAsset(bgImage);
            } 
        }
        
        public void Start()
        {
            flipPanelAnimation.Start();
        }
        
        public void Stop()
        {
            flipPanelAnimation.Stop();
        }
    }
}
