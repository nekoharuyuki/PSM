/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    public partial class CompleteDialog : Dialog
    {
        private ShowSavePanelAction showSavePanelAction;
        
        public CompleteDialog(ShowSavePanelAction action)
        {
            InitializeWidget();
            
            Button_1.ButtonAction += new EventHandler<TouchEventArgs>(SaveButtonExecuteAction);
            Button_2.ButtonAction += new EventHandler<TouchEventArgs>(CloseButtonExecuteAction);
            
            showSavePanelAction = action;
        }
        
        private void SaveButtonExecuteAction(object sender, TouchEventArgs e)
        {
            this.Hide();
            showSavePanelAction();
        }
        
        private void CloseButtonExecuteAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }
        
    }
}
