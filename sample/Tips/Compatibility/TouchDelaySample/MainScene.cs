using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

using Sce.PlayStation.Core.Input;

namespace Sample
{
    public partial class MainScene : Scene
    {
        Stopwatch stopwatch;
        float delay;
        Label[] records;

        //  Constructor

        public MainScene()
        {
            InitializeWidget();

            stopwatch = new Stopwatch();
            stopwatch.Start();

            this.Updated += OnUpdated;

            records = new [] {
                Label_3, Label_4, Label_5, Label_6, Label_7,
                Label_8, Label_9, Label_10, Label_11, Label_12
            };
        }

        //  UI events

        public void OnUpdated(object sender, UpdateEventArgs e)
        {
            float sec = (float)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            float scale = RootWidget.Height / DesignHeight ;

            float t = FMath.Repeat(sec, 0.0f, 1.0f);
            float y = (t < 0.5f) ? 1.0f : t * 2.0f - 1.0f;
            ImageBox_2.Y = (50 + y * 245) * scale;
        }

        //  Device events

        public void OnTouch()
        {
            float msec = (float)stopwatch.ElapsedTicks * 1000 / Stopwatch.Frequency;
            delay = FMath.Repeat(msec, -500.0f, 500.0f);
            string text = string.Format( "{0:+###;-###;0} ms", delay);
            if (delay < 0) text += " early";
            if (delay > 0) text += " late";
            Label_2.Text = text;

            for (int i = 0; i < records.Length - 1; i ++) {
                records[ i ].Text = records[ i + 1 ].Text;
            }
            records[ records.Length - 1 ].Text = text;
        }
    }
}
