// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class LabelScene
    {
        Panel contentPanel;
        Label labelTextE;
        Panel Panel_1;
        Label labelTrim;
        PopupList popupTrim;
        Panel Panel_2;
        Label labelLineBreak;
        PopupList popupLineBreak;
        Panel Panel_3;
        Label labelHorizontal;
        PopupList popupHorizontal;
        Panel Panel_4;
        Label labelVertical;
        PopupList popupVertical;
        Panel Panel_5;
        CheckBox checkBoxShadow;
        Label labelShadow;
        Panel Panel_6;
        Label labelWidth;
        Slider sliderH;
        Panel Panel_7;
        Label labelHeight;
        Slider sliderV;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            labelTextE = new Label();
            labelTextE.Name = "labelTextE";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";
            labelTrim = new Label();
            labelTrim.Name = "labelTrim";
            popupTrim = new PopupList();
            popupTrim.Name = "popupTrim";
            Panel_2 = new Panel();
            Panel_2.Name = "Panel_2";
            labelLineBreak = new Label();
            labelLineBreak.Name = "labelLineBreak";
            popupLineBreak = new PopupList();
            popupLineBreak.Name = "popupLineBreak";
            Panel_3 = new Panel();
            Panel_3.Name = "Panel_3";
            labelHorizontal = new Label();
            labelHorizontal.Name = "labelHorizontal";
            popupHorizontal = new PopupList();
            popupHorizontal.Name = "popupHorizontal";
            Panel_4 = new Panel();
            Panel_4.Name = "Panel_4";
            labelVertical = new Label();
            labelVertical.Name = "labelVertical";
            popupVertical = new PopupList();
            popupVertical.Name = "popupVertical";
            Panel_5 = new Panel();
            Panel_5.Name = "Panel_5";
            checkBoxShadow = new CheckBox();
            checkBoxShadow.Name = "checkBoxShadow";
            labelShadow = new Label();
            labelShadow.Name = "labelShadow";
            Panel_6 = new Panel();
            Panel_6.Name = "Panel_6";
            labelWidth = new Label();
            labelWidth.Name = "labelWidth";
            sliderH = new Slider();
            sliderH.Name = "sliderH";
            Panel_7 = new Panel();
            Panel_7.Name = "Panel_7";
            labelHeight = new Label();
            labelHeight.Name = "labelHeight";
            sliderV = new Slider();
            sliderV.Name = "sliderV";

            // LabelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(labelTextE);
            contentPanel.AddChildLast(Panel_1);
            contentPanel.AddChildLast(Panel_2);
            contentPanel.AddChildLast(Panel_3);
            contentPanel.AddChildLast(Panel_4);
            contentPanel.AddChildLast(Panel_5);
            contentPanel.AddChildLast(Panel_6);
            contentPanel.AddChildLast(Panel_7);

            // labelTextE
            labelTextE.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelTextE.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
            labelTextE.LineBreak = LineBreak.Character;

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(labelTrim);
            Panel_1.AddChildLast(popupTrim);

            // labelTrim
            labelTrim.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelTrim.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelTrim.LineBreak = LineBreak.Character;
            labelTrim.VerticalAlignment = VerticalAlignment.Bottom;

            // popupTrim
            popupTrim.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupTrim.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupTrim.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupTrim.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupTrim.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupTrim.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Panel_2
            Panel_2.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_2.Clip = true;
            Panel_2.AddChildLast(labelLineBreak);
            Panel_2.AddChildLast(popupLineBreak);

            // labelLineBreak
            labelLineBreak.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelLineBreak.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelLineBreak.LineBreak = LineBreak.Character;
            labelLineBreak.VerticalAlignment = VerticalAlignment.Bottom;

            // popupLineBreak
            popupLineBreak.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupLineBreak.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupLineBreak.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupLineBreak.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupLineBreak.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupLineBreak.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Panel_3
            Panel_3.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_3.Clip = true;
            Panel_3.AddChildLast(labelHorizontal);
            Panel_3.AddChildLast(popupHorizontal);

            // labelHorizontal
            labelHorizontal.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelHorizontal.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelHorizontal.LineBreak = LineBreak.Character;
            labelHorizontal.VerticalAlignment = VerticalAlignment.Bottom;

            // popupHorizontal
            popupHorizontal.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupHorizontal.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupHorizontal.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupHorizontal.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupHorizontal.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupHorizontal.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Panel_4
            Panel_4.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_4.Clip = true;
            Panel_4.AddChildLast(labelVertical);
            Panel_4.AddChildLast(popupVertical);

            // labelVertical
            labelVertical.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelVertical.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelVertical.LineBreak = LineBreak.Character;
            labelVertical.VerticalAlignment = VerticalAlignment.Bottom;

            // popupVertical
            popupVertical.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupVertical.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupVertical.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupVertical.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupVertical.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupVertical.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Panel_5
            Panel_5.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_5.Clip = true;
            Panel_5.AddChildLast(checkBoxShadow);
            Panel_5.AddChildLast(labelShadow);

            // labelShadow
            labelShadow.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelShadow.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelShadow.LineBreak = LineBreak.Character;

            // Panel_6
            Panel_6.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_6.Clip = true;
            Panel_6.AddChildLast(labelWidth);
            Panel_6.AddChildLast(sliderH);

            // labelWidth
            labelWidth.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelWidth.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
            labelWidth.LineBreak = LineBreak.Character;
            labelWidth.VerticalAlignment = VerticalAlignment.Bottom;

            // sliderH
            sliderH.MinValue = 25;
            sliderH.MaxValue = 100;
            sliderH.Value = 0;
            sliderH.Step = 1;

            // Panel_7
            Panel_7.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_7.Clip = true;
            Panel_7.AddChildLast(labelHeight);
            Panel_7.AddChildLast(sliderV);

            // labelHeight
            labelHeight.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelHeight.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
            labelHeight.LineBreak = LineBreak.Character;
            labelHeight.VerticalAlignment = VerticalAlignment.Bottom;

            // sliderV
            sliderV.MinValue = 25;
            sliderV.MaxValue = 100;
            sliderV.Value = 0;
            sliderV.Step = 1;

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
                    contentPanel.SetSize(480, 646);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    labelTextE.SetPosition(0, 0);
                    labelTextE.SetSize(214, 36);
                    labelTextE.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelTextE.Visible = true;

                    Panel_1.SetPosition(-6, 177);
                    Panel_1.SetSize(480, 82);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    labelTrim.SetPosition(7, 24);
                    labelTrim.SetSize(189, 36);
                    labelTrim.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelTrim.Visible = true;

                    popupTrim.SetPosition(265, 14);
                    popupTrim.SetSize(197, 56);
                    popupTrim.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupTrim.Visible = true;

                    Panel_2.SetPosition(23, 259);
                    Panel_2.SetSize(429, 67);
                    Panel_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_2.Visible = true;

                    labelLineBreak.SetPosition(8, 11);
                    labelLineBreak.SetSize(130, 36);
                    labelLineBreak.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelLineBreak.Visible = true;

                    popupLineBreak.SetPosition(165, 1);
                    popupLineBreak.SetSize(253, 56);
                    popupLineBreak.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupLineBreak.Visible = true;

                    Panel_3.SetPosition(7, 345);
                    Panel_3.SetSize(477, 73);
                    Panel_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_3.Visible = true;

                    labelHorizontal.SetPosition(16, 8);
                    labelHorizontal.SetSize(139, 36);
                    labelHorizontal.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelHorizontal.Visible = true;

                    popupHorizontal.SetPosition(191, 8);
                    popupHorizontal.SetSize(264, 57);
                    popupHorizontal.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupHorizontal.Visible = true;

                    Panel_4.SetPosition(11, 445);
                    Panel_4.SetSize(469, 66);
                    Panel_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_4.Visible = true;

                    labelVertical.SetPosition(8, 0);
                    labelVertical.SetSize(106, 40);
                    labelVertical.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelVertical.Visible = true;

                    popupVertical.SetPosition(200, 0);
                    popupVertical.SetSize(203, 56);
                    popupVertical.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupVertical.Visible = true;

                    Panel_5.SetPosition(-258, 285);
                    Panel_5.SetSize(100, 100);
                    Panel_5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_5.Visible = true;

                    checkBoxShadow.SetPosition(-38, -134);
                    checkBoxShadow.SetSize(56, 56);
                    checkBoxShadow.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    checkBoxShadow.Visible = true;

                    labelShadow.SetPosition(-65, -102);
                    labelShadow.SetSize(214, 36);
                    labelShadow.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelShadow.Visible = true;

                    Panel_6.SetPosition(12, 522);
                    Panel_6.SetSize(442, 58);
                    Panel_6.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_6.Visible = true;

                    labelWidth.SetPosition(13, 11);
                    labelWidth.SetSize(128, 36);
                    labelWidth.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelWidth.Visible = true;

                    sliderH.SetPosition(205, 0);
                    sliderH.SetSize(213, 58);
                    sliderH.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    sliderH.Visible = true;

                    Panel_7.SetPosition(3, 580);
                    Panel_7.SetSize(477, 66);
                    Panel_7.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_7.Visible = true;

                    labelHeight.SetPosition(16, 16);
                    labelHeight.SetSize(91, 36);
                    labelHeight.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelHeight.Visible = true;

                    sliderV.SetPosition(181, 8);
                    sliderV.SetSize(279, 58);
                    sliderV.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    sliderV.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    labelTextE.SetPosition(49, 8);
                    labelTextE.SetSize(369, 112);
                    labelTextE.Anchors = Anchors.None;
                    labelTextE.Visible = true;

                    Panel_1.SetPosition(443, 8);
                    Panel_1.SetSize(359, 90);
                    Panel_1.Anchors = Anchors.None;
                    Panel_1.Visible = true;

                    labelTrim.SetPosition(0, 0);
                    labelTrim.SetSize(355, 34);
                    labelTrim.Anchors = Anchors.Height;
                    labelTrim.Visible = true;

                    popupTrim.SetPosition(0, 34);
                    popupTrim.SetSize(355, 56);
                    popupTrim.Anchors = Anchors.Height;
                    popupTrim.Visible = true;

                    Panel_2.SetPosition(445, 98);
                    Panel_2.SetSize(359, 90);
                    Panel_2.Anchors = Anchors.None;
                    Panel_2.Visible = true;

                    labelLineBreak.SetPosition(0, 0);
                    labelLineBreak.SetSize(355, 34);
                    labelLineBreak.Anchors = Anchors.Height;
                    labelLineBreak.Visible = true;

                    popupLineBreak.SetPosition(0, 34);
                    popupLineBreak.SetSize(355, 56);
                    popupLineBreak.Anchors = Anchors.Height;
                    popupLineBreak.Visible = true;

                    Panel_3.SetPosition(444, 188);
                    Panel_3.SetSize(359, 90);
                    Panel_3.Anchors = Anchors.None;
                    Panel_3.Visible = true;

                    labelHorizontal.SetPosition(0, 0);
                    labelHorizontal.SetSize(355, 34);
                    labelHorizontal.Anchors = Anchors.Height;
                    labelHorizontal.Visible = true;

                    popupHorizontal.SetPosition(0, 34);
                    popupHorizontal.SetSize(355, 56);
                    popupHorizontal.Anchors = Anchors.Height;
                    popupHorizontal.Visible = true;

                    Panel_4.SetPosition(443, 280);
                    Panel_4.SetSize(359, 90);
                    Panel_4.Anchors = Anchors.None;
                    Panel_4.Visible = true;

                    labelVertical.SetPosition(0, 0);
                    labelVertical.SetSize(355, 34);
                    labelVertical.Anchors = Anchors.Height;
                    labelVertical.Visible = true;

                    popupVertical.SetPosition(0, 34);
                    popupVertical.SetSize(355, 56);
                    popupVertical.Anchors = Anchors.Height;
                    popupVertical.Visible = true;

                    Panel_5.SetPosition(49, 120);
                    Panel_5.SetSize(369, 66);
                    Panel_5.Anchors = Anchors.None;
                    Panel_5.Visible = true;

                    checkBoxShadow.SetPosition(0, 5);
                    checkBoxShadow.SetSize(56, 56);
                    checkBoxShadow.Anchors = Anchors.Height | Anchors.Left;
                    checkBoxShadow.Visible = true;

                    labelShadow.SetPosition(67, 5);
                    labelShadow.SetSize(291, 56);
                    labelShadow.Anchors = Anchors.Height | Anchors.Left;
                    labelShadow.Visible = true;

                    Panel_6.SetPosition(49, 186);
                    Panel_6.SetSize(369, 92);
                    Panel_6.Anchors = Anchors.None;
                    Panel_6.Visible = true;

                    labelWidth.SetPosition(0, 2);
                    labelWidth.SetSize(360, 34);
                    labelWidth.Anchors = Anchors.Height;
                    labelWidth.Visible = true;

                    sliderH.SetPosition(0, 36);
                    sliderH.SetSize(360, 58);
                    sliderH.Anchors = Anchors.Height;
                    sliderH.Visible = true;

                    Panel_7.SetPosition(49, 278);
                    Panel_7.SetSize(369, 92);
                    Panel_7.Anchors = Anchors.None;
                    Panel_7.Visible = true;

                    labelHeight.SetPosition(0, 2);
                    labelHeight.SetSize(360, 34);
                    labelHeight.Anchors = Anchors.Height;
                    labelHeight.Visible = true;

                    sliderV.SetPosition(0, 36);
                    sliderV.SetSize(360, 58);
                    sliderV.Anchors = Anchors.Height;
                    sliderV.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            labelTextE.Text = "The quick brown fox jumps over the lazy dog. #0123456789 : The quick brown fox jumps over the lazy dog.";

            labelTrim.Text = "TextTrimming";

            popupTrim.ListTitle = "TextTrimming";
            popupTrim.ListItems.Clear();
            popupTrim.ListItems.AddRange(new String[]
            {
                "None",
                "Character",
                "Word",
                "EllipsisCharacter",
                "EllipsisWord",
            });
            popupTrim.SelectedIndex = 3;

            labelLineBreak.Text = "LineBreak";

            popupLineBreak.ListTitle = "LineBreak";
            popupLineBreak.ListItems.Clear();
            popupLineBreak.ListItems.AddRange(new String[]
            {
                "Character",
                "Word",
                "Hyphenation",
                "AtCode",
            });
            popupLineBreak.SelectedIndex = 0;

            labelHorizontal.Text = "Horizontal";

            popupHorizontal.ListTitle = "Horizontal";
            popupHorizontal.ListItems.Clear();
            popupHorizontal.ListItems.AddRange(new String[]
            {
                "Left",
                "Center",
                "Right",
            });
            popupHorizontal.SelectedIndex = 0;

            labelVertical.Text = "Vertical";

            popupVertical.ListTitle = "Vertical";
            popupVertical.ListItems.Clear();
            popupVertical.ListItems.AddRange(new String[]
            {
                "Top",
                "Middle",
                "Bottom",
            });
            popupVertical.SelectedIndex = 1;

            labelShadow.Text = "Shadow";

            labelWidth.Text = "Width";

            labelHeight.Text = "Height";
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
