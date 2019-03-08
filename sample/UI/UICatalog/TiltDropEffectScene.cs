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
    public partial class TiltDropEffectScene : ContentsScene
    {
        private TiltDropEffect effect;

        public TiltDropEffectScene()
        {
            InitializeWidget();
            this.Name = "TiltDropEffect";

            buttonTiltDrop.ButtonAction += new EventHandler<TouchEventArgs>(ButtonTiltDropExecute);
            buttonReset.ButtonAction += new EventHandler<TouchEventArgs>(ButtonResetExecute);
        }

        private void ButtonTiltDropExecute(object sender, TouchEventArgs e)
        {
            effect = new TiltDropEffect(image);
            effect.EffectStopped += new EventHandler<EventArgs>(OnStopEffect);
            effect.Start();
            buttonTiltDrop.Enabled = false;
        }

        private void ButtonResetExecute(object sender, TouchEventArgs e)
        {
            if (effect != null && effect.Playing)
            {
                effect.Stop();
            }

            image.Visible = true;
            buttonTiltDrop.Enabled = true;
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            if (sender is Effect)
            {
                buttonTiltDrop.Enabled = true;
            }
        }
    }
}
