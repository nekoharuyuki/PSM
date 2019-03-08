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

namespace Weather
{
    public partial class MainScene : Scene
    {
        // Scale of world map
        private const float MAP_SCALE = 2.0f;
        
        // Animation duration for show/hide detail panel
        private const float DETAIL_PANEL_ANIMATION_DURATION = 400.0f;
        
        // Animation duration for show/hide detail panel
        private const float NO_CONTROL_TIMEOUT = 2000.0f;
        
        // Epsilon
        private const float EPSILON = 0.01f;
        
        // City information list
        public List<CityInfo> cityInfoList = new List<CityInfo>();
        
        // News tips list
        public List<String> newsList = new List<String>();
        
        // City panel
        private PagePanel cityPagePanel;
        
        // JumpFlipEffect for cityPagePanel
        private JumpFlipEffect effect;

        // News ticker
        private Ticker newsTicker;
        
        // Current news id
        private int curNewsId = 0;
        
        // Total time of no control
        private float noControlTime = 0.0f;
        
        // Move animation
        private float moveFromX = 0.0f;
        private float moveFromY = 0.0f;
        private float moveToX = 0.0f;
        private float moveToY = 0.0f;
        private float totalAnmTime = 0.0f;
        private float moveAnmTime = 0.0f;
        private int moveTargetCityId = 0;
        
        // Random
//        private Random rnd = new Random();
        
