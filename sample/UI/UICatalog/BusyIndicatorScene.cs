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
    public partial class BusyIndicatorScene : ContentsScene
    {
        public BusyIndicatorScene()
        {
            InitializeWidget();

            this.Name = "BusyIndicator";

            // Start Button
            startButton.ButtonAction += new EventHandler<TouchEventArgs>(OnStartButtonExecuteAction);

            // Stop Button
            stopButton.ButtonAction += new EventHandler<TouchEventArgs>(OnStopButtonExecuteAction);
            stopButton.Enabled = false;
        }

        public void OnStartButtonExecuteAction(object sender, TouchEventArgs e)
        {
            startButton.Enabled = false;
            indicator.Start();
            stopButton.Enabled = true;
        }

        public void OnStopButtonExecuteAction(object sender, TouchEventArgs e)
        {
            stopButton.Enabled = false;
            indicator.Stop();
            startButton.Enabled = true;
        }
    }
}
