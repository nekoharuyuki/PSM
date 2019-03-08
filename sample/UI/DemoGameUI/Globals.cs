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
    public static class Globals
    {
        public static int Speed
        {
            get { return 5; }
            //set { Slider_speed.Value = value; }
        }

        private static Vector4 settingColorRgba = new Vector4(0.0f, 0.5f, 1.0f, 1.0f);

        public static Vector4 SettingColorRgba
        {
            get { return settingColorRgba; }
            set { settingColorRgba = value; }
        }

        private static GameState state;

        public static GameState GameState
        {
            get { return state; }
            set { state = value; }
        }
    }

    public enum GameState
    {
        Stop,
        Pause,
        Play,
    }
}
