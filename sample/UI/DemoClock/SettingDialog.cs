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
    public partial class SettingDialog : Dialog
    {
        public SettingDialog(SettingData settingData, MainScene mainScene)
        {
            InitializeWidget();
            
            Button_1.ButtonAction += HandleButton_1ButtonAction;
            Button_2.ButtonAction += HandleButton_2ButtonAction;
            
            CheckBox_1.CheckedChanged += HandleRadioButton_CheckedChange;
            CheckBox_2.CheckedChanged += HandleRadioButton_CheckedChange;
            
            data = settingData;
            
            CheckBox_1.Checked = (data.ClockType == ClockType.Analog);
            CheckBox_2.Checked = (data.ClockType == ClockType.Flip);
            
            targetScene = mainScene;
        }

        void HandleRadioButton_CheckedChange (object sender, TouchEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if( cb == CheckBox_1 )
            {
                CheckBox_2.Checked = !CheckBox_1.Checked;
            }
            else
            {
                CheckBox_1.Checked = !CheckBox_2.Checked;
            }
        }

        void HandleButton_1ButtonAction (object sender, TouchEventArgs e)
        {
            // "Apply" Button
            data.ClockType = CheckBox_1.Checked ? ClockType.Analog : ClockType.Flip;

            targetScene.ApplyChange();

            this.StartToHide();
        }

        void HandleButton_2ButtonAction (object sender, TouchEventArgs e)
        {
            // "Cancel" Button do nothing
            this.StartToHide();
        }
        
        void StartToHide()
        {
            this.Hide(new SlideOutEffect(this, 300, FourWayDirection.Up, SlideOutEffectInterpolator.Linear));
        }
        
        
        public void OnShake(float x, float y, float accelZ)
        {
        }

        SettingData data;
        MainScene targetScene;
    }
}
