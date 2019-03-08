// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class WallpaperSelectionPanel
    {
        GridListPanel GridListPanel_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            GridListPanel_1 = new GridListPanel(GridListScrollOrientation.Vertical);
            GridListPanel_1.Name = "GridListPanel_1";

            // GridListPanel_1
            GridListPanel_1.ScrollBarVisibility = ScrollBarVisibility.ScrollingVisible;
            GridListPanel_1.SetListItemCreator(WallpaperGridListItem.Creator);

            // WallpaperSelectionPanel
            this.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(GridListPanel_1);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(480, 700);
                    this.Anchors = Anchors.None;

                    GridListPanel_1.SetPosition(0, 0);
                    GridListPanel_1.SetSize(100, 50);
                    GridListPanel_1.Anchors = Anchors.None;
                    GridListPanel_1.Visible = true;
                    GridListPanel_1.ItemWidth = 120;
                    GridListPanel_1.ItemHeight = 120;

                    break;

                default:
                    this.SetSize(754, 480);
                    this.Anchors = Anchors.None;

                    GridListPanel_1.SetPosition(14, 0);
                    GridListPanel_1.SetSize(728, 480);
                    GridListPanel_1.Anchors = Anchors.None;
                    GridListPanel_1.Visible = true;
                    GridListPanel_1.ItemWidth = 120;
                    GridListPanel_1.ItemHeight = 120;

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
