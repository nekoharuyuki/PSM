// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class WallpaperViewerScene
    {
        Panel sceneBackgroundPanel;
        ImageBox ImageBox_2;
        ImageBox ImageBox_1;
        GridListPanel GridListPanel_1;
        Button backButton;
        ImageBox ImageBox_3;
        ImageBox ImageBox_4;
        ImageBox ImageBox_5;
        ImageBox ImageBox_6;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            GridListPanel_1 = new GridListPanel(GridListScrollOrientation.Horizontal);
            GridListPanel_1.Name = "GridListPanel_1";
            backButton = new Button();
            backButton.Name = "backButton";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            ImageBox_4 = new ImageBox();
            ImageBox_4.Name = "ImageBox_4";
            ImageBox_5 = new ImageBox();
            ImageBox_5.Name = "ImageBox_5";
            ImageBox_6 = new ImageBox();
            ImageBox_6.Name = "ImageBox_6";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/main_background.png");

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/frame.png");
            ImageBox_1.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_1.NinePatchMargin = new NinePatchMargin(40, 30, 40, 30);

            // GridListPanel_1
            GridListPanel_1.ScrollBarVisibility = ScrollBarVisibility.Invisible;
            GridListPanel_1.SetListItemCreator(WallpaperGalleryItem.Creator);

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

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/panel_1.png");

            // ImageBox_4
            ImageBox_4.Image = new ImageAsset("/Application/assets/panel_1.png");

            // ImageBox_5
            ImageBox_5.Image = new ImageAsset("/Application/assets/panel_1.png");

            // ImageBox_6
            ImageBox_6.Image = new ImageAsset("/Application/assets/panel_1.png");

            // WallpaperViewerScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(ImageBox_2);
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(GridListPanel_1);
            this.RootWidget.AddChildLast(backButton);
            this.RootWidget.AddChildLast(ImageBox_3);
            this.RootWidget.AddChildLast(ImageBox_4);
            this.RootWidget.AddChildLast(ImageBox_5);
            this.RootWidget.AddChildLast(ImageBox_6);
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
                    this.DesignWidth = 544;
                    this.DesignHeight = 960;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(544, 960);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    ImageBox_2.SetPosition(0, 0);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    ImageBox_1.SetPosition(266, 0);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    GridListPanel_1.SetPosition(341, 134);
                    GridListPanel_1.SetSize(100, 50);
                    GridListPanel_1.Anchors = Anchors.None;
                    GridListPanel_1.Visible = true;
                    GridListPanel_1.ItemWidth = 150;
                    GridListPanel_1.ItemHeight = 255;

                    backButton.SetPosition(20, -14);
                    backButton.SetSize(214, 56);
                    backButton.Anchors = Anchors.None;
                    backButton.Visible = true;

                    ImageBox_3.SetPosition(-44, -47);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(0, 96);
                    ImageBox_4.SetSize(200, 200);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = true;

                    ImageBox_5.SetPosition(0, 192);
                    ImageBox_5.SetSize(200, 200);
                    ImageBox_5.Anchors = Anchors.None;
                    ImageBox_5.Visible = true;

                    ImageBox_6.SetPosition(0, 284);
                    ImageBox_6.SetSize(200, 200);
                    ImageBox_6.Anchors = Anchors.None;
                    ImageBox_6.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(854, 480);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    ImageBox_2.SetPosition(0, 0);
                    ImageBox_2.SetSize(854, 480);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    ImageBox_1.SetPosition(116, 32);
                    ImageBox_1.SetSize(658, 416);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    GridListPanel_1.SetPosition(148, 68);
                    GridListPanel_1.SetSize(596, 344);
                    GridListPanel_1.Anchors = Anchors.None;
                    GridListPanel_1.Visible = true;
                    GridListPanel_1.ItemWidth = 120;
                    GridListPanel_1.ItemHeight = 120;

                    backButton.SetPosition(0, 380);
                    backButton.SetSize(100, 100);
                    backButton.Anchors = Anchors.None;
                    backButton.Visible = true;

                    ImageBox_3.SetPosition(0, 0);
                    ImageBox_3.SetSize(100, 96);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(0, 96);
                    ImageBox_4.SetSize(100, 96);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = true;

                    ImageBox_5.SetPosition(0, 192);
                    ImageBox_5.SetSize(100, 96);
                    ImageBox_5.Anchors = Anchors.None;
                    ImageBox_5.Visible = true;

                    ImageBox_6.SetPosition(0, 288);
                    ImageBox_6.SetSize(100, 96);
                    ImageBox_6.Anchors = Anchors.None;
                    ImageBox_6.Visible = true;

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
