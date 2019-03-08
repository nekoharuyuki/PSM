// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class MainScene
    {
        Panel sceneBackgroundPanel;
        ImageBox earthBg;
        LiveSphere earth;
        Panel shinePanel;
        ImageBox ImageBox_1;
        ImageBox ImageBox_2;
        Label Label_1;
        ListPanel rssFeedPanel;
        ImageBox ImageBox_3;
        Button feedAddButton;
        Button feedRemoveButton;
        Panel feedPanel;
        Panel baseLiveListPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            earthBg = new ImageBox();
            earthBg.Name = "earthBg";
            earth = new LiveSphere();
            earth.Name = "earth";
            shinePanel = new Panel();
            shinePanel.Name = "shinePanel";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            ImageBox_2 = new ImageBox();
            ImageBox_2.Name = "ImageBox_2";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            rssFeedPanel = new ListPanel();
            rssFeedPanel.Name = "rssFeedPanel";
            ImageBox_3 = new ImageBox();
            ImageBox_3.Name = "ImageBox_3";
            feedAddButton = new Button();
            feedAddButton.Name = "feedAddButton";
            feedRemoveButton = new Button();
            feedRemoveButton.Name = "feedRemoveButton";
            feedPanel = new Panel();
            feedPanel.Name = "feedPanel";
            baseLiveListPanel = new Panel();
            baseLiveListPanel.Name = "baseLiveListPanel";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(80f / 255f, 96f / 255f, 96f / 255f, 255f / 255f);

            // earthBg
            earthBg.Image = new ImageAsset("/Application/assets/earthShine.png");
            earthBg.ImageScaleType = ImageScaleType.AspectInside;

            // earth
            earth.Image = new ImageAsset("/Application/assets/worldMap.png");
            earth.TurnCount = 0;
            earth.TouchEnabled = true;

            // shinePanel
            shinePanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            shinePanel.Clip = true;

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/rsslist_shadow.png");
            ImageBox_1.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_1.NinePatchMargin = new NinePatchMargin(40, 40, 40, 40);

            // ImageBox_2
            ImageBox_2.Image = new ImageAsset("/Application/assets/list_top.png");

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // rssFeedPanel
            rssFeedPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            rssFeedPanel.ShowItemBorder = false;
            rssFeedPanel.ShowEmptySection = false;

            // ImageBox_3
            ImageBox_3.Image = new ImageAsset("/Application/assets/list_botom.png");

            // feedAddButton
            feedAddButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            feedAddButton.TextFont = new UIFont(FontAlias.System, 16, FontStyle.Regular);
            feedAddButton.Style = ButtonStyle.Custom;
            feedAddButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/plus_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/plus_button_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/plus_button_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // feedRemoveButton
            feedRemoveButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            feedRemoveButton.TextFont = new UIFont(FontAlias.System, 16, FontStyle.Regular);
            feedRemoveButton.Style = ButtonStyle.Custom;
            feedRemoveButton.CustomImage = new CustomButtonImageSettings()
            {
                BackgroundNormalImage = new ImageAsset("/Application/assets/minus_button_normal.png"),
                BackgroundPressedImage = new ImageAsset("/Application/assets/minus_button_pressed.png"),
                BackgroundDisabledImage = new ImageAsset("/Application/assets/minus_button_disable.png"),
                BackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0),
            };

            // feedPanel
            feedPanel.BackgroundColor = new UIColor(0f / 255f, 64f / 255f, 208f / 255f, 255f / 255f);
            feedPanel.Clip = true;
            feedPanel.AddChildLast(ImageBox_1);
            feedPanel.AddChildLast(ImageBox_2);
            feedPanel.AddChildLast(Label_1);
            feedPanel.AddChildLast(rssFeedPanel);
            feedPanel.AddChildLast(ImageBox_3);
            feedPanel.AddChildLast(feedAddButton);
            feedPanel.AddChildLast(feedRemoveButton);

            // baseLiveListPanel
            baseLiveListPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 0f / 255f);
            baseLiveListPanel.Clip = true;

            // MainScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(earthBg);
            this.RootWidget.AddChildLast(earth);
            this.RootWidget.AddChildLast(shinePanel);
            this.RootWidget.AddChildLast(feedPanel);
            this.RootWidget.AddChildLast(baseLiveListPanel);

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

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(480, 854);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    earthBg.SetPosition(385, 58);
                    earthBg.SetSize(200, 200);
                    earthBg.Anchors = Anchors.None;
                    earthBg.Visible = true;

                    earth.SetPosition(620, 241);
                    earth.SetSize(200, 200);
                    earth.Anchors = Anchors.None;
                    earth.Visible = true;

                    shinePanel.SetPosition(380, 60);
                    shinePanel.SetSize(100, 100);
                    shinePanel.Anchors = Anchors.None;
                    shinePanel.Visible = true;

                    ImageBox_1.SetPosition(20, -53);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(217, 0);
                    ImageBox_2.SetSize(200, 200);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Label_1.SetPosition(56, 14);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    rssFeedPanel.SetPosition(34, 175);
                    rssFeedPanel.SetSize(854, 400);
                    rssFeedPanel.Anchors = Anchors.None;
                    rssFeedPanel.Visible = true;

                    ImageBox_3.SetPosition(207, 364);
                    ImageBox_3.SetSize(200, 200);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    feedAddButton.SetPosition(16, 47);
                    feedAddButton.SetSize(214, 56);
                    feedAddButton.Anchors = Anchors.None;
                    feedAddButton.Visible = true;

                    feedRemoveButton.SetPosition(16, 47);
                    feedRemoveButton.SetSize(214, 56);
                    feedRemoveButton.Anchors = Anchors.None;
                    feedRemoveButton.Visible = true;

                    feedPanel.SetPosition(0, 0);
                    feedPanel.SetSize(100, 100);
                    feedPanel.Anchors = Anchors.None;
                    feedPanel.Visible = true;

                    baseLiveListPanel.SetPosition(321, 1);
                    baseLiveListPanel.SetSize(100, 100);
                    baseLiveListPanel.Anchors = Anchors.None;
                    baseLiveListPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(854, 480);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    earthBg.SetPosition(350, 30);
                    earthBg.SetSize(760, 760);
                    earthBg.Anchors = Anchors.None;
                    earthBg.Visible = true;

                    earth.SetPosition(380, 60);
                    earth.SetSize(700, 700);
                    earth.Anchors = Anchors.None;
                    earth.Visible = true;

                    shinePanel.SetPosition(300, 0);
                    shinePanel.SetSize(554, 479);
                    shinePanel.Anchors = Anchors.None;
                    shinePanel.Visible = true;

                    ImageBox_1.SetPosition(-8, 0);
                    ImageBox_1.SetSize(300, 470);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    ImageBox_2.SetPosition(0, 0);
                    ImageBox_2.SetSize(282, 40);
                    ImageBox_2.Anchors = Anchors.None;
                    ImageBox_2.Visible = true;

                    Label_1.SetPosition(53, 2);
                    Label_1.SetSize(176, 38);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    rssFeedPanel.SetPosition(0, 40);
                    rssFeedPanel.SetSize(282, 369);
                    rssFeedPanel.Anchors = Anchors.None;
                    rssFeedPanel.Visible = true;

                    ImageBox_3.SetPosition(0, 407);
                    ImageBox_3.SetSize(282, 54);
                    ImageBox_3.Anchors = Anchors.None;
                    ImageBox_3.Visible = true;

                    feedAddButton.SetPosition(181, 408);
                    feedAddButton.SetSize(50, 50);
                    feedAddButton.Anchors = Anchors.None;
                    feedAddButton.Visible = true;

                    feedRemoveButton.SetPosition(229, 408);
                    feedRemoveButton.SetSize(50, 50);
                    feedRemoveButton.Anchors = Anchors.None;
                    feedRemoveButton.Visible = true;

                    feedPanel.SetPosition(11, 10);
                    feedPanel.SetSize(282, 460);
                    feedPanel.Anchors = Anchors.None;
                    feedPanel.Visible = true;

                    baseLiveListPanel.SetPosition(320, 10);
                    baseLiveListPanel.SetSize(500, 458);
                    baseLiveListPanel.Anchors = Anchors.None;
                    baseLiveListPanel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "RSS Reader";
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
