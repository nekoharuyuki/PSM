// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    partial class UIOverlayScene
    {
        Button Button_preference;
        ImageBox ImageBox_1;
        ImageBox ImageBox_2;
        ImageBox ImageBox_3;
        ImageBox ImageBox_4;
        Button buttonSound;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Button_preference = new Button();
            Button_preference.Name = "Button_preference";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            ImageBox_4 = new ImageBox();
            ImageBox_4.Name = "ImageBox_4";
            buttonSound = new Button();
            buttonSound.Name = "buttonSound";

            // Button_preference
            Button_preference.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_preference.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_preference.Style = ButtonStyle.Custom;
            Button_preference.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/btn_preference.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/btn_preference_2.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/btn_preference_3.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/score.png");

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/hiscore.png");

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/number.png");

            // ImageBox_4
            ImageBox_4.Image = new ImageAsset("/Application/assets/number.png");

            // buttonSound
            buttonSound.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            buttonSound.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            buttonSound.Style = ButtonStyle.Custom;
            buttonSound.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/btn_volume.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/btn_volume_2.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/btn_volume_3.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // UIOverlayScene
            this.RootWidget.AddChildLast(Button_preference);
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(ImageBox_2);
            this.RootWidget.AddChildLast(ImageBox_3);
            this.RootWidget.AddChildLast(ImageBox_4);
            this.RootWidget.AddChildLast(buttonSound);
            this.Showing += new EventHandler(onShowing);
            this.Shown += new EventHandler(onShown);

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

                    Button_preference.SetPosition(653, 402);
                    Button_preference.SetSize(214, 56);
                    Button_preference.Anchors = Anchors.None;
                    Button_preference.Visible = true;

                    ImageBox_1.SetPosition(276, 0);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(245, 88);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    ImageBox_3.SetPosition(283, 183);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(283, 183);
                    ImageBox_4.SetSize(200, 200);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = true;

                    buttonSound.SetPosition(13, 289);
                    buttonSound.SetSize(214, 56);
                    buttonSound.Anchors = Anchors.None;
                    buttonSound.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    Button_preference.SetPosition(24, 400);
                    Button_preference.SetSize(80, 56);
                    Button_preference.Anchors = Anchors.None;
                    Button_preference.Visible = true;

                    ImageBox_1.SetPosition(448, 24);
                    ImageBox_1.SetSize(112, 40);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(416, 64);
                    ImageBox_2.SetSize(144, 40);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    ImageBox_3.SetPosition(576, 24);
                    ImageBox_3.SetSize(256, 40);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    ImageBox_4.SetPosition(576, 64);
                    ImageBox_4.SetSize(256, 40);
                    ImageBox_4.Anchors = Anchors.None;
                    ImageBox_4.Visible = true;

                    buttonSound.SetPosition(760, 400);
                    buttonSound.SetSize(80, 56);
                    buttonSound.Anchors = Anchors.None;
                    buttonSound.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    Button_preference.Visible = false;
                    buttonSound.Visible = false;
                    break;

                default:
                    Button_preference.Visible = false;
                    buttonSound.Visible = false;
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    new FadeInEffect()
                    {
                        Widget = Button_preference,
                    }.Start();
                    new FadeInEffect()
                    {
                        Widget = buttonSound,
                    }.Start();
                    break;

                default:
                    new FadeInEffect()
                    {
                        Widget = Button_preference,
                    }.Start();
                    new FadeInEffect()
                    {
                        Widget = buttonSound,
                    }.Start();
                    break;
            }
        }

    }
}
