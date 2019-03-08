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
    public partial class WallpaperSizingPanel : Panel
    {
        private CompleteModeAction itemClickAction;
        
        DragGestureDetector drag;
        PinchGestureDetector pinch;

        private float wallpaperCurrentWidth;
        private float wallpaperCurrentHeight;
        private float prevPinchAngle = 0.0f;
        private Vector2 pinchCenterOffset = Vector2.Zero;
        private bool canRotate = false;

        private float WALLPAPER_MAX_WIDTH;
        private float WALLPAPER_MAX_HEIGHT;
        private float WALLPAPER_MIN_WIDTH = 100;
        private float WALLPAPER_MIN_HEIGHT = 100;
        
        public WallpaperSizingPanel(CompleteModeAction action)
        {
            InitializeWidget();
            
            WALLPAPER_MAX_WIDTH = this.Width * 4.0f;
            WALLPAPER_MAX_HEIGHT = this.Height * 4.0f;
            
            this.HookChildTouchEvent = true;
            this.wallpaper.PivotType = PivotType.MiddleCenter;
            this.wallpaper.TouchResponse = false;

            wallpaperCurrentWidth = wallpaper.Width;
            wallpaperCurrentHeight = wallpaper.Height;

            prevPinchAngle = 0.0f;

            // Init button action
            Button_1.ButtonAction += new EventHandler<TouchEventArgs>(okButtonExecuteAction);

            // Init Drag
            drag = new DragGestureDetector();
            drag.DragDetected += new EventHandler<DragEventArgs>(DragHandler);

            // Init Pinch
            pinch = new PinchGestureDetector();
            pinch.PinchStartDetected += new EventHandler<PinchEventArgs>(PinchStartHandler);
            pinch.PinchDetected += new EventHandler<PinchEventArgs>(PinchHandler);

            // Add Gesture
            AddGestureDetector(pinch);
            AddGestureDetector(drag);
            itemClickAction = action;

            this.wallpaper.PivotType = PivotType.MiddleCenter;
        }

        void okButtonExecuteAction(object sender, EventArgs e)
        {
            itemClickAction();
        }

        void PinchStartHandler(object sender, PinchEventArgs e)
        {
            Vector2 current = new Vector2(this.wallpaper.X, this.wallpaper.Y);
            pinchCenterOffset = current - e.LocalCenterPosition;
            wallpaperCurrentWidth = wallpaper.Width;
            wallpaperCurrentHeight = wallpaper.Height;
            prevPinchAngle = 0.0f;
        }
        
        void PinchHandler(object sender, PinchEventArgs e)
        {
            // Change position
            Vector2 nextPos = e.LocalCenterPosition + pinchCenterOffset;
            if (nextPos.X > 0 && nextPos.Y > 0
                && nextPos.X < this.Width && nextPos.Y < this.Height)
            {
                this.wallpaper.SetPosition(nextPos.X, nextPos.Y);
            }

            // Change scale
            float nextWidth = wallpaperCurrentWidth * e.Scale;
            float nextHeight = wallpaperCurrentHeight * e.Scale;
            if(nextWidth < WALLPAPER_MAX_WIDTH && nextHeight < WALLPAPER_MAX_HEIGHT 
                && nextWidth > WALLPAPER_MIN_WIDTH && nextHeight > WALLPAPER_MIN_HEIGHT)
            {
                this.wallpaper.SetSize(nextWidth, nextHeight);
            }

            // Change angle
            if (canRotate)
            {
                Matrix4 rotationMatrix = Matrix4.RotationZ(e.Angle - prevPinchAngle);
                this.wallpaper.Transform3D = this.wallpaper.Transform3D.Multiply(rotationMatrix);
                prevPinchAngle = e.Angle;
            }
        }

        void DragHandler(object sender, DragEventArgs e)
        {
            if (pinch.State != GestureDetectorResponse.DetectedAndContinue)
            {
                Vector2 nextPos = new Vector2(wallpaper.X, wallpaper.Y); 
                nextPos += e.Distance;
                if (nextPos.X > 0 && nextPos.Y > 0
                    && nextPos.X < this.Width && nextPos.Y < this.Height)
                {
                    this.wallpaper.SetPosition(nextPos.X, nextPos.Y);
                }
            }
        }
        
        public ImageAsset WallpaperImage
        {
            get
            {
                return this.wallpaper.Image;
            }
            set
            {
                this.wallpaper.Image = value;
            }
        }
        public float WallpaperX
        {
            get {return wallpaper.X; }
        }
        public float WallpaperY
        {
            get {return wallpaper.Y; }
        }
        public float WallpaperWidth
        {
            get {return wallpaper.Width; }
        }
        public float WallpaperHeight
        {
            get {return wallpaper.Height; }
        }
    }
}
