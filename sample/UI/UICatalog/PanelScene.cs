﻿/* PlayStation(R)Mobile SDK 1.21.01
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
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class PanelScene : ContentsScene
    {
        public PanelScene()
        {
            InitializeWidget();

            this.Name = "Panel";

            // Panel (Cliping off)
            clipOffPanel.Clip = false;
            
            // Panel (Cliping on)
            clipOnPanel.Clip = true;
        }
    }
}
