using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    public partial class ExplanationDialog : Dialog
    {

        public ExplanationDialog()
            : base(null, null)
        {
            InitializeWidget();

            this.HideOnTouchOutside = true;
            this.CustomBackgroundImage = null;

            this.ShowEffect = new FadeInEffect(null, 300, FadeInEffectInterpolator.EaseOutQuad);
            this.HideEffect = new FadeOutEffect(null, 200, FadeOutEffectInterpolator.EaseOutQuad);

            this.Width = UISystem.FramebufferWidth;
            this.Height = UISystem.FramebufferHeight;

            closeButton.ButtonAction += new EventHandler<TouchEventArgs>(CloseButtonExecuteAction);

        }

        private void CloseButtonExecuteAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }
        
        public void ImageSelectionGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID12);
            unvisibleBarImage();
            bar1Image.Visible = true;
            this.Show();
        }
        public void ImageSizeGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID13);
            unvisibleBarImage();
            bar2Image.Visible = true;
            this.Show();
        }
        
        public void CalendarTypeGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID14);
            unvisibleBarImage();
            bar3Image.Visible = true;
            this.Show();
        }

        public void CalendarPositionGuide()
        {
            guideLabel.Text = UIStringTable.Get(UIStringID.RESID15);
            unvisibleBarImage();
            bar4Image.Visible = true;
            this.Show();
        }

        private void unvisibleBarImage()
        {
            bar1Image.Visible = false;
            bar2Image.Visible = false;
            bar3Image.Visible = false;
            bar4Image.Visible = false;
        }

    }
}
