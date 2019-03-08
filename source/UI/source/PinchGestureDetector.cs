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
        /// <summary>ピンチ検出器</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Pinch detector</summary>
        /// @endif
        public class PinchGestureDetector : GestureDetector
        {
            const float defaultMinPinchDistanceInch = 0.169f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PinchGestureDetector()
            {
                this.MinPinchDistance = defaultMinPinchDistanceInch * UISystem.Dpi;
                this.PinchDetected = null;
                this.PinchStartDetected = null;
                this.PinchEndDetected = null;
                this.firstDistance = -1.0f;
                this.firstTouchVector = Vector2.Zero;
            }

            /// @if LANG_JA
            /// <summary>最小ピンチ距離を取得・設定する。</summary>
            /// <remarks>2本downしてから2本の相対距離がMinPinchDirection以上変化したらピンチを検出する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the minimum pinch distance.</summary>
            /// <remarks>Detects the pinch when the relative distance of two becomes equal to or greater than MinPinchDirection after two are moved down.</remarks>
            /// @endif
            public float MinPinchDistance
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
                TouchEventCollection pinchEvents = new TouchEventCollection();

                // Create a TouchEventCollection for Pinch
                foreach (TouchEvent e in touchEvents)
                {
                    if (e.Type == TouchEventType.Enter ||
                          e.Type == TouchEventType.Leave)
                    {
                        return response;
                    }
                    else if (e.Type != TouchEventType.None)
                    {
                        pinchEvents.Add(e);
                    }
                }
                
                if (pinchEvents.Count >= REQUIRED_TOUCH_COUNT)
                {
                    bool endPinch = false;
                    Vector2[] touchLocalPosition = new Vector2[REQUIRED_TOUCH_COUNT];
                    Vector2[] touchWorldPosition = new Vector2[REQUIRED_TOUCH_COUNT];

                    // Get loca between the two touch events
                    for (int i = 0; i < REQUIRED_TOUCH_COUNT; i++)
                    {
                        TouchEvent e = pinchEvents[i];
                        touchLocalPosition[i] = e.LocalPosition;
                        touchWorldPosition[i] = e.WorldPosition;

                        if (e.Type == TouchEventType.Up)
                        {
                            endPinch = true;
                        }
                    }

                    float distance = touchLocalPosition[0].Distance(touchLocalPosition[1]);
                    if (this.firstDistance < 0)
                    {
                        this.firstDistance = distance;
                        this.firstTouchVector = touchLocalPosition[1] - touchLocalPosition[0];
                    }

                    if (State == GestureDetectorResponse.UndetectedAndContinue || 
                            State == GestureDetectorResponse.None)
                    {
                        // Check a condition of starting pinch gesture
                        if (FMath.Abs(distance - this.firstDistance) > MinPinchDistance)
                        {
                            Vector2 localCenter = (touchLocalPosition[0] + touchLocalPosition[1]) / 2.0f;
                            Vector2 worldCenter = (touchWorldPosition[0] + touchWorldPosition[1]) / 2.0f;
                            if (this.PinchStartDetected != null)
                            {
                                this.PinchStartDetected(this, new PinchEventArgs(this.TargetWidget, distance, 1.0f, 0.0f, worldCenter, localCenter));
                            }
                            response = GestureDetectorResponse.DetectedAndContinue;
                        }
                        else
                        {
                            response = GestureDetectorResponse.UndetectedAndContinue;
                        }
                    }
                    else if (State == GestureDetectorResponse.DetectedAndContinue)
                    {
                        // Calculate the event args
                        Vector2 currentTouchVector = touchLocalPosition[1] - touchLocalPosition[0];
                        float scale = distance / this.firstDistance;
                        float angle = this.firstTouchVector.Angle(currentTouchVector);
                        Vector2 localCenter = (touchLocalPosition[0] + touchLocalPosition[1]) / 2.0f;
                        Vector2 worldCenter = (touchWorldPosition[0] + touchWorldPosition[1]) / 2.0f;

                        // Publish event
                        if (endPinch)
                        {
                            if (this.PinchEndDetected != null)
                            {
                                this.PinchEndDetected(this, new PinchEventArgs(this.TargetWidget, distance, scale, angle, worldCenter, localCenter));
                            }
                            response = GestureDetectorResponse.DetectedAndStop;
                            this.firstDistance = -1.0f;
                            this.firstTouchVector = Vector2.Zero;
                        }
                        else
                        {
                            if (this.PinchDetected != null)
                            {
                                this.PinchDetected(this, new PinchEventArgs(this.TargetWidget, distance, scale, angle, worldCenter, localCenter));
                            }
                            response = GestureDetectorResponse.DetectedAndContinue;
                        }
                    }
                }
                else
                {
                    if (State == GestureDetectorResponse.DetectedAndContinue)
                    {
                        response = GestureDetectorResponse.FailedAndStop;
                    }
                    else
                    {
                        response = GestureDetectorResponse.UndetectedAndContinue;
                    }
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
                this.firstDistance = -1.0f;
                this.firstTouchVector = Vector2.Zero;
            }

            /// @if LANG_JA
            /// <summary>ピンチイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Pinch event handler</summary>
            /// @endif
            public event EventHandler<PinchEventArgs> PinchDetected;

            /// @if LANG_JA
            /// <summary>ピンチ開始のイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Pinch start event handler</summary>
            /// @endif
            public event EventHandler<PinchEventArgs> PinchStartDetected;

            /// @if LANG_JA
            /// <summary>ピンチ終了のイベントハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Pinch end event handler</summary>
            /// @endif
            public event EventHandler<PinchEventArgs> PinchEndDetected;

            private const int REQUIRED_TOUCH_COUNT = 2;
            private float firstDistance;
            private Vector2 firstTouchVector;
        }


        /// @if LANG_JA
        /// <summary>ピンチイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Pinch event argument</summary>
        /// @endif
        public class PinchEventArgs : GestureEventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="source">イベント発生元ウィジェット</param>
            /// <param name="distance">ピンチ距離</param>
            /// <param name="scale">ピンチ倍率</param>
            /// <param name="angle">ピンチ角度</param>
            /// <param name="worldCenter">ピンチのワールド（スクリーン）座標系での中心座標</param>
            /// <param name="localCenter">ピンチのローカル座標系での中心座標</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="source">Event source widget</param>
            /// <param name="distance">Pinch distance</param>
            /// <param name="scale">Pinch scale</param>
            /// <param name="angle">Pinch angle</param>
            /// <param name="worldCenter">Center position in the world (screen) coordinate system of a pinch</param>
            /// <param name="localCenter">Center position in the local coordinate system of a pinch</param>
            /// @endif
            public PinchEventArgs(Widget source, float distance, float scale, float angle, Vector2 worldCenter, Vector2 localCenter) : base(source)
            {
                this.Distance = distance;
                this.Scale = scale;
                this.Angle = angle;
                this.WorldCenterPosition = worldCenter;
                this.LocalCenterPosition = localCenter;
            }

            /// @if LANG_JA
            /// <summary>ピンチしている指間の距離を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the distance between the pinching fingers.</summary>
            /// @endif
            public float Distance
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>ピンチ倍率を取得する。</summary>
            /// <remarks>ピンチが開始された時の指間の距離を基準とした値</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the pinch scale.</summary>
            /// <remarks>Value based on the distance between the fingers at the time a pinch starts</remarks>
            /// @endif
            public float Scale
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>ピンチ角度を取得する。</summary>
            /// <remarks>ピンチが開始された時の指の角度を基準とした値</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the pinch angle.</summary>
            /// <remarks>Value based on the angle of the fingers at the time a pinch starts</remarks>
            /// @endif
            public float Angle
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>ワールド（スクリーン）座標系での位置を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the position in the world (screen) coordinate system.</summary>
            /// @endif
            public Vector2 WorldCenterPosition
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>ローカル座標系におけるピンチの中心座標を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the center position of a pinch gesture in the local coordinate system.</summary>
            /// @endif
            public Vector2 LocalCenterPosition
            {
                get;
                private set;
            }
        }


    }
}
