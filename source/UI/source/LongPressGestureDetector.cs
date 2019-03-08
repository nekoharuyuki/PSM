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
        /// <summary>ロングプレス検出器</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Long press detector</summary>
        /// @endif
        public class LongPressGestureDetector : GestureDetector
        {
            const float defaultMaxDistanceInch = 0.169f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public LongPressGestureDetector()
            {
                this.MaxDistance = defaultMaxDistanceInch * UISystem.Dpi;
                this.MinPressDuration = 1000.0f;
                this.LongPressDetected = null;

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
            /// <summary>ロングプレスを検出するまでの最小時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the minimum time until a long press is detected. (ms)</summary>
            /// @endif
            public float MinPressDuration
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

                    case TouchEventType.Move:
                        Vector2 currentPox = touchEvent.WorldPosition;
                        float distance = currentPox.Distance(downPos);

                        float elapsedTime = (float)(touchEvent.Time - this.downTime).TotalMilliseconds;

                        if (distance <= this.MaxDistance)
                        {
                            if (elapsedTime >= this.MinPressDuration)
                            {
                                if (this.LongPressDetected != null)
                                {
                                    LongPressEventArgs args = new LongPressEventArgs(this.TargetWidget,
                                                                                     touchEvent.WorldPosition,
                                                                                     touchEvent.LocalPosition,
                                                                                     elapsedTime);
                                    this.LongPressDetected(this, args);
                                }
                                response = GestureDetectorResponse.DetectedAndContinue;
                            }
                        }
                        else
                        {
                            response = GestureDetectorResponse.FailedAndStop;
                        }
                        break;

                    case TouchEventType.Up:
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
                this.downPos = Vector2.Zero;
                this.downTime = TimeSpan.Zero;
            }

            /// @if LANG_JA
            /// <summary>ロングプレスイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Long press event handler</summary>
            /// @endif
            public event EventHandler<LongPressEventArgs> LongPressDetected;

            private int downID;
            private Vector2 downPos;
            private TimeSpan downTime;

        }


        /// @if LANG_JA
        /// <summary>ロングプレスイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Long press event argument</summary>
        /// @endif
        public class LongPressEventArgs : GestureEventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="source">イベント発生元ウィジェット</param>
            /// <param name="worldPosition">ワールド（スクリーン）座標系での位置</param>
            /// <param name="localPosition">ローカル座標系での位置</param>
            /// <param name="elapsedTime">経過時間</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="source">Event source widget</param>
            /// <param name="worldPosition">Position in the world (screen) coordinate system</param>
            /// <param name="localPosition">Position in the local coordinate system</param>
            /// <param name="elapsedTime">Elapsed time</param>
            /// @endif
            public LongPressEventArgs(Widget source, Vector2 worldPosition, Vector2 localPosition, float elapsedTime) : base(source)
            {
                this.WorldPosition = worldPosition;
                this.LocalPosition = localPosition;
                this.ElapsedTime = elapsedTime;
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
            /// <summary>経過時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the elapsed time. (ms)</summary>
            /// @endif
            public float ElapsedTime
            {
                get;
                private set;
            }
        }


    }
}
