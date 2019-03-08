using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScreenOrientationSample
{
    public partial class TopScene : Scene
    {
        ScreenOrientationScene TargetScene;

        public TopScene()
        {
            InitializeWidget();

            this.ButtonLandscape.ButtonAction += ButtonLandscape_ButtonAction;
            this.ButtonReverseLandscape.ButtonAction += ButtonReverseLandscape_ButtonAction;
            this.ButtonPortrait.ButtonAction += ButtonPortrait_ButtonAction;
            this.ButtonReversePortrait.ButtonAction +=ButtonReversePortrait_ButtonAction;

            this.TargetScene = new ScreenOrientationScene();
            this.TargetScene.BackButtonAction += BackButton_ButtonAction;
        }

        void ButtonLandscape_ButtonAction(object sender, TouchEventArgs e)
        {
            // set ScreenOrientation
            TargetScene.ScreenOrientation = ScreenOrientation.Landscape;
            // relayout as Horizontal
            TargetScene.SetWidgetLayout(LayoutOrientation.Horizontal);

            TargetScene.TitleText = "Landscape";
            UISystem.SetScene(TargetScene);
        }

        void ButtonReverseLandscape_ButtonAction(object sender, TouchEventArgs e)
        {
            // set ScreenOrientation
            TargetScene.ScreenOrientation = ScreenOrientation.ReverseLandscape;
            // relayout as Horizontal
            TargetScene.SetWidgetLayout(LayoutOrientation.Horizontal);

            TargetScene.TitleText = "ReverseLandscape";
            UISystem.SetScene(TargetScene);
        }

        void ButtonPortrait_ButtonAction(object sender, TouchEventArgs e)
        {
            // set ScreenOrientation
            TargetScene.ScreenOrientation = ScreenOrientation.Portrait;
            // relayout as Vertical
            TargetScene.SetWidgetLayout(LayoutOrientation.Vertical);

            TargetScene.TitleText = "Portrait";
            UISystem.SetScene(TargetScene);
        }

        void ButtonReversePortrait_ButtonAction(object sender, TouchEventArgs e)
        {
            // set ScreenOrientation
            TargetScene.ScreenOrientation = ScreenOrientation.ReversePortrait;
            // relayout as Vertical
            TargetScene.SetWidgetLayout(LayoutOrientation.Vertical);

            TargetScene.TitleText = "ReversePortrait";
            UISystem.SetScene(TargetScene);
        }

        void BackButton_ButtonAction(object sender, TouchEventArgs e)
        {
            UISystem.SetScene(this);
        }
    }
}
