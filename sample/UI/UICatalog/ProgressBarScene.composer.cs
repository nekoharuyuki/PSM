// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ProgressBarScene
    {
        Panel contentPanel;
        ProgressBar normalProgressBar;
        ProgressBar animationProgressBar;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            normalProgressBar = new ProgressBar();
            normalProgressBar.Name = "normalProgressBar";
            animationProgressBar = new ProgressBar();
            animationProgressBar.Name = "animationProgressBar";

            // ProgressBarScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(normalProgressBar);
            contentPanel.AddChildLast(animationProgressBar);

            // animationProgressBar
            animationProgressBar.Style = ProgressBarStyle.Animation;

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

                    normalProgressBar.SetPosition(59, 166);
                    normalProgressBar.SetSize(362, 16);
                    normalProgressBar.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    normalProgressBar.Visible = true;

                    animationProgressBar.SetPosition(59, 339);
                    animationProgressBar.SetSize(362, 16);
                    animationProgressBar.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    animationProgressBar.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    normalProgressBar.SetPosition(247, 115);
                    normalProgressBar.SetSize(360, 16);
                    normalProgressBar.Anchors = Anchors.Height;
                    normalProgressBar.Visible = true;

                    animationProgressBar.SetPosition(247, 220);
                    animationProgressBar.SetSize(360, 16);
                    animationProgressBar.Anchors = Anchors.Height;
                    animationProgressBar.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
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
