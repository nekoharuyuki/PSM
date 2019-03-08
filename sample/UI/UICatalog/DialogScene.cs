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

    public partial class DialogScene : ContentsScene
    {
        private Dialog dialog;

        public DialogScene()
        {
            InitializeWidget();

            this.Name = "Dialog";

            // Button (Show)
            button1.ButtonAction += new EventHandler<TouchEventArgs>(Button1ExecuteAction);

            // Dialog
            dialog = new SampleDialog();
        }

        private void Button1ExecuteAction(object sender, TouchEventArgs e)
        {
            dialog.Width = this.RootWidget.Width * 0.8f;
            dialog.Height = this.RootWidget.Height * 0.8f;
            dialog.Show();
        }
    }
}
