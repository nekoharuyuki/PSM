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
    public partial class ExplanationPanel : Panel
    {
        public ExplanationPanel()
        {
            InitializeWidget();
            closeButton.ButtonAction += new EventHandler<TouchEventArgs>(CloseButtonExecuteAction);
            
            ImageSelectionGuide();
        }
        
        private void CloseButtonExecuteAction(object sender, TouchEventArgs e)
        {
            this.Visible = false;
        }
        
        public void ImageSelectionGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID12);
            baseImage.Image = new ImageAsset("/Application/assets/base/base01.png");
            FadeInEffectExecute();
        }
        public void ImageSizeGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID13);
            baseImage.Image = new ImageAsset("/Application/assets/base/base02.png");
            FadeInEffectExecute();
        }
        
        public void CalendarTypeGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID14);
            baseImage.Image = new ImageAsset("/Application/assets/base/base03.png");
            FadeInEffectExecute();
        }
        
        public void CalendarPositionGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID15);
            baseImage.Image = new ImageAsset("/Application/assets/base/base04.png");
            FadeInEffectExecute();
        }
        private void FadeInEffectExecute()
        {
            float time = 300;
            FadeInEffect fadeIn = new FadeInEffect(this, time, FadeInEffectInterpolator.EaseOutQuad);
            fadeIn.Start();
        }
        
        private void FadeOutEffectExecute()
        {
            float time = 200;
            FadeOutEffect fadeOut = new FadeOutEffect(this, time, FadeOutEffectInterpolator.EaseOutQuad);
            fadeOut.Start();
        }
    }
}
