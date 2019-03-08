// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class BunjeeJumpEffectScene
    {
        Panel contentPanel;
        ImageBox image;
        Panel Panel_1;
        Button buttonBunjee;

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
            buttonBunjee = new Button();
            buttonBunjee.Name = "buttonBunjee";

            // BunjeeJumpEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(image);
            contentPanel.AddChildLast(Panel_1);

            // image
            image.Image = new ImageAsset("/Application/assets/photo06.png");
            image.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonBunjee);

            // buttonBunjee
            buttonBunjee.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonBunjee.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 207);
                    contentPanel.SetSize(480, 646);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    image.SetPosition(140, 292);
                    image.SetSize(200, 200);
                    image.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image.Visible = true;

                    Panel_1.SetPosition(51, 540);
                    Panel_1.SetSize(357, 106);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonBunjee.SetPosition(81, 25);
                    buttonBunjee.SetSize(214, 56);
                    buttonBunjee.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonBunjee.Visible = true;

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
                    Panel_1.SetSize(200, 56);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonBunjee.SetPosition(0, 0);
                    buttonBunjee.SetSize(200, 56);
                    buttonBunjee.Anchors = Anchors.Top | Anchors.Height;
                    buttonBunjee.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonBunjee.Text = "Bunjee!";
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
