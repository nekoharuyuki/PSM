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
    public partial class FadeInOutEffectScene : ContentsScene
    {
        public FadeInOutEffectScene()
        {
            InitializeWidget();

            this.Name = "FadeIn / FadeOutEffect";

            // FadeIn Button
            buttonFadeIn.ButtonAction += new EventHandler<TouchEventArgs>(FadeInAction);
            buttonFadeIn.Enabled = false;

            // FadeOut Button
            buttonFadeOut.ButtonAction += new EventHandler<TouchEventArgs>(FadeOutAction);
            buttonFadeOut.Enabled = true;
        }

        private void FadeInAction(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                FadeInEffect effect = new FadeInEffect(imageBox, 1000.0f, FadeInEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                buttonFadeIn.Enabled = false;
                buttonFadeOut.Enabled = false;
            }
        }

        private void FadeOutAction(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                FadeOutEffect effect = new FadeOutEffect(imageBox, 1000.0f, FadeOutEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                buttonFadeIn.Enabled = false;
                buttonFadeOut.Enabled = false;
            }
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            if (sender is FadeInEffect)
            {
                buttonFadeIn.Enabled = false;
                buttonFadeOut.Enabled = true;
            }
            else if (sender is FadeOutEffect)
            {
                buttonFadeIn.Enabled = true;
                buttonFadeOut.Enabled = false;
            }
        }
    }
}
