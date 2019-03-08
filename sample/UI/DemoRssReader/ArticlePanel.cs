/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    public partial class ArticlePanel : Panel
    {
        private const float articlePanelInEffectTime = 500.0f;
        private const float articlePanelOutEffectTime = 500.0f;

        private RssFeed currentRssFeed;
        private RssArticle currentArticle;
        private int currentArticleIndex;
        private ArticleDetailPanel currentArticleDetailPanel;

        private RssArticle nextArticle;
        private int nextArticleIndex;
        private ArticleDetailPanel nextArticleDetailPanel;

        public delegate void ArticleChangeEventHandler(object sender, ArticleChangeEventArgs e);
        public event ArticleChangeEventHandler ChangeArticle;

        public event EventHandler CloseButtonAction;

        public ImageAsset BackImage
        {
            get
            {
                return this.bgImage.Image;
            }
            set
            {
                this.bgImage.Image = value;
            }
        }

        public ArticlePanel(RssFeed rssFeed, int articleIndex)
        {
            InitializeWidget();

            currentRssFeed = rssFeed;
            currentArticleIndex = articleIndex;
            currentArticle = currentRssFeed.GetArticle(currentArticleIndex);

            // Button Event
            // Panel PrevArticle Button
            this.prevArticleButton.ButtonAction += new EventHandler<TouchEventArgs>(PrevArticleButtonAction);

            // Panel Close Button
            this.dialogCloseButton.ButtonAction += new EventHandler<TouchEventArgs>(PanelCloseButtonAction);

            // Panel Browse Button
//            this.articleBrowseButton.ButtonAction += new EventHandler<TouchEventArgs>(ArticleBrowseButtonAction);

            // Panel NextArticle Button
            this.nextArticleButton.ButtonAction += new EventHandler<TouchEventArgs>(NextArticleButtonAction);

            // Panel Article Panel
            currentArticleDetailPanel = new ArticleDetailPanel(currentArticle);
            currentArticleDetailPanel.X = 0.0f;
            currentArticleDetailPanel.Y = 0.0f;
            this.basePanel.AddChildLast(currentArticleDetailPanel);

            CheckArticleButton();
        }

        protected override void Render ()
        {
            currentArticleDetailPanel.Width = this.basePanel.Width;
            currentArticleDetailPanel.Height = this.basePanel.Height;

            base.Render ();
        }

        private void CheckArticleButton()
        {
            if (currentArticleIndex == 0)
            {
                this.prevArticleButton.Enabled = false;
            }

            if (currentArticleIndex == currentRssFeed.ArticleCount - 1)
            {
                this.nextArticleButton.Enabled = false;
            }
        }

        private void ChangeEnabledArticleButton(bool enabled)
        {
            this.prevArticleButton.Enabled = enabled;
            this.nextArticleButton.Enabled = enabled;
            this.dialogCloseButton.Enabled = enabled;
//            this.articleBrowseButton.Enabled = enabled;
        }


        private void PrevArticleButtonAction(object sender, TouchEventArgs e)
        {
            ChangeEnabledArticleButton(false);

            nextArticleIndex = currentArticleIndex - 1;
            nextArticle = currentRssFeed.GetArticle(nextArticleIndex);

            nextArticleDetailPanel = new ArticleDetailPanel(nextArticle);
            nextArticleDetailPanel.X = 0.0f;
            nextArticleDetailPanel.Y = 0.0f;
            nextArticleDetailPanel.Width = this.basePanel.Width;
            nextArticleDetailPanel.Height = this.basePanel.Height;
            this.basePanel.AddChildLast(nextArticleDetailPanel);

            SlideOutEffect articlePanelOutEffect
                = new SlideOutEffect(currentArticleDetailPanel,
                    articlePanelOutEffectTime, FourWayDirection.Down, SlideOutEffectInterpolator.EaseOutQuad);;
            articlePanelOutEffect.EffectStopped += new EventHandler<EventArgs>(ArticleSlideOutEffectStop);
            articlePanelOutEffect.Start();

            SlideInEffect articlePanelInEffect
                = new SlideInEffect(nextArticleDetailPanel,
                    articlePanelInEffectTime, FourWayDirection.Up, SlideInEffectInterpolator.EaseOutQuad);
            articlePanelInEffect.EffectStopped += new EventHandler<EventArgs>(ArticleSlideInEffectStop);
            articlePanelInEffect.Start();

        }

        private void PanelCloseButtonAction(object sender, TouchEventArgs e)
        {
            if (this.CloseButtonAction != null)
            {
                CloseButtonAction(this, EventArgs.Empty);
            }
        }

        private void ArticleBrowseButtonAction(object sender, TouchEventArgs e)
        {
            Shell.Action rssAction = Shell.Action.BrowserAction(currentArticle.ArticleLink);
            Shell.Execute(ref rssAction);
        }

        private void NextArticleButtonAction(object sender, TouchEventArgs e)
        {
            ChangeEnabledArticleButton(false);

            nextArticleIndex = currentArticleIndex + 1;
            nextArticle = currentRssFeed.GetArticle(nextArticleIndex);

            nextArticleDetailPanel = new ArticleDetailPanel(nextArticle);
            nextArticleDetailPanel.X = 0.0f;
            nextArticleDetailPanel.Y = 0.0f;
            nextArticleDetailPanel.Width = this.basePanel.Width;
            nextArticleDetailPanel.Height = this.basePanel.Height;
            this.basePanel.AddChildLast(nextArticleDetailPanel);

            SlideOutEffect articlePanelOutEffect
                = new SlideOutEffect(currentArticleDetailPanel,
                    articlePanelOutEffectTime, FourWayDirection.Up, SlideOutEffectInterpolator.EaseOutQuad);
            articlePanelOutEffect.EffectStopped += new EventHandler<EventArgs>(ArticleSlideOutEffectStop);
            articlePanelOutEffect.Start();

            SlideInEffect articlePanelInEffect
                = new SlideInEffect(nextArticleDetailPanel,
                    articlePanelInEffectTime, FourWayDirection.Down, SlideInEffectInterpolator.EaseOutQuad);
            articlePanelInEffect.EffectStopped += new EventHandler<EventArgs>(ArticleSlideInEffectStop);
            articlePanelInEffect.Start();
        }

        private void ArticleSlideOutEffectStop(object sender, EventArgs e)
        {
            Effect effect = (Effect)sender;

            this.basePanel.RemoveChild(effect.Widget);
        }

        private void ArticleSlideInEffectStop(object sender, EventArgs e)
        {
            currentRssFeed.SetArticleRead(nextArticleIndex);

            ArticleChangeEventArgs articleChangeEvent = new ArticleChangeEventArgs();
            articleChangeEvent.OldIndex = currentArticleIndex;
            articleChangeEvent.NewIndex = nextArticleIndex;
            OnChangeArticle(articleChangeEvent);

            currentArticleDetailPanel = nextArticleDetailPanel;
            currentArticleIndex = nextArticleIndex;
            currentArticle = nextArticle;

            ChangeEnabledArticleButton(true);

            CheckArticleButton();

        }

        private void OnChangeArticle(ArticleChangeEventArgs e)
        {
            if (ChangeArticle != null)
            {
                ChangeArticle(this, e);
            }
        }
    }

    public class ArticleChangeEventArgs : EventArgs
    {
        public int OldIndex;
        public int NewIndex;
    }
}
