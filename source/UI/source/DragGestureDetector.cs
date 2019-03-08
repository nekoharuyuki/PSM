/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>ドラッグ検出器</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Drag detector</summary>
        /// @endif
        public class DragGestureDetector : GestureDetector
        {
            const float defaultMaxDistanceInch = 0.169f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public DragGestureDetector()
            {
                this.MaxDistance = defaultMaxDistanceInch * UISystem.Dpi;
                this.DragDetected = null;

                this.downID = 0;
                this.previousWorldPosition = Vector2.Zero;
            }

            /// @if LANG_JA
            /// <summary>最大移動距離を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum travel distance.</summary>
            /// @endif
            public float MaxDistance
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>ドラッグジェスチャの判定方向を取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the determination direction of the drag gesture</summary>
            /// @endif
            public DragDirection Direction { get; set; }            

            /// @if LANG_JA
            /// <summary>タッチイベントを配信する。</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// <returns>タッチイベント配信の応答</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Distributes a touch event.</summary>
            /// <param name="touchEvents">Touch event</param>
            /// <returns>Response to touch event distribution</returns>
            /// @endif
            protected internal override GestureDetectorResponse OnTouchEvent(TouchEventCollection touchEvents)
            {
                GestureDetectorResponse response = this.State;
                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

                if ((this.State != GestureDetectorResponse.None) && (this.downID != touchEvent.FingerID))
                {
                    return response;
                }

                switch (touchEvent.Type)
                {
                    case TouchEventType.Down:
                        this.downID = touchEvent.FingerID;
                        this.previousWorldPosition = touchEvent.WorldPosition;
                        response = GestureDetectorResponse.UndetectedAndContinue;
                        break;

                    case TouchEventType.Move:
                        if (this.State == GestureDetectorResponse.UndetectedAndContinue)
                        {
                            Vector2 distance = touchEvent.WorldPosition - this.previousWorldPosition;
                            if (Direction == DragDirection.All)
                            {
                                if (distance.LengthSquared() > this.MaxDistance * this.MaxDistance)
                                {
                                    response = GestureDetectorResponse.DetectedAndContinue;
                                }
                            }
                            else
                            {
                                float targetDistance, otherDistance;
                                if (this.Direction == DragDirection.Horizontal ||
                                    this.Direction == DragDirection.LimitedHorizontal)
                                {
                                    targetDistance = Math.Abs(distance.X);
                                    otherDistance = Math.Abs(distance.Y);
                                }
                                else // Vertical
                                {
                                    targetDistance = Math.Abs(distance.Y);
                                    otherDistance = Math.Abs(distance.X);
                                }

                                if (this.Direction == DragDirection.Horizontal ||
                                    this.Direction == DragDirection.Vertical)
                                {
                                    if (targetDistance > this.MaxDistance && targetDistance > otherDistance)
                                    {
                                        response = GestureDetectorResponse.DetectedAndContinue;
                                    }
                                }
                                else // LimitedH/V
                                {
                                    if (targetDistance > this.MaxDistance)
                                    {
                                        response = GestureDetectorResponse.DetectedAndContinue;
                                    }
                                    else
                                    {
                                        if (otherDistance > this.MaxDistance)
                                        {
                                            response = GestureDetectorResponse.FailedAndStop;
                                        }
                                    }
                                }
                            }

                            if (response == GestureDetectorResponse.DetectedAndContinue)
                            {
                                DragEventArgs args = new DragEventArgs(
                                    this.TargetWidget,
                                    touchEvent.WorldPosition,
                                    touchEvent.LocalPosition,
                                    distance);

                                previousWorldPosition = touchEvent.WorldPosition;

                                if (this.DragStartDetected != null)
                                {
                                    this.DragStartDetected(this, args);
                                }
                            }
                        }
                        else
                        {
                            if (this.DragDetected != null)
                            {
                                DragEventArgs args = new DragEventArgs(
                                    this.TargetWidget,
                                    touchEvent.WorldPosition,
                                    touchEvent.LocalPosition,
                                    touchEvent.WorldPosition - this.previousWorldPosition);
                                this.DragDetected(this, args);
                            }
                            previousWorldPosition = touchEvent.WorldPosition;
                            response = GestureDetectorResponse.DetectedAndContinue;
                        }
                        break;

                    case TouchEventType.Up:
                        if (this.DragEndDetected != null)
                        {
                            DragEventArgs args = new DragEventArgs(
                                this.TargetWidget,
                                touchEvent.WorldPosition,
                                touchEvent.LocalPosition,
                                touchEvent.WorldPosition - this.previousWorldPosition);
                            this.DragEndDetected(this, args);
                        }
                        previousWorldPosition = touchEvent.WorldPosition;
                        response = GestureDetectorResponse.FailedAndStop;
                        break;
                }

                return response;
            }

            /// @if LANG_JA
            /// <summary>ジェスチャ解析の状態をリセットする。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Resets the state of gesture analysis.</summary>
            /// @endif
            protected internal override void OnResetState()
            {
                this.downID = 0;
                this.previousWorldPosition = Vector2.Zero;
            }

            /// @if LANG_JA
            /// <summary>ドラッグイベントハンドラ</summary>
            /// <remarks>初回のDragDetectedイベントはDragStartDetectedイベントの次のフレームから発行されます。
            /// ドラッグ操作が開始すると、タッチアップか他のジェスチャディテクタによって終了させられるまで、移動距離にかかわらず毎フレーム発行します。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Drag event handler</summary>
            /// <remarks>The initial DragDetected event is issued from the next frame of the DragStartDetected event.
            /// When dragging is started, each frame is issued regardless of travel distance until ended either by touching up or the gesture detector.</remarks>
            /// @endif
            public event EventHandler<DragEventArgs> DragDetected;

            /// @if LANG_JA
            /// <summary>ドラッグ開始のイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Drag start event handler</summary>
            /// @endif
            public event EventHandler<DragEventArgs> DragStartDetected;

            /// @if LANG_JA
            /// <summary>ドラッグ終了のイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Drag end event handler</summary>
            /// @endif
            public event EventHandler<DragEventArgs> DragEndDetected;

            private int downID;
            private Vector2 previousWorldPosition;

        }

        /// @if LANG_JA
        /// <summary>DragGestureの判定方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>DragGesture evaluation direction</summary>
        /// @endif
        public enum DragDirection
        {
            /// @if LANG_JA
            /// <summary>全方向判定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines all directions</summary>
            /// @endif
            All = 0,
            /// @if LANG_JA
            /// <summary>垂直方向のみ判定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines only the vertical direction</summary>
            /// @endif
            Vertical,
            /// @if LANG_JA
            /// <summary>水平方向のみ判定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines only the horizontal direction</summary>
            /// @endif
            Horizontal,
            /// @if LANG_JA
            /// <summary>垂直方向のみ判定し、水平方向に移動した場合はその時点で判定を終了する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines only the vertical direction, and ends determination at the point where moved to the horizontal direction</summary>
            /// @endif
            LimitedVertical,
            /// @if LANG_JA
            /// <summary>水平方向のみ判定し、垂直方向に移動した場合はその時点で判定を終了する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines only the horizontal direction, and ends determination at the point where moved to the vertical direction</summary>
            /// @endif
            LimitedHorizontal,
        }


        /// @if LANG_JA
        /// <summary>ドラッグイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Drag event argument</summary>
        /// @endif
        public class DragEventArgs : GestureEventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="source">イベント発生元ウィジェット</param>
            /// <param name="worldPosition">ワールド（スクリーン）座標系での位置</param>
            /// <param name="localPosition">ローカル座標系での位置</param>
            /// <param name="distance">移動距離</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="source">Event source widget</param>
            /// <param name="worldPosition">Position in the world (screen) coordinate system</param>
            /// <param name="localPosition">Position in the local coordinate system</param>
            /// <param name="distance">Travel distance</param>
            /// @endif
            public DragEventArgs(Widget source, Vector2 worldPosition, Vector2 localPosition, Vector2 distance) : base(source)
            {
                this.WorldPosition = worldPosition;
                this.LocalPosition = localPosition;
                this.Distance = distance;
            }

            /// @if LANG_JA
            /// <summary>ワールド（スクリーン）座標系での位置を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the position in the world (screen) coordinate system.</summary>
            /// @endif
            public Vector2 WorldPosition
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>ローカル座標系での位置を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the position in the local coordinate system.</summary>
            /// @endif
            public Vector2 LocalPosition
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>前回のイベントからの移動距離（ベクトル）を取得する</summary>
            /// <remarks>この値は、DragGestureDetector.DetectDirection の値に関係なく、水平垂直両方向の移動距離となります。
            /// DragStartDetectedイベントではタッチダウン位置からの距離、DragDetectedイベントでは1フレーム前のタッチ位置からの距離となります。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the travel distance (vector) from the previous event</summary>
            /// <remarks>This value is the travel distance in the horizontal and vertical directions and is not related to the DragGestureDetector.DetectDirection value.
            /// At DragStartDetected events, it is the distance from the touch-down position, and at DragDetected events, it is the distance from the touch position one frame earlier.</remarks>
            /// @endif
            public Vector2 Distance
            {
                get;
                private set;
            }

        }


    }
}
