/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
namespace DemoClock
{
    public enum ClockType
    {
        Analog,
        Flip
    };
    
    public class SettingData
    {
        public SettingData ()
        {
            ClockType = ClockType.Analog;
        }

        public ClockType ClockType;
    }
}

