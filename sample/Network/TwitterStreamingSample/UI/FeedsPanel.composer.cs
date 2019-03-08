/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterStreamingSample
{
    partial class FeedsPanel
    {
        ListPanel ListPanelFeeds;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ListPanelFeeds = new ListPanel();
            ListPanelFeeds.Name = "ListPanelFeeds";

            // FeedsPanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(ListPanelFeeds);

            // ListPanelFeeds
            ListPanelFeeds.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            ListPanelFeeds.ShowSection = false;
            ListPanelFeeds.SetListItemCreator(FeedListPanelItem.Creator);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 960);
                    this.Anchors = Anchors.None;

                    ListPanelFeeds.SetPosition(0, 0);
                    ListPanelFeeds.SetSize(544, 960);
                    ListPanelFeeds.Anchors = Anchors.None;
                    ListPanelFeeds.Visible = true;

                    break;

                default:
                    this.SetSize(860, 480);
                    this.Anchors = Anchors.None;

                    ListPanelFeeds.SetPosition(0, 0);
                    ListPanelFeeds.SetSize(860, 480);
                    ListPanelFeeds.Anchors = Anchors.None;
                    ListPanelFeeds.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public void StartDefaultEffect()
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
