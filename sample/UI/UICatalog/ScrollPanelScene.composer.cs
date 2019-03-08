// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ScrollPanelScene
    {
        Panel contentPanel;
        ScrollPanel scrollPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            scrollPanel = new ScrollPanel();
            scrollPanel.Name = "scrollPanel";

            // ScrollPanelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(scrollPanel);

            // scrollPanel
            scrollPanel.HorizontalScroll = true;
            scrollPanel.VerticalScroll = true;
            scrollPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            var scrollPanel_SampleScrollPanelInnerPanel = new SampleScrollPanelInnerPanel();
            scrollPanel.PanelWidth = scrollPanel_SampleScrollPanelInnerPanel.Width;
            scrollPanel.PanelHeight = scrollPanel_SampleScrollPanelInnerPanel.Height;
            scrollPanel.PanelX = 0;
            scrollPanel.PanelY = 0;
            scrollPanel.AddChildLast(scrollPanel_SampleScrollPanelInnerPanel);

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

                    contentPanel.SetPosition(0, 217);
                    contentPanel.SetSize(480, 636);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    scrollPanel.SetPosition(18, 18);
                    scrollPanel.SetSize(444, 597);
                    scrollPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    scrollPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    scrollPanel.SetPosition(0, 1);
                    scrollPanel.SetSize(854, 370);
                    scrollPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    scrollPanel.Visible = true;

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
