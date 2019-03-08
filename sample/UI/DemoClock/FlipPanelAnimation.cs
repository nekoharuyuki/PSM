/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    public class FlipPanelAnimation : Effect
    {
        public FlipPanelAnimation (Panel mainPanel, float bgx_hour, float bgx_minute, float bgx_second,
                                   float bgy, float bgw, float bgh,
                                   float x1, float x2, float y, float w, float h,
                                   ImageAsset bgImage, TimeSpan timeSpan, Anchors anchors)
        {
            X1 = x1;
            X2 = x2;
            Y = y;
            Width = w;
            Height = h;
            
            TimeSpan = timeSpan;

            for( int i=1; i>=0; i-- )
            {
                HourPanel[i] = new FlipPanelWidget(FlipPanelType.Hour);
                MinutePanel[i] = new FlipPanelWidget(FlipPanelType.Minute);
                SecondPanel[i] = new FlipPanelWidget(FlipPanelType.Second);
                
                HourPanel[i].Clip = false;
                MinutePanel[i].Clip = false;
                SecondPanel[i].Clip = false;
                
                HourPanel[i].X = bgx_hour;
                HourPanel[i].Y = bgy;
                HourPanel[i].Width = HourPanel[i].BgImageBox.Width = bgw;
                HourPanel[i].Height = HourPanel[i].BgImageBox.Height = bgh;
    
                MinutePanel[i].X = bgx_minute;
                MinutePanel[i].Y = bgy;
                MinutePanel[i].Width = MinutePanel[i].BgImageBox.Width = bgw;
                MinutePanel[i].Height = MinutePanel[i].BgImageBox.Height = bgh;

                SecondPanel[i].X = bgx_second;
                SecondPanel[i].Y = bgy;
                SecondPanel[i].Width = SecondPanel[i].BgImageBox.Width = bgw;
                SecondPanel[i].Height = SecondPanel[i].BgImageBox.Height = bgh;
    

                HourPanel[i].BgImageBox.Image = bgImage;
                MinutePanel[i].BgImageBox.Image = bgImage;
                SecondPanel[i].BgImageBox.Image = bgImage;
                
                HourPanel[i].Anchors = anchors;
                MinutePanel[i].Anchors = anchors;
                SecondPanel[i].Anchors = anchors;

                mainPanel.AddChildLast(HourPanel[i]);
                mainPanel.AddChildLast(MinutePanel[i]);
                mainPanel.AddChildLast(SecondPanel[i]);

            }
            
            numImages = new ImageAsset[10];
            for( int i=0; i<10; i++ )
            {
                string fileName = "/Application/assets/num_" + (i).ToString("0") + ".png";
                Console.WriteLine(fileName);
                numImages[i] = new ImageAsset(fileName);
            }
            
        }
        ImageAsset[] numImages;

        protected override void OnStart()
        {
            DateTime now = DateTime.UtcNow + TimeSpan;

            SetupFlipPanel(HourPanel[(hourIndex+1)%2], now.Hour);
            StartFlipPanel(HourPanel[hourIndex], HourPanel[(hourIndex+1)%2]);

            SetupFlipPanel(MinutePanel[(minIndex+1)%2], now.Minute);
            StartFlipPanel(MinutePanel[minIndex], MinutePanel[(minIndex+1)%2]);

            SetupFlipPanel(SecondPanel[(secIndex+1)%2], now.Second);
            StartFlipPanel(SecondPanel[secIndex], SecondPanel[(secIndex+1)%2]);

            previousTime = now;
        }

        protected override EffectUpdateResponse OnUpdate(float elapsedTime)
        {
            DateTime now = DateTime.UtcNow + TimeSpan;

            if( now.Hour != previousTime.Hour )
            {
                SetupFlipPanel(HourPanel[(hourIndex+1)%2], now.Hour);
                StartFlipPanel(HourPanel[hourIndex], HourPanel[(hourIndex+1)%2]);
            }
            
            if( now.Minute != previousTime.Minute )
            {
                SetupFlipPanel(MinutePanel[(minIndex+1)%2], now.Minute);
                StartFlipPanel(MinutePanel[minIndex], MinutePanel[(minIndex+1)%2]);
            }
            
            if( now.Second != previousTime.Second )
            {
                SetupFlipPanel(SecondPanel[(secIndex+1)%2], now.Second);
                StartFlipPanel(SecondPanel[secIndex], SecondPanel[(secIndex+1)%2]);
            }
            
            previousTime = now;

            return EffectUpdateResponse.Continue;
        }

        protected override void OnStop()
        {
        }
        
        
        void SetupFlipPanel(FlipPanelWidget targetPanel, int timeNum)
        {
            targetPanel.ImageBox0.Image = numImages[(int)(timeNum/10)];
            targetPanel.ImageBox0.X = X1;
            targetPanel.ImageBox0.Y = Y;
            targetPanel.ImageBox0.Width = Width;
            targetPanel.ImageBox0.Height = Height;

            targetPanel.ImageBox1.Image = numImages[timeNum%10];
            targetPanel.ImageBox1.X = X2;
            targetPanel.ImageBox1.Y = Y;
            targetPanel.ImageBox1.Width = Width;
            targetPanel.ImageBox1.Height = Height;
        }
        
        public void StartFlipPanel(FlipPanelWidget targetPanel, FlipPanelWidget nextPanel)
        {
            targetPanel.Visible = true;
            nextPanel.Visible = true;

            FlipBoardEffect effect = new FlipBoardEffect(targetPanel, nextPanel);
            effect.EffectStopped += new EventHandler<EventArgs>(OnStopEffect);
            effect.Start();
        }
        
        private void OnStopEffect(object sender, EventArgs e)
        {
            FlipBoardEffect ef = sender as FlipBoardEffect;
            FlipPanelWidget fpw = ef.Widget as FlipPanelWidget;
            if( fpw.Type == FlipPanelType.Hour )
            {
                hourIndex = (hourIndex + 1) % 2;
            }
            else if( fpw.Type == FlipPanelType.Minute )
            {
                minIndex = (minIndex + 1) % 2;
            }
            else if( fpw.Type == FlipPanelType.Second )
            {
                secIndex = (secIndex + 1) % 2;
            }
        }
        
        FlipPanelWidget[] HourPanel = new FlipPanelWidget[2];
        FlipPanelWidget[] MinutePanel = new FlipPanelWidget[2];
        FlipPanelWidget[] SecondPanel = new FlipPanelWidget[2];
        
        float X1;
        float X2;
        float Y;
        float Width;
        float Height;
        
        int hourIndex = 0;
        int minIndex = 0;
        int secIndex = 0;
        
        DateTime previousTime;
        
        TimeSpan TimeSpan;
    }
    
    public enum FlipPanelType
    {
        Hour,
        Minute,
        Second
    }
    
    public class FlipPanelWidget : Panel
    {
        public FlipPanelType Type
        {
            get;
            set;
        }
        
        public FlipPanelWidget(FlipPanelType type)
        {
            Type = type;
            
            this.Clip = false;

            BgImageBox = new ImageBox();
            ImageBox0 = new ImageBox();
            ImageBox1 = new ImageBox();

            this.AddChildLast(BgImageBox);
            this.AddChildLast(ImageBox0);
            this.AddChildLast(ImageBox1);
        }
        
        public ImageBox ImageBox0
        {
            get;
            set;
        }
        public ImageBox ImageBox1
        {
            get;
            set;
        }
        
        public ImageBox BgImageBox
        {
            get;
            set;
        }
    }
}

