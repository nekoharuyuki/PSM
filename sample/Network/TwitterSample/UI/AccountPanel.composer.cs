/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    partial class AccountPanel
    {
        Label m_AccountName;
        Label m_AccountScreenName;
        Label m_AccountProfile;
        Label m_AccountLocattion;
        ImageBox m_AccountImg;
        Label m_AccountTweets;
        Label m_AccountFollow;
        Label m_AccountFollower;
        ListPanel ListPanelFeeds;
        Label m_Label_Tweet;
        BusyIndicator m_BusyIndicator;
        Label Label_1;
        Label Label_2;
        Label Label_3;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_AccountName = new Label();
            m_AccountName.Name = "m_AccountName";
            m_AccountScreenName = new Label();
            m_AccountScreenName.Name = "m_AccountScreenName";
            m_AccountProfile = new Label();
            m_AccountProfile.Name = "m_AccountProfile";
            m_AccountLocattion = new Label();
            m_AccountLocattion.Name = "m_AccountLocattion";
            m_AccountImg = new ImageBox();
            m_AccountImg.Name = "m_AccountImg";
            m_AccountTweets = new Label();
            m_AccountTweets.Name = "m_AccountTweets";
            m_AccountFollow = new Label();
            m_AccountFollow.Name = "m_AccountFollow";
            m_AccountFollower = new Label();
            m_AccountFollower.Name = "m_AccountFollower";
            ListPanelFeeds = new ListPanel();
            ListPanelFeeds.Name = "ListPanelFeeds";
            m_Label_Tweet = new Label();
            m_Label_Tweet.Name = "m_Label_Tweet";
            m_BusyIndicator = new BusyIndicator(true);
            m_BusyIndicator.Name = "m_BusyIndicator";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";

            // AccountPanel
            this.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(m_AccountName);
            this.AddChildLast(m_AccountScreenName);
            this.AddChildLast(m_AccountProfile);
            this.AddChildLast(m_AccountLocattion);
            this.AddChildLast(m_AccountImg);
            this.AddChildLast(m_AccountTweets);
            this.AddChildLast(m_AccountFollow);
            this.AddChildLast(m_AccountFollower);
            this.AddChildLast(ListPanelFeeds);
            this.AddChildLast(m_Label_Tweet);
            this.AddChildLast(m_BusyIndicator);
            this.AddChildLast(Label_1);
            this.AddChildLast(Label_2);
            this.AddChildLast(Label_3);

            // m_AccountName
            m_AccountName.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountName.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_AccountName.LineBreak = LineBreak.Character;
            m_AccountName.HorizontalAlignment = HorizontalAlignment.Center;

            // m_AccountScreenName
            m_AccountScreenName.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountScreenName.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_AccountScreenName.LineBreak = LineBreak.Character;
            m_AccountScreenName.HorizontalAlignment = HorizontalAlignment.Center;

            // m_AccountProfile
            m_AccountProfile.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountProfile.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_AccountProfile.LineBreak = LineBreak.Character;
            m_AccountProfile.HorizontalAlignment = HorizontalAlignment.Center;

            // m_AccountLocattion
            m_AccountLocattion.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountLocattion.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_AccountLocattion.LineBreak = LineBreak.Character;
            m_AccountLocattion.HorizontalAlignment = HorizontalAlignment.Center;

            // m_AccountImg
            m_AccountImg.Image = new ImageAsset("/Application/assets/default_profile_0_normal.png");
            m_AccountImg.ImageScaleType = ImageScaleType.AspectInside;

            // m_AccountTweets
            m_AccountTweets.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountTweets.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_AccountTweets.LineBreak = LineBreak.Character;

            // m_AccountFollow
            m_AccountFollow.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountFollow.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_AccountFollow.LineBreak = LineBreak.Character;

            // m_AccountFollower
            m_AccountFollower.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_AccountFollower.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_AccountFollower.LineBreak = LineBreak.Character;

            // ListPanelFeeds
            ListPanelFeeds.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            ListPanelFeeds.ShowSection = false;
            ListPanelFeeds.ShowEmptySection = false;
            ListPanelFeeds.SetListItemCreator(FeedListPanelItem.Creator);

            // m_Label_Tweet
            m_Label_Tweet.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_Label_Tweet.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_Label_Tweet.LineBreak = LineBreak.Character;

            // Label_1
            Label_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 720);
                    this.Anchors = Anchors.None;

                    m_AccountName.SetPosition(22, 86);
                    m_AccountName.SetSize(500, 36);
                    m_AccountName.Anchors = Anchors.None;
                    m_AccountName.Visible = true;

                    m_AccountScreenName.SetPosition(22, 121);
                    m_AccountScreenName.SetSize(500, 36);
                    m_AccountScreenName.Anchors = Anchors.None;
                    m_AccountScreenName.Visible = true;

                    m_AccountProfile.SetPosition(22, 155);
                    m_AccountProfile.SetSize(500, 36);
                    m_AccountProfile.Anchors = Anchors.None;
                    m_AccountProfile.Visible = true;

                    m_AccountLocattion.SetPosition(22, 190);
                    m_AccountLocattion.SetSize(500, 36);
                    m_AccountLocattion.Anchors = Anchors.None;
                    m_AccountLocattion.Visible = true;

                    m_AccountImg.SetPosition(232, 2);
                    m_AccountImg.SetSize(82, 82);
                    m_AccountImg.Anchors = Anchors.None;
                    m_AccountImg.Visible = true;

                    m_AccountTweets.SetPosition(40, 234);
                    m_AccountTweets.SetSize(140, 36);
                    m_AccountTweets.Anchors = Anchors.None;
                    m_AccountTweets.Visible = true;

                    m_AccountFollow.SetPosition(202, 234);
                    m_AccountFollow.SetSize(140, 36);
                    m_AccountFollow.Anchors = Anchors.None;
                    m_AccountFollow.Visible = true;

                    m_AccountFollower.SetPosition(359, 234);
                    m_AccountFollower.SetSize(140, 36);
                    m_AccountFollower.Anchors = Anchors.None;
                    m_AccountFollower.Visible = true;

                    ListPanelFeeds.SetPosition(3, 340);
                    ListPanelFeeds.SetSize(539, 380);
                    ListPanelFeeds.Anchors = Anchors.None;
                    ListPanelFeeds.Visible = true;

                    m_Label_Tweet.SetPosition(3, 304);
                    m_Label_Tweet.SetSize(214, 36);
                    m_Label_Tweet.Anchors = Anchors.None;
                    m_Label_Tweet.Visible = true;

                    m_BusyIndicator.SetPosition(249, 165);
                    m_BusyIndicator.SetSize(48, 48);
                    m_BusyIndicator.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator.Visible = true;

                    Label_1.SetPosition(40, 268);
                    Label_1.SetSize(140, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(202, 268);
                    Label_2.SetSize(140, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(359, 268);
                    Label_3.SetSize(140, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    break;

                default:
                    this.SetSize(860, 480);
                    this.Anchors = Anchors.None;

                    m_AccountName.SetPosition(16, 78);
                    m_AccountName.SetSize(823, 20);
                    m_AccountName.Anchors = Anchors.None;
                    m_AccountName.Visible = true;

                    m_AccountScreenName.SetPosition(16, 97);
                    m_AccountScreenName.SetSize(823, 18);
                    m_AccountScreenName.Anchors = Anchors.None;
                    m_AccountScreenName.Visible = true;

                    m_AccountProfile.SetPosition(16, 114);
                    m_AccountProfile.SetSize(823, 18);
                    m_AccountProfile.Anchors = Anchors.None;
                    m_AccountProfile.Visible = true;

                    m_AccountLocattion.SetPosition(16, 131);
                    m_AccountLocattion.SetSize(823, 18);
                    m_AccountLocattion.Anchors = Anchors.None;
                    m_AccountLocattion.Visible = true;

                    m_AccountImg.SetPosition(394, 2);
                    m_AccountImg.SetSize(74, 74);
                    m_AccountImg.Anchors = Anchors.None;
                    m_AccountImg.Visible = true;

                    m_AccountTweets.SetPosition(295, 149);
                    m_AccountTweets.SetSize(80, 20);
                    m_AccountTweets.Anchors = Anchors.None;
                    m_AccountTweets.Visible = true;

                    m_AccountFollow.SetPosition(394, 149);
                    m_AccountFollow.SetSize(80, 20);
                    m_AccountFollow.Anchors = Anchors.None;
                    m_AccountFollow.Visible = true;

                    m_AccountFollower.SetPosition(508, 149);
                    m_AccountFollower.SetSize(80, 20);
                    m_AccountFollower.Anchors = Anchors.None;
                    m_AccountFollower.Visible = true;

                    ListPanelFeeds.SetPosition(1, 195);
                    ListPanelFeeds.SetSize(858, 284);
                    ListPanelFeeds.Anchors = Anchors.None;
                    ListPanelFeeds.Visible = true;

                    m_Label_Tweet.SetPosition(1, 169);
                    m_Label_Tweet.SetSize(102, 27);
                    m_Label_Tweet.Anchors = Anchors.None;
                    m_Label_Tweet.Visible = true;

                    m_BusyIndicator.SetPosition(410, 196);
                    m_BusyIndicator.SetSize(48, 48);
                    m_BusyIndicator.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator.Visible = true;

                    Label_1.SetPosition(295, 168);
                    Label_1.SetSize(80, 20);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(394, 168);
                    Label_2.SetSize(95, 20);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(508, 168);
                    Label_3.SetSize(95, 20);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_Label_Tweet.Text = UIStringTable.Get(UIStringID.RESID_TWEETS);

            Label_1.Text = UIStringTable.Get(UIStringID.RESID_TWEET);

            Label_2.Text = UIStringTable.Get(UIStringID.RESID_FOLLOWING);

            Label_3.Text = UIStringTable.Get(UIStringID.RESID_FOLLOWER);
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
