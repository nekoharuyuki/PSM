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
    public partial class PagePanelScene : ContentsScene
    {
        public PagePanelScene()
        {
            InitializeWidget();

            this.Name = "PagePanel";
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            if (pagePanel.PageCount <= 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    pagePanel.AddPage();
                    Panel panel = pagePanel.GetPage(i);
                    if (i == 0)
                    {
                        panel.BackgroundColor = new UIColor(1.0f, 0.0f, 0.0f, 0.5f);
                    }
                    else if (i == 1)
                    {
                        panel.BackgroundColor = new UIColor(0.0f, 1.0f, 0.0f, 0.5f);
                    }
                    else if (i == 2)
                    {
                        panel.BackgroundColor = new UIColor(0.0f, 0.0f, 1.0f, 0.5f);
                    }
                    Label label = new Label();
                    panel.AddChildLast(label);
                    label.Width = panel.Width;
                    label.Height = panel.Height;
                    label.VerticalAlignment = VerticalAlignment.Middle;
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    label.Text = "Page " + i;
                }
            }
        }
    }
}
