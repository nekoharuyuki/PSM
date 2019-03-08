// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class LiveSpringPanelScene
    {
        LiveSpringPanel springPanel;
        ImageBox background;
        LiveSphere sphere;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            springPanel = new LiveSpringPanel();
            springPanel.Name = "springPanel";
            background = new ImageBox();
            background.Name = "background";
            sphere = new LiveSphere();
            sphere.Name = "sphere";

            // LiveSpringPanelScene
            this.RootWidget.AddChildLast(springPanel);

            // springPanel
            springPanel.ReflectSensorAcceleration = true;
            springPanel.ReflectMotionAcceleration = true;
            springPanel.SetDampingConstant(null, SpringType.All, 0.2f);
            springPanel.SetSpringConstant(null, SpringType.All, 0.3f);
            springPanel.AddChildLast(background);
            springPanel.AddChildLast(sphere);

            // background
            background.Image = new ImageAsset("/Application/assets/spring_panel_normal.png");
            background.ImageScaleType = ImageScaleType.AspectInside;

            // sphere
            sphere.Image = new ImageAsset("/Application/assets/map.png");
            sphere.TurnCount = 16;
            sphere.TouchEnabled = false;

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

                    springPanel.SetPosition(100, 359);
                    springPanel.SetSize(314, 325);
                    springPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    springPanel.Visible = true;

                    background.SetPosition(0, 0);
                    background.SetSize(314, 325);
                    background.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    background.Visible = true;

                    sphere.SetPosition(84, 97);
                    sphere.SetSize(145, 131);
                    sphere.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    sphere.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    springPanel.SetPosition(302, 159);
                    springPanel.SetSize(250, 250);
                    springPanel.Anchors = Anchors.None;
                    springPanel.Visible = true;

                    background.SetPosition(0, 0);
                    background.SetSize(250, 250);
                    background.Anchors = Anchors.None;
                    background.Visible = true;

                    sphere.SetPosition(75, 75);
                    sphere.SetSize(100, 100);
                    sphere.Anchors = Anchors.None;
                    sphere.Visible = true;

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
