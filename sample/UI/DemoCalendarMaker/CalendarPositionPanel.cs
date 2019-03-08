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
    public partial class CalendarPositionPanel : Panel
    {
        TapGestureDetector singleTap;
        DragGestureDetector drag;
        PinchGestureDetector pinch;
        
        // SaveAction
        private ShowSavePanelAction showSavePanelAction;
        
        ImageAsset wallpaperImage;
        ImageAsset calendarImage;
        
        public CalendarPositionPanel(ShowSavePanelAction action)
        {
            InitializeWidget();

            this.Clip = false;
            this.HookChildTouchEvent = true;

            // Init Single Tap
            singleTap = new TapGestureDetector();
            singleTap.TapDetected += new EventHandler<TapEventArgs>(SingleTapHandler);
            
            // Init Drag
            drag = new DragGestureDetector();
            drag.DragDetected += new EventHandler<DragEventArgs>(DragHandler);
            
            // Init Pinch
            pinch = new PinchGestureDetector();
            pinch.PinchDetected += new EventHandler<PinchEventArgs>(PinchHandler);
            
            // Add Gesture
            AddGestureDetector(pinch);
            AddGestureDetector(drag);
            AddGestureDetector(singleTap);
            
            // Init Dialog
            showSavePanelAction = action;

            wallpaper.PivotType = PivotType.MiddleCenter;
            
            Button_1.ButtonAction += new EventHandler<TouchEventArgs>(okButtonExecuteAction);
        }
        
        void okButtonExecuteAction(object sender, EventArgs e)
        {
            var dialog = new CompleteDialog(showSavePanelAction);
            dialog.Show();
        }
        
        void PinchHandler(object sender, PinchEventArgs e)
        {
        }
        
        void DragHandler(object sender, DragEventArgs e)
        {
                float currentX = calendar.X;
                float currentY = calendar.Y;
                this.calendar.SetPosition(currentX+e.Distance.X, currentY+e.Distance.Y);
        }
        
        void SingleTapHandler(object sender, TapEventArgs e)
        {
        }
        
        public ImageAsset WallpaperImage
        {
            get
            {
                return wallpaperImage;
            }
            
            set
            {
                wallpaperImage = value;
                this.wallpaper.Image = value;
            }
        }
        
        public float WallpaperX
        {
            get { return wallpaper.X; }
            set { wallpaper.X = value; }
        }
        
        public float WallpaperY
        {
            get { return wallpaper.Y; }
            set { wallpaper.Y = value; }
        }
        
        public float WallpaperWidth
        {
            get { return wallpaper.Width; }
            set { wallpaper.Width = value; }
        }
        
        public float WallpaperHeight
        {
            get { return wallpaper.Height; }
            set { wallpaper.Height = value; }
        }

        public ImageAsset CalendarImage
        {
            get
            {
                return calendarImage;
            }
            set
            {
                calendarImage = value;
                this.calendar.Image = value;
            }
        }
        public float CalendarX
        {
            get { return this.calendar.X; }
            set { this.calendar.X = value; }
        }
        public float CalendarY
        {
            get { return this.calendar.Y; }
            set { this.calendar.Y = value; }
        }
        public float CalendarWidth
        {
            get { return this.calendar.Width; }
            set { this.calendar.Width = value; }
        }
        public float CalendarHeight
        {
            get { return this.calendar.Height; }
            set { this.calendar.Height = value; }
        }
        public Button OkButton
        {
            get
            {
                return Button_1;
            }
        }
    }
}
