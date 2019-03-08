/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;
using System.Diagnostics;
using System.IO;

namespace CalendarMaker
{
    // show save panel
    public delegate void ShowSavePanelAction();
    public delegate void HideSavePanelAction();
    
    public partial class CalendarMakerScene : Scene
    {
        ExplanationDialog explanationDialog;
        
        // Select Image Mode
        WallpaperSelectionPanel imgSelectionPanel;
        
        // Resize Image Mode
        WallpaperSizingPanel imgSizingPanel;
        
        // Select Calendar Mode
        CalendarTypePanel calTypePanel;
        
        // Locate Calendar Mode
        CalendarPositionPanel calPositionPanel;
        
        // Save Mode
        SavePanel savePanel;
        
        // Current Mode 
        CurrentMode curModeId;

        // Return to MainScene callback
        public Action ReturnMainScene;

        bool isAnimation = false;
        

        enum CurrentMode
        {
            ImageSelection = 0,
            ImageSize = 1,
            CalendarType = 2,
            CalendarPosition =3
        };
        
        public CalendarMakerScene()
        {
            InitializeWidget();
            
            // set current mode
            curModeId = CurrentMode.ImageSelection;

            // set imgSelectionPanel
            imgSelectionPanel = new WallpaperSelectionPanel(GoToNextProcess);
            contentPanel.AddChildLast(imgSelectionPanel);
            imgSelectionPanel.SetSize(contentPanel.Width, contentPanel.Height);


            // Default Mode
            SetImageSelectionMode();
            
            imageSelectionButton.ButtonAction += new EventHandler<TouchEventArgs>(ImageSelectionButtonExecuteAction);
            imageSizeButton.ButtonAction += new EventHandler<TouchEventArgs>(ImageSizeButtonExecuteAction);
            calendarTypeButton.ButtonAction += new EventHandler<TouchEventArgs>(CalendarTypeButtonExecuteAction);
            CalendarPositionButton.ButtonAction += new EventHandler<TouchEventArgs>(CalendarPositionButtonExecuteAction);
            backButton.ButtonAction += new EventHandler<TouchEventArgs>(backButtonExecuteAction);
            forcus.TouchEventReceived += new EventHandler<TouchEventArgs>(TouchForcusEvent);

            contentPanel.Clip = false;
            tabPanel.Clip = false;
        }

        protected override void OnShowing ()
        {
            base.OnShowing ();
            float width = UISystem.FramebufferWidth - tabPanel.Width;
            float height = width / UISystem.FramebufferWidth * UISystem.FramebufferHeight;

            topSpacerImage.Height = (int)((UISystem.FramebufferHeight - height + 0.5f) / 2f);
            bottomSpacerImage.Height = topSpacerImage.Height;
            bottomSpacerImage.Y = UISystem.FramebufferHeight - bottomSpacerImage.Height;

            contentPanel.Width = FMath.Ceiling(width);
            contentPanel.X = UISystem.FramebufferWidth - contentPanel.Width;
            contentPanel.Y = topSpacerImage.Height;
            contentPanel.Height = UISystem.FramebufferHeight - (topSpacerImage.Height) * 2;

            imgSelectionPanel.Init();
        }



        #region Navigation Button
        
        private void TouchForcusEvent(object sender, TouchEventArgs e)
        {
            if(isAnimation) return;

            if(explanationDialog == null)
            {
                explanationDialog = new ExplanationDialog();
            }

            switch(curModeId)
            {
                case CurrentMode.ImageSize:
                    explanationDialog.ImageSizeGuide();
                    break;
                case CurrentMode.CalendarType:
                    explanationDialog.CalendarTypeGuide();
                    break;
                case CurrentMode.CalendarPosition:
                    explanationDialog.CalendarPositionGuide();
                    break;
                default:
                    explanationDialog.ImageSelectionGuide();
                    break;
            }
        }
        
        private void ImageSelectionButtonExecuteAction(object sender, TouchEventArgs e)
        {
            if(curModeId != CurrentMode.ImageSelection && !isAnimation)
            {
                SetImageSelectionMode();
            }
        }
        
