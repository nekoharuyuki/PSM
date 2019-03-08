/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    public delegate void OnScrollEnd();

    public class Ticker : Widget
    {
        // Animation speed
        private const float SCROLL_VELOCITY = -0.13f;
            
        // Stop duration at text head
        private const float HEAD_STOP_DURATION = 1000.0f;
        
        // Ticker text margin
        private const float TEXT_MARGIN = 100.0f;
        
        // Scroll label
        private Label tickerLabel = new Label();
        
        // Total stop time at text head
        private float headStopTime;
        
        // Callback
        private OnScrollEnd onScrollEnd;
        
        // Loop same text
        private bool isLoop = false;
        
        // Animation
        private bool isAnimation = false;
        
        public Ticker ()
        {
            this.Clip = true;
            
            // Set up ticker label
            tickerLabel.X = 0;
            tickerLabel.Y = 0;
            tickerLabel.TextColor = new UIColor(224f / 255f, 255f / 255f, 240f / 255f, 255f / 255f);
            tickerLabel.Font = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            tickerLabel.LineBreak = LineBreak.Character;
            TextShadowSettings textShadow = new TextShadowSettings();
            textShadow.Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f);
            textShadow.HorizontalOffset = 2;
            textShadow.VerticalOffset = 2;
            tickerLabel.TextShadow = textShadow;
            this.AddChildLast(tickerLabel);
            
            ResetPos();
        }
        
        protected override void OnUpdate(float elapsedTime)
        {
            if (!isAnimation)
            {
                return;
            }
            
            // Stop at head position
            if (tickerLabel.X < 10 && headStopTime < HEAD_STOP_DURATION)
            {
                headStopTime += elapsedTime;
                return;
            }
                
            // Update position
            tickerLabel.X += SCROLL_VELOCITY * elapsedTime;
            
            // Scroll end
            if (tickerLabel.X < -tickerLabel.Width)
            {
                if (isLoop)
                {
                    tickerLabel.X = this.Width;
                }
                else
                {
                    isAnimation = false;
                }

                if (onScrollEnd != null)
                {
                    onScrollEnd();
                }
            }
        }
        
        private void UpdateLabelSize()
        {
            tickerLabel.Width = tickerLabel.Font.GetFont().GetTextWidth(tickerLabel.Text)
                                + tickerLabel.TextShadow.HorizontalOffset
                                + TEXT_MARGIN;
            tickerLabel.Height = this.Height;
        }
        
        public void SetText(string text)
        {
            tickerLabel.Text = text;
            isAnimation = true;
            UpdateLabelSize();
            ResetPos();
        }
        
        public void SetOnScrollEnd(OnScrollEnd func)
        {
            onScrollEnd = func;
        }
        
        public void ResetPos()
        {
            tickerLabel.X = this.Width;
            headStopTime = 0.0f;
        }
        
        public void SetTextColor(UIColor color)
        {
            tickerLabel.TextColor = color;
        }
    }
}

