/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;

namespace OverlaySample
{
    public enum SettingColor : int
    {
        Blue = 0,
        Red = 1,
        Green = 2,
    }

    public partial class UISettingDialog : Dialog
    {

        public UISettingDialog()
        {
            InitializeWidget();
            PopupList_color.SelectedIndex = 0;
            Button_OK.ButtonAction += new EventHandler<TouchEventArgs>(Button_OK_ButtonAction);
        }

        public int Speed
        {
            get { return (int)Slider_speed.Value; }
            set { Slider_speed.Value = value; }
        }

        public SettingColor SettingColor
        {
            get { return (SettingColor)PopupList_color.SelectedIndex; }
            set { PopupList_color.SelectedIndex = (int)value; }
        }

        public Vector4 SettingColorRgba
        {
            get
            {
                switch (this.SettingColor)
                {
                    case SettingColor.Blue:
                        return new Vector4(0.0f, 0.5f, 1.0f, 1.0f);
                    case SettingColor.Red:
                        return new Vector4(1.0f, 0.2f, 0.0f, 1.0f);
                    case SettingColor.Green:
                        return new Vector4(0.0f, 1.0f, 0.2f, 1.0f);
                    default:
                        return new Vector4(0.0f, 0.5f, 1.0f, 1.0f);
                }
            }
        }

        void Button_OK_ButtonAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }

    }
}
