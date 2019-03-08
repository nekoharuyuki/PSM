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
    partial class HeaderPanel
    {
        Button m_ButtonWrite;
        Button m_ButtonSearch;
        Label m_LabelTitle;
        EditableText m_TweetTexts;
        Label m_LabelCount;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_ButtonWrite = new Button();
            m_ButtonWrite.Name = "m_ButtonWrite";
            m_ButtonSearch = new Button();
            m_ButtonSearch.Name = "m_ButtonSearch";
            m_LabelTitle = new Label();
            m_LabelTitle.Name = "m_LabelTitle";
            m_TweetTexts = new EditableText();
            m_TweetTexts.Name = "m_TweetTexts";
            m_LabelCount = new Label();
            m_LabelCount.Name = "m_LabelCount";

            // HeaderPanel
            this.BackgroundColor = new UIColor(80f / 255f, 80f / 255f, 80f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(m_ButtonWrite);
            this.AddChildLast(m_ButtonSearch);
            this.AddChildLast(m_LabelTitle);
            this.AddChildLast(m_TweetTexts);
            this.AddChildLast(m_LabelCount);

            // m_ButtonWrite
            m_ButtonWrite.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonWrite.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Regular);

            // m_ButtonSearch
            m_ButtonSearch.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonSearch.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_LabelTitle
            m_LabelTitle.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_LabelTitle.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_LabelTitle.LineBreak = LineBreak.Character;

            // m_TweetTexts
            m_TweetTexts.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_TweetTexts.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_TweetTexts.LineBreak = LineBreak.Character;

            // m_LabelCount
            m_LabelCount.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_LabelCount.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_LabelCount.LineBreak = LineBreak.Character;
            m_LabelCount.HorizontalAlignment = HorizontalAlignment.Right;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 120);
                    this.Anchors = Anchors.None;

                    m_ButtonWrite.SetPosition(330, 64);
                    m_ButtonWrite.SetSize(214, 56);
                    m_ButtonWrite.Anchors = Anchors.None;
                    m_ButtonWrite.Visible = true;

                    m_ButtonSearch.SetPosition(330, 0);
                    m_ButtonSearch.SetSize(214, 56);
                    m_ButtonSearch.Anchors = Anchors.None;
                    m_ButtonSearch.Visible = true;

                    m_LabelTitle.SetPosition(0, 3);
                    m_LabelTitle.SetSize(221, 53);
                    m_LabelTitle.Anchors = Anchors.None;
                    m_LabelTitle.Visible = true;

                    m_TweetTexts.SetPosition(0, 64);
                    m_TweetTexts.SetSize(330, 56);
                    m_TweetTexts.Anchors = Anchors.None;
                    m_TweetTexts.Visible = true;

                    m_LabelCount.SetPosition(229, 0);
                    m_LabelCount.SetSize(101, 64);
                    m_LabelCount.Anchors = Anchors.None;
                    m_LabelCount.Visible = true;

                    break;

                default:
                    this.SetSize(960, 64);
                    this.Anchors = Anchors.None;

                    m_ButtonWrite.SetPosition(800, 0);
                    m_ButtonWrite.SetSize(160, 64);
                    m_ButtonWrite.Anchors = Anchors.None;
                    m_ButtonWrite.Visible = true;

                    m_ButtonSearch.SetPosition(640, 0);
                    m_ButtonSearch.SetSize(160, 64);
                    m_ButtonSearch.Anchors = Anchors.None;
                    m_ButtonSearch.Visible = true;

                    m_LabelTitle.SetPosition(0, 0);
                    m_LabelTitle.SetSize(140, 60);
                    m_LabelTitle.Anchors = Anchors.None;
                    m_LabelTitle.Visible = true;

                    m_TweetTexts.SetPosition(216, 4);
                    m_TweetTexts.SetSize(424, 56);
                    m_TweetTexts.Anchors = Anchors.None;
                    m_TweetTexts.Visible = true;

                    m_LabelCount.SetPosition(140, 0);
                    m_LabelCount.SetSize(76, 60);
                    m_LabelCount.Anchors = Anchors.None;
                    m_LabelCount.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_ButtonWrite.Text = UIStringTable.Get(UIStringID.RESID_WRITE);

            m_ButtonSearch.Text = UIStringTable.Get(UIStringID.RESID_SEARCH);

            m_LabelTitle.Text = "TESTTEST";
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
