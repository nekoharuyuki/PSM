// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace OverlaySample
{
    partial class UIOverlayScene
    {
        Button Button_preference;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Button_preference = new Button();
            Button_preference.Name = "Button_preference";

            // Button_preference
            Button_preference.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_preference.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_preference.Style = ButtonStyle.Custom;
            Button_preference.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/btn_preference.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/btn_preference_press.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/btn_preference.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // UIOverlayScene
            this.RootWidget.AddChildLast(Button_preference);

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

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    Button_preference.SetPosition(784, 421);
                    Button_preference.SetSize(42, 34);
                    Button_preference.Anchors = Anchors.None;
                    Button_preference.Visible = true;

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
