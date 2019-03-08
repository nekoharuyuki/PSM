/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;

namespace GameUI
{
    class Util
    {
    }

    internal delegate void Action();

    internal class DelayEffect : Effect
    {

        public DelayEffect()
        {
            Widget = null;
            Time = 0.0f;
            Action = null;
        }

        public DelayEffect(float time, Action action)
        {
            Widget = null;
            Time = time;
            this.action = action;
        }

        public static DelayEffect CreateAndStart(float time, Action action)
        {
            var effect = new DelayEffect(time, action);
            effect.Start();
            return effect;
        }

        protected override void OnStart()
        {
            if (action == null)
            {
                this.Stop();
            }
        }

        protected override EffectUpdateResponse OnUpdate(float elapsedTime)
        {
            if (this.TotalElapsedTime >= this.Time)
            {
                if (action != null)
                {
                    action();
                }
                return EffectUpdateResponse.Finish;
            }
            else
            {
                return EffectUpdateResponse.Continue;
            }
        }

        protected override void OnStop()
        {
        }

        public float Time
        {
            get;
            set;
        }

        public Action Action
        {
            get { return action; }
            set { action = value; }
        }
        private Action action;

    }


}
