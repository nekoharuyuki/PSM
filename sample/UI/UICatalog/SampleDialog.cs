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
    public partial class SampleDialog : Dialog
    {
        public SampleDialog()
        {
            InitializeWidget();
            button1.ButtonAction += new EventHandler<TouchEventArgs>(Button2ExecuteAction);
        }

        private void Button2ExecuteAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }
    }
}
