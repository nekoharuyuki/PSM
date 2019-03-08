// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class FeedListPanelItem
    {
        ImageBox feedImageBox;
        Label feedLabel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            feedImageBox = new ImageBox();
            feedImageBox.Name = "feedImageBox";
            feedLabel = new Label();
            feedLabel.Name = "feedLabel";

            // feedImageBox
            feedImageBox.Image = new ImageAsset("/Application/assets/dummyBackground.png");
            feedImageBox.ImageScaleType = ImageScaleType.NinePatch;
            feedImageBox.NinePatchMargin = new NinePatchMargin(16, 16, 16, 16);

            // feedLabel
            feedLabel.TextColor = new UIColor(34f / 255f, 34f / 255f, 34f / 255f, 255f / 255f);
            feedLabel.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
            feedLabel.LineBreak = LineBreak.Character;

            // FeedListPanelItem
            this.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            this.AddChildLast(feedImageBox);
            this.AddChildLast(feedLabel);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(80, 300);
                    this.Anchors = Anchors.None;

                    feedImageBox.SetPosition(39, -63);
                    feedImageBox.SetSize(200, 200);
                    feedImageBox.Anchors = Anchors.None;
                    feedImageBox.Visible = true;

                    feedLabel.SetPosition(64, 9);
                    feedLabel.SetSize(214, 36);
                    feedLabel.Anchors = Anchors.None;
                    feedLabel.Visible = true;

                    break;

                default:
                    this.SetSize(280, 45);
                    this.Anchors = Anchors.None;

                    feedImageBox.SetPosition(0, 0);
                    feedImageBox.SetSize(280, 45);
                    feedImageBox.Anchors = Anchors.None;
                    feedImageBox.Visible = true;

                    feedLabel.SetPosition(9, 0);
                    feedLabel.SetSize(270, 45);
                    feedLabel.Anchors = Anchors.None;
                    feedLabel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            feedLabel.Text = "label";
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
            return new FeedListPanelItem();
        }

    }
}
