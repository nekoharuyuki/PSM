// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class GridListPanelScene
    {
        Panel contentPanel;
        GridListPanel gridListPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            gridListPanel = new GridListPanel(GridListScrollOrientation.Horizontal);
            gridListPanel.Name = "gridListPanel";

            // GridListPanelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(gridListPanel);

            // gridListPanel
            gridListPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollingVisible;
            gridListPanel.SetListItemCreator(SampleGridListPanelItem.Creator);

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

                    contentPanel.SetPosition(0, 123);
                    contentPanel.SetSize(480, 730);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    gridListPanel.SetPosition(13, 67);
                    gridListPanel.SetSize(453, 643);
                    gridListPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    gridListPanel.Visible = true;
                    gridListPanel.ItemWidth = 40;
                    gridListPanel.ItemHeight = 60;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    gridListPanel.SetPosition(127, 1);
                    gridListPanel.SetSize(600, 370);
                    gridListPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    gridListPanel.Visible = true;
                    gridListPanel.ItemWidth = 60;
                    gridListPanel.ItemHeight = 40;

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
