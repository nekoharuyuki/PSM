/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;


namespace UICatalog
{
    public partial class LabelScene : ContentsScene
    {
        private bool isSliderInitialized = false;

        public LabelScene()
        {
            InitializeWidget();

            this.Name = "Label";

            // Lavel
            labelTextE.BackgroundColor = new UIColor((float)80/255, (float)48/255, (float)48/255, 1f);

            // Slider (Horizontal)
            sliderH.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderHValueChangeAction);
            sliderH.ValueChangeEventEnabled = true;

            // Slider (Vartical)
            sliderV.ValueChanging += new EventHandler<SliderValueChangeEventArgs>(OnSliderVValueChangeAction);
            sliderV.ValueChangeEventEnabled = true;

            // PopupList (TextTrimming)
            popupTrim.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupTrimSelectedChangeAction);

            // PopupList (LineBreak)
            popupLineBreak.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupLineBreakSelectedChangeAction);

            // PopupList (Horizontal)
            popupHorizontal.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupHorizontalSelectedChangeAction);

            // PopupList (Vertical)
            popupVertical.SelectionChanged +=
                    new EventHandler<PopupSelectionChangedEventArgs>(PopupVerticalSelectedChangeAction);

            // CheckBox (Shadow)
            checkBoxShadow.CheckedChanged +=
                new EventHandler<TouchEventArgs>(ButtonShadowCheckedChangeAction);
        }

        protected override void OnShowing()
        {
            base.OnShowing();

            if (!isSliderInitialized)
            {
                sliderH.MaxValue = labelTextE.Width;
                sliderH.Value = labelTextE.Width;
                sliderV.MaxValue = labelTextE.Height;
                sliderV.Value = labelTextE.Height;
                isSliderInitialized = true;
            }
        }

        private void OnSliderHValueChangeAction(object sender, SliderValueChangeEventArgs e)
        {
            labelTextE.Width = (float)((Slider)sender).Value;
        }

        private void OnSliderVValueChangeAction(object sender, SliderValueChangeEventArgs e)
        {
            labelTextE.Height = (float)((Slider)sender).Value;
        }

        private void PopupTrimSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            labelTextE.TextTrimming = TextTrimming.None + args.NewIndex;
        }

        private void PopupLineBreakSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            labelTextE.LineBreak = LineBreak.Character + args.NewIndex;
        }

        private void PopupHorizontalSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            labelTextE.HorizontalAlignment = HorizontalAlignment.Left + args.NewIndex;
        }

        private void PopupVerticalSelectedChangeAction(object sender, PopupSelectionChangedEventArgs args)
        {
            labelTextE.VerticalAlignment = VerticalAlignment.Top + args.NewIndex;
        }

        private void ButtonShadowCheckedChangeAction(object sender, TouchEventArgs e)
        {
            if (checkBoxShadow.Checked == true)
            {
                labelTextE.TextShadow = new TextShadowSettings();
            }
            else
            {
                labelTextE.TextShadow = null;
            }
        }
    }
}
