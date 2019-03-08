// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class CompleteDialog
    {
        Label Label_1;
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
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";

            // Label_1
            Label_1.TextColor = new UIColor(224f / 255f, 255f / 255f, 240f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            Label_1.VerticalAlignment = VerticalAlignment.Top;

            // Button_1
            Button_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.Style = ButtonStyle.Custom;
            Button_1.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/save_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/save_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/save_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // Button_2
            Button_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_2.Style = ButtonStyle.Custom;
            Button_2.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/cancel_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/cancel_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/cancel_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // CompleteDialog
            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = new ImageAsset("/Application/assets/menu_window_2.png");
            this.CustomBackgroundNinePatchMargin = new NinePatchMargin(16, 16, 16, 16);
            this.CustomBackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            this.AddChildLast(Label_1);
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
                    this.SetPosition(0, 0);
                    this.SetSize(300, 400);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(65, 83);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Button_1.SetPosition(52, 203);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(186, 218);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(500, 244);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(41, 37);
                    Label_1.SetSize(414, 55);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Button_1.SetPosition(48, 160);
                    Button_1.SetSize(144, 40);
                    Button_1.Anchors = Anchors.None;
                    Button_1.Visible = true;

                    Button_2.SetPosition(274, 160);
                    Button_2.SetSize(176, 40);
                    Button_2.Anchors = Anchors.None;
                    Button_2.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Do you want to save?";
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
