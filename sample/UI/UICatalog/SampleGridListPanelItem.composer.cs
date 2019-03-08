// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SampleGridListPanelItem
    {
        ImageBox imageBox;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            imageBox = new ImageBox();
            imageBox.Name = "imageBox";

            // SampleGridListPanelItem
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            this.AddChildLast(imageBox);

            // imageBox
            imageBox.Image = new ImageAsset("/Application/assets/photo01.png");

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(40, 60);
                    this.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;

                    imageBox.SetPosition(0, 0);
                    imageBox.SetSize(40, 60);
                    imageBox.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox.Visible = true;

                    break;

                default:
                    this.SetSize(60, 40);
                    this.Anchors = Anchors.Height | Anchors.Width;

                    imageBox.SetPosition(0, 0);
                    imageBox.SetSize(60, 40);
                    imageBox.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    imageBox.Visible = true;

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
            return new SampleGridListPanelItem();
        }

    }
}
