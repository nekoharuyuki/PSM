// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ZoomEffectScene
    {
        Panel contentPanel;
        ImageBox imageBox1;
        ImageBox imageBox2;
        Panel Panel_1;
        Button buttonZoom;
        Label Label_1;
        PopupList popupInterpolator;

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
            imageBox2 = new ImageBox();
            imageBox2.Name = "imageBox2";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            buttonZoom = new Button();
            buttonZoom.Name = "buttonZoom";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            popupInterpolator = new PopupList();
            popupInterpolator.Name = "popupInterpolator";

            // ZoomEffectScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(imageBox1);
            contentPanel.AddChildLast(imageBox2);
            contentPanel.AddChildLast(Panel_1);

            // imageBox1
            imageBox1.Image = new ImageAsset("/Application/assets/photo06.png");
            imageBox1.ImageScaleType = ImageScaleType.AspectInside;

            // imageBox2
            imageBox2.Image = new ImageAsset("/Application/assets/photo02_s.png");
            imageBox2.ImageScaleType = ImageScaleType.AspectInside;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(buttonZoom);
            Panel_1.AddChildLast(Label_1);
            Panel_1.AddChildLast(popupInterpolator);

            // buttonZoom
            buttonZoom.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonZoom.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.VerticalAlignment = VerticalAlignment.Bottom;

            // popupInterpolator
            popupInterpolator.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupInterpolator.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupInterpolator.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupInterpolator.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupInterpolator.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupInterpolator.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 203);
                    contentPanel.SetSize(473, 650);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    imageBox1.SetPosition(28, 138);
                    imageBox1.SetSize(200, 200);
                    imageBox1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox1.Visible = true;

                    imageBox2.SetPosition(235, 138);
                    imageBox2.SetSize(200, 200);
                    imageBox2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    imageBox2.Visible = true;

                    Panel_1.SetPosition(11, 326);
                    Panel_1.SetSize(450, 303);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    buttonZoom.SetPosition(128, 51);
                    buttonZoom.SetSize(214, 56);
                    buttonZoom.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonZoom.Visible = true;

                    Label_1.SetPosition(17, 167);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    popupInterpolator.SetPosition(36, 212);
                    popupInterpolator.SetSize(360, 56);
                    popupInterpolator.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupInterpolator.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    imageBox1.SetPosition(363, 128);
                    imageBox1.SetSize(150, 100);
                    imageBox1.Anchors = Anchors.None;
                    imageBox1.Visible = true;

                    imageBox2.SetPosition(577, 128);
                    imageBox2.SetSize(150, 100);
                    imageBox2.Anchors = Anchors.None;
                    imageBox2.Visible = true;

                    Panel_1.SetPosition(60, 51);
                    Panel_1.SetSize(200, 162);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    buttonZoom.SetPosition(0, 0);
                    buttonZoom.SetSize(200, 56);
                    buttonZoom.Anchors = Anchors.Top | Anchors.Height;
                    buttonZoom.Visible = true;

                    Label_1.SetPosition(0, 70);
                    Label_1.SetSize(200, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height;
                    Label_1.Visible = true;

                    popupInterpolator.SetPosition(0, 106);
                    popupInterpolator.SetSize(200, 56);
                    popupInterpolator.Anchors = Anchors.Top | Anchors.Height;
                    popupInterpolator.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonZoom.Text = "Zoom";

            Label_1.Text = "Interpolator";

            popupInterpolator.ListItems.Clear();
            popupInterpolator.ListItems.AddRange(new String[]
            {
                "Linear",
                "EaseOutQuad",
                "Overshoot",
                "Elastic",
            });
            popupInterpolator.SelectedIndex = 1;
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
