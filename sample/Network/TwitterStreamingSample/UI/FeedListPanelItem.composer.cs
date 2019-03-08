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

namespace TwitterStreamingSample
{
    partial class FeedListPanelItem
    {
        ImageBox m_ImageBox;
        Label m_LabelUserName;
        Label m_LabelText;
        Label m_LabelUserScreenName;
        Label m_LabelDateTime;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_ImageBox = new ImageBox();
            m_ImageBox.Name = "m_ImageBox";
            m_LabelUserName = new Label();
            m_LabelUserName.Name = "m_LabelUserName";
            m_LabelText = new Label();
            m_LabelText.Name = "m_LabelText";
            m_LabelUserScreenName = new Label();
            m_LabelUserScreenName.Name = "m_LabelUserScreenName";
            m_LabelDateTime = new Label();
            m_LabelDateTime.Name = "m_LabelDateTime";

            // FeedListPanelItem
            this.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            this.AddChildLast(m_ImageBox);
            this.AddChildLast(m_LabelUserName);
            this.AddChildLast(m_LabelText);
            this.AddChildLast(m_LabelUserScreenName);
            this.AddChildLast(m_LabelDateTime);

            // m_ImageBox
            m_ImageBox.Image = new ImageAsset("/Application/assets/default_profile_0_normal.png");
            m_ImageBox.ImageScaleType = ImageScaleType.AspectInside;

            // m_LabelUserName
            m_LabelUserName.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_LabelUserName.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
            m_LabelUserName.LineBreak = LineBreak.Character;
            m_LabelUserName.VerticalAlignment = VerticalAlignment.Top;

            // m_LabelText
            m_LabelText.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_LabelText.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_LabelText.LineBreak = LineBreak.Character;
            m_LabelText.VerticalAlignment = VerticalAlignment.Top;

            // m_LabelUserScreenName
            m_LabelUserScreenName.TextColor = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 255f / 255f);
            m_LabelUserScreenName.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_LabelUserScreenName.LineBreak = LineBreak.Character;
            m_LabelUserScreenName.VerticalAlignment = VerticalAlignment.Top;

            // m_LabelDateTime
            m_LabelDateTime.TextColor = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 255f / 255f);
            m_LabelDateTime.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            m_LabelDateTime.LineBreak = LineBreak.Character;
            m_LabelDateTime.HorizontalAlignment = HorizontalAlignment.Right;
            m_LabelDateTime.VerticalAlignment = VerticalAlignment.Top;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 150);
                    this.Anchors = Anchors.None;

                    m_ImageBox.SetPosition(6, 0);
                    m_ImageBox.SetSize(100, 100);
                    m_ImageBox.Anchors = Anchors.None;
                    m_ImageBox.Visible = true;

                    m_LabelUserName.SetPosition(113, 2);
                    m_LabelUserName.SetSize(214, 36);
                    m_LabelUserName.Anchors = Anchors.None;
                    m_LabelUserName.Visible = true;

                    m_LabelText.SetPosition(114, 72);
                    m_LabelText.SetSize(429, 70);
                    m_LabelText.Anchors = Anchors.None;
                    m_LabelText.Visible = true;

                    m_LabelUserScreenName.SetPosition(113, 37);
                    m_LabelUserScreenName.SetSize(214, 36);
                    m_LabelUserScreenName.Anchors = Anchors.None;
                    m_LabelUserScreenName.Visible = true;

                    m_LabelDateTime.SetPosition(327, 2);
                    m_LabelDateTime.SetSize(214, 36);
                    m_LabelDateTime.Anchors = Anchors.None;
                    m_LabelDateTime.Visible = true;

                    break;

                default:
                    this.SetSize(960, 200);
                    this.Anchors = Anchors.None;

                    m_ImageBox.SetPosition(6, 7);
                    m_ImageBox.SetSize(96, 96);
                    m_ImageBox.Anchors = Anchors.None;
                    m_ImageBox.Visible = true;

                    m_LabelUserName.SetPosition(104, 5);
                    m_LabelUserName.SetSize(420, 30);
                    m_LabelUserName.Anchors = Anchors.None;
                    m_LabelUserName.Visible = true;

                    m_LabelText.SetPosition(104, 60);
                    m_LabelText.SetSize(838, 140);
                    m_LabelText.Anchors = Anchors.None;
                    m_LabelText.Visible = true;

                    m_LabelUserScreenName.SetPosition(104, 35);
                    m_LabelUserScreenName.SetSize(420, 25);
                    m_LabelUserScreenName.Anchors = Anchors.None;
                    m_LabelUserScreenName.Visible = true;

                    m_LabelDateTime.SetPosition(540, 0);
                    m_LabelDateTime.SetSize(410, 25);
                    m_LabelDateTime.Anchors = Anchors.None;
                    m_LabelDateTime.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_LabelUserName.Text = "label";

            m_LabelText.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０";

            m_LabelUserScreenName.Text = "label";

            m_LabelDateTime.Text = "label";
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

        public static ListPanelItem Creator()
        {
            return new FeedListPanelItem();
        }

    }
}
