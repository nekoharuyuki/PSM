// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    partial class MyStoreItem
    {
        CheckBox CheckBox_1;
        Label Label_1;
        Label Label_2;
        Label Label_3;
        Label Label_4;
        Label Label_5;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            CheckBox_1 = new CheckBox();
            CheckBox_1.Name = "CheckBox_1";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Label_4 = new Label();
            Label_4.Name = "Label_4";
            Label_5 = new Label();
            Label_5.Name = "Label_5";

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;

            // Label_4
            Label_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_4.LineBreak = LineBreak.Character;
            Label_4.HorizontalAlignment = HorizontalAlignment.Right;

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_5.LineBreak = LineBreak.Character;
            Label_5.HorizontalAlignment = HorizontalAlignment.Right;

            // MyStoreItem
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 12f / 255f);
            this.AddChildLast(CheckBox_1);
            this.AddChildLast(Label_1);
            this.AddChildLast(Label_2);
            this.AddChildLast(Label_3);
            this.AddChildLast(Label_4);
            this.AddChildLast(Label_5);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(96, 770);
                    this.Anchors = Anchors.None;

                    CheckBox_1.SetPosition(17, 18);
                    CheckBox_1.SetSize(56, 56);
                    CheckBox_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    CheckBox_1.Visible = true;

                    Label_1.SetPosition(17, 18);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(100, 80);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    Label_3.SetPosition(277, 78);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_3.Visible = true;

                    Label_4.SetPosition(500, 78);
                    Label_4.SetSize(214, 36);
                    Label_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_4.Visible = true;

                    Label_5.SetPosition(500, 78);
                    Label_5.SetSize(214, 36);
                    Label_5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_5.Visible = true;

                    break;

                default:
                    this.SetSize(780, 88);
                    this.Anchors = Anchors.None;

                    CheckBox_1.SetPosition(12, 18);
                    CheckBox_1.SetSize(56, 56);
                    CheckBox_1.Anchors = Anchors.Height | Anchors.Left | Anchors.Width;
                    CheckBox_1.Visible = true;

                    Label_1.SetPosition(68, 28);
                    Label_1.SetSize(44, 36);
                    Label_1.Anchors = Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(114, 28);
                    Label_2.SetSize(152, 36);
                    Label_2.Anchors = Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    Label_3.SetPosition(268, 14);
                    Label_3.SetSize(203, 64);
                    Label_3.Anchors = Anchors.Height | Anchors.Left | Anchors.Right;
                    Label_3.Visible = true;

                    Label_4.SetPosition(473, 28);
                    Label_4.SetSize(164, 36);
                    Label_4.Anchors = Anchors.Height | Anchors.Right | Anchors.Width;
                    Label_4.Visible = true;

                    Label_5.SetPosition(639, 28);
                    Label_5.SetSize(101, 36);
                    Label_5.Anchors = Anchors.Height | Anchors.Right | Anchors.Width;
                    Label_5.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public void StartDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public static ListPanelItem Creator()
        {
            return new MyStoreItem();
        }

    }
}
