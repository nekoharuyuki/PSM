/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class ButtonScene : ContentsScene
    {
        public ButtonScene()
        {
            InitializeWidget();

            this.Name = "Button";

            // Button 1
            button1.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction);

            // Button 2
            button2.Text = "Button2";
            button2.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction);

            // Button 3
            button3.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction);

            // Button 4
            button4.ButtonAction += new EventHandler<TouchEventArgs>(ChangeEnableButtonExecuteAction);

            // Button 5
            enableButton.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction);
            ChangeEnableButton();
        }

        private void ButtonExecuteAction(object sender, TouchEventArgs e)
        {
            UpdateExecuteButton((Button)sender);
        }

        private void ChangeEnableButtonExecuteAction(object sender, TouchEventArgs e)
        {
            ChangeEnableButton();
            UpdateExecuteButton((Button)sender);
        }

        private void ChangeEnableButton()
        {
            enableButton.Enabled = !enableButton.Enabled;
            if (enableButton.Enabled)
            {
                enableButton.Text = "Enable";
            }
            else
            {
                enableButton.Text = "Disable";
            }
        }

        private void UpdateExecuteButton(Button button)
        {
            labelExecuteButton.Text = button.Text + " is Executed.";
        }
    }
}