        private void ImageSizeButtonExecuteAction(object sender, TouchEventArgs e)
        {
            if(curModeId != CurrentMode.ImageSize && !isAnimation)
            {
                SetImageSizeMode();
            }
        }
        
        private void CalendarTypeButtonExecuteAction(object sender, TouchEventArgs e)
        {
            if(curModeId != CurrentMode.CalendarType && !isAnimation)
            {
                SetCalTypeMode();
            }
        }
        
        private void CalendarPositionButtonExecuteAction(object sender, TouchEventArgs e)
        {
            if(curModeId != CurrentMode.CalendarPosition && !isAnimation)
            {
                SetCalPositionMode();
            }
        }

        private void backButtonExecuteAction(object sender, TouchEventArgs e)
        {
            Debug.Assert(ReturnMainScene != null);
            ReturnMainScene();
        }

        private void DisableAllButton()
        {
            this.imageSelectionButton.Enabled = false;
            this.imageSizeButton.Enabled = false;
            this.calendarTypeButton.Enabled = false;
            this.CalendarPositionButton.Enabled = false;
        }
        
        #endregion
        
        #region Init Each Mode
        
        private void SetImageSelectionMode()
        {
            HideAllPanels();
            DisableAllButton();
            curModeId = CurrentMode.ImageSelection;
            imgSelectionPanel.Visible = true;
            this.imageSelectionButton.Enabled = true;
            ForcusMoveEffectExecute();
        }
        
        private void SetImageSizeMode()
        {
            if (imgSizingPanel == null)
            {
                //set imgSizingPanel
                imgSizingPanel = new WallpaperSizingPanel(GoToNextProcess);
                contentPanel.AddChildLast(imgSizingPanel);
                imgSizingPanel.SetSize(contentPanel.Width, contentPanel.Height);
            }

            HideAllPanels();
            curModeId = CurrentMode.ImageSize;
            imgSizingPanel.WallpaperImage = imgSelectionPanel.SelectedImage;
            imgSizingPanel.Visible = true;
            this.imageSizeButton.Enabled = true;
            ForcusMoveEffectExecute();
        }
        
        private void SetCalTypeMode()
        {
            if (calTypePanel == null)
            {
                // set calTypePanel
                calTypePanel = new CalendarTypePanel(GoToNextProcess);
                contentPanel.AddChildLast(calTypePanel);
                calTypePanel.SetSize(contentPanel.Width, contentPanel.Height);
            }

            HideAllPanels();
            curModeId = CurrentMode.CalendarType;
            calTypePanel.Visible = true;
            this.calendarTypeButton.Enabled = true;
            ForcusMoveEffectExecute();
        }
        
        private void SetCalPositionMode()
        {
            if (calPositionPanel == null)
            {
                // set calPositionPanel
                calPositionPanel = new CalendarPositionPanel(ShowSavePanel);
                contentPanel.AddChildLast(calPositionPanel);
                calPositionPanel.SetSize(contentPanel.Width, contentPanel.Height);
            }

            HideAllPanels();
            curModeId = CurrentMode.CalendarPosition;
            
            // Update Panel Info
            
            calPositionPanel.WallpaperImage = imgSizingPanel.WallpaperImage;
            calPositionPanel.WallpaperX = imgSizingPanel.WallpaperX;
            calPositionPanel.WallpaperY = imgSizingPanel.WallpaperY;
            calPositionPanel.WallpaperWidth = imgSizingPanel.WallpaperWidth;
            calPositionPanel.WallpaperHeight = imgSizingPanel.WallpaperHeight;

            calPositionPanel.CalendarImage = calTypePanel.CalendarImage;
            calPositionPanel.CalendarWidth = calTypePanel.CalendarWidth;
            calPositionPanel.CalendarHeight = calTypePanel.CalendarHeight;
            calPositionPanel.Visible = true;
            this.CalendarPositionButton.Enabled = true;
            ForcusMoveEffectExecute();
        }
        #endregion
        
        private void HideAllPanels()
        {
            if (imgSelectionPanel != null)
                imgSelectionPanel.Visible = false;
            if (imgSizingPanel != null)
                imgSizingPanel.Visible = false;
            if (calTypePanel != null)
                calTypePanel.Visible = false;
            if (calPositionPanel != null)
                calPositionPanel.Visible = false;
        }
        
