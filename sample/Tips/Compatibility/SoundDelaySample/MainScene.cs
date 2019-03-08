using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

using Sce.PlayStation.Core.Audio;

namespace Sample
{
    public partial class MainScene : Scene
    {
        SoundPlayer player;
        float delay;

        //  Constructor

        public MainScene()
        {
            InitializeWidget();

            player = new Sound("/Application/assets/sound.wav").CreatePlayer();
            player.Loop = true;
            player.Play();

            this.Updated += OnUpdated;
            Button_1.ButtonAction += OnEarlier;
            Button_2.ButtonAction += OnLater;

            Slider_1.ValueChanging += OnChanged;
            Slider_1.Value = 50;
            Label_2.Text = "0 ms";
        }

        //  UI events

        public void OnUpdated(object sender, UpdateEventArgs e)
        {
            float sec = (float)player.Time - delay / 1000.0f;
            float scale = RootWidget.Height / DesignHeight ;

            float t = FMath.Repeat(sec, 0.0f, 1.0f);
            float y = (t > 0.05f) ? 0.0f : 1.0f;
            ImageBox_1.Y = (180 + y * 50) * scale;
        }

        public void OnChanged(object sender, SliderValueChangeEventArgs e)
        {
            ChangeDelay();
        }
        public void OnEarlier(object sender, TouchEventArgs e)
        {
            Slider_1.Value = ( (int)Slider_1.Value - 1 ) / 2 * 2 ;
            ChangeDelay();
        }
        public void OnLater(object sender, TouchEventArgs e)
        {
            Slider_1.Value = (int)Slider_1.Value / 2 * 2 + 2 ;
            ChangeDelay();
        }

        //  Subroutines

        void ChangeDelay()
        {
            delay = (Slider_1.Value - 50) * 5.0f;
            Label_2.Text = string.Format( "{0:+###;-###;0} ms", delay ) ;
        }
    }
}
