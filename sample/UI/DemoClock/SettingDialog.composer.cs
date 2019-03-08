// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace DemoClock
{
    partial class SettingDialog
    {
        Label Label_1;
        CheckBox CheckBox_1;
        Label Label_2;
        CheckBox CheckBox_2;
        Label Label_3;
        Button Button_1;
        Button Button_2;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            CheckBox_1 = new CheckBox();
            CheckBox_1.Name = "CheckBox_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            CheckBox_2 = new CheckBox();
            CheckBox_2.Name = "CheckBox_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";

            // Label_1
            Label_1.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Word;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            Label_1.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 127f / 255f),
                HorizontalOffset = 1f,
                VerticalOffset = 1f,
            };

            // CheckBox_1
            CheckBox_1.Style = CheckBoxStyle.RadioButton;
            CheckBox_1.Checked = true;

            // Label_2
            Label_2.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Word;
            Label_2.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 127f / 255f),
                HorizontalOffset = 1f,
                VerticalOffset = 1f,
            };

            // CheckBox_2
            CheckBox_2.Style = CheckBoxStyle.RadioButton;

            // Label_3
            Label_3.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Word;
            Label_3.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 127f / 255f),
                HorizontalOffset = 1f,
                VerticalOffset = 1f,
            };

            // Button_1
            Button_1.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 1f,
                VerticalOffset = 1f,
            };

            // Button_2
            Button_2.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_2.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 1f,
                VerticalOffset = 1f,
            };

            // SettingDialog
            this.AddChildLast(Label_1);
            this.AddChildLast(CheckBox_1);
            this.AddChildLast(Label_2);
            this.AddChildLast(CheckBox_2);
            this.AddChildLast(Label_3);
            this.AddChildLast(Button_1);
            this.AddChildLast(Button_2);
            this.HideEffect = new TiltDropEffect();

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetPosition(1, 1);
                    this.SetSize(544, 960);
                    this.Anchors = Anchors.Height | Anchors.Width;

                    Label_1.SetPosition(278, 128);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = false;

                    CheckBox_1.SetPosition(376, 210);
                    CheckBox_1.SetSize(56, 56);
                    CheckBox_1.Anchors = Anchors.Height | Anchors.Width;
                    CheckBox_1.Visible = false;

                    Label_2.SetPosition(331, 141);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = false;

                    CheckBox_2.SetPosition(355, 236);
                    CheckBox_2.SetSize(56, 56);
                    CheckBox_2.Anchors = Anchors.Height | Anchors.Width;
                    CheckBox_2.Visible = false;

                    Label_3.SetPosition(330, 233);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = false;

                    Button_1.SetPosition(442, 323);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = false;

                    Button_2.SetPosition(461, 355);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = false;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(300, 300);
                    this.Anchors = Anchors.Height | Anchors.Width;

                    Label_1.SetPosition(40, 18);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Width;
                    Label_1.Visible = true;

                    CheckBox_1.SetPosition(40, 92);
                    CheckBox_1.SetSize(39, 39);
                    CheckBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    CheckBox_1.Visible = true;

                    Label_2.SetPosition(87, 92);
                    Label_2.SetSize(173, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    CheckBox_2.SetPosition(40, 145);
                    CheckBox_2.SetSize(39, 39);
                    CheckBox_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    CheckBox_2.Visible = true;

                    Label_3.SetPosition(87, 145);
                    Label_3.SetSize(173, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_3.Visible = true;

                    Button_1.SetPosition(40, 222);
                    Button_1.SetSize(100, 56);
                    Button_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_1.Visible = true;

                    Button_2.SetPosition(154, 222);
                    Button_2.SetSize(100, 56);
                    Button_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "MENU";

            Label_2.Text = "Analog Clock";

            Label_3.Text = "Flip Clock";

            Button_1.Text = "Apply";

            Button_2.Text = "Cancel";
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
