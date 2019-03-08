// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class FlipBoardEffectScene
    {
        Panel contentPanel;
        ImageBox image1;
        ImageBox image2;
        Panel Panel_1;
        Button buttonFlip;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            image1 = new ImageBox();
            image1.Name = "image1";
            image2 = new ImageBox();
            image2.Name = "image2";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonFlip = new Button();
            buttonFlip.Name = "buttonFlip";

            // FlipBoardEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(image1);
            contentPanel.AddChildLast(image2);
            contentPanel.AddChildLast(Panel_1);

            // image1
            image1.Image = new ImageAsset("/Application/assets/photo04.png");
            image1.ImageScaleType = ImageScaleType.AspectInside;

            // image2
            image2.Image = new ImageAsset("/Application/assets/photo05.png");
            image2.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonFlip);

            // buttonFlip
            buttonFlip.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonFlip.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 195);
                    contentPanel.SetSize(480, 659);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    image1.SetPosition(154, 174);
                    image1.SetSize(200, 200);
                    image1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image1.Visible = true;

                    image2.SetPosition(154, 174);
                    image2.SetSize(200, 200);
                    image2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image2.Visible = true;

                    Panel_1.SetPosition(61, 450);
                    Panel_1.SetSize(358, 100);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonFlip.SetPosition(72, 22);
                    buttonFlip.SetSize(214, 56);
                    buttonFlip.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonFlip.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    image1.SetPosition(300, 51);
                    image1.SetSize(500, 260);
                    image1.Anchors = Anchors.None;
                    image1.Visible = true;

                    image2.SetPosition(300, 51);
                    image2.SetSize(500, 260);
                    image2.Anchors = Anchors.None;
                    image2.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 56);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonFlip.SetPosition(0, 0);
                    buttonFlip.SetSize(200, 56);
                    buttonFlip.Anchors = Anchors.Top | Anchors.Height;
                    buttonFlip.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonFlip.Text = "Flip";
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
