/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    class ArticleDetailPanel : Panel
    {
        private const int textFontSize = 30;

        private const float scrollPanelOffsetX = 10.0f;
        private const float titleLabelMarginX = 20.0f;
        private const float scrollPanelMarginX = 20.0f;
        private const float descriptionLabelMarginX = 15.0f;

        private const float titleLabelPositionX = 10.0f;
        private const float titleLabelPositionY = 10.0f;
        private const float scrollPanelPositionX = 10.0f;

        private Label titleLabel;
        private ScrollPanel scrollPanel;
        private Label descriptionLabel;

        public ArticleDetailPanel(RssArticle rssArticle)
        {
            // Font
            UIFont titleTextFont = new UIFont(FontAlias.System, textFontSize, FontStyle.Bold);
            UIFont descriptionTextFont = new UIFont(FontAlias.System, textFontSize, FontStyle.Regular);

            // Label
            titleLabel = new Label();
            titleLabel.X = titleLabelPositionX;
            titleLabel.Y = titleLabelPositionY;
            titleLabel.Width = this.Width - titleLabelMarginX;
            titleLabel.Text = rssArticle.ArticleTitle;
            titleLabel.Font = titleTextFont;
            titleLabel.HorizontalAlignment = HorizontalAlignment.Left;
            titleLabel.VerticalAlignment = VerticalAlignment.Top;
            titleLabel.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);

            float textHeight = titleLabel.TextHeight;
            if (titleLabel.Height < textHeight)
            {
                titleLabel.Height = textHeight;
            }
            this.AddChildLast(titleLabel);

            // ScrollPanel
            scrollPanel = new ScrollPanel();
            scrollPanel.X = scrollPanelPositionX;
            scrollPanel.Y = titleLabel.Y + titleLabel.Height + scrollPanelOffsetX;
            scrollPanel.Width = this.Width - scrollPanelMarginX;
            scrollPanel.Height = this.Height - (titleLabelPositionX + titleLabel.Height + (scrollPanelOffsetX * 2));
            scrollPanel.PanelX = 0.0f;
            scrollPanel.PanelY = 0.0f;
            scrollPanel.PanelWidth = scrollPanel.Width;
            scrollPanel.PanelHeight = scrollPanel.Height;
            scrollPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            this.AddChildLast(scrollPanel);

            // description
            descriptionLabel = new Label();
            descriptionLabel.X = 0.0f;
            descriptionLabel.Y = 0.0f;
            descriptionLabel.Width = scrollPanel.PanelWidth - descriptionLabelMarginX;
            descriptionLabel.Height = scrollPanel.Height;
            descriptionLabel.Text = rssArticle.ArticleDescription;
            descriptionLabel.Font = descriptionTextFont;
            descriptionLabel.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            if ( descriptionLabel.Text == "")
            {
                descriptionLabel.Text = "(no description)";
            }
            descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
            descriptionLabel.VerticalAlignment = VerticalAlignment.Top;

            descriptionLabel.Height = descriptionLabel.TextHeight;
            scrollPanel.PanelHeight = descriptionLabel.Height;
            if (descriptionLabel.Height <= scrollPanel.Height)
            {
                descriptionLabel.Height = scrollPanel.Height;
                scrollPanel.PanelHeight = descriptionLabel.Height;
            }
            scrollPanel.AddChildLast(descriptionLabel);
        }

        public override float Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                if (titleLabel != null &&
                    scrollPanel != null &&
                    descriptionLabel != null)
                {
                    base.Width = value;
                    titleLabel.Width = value - titleLabelMarginX;
                    scrollPanel.Width = value - scrollPanelMarginX;
                    scrollPanel.PanelWidth = scrollPanel.Width;
                    descriptionLabel.Width = scrollPanel.PanelWidth - descriptionLabelMarginX;

                    float textHeight = titleLabel.TextHeight;
                    if (titleLabel.Height < textHeight)
                    {
                        titleLabel.Height = textHeight;

                        scrollPanel.Y = titleLabel.Y + titleLabel.Height + scrollPanelOffsetX;

                        scrollPanel.Height = base.Height - (titleLabelPositionX + titleLabel.Height + (scrollPanelOffsetX * 2));
                        scrollPanel.PanelHeight = scrollPanel.Height;
                        descriptionLabel.Height = scrollPanel.Height;
                    }

                    descriptionLabel.Height = descriptionLabel.TextHeight;
                    scrollPanel.PanelHeight = descriptionLabel.Height;
                    if (descriptionLabel.Height <= scrollPanel.Height)
                    {
                        descriptionLabel.Height = scrollPanel.Height;
                        scrollPanel.PanelHeight = descriptionLabel.Height;
                    }
                }
            }
        }

        public override float Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                if (scrollPanel != null &&
                    descriptionLabel != null)
                {
                    base.Height = value;
                    scrollPanel.Y = titleLabel.Y + titleLabel.Height + scrollPanelOffsetX;

                    scrollPanel.Height = value - (titleLabelPositionX + titleLabel.Height + (scrollPanelOffsetX * 2));
                    if (descriptionLabel.Height <= scrollPanel.Height)
                    {
                        descriptionLabel.Height = scrollPanel.Height;
                        scrollPanel.PanelHeight = descriptionLabel.Height;
                    }
                }
            }
        }

    }
}

