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
 * SampleWidget class
 */
public class SampleWidget : IDisposable
{
    protected int rectX;
    protected int rectY;
    protected int rectW;
    protected int rectH;
    protected uint buttonColor;

    public SampleWidget(int rectX, int rectY, int rectW, int rectH)
    {
        SetRect(rectX, rectY, rectW, rectH);
        buttonColor = 0xffffffff;
    }

    public uint ButtonColor
    {
        get {return buttonColor;}
        set {buttonColor = value;}
    }

    /// Set the rectangle
    public void SetRect(int rectX, int rectY, int rectW, int rectH)
    {
        this.rectX = rectX;
        this.rectY = rectY;
        this.rectW = rectW;
        this.rectH = rectH;
    }

    /// Rectangle touch test
    public bool TouchDown(List<TouchData> touchDataList)
    {
        foreach (var touchData in touchDataList) {
            if (TouchDown(touchData)) {
                return true;
            }
        }

        return false;
    }

    /// Rectangle touch test
    public bool TouchDown(TouchData touchData)
    {
        if (touchData.Status == TouchStatus.Down) {
            return InsideRect(SampleDraw.TouchPixelX(touchData),
                              SampleDraw.TouchPixelY(touchData));
        }

        return false;
    }

    /// Rectangle touch test
    public bool TouchMove(List<TouchData> touchDataList)
    {
        foreach (var touchData in touchDataList) {
            if (TouchMove(touchData)) {
                return true;
            }
        }

        return false;
    }

    /// Rectangle touch test
    public bool TouchMove(TouchData touchData)
    {
        if (touchData.Status == TouchStatus.Move) {
            return InsideRect(SampleDraw.TouchPixelX(touchData),
                              SampleDraw.TouchPixelY(touchData));
        }

        return false;
    }

    /// Inside rectangle test
    public bool InsideRect(int pixelX, int pixelY)
    {
        if (rectX <= pixelX && rectX + rectW >= pixelX &&
            rectY <= pixelY && rectY + rectH >= pixelY) {
            return true;
        }

        return false;
    }

    public virtual void Dispose()
    {
    }

    public virtual void Draw()
    {
        SampleDraw.FillRect(buttonColor, rectX, rectY, rectW, rectH);
    }

}

} // Sample
