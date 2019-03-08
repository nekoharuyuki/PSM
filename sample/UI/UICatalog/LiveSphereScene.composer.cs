// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class LiveSphereScene
    {
        Panel contentPanel;
        Label Label_1;
        Label Label_2;
        LiveSphere toggleSphere1;
        Label Label_3;
        LiveSphere toggleSphere2;
        LiveSphere toggleSphere3;
        LiveSphere earthSphere;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            toggleSphere1 = new LiveSphere();
            toggleSphere1.Name = "toggleSphere1";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            toggleSphere2 = new LiveSphere();
            toggleSphere2.Name = "toggleSphere2";
            toggleSphere3 = new LiveSphere();
            toggleSphere3.Name = "toggleSphere3";
            earthSphere = new LiveSphere();
            earthSphere.Name = "earthSphere";

            // LiveSphereScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(Label_1);
            contentPanel.AddChildLast(Label_2);
            contentPanel.AddChildLast(toggleSphere1);
            contentPanel.AddChildLast(Label_3);
            contentPanel.AddChildLast(toggleSphere2);
            contentPanel.AddChildLast(toggleSphere3);
            contentPanel.AddChildLast(earthSphere);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Right;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Right;

            // toggleSphere1
            toggleSphere1.Image = new ImageAsset("/Application/assets/on_off_sphere.png");
            toggleSphere1.TurnCount = -1;
            toggleSphere1.TouchEnabled = true;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;
            Label_3.HorizontalAlignment = HorizontalAlignment.Right;

            // toggleSphere2
            toggleSphere2.Image = new ImageAsset("/Application/assets/on_off_sphere.png");
            toggleSphere2.TurnCount = -1;
            toggleSphere2.TouchEnabled = true;

            // toggleSphere3
            toggleSphere3.Image = new ImageAsset("/Application/assets/on_off_sphere.png");
            toggleSphere3.TurnCount = -1;
            toggleSphere3.TouchEnabled = true;

            // earthSphere
            earthSphere.Image = new ImageAsset("/Application/assets/map.png");
            earthSphere.TurnCount = -1;
            earthSphere.TouchEnabled = true;

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

                    contentPanel.SetPosition(0, 258);
                    contentPanel.SetSize(480, 595);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    Label_1.SetPosition(59, 119);
                    Label_1.SetSize(94, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(198, 119);
                    Label_2.SetSize(94, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    toggleSphere1.SetPosition(60, 22);
                    toggleSphere1.SetSize(94, 95);
                    toggleSphere1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    toggleSphere1.Visible = true;

                    Label_3.SetPosition(327, 119);
                    Label_3.SetSize(94, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_3.Visible = true;

                    toggleSphere2.SetPosition(198, 22);
                    toggleSphere2.SetSize(94, 97);
                    toggleSphere2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    toggleSphere2.Visible = true;

                    toggleSphere3.SetPosition(327, 22);
                    toggleSphere3.SetSize(94, 97);
                    toggleSphere3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    toggleSphere3.Visible = true;

                    earthSphere.SetPosition(94, 274);
                    earthSphere.SetSize(292, 283);
                    earthSphere.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    earthSphere.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    Label_1.SetPosition(102, 95);
                    Label_1.SetSize(108, 50);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(102, 160);
                    Label_2.SetSize(108, 50);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    toggleSphere1.SetPosition(226, 95);
                    toggleSphere1.SetSize(50, 50);
                    toggleSphere1.Anchors = Anchors.None;
                    toggleSphere1.Visible = true;

                    Label_3.SetPosition(102, 225);
                    Label_3.SetSize(108, 50);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    toggleSphere2.SetPosition(226, 160);
                    toggleSphere2.SetSize(50, 50);
                    toggleSphere2.Anchors = Anchors.None;
                    toggleSphere2.Visible = true;

                    toggleSphere3.SetPosition(226, 225);
                    toggleSphere3.SetSize(50, 50);
                    toggleSphere3.Anchors = Anchors.None;
                    toggleSphere3.Visible = true;

                    earthSphere.SetPosition(412, 16);
                    earthSphere.SetSize(340, 340);
                    earthSphere.Anchors = Anchors.None;
                    earthSphere.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Item 1";

            Label_2.Text = "Item 2";

            Label_3.Text = "Item 3";
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
