// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class TiltDropEffectScene
    {
        Panel contentPanel;
        ImageBox image;
        Panel Panel_1;
        Button buttonTiltDrop;
        Button buttonReset;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            image = new ImageBox();
            image.Name = "image";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonTiltDrop = new Button();
            buttonTiltDrop.Name = "buttonTiltDrop";
            buttonReset = new Button();
            buttonReset.Name = "buttonReset";

            // TiltDropEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(image);
            contentPanel.AddChildLast(Panel_1);

            // image
            image.Image = new ImageAsset("/Application/assets/photo05.png");
            image.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonTiltDrop);
            Panel_1.AddChildLast(buttonReset);

            // buttonTiltDrop
            buttonTiltDrop.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonTiltDrop.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // buttonReset
            buttonReset.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonReset.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 233);
                    contentPanel.SetSize(480, 620);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    image.SetPosition(140, 269);
                    image.SetSize(200, 200);
                    image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image.Visible = true;

                    Panel_1.SetPosition(14, 519);
                    Panel_1.SetSize(452, 94);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonTiltDrop.SetPosition(15, 23);
                    buttonTiltDrop.SetSize(214, 56);
                    buttonTiltDrop.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonTiltDrop.Visible = true;

                    buttonReset.SetPosition(238, 23);
                    buttonReset.SetSize(214, 56);
                    buttonReset.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonReset.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    image.SetPosition(300, 51);
                    image.SetSize(500, 260);
                    image.Anchors = Anchors.None;
                    image.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 162);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonTiltDrop.SetPosition(0, 0);
                    buttonTiltDrop.SetSize(200, 56);
                    buttonTiltDrop.Anchors = Anchors.Top | Anchors.Height;
                    buttonTiltDrop.Visible = true;

                    buttonReset.SetPosition(0, 106);
                    buttonReset.SetSize(200, 56);
                    buttonReset.Anchors = Anchors.Top | Anchors.Height;
                    buttonReset.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonTiltDrop.Text = "TiltDrop";

            buttonReset.Text = "Reset";
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
