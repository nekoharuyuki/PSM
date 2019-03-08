using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    public partial class SamplePagePanelInnerPanel : Scene
    {
        public SamplePagePanelInnerPanel()
        {
            InitializeWidget();
        }
        protected override void OnShown()
        {
            this.StartDefaultEffect();
        }
    }
}
