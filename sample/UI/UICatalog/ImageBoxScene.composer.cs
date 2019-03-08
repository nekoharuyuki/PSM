// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ImageBoxScene
    {
        Panel contentPanel;
        ImageBox normalImage;
        Label label1;
        ImageBox alphaImage;
        Label label2;
        Label Label_3;
        Panel panel1;
        ImageBox image1;
        Label Label_4;
        Panel panel2;
        ImageBox image2;
        Label Label_5;
        Panel panel3;
        ImageBox image3;
        Label Label_6;
        Panel panel4;
        ImageBox image4;
        Label Label_7;
        Panel panel5;
        ImageBox image5;
        Panel panelH;
        Slider sliderH;
        Label Label_8;
        Panel panelV;
        Slider sliderV;
        Label Label_9;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            normalImage = new ImageBox();
            normalImage.Name = "normalImage";
            label1 = new Label();
            label1.Name = "label1";
            alphaImage = new ImageBox();
            alphaImage.Name = "alphaImage";
            label2 = new Label();
            label2.Name = "label2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            panel1 = new Panel();
            panel1.Name = "panel1";
            image1 = new ImageBox();
            image1.Name = "image1";
            Label_4 = new Label();
            Label_4.Name = "Label_4";
            panel2 = new Panel();
            panel2.Name = "panel2";
            image2 = new ImageBox();
            image2.Name = "image2";
            Label_5 = new Label();
            Label_5.Name = "Label_5";
            panel3 = new Panel();
            panel3.Name = "panel3";
            image3 = new ImageBox();
            image3.Name = "image3";
            Label_6 = new Label();
            Label_6.Name = "Label_6";
            panel4 = new Panel();
            panel4.Name = "panel4";
            image4 = new ImageBox();
            image4.Name = "image4";
            Label_7 = new Label();
            Label_7.Name = "Label_7";
            panel5 = new Panel();
            panel5.Name = "panel5";
            image5 = new ImageBox();
            image5.Name = "image5";
            panelH = new Panel();
            panelH.Name = "panelH";
            sliderH = new Slider();
            sliderH.Name = "sliderH";
            Label_8 = new Label();
            Label_8.Name = "Label_8";
            panelV = new Panel();
            panelV.Name = "panelV";
            sliderV = new Slider();
            sliderV.Name = "sliderV";
            Label_9 = new Label();
            Label_9.Name = "Label_9";

            // ImageBoxScene
            this.RootWidget.AddChildLast(contentPanel);
            this.RootWidget.AddChildLast(panelH);
            this.RootWidget.AddChildLast(panelV);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(normalImage);
            contentPanel.AddChildLast(label1);
            contentPanel.AddChildLast(alphaImage);
            contentPanel.AddChildLast(label2);
            contentPanel.AddChildLast(Label_3);
            contentPanel.AddChildLast(panel1);
            contentPanel.AddChildLast(Label_4);
            contentPanel.AddChildLast(panel2);
            contentPanel.AddChildLast(Label_5);
            contentPanel.AddChildLast(panel3);
            contentPanel.AddChildLast(Label_6);
            contentPanel.AddChildLast(panel4);
            contentPanel.AddChildLast(Label_7);
            contentPanel.AddChildLast(panel5);

            // normalImage
            normalImage.Image = new ImageAsset("/Application/assets/image_scale.png");
            normalImage.ImageScaleType = ImageScaleType.AspectInside;

            // label1
            label1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label1.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            label1.LineBreak = LineBreak.Character;
            label1.HorizontalAlignment = HorizontalAlignment.Center;
            label1.VerticalAlignment = VerticalAlignment.Bottom;

            // alphaImage
            alphaImage.Image = new ImageAsset("/Application/assets/image_scale.png");
            alphaImage.ImageScaleType = ImageScaleType.AspectInside;

            // label2
            label2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label2.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            label2.LineBreak = LineBreak.Character;
            label2.HorizontalAlignment = HorizontalAlignment.Center;
            label2.VerticalAlignment = VerticalAlignment.Bottom;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;
            Label_3.VerticalAlignment = VerticalAlignment.Bottom;

            // panel1
            panel1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel1.Clip = true;
            panel1.AddChildLast(image1);

            // image1
            image1.Image = new ImageAsset("/Application/assets/image_scale.png");
            image1.ImageScaleType = ImageScaleType.Center;

            // Label_4
            Label_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_4.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_4.LineBreak = LineBreak.Character;
            Label_4.VerticalAlignment = VerticalAlignment.Bottom;

            // panel2
            panel2.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel2.Clip = true;
            panel2.AddChildLast(image2);

            // image2
            image2.Image = new ImageAsset("/Application/assets/image_scale.png");

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_5.LineBreak = LineBreak.Character;
            Label_5.VerticalAlignment = VerticalAlignment.Bottom;

            // panel3
            panel3.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel3.Clip = true;
            panel3.AddChildLast(image3);

            // image3
            image3.Image = new ImageAsset("/Application/assets/image_scale.png");
            image3.ImageScaleType = ImageScaleType.AspectInside;

            // Label_6
            Label_6.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_6.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_6.LineBreak = LineBreak.Character;
            Label_6.VerticalAlignment = VerticalAlignment.Bottom;

            // panel4
            panel4.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel4.Clip = true;
            panel4.AddChildLast(image4);

            // image4
            image4.Image = new ImageAsset("/Application/assets/image_scale.png");
            image4.ImageScaleType = ImageScaleType.AspectOutside;

            // Label_7
            Label_7.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_7.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_7.LineBreak = LineBreak.Character;
            Label_7.VerticalAlignment = VerticalAlignment.Bottom;

            // panel5
            panel5.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel5.Clip = true;
            panel5.AddChildLast(image5);

            // image5
            image5.Image = new ImageAsset("/Application/assets/image_scale.png");
            image5.ImageScaleType = ImageScaleType.NinePatch;
            image5.NinePatchMargin = new NinePatchMargin(20, 20, 20, 20);

            // panelH
            panelH.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            panelH.Clip = true;
            panelH.AddChildLast(sliderH);
            panelH.AddChildLast(Label_8);

            // sliderH
            sliderH.MinValue = 25;
            sliderH.MaxValue = 100;
            sliderH.Value = 0;
            sliderH.Step = 1;

            // Label_8
            Label_8.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_8.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_8.LineBreak = LineBreak.Character;
            Label_8.VerticalAlignment = VerticalAlignment.Bottom;

            // panelV
            panelV.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            panelV.Clip = true;
            panelV.AddChildLast(sliderV);
            panelV.AddChildLast(Label_9);

            // sliderV
            sliderV.MinValue = 25;
            sliderV.MaxValue = 100;
            sliderV.Value = 0;
            sliderV.Step = 1;

            // Label_9
            Label_9.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_9.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_9.LineBreak = LineBreak.Character;
            Label_9.VerticalAlignment = VerticalAlignment.Bottom;

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

                    contentPanel.SetPosition(0, 92);
                    contentPanel.SetSize(473, 530);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    normalImage.SetPosition(18, 12);
                    normalImage.SetSize(122, 91);
                    normalImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    normalImage.Visible = true;

                    label1.SetPosition(131, 32);
                    label1.SetSize(118, 36);
                    label1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label1.Visible = true;

                    alphaImage.SetPosition(244, 12);
                    alphaImage.SetSize(105, 99);
                    alphaImage.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    alphaImage.Visible = true;

                    label2.SetPosition(356, 32);
                    label2.SetSize(90, 36);
                    label2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label2.Visible = true;

                    Label_3.SetPosition(31, 224);
                    Label_3.SetSize(83, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_3.Visible = true;

                    panel1.SetPosition(18, 111);
                    panel1.SetSize(100, 100);
                    panel1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel1.Visible = true;

                    image1.SetPosition(0, 0);
                    image1.SetSize(100, 100);
                    image1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image1.Visible = true;

                    Label_4.SetPosition(145, 211);
                    Label_4.SetSize(90, 36);
                    Label_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_4.Visible = true;

                    panel2.SetPosition(140, 111);
                    panel2.SetSize(100, 100);
                    panel2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel2.Visible = true;

                    image2.SetPosition(0, 0);
                    image2.SetSize(100, 100);
                    image2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image2.Visible = true;

                    Label_5.SetPosition(313, 224);
                    Label_5.SetSize(122, 36);
                    Label_5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_5.Visible = true;

                    panel3.SetPosition(313, 124);
                    panel3.SetSize(100, 100);
                    panel3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel3.Visible = true;

                    image3.SetPosition(0, 0);
                    image3.SetSize(100, 100);
                    image3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image3.Visible = true;

                    Label_6.SetPosition(42, 443);
                    Label_6.SetSize(151, 36);
                    Label_6.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_6.Visible = true;

                    panel4.SetPosition(67, 321);
                    panel4.SetSize(100, 100);
                    panel4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel4.Visible = true;

                    image4.SetPosition(0, 0);
                    image4.SetSize(100, 100);
                    image4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image4.Visible = true;

                    Label_7.SetPosition(297, 443);
                    Label_7.SetSize(107, 36);
                    Label_7.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_7.Visible = true;

                    panel5.SetPosition(322, 335);
                    panel5.SetSize(100, 100);
                    panel5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel5.Visible = true;

                    image5.SetPosition(0, 0);
                    image5.SetSize(100, 100);
                    image5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image5.Visible = true;

                    panelH.SetPosition(0, 742);
                    panelH.SetSize(480, 112);
                    panelH.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panelH.Visible = true;

                    sliderH.SetPosition(42, 43);
                    sliderH.SetSize(362, 58);
                    sliderH.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    sliderH.Visible = true;

                    Label_8.SetPosition(18, 0);
                    Label_8.SetSize(214, 36);
                    Label_8.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_8.Visible = true;

                    panelV.SetPosition(0, 647);
                    panelV.SetSize(480, 95);
                    panelV.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panelV.Visible = true;

                    sliderV.SetPosition(42, 36);
                    sliderV.SetSize(362, 58);
                    sliderV.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    sliderV.Visible = true;

                    Label_9.SetPosition(18, 0);
                    Label_9.SetSize(214, 36);
                    Label_9.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_9.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    normalImage.SetPosition(312, 37);
                    normalImage.SetSize(66, 53);
                    normalImage.Anchors = Anchors.None;
                    normalImage.Visible = true;

                    label1.SetPosition(279, 1);
                    label1.SetSize(128, 36);
                    label1.Anchors = Anchors.None;
                    label1.Visible = true;

                    alphaImage.SetPosition(479, 37);
                    alphaImage.SetSize(66, 53);
                    alphaImage.Anchors = Anchors.None;
                    alphaImage.Visible = true;

                    label2.SetPosition(449, 1);
                    label2.SetSize(121, 36);
                    label2.Anchors = Anchors.None;
                    label2.Visible = true;

                    Label_3.SetPosition(14, 95);
                    Label_3.SetSize(150, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    panel1.SetPosition(14, 131);
                    panel1.SetSize(150, 130);
                    panel1.Anchors = Anchors.None;
                    panel1.Visible = true;

                    image1.SetPosition(0, 0);
                    image1.SetSize(150, 130);
                    image1.Anchors = Anchors.None;
                    image1.Visible = true;

                    Label_4.SetPosition(183, 95);
                    Label_4.SetSize(150, 36);
                    Label_4.Anchors = Anchors.None;
                    Label_4.Visible = true;

                    panel2.SetPosition(183, 131);
                    panel2.SetSize(150, 130);
                    panel2.Anchors = Anchors.None;
                    panel2.Visible = true;

                    image2.SetPosition(0, 0);
                    image2.SetSize(150, 130);
                    image2.Anchors = Anchors.None;
                    image2.Visible = true;

                    Label_5.SetPosition(352, 95);
                    Label_5.SetSize(150, 36);
                    Label_5.Anchors = Anchors.None;
                    Label_5.Visible = true;

                    panel3.SetPosition(352, 131);
                    panel3.SetSize(150, 130);
                    panel3.Anchors = Anchors.None;
                    panel3.Visible = true;

                    image3.SetPosition(0, -1);
                    image3.SetSize(150, 130);
                    image3.Anchors = Anchors.None;
                    image3.Visible = true;

                    Label_6.SetPosition(521, 95);
                    Label_6.SetSize(150, 36);
                    Label_6.Anchors = Anchors.None;
                    Label_6.Visible = true;

                    panel4.SetPosition(521, 131);
                    panel4.SetSize(150, 130);
                    panel4.Anchors = Anchors.None;
                    panel4.Visible = true;

                    image4.SetPosition(0, 0);
                    image4.SetSize(150, 130);
                    image4.Anchors = Anchors.None;
                    image4.Visible = true;

                    Label_7.SetPosition(689, 95);
                    Label_7.SetSize(150, 36);
                    Label_7.Anchors = Anchors.None;
                    Label_7.Visible = true;

                    panel5.SetPosition(689, 131);
                    panel5.SetSize(150, 130);
                    panel5.Anchors = Anchors.None;
                    panel5.Visible = true;

                    image5.SetPosition(0, 0);
                    image5.SetSize(150, 130);
                    image5.Anchors = Anchors.None;
                    image5.Visible = true;

                    panelH.SetPosition(133, 380);
                    panelH.SetSize(304, 91);
                    panelH.Anchors = Anchors.None;
                    panelH.Visible = true;

                    sliderH.SetPosition(0, 32);
                    sliderH.SetSize(300, 58);
                    sliderH.Anchors = Anchors.Height;
                    sliderH.Visible = true;

                    Label_8.SetPosition(0, 0);
                    Label_8.SetSize(300, 32);
                    Label_8.Anchors = Anchors.Height;
                    Label_8.Visible = true;

                    panelV.SetPosition(486, 380);
                    panelV.SetSize(304, 91);
                    panelV.Anchors = Anchors.None;
                    panelV.Visible = true;

                    sliderV.SetPosition(0, 32);
                    sliderV.SetSize(300, 58);
                    sliderV.Anchors = Anchors.Height;
                    sliderV.Visible = true;

                    Label_9.SetPosition(0, 0);
                    Label_9.SetSize(300, 32);
                    Label_9.Anchors = Anchors.Height;
                    Label_9.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            label1.Text = "No alpha";

            label2.Text = "Alpha";

            Label_3.Text = "Center";

            Label_4.Text = "Stretch";

            Label_5.Text = "AspectInside";

            Label_6.Text = "AspectOutSide";

            Label_7.Text = "NinePatch";

            Label_8.Text = "Width";

            Label_9.Text = "Height";
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
