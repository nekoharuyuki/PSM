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
    /// <summary>日付を選択するためのウィジェット</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>A widget to select the date</summary>
    /// @endif
    public class DatePicker : Widget
    {
        private const float bgWidthGap = 30.0f;
        float[] unitPosX;
        float[] bgWidth;
        private Label separatorLabelLeft;
        private Label separatorLabelRight;
        private InternalSpinBox spinLeft;
        private InternalSpinBox spinMiddle;
        private InternalSpinBox spinRight;
        private TextRenderHelper textRenderer;
        static string separatorCharactor = "/";
        private DateTime lastDate;

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public DatePicker()
        {
            unitPosX = new float[] { 0.0f, 110.0f, 220.0f };
            bgWidth = new float[] { 80.0f, 80.0f, 120.0f };

            this.Date = DateTime.Now;
            MinYear = this.Date.Year - 20;
            MaxYear = this.Date.Year + 20;

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

            separatorLabelRight = new Label ();
            separatorLabelRight.SetPosition(unitPosX[1] + bgWidth[1], 0.0f);
            separatorLabelRight.SetSize(bgWidthGap, InternalSpinBox.defaultHeight);
            separatorLabelRight.VerticalAlignment = VerticalAlignment.Middle;
            separatorLabelRight.HorizontalAlignment = HorizontalAlignment.Center;
            separatorLabelRight.Font = textRenderer.Font;
            separatorLabelRight.Text = separatorCharactor;
            AddChildLast(separatorLabelRight);

            spinRight = new InternalSpinBox ();
            spinRight.Width = bgWidth[2];
            spinRight.X = unitPosX[2];
            AddChildLast(spinRight);

            spinLeft.spinList.ItemRequestAction += GridListItemRequestAction1Origin;
            spinMiddle.spinList.ItemRequestAction += GridListItemRequestActionDate;
            spinRight.spinList.ItemRequestAction += GridListItemRequestActionYear;
            UpdateListDate();

            spinLeft.spinList.StartItemRequest();
            spinMiddle.spinList.StartItemRequest();
            spinRight.spinList.StartItemRequest();

            spinLeft.spinList.FocusChanged += ItemFocusChanged;
            spinMiddle.spinList.FocusChanged += ItemFocusChanged;
            spinRight.spinList.FocusChanged += ItemFocusChanged;

            this.PriorityHit = true;

            lastDate = this.Date;
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
        /// <summary>年の最小値を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the minimum value for the year.</summary>
        /// @endif
        public int MinYear
        {
            get
            {
                return minYear;
            }
            set
            {
                minYear = value;
                UpdateListDate();
            }
        }

        private int minYear;


        /// @if LANG_JA
        /// <summary>年の最大値を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the maximum value for the year.</summary>
        /// @endif
        public int MaxYear
        {
            get
            {
                return maxYear;
            }
            set
            {
                maxYear = value;
                UpdateListDate();
            }
        }

        private int maxYear;


        /// @if LANG_JA
        /// <summary>年を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the year.</summary>
        /// @endif
        public int Year
        {
            get
            {
                if (spinRight != null)
                {
                    year = spinRight.spinList.ScrollAreaFirstLine + MinYear;
                }
                return year;
            }
            set
            {
                year = value;
                UpdateListDate();
            }
        }

        private int year;


        /// @if LANG_JA
        /// <summary>月を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the month.</summary>
        /// @endif
        public int Month
        {
            get
            {
                if (spinLeft != null)
                {
                    month = spinLeft.spinList.ScrollAreaFirstLine + 3;
                    if (month > 12)
                        month -= 12;
                }
                return month;
            }
            set
            {
                month = value;
                UpdateListDate();
            }
        }

        private int month;


        /// @if LANG_JA
        /// <summary>日を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the day.</summary>
        /// @endif
        public int Day
        {
            get
            {
                if (spinMiddle != null)
                {
                    day = spinMiddle.spinList.ScrollAreaFirstLine + 3;
                    if (day > 31)
                        day -= 31;
                }
                return day;
            }
            set
            {
                day = value;
                UpdateListDate();
            }
        }

        private int day;


        /// @if LANG_JA
        /// <summary>日付をDateTime構造体で取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the date with the DateTime structure.</summary>
        /// @endif
        public DateTime Date
        {
            get
            {
                return new DateTime (this.Year, this.Month, this.Day);
            }
            set
            {
                this.Year = value.Year;
                this.Month = value.Month;
                this.Day = value.Day;
            }
        }

        /// @if LANG_JA
        /// <summary>値が変更されたときに発行されるイベント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the value is changed</summary>
        /// @endif
        public event EventHandler<DatePickerValueChangedEventArgs> ValueChanged;

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


        private void UpdateListDate()
        {
            if (spinLeft == null || spinMiddle == null || spinRight == null)
            {
                return;
            }

            // Year
            if (MaxYear < MinYear)
            {
                int temp = MaxYear;
                maxYear = MinYear;
                minYear = temp;
            }

            int scrollTo;


            // Month
            month = Clamp(month, 1, 12);
            spinLeft.spinList.ListItemNum = 12;
            scrollTo = month - 1;
            spinLeft.spinList.ScrollTo(scrollTo - (InternalSpinBox.visibleCount / 2), 0.0f, false);
            spinLeft.spinList.FocusIndex = scrollTo;

            // Day
            day = Clamp(day, 1, 31);
            spinMiddle.spinList.ListItemNum = 31;
            scrollTo = day - 1;
            spinMiddle.spinList.ScrollTo(scrollTo - (InternalSpinBox.visibleCount / 2), 0.0f, false);
            spinMiddle.spinList.FocusIndex = scrollTo;

            // Year
            year = Clamp(year, MinYear, MaxYear);
            spinRight.spinList.ListItemNum = MaxYear - MinYear + 1 + (InternalSpinBox.visibleCount - 1);
            scrollTo = year - MinYear;
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

        private void GridListItemRequestActionYear
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
                        if (e.Index < (InternalSpinBox.visibleCount / 2) ||
                            e.Index >= glist.ListItemNum - (InternalSpinBox.visibleCount / 2))
                        {
                            text = "";
                        }
                        else
                        {
                            text = (e.Index + MinYear - (InternalSpinBox.visibleCount / 2)).ToString("0000");
                        }

                        // TODO: cache imageAsset or make TextureAtlas
                        if (item.ImageAsset != null)
                            item.ImageAsset.Dispose();

                        item.ImageAsset = textRenderer.DrawText(
                            ref text,
                            (int)(InternalSpinBox.unitWidth * InternalSpinBox.unitWidthPower),
                            (int)InternalSpinBox.unitHeight);
                        item.ShaderType = ShaderType.TextTexture;
                        item.Width = glist.ItemGapStep;
                        item.Height = glist.ItemGapLine;
                    }
                }
            }
        }

        private void GridListItemRequestAction1Origin
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
                    text = (index + 1).ToString("00");

                    if (item.ImageAsset != null)
                        item.ImageAsset.Dispose();

                    item.ImageAsset = textRenderer.DrawText(
                        ref text,
                        (int)InternalSpinBox.unitWidth,
                        (int)InternalSpinBox.unitHeight);
                    item.ShaderType = ShaderType.TextTexture;
                    item.Width = glist.ItemGapStep;
                    item.Height = glist.ItemGapLine;
                }
            }
        }

        private void GridListItemRequestActionDate
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
                    text = (index + 1).ToString("00");

                    if (item.ImageAsset != null)
                        item.ImageAsset.Dispose();

                    item.ImageAsset = textRenderer.DrawText(
                        ref text,
                        (int)InternalSpinBox.unitWidth,
                        (int)InternalSpinBox.unitHeight);
                    item.ShaderType = ShaderType.TextTexture;
                    item.Width = glist.ItemGapStep;
                    item.Height = glist.ItemGapLine;
                    item.Enabled = (item.ItemId + 1 <= GetDaysInMonth());
                }
            }
        }

        private int GetDaysInMonth()
        {
            int year = spinRight.spinList.ScrollAreaFirstLine + MinYear;

            // add 1 so that month and day start 1
            int month = spinLeft.spinList.ScrollAreaFirstLine + (InternalSpinBox.visibleCount / 2) + 1;
            if (month > spinLeft.spinList.TotalLineNum)
            {
                month -= spinLeft.spinList.TotalLineNum;
            }
            return DateTime.DaysInMonth(year, month);
        }

        private void ItemFocusChanged(object sender, EventArgs e)
        {
            int daysInMonth = GetDaysInMonth();

            int day = spinMiddle.spinList.ScrollAreaFirstLine + (InternalSpinBox.visibleCount / 2) + 1;
            if (day > spinMiddle.spinList.TotalLineNum)
            {
                day -= spinMiddle.spinList.TotalLineNum;
            }

            int scrollTo;
            if (day > daysInMonth)
            {
                if (daysInMonth < 30 && day > 30)
                {
                    scrollTo = 1;
                    spinMiddle.spinList.ScrollTo(29, 0.0f, true);
                }
                else
                {
                    scrollTo = daysInMonth - 1;
                    spinMiddle.spinList.ScrollTo(scrollTo - (InternalSpinBox.visibleCount / 2), 0.0f, true);
                }
            }
            else
            {
                if (ValueChanged != null)
                {
                    var oldDate = lastDate;
                    lastDate = this.Date;

                    ValueChanged(this, new DatePickerValueChangedEventArgs (lastDate, oldDate));
                }
            }

            foreach (var item in spinMiddle.spinList.listItem)
            {
                item.listItem.Enabled = (item.listItem.ItemId + 1 <= daysInMonth);
            }
        }

    }
            
    /// @if LANG_JA
    /// <summary>DatePicker の値が変更されたときのイベント引数</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Event argument when the DatePicker value has changed</summary>
    /// @endif
    public class DatePickerValueChangedEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public DatePickerValueChangedEventArgs()
        {
        }
        
        internal DatePickerValueChangedEventArgs(DateTime newDate, DateTime oldDate)
        {
            NewYear = newDate.Year;
            NewMonth = newDate.Month;
            NewDay = newDate.Day;
            OldYear = oldDate.Year;
            OldMonth = oldDate.Month;
            OldDay = oldDate.Day;
        }


        /// @if LANG_JA
        /// <summary>今回選択された年</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Currently selected year</summary>
        /// @endif
        public int NewYear
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>今回選択された月</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Currently selected month</summary>
        /// @endif
        public int NewMonth
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>今回選択された日</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Currently selected day</summary>
        /// @endif
        public int NewDay
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>前回選択されていた年</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Previously selected year</summary>
        /// @endif
        public int OldYear
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>前回選択されていた月</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Previously selected month</summary>
        /// @endif
        public int OldMonth
        {
            get;
            set;
        }
        
        
        /// @if LANG_JA
        /// <summary>前回選択されていた日</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Previously selected day</summary>
        /// @endif
        public int OldDay
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
                "New = {1:00}/{2:00}/{0:0000}, Old = {4:00}/{5:00}/{3:0000}",
                NewYear, NewMonth, NewDay, OldYear, OldMonth, OldDay);
        }
    }

}