        private void GoToNextProcess()
        {
            if(isAnimation) return;

            if (curModeId != CurrentMode.CalendarPosition)
            {
                curModeId++;
            }
            
            Widget prevPanel = new Widget();
            Widget nextPanel = new Widget();
            
            switch (curModeId)
            {
            case CurrentMode.ImageSize:
                SetImageSizeMode();
                prevPanel = imgSelectionPanel;
                nextPanel = imgSizingPanel;
                break;
                
            case CurrentMode.CalendarType:
                SetCalTypeMode();
                prevPanel = imgSizingPanel;
                nextPanel = calTypePanel;
                break;
                
            case CurrentMode.CalendarPosition:
                SetCalPositionMode();
                prevPanel = calTypePanel;
                nextPanel = calPositionPanel;
                break;
                
            default:
                Debug.Assert(false);
                break;
            }
            Debug.Assert(prevPanel != null && nextPanel != null);
            MoveEffectExecute(nextPanel, prevPanel);
        }
        
        private void ShowSavePanel()
        {
#if ENABLE_PB_SAVE
            var panel = new Panel();
            panel.SetSize(UISystem.FramebufferWidth, UISystem.FramebufferHeight);
            panel.BackgroundColor = new UIColor(0, 0, 0, 1);
            float scale = UISystem.FramebufferWidth / calPositionPanel.Width;
            var wallImage = new ImageBox();
            wallImage.PivotType = PivotType.MiddleCenter;
            wallImage.ImageScaleType = ImageScaleType.Stretch;
            wallImage.X = calPositionPanel.WallpaperX * scale;
            wallImage.Y = calPositionPanel.WallpaperY * scale;
            wallImage.Width = calPositionPanel.WallpaperWidth * scale;
            wallImage.Height = calPositionPanel.WallpaperHeight * scale;
            wallImage.Image = calPositionPanel.WallpaperImage;
            var calImage = new ImageBox();
            calImage.X = calPositionPanel.CalendarX * scale;
            calImage.Y = calPositionPanel.CalendarY * scale;
            calImage.Width = calPositionPanel.CalendarWidth * scale;
            calImage.Height = calPositionPanel.CalendarHeight * scale;
            calImage.Image = calPositionPanel.CalendarImage;
            panel.AddChildLast(wallImage);
            panel.AddChildLast(calImage);

            ImageManager.SaveImage(panel);

            ReturnMainScene();
#else
            if (savePanel == null)
            {
                // Init SavePanel
                savePanel = new SavePanel();
                //savePanel.Visible = false;
                this.RootWidget.AddChildLast(savePanel);
                savePanel.SetPosition((UISystem.FramebufferWidth - savePanel.Width) / 2f, (UISystem.FramebufferHeight - savePanel.Height) / 2f);
            }
            savePanel.Start();
#endif
        }
        

        #region Animation
        private void MoveEffectExecute(Widget inWidget, Widget outWidget)
        {
            float time = 500;

            var slideIn = new SlideInEffect(inWidget, time,
                                            FourWayDirection.Up,
                                            SlideInEffectInterpolator.EaseOutQuad);
            var slideOut = new SlideOutEffect(outWidget, time,
                                              FourWayDirection.Up,
                                              SlideOutEffectInterpolator.EaseOutQuad);

            Debug.Assert(!isAnimation);
            isAnimation = true;
            slideIn.EffectStopped += (sender, e) => isAnimation = false;

            slideIn.Start();
            slideOut.Start();
        }
        
        private void ForcusMoveEffectExecute()
        {
            Widget target;
            switch(curModeId)
            {
            case CurrentMode.ImageSize:
               target = imageSizeButton;
                break;
                
            case CurrentMode.CalendarType:
                target = calendarTypeButton;
                break;
                
            case CurrentMode.CalendarPosition:
                target = CalendarPositionButton;
                break;
                
            default:
                target = imageSelectionButton;
                break;
            }//            
            MoveEffect moveEffect = new MoveEffect(forcus, 300, target.X, target.Y, MoveEffectInterpolator.EaseOutQuad);
            moveEffect.Start();
        }
        
        #endregion
    }
}
