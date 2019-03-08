/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample
{
    public abstract class SceneBase : Scene
    {
        public bool m_IsDialogShown = false;
        public bool IsDialogShown
        {
            get
            {
                return this.m_IsDialogShown;
            }
            set
            {
                if (this.m_IsDialogShown == value) { return; }
                
                this.m_IsDialogShown = value;
                
                if (value)
                {
                    this.OnDialogShown();
                }
                else
                {
                    this.OnDialogHidden();
                }
            }
        }
        
        protected override void OnShowing()
        {
            base.OnShowing();
            
            var application = Application.Current;
            var screenOrientationManager = application.ScreenOrientationManager;
            
            this.ScreenOrientation = screenOrientationManager.CurrentUIOrientation;
            screenOrientationManager.ScreenOrientationChanged += this.OnScreenOrientationChanged;
        }

        protected override void OnHiding()
        {
            var application = Application.Current;
            var screenOrientationManager = application.ScreenOrientationManager;
            
            screenOrientationManager.ScreenOrientationChanged -= this.OnScreenOrientationChanged;
            
            base.OnHiding();
        }
        
		public bool IsScreenOrientationChanging{get; set;}
		
        protected virtual void OnScreenOrientationChanged(object sender, ScreenOrientationChangedEventArgs e)
        {
            if (this.IsDialogShown) { return; }
			this.IsScreenOrientationChanging = true;
            
			try
			{
            	this.ScreenOrientation = e.ScreenOrientation;
				UISystem.SetScene(this);
			}
			finally
			{
				this.IsScreenOrientationChanging = false;
			}
        }
        
        protected virtual void OnDialogShown()
        { }
        
        protected virtual void OnDialogHidden()
        {
            var application = Application.Current;
            var screenOrientationManager = application.ScreenOrientationManager;
            
            this.ScreenOrientation = screenOrientationManager.CurrentUIOrientation;
        }
    }
}

