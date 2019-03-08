// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class ArticlePanel
    {
        ImageBox bgImage;
        Button prevArticleButton;
        Button dialogCloseButton;
        Button nextArticleButton;
        Panel basePanel;
        Panel panel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            bgImage = new ImageBox();
            bgImage.Name = "bgImage";
            prevArticleButton = new Button();
            prevArticleButton.Name = "prevArticleButton";
            dialogCloseButton = new Button();
            dialogCloseButton.Name = "dialogCloseButton";
            nextArticleButton = new Button();
            nextArticleButton.Name = "nextArticleButton";
            basePanel = new Panel();
            basePanel.Name = "basePanel";
            panel = new Panel();
            panel.Name = "panel";

            // bgImage
            bgImage.Image = new ImageAsset("/Application/assets/itemBack01.png");
            bgImage.ImageScaleType = ImageScaleType.NinePatch;
            bgImage.NinePatchMargin = new NinePatchMargin(20, 20, 20, 20);

            // prevArticleButton
            prevArticleButton.IconImage = null;
            prevArticleButton.Style = ButtonStyle.Custom;
            prevArticleButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/left_arrow_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/left_arrow_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/left_arrow_normal.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // dialogCloseButton
            dialogCloseButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            dialogCloseButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            dialogCloseButton.Style = ButtonStyle.Custom;
            dialogCloseButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/close_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/close_button_normal.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/close_button_normal.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // nextArticleButton
            nextArticleButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            nextArticleButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            nextArticleButton.Style = ButtonStyle.Custom;
            nextArticleButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/right_arrow_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/right_arrow_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/right_arrow_normal.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // basePanel
            basePanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            basePanel.Clip = true;

            // panel
            panel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            panel.Clip = true;
            panel.AddChildLast(prevArticleButton);
            panel.AddChildLast(dialogCloseButton);
            panel.AddChildLast(nextArticleButton);
            panel.AddChildLast(basePanel);

            // ArticlePanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(bgImage);
            this.AddChildLast(panel);

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

                    bgImage.SetPosition(50, 25);
                    bgImage.SetSize(200, 200);
                    bgImage.Anchors = Anchors.None;
                    bgImage.Visible = true;

                    prevArticleButton.SetPosition(124, -55);
                    prevArticleButton.SetSize(214, 56);
                    prevArticleButton.Anchors = Anchors.None;
                    prevArticleButton.Visible = true;

                    dialogCloseButton.SetPosition(124, -55);
                    dialogCloseButton.SetSize(214, 56);
                    dialogCloseButton.Anchors = Anchors.None;
                    dialogCloseButton.Visible = true;

                    nextArticleButton.SetPosition(124, -55);
                    nextArticleButton.SetSize(214, 56);
                    nextArticleButton.Anchors = Anchors.None;
                    nextArticleButton.Visible = true;

                    basePanel.SetPosition(-140, 5);
                    basePanel.SetSize(100, 100);
                    basePanel.Anchors = Anchors.None;
                    basePanel.Visible = true;

                    panel.SetPosition(152, 68);
                    panel.SetSize(100, 100);
                    panel.Anchors = Anchors.None;
                    panel.Visible = true;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    bgImage.SetPosition(50, 25);
                    bgImage.SetSize(754, 430);
                    bgImage.Anchors = Anchors.None;
                    bgImage.Visible = true;

                    prevArticleButton.SetPosition(0, 190);
                    prevArticleButton.SetSize(50, 49);
                    prevArticleButton.Anchors = Anchors.None;
                    prevArticleButton.Visible = true;

                    dialogCloseButton.SetPosition(693, 11);
                    dialogCloseButton.SetSize(50, 53);
                    dialogCloseButton.Anchors = Anchors.None;
                    dialogCloseButton.Visible = true;

                    nextArticleButton.SetPosition(704, 190);
                    nextArticleButton.SetSize(50, 49);
                    nextArticleButton.Anchors = Anchors.None;
                    nextArticleButton.Visible = true;

                    basePanel.SetPosition(50, 61);
                    basePanel.SetSize(654, 291);
                    basePanel.Anchors = Anchors.None;
                    basePanel.Visible = true;

                    panel.SetPosition(50, 25);
                    panel.SetSize(754, 430);
                    panel.Anchors = Anchors.None;
                    panel.Visible = true;

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
