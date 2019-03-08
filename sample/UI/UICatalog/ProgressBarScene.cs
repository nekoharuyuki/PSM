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
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class ProgressBarScene : ContentsScene
    {
        static float progress = 0.0f;

        public ProgressBarScene()
        {
            InitializeWidget();

            this.Name = "ProgressBar";
        }
        
        protected override void OnUpdate(float elapsedTime)
        {
            base.OnUpdate (elapsedTime);

            progress += 0.001f;
            if (progress > 1.0f)
            {
                progress = 0.0f;
            }
            normalProgressBar.Progress = progress;
            animationProgressBar.Progress = progress;
        }
    }
}
