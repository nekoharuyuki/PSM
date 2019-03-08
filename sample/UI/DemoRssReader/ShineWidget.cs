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

namespace RssApp
{
    public class ShineWidget : Widget
    {
        private const float MIN_DURATION = 500.0f;
        private const float MAX_DURATION = 1500.0f;

        static private List<ImageAsset> shineImageList = new List<ImageAsset>();
        static private Random random = new Random();

        private ImageBox shineImage = new ImageBox();

        private float regionX;
        private float regionY;
        private float regionWidth;
        private float regionHeight;

        private float animationTime = 0.0f;
        private float animationDuration;

        private float velocityX;
        private float velocityY;

        static ShineWidget() {
            shineImageList.Add(new ImageAsset("/Application/assets/shine1.png"));
            shineImageList.Add(new ImageAsset("/Application/assets/shine2.png"));
            shineImageList.Add(new ImageAsset("/Application/assets/shine3.png"));
            shineImageList.Add(new ImageAsset("/Application/assets/shine4.png"));
            shineImageList.Add(new ImageAsset("/Application/assets/shine5.png"));
            shineImageList.Add(new ImageAsset("/Application/assets/shine6.png"));
        }

        public ShineWidget (float x, float y, float width, float height)
        {
            regionX = x;
            regionY = y;
            regionWidth = width;
            regionHeight = height;

            shineImage.PivotType = PivotType.MiddleCenter;
            this.AddChildLast(shineImage);
        }

        protected override void OnUpdate(float elapsedTime)
        {
            // Init animation
            if (animationTime <= 0.1f)
            {
                int randomIndex = random.Next(shineImageList.Count);
                shineImage.Image = shineImageList[randomIndex];
                shineImage.SetPosition(regionX + (float)random.NextDouble() * regionWidth,
                                        regionY + (float)random.NextDouble() * regionHeight);
                animationDuration = MIN_DURATION + (float)random.NextDouble() * (MAX_DURATION - MIN_DURATION);

                velocityX = (shineImage.X - (regionX + regionWidth)) / 100.0f;
                velocityY = (shineImage.Y - (regionY + regionHeight)) / 100.0f;
            }

            // Animate
            shineImage.SetSize(0.05f * animationTime * (float)Math.Abs(0.9f + 0.1f * Math.Sin(animationTime / 50.0f)),
                                0.05f * animationTime * (float)Math.Abs(0.9f + 0.1f * Math.Sin(animationTime / 50.0f)));
            shineImage.X += velocityX;
            shineImage.Y += velocityY;
            shineImage.Alpha = 0.1f + (animationDuration - animationTime) / animationDuration;

            // Update animationTime
            animationTime += elapsedTime;

            // Check end animation
            if (animationTime > animationDuration)
            {
                animationTime = 0.0f;
            }
        }
    }
}

