// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class LiveFlipPanelScene
    {
        Panel contentPanel;
        LiveFlipPanel liveFlipPanel;
        Panel Panel_1;
        Button startButton;
        Button stopButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            liveFlipPanel = new LiveFlipPanel();
            liveFlipPanel.Name = "liveFlipPanel";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            startButton = new Button();
            startButton.Name = "startButton";
            stopButton = new Button();
            stopButton.Name = "stopButton";

            // LiveFlipPanelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(liveFlipPanel);
            contentPanel.AddChildLast(Panel_1);

            // liveFlipPanel
            liveFlipPanel.FlipCount = 16;
            liveFlipPanel.TouchEnabled = true;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(startButton);
            Panel_1.AddChildLast(stopButton);

            // startButton
            startButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            startButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // stopButton
            stopButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            stopButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 206);
                    contentPanel.SetSize(480, 647);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    liveFlipPanel.SetPosition(78, 108);
                    liveFlipPanel.SetSize(335, 334);
                    liveFlipPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    liveFlipPanel.Visible = true;

                    Panel_1.SetPosition(0, 524);
                    Panel_1.SetSize(480, 122);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    startButton.SetPosition(8, 45);
                    startButton.SetSize(214, 56);
                    startButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    startButton.Visible = true;

                    stopButton.SetPosition(245, 45);
                    stopButton.SetSize(214, 56);
                    stopButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    stopButton.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    liveFlipPanel.SetPosition(350, 51);
                    liveFlipPanel.SetSize(390, 260);
                    liveFlipPanel.Anchors = Anchors.None;
                    liveFlipPanel.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 162);
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

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            startButton.Text = "Start";

            stopButton.Text = "Stop";
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
