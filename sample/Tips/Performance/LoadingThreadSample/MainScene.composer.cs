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
        Label Label_2;
        Label Label_3;
        Label Label_4;
        Label Label_5;
        Button Button_1;
        Button Button_2;
        Button Button_3;
        ProgressBar ProgressBar_1;
        BusyIndicator BusyIndicator_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Label_4 = new Label();
            Label_4.Name = "Label_4";
            Label_5 = new Label();
            Label_5.Name = "Label_5";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";
            Button_3 = new Button();
            Button_3.Name = "Button_3";
            ProgressBar_1 = new ProgressBar();
            ProgressBar_1.Name = "ProgressBar_1";
            BusyIndicator_1 = new BusyIndicator(true);
            BusyIndicator_1.Name = "BusyIndicator_1";

            // MainScene
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(Label_2);
            this.RootWidget.AddChildLast(Label_3);
            this.RootWidget.AddChildLast(Label_4);
            this.RootWidget.AddChildLast(Label_5);
            this.RootWidget.AddChildLast(Button_1);
            this.RootWidget.AddChildLast(Button_2);
            this.RootWidget.AddChildLast(Button_3);
            this.RootWidget.AddChildLast(ProgressBar_1);
            this.RootWidget.AddChildLast(BusyIndicator_1);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;
            Label_3.HorizontalAlignment = HorizontalAlignment.Right;

            // Label_4
            Label_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_4.LineBreak = LineBreak.Character;
            Label_4.HorizontalAlignment = HorizontalAlignment.Right;

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_5.LineBreak = LineBreak.Character;
            Label_5.HorizontalAlignment = HorizontalAlignment.Right;

            // Button_1
            Button_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Button_2
            Button_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Button_3
            Button_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_3.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    Label_2.SetPosition(78, 98);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(218, 338);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    Label_4.SetPosition(438, 418);
                    Label_4.SetSize(214, 36);
                    Label_4.Anchors = Anchors.None;
                    Label_4.Visible = true;

                    Label_5.SetPosition(438, 418);
                    Label_5.SetSize(214, 36);
                    Label_5.Anchors = Anchors.None;
                    Label_5.Visible = true;

                    Button_1.SetPosition(304, 365);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_1.Visible = true;

                    Button_2.SetPosition(304, 365);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_2.Visible = true;

                    Button_3.SetPosition(304, 365);
                    Button_3.SetSize(214, 56);
                    Button_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_3.Visible = true;

                    ProgressBar_1.SetPosition(298, 342);
                    ProgressBar_1.SetSize(362, 16);
                    ProgressBar_1.Anchors = Anchors.Height;
                    ProgressBar_1.Visible = true;

                    BusyIndicator_1.SetPosition(112, 220);
                    BusyIndicator_1.SetSize(48, 48);
                    BusyIndicator_1.Anchors = Anchors.Height | Anchors.Width;
                    BusyIndicator_1.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    Label_1.SetPosition(20, 20);
                    Label_1.SetSize(320, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(240, 236);
                    Label_2.SetSize(220, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(520, 270);
                    Label_3.SetSize(134, 36);
                    Label_3.Anchors = Anchors.Bottom | Anchors.Height;
                    Label_3.Visible = true;

                    Label_4.SetPosition(521, 351);
                    Label_4.SetSize(132, 33);
                    Label_4.Anchors = Anchors.Bottom | Anchors.Height;
                    Label_4.Visible = true;

                    Label_5.SetPosition(521, 431);
                    Label_5.SetSize(132, 33);
                    Label_5.Anchors = Anchors.Bottom | Anchors.Height;
                    Label_5.Visible = true;

                    Button_1.SetPosition(680, 260);
                    Button_1.SetSize(220, 56);
                    Button_1.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_1.Visible = true;

                    Button_2.SetPosition(680, 340);
                    Button_2.SetSize(220, 56);
                    Button_2.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_2.Visible = true;

                    Button_3.SetPosition(680, 420);
                    Button_3.SetSize(220, 56);
                    Button_3.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_3.Visible = true;

                    ProgressBar_1.SetPosition(224, 272);
                    ProgressBar_1.SetSize(256, 16);
                    ProgressBar_1.Anchors = Anchors.Height;
                    ProgressBar_1.Visible = true;

                    BusyIndicator_1.SetPosition(160, 240);
                    BusyIndicator_1.SetSize(48, 48);
                    BusyIndicator_1.Anchors = Anchors.Height | Anchors.Width;
                    BusyIndicator_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Loading Thread Sample";

            Label_2.Text = "READY";

            Button_1.Text = "Batch";

            Button_2.Text = "Sequential";

            Button_3.Text = "Multithreaded";
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
