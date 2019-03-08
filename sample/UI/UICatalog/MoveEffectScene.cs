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
    public partial class MoveEffectScene : ContentsScene
    {
        private MoveEffectInterpolator interpolator;
        private const float animationDuration  = 750.0f;
        int index = 0;
        float[] imageBox1X;
        float[] imageBox1Y;
        float[] imageBox2X;
        float[] imageBox2Y;
        float[] imageBox3X;
        float[] imageBox3Y;
        float[] imageBox4X;
        float[] imageBox4Y;

        public MoveEffectScene()
        {
            InitializeWidget();

            this.Name = "MoveEffect";

            // Button Move
            buttonMove.ButtonAction += new EventHandler<TouchEventArgs>(ButtonMoveExecute);

            interpolator = MoveEffectInterpolator.EaseOutQuad;

            // Button Interp
            popupInterpolator.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupInterpolatorSelectedChangeAction);
        }
        
        protected override void OnShowing()
        {
            base.OnShowing();

            imageBox1X = new float[2];
            imageBox1Y = new float[2];
            imageBox1X[0] = imageBox1.X;
            imageBox1Y[0] = imageBox1.Y;
            imageBox1X[1] = imageBox4.X;
            imageBox1Y[1] = imageBox4.Y;

            imageBox2X = new float[2];
            imageBox2Y = new float[2];
            imageBox2X[0] = imageBox2.X;
            imageBox2Y[0] = imageBox2.Y;
            imageBox2X[1] = imageBox3.X;
            imageBox2Y[1] = imageBox3.Y;

            imageBox3X = new float[2];
            imageBox3Y = new float[2];
            imageBox3X[0] = imageBox3.X;
            imageBox3Y[0] = imageBox3.Y;
            imageBox3X[1] = imageBox2.X;
            imageBox3Y[1] = imageBox2.Y;

            imageBox4X = new float[2];
            imageBox4Y = new float[2];
            imageBox4X[0] = imageBox4.X;
            imageBox4Y[0] = imageBox4.Y;
            imageBox4X[1] = imageBox1.X;
            imageBox4Y[1] = imageBox1.Y;
        }

        private void ButtonMoveExecute(object sender, TouchEventArgs e)
        {
            if (sender is Button)
            {
                MoveEffect effect;

                int nextIndex = (index == 0) ? 1 : 0;

                imageBox1.X = imageBox1X[index]; imageBox1.Y = imageBox1Y[index];
                effect = new MoveEffect(imageBox1, animationDuration, imageBox1X[nextIndex], imageBox1Y[nextIndex], interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                imageBox2.X = imageBox2X[index]; imageBox2.Y = imageBox2Y[index];
                effect = new MoveEffect(imageBox2, animationDuration, imageBox2X[nextIndex], imageBox2Y[nextIndex], interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                imageBox3.X = imageBox3X[index]; imageBox3.Y = imageBox3Y[index];
                effect = new MoveEffect(imageBox3, animationDuration, imageBox3X[nextIndex], imageBox3Y[nextIndex], interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                imageBox4.X = imageBox4X[index]; imageBox4.Y = imageBox4Y[index];
                effect = new MoveEffect(imageBox4, animationDuration, imageBox4X[nextIndex], imageBox4Y[nextIndex], interpolator);
                effect.EffectStopped += OnStopEffect;
                effect.Start();

                index = nextIndex;

                buttonMove.Enabled = false;
            }
        }

        private void PopupInterpolatorSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            interpolator = MoveEffectInterpolator.Linear + args.NewIndex;
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            buttonMove.Enabled = true;
        }
    }
}
