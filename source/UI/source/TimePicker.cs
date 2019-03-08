/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.UI
{

    /// @if LANG_JA
    /// <summary>時刻を選択するためのウィジェット</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>A widget to select the time</summary>
    /// @endif
    public class TimePicker : Widget
    {
        private const float bgWidthGap = 30.0f;
        float[] unitPosX;
        float[] bgWidth;
        private Label separatorLabelLeft;
        private InternalSpinBox spinLeft;
        private InternalSpinBox spinMiddle;
        private InternalSpinBox spinRight;
        private TextRenderHelper textRenderer;
        static string[] noonText = { "AM", "PM" };
        static string separatorCharactor = ":";
        int lastHour;
        int lastMinute;

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public TimePicker()
        {
            unitPosX = new float[] { 0.0f, 110.0f, 200.0f };
            bgWidth = new float[] { 80.0f, 80.0f, 80.0f };

            this.Time = DateTime.Now;

            textRenderer = new TextRenderHelper ();

            base.Width = bgWidth[2] + unitPosX[2];
            base.Height = InternalSpinBox.defaultHeight;
   
            spinLeft = new InternalSpinBox ();
            spinLeft.Width = bgWidth[0];
            spinLeft.X = unitPosX[0];
            AddChildLast(spinLeft);

            separatorLabelLeft = new Label ();
            separatorLabelLeft.SetPosition(bgWidth[0], 0.0f);
            separatorLabelLeft.SetSize(bgWidthGap, InternalSpinBox.defaultHeight);
            separatorLabelLeft.Font = textRenderer.Font;
            separatorLabelLeft.VerticalAlignment = VerticalAlignment.Middle;
            separatorLabelLeft.HorizontalAlignment = HorizontalAlignment.Center;
            separatorLabelLeft.Text = separatorCharactor;
            AddChildLast(separatorLabelLeft);

            spinMiddle = new InternalSpinBox ();
            spinMiddle.Width = bgWidth[1];
            spinMiddle.X = unitPosX[1];
            AddChildLast(spinMiddle);

            spinRight = new InternalSpinBox ();
            spinRight.Width = bgWidth[2];
            spinRight.X = unitPosX[2];
            AddChildLast(spinRight);

            spinLeft.spinList.ItemRequestAction += GridListItemRequestAction0Origin;
            spinMiddle.spinList.ItemRequestAction += GridListItemRequestAction0Origin;
            spinRight.spinList.ItemRequestAction += GridListItemRequestActionAmPm;
            UpdateListTime();

            spinLeft.spinList.StartItemRequest();
            spinMiddle.spinList.StartItemRequest();
            spinRight.spinList.StartItemRequest();

            spinLeft.spinList.FocusChanged += ItemFocusChanged;
            spinMiddle.spinList.FocusChanged += ItemFocusChanged;
            spinRight.spinList.FocusChanged += ItemFocusChanged;

            this.PriorityHit = true;
            this.lastHour = this.Hour;
            this.lastMinute = this.Minute;
        }

            #region property

        /// @if LANG_JA
        /// <summary>幅を取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the width.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float Width
        {
            get
            {
                return base.Width;
            }
            set
            {
            }
        }


        /// @if LANG_JA
        /// <summary>高さを取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the height.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float Height
        {
            get
            {
                return base.Height;
            }
            set
            {
            }
        }

        /// @if LANG_JA
        /// <summary>ヒットテストハンドラ</summary>
        /// <param name="screenPoint">スクリーン座標系での位置</param>
        /// <returns>ヒットしている場合はtrue。ヒットしていない場合はfalse。</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Hit test handler</summary>
        /// <param name="screenPoint">Position in the screen coordinate system</param>
        /// <returns>If hit, then true. If not hit, then false.</returns>
        /// @endif
        public override bool HitTest(Vector2 screenPoint)
        {
            // Always skip hit test itself, but children are not skiped 
            return false;
        }

        /// @if LANG_JA
        /// <summary>優先してタッチに反応するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to prioritize and respond to a touch.</summary>
        /// @endif
        public override bool PriorityHit
        {
            get
            {
                return base.PriorityHit;
            }
            set
            {
                base.PriorityHit = value;
                if (this.spinLeft != null)
                {
                    this.spinLeft.PriorityHit = value;
                }
                if (this.spinMiddle != null)
                {
                    this.spinMiddle.PriorityHit = value;
                }
                if (this.spinRight != null)
                {
                    this.spinRight.PriorityHit = value;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>時間（24時間表記）を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the time (24-hour display).</summary>
        /// @endif
        public int Hour
        {
            get
            {
                if (spinLeft != null && spinRight != null)
                {
                    hour = spinLeft.spinList.ScrollAreaFirstLine + 2;
                    if (hour > 11)
                        hour -= 12;
                    hour += spinRight.spinList.ScrollAreaFirstLine * 12;
                }
                return hour;
            }
            set
            {
                hour = value;
                UpdateListTime();
            }
        }

        private int hour;


        /// @if LANG_JA
        /// <summary>分を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the minute.</summary>
        /// @endif
        public int Minute
        {
            get
            {
                if (spinMiddle != null)
                {
                    minute = spinMiddle.spinList.ScrollAreaFirstLine + 2;
                    if (minute > 59)
                        minute -= 60;
                }
                return minute;
            }
            set
            {
                minute = value;
                UpdateListTime();
            }
        }

        private int minute;

        /// @if LANG_JA
        /// <summary>時間をDateTime構造体で取得・設定する。</summary>
        /// <remarks>設定時に時間と分以外は無視され、取得時の日付は1年1月1日になります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the time with the DateTime structure.</summary>
        /// <remarks>The time other than the hour and minute is ignored at the time of setting, and the date at the time of obtaining becomes 1/1/1.</remarks>
        /// @endif
        public DateTime Time
        {
            get
            {
                return new DateTime (1, 1, 1, this.Hour, this.Minute, 0);
            }
            set
            {
                this.Hour = value.Hour;
                this.Minute = value.Minute;
            }
        }

        /// @if LANG_JA
        /// <summary>値が変更されたときに発行されるイベント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the value is changed</summary>
        /// @endif
        public event EventHandler<TimePickerValueChangedEventArgs> ValueChanged;

        float flickInterpRate
        {
            get
            {
                return this.spinLeft.spinList.flickInterpRate;
            }
            set
            {
                this.spinLeft.spinList.flickInterpRate = value;
                this.spinMiddle.spinList.flickInterpRate = value;
                this.spinRight.spinList.flickInterpRate = value;
            }
        }

        float flickSpeedCoeff
        {
            get
            {
                return this.spinLeft.spinList.flickSpeedCoeff;
            }
            set
            {
                this.spinLeft.spinList.flickSpeedCoeff = value;
                this.spinMiddle.spinList.flickSpeedCoeff = value;
                this.spinRight.spinList.flickSpeedCoeff = value;
            }
        }

        float fitInterpRate
        {
            get
            {
                return this.spinLeft.spinList.fitInterpRate;
            }
            set
            {
                this.spinLeft.spinList.fitInterpRate = value;
                this.spinMiddle.spinList.fitInterpRate = value;
                this.spinRight.spinList.fitInterpRate = value;
            }
        }


            #endregion


        private void UpdateListTime()
        {
            if (spinLeft == null || spinMiddle == null || spinRight == null)
            {
                return;
            }

            int scrollTo;

            // Hour
            hour = Clamp(hour, 0, 23);
            spinLeft.spinList.ListItemNum = 12;
            scrollTo = hour % 12;
            spinLeft.spinList.ScrollTo(scrollTo - (InternalSpinBox.visibleCount / 2), 0.0f, false);
            spinLeft.spinList.FocusIndex = scrollTo;

            // Minute
            minute = Clamp(minute, 0, 59);
            spinMiddle.spinList.ListItemNum = 60;
            scrollTo = minute;
            spinMiddle.spinList.ScrollTo(scrollTo - (InternalSpinBox.visibleCount / 2), 0.0f, false);
            spinMiddle.spinList.FocusIndex = scrollTo;

            // AmPm
            spinRight.spinList.ListItemNum = 2 + (InternalSpinBox.visibleCount - 1);
            scrollTo = (hour < 12) ? 0 : 1;
            spinRight.spinList.ScrollTo(scrollTo, 0.0f, false);
            spinRight.spinList.FocusIndex = scrollTo + (InternalSpinBox.visibleCount / 2);

            spinLeft.spinList.IsLoop = true;
            spinMiddle.spinList.IsLoop = true;
            spinRight.spinList.IsLoop = false;
        }

        int Clamp(int x, int min, int max)
        {
            return (x < min) ? min : ((x > max) ? max : x);
        }

        private void GridListItemRequestAction0Origin
            (object sender, InternalSpinBox.SpinItemRequestEventArgs e)
        {
            if (sender is InternalSpinBox.SpinList)
            {
                var glist = (InternalSpinBox.SpinList)sender;

                int index = e.Index % glist.ListItemNum;
                if (index < 0)
                {
                    index += glist.ListItemNum;
                }
                var item = glist.GetListItem(index);

                if (item != null)
                {
                    string text;
                    text = index.ToString("00");

                    if (item.ImageAsset != null)
                        item.ImageAsset.Dispose();

                    item.ImageAsset = textRenderer.DrawText(
                        ref text,
                        (int)InternalSpinBox.unitWidth,
                        (int)InternalSpinBox.unitHeight);
                    item.ShaderType = ShaderType.TextTexture;
                    item.Width = glist.ItemGapStep;
                    item.Height = glist.ItemGapLine;

                    //                        listAsset.Add(item.ImageAsset);
                }
            }
        }

        private void GridListItemRequestActionAmPm
            (object sender, InternalSpinBox.SpinItemRequestEventArgs e)
        {
            if (sender is InternalSpinBox.SpinList)
            {
                var glist = (InternalSpinBox.SpinList)sender;
                var item = glist.GetListItem(e.Index);

                if (item != null)
                {
                    if (0 <= e.Index && e.Index < glist.ListItemNum)
                    {
                        string text;
                        if (e.Index < (InternalSpinBox.visibleCount / 2)
                            || e.Index >= glist.ListItemNum - (InternalSpinBox.visibleCount / 2))
                        {
                            text = "";
                        }
                        else
                        {
                            text = noonText[e.Index - (InternalSpinBox.visibleCount / 2)];
                        }

                        if (item.ImageAsset != null)
                            item.ImageAsset.Dispose();

                        item.ImageAsset = textRenderer.DrawText(
                            ref text,
                            (int)InternalSpinBox.unitWidth,
                            (int)InternalSpinBox.unitHeight);
                        item.ShaderType = ShaderType.TextTexture;
                        item.Width = glist.ItemGapStep;
                        item.Height = glist.ItemGapLine;

                        //                            listAsset.Add(item.ImageAsset);
                    }
                }
            }
        }

        private void ItemFocusChanged(object sender, EventArgs e)
        {
            var oldh = lastHour;
            var oldm = lastMinute;
            lastHour = this.Hour;
            lastMinute = this.Minute;
            if (ValueChanged != null)
            {
                ValueChanged(this, new TimePickerValueChangedEventArgs (lastHour,lastMinute,oldh,oldm));
            }
        }

    }
    /// @if LANG_JA
    /// <summary>TimePicker の値が変更されたときのイベント引数</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Event argument when the TimePicker value has changed</summary>
    /// @endif
    public class TimePickerValueChangedEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public TimePickerValueChangedEventArgs()
        {
        }
        
        internal TimePickerValueChangedEventArgs(int newH, int newM, int oldH, int oldM)
        {
            NewHour = newH;
            NewMinute = newM;
            OldHour = oldH;
            OldMinute = oldM;
        }

        /// @if LANG_JA
        /// <summary>今回選択された時間</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Currently selected hour</summary>
        /// @endif
        public int NewHour
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>今回選択された分</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Currently selected minute</summary>
        /// @endif
        public int NewMinute
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>前回選択されていた時間</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Previously selected hour</summary>
        /// @endif
        public int OldHour
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>前回選択されていた分</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Previously selected minute</summary>
        /// @endif
        public int OldMinute
        {
            get;
            set;
        }
        
        /// @if LANG_JA
        /// <summary>文字列を取得する。</summary>
        /// <returns>変換された文字列</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the character string.</summary>
        /// <returns>Converted character string</returns>
        /// @endif
        public override string ToString ()
        {
            return string.Format (
                "New = {0:00}:{1:00}, Old = {2:00}:{3:00}",
                NewHour, NewMinute, OldHour, OldMinute);
        }
    }


}

