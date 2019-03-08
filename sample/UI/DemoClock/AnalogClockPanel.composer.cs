// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    partial class AnalogClockPanel
    {
        ImageBox ImageBox_5;
        ImageBox ImageBox_1;
        Label Label_1;
        ImageBox ImageBox_2;
        ImageBox ImageBox_3;
        ImageBox ImageBox_4;
        LiveJumpPanel LiveJumpPanel_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_5 = new ImageBox();
            ImageBox_5.Name = "ImageBox_5";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            ImageBox_4 = new ImageBox();
            ImageBox_4.Name = "ImageBox_4";
            LiveJumpPanel_1 = new LiveJumpPanel();
            LiveJumpPanel_1.Name = "LiveJumpPanel_1";

            // ImageBox_5
            ImageBox_5.Image = new ImageAsset("/Application/assets/Kyoto1.JPG");
            ImageBox_5.ImageScaleType = ImageScaleType.AspectOutside;

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/clock_bg.png");
            ImageBox_1.ImageScaleType = ImageScaleType.AspectOutside;

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Word;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/hour_hand_10.png");

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/minute_hand_16.png");

            // ImageBox_4
            ImageBox_4.Image = new ImageAsset("/Application/assets/second_hand.png");

            // LiveJumpPanel_1
            LiveJumpPanel_1.Clip = true;
            LiveJumpPanel_1.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            LiveJumpPanel_1.JumpHeight = 350f;
            LiveJumpPanel_1.JumpDelayTime = 1f;
            LiveJumpPanel_1.JumpTime = 500f;
            LiveJumpPanel_1.TiltAngle = 0.17f;
            LiveJumpPanel_1.AddChildLast(ImageBox_1);
            LiveJumpPanel_1.AddChildLast(Label_1);
            LiveJumpPanel_1.AddChildLast(ImageBox_2);
            LiveJumpPanel_1.AddChildLast(ImageBox_3);
            LiveJumpPanel_1.AddChildLast(ImageBox_4);

            // AnalogClockPanel
            this.BackgroundColor = new UIColor(47f / 255f, 77f / 255f, 195f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(ImageBox_5);
            this.AddChildLast(LiveJumpPanel_1);

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

                    ImageBox_5.SetPosition(-35, -130);
                    ImageBox_5.SetSize(200, 200);
                    ImageBox_5.Anchors = Anchors.None;
                    ImageBox_5.Visible = true;

                    ImageBox_1.SetPosition(-447, -151);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = false;

                    Label_1.SetPosition(-374, -135);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = false;

                    ImageBox_2.SetPosition(-285, 19);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = false;

                    ImageBox_3.SetPosition(-170, 35);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = false;

                    ImageBox_4.SetPosition(52, 71);
                    ImageBox_4.SetSize(200, 200);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = false;

                    LiveJumpPanel_1.SetPosition(704, 239);
                    LiveJumpPanel_1.SetSize(100, 100);
                    LiveJumpPanel_1.Anchors = Anchors.None;
                    LiveJumpPanel_1.Visible = false;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    ImageBox_5.SetPosition(0, 0);
                    ImageBox_5.SetSize(854, 480);
                    ImageBox_5.Anchors = Anchors.None;
                    ImageBox_5.Visible = true;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(212, 212);
                    ImageBox_1.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_1.Visible = true;

                    Label_1.SetPosition(0, 127);
                    Label_1.SetSize(212, 25);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    ImageBox_2.SetPosition(0, 0);
                    ImageBox_2.SetSize(10, 66);
                    ImageBox_2.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_2.Visible = true;

                    ImageBox_3.SetPosition(0, 0);
                    ImageBox_3.SetSize(6, 100);
                    ImageBox_3.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(0, 0);
                    ImageBox_4.SetSize(12, 96);
                    ImageBox_4.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_4.Visible = true;

                    LiveJumpPanel_1.SetPosition(20, 20);
                    LiveJumpPanel_1.SetSize(230, 217);
                    LiveJumpPanel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    LiveJumpPanel_1.Visible = true;

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
