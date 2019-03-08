// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    partial class UIMenuDialog
    {
        ImageBox ImageBox_background;
        ImageBox menuContinue;
        ImageBox menuReturn;
        ImageBox menuRanking;
        ImageBox menuHelp;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_background = new ImageBox();
            ImageBox_background.Name = "ImageBox_background";
            menuContinue = new ImageBox();
            menuContinue.Name = "menuContinue";
            menuReturn = new ImageBox();
            menuReturn.Name = "menuReturn";
            menuRanking = new ImageBox();
            menuRanking.Name = "menuRanking";
            menuHelp = new ImageBox();
            menuHelp.Name = "menuHelp";

            // ImageBox_background
            ImageBox_background.Image = new ImageAsset("/Application/assets/menudialog_bg.png");

            // menuContinue
            menuContinue.Image = new ImageAsset("/Application/assets/menu_continue.png");

            // menuReturn
            menuReturn.Image = new ImageAsset("/Application/assets/menu_return.png");

            // menuRanking
            menuRanking.Image = new ImageAsset("/Application/assets/menu_ranking.png");

            // menuHelp
            menuHelp.Image = new ImageAsset("/Application/assets/menu_help.png");

            // UIMenuDialog
            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = new ImageAsset("/Application/assets/menudialog_bg.png");
            this.CustomBackgroundNinePatchMargin = new NinePatchMargin(34, 34, 34, 34);
            this.CustomBackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(ImageBox_background);
            this.AddChildLast(menuContinue);
            this.AddChildLast(menuReturn);
            this.AddChildLast(menuRanking);
            this.AddChildLast(menuHelp);
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
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.None;

                    ImageBox_background.SetPosition(0, 0);
                    ImageBox_background.SetSize(200, 200);
                    ImageBox_background.Anchors = Anchors.None;
                    ImageBox_background.Visible = true;

                    menuContinue.SetPosition(173, 32);
                    menuContinue.SetSize(200, 200);
                    menuContinue.Anchors = Anchors.None;
                    menuContinue.Visible = true;

                    menuReturn.SetPosition(94, 127);
                    menuReturn.SetSize(200, 200);
                    menuReturn.Anchors = Anchors.None;
                    menuReturn.Visible = true;

                    menuRanking.SetPosition(195, 244);
                    menuRanking.SetSize(200, 200);
                    menuRanking.Anchors = Anchors.None;
                    menuRanking.Visible = true;

                    menuHelp.SetPosition(259, 328);
                    menuHelp.SetSize(200, 200);
                    menuHelp.Anchors = Anchors.None;
                    menuHelp.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    ImageBox_background.SetPosition(0, 0);
                    ImageBox_background.SetSize(854, 480);
                    ImageBox_background.Anchors = Anchors.None;
                    ImageBox_background.Visible = true;

                    menuContinue.SetPosition(216, 16);
                    menuContinue.SetSize(424, 112);
                    menuContinue.Anchors = Anchors.None;
                    menuContinue.Visible = true;

                    menuReturn.SetPosition(139, 125);
                    menuReturn.SetSize(576, 112);
                    menuReturn.Anchors = Anchors.None;
                    menuReturn.Visible = true;

                    menuRanking.SetPosition(230, 242);
                    menuRanking.SetSize(394, 112);
                    menuRanking.Anchors = Anchors.None;
                    menuRanking.Visible = true;

                    menuHelp.SetPosition(316, 362);
                    menuHelp.SetSize(224, 112);
                    menuHelp.Anchors = Anchors.None;
                    menuHelp.Visible = true;

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
