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

namespace RssApp
{
    public partial class AddFeedDialog : Dialog
    {
        public AddFeedDialog()
        {
            InitializeWidget();

            // Button Event
            this.addFeedCancelButton.ButtonAction += new EventHandler<TouchEventArgs>(AddDialogCancelButtonPressed);
            this.addFeedOkButton.ButtonAction += new EventHandler<TouchEventArgs>(AddDialogOkButtonPressed);
        }

        private void AddDialogOkButtonPressed(object sender, TouchEventArgs e)
        {
            FadeOutEffect addDialogFadeOutEffect = new FadeOutEffect();
            addDialogFadeOutEffect.Time = 0.0f;
            this.Hide(addDialogFadeOutEffect);

            System.Console.WriteLine("AddDialogEditableText: " + this.addFeedEditableText.Text);
        }

        private void AddDialogCancelButtonPressed(object sender, TouchEventArgs e)
        {
            FadeOutEffect addDialogFadeOutEffect = new FadeOutEffect();
            addDialogFadeOutEffect.Time = 0.0f;
            this.Hide(addDialogFadeOutEffect);
        }

    }
}
