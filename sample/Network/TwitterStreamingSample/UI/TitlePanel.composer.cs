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
    partial class TitlePanel
    {
        Label m_LabelTitle;
        Button m_ButtonLogin;
        Label m_LabelProxy;
        EditableText m_Proxy;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_LabelTitle = new Label();
            m_LabelTitle.Name = "m_LabelTitle";
            m_ButtonLogin = new Button();
            m_ButtonLogin.Name = "m_ButtonLogin";
            m_LabelProxy = new Label();
            m_LabelProxy.Name = "m_LabelProxy";
            m_Proxy = new EditableText();
            m_Proxy.Name = "m_Proxy";

            // TitlePanel
            this.BackgroundColor = new UIColor(102f / 255f, 102f / 255f, 102f / 255f, 255f / 255f);
            this.Clip = true;
            this.AddChildLast(m_LabelTitle);
            this.AddChildLast(m_ButtonLogin);
            this.AddChildLast(m_LabelProxy);
            this.AddChildLast(m_Proxy);

            // m_LabelTitle
            m_LabelTitle.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_LabelTitle.Font = new UIFont(FontAlias.System, 40, FontStyle.Bold);
            m_LabelTitle.LineBreak = LineBreak.Character;
            m_LabelTitle.HorizontalAlignment = HorizontalAlignment.Center;

            // m_ButtonLogin
            m_ButtonLogin.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonLogin.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_LabelProxy
            m_LabelProxy.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            m_LabelProxy.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_LabelProxy.TextTrimming = TextTrimming.None;
            m_LabelProxy.LineBreak = LineBreak.Character;
            m_LabelProxy.HorizontalAlignment = HorizontalAlignment.Center;

            // m_Proxy
            m_Proxy.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Proxy.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_Proxy.LineBreak = LineBreak.Character;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 960);
                    this.Anchors = Anchors.None;

                    m_LabelTitle.SetPosition(0, 172);
                    m_LabelTitle.SetSize(544, 136);
                    m_LabelTitle.Anchors = Anchors.None;
                    m_LabelTitle.Visible = true;

                    m_ButtonLogin.SetPosition(162, 635);
                    m_ButtonLogin.SetSize(214, 56);
                    m_ButtonLogin.Anchors = Anchors.None;
                    m_ButtonLogin.Visible = true;

                    m_LabelProxy.SetPosition(162, 331);
                    m_LabelProxy.SetSize(214, 36);
                    m_LabelProxy.Anchors = Anchors.None;
                    m_LabelProxy.Visible = true;

                    m_Proxy.SetPosition(92, 367);
                    m_Proxy.SetSize(360, 56);
                    m_Proxy.Anchors = Anchors.None;
                    m_Proxy.Visible = true;

                    break;

                default:
                    this.SetSize(960, 544);
                    this.Anchors = Anchors.None;

                    m_LabelTitle.SetPosition(175, 50);
                    m_LabelTitle.SetSize(607, 138);
                    m_LabelTitle.Anchors = Anchors.None;
                    m_LabelTitle.Visible = true;

                    m_ButtonLogin.SetPosition(370, 427);
                    m_ButtonLogin.SetSize(220, 60);
                    m_ButtonLogin.Anchors = Anchors.Height;
                    m_ButtonLogin.Visible = true;

                    m_LabelProxy.SetPosition(151, 267);
                    m_LabelProxy.SetSize(137, 36);
                    m_LabelProxy.Anchors = Anchors.Width;
                    m_LabelProxy.Visible = true;

                    m_Proxy.SetPosition(288, 257);
                    m_Proxy.SetSize(375, 56);
                    m_Proxy.Anchors = Anchors.Height;
                    m_Proxy.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_LabelTitle.Text = "TwitterStreaming\n Sample";

            m_ButtonLogin.Text = UIStringTable.Get(UIStringID.RESID_LOGIN);

            m_LabelProxy.Text = "PROXY";

            m_Proxy.DefaultText = "http://example.com:8080";
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
