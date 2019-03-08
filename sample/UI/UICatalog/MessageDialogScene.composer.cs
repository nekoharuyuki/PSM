// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class MessageDialogScene
    {
        Panel contentPanel;
        Button button1;
        Button button2;
        Label label1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            button1 = new Button();
            button1.Name = "button1";
            button2 = new Button();
            button2.Name = "button2";
            label1 = new Label();
            label1.Name = "label1";

            // MessageDialogScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(button1);
            contentPanel.AddChildLast(button2);
            contentPanel.AddChildLast(label1);

            // button1
            button1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // button2
            button2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // label1
            label1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label1.LineBreak = LineBreak.Character;
            label1.HorizontalAlignment = HorizontalAlignment.Center;

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

                    contentPanel.SetPosition(0, 210);
                    contentPanel.SetSize(480, 643);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    button1.SetPosition(13, 132);
                    button1.SetSize(453, 56);
                    button1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button1.Visible = true;

                    button2.SetPosition(13, 219);
                    button2.SetSize(453, 56);
                    button2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button2.Visible = true;

                    label1.SetPosition(117, 350);
                    label1.SetSize(214, 36);
                    label1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label1.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    button1.SetPosition(202, 51);
                    button1.SetSize(450, 56);
                    button1.Anchors = Anchors.Height;
                    button1.Visible = true;

                    button2.SetPosition(202, 156);
                    button2.SetSize(450, 56);
                    button2.Anchors = Anchors.Height;
                    button2.Visible = true;

                    label1.SetPosition(202, 256);
                    label1.SetSize(450, 36);
                    label1.Anchors = Anchors.Height;
                    label1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            button1.Text = "Show MessageDialog (OK)";

            button2.Text = "Show MessageDialog (OK/Cancel)";
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
