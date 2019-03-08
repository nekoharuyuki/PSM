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

namespace CalendarMaker
{
    public partial class FullScreenPanel : Panel
    {
        float ANIMATION_DURATION = 400.0f;
        public FullScreenPanel()
        {
            InitializeWidget();
            
            // Add close event
            closeButton.ButtonAction += new EventHandler<TouchEventArgs>(HideWallpaperAction);
        }
        
        public ImageAsset WallpaperImage
        {
            get {return this.wallpaper.Image;}
            
            set {this. wallpaper.Image = value;}
        }
        
        public void Show()
        {
            this.Visible = true;
            this.ShowWallpaper();
        }
        
        private void ShowWallpaper()
        {
            this.PivotType = PivotType.MiddleCenter;
            this.Visible = true;
            // Zoom
            ZoomEffect zoomEffect = new ZoomEffect(this, 
                                                    ANIMATION_DURATION, 
                                                    1.0f, 
                                                    ZoomEffectInterpolator.EaseOutQuad);
            zoomEffect.Start();
            
            
            // Fade
            FadeInEffect fadeEffect = new FadeInEffect(this,
                                                    0.0f,
                                                    FadeInEffectInterpolator.EaseOutQuad);
            fadeEffect.Start();
        }
        
        private void HideWallpaperAction(object sender, TouchEventArgs e)
        {

            this.PivotType = PivotType.MiddleCenter;

            // Zoom
            ZoomEffect zoomEffect = new ZoomEffect(this, 
                                                    ANIMATION_DURATION, 
                                                    0.01f, 
                                                    ZoomEffectInterpolator.EaseOutQuad);
            zoomEffect.Start();
            
            
            // Fade
            FadeOutEffect fadeEffect = new FadeOutEffect(this,
                                                    400.0f,
                                                    FadeOutEffectInterpolator.EaseOutQuad);
            fadeEffect.EffectStopped += OnHideAnimationEnd;
            fadeEffect.Start();
        }
        private void OnHideAnimationEnd(object sender, EventArgs e)
        {
            this.Visible = false;
        }
            
    }
}
