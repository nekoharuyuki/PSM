// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    partial class MainScene
    {
        ImageBox ImageBox_1;
        PagePanel PagePanel_1;
        Button Button_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            PagePanel_1 = new PagePanel();
            PagePanel_1.Name = "PagePanel_1";
            Button_1 = new Button();
            Button_1.Name = "Button_1";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/bg.png");

            // Button_1
            Button_1.IconImage = null;
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/setting_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/setting_press.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/setting_press.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // MainScene
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(PagePanel_1);
            this.RootWidget.AddChildLast(Button_1);

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

                    ImageBox_1.SetPosition(65, 14);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = false;

                    PagePanel_1.SetPosition(287, 93);
                    PagePanel_1.SetSize(100, 50);
                    PagePanel_1.Anchors = Anchors.None;
                    PagePanel_1.Visible = false;

                    Button_1.SetPosition(651, 399);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = false;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(854, 480);
                    ImageBox_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    ImageBox_1.Visible = true;

                    PagePanel_1.SetPosition(0, 0);
                    PagePanel_1.SetSize(854, 480);
                    PagePanel_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    PagePanel_1.Visible = true;

                    Button_1.SetPosition(762, 402);
                    Button_1.SetSize(82, 68);
                    Button_1.Anchors = Anchors.Bottom | Anchors.Height | Anchors.Right | Anchors.Width;
                    Button_1.Visible = true;

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
