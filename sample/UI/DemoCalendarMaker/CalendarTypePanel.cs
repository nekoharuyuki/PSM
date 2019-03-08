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
    public partial class CalendarTypePanel : Panel
    {
        private CompleteModeAction itemClickAction;
        private float selectedCalendarWidth;
        private float selectedCalendarHeight;
        private ImageAsset selectedCalendarImage;
  
        public CalendarTypePanel(CompleteModeAction action)
        {
            InitializeWidget();

            this.Clip = false;
            calTypeOne.TouchEventReceived += new EventHandler<TouchEventArgs>(TouchEvent);
            calTypeTwo.TouchEventReceived += new EventHandler<TouchEventArgs>(TouchEvent);
            Button_1.TouchEventReceived += new EventHandler<TouchEventArgs>(TouchEvent);
            Button_2.TouchEventReceived += new EventHandler<TouchEventArgs>(TouchEvent);
            itemClickAction = action;
        }
  
        void TouchEvent(object sender, TouchEventArgs e)
        {
            TouchEvent touchEvent = e.TouchEvents.PrimaryTouchEvent;
            if(touchEvent.Type == TouchEventType.Up)
            {
                if(sender == calTypeOne || sender == Button_1)
                {
                    if(0 < touchEvent.LocalPosition.X && touchEvent.LocalPosition.X < calTypeOne.Width && 
                       0 < touchEvent.LocalPosition.Y && touchEvent.LocalPosition.Y < calTypeOne.Height)
                    {
                        SetSelectedCalendarType(calTypeOne);
                        itemClickAction();
                    }
                }
                else if(sender == calTypeTwo || sender == Button_2)
                {
                    if(0 < touchEvent.LocalPosition.X && touchEvent.LocalPosition.X < calTypeTwo.Width && 
                       0 < touchEvent.LocalPosition.Y && touchEvent.LocalPosition.Y < calTypeTwo.Height)
                    {
                        SetSelectedCalendarType(calTypeTwo);
                        itemClickAction();
                    }
                }
                else
                {
                }
            }
        }
        
        public ImageAsset CalendarImage
        {
            get { return selectedCalendarImage; }
        }
        public float CalendarWidth
        {
            get{return selectedCalendarWidth;}
        }
        public float CalendarHeight
        {
            get{return selectedCalendarHeight;}
        }
        
        private void SetSelectedCalendarType(ImageBox calImageBox)
        {
            selectedCalendarImage = calImageBox.Image;
            selectedCalendarWidth = calImageBox.Width;
            selectedCalendarHeight = calImageBox.Height;
        }
    }
}
