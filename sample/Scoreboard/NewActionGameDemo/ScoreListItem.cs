using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    public partial class ScoreListItem : ListPanelItem
    {
        public ScoreListItem()
        {
            InitializeWidget();
        }
		
		public void onUpdate(string rank, string name, string score)
		{
			this.rank.Text = rank;
			this.name.Text = name;	
			this.score.Text = score;
		}
    }
}
