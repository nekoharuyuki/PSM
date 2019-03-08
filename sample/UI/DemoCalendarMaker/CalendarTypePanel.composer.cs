// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class CalendarTypePanel
    {
        ImageBox ImageBox_1;
        ImageBox calTypeOne;
        ImageBox calTypeTwo;
        Button Button_1;
        Button Button_2;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            calTypeOne = new ImageBox();
            calTypeOne.Name = "calTypeOne";
            calTypeTwo = new ImageBox();
            calTypeTwo.Name = "calTypeTwo";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/main_background.png");

            // calTypeOne
            calTypeOne.Image = new ImageAsset("/Application/assets/cal_2011_6.png");

            // calTypeTwo
            calTypeTwo.Image = new ImageAsset("/Application/assets/cal_2011_6_7.png");

            // Button_1
            Button_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/one_month_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/one_month_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/one_month_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // Button_2
            Button_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_2.Style = ButtonStyle.Custom;
            Button_2.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/two_months_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/two_months_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/two_months_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // CalendarTypePanel
            this.BackgroundColor = new UIColor(236f / 255f, 240f / 255f, 244f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(calTypeOne);
            this.AddChildLast(calTypeTwo);
            this.AddChildLast(Button_1);
            this.AddChildLast(Button_2);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(-100, 0);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    calTypeOne.SetPosition(-47, -88);
                    calTypeOne.SetSize(200, 200);
                    calTypeOne.Anchors = Anchors.None;
                    calTypeOne.Visible = true;

                    calTypeTwo.SetPosition(191, 121);
                    calTypeTwo.SetSize(200, 200);
                    calTypeTwo.Anchors = Anchors.None;
                    calTypeTwo.Visible = true;

                    Button_1.SetPosition(12, 57);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(395, 64);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    break;

                default:
                    this.SetSize(754, 480);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(-100, 0);
                    ImageBox_1.SetSize(854, 480);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    calTypeOne.SetPosition(32, 162);
                    calTypeOne.SetSize(240, 192);
                    calTypeOne.Anchors = Anchors.None;
                    calTypeOne.Visible = true;

                    calTypeTwo.SetPosition(280, 160);
                    calTypeTwo.SetSize(448, 200);
                    calTypeTwo.Anchors = Anchors.None;
                    calTypeTwo.Visible = true;

                    Button_1.SetPosition(32, 112);
                    Button_1.SetSize(244, 32);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(376, 112);
                    Button_2.SetSize(264, 32);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public void StartDefaultEffect()
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
