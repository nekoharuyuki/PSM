// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SlideInOutEffectScene
    {
        Panel contentPanel;
        ImageBox imageBox1;
        ImageBox imageBox3;
        ImageBox imageBox2;
        ImageBox imageBox4;
        Panel Panel_1;
        Button buttonSlideIn;
        Button buttonSlideOut;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            imageBox1 = new ImageBox();
            imageBox1.Name = "imageBox1";
            imageBox3 = new ImageBox();
            imageBox3.Name = "imageBox3";
            imageBox2 = new ImageBox();
            imageBox2.Name = "imageBox2";
            imageBox4 = new ImageBox();
            imageBox4.Name = "imageBox4";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonSlideIn = new Button();
            buttonSlideIn.Name = "buttonSlideIn";
            buttonSlideOut = new Button();
            buttonSlideOut.Name = "buttonSlideOut";

            // SlideInOutEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(imageBox1);
            contentPanel.AddChildLast(imageBox3);
            contentPanel.AddChildLast(imageBox2);
            contentPanel.AddChildLast(imageBox4);
            contentPanel.AddChildLast(Panel_1);

            // imageBox1
            imageBox1.Image = new ImageAsset("/Application/assets/photo05.png");
            imageBox1.ImageScaleType = ImageScaleType.AspectInside;

            // imageBox3
            imageBox3.Image = new ImageAsset("/Application/assets/photo07.png");
            imageBox3.ImageScaleType = ImageScaleType.AspectInside;

            // imageBox2
            imageBox2.Image = new ImageAsset("/Application/assets/photo06.png");
            imageBox2.ImageScaleType = ImageScaleType.AspectInside;

            // imageBox4
            imageBox4.Image = new ImageAsset("/Application/assets/photo02.png");
            imageBox4.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonSlideIn);
            Panel_1.AddChildLast(buttonSlideOut);

            // buttonSlideIn
            buttonSlideIn.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonSlideIn.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // buttonSlideOut
            buttonSlideOut.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonSlideOut.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 214);
                    contentPanel.SetSize(480, 639);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    imageBox1.SetPosition(36, 135);
                    imageBox1.SetSize(200, 200);
                    imageBox1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox1.Visible = true;

                    imageBox3.SetPosition(36, 354);
                    imageBox3.SetSize(200, 200);
                    imageBox3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox3.Visible = true;

                    imageBox2.SetPosition(264, 135);
                    imageBox2.SetSize(200, 200);
                    imageBox2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox2.Visible = true;

                    imageBox4.SetPosition(264, 354);
                    imageBox4.SetSize(200, 200);
                    imageBox4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox4.Visible = true;

                    Panel_1.SetPosition(0, 541);
                    Panel_1.SetSize(480, 88);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonSlideIn.SetPosition(73, 12);
                    buttonSlideIn.SetSize(163, 56);
                    buttonSlideIn.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonSlideIn.Visible = true;

                    buttonSlideOut.SetPosition(285, 12);
                    buttonSlideOut.SetSize(123, 56);
                    buttonSlideOut.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonSlideOut.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    imageBox1.SetPosition(330, 51);
                    imageBox1.SetSize(200, 100);
                    imageBox1.Anchors = Anchors.None;
                    imageBox1.Visible = true;

                    imageBox3.SetPosition(330, 210);
                    imageBox3.SetSize(200, 100);
                    imageBox3.Anchors = Anchors.None;
                    imageBox3.Visible = true;

                    imageBox2.SetPosition(600, 51);
                    imageBox2.SetSize(200, 100);
                    imageBox2.Anchors = Anchors.None;
                    imageBox2.Visible = true;

                    imageBox4.SetPosition(600, 210);
                    imageBox4.SetSize(200, 100);
                    imageBox4.Anchors = Anchors.None;
                    imageBox4.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 162);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonSlideIn.SetPosition(0, 0);
                    buttonSlideIn.SetSize(200, 56);
                    buttonSlideIn.Anchors = Anchors.Top | Anchors.Height;
                    buttonSlideIn.Visible = true;

                    buttonSlideOut.SetPosition(0, 106);
                    buttonSlideOut.SetSize(200, 56);
                    buttonSlideOut.Anchors = Anchors.Top | Anchors.Height;
                    buttonSlideOut.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonSlideIn.Text = "SlideIn";

            buttonSlideOut.Text = "SlideOut";
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
