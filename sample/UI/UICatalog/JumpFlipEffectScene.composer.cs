// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class JumpFlipEffectScene
    {
        Panel contentPanel;
        ImageBox image2;
        ImageBox currentImage;
        Panel Panel_1;
        Button buttonJumpFlip;
        Label rvLabel;
        Slider rxSlider;
        Label raLabel;
        Label xLabel;
        CheckBox xRadioButton;
        Label yLabel;
        CheckBox yRadioButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            image2 = new ImageBox();
            image2.Name = "image2";
            currentImage = new ImageBox();
            currentImage.Name = "currentImage";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonJumpFlip = new Button();
            buttonJumpFlip.Name = "buttonJumpFlip";
            rvLabel = new Label();
            rvLabel.Name = "rvLabel";
            rxSlider = new Slider();
            rxSlider.Name = "rxSlider";
            raLabel = new Label();
            raLabel.Name = "raLabel";
            xLabel = new Label();
            xLabel.Name = "xLabel";
            xRadioButton = new CheckBox();
            xRadioButton.Name = "xRadioButton";
            yLabel = new Label();
            yLabel.Name = "yLabel";
            yRadioButton = new CheckBox();
            yRadioButton.Name = "yRadioButton";

            // JumpFlipEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(image2);
            contentPanel.AddChildLast(currentImage);
            contentPanel.AddChildLast(Panel_1);

            // image2
            image2.Image = new ImageAsset("/Application/assets/photo05.png");
            image2.ImageScaleType = ImageScaleType.AspectInside;

            // currentImage
            currentImage.Image = new ImageAsset("/Application/assets/photo02.png");
            currentImage.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonJumpFlip);
            Panel_1.AddChildLast(rvLabel);
            Panel_1.AddChildLast(rxSlider);
            Panel_1.AddChildLast(raLabel);
            Panel_1.AddChildLast(xLabel);
            Panel_1.AddChildLast(xRadioButton);
            Panel_1.AddChildLast(yLabel);
            Panel_1.AddChildLast(yRadioButton);

            // buttonJumpFlip
            buttonJumpFlip.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonJumpFlip.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // rvLabel
            rvLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            rvLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            rvLabel.LineBreak = LineBreak.Character;
            rvLabel.VerticalAlignment = VerticalAlignment.Bottom;

            // rxSlider
            rxSlider.MinValue = -10;
            rxSlider.MaxValue = 10;
            rxSlider.Value = 0;
            rxSlider.Step = 1;

            // raLabel
            raLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            raLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            raLabel.LineBreak = LineBreak.Character;
            raLabel.VerticalAlignment = VerticalAlignment.Bottom;

            // xLabel
            xLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            xLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            xLabel.LineBreak = LineBreak.Character;

            // xRadioButton
            xRadioButton.Style = CheckBoxStyle.RadioButton;

            // yLabel
            yLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            yLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            yLabel.LineBreak = LineBreak.Character;

            // yRadioButton
            yRadioButton.Style = CheckBoxStyle.RadioButton;
            yRadioButton.Checked = true;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 480;
                    this.DesignHeight = 854;

                    contentPanel.SetPosition(0, 152);
                    contentPanel.SetSize(480, 701);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    image2.SetPosition(29, 34);
                    image2.SetSize(200, 200);
                    image2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image2.Visible = true;

                    currentImage.SetPosition(29, 34);
                    currentImage.SetSize(200, 200);
                    currentImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    currentImage.Visible = true;

                    Panel_1.SetPosition(0, 288);
                    Panel_1.SetSize(480, 413);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonJumpFlip.SetPosition(112, 252);
                    buttonJumpFlip.SetSize(214, 56);
                    buttonJumpFlip.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonJumpFlip.Visible = true;

                    rvLabel.SetPosition(154, 141);
                    rvLabel.SetSize(214, 36);
                    rvLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    rvLabel.Visible = true;

                    rxSlider.SetPosition(80, 177);
                    rxSlider.SetSize(362, 58);
                    rxSlider.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    rxSlider.Visible = true;

                    raLabel.SetPosition(0, 465);
                    raLabel.SetSize(214, 36);
                    raLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    raLabel.Visible = true;

                    xLabel.SetPosition(133, 25);
                    xLabel.SetSize(214, 36);
                    xLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    xLabel.Visible = true;

                    xRadioButton.SetPosition(73, 42);
                    xRadioButton.SetSize(39, 39);
                    xRadioButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    xRadioButton.Visible = true;

                    yLabel.SetPosition(126, 87);
                    yLabel.SetSize(214, 36);
                    yLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    yLabel.Visible = true;

                    yRadioButton.SetPosition(73, 87);
                    yRadioButton.SetSize(39, 39);
                    yRadioButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    yRadioButton.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    image2.SetPosition(300, 51);
                    image2.SetSize(500, 260);
                    image2.Anchors = Anchors.None;
                    image2.Visible = true;

                    currentImage.SetPosition(300, 51);
                    currentImage.SetSize(500, 260);
                    currentImage.Anchors = Anchors.None;
                    currentImage.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(220, 299);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonJumpFlip.SetPosition(0, 0);
                    buttonJumpFlip.SetSize(200, 56);
                    buttonJumpFlip.Anchors = Anchors.Top | Anchors.Height;
                    buttonJumpFlip.Visible = true;

                    rvLabel.SetPosition(0, 56);
                    rvLabel.SetSize(200, 36);
                    rvLabel.Anchors = Anchors.Top | Anchors.Height;
                    rvLabel.Visible = true;

                    rxSlider.SetPosition(0, 92);
                    rxSlider.SetSize(200, 58);
                    rxSlider.Anchors = Anchors.Top | Anchors.Height;
                    rxSlider.Visible = true;

                    raLabel.SetPosition(0, 150);
                    raLabel.SetSize(200, 36);
                    raLabel.Anchors = Anchors.Top | Anchors.Height;
                    raLabel.Visible = true;

                    xLabel.SetPosition(0, 186);
                    xLabel.SetSize(25, 39);
                    xLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    xLabel.Visible = true;

                    xRadioButton.SetPosition(25, 186);
                    xRadioButton.SetSize(39, 39);
                    xRadioButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    xRadioButton.Visible = true;

                    yLabel.SetPosition(0, 235);
                    yLabel.SetSize(25, 40);
                    yLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    yLabel.Visible = true;

                    yRadioButton.SetPosition(25, 235);
                    yRadioButton.SetSize(39, 39);
                    yRadioButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    yRadioButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonJumpFlip.Text = "JumpFlip";

            rvLabel.Text = "Revolution: 0";

            raLabel.Text = "Rotation axis";

            xLabel.Text = "X";

            yLabel.Text = "Y";
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
