// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    partial class MainScene
    {
        Label Label_1;
        ImageBox ImageBox_1;
        Label Label_2;
        ImageBox ImageBox_2;
        Label Label_3;
        Label Label_4;
        Label Label_5;
        Label Label_6;
        Label Label_7;
        Label Label_8;
        Label Label_9;
        Label Label_10;
        Label Label_11;
        Label Label_12;
        ImageBox ImageBox_3;
        Label Label_13;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Label_4 = new Label();
            Label_4.Name = "Label_4";
            Label_5 = new Label();
            Label_5.Name = "Label_5";
            Label_6 = new Label();
            Label_6.Name = "Label_6";
            Label_7 = new Label();
            Label_7.Name = "Label_7";
            Label_8 = new Label();
            Label_8.Name = "Label_8";
            Label_9 = new Label();
            Label_9.Name = "Label_9";
            Label_10 = new Label();
            Label_10.Name = "Label_10";
            Label_11 = new Label();
            Label_11.Name = "Label_11";
            Label_12 = new Label();
            Label_12.Name = "Label_12";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            Label_13 = new Label();
            Label_13.Name = "Label_13";

            // MainScene
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(Label_2);
            this.RootWidget.AddChildLast(ImageBox_2);
            this.RootWidget.AddChildLast(Label_3);
            this.RootWidget.AddChildLast(Label_4);
            this.RootWidget.AddChildLast(Label_5);
            this.RootWidget.AddChildLast(Label_6);
            this.RootWidget.AddChildLast(Label_7);
            this.RootWidget.AddChildLast(Label_8);
            this.RootWidget.AddChildLast(Label_9);
            this.RootWidget.AddChildLast(Label_10);
            this.RootWidget.AddChildLast(Label_11);
            this.RootWidget.AddChildLast(Label_12);
            this.RootWidget.AddChildLast(ImageBox_3);
            this.RootWidget.AddChildLast(Label_13);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/floor.png");

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Right;

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/object.png");

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;

            // Label_4
            Label_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_4.LineBreak = LineBreak.Character;

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_5.LineBreak = LineBreak.Character;

            // Label_6
            Label_6.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_6.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_6.LineBreak = LineBreak.Character;

            // Label_7
            Label_7.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_7.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_7.LineBreak = LineBreak.Character;

            // Label_8
            Label_8.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_8.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_8.LineBreak = LineBreak.Character;

            // Label_9
            Label_9.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_9.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_9.LineBreak = LineBreak.Character;

            // Label_10
            Label_10.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_10.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_10.LineBreak = LineBreak.Character;

            // Label_11
            Label_11.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_11.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_11.LineBreak = LineBreak.Character;

            // Label_12
            Label_12.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_12.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_12.LineBreak = LineBreak.Character;

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/touch.png");

            // Label_13
            Label_13.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_13.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_13.LineBreak = LineBreak.AtCode;
            Label_13.HorizontalAlignment = HorizontalAlignment.Center;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 544;
                    this.DesignHeight = 960;

                    Label_1.SetPosition(77, 58);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    ImageBox_1.SetPosition(360, 130);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Label_2.SetPosition(420, 420);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    ImageBox_2.SetPosition(360, 130);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Label_3.SetPosition(40, 120);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    Label_4.SetPosition(40, 120);
                    Label_4.SetSize(214, 36);
                    Label_4.Anchors = Anchors.None;
                    Label_4.Visible = true;

                    Label_5.SetPosition(40, 120);
                    Label_5.SetSize(214, 36);
                    Label_5.Anchors = Anchors.None;
                    Label_5.Visible = true;

                    Label_6.SetPosition(40, 120);
                    Label_6.SetSize(214, 36);
                    Label_6.Anchors = Anchors.None;
                    Label_6.Visible = true;

                    Label_7.SetPosition(40, 120);
                    Label_7.SetSize(214, 36);
                    Label_7.Anchors = Anchors.None;
                    Label_7.Visible = true;

                    Label_8.SetPosition(40, 120);
                    Label_8.SetSize(214, 36);
                    Label_8.Anchors = Anchors.None;
                    Label_8.Visible = true;

                    Label_9.SetPosition(40, 120);
                    Label_9.SetSize(214, 36);
                    Label_9.Anchors = Anchors.None;
                    Label_9.Visible = true;

                    Label_10.SetPosition(40, 120);
                    Label_10.SetSize(214, 36);
                    Label_10.Anchors = Anchors.None;
                    Label_10.Visible = true;

                    Label_11.SetPosition(40, 120);
                    Label_11.SetSize(214, 36);
                    Label_11.Anchors = Anchors.None;
                    Label_11.Visible = true;

                    Label_12.SetPosition(40, 120);
                    Label_12.SetSize(214, 36);
                    Label_12.Anchors = Anchors.None;
                    Label_12.Visible = true;

                    ImageBox_3.SetPosition(360, 130);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    Label_13.SetPosition(658, 140);
                    Label_13.SetSize(214, 36);
                    Label_13.Anchors = Anchors.None;
                    Label_13.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    Label_1.SetPosition(20, 20);
                    Label_1.SetSize(320, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    ImageBox_1.SetPosition(320, 380);
                    ImageBox_1.SetSize(320, 100);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Label_2.SetPosition(660, 460);
                    Label_2.SetSize(240, 36);
                    Label_2.Anchors = Anchors.Bottom | Anchors.Right;
                    Label_2.Visible = true;

                    ImageBox_2.SetPosition(400, 50);
                    ImageBox_2.SetSize(160, 100);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Label_3.SetPosition(40, 120);
                    Label_3.SetSize(160, 36);
                    Label_3.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_3.Visible = true;

                    Label_4.SetPosition(40, 160);
                    Label_4.SetSize(160, 36);
                    Label_4.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_4.Visible = true;

                    Label_5.SetPosition(40, 200);
                    Label_5.SetSize(160, 36);
                    Label_5.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_5.Visible = true;

                    Label_6.SetPosition(40, 240);
                    Label_6.SetSize(160, 36);
                    Label_6.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_6.Visible = true;

                    Label_7.SetPosition(40, 280);
                    Label_7.SetSize(160, 36);
                    Label_7.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_7.Visible = true;

                    Label_8.SetPosition(40, 320);
                    Label_8.SetSize(160, 36);
                    Label_8.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_8.Visible = true;

                    Label_9.SetPosition(40, 360);
                    Label_9.SetSize(160, 36);
                    Label_9.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_9.Visible = true;

                    Label_10.SetPosition(40, 400);
                    Label_10.SetSize(160, 36);
                    Label_10.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_10.Visible = true;

                    Label_11.SetPosition(40, 440);
                    Label_11.SetSize(160, 36);
                    Label_11.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_11.Visible = true;

                    Label_12.SetPosition(40, 480);
                    Label_12.SetSize(160, 36);
                    Label_12.Anchors = Anchors.Bottom | Anchors.Left;
                    Label_12.Visible = true;

                    ImageBox_3.SetPosition(804, 344);
                    ImageBox_3.SetSize(96, 96);
                    ImageBox_3.Anchors = Anchors.Bottom | Anchors.Right;
                    ImageBox_3.Visible = true;

                    Label_13.SetPosition(618, 40);
                    Label_13.SetSize(322, 124);
                    Label_13.Anchors = Anchors.Top | Anchors.Right;
                    Label_13.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Touch Delay Sample";

            Label_13.Text = "Please tap the display\njust when the object\ntouches the floor.";
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
