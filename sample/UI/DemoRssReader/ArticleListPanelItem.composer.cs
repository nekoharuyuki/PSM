// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class ArticleListPanelItem
    {
        ImageBox articleImageBox;
        Label articleLabel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            articleImageBox = new ImageBox();
            articleImageBox.Name = "articleImageBox";
            articleLabel = new Label();
            articleLabel.Name = "articleLabel";

            // articleImageBox
            articleImageBox.Image = new ImageAsset("/Application/assets/dummyBackground.png");
            articleImageBox.ImageScaleType = ImageScaleType.NinePatch;
            articleImageBox.NinePatchMargin = new NinePatchMargin(16, 16, 16, 16);

            // articleLabel
            articleLabel.TextColor = new UIColor(17f / 255f, 17f / 255f, 17f / 255f, 255f / 255f);
            articleLabel.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
            articleLabel.TextTrimming = TextTrimming.Word;
            articleLabel.LineBreak = LineBreak.Character;

            // ArticleListPanelItem
            this.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            this.AddChildLast(articleImageBox);
            this.AddChildLast(articleLabel);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(205, 500);
                    this.Anchors = Anchors.None;

                    articleImageBox.SetPosition(152, 5);
                    articleImageBox.SetSize(200, 200);
                    articleImageBox.Anchors = Anchors.None;
                    articleImageBox.Visible = true;

                    articleLabel.SetPosition(184, 47);
                    articleLabel.SetSize(214, 36);
                    articleLabel.Anchors = Anchors.None;
                    articleLabel.Visible = true;

                    break;

                default:
                    this.SetSize(480, 75);
                    this.Anchors = Anchors.None;

                    articleImageBox.SetPosition(0, 0);
                    articleImageBox.SetSize(480, 75);
                    articleImageBox.Anchors = Anchors.None;
                    articleImageBox.Visible = true;

                    articleLabel.SetPosition(22, 0);
                    articleLabel.SetSize(432, 75);
                    articleLabel.Anchors = Anchors.None;
                    articleLabel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            articleLabel.Text = "label";
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
            return new ArticleListPanelItem();
        }

    }
}
