// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class FadeInOutEffectScene
    {
        Panel contentPanel;
        ImageBox imageBox;
        Panel Panel_1;
        Button buttonFadeIn;
        Button buttonFadeOut;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            imageBox = new ImageBox();
            imageBox.Name = "imageBox";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonFadeIn = new Button();
            buttonFadeIn.Name = "buttonFadeIn";
            buttonFadeOut = new Button();
            buttonFadeOut.Name = "buttonFadeOut";

            // FadeInOutEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(imageBox);
            contentPanel.AddChildLast(Panel_1);

            // imageBox
            imageBox.Image = new ImageAsset("/Application/assets/photo05.png");
            imageBox.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonFadeIn);
            Panel_1.AddChildLast(buttonFadeOut);

            // buttonFadeIn
            buttonFadeIn.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonFadeIn.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // buttonFadeOut
            buttonFadeOut.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonFadeOut.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 241);
                    contentPanel.SetSize(480, 612);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    imageBox.SetPosition(147, 57);
                    imageBox.SetSize(200, 200);
                    imageBox.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox.Visible = true;

                    Panel_1.SetPosition(0, 327);
                    Panel_1.SetSize(480, 285);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonFadeIn.SetPosition(133, 172);
                    buttonFadeIn.SetSize(214, 56);
                    buttonFadeIn.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonFadeIn.Visible = true;

                    buttonFadeOut.SetPosition(133, 68);
                    buttonFadeOut.SetSize(214, 56);
                    buttonFadeOut.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonFadeOut.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    imageBox.SetPosition(300, 51);
                    imageBox.SetSize(500, 260);
                    imageBox.Anchors = Anchors.None;
                    imageBox.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 162);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonFadeIn.SetPosition(0, 0);
                    buttonFadeIn.SetSize(200, 56);
                    buttonFadeIn.Anchors = Anchors.Top | Anchors.Height;
                    buttonFadeIn.Visible = true;

                    buttonFadeOut.SetPosition(0, 106);
                    buttonFadeOut.SetSize(200, 56);
                    buttonFadeOut.Anchors = Anchors.Top | Anchors.Height;
                    buttonFadeOut.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonFadeIn.Text = "FadeIn";

            buttonFadeOut.Text = "FadeOut";
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
