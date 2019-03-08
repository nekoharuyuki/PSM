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
    public partial class MainScene : Scene
    {
        private RssFeed currentRssFeed;
        private AddFeedDialog addFeedDialog;

        public MainScene()
        {
            InitializeWidget();

            // Button Defalut
            RemoveButtonEnabled = false;

            // Button Event
            this.feedAddButton.ButtonAction += new EventHandler<TouchEventArgs>(FeedAddButtonAction);
            this.feedRemoveButton.ButtonAction += new EventHandler<TouchEventArgs>(FeedRemoveButtonAction);

            // Setup live sphere
            earth.ZAxis = (float)((23.4f / 180.0f) * Math.PI);

            // Add shine
            for (int i = 0; i < 7; i++)
            {
                shinePanel.AddChildLast(new ShineWidget(80.0f, 60.0f, shinePanel.Width - 80.0f, shinePanel.Height - 60.0f));
            }
        }

        protected override void OnUpdate(float elapsedTime)
        {
            // Live sphere
            earth.YAxis += 0.002f;
        }

        internal ListPanel RssFeedPanel
        {
            get
            {
                return this.rssFeedPanel;
            }
            set
            {
                if (this.rssFeedPanel != null)
                {
                    this.rssFeedPanel = value;
                }
            }
        }

        internal Panel BaseLiveListPanel
        {
            get
            {
                return this.baseLiveListPanel;
            }
            set
            {
                if (this.baseLiveListPanel != null)
                {
                    this.baseLiveListPanel = value;
                }
            }
        }

        internal bool RemoveButtonEnabled
        {
            set
            {
                if (feedRemoveButton != null)
                {
                    feedRemoveButton.Enabled = value;
                }
            }
        }

        internal RssFeed CurrentRssFeed
        {
            set
            {
                currentRssFeed = value;
            }
        }

        private void FeedAddButtonAction(object sender, TouchEventArgs e)
        {
            addFeedDialog = new AddFeedDialog();

            FadeInEffect addDialogFadeInEffect = new FadeInEffect();
            addDialogFadeInEffect.Time = 0.0f;

            addFeedDialog.Show(addDialogFadeInEffect);
        }

        private void FeedRemoveButtonAction(object sender, TouchEventArgs e)
        {
            MessageDialog removeCheckDialog = new MessageDialog();
            removeCheckDialog.Title = "Remove feed?";
            removeCheckDialog.Message = currentRssFeed.RssTitle + " will be removed.";
            removeCheckDialog.ButtonPressed
                += new EventHandler<MessageDialogButtonEventArgs>(RemoveDialogButtonPressed);
            removeCheckDialog.Show();
        }

        private void RemoveDialogButtonPressed(object sender, MessageDialogButtonEventArgs e)
        {
            MessageDialog messageDialog = (MessageDialog)sender;
            messageDialog.Hide();

            System.Console.WriteLine("RemoveDialogButtonPressed: " + e.Result);
        }
    }
}
