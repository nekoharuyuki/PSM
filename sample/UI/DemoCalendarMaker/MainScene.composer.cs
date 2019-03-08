// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class MainScene
    {
        ImageBox ImageBox_1;
        ImageBox ImageBox_2;
        Button Button_1;
        Button Button_2;
        Panel newsTickerPanel;
        ImageBox ImageBox_3;
        ImageBox slideShowbackImage;
        ImageBox slideShowImage;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";
            newsTickerPanel = new Panel();
            newsTickerPanel.Name = "newsTickerPanel";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            slideShowbackImage = new ImageBox();
            slideShowbackImage.Name = "slideShowbackImage";
            slideShowImage = new ImageBox();
            slideShowImage.Name = "slideShowImage";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/main_background.png");

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/menu_window_1.png");
            ImageBox_2.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_2.NinePatchMargin = new NinePatchMargin(16, 16, 16, 16);

            // Button_1
            Button_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/mc_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/mc_button_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/mc_button_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // Button_2
            Button_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_2.Style = ButtonStyle.Custom;
            Button_2.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/sw_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/sw_button_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/sw_button_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // newsTickerPanel
            newsTickerPanel.BackgroundColor = new UIColor(0f / 255f, 48f / 255f, 64f / 255f, 127f / 255f);
            newsTickerPanel.Clip = true;

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/menu_window_1.png");
            ImageBox_3.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_3.NinePatchMargin = new NinePatchMargin(16, 16, 16, 16);

            // slideShowbackImage
            slideShowbackImage.Image = null;

            // slideShowImage
            slideShowImage.Image = new ImageAsset("/Application/assets/wallpaper01.jpg");

            // MainScene
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(ImageBox_2);
            this.RootWidget.AddChildLast(Button_1);
            this.RootWidget.AddChildLast(Button_2);
            this.RootWidget.AddChildLast(newsTickerPanel);
            this.RootWidget.AddChildLast(ImageBox_3);
            this.RootWidget.AddChildLast(slideShowbackImage);
            this.RootWidget.AddChildLast(slideShowImage);

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

                    ImageBox_1.SetPosition(-58, -6);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(201, 7);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Button_1.SetPosition(469, 269);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(499, 251);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    newsTickerPanel.SetPosition(527, 364);
                    newsTickerPanel.SetSize(100, 100);
                    newsTickerPanel.Anchors = Anchors.None;
                    newsTickerPanel.Visible = true;

                    ImageBox_3.SetPosition(477, 11);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    slideShowbackImage.SetPosition(259, 86);
                    slideShowbackImage.SetSize(200, 200);
                    slideShowbackImage.Anchors = Anchors.None;
                    slideShowbackImage.Visible = true;

                    slideShowImage.SetPosition(-314, -283);
                    slideShowImage.SetSize(200, 200);
                    slideShowImage.Anchors = Anchors.None;
                    slideShowImage.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(854, 480);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(8, 200);
                    ImageBox_2.SetSize(224, 260);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Button_1.SetPosition(10, 208);
                    Button_1.SetSize(216, 120);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(10, 328);
                    Button_2.SetSize(216, 120);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    newsTickerPanel.SetPosition(342, 388);
                    newsTickerPanel.SetSize(490, 70);
                    newsTickerPanel.Anchors = Anchors.None;
                    newsTickerPanel.Visible = true;

                    ImageBox_3.SetPosition(240, 72);
                    ImageBox_3.SetSize(398, 232);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    slideShowbackImage.SetPosition(259, 86);
                    slideShowbackImage.SetSize(356, 200);
                    slideShowbackImage.Anchors = Anchors.None;
                    slideShowbackImage.Visible = true;

                    slideShowImage.SetPosition(259, 86);
                    slideShowImage.SetSize(356, 200);
                    slideShowImage.Anchors = Anchors.None;
                    slideShowImage.Visible = true;

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
