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
    public partial class BunjeeJumpEffectScene : ContentsScene
    {
        private BunjeeJumpEffect effect;
        private float x0;
        private float y0;
        private float elasticity = 0.4f;

        public BunjeeJumpEffectScene()
        {
            InitializeWidget();

            this.Name = "BunjeeJumpEffect";

            // Button
            buttonBunjee.ButtonAction += new EventHandler<TouchEventArgs>(ButtonBunjeeAction);
        }
        
        protected override void OnShowing()
        {
            base.OnShowing();

            x0 = image.X;
            y0 = image.Y;
        }

        private void ButtonBunjeeAction(object sender, TouchEventArgs e)
        {
            buttonBunjee.Enabled = false;

            if (effect != null)
            {
                effect.Stop();
            }
            image.X = x0;
            image.Y = y0;
            image.Transform3D = Matrix4.Translation(new Vector3(image.X, image.Y, 0.0f));

            effect = new BunjeeJumpEffect(image, elasticity);
            effect.EffectStopped += new EventHandler<EventArgs>(OnStopEffect);
            effect.Start();
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            buttonBunjee.Enabled = true;
        }
    }
}
