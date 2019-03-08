/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>アイテムが不規則な配置でスクロールするリストウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List widgets scrolled by irregular position of items.</summary>
        /// @endif
        public class LiveListPanel : Widget
        {
            // const
            private const float defaultLiveListPanelWidth = 320.0f;
            private const float defaultLiveListPanelHeight = 480.0f;
            private const float defaultItemLineGap = 32.0f;
            private const float defaultItemHeight = 95.0f;

            // parameter
            float animationTime = 1500.0f;
            float itemSideMargin = 10;

            float itemTiltAngle = 0.1f;
            float itemOffsetX = 10f;

            float itemSlideInOffsetX = 200f;
            float itemSlideInHeight = -500f;
            float itemSlideInRotationY = 0.0f;
            float itemSlideInTime = 500f;
            float itemSlideInDelay = 100f;

            float terminalAnimationFactor = 1f;
            float terminalDecay = 2f;

            // child element
            private ContainerWidget itemContainerPanel;
            private ScrollBar scrollBar;
            private UISprite sprt;

            // other
            private ListItemCreator itemCreator;
            private ListItemUpdater itemUpdater;

            private List<ListItemData> usingItemDataList;
            private List<ListItemData> cacheItemDataList;

            private FourWayDirection scrollDirection;
            private FourWayDirection prevScrollDirection;
            private AnimationState animationState;
            private float flickDistance;
            private float flickStartDistance;
            private float flickElapsedTime;
            float terminalResetElapsedTime = 0f;
            float scrollPosition = 0f;
            
            int scrollBaseIndex = -1;

            bool needRefresh = true;
            bool isScrolling = false;
            
            private float scrollGravity = 0.3f;
            private int slideInRotationDelta = 1;
            private float slideInOffsetX = 0.0f;
            private bool isMoveX = true;
            private bool isMoveZ = true;
            private float rotationSpeed = 0.5f;

            private float[] elapsedTimes;
            private const int ELAPSED_TIME_COUNT = 60;
            private int elapsedTimeIndex = 0;
            private bool initElapsedTimes = false;

            static Random rand = new Random();

            private enum AnimationState
            {
                None = 0,
                Drag,
                Flick
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LiveListPanel()
            {
                this.sprt = new UISprite(1);
                this.RootUIElement.AddChildLast(this.sprt);

                this.itemContainerPanel = new ContainerWidget();
                this.itemContainerPanel.EnabledChildrenAnchors = false;
                this.itemContainerPanel.SetSize(float.MaxValue, float.MaxValue);
                this.AddChildLast(itemContainerPanel);

                this.scrollBar = new ScrollBar(ScrollBarOrientation.Vertical);
                this.AddChildLast(scrollBar);

                this.BackgroundColor = new UIColor();

                this.ItemCount = 0;
                this.ItemVerticalGap = defaultItemLineGap;
                this.ItemHeight = defaultItemHeight;

                this.usingItemDataList = new List<ListItemData>();
                this.cacheItemDataList = new List<ListItemData>();

                this.Width = defaultLiveListPanelWidth;
                this.Height = defaultLiveListPanelHeight;

                this.Clip = true;
                this.HookChildTouchEvent = true;

                //this.Focusable = false;

                DragGestureDetector drag = new DragGestureDetector();
                drag.Direction = DragDirection.Vertical;
                drag.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(drag);

                FlickGestureDetector flick = new FlickGestureDetector();
                flick.Direction = FlickDirection.Vertical;
                flick.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flick);
                
                elapsedTimes = new float[ELAPSED_TIME_COUNT];
            }


            #region property

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

                    if (this.sprt != null)
                    {
                        UISpriteUnit unit = this.sprt.GetUnit(0);
                        unit.Width = value;
                    }

                    if (scrollBar != null)
                    {
                        scrollBar.X = value - scrollBar.Width;
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

                    if (this.sprt != null)
                    {
                        UISpriteUnit unit = this.sprt.GetUnit(0);
                        unit.Height = value;
                    }

                    if (scrollBar != null)
                    {
                        scrollBar.Height = value;
                    }
                }
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
                    UISpriteUnit unit = this.sprt.GetUnit(0);
                    return unit.Color;
                }
                set
                {
                    UISpriteUnit unit = this.sprt.GetUnit(0);
                    unit.Color = value;
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムの高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of an item.</summary>
            /// @endif
            public float ItemHeight
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>アイテムの高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of an item.</summary>
            /// @endif
            public float ItemWidth
            {
                get
                {
                    return this.Width - itemSideMargin * 2;
                }
                set
                {
                    itemSideMargin = (this.Width - value) / 2f;
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
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>アイテムの行間を取得・取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of an item.</summary>
            /// @endif
            public float ItemVerticalGap
            {
                get;
                set;
            }

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
                    UpdateScrollBarVisible();
                }
            }
            private ScrollBarVisibility scrollBarVisibility = ScrollBarVisibility.ScrollingVisible;

            /// @if LANG_JA
            /// <summary>アイテムの最大傾き（ラジアン）。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Maximum tilt of an item (radian).</summary>
            /// @endif
            public float ItemTiltAngle
            {
                get { return itemTiltAngle; }
                set { itemTiltAngle = value; }
            }

            /// @if LANG_JA
            /// <summary>項目が横から入ってくるのにかかる時間（ミリ秒）。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Time required for an item to enter from the side (ms).</summary>
            /// @endif
            public float ItemSlideInTime
            {
                get { return itemSlideInTime; }
                set { itemSlideInTime = value; }
            }

            /// @if LANG_JA
            /// <summary>項目が横から入ってくるときのZの高さ。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Height of Z when an item enters from the side.</summary>
            /// @endif
            public float ItemSlideInHeight
            {
                get { return -itemSlideInHeight; }
                set { itemSlideInHeight = -value; }
            }

            /// @if LANG_JA
            /// <summary>項目が横から入ってくるときのY軸の角度。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Angle of the Y axis when an item enters from the side.</summary>
            /// @endif
            public float ItemSlideInTiltAngle
            {
                get { return itemSlideInRotationY; }
                set { itemSlideInRotationY = value; }
            }

            /// @if LANG_JA
            /// <summary>項目が横から入ってくるときの水平方向のオフセット。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Horizontal offset when an item enters from the side.</summary>
            /// @endif
            public float ItemSlideInOffset
            {
                get { return itemSlideInOffsetX; }
                set { itemSlideInOffsetX = value; }
            }

            #endregion

            /// @if LANG_JA
            /// <summary>アイテムの要求を開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the request of an item.</summary>
            /// @endif
            public void StartItemRequest()
            {
            }
            void refresh()
            {
                usingItemDataList.Clear();
                cacheItemDataList.Clear();
                scrollPosition = 0f;
                UpdateListItemData();
            }

            private int ScrollAreaItemCount
            {
                get
                {
                    float overPixel = 0;
                    if (0 < scrollPosition && scrollPosition < MaxScrollPosition)
                    {
                        overPixel = scrollPosition - this.LineHeight * ScrollAreaFirstItemIndex;
                    }
                    return (int)((this.Height + overPixel) / this.LineHeight + 1);
                }
            }

            private int ScrollAreaFirstItemIndex
            {
                get
                {
                    if (scrollPosition <= 0)
                    {
                        return 0;
                    }
                    if (scrollPosition > MaxScrollPosition)
                    {
                        int i = ItemCount - ScrollAreaItemCount;
                        return i > 0 ? i : 0;
                    }
                    return (int)(scrollPosition / LineHeight);
                }
            }

            private int ScrollAreaLastItemIndex
            {
                get
                {
                    if (ItemCount == 0)
                        return 0;
                    if (ItemCount < ScrollAreaItemCount)
                        return ItemCount - 1;
                    return ScrollAreaFirstItemIndex + ScrollAreaItemCount - 1;
                }
            }

            private float ScrollAreaFirstItemOffset
            {
                get
                {
                    return -(this.scrollPosition - this.LineHeight * ScrollAreaFirstItemIndex);
                }
            }

            private int MaxScrollAreaFirstItemIndex
            {
                get
                {
                    return Math.Max(this.ItemCount - this.ScrollAreaItemCount, 0);
                }
            }

            float MaxScrollPosition
            {
                get
                {
                    float max = LineHeight * ItemCount - this.Height;
                    return max > 0f ? max : 0f;
                }
            }

            private float LineHeight
            {
                get { return this.ItemHeight + this.ItemVerticalGap; }
            }

            private void UpdateScrollBar()
            {
                if (usingItemDataList.Count != 0)
                {
                    scrollBar.BarPosition = this.scrollPosition;
                    scrollBar.BarLength = this.Height;
                    scrollBar.Length = this.ItemCount * LineHeight;
                    if (scrollPosition < 0f)
                    {
                        scrollBar.BarLength /= (float)Math.Pow(2, -scrollPosition / Height);
                    }
                    else if (scrollPosition > MaxScrollPosition)
                    {
                        scrollBar.BarLength /= (float)Math.Pow(2, (scrollPosition - MaxScrollPosition) / Height);
                        scrollBar.BarPosition = scrollBar.Length;
                    }
                }
                UpdateScrollBarVisible();
            }

            private void UpdateScrollBarVisible()
            {
                switch (this.scrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        this.scrollBar.Visible = true;
                        break;
                    case ScrollBarVisibility.ScrollableVisible:
                        this.scrollBar.Visible = (usingItemDataList.Count < ItemCount);
                        break;
                    case ScrollBarVisibility.ScrollingVisible:
                        this.scrollBar.Visible = (animationState != AnimationState.None) || isScrolling;
                        break;
                    case ScrollBarVisibility.Invisible:
                        this.scrollBar.Visible = false;
                        break;
                }
            }

            private ListItemData CreateListItemData(int index)
            {
                if (itemCreator == null) return null;
                ListItemData data = null;
                if (this.cacheItemDataList.Count == 0)
                {
                    data = new ListItemData();
                    data.item = this.itemCreator();
                    data.item.PivotType = PivotType.MiddleCenter;
                }
                else
                {
                    data = this.cacheItemDataList[0];
                    this.cacheItemDataList.RemoveAt(0);
                }

                ListPanelItem item = data.item;
                item.Width = this.ItemWidth;
                item.Height = this.ItemHeight;
                item.Transform3D = Matrix4.Identity;
                item.Index = index;
                item.Visible = true;
                item.Y = item.Index * LineHeight + LineHeight / 2f - scrollPosition;
                if (itemUpdater != null)
                {
                    this.itemUpdater(item);
                }

                if ((this.usingItemDataList.Count == 0) || (index > this.ScrollAreaFirstItemIndex))
                {
                    if (!usingItemDataList.Exists(delegate(ListItemData listData) {return listData.item.Index == data.item.Index;}))
                    {
                        this.usingItemDataList.Add(data);
                    }
                    else
                    {
                        item.Visible = false;
                        cacheItemDataList.Add(data);
                    }
                }
                else
                {
                    if (!usingItemDataList.Exists(delegate(ListItemData listData) {return listData.item.Index == data.item.Index;}))
                    {
                        this.usingItemDataList.Insert(0, data);
                    }
                    else
                    {
                        item.Visible = false;
                        cacheItemDataList.Add(data);
                    }
                }
                SetupItemDataForSlide(data);
                return data;
            }

            private void DestroyListItemData(ListItemData data)
            {
                ListPanelItem item = data.item;
                item.Visible = false;

                this.usingItemDataList.Remove(data);
                if (!cacheItemDataList.Exists(delegate(ListItemData listData) {return listData.item.Index == data.item.Index;}))
                {
                    this.cacheItemDataList.Add(data);
                }
            }

            private void UpdateTransform3D(ListItemData data, float elapsedTime)
            {
                ListPanelItem item = data.item;

                Vector3 pos;
                float rotationZ = 0.0f;
                float rotationY = 0.0f;

                // Slide animation
                if (data.IsSliding)
                {
                    if (data.SlideElapsedTime > 0)
                    {
                        pos = new Vector3(
                            AnimationUtility.EaseOutQuadInterpolator(data.slideStartPos.X, data.slideEndPos.X, data.SlideElapsedTime / itemSlideInTime),
                            AnimationUtility.EaseOutQuadInterpolator(data.slideStartPos.Y, data.slideEndPos.Y, data.SlideElapsedTime / itemSlideInTime),
                            AnimationUtility.EaseOutQuintInterpolator(data.slideStartPos.Z, data.slideEndPos.Z, data.SlideElapsedTime / itemSlideInTime));
                        rotationZ = AnimationUtility.EaseOutQuadInterpolator(data.slideStartRotationZ, data.slideEndRotationZ, data.SlideElapsedTime / itemSlideInTime);
                        rotationY = AnimationUtility.EaseOutQuadInterpolator(data.slideStartRotationY, 0.0f, data.SlideElapsedTime / itemSlideInTime);
                    }
                    else
                    {
                        pos = data.slideStartPos;
                        rotationZ = data.slideStartRotationZ;
                        rotationY = data.slideStartRotationY;
                        if (this.scrollDirection == FourWayDirection.Up)
                        {
                            item.Y = data.slideStartPos.Y + Height;
                        }
                    }
                }
                else
                {
                    pos = data.slideEndPos;
                    rotationZ = data.slideEndRotationZ;
                    rotationY = 0.0f;
                }

                // Calc position Y 
                float scrollPos = scrollPosition < 0 ? 0f : (scrollPosition > MaxScrollPosition ? MaxScrollPosition : scrollPosition);
                if (animationState == AnimationState.Drag || animationState == AnimationState.Flick)
                {
                    if ((this.scrollDirection == FourWayDirection.Down && item.Index >= this.scrollBaseIndex) 
                        || (this.scrollDirection == FourWayDirection.Up && item.Index <= this.scrollBaseIndex))
                    {
                        pos.Y += item.Index * LineHeight + LineHeight / 2f - scrollPos;
                    }
                    else
                    {
                        pos.Y = GetItemPosYWithSpring(data, scrollPos, elapsedTime);
                    }
                }
                else
                {
                    pos.Y = GetItemPosYWithSpring(data, scrollPos, elapsedTime);
                }

                // Calc position X 
                pos.X += this.Width / 2f;
                if (isMoveX)
                {
                    pos.X += (data.scrollEndPosX - data.slideEndPos.X) * (pos.Y - data.slideEndPos.Y) / this.Height;
                }

                // Calc item angle
                if (isMoveZ)
                {
                    data.slideEndRotationZ = FMath.Lerp(data.scrollStartRotationZ, data.scrollEndRotationZ,
                        FMath.Clamp(data.totalScrollIncrement / (this.Height * (1.01f - rotationSpeed)), 0.0f, 1.0f));
                }
                
                // Calc edge vibration
                float rotationX = 0f;
                if (scrollPosition < -0.5f || scrollPosition > MaxScrollPosition + 0.5f)
                {
                    float factor = AnimationUtility.EaseOutQuadInterpolator(terminalAnimationFactor, 0, FMath.Clamp(terminalResetElapsedTime / 100f, 0.0f, 1.0f));
                    if( scrollPosition < 0 )
                    {
                      factor = factor * Math.Max(4 - item.Index, 0);
                    }
                    else
                    {
                      factor = factor * Math.Max(4 - (ItemCount - 1 - item.Index), 0);
                    }
                    rotationX += (float)(rand.NextDouble() - 0.5) / 10 * factor;
                    rotationY += (float)(rand.NextDouble() - 0.5) / 20 * factor;
                    rotationZ += (float)(rand.NextDouble() - 0.5) / 40 * factor;
                    pos.X += (float)(rand.NextDouble() - 0.5) * 4 * factor;
                    pos.Y += (float)(rand.NextDouble() - 0.5) * 4 * factor;
                    pos.Z += (float)(rand.NextDouble() - 0.5) * 4 * factor;
                }

                item.Transform3D = Matrix4.Translation(pos) * Matrix4.RotationZ(rotationZ) * Matrix4.RotationY(rotationY) * Matrix4.RotationX(rotationX);
            }
            
            private float GetItemPosYWithSpring(ListItemData data, float scrollPos, float elapsedTime)
            {
                // Calc diff
                ListPanelItem item = data.item;
                float diff = 0.0f;
                if (this.scrollDirection == FourWayDirection.Up)
                {
                    if (item.Index <= ScrollAreaFirstItemIndex)
                    {
                        diff = item.Y - (item.Index * LineHeight + LineHeight / 2f - scrollPos);
                    }
                    else
                    {
                        ListPanelItem prevItem = GetItem(item.Index - 1);
                        if (prevItem != null)
                        {
                            diff = item.Y - (prevItem.Y + LineHeight);
                        }
                    }
                }
                else if (this.scrollDirection == FourWayDirection.Down)
                {
                    if (item.Index >= ScrollAreaLastItemIndex)
                    {
                        diff = item.Y - (item.Index * LineHeight + LineHeight / 2f - scrollPos);
                    }
                    else
                    {
                        ListPanelItem nextItem = GetItem(item.Index + 1);
                        if (nextItem != null)
                        {
                            diff = item.Y - (nextItem.Y - LineHeight);
                        }
                    }
                }

                float approachRate = 0.0f;
                if (animationState == AnimationState.Drag)
                {
                    approachRate = scrollGravity + 0.01f * (FMath.Abs(scrollBaseIndex - item.Index) - 1);
                }
                else if (animationState == AnimationState.Flick)
                {
                    approachRate = scrollGravity + 0.05f * (FMath.Abs(scrollBaseIndex - item.Index));
                }
                else
                {
                    approachRate = scrollGravity + 0.05f * (FMath.Abs(scrollBaseIndex - item.Index));
                }

                // Skip frame
                float skipRate = elapsedTime / 16.6f;;

                // Calc approach distance
                approachRate = FMath.Clamp(approachRate, -1.0f, 1.0f);
                float approachDist = (diff * approachRate) * skipRate;
                if (diff < 0)
                {
                    approachDist = FMath.Clamp(approachDist, diff, 0.0f);
                }
                else
                {
                    approachDist = FMath.Clamp(approachDist, 0.0f, diff);
                }
                data.IsScrolling = FMath.Abs(diff) > 1f;

                return item.Y - approachDist;
            }

            private ListPanelItem GetItem(int itemIndex)
            {
                foreach (ListItemData data in this.usingItemDataList)
                {
                    if (data.item.Index == itemIndex)
                    {
                        return data.item;
                    }
                }
                return null;
            }

            void SetupItemDataForSlide(ListItemData itemData)
            {
                itemData.slideEndRotationZ = (float)rand.NextDouble() * itemTiltAngle;

                if ((itemData.item.Index & 1) == 0)
                {
                    // right
                    itemData.slideStartPos = new Vector3(Width * slideInOffsetX, 0, itemSlideInHeight);
                    itemData.slideEndPos = new Vector3((float)rand.NextDouble() * itemOffsetX, 0, 0);
                    itemData.scrollEndPosX = -(float)rand.NextDouble() * itemOffsetX;
                    if (scrollDirection == FourWayDirection.Down)
                    {
                        itemData.slideEndRotationZ = -itemData.slideEndRotationZ;
                    }

                    itemData.slideStartRotationY = itemSlideInRotationY;
                }
                else
                {
                    // left
                    itemData.slideStartPos = new Vector3(-Width * slideInOffsetX, 0, itemSlideInHeight);
                    itemData.slideEndPos = new Vector3(-(float)rand.NextDouble() * itemOffsetX, 0, 0);
                    itemData.scrollEndPosX = (float)rand.NextDouble() * itemOffsetX;
                    if (scrollDirection == FourWayDirection.Up)
                    {
                        itemData.slideEndRotationZ = -itemData.slideEndRotationZ;
                    }

                    itemData.slideStartRotationY = -itemSlideInRotationY;
                }

                itemData.scrollStartRotationZ = itemData.slideEndRotationZ;
                itemData.scrollEndRotationZ = itemData.slideEndRotationZ;
                itemData.slideStartPos.Y = itemData.slideEndRotationZ * itemData.slideStartPos.X;
                itemData.slideStartRotationZ = itemData.slideEndRotationZ * slideInRotationDelta;
                itemData.SlideElapsedTime = -itemSlideInDelay;
                itemData.IsSliding = true;

                itemContainerPanel.AddChildLast(itemData.item);
            }

            private void UpdateListItemData()
            {
                UpdateListItemData(0.0f);
            }

            private void UpdateListItemData(float elapsedTime)
            {
                // destroy unuse items
                List<ListItemData> destroyItems = new List<ListItemData>();

                foreach (ListItemData data in this.usingItemDataList)
                {
                    if (data.item.Index < ScrollAreaFirstItemIndex || data.item.Index > ScrollAreaLastItemIndex)
                    {
                        destroyItems.Add(data);
                    }
                }
                foreach (ListItemData data in destroyItems)
                {
                    DestroyListItemData(data);
                }

                // create new items
                int usingLastIndex;
                if (usingItemDataList.Count == 0)
                {
                    usingLastIndex = ScrollAreaFirstItemIndex - 1;
                }
                else
                {
                    usingLastIndex = usingItemDataList[usingItemDataList.Count - 1].item.Index;
                }
                for (int index = usingLastIndex + 1; index <= ScrollAreaLastItemIndex; index++)
                {
                    CreateListItemData(index);
                }
                int usingFirstIndex;
                if (usingItemDataList.Count == 0)
                {
                    usingFirstIndex = ScrollAreaLastItemIndex + 1;
                }
                else
                {
                    usingFirstIndex = usingItemDataList[0].item.Index;
                }
                for (int index = usingFirstIndex - 1; index >= ScrollAreaFirstItemIndex; index--)
                {
                    CreateListItemData(index);
                }

                // update position
                if (scrollDirection == FourWayDirection.Up)
                {
                    foreach (ListItemData data in this.usingItemDataList)
                    {
                        UpdateTransform3D(data, elapsedTime);
                    }
                }
                else
                {
                    for (int i = this.usingItemDataList.Count - 1; i >= 0 ; i--)
                    {
                        ListItemData data = this.usingItemDataList[i];
                        UpdateTransform3D(data, elapsedTime);
                    }
                }
                UpdateScrollBar();
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

                elapsedTime = GetElapsedTimeMovingAverage(elapsedTime);

                if (needRefresh)
                {
                    refresh();
                    needRefresh = false;
                }


                bool needUpdate = false;

                if (scrollPosition < 0 || scrollPosition > MaxScrollPosition)
                {
                    needUpdate = true;
                    terminalResetElapsedTime = 0f;
                }
                else
                {
                    terminalResetElapsedTime += elapsedTime;
                }

                // slide animation
                foreach (var data in this.usingItemDataList)
                {
                    if (data.IsSliding)
                    {
                        needUpdate = true;
                        data.SlideElapsedTime += elapsedTime;
                        if (data.SlideElapsedTime > this.itemSlideInTime)
                        {
                            data.IsSliding = false;
                        }
                    }
                }

                // scroll animation (drag)
                foreach (var data in this.usingItemDataList)
                {
                    if (data.IsScrolling)
                    {
                        needUpdate = true;
                        break;
                    }
                }

                // scroll animation (flick)
                if (this.animationState == AnimationState.Flick)
                {
                    this.flickElapsedTime += elapsedTime;
                    needUpdate = true;

                    this.flickDistance = flickStartDistance * (1 - flickElapsedTime / animationTime) * (1 - flickElapsedTime / animationTime);

                    bool isFlick = false;
                    foreach (var data in this.usingItemDataList)
                    {
                        if (data.IsScrolling)
                        {
                            isFlick = true;
                        }
                    }

                    if (!isFlick)
                    {
                        this.animationState = AnimationState.None;
                    }
                    else
                    {
                        if (flickElapsedTime > animationTime)
                        {
                            ScrollOffset(0);
                        }
                        else
                        {
                            ScrollOffset(this.flickDistance);
                        }
                    }
                }

                if (this.animationState != AnimationState.Drag)
                {
                    if (scrollPosition < 0)
                    {
                        scrollPosition += elapsedTime * terminalDecay;
                        if (scrollPosition > 0) scrollPosition = 0;
                    }
                    else if (scrollPosition > MaxScrollPosition)
                    {
                        scrollPosition -= elapsedTime * terminalDecay;
                        if (scrollPosition < MaxScrollPosition) scrollPosition = MaxScrollPosition;
                    }
                }

                if (needUpdate)
                {
                    UpdateListItemData(elapsedTime);
                }
                else
                {
                    isScrolling = false;
                    UpdateScrollBarVisible();
                }
            }
            
            private float GetElapsedTimeMovingAverage(float elapsedTime)
            {
                elapsedTimes[elapsedTimeIndex] = elapsedTime;

                elapsedTimeIndex++;
                if (elapsedTimeIndex >= ELAPSED_TIME_COUNT)
                {
                    elapsedTimeIndex = 0;
                    initElapsedTimes = true;
                }
    
                if (initElapsedTimes)
                {
                    elapsedTime = 0;
                    for (int i = 0; i < ELAPSED_TIME_COUNT; i++)
                    {
                        elapsedTime += elapsedTimes[i];
                    }
                    return elapsedTime / ELAPSED_TIME_COUNT;
                }
                else
                {
                    return elapsedTime;
                }
            }

            #region touch

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

                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;
                bool dragUp = false;
                switch (touchEvent.Type)
                {
                    case TouchEventType.Down:
                        if (animationState == AnimationState.Flick)
                        {
                            animationState = AnimationState.None;
                        }
                        float orgPosY = touchEvent.LocalPosition.Y;
                        foreach (ListItemData data in this.usingItemDataList)
                        {
                            ListPanelItem item = GetItem(data.item.Index + 1);
                            if (item != null && data.item.Y <= orgPosY && orgPosY < item.Y)
                            {
                                this.scrollBaseIndex = data.item.Index;
                            }
                            data.scrollStartRotationZ = data.slideEndRotationZ;
                        }
                        UpdateScrollEndRotationZ();
                        break;
                    case TouchEventType.Up:
                        if (animationState == AnimationState.Drag)
                        {
                            animationState = AnimationState.None;
                            isScrolling = false;
                            dragUp = true;
                        }
                        UpdateScrollBarVisible();
                        break;
                }

                if (animationState == AnimationState.None && !isScrolling && !dragUp)
                {
                    touchEvents.Forward = true;
                }
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

                if (animationState == AnimationState.Drag)
                {
                    animationState = AnimationState.None;
                    isScrolling = false;
                }
                UpdateScrollBarVisible();
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                if (e.Distance.Y == 0)
                {
                    return;
                }

                this.animationState = AnimationState.Drag;
                this.isScrolling = true;
                if (e.Distance.Y > 0)
                {
                    this.scrollDirection = FourWayDirection.Down;
                }
                else
                {
                    this.scrollDirection = FourWayDirection.Up;
                }

                if (this.prevScrollDirection != this.scrollDirection)
                {
                    UpdateScrollEndRotationZ();
                }
                this.prevScrollDirection = this.scrollDirection;

                ScrollOffset(e.Distance.Y);
                UpdateListItemData();
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                if (e.Speed.Y == 0)
                {
                    return;
                }

                this.animationState = AnimationState.Flick;
                if (e.Speed.Y > 0)
                {
                    this.scrollDirection = FourWayDirection.Down;
                }
                else
                {
                    this.scrollDirection = FourWayDirection.Up;
                }
                this.flickDistance = e.Speed.Y * 0.015f;
                this.flickStartDistance = this.flickDistance;
                this.flickElapsedTime = 0.0f;

                ScrollOffset(this.flickDistance);
                UpdateListItemData();
            }

            private void UpdateScrollEndRotationZ()
            {
                foreach (ListItemData data in this.usingItemDataList)
                {
                    float angle = (float)rand.NextDouble() * itemTiltAngle;
                    data.scrollEndRotationZ = data.scrollEndRotationZ > 0 ? angle : -angle;
                    data.scrollStartRotationZ = data.slideEndRotationZ;
                    data.totalScrollIncrement = 0.0f;
                }
            }

            private void ScrollOffset(float offsetY)
            {
                scrollPosition -= offsetY;
                if (0 < scrollPosition && scrollPosition < MaxScrollPosition)
                {
                    foreach (var data in this.usingItemDataList)
                    {
                        data.totalScrollIncrement += FMath.Abs(offsetY);
                    }
                }
            }

            #endregion

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
                itemCreator = creator;
                needRefresh = true;
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
                itemUpdater = updater;
                needRefresh = true;
            }

            #region Focus
            private const float focusMoveMargin = 30;

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
                // TODO:Focus marge ListPanel
                if (keyEvent.KeyEventType == KeyEventType.Down ||
                    keyEvent.KeyEventType == KeyEventType.Repeat)
                {
                    FourWayDirection direction = FourWayDirection.Down;

                    if (KeyEvent.TryConvertToDirection(keyEvent.KeyType, ref direction))
                    {
                        int index = getFocusedIndex();
                        UIDebug.Assert(index >= 0);
                        ListPanelItem nextFocusItem = null;
                        if (direction == FourWayDirection.Down)
                        {
                            this.scrollDirection = FourWayDirection.Up;
                            keyEvent.Handled = true;
                            if (ScrollAreaLastItemIndex - 1 > index)
                            {
                                nextFocusItem = GetItem(index + 1);
                                UIDebug.Assert(nextFocusItem != null);
                            }
                            else
                            {
                                if (index < ItemCount - 1)
                                {
                                    ScrollOffset(-LineHeight);
                                    UpdateListItemData();
                                    nextFocusItem = GetItem(index + 1);
                                    UIDebug.Assert(nextFocusItem != null);
                                }
                                else
                                {
                                    if (scrollPosition < MaxScrollPosition)
                                    {
                                        ScrollOffset(scrollPosition - MaxScrollPosition);
                                        UpdateListItemData();
                                    }
                                    else
                                    {
                                        keyEvent.Handled = false;
                                    }
                                }
                            }
                        }
                        else if (direction == FourWayDirection.Up)
                        {
                            this.scrollDirection = FourWayDirection.Down;
                            keyEvent.Handled = true;
                            if (index > ScrollAreaFirstItemIndex + 1)
                            {
                                nextFocusItem = GetItem(index - 1);
                                UIDebug.Assert(nextFocusItem != null);
                            }
                            else
                            {
                                if (index > 0)
                                {
                                    ScrollOffset(LineHeight);
                                    UpdateListItemData();
                                    nextFocusItem = GetItem(index - 1);
                                    UIDebug.Assert(nextFocusItem != null);
                                }
                                else
                                {
                                    if (scrollPosition > 0)
                                    {
                                        ScrollOffset(scrollPosition);
                                        UpdateListItemData();
                                    }
                                    else
                                    {
                                        keyEvent.Handled = false;
                                    }
                                }
                            }
                        }
                        if (nextFocusItem != null)
                        {
                            nextFocusItem.SetFocus(true);
                        }
                    }
                }

                base.OnPreviewKeyEvent(keyEvent);
            }

            internal override void OnFocusChangedChild(FocusChangedEventArgs args)
            {
                if (args.Focused && !args.OldFocusWidget.belongTo(this))
                {
                    int index = getFocusedIndex();
                    if (index >= 0)
                    {
                        ListPanelItem nextFocusItem = null;
                        if (index < ScrollAreaFirstItemIndex + 1)
                        {
                            if(index != 0 || scrollPosition > 0)
                                nextFocusItem = GetItem(index + 1);
                        }
                        else if (index > ScrollAreaLastItemIndex - 1)
                        {
                            if (index != ItemCount - 1 || scrollPosition < MaxScrollPosition)
                                nextFocusItem = GetItem(index - 1);
                        }

                        if (nextFocusItem != null)
                        {
                            nextFocusItem.SetFocus(true);
                        }
                    }
                }
            }

            private int getFocusedIndex()
            {
                int index = -1;
                var scene = this.ParentScene;
                if (scene != null && scene.FocusWidget != null)
                {
                    foreach (var item in usingItemDataList)
                    {
                        if (item.item == scene.FocusWidget || scene.FocusWidget.belongTo(item.item))
                        {
                            index = item.item.Index;
                            break;
                        }
                    }
                }
                return index;
            }

            #endregion

            private class ListItemData
            {
                public ListPanelItem item = null;

                public float rotationZ = 0.0f;

                public Vector3 slideStartPos;
                public Vector3 slideEndPos;
                public float slideStartRotationZ = 0.0f;
                public float slideEndRotationZ = 0.0f;
                public float slideStartRotationY = 0.0f;

                public float scrollEndPosX = 0.0f;
                public float scrollStartRotationZ = 0.0f;
                public float scrollEndRotationZ = 0.0f;
                public float totalScrollIncrement = 0.0f;

                public float SlideElapsedTime = 0.0f;
                public bool IsSliding = false;

                public bool IsScrolling = false;
            }
        }


    }
}
