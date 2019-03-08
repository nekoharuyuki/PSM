/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Input;

namespace Sample
{

/**
 * SampleSlider class
 */
public class SampleSlider : SampleWidget
{
    private uint barColor;
    private float rate;

    public SampleSlider(int rectX, int rectY, int rectW, int rectH) : base(rectX, rectY, rectW, rectH)
    {
        BarColor = 0xffff0000;
        Rate = 0.5f;
    }

    /// 
    public float Rate
    {
        get {return rate;}
        set {rate = value;}
    }

    public uint BarColor
    {
        get {return barColor;}
        set {barColor = value;}
    }

    /// Bar update
    public void Update(int pixelX)
    {
        rate = (float)(pixelX - rectX) / rectW;
    }

    public void Update(List<TouchData> touchDataList)
    {
        foreach (var touchData in touchDataList) {
            if (TouchMove(touchData)) {
                Update(SampleDraw.TouchPixelX(touchData));
            }
        }
    }

    public override void Draw()
    {
        base.Draw();

        SampleDraw.FillRect(barColor, rectX, rectY, (int)(rectW * rate), rectH);
    }
}

} // Sample
