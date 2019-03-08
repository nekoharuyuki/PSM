/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample
{
    public class ScreenOrientationChangedEventArgs : EventArgs
    {
        public ScreenOrientation ScreenOrientation { get; set; }
        
        public ScreenOrientationChangedEventArgs(ScreenOrientation orientation)
        {
            this.ScreenOrientation = orientation;
        }
    }
    
    public class ScreenOrientationManager
    {
		public static readonly int ORIENTATION_MILLISECOND = 500;
        private Tuple<ScreenOrientation, TimeSpan> m_PendingScreenOrientation = null;
        
        private ScreenOrientation m_CurrentScreenOrientation = ScreenOrientation.Landscape;
        public ScreenOrientation CurrentUIOrientation
        {
            get
            {
                return this.m_CurrentScreenOrientation;
            }
            private set
            {
                if (this.m_CurrentScreenOrientation == value) { return; }
                
                this.m_CurrentScreenOrientation = value;
                this.ScreenOrientationChanged(this, new ScreenOrientationChangedEventArgs(value));
            }
        }
        
        private MotionData m_CurrentMotionData = new MotionData();
        public MotionData CurrentMotionData
        {
            get
            {
                return this.m_CurrentMotionData;
            }
            private set
            {
                this.m_CurrentMotionData = value;
                
                var newOrientation = this.CalcOrientation(this.m_CurrentMotionData);
                if (this.CurrentUIOrientation == newOrientation)
                {
                    this.m_PendingScreenOrientation = null;
                    
                    return;
                }
                
                if (null == this.m_PendingScreenOrientation ||
                    this.m_PendingScreenOrientation.Item1 != newOrientation)
                {
                    this.m_PendingScreenOrientation =
                        new Tuple<ScreenOrientation, TimeSpan>(
                            newOrientation,
                            UISystem.CurrentTime);
                    
                    return;
                }
                
                var duration = UISystem.CurrentTime - this.m_PendingScreenOrientation.Item2;
                if (ORIENTATION_MILLISECOND >= duration.TotalMilliseconds)
                {
                    return;
                }
                
                this.m_PendingScreenOrientation = null;
                this.CurrentUIOrientation = newOrientation;
            }
        }
        
        public event EventHandler<ScreenOrientationChangedEventArgs> ScreenOrientationChanged = delegate { };
        
        public ScreenOrientationManager()
        {
            this.Update(Motion.GetData(0));
        }
        
        public void Update(MotionData motionData)
        {
            this.CurrentMotionData = motionData;
        }
        
        private ScreenOrientation CalcOrientation(MotionData motionData)
        {
			var x = motionData.Acceleration.X;
            var y = motionData.Acceleration.Y;
            var z = motionData.Acceleration.Z;
			
            var absX = Math.Abs(x);
            var absY = Math.Abs(y);
            var absZ = Math.Abs(z);
            
            if (absZ > absX + absY)
            {
                return this.m_CurrentScreenOrientation;
            }
            
            if (absX > absY)
			{
				return (0 < x) ? ScreenOrientation.Portrait : ScreenOrientation.ReversePortrait;
			}
			else
			{
				return (0 < y) ? ScreenOrientation.ReverseLandscape : ScreenOrientation.Landscape;
			}
        }
    }
}

