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
    public partial class ZoomEffectScene : ContentsScene
    {
        private ZoomEffectInterpolator interpolator;

        private float time = 1000f;
        private float scale1 = 0.3f;
        private float scale2 = 2.0f;
        private bool zoomOut = true;

        public ZoomEffectScene()
        {
            InitializeWidget();

            this.Name = "ZoomEffect";

            // Button Move
            buttonZoom.ButtonAction += new EventHandler<TouchEventArgs>(ButtonMoveExecute);

            interpolator = ZoomEffectInterpolator.EaseOutQuad;

            // Button Interp
            popupInterpolator.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupInterpolatorSelectedChangeAction);
            
            imageBox1.PivotType = PivotType.MiddleCenter;
            imageBox2.PivotType = PivotType.MiddleCenter;
        }
  
        private void PopupInterpolatorSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            interpolator = ZoomEffectInterpolator.Linear + args.NewIndex;
        }

        private void ButtonInterpExecute(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                int i = (int)interpolator;
                if (++i >= Enum.GetValues(typeof(ZoomEffectInterpolator)).Length) i = 0;
                interpolator = (ZoomEffectInterpolator)i;
                Button button = sender as Button;
                button.Text = "Interp (" + interpolator.ToString() + ")";
            }
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            buttonZoom.Enabled = true;
        }

        private void ButtonMoveExecute(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                ZoomEffect effect;

                effect = new ZoomEffect(imageBox1, time, zoomOut ? scale1 : 1, interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.CustomInterpolator = AnimationUtility.EaseInOutExpoInterpolator;
                effect.Start();

                effect = new ZoomEffect(imageBox2, time, zoomOut ? scale2 : 1, interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.CustomInterpolator = AnimationUtility.EaseInOutExpoInterpolator;
                effect.Start();

                buttonZoom.Enabled = false;
                zoomOut ^= true;
            }
        }
    }
}
