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
    public partial class SlideInOutEffectScene : ContentsScene
    {
        public SlideInOutEffectScene()
        {
            InitializeWidget();

            this.Name = "SlideIn / SlideOutEffect";

            // SlideIn Button
            buttonSlideIn.ButtonAction += new EventHandler<TouchEventArgs>(SlideInAction);
            buttonSlideIn.Enabled = false;
            
            // SlideOut Button
            buttonSlideOut.ButtonAction += new EventHandler<TouchEventArgs>(SlideOutAction);
            buttonSlideOut.Enabled = true;
        }

        private void SlideInAction(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                SlideInEffect effect;

                effect = new SlideInEffect(imageBox1, 1000.0f, FourWayDirection.Left, SlideInEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideInEffect(imageBox2, 1000.0f, FourWayDirection.Up, SlideInEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideInEffect(imageBox3, 1000.0f, FourWayDirection.Down, SlideInEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideInEffect(imageBox4, 1000.0f, FourWayDirection.Right, SlideInEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                buttonSlideIn.Enabled = false;
                buttonSlideOut.Enabled = false;
            }
        }

        private void SlideOutAction(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                SlideOutEffect effect;

                effect = new SlideOutEffect(imageBox1, 1000.0f, FourWayDirection.Right, SlideOutEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideOutEffect(imageBox2, 1000.0f, FourWayDirection.Down, SlideOutEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideOutEffect(imageBox3, 1000.0f, FourWayDirection.Up, SlideOutEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                effect = new SlideOutEffect(imageBox4, 1000.0f, FourWayDirection.Left, SlideOutEffectInterpolator.Linear);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                buttonSlideIn.Enabled = false;
                buttonSlideOut.Enabled = false;
            }
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            if (sender is SlideInEffect)
            {
                buttonSlideIn.Enabled = false;
                buttonSlideOut.Enabled = true;
            }
            else if (sender is SlideOutEffect)
            {
                buttonSlideIn.Enabled = true;
                buttonSlideOut.Enabled = false;
            }
        }
    }
}
