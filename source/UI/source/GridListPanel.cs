/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>スクロール方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll direction</summary>
        /// @endif
        public enum GridListScrollOrientation
        {
            /// @if LANG_JA
            /// <summary>水平方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Horizontal</summary>
            /// @endif
            Horizontal = 0,

            /// @if LANG_JA
            /// <summary>垂直方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Vertical</summary>
            /// @endif
            Vertical
        }


        /// @if LANG_JA
        /// <summary>複数の行と列を持つリストウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List widget with multiple rows and columns</summary>
        /// @endif
        public class GridListPanel : ContainerWidget
        {
            private const float focusMoveMargin = 30;


            private enum AnimationState
            {
                None = 0,
                Drag,
                Flick,
                Fit
            }

            private enum TerminalState
            {
                None,
                Top,
                TopReset,
                Bottom,
                BottomReset
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="scrollOrientation">スクロール方向</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="scrollOrientation">Scroll direction</param>
            /// @endif
            public GridListPanel(GridListScrollOrientation scrollOrientation)
            {
                this.backgroundSprt = new UISprite(1);
                base.RootUIElement.AddChildLast(this.backgroundSprt);

                this.basePanel = new Panel();
                this.basePanel.EnabledChildrenAnchors = false;
                base.AddChildLast(this.basePanel);

                if (scrollOrientation == GridListScrollOrientation.Horizontal)
                {
                    this.scrollBar = new ScrollBar(ScrollBarOrientation.Horizontal);
                }
                else
                {
                    this.scrollBar = new ScrollBar(ScrollBarOrientation.Vertical);
                }

                this.Width = defaultWidth;
                this.Height = defaultHeight;
                this.X = 0;
                this.Y = 0;
                this.ScrollOrientation = scrollOrientation;
                this.scrollAreaStepCount = 1;
                this.scrollAreaLineCount = 1;
                this.scrollAreaFirstIndex = 0;
                this.ItemVerticalGap = 5.0f;
                this.ItemHorizontalGap = 5.0f;
                this.SnapScroll = false;
                this.isSetup = false;
                this.isScrollSyncEnabled = true;
                this.itemCount = 0;
                this.itemWidth = 64;
                this.itemHeight = 64;
                this.cacheLineCount = 1;
                this.cacheStartIndex = -1;
                this.cacheEndIndex = -1;
                this.fitTargetIndex = 0;
                this.fitTargetOffsetPixel = 0.0f;
                this.fitInterpRatio = 0.2f;
                this.scrollVelocity = 0.0f;
                this.listItem = new List<ListContainer>();
                this.poolItem = new List<ListContainer>();
                this.animationState = AnimationState.None;
                this.terminalState = TerminalState.None;
                this.terminalDistance = 0.0f;

                base.Clip = true;
                base.HookChildTouchEvent = true;
                base.EnabledChildrenAnchors = false;

                this.animation = false;
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;

                    if (this.backgroundSprt != null)
                    {
                        UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                        unit.Width = value;
                    }

                    if (this.scrollBar != null)
                    {
                        if (this.scrollBar.Orientation == ScrollBarOrientation.Horizontal)
                        {
                            this.scrollBar.Y = this.Height - this.scrollBar.Height;
                            this.scrollBar.Width = value;
                        }
                        else
                        {
                            this.scrollBar.X = this.Width - this.scrollBar.Width;
                        }
                    }

                    if(this.basePanel != null)
                    {
                        this.basePanel.Width = value;
                    }
                    
                    CalcScrollAreaStepNum();
                    CalcEdgeMarginHorizontal();

                    if (this.isSetup)
                    {
                        UpdateListItem(false);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;

                    if (this.backgroundSprt != null)
                    {
                        UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                        unit.Height = value;
                    }

                    if (this.scrollBar != null)
                    {
                        if (this.scrollBar.Orientation == ScrollBarOrientation.Vertical)
                        {
                            this.scrollBar.X = this.Width - this.scrollBar.Width;
                            this.scrollBar.Height = value;
                        }
                        else
                        {
                            this.scrollBar.Y = this.Height - this.scrollBar.Height;
                        }
                    }

                    if(this.basePanel != null)
                    {
                        this.basePanel.Height = value;
                    }

                    CalcScrollAreaLineNum();
                    CalcEdgeMarginVertical();

                    if (this.isSetup)
                    {
                        UpdateListItem(false);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>スクロール方向を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the scroll direction.</summary>
            /// @endif
            public GridListScrollOrientation ScrollOrientation
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>背景色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background color.</summary>
            /// @endif
            public UIColor BackgroundColor
            {
                get
                {
                    UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                    return unit.Color;
                }
                set
                {
                    UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                    unit.Color = value;
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムの幅を取得・設定する。</summary>
            /// <exception cref="ArgumentOutOfRangeException">アイテムの幅とアイテムの列間の合計がリストの幅より大きい</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width of an item.</summary>
            /// <exception cref="ArgumentOutOfRangeException">The total of the item width and item column spacing is larger than the list width.</exception>
            /// @endif
            public float ItemWidth
            {
                get
                {
                    return this.itemWidth;
                }
                set
                {
                    if (this.itemWidth != value)
                    {
                        this.itemWidth = (value > 0) ? value : 0;
                        CalcScrollAreaStepNum();
                        CalcEdgeMarginHorizontal();

                        if (this.isSetup)
                        {
                            UpdateListItem(false);
                        }
                    }
                }
            }
            private float itemWidth;

            /// @if LANG_JA
            /// <summary>アイテムの高さを取得・設定する。</summary>
            /// <exception cref="ArgumentOutOfRangeException">アイテムの高さとアイテムの行間の合計がリストの高さより大きい</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of an item.</summary>
            /// <exception cref="ArgumentOutOfRangeException">The total of the item height and item row spacing is larger than the list height.</exception>
            /// @endif
            public float ItemHeight
            {
                get
                {
                    return this.itemHeight;
                }
                set
                {
                    if (this.itemHeight != value)
                    {
                        this.itemHeight = (value > 0) ? value : 0;
                        CalcScrollAreaLineNum();
                        CalcEdgeMarginVertical();

                        if (this.isSetup)
                        {
                            UpdateListItem(false);
                        }
                    }
                }
            }
            private float itemHeight;

            /// @if LANG_JA
            /// <summary>アイテムの行間を取得・設定する。</summary>
            /// <exception cref="ArgumentOutOfRangeException">アイテムの行間とアイテムの高さの合計がリストの高さより大きい</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of an item.</summary>
            /// <exception cref="ArgumentOutOfRangeException">The total of the item row spacing and item height is larger than the list height.</exception>
            /// @endif
            public float ItemVerticalGap
            {
                get
                {
                    return this.itemVerticalGap;
                }
                set
                {
                    if (this.ItemVerticalGap != value)
                    {
                        this.itemVerticalGap = value;
                        CalcScrollAreaLineNum();
                        CalcEdgeMarginVertical();

                        if (this.isSetup)
                        {
                            UpdateListItem(false);
                        }
                    }
                }
            }
            private float itemVerticalGap;

            /// @if LANG_JA
            /// <summary>アイテムの列間を取得・設定する。</summary>
            /// <exception cref="ArgumentOutOfRangeException">アイテムの列間とアイテムの幅の合計がリストの幅より大きい</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the column spacing of an item.</summary>
            /// <exception cref="ArgumentOutOfRangeException">The total of the item column spacing and item width is larger than the list width.</exception>
            /// @endif
            public float ItemHorizontalGap
            {
                get
                {
                    return this.itemHorizontalGap;
                }
                set
                {
                    if (this.itemHorizontalGap != value)
                    {
                        this.itemHorizontalGap = value;
                        CalcScrollAreaStepNum();
                        CalcEdgeMarginHorizontal();

                        if (this.isSetup)
                        {
                            UpdateListItem(false);
                        }
                    }
                }
            }
            private float itemHorizontalGap;

            /// @if LANG_JA
            /// <summary>スクロールバーの見え方を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scroll bar view.</summary>
            /// @endif
            public ScrollBarVisibility ScrollBarVisibility
            {
                get
                {
                    return this.scrollBarVisibility;
                }
                set
                {
                    this.scrollBarVisibility = value;
                    updateScrollBarVisible();
                }
            }
            private ScrollBarVisibility scrollBarVisibility = ScrollBarVisibility.ScrollableVisible;

            private void CalcScrollAreaLineNum()
            {
                if (this.Height < 1 || this.ItemHeight + this.ItemVerticalGap < 1)
                {
                    this.scrollAreaLineCount = 1;
                    return;
                }

                float tempCount = (this.Height - this.ItemVerticalGap) / (this.ItemHeight + this.ItemVerticalGap);
                if (this.ScrollOrientation == GridListScrollOrientation.Vertical)
                {
                    this.scrollAreaLineCount = (int)Math.Ceiling(tempCount);
                }
                else
                {
                    this.scrollAreaLineCount = (int)tempCount;
                }

                if (this.scrollAreaLineCount <= 0)
                {
                    this.scrollAreaLineCount = 1;
                }
            }

            private void CalcScrollAreaStepNum()
            {
                if (this.Width < 1 || this.ItemWidth + this.ItemHorizontalGap < 1)
                {
                    this.scrollAreaStepCount = 1;
                    return;
                }

                float tempCount = (this.Width - this.ItemHorizontalGap) / (this.ItemWidth + this.ItemHorizontalGap);
                if (this.ScrollOrientation == GridListScrollOrientation.Vertical)
                {
                    this.scrollAreaStepCount = (int)tempCount;
                }
                else
                {
                    this.scrollAreaStepCount = (int)Math.Ceiling(tempCount);
                }

                if (this.scrollAreaStepCount <= 0)
                {
                    this.scrollAreaStepCount = 1;
                }
            }

            private void CalcEdgeMarginVertical()
            {
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        this.verticalEdgeMargin =  this.ItemVerticalGap;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        this.verticalEdgeMargin = (this.Height - this.scrollAreaLineCount * (this.ItemHeight + this.ItemVerticalGap)
                                                    + this.ItemVerticalGap) / 2.0f;
                        if (this.verticalEdgeMargin < 0)
                            this.verticalEdgeMargin = 0;
                        break;
                }
            }

            private void CalcEdgeMarginHorizontal()
            {
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        this.horizontalEdgeMargin = (this.Width - this.scrollAreaStepCount * (this.ItemWidth + this.ItemHorizontalGap)
                                                    + this.ItemHorizontalGap) / 2.0f;
                        if (this.horizontalEdgeMargin < 0)
                            this.horizontalEdgeMargin = 0;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        this.horizontalEdgeMargin = this.ItemHorizontalGap;
                        break;
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムごとに停止するかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to stop for each item.</summary>
            /// @endif
            public bool SnapScroll
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>アイテムの要求を開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the request of an item.</summary>
            /// @endif
            public void StartItemRequest()
            {
                if (itemCreator == null)
                {
                    return;
                }

                if (!isSetup)
                {
                    DragGestureDetector drag = new DragGestureDetector();
                    drag.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                    drag.Direction = (this.ScrollOrientation == GridListScrollOrientation.Horizontal) ? DragDirection.Horizontal : DragDirection.Vertical;
                    this.AddGestureDetector(drag);

                    FlickGestureDetector flick = new FlickGestureDetector();
                    flick.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                    flick.Direction = (this.ScrollOrientation == GridListScrollOrientation.Horizontal) ? FlickDirection.Horizontal : FlickDirection.Vertical;
                    this.AddGestureDetector(flick);
                }

                if (this.scrollAreaFirstIndex > this.MaxScrollAreaFirstIndex)
                {
                    SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                }
                else
                {
                    SetScrollAreaFirstIndex(this.scrollAreaFirstIndex, 0.0f);
                }

                this.listItem.Clear();
                this.poolItem.Clear();

                this.cacheStartIndex = 0;
                this.cacheEndIndex = 0;

                this.isSetup = true;

                UpdateListItem(false);
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスのアイテムを取得する。</summary>
            /// <param name="index">アイテムのインデックス</param>
            /// <returns>アイテム（nullが返ることがある。）</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the specified index item.</summary>
            /// <param name="index">Item index</param>
            /// <returns>Item (null may be returned)</returns>
            /// @endif
            public ListPanelItem GetListItem(int index)
            {
                if (index < 0)
                {
                    return null;
                }

                for (int i = 0; i < this.listItem.Count; i++)
                {
                    var item = listItem[i].Item;
                    if (item != null && item.Index == index)
                    {
                        return (item);
                    }
                }

                return null;
            }

            /// @if LANG_JA
            /// <summary>表示領域の左上のアイテムのインデックスを取得する。</summary>
            /// <remarks>アイテムは行間と列間を含む。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index of the top left item in the display area.</summary>
            /// <remarks>The item includes the row and column spacing.</remarks>
            /// @endif
            public int ItemIndex
            {
                get
                {
                    int index = 0;
                    switch (this.ScrollOrientation)
                    {
                        case GridListScrollOrientation.Vertical:
                            index = this.scrollAreaFirstIndex * this.scrollAreaStepCount;
                            break;
                        case GridListScrollOrientation.Horizontal:
                            index = this.scrollAreaFirstIndex * this.scrollAreaLineCount;
                            break;
                    }
                    return index;
                }
            }

            /// @if LANG_JA
            /// <summary>表示領域の左上のアイテムのずれを取得する。（ピクセル）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the deviation of the top left item in the display area. (pixels)</summary>
            /// @endif
            public float ItemPixelOffset
            {
                get
                {
                    float offset = 0.0f;
                    switch (this.ScrollOrientation)
                    {
                        case GridListScrollOrientation.Vertical:
                            offset = -this.basePanel.Y;
                            break;
                        case GridListScrollOrientation.Horizontal:
                            offset = -this.basePanel.X;
                            break;
                    }
//                    int index = 0;
//                    NormalizeScrollAreaFirstIndex(ref index, ref offset);
                    return offset;
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムの総数を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the total number of items.</summary>
            /// @endif
            public int ItemCount
            {
                get
                {
                    return this.itemCount;
                }
                set
                {
                    this.itemCount = value;

                    if (this.scrollAreaFirstIndex > this.MaxScrollAreaFirstIndex)
                    {
                        SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                    }

                    if (this.isSetup)
                    {
                        UpdateListItem(false);
                    }
                }
            }
            private int itemCount;

            private int TotalLineCount
            {
                get
                {
                    if (this.scrollAreaStepCount == 0 || this.itemCount == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return (int)((this.itemCount - 1) / this.scrollAreaStepCount + 1);
                    }
                }
            }

            private int TotalStepCount
            {
                get
                {
                    if (this.scrollAreaLineCount == 0 || this.itemCount == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return (int)((this.itemCount - 1) / this.scrollAreaLineCount + 1);
                    }
                }
            }

            private void NormalizeScrollAreaFirstIndex(ref int lineIndex, ref float offsetPixel)
            {
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        while (offsetPixel < 0.0f)
                        {
                            offsetPixel += (this.ItemHeight + this.ItemVerticalGap);
                            lineIndex--;
                        }
                        while (offsetPixel >= (this.ItemHeight + this.ItemVerticalGap))
                        {
                            offsetPixel -= (this.ItemHeight + this.ItemVerticalGap);
                            lineIndex++;
                        }
                        break;

                    case GridListScrollOrientation.Horizontal:
                        while (offsetPixel < 0.0f)
                        {
                            offsetPixel += (this.ItemWidth + this.ItemHorizontalGap);
                            lineIndex--;
                        }
                        while (offsetPixel >= (this.ItemWidth + this.ItemHorizontalGap))
                        {
                            offsetPixel -= (this.ItemWidth + this.ItemHorizontalGap);
                            lineIndex++;
                        }
                        break;
                }
            }

            private void SetScrollAreaFirstIndex(int lineIndex, float offsetPixel)
            {
                NormalizeScrollAreaFirstIndex(ref lineIndex, ref offsetPixel);

                if (0 <= lineIndex && lineIndex <= this.MaxScrollAreaFirstIndex)
                {
                    bool isMod = false;
                    if (this.scrollAreaFirstIndex != lineIndex)
                    {
                        this.scrollAreaFirstIndex = lineIndex;
                        isMod = true;
                    }
                    if (this.terminalState != TerminalState.None)
                    {
                        isMod = true;
                    }

                    SetNodePos(offsetPixel);

                    if (this.isSetup)
                    {
                        UpdateListItem(isMod);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>指定したアイテムの位置までスクロールする。</summary>
            /// <param name="itemIndex">アイテムのインデックス</param>
            /// <param name="pixelOffset">アイテムのずれ（ピクセル）</param>
            /// <param name="withAnimation">アニメーションするかどうか</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scrolls to the specified item position.</summary>
            /// <param name="itemIndex">Item index</param>
            /// <param name="pixelOffset">Item deviation (pixels)</param>
            /// <param name="withAnimation">Whether to execute the animation</param>
            /// @endif
            public void ScrollTo(int itemIndex, float pixelOffset, bool withAnimation)
            {
                if (itemIndex >= ItemCount)
                {
                    return;
                }
                if(this.scrollAreaLineCount <= 0 || this.scrollAreaStepCount <= 0)
                {
                    return;
                }

                if (this.isSetup)
                {
                    this.isScrollSyncEnabled = true;
                }

                int scrollAreaFirstIndex = CalcLineIndex(itemIndex);
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        scrollAreaFirstIndex = CalcLineIndex(itemIndex);
                        break;
                    case GridListScrollOrientation.Horizontal:
                        scrollAreaFirstIndex = CalcStepIndex(itemIndex);
                        break;
                }

                NormalizeScrollAreaFirstIndex(ref scrollAreaFirstIndex, ref pixelOffset);

                if (this.SnapScroll)
                {
                    if (pixelOffset > (this.itemHeight + this.ItemVerticalGap) * 0.5f)
                    {
                        scrollAreaFirstIndex += 1;
                    }

                    pixelOffset = 0.0f;
                }

                if (IsUnderOrEqualMinScrollArea(scrollAreaFirstIndex, pixelOffset))
                {
                    scrollAreaFirstIndex = 0;
                    pixelOffset = 0.0f;
                }
                else if (IsOverOrEqualMaxScrollArea(scrollAreaFirstIndex, pixelOffset))
                {
                    scrollAreaFirstIndex = this.MaxScrollAreaFirstIndex;
                    pixelOffset = this.MaxScrollAreaFirstOffset;
                }

                if (isSetup && withAnimation)
                {
                    fitTargetIndex = scrollAreaFirstIndex;
                    fitTargetOffsetPixel = pixelOffset;

                    animationState = AnimationState.Fit;
                    fitInterpRatio = 0.2f;
                    this.animation = true;
                }
                else
                {
                    SetScrollAreaFirstIndex(scrollAreaFirstIndex, pixelOffset);
                }
            }

            /// @if LANG_JA
            /// <summary>指定したアイテムの位置までスクロールする。</summary>
            /// <param name="itemIndex">アイテムのインデックス</param>
            /// <param name="withAnimation">アニメーションするかどうか</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scrolls to the specified item position.</summary>
            /// <param name="itemIndex">Item index</param>
            /// <param name="withAnimation">Whether to execute the animation</param>
            /// @endif
            public void ScrollTo(int itemIndex, bool withAnimation)
            {
                ScrollTo(itemIndex, 0.0f, withAnimation);
            }

            private void UpdateListItem(bool isMod)
            {
                int cacheStartIndex = 0;
                int cacheEndIndex = 0;

                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        cacheStartIndex =
                            (this.scrollAreaFirstIndex - this.cacheLineCount) * this.scrollAreaStepCount;
                        cacheEndIndex =
                            (this.scrollAreaFirstIndex + this.scrollAreaLineCount + this.cacheLineCount + 1) * this.scrollAreaStepCount;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        cacheStartIndex =
                            (this.scrollAreaFirstIndex - this.cacheLineCount) * this.scrollAreaLineCount;
                        cacheEndIndex =
                            (this.scrollAreaFirstIndex + this.scrollAreaStepCount + this.cacheLineCount + 1) * this.scrollAreaLineCount;
                        break;
                }

                if (cacheStartIndex < 0)
                {
                    cacheStartIndex = 0;
                }
                if (cacheEndIndex > itemCount)
                {
                    cacheEndIndex = itemCount;
                }

                bool isModify = isMod;

                ListContainer container = new ListContainer();

                // all reset
                if (this.cacheEndIndex - 1 < cacheStartIndex || cacheEndIndex - 1 < this.cacheStartIndex)
                {
                    while (listItem.Count != 0)
                    {
                        container = listItem[0];
                        listItem.RemoveAt(0);
                        PoolContainer(ref container);
                        isModify = true;
                    }

                    for (int diffIndex = cacheStartIndex;
                        diffIndex < cacheEndIndex;
                        diffIndex++)
                    {
                        container = GetContainer();
                        CreateListItem(ref container, diffIndex);
                        listItem.Add(container);
                        isModify = true;
                    }
                }
                else if (this.cacheStartIndex != cacheStartIndex || this.cacheEndIndex != cacheEndIndex)
                {
                    if (this.cacheStartIndex < cacheStartIndex)
                    {
                        for (int diffIndex = cacheStartIndex - 1;
                            diffIndex >= this.cacheStartIndex;
                            diffIndex--)
                        {
                            container = listItem[0];
                            listItem.RemoveAt(0);
                            PoolContainer(ref container);
                            isModify = true;
                        }
                    }

                    if (this.cacheEndIndex > cacheEndIndex)
                    {
                        for (int diffIndex = cacheEndIndex;
                            diffIndex < this.cacheEndIndex;
                            diffIndex++)
                        {
                            container = listItem[listItem.Count - 1];
                            listItem.RemoveAt(listItem.Count - 1);
                            PoolContainer(ref container);
                            isModify = true;
                        }
                    }

                    if (this.cacheStartIndex > cacheStartIndex)
                    {
                        for (int diffIndex = this.cacheStartIndex - 1;
                            diffIndex >= cacheStartIndex;
                            diffIndex--)
                        {
                            container = GetContainer();
                            CreateListItem(ref container, diffIndex);
                            listItem.Insert(0, container);
                            isModify = true;
                        }
                    }

                    if (this.cacheEndIndex < cacheEndIndex)
                    {
                        for (int diffIndex = this.cacheEndIndex;
                            diffIndex < cacheEndIndex;
                            diffIndex++)
                        {
                            container = GetContainer();
                            CreateListItem(ref container, diffIndex);
                            listItem.Add(container);
                            isModify = true;
                        }
                    }
                }

                this.cacheStartIndex = cacheStartIndex;
                this.cacheEndIndex = cacheEndIndex;

                if (isModify)
                {
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        int currentId = listItem[i].TotalIndex;
                        listItem[i].Item.X = CalcItemPosOnScrollNodeX(currentId);
                        listItem[i].Item.Y = CalcItemPosOnScrollNodeY(currentId);

                        if (listItem[i].Updated)
                        {
                            if (this.itemUpdater != null)
                            {
                                this.itemUpdater(listItem[i].Item);
                                listItem[i].Updated = false;
                            }
                        }
                    }
                }

                if (isScrollSyncEnabled)
                {
                    if (Scrolling != null)
                    {
                        Scrolling(this, null);
                    }
                }

                UpdateScrollBar();
            }

            private ListContainer GetContainer()
            {
                ListContainer container = new ListContainer();

                if (poolItem.Count != 0)
                {
                    container = poolItem[0];
                    poolItem.RemoveAt(0);
                    container.Item.Visible = true;
                    container.Item.TouchResponse = true;
                }
                else
                {
                    container = new ListContainer();
                    container.Item = null;
                }

                return container;
            }

            private void PoolContainer(ref ListContainer container)
            {
                if (container != null)
                {
                    poolItem.Add(container);
                    container.Item.Visible = false;
                }
            }

            private void CreateListItem(ref ListContainer listContainer, int index)
            {
                UIDebug.Assert(index >= 0 && index < itemCount, "GlidList.CreateListItem index is OutOfRange");

                ListPanelItem item;

                if (this.itemCreator == null)
                    return;

                if (listContainer.Item == null)
                {
                    item = this.itemCreator();
                    item.PressStateChanged += itemPressStateChangedEventHandler;
                }
                else
                {
                    item = listContainer.Item;
                }

                listContainer.Item = item;
                listContainer.TotalIndex = index;
                listContainer.Updated = true;

                item.Width = this.ItemWidth;
                item.Height = this.ItemHeight;
                item.Index = index;
                this.basePanel.AddChildLast(item);
            }
            
            void itemPressStateChangedEventHandler(object sender, PressStateChangedEventArgs e)
            {
                UIDebug.Assert(sender is ListPanelItem);

                var item = sender as ListPanelItem;
                if (itemUpdater != null)
                {
                    itemUpdater(item);
                }
                if (e.checkDetectedButtonAction() && SelectItemChanged != null)
                {
                    SelectItemChanged(this, new ListPanelItemSelectChangedEventArgs(item));
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムが選択された場合に呼び出されるイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event called when item is selected</summary>
            /// @endif
            public event EventHandler<ListPanelItemSelectChangedEventArgs> SelectItemChanged;

            /// @if LANG_JA
            /// <summary>事前キーイベントのハンドラ</summary>
            /// <param name="keyEvent">キーイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Advance key event handler</summary>
            /// <param name="keyEvent">Key event</param>
            /// @endif
            internal protected override void OnPreviewKeyEvent(KeyEvent keyEvent)
            {
                // TODO: marge ScrollPanel
                if (keyEvent.KeyEventType == KeyEventType.Down ||
                    keyEvent.KeyEventType == KeyEventType.Repeat)
                {
                    FourWayDirection direction = FourWayDirection.Down;
                    if (KeyEvent.TryConvertToDirection(keyEvent.KeyType, ref direction))
                    {
                        var scene = ParentScene;
                        if (scene == null)
                            return;

                        var w = scene.FocusWidget.SearchNextFocus(direction);
                        keyEvent.Handled = true;

                        if (w != null && w.belongTo(this) && w.getClippedState() == ClippedState.NoClipped)
                        {
                            w.SetFocus(true);
                        }
                        else
                        {
                            bool moved = scrollByFourWayKey(direction);
                            if (moved)
                            {
                                w = scene.FocusWidget.SearchNextFocus(direction);
                                if (w != null && w.belongTo(this))
                                {
                                    w.SetFocus(true);
                                }
                                //else if (!(scene.FocusWidget.GetAllFocusable() &&
                                //          scene.FocusWidget.getClippedState() == ClippedState.NoClipped))
                                //{
                                //    this.SetFocus(true);
                                //}
                            }
                            else
                            {
                                if (w != null)
                                {
                                    w.SetFocus(true);
                                }
                            }
                        }
                          
                        keyEvent.Handled = true;

                    }
                }


                base.OnPreviewKeyEvent(keyEvent);
            }

            internal override void OnFocusChangedChild(FocusChangedEventArgs args)
            {
                if (args.Focused && !args.OldFocusWidget.belongTo(this))
                {
                    var item = getFocusedItem();
                    if (item != null)
                    {
                        ListPanelItem nextFocusItem = null;
                        if (this.ScrollOrientation == GridListScrollOrientation.Vertical)
                        {
                            if (item.Y + this.basePanel.Y < 0)
                            {
                                nextFocusItem = GetListItem(item.Index + this.scrollAreaStepCount);
                            }
                            else if (item.Y + item.Height + this.basePanel.Y > this.Height)
                            {
                                nextFocusItem = GetListItem(item.Index - this.scrollAreaStepCount);
                            }
                        }
                        else
                        {
                            if (item.X + this.basePanel.X < 0)
                            {
                                nextFocusItem = GetListItem(item.Index + this.scrollAreaLineCount);
                            }
                            else if (item.X + item.Width + this.basePanel.X > this.Width)
                            {
                                nextFocusItem = GetListItem(item.Index - this.scrollAreaLineCount);
                            }
                        }

                        if (nextFocusItem != null)
                        {
                            nextFocusItem.SetFocus(true);
                        }
                    }
                }
            }

            private ListPanelItem getFocusedItem()
            {
                var scene = this.ParentScene;
                if (scene != null && scene.FocusWidget != null)
                {
                    foreach (var item in listItem)
                    {
                        if (item.Item == scene.FocusWidget || scene.FocusWidget.belongTo(item.Item))
                        {
                            return item.Item;
                        }
                    }
                }
                return null;
            }

            private bool scrollByFourWayKey(FourWayDirection direction)
            {
                const float epsilon = 0.9999999f;
                if (ScrollOrientation == GridListScrollOrientation.Horizontal)
                {
                    if (direction == FourWayDirection.Right)
                    {
                        if (this.scrollAreaFirstIndex < this.MaxScrollAreaFirstIndex)
                        {
                            this.ScrollTo((this.scrollAreaFirstIndex + 1)*this.scrollAreaLineCount, this.ItemPixelOffset, false);
                            return true;
                        }
                        else if (this.ItemPixelOffset < this.MaxScrollAreaFirstOffset - epsilon)
                        {
                            this.ScrollTo(this.MaxScrollAreaFirstIndex * this.scrollAreaLineCount, this.MaxScrollAreaFirstOffset, false);
                            return true;
                        }
                    }
                    else if (direction == FourWayDirection.Left)
                    {
                        if (this.scrollAreaFirstIndex > 0)
                        {
                            this.ScrollTo((this.scrollAreaFirstIndex - 1) * this.scrollAreaLineCount, this.ItemPixelOffset, false);
                            return true;
                        }
                        else if ( this.ItemPixelOffset > epsilon)
                        {
                            this.ScrollTo(0, 0, false);
                            return true;
                        }
                    }
                }
                else
                {
                    if (direction == FourWayDirection.Down)
                    {
                        if (this.scrollAreaFirstIndex < this.MaxScrollAreaFirstIndex || this.ItemPixelOffset < this.MaxScrollAreaFirstOffset - epsilon)
                        {
                            this.ScrollTo((this.scrollAreaFirstIndex + 1) * this.scrollAreaStepCount, this.MaxScrollAreaFirstOffset, false);
                            return true;
                        }
                    }
                    else if (direction == FourWayDirection.Up)
                    {
                        if (this.scrollAreaFirstIndex > 0 || this.ItemPixelOffset > epsilon)
                        {
                            this.ScrollTo((this.scrollAreaFirstIndex - 1) * this.scrollAreaStepCount, 0, false);
                            return true;
                        }
                    }
                }
                return false;
                //this.ScrollTo(item.Index, focusMoveMargin, true);
            }

            private int MaxScrollAreaFirstIndex
            {
                get
                {
                    int lineIndex;
                    if (ScrollOrientation == GridListScrollOrientation.Vertical)
                        lineIndex = this.TotalLineCount - this.scrollAreaLineCount;
                    else
                        lineIndex = this.TotalStepCount - this.scrollAreaStepCount;

                    if (lineIndex < 0) lineIndex = 0;

                    return lineIndex;
                }
            }

            private float MaxScrollAreaFirstOffset
            {
                get
                {
                    float offset = 0;

                    switch (this.ScrollOrientation)
                    {
                        case GridListScrollOrientation.Vertical:
                            float remainderV = (this.Height - this.ItemVerticalGap) % (this.ItemHeight + this.ItemVerticalGap);
                            if (remainderV > 0)
                            {
                                offset = -(remainderV - (this.ItemHeight + this.ItemVerticalGap));
                            }
                            break;

                        case GridListScrollOrientation.Horizontal:
                            float remainderH = (this.Width - this.ItemHorizontalGap) % (this.ItemWidth + this.ItemHorizontalGap);
                            if (remainderH > 0)
                            {
                                offset = -(remainderH - (this.ItemWidth + this.ItemHorizontalGap));
                            }
                            break;
                    }

                    return offset;
                }
            }

            private int CalcStepIndex(int index)
            {
                if (index <= 0)
                {
                    return 0;
                }

                int stepIndex = 0;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        stepIndex = index % this.scrollAreaStepCount;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        stepIndex = index / this.scrollAreaLineCount;
                        break;
                }
                return stepIndex;
            }

            private int CalcLineIndex(int index)
            {
                if (index <= 0)
                {
                    return 0;
                }

                int lineIndex = 0;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        lineIndex = index / this.scrollAreaStepCount;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        lineIndex = index % this.scrollAreaLineCount;
                        break;
                }
                return lineIndex;
            }

            private float CalcItemPosOnScrollNodeX(int totalIndex)
            {
                float pos = 0.0f;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        pos = CalcStepIndex(totalIndex) * (this.itemWidth + this.ItemHorizontalGap) + this.horizontalEdgeMargin;
                        break;

                    case GridListScrollOrientation.Horizontal:
                        if (this.terminalState == TerminalState.Top || this.terminalState == TerminalState.TopReset)
                        {
                            pos = CalcStepIndex(totalIndex) * (this.itemWidth + this.ItemHorizontalGap + this.terminalDistance * terminalDistanceRatio) + this.horizontalEdgeMargin;
                        }
                        else if (this.terminalState == TerminalState.Bottom || this.terminalState == TerminalState.BottomReset)
                        {
                            pos = (CalcStepIndex(totalIndex) - this.MaxScrollAreaFirstIndex) * (this.itemWidth + this.ItemHorizontalGap) + this.horizontalEdgeMargin;
                            pos -= (CalcStepIndex(this.ItemCount - 1) - CalcStepIndex(totalIndex)) * this.terminalDistance * terminalDistanceRatio;
                        }
                        else
                        {
                            pos = (CalcStepIndex(totalIndex) - this.scrollAreaFirstIndex) * (this.itemWidth + this.ItemHorizontalGap) + this.horizontalEdgeMargin;
                        }
                        break;
                }
                return pos;
            }

            private float CalcItemPosOnScrollNodeY(int totalIndex)
            {
                float pos = 0.0f;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        if (this.terminalState == TerminalState.Top || this.terminalState == TerminalState.TopReset)
                        {
                            pos = CalcLineIndex(totalIndex) * (this.itemHeight + this.ItemVerticalGap + this.terminalDistance * terminalDistanceRatio) +  + this.verticalEdgeMargin;
                        }
                        else if (this.terminalState == TerminalState.Bottom || this.terminalState == TerminalState.BottomReset)
                        {
                            pos = (CalcLineIndex(totalIndex) - this.MaxScrollAreaFirstIndex) * (this.itemHeight + this.ItemVerticalGap) + this.verticalEdgeMargin;
                            pos -= (CalcLineIndex(this.ItemCount - 1) - CalcLineIndex(totalIndex)) * this.terminalDistance * terminalDistanceRatio;
                        }
                        else
                        {
                            pos = (CalcLineIndex(totalIndex) - this.scrollAreaFirstIndex) * (this.itemHeight + this.ItemVerticalGap) + this.verticalEdgeMargin;
                        }
                        break;

                    case GridListScrollOrientation.Horizontal:
                        pos = CalcLineIndex(totalIndex) * (this.itemHeight + this.ItemVerticalGap) + this.verticalEdgeMargin;
                        break;
                }
                return pos;
            }

            private void SetNodePos(float pos)
            {
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        this.basePanel.Y = -pos;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        this.basePanel.X = -pos;
                        break;
                }
            }

            private float GetVirtualVec(Vector2 realVec)
            {
                float vec = 0.0f;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        vec = -realVec.Y;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        vec = -realVec.X;
                        break;
                }
                return vec;
            }

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);

                if (!isSetup)
                {
                    return;
                }

                if (IsScrollable() == false) {
                    touchEvents.Forward = true;
                    return;
                }

                if (animationState != AnimationState.Drag &&
                    animationState != AnimationState.Flick)
                {
                    touchEvents.Forward = true;
                }

                switch (touchEvents.PrimaryTouchEvent.Type)
                {
                    case TouchEventType.Down:
                        if (this.terminalState != TerminalState.None)
                        {
                            ResetTerminalAnimation();
                        }

                        this.animationState = AnimationState.None;
                        this.animation = false;
                        break;

                    case TouchEventType.Up:
                        if (animationState == AnimationState.Drag)
                        {
                            if (this.terminalState == TerminalState.Top)
                            {
                                this.terminalState = TerminalState.TopReset;
                                this.animationState = AnimationState.None;
                                this.animation = true;
                            }
                            else if (this.terminalState == TerminalState.Bottom)
                            {
                                this.terminalState = TerminalState.BottomReset;
                                this.animationState = AnimationState.None;
                                this.animation = true;
                            }
                            else
                            {
                                if (this.SnapScroll)
                                {
                                    setupSnap();
                                }
                            }
                        }
                        updateScrollBarVisible();
                        break;
                }
            }

            private void setupSnap()
            {
                float itemSize = 0;
                switch (this.ScrollOrientation)
                {
                    case GridListScrollOrientation.Vertical:
                        itemSize = this.ItemHeight + this.ItemVerticalGap;
                        break;
                    case GridListScrollOrientation.Horizontal:
                        itemSize = this.ItemWidth + this.ItemHorizontalGap;
                        break;
                }

                if (this.scrollAreaFirstIndex == this.MaxScrollAreaFirstIndex
                        && this.MaxScrollAreaFirstOffset != 0.0f)
                {
                    itemSize = this.MaxScrollAreaFirstOffset;
                }

                if (this.ItemPixelOffset > itemSize * 0.5f)
                {
                    fitTargetIndex = this.scrollAreaFirstIndex + 1;
                }
                else
                {
                    fitTargetIndex = this.scrollAreaFirstIndex;
                }
                fitTargetOffsetPixel = 0.0f;

                if (fitTargetIndex > this.MaxScrollAreaFirstIndex)
                {
                    fitTargetIndex = this.MaxScrollAreaFirstIndex;
                    fitTargetOffsetPixel = this.MaxScrollAreaFirstOffset;
                }

                animationState = AnimationState.Fit;
                fitInterpRatio = 0.2f;
                this.animation = true;
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                if (IsScrollable() == false) {
                    return;
                }

                isScrollSyncEnabled = true;
                animationState = AnimationState.Drag;

                float currentOffset = GetVirtualVec(e.Distance) + this.ItemPixelOffset;
                float dragDistance = GetVirtualVec(e.Distance);

                int currentIndex = this.scrollAreaFirstIndex;
                NormalizeScrollAreaFirstIndex(ref currentIndex, ref currentOffset);

                if (IsUnderOrEqualMinScrollArea(currentIndex, currentOffset))
                {
                    // under
                    this.terminalDistance -= dragDistance;
                    UpdateTerminalAnimation(TerminalState.Top);
                }
                else if (IsOverOrEqualMaxScrollArea(currentIndex, currentOffset))
                {
                    // over
                    this.terminalDistance += dragDistance;
                    UpdateTerminalAnimation(TerminalState.Bottom);
                }
                else
                {
                    if (this.terminalState == TerminalState.Top)
                    {
                        if (this.terminalDistance <= 0.0f)
                        {
                            this.terminalDistance = 0.0f;
                            UpdateTerminalAnimation(TerminalState.None);
                        }
                        else
                        {
                            this.terminalDistance -= dragDistance;
                            UpdateTerminalAnimation(this.terminalState);
                        }
                    }
                    else if (this.terminalState == TerminalState.Bottom)
                    {
                        if (this.terminalDistance <= 0.0f)
                        {
                            this.terminalDistance = 0.0f;
                            UpdateTerminalAnimation(TerminalState.None);
                        }
                        else
                        {
                            this.terminalDistance += dragDistance;
                            UpdateTerminalAnimation(this.terminalState);
                        }
                    }
                    else
                    {
                        SetScrollAreaFirstIndex(currentIndex, currentOffset);
                    }
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                if (IsScrollable() == false) {
                    return;
                }

                isScrollSyncEnabled = true;
                animationState = AnimationState.Flick;

                float flickSpeed = GetVirtualVec(e.Speed);
                if (this.terminalState == TerminalState.None) {
                    scrollVelocity = flickSpeed * flickStartRatio;
                }
                else {
                    scrollVelocity = 0.0f;
                }
                
                if (this.terminalState == TerminalState.Top)
                {
                    if (scrollVelocity > 0.0f)
                    {
                        ResetTerminalAnimation();
                    }
                    else
                    {
                        this.terminalDistance += scrollVelocity;
                        UpdateTerminalAnimation(this.terminalState);
                    }
                }
                else if (this.terminalState == TerminalState.Bottom)
                {
                    if (scrollVelocity < 0.0f)
                    {
                        ResetTerminalAnimation();
                    }
                    else
                    {
                        this.terminalDistance += scrollVelocity;
                        UpdateTerminalAnimation(this.terminalState);
                    }
                }
                else
                {
                    if (this.SnapScroll)
                    {
                        float itemSize = 0;
                        switch (this.ScrollOrientation)
                        {
                            case GridListScrollOrientation.Vertical:
                                itemSize = this.ItemHeight + this.ItemVerticalGap;
                                break;
                            case GridListScrollOrientation.Horizontal:
                                itemSize = this.ItemWidth + this.ItemHorizontalGap;
                                break;
                        }

                        if (scrollVelocity > 0.0f)
                        {
                            if (itemSize * 2.0 - this.ItemPixelOffset - scrollVelocity / flickDecelerationRatio > 0.0f)
                            {
                                animationState = AnimationState.Fit;

                                if (itemSize - this.ItemPixelOffset != 0)
                                {
                                    fitInterpRatio = scrollVelocity / (itemSize - this.ItemPixelOffset);
                                }
                                else
                                {
                                    fitInterpRatio = 1.0f;
                                }

                                if (this.scrollAreaFirstIndex >= this.MaxScrollAreaFirstIndex)
                                {
                                    fitTargetIndex = this.scrollAreaFirstIndex;
                                }
                                else
                                {
                                    fitTargetIndex = this.scrollAreaFirstIndex + 1;
                                }
                                fitTargetOffsetPixel = 0.0f;
                            }
                        }
                        else
                        {
                            if (itemSize + this.ItemPixelOffset + scrollVelocity / flickDecelerationRatio > 0.0)
                            {
                                animationState = AnimationState.Fit;

                                if (this.ItemPixelOffset != 0)
                                {
                                    fitInterpRatio = -scrollVelocity / this.ItemPixelOffset;
                                }
                                else
                                {
                                    fitInterpRatio = 1.0f;
                                }

                                fitTargetIndex = this.scrollAreaFirstIndex;
                                fitTargetOffsetPixel = 0.0f;
                            }
                        }
                    }
                }

                this.animation = true;
            }

            private bool IsOverOrEqualMaxScrollArea(int index, float offset)
            {
                return index > this.MaxScrollAreaFirstIndex || (index == this.MaxScrollAreaFirstIndex && offset >= this.MaxScrollAreaFirstOffset);
            }
            private bool IsUnderOrEqualMinScrollArea(int index, float offset)
            {
                return index < 0 || (index == 0 && offset <= 0.0f);
            }

            /// @if LANG_JA
            /// <summary>状態リセットハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Status reset handler</summary>
            /// @endif
            protected internal override void OnResetState()
            {
                base.OnResetState();

                animationState = AnimationState.None;

                if (this.terminalState == TerminalState.Top)
                {
                    this.terminalState = TerminalState.TopReset;
                }
                else if (this.terminalState == TerminalState.Bottom)
                {
                    this.terminalState = TerminalState.BottomReset;
                }

                if (this.terminalState != TerminalState.None)
                {
                    animation = true;
                }
                else
                {
                    if (this.SnapScroll)
                    {
                        setupSnap();
                    }
                    else
                    {
                        updateScrollBarVisible();
                        animation = false;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// @endif
            protected override void OnUpdate(float elapsedTime)
            {
                base.OnUpdate(elapsedTime);

                if (!animation) return;

                if (isSetup == false)
                {
                    StartItemRequest();
                }

                switch (this.animationState)
                {
                    case AnimationState.Flick:
                        if (Math.Abs(scrollVelocity * elapsedTime / 16.6f) < 5.0f / 16.6f || Math.Abs(terminalDistance) > maxTerminalDistance)
                        {
                            if (this.terminalState == TerminalState.Top)
                            {
                                UpdateTerminalAnimation(TerminalState.TopReset);
                                if (this.terminalState == TerminalState.None)
                                {
                                    break;
                                }
                            }
                            else if (this.terminalState == TerminalState.Bottom)
                            {
                                UpdateTerminalAnimation(TerminalState.BottomReset);
                                if (this.terminalState == TerminalState.None)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (!this.SnapScroll)
                                {
                                    scrollVelocity = 0.0f;
                                    animationState = AnimationState.None;
                                    this.animation = false;
                                }
                            }
                        }

                        float distance = scrollVelocity * elapsedTime / 16.6f;
                        float currentOffset = distance + this.ItemPixelOffset;

                        int currentIndex = this.scrollAreaFirstIndex;
                        NormalizeScrollAreaFirstIndex(ref currentIndex, ref currentOffset);

                        if (IsUnderOrEqualMinScrollArea(currentIndex, currentOffset))
                        {
                            // under
                            if (this.terminalState == TerminalState.BottomReset || this.terminalState == TerminalState.TopReset) {
                                this.terminalDistance = this.terminalDistance * (float)Math.Pow((1.0f - terminalReturnRatio), elapsedTime / 16.6f);
                                UpdateTerminalAnimation(TerminalState.Top);
                            } else {
                                this.terminalDistance -= distance * 2.0f;
                                UpdateTerminalAnimation(TerminalState.Top);
                            }

                        }
                        else if (IsOverOrEqualMaxScrollArea(currentIndex, currentOffset))
                        {
                            // over
                            if (this.terminalState == TerminalState.BottomReset || this.terminalState == TerminalState.TopReset)
                            {
                                this.terminalDistance = this.terminalDistance * (float)Math.Pow((1.0f - terminalReturnRatio), elapsedTime / 16.6f);
                                UpdateTerminalAnimation(TerminalState.Bottom);
                            }
                            else
                            {
                                this.terminalDistance += distance * 2.0f;
                                UpdateTerminalAnimation(TerminalState.Bottom);
                            }
                        }
                        else
                        {
                            SetScrollAreaFirstIndex(currentIndex, currentOffset);
                        }

                        if (terminalState == TerminalState.None) {
                            scrollVelocity = scrollVelocity * (float)Math.Pow((1.0f - flickDecelerationRatio), elapsedTime / 16.6f);
                        }
                        else {
                            scrollVelocity = scrollVelocity * (float)Math.Pow((1.0f - terminalDecelerationRatio), elapsedTime / 16.6f);
                        }

                        if (this.terminalState == TerminalState.None && this.SnapScroll)
                        {
                            float itemSize = 0;
                            switch (this.ScrollOrientation)
                            {
                                case GridListScrollOrientation.Vertical:
                                    itemSize = this.ItemHeight + this.ItemVerticalGap;
                                    break;
                                case GridListScrollOrientation.Horizontal:
                                    itemSize = this.ItemWidth + this.ItemHorizontalGap;
                                    break;
                            }

                            if (scrollVelocity > 0.0f)
                            {
                                if ((this.itemHeight + this.ItemVerticalGap) - this.ItemPixelOffset - scrollVelocity * elapsedTime / 16.6f < 0)
                                {
                                    if (itemSize * 3.0f - this.ItemPixelOffset - scrollVelocity * elapsedTime / 16.6f / flickDecelerationRatio > 0.0f)
                                    {
                                        animationState = AnimationState.Fit;
                                        fitInterpRatio = scrollVelocity * elapsedTime / 16.6f / ((this.itemHeight + this.ItemVerticalGap) * 2.0f - this.ItemPixelOffset);
                                        fitTargetIndex = this.scrollAreaFirstIndex + 2;
                                        fitTargetOffsetPixel = 0.0f;
                                    }
                                }
                            }
                            else
                            {
                                if (this.ItemPixelOffset + scrollVelocity * elapsedTime / 16.6f < 0.0f)
                                {
                                    if (itemSize * 2.0f + this.ItemPixelOffset + scrollVelocity * elapsedTime / 16.6f / flickDecelerationRatio > 0.0f)
                                    {
                                        animationState = AnimationState.Fit;
                                        fitInterpRatio = -scrollVelocity * elapsedTime / 16.6f / ((this.itemHeight + this.ItemVerticalGap) * 1.0f + this.ItemPixelOffset);
                                        fitTargetIndex = this.scrollAreaFirstIndex - 1;
                                        fitTargetOffsetPixel = 0.0f;
                                    }
                                }
                            }
                        }
                        break;

                    case AnimationState.Fit:
                        int diffIndex = this.scrollAreaFirstIndex - this.fitTargetIndex;

                        float diffIndexReal = diffIndex * (float)Math.Pow((1.0f - this.fitInterpRatio), elapsedTime/16.6f);
                        diffIndex = (int)(diffIndexReal);

                        float diffPixel = 0.0f;
                        switch (this.ScrollOrientation)
                        {
                            case GridListScrollOrientation.Vertical:
                                diffPixel = (diffIndexReal - diffIndex) * (this.itemHeight + this.ItemVerticalGap);
                                diffPixel += (this.ItemPixelOffset - this.fitTargetOffsetPixel) * (float)Math.Pow((1.0f - this.fitInterpRatio), elapsedTime/16.6f);
                                break;
                            case GridListScrollOrientation.Horizontal:
                                diffPixel = (diffIndexReal - diffIndex) * (this.itemWidth + this.itemHorizontalGap);
                                diffPixel += (this.ItemPixelOffset - this.fitTargetOffsetPixel) * (float)Math.Pow((1.0f - this.fitInterpRatio), elapsedTime/16.6f);
                                break;
                        }

                        SetScrollAreaFirstIndex(diffIndex + fitTargetIndex, diffPixel + fitTargetOffsetPixel);

                        if (diffIndex == 0 && Math.Abs(diffPixel) <= 0.5f)
                        {
                            animationState = AnimationState.None;
                            SetScrollAreaFirstIndex(fitTargetIndex, fitTargetOffsetPixel);
                            this.animation = false;
                        }

                        if (IsOverOrEqualMaxScrollArea(this.scrollAreaFirstIndex, this.ItemPixelOffset))
                        {
                            animationState = AnimationState.None;
                            SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                            this.animation = false;
                        }
                        break;

                    case AnimationState.None:
                        this.terminalDistance = this.terminalDistance * (float)Math.Pow((1.0f - terminalReturnRatio), elapsedTime/16.6f);
                        UpdateTerminalAnimation(this.terminalState);
                        break;
                }
            }

            private void UpdateTerminalAnimation(TerminalState state)
            {
                if (Math.Abs(this.terminalDistance) > maxTerminalDistance) {
                    if (this.terminalDistance > 0)
                    {
                        this.terminalDistance = maxTerminalDistance;
                        this.terminalState = TerminalState.TopReset;
                        this.scrollVelocity = 0.0f;
                    }
                    else
                    {
                        this.terminalDistance = -maxTerminalDistance;
                        this.terminalState = TerminalState.BottomReset;
                        this.scrollVelocity = 0.0f;
                    }
                }

                this.terminalState = state;

                switch (this.terminalState)
                {
                    case TerminalState.Top:
                        SetScrollAreaFirstIndex(0, 0.0f);
                        break;

                    case TerminalState.Bottom:
                        SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                        break;

                    case TerminalState.TopReset:
                        if (this.terminalDistance < 0.5f)
                        {
                            this.terminalState = TerminalState.None;
                            this.terminalDistance = 0.0f;
                            this.animationState = AnimationState.None;
                            this.animation = false;
                        }
                        SetScrollAreaFirstIndex(0, 0.0f);
                        break;

                    case TerminalState.BottomReset:
                        if (this.terminalDistance < 0.5f)
                        {
                            this.terminalState = TerminalState.None;
                            this.terminalDistance = 0.0f;
                            this.animationState = AnimationState.None;
                            this.animation = false;
                        }
                        SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                        break;
                }

                UpdateScrollBar();
            }

            private void ResetTerminalAnimation()
            {
                switch (this.terminalState)
                {
                    case TerminalState.Top:
                    case TerminalState.TopReset:
                        this.terminalDistance = 0.0f;
                        SetScrollAreaFirstIndex(0, 0.0f);
                        this.terminalState = TerminalState.None;
                        break;

                    case TerminalState.Bottom:
                    case TerminalState.BottomReset:
                        this.terminalDistance = 0.0f;
                        SetScrollAreaFirstIndex(this.MaxScrollAreaFirstIndex, this.MaxScrollAreaFirstOffset);
                        this.terminalState = TerminalState.None;
                        break;
                }
            }

            private void UpdateScrollBar()
            {
                if (this.scrollBar.Orientation == ScrollBarOrientation.Vertical)
                {
                    float itemHeightWithGap = this.itemHeight + this.itemVerticalGap;
                    this.scrollBar.BarPosition = this.scrollAreaFirstIndex + this.ItemPixelOffset / itemHeightWithGap;
                    if (this.terminalState == TerminalState.Bottom || this.terminalState == TerminalState.BottomReset)
                    {
                        for (int i = 0; i < this.scrollAreaLineCount; i++)
                        {
                            this.scrollBar.BarPosition += (this.terminalDistance * terminalDistanceRatio / itemHeightWithGap);
                        }
                    }
                    this.scrollBar.BarLength = this.Height / (itemHeightWithGap + Math.Abs(terminalDistance) * terminalDistanceRatio);
                    this.scrollBar.Length = this.TotalLineCount;
                }
                else
                {
                    float itemWidthWithGap = this.itemWidth + this.itemHorizontalGap;
                    this.scrollBar.BarPosition = this.scrollAreaFirstIndex + this.ItemPixelOffset / itemWidthWithGap;
                    if (this.terminalState == TerminalState.Bottom || this.terminalState == TerminalState.BottomReset)
                    {
                        for (int i = 0; i < this.scrollAreaStepCount; i++)
                        {
                            this.scrollBar.BarPosition += (this.terminalDistance * terminalDistanceRatio / itemWidthWithGap);
                        }
                    }
                    this.scrollBar.BarLength = this.Width / (itemWidthWithGap + Math.Abs(terminalDistance) * terminalDistanceRatio);
                    this.scrollBar.Length = this.TotalStepCount;
                }

                AddChildLast(this.scrollBar);

                updateScrollBarVisible();
            }

            private void updateScrollBarVisible()
            {
                switch (this.scrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        this.scrollBar.Visible = true;
                        break;
                    case ScrollBarVisibility.ScrollableVisible:
                        this.scrollBar.Visible = IsScrollable();
                        break;
                    case ScrollBarVisibility.ScrollingVisible:
                        this.scrollBar.Visible = (animationState != AnimationState.None) || (terminalState != TerminalState.None);
                        break;
                    case ScrollBarVisibility.Invisible:
                        this.scrollBar.Visible = false;
                        break;
                }
            }

            private Boolean IsScrollable() {
                switch (this.ScrollOrientation)
                {
                case GridListScrollOrientation.Vertical:
                    float allHeight = (itemHeight + this.ItemVerticalGap) * this.TotalLineCount;
                    if (allHeight <= Height) {
                        return false;
                    }
                    break;
                case GridListScrollOrientation.Horizontal:
                    float allWidth = (itemWidth + this.ItemHorizontalGap) * this.TotalStepCount;
                    if (allWidth <= Width) {
                        return false;
                    }
                    break;
                }
                return true;
            }

            /// @if LANG_JA
            /// <summary>アイテムを作成するメソッドを登録する。</summary>
            /// <param name="creator">アイテムを作成するメソッド</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Registers the method of creating items.</summary>
            /// <param name="creator">Method of creating items</param>
            /// @endif
            public void SetListItemCreator(ListItemCreator creator)
            {
                this.itemCreator = creator;
            }

            /// @if LANG_JA
            /// <summary>アイテムを更新するメソッドを登録する。</summary>
            /// <param name="updater">アイテムを更新するメソッド</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Registers the method of updating items.</summary>
            /// <param name="updater">Method of updating items</param>
            /// @endif
            public void SetListItemUpdater(ListItemUpdater updater)
            {
                this.itemUpdater = updater;
            }

            /// @if LANG_JA
            /// <summary>スクロール中に呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called during scrolling</summary>
            /// @endif
            public event EventHandler<TouchEventArgs> Scrolling;

            private class ListContainer
            {

                public ListContainer()
                {
                    this.Item = null;
                    this.TotalIndex = 0;
                    this.Updated = false;
                }

                public ListPanelItem Item
                {
                    get;
                    set;
                }

                public int TotalIndex
                {
                    get;
                    set;
                }

                public bool Updated
                {
                    get;
                    set;
                }

            }


            private UISprite backgroundSprt;
            private Panel basePanel;

            private int scrollAreaFirstIndex;
            private int scrollAreaStepCount;
            private int scrollAreaLineCount;

            private bool isSetup;
            private bool isScrollSyncEnabled;

            private int cacheLineCount;
            private int cacheStartIndex;
            private int cacheEndIndex;

            private int fitTargetIndex;
            private float fitTargetOffsetPixel;
            private float fitInterpRatio;

            private AnimationState animationState;
            private TerminalState terminalState;
            private bool animation = false;

            private float scrollVelocity;
            private float terminalDistance;

            private List<ListContainer> listItem;
            private List<ListContainer> poolItem;

            private ListItemCreator itemCreator;
            private ListItemUpdater itemUpdater;

            private ScrollBar scrollBar;
   
            private float flickStartRatio = 0.022f;
            private float flickDecelerationRatio = 0.012f;
            private float terminalDistanceRatio = 0.073f;
            private float terminalDecelerationRatio = 0.48f;
            private float maxTerminalDistance = 240.0f;
            private float terminalReturnRatio = 0.09f;

            private const float minDelayDragDist = 0.5f;

            private const float defaultWidth = 100.0f;
            private const float defaultHeight = 100.0f;

            private float verticalEdgeMargin;
            private float horizontalEdgeMargin;
        }


    }
}
