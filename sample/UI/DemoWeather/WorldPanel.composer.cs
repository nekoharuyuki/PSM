// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Weather
{
    partial class WorldPanel
    {
        ImageBox worldMap;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            worldMap = new ImageBox();
            worldMap.Name = "worldMap";

            // worldMap
            worldMap.Image = new ImageAsset("/Application/assets/world_map.png");

            // WorldPanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(worldMap);

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

                    worldMap.SetPosition(472, 72);
                    worldMap.SetSize(200, 200);
                    worldMap.Anchors = Anchors.None;
                    worldMap.Visible = true;

                    break;

                default:
                    this.SetSize(1708, 960);
                    this.Anchors = Anchors.None;

                    worldMap.SetPosition(0, 0);
                    worldMap.SetSize(1708, 960);
                    worldMap.Anchors = Anchors.None;
                    worldMap.Visible = true;

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
