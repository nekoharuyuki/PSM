/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    public partial class MainScene : Scene
    {
        // News Ticker
        private Ticker newsTicker;
        // News List
        private List<string> newsList = new List<string>();
        // Current news id
        private int curNewsId = 0;
        

        public MainScene()
        {
            InitializeWidget();

            // init news list
            newsList.Add("PlayStation Fans: Watch E3 Online Right Here");
            newsList.Add("GT Academy: Silverstone, Here We Come!");
            newsList.Add("Resistance Dual Pack Deploying This July");
            newsList.Add("PlayStation Move sharp shooter Origins ? Part One");
            
            // init news ticker
            newsTicker = new Ticker();
            newsTicker.SetSize(newsTickerPanel.Width, newsTickerPanel.Height);
            newsTicker.SetPosition(0.0f, 0.0f);
            newsTicker.SetOnScrollEnd(OnNewsTickerEnd);
            newsTicker.SetText(newsList[curNewsId]);
            newsTicker.Anchors = Anchors.None;
            newsTickerPanel.AddChildLast(newsTicker);
            
            // init make calendar button
            Button_1.ButtonAction += new EventHandler<TouchEventArgs>(Button1ExecuteAction);
            Button_2.ButtonAction += new EventHandler<TouchEventArgs>(Button2ExecuteAction);
        }
        
        private void Button1ExecuteAction(object sender, TouchEventArgs e)
        {
            var CMScene = new CalendarMakerScene();
            CMScene.ReturnMainScene = returnMainScene;
            UISystem.SetScene(CMScene);
        }
        
        private void Button2ExecuteAction(object sender, TouchEventArgs e)
        {
            var WVScene = new WallpaperViewerScene();
            WVScene.ReturnMainScene = returnMainScene;
            UISystem.SetScene(WVScene);
        }
        
        private void OnNewsTickerEnd()
        {
            newsTicker.SetText(newsList[++curNewsId%newsList.Count]);
        }
  
        private void StartSlideShow()
        {
            FlipBoardEffect effect1 = new FlipBoardEffect(slideShowImage, slideShowbackImage);
            effect1.EffectStopped += new EventHandler<EventArgs>(OnStopFlipEffect);
            effect1.Start();
        }
        
        private void OnStopFlipEffect(object sender, EventArgs e)
        {
            ImageBox tmp;

            tmp = slideShowImage;
            slideShowImage = slideShowbackImage;
            slideShowbackImage = tmp;
        }

        private void returnMainScene()
        {
            var transition = new PushTransition(
                400,
                FourWayDirection.Right,
                PushTransitionInterpolator.EaseOutQuad);

            // Release the previous scene memory
            transition.TransitionStopped += (sender, e) => GC.Collect();

            UISystem.SetScene(this, transition);
        }
    }
}
