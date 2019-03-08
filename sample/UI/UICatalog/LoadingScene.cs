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
using System.Diagnostics;

namespace UICatalog
{
    public partial class LoadingScene : Scene
    {
        readonly TimeSpan minProcTime = new TimeSpan(0, 0, 0, 0, 30);
        List<Action> loadingProc;
        Stopwatch stopwatch;
        TopScene topScene;
        int loadingIndex = 0;
        
        public LoadingScene()
        {
            stopwatch = Stopwatch.StartNew();
            
            InitializeWidget();
            
            //var start = stopwatch.Elapsed;
            
            loadingProc = new List<Action>{
            // Dummy
                ()=>{
                    ;
                },

            // TopScene
                ()=>{
                    topScene = new TopScene();
                },
                
            // WidgetScenes
                ()=>{
                    topScene.AddWidgetScene(new AnimationImageBoxScene());
                },
                ()=>{
                    topScene.AddWidgetScene(new BusyIndicatorScene());   
                },
                ()=>{
                    topScene.AddWidgetScene(new ButtonScene());         
                },
                ()=>{
                    topScene.AddWidgetScene(new MessageDialogScene());   
                },
                ()=>{
                    topScene.AddWidgetScene(new CheckBoxScene());            
                },
                ()=>{
                    topScene.AddWidgetScene(new DialogScene());         
                },
                ()=>{
                    topScene.AddWidgetScene(new EditableTextScene());       
                },
                ()=>{
                    topScene.AddWidgetScene(new ImageBoxScene());           
                },
                ()=>{
                    topScene.AddWidgetScene(new LabelScene());              
                },
                ()=>{
                    topScene.AddWidgetScene(new PanelScene());              
                },
                ()=>{
                    topScene.AddWidgetScene(new ListPanelScene());          
                },
                ()=>{
                    topScene.AddWidgetScene(new GridListPanelScene());      
                },
                ()=>{
                    topScene.AddWidgetScene(new PagePanelScene());          
                },
                ()=>{
                    topScene.AddWidgetScene(new ProgressBarScene());            
                },
                ()=>{
                    topScene.AddWidgetScene(new ScrollPanelScene());            
                },
                ()=>{
                    topScene.AddWidgetScene(new SliderScene());             
                },
                ()=>{
                    topScene.AddWidgetScene(new PopupListScene());      
                },
                ()=>{
                    topScene.AddWidgetScene(new SpinBoxScene());                
                },
                
            // LiveWidgetScenes
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveFlipPanelScene());     
                },
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveJumpPanelScene());     
                },
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveListPanelScene());     
                },
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveScrollPanelScene());   
                },
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveSpringPanelScene());   
                },
                ()=>{
                    topScene.AddLiveWidgetScene(new LiveSphereScene());       
                },
                
            // EffectScenes
                ()=>{
                    topScene.AddEffectScene(new SlideInOutEffectScene());
                },
                ()=>{
                    topScene.AddEffectScene(new FadeInOutEffectScene());
                },
                ()=>{
                    topScene.AddEffectScene(new MoveEffectScene());
                },
                ()=>{
                    topScene.AddEffectScene(new JumpFlipEffectScene());
                },
                ()=>{
                    topScene.AddEffectScene(new FlipBoardEffectScene());   
                },
                ()=>{
                    topScene.AddEffectScene(new BunjeeJumpEffectScene());   
                },
                ()=>{
                    topScene.AddEffectScene(new TiltDropEffectScene());    
                },
                ()=>{
                    topScene.AddEffectScene(new ZoomEffectScene());        
                },
                
            // TopScene setup
                ()=>{
                    topScene.SetupContents();
                },
                
            };
            
            // Console.WriteLine ("loadinit: {0}\n", (stopwatch.Elapsed - start).TotalMilliseconds);

        }
        
        protected override void OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            
            var totalProcTime = new TimeSpan();
                
            do
            {
                var start = stopwatch.Elapsed;
                loadingProc[loadingIndex++]();
                var procTime = stopwatch.Elapsed - start;
                // Console.Write ("loading{0}: {1}\n", loadingIndex - 1, procTime.TotalMilliseconds);
                totalProcTime += procTime;
            } while (totalProcTime < minProcTime && loadingIndex < loadingProc.Count);

            
            ProgressBar_load.Progress = loadingIndex / (float)loadingProc.Count;
            
            if (loadingIndex >= loadingProc.Count)
            {
                UISystem.SetScene(topScene);
            }

        }
    }
}
