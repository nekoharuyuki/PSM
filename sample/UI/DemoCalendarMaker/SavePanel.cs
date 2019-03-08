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
    public partial class SavePanel : Panel
    {

        public SavePanel()
        {
            InitializeWidget();
            this.Clip = false;
        }
        
        public void Start()
        {
            FadeInEffect.CreateAndStart(this, 100, FadeInEffectInterpolator.Linear);

            SavingAnimation();
        }
        
        private void SavingAnimation()
        {
            Label_1.Text = "Now Saving ...";
            BusyIndicator_1.Start();
            DelayedExecutor.CreateAndStart(3000, ShowInfo);
        }
        private void ShowInfo()
        {
            Label_1.Text = "Completed !";
            BusyIndicator_1.Stop();
            DelayedExecutor.CreateAndStart(1500, SavingEnd);
        }
        private void SavingEnd()
        {
            FadeOutEffect.CreateAndStart(this, 500, FadeOutEffectInterpolator.Linear);
        }
    }
}
