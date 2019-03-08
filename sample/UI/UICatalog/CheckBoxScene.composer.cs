// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class CheckBoxScene
    {
        Panel contentPanel;
        Panel checkPanel;
        CheckBox check1;
        CheckBox check2;
        Label checkabel1;
        Label checkLabel2;
        Panel radioPanel;
        CheckBox radio1;
        CheckBox radio2;
        CheckBox radio3;
        Label radioLabel1;
        Label radioLabel2;
        Label radioLabel3;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            checkPanel = new Panel();
            checkPanel.Name = "checkPanel";
            check1 = new CheckBox();
            check1.Name = "check1";
            check2 = new CheckBox();
            check2.Name = "check2";
            checkabel1 = new Label();
            checkabel1.Name = "checkabel1";
            checkLabel2 = new Label();
            checkLabel2.Name = "checkLabel2";
            radioPanel = new Panel();
            radioPanel.Name = "radioPanel";
            radio1 = new CheckBox();
            radio1.Name = "radio1";
            radio2 = new CheckBox();
            radio2.Name = "radio2";
            radio3 = new CheckBox();
            radio3.Name = "radio3";
            radioLabel1 = new Label();
            radioLabel1.Name = "radioLabel1";
            radioLabel2 = new Label();
            radioLabel2.Name = "radioLabel2";
            radioLabel3 = new Label();
            radioLabel3.Name = "radioLabel3";

            // CheckBoxScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(checkPanel);
            contentPanel.AddChildLast(radioPanel);

            // checkPanel
            checkPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 63f / 255f);
            checkPanel.Clip = true;
            checkPanel.AddChildLast(check1);
            checkPanel.AddChildLast(check2);
            checkPanel.AddChildLast(checkabel1);
            checkPanel.AddChildLast(checkLabel2);

            // checkabel1
            checkabel1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            checkabel1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            checkabel1.LineBreak = LineBreak.Character;

            // checkLabel2
            checkLabel2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            checkLabel2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            checkLabel2.LineBreak = LineBreak.Character;

            // radioPanel
            radioPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 63f / 255f);
            radioPanel.Clip = true;
            radioPanel.AddChildLast(radio1);
            radioPanel.AddChildLast(radio2);
            radioPanel.AddChildLast(radio3);
            radioPanel.AddChildLast(radioLabel1);
            radioPanel.AddChildLast(radioLabel2);
            radioPanel.AddChildLast(radioLabel3);

            // radio1
            radio1.Style = CheckBoxStyle.RadioButton;

            // radio2
            radio2.Style = CheckBoxStyle.RadioButton;

            // radio3
            radio3.Style = CheckBoxStyle.RadioButton;

            // radioLabel1
            radioLabel1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            radioLabel1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            radioLabel1.LineBreak = LineBreak.Character;

            // radioLabel2
            radioLabel2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            radioLabel2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            radioLabel2.LineBreak = LineBreak.Character;

            // radioLabel3
            radioLabel3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            radioLabel3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            radioLabel3.LineBreak = LineBreak.Character;

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

                    contentPanel.SetPosition(0, 212);
                    contentPanel.SetSize(480, 641);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    checkPanel.SetPosition(64, 50);
                    checkPanel.SetSize(380, 152);
                    checkPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    checkPanel.Visible = true;

                    check1.SetPosition(48, 12);
                    check1.SetSize(56, 56);
                    check1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    check1.Visible = true;

                    check2.SetPosition(48, 80);
                    check2.SetSize(56, 56);
                    check2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    check2.Visible = true;

                    checkabel1.SetPosition(149, 12);
                    checkabel1.SetSize(214, 36);
                    checkabel1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    checkabel1.Visible = true;

                    checkLabel2.SetPosition(149, 80);
                    checkLabel2.SetSize(214, 36);
                    checkLabel2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    checkLabel2.Visible = true;

                    radioPanel.SetPosition(59, 328);
                    radioPanel.SetSize(385, 279);
                    radioPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radioPanel.Visible = true;

                    radio1.SetPosition(71, 6);
                    radio1.SetSize(56, 56);
                    radio1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radio1.Visible = true;

                    radio2.SetPosition(53, 95);
                    radio2.SetSize(56, 56);
                    radio2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radio2.Visible = true;

                    radio3.SetPosition(64, 176);
                    radio3.SetSize(56, 56);
                    radio3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radio3.Visible = true;

                    radioLabel1.SetPosition(188, 16);
                    radioLabel1.SetSize(214, 36);
                    radioLabel1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radioLabel1.Visible = true;

                    radioLabel2.SetPosition(171, 105);
                    radioLabel2.SetSize(214, 36);
                    radioLabel2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radioLabel2.Visible = true;

                    radioLabel3.SetPosition(163, 176);
                    radioLabel3.SetSize(214, 36);
                    radioLabel3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    radioLabel3.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    checkPanel.SetPosition(93, 76);
                    checkPanel.SetSize(300, 157);
                    checkPanel.Anchors = Anchors.Height | Anchors.Width;
                    checkPanel.Visible = true;

                    check1.SetPosition(15, 15);
                    check1.SetSize(56, 56);
                    check1.Anchors = Anchors.Height | Anchors.Width;
                    check1.Visible = true;

                    check2.SetPosition(15, 86);
                    check2.SetSize(56, 56);
                    check2.Anchors = Anchors.Height | Anchors.Width;
                    check2.Visible = true;

                    checkabel1.SetPosition(78, 15);
                    checkabel1.SetSize(205, 56);
                    checkabel1.Anchors = Anchors.Height | Anchors.Width;
                    checkabel1.Visible = true;

                    checkLabel2.SetPosition(78, 86);
                    checkLabel2.SetSize(205, 56);
                    checkLabel2.Anchors = Anchors.Height | Anchors.Width;
                    checkLabel2.Visible = true;

                    radioPanel.SetPosition(459, 76);
                    radioPanel.SetSize(300, 229);
                    radioPanel.Anchors = Anchors.Height | Anchors.Width;
                    radioPanel.Visible = true;

                    radio1.SetPosition(15, 20);
                    radio1.SetSize(39, 39);
                    radio1.Anchors = Anchors.Height | Anchors.Width;
                    radio1.Visible = true;

                    radio2.SetPosition(15, 95);
                    radio2.SetSize(39, 39);
                    radio2.Anchors = Anchors.Height | Anchors.Width;
                    radio2.Visible = true;

                    radio3.SetPosition(15, 170);
                    radio3.SetSize(39, 39);
                    radio3.Anchors = Anchors.Height | Anchors.Width;
                    radio3.Visible = true;

                    radioLabel1.SetPosition(68, 20);
                    radioLabel1.SetSize(214, 39);
                    radioLabel1.Anchors = Anchors.Height | Anchors.Width;
                    radioLabel1.Visible = true;

                    radioLabel2.SetPosition(68, 95);
                    radioLabel2.SetSize(214, 39);
                    radioLabel2.Anchors = Anchors.Height | Anchors.Width;
                    radioLabel2.Visible = true;

                    radioLabel3.SetPosition(68, 170);
                    radioLabel3.SetSize(214, 39);
                    radioLabel3.Anchors = Anchors.Height | Anchors.Width;
                    radioLabel3.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            checkabel1.Text = "CheckButton 1";

            checkLabel2.Text = "CheckButton 2";

            radioLabel1.Text = "RadioButton 1";

            radioLabel2.Text = "RadioButton 2";

            radioLabel3.Text = "RadioButton 3";
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
