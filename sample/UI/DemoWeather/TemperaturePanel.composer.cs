// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Weather
{
    partial class TemperaturePanel
    {
        ImageBox BgImage;
        Label curDate;
        Label cityName;
        Label highTemperature;
        Label diagonal;
        Label lowTemperature;
        ImageBox weatherImage;
        Button closeButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            BgImage = new ImageBox();
            BgImage.Name = "BgImage";
            curDate = new Label();
            curDate.Name = "curDate";
            cityName = new Label();
            cityName.Name = "cityName";
            highTemperature = new Label();
            highTemperature.Name = "highTemperature";
            diagonal = new Label();
            diagonal.Name = "diagonal";
            lowTemperature = new Label();
            lowTemperature.Name = "lowTemperature";
            weatherImage = new ImageBox();
            weatherImage.Name = "weatherImage";
            closeButton = new Button();
            closeButton.Name = "closeButton";

            // BgImage
            BgImage.Image = new ImageAsset("/Application/assets/panel_9patch.png");
            BgImage.ImageScaleType = ImageScaleType.NinePatch;
            BgImage.NinePatchMargin = new NinePatchMargin(32, 32, 32, 32);

            // curDate
            curDate.TextColor = new UIColor(64f / 255f, 40f / 255f, 16f / 255f, 255f / 255f);
            curDate.Font = new UIFont(FontAlias.System, 32, FontStyle.Regular);
            curDate.LineBreak = LineBreak.Character;
            curDate.HorizontalAlignment = HorizontalAlignment.Center;
            curDate.VerticalAlignment = VerticalAlignment.Top;

            // cityName
            cityName.TextColor = new UIColor(64f / 255f, 40f / 255f, 16f / 255f, 255f / 255f);
            cityName.Font = new UIFont(FontAlias.System, 72, FontStyle.Regular);
            cityName.LineBreak = LineBreak.Character;
            cityName.HorizontalAlignment = HorizontalAlignment.Center;
            cityName.VerticalAlignment = VerticalAlignment.Top;

            // highTemperature
            highTemperature.TextColor = new UIColor(255f / 255f, 72f / 255f, 64f / 255f, 255f / 255f);
            highTemperature.Font = new UIFont(FontAlias.System, 88, FontStyle.Regular);
            highTemperature.LineBreak = LineBreak.Character;
            highTemperature.HorizontalAlignment = HorizontalAlignment.Center;
            highTemperature.VerticalAlignment = VerticalAlignment.Bottom;

            // diagonal
            diagonal.TextColor = new UIColor(80f / 255f, 80f / 255f, 80f / 255f, 255f / 255f);
            diagonal.Font = new UIFont(FontAlias.System, 88, FontStyle.Regular);
            diagonal.LineBreak = LineBreak.Character;
            diagonal.HorizontalAlignment = HorizontalAlignment.Center;
            diagonal.VerticalAlignment = VerticalAlignment.Bottom;

            // lowTemperature
            lowTemperature.TextColor = new UIColor(64f / 255f, 160f / 255f, 224f / 255f, 255f / 255f);
            lowTemperature.Font = new UIFont(FontAlias.System, 88, FontStyle.Regular);
            lowTemperature.LineBreak = LineBreak.Character;
            lowTemperature.HorizontalAlignment = HorizontalAlignment.Center;
            lowTemperature.VerticalAlignment = VerticalAlignment.Bottom;

            // weatherImage
            weatherImage.Image = new ImageAsset("/Application/assets/1.png");

            // closeButton
            closeButton.IconImage = null;
            closeButton.Style = ButtonStyle.Custom;
            closeButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/close_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/close_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/close_normal.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // TemperaturePanel
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(BgImage);
            this.AddChildLast(curDate);
            this.AddChildLast(cityName);
            this.AddChildLast(highTemperature);
            this.AddChildLast(diagonal);
            this.AddChildLast(lowTemperature);
            this.AddChildLast(weatherImage);
            this.AddChildLast(closeButton);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(460, 834);
                    this.Anchors = Anchors.None;

                    BgImage.SetPosition(583, 8);
                    BgImage.SetSize(200, 200);
                    BgImage.Anchors = Anchors.None;
                    BgImage.Visible = true;

                    curDate.SetPosition(0, 0);
                    curDate.SetSize(214, 36);
                    curDate.Anchors = Anchors.None;
                    curDate.Visible = true;

                    cityName.SetPosition(0, 0);
                    cityName.SetSize(214, 36);
                    cityName.Anchors = Anchors.None;
                    cityName.Visible = true;

                    highTemperature.SetPosition(128, 400);
                    highTemperature.SetSize(214, 36);
                    highTemperature.Anchors = Anchors.None;
                    highTemperature.Visible = true;

                    diagonal.SetPosition(235, 344);
                    diagonal.SetSize(214, 36);
                    diagonal.Anchors = Anchors.None;
                    diagonal.Visible = true;

                    lowTemperature.SetPosition(128, 400);
                    lowTemperature.SetSize(214, 36);
                    lowTemperature.Anchors = Anchors.None;
                    lowTemperature.Visible = true;

                    weatherImage.SetPosition(0, 0);
                    weatherImage.SetSize(200, 200);
                    weatherImage.Anchors = Anchors.None;
                    weatherImage.Visible = true;

                    closeButton.SetPosition(640, -7);
                    closeButton.SetSize(214, 56);
                    closeButton.Anchors = Anchors.None;
                    closeButton.Visible = true;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    BgImage.SetPosition(151, 20);
                    BgImage.SetSize(552, 440);
                    BgImage.Anchors = Anchors.None;
                    BgImage.Visible = true;

                    curDate.SetPosition(225, 29);
                    curDate.SetSize(400, 50);
                    curDate.Anchors = Anchors.None;
                    curDate.Visible = true;

                    cityName.SetPosition(160, 60);
                    cityName.SetSize(533, 150);
                    cityName.Anchors = Anchors.None;
                    cityName.Visible = true;

                    highTemperature.SetPosition(160, 300);
                    highTemperature.SetSize(242, 150);
                    highTemperature.Anchors = Anchors.None;
                    highTemperature.Visible = true;

                    diagonal.SetPosition(400, 300);
                    diagonal.SetSize(47, 150);
                    diagonal.Anchors = Anchors.None;
                    diagonal.Visible = true;

                    lowTemperature.SetPosition(447, 300);
                    lowTemperature.SetSize(243, 150);
                    lowTemperature.Anchors = Anchors.None;
                    lowTemperature.Visible = true;

                    weatherImage.SetPosition(315, 140);
                    weatherImage.SetSize(224, 224);
                    weatherImage.Anchors = Anchors.None;
                    weatherImage.Visible = true;

                    closeButton.SetPosition(624, 40);
                    closeButton.SetSize(52, 52);
                    closeButton.Anchors = Anchors.None;
                    closeButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            curDate.Text = "May 20, 2011 (Fri)";

            cityName.Text = "Tokyo";

            highTemperature.Text = "25℃";

            diagonal.Text = "/";

            lowTemperature.Text = "17℃";
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
