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

namespace GameUI
{
    public partial class UIHelpPanel : Panel
    {
        public UIHelpPanel()
        {
            InitializeWidget();
            this.Clip = false;
        }


        public Button ReturnButton
        {
            get { return Button_return; }
            set { Button_return = value; }
        }

    }
}
