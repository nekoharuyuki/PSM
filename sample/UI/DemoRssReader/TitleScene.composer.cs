// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class TitleScene
    {
        Panel sceneBackgroundPanel;
        Label titleLabel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            titleLabel = new Label();
            titleLabel.Name = "titleLabel";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // titleLabel
            titleLabel.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            titleLabel.Font = new UIFont(FontAlias.System, 72, FontStyle.Bold);
            titleLabel.TextTrimming = TextTrimming.None;
            titleLabel.LineBreak = LineBreak.Character;
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;

            // TitleScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(titleLabel);

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

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(480, 854);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    titleLabel.SetPosition(230, 91);
                    titleLabel.SetSize(214, 36);
                    titleLabel.Anchors = Anchors.None;
                    titleLabel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(854, 480);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    titleLabel.SetPosition(0, 0);
                    titleLabel.SetSize(854, 480);
                    titleLabel.Anchors = Anchors.None;
                    titleLabel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            titleLabel.Text = "RSS Reader";
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
