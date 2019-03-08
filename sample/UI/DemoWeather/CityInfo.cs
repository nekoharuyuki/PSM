/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

namespace Weather
{
public class CityInfo
    {
        public int cityId;
        
        public String cityName;
        
        public float locationX;
        
        public float locationY;
        
        public int weatherId;
        
        public int highTemp;
        
        public int lowTemp;
        
        public int precipitation;
        
        public CityInfo (int id, String name, float x, float y, int weather, int low, int high, int prec)
        {
            cityId = id;
            cityName = name;
            locationX = x;
            locationY = y;
            weatherId = weather;
            lowTemp = low;
            highTemp = high;
            precipitation = prec;
        }
    }
}

