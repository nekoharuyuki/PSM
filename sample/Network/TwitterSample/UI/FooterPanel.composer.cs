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
    partial class FooterPanel
    {
        Button m_ButtonHome;
        Button m_ButtonMentions;
        Button m_ButtonTrend;
        Button m_ButtonAccount;
        Button m_ButtonLogout;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_ButtonHome = new Button();
            m_ButtonHome.Name = "m_ButtonHome";
            m_ButtonMentions = new Button();
            m_ButtonMentions.Name = "m_ButtonMentions";
            m_ButtonTrend = new Button();
            m_ButtonTrend.Name = "m_ButtonTrend";
            m_ButtonAccount = new Button();
            m_ButtonAccount.Name = "m_ButtonAccount";
            m_ButtonLogout = new Button();
            m_ButtonLogout.Name = "m_ButtonLogout";

            // FooterPanel
            this.BackgroundColor = new UIColor(80f / 255f, 80f / 255f, 80f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(m_ButtonHome);
            this.AddChildLast(m_ButtonMentions);
            this.AddChildLast(m_ButtonTrend);
            this.AddChildLast(m_ButtonAccount);
            this.AddChildLast(m_ButtonLogout);

            // m_ButtonHome
            m_ButtonHome.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonHome.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);

            // m_ButtonMentions
            m_ButtonMentions.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonMentions.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);

            // m_ButtonTrend
            m_ButtonTrend.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonTrend.TextFont = new UIFont(FontAlias.System, 20, FontStyle.Regular);

            // m_ButtonAccount
            m_ButtonAccount.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonAccount.TextFont = new UIFont(FontAlias.System, 18, FontStyle.Regular);

            // m_ButtonLogout
            m_ButtonLogout.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonLogout.TextFont = new UIFont(FontAlias.System, 18, FontStyle.Regular);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(92, 839);
                    this.Anchors = Anchors.None;

                    m_ButtonHome.SetPosition(0, 0);
                    m_ButtonHome.SetSize(91, 165);
                    m_ButtonHome.Anchors = Anchors.None;
                    m_ButtonHome.Visible = true;

                    m_ButtonMentions.SetPosition(0, 169);
                    m_ButtonMentions.SetSize(91, 165);
                    m_ButtonMentions.Anchors = Anchors.None;
                    m_ButtonMentions.Visible = true;

                    m_ButtonTrend.SetPosition(1, 337);
                    m_ButtonTrend.SetSize(91, 165);
                    m_ButtonTrend.Anchors = Anchors.None;
                    m_ButtonTrend.Visible = true;

                    m_ButtonAccount.SetPosition(0, 506);
                    m_ButtonAccount.SetSize(91, 165);
                    m_ButtonAccount.Anchors = Anchors.None;
                    m_ButtonAccount.Visible = true;

                    m_ButtonLogout.SetPosition(1, 674);
                    m_ButtonLogout.SetSize(91, 165);
                    m_ButtonLogout.Anchors = Anchors.None;
                    m_ButtonLogout.Visible = true;

                    break;

                default:
                    this.SetSize(100, 480);
                    this.Anchors = Anchors.None;

                    m_ButtonHome.SetPosition(0, 0);
                    m_ButtonHome.SetSize(100, 95);
                    m_ButtonHome.Anchors = Anchors.None;
                    m_ButtonHome.Visible = true;

                    m_ButtonMentions.SetPosition(0, 96);
                    m_ButtonMentions.SetSize(100, 95);
                    m_ButtonMentions.Anchors = Anchors.None;
                    m_ButtonMentions.Visible = true;

                    m_ButtonTrend.SetPosition(0, 192);
                    m_ButtonTrend.SetSize(100, 95);
                    m_ButtonTrend.Anchors = Anchors.None;
                    m_ButtonTrend.Visible = true;

                    m_ButtonAccount.SetPosition(0, 288);
                    m_ButtonAccount.SetSize(100, 95);
                    m_ButtonAccount.Anchors = Anchors.None;
                    m_ButtonAccount.Visible = true;

                    m_ButtonLogout.SetPosition(0, 385);
                    m_ButtonLogout.SetSize(100, 95);
                    m_ButtonLogout.Anchors = Anchors.None;
                    m_ButtonLogout.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_ButtonHome.Text = UIStringTable.Get(UIStringID.RESID_HOME);

            m_ButtonMentions.Text = UIStringTable.Get(UIStringID.RESID_CONNECT);

            m_ButtonTrend.Text = UIStringTable.Get(UIStringID.RESID_DISCOVER);

            m_ButtonAccount.Text = UIStringTable.Get(UIStringID.RESID_ME);

            m_ButtonLogout.Text = UIStringTable.Get(UIStringID.RESID_LOGOUT);
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
