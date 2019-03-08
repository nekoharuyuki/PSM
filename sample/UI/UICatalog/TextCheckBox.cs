/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public class TextCheckBox : Panel
    {
        InternalCheckbox checkbox;
        Label label;
        float gapY = 10.0f;

        public TextCheckBox(string text)
            : this()
        {
            this.label.Text = text;
        }

        public TextCheckBox()
        {
            checkbox = new InternalCheckbox();
            label = new Label();

            this.Height = checkbox.Height;
            this.Width = checkbox.Width + label.Width + gapY;
            this.PriorityHit = true;

            checkbox.Anchors = Anchors.Left | Anchors.Width | Anchors.Height;
            checkbox.TouchResponse = false;
            checkbox.CheckedChanged += new EventHandler<TouchEventArgs>(checkbox_CheckedChange);

            label.X = checkbox.X + checkbox.Width + 5.0f;
            label.Height = this.Height;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Middle;
            label.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
            label.TouchResponse = false;

            this.AddChildLast(checkbox);
            this.AddChildLast(label);
        }

        protected override void OnTouchEvent(TouchEventCollection touchEvents)
        {
            checkbox.OnTouchEvent(touchEvents);
        }

        public int Index = 0;

        // property wrapper
        public string Text
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public bool Checked
        {
            get { return checkbox.Checked; }
            set { checkbox.Checked = value; }
        }

        public override bool Enabled
        {
            get { return checkbox.Enabled; }
            set
            {
                base.Enabled = value;
                checkbox.Enabled = value;
                label.Alpha = value ? 1.0f : 0.25f;
            }
        }

        public CheckBoxStyle Style
        {
            get { return checkbox.Style; }
            set
            {
                checkbox.Style = value;
                checkbox.Y = (this.Height - checkbox.Height) / 2;
                label.X = checkbox.Width + gapY;
                label.Width = this.Width - label.X;
            }
        }

        // event wrapper
        public event EventHandler<TouchEventArgs> CheckedChange;
        void checkbox_CheckedChange(object sender, TouchEventArgs e)
        {
            if (CheckedChange != null)
            {
                CheckedChange(this, e);
            }
        }

        // internal class
        private class InternalCheckbox : CheckBox
        {
            new public void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);
            }
        }
    }
}
