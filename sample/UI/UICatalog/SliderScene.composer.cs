// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SliderScene
    {
        Panel contentPanel;
        Panel panel;
        Panel Panel_1;
        Slider slider4;
        Label label4;
        Panel Panel_2;
        Label label1;
        Slider slider1;
        Panel Panel_3;
        Label label2;
        Slider slider2;
        Panel Panel_4;
        Label label3;
        Slider slider3;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            panel = new Panel();
            panel.Name = "panel";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            slider4 = new Slider();
            slider4.Name = "slider4";
            label4 = new Label();
            label4.Name = "label4";
            Panel_2 = new Panel();
            Panel_2.Name = "Panel_2";
            label1 = new Label();
            label1.Name = "label1";
            slider1 = new Slider();
            slider1.Name = "slider1";
            Panel_3 = new Panel();
            Panel_3.Name = "Panel_3";
            label2 = new Label();
            label2.Name = "label2";
            slider2 = new Slider();
            slider2.Name = "slider2";
            Panel_4 = new Panel();
            Panel_4.Name = "Panel_4";
            label3 = new Label();
            label3.Name = "label3";
            slider3 = new Slider();
            slider3.Name = "slider3";

            // SliderScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(panel);

            // panel
            panel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            panel.Clip = true;
            panel.AddChildLast(Panel_1);
            panel.AddChildLast(Panel_2);
            panel.AddChildLast(Panel_3);
            panel.AddChildLast(Panel_4);

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(slider4);
            Panel_1.AddChildLast(label4);

            // slider4
            slider4.Orientation = SliderOrientation.Vertical;
            slider4.MinValue = 0;
            slider4.MaxValue = 255;
            slider4.Value = 128;
            slider4.Step = 1;

            // label4
            label4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label4.LineBreak = LineBreak.Character;
            label4.HorizontalAlignment = HorizontalAlignment.Center;
            label4.VerticalAlignment = VerticalAlignment.Bottom;
            label4.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // Panel_2
            Panel_2.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_2.Clip = true;
            Panel_2.AddChildLast(label1);
            Panel_2.AddChildLast(slider1);

            // label1
            label1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label1.LineBreak = LineBreak.Character;
            label1.VerticalAlignment = VerticalAlignment.Bottom;
            label1.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // slider1
            slider1.MinValue = 0;
            slider1.MaxValue = 255;
            slider1.Value = 200;
            slider1.Step = 1;

            // Panel_3
            Panel_3.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_3.Clip = true;
            Panel_3.AddChildLast(label2);
            Panel_3.AddChildLast(slider2);

            // label2
            label2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label2.LineBreak = LineBreak.Character;
            label2.VerticalAlignment = VerticalAlignment.Bottom;
            label2.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // slider2
            slider2.MinValue = 0;
            slider2.MaxValue = 255;
            slider2.Value = 0;
            slider2.Step = 1;

            // Panel_4
            Panel_4.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_4.Clip = true;
            Panel_4.AddChildLast(label3);
            Panel_4.AddChildLast(slider3);

            // label3
            label3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label3.LineBreak = LineBreak.Character;
            label3.VerticalAlignment = VerticalAlignment.Bottom;
            label3.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // slider3
            slider3.MinValue = 0;
            slider3.MaxValue = 255;
            slider3.Value = 0;
            slider3.Step = 1;

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

                    contentPanel.SetPosition(0, 213);
                    contentPanel.SetSize(480, 640);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    panel.SetPosition(12, 69);
                    panel.SetSize(449, 557);
                    panel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel.Visible = true;

                    Panel_1.SetPosition(286, 77);
                    Panel_1.SetSize(162, 469);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    slider4.SetPosition(92, 94);
                    slider4.SetSize(58, 362);
                    slider4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    slider4.Visible = true;

                    label4.SetPosition(41, 21);
                    label4.SetSize(110, 38);
                    label4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label4.Visible = true;

                    Panel_2.SetPosition(23, 182);
                    Panel_2.SetSize(293, 127);
                    Panel_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_2.Visible = true;

                    label1.SetPosition(21, 19);
                    label1.SetSize(214, 36);
                    label1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label1.Visible = true;

                    slider1.SetPosition(6, 67);
                    slider1.SetSize(268, 62);
                    slider1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    slider1.Visible = true;

                    Panel_3.SetPosition(14, 312);
                    Panel_3.SetSize(313, 131);
                    Panel_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_3.Visible = true;

                    label2.SetPosition(15, 16);
                    label2.SetSize(214, 36);
                    label2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label2.Visible = true;

                    slider2.SetPosition(9, 62);
                    slider2.SetSize(281, 58);
                    slider2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    slider2.Visible = true;

                    Panel_4.SetPosition(14, 444);
                    Panel_4.SetSize(312, 102);
                    Panel_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_4.Visible = true;

                    label3.SetPosition(9, 8);
                    label3.SetSize(214, 36);
                    label3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label3.Visible = true;

                    slider3.SetPosition(9, 44);
                    slider3.SetSize(293, 58);
                    slider3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    slider3.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    panel.SetPosition(109, 28);
                    panel.SetSize(634, 322);
                    panel.Anchors = Anchors.None;
                    panel.Visible = true;

                    Panel_1.SetPosition(479, 24);
                    Panel_1.SetSize(100, 281);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    slider4.SetPosition(20, 37);
                    slider4.SetSize(58, 245);
                    slider4.Anchors = Anchors.Width;
                    slider4.Visible = true;

                    label4.SetPosition(0, 0);
                    label4.SetSize(100, 36);
                    label4.Anchors = Anchors.Width;
                    label4.Visible = true;

                    Panel_2.SetPosition(64, 24);
                    Panel_2.SetSize(362, 94);
                    Panel_2.Anchors = Anchors.None;
                    Panel_2.Visible = true;

                    label1.SetPosition(0, 0);
                    label1.SetSize(355, 36);
                    label1.Anchors = Anchors.None;
                    label1.Visible = true;

                    slider1.SetPosition(0, 36);
                    slider1.SetSize(355, 58);
                    slider1.Anchors = Anchors.Height;
                    slider1.Visible = true;

                    Panel_3.SetPosition(64, 118);
                    Panel_3.SetSize(362, 94);
                    Panel_3.Anchors = Anchors.None;
                    Panel_3.Visible = true;

                    label2.SetPosition(0, 0);
                    label2.SetSize(355, 36);
                    label2.Anchors = Anchors.None;
                    label2.Visible = true;

                    slider2.SetPosition(0, 36);
                    slider2.SetSize(355, 58);
                    slider2.Anchors = Anchors.Height;
                    slider2.Visible = true;

                    Panel_4.SetPosition(64, 212);
                    Panel_4.SetSize(362, 94);
                    Panel_4.Anchors = Anchors.None;
                    Panel_4.Visible = true;

                    label3.SetPosition(0, 0);
                    label3.SetSize(355, 36);
                    label3.Anchors = Anchors.None;
                    label3.Visible = true;

                    slider3.SetPosition(0, 35);
                    slider3.SetSize(355, 58);
                    slider3.Anchors = Anchors.Height;
                    slider3.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            label4.Text = "A:";

            label1.Text = "R: ";

            label2.Text = "G: ";

            label3.Text = "B: ";
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
