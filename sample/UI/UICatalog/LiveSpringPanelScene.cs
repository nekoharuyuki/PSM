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

namespace UICatalog
{
    public partial class LiveSpringPanelScene : ContentsScene
    {
        Vector2 panelDragStartPos = Vector2.Zero;
        Vector2 sphereDragStartPos = Vector2.Zero;
        Vector2 sphereDisplacement = Vector2.Zero;

        ImageAsset normalPanelIamge;
        ImageAsset pressedPanelIamge;

        public LiveSpringPanelScene()
        {
            InitializeWidget();

            this.Name = "LiveSpringPanel";

            normalPanelIamge = new ImageAsset("/Application/assets/spring_panel_normal.png");
            pressedPanelIamge =  new ImageAsset("/Application/assets/spring_panel_pressed.png");

            // Setup LiveSpringPanel
            DragGestureDetector panelDrag = new DragGestureDetector();
            panelDrag.DragStartDetected += PanelStartDragEventHandler;
            panelDrag.DragDetected += PanelDragEventHandler;
            springPanel.AddGestureDetector(panelDrag);
            springPanel.TouchEventReceived += PanelTouchEventHandler;
            springPanel.SetSpringConstant(null, SpringType.PositionZ, 0.5f);

            // Setup background of LiveSpringPanel 
            background.ZSort = false;
            background.TouchResponse = false;
            springPanel.SetSpringConstant(background, SpringType.All, 1.0f);

            // Setup LiveSphere
            DragGestureDetector sphereDrag = new DragGestureDetector();
            sphereDrag.DragStartDetected += SphereStartDragEventHandler;
            sphereDrag.DragDetected += SphereDragEventHandler;
            sphere.AddGestureDetector(sphereDrag);
            sphere.TouchEnabled = false;
        }

        void PanelTouchEventHandler(object sender, TouchEventArgs e)
        {
            TouchEvent touchEvent = e.TouchEvents.PrimaryTouchEvent;
            if (touchEvent.Type == TouchEventType.Down)
            {
                background.Image = pressedPanelIamge;
            }
            else if(touchEvent.Type == TouchEventType.Up)
            {
                background.Image = normalPanelIamge;
            }
        }

        void PanelStartDragEventHandler(object sender, DragEventArgs e)
        {
            panelDragStartPos = e.LocalPosition;
        }

        void PanelDragEventHandler(object sender, DragEventArgs e)
        {
            Vector2 diff = e.LocalPosition - panelDragStartPos;
            springPanel.X += diff.X;
            springPanel.Y += diff.Y;
            
            if (springPanel.X < 0)
            {
                springPanel.X = 0;
            }
            
            if (springPanel.X + springPanel.Width > this.RootWidget.Width)
            {
                springPanel.X = this.RootWidget.Width - springPanel.Width;
            }
            
            if (springPanel.Y < ControlPanel.Y + ControlPanel.Height)
            {
                springPanel.Y = ControlPanel.Y + ControlPanel.Height;
            }
            
            if (springPanel.Y + springPanel.Height > this.RootWidget.Height)
            {
                springPanel.Y = this.RootWidget.Height - springPanel.Height;
            }
        }

        void SphereStartDragEventHandler(object sender, DragEventArgs e)
        {
            sphereDragStartPos = e.LocalPosition;
            sphereDisplacement = Vector2.Zero;
        }

        void SphereDragEventHandler(object sender, DragEventArgs e)
        {
            sphereDisplacement += e.LocalPosition - sphereDragStartPos;
            springPanel.SetDisplacement(sphere, SpringType.PositionX, sphereDisplacement.X);
            springPanel.SetDisplacement(sphere, SpringType.PositionY, sphereDisplacement.Y);
        }
    }
}
