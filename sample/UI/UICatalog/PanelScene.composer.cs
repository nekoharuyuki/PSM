// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class PanelScene
    {
        Panel contentPanel;
        Panel clipOffPanel;
        Panel panel1;
        ImageBox image1;
        Button button1;
        Panel clipOnPanel;
        Panel panel2;
        ImageBox image2;
        Button button2;
        Label Label_1;
        Label Label_2;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            clipOffPanel = new Panel();
            clipOffPanel.Name = "clipOffPanel";
            panel1 = new Panel();
            panel1.Name = "panel1";
            image1 = new ImageBox();
            image1.Name = "image1";
            button1 = new Button();
            button1.Name = "button1";
            clipOnPanel = new Panel();
            clipOnPanel.Name = "clipOnPanel";
            panel2 = new Panel();
            panel2.Name = "panel2";
            image2 = new ImageBox();
            image2.Name = "image2";
            button2 = new Button();
            button2.Name = "button2";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";

            // PanelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(clipOffPanel);
            contentPanel.AddChildLast(clipOnPanel);
            contentPanel.AddChildLast(Label_1);
            contentPanel.AddChildLast(Label_2);

            // clipOffPanel
            clipOffPanel.BackgroundColor = new UIColor(0f / 255f, 85f / 255f, 0f / 255f, 255f / 255f);
            clipOffPanel.Clip = true;
            clipOffPanel.AddChildLast(panel1);
            clipOffPanel.AddChildLast(image1);
            clipOffPanel.AddChildLast(button1);

            // panel1
            panel1.BackgroundColor = new UIColor(255f / 255f, 0f / 255f, 255f / 255f, 127f / 255f);
            panel1.Clip = true;

            // image1
            image1.Image = new ImageAsset("/Application/assets/photo02_s.png");
            image1.ImageScaleType = ImageScaleType.AspectOutside;

            // button1
            button1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            button1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // clipOnPanel
            clipOnPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 85f / 255f, 255f / 255f);
            clipOnPanel.Clip = true;
            clipOnPanel.AddChildLast(panel2);
            clipOnPanel.AddChildLast(image2);
            clipOnPanel.AddChildLast(button2);

            // panel2
            panel2.BackgroundColor = new UIColor(255f / 255f, 0f / 255f, 255f / 255f, 127f / 255f);
            panel2.Clip = true;

            // image2
            image2.Image = new ImageAsset("/Application/assets/photo02_s.png");
            image2.ImageScaleType = ImageScaleType.AspectOutside;

            // button2
            button2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            button2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.VerticalAlignment = VerticalAlignment.Bottom;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.VerticalAlignment = VerticalAlignment.Bottom;

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

                    contentPanel.SetPosition(0, 225);
                    contentPanel.SetSize(480, 629);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    clipOffPanel.SetPosition(193, 70);
                    clipOffPanel.SetSize(223, 241);
                    clipOffPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    clipOffPanel.Visible = true;

                    panel1.SetPosition(-38, 10);
                    panel1.SetSize(100, 100);
                    panel1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel1.Visible = true;

                    image1.SetPosition(-73, 20);
                    image1.SetSize(200, 200);
                    image1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image1.Visible = true;

                    button1.SetPosition(-87, 185);
                    button1.SetSize(214, 56);
                    button1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button1.Visible = true;

                    clipOnPanel.SetPosition(173, 396);
                    clipOnPanel.SetSize(262, 241);
                    clipOnPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    clipOnPanel.Visible = true;

                    panel2.SetPosition(-43, 0);
                    panel2.SetSize(100, 100);
                    panel2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    panel2.Visible = true;

                    image2.SetPosition(-93, 30);
                    image2.SetSize(200, 200);
                    image2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    image2.Visible = true;

                    button2.SetPosition(-82, 185);
                    button2.SetSize(214, 56);
                    button2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button2.Visible = true;

                    Label_1.SetPosition(42, 12);
                    Label_1.SetSize(113, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(34, 326);
                    Label_2.SetSize(128, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    clipOffPanel.SetPosition(159, 37);
                    clipOffPanel.SetSize(200, 320);
                    clipOffPanel.Anchors = Anchors.None;
                    clipOffPanel.Visible = true;

                    panel1.SetPosition(148, 36);
                    panel1.SetSize(100, 50);
                    panel1.Anchors = Anchors.None;
                    panel1.Visible = true;

                    image1.SetPosition(148, 135);
                    image1.SetSize(100, 50);
                    image1.Anchors = Anchors.None;
                    image1.Visible = true;

                    button1.SetPosition(148, 228);
                    button1.SetSize(100, 56);
                    button1.Anchors = Anchors.None;
                    button1.Visible = true;

                    clipOnPanel.SetPosition(494, 37);
                    clipOnPanel.SetSize(200, 320);
                    clipOnPanel.Anchors = Anchors.None;
                    clipOnPanel.Visible = true;

                    panel2.SetPosition(148, 36);
                    panel2.SetSize(100, 50);
                    panel2.Anchors = Anchors.None;
                    panel2.Visible = true;

                    image2.SetPosition(148, 135);
                    image2.SetSize(100, 50);
                    image2.Anchors = Anchors.None;
                    image2.Visible = true;

                    button2.SetPosition(148, 228);
                    button2.SetSize(100, 56);
                    button2.Anchors = Anchors.None;
                    button2.Visible = true;

                    Label_1.SetPosition(159, 1);
                    Label_1.SetSize(200, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(494, 1);
                    Label_2.SetSize(200, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            button1.Text = "Button";

            button2.Text = "Button";

            Label_1.Text = "No clip";

            Label_2.Text = "Clip";
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
