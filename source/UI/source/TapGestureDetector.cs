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
        /// <summary>タップ検出器</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Tap detector</summary>
        /// @endif
        public class TapGestureDetector : GestureDetector
        {
            const float defaultMaxDistanceInch = 0.169f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TapGestureDetector()
            {
                this.MaxDistance = defaultMaxDistanceInch * UISystem.Dpi;
                this.MaxPressDuration = 300.0f;
                this.TapDetected = null;

                this.downID = 0;
                this.downPos = Vector2.Zero;
                this.downTime = TimeSpan.Zero;
            }

            /// @if LANG_JA
            /// <summary>最初のdownの位置からの最大許容距離を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum allowable distance from the first down position.</summary>
            /// @endif
            public float MaxDistance
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>downからupまでの最大時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum time until down changes to up. (ms)</summary>
            /// @endif
            public float MaxPressDuration
            {
                get;
                set;
            }

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
                        this.downPos = touchEvent.WorldPosition;
                        this.downTime = touchEvent.Time;
                        response = GestureDetectorResponse.UndetectedAndContinue;
                        break;

                    case TouchEventType.Up:
                        Vector2 upPos = touchEvent.WorldPosition;
                        float distance = downPos.Distance(upPos);

                        float elapsedTime = (float)(touchEvent.Time - this.downTime).TotalMilliseconds;

                        if ((distance <= this.MaxDistance) && (elapsedTime <= this.MaxPressDuration))
                        {
                            if (this.TapDetected != null)
                            {
                                TapEventArgs args = new TapEventArgs(this.TargetWidget,
                                                                     touchEvent.WorldPosition,
                                                                     touchEvent.LocalPosition);
                                this.TapDetected(this, args);
                            }
                            response = GestureDetectorResponse.DetectedAndStop;
                        }
                        else
                        {
                            response = GestureDetectorResponse.FailedAndStop;
                        }
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
                this.downPos = Vector2.Zero;
                this.downTime = TimeSpan.Zero;
            }

            /// @if LANG_JA
            /// <summary>タップイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Tap event handler</summary>
            /// @endif
            public event EventHandler<TapEventArgs> TapDetected;

            private int downID;
            private Vector2 downPos;
            private TimeSpan downTime;

        }


        /// @if LANG_JA
        /// <summary>タップイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Tap event argument</summary>
        /// @endif
        public class TapEventArgs : GestureEventArgs
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="source">イベント発生元ウィジェット</param>
            /// <param name="worldPosition">ワールド（スクリーン）座標系での位置</param>
            /// <param name="localPosition">ローカル座標系での位置</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="source">Event source widget</param>
            /// <param name="worldPosition">Position in the world (screen) coordinate system</param>
            /// <param name="localPosition">Position in the local coordinate system</param>
            /// @endif
            public TapEventArgs(Widget source, Vector2 worldPosition, Vector2 localPosition) : base(source)
            {
                this.WorldPosition = worldPosition;
                this.LocalPosition = localPosition;
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
        }


    }
}
