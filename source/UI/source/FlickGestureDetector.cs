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
        /// <summary>フリックの方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Flick direction</summary>
        /// @endif
        public enum FlickDirection
        {
            /// @if LANG_JA
            /// <summary>全方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>All directions</summary>
            /// @endif
            All = 0,

            /// @if LANG_JA
            /// <summary>垂直方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Vertical</summary>
            /// @endif
            Vertical,

            /// @if LANG_JA
            /// <summary>水平方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Horizontal</summary>
            /// @endif
            Horizontal,
            
            
            /// @internal
            /// @if LANG_JA
            /// <summary></summary>
            /// @endif
            /// @if LANG_EN
            /// <summary></summary>
            /// @endif
            [Obsolete("use Vertical")]
            Virtical = Vertical,
        }


        /// @if LANG_JA
        /// <summary>フリック検出器</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Flick detector</summary>
        /// @endif
        public class FlickGestureDetector : GestureDetector
        {
            const float defaultMinSpeedInch = 0.4f;
            const float defaultMaxSpeedInch = 8.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public FlickGestureDetector()
            {
                this.MinSpeed = defaultMinSpeedInch * UISystem.Dpi;
                this.MaxSpeed = defaultMaxSpeedInch * UISystem.Dpi;
                this.Direction = FlickDirection.All;

                Initialize();
            }

            /// @if LANG_JA
            /// <summary>フリックとして判定する最小速度を取得・設定する。（ピクセル/秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the minimum speed for determining a flick. (pixels/second)</summary>
            /// @endif
            public float MinSpeed
            {
                get{ return minSpeed;}
                set
                {
                    if (value < 0.0f)
                        minSpeed = 0.0f;
                    else
                        minSpeed = value;
                }
            }
            private float minSpeed;

            /// @if LANG_JA
            /// <summary>フリック速度の最大値を取得・設定する。（ピクセル/秒）</summary>
            /// <remarks>FlickDetectedイベント発生時にフリック速度の絶対値がこの値以上であった場合、フリック速度にこの値が設定されます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the maximum flick speed value. (pixels/second)</summary>
            /// <remarks>This value is set to the flick speed when the absolute value of the flick speed is greater than this value at the time a FlickDetected event occurs.</remarks>
            /// @endif
            public float MaxSpeed
            {
                get { return maxSpeed; }
                set
                {
                    if (value < 0.0f)
                        maxSpeed = 0.0f;
                    else
                        maxSpeed = value;
                }
            }
            private float maxSpeed;

            /// @if LANG_JA
            /// <summary>検出するフリックの方向を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the flick direction to be detected.</summary>
            /// @endif
            public FlickDirection Direction
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

                        for (int i = 0; i < 2; i++)
                        {
                            this.previousWorldPosition[i] = touchEvent.WorldPosition;
                            this.previousTime[i] = touchEvent.Time;
                        }

                        response = GestureDetectorResponse.UndetectedAndContinue;
                        break;

                    case TouchEventType.Move:
                        if ((touchEvent.Time - this.previousTime[0]).TotalMilliseconds > this.pollingMilliseconds)
                        {
                            this.previousWorldPosition[1] = this.previousWorldPosition[0];
                            this.previousTime[1] = this.previousTime[0];

                            this.previousWorldPosition[0] = touchEvent.WorldPosition;
                            this.previousTime[0] = touchEvent.Time;

                            response = GestureDetectorResponse.UndetectedAndContinue;
                        }
                        break;

                    case TouchEventType.Up:
                        Vector2 distance = touchEvent.WorldPosition - this.previousWorldPosition[1];
                        float elapsedTime = (float)(touchEvent.Time - this.previousTime[1]).TotalMilliseconds;
                        if (previousTime[0] == previousTime[1] || elapsedTime < 16.6)
                        {
                            UIDebug.Assert(elapsedTime > 10);
                            return GestureDetectorResponse.FailedAndStop;
                        }

                        Vector2 speed = distance / elapsedTime * 1000.0f;
                        float speedNorm = speed.Length();

                        switch (this.Direction)
                        {
                            case FlickDirection.Horizontal:
                                if (Math.Abs(speed.X) > Math.Abs(speed.Y) && Math.Abs(speed.X) >= this.MinSpeed)
                                {
                                    response = GestureDetectorResponse.DetectedAndStop;
                                }
                                break;
                            case FlickDirection.Vertical:
                                if (Math.Abs(speed.Y) > Math.Abs(speed.X) && Math.Abs(speed.Y) >= this.MinSpeed)
                                {
                                    response = GestureDetectorResponse.DetectedAndStop;
                                }
                                break;
                            case FlickDirection.All:
                                if (speedNorm >= this.MinSpeed)
                                {
                                    response = GestureDetectorResponse.DetectedAndStop;
                                }
                                break;
                        }

                        if (response == GestureDetectorResponse.DetectedAndStop)
                        {
                            if (this.FlickDetected != null)
                            {
                                if (speedNorm > this.MaxSpeed)
                                {
                                    speed *= this.MaxSpeed / speedNorm;
                                }
                                FlickEventArgs args = new FlickEventArgs(this.TargetWidget, touchEvent.WorldPosition, touchEvent.LocalPosition, speed);
                                this.FlickDetected(this, args);
                            }
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
                Initialize();
            }

            private void Initialize()
            {
                this.downID = 0;
                for (int i = 0; i < 2; i++)
                {
                    this.previousWorldPosition[i] = Vector2.Zero;
                    this.previousTime[i] = TimeSpan.Zero;
                }
            }

            /// @if LANG_JA
            /// <summary>フリックイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Flick event handler</summary>
            /// @endif
            public event EventHandler<FlickEventArgs> FlickDetected;

            private int downID;
            private Vector2[] previousWorldPosition = new Vector2[2];
            private TimeSpan[] previousTime = new TimeSpan[2];
            private readonly long pollingMilliseconds = 60;  // todo: #5606
        }


        /// @if LANG_JA
        /// <summary>フリックイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Flick event argument</summary>
        /// @endif
        public class FlickEventArgs : GestureEventArgs
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="source">イベント発生元ウィジェット</param>
            /// <param name="worldPosition">ワールド（スクリーン）座標系での位置</param>
            /// <param name="localPosition">ローカル座標系での位置</param>
            /// <param name="flickSpeed">フリックの速度</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="source">Event source widget</param>
            /// <param name="worldPosition">Position in the world (screen) coordinate system</param>
            /// <param name="localPosition">Position in the local coordinate system</param>
            /// <param name="flickSpeed">Flick speed</param>
            /// @endif
            public FlickEventArgs(Widget source, Vector2 worldPosition, Vector2 localPosition, Vector2 flickSpeed) : base(source)
            {
                this.WorldPosition = worldPosition;
                this.LocalPosition = localPosition;
                this.Speed = flickSpeed;
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
            /// <summary>フリックの速度を取得する。（ピクセル/秒）</summary>
            /// <remarks>この値は、FlickGestureDetector.Direction の値にかかわらず、水平垂直両方向の速度ベクトルとなります。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the flick speed. (pixels/second)</summary>
            /// <remarks>This value is the speed vector in the horizontal and vertical directions and is not related to the FlickGestureDetector.Direction value.</remarks>
            /// @endif
            public Vector2 Speed
            {
                get;
                private set;
            }
        }


    }
}
