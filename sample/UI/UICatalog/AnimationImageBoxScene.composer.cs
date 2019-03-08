// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class AnimationImageBoxScene
    {
        Panel contentPanel;
        Panel Panel_1;
        Button startButton;
        Button stopButton;
        Label labelInterval;
        Slider slider;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            startButton = new Button();
            startButton.Name = "startButton";
            stopButton = new Button();
            stopButton.Name = "stopButton";
            labelInterval = new Label();
            labelInterval.Name = "labelInterval";
            slider = new Slider();
            slider.Name = "slider";

            // AnimationImageBoxScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(Panel_1);

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(startButton);
            Panel_1.AddChildLast(stopButton);
            Panel_1.AddChildLast(labelInterval);
            Panel_1.AddChildLast(slider);

            // startButton
            startButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            startButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // stopButton
            stopButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            stopButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // labelInterval
            labelInterval.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelInterval.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelInterval.LineBreak = LineBreak.Character;

            // slider
            slider.MinValue = 100;
            slider.MaxValue = 1000;
            slider.Value = 333;
            slider.Step = 1;

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

                    contentPanel.SetPosition(0, 210);
                    contentPanel.SetSize(480, 644);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    contentPanel.Visible = true;

                    Panel_1.SetPosition(0, 0);
                    Panel_1.SetSize(480, 646);
                    Panel_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    Panel_1.Visible = true;

                    startButton.SetPosition(133, 94);
                    startButton.SetSize(214, 56);
                    startButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    startButton.Visible = true;

                    stopButton.SetPosition(133, 180);
                    stopButton.SetSize(214, 56);
                    stopButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    stopButton.Visible = true;

                    labelInterval.SetPosition(122, 451);
                    labelInterval.SetSize(214, 36);
                    labelInterval.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelInterval.Visible = true;

                    slider.SetPosition(70, 524);
                    slider.SetSize(362, 58);
                    slider.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    slider.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 109);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(250, 276);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    startButton.SetPosition(0, 0);
                    startButton.SetSize(200, 56);
                    startButton.Anchors = Anchors.Top | Anchors.Height;
                    startButton.Visible = true;

                    stopButton.SetPosition(0, 106);
                    stopButton.SetSize(200, 56);
                    stopButton.Anchors = Anchors.Top | Anchors.Height;
                    stopButton.Visible = true;

                    labelInterval.SetPosition(0, 182);
                    labelInterval.SetSize(250, 36);
                    labelInterval.Anchors = Anchors.Top | Anchors.Height;
                    labelInterval.Visible = true;

                    slider.SetPosition(0, 218);
                    slider.SetSize(200, 58);
                    slider.Anchors = Anchors.Top | Anchors.Height;
                    slider.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            startButton.Text = "Start";

            stopButton.Text = "Stop";

            labelInterval.Text = "Interval = 33.3 ms";
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
