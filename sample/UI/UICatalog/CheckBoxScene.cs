/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class CheckBoxScene : ContentsScene
    {
        private CheckBox[] radioGroup;

        public CheckBoxScene()
        {
            InitializeWidget();

            this.Name = "CheckBox";

            // CheckBox
            check1.CheckedChanged += new EventHandler<TouchEventArgs>(check_CheckedChange);
            check2.Enabled = false;

            // RadioButton
            radioGroup = new CheckBox[3];
            radioGroup[0] = radio1;
            radioGroup[1] = radio2;
            radioGroup[2] = radio3;
            for (int i = 0; i < radioGroup.Length; i++)
            {
                radioGroup[i].CheckedChanged += new EventHandler<TouchEventArgs>(radio_CheckedChange);
            }
        }

        bool radioUpdating = false;

        private void radio_CheckedChange(object sender, TouchEventArgs e)
        {
            if (radioUpdating) return;

            radioUpdating = true;

            CheckBox radio = sender as CheckBox;
            if (radio.Checked)
            {
                foreach (CheckBox r in radioGroup)
                {
                    if (r != radio)
                    {
                        r.Checked = false;
                    }
                }
            }
            else
            {
                radio.Checked = true;
            }

            radioUpdating = false;
        }

        private void check_CheckedChange(object sender, TouchEventArgs e)
        {
            check2.Enabled = check1.Checked;
        }
    }
}
