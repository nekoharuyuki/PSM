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
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class EditableTextScene : ContentsScene
    {
        public EditableTextScene()
        {
            InitializeWidget();

            this.Name = "EditableText";

            // Button clear
            buttonClear.ButtonAction += new EventHandler<TouchEventArgs>(ButtonClearExecuteAction);
        }

        private void ButtonExecuteAction(object sender, TouchEventArgs e)
        {
            Button button = (Button)sender;
            edit1.Text += button.Text;
        }

        private void ButtonClearExecuteAction(object sender, TouchEventArgs e)
        {
            edit1.Text = "";
        }
    }
}
