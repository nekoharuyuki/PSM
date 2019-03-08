/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using System.Diagnostics;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>ピボットの種別</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Type of pivot</summary>
    /// @endif
    public enum PivotType : int
    {
        // int value use to calculate PivotAlignmentX/Y

        /// @if LANG_JA
        /// <summary>左上</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Top left</summary>
        /// @endif
        TopLeft = 0x00,

        /// @if LANG_JA
        /// <summary>上</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Top</summary>
        /// @endif
        TopCenter = 0x01,

        /// @if LANG_JA
        /// <summary>右上</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Top right</summary>
        /// @endif
        TopRight = 0x02,

        /// @if LANG_JA
        /// <summary>左</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Left</summary>
        /// @endif
        MiddleLeft = 0x10,

        /// @if LANG_JA
        /// <summary>中央</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Center</summary>
        /// @endif
        MiddleCenter = 0x11,

        /// @if LANG_JA
        /// <summary>右</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Right</summary>
        /// @endif
        MiddleRight = 0x12,

        /// @if LANG_JA
        /// <summary>左下</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Bottom left</summary>
        /// @endif
        BottomLeft = 0x20,

        /// @if LANG_JA
        /// <summary>下</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Bottom</summary>
        /// @endif
        BottomCenter = 0x21,

        /// @if LANG_JA
        /// <summary>右下</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Bottom right</summary>
        /// @endif
        BottomRight = 0x22
    }


    /// @if LANG_JA
    /// <summary>アンカーの種別</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Type of anchor</summary>
    /// @endif
    [Flags]
    public enum Anchors
    {
        /// @if LANG_JA
        /// <summary>なし</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>None</summary>
        /// @endif
        None = 0x00,

        /// @if LANG_JA
        /// <summary>上固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed to top</summary>
        /// @endif
        Top = 0x01,

        /// @if LANG_JA
        /// <summary>下固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed to bottom</summary>
        /// @endif
        Bottom = 0x02,

        /// @if LANG_JA
        /// <summary>高さ固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed height</summary>
        /// @endif
        Height = 0x04,

        /// @if LANG_JA
        /// <summary>左固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed to left</summary>
        /// @endif
        Left = 0x10,

        /// @if LANG_JA
        /// <summary>右固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed to right</summary>
        /// @endif
        Right = 0x20,

        /// @if LANG_JA
        /// <summary>幅固定</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fixed width</summary>
        /// @endif
        Width = 0x40
    }

    /// @if LANG_JA
    /// <summary>ウィジェットの基底クラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Base class of widget</summary>
    /// @endif
    public class Widget : IDisposable
    {
        const int PivotHorizontalMask = 0x0f;
        const int PivotVerticalMask = 0xf0;

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public Widget()
        {
            linkedTree = new LinkedTree<Widget>(this);

            rootUIElement = new RootUIElement(this);

            PivotType = PivotType.TopLeft;
            Anchors = (Anchors)(Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width);
            TouchResponse = true;

            Visible = true;
            Focusable = false;

            Width = 0.0f;
            Height = 0.0f;

            TouchEventReceived = null;
            KeyEventReceived = null;
            MotionEventReceived = null;

            Clip = false;
            HookChildTouchEvent = false;

            this.finalClipX = 0.0f;
            this.finalClipY = 0.0f;
            this.finalClipWidth = 0.0f;
            this.finalClipHeight = 0.0f;

            PriorityHit = false;

            this.GestureDetectors = new List<GestureDetector>();
        }

        /// @if LANG_JA
        /// <summary>このウィジェット、および子ウィジェットの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>このウィジェットとそのツリー以下のすべてのウィジェット、およびそれぞれの RootUIElement のツリーに所属するすべての UIElement を再帰的に解放します。
        /// 一度 Dispose() が呼ばれたウィジェットは再利用することはできません。
        /// ウィジェット派生クラスでツリーに所属しないウィジェットやUIElement、その他の Dispose が必要なオブジェクトを作成した場合は、 DisposeSelf() をオーバーライドしてそれらの Dispose を呼ばなければなりません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this widget and child widgets.</summary>
        /// <remarks>Recursively frees this widget and all widgets in and below its tree as well as all UIElements that belong to the RootUIElement tree of each.
        /// Once Dispose() has been called for a widget, it cannot be reused.
        /// If widgets/UIElements that do not belong to a tree and other objects that require Dispose have been created with widget derivative classes, DisposeSelf() must be overridden and Dispose must be called for them.</remarks>
        /// @endif
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;

            Dispose(true);
        }

        /// @if LANG_JA
        /// <summary>このウィジェット、および子ウィジェットの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>親ウィジェットから自身を削除し、子ウィジェットに対して DisposeSelf() を再帰的に呼び出します。
        /// 派生クラスでDisposeを実装する場合は DisposeSelf() をオーバーライドしてください。</remarks>
        /// <param name="disposing">Dispose() から呼び出された場合 true</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this widget and child widgets.</summary>
        /// <remarks>Deletes itself from a parent widget and recursively calls DisposeSelf() for child widgets.
        /// When implementing Dispose with a derivative class, override DisposeSelf().</remarks>
        /// <param name="disposing">true when called from Dispose()</param>
        /// @endif
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.linkedTree != null)
                {
                    this.linkedTree.RemoveChild();

                    for (var node = linkedTree; node != null; node = node.NextAsList)
                    {
                        if (node.Value != null)
                        {
                            node.Value.DisposeSelf();
                        }
                    }
                }
            }
        }

        /// @if LANG_JA
        /// <summary>このウィジェットの所有するアンマネージドリソースを解放する</summary>
        /// <remarks>このメソッドは Dispose() からその子ウィジェットに対して再帰的に呼び出されます。
        /// ウィジェット派生クラスでツリーに所属しないウィジェットやUIElement、その他の Dispose が必要なオブジェクトを作成した場合は、 このメソッドをオーバーライドしてそれらの Dispose を呼ばなければなりません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees the unmanaged resources held by this widget</summary>
        /// <remarks>This method is recursively called from Dispose() for the child widgets.
        /// If widgets/UIElements that do not belong to a tree and other objects that require Dispose have been created with widget derivative classes, this method must be overridden and Dispose must be called for them.</remarks>
        /// @endif
        protected virtual void DisposeSelf()
        {
            if (this.rootUIElement != null)
            {
                this.rootUIElement.Dispose();
            }
        }


        /// @if LANG_JA
        /// <summary>すでにDisposeされたかどうかを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether Dispose is already performed</summary>
        /// @endif
        internal bool Disposed
        {
            get;
            private set;
        }


        /// @if LANG_JA
        /// <summary>ウィジェットの名前を取得・設定する。</summary>
        /// <remarks>このプロパティはアプリケーションで任意に使用可能。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the name of the widget.</summary>
        /// <remarks>This property can be used arbitrarily with applications.</remarks>
        /// @endif
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private String name = "";

        /// @if LANG_JA
        /// <summary>エレメントツリーのルートを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the root of the element tree.</summary>
        /// @endif
        protected internal RootUIElement RootUIElement
        {
            get
            {
                return rootUIElement;
            }
        }

        private RootUIElement rootUIElement;

        /// @if LANG_JA
        /// <summary>親の座標系でのX座標を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the X coordinate in the parent coordinate system.</summary>
        /// @endif
        public virtual float X
        {
            get
            {
                return RootUIElement.X;
            }
            set
            {
                RootUIElement.X = value;
                this.NeedUpdateLocalToWorld = true;
            }
        }

        /// @if LANG_JA
        /// <summary>親の座標系でのY座標を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the Y coordinate in the parent coordinate system.</summary>
        /// @endif
        public virtual float Y
        {
            get
            {
                return RootUIElement.Y;
            }
            set
            {
                RootUIElement.Y = value;
                this.NeedUpdateLocalToWorld = true;
            }
        }

        /// @if LANG_JA
        /// <summary>親の座標系での位置を設定する。</summary>
        /// <param name="x">親の座標系でのX座標</param>
        /// <param name="y">親の座標系でのY座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the position in the parent coordinate system.</summary>
        /// <param name="x">X coordinate in the parent coordinate system</param>
        /// <param name="y">Y coordinate in the parent coordinate system</param>
        /// @endif
        public virtual void SetPosition(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        private float width;

        /// @if LANG_JA
        /// <summary>幅を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the width.</summary>
        /// @endif
        public virtual float Width
        {
            get { return width; }
            set
            {
                if (((int)pivotType & PivotHorizontalMask) != ((int)PivotType.TopLeft & PivotHorizontalMask))
                {
                    this.NeedUpdateLocalToWorld = true;
                }
                width = value;
            }
        }

        private float height;

        /// @if LANG_JA
        /// <summary>高さを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the height.</summary>
        /// @endif
        public virtual float Height
        {
            get { return height; }
            set
            {
                if (((int)pivotType & PivotVerticalMask) != ((int)PivotType.TopLeft & PivotVerticalMask))
                {
                    this.NeedUpdateLocalToWorld = true;
                }
                height = value;
            }
        }

        /// @if LANG_JA
        /// <summary>サイズを設定する。</summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the size.</summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// @endif
        public virtual void SetSize(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// @if LANG_JA
        /// <summary>親の座標系への変換行列を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transformation matrix to the parent coordinate system.</summary>
        /// @endif
        public virtual Matrix4 Transform3D
        {
            get
            {
                return RootUIElement.transform3D;
            }
            set
            {
                RootUIElement.transform3D = value;
                this.NeedUpdateLocalToWorld = true;
            }
        }

        /// @if LANG_JA
        /// <summary>透明度を取得・設定する。（０～１）</summary>
        /// <remarks>すべての子Widgetに透明度が乗算されて表示される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transparency. (0 - 1)</summary>
        /// <remarks>Multiplies the transparency by all child widgets and displays it.</remarks>
        /// @endif
        public virtual float Alpha
        {
            get
            {
                return RootUIElement.Alpha;
            }
            set
            {
                RootUIElement.Alpha = value;
            }
        }

        /// @if LANG_JA
        /// <summary>ウィジェットの原点を取得・設定する。</summary>
        /// <remarks>移動、回転、スケーリングするときの中心となる点。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the origin of the widget.</summary>
        /// <remarks>The point that serves as the center when moving, rotating, and scaling.</remarks>
        /// @endif
        public virtual PivotType PivotType
        {
            get
            {
                return pivotType;
            }
            set
            {
                X -= PivotAlignmentX;
                Y -= PivotAlignmentY;

                pivotType = value;

                X += PivotAlignmentX;
                Y += PivotAlignmentY;

                this.NeedUpdateLocalToWorld = true;
            }
        }

        private PivotType pivotType;

        /// @if LANG_JA
        /// <summary>Zソートするかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to Z sort.</summary>
        /// @endif
        public virtual bool ZSort
        {
            get { return this.RootUIElement.ZSort; }
            set { this.RootUIElement.ZSort = value; }
        }

        /// @if LANG_JA
        /// <summary>Zソートオフセットを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the Z-sort offset.</summary>
        /// @endif
        public virtual float ZSortOffset
        {
            get { return this.RootUIElement.ZSortOffset; }
            set { this.RootUIElement.ZSortOffset = value; }
        }


        /// @if LANG_JA
        /// <summary>親のリサイズ時にどこを固定するかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the area to fix when resizing the parent.</summary>
        /// @endif
        public virtual Anchors Anchors
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>表示するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to display.</summary>
        /// @endif
        public virtual bool Visible
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>子ウィジェットも含めてタッチイベントに応答するかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to respond to the touch event including the child widget.</summary>
        /// @endif
        public virtual bool TouchResponse
        {
            get
            {
                return touchMode;
            }
            set
            {
                this.touchMode = value;

                if (!this.touchMode)
                {
                    ResetState(true);
                }
            }
        }

        private bool touchMode;

        /// @if LANG_JA
        /// <summary>子要素(Widget、UIElement)をクリップするかどうかを取得・設定する。</summary>
        /// <remarks>クリップ領域は2次元の座標として計算される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to clip the child element (Widget, UIElement).</summary>
        /// <remarks>Calculates the clip area as a 2D coordinate.</remarks>
        /// @endif
        protected internal bool Clip
        {
            get { return clip; }
            set { clip = value; }
        }

        private bool clip;


        /// @if LANG_JA
        /// <summary>子ウィジェットのタッチイベントをフックするかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to hook the touch event of the child widget.</summary>
        /// @endif
        protected internal bool HookChildTouchEvent
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>ウィジェットツリーを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the widget tree.</summary>
        /// @endif
        internal LinkedTree<Widget> LinkedTree
        {
            get
            {
                return linkedTree;
            }
        }

        private LinkedTree<Widget> linkedTree;

        /// @if LANG_JA
        /// <summary>親ウィジェットを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the parent widget.</summary>
        /// @endif
        public Widget Parent
        {
            get
            {
                LinkedTree<Widget> target = linkedTree.Parent;
                if (target != null)
                {
                    return target.Value;
                }
                else
                {
                    return null;
                }
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
        protected IEnumerable<Widget> Children
        {
            get
            {
                for (LinkedTree<Widget> target = this.linkedTree.FirstChild;
                    target != null;
                    target = target.NextSibling)
                {
                    if (target.Value != null)
                    {
                        yield return target.Value;
                    }
                    else
                    {
                        yield break;
                    }

                }
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
        protected void AddChildFirst(Widget child)
        {
            linkedTree.AddChildFirst(child.linkedTree);
            child.NeedUpdateLocalToWorld = true;
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
        protected void AddChildLast(Widget child)
        {
            linkedTree.AddChildLast(child.linkedTree);
            child.NeedUpdateLocalToWorld = true;
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
        protected void InsertChildBefore(Widget child, Widget nextChild)
        {
            child.linkedTree.InsertChildBefore(nextChild.linkedTree);
            child.NeedUpdateLocalToWorld = true;
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
        protected void InsertChildAfter(Widget child, Widget prevChild)
        {
            child.linkedTree.InsertChildAfter(prevChild.linkedTree);
            child.NeedUpdateLocalToWorld = true;
        }

        /// @if LANG_JA
        /// <summary>指定された子ウィジェットを削除する。</summary>
        /// <param name="child">削除する子ウィジェット</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Deletes the specified child widget.</summary>
        /// <param name="child">Child widget to be deleted</param>
        /// @endif
        protected void RemoveChild(Widget child)
        {
            child.linkedTree.RemoveChild();
            child.NeedUpdateLocalToWorld = true;
        }

        /// @if LANG_JA
        /// <summary>タッチイベントを受け取ったときに呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when a touch event is received</summary>
        /// @endif
        public event EventHandler<TouchEventArgs> TouchEventReceived;

        /// @if LANG_JA
        /// <summary>キーイベントを受け取ったときに呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when a key event is received</summary>
        /// @endif
        public event EventHandler<KeyEventArgs> KeyEventReceived;

        /// @if LANG_JA
        /// <summary>事前キーイベントを受け取ったときに呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when an advance key event is received</summary>
        /// @endif
        public event EventHandler<KeyEventArgs> PreviewKeyEventReceived;

        /// @if LANG_JA
        /// <summary>モーションイベントを受け取ったときに呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when a motion event is received</summary>
        /// @endif
        public event EventHandler<MotionEventArgs> MotionEventReceived;

        /// @if LANG_JA
        /// <summary>タッチイベントハンドラ</summary>
        /// <param name="touchEvents">タッチイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch event handler</summary>
        /// <param name="touchEvents">Touch event</param>
        /// @endif
        protected internal virtual void OnTouchEvent(TouchEventCollection touchEvents)
        {
            if (pressable)
            {
                pressableOnTouchEvent(touchEvents);
            }

            SendTouchEventToGestureDetectors(touchEvents);

            if (this.TouchEventReceived != null)
            {
                this.TouchEventReceived(this, new TouchEventArgs(touchEvents));
            }
        }

        private void SendTouchEventToGestureDetectors(TouchEventCollection touchEvents)
        {
            foreach (GestureDetector detector in this.GestureDetectors)
            {
                // Send the touch events when one of the touchEvents' TouchEventType is down or a gesture is detected.
                bool containDownEvent = false;
                foreach (TouchEvent touchEvent in touchEvents)
                {
                    containDownEvent |= (touchEvent.Type == TouchEventType.Down);
                }

                if (containDownEvent || detector.State != GestureDetectorResponse.None)
                {
                    detector.State = detector.OnTouchEvent(touchEvents);
                    if (detector.State == GestureDetectorResponse.None)
                    {
                        detector.State = GestureDetectorResponse.FailedAndStop;
                    }
                }
            }
        }

        /// @internal
        /// @if LANG_JA
        /// <summary></summary>
        /// @endif
        /// @if LANG_EN
        /// <summary></summary>
        /// @endif
        [Obsolete("use focus system")]
        public Widget LocalKeyReceiverWidget
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>キーイベントハンドラ</summary>
        /// <param name="keyEvent">キーイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Key event handler</summary>
        /// <param name="keyEvent">Key event</param>
        /// @endif
        protected internal virtual void OnKeyEvent(KeyEvent keyEvent)
        {
            if (pressable)
            {
                pressableOnKeyEvent(keyEvent);
            }

            if (KeyEventReceived != null)
            {
                KeyEventArgs args = new KeyEventArgs(keyEvent);
                KeyEventReceived(this, args);
                keyEvent.Handled = args.Handled;
                keyEvent.EnabledEachFrameRepeat = args.EnabledEachFrameRepeat;
            }
        }

        /// @if LANG_JA
        /// <summary>事前キーイベントのハンドラ</summary>
        /// <param name="keyEvent">キーイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Advance key event handler</summary>
        /// <param name="keyEvent">Key event</param>
        /// @endif
        protected internal virtual void OnPreviewKeyEvent(KeyEvent keyEvent)
        {
            if (PreviewKeyEventReceived != null)
            {
                KeyEventArgs args = new KeyEventArgs(keyEvent);
                PreviewKeyEventReceived(this, args);
                keyEvent.Handled = args.Handled;
                keyEvent.EnabledEachFrameRepeat = args.EnabledEachFrameRepeat;
            }
        }

        /// @if LANG_JA
        /// <summary>ウィジェットツリーの順番で、次にフォーカスが当たるものを探す</summary>
        /// <remarks>ツリーの終端までいったら先頭から探す</remarks>
        /// <param name="reverse">true: 逆順で探す</param>
        /// <returns>次のウィジェット</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Searches for the next item that has focus in the order of the widget tree</summary>
        /// <remarks>Searches from the start when the end of the tree is reached</remarks>
        /// <param name="reverse">true: Search in reverse order</param>
        /// <returns>Next widget</returns>
        /// @endif
        internal Widget SearchNextTreeNodeFocus(bool reverse)
        {
            // to be safe infinite loop
            bool looped = false;

            Widget last = null;

            for (var node = this.linkedTree.NextAsList; node != this.linkedTree; node = node.NextAsList)
            {
                if (node == null)
                {
                    if (looped)
                    {
                        UIDebug.Assert(false, "Infinite loop at SearchNextTreeNodeFocus.");
                        return null;
                    }
                    looped = true;

                    // move to first node
                    node = this.linkedTree;
                    while (node.Parent != null)
                        node = node.Parent;
                }
                if (node == this.linkedTree)
                {
                    return null;
                }

                Widget w = node.Value;
                //UIDebug.LogFocus("{0}: V:{1}, E:{2}, Clip:{3}, F:{4}",w.ToString(), w.Visible,w.Enabled,w.getClippedState(),w.Focusable);
                if (!w.Visible || !w.Enabled || w.getClippedState() == ClippedState.ClippedAll)
                {
                    // skip this node's descendants
                    node = node.LastDescendant;
                    continue;
                }
                if (!w.Focusable)
                {
                    continue;
                }

                if (reverse)
                {
                    last = w;
                    continue;
                }

                return w;
            }

            // TODO: reverse is temporary imple
            return last;
        }

        /// @if LANG_JA
        /// <summary>自分自身にフォーカスをあてる</summary>
        /// <param name="active">true: フォーカスをアクティブにする false: 現状維持</param>
        /// <remarks>Focusable、Visible、Enabledプロパティおよび表示範囲内にあるかにかかわらずフォーカスを設定します。
        /// active を true に設定した場合でも、所属するSceneがフォーカス対象のSceneでない場合はフォーカスがアクティブになりません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Places focus on self</summary>
        /// <param name="active">true: Enables focus false: Keeps the status</param>
        /// <remarks>Sets the focus regardless of the Focusable, Visible, and Enabled properties and whether it is within the display range.
        /// If an affiliated scene is not the target focus scene, focus does not become active even when active is set to true.</remarks>
        /// @endif
        public void SetFocus(bool active)
        {
            UIDebug.LogFocus("SetFocus({0}): {1}", active, this);

            var scene = this.ParentScene;
            if (scene == null)
            {
                UIDebug.LogFocus("Failed SetFocus scene == null");
                return;
            }

            UIDebug.LogFocusIf(scene.FocusWidget != this, "Change Focus [{0}] -> [{1}]", scene.FocusWidget, this);

            var oldFocusWidget = scene.FocusWidget;
            scene.FocusWidget = this;
            if(scene.FocusActive)
            {
                if (active)
                {
                    UISystem.FocusActive = true;
                }

                var rect = this.getViewFocusScreenRectangle();
                //UISystem.setFocusSize(rect.X, rect.Y, rect.Width, rect.Height, oldFocusWidget != this);

                UISystem.setFocusSizeAndImage(rect, this.FocusStyle, this.FocusCustomSettings, oldFocusWidget != this);

                if (oldFocusWidget != this)
                {
                    oldFocusWidget.OnFocusChanged(new FocusChangedEventArgs(false, this, oldFocusWidget));
                    this.OnFocusChanged(new FocusChangedEventArgs(true, this, oldFocusWidget));
                }
                UISystem.SuppressFocusKeyEvent = false;
                if (this == scene.RootWidget)
                {
                    UISystem.setFocusVisible(false, false);
                }
            }
        }

        /// @if LANG_JA
        /// <summary>現在自分にフォーカスが当たっているかどうかを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether the focus is currently placed on self</summary>
        /// @endif
        public bool Focused
        {
            get
            {
                var scene = UISystem.FocusActiveScene;
                if (scene == null)
                {
                    return false;
                }

                return scene.FocusWidget == this;
            }
        }

        /// @if LANG_JA
        /// <summary>所属しているシーン</summary>
        /// <remarks>シーンに所属していない場合は null</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Affiliated scene</summary>
        /// <remarks>If it does not belong to the scene, then null</remarks>
        /// @endif
        internal Scene ParentScene
        {
            get
            {
                // TODO: Cache ParentScene
                var node = this.linkedTree;
                while (node.Parent != null)
                {
                    node = node.Parent;
                }
                var root = node.Value as RootWidget;
                if (root != null)
                    return root.parentScene;
                return null;
            }
        }

        /// @if LANG_JA
        /// <summary>指定したウィジェットの子孫かどうかを取得する</summary>
        /// <remarks>自分自身を指定した場合はfalse</remarks>
        /// <param name="w">親となるウィジェット</param>
        /// <returns>true: 所属する</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether it is a descendant of the specified widget</summary>
        /// <remarks>If self is specified, then false</remarks>
        /// <param name="w">Widget to be the parent</param>
        /// <returns>true: Affiliated</returns>
        /// @endif
        internal bool belongTo(Widget w)
        {
            if (w == null)
                return false;

            var node = this.linkedTree;
            while (node.Parent != null)
            {
                node = node.Parent;
                if (node.Value == w)
                    return true;
            }
            return false;
        }

        /// @if LANG_JA
        /// <summary>次にフォーカスの当たるウィジェットを探す</summary>
        /// <param name="direction">方向キーの方向</param>
        /// <returns>見つからない場合は null</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Searches for the widget in focus next</summary>
        /// <param name="direction">Direction of directional buttons</param>
        /// <returns>If not found, null.</returns>
        /// @endif
        internal protected Widget SearchNextFocus(FourWayDirection direction)
        {
            var scene = this.ParentScene;
            if (scene != null)
                return scene.RootWidget.SearchNextFocusFromChild(direction, this);
            else
                return null;
        }

        /// @if LANG_JA
        /// <summary>自分の子ウィジェット（子孫を含む）の中から次にフォーカスの当たるウィジェットを探す</summary>
        /// <remarks>自分自身は検索対象から除きます。</remarks>
        /// <param name="direction">方向キーの方向</param>
        /// <param name="currentFocusWidget">現在フォーカスのあるウィジェット</param>
        /// <returns>見つからない場合は null</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Searches for the widget in focus next from among own child widgets (including descendants)</summary>
        /// <remarks>Self is excluded from the search target.</remarks>
        /// <param name="direction">Direction of directional buttons</param>
        /// <param name="currentFocusWidget">Widget currently in focus</param>
        /// <returns>If not found, null.</returns>
        /// @endif
        internal protected Widget SearchNextFocusFromChild(FourWayDirection direction, Widget currentFocusWidget)
        {
            if (currentFocusWidget.FocusCustomSettings != null)
            {
                var w = currentFocusWidget.FocusCustomSettings.getFourwayWidget(direction);
                if (w != null && w.ParentScene == currentFocusWidget.ParentScene)
                    return w;
            }

            FocusRegion currentFocusRegion = currentFocusWidget.getFocusRegionNormalizedOnRightKey(direction, false);

            Widget resultWidget = null;
            var resultScore = float.MaxValue / 2f;

            var targetNode = currentFocusWidget.linkedTree;
            var endNode = this.linkedTree.LastDescendant.NextAsList;
            for (var node = this.linkedTree.NextAsList; node != endNode; node = node.NextAsList)
            {
                var widget = node.Value;
                if (!widget.Visible || !widget.Enabled ||
                    widget.Width <= 0 || widget.Height <= 0 ||
                    widget.getClippedState() == ClippedState.ClippedAll)
                {
                    node = node.LastDescendant;
                    continue;
                }

                if (!widget.Focusable || node == targetNode) continue;

                var score = widget.calcFocusScore(direction, currentFocusRegion);

                if (score < resultScore)
                {
                    resultScore = score;
                    resultWidget = widget;
                }
            }

            return resultWidget;
        }

        internal bool avoidFocusFromChildren = false;

        float calcFocusScore(FourWayDirection direction, FocusRegion focusR)
        {
            if (avoidFocusFromChildren)
            {
                if (focusR.Widget != null && focusR.Widget.belongTo(this))
                    return float.MaxValue;
            }

            if (this.finalClipWidth <= 0 || this.finalClipHeight <= 0)
                return float.MaxValue;

            var selfR = this.getFocusRegionNormalizedOnRightKey(direction, false);

            if (Math.Abs(selfR.Left - selfR.Right) < 0.5f)
                return float.MaxValue;

            float slopeRate = 1.2f;

            Vector2 vec = new Vector2();
            float score = 0;

            const float crossoverThreshold = 1;

            if (selfR.Left > focusR.Right - crossoverThreshold)
            {
                score += 10000;
                vec.X = selfR.Left - focusR.Right;
                if (selfR.Bottom < focusR.Top)
                {
                    vec.Y = focusR.Top - selfR.Bottom;
                }
                else if (selfR.Top > focusR.Bottom)
                {
                    vec.Y = selfR.Top - focusR.Bottom;
                }

                float sx = (vec.X + crossoverThreshold) * slopeRate;
                if (vec.Y < -sx || vec.Y > sx)
                {
                    return float.MaxValue;
                }

                if (vec.X < 0) vec.X = 0;
                score += Math.Abs(selfR.Center.Y - focusR.Center.Y) * 0.1f;
            }
            else if (selfR.Bottom < focusR.Top || selfR.Top > focusR.Bottom ||
                selfR.Center.X < focusR.Center.X-crossoverThreshold)
            {
                return float.MaxValue;
            }
            else if (selfR.Left < focusR.Left && selfR.Top < focusR.Top &&
                selfR.Right > focusR.Right && selfR.Bottom > focusR.Bottom)
            {
                score += 10000;

                float sx = ((selfR.Center.X - focusR.Center.X) + crossoverThreshold) * slopeRate;
                float sy = selfR.Center.Y - focusR.Center.Y;
                if (sy <= -sx || sy >= sx)
                {
                    return float.MaxValue;
                }
                vec.X = selfR.Right - focusR.Right;
            }
            else
            {
                vec = selfR.Center - focusR.Center;
                float sx = vec.X * slopeRate;
                if (vec.Y < -sx || vec.Y > sx)
                {
                    return float.MaxValue;
                }
            }

            score += vec.LengthSquared();

            return score;
        }

        internal class FocusRegion
        {
            public Widget Widget;
            // screen coord
            public Vector2 Center;
            public float Left;
            public float Top;
            // x-coordinate (length of the screen left side to this right side)
            public float Right;
            // y-coordinate (length of the screen top side to this bottom side)
            public float Bottom;
        }

        FocusRegion getFocusRegionNormalizedOnRightKey(FourWayDirection direction, bool clip)
        {
            var region = new FocusRegion();
            region.Widget = this;

            Rectangle localRect;
            Rectangle rect;
            if (this.FocusCustomSettings != null && this.FocusCustomSettings.hasSearchHintRectangle)
            {
                localRect = this.FocusCustomSettings.SearchHintRectangle;
            }
            else
            {
                localRect.X = 0;
                localRect.Y = 0;
                localRect.Width = this.Width;
                localRect.Height = this.Height;
            }

            convertLocalToScreen(ref localRect, out rect);

            if (clip)
            {
                if (rect.X < this.finalClipX)
                    rect.X = this.finalClipX;
                if (rect.Y < this.finalClipY)
                    rect.Y = this.finalClipY;
                if (rect.X + rect.Width > this.finalClipY + this.finalClipWidth)
                    rect.Width = this.finalClipY + this.finalClipWidth - rect.X;
                if (rect.Y + rect.Height > this.finalClipY + this.finalClipHeight)
                    rect.Height = this.finalClipY + this.finalClipHeight - rect.Y;
            }

            if (rect.Width < 0.5f || rect.Height < 0.5f)
            {
                region.Center.X = region.Left = region.Right = rect.X;
                region.Center.Y = region.Top = region.Bottom = rect.Y;
                return region;
            }

            float left = rect.X;
            float top = rect.Y;
            float right = rect.X + rect.Width;
            float bottom = rect.Y + rect.Height;

            float w = UISystem.FramebufferWidth;

            switch (direction)
            {
                case FourWayDirection.Up:
                    region.Left = w - bottom;
                    region.Right = w - top;
                    region.Top = left;
                    region.Bottom = right;
                    break;
                case FourWayDirection.Down:
                    region.Left = top;
                    region.Right = bottom;
                    region.Top = left;
                    region.Bottom = right;
                    break;
                case FourWayDirection.Left:
                    region.Left = w - right;
                    region.Right = w - left;
                    region.Top = top;
                    region.Bottom = bottom;
                    break;
                case FourWayDirection.Right:
                    region.Left = left;
                    region.Right = right;
                    region.Top = top;
                    region.Bottom = bottom;
                    break;
            }
            region.Center.X = (region.Left + region.Right) / 2;
            region.Center.Y = (region.Top + region.Bottom) / 2;

            UIDebug.Assert(region.Left < region.Right && region.Top < region.Bottom);

            return region;
        }

        internal Rectangle getViewFocusScreenRectangle()
        {
            Rectangle localRect;
            if (this.FocusCustomSettings != null && this.FocusCustomSettings.hasFocusImageRectangle)
            {
                localRect = this.FocusCustomSettings.FocusImageRectangle;
            }
            else
            {
                localRect.X = 0;
                localRect.Y = 0;
                localRect.Width = this.Width;
                localRect.Height = this.Height;
            }

            Rectangle screenRect;
            convertLocalToScreen(ref localRect, out screenRect);

            return screenRect;
        }

        /// @if LANG_JA
        /// <summary>モーションイベントハンドラ</summary>
        /// <param name="motionEvent">モーションイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Motion event handler</summary>
        /// <param name="motionEvent">Motion event</param>
        /// @endif
        protected internal virtual void OnMotionEvent(MotionEvent motionEvent)
        {
            if (this.MotionEventReceived != null)
            {
                this.MotionEventReceived(this, new MotionEventArgs(motionEvent));
            }
        }

        /// @if LANG_JA
        /// <summary>状態リセットハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Status reset handler</summary>
        /// @endif
        protected internal virtual void OnResetState()
        {
            foreach (GestureDetector detector in this.GestureDetectors)
            {
                detector.OnResetState();
                detector.State = GestureDetectorResponse.None;
            }

            this.IsReset = true;
            if (this.Enabled)
            {
                if (this.pressState != PressState.Normal)
                {
                    var oldState = pressState;
                    this.pressState = PressState.Normal;

                    OnPressStateChanged(new PressStateChangedEventArgs(PressStateChangedReason.Cancel, pressState, oldState));
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
        protected virtual void OnUpdate(float elapsedTime)
        {
        }

        internal void updateForEachDescendant(float elapsedTime)
        {
            var endNode = this.linkedTree.LastDescendant.NextAsList;
            for (var node = this.linkedTree; node != endNode; node = node.NextAsList)
            {
                Widget widget = node.Value;
                if (widget.Visible == true)
                {
                    widget.OnUpdate(elapsedTime);
                }
                else
                {
                    node = node.LastDescendant;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>指定した座標でこのウィジェットがヒットするかどうかを取得する</summary>
        /// <param name="screenPoint">スクリーン座標系での位置</param>
        /// <returns>ヒットしている場合はtrue。ヒットしていない場合はfalse。</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether this widget hits with the specified coordinates</summary>
        /// <param name="screenPoint">Position in the screen coordinate system</param>
        /// <returns>If hit, then true. If not hit, then false.</returns>
        /// @endif
        public virtual bool HitTest(Vector2 screenPoint)
        {
            if (this.Width <= 0.0f || this.Height <= 0.0f)
            {
                return false;
            }

            // TODO: use 3D coordinate system
            //Vector2 localPos = this.ConvertScreenToLocal(screenPoint);

            updateLocalToWorld();
            float left = localToWorld.M41;
            float top = localToWorld.M42;
            float right = left + this.Width;
            float bottom = top + this.Height;

            left = (finalClipX > left) ? finalClipX : left;
            top = (finalClipY > top) ? finalClipY : top;
            right = ((finalClipX + finalClipWidth) < right) ? (finalClipX + finalClipWidth) : right;
            bottom = ((finalClipY + finalClipHeight) < bottom) ? (finalClipY + finalClipHeight) : bottom;
            return (screenPoint.X >= left
                        && screenPoint.Y >= top
                        && screenPoint.X < right
                        && screenPoint.Y < bottom);
        }

        internal virtual void AddZSortUIElements(ref UIElement zSortList)
        {
            for (LinkedTree<UIElement> tree = rootUIElement.linkedTree; tree != null; tree = tree.NextAsList)
            {
                UIElement element = tree.Value;

                if (element.Visible)
                {
                    element.SetupFinalAlpha();

                    if (element.ZSort)
                    {
                        element.SetupSortValue();

                        element.nextZSortElement = zSortList;
                        zSortList = element;
                    }
                }
                else
                {
                    tree = tree.LastDescendant;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>シーングラフを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders a scene graph.</summary>
        /// @endif
        protected internal virtual void Render()
        {
            for (LinkedTree<UIElement> tree = rootUIElement.linkedTree; tree != null; tree = tree.NextAsList)
            {
                UIElement element = tree.Value;

                if (element.Visible && !element.ZSort)
                {
                    element.SetupFinalAlpha();
                    element.SetupDrawState();
                    element.Render();
                }
                else
                {
                    tree = tree.LastDescendant;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>自分以下の全ウィジェットの状態をリセットする。</summary>
        /// <param name="includeSelf">自分をリセットする場合はtrue。自分をリセットしない場合はfalse。</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resets the status of all widgets for under self.</summary>
        /// <param name="includeSelf">If self is reset, then true. If self is not reset, then false.</param>
        /// @endif
        protected internal void ResetState(bool includeSelf)
        {
            LinkedTree<Widget> endNode = this.LinkedTree.LastDescendant.NextAsList;
            for (LinkedTree<Widget> tree = this.LinkedTree; tree != endNode; tree = tree.NextAsList)
            {
                if (includeSelf || !tree.Value.Equals(this))
                {
                    tree.Value.OnResetState();
                }
            }
        }

        private bool _needUpdateLocalToWorld = true;

        internal bool NeedUpdateLocalToWorld
        {
            get
            {
                return _needUpdateLocalToWorld;
            }
            set
            {
                // set all desendants and uielement needUpdateLocalToWorld to true
                if (_needUpdateLocalToWorld == false && value == true)
                {
                    var end = this.linkedTree.LastDescendant.NextAsList;
                    for (var child = this.linkedTree.NextAsList; child != end; child = child.NextAsList)
                    {
                        child.Value._needUpdateLocalToWorld = true;
                        child.Value.RootUIElement.NeedUpdateLocalToWorld = true;
                    }
                    this.RootUIElement.NeedUpdateLocalToWorld = true;
                }
                _needUpdateLocalToWorld = value;
            }
        }


        /// @if LANG_JA
        /// <summary>ローカル座標系からワールド座標系に変換する行列を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the matrix to be converted from the local coordinate system to the world coordinate system.</summary>
        /// @endif
        public Matrix4 LocalToWorld
        {
            get
            {
                updateLocalToWorld();
                return this.localToWorld;
            }
        }

        internal void updateLocalToWorld()
        {
            if (NeedUpdateLocalToWorld)
            {
                if (this.Parent == null)
                {
                    localToWorld = this.Transform3D;
                }
                else
                {
                    // Following code means
                    // Matrix4 translation = Matrix4.Translation(-PivotAlignmentX, -PivotAlignmentY, 0.0f);
                    // this.LocalToWorld = Parent.LocalToWorld * this.Transform3D * translation;
                    Parent.updateLocalToWorld();
                    Parent.localToWorld.Multiply(ref this.rootUIElement.transform3D, out this.localToWorld);
                    var alignX = PivotAlignmentX;
                    var alignY = PivotAlignmentY;
                    localToWorld.M41 -= localToWorld.M11 * alignX + localToWorld.M21 * alignY;
                    localToWorld.M42 -= localToWorld.M12 * alignX + localToWorld.M22 * alignY;
                    localToWorld.M43 -= localToWorld.M13 * alignX + localToWorld.M23 * alignY;
                }

                NeedUpdateLocalToWorld = false;

                if(Focused && UISystem.focusVisible)
                {
                    SetFocus(false);
                }
            }
        }


        /// @if LANG_JA
        /// <summary>指定した座標をスクリーン座標系からローカル座標系に変換する。</summary>
        /// <returns>ローカル座標系に変換された座標</returns>
        /// <param name='screenPoint'>スクリーン座標系の座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the screen coordinate system to the local coordinate system.</summary>
        /// <returns>Coordinates converted to the local coordinate system</returns>
        /// <param name="screenPoint">Screen coordinate system coordinates</param>
        /// @endif
        public Vector2 ConvertScreenToLocal(Vector2 screenPoint)
        {
            return ConvertScreenToLocal(screenPoint.X, screenPoint.Y);
        }


        /// @if LANG_JA
        /// <summary>指定した座標をスクリーン座標系からローカル座標系に変換する。</summary>
        /// <returns>ローカル座標系に変換された座標</returns>
        /// <param name='screenX'>スクリーン座標系の座標 X</param>
        /// <param name='screenY'>スクリーン座標系の座標 Y</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the screen coordinate system to the local coordinate system.</summary>
        /// <returns>Coordinates converted to the local coordinate system</returns>
        /// <param name="screenX">Screen coordinate system coordinate X</param>
        /// <param name="screenY">Screen coordinate system coordinate Y</param>
        /// @endif
        public Vector2 ConvertScreenToLocal(float screenX, float screenY)
        {
            float focusDist = 1000.0f;

            updateLocalToWorld();
            Matrix4 inv = localToWorld.Inverse();
            Vector4 p1 = inv * new Vector4(UISystem.FramebufferWidth, UISystem.FramebufferHeight, -focusDist, 1);
            Vector4 p2 = inv * new Vector4(screenX, screenY, 0, 1);
            p1 /= p1.W;
            p2 /= p2.W;

            float a = 1 - p2.Z / p1.Z;
            Vector2 p3 = p1.Xy + (p2.Xy - p1.Xy) / a;


            return p3;
        }


        /// @if LANG_JA
        /// <summary>指定した座標をローカル座標系からスクリーン座標系に変換する。</summary>
        /// <returns>スクリーン座標系に変換された座標</returns>
        /// <param name='localPoint'>ーカル座標系の座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the local coordinate system to the screen coordinate system.</summary>
        /// <returns>Coordinates converted to the screen coordinate system</returns>
        /// <param name="localPoint">Local coordinate system coordinates</param>
        /// @endif
        public Vector2 ConvertLocalToScreen(Vector2 localPoint)
        {
            Vector2 result;
            convertLocalToScreen(ref localPoint, out result);
            return result;
        }


        /// @if LANG_JA
        /// <summary>指定した座標をローカル座標系からスクリーン座標系に変換する。</summary>
        /// <returns>スクリーン座標系に変換された座標</returns>
        /// <param name='localX'>ローカル座標系の座標 X</param>
        /// <param name='localY'>ローカル座標系の座標 Y</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the local coordinate system to the screen coordinate system.</summary>
        /// <returns>Coordinates converted to the screen coordinate system</returns>
        /// <param name="localX">Local coordinate system coordinate X</param>
        /// <param name="localY">Local coordinate system coordinate Y</param>
        /// @endif
        public Vector2 ConvertLocalToScreen(float localX, float localY)
        {
            Vector2 result;
            Vector2 localPoint = new Vector2(localX, localY);
            convertLocalToScreen(ref localPoint, out result);
            return result;
        }

        private void convertLocalToScreen(ref Vector2 localPoint, out Vector2 result)
        {
            //Vector4 ret = UISystem.screenMatrix * UISystem.ViewProjectionMatrix * this.LocalToWorld * new Vector4(localX, localY, 0, 1);
            //return (ret.Xy / ret.W);

            Matrix4 mvpMat;
            Vector2 tmp;

            updateLocalToWorld();
            UISystem.viewProjectionMatrix.Multiply(ref localToWorld, out mvpMat);
            mvpMat.TransformProjection(ref localPoint, out tmp);
            UISystem.screenMatrix.TransformProjection(ref tmp, out result);
        }

        private void convertLocalToScreen(ref Rectangle localPoint, out Rectangle result)
        {
            var left = localPoint.X;
            var top = localPoint.Y;
            var right = localPoint.X + localPoint.Width;
            var bottom = localPoint.Y + localPoint.Height;

            var v1 = ConvertLocalToScreen(left, top);
            var v2 = ConvertLocalToScreen(right, top);
            var v3 = ConvertLocalToScreen(right, bottom);
            var v4 = ConvertLocalToScreen(left, bottom);

            result.X = Math.Min(Math.Min(v1.X, v2.X), Math.Min(v3.X, v4.X));
            result.Y = Math.Min(Math.Min(v1.Y, v2.Y), Math.Min(v3.Y, v4.Y));
            result.Width = Math.Max(Math.Max(v1.X, v2.X), Math.Max(v3.X, v4.X)) - result.X;
            result.Height = Math.Max(Math.Max(v1.Y, v2.Y), Math.Max(v3.Y, v4.Y)) - result.Y;
        }


        /// @if LANG_JA
        /// <summary>指定した座標を自身のローカル座標系から他のWidgetのローカル座標系に変換する。</summary>
        /// <returns>targetWidget のローカル座標系に変換された座標</returns>
        /// <param name='targetWidget'>変換先の Widget</param>
        /// <param name='localPoint'>自身のローカル座標系の座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the local coordinate system of itself to the local coordinate system of another widget.</summary>
        /// <returns>Coordinates converted to the targetWidget local coordinate system</returns>
        /// <param name="targetWidget">Conversion destination widget</param>
        /// <param name="localPoint">Self local coordinate system coordinates</param>
        /// @endif
        public Vector2 ConvertLocalToOtherWidget(Vector2 localPoint, Widget targetWidget)
        {
            return ConvertLocalToOtherWidget(localPoint.X, localPoint.Y, targetWidget);
        }


        /// @if LANG_JA
        /// <summary>指定した座標を自身のローカル座標系から他のWidgetのローカル座標系に変換する。</summary>
        /// <returns>targetWidget のローカル座標系に変換された座標</returns>
        /// <param name='targetWidget'>変換先の Widget</param>
        /// <param name='x'>自身のローカル座標系の座標 X</param>
        /// <param name='y'>自身のローカル座標系の座標 Y</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Converts the specified coordinates from the local coordinate system of itself to the local coordinate system of another widget.</summary>
        /// <returns>Coordinates converted to the targetWidget local coordinate system</returns>
        /// <param name="targetWidget">Conversion destination widget</param>
        /// <param name="x">Self local coordinate system coordinate X</param>
        /// <param name="y">Self local coordinate system coordinate Y</param>
        /// @endif
        public Vector2 ConvertLocalToOtherWidget(float x, float y, Widget targetWidget)
        {
            Vector4 ret = targetWidget.LocalToWorld.Inverse() *
                (this.LocalToWorld * new Vector4(x, y, 0, 1));
            ret /= ret.W;
            return ret.Xy;
        }


        /// @if LANG_JA
        /// <summary>最終的なクリップ領域を構築する。</summary>
        /// <returns>表示領域が存在する場合はtrue、表示領域が存在しない場合はfalse。</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructs the final clip area.</summary>
        /// <returns>If a display area exists, then true. If a display area does not exist, then false.</returns>
        /// @endif
        internal bool SetupClipArea(bool isRootWidget)
        {
            if(!updateFinalClip(isRootWidget))
                return false;
            
            return UISystem.SetClipRegion(
                        this.finalClipX,
                        this.finalClipY,
                        this.finalClipWidth,
                        this.finalClipHeight);
        }

        internal bool updateFinalClip(bool isRootWidget)
        {
            if (isRootWidget || this.Parent == null)
            {
                this.finalClipX = 0;//l2w.M41;
                this.finalClipY = 0;//l2w.M42;
                this.finalClipWidth = this.Width;
                this.finalClipHeight = this.Height;
            }
            else
            {
                // clipping
                updateLocalToWorld();
                if (UIDebug.WidgetClipping == UIDebug.ForceMode.ForceEnabled ||
                    this.Clip == true && UIDebug.WidgetClipping == UIDebug.ForceMode.Normal)
                {
                    float left = Math.Max(localToWorld.M41, this.Parent.finalClipX);
                    float top = Math.Max(localToWorld.M42, this.Parent.finalClipY);
                    float right = Math.Min(localToWorld.M41 + this.Width, this.Parent.finalClipX + this.Parent.finalClipWidth);
                    float bottom = Math.Min(localToWorld.M42 + this.Height, this.Parent.finalClipY + this.Parent.finalClipHeight);

                    this.finalClipX = left;
                    this.finalClipY = top;
                    this.finalClipWidth = (right > left) ? right - left : 0;
                    this.finalClipHeight = (bottom > top) ? bottom - top : 0;
                }
                else
                {
                    this.finalClipX = this.Parent.finalClipX;
                    this.finalClipY = this.Parent.finalClipY;
                    this.finalClipWidth = this.Parent.finalClipWidth;
                    this.finalClipHeight = this.Parent.finalClipHeight;
                }

                if (this.finalClipWidth < 0.5f || this.finalClipHeight < 0.5f)
                {
                    return false;
                }
            }
            return true;
        }

        /// @if LANG_JA
        /// <summary>最終的なアルファ値を計算する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Calculates the final alpha value.</summary>
        /// @endif
        protected internal virtual void SetupFinalAlpha()
        {
            if (this.Parent != null)
            {
                this.RootUIElement.finalAlpha = this.Alpha * this.Parent.RootUIElement.finalAlpha;
            }
            else
            {
                this.RootUIElement.finalAlpha = this.Alpha;
            }
        }

        private float PivotAlignmentX
        {
            get
            {
                switch ((int)pivotType & PivotHorizontalMask)
                {
                    case ((int)PivotType.TopLeft & PivotHorizontalMask):
                        // case XxxLeft
                        return 0.0f;
                    case ((int)PivotType.TopCenter & PivotHorizontalMask):
                        // case XxxCenter
                        return Width / 2.0f;
                    case ((int)PivotType.TopRight & PivotHorizontalMask):
                        // case XxxRight
                        return Width;
                    default:
                        UIDebug.Assert(false, "invalid PivotType");
                        return 0.0f;
                }

            }
        }

        private float PivotAlignmentY
        {
            get
            {
                switch ((int)pivotType & PivotVerticalMask)
                {
                    case ((int)PivotType.TopLeft & PivotVerticalMask):
                        // case TopXxx
                        return 0.0f;
                    case ((int)PivotType.MiddleLeft & PivotVerticalMask):
                        // case MiddleXxx
                        return Height / 2.0f;
                    case ((int)PivotType.BottomLeft & PivotVerticalMask):
                        // case BottomXxx
                        return Height;
                    default:
                        UIDebug.Assert(false, "invalid PivotType");
                        return 0.0f;
                }
            }
        }

        internal float CenterX
        {
            get
            {
                return X - PivotAlignmentX + Width / 2.0f;
            }
            set
            {
                X = value + PivotAlignmentX - Width / 2.0f;
            }
        }

        internal float CenterY
        {
            get
            {
                return Y - PivotAlignmentY + Height / 2.0f;
            }
            set
            {
                Y = value + PivotAlignmentY - Height / 2.0f;
            }
        }

        /// @if LANG_JA
        /// <summary>優先してタッチに反応するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to prioritize and respond to a touch.</summary>
        /// @endif
        public virtual bool PriorityHit
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>テクスチャへ描画する。</summary>
        /// <param name="texture">2Dのテクスチャ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders to the texture.</summary>
        /// <param name="texture">2D texture</param>
        /// @endif
        public void RenderToTexture(Texture2D texture)
        {
            RenderToTexture(texture, Matrix4.Identity);
        }

        /// @if LANG_JA
        /// <summary>テクスチャへ描画する。</summary>
        /// <param name="texture">2Dのテクスチャ</param>
        /// <param name="transform">変換行列</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders to the texture.</summary>
        /// <param name="texture">2D texture</param>
        /// <param name="transform">Conversion matrix</param>
        /// @endif
        public void RenderToTexture(Texture2D texture, Matrix4 transform)
        {
#if !BUG1967
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }
            if (! texture.IsRenderable)
            {
                throw new ArgumentException("Texture is not renderable.", "texture");
            }
#endif
            
            FrameBuffer frameBuffer = UISystem.offScreenFramebufferCache;
            frameBuffer.SetColorTarget(texture, 0);

            RenderToFrameBuffer(frameBuffer, ref transform, true);
        }

        /// @if LANG_JA
        /// <summary>フレームバッファへ描画する。</summary>
        /// <param name="frameBuffer">フレームバッファ</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders to the frame buffer.</summary>
        /// <param name="frameBuffer">FrameBuffer</param>
        /// @endif
        public void RenderToFrameBuffer(FrameBuffer frameBuffer)
        {
            Matrix4 transform = Matrix4.Identity;
            RenderToFrameBuffer(frameBuffer, ref transform, true);
        }

        /// @if LANG_JA
        /// <summary>フレームバッファへ描画する。</summary>
        /// <param name="frameBuffer">フレームバッファ</param>
        /// <param name="transform">変換行列</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders to the frame buffer.</summary>
        /// <param name="frameBuffer">FrameBuffer</param>
        /// <param name="transform">Conversion matrix</param>
        /// @endif
        public void RenderToFrameBuffer(FrameBuffer frameBuffer, Matrix4 transform)
        {
            RenderToFrameBuffer(frameBuffer, ref transform, true);
        }

        internal void RenderToFrameBuffer(FrameBuffer frameBuffer, ref Matrix4 transform, bool useOrthoProjection)
        {
#if !BUG1967
            if (frameBuffer == null)
            {
                throw new ArgumentNullException("frameBuffer");
            }
            if (!frameBuffer.Status)
            {
                throw new ArgumentException("Frame buffer is not renderable.", "frameBuffer");
            }
#endif
            
            UISystem.IsOffScreenRendering = true;

            GraphicsContext graphics = UISystem.GraphicsContext;
            int width = frameBuffer.Width;
            int height = frameBuffer.Height;

            // save state
            Matrix4 originalProjectionMatrix = UISystem.ViewProjectionMatrix;
            Matrix4 originalTransform3D = this.Transform3D;
            var originalPivotType = this.PivotType;
            var originalCullFace = graphics.GetCullFace();
            var originalColorMask = graphics.GetColorMask();
            var originalFrameBuffer = graphics.GetFrameBuffer();
            var originalScreenOrientation = UISystem.CurrentScreenOrientation;

            this.PivotType = PivotType.TopLeft;
            this.Transform3D = transform;

            // change widget-tree-node into root
            var originalParentNode = this.linkedTree.Parent;
            LinkedTree<Widget> originalNextSiblingNode = null;
            if (originalParentNode != null)
            {
                originalNextSiblingNode = this.LinkedTree.NextSibling;
                this.linkedTree.RemoveChild();
            }

            graphics.SetFrameBuffer(frameBuffer);
            graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Cw); // invert y-axis
            graphics.SetColorMask(ColorMask.Rgba);
            UISystem.SetClipRegionFull();
            graphics.SetViewport(0, 0, width, height);
            graphics.SetClearColor(0f, 0f, 0f, 0f);
            graphics.Clear();

            if (useOrthoProjection)
            {
                UISystem.SetCurrentScreenOrientation(ScreenOrientation.Landscape);

                // set ortho matrix
                const float far = 100000.0f;
                const float near = -100000.0f;
                const float left = 0;
                      float right = width / UISystem.PixelDensity;
                const float bottom = 0; // invert y-axis
                      float top = height / UISystem.PixelDensity;  // invert y-axis
                UISystem.ViewProjectionMatrix =
                    Matrix4.Ortho(left,right,bottom,top,near,far);

//                UISystem.ViewProjectionMatrix = new Matrix4(
//                    2.0f / width * UISystem.PixelDensity, 0.0f, 0.0f, 0.0f,
//                    0.0f, 2.0f / height * UISystem.PixelDensity, 0.0f, 0.0f,
//                    0.0f, 0.0f, -2.0f / (far - near), 0.0f,
//                    -1.0f, 1.0f, (far + near) / (far - near), 1.0f
//                );
            }
            else
            {
                var scene = this.ParentScene;
                if(scene != null)
                    UISystem.SetCurrentScreenOrientation(scene.ScreenOrientation);
                else
                    UISystem.SetCurrentScreenOrientation(ScreenOrientation.Landscape);

                // invert y-axis
                Matrix4 yInvertMat = new Matrix4(1, 0, 0, 0,
                                                 0,-1, 0, 0,
                                                 0, 0, 1, 0,
                                                 0, 0, 0, 1);
                UISystem.viewProjectionMatrix = yInvertMat * UISystem.viewProjectionMatrix;
            }

            // render this widget
            UISystem.Render(this);


            // restore
            graphics.SetFrameBuffer(originalFrameBuffer);
            UISystem.SetClipRegionFull();
            graphics.SetColorMask(originalColorMask);
            graphics.SetCullFace(originalCullFace);
            UISystem.ViewProjectionMatrix = originalProjectionMatrix;
            UISystem.SetCurrentScreenOrientation(originalScreenOrientation);
            // restore widget tree
            if (originalNextSiblingNode != null)
            {
                this.linkedTree.InsertChildBefore(originalNextSiblingNode);
            }
            else if (originalParentNode != null)
            {
                originalParentNode.AddChildLast(this.linkedTree);
            }
            // restore widget position
            this.pivotType = originalPivotType;
            this.Transform3D = originalTransform3D;


            UISystem.IsOffScreenRendering = false;
        }

        /// @if LANG_JA
        /// <summary>ジェスチャー検出機構を追加する。</summary>
        /// <param name="gestureDetector">ジェスチャ検出機構</param>
        /// <exception cref="ArgumentNullException">GestureDetectorがnull</exception>
        /// <returns>追加できたかどうか</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Adds a gesture detection mechanism.</summary>
        /// <param name="gestureDetector">Gesture detection mechanism</param>
        /// <exception cref="ArgumentNullException">GestureDetector is null</exception>
        /// <returns>Whether addition was possible or not</returns>
        /// @endif
        public bool AddGestureDetector(GestureDetector gestureDetector)
        {
            if (gestureDetector == null)
            {
                throw new ArgumentNullException("gestureDetector");
            }

            if (gestureDetector.TargetWidget == null)
            {
                this.GestureDetectors.Add(gestureDetector);
                gestureDetector.TargetWidget = this;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// @if LANG_JA
        /// <summary>ジェスチャ検出機能を削除する。</summary>
        /// <param name="gestureDetector">ジェスチャ検出機構</param>
        /// <exception cref="ArgumentNullException">GestureDetectorがnull</exception>
        /// <returns>削除できたかどうか</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Deletes the gesture detection mechanism.</summary>
        /// <param name="gestureDetector">Gesture detection mechanism</param>
        /// <exception cref="ArgumentNullException">GestureDetector is null</exception>
        /// <returns>Whether deletion was possible or not</returns>
        /// @endif
        public bool RemoveGestureDetector(GestureDetector gestureDetector)
        {
            if (gestureDetector == null)
            {
                throw new ArgumentNullException("gestureDetector");
            }

            if (gestureDetector.TargetWidget == this && this.GestureDetectors.Remove(gestureDetector))
            {
                gestureDetector.TargetWidget = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal List<GestureDetector> GestureDetectors
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>文字列を返す</summary>
        /// <returns>"Name : 型名" 形式の文字列（Nameプロパティが設定されている場合）</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Returns a character string</summary>
        /// <returns>Character string of "Name: model name" (when Name property is set)</returns>
        /// @endif
        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                return base.ToString();
            }
            else
            {
                return Name + " : " + this.GetType().Name;
            }
        }

        /// @if LANG_JA
        /// <summary>フォーカスが当たるかどうかを取得・設定する</summary>
        /// <remarks>親ウィジェットが false でも、子ウィジェットが true であれば子ウィジェットにフォーカスが当たる可能性があります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to set the focus</summary>
        /// <remarks>If the child widget is true, the child widget may have focus even when the parent widget is false.</remarks>
        /// @endif
        public virtual bool Focusable
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>フォーカスの設定情報を取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the focus setting information</summary>
        /// @endif
        public virtual FocusCustomSettings FocusCustomSettings
        {
            get;
            set;
        }

        internal bool getFinalVisible()
        {
            for (var node = this.linkedTree; node != null; node = node.Parent)
            {
                if (!node.Value.Visible)
                    return false;
            }

            return true;
        }

        internal ClippedState getClippedState()
        {
            const float epsilon = 0.5f;

            updateLocalToWorld();
            var l = localToWorld.M41;
            var r = l + this.Width;
            var t = localToWorld.M42;
            var b = t + this.Height;

            var clipL = this.finalClipX - epsilon;
            var clipR = this.finalClipX + this.finalClipWidth + epsilon;
            var clipT = this.finalClipY - epsilon;
            var clipB = this.finalClipY + this.finalClipHeight + epsilon;

            if (l >= clipR || r <= clipL || t >= clipB || b <= clipT)
                return ClippedState.ClippedAll;

            if (l < clipL || r > clipR || t < clipT || b > clipB)
                return ClippedState.ClippedHalf;

            return ClippedState.NoClipped;
        }

        internal enum ClippedState
        {
            ClippedAll,
            ClippedHalf,
            NoClipped,
        }

        /// @if LANG_JA
        /// <summary>このウィジェットがフォーカスを取得または喪失したときに呼び出される</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Called when this widget obtains or loses focus</summary>
        /// @endif
        internal protected virtual void OnFocusChanged(FocusChangedEventArgs args)
        {
            if (!args.Focused)
            {
                this.ResetState(true);
            }

            for (var node = this.linkedTree.Parent; node != null; node = node.Parent)
            {
                node.Value.OnFocusChangedChild(args);
            }

            if (FocusChanged != null)
            {
                FocusChanged(this, args);
            }
        }

        internal virtual void OnFocusChangedChild(FocusChangedEventArgs args)
        {
        }

        /// @if LANG_JA
        /// <summary>このウィジェットがフォーカスを取得または喪失したときに呼び出されるイベント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event called when this widget obtains or loses focus</summary>
        /// @endif
        public event EventHandler<FocusChangedEventArgs> FocusChanged;

        /// @if LANG_JA
        /// <summary>フォーカスの表示スタイルを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the focus display style</summary>
        /// @endif
        public FocusStyle FocusStyle { get; set; }

        #region pressable

        private bool pressable = false;

        /// @if LANG_JA
        /// <summary>タッチやキーでプレスできるかどうかを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether it can be pressed by a touch or key</summary>
        /// @endif
        protected bool Pressable
        {
            get { return pressable; }
            set
            {
                if (pressable != value)
                {
                    pressable = value;
                    PriorityHit = pressable;
                    Focusable = pressable;
                }
            }
        }


        private PressState pressState = PressState.Normal;

        /// @if LANG_JA
        /// <summary>このウィジェットが押されているかどうかの状態を取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the status for whether or not this widget is pressed</summary>
        /// @endif
        protected virtual PressState PressState
        {
            get { return pressState; }
            set
            {
                var oldState = pressState;
                pressState = value;

                OnPressStateChanged(new PressStateChangedEventArgs(PressStateChangedReason.ChangePressStateProperty, pressState, oldState));
            }
        }

        bool IsReset = false;

        /// @if LANG_JA
        /// <summary>タッチが領域の内側から外側に移動した場合のプレス状態の挙動を取得・設定する</summary>
        /// <remarks>この値は Pressable プロパティがtrueの場合のみ有効です。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the behavior of the press status when the touch is moved from the inside of the area to the outside</summary>
        /// <remarks>This value is enabled only when the Pressable property is true.</remarks>
        /// @endif
        protected PressStateTouchLeaveBehavior TouchLeaveBehavior { get; set; }

        /// @if LANG_JA
        /// <summary>機能を有効にするかどうかを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to enable the feature</summary>
        /// @endif
        public virtual bool Enabled
        {
            get
            {
                return this.pressState != PressState.Disabled;
            }
            set
            {
                PressStateChangedEventArgs args = null;
                if (value)
                {
                    if (this.pressState == PressState.Disabled)
                    {
                        args = new PressStateChangedEventArgs(PressStateChangedReason.ChangeEnabledProperty, PressState.Normal, this.pressState);
                        this.pressState = PressState.Normal;
                    }
                }
                else
                {
                    if (this.pressState != PressState.Disabled)
                    {
                        args = new PressStateChangedEventArgs(PressStateChangedReason.ChangeEnabledProperty, PressState.Disabled, this.pressState);
                        this.pressState = PressState.Disabled;
                    }
                }

                if (args != null)
                {
                    OnPressStateChanged(args);
                }
            }
        }


        void pressableOnTouchEvent(TouchEventCollection touchEvents)
        {
            if (this.IsReset && touchEvents.PrimaryTouchEvent.Type == TouchEventType.Down)
            {
                this.IsReset = false;
            }

            if (this.pressState != PressState.Disabled && !this.IsReset)
            {
                PressStateChangedEventArgs args = null;

                switch (touchEvents.PrimaryTouchEvent.Type)
                {
                    case TouchEventType.Down:
                        if(this.Focusable) this.SetFocus(false);
                        this.IsReset = false;
                        args = new PressStateChangedEventArgs(PressStateChangedReason.TouchDown, PressState.Pressed, this.pressState);
                        this.pressState = PressState.Pressed;
                        break;
                    case TouchEventType.Up:
                        if (this.pressState == PressState.Pressed)
                        {
                            args = new PressStateChangedEventArgs(PressStateChangedReason.TouchUp, PressState.Normal, this.pressState);
                            this.pressState = PressState.Normal;
                        }
                        break;
                    case TouchEventType.Enter:
                        if (this.TouchLeaveBehavior == PressStateTouchLeaveBehavior.ResumeByTouchEnter)
                        {
                            args = new PressStateChangedEventArgs(PressStateChangedReason.TouchEnter, PressState.Pressed, this.pressState);
                            this.pressState = PressState.Pressed;
                        }
                        UIDebug.Assert(this.TouchLeaveBehavior == PressStateTouchLeaveBehavior.End || this.pressState == PressState.Pressed);
                        break;
                    case TouchEventType.Leave:
                        if (this.TouchLeaveBehavior != PressStateTouchLeaveBehavior.KeepPressed)
                        {
                            args = new PressStateChangedEventArgs(PressStateChangedReason.TouchLeave, PressState.Normal, this.pressState);
                            this.pressState = PressState.Normal;
                        }
                        break;
                }

                if (args != null)
                {
                    OnPressStateChanged(args);
                }
            }
        }

        void pressableOnKeyEvent(KeyEvent keyEvent)
        {
            if (this.IsReset && keyEvent.KeyType == KeyType.Enter && keyEvent.KeyEventType == KeyEventType.Down)
            {
                this.IsReset = false;
            }

            if (this.pressState != PressState.Disabled && !this.IsReset)
            {
                PressStateChangedEventArgs args = null;

                if (keyEvent.KeyType == KeyType.Enter)
                {
                    if (keyEvent.KeyEventType == KeyEventType.Down && this.Focused)
                    {
                        args = new PressStateChangedEventArgs(PressStateChangedReason.KeyDown, PressState.Pressed, this.pressState);
                        this.pressState = PressState.Pressed;
                    }
                    else if (keyEvent.KeyEventType == KeyEventType.Up)
                    {
                        if (this.pressState == PressState.Pressed)
                        {
                            args = new PressStateChangedEventArgs(PressStateChangedReason.KeyUp, PressState.Normal, this.pressState);
                            this.pressState = PressState.Normal;
                        }
                        UIDebug.Assert(this.pressState == PressState.Normal);
                    }
                }

                if (args != null)
                {
                    OnPressStateChanged(args);
                }
            }
        }

        /// @if LANG_JA
        /// <summary>プレス状態が変化したときに呼び出される</summary>
        /// <param name="e">イベント引数</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Called when the press status changes</summary>
        /// <param name="e">Event argument</param>
        /// @endif
        protected virtual void OnPressStateChanged(PressStateChangedEventArgs e)
        {
        }

        #endregion


        internal float finalClipX;
        internal float finalClipY;
        internal float finalClipWidth;
        internal float finalClipHeight;
        private Matrix4 localToWorld;
    }

}
