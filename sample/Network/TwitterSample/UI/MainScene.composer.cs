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
    partial class MainScene
    {
        Panel sceneBackgroundPanel;
        UserInterface.HeaderPanel m_HeaderPanel;
        UserInterface.FooterPanel m_FooterPanel;
        TwitterSample.UserInterface.FeedsPanel m_FeedsPanel;
        TwitterSample.UserInterface.AccountPanel m_AccountPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            m_HeaderPanel = new UserInterface.HeaderPanel();
            m_HeaderPanel.Name = "m_HeaderPanel";
            m_FooterPanel = new UserInterface.FooterPanel();
            m_FooterPanel.Name = "m_FooterPanel";
            m_FeedsPanel = new TwitterSample.UserInterface.FeedsPanel();
            m_FeedsPanel.Name = "m_FeedsPanel";
            m_AccountPanel = new TwitterSample.UserInterface.AccountPanel();
            m_AccountPanel.Name = "m_AccountPanel";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // MainScene
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(m_HeaderPanel);
            this.RootWidget.AddChildLast(m_FooterPanel);
            this.RootWidget.AddChildLast(m_FeedsPanel);
            this.RootWidget.AddChildLast(m_AccountPanel);

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

                    m_HeaderPanel.SetPosition(0, 0);
                    m_HeaderPanel.SetSize(544, 120);
                    m_HeaderPanel.Anchors = Anchors.None;
                    m_HeaderPanel.Visible = true;

                    m_FooterPanel.SetPosition(451, 120);
                    m_FooterPanel.SetSize(92, 839);
                    m_FooterPanel.Anchors = Anchors.None;
                    m_FooterPanel.Visible = true;

                    m_FeedsPanel.SetPosition(0, 120);
                    m_FeedsPanel.SetSize(451, 839);
                    m_FeedsPanel.Anchors = Anchors.None;
                    m_FeedsPanel.Visible = true;

                    m_AccountPanel.SetPosition(0, 120);
                    m_AccountPanel.SetSize(451, 839);
                    m_AccountPanel.Anchors = Anchors.None;
                    m_AccountPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(960, 544);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    m_HeaderPanel.SetPosition(0, 0);
                    m_HeaderPanel.SetSize(960, 64);
                    m_HeaderPanel.Anchors = Anchors.Top | Anchors.Left | Anchors.Right;
                    m_HeaderPanel.Visible = true;

                    m_FooterPanel.SetPosition(860, 64);
                    m_FooterPanel.SetSize(100, 480);
                    m_FooterPanel.Anchors = Anchors.Right | Anchors.Width;
                    m_FooterPanel.Visible = true;

                    m_FeedsPanel.SetPosition(0, 64);
                    m_FeedsPanel.SetSize(860, 480);
                    m_FeedsPanel.Anchors = Anchors.Left | Anchors.Right;
                    m_FeedsPanel.Visible = true;

                    m_AccountPanel.SetPosition(0, 64);
                    m_AccountPanel.SetSize(860, 480);
                    m_AccountPanel.Anchors = Anchors.Left | Anchors.Right;
                    m_AccountPanel.Visible = false;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
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
