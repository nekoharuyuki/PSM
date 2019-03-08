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
    public partial class AnalogClockPanel : LiveJumpPanel
    {
        AnalogClockAnimation[] pinAnimation;

        public AnalogClockPanel(String placeName, TimeSpan timeSpan, String bgImage)
        {
            InitializeWidget();

            // temporal code (current Interface Composer doesn't output some properties...)
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            //

            // set place name
            Label_1.Text = placeName;
            
            if( bgImage != null )
            {
                ImageBox_5.Image = new ImageAsset(bgImage);
            } 
            
            // Animation for pins
            pinAnimation = new AnalogClockAnimation[3];
            pinAnimation[0] = new AnalogClockAnimation(ImageBox_1, ImageBox_2, AnalogClockPinType.Hour, timeSpan);
            pinAnimation[1] = new AnalogClockAnimation(ImageBox_1, ImageBox_3, AnalogClockPinType.Minute, timeSpan);
            pinAnimation[2] = new AnalogClockAnimation(ImageBox_1, ImageBox_4, AnalogClockPinType.Second, timeSpan);
            
            this.JumpDelayTime = 0.3f;
        }
        
        public void Start()
        {
            for( int i=0; i<3; i++ )
            {
                pinAnimation[i].Start();
            }
        }
        
        public void Stop()
        {
            for( int i=0; i<3; i++ )
            {
                pinAnimation[i].Stop();
            }
        }
        
        public void OnShake(float x, float y, float accelZ)
        {
            LiveJumpPanel_1.Jump(LiveJumpPanel_1.X + LiveJumpPanel_1.Width/2, LiveJumpPanel_1.Y + LiveJumpPanel_1.Height/2);
        }
    }
}
