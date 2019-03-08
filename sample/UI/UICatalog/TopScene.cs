/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public class TopScene : ContentsScene
    {
        Panel contentsPanel;
        Panel widgetPanel;
        Panel liveWidgetPanel;
        Panel effectPanel;
        Button widgetButton;
        Button liveWidgetButton;
        Button effectButton;
        TransitionSettingDialog transitionSettingDialog;
        Transition transition { get { return transitionSettingDialog.Transition; } }

        List<ContentsScene> currentSceneList;
        List<ContentsScene> widgetSceneList = new List<ContentsScene>();
        List<ContentsScene> liveWidgetSceneList = new List<ContentsScene>();
        List<ContentsScene> effectSceneList = new List<ContentsScene>();


        int sceneIndex = -1;
        private const float LeftMargine = 30.0f;
        private const float topMargine = 20.0f;
        private const float RightMargine = 30.0f;

        private const float buttonWidth = 84.0f;
        private const float buttonHeight = 58.0f;

        private const float itemGap = 5.0f;
        private const float rowGap = 6.0f;
        private const float columnGap = 15.0f;

        public TopScene ()
        {
        }

        public void SetupContents ()
        {
            const float buttonHorizontalOffset = 20.0f;
            const float buttonVerticalOffset = 5.0f;

            transitionSettingDialog = new TransitionSettingDialog();

            var delButton = new List<Widget>(ControlPanel.Children);
            foreach (var item in delButton)
            {
                ControlPanel.RemoveChild(item);
            }

            navigationPanel.HeaderPanel.Visible = false;
            navigationPanel.ControlPanel.Y = 0.0f;
            navigationPanel.Width = this.RootWidget.Width;
            navigationPanel.Height = this.RootWidget.Height;

            contentsPanel = new Panel();
            contentsPanel.SetPosition(0, ControlPanel.Y + ControlPanel.Height);
            contentsPanel.Width = this.RootWidget.Width;
            contentsPanel.Height = this.RootWidget.Height - ControlPanel.Height;
            this.RootWidget.AddChildLast(contentsPanel);
            
            widgetButton = new Button();
            ControlPanel.AddChildLast(widgetButton);
            widgetButton.Width = (ControlPanel.Width - buttonHorizontalOffset) / 3 - buttonHorizontalOffset;
            widgetButton.X = buttonHorizontalOffset;
            widgetButton.Y = buttonVerticalOffset;
            widgetButton.Text = "Widgets";
            widgetButton.ButtonAction += new EventHandler<TouchEventArgs>(widgetButton_ButtonAction);

            liveWidgetButton = new Button();
            ControlPanel.AddChildLast(liveWidgetButton);
            liveWidgetButton.Width = (ControlPanel.Width - buttonHorizontalOffset) / 3 - buttonHorizontalOffset;
            liveWidgetButton.X = widgetButton.X + widgetButton.Width + buttonHorizontalOffset;
            liveWidgetButton.Y = buttonVerticalOffset;
            liveWidgetButton.Text = "Live widgets";
            liveWidgetButton.ButtonAction += new EventHandler<TouchEventArgs>(liveWidgetButton_ButtonAction);

            effectButton = new Button();
            ControlPanel.AddChildLast(effectButton);
            effectButton.Width = widgetButton.Width;
            effectButton.X = liveWidgetButton.X + liveWidgetButton.Width + buttonHorizontalOffset;
            effectButton.Y = widgetButton.Y;
            effectButton.Text = "Effects";
            effectButton.ButtonAction += new EventHandler<TouchEventArgs>(effectButton_ButtonAction);

            ControlPanel.Height = widgetButton.Height + buttonVerticalOffset * 2;

            widgetPanel = new Panel();
            widgetPanel.Width = contentsPanel.Width;
            widgetPanel.Height = contentsPanel.Height;
            contentsPanel.AddChildLast(widgetPanel);

            liveWidgetPanel = new Panel();
            liveWidgetPanel.Width = contentsPanel.Width;
            liveWidgetPanel.Height = contentsPanel.Height;
            contentsPanel.AddChildLast(liveWidgetPanel);

            effectPanel = new Panel();
            effectPanel.Width = contentsPanel.Width;
            effectPanel.Height = contentsPanel.Height;
            contentsPanel.AddChildLast(effectPanel);

            setupSceneButtonAction(widgetSceneList);
            setupSceneButtonAction(liveWidgetSceneList);
            setupSceneButtonAction(effectSceneList);

            setupTitlePanel(widgetPanel, widgetSceneList);
            setupTitlePanel(liveWidgetPanel, liveWidgetSceneList);
            setupTitlePanel(effectPanel, effectSceneList);


            liveWidgetPanel.Visible = false;
            effectPanel.Visible = false;
            widgetButton.Enabled = false;
            currentSceneList = widgetSceneList;
            BackgroundVisible = false;
        }

        public void AddWidgetScene(ContentsScene scene)
        {
            widgetSceneList.Add(scene);
        }
        
        public void AddLiveWidgetScene(ContentsScene scene)
        {
            liveWidgetSceneList.Add(scene);
        }
        
        public void AddEffectScene(ContentsScene scene)
        {
            effectSceneList.Add(scene);
        }


        void setupSceneButtonAction(List<ContentsScene> scenes)
        {
            Debug.Assert(scenes != null && scenes.Count > 0);

            foreach (var scene in scenes)
            {
                scene.MovePreviousAction = () => ContentsSceneChange(sceneIndex - 1);
                scene.ReturnTopAction = () => ContentsSceneChange(-1);
                scene.MoveNextAction = () => ContentsSceneChange(sceneIndex + 1);
                scene.OpenTransitionSettingAction = () => transitionSettingDialog.Show();
            }
            scenes[0].PreviousButton.Enabled = false;
            scenes[scenes.Count - 1].NextButton.Enabled = false;
        }

        void widgetButton_ButtonAction(object sender, TouchEventArgs e)
        {
            this.currentSceneList = this.widgetSceneList;

            widgetPanel.Visible = true;
            liveWidgetPanel.Visible = false;
            effectPanel.Visible = false;

            widgetButton.Enabled = false;
            liveWidgetButton.Enabled = true;
            effectButton.Enabled = true;
        }

        void liveWidgetButton_ButtonAction(object sender, TouchEventArgs e)
        {
            this.currentSceneList = this.liveWidgetSceneList;

            widgetPanel.Visible = false;
            liveWidgetPanel.Visible = true;
            effectPanel.Visible = false;

            widgetButton.Enabled = true;
            liveWidgetButton.Enabled = false;
            effectButton.Enabled = true;
        }

        void effectButton_ButtonAction(object sender, TouchEventArgs e)
        {
            this.currentSceneList = this.effectSceneList;

            widgetPanel.Visible = false;
            liveWidgetPanel.Visible = false;
            effectPanel.Visible = true;

            widgetButton.Enabled = true;
            liveWidgetButton.Enabled = true;
            effectButton.Enabled = false;
        }

        void setupTitlePanel(Panel panel, List<ContentsScene> list)
        {
            int rowNum = (int)((panel.Height - topMargine) / (buttonHeight + rowGap));
            int colNum = (list.Count - 1) / rowNum + 1;

            float width = (int)((int)(panel.Width - (LeftMargine + RightMargine - columnGap)) / colNum);
            float height = buttonHeight + rowGap;
            float labelWidth = width - (buttonWidth + itemGap + columnGap);

            float x = LeftMargine;
            float y = topMargine;
            for (int i = 0; i < list.Count; i++)
            {
                Button button = new Button();
                button.SetPosition(x, y);
                button.SetSize(buttonWidth, buttonHeight);
                button.Text = (i + 1).ToString();
                button.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction);
                panel.AddChildLast(button);

                Label label = new Label();
                label.SetPosition(x + buttonWidth + itemGap, y);
                label.SetSize(labelWidth, buttonHeight);
                label.VerticalAlignment = VerticalAlignment.Middle;
                label.Text = list[i].Name;
                panel.AddChildLast(label);

                if (i % rowNum == rowNum - 1)
                {
                    x += width;
                    y = topMargine;
                }
                else
                {
                    y += height;
                }
            }
        }


        public void ButtonExecuteAction(object sender, TouchEventArgs e)
        {
            Button button = sender as Button;
            int index;
            int.TryParse(button.Text, out index);

            ContentsSceneChange(index - 1);
        }

        void ContentsSceneChange(int index)
        {
            if (transition != null)
            {
                transition.TransitionStopped += new EventHandler<EventArgs>(transition_TransitionStopped);
            }
            var currentContentsScene = UISystem.CurrentScene as ContentsScene;
            if (currentContentsScene != null)
            {
                currentContentsScene.BackgroundVisible = true;
            }

            if (0 <= index && index < currentSceneList.Count)
            {
                sceneIndex = index;
                currentSceneList[sceneIndex].RootWidget.AddChildLast(currentSceneList[sceneIndex].HeaderPanel);
                currentSceneList[sceneIndex].RootWidget.AddChildLast(currentSceneList[sceneIndex].ControlPanel);

                currentSceneList[sceneIndex].BackgroundVisible = (transition != null);

                UISystem.SetScene(currentSceneList[sceneIndex], transition);
            }
            else
            {
                sceneIndex = -1;
                this.BackgroundVisible = (transition != null);

                UISystem.SetScene(this, transition);
            }
        }

        void transition_TransitionStopped(object sender, EventArgs e)
        {
            var currentContentsScene = UISystem.CurrentScene as ContentsScene;
            if (currentContentsScene != null)
            {
                currentContentsScene.BackgroundVisible = false;
            }

            transition.TransitionStopped -= transition_TransitionStopped;
        }
    }

}
