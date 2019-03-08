// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    partial class UITopMenu
    {
        ImageBox background;
        ImageBox ImageBox_title2;
        ImageBox ImageBox_title1;
        ImageBox menuStartBgTop;
        ImageBox menuStartTop;
        ImageBox menuRankingBgTop;
        ImageBox menuRankingTop;
        ImageBox menuHelpBgTop;
        ImageBox menuHelpTop;
        ImageBox ImageBox_animal1;
        ImageBox ImageBox_animal2;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            background = new ImageBox();
            background.Name = "background";
            ImageBox_title2 = new ImageBox();
            ImageBox_title2.Name = "ImageBox_title2";
            ImageBox_title1 = new ImageBox();
            ImageBox_title1.Name = "ImageBox_title1";
            menuStartBgTop = new ImageBox();
            menuStartBgTop.Name = "menuStartBgTop";
            menuStartTop = new ImageBox();
            menuStartTop.Name = "menuStartTop";
            menuRankingBgTop = new ImageBox();
            menuRankingBgTop.Name = "menuRankingBgTop";
            menuRankingTop = new ImageBox();
            menuRankingTop.Name = "menuRankingTop";
            menuHelpBgTop = new ImageBox();
            menuHelpBgTop.Name = "menuHelpBgTop";
            menuHelpTop = new ImageBox();
            menuHelpTop.Name = "menuHelpTop";
            ImageBox_animal1 = new ImageBox();
            ImageBox_animal1.Name = "ImageBox_animal1";
            ImageBox_animal2 = new ImageBox();
            ImageBox_animal2.Name = "ImageBox_animal2";

            // background
            background.Image = new ImageAsset("/Application/assets/title_bg.png");

            // ImageBox_title2
            ImageBox_title2.Image = new ImageAsset("/Application/assets/title_logo_2.png");

            // ImageBox_title1
            ImageBox_title1.Image = new ImageAsset("/Application/assets/title_logo_1.png");

            // menuStartBgTop
            menuStartBgTop.Image = new ImageAsset("/Application/assets/Board.png");

            // menuStartTop
            menuStartTop.Image = new ImageAsset("/Application/assets/menu_start.png");

            // menuRankingBgTop
            menuRankingBgTop.Image = new ImageAsset("/Application/assets/Board.png");

            // menuRankingTop
            menuRankingTop.Image = new ImageAsset("/Application/assets/menu_ranking.png");

            // menuHelpBgTop
            menuHelpBgTop.Image = new ImageAsset("/Application/assets/Board.png");

            // menuHelpTop
            menuHelpTop.Image = new ImageAsset("/Application/assets/menu_help.png");

            // ImageBox_animal1
            ImageBox_animal1.Image = new ImageAsset("/Application/assets/gorira.png");

            // ImageBox_animal2
            ImageBox_animal2.Image = new ImageAsset("/Application/assets/pig.png");

            // UITopMenu
            this.RootWidget.AddChildLast(background);
            this.RootWidget.AddChildLast(ImageBox_title2);
            this.RootWidget.AddChildLast(ImageBox_title1);
            this.RootWidget.AddChildLast(menuStartBgTop);
            this.RootWidget.AddChildLast(menuStartTop);
            this.RootWidget.AddChildLast(menuRankingBgTop);
            this.RootWidget.AddChildLast(menuRankingTop);
            this.RootWidget.AddChildLast(menuHelpBgTop);
            this.RootWidget.AddChildLast(menuHelpTop);
            this.RootWidget.AddChildLast(ImageBox_animal1);
            this.RootWidget.AddChildLast(ImageBox_animal2);
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

                    background.SetPosition(157, 99);
                    background.SetSize(200, 200);
                    background.Anchors = Anchors.None;
                    background.Visible = true;

                    ImageBox_title2.SetPosition(347, 120);
                    ImageBox_title2.SetSize(200, 200);
                    ImageBox_title2.Anchors = Anchors.None;
                    ImageBox_title2.Visible = true;

                    ImageBox_title1.SetPosition(26, 8);
                    ImageBox_title1.SetSize(200, 200);
                    ImageBox_title1.Anchors = Anchors.None;
                    ImageBox_title1.Visible = true;

                    menuStartBgTop.SetPosition(331, 152);
                    menuStartBgTop.SetSize(200, 200);
                    menuStartBgTop.Anchors = Anchors.None;
                    menuStartBgTop.Visible = true;

                    menuStartTop.SetPosition(281, 214);
                    menuStartTop.SetSize(200, 200);
                    menuStartTop.Anchors = Anchors.None;
                    menuStartTop.Visible = true;

                    menuRankingBgTop.SetPosition(432, 330);
                    menuRankingBgTop.SetSize(200, 200);
                    menuRankingBgTop.Anchors = Anchors.None;
                    menuRankingBgTop.Visible = true;

                    menuRankingTop.SetPosition(214, 288);
                    menuRankingTop.SetSize(200, 200);
                    menuRankingTop.Anchors = Anchors.None;
                    menuRankingTop.Visible = true;

                    menuHelpBgTop.SetPosition(280, 384);
                    menuHelpBgTop.SetSize(200, 200);
                    menuHelpBgTop.Anchors = Anchors.None;
                    menuHelpBgTop.Visible = true;

                    menuHelpTop.SetPosition(284, 353);
                    menuHelpTop.SetSize(200, 200);
                    menuHelpTop.Anchors = Anchors.None;
                    menuHelpTop.Visible = true;

                    ImageBox_animal1.SetPosition(653, 260);
                    ImageBox_animal1.SetSize(200, 200);
                    ImageBox_animal1.Anchors = Anchors.None;
                    ImageBox_animal1.Visible = true;

                    ImageBox_animal2.SetPosition(45, 303);
                    ImageBox_animal2.SetSize(200, 200);
                    ImageBox_animal2.Anchors = Anchors.None;
                    ImageBox_animal2.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    background.SetPosition(0, 0);
                    background.SetSize(854, 480);
                    background.Anchors = Anchors.None;
                    background.Visible = true;

                    ImageBox_title2.SetPosition(512, 56);
                    ImageBox_title2.SetSize(304, 96);
                    ImageBox_title2.Anchors = Anchors.None;
                    ImageBox_title2.Visible = true;

                    ImageBox_title1.SetPosition(24, 24);
                    ImageBox_title1.SetSize(480, 144);
                    ImageBox_title1.Anchors = Anchors.None;
                    ImageBox_title1.Visible = true;

                    menuStartBgTop.SetPosition(280, 224);
                    menuStartBgTop.SetSize(280, 80);
                    menuStartBgTop.Anchors = Anchors.None;
                    menuStartBgTop.Visible = true;

                    menuStartTop.SetPosition(352, 232);
                    menuStartTop.SetSize(134, 56);
                    menuStartTop.Anchors = Anchors.None;
                    menuStartTop.Visible = true;

                    menuRankingBgTop.SetPosition(280, 304);
                    menuRankingBgTop.SetSize(280, 80);
                    menuRankingBgTop.Anchors = Anchors.None;
                    menuRankingBgTop.Visible = true;

                    menuRankingTop.SetPosition(321, 312);
                    menuRankingTop.SetSize(197, 56);
                    menuRankingTop.Anchors = Anchors.None;
                    menuRankingTop.Visible = true;

                    menuHelpBgTop.SetPosition(280, 384);
                    menuHelpBgTop.SetSize(280, 80);
                    menuHelpBgTop.Anchors = Anchors.None;
                    menuHelpBgTop.Visible = true;

                    menuHelpTop.SetPosition(364, 392);
                    menuHelpTop.SetSize(112, 56);
                    menuHelpTop.Anchors = Anchors.None;
                    menuHelpTop.Visible = true;

                    ImageBox_animal1.SetPosition(48, 176);
                    ImageBox_animal1.SetSize(176, 296);
                    ImageBox_animal1.Anchors = Anchors.None;
                    ImageBox_animal1.Visible = true;

                    ImageBox_animal2.SetPosition(640, 240);
                    ImageBox_animal2.SetSize(160, 208);
                    ImageBox_animal2.Anchors = Anchors.None;
                    ImageBox_animal2.Visible = true;

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
                    ImageBox_title2.Visible = false;
                    ImageBox_title1.Visible = false;
                    break;

                default:
                    ImageBox_title2.Visible = false;
                    ImageBox_title1.Visible = false;
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    new SlideInEffect()
                    {
                        Widget = ImageBox_title2,
                        MoveDirection = FourWayDirection.Right,
                    }.Start();
                    new SlideInEffect()
                    {
                        Widget = ImageBox_title1,
                        MoveDirection = FourWayDirection.Left,
                    }.Start();
                    break;

                default:
                    new SlideInEffect()
                    {
                        Widget = ImageBox_title2,
                        MoveDirection = FourWayDirection.Right,
                    }.Start();
                    new SlideInEffect()
                    {
                        Widget = ImageBox_title1,
                        MoveDirection = FourWayDirection.Left,
                    }.Start();
                    break;
            }
        }

    }
}
