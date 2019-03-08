/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    public class ContentsScene : Scene
    {
        protected NavigationPanel navigationPanel;

        public ContentsScene()
        {
            navigationPanel = new NavigationPanel();
            this.RootWidget.AddChildLast(navigationPanel);

            navigationPanel.TopButton.ButtonAction += (sender, e) =>
            {
                if (ReturnTopAction != null)
                    ReturnTopAction();
            };
            navigationPanel.PreviousButton.ButtonAction += (sender, e) =>
            {
                if (MovePreviousAction != null)
                    MovePreviousAction();
            };
            navigationPanel.NextButton.ButtonAction += (sender, e) =>
            {
                if (MoveNextAction != null)
                    MoveNextAction();
            };
            navigationPanel.TransitionSettingButton.ButtonAction += (sender, e) =>
            {
                if (OpenTransitionSettingAction != null)
                    OpenTransitionSettingAction();
            };

            this.RootWidget.KeyEventReceived += HandleRootWidgethandleKeyEventReceived;
        }

        void HandleRootWidgethandleKeyEventReceived(object sender, KeyEventArgs e)
        {
            if (e.KeyEventType == KeyEventType.Down)
            {
                switch (e.KeyType)
                {
                    case KeyType.Back:
                        if (ReturnTopAction != null)
                        {
                            ReturnTopAction();
                            e.Handled = true;
                        }
                        break;
                    case KeyType.Triangle:
                        if (OpenTransitionSettingAction != null)
                        {
                            OpenTransitionSettingAction();
                            e.Handled = true;
                        }
                        break;
                    case KeyType.L:
                        if (MovePreviousAction != null)
                        {
                            MovePreviousAction();
                            e.Handled = true;
                        }
                        break;
                    case KeyType.R:
                        if (MoveNextAction != null)
                        {
                            MoveNextAction();
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public string Name
        {
            get
            {
                return navigationPanel.Name;
            }
            set
            {
                navigationPanel.Name = value;
            }
        }

        public Action MovePreviousAction { get; set; }

        public Action MoveNextAction { get; set; }

        public Action ReturnTopAction { get; set; }

        public Action OpenTransitionSettingAction { get; set; }

        public Panel HeaderPanel
        {
            get
            {
                return navigationPanel.HeaderPanel;
            }
        }

        public Panel ControlPanel
        {
            get
            {
                return navigationPanel.ControlPanel;
            }
        }

        public Button PreviousButton
        {
            get
            {
                return navigationPanel.PreviousButton;
            }
            set
            {
                navigationPanel.PreviousButton = value;
            }
        }

        public Button NextButton
        {
            get
            {
                return navigationPanel.NextButton;
            }
            set
            {
                navigationPanel.NextButton = value;
            }
        }

        public Button TopButton
        {
            get
            {
                return navigationPanel.TopButton;
            }
            set
            {
                navigationPanel.TopButton = value;
            }
        }

        public Button TransitionSettingButton
        {
            get
            {
                return navigationPanel.TransitionSettingButton;
            }
            set
            {
                navigationPanel.TransitionSettingButton = value;
            }
        }

        public bool BackgroundVisible
        {
            get
            {
                return navigationPanel.BackgroundVisible;
            }
            set
            {
                navigationPanel.BackgroundVisible = value;
            }
        }
    }
}
