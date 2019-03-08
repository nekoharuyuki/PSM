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
    public partial class LiveSphereScene : ContentsScene
    {
        public LiveSphereScene()
        {
            InitializeWidget();

            this.Name = "LiveSphere";

            toggleSphere1.ButtonAction += new EventHandler<TouchEventArgs>(ToggleSphere1ExecuteAction);
            toggleSphere1.ToggleEnabled = true;
            toggleSphere2.ButtonAction += new EventHandler<TouchEventArgs>(ToggleSphere2ExecuteAction);
            toggleSphere2.ToggleEnabled = true;
            toggleSphere3.ButtonAction += new EventHandler<TouchEventArgs>(ToggleSphere3ExecuteAction);
            toggleSphere3.ToggleEnabled = true;
        }

        private void ToggleSphere1ExecuteAction(object sender, TouchEventArgs e)
        {
            toggleSphere1.Stop();
            if (toggleSphere1.FrontFace)
            {
                toggleSphere1.TurnCount = -1;
            }
            else
            {
                toggleSphere1.TurnCount = 1;
            }
            toggleSphere1.Start();
        }

        private void ToggleSphere2ExecuteAction(object sender, TouchEventArgs e)
        {
            toggleSphere2.Stop();
            if (toggleSphere2.FrontFace)
            {
                toggleSphere2.TurnCount = -1;
            }
            else
            {
                toggleSphere2.TurnCount = 1;
            }
            toggleSphere2.Start();
        }

        private void ToggleSphere3ExecuteAction(object sender, TouchEventArgs e)
        {
            toggleSphere3.Stop();
            if (toggleSphere3.FrontFace)
            {
                toggleSphere3.TurnCount = -1;
            }
            else
            {
                toggleSphere3.TurnCount = 1;
            }
            toggleSphere3.Start();
        }
    }
}
