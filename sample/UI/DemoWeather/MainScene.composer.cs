// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Weather
{
    partial class MainScene
    {
        ScrollPanel scrollPanel;
        Label dateLabel;
        Panel datePanel;
        ImageBox newsImage;
        Panel newsTickerPanel;
        Panel newsPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            scrollPanel = new ScrollPanel();
            scrollPanel.Name = "scrollPanel";
            dateLabel = new Label();
            dateLabel.Name = "dateLabel";
            datePanel = new Panel();
            datePanel.Name = "datePanel";
            newsImage = new ImageBox();
            newsImage.Name = "newsImage";
            newsTickerPanel = new Panel();
            newsTickerPanel.Name = "newsTickerPanel";
            newsPanel = new Panel();
            newsPanel.Name = "newsPanel";

            // scrollPanel
            scrollPanel.HorizontalScroll = true;
            scrollPanel.VerticalScroll = true;
            scrollPanel.ScrollBarVisibility = ScrollBarVisibility.Invisible;
            var scrollPanel_WorldPanel = new WorldPanel();
            scrollPanel.PanelWidth = scrollPanel_WorldPanel.Width;
            scrollPanel.PanelHeight = scrollPanel_WorldPanel.Height;
            scrollPanel.PanelX = 0;
            scrollPanel.PanelY = 0;
            scrollPanel.AddChildLast(scrollPanel_WorldPanel);

            // dateLabel
            dateLabel.TextColor = new UIColor(72f / 255f, 40f / 255f, 16f / 255f, 255f / 255f);
            dateLabel.Font = new UIFont(FontAlias.System, 30, FontStyle.Regular);
            dateLabel.LineBreak = LineBreak.Character;
            dateLabel.HorizontalAlignment = HorizontalAlignment.Center;

            // datePanel
            datePanel.BackgroundColor = new UIColor(232f / 255f, 192f / 255f, 120f / 255f, 191f / 255f);
            datePanel.Clip = true;
            datePanel.AddChildLast(dateLabel);

            // newsImage
            newsImage.Image = new ImageAsset("/Application/assets/world_news.png");

            // newsTickerPanel
            newsTickerPanel.BackgroundColor = new UIColor(232f / 255f, 192f / 255f, 120f / 255f, 0f / 255f);
            newsTickerPanel.Clip = true;

            // newsPanel
            newsPanel.BackgroundColor = new UIColor(232f / 255f, 192f / 255f, 120f / 255f, 191f / 255f);
            newsPanel.Clip = true;
            newsPanel.AddChildLast(newsImage);
            newsPanel.AddChildLast(newsTickerPanel);

            // MainScene
            this.RootWidget.AddChildLast(scrollPanel);
            this.RootWidget.AddChildLast(datePanel);
            this.RootWidget.AddChildLast(newsPanel);
            this.Transition = new FlipBoardTransition();

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

                    scrollPanel.SetPosition(408, 211);
                    scrollPanel.SetSize(100, 50);
                    scrollPanel.Anchors = Anchors.None;
                    scrollPanel.Visible = true;

                    dateLabel.SetPosition(-281, -66);
                    dateLabel.SetSize(214, 36);
                    dateLabel.Anchors = Anchors.None;
                    dateLabel.Visible = true;

                    datePanel.SetPosition(281, 66);
                    datePanel.SetSize(100, 100);
                    datePanel.Anchors = Anchors.None;
                    datePanel.Visible = true;

                    newsImage.SetPosition(235, 405);
                    newsImage.SetSize(200, 200);
                    newsImage.Anchors = Anchors.None;
                    newsImage.Visible = true;

                    newsTickerPanel.SetPosition(616, 480);
                    newsTickerPanel.SetSize(100, 100);
                    newsTickerPanel.Anchors = Anchors.None;
                    newsTickerPanel.Visible = true;

                    newsPanel.SetPosition(-180, -205);
                    newsPanel.SetSize(100, 100);
                    newsPanel.Anchors = Anchors.None;
                    newsPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    scrollPanel.SetPosition(0, 0);
                    scrollPanel.SetSize(854, 480);
                    scrollPanel.Anchors = Anchors.None;
                    scrollPanel.Visible = true;

                    dateLabel.SetPosition(0, 1);
                    dateLabel.SetSize(385, 38);
                    dateLabel.Anchors = Anchors.None;
                    dateLabel.Visible = true;

                    datePanel.SetPosition(16, 12);
                    datePanel.SetSize(385, 40);
                    datePanel.Anchors = Anchors.None;
                    datePanel.Visible = true;

                    newsImage.SetPosition(16, 2);
                    newsImage.SetSize(76, 36);
                    newsImage.Anchors = Anchors.None;
                    newsImage.Visible = true;

                    newsTickerPanel.SetPosition(111, 0);
                    newsTickerPanel.SetSize(692, 40);
                    newsTickerPanel.Anchors = Anchors.None;
                    newsTickerPanel.Visible = true;

                    newsPanel.SetPosition(16, 430);
                    newsPanel.SetSize(820, 40);
                    newsPanel.Anchors = Anchors.None;
                    newsPanel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            dateLabel.Text = "May 20, 2011 (Fri)";
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
