// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SpinBoxScene
    {
        Panel contentPanel;
        DatePicker SpinBox_1;
        TimePicker SpinBox_2;
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
            SpinBox_1 = new DatePicker();
            SpinBox_1.Name = "SpinBox_1";
            SpinBox_2 = new TimePicker();
            SpinBox_2.Name = "SpinBox_2";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";

            // SpinBoxScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(SpinBox_1);
            contentPanel.AddChildLast(SpinBox_2);
            contentPanel.AddChildLast(Label_1);
            contentPanel.AddChildLast(Label_2);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            Label_1.VerticalAlignment = VerticalAlignment.Bottom;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;
            Label_2.HorizontalAlignment = HorizontalAlignment.Center;
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

                    contentPanel.SetPosition(0, 207);
                    contentPanel.SetSize(486, 646);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    SpinBox_1.SetPosition(73, 57);
                    SpinBox_1.SetSize(340, 228);
                    SpinBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    SpinBox_1.Visible = true;

                    SpinBox_2.SetPosition(105, 389);
                    SpinBox_2.SetSize(276, 228);
                    SpinBox_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    SpinBox_2.Visible = true;

                    Label_1.SetPosition(136, 21);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(136, 340);
                    Label_2.SetSize(214, 36);
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

                    SpinBox_1.SetPosition(90, 90);
                    SpinBox_1.SetSize(340, 228);
                    SpinBox_1.Anchors = Anchors.Height | Anchors.Width;
                    SpinBox_1.Visible = true;

                    SpinBox_2.SetPosition(478, 90);
                    SpinBox_2.SetSize(276, 228);
                    SpinBox_2.Anchors = Anchors.Height | Anchors.Width;
                    SpinBox_2.Visible = true;

                    Label_1.SetPosition(90, 39);
                    Label_1.SetSize(340, 36);
                    Label_1.Anchors = Anchors.Height | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(478, 39);
                    Label_2.SetSize(276, 36);
                    Label_2.Anchors = Anchors.Height | Anchors.Width;
                    Label_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Date";

            Label_2.Text = "Time";
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
