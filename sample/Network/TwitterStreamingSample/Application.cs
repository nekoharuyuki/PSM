/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;	
using Sce.PlayStation.HighLevel.UI;
using TweetSharp;

namespace TwitterStreamingSample
{
	public class Application
	{
		public static readonly string TWITTER_CONSUMER_KEY = "";
        public static readonly string TWITTER_CONSUMER_SECRET = "";
		
        public static Application Current { get; private set; }
		public GraphicsContext GraphicsContext{get; set;}
		public Settings Settings{get; private set;}
		public Dispatcher Dispatcher{get; set;}
		public TwitterService TwitterService{get; private set;}
		public ScreenOrientationManager ScreenOrientationManager { get; private set; }
		public bool IsAlive{get; set;}
		public TwitterUserProfileImageCacheStore TwitterUserProfileImageCacheStore{get; private set;}

		public Application()
		{
			Application.Current = this;
		}
		
		public void Initialize()
		{
            this.IsAlive = true;
            
            this.GraphicsContext = new GraphicsContext();
			UISystem.Initialize(this.GraphicsContext);
			
			this.Settings = new Settings();
			this.Settings.Load();
            
            this.Dispatcher = new Dispatcher();
            this.ScreenOrientationManager = new ScreenOrientationManager();
            
			this.TwitterService = 
				new TwitterService(
					Application.TWITTER_CONSUMER_KEY,
					Application.TWITTER_CONSUMER_SECRET);
			this.TwitterUserProfileImageCacheStore = new TwitterUserProfileImageCacheStore();
			var scene = new TwitterStreamingSample.LoginScene();
			UISystem.SetScene(scene);
		}

		public void Update()
		{
			var touchDataList = Touch.GetData(0);
			var gamePadData = GamePad.GetData(0);
			var motionData = Motion.GetData(0);

			ScreenOrientationManager.Update(motionData);
			
			if((gamePadData.Buttons == GamePadButtons.Down && gamePadData.ButtonsPrev == GamePadButtons.Down) ||
			   (gamePadData.Buttons == GamePadButtons.Up && gamePadData.ButtonsPrev == GamePadButtons.Up) ||
			   (gamePadData.Buttons == GamePadButtons.Right && gamePadData.ButtonsPrev == GamePadButtons.Right) ||
			   (gamePadData.Buttons == GamePadButtons.Left && gamePadData.ButtonsPrev == GamePadButtons.Left))
			{
				return;
			}
			UISystem.Update(touchDataList, ref gamePadData, ref motionData);
			this.Dispatcher.DoEvents();
		}

		public void Render()
		{
			this.GraphicsContext.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			this.GraphicsContext.Clear();
			
			UISystem.Render();

			this.GraphicsContext.SwapBuffers();
		}
        
		public void Run()
		{
			this.Initialize();

			while (this.IsAlive)
			{
				SystemEvents.CheckEvents();
				this.Update();
				this.Render();
			}
        }
    }
				
		
}
