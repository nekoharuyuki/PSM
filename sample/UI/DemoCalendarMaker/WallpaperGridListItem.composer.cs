// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class WallpaperGridListItem
    {
        ImageBox shadow;
        ImageBox ImageBox_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            shadow = new ImageBox();
            shadow.Name = "shadow";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";

            // shadow
            shadow.Image = new ImageAsset("/Application/assets/shadow.png");
            shadow.ImageScaleType = ImageScaleType.NinePatch;
            shadow.NinePatchMargin = new NinePatchMargin(40, 40, 40, 40);

            // ImageBox_1
            ImageBox_1.Image = null;
            ImageBox_1.ImageScaleType = ImageScaleType.AspectOutside;

            // WallpaperGridListItem
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(shadow);
            this.AddChildLast(ImageBox_1);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(120, 120);
                    this.Anchors = Anchors.None;

                    shadow.SetPosition(35, 0);
                    shadow.SetSize(200, 200);
                    shadow.Anchors = Anchors.None;
                    shadow.Visible = true;

                    ImageBox_1.SetPosition(16, 37);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    break;

                default:
                    this.SetSize(120, 120);
                    this.Anchors = Anchors.None;

                    shadow.SetPosition(0, 0);
                    shadow.SetSize(120, 120);
                    shadow.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    shadow.Visible = true;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(106, 106);
                    ImageBox_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    ImageBox_1.Visible = true;

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
            return new WallpaperGridListItem();
        }

    }
}
