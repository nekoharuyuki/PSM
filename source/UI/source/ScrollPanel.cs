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
        /// <summary>スクロールバーの見え方</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar view</summary>
        /// @endif
        public enum ScrollBarVisibility
        {
            /// @if LANG_JA
            /// <summary>常に表示</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Always display</summary>
            /// @endif
            Visible = 0,

            /// @if LANG_JA
            /// <summary>スクロールできるなら表示</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Display when scrolling is enabled</summary>
            /// @endif
            ScrollableVisible,

            /// @if LANG_JA
            /// <summary>スクロール中のみ表示</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Display when scrolling</summary>
            /// @endif
            ScrollingVisible,

            /// @if LANG_JA
            /// <summary>常に非表示</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Always hide</summary>
            /// @endif
            Invisible
        }


        /// @if LANG_JA
        /// <summary>スクロール可能な領域を持つコンテナウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A container widget which has the area where scrolling is enabled</summary>
        /// @endif
        public class ScrollPanel : ContainerWidget
        {
            const float toleranceSizeAsScrollable = 0.999f;

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
            public ScrollPanel()
            {
                this.panel = new Panel();
                this.panel.Clip = true;
                base.AddChildLast(panel);

                this.scrollBarH = new ScrollBar(ScrollBarOrientation.Horizontal);
                this.scrollBarV = new ScrollBar(ScrollBarOrientation.Vertical);
                base.AddChildLast(scrollBarH);
                base.AddChildLast(scrollBarV);

                this.animationState = AnimationState.None;

                this.Clip = true;
                this.HookChildTouchEvent = true;
                this.Focusable = true;
                this.FocusStyle = FocusStyle.Rectangle;
                this.avoidFocusFromChildren = true;

                dragGesture = new DragGestureDetector();
                dragGesture.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);

                flickGesture = new FlickGestureDetector();
                flickGesture.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);

                updateGestureDetectDirection();

                UpdateView();
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
                    UpdateView();
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
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのパネルのX座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the X coordinate of the panel in the parent coordinate system.</summary>
            /// @endif
            public float PanelX
            {
                get
                {
                    return this.panel.X;
                }
                set
                {
                    if (this.Width < this.panel.Width)
                    {
                        this.panel.X = FMath.Clamp(value, this.Width - this.panel.Width, 0.0f);
                        this.scrollBarH.BarPosition = -this.panel.X;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのパネルのY座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Y coordinate of the panel in the parent coordinate system.</summary>
            /// @endif
            public float PanelY
            {
                get
                {
                    return panel.Y;
                }
                set
                {
                    if (this.Height < this.panel.Height)
                    {
                        this.panel.Y = FMath.Clamp(value, this.Height - this.panel.Height, 0.0f);
                        this.scrollBarV.BarPosition = -this.panel.Y;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width of the panel.</summary>
            /// @endif
            public float PanelWidth
            {
                get
                {
                    return this.panel.Width;
                }
                set
                {
                    this.panel.Width = value;
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height of the panel.</summary>
            /// @endif
            public float PanelHeight
            {
                get
                {
                    return this.panel.Height;
                }
                set
                {
                    this.panel.Height = value;
                    UpdateView();
                }
            }

            /// @if LANG_JA
            /// <summary>パネルの色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the panel.</summary>
            /// @endif
            public UIColor PanelColor
            {
                get
                {
                    return this.panel.BackgroundColor;
                }
                set
                {
                    this.panel.BackgroundColor = value;
                }
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
                    updateGestureDetectDirection();
                }
            }
            private ScrollBarVisibility scrollBarVisibility = ScrollBarVisibility.ScrollableVisible;

            private bool horizontalScroll = true;

            /// @if LANG_JA
            /// <summary>水平方向のスクロールをするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to perform horizontal scrolling.</summary>
            /// @endif
            public bool HorizontalScroll
            {
                get { return horizontalScroll; }
                set
                {
                    horizontalScroll = value;
                    UpdateScrollBarVisible();
                    updateGestureDetectDirection();
                }
            }

            private bool verticalScroll = true;

            /// @if LANG_JA
            /// <summary>垂直方向のスクロールをするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to perform vertical scrolling.</summary>
            /// @endif
            public bool VerticalScroll
            {
                get { return verticalScroll; }
                set
                {
                    verticalScroll = value;
                    UpdateScrollBarVisible();
                    updateGestureDetectDirection();
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットを取得する。</summary>
            /// <remarks>コレクションを反復処理する列挙子を返す。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the child widget.</summary>
            /// <remarks>Returns the enumerator that repetitively handles the collection.</remarks>
            /// @endif
            public override IEnumerable<Widget> Children
            {
                get
                {
                    return this.panel.Children;
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットを先頭に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は先頭に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the beginning.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the beginning if it has already been added.</remarks>
            /// @endif
            public override void AddChildFirst(Widget child)
            {
                this.panel.AddChildFirst(child);
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットを末尾に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は末尾に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the end.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the end if it has already been added.</remarks>
            /// @endif
            public override void AddChildLast(Widget child)
            {
                this.panel.AddChildLast(child);
            }

            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直前に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="nextChild">挿入する子ウィジェットの直後となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直前に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately in front of the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="nextChild">Child widget immediately after the inserted child widget</param>
            /// <remarks>Moves it immediately in front of the specified child widget if it has already been added.</remarks>
            /// @endif
            public override void InsertChildBefore(Widget child, Widget nextChild)
            {
                this.panel.InsertChildBefore(child, nextChild);
            }

            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直後に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="prevChild">挿入する子ウィジェットの直前となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直後に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately after the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="prevChild">Child widget immediately in front of the inserted child widget</param>
            /// <remarks>Moves it immediately after the specified child widget if it has already been added.</remarks>
            /// @endif
            public override void InsertChildAfter(Widget child, Widget prevChild)
            {
                this.panel.InsertChildAfter(child, prevChild);
            }

            /// @if LANG_JA
            /// <summary>指定された子ウィジェットを削除する。</summary>
            /// <param name="child">削除する子ウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified child widget.</summary>
            /// <param name="child">Child widget to be deleted</param>
            /// @endif
            public override void RemoveChild(Widget child)
            {
                this.panel.RemoveChild(child);
            }

            /// @if LANG_JA
            /// <summary>指定した位置へスクロールする</summary>
            /// <remarks>スクロールできない方向は無視されます。</remarks>
            /// <param name='x'>右上の内部パネルのX座標。PanelX と正負が反転した値となる。</param>
            /// <param name='y'>右上の内部パネルのY座標。PanelY と正負が反転した値となる。</param>
            /// <param name='withAnimation'>アニメーションするかどうか</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scrolls to the specified position</summary>
            /// <remarks>Directions in which scrolling cannot be performed are ignored.</remarks>
            /// <param name="x">X coordinate of inside panel at top right. This is the value in which PanelX and positive and negative are inverted.</param>
            /// <param name="y">Y coordinate of inside panel at top right. This is the value in which PanelY and positive and negative are inverted.</param>
            /// <param name="withAnimation">Whether to execute the animation</param>
            /// @endif
            public void ScrollTo(float x, float y, bool withAnimation)
            {
                // TODO: withAnimation
                if (isHorizontalScrollable())
                    this.PanelX = -x;
                if (isVerticalScrollable())
                    this.PanelY = -y;
            }

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
                                if (w != null && w.belongTo(this) && w.getClippedState() == ClippedState.NoClipped)
                                {
                                    w.SetFocus(true);
                                }
                                else if (scene.FocusWidget.getClippedState() != ClippedState.NoClipped)
                                {
                                    this.SetFocus(true);
                                }
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
                    if (args.NewFocusWidget.getClippedState() != ClippedState.NoClipped)
                        this.SetFocus(false);
                }
            }

            bool scrollByFourWayKey(FourWayDirection direction)
            {
                const float epsilon = 0.5f;
                const float scrollDistance = 20f;
                switch (direction)
                {
                    case FourWayDirection.Left:
                        if (isHorizontalScrollable() && -PanelX > epsilon)
                        {
                            ScrollTo(-PanelX - scrollDistance, -PanelY, true);
                            return true;
                        }
                        return false;
                    case FourWayDirection.Right:
                        if (isHorizontalScrollable() && this.Width - PanelX < this.PanelWidth - epsilon)
                        {
                            ScrollTo(-PanelX + scrollDistance, -PanelY, true);
                            return true;
                        }
                        return false;
                    case FourWayDirection.Up:
                        if (isVerticalScrollable() && -PanelY > epsilon)
                        {
                            ScrollTo(-PanelX, -PanelY - scrollDistance, true);
                            return true;
                        }
                        return false;
                    case FourWayDirection.Down:
                        if (isVerticalScrollable() && this.Height - this.PanelY < this.PanelHeight - epsilon)
                        {
                            ScrollTo(-PanelX, -PanelY + scrollDistance, true);
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }

            }

            /// @if LANG_JA
            /// <summary>フォーカスが当たるかどうかを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to set the focus</summary>
            /// @endif
            public override bool Focusable
            {
                get
                {
                    return base.Focusable && isHorizontalScrollable() || isVerticalScrollable();
                }
                set
                {
                    base.Focusable = value;
                }
            }

            private void UpdateView()
            {
                if (this.scrollBarH == null || this.scrollBarV == null)
                {
                    return;
                }

                // horizontal
                if (this.Width > this.PanelWidth)
                {
                    this.PanelX = 0f;
                    this.scrollBarH.Length = this.Width;
                }
                else
                {
                    this.panel.X = FMath.Clamp(this.panel.X, this.Width - this.panel.Width, 0.0f);
                    this.scrollBarH.Length = this.panel.Width;
                }
                this.scrollBarH.Y = this.Height - this.scrollBarH.Height;
                this.scrollBarH.Width = this.Width - this.scrollBarV.Width;
                this.scrollBarH.BarLength = this.Width;

                // vertical
                if (this.Height > this.PanelHeight)
                {
                    this.PanelY = 0f;
                    this.scrollBarV.Length = this.Height;
                }
                else
                {
                    this.panel.Y = FMath.Clamp(this.panel.Y, this.Height - this.panel.Height, 0.0f);
                    this.scrollBarV.Length = this.panel.Height;
                }
                this.scrollBarV.X = this.Width - this.scrollBarV.Width;
                this.scrollBarV.Height = this.Height - this.scrollBarH.Height;
                this.scrollBarV.BarLength = this.Height;

                UpdateScrollBarPos();
                UpdateScrollBarVisible();
            }

            private void UpdateScrollBarPos()
            {
                this.scrollBarH.BarPosition = -this.panel.X;
                this.scrollBarV.BarPosition = -this.panel.Y;
            }

            private void UpdateScrollBarVisible()
            {
                switch (this.scrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        this.scrollBarH.Visible = (this.HorizontalScroll == true);
                        this.scrollBarV.Visible = (this.VerticalScroll == true);
                        break;
                    case ScrollBarVisibility.ScrollableVisible:
                        this.scrollBarH.Visible = isHorizontalScrollable();
                        this.scrollBarV.Visible = isVerticalScrollable();
                        break;
                    case ScrollBarVisibility.ScrollingVisible:
                    case ScrollBarVisibility.Invisible:
                        this.scrollBarH.Visible = false;
                        this.scrollBarV.Visible = false;
                        break;
                }
            }

            private void updateGestureDetectDirection()
            {
                if (dragGesture == null) return;

                this.RemoveGestureDetector(dragGesture);
                this.RemoveGestureDetector(flickGesture);

                if (HorizontalScroll | VerticalScroll)
                {
                    this.AddGestureDetector(dragGesture);
                    this.AddGestureDetector(flickGesture);

                    if (!VerticalScroll)
                    {
                        this.dragGesture.Direction = DragDirection.Horizontal;
                        this.flickGesture.Direction = FlickDirection.Horizontal;
                    }
                    else if (!HorizontalScroll)
                    {
                        this.dragGesture.Direction = DragDirection.Vertical;
                        this.flickGesture.Direction = FlickDirection.Vertical;
                    }
                    else
                    {
                        this.dragGesture.Direction = DragDirection.All;
                        this.flickGesture.Direction = FlickDirection.All;

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

                if (Math.Abs(this.flickDistance.X) < 0.5f && Math.Abs(this.flickDistance.Y) < 0.5f)
                {
                    this.flickDistance = Vector2.Zero;
                    this.animationState = AnimationState.None;
                    this.animation = false;
                    
                    UpdateView();
                }
                else
                {
                    animationElapsedTime += elapsedTime;
                    float exp = (float)Math.Exp(-animationElapsedTime * (0.09f / 16.67f));

                    this.flickDistance = startFlickDistance * exp;

                    if (Math.Abs(flickDistance.X) > 0.5f)
                    {
                        this.panel.X = startPanelPos.X + (startFlickDistance.X - this.flickDistance.X) / 0.09f;
                        this.panel.X = FMath.Clamp(this.panel.X, this.Width - this.panel.Width, 0.0f);
                    }

                    if (Math.Abs(flickDistance.Y) > 0.5f)
                    {
                        this.panel.Y = startPanelPos.Y + (startFlickDistance.Y - this.flickDistance.Y) / 0.09f;
                        this.panel.Y = FMath.Clamp(this.panel.Y, this.Height - this.panel.Height, 0.0f);
                    }

                    UpdateScrollBarPos();
                }
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

                if (touchEvents.PrimaryTouchEvent.Type == TouchEventType.Up)
                {
                    if (this.animation == false)
                    {
                        if (this.ScrollBarVisibility == ScrollBarVisibility.ScrollingVisible)
                        {
                            this.scrollBarH.Visible = false;
                            this.scrollBarV.Visible = false;
                        }

                        this.animationState = AnimationState.None;
                    }
                }

                if (this.animationState != AnimationState.Drag &&
                    this.animationState != AnimationState.Flick)
                {
                    touchEvents.Forward = true;
                }
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                this.ResetState(false);
                if (this.animation)
                {
                    flickDistance = Vector2.Zero;
                    this.animation = false;
                }
                this.animationState = AnimationState.Drag;

                if (this.ScrollBarVisibility == ScrollBarVisibility.ScrollingVisible)
                {
                    this.scrollBarH.Visible = isHorizontalScrollable();
                    this.scrollBarV.Visible = isVerticalScrollable();
                }

                if (isHorizontalScrollable())
                {
                    panel.X += e.Distance.X;
                    panel.X = FMath.Clamp(panel.X, this.Width - this.panel.Width, 0.0f);
                }

                if (isVerticalScrollable())
                {
                    panel.Y += e.Distance.Y;
                    panel.Y = FMath.Clamp(panel.Y, this.Height - this.panel.Height, 0.0f);
                }

                UpdateScrollBarPos();
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                this.ResetState(false);
                animationState = AnimationState.Flick;

                if (this.ScrollBarVisibility == ScrollBarVisibility.ScrollingVisible)
                {
                    this.scrollBarH.Visible = isHorizontalScrollable();
                    this.scrollBarV.Visible = isVerticalScrollable();
                }

                if (isHorizontalScrollable())
                {
                    flickDistance.X = e.Speed.X * 0.03f;
                }

                if (isVerticalScrollable())
                {
                    flickDistance.Y = e.Speed.Y * 0.03f;
                }
    
                startFlickDistance = flickDistance;
                startPanelPos.X = panel.X;
                startPanelPos.Y = panel.Y;
                animationElapsedTime = 0f;
                
                this.animation = true;
            }

            private bool isHorizontalScrollable()
            {
                return this.HorizontalScroll && this.Width < this.PanelWidth - toleranceSizeAsScrollable;
            }
            private bool isVerticalScrollable()
            {
                return this.VerticalScroll && this.Height < this.PanelHeight - toleranceSizeAsScrollable;
            }

            private Panel panel;
            private ScrollBar scrollBarH;
            private ScrollBar scrollBarV;
            private AnimationState animationState;
            private bool animation = false;
            private Vector2 flickDistance;
            private Vector2 startFlickDistance;
            private Vector2 startPanelPos;
            private float animationElapsedTime;
            DragGestureDetector dragGesture;
            FlickGestureDetector flickGesture;
        }


    }
}
