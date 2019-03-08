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
    public partial class MainScene : Scene
    {
        AnalogClockPanel[] analogClockPanel;
        FlipClockPanel[] flipClockPanel;
        
        public MainScene()
        {
            InitializeWidget();

            flipClockPanel = new FlipClockPanel[5];
            flipClockPanel[0] = new FlipClockPanel("Kyoto", new TimeSpan(9,0,0), "/Application/assets/Kyoto2.JPG");
            flipClockPanel[1] = new FlipClockPanel("London", new TimeSpan(0,0,0), "/Application/assets/London2.JPG");
            flipClockPanel[2] = new FlipClockPanel("Milan", new TimeSpan(1,0,0), "/Application/assets/Milan.JPG");
            flipClockPanel[3] = new FlipClockPanel("Peru", new TimeSpan(-5,0,0), "/Application/assets/Peru.JPG");
            flipClockPanel[4] = new FlipClockPanel("Rennes", new TimeSpan(1,0,0), "/Application/assets/MontSaintMichel.JPG");
            
            analogClockPanel = new AnalogClockPanel[5];
            analogClockPanel[0] = new AnalogClockPanel("Kyoto", new TimeSpan(9,0,0), "/Application/assets/Kyoto1.JPG");
            analogClockPanel[1] = new AnalogClockPanel("London", new TimeSpan(0,0,0), "/Application/assets/London1.JPG");
            analogClockPanel[2] = new AnalogClockPanel("San Francisco", new TimeSpan(-8,0,0), "/Application/assets/SanFrancisco.JPG");
            analogClockPanel[3] = new AnalogClockPanel("München", new TimeSpan(1,0,0), "/Application/assets/Munich.JPG");
            analogClockPanel[4] = new AnalogClockPanel("Venice", new TimeSpan(1,0,0), "/Application/assets/Venice.JPG");

            for( int i=0; i<5; i++ )
            {
                PagePanel_1.AddPage( analogClockPanel[i] );
                analogClockPanel[i].Start();
            }

            Button_1.ButtonAction += HandleButton_1ButtonAction;
            
            settingData = new SettingData();
            settingDialog = null;
        }

        void HandleButton_1ButtonAction (object sender, TouchEventArgs e)
        {
            settingDialog = new SettingDialog(settingData, this);
            settingDialog.Show(new BunjeeJumpEffect(settingDialog, 0.3f));
        }
        
        public void ApplyChange()
        {
            int currentPageIndex = PagePanel_1.CurrentPageIndex;

            while( PagePanel_1.PageCount > 0 )
            {
                PagePanel_1.RemovePageAt(0);
            }

            if( settingData.ClockType == ClockType.Analog )
            {
                for( int i=0; i<5; i++ )
                {
                    flipClockPanel[i].Stop();
                    PagePanel_1.AddPage( analogClockPanel[i] );
                    analogClockPanel[i].Start();
                }
            }
            else
            {
                for( int i=0; i<5; i++ )
                {
                    analogClockPanel[i].Stop();
                    PagePanel_1.AddPage( flipClockPanel[i] );
                    flipClockPanel[i].Start();
                }
            }
            
            PagePanel_1.CurrentPageIndex = currentPageIndex;
        }
        
        public void OnShake(float accelZ)
        {
            if( settingData.ClockType == ClockType.Analog )
            {
                for( int i=0; i<5; i++ )
                {
                    analogClockPanel[i].OnShake(analogClockPanel[i].Width/2, analogClockPanel[i].Height/2, accelZ);
                }
            }
            
            if( settingDialog != null )
            {
                settingDialog.OnShake(settingDialog.Width/2, settingDialog.Height/2, accelZ);
            }
            
            
        }
        
        SettingData settingData;
        SettingDialog settingDialog;
    }
}
