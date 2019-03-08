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
        Button Button_1;
        ImageBox ImageBox_1;
        Slider Slider_1;
        Label Label_2;
        Button Button_2;
        ImageBox ImageBox_2;
        Label Label_3;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Slider_1 = new Slider();
            Slider_1.Name = "Slider_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Button_2 = new Button();
            Button_2.Name = "Button_2";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";

            // MainScene
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(Button_1);
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(Slider_1);
            this.RootWidget.AddChildLast(Label_2);
            this.RootWidget.AddChildLast(Button_2);
            this.RootWidget.AddChildLast(ImageBox_2);
            this.RootWidget.AddChildLast(Label_3);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // Button_1
            Button_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/object.png");

            // Slider_1
            Slider_1.MinValue = 0;
            Slider_1.MaxValue = 100;
            Slider_1.Value = 0;
            Slider_1.Step = 1;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Right;

            // Button_2
            Button_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/sound.png");

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.AtCode;
            Label_3.HorizontalAlignment = HorizontalAlignment.Center;

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

                    Button_1.SetPosition(304, 365);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_1.Visible = true;

                    ImageBox_1.SetPosition(360, 130);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Slider_1.SetPosition(38, 420);
                    Slider_1.SetSize(362, 58);
                    Slider_1.Anchors = Anchors.Height;
                    Slider_1.Visible = true;

                    Label_2.SetPosition(420, 420);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Button_2.SetPosition(304, 365);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_2.Visible = true;

                    ImageBox_2.SetPosition(60, 100);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Label_3.SetPosition(658, 118);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    Label_1.SetPosition(20, 20);
                    Label_1.SetSize(320, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Button_1.SetPosition(60, 420);
                    Button_1.SetSize(220, 56);
                    Button_1.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_1.Visible = true;

                    ImageBox_1.SetPosition(400, 180);
                    ImageBox_1.SetSize(160, 100);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Slider_1.SetPosition(300, 382);
                    Slider_1.SetSize(362, 58);
                    Slider_1.Anchors = Anchors.Bottom | Anchors.Height;
                    Slider_1.Visible = true;

                    Label_2.SetPosition(532, 430);
                    Label_2.SetSize(118, 36);
                    Label_2.Anchors = Anchors.Bottom;
                    Label_2.Visible = true;

                    Button_2.SetPosition(680, 420);
                    Button_2.SetSize(220, 56);
                    Button_2.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_2.Visible = true;

                    ImageBox_2.SetPosition(60, 100);
                    ImageBox_2.SetSize(96, 96);
                    ImageBox_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    ImageBox_2.Visible = true;

                    Label_3.SetPosition(618, 40);
                    Label_3.SetSize(322, 124);
                    Label_3.Anchors = Anchors.Top | Anchors.Right;
                    Label_3.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Sound Delay Sample";

            Button_1.Text = "Earlier";

            Button_2.Text = "Later";

            Label_3.Text = "Please adjust the timing of\nthe object animation in\nresponse to sound.";
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
