// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class WallpaperGalleryItem
    {
        ImageBox shadow;
        ImageBox wallpaper;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            shadow = new ImageBox();
            shadow.Name = "shadow";
            wallpaper = new ImageBox();
            wallpaper.Name = "wallpaper";

            // shadow
            shadow.Image = new ImageAsset("/Application/assets/shadow.png");
            shadow.ImageScaleType = ImageScaleType.NinePatch;
            shadow.NinePatchMargin = new NinePatchMargin(40, 40, 40, 40);

            // wallpaper
            wallpaper.Image = null;

            // WallpaperGalleryItem
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(shadow);
            this.AddChildLast(wallpaper);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(150, 255);
                    this.Anchors = Anchors.None;

                    shadow.SetPosition(0, 0);
                    shadow.SetSize(200, 200);
                    shadow.Anchors = Anchors.None;
                    shadow.Visible = true;

                    wallpaper.SetPosition(-40, -53);
                    wallpaper.SetSize(200, 200);
                    wallpaper.Anchors = Anchors.None;
                    wallpaper.Visible = true;

                    break;

                default:
                    this.SetSize(120, 120);
                    this.Anchors = Anchors.None;

                    shadow.SetPosition(0, 0);
                    shadow.SetSize(120, 120);
                    shadow.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    shadow.Visible = true;

                    wallpaper.SetPosition(14, 14);
                    wallpaper.SetSize(106, 106);
                    wallpaper.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    wallpaper.Visible = true;

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

        public static ListPanelItem Creator()
        {
            return new WallpaperGalleryItem();
        }

    }
}
