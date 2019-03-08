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
    public class TransitionSettingDialog : Dialog
    {
        private const float LeftMargine = 30.0f;
        private const float topMargine = 20.0f;
        private const float checkboxHeight = 62.0f;
        private const float checkboxWidth = 300.0f;

        List<TransitionSettingPanel> transitionPanelList = new List<TransitionSettingPanel>();
        TextCheckBox[] transitionCheckBoxs;

        public Transition Transition
        {
            get { return transition; }
            set { transition = value; }
        }
        private Transition transition = null;


        public TransitionSettingDialog()
        {
            this.Width = UISystem.FramebufferWidth * 0.8f;
            this.Height = UISystem.FramebufferHeight * 0.95f;

            BunjeeJumpEffect showEffect = new BunjeeJumpEffect();
            showEffect.Elasticity = 0.3f;
            this.ShowEffect = showEffect;

            SlideOutEffect effect = new SlideOutEffect();
            effect.Time = 300;
            effect.MoveDirection = FourWayDirection.Up;
            this.HideEffect = effect;

            transitionPanelList.Add(new TransitionSettingPanel());

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[1].Transition = new PushTransition(300.0f,FourWayDirection.Left,PushTransitionInterpolator.EaseOutQuad);
            transitionPanelList[1].Name = "PushTransition";

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[2].Transition = new SlideTransition(300.0f,FourWayDirection.Left,MoveTarget.CurrentScene,SlideTransitionInterpolator.EaseOutQuad);
            transitionPanelList[2].Name = "SlideTransition";

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[3].Transition = new CrossFadeTransition(300.0f,CrossFadeTransitionInterpolator.EaseOutQuad);
            transitionPanelList[3].Name = "CrossFadeTransition";

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[4].Transition = new FlipBoardTransition();
            transitionPanelList[4].Name = "FlipBoardTransition";

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[5].Transition = new JumpFlipTransition();
            transitionPanelList[5].Name = "JumpFlipTransition";

            transitionPanelList.Add(new TransitionSettingPanel());
            transitionPanelList[6].Transition = new TiltDropTransition();
            transitionPanelList[6].Name = "TiltDropTransition";

            float x = LeftMargine;
            float y = topMargine;
            transitionCheckBoxs = new TextCheckBox[transitionPanelList.Count];
            for (int i = 0; i < transitionPanelList.Count; i++)
            {
                TextCheckBox checkbox = new TextCheckBox();
                this.AddChildLast(checkbox);
                checkbox.Width = checkboxWidth;
                checkbox.X = x;
                checkbox.Y = y;
                checkbox.Index = i;
                checkbox.Text = transitionPanelList[i].Name;
                checkbox.Style = CheckBoxStyle.RadioButton;
                checkbox.CheckedChange += new EventHandler<TouchEventArgs>(checkbox_CheckedChange);
                transitionCheckBoxs[i] = checkbox;
                y += checkboxHeight;
            }
            transitionCheckBoxs[0].Checked = true;

            Button button = new Button();
            button.SetPosition(this.Width - button.Width - LeftMargine, this.Height - button.Height - topMargine);
            button.Text = "OK";
            button.ButtonAction += new EventHandler<TouchEventArgs>(button_ButtonAction);
            this.AddChildLast(button);


        }

        void button_ButtonAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }

        bool isCheckUpdating = false;
        void checkbox_CheckedChange(object sender, TouchEventArgs e)
        {
            if (isCheckUpdating) return;
            isCheckUpdating = true;
            TextCheckBox check = sender as TextCheckBox;
            if (check.Checked)
            {
                foreach (var item in transitionCheckBoxs)
                {
                    if (item != check) item.Checked = false;
                }
                transition = transitionPanelList[check.Index].Transition;
            }
            else
            {
                check.Checked = true;
            }

            isCheckUpdating = false;

        }


    }

    public class TransitionSettingPanel : Panel
    {
        public TransitionSettingPanel()
        {
            this.Name = "None";
        }

        public Transition Transition = null;
    }

}
