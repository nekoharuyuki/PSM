/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace Sample
{

/**
 * SampleButton class
 */
public class SampleButton : SampleWidget
{
    private SampleSprite textSprite = null;
    private string label;
    private TextAlign textAlign;
    private VerticalAlign verticalAlign;
    private uint textColor;

    /// Horizontal alignment
    public enum TextAlign
    {
        Left,
        Center,
        Right
    }

    /// Vertical alignment
    public enum VerticalAlign
    {
        Top,
        Middle,
        Bottom
    }

    public SampleButton(int rectX, int rectY, int rectW, int rectH) : base(rectX, rectY, rectW, rectH)
    {
        TextColor = 0xff000000;
    }

    public void SetText(string label)
    {
        SetText(label, TextAlign.Center, VerticalAlign.Middle);
    }

    public void SetText(string label, TextAlign textAlign, VerticalAlign verticalAlign)
    {
        this.textAlign = textAlign;
        this.verticalAlign = verticalAlign;

        Label = label;
    }

    public string Label
    {
        get {return label;}
        set {
            label = value;

            int textW = SampleDraw.CurrentFont.GetTextWidth(label, 0, label.Length);
            int textH = SampleDraw.CurrentFont.Metrics.Height;

            // clamp to max size
            textW = Math.Min( textW, 2048 );
            
            int textX = rectX;
            int textY = rectY;

            switch (textAlign) {
            case TextAlign.Center:
                textX += (rectW - textW) / 2;
                break;
            case TextAlign.Right:
                textX += rectW - textW;
                break;
            }

            switch (verticalAlign) {
            case VerticalAlign.Middle:
                textY += (rectH - textH) / 2;
                break;
            case VerticalAlign.Bottom:
                textY += rectH - textH;
                break;
            }

            if (textSprite != null) {
                textSprite.Dispose();
            }
            textSprite = new SampleSprite(label, textColor, SampleDraw.CurrentFont, textX, textY);
        }
    }

    public uint TextColor
    {
        get {return textColor;}
        set {textColor = value;}
    }

    public override void Dispose()
    {
        if (textSprite != null) {
            textSprite.Dispose();
            textSprite = null;
        }
    }

    public override void Draw()
    {
        base.Draw();

        SampleDraw.DrawSprite(textSprite);
    }

}

} // Sample
