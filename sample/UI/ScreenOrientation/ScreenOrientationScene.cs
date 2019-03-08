using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScreenOrientationSample
{
    public partial class ScreenOrientationScene : Scene
    {
        public ScreenOrientationScene()
        {
            InitializeWidget();
        }

        public event EventHandler<TouchEventArgs> BackButtonAction
        {
            add { ButtonBack.ButtonAction += value; }
            remove { ButtonBack.ButtonAction -= value; }
        }

        public string TitleText
        {
            get { return this.LabelTitle.Text; }
            set { this.LabelTitle.Text = value; }
        }
    }
}
