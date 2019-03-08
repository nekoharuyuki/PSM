/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace OverlaySample
{
    public partial class UIOverlayScene : Scene
    {
        public UIOverlayScene()
        {
            InitializeWidget();


            SettingDialog = new UISettingDialog();

            Button_preference.ButtonAction += new EventHandler<TouchEventArgs>(Button_preference_ButtonAction);
        }

        void Button_preference_ButtonAction(object sender, TouchEventArgs e)
        {
            SettingDialog.SetSize(this.RootWidget.Width * 0.8f, this.RootWidget.Height * 0.8f);
            SettingDialog.Show();
        }
        
        public UISettingDialog SettingDialog  {
            get;
            private set;
        }
    }
}