        public MainScene()
        {
            InitializeWidget();
            
            float scaleX = MAP_SCALE * this.RootWidget.Width / this.DesignWidth;
            float scaleY = MAP_SCALE * this.RootWidget.Height / this.DesignHeight; 

            // Init city info (Name, X, Y, WeatherID, Low, High, Prec)
            cityInfoList.Add(new CityInfo(0,     "Paris",   50 * scaleX,    80 * scaleY, 1, 18, 29,  0));
            cityInfoList.Add(new CityInfo(1,  "CapeTown",   85 * scaleX,   290 * scaleY, 1,  7, 20, 10));
            cityInfoList.Add(new CityInfo(2,     "Cairo",  110 * scaleX,   140 * scaleY, 1, 26, 36,  0));
            cityInfoList.Add(new CityInfo(3,    "Moscow",  150 * scaleX,    65 * scaleY, 3, 10, 19, 80));
            cityInfoList.Add(new CityInfo(4,  "NewDelhi",  225 * scaleX,   140 * scaleY, 2, 22, 35, 50));
            cityInfoList.Add(new CityInfo(5,   "Beijing",  305 * scaleX,   105 * scaleY, 1, 16, 30, 10));
            cityInfoList.Add(new CityInfo(6,     "Tokyo",  370 * scaleX,   115 * scaleY, 1, 23, 31,  0));
            cityInfoList.Add(new CityInfo(7,    "Sydney",  395 * scaleX,   290 * scaleY, 1, 12, 22, 20));
            cityInfoList.Add(new CityInfo(8, "Anchorage",  535 * scaleX,    45 * scaleY, 3,  8, 16, 80));
            cityInfoList.Add(new CityInfo(9,"LosAngeles",  615 * scaleX,   110 * scaleY, 1, 17, 26,  0));
            cityInfoList.Add(new CityInfo(10,  "NewYork",  715 * scaleX,    90 * scaleY, 1, 16, 28, 20));
            cityInfoList.Add(new CityInfo(11,     "Lima",  715 * scaleX,   240 * scaleY, 2, 11, 22, 40));
            cityInfoList.Add(new CityInfo(12, "SaoPaulo",  785 * scaleX,   270 * scaleY, 3, 12, 18, 80));
            
            // Init news info
            newsList.Add("Reality Fighters™ : Put yourself in the battle with Reality Fighters!");
            newsList.Add("UNIT 13™ : Jump into the action with Unit 13 for PlayStation Vita System.");
            newsList.Add("LittleBigPlanet™ : Join Sackboy in a completely new adventure, peril and excitement!");
            
            // Init date (Tokyo)
            DateTime now = DateTime.UtcNow + new TimeSpan(9,0,0);
            String dateString = now.ToString("MMMMM d, yyyy (ddd)");
            dateLabel.Text = dateString;
            // Init world panel
            scrollPanel.PanelWidth = this.RootWidget.Width * MAP_SCALE;
            scrollPanel.PanelHeight = this.RootWidget.Height * MAP_SCALE;
            scrollPanel.ScrollBarVisibility = ScrollBarVisibility.Invisible;
            scrollPanel.TouchResponse = false;
            scrollPanel.TouchEventReceived += new EventHandler<TouchEventArgs>(LiveScrollTouchAction);
            scrollPanel.AddChildLast(new WorldPanel(scrollPanel.PanelWidth, scrollPanel.PanelHeight, cityInfoList, ShowDetailAction));

            // Add city page panel
            cityPagePanel = new PagePanel();
            cityPagePanel.SetPosition(0.0f, this.RootWidget.Height);
            cityPagePanel.SetSize(this.RootWidget.Width, this.RootWidget.Height);
            Matrix4 scaleMat = Matrix4.Scale(new Vector3(0.01f, 0.01f, 0.01f));
            Matrix4 baseMat = cityPagePanel.Transform3D;
            Matrix4 mat;
            Matrix4.Multiply(ref baseMat, ref scaleMat, out mat);
            cityPagePanel.Transform3D = mat;
            cityPagePanel.Visible = false;
            this.RootWidget.AddChildLast(cityPagePanel);

            // Add city detail panel
            for(int i = 0; i < cityInfoList.Count; i++)
            {
                CityInfo info = cityInfoList[i];
                cityPagePanel.AddPage();
                Panel panel = cityPagePanel.GetPage(i);
                
                PrecipitationPanel precPanel= new PrecipitationPanel(info,
                                            new EventHandler<TouchEventArgs>(HideDetailAction),
                                            new EventHandler<TapEventArgs>(DetailJumpFlipAction));
                precPanel.Visible = false;
                precPanel.SetSize(this.RootWidget.Width, this.RootWidget.Height);
                precPanel.Date = dateString;
                panel.AddChildLast(precPanel);
                
                TemperaturePanel tempPanel= new TemperaturePanel(info,
                                            new EventHandler<TouchEventArgs>(HideDetailAction),
                                            new EventHandler<TapEventArgs>(DetailJumpFlipAction));
                tempPanel.Visible = true;
                tempPanel.SetSize(this.RootWidget.Width, this.RootWidget.Height);
                tempPanel.Date = dateString;
                panel.AddChildLast(tempPanel);
            }
            
            // Set panel background
            datePanel.Visible = false;
            newsPanel.Visible = false;
            
            // Init ticker
            newsTicker = new Ticker();
            newsTicker.SetPosition(0.0f, 0.0f);

            newsTicker.SetOnScrollEnd(OnNewsTickerEnd);
            newsTicker.SetText(newsList[curNewsId]);
            newsTickerPanel.AddChildLast(newsTicker);
            
//            moveTargetCityId = rnd.Next(cityInfoList.Count);
            moveTargetCityId = 0;
            setMoveTargetCity(moveTargetCityId);
        }

        protected override void OnShown()
        {
            // News panel slide in
            SlideInEffect inEffect;
            inEffect = new SlideInEffect(newsPanel,
                                            DETAIL_PANEL_ANIMATION_DURATION * 2,
                                            FourWayDirection.Up,
                                            SlideInEffectInterpolator.EaseOutQuad);
            inEffect.EffectStopped += OnHideAnimationEnd;
            inEffect.Start();
            
            // Date panel slide in
            SlideInEffect inEffectDate;
            inEffectDate = new SlideInEffect(datePanel,
                                            DETAIL_PANEL_ANIMATION_DURATION * 2,
                                            FourWayDirection.Down,
                                            SlideInEffectInterpolator.EaseOutQuad);
            inEffectDate.Start();

            // Set ticker size
            newsTicker.Width = newsTickerPanel.Width;
            newsTicker.Height = newsTickerPanel.Height;
        }
        
