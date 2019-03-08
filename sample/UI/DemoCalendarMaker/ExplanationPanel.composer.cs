// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class ExplanationPanel
    {
        ImageBox baseImage;
        Label guideLabel;
        Button closeButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            baseImage = new ImageBox();
            baseImage.Name = "baseImage";
            guideLabel = new Label();
            guideLabel.Name = "guideLabel";
            closeButton = new Button();
            closeButton.Name = "closeButton";

            // baseImage
            baseImage.Image = new ImageAsset("/Application/assets/base/base01.png");

            // guideLabel
            guideLabel.TextColor = new UIColor(224f / 255f, 255f / 255f, 240f / 255f, 255f / 255f);
            guideLabel.Font = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            guideLabel.LineBreak = LineBreak.Character;
            guideLabel.VerticalAlignment = VerticalAlignment.Top;

            // closeButton
            closeButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            closeButton.TextFont = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            closeButton.Style = ButtonStyle.Custom;
            CustomButtonImageSettings closeButton_custom = new CustomButtonImageSettings();
            closeButton_custom.BackgroundNormalImage = new ImageAsset("/Application/assets/ok_normal.png");
            closeButton_custom.BackgroundPressedImage = new ImageAsset("/Application/assets/ok_pressed.png");
            closeButton_custom.BackgroundDisabledImage = new ImageAsset("/Application/assets/ok_disable.png");
            closeButton_custom.BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0);
            closeButton.CustomImage = closeButton_custom;

            // ExplanationPanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            this.Clip = true;

            // Panel
            this.AddChildLast(baseImage);
            this.AddChildLast(guideLabel);
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
                this.SetSize(480, 700);
                this.Anchors = Anchors.None;

                baseImage.SetPosition(0, 0);
                baseImage.SetSize(200, 200);
                baseImage.Anchors = Anchors.None;
                baseImage.Visible = true;

                guideLabel.SetPosition(110, 76);
                guideLabel.SetSize(214, 36);
                guideLabel.Anchors = Anchors.None;
                guideLabel.Visible = true;

                closeButton.SetPosition(222, 377);
                closeButton.SetSize(214, 56);
                closeButton.Anchors = Anchors.None;
                closeButton.Visible = true;

                break;

            default:
                this.SetSize(700, 480);
                this.Anchors = Anchors.None;

                baseImage.SetPosition(0, 0);
                baseImage.SetSize(640, 480);
                baseImage.Anchors = Anchors.None;
                baseImage.Visible = true;

                guideLabel.SetPosition(72, 56);
                guideLabel.SetSize(512, 250);
                guideLabel.Anchors = Anchors.None;
                guideLabel.Visible = true;

                closeButton.SetPosition(296, 392);
                closeButton.SetSize(88, 32);
                closeButton.Anchors = Anchors.None;
                closeButton.Visible = true;

                break;
            }
            _currentLayoutOrientation = orientation;
        }
        public void UpdateLanguage()
        {
            guideLabel.Text = "説明文";
        }
        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
        public void StartDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
    }
}
