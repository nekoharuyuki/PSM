// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class FullScreenPanel
    {
        ImageBox wallpaper;
        Button closeButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            wallpaper = new ImageBox();
            wallpaper.Name = "wallpaper";
            closeButton = new Button();
            closeButton.Name = "closeButton";

            // wallpaper
            wallpaper.Image = null;
            wallpaper.ImageScaleType = ImageScaleType.AspectInside;

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

            // FullScreenPanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(wallpaper);
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
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.None;

                    wallpaper.SetPosition(277, 39);
                    wallpaper.SetSize(200, 200);
                    wallpaper.Anchors = Anchors.None;
                    wallpaper.Visible = true;

                    closeButton.SetPosition(649, 58);
                    closeButton.SetSize(214, 56);
                    closeButton.Anchors = Anchors.None;
                    closeButton.Visible = true;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    wallpaper.SetPosition(0, 0);
                    wallpaper.SetSize(854, 480);
                    wallpaper.Anchors = Anchors.None;
                    wallpaper.Visible = true;

                    closeButton.SetPosition(802, 0);
                    closeButton.SetSize(52, 52);
                    closeButton.Anchors = Anchors.None;
                    closeButton.Visible = true;

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