        protected override void OnUpdate(float elapsedTime)
        {
            noControlTime += elapsedTime;

            if (!cityPagePanel.Visible && noControlTime < NO_CONTROL_TIMEOUT)
            {
                totalAnmTime = 0.0f;
                return;
            }
            
            if (totalAnmTime < EPSILON)
            {
                moveFromX = scrollPanel.PanelX;
                moveFromY = scrollPanel.PanelY;
                float dist = Math.Abs(moveToX - moveFromX) + Math.Abs(moveToY - moveFromY);
                moveAnmTime = dist * (cityPagePanel.Visible ? 2.0f : 10.0f); 
            }

            totalAnmTime += elapsedTime;

            scrollPanel.PanelX = Lerp(moveFromX, moveToX, totalAnmTime / moveAnmTime);
            scrollPanel.PanelY = Lerp(moveFromY, moveToY, totalAnmTime / moveAnmTime);
            
            if (Math.Abs(scrollPanel.PanelX - moveToX) < EPSILON &&
                    Math.Abs(scrollPanel.PanelY - moveToY) < EPSILON)
            {
                if (!cityPagePanel.Visible)
                {
                    moveTargetCityId++;
                    if (moveTargetCityId >= cityInfoList.Count)
                    {
                        moveTargetCityId = 0;
                    }
                }
                else
                {
                    moveTargetCityId = cityPagePanel.CurrentPageIndex;
                }
                setMoveTargetCity(moveTargetCityId);
                totalAnmTime = 0.0f;
            }
        }
        
        private void setMoveTargetCity(int cityId)
        {
            CityInfo info = cityInfoList[cityId];
            moveToX = -info.locationX + scrollPanel.Width / 2 - 100.0f / 2;
            moveToY = -info.locationY + scrollPanel.Height / 2 - 100.0f / 2;
            
            if (moveToX > 0.0f)
            {
                moveToX = 0.0f;
            }
            else if (moveToX < -(scrollPanel.PanelWidth - scrollPanel.Width))
            {
                moveToX = -(scrollPanel.PanelWidth - scrollPanel.Width);
            }
            
            if (moveToY > 0.0f)
            {
                moveToY = 0.0f;
            }
            else if (moveToY < -(scrollPanel.PanelHeight - scrollPanel.Height))
            {
                moveToY = -(scrollPanel.PanelHeight - scrollPanel.Height);
            }
        }
  
        private void LiveScrollTouchAction(object sender, TouchEventArgs e)
        {
            noControlTime = 0.0f;
        }
        
        private void ShowDetailAction(object sender, TouchEventArgs e)
        {
            if (sender is CityButton)
            {
                // Get city information
                CityButton city = (CityButton)sender;
                cityPagePanel.PivotType = PivotType.MiddleCenter;
                float cityCenterX = city.X + scrollPanel.PanelX + city.Width / 2.0f;
                float cityCenterY = city.Y + scrollPanel.PanelY + city.Height / 2.0f;
                cityPagePanel.SetPosition(cityCenterX, cityCenterY);
                cityPagePanel.CurrentPageIndex = city.cityInfo.cityId;
                cityPagePanel.Visible = true;

                // Detail panel effect (Zoom, Move, Fade)
                ZoomEffect zoomEffect;
                zoomEffect = new ZoomEffect(cityPagePanel,
                                                DETAIL_PANEL_ANIMATION_DURATION, 
                                                1.0f, 
                                                ZoomEffectInterpolator.EaseOutQuad);
                zoomEffect.Start();

                MoveEffect moveEffect;
                moveEffect = new MoveEffect(cityPagePanel,
                                                DETAIL_PANEL_ANIMATION_DURATION, 
                                                this.RootWidget.Width / 2.0f, 
                                                this.RootWidget.Height / 2.0f, 
                                                MoveEffectInterpolator.EaseOutQuad);
                moveEffect.Start();

                FadeInEffect fadeEffect = new FadeInEffect(cityPagePanel, 
                                                            0.0f, 
                                                            FadeInEffectInterpolator.EaseOutQuad);
                fadeEffect.Start();

                // News panel slide out
                SlideOutEffect outEffect;
                outEffect = new SlideOutEffect(newsPanel,
                                                DETAIL_PANEL_ANIMATION_DURATION,
                                                FourWayDirection.Down,
                                                SlideOutEffectInterpolator.EaseOutQuad);
                outEffect.EffectStopped += OnShowAnimationEnd;
                outEffect.Start();
                
                // Date panel slide out
                SlideOutEffect outEffectDate;
                outEffectDate = new SlideOutEffect(datePanel,
                                                DETAIL_PANEL_ANIMATION_DURATION,
                                                FourWayDirection.Up,
                                                SlideOutEffectInterpolator.EaseOutQuad);
                outEffectDate.Start();
                
                // Control touch response
                scrollPanel.TouchResponse = false;

                setMoveTargetCity(city.cityInfo.cityId);
            }
        }
        
