/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    public partial class UIOverlayScene : Scene
    {
        public UIOverlayScene()
        {
            InitializeWidget();

            Button_preference.ButtonAction += new EventHandler<TouchEventArgs>(Button_preference_ButtonAction);
        }

        void Button_preference_ButtonAction(object sender, TouchEventArgs e)
        {
            Globals.GameState = GameState.Pause;

            var menu = new UIMenuDialog();
            menu.SetSize(this.RootWidget.Width, this.RootWidget.Height);
            menu.ShowEffect = new FadeInEffect(null, 300, FadeInEffectInterpolator.EaseOutQuad);
            menu.Show();
        }
        
    }
}
