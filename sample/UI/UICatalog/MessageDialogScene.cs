/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class MessageDialogScene : ContentsScene
    {
        private MessageDialog dialog;

        public MessageDialogScene()
        {
            InitializeWidget();

            this.Name = "MessageDialog";

            // MessageDialog
            dialog = new MessageDialog();
            dialog.Title = "Title";
            dialog.Message = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 "
                           + "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 12345678890 ";
            dialog.ButtonPressed += new EventHandler<MessageDialogButtonEventArgs>(ButtonDownHandler);

            // Button1
            button1.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction1);

            // Button2
            button2.ButtonAction += new EventHandler<TouchEventArgs>(ButtonExecuteAction2);
        }

        private void ButtonExecuteAction1(object sender, TouchEventArgs e)
        {
            dialog.Style = MessageDialogStyle.Ok;
            dialog.Show();
        }

        private void ButtonExecuteAction2(object sender, TouchEventArgs e)
        {
            dialog.Style = MessageDialogStyle.OkCancel;
            dialog.Show();
        }

        private void ButtonDownHandler(object sender, MessageDialogButtonEventArgs e)
        {
            label1.Text = "Result: " + e.Result.ToString(); 
        }

    }
}
