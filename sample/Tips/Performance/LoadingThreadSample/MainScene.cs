using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
	public partial class MainScene : Scene
	{
		//	Constructor

		public MainScene()
		{
			InitializeWidget();

			Button_1.ButtonAction += (sender, e)=>{ if (OnButton != null) OnButton(1); };
			Button_2.ButtonAction += (sender, e)=>{ if (OnButton != null) OnButton(2); };
			Button_3.ButtonAction += (sender, e)=>{ if (OnButton != null) OnButton(3); };
		}

		//	UI events

		public Action<int> OnButton { get; set; }

		//	UI control

		public string Message {
			get { return Label_2.Text; }
			set { Label_2.Text = value; }
		}
		public float Progress {
			get { return ProgressBar_1.Progress; }
			set { ProgressBar_1.Progress = value; }
		}
		public string Result1 {
			get { return Label_3.Text; }
			set { Label_3.Text = value; }
		}
		public string Result2 {
			get { return Label_4.Text; }
			set { Label_4.Text = value; }
		}
		public string Result3 {
			get { return Label_5.Text; }
			set { Label_5.Text = value; }
		}

		bool loading;
	}
}
