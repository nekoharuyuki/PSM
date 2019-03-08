// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    partial class UIOverlayScene
    {
        Button scoreboardButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            scoreboardButton = new Button();
            scoreboardButton.Name = "scoreboardButton";

            // scoreboardButton
            scoreboardButton.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            scoreboardButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            scoreboardButton.Style = ButtonStyle.Custom;
            scoreboardButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/btn_rank.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/btn_rankp.png"),
                BackgroundDisabledImage = null,
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // UIOverlayScene
            this.RootWidget.AddChildLast(scoreboardButton);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 544;
                    this.DesignHeight = 960;

                    scoreboardButton.SetPosition(653, 402);
                    scoreboardButton.SetSize(214, 56);
                    scoreboardButton.Anchors = Anchors.None;
                    scoreboardButton.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    scoreboardButton.SetPosition(739, 355);
                    scoreboardButton.SetSize(159, 106);
                    scoreboardButton.Anchors = Anchors.None;
                    scoreboardButton.Visible = true;

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
