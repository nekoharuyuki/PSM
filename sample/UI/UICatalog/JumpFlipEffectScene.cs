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
    public partial class JumpFlipEffectScene : ContentsScene
    {
        private int revolution = 1;
        private CheckBox[] radioGroup;

        public JumpFlipEffectScene()
        {
            InitializeWidget();

            this.Name = "JumpFlipEffect";

            // Button Fast
            buttonJumpFlip.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecute);

            rxSlider.Value = revolution;
            rxSlider.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(RevolutionValueChanging);
            rxSlider.ValueChangeEventEnabled = true;
            
            radioGroup = new CheckBox[2];
            radioGroup[0] = xRadioButton;
            radioGroup[1] = yRadioButton;
            for (int i = 0; i < radioGroup.Length; i++)
            {
                radioGroup[i].CheckedChanged += new EventHandler<TouchEventArgs>(radio_CheckedChange);
            }

            rvLabel.Text = "Revolution: " + revolution;
        }

        private void ButtonExecute(object sender, TouchEventArgs e)
        {
            EffectStart();
        }

        private void RevolutionValueChanging(object sender, SliderValueChangeEventArgs e)
        {
            revolution = (int)e.Value;
            rvLabel.Text = "Revolution: " + revolution;
        }
        
        bool radioUpdating = false;

        private void radio_CheckedChange(object sender, TouchEventArgs e)
        {
            if (radioUpdating) return;

            radioUpdating = true;

            CheckBox radio = sender as CheckBox;
            if (radio.Checked)
            {
                foreach (CheckBox r in radioGroup)
                {
                    if (r != radio)
                    {
                        r.Checked = false;
                    }
                }
            }
            else
            {
                radio.Checked = true;
            }

            radioUpdating = false;
        }

        private void EffectStart()
        {
            var img1 = currentImage;
            var img2 = currentImage;
            if(currentImage.Visible)
            {
                img2 = image2;
            }
            else
            {
                img1 = image2;
            }
            JumpFlipEffect effect = new JumpFlipEffect(img1, img2);
            effect.EffectStopped += new EventHandler<EventArgs>(OnStopEffect);
            effect.Revolution = revolution;
            if (xRadioButton.Checked)
            {
                effect.RotationAxis = JumpFlipEffectAxis.X;
            }
            else
            {
                effect.RotationAxis = JumpFlipEffectAxis.Y;
            }
            effect.Start();

            buttonJumpFlip.Enabled = false;
        }

        private void OnStopEffect(object sender, EventArgs e)
        {
            if (sender is JumpFlipEffect)
            {
                buttonJumpFlip.Enabled = true;
            }
        }

    }

}