        private void HideDetailAction(object sender, TouchEventArgs e)
        {
            // Get city information
            CityInfo cityInfo = cityInfoList[cityPagePanel.CurrentPageIndex];
            float cityCenterX = cityInfo.locationX + scrollPanel.PanelX + 100.0f / 2.0f;
            float cityCenterY = cityInfo.locationY + scrollPanel.PanelY + 100.0f / 2.0f;
            
            // Detail panel slide out
            ZoomEffect zoomEffect;
            zoomEffect = new ZoomEffect(cityPagePanel,
                                            DETAIL_PANEL_ANIMATION_DURATION, 
                                            0.01f, 
                                            ZoomEffectInterpolator.EaseOutQuad);
            zoomEffect.Start();

            MoveEffect moveEffect;
            moveEffect = new MoveEffect(cityPagePanel, 
                                            DETAIL_PANEL_ANIMATION_DURATION, 
                                            cityCenterX, 
                                            cityCenterY, 
                                            MoveEffectInterpolator.EaseOutQuad);
            moveEffect.Start();

            FadeOutEffect fadeEffect = new FadeOutEffect(cityPagePanel, 
                                                            DETAIL_PANEL_ANIMATION_DURATION, 
                                                            FadeOutEffectInterpolator.EaseOutQuad);
            fadeEffect.Start();
            
            // News panel slide in
            SlideInEffect inEffect;
            inEffect = new SlideInEffect(newsPanel,
                                            DETAIL_PANEL_ANIMATION_DURATION,
                                            FourWayDirection.Up,
                                            SlideInEffectInterpolator.EaseOutQuad);
            inEffect.EffectStopped += OnHideAnimationEnd;
            inEffect.Start();
            
            // Date panel slide in
            SlideInEffect inEffectDate;
            inEffectDate = new SlideInEffect(datePanel,
                                            DETAIL_PANEL_ANIMATION_DURATION,
                                            FourWayDirection.Down,
                                            SlideInEffectInterpolator.EaseOutQuad);
            inEffectDate.Start();
            
            // Control touch response
            cityPagePanel.TouchResponse = false;
            totalAnmTime = 0.0f;
            noControlTime = 0.0f;
        }
        
        private void DetailJumpFlipAction(object sender, TapEventArgs e)
        {
            if (effect == null || !effect.Playing)
            {
                for (int i = 0; i < cityPagePanel.PageCount; i++)
                {
                    Panel prevPanel = null;
                    Panel nextPanel = null;
                    IEnumerator<Widget> myEnumerator = cityPagePanel.GetPage(i).Children.GetEnumerator();
                    while(myEnumerator.MoveNext())
                    {
                        Panel panel = (Panel)myEnumerator.Current;
                        if (panel.Visible && prevPanel == null)
                        {
                            prevPanel = panel;
                        }
                        else
                        {
                            nextPanel = panel;
                        }
                    }
                    effect = new JumpFlipEffect(prevPanel, nextPanel);
                    effect.Revolution = prevPanel is TemperaturePanel ? 1 : -1;
                    effect.RotationAxis = JumpFlipEffectAxis.Y;
                    effect.Start();
                }
            }
        }

        private void OnNewsTickerEnd()
        {
            curNewsId++;
            if (curNewsId >= newsList.Count)
            {
                curNewsId = 0;
            }
            newsTicker.SetText(newsList[curNewsId]);
        }
        
        private void OnHideAnimationEnd(object sender, EventArgs e)
        {
            scrollPanel.TouchResponse = true;
            cityPagePanel.Visible = false;
        }
        
        private void OnShowAnimationEnd(object sender, EventArgs e)
        {
            cityPagePanel.TouchResponse = true;
        }

        float Lerp(float from, float to, float ratio)
        {
            if (ratio < Single.Epsilon)
            {
                return from;
            }
            else if (ratio > 1.0f - Single.Epsilon)
            {
                return to;
            }
            else
            {
                return from * (1.0f - ratio) + to * ratio;
            }
        }

    }
}
