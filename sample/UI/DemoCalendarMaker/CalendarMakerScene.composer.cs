// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class CalendarMakerScene
    {
        Panel sceneBackgroundPanel;
        ImageBox backgroundImage;
        Panel contentPanel;
        ImageBox bottomSpacerImage;
        ImageBox topSpacerImage;
        Button imageSizeButton;
        Button imageSelectionButton;
        Button calendarTypeButton;
        Button CalendarPositionButton;
        Button backButton;
        ImageBox forcus;
        Panel tabPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            backgroundImage = new ImageBox();
            backgroundImage.Name = "backgroundImage";
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            bottomSpacerImage = new ImageBox();
            bottomSpacerImage.Name = "bottomSpacerImage";
            topSpacerImage = new ImageBox();
            topSpacerImage.Name = "topSpacerImage";
            imageSizeButton = new Button();
            imageSizeButton.Name = "imageSizeButton";
            imageSelectionButton = new Button();
            imageSelectionButton.Name = "imageSelectionButton";
            calendarTypeButton = new Button();
            calendarTypeButton.Name = "calendarTypeButton";
            CalendarPositionButton = new Button();
            CalendarPositionButton.Name = "CalendarPositionButton";
            backButton = new Button();
            backButton.Name = "backButton";
            forcus = new ImageBox();
            forcus.Name = "forcus";
            tabPanel = new Panel();
            tabPanel.Name = "tabPanel";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // backgroundImage
            backgroundImage.Image = new ImageAsset("/Application/assets/main_background.png");

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            contentPanel.Clip = true;

            // bottomSpacerImage
            bottomSpacerImage.Image = new ImageAsset("/Application/assets/spacer.png");
            bottomSpacerImage.ImageScaleType = ImageScaleType.NinePatch;
            bottomSpacerImage.NinePatchMargin = new NinePatchMargin(3, 3, 3, 3);

            // topSpacerImage
            topSpacerImage.Image = new ImageAsset("/Application/assets/spacer.png");
            topSpacerImage.ImageScaleType = ImageScaleType.NinePatch;
            topSpacerImage.NinePatchMargin = new NinePatchMargin(3, 3, 3, 3);

            // imageSizeButton
            imageSizeButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            imageSizeButton.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            imageSizeButton.Style = ButtonStyle.Custom;
            imageSizeButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/resize_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/resize_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/resize_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // imageSelectionButton
            imageSelectionButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            imageSelectionButton.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            imageSelectionButton.Style = ButtonStyle.Custom;
            imageSelectionButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/selection_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/selection_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/selection_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // calendarTypeButton
            calendarTypeButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            calendarTypeButton.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            calendarTypeButton.Style = ButtonStyle.Custom;
            calendarTypeButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/type_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/type_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/type_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // CalendarPositionButton
            CalendarPositionButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            CalendarPositionButton.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            CalendarPositionButton.Style = ButtonStyle.Custom;
            CalendarPositionButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/position_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/position_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/position_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // backButton
            backButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            backButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            backButton.Style = ButtonStyle.Custom;
            backButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/back_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/back_button_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/back_button_normal.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // forcus
            forcus.Image = new ImageAsset("/Application/assets/forcus.png");

            // tabPanel
            tabPanel.BackgroundColor = new UIColor(238f / 255f, 238f / 255f, 238f / 255f, 204f / 255f);
            tabPanel.Clip = true;
            tabPanel.AddChildLast(imageSizeButton);
            tabPanel.AddChildLast(imageSelectionButton);
            tabPanel.AddChildLast(calendarTypeButton);
            tabPanel.AddChildLast(CalendarPositionButton);
            tabPanel.AddChildLast(backButton);
            tabPanel.AddChildLast(forcus);

            // CalendarMakerScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(backgroundImage);
            this.RootWidget.AddChildLast(contentPanel);
            this.RootWidget.AddChildLast(bottomSpacerImage);
            this.RootWidget.AddChildLast(topSpacerImage);
            this.RootWidget.AddChildLast(tabPanel);
            this.Transition = new PushTransition();

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 480;
                    this.DesignHeight = 854;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(480, 854);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    backgroundImage.SetPosition(0, 0);
                    backgroundImage.SetSize(200, 200);
                    backgroundImage.Anchors = Anchors.None;
                    backgroundImage.Visible = true;

                    contentPanel.SetPosition(126, 28);
                    contentPanel.SetSize(100, 100);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    bottomSpacerImage.SetPosition(100, 424);
                    bottomSpacerImage.SetSize(200, 200);
                    bottomSpacerImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bottomSpacerImage.Visible = true;

                    topSpacerImage.SetPosition(100, 424);
                    topSpacerImage.SetSize(200, 200);
                    topSpacerImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    topSpacerImage.Visible = true;

                    imageSizeButton.SetPosition(-32, 170);
                    imageSizeButton.SetSize(214, 56);
                    imageSizeButton.Anchors = Anchors.None;
                    imageSizeButton.Visible = true;

                    imageSelectionButton.SetPosition(232, 78);
                    imageSelectionButton.SetSize(214, 56);
                    imageSelectionButton.Anchors = Anchors.None;
                    imageSelectionButton.Visible = true;

                    calendarTypeButton.SetPosition(232, 78);
                    calendarTypeButton.SetSize(214, 56);
                    calendarTypeButton.Anchors = Anchors.None;
                    calendarTypeButton.Visible = true;

                    CalendarPositionButton.SetPosition(-32, 170);
                    CalendarPositionButton.SetSize(214, 56);
                    CalendarPositionButton.Anchors = Anchors.None;
                    CalendarPositionButton.Visible = true;

                    backButton.SetPosition(20, -14);
                    backButton.SetSize(214, 56);
                    backButton.Anchors = Anchors.None;
                    backButton.Visible = true;

                    forcus.SetPosition(259, 180);
                    forcus.SetSize(200, 200);
                    forcus.Anchors = Anchors.None;
                    forcus.Visible = true;

                    tabPanel.SetPosition(0, 0);
                    tabPanel.SetSize(100, 100);
                    tabPanel.Anchors = Anchors.None;
                    tabPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(854, 480);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    backgroundImage.SetPosition(0, 0);
                    backgroundImage.SetSize(854, 480);
                    backgroundImage.Anchors = Anchors.None;
                    backgroundImage.Visible = true;

                    contentPanel.SetPosition(100, 28);
                    contentPanel.SetSize(754, 424);
                    contentPanel.Anchors = Anchors.None;
                    contentPanel.Visible = true;

                    bottomSpacerImage.SetPosition(100, 452);
                    bottomSpacerImage.SetSize(754, 28);
                    bottomSpacerImage.Anchors = Anchors.None;
                    bottomSpacerImage.Visible = true;

                    topSpacerImage.SetPosition(100, 0);
                    topSpacerImage.SetSize(754, 28);
                    topSpacerImage.Anchors = Anchors.Top;
                    topSpacerImage.Visible = true;

                    imageSizeButton.SetPosition(0, 96);
                    imageSizeButton.SetSize(100, 96);
                    imageSizeButton.Anchors = Anchors.None;
                    imageSizeButton.Visible = true;

                    imageSelectionButton.SetPosition(0, 0);
                    imageSelectionButton.SetSize(100, 96);
                    imageSelectionButton.Anchors = Anchors.None;
                    imageSelectionButton.Visible = true;

                    calendarTypeButton.SetPosition(0, 192);
                    calendarTypeButton.SetSize(100, 96);
                    calendarTypeButton.Anchors = Anchors.None;
                    calendarTypeButton.Visible = true;

                    CalendarPositionButton.SetPosition(0, 288);
                    CalendarPositionButton.SetSize(100, 96);
                    CalendarPositionButton.Anchors = Anchors.None;
                    CalendarPositionButton.Visible = true;

                    backButton.SetPosition(0, 384);
                    backButton.SetSize(100, 96);
                    backButton.Anchors = Anchors.None;
                    backButton.Visible = true;

                    forcus.SetPosition(0, 0);
                    forcus.SetSize(100, 96);
                    forcus.Anchors = Anchors.None;
                    forcus.Visible = true;

                    tabPanel.SetPosition(0, 0);
                    tabPanel.SetSize(100, 480);
                    tabPanel.Anchors = Anchors.None;
                    tabPanel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

    }
}
