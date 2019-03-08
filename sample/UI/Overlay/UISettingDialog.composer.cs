// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace OverlaySample
{
    partial class UISettingDialog
    {
        Button Button_OK;
        ImageBox ImageBox_3;
        Label Label_title;
        Label Label_color;
        PopupList PopupList_color;
        Label Label_speed;
        Slider Slider_speed;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Button_OK = new Button();
            Button_OK.Name = "Button_OK";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            Label_title = new Label();
            Label_title.Name = "Label_title";
            Label_color = new Label();
            Label_color.Name = "Label_color";
            PopupList_color = new PopupList();
            PopupList_color.Name = "PopupList_color";
            Label_speed = new Label();
            Label_speed.Name = "Label_speed";
            Slider_speed = new Slider();
            Slider_speed.Name = "Slider_speed";

            // Button_OK
            Button_OK.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 127f / 255f);
            Button_OK.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/title_separator_9patch.png");
            ImageBox_3.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_3.NinePatchMargin = new NinePatchMargin(90, 0, 48, 0);

            // Label_title
            Label_title.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 127f / 255f);
            Label_title.Font = new UIFont(FontAlias.System, 28, FontStyle.Regular);
            Label_title.LineBreak = LineBreak.Word;
            Label_title.HorizontalAlignment = HorizontalAlignment.Center;

            // Label_color
            Label_color.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_color.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_color.LineBreak = LineBreak.Word;
            Label_color.HorizontalAlignment = HorizontalAlignment.Right;

            // PopupList_color
            PopupList_color.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            PopupList_color.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            PopupList_color.ListItemTextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            PopupList_color.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            PopupList_color.ListTitleTextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            PopupList_color.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Label_speed
            Label_speed.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_speed.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_speed.LineBreak = LineBreak.Word;
            Label_speed.HorizontalAlignment = HorizontalAlignment.Right;

            // Slider_speed
            Slider_speed.MinValue = 1;
            Slider_speed.MaxValue = 10;
            Slider_speed.Value = 5;
            Slider_speed.Step = 1;

            // UISettingDialog
            this.AddChildLast(Button_OK);
            this.AddChildLast(ImageBox_3);
            this.AddChildLast(Label_title);
            this.AddChildLast(Label_color);
            this.AddChildLast(PopupList_color);
            this.AddChildLast(Label_speed);
            this.AddChildLast(Slider_speed);
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
                    this.SetSize(400, 640);
                    this.Anchors = Anchors.None;

                    Button_OK.SetPosition(144, 310);
                    Button_OK.SetSize(214, 56);
                    Button_OK.Anchors = Anchors.None;
                    Button_OK.Visible = true;

                    ImageBox_3.SetPosition(328, 113);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    Label_title.SetPosition(312, 62);
                    Label_title.SetSize(214, 36);
                    Label_title.Anchors = Anchors.None;
                    Label_title.Visible = true;

                    Label_color.SetPosition(186, 146);
                    Label_color.SetSize(214, 36);
                    Label_color.Anchors = Anchors.None;
                    Label_color.Visible = true;

                    PopupList_color.SetPosition(391, 192);
                    PopupList_color.SetSize(360, 56);
                    PopupList_color.Anchors = Anchors.None;
                    PopupList_color.Visible = true;

                    Label_speed.SetPosition(88, 220);
                    Label_speed.SetSize(214, 36);
                    Label_speed.Anchors = Anchors.None;
                    Label_speed.Visible = true;

                    Slider_speed.SetPosition(16, 103);
                    Slider_speed.SetSize(362, 58);
                    Slider_speed.Anchors = Anchors.None;
                    Slider_speed.Visible = true;

                    break;

                default:
                    this.SetPosition(1, 1);
                    this.SetSize(640, 400);
                    this.Anchors = Anchors.None;

                    Button_OK.SetPosition(196, 314);
                    Button_OK.SetSize(250, 56);
                    Button_OK.Anchors = Anchors.None;
                    Button_OK.Visible = true;

                    ImageBox_3.SetPosition(21, 61);
                    ImageBox_3.SetSize(600, 3);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    Label_title.SetPosition(214, 19);
                    Label_title.SetSize(214, 36);
                    Label_title.Anchors = Anchors.None;
                    Label_title.Visible = true;

                    Label_color.SetPosition(21, 205);
                    Label_color.SetSize(224, 36);
                    Label_color.Anchors = Anchors.None;
                    Label_color.Visible = true;

                    PopupList_color.SetPosition(288, 195);
                    PopupList_color.SetSize(311, 56);
                    PopupList_color.Anchors = Anchors.None;
                    PopupList_color.Visible = true;

                    Label_speed.SetPosition(136, 113);
                    Label_speed.SetSize(109, 36);
                    Label_speed.Anchors = Anchors.None;
                    Label_speed.Visible = true;

                    Slider_speed.SetPosition(288, 102);
                    Slider_speed.SetSize(311, 58);
                    Slider_speed.Anchors = Anchors.None;
                    Slider_speed.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Button_OK.Text = "OK";

            Label_title.Text = "Settings";

            Label_color.Text = "Background Color";

            PopupList_color.ListItems.Clear();
            PopupList_color.ListItems.AddRange(new String[]
            {
                "Blue",
                "Red",
                "Green",
            });
            PopupList_color.SelectedIndex = 1;

            Label_speed.Text = "Speed";
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
