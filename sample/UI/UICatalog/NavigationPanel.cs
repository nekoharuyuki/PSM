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

namespace UICatalog
{
    public partial class NavigationPanel : Panel
    {
        public NavigationPanel()
        {
            InitializeWidget();
            ControlPanel.Clip = false;
            this.Anchors = Anchors.None;
        }

        public new string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                titleLabel.Text = name;
            }
        }
        private string name = "";

        public Panel HeaderPanel
        {
            get
            {
                return headerPanel;
            }
        }

        public Panel ControlPanel
        {
            get
            {
                return controlPanel;
            }
        }

        public Button PreviousButton
        {
            get
            {
                return previousButton;
            }
            set
            {
                previousButton = value;
            }
        }

        public Button NextButton
        {
            get
            {
                return nextButton;
            }
            set
            {
                nextButton = value;
            }
        }

        public Button TopButton
        {
            get
            {
                return topButton;
            }
            set
            {
                topButton = value;
            }
        }

        public Button TransitionSettingButton
        {
            get
            {
                return transitionSettingButton;
            }
            set
            {
                transitionSettingButton = value;
            }
        }

        public bool BackgroundVisible
        {
            get
            {
                return backgroundVisible;
            }
            set
            { 
                backgroundVisible = value;
                backgroundPanel.BackgroundColor = new UIColor(0.2f,0.2f,0.2f,value?1f:0f);
            }
        }
        private bool backgroundVisible;
    }
}
