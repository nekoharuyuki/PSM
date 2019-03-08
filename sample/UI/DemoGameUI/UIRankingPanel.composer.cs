// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    partial class UIRankingPanel
    {
        ImageBox ImageBox_1;
        Button Button_return;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Button_return = new Button();
            Button_return.Name = "Button_return";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/menu_ranking.png");

            // Button_return
            Button_return.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Button_return.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_return.Style = ButtonStyle.Custom;
            Button_return.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/arrow.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/arrow_2.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/arrow_3.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // UIRankingPanel
            this.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(Button_return);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(123, 169);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Button_return.SetPosition(500, 381);
                    Button_return.SetSize(214, 56);
                    Button_return.Anchors = Anchors.None;
                    Button_return.Visible = true;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(120, 144);
                    ImageBox_1.SetSize(591, 168);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Button_return.SetPosition(760, 400);
                    Button_return.SetSize(80, 56);
                    Button_return.Anchors = Anchors.None;
                    Button_return.Visible = true;

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

    }
}
