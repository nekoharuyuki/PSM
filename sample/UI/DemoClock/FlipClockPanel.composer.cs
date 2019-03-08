// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    partial class FlipClockPanel
    {
        ImageBox ImageBox_10;
        ImageBox ImageBox_11;
        ImageBox ImageBox_1;
        ImageBox ImageBox_2;
        ImageBox ImageBox_3;
        ImageBox ImageBox_4;
        ImageBox ImageBox_5;
        ImageBox ImageBox_6;
        ImageBox ImageBox_7;
        ImageBox ImageBox_8;
        ImageBox ImageBox_9;
        Label Label_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_10 = new ImageBox();
            ImageBox_10.Name = "ImageBox_10";
            ImageBox_11 = new ImageBox();
            ImageBox_11.Name = "ImageBox_11";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            ImageBox_4 = new ImageBox();
            ImageBox_4.Name = "ImageBox_4";
            ImageBox_5 = new ImageBox();
            ImageBox_5.Name = "ImageBox_5";
            ImageBox_6 = new ImageBox();
            ImageBox_6.Name = "ImageBox_6";
            ImageBox_7 = new ImageBox();
            ImageBox_7.Name = "ImageBox_7";
            ImageBox_8 = new ImageBox();
            ImageBox_8.Name = "ImageBox_8";
            ImageBox_9 = new ImageBox();
            ImageBox_9.Name = "ImageBox_9";
            Label_1 = new Label();
            Label_1.Name = "Label_1";

            // ImageBox_10
            ImageBox_10.Image = new ImageAsset("/Application/assets/Kyoto2.JPG");
            ImageBox_10.ImageScaleType = ImageScaleType.AspectOutside;

            // ImageBox_11
            ImageBox_11.Image = new ImageAsset("/Application/assets/flip_bg.png");
            ImageBox_11.ImageScaleType = ImageScaleType.AspectInside;

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/flip.png");
            ImageBox_1.ImageScaleType = ImageScaleType.AspectInside;

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/flip.png");

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/flip.png");

            // ImageBox_4
            ImageBox_4.Image = new ImageAsset("/Application/assets/num_1.png");

            // ImageBox_5
            ImageBox_5.Image = new ImageAsset("/Application/assets/num_2.png");

            // ImageBox_6
            ImageBox_6.Image = new ImageAsset("/Application/assets/num_3.png");

            // ImageBox_7
            ImageBox_7.Image = new ImageAsset("/Application/assets/num_4.png");

            // ImageBox_8
            ImageBox_8.Image = new ImageAsset("/Application/assets/num_5.png");

            // ImageBox_9
            ImageBox_9.Image = new ImageAsset("/Application/assets/num_6.png");

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Word;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // FlipClockPanel
            this.BackgroundColor = new UIColor(162f / 255f, 32f / 255f, 32f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(ImageBox_10);
            this.AddChildLast(ImageBox_11);
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(ImageBox_2);
            this.AddChildLast(ImageBox_3);
            this.AddChildLast(ImageBox_4);
            this.AddChildLast(ImageBox_5);
            this.AddChildLast(ImageBox_6);
            this.AddChildLast(ImageBox_7);
            this.AddChildLast(ImageBox_8);
            this.AddChildLast(ImageBox_9);
            this.AddChildLast(Label_1);

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

                    ImageBox_10.SetPosition(176, -100);
                    ImageBox_10.SetSize(200, 200);
                    ImageBox_10.Anchors = Anchors.None;
                    ImageBox_10.Visible = true;

                    ImageBox_11.SetPosition(347, 96);
                    ImageBox_11.SetSize(200, 200);
                    ImageBox_11.Anchors = Anchors.None;
                    ImageBox_11.Visible = true;

                    ImageBox_1.SetPosition(320, 105);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = false;

                    ImageBox_2.SetPosition(292, 104);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = false;

                    ImageBox_3.SetPosition(598, 102);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = false;

                    ImageBox_4.SetPosition(-21, 127);
                    ImageBox_4.SetSize(200, 200);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = false;

                    ImageBox_5.SetPosition(254, 135);
                    ImageBox_5.SetSize(200, 200);
                    ImageBox_5.Anchors = Anchors.None;
                    ImageBox_5.Visible = false;

                    ImageBox_6.SetPosition(252, 127);
                    ImageBox_6.SetSize(200, 200);
                    ImageBox_6.Anchors = Anchors.None;
                    ImageBox_6.Visible = false;

                    ImageBox_7.SetPosition(382, 117);
                    ImageBox_7.SetSize(200, 200);
                    ImageBox_7.Anchors = Anchors.None;
                    ImageBox_7.Visible = false;

                    ImageBox_8.SetPosition(534, 127);
                    ImageBox_8.SetSize(200, 200);
                    ImageBox_8.Anchors = Anchors.None;
                    ImageBox_8.Visible = false;

                    ImageBox_9.SetPosition(654, 127);
                    ImageBox_9.SetSize(200, 200);
                    ImageBox_9.Anchors = Anchors.None;
                    ImageBox_9.Visible = false;

                    Label_1.SetPosition(434, 93);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = false;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    ImageBox_10.SetPosition(0, 0);
                    ImageBox_10.SetSize(854, 480);
                    ImageBox_10.Anchors = Anchors.None;
                    ImageBox_10.Visible = true;

                    ImageBox_11.SetPosition(20, 19);
                    ImageBox_11.SetSize(350, 164);
                    ImageBox_11.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    ImageBox_11.Visible = true;

                    ImageBox_1.SetPosition(39, 39);
                    ImageBox_1.SetSize(92, 102);
                    ImageBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(149, 39);
                    ImageBox_2.SetSize(92, 102);
                    ImageBox_2.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_2.Visible = true;

                    ImageBox_3.SetPosition(259, 39);
                    ImageBox_3.SetSize(92, 102);
                    ImageBox_3.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(42, 58);
                    ImageBox_4.SetSize(40, 64);
                    ImageBox_4.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_4.Visible = true;

                    ImageBox_5.SetPosition(87, 58);
                    ImageBox_5.SetSize(40, 64);
                    ImageBox_5.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_5.Visible = true;

                    ImageBox_6.SetPosition(152, 58);
                    ImageBox_6.SetSize(46, 64);
                    ImageBox_6.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_6.Visible = true;

                    ImageBox_7.SetPosition(198, 58);
                    ImageBox_7.SetSize(40, 64);
                    ImageBox_7.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_7.Visible = true;

                    ImageBox_8.SetPosition(265, 58);
                    ImageBox_8.SetSize(40, 64);
                    ImageBox_8.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_8.Visible = true;

                    ImageBox_9.SetPosition(305, 58);
                    ImageBox_9.SetSize(40, 64);
                    ImageBox_9.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_9.Visible = true;

                    Label_1.SetPosition(39, 141);
                    Label_1.SetSize(312, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "label";
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
