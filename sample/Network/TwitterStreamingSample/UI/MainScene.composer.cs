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
    partial class MainScene
    {
        Panel sceneBackgroundPanel;
        Button m_SetButton;
        Label Label_1;
        TwitterStreamingSample.FeedsPanel m_FeedsPanel;
        BusyIndicator m_BusyIndicator;
        Button m_ButtonLogOut;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            m_SetButton = new Button();
            m_SetButton.Name = "m_SetButton";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            m_FeedsPanel = new TwitterStreamingSample.FeedsPanel();
            m_FeedsPanel.Name = "m_FeedsPanel";
            m_BusyIndicator = new BusyIndicator(true);
            m_BusyIndicator.Name = "m_BusyIndicator";
            m_ButtonLogOut = new Button();
            m_ButtonLogOut.Name = "m_ButtonLogOut";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(62f / 255f, 61f / 255f, 61f / 255f, 255f / 255f);

            // MainScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(m_SetButton);
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(m_FeedsPanel);
            this.RootWidget.AddChildLast(m_BusyIndicator);
            this.RootWidget.AddChildLast(m_ButtonLogOut);

            // m_SetButton
            m_SetButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_SetButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold | FontStyle.Italic);
            Label_1.LineBreak = LineBreak.Character;

            // m_ButtonLogOut
            m_ButtonLogOut.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonLogOut.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(544, 960);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    m_SetButton.SetPosition(330, 9);
                    m_SetButton.SetSize(214, 56);
                    m_SetButton.Anchors = Anchors.None;
                    m_SetButton.Visible = true;

                    Label_1.SetPosition(6, 12);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    m_FeedsPanel.SetPosition(0, 65);
                    m_FeedsPanel.SetSize(544, 894);
                    m_FeedsPanel.Anchors = Anchors.None;
                    m_FeedsPanel.Visible = true;

                    m_BusyIndicator.SetPosition(248, 464);
                    m_BusyIndicator.SetSize(48, 48);
                    m_BusyIndicator.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator.Visible = true;

                    m_ButtonLogOut.SetPosition(785, 10);
                    m_ButtonLogOut.SetSize(214, 56);
                    m_ButtonLogOut.Anchors = Anchors.None;
                    m_ButtonLogOut.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(960, 544);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    m_SetButton.SetPosition(525, 1);
                    m_SetButton.SetSize(215, 56);
                    m_SetButton.Anchors = Anchors.None;
                    m_SetButton.Visible = true;

                    Label_1.SetPosition(0, 4);
                    Label_1.SetSize(400, 53);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    m_FeedsPanel.SetPosition(0, 58);
                    m_FeedsPanel.SetSize(960, 486);
                    m_FeedsPanel.Anchors = Anchors.None;
                    m_FeedsPanel.Visible = true;

                    m_BusyIndicator.SetPosition(446, 276);
                    m_BusyIndicator.SetSize(48, 48);
                    m_BusyIndicator.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator.Visible = true;

                    m_ButtonLogOut.SetPosition(745, 0);
                    m_ButtonLogOut.SetSize(215, 56);
                    m_ButtonLogOut.Anchors = Anchors.None;
                    m_ButtonLogOut.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            this.Title = "TwitterSample StreamingAPI";

            m_SetButton.Text = UIStringTable.Get(UIStringID.RESID_SETTING);

            Label_1.Text = "TwitterStreaming";

            m_ButtonLogOut.Text = UIStringTable.Get(UIStringID.RESID_LOGOUT);
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
