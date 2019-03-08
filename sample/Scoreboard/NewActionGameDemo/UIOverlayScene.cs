using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    public partial class UIOverlayScene : Scene
    {
		public ScoreboardDialog scoreboardDialog { get; private set; }
		
        public UIOverlayScene()
        {
            InitializeWidget();
			
			scoreboardDialog = new ScoreboardDialog();
			
			scoreboardButton.ButtonAction += new EventHandler<TouchEventArgs>(scoreboardButtonAction);
        }
		
		protected override void OnUpdate(float elapsedTime)
		{
			scoreboardDialog.CheckGetScoreboard();
		}
		
		void scoreboardButtonAction(object sender, TouchEventArgs e)
        {
            scoreboardDialog.SetSize(this.RootWidget.Width, this.RootWidget.Height);
			scoreboardDialog.SetTimer();
            scoreboardDialog.Show();
        }
    }
}
