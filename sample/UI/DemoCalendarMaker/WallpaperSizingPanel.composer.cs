// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class WallpaperSizingPanel
    {
        ImageBox wallpaper;
        Button Button_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            wallpaper = new ImageBox();
            wallpaper.Name = "wallpaper";
            Button_1 = new Button();
            Button_1.Name = "Button_1";

            // wallpaper
            wallpaper.Image = null;
            wallpaper.ImageScaleType = ImageScaleType.AspectInside;

            // Button_1
            Button_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/ok_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/ok_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/ok_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // WallpaperSizingPanel
            this.BackgroundColor = new UIColor(97f / 255f, 97f / 255f, 97f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(wallpaper);
            this.AddChildLast(Button_1);

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

                    wallpaper.SetPosition(66, 35);
                    wallpaper.SetSize(426, 349);
                    wallpaper.Anchors = Anchors.None;
                    wallpaper.Visible = true;

                    Button_1.SetPosition(-169, 306);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    break;

                default:
                    this.SetSize(754, 480);
                    this.Anchors = Anchors.None;

                    wallpaper.SetPosition(0, 0);
                    wallpaper.SetSize(754, 480);
                    wallpaper.Anchors = Anchors.None;
                    wallpaper.Visible = true;

                    Button_1.SetPosition(654, 424);
                    Button_1.SetSize(88, 32);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

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
