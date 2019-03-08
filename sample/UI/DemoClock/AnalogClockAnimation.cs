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
    /// <summary>
    /// type of analog clock's pin
    /// </summary>
    public enum AnalogClockPinType
    {
        Hour,
        Minute,
        Second
    }   

    /// <summary>
    /// Animation for analog clock pins.
    /// </summary>
    public class AnalogClockAnimation : Effect
    {
        public AnalogClockAnimation(Widget bgWidget, Widget currentWidget, AnalogClockPinType pinType, TimeSpan timeSpan)
        {
            BgWidget = bgWidget;
            CurrentWidget = currentWidget;
            PinType = pinType;
            
            _timeSpan = timeSpan;
        }

        protected override void OnStart()
        {
        }

        protected override EffectUpdateResponse OnUpdate(float elapsedTime)
        {
            DateTime now = DateTime.UtcNow + _timeSpan;
            
            float transX = BgWidget.X + BgWidget.Width/2;
            float transY = BgWidget.Y + BgWidget.Height/2;
            float transZ = 0.0f;

            float degree = 0.0f;
            if( PinType == AnalogClockPinType.Hour )
            {
                degree = ((now.Hour % 12) / 12.0f) * 360.0f;
                degree += (now.Minute + ((now.Second+now.Millisecond/1000.0f) / 60.0f)) / 60.0f * 30.0f;

            }
            else if( PinType == AnalogClockPinType.Minute )
            {
                degree = (now.Minute + ((now.Second+now.Millisecond/1000.0f) / 60.0f)) / 60.0f * 360.0f;
            }
            else if( PinType == AnalogClockPinType.Second )
            {
                degree = AnimationUtility.ElasticInterpolator(now.Second-1, now.Second, Math.Min(now.Millisecond/400.0f, 1.0f)) / 60.0f * 360.0f;
            }
            

            CurrentWidget.Transform3D = GetTransform3D(transX, transY, transZ, degree);

            return EffectUpdateResponse.Continue;
        }
        
        private Matrix4 GetTransform3D(float transX, float transY, float transZ, float degree)
        {
            Matrix4 mat;
            Matrix4 mat0;
            
            switch( PinType )
            {
            case AnalogClockPinType.Hour:
                mat0 = Matrix4.Translation(new Vector3(-CurrentWidget.Width/2, -CurrentWidget.Height+10, 0));
                break;
            case AnalogClockPinType.Minute:
                mat0 = Matrix4.Translation(new Vector3(-CurrentWidget.Width/2, -CurrentWidget.Height+16, 0));
                break;
            case AnalogClockPinType.Second:
            default:
                mat0 = Matrix4.Translation(new Vector3(-CurrentWidget.Width/2, -CurrentWidget.Height+0, 0));
                break;
            }

            Matrix4 mat1 = Matrix4.RotationZ(degree / 360.0f * 2.0f * (float)Math.PI);
            Matrix4 mat2 = Matrix4.Translation(new Vector3(transX, transY, transZ));

            mat = mat2 * mat1 * mat0;

            return mat;
        }

        protected override void OnStop()
        {
        }

        public Widget BgWidget
        {
            get;
            set;
        }
        
        public Widget CurrentWidget
        {
            get;
            set;
        }

        public AnalogClockPinType PinType
        {
            get;
            set;
        }
        
        TimeSpan _timeSpan;
    }

}

