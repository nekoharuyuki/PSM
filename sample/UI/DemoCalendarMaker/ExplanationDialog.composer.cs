// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class ExplanationDialog
    {
        ImageBox baseImage;
        ImageBox bar1Image;
        ImageBox bar2Image;
        ImageBox bar3Image;
        ImageBox bar4Image;
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
            bar1Image = new ImageBox();
            bar1Image.Name = "bar1Image";
            bar2Image = new ImageBox();
            bar2Image.Name = "bar2Image";
            bar3Image = new ImageBox();
            bar3Image.Name = "bar3Image";
            bar4Image = new ImageBox();
            bar4Image.Name = "bar4Image";
            guideLabel = new Label();
            guideLabel.Name = "guideLabel";
            closeButton = new Button();
            closeButton.Name = "closeButton";

            // baseImage
            baseImage.Image = new ImageAsset("/Application/assets/help_base.png");
            baseImage.ImageScaleType = ImageScaleType.NinePatch;
            baseImage.NinePatchMargin = new NinePatchMargin(15, 15, 15, 15);

            // bar1Image
            bar1Image.Image = new ImageAsset("/Application/assets/help_bar_up.png");

            // bar2Image
            bar2Image.Image = new ImageAsset("/Application/assets/help_bar_up.png");

            // bar3Image
            bar3Image.Image = new ImageAsset("/Application/assets/help_bar_down.png");

            // bar4Image
            bar4Image.Image = new ImageAsset("/Application/assets/help_bar_down.png");

            // guideLabel
            guideLabel.TextColor = new UIColor(224f / 255f, 255f / 255f, 240f / 255f, 255f / 255f);
            guideLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            guideLabel.LineBreak = LineBreak.Character;
            guideLabel.VerticalAlignment = VerticalAlignment.Top;

            // closeButton
            closeButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            closeButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            closeButton.Style = ButtonStyle.Custom;
            closeButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/ok_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/ok_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/ok_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // ExplanationDialog
            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = new ImageAsset("/Application/assets/main_background.png");
            this.CustomBackgroundNinePatchMargin = new NinePatchMargin(34, 34, 34, 34);
            this.CustomBackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(baseImage);
            this.AddChildLast(bar1Image);
            this.AddChildLast(bar2Image);
            this.AddChildLast(bar3Image);
            this.AddChildLast(bar4Image);
            this.AddChildLast(guideLabel);
            this.AddChildLast(closeButton);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetPosition(0, 0);
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.None;

                    baseImage.SetPosition(49, 10);
                    baseImage.SetSize(200, 200);
                    baseImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    baseImage.Visible = true;

                    bar1Image.SetPosition(0, 39);
                    bar1Image.SetSize(200, 200);
                    bar1Image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bar1Image.Visible = true;

                    bar2Image.SetPosition(0, 39);
                    bar2Image.SetSize(200, 200);
                    bar2Image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bar2Image.Visible = true;

                    bar3Image.SetPosition(0, 238);
                    bar3Image.SetSize(200, 200);
                    bar3Image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bar3Image.Visible = true;

                    bar4Image.SetPosition(0, 238);
                    bar4Image.SetSize(200, 200);
                    bar4Image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bar4Image.Visible = true;

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
                    this.SetPosition(0, 0);
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    baseImage.SetPosition(149, 24);
                    baseImage.SetSize(574, 354);
                    baseImage.Anchors = Anchors.None;
                    baseImage.Visible = true;

                    bar1Image.SetPosition(101, 48);
                    bar1Image.SetSize(54, 27);
                    bar1Image.Anchors = Anchors.None;
                    bar1Image.Visible = true;

                    bar2Image.SetPosition(101, 145);
                    bar2Image.SetSize(54, 27);
                    bar2Image.Anchors = Anchors.None;
                    bar2Image.Visible = true;

                    bar3Image.SetPosition(101, 231);
                    bar3Image.SetSize(54, 27);
                    bar3Image.Anchors = Anchors.None;
                    bar3Image.Visible = true;

                    bar4Image.SetPosition(101, 328);
                    bar4Image.SetSize(54, 27);
                    bar4Image.Anchors = Anchors.None;
                    bar4Image.Visible = true;

                    guideLabel.SetPosition(178, 48);
                    guideLabel.SetSize(512, 248);
                    guideLabel.Anchors = Anchors.None;
                    guideLabel.Visible = true;

                    closeButton.SetPosition(392, 313);
                    closeButton.SetSize(88, 32);
                    closeButton.Anchors = Anchors.None;
                    closeButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            guideLabel.Text = "text";
        }

        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
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
