/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>GestureDetectorのタッチイベント配信応答</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Response to touch event distribution of GestureDetector</summary>
        /// @endif
        public enum GestureDetectorResponse
        {
            /// @if LANG_JA
            /// <summary>初期状態</summary>
            /// <remarks>Systemで設定する値なのでOnTouchEventの戻り値では使用しないこと。OnTouchEventの戻り値で使用した場合はFailedAndStopと同じ扱いとする。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Initial state</summary>
            /// <remarks>This value is set with System, so it is not used with the OnTouchEvent return value. When this is used with the OnTouchEvent return value, it is treated the same as FailedAndStop.</remarks>
            /// @endif
            None = 0,

            /// @if LANG_JA
            /// <summary>ジェスチャ未検出。タッチイベント配信は継続。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Gesture not detected. Distribution of the touch event is continued.</summary>
            /// @endif
            UndetectedAndContinue,

            /// @if LANG_JA
            /// <summary>ジェスチャ検出済。タッチイベント配信は継続。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Gesture already detected. Distribution of the touch event is continued.</summary>
            /// @endif
            DetectedAndContinue,

            /// @if LANG_JA
            /// <summary>ジェスチャ検出済。タッチイベント配信は終了。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Gesture already detected. Distribution of the touch event is ended.</summary>
            /// @endif
            DetectedAndStop,

            /// @if LANG_JA
            /// <summary>ジェスチャ検出失敗。タッチイベント配信は終了。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Gesture detection failed. Distribution of the touch event is ended.</summary>
            /// @endif
            FailedAndStop
        }


        /// @if LANG_JA
        /// <summary>ジェスチャ検出機構の基底クラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Base class of gesture detection mechanism</summary>
        /// @endif
        public abstract class GestureDetector
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public GestureDetector()
            {
                this.State = GestureDetectorResponse.None;
                this.TargetWidget = null;
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
            protected internal abstract GestureDetectorResponse OnTouchEvent(TouchEventCollection touchEvents);

            /// @if LANG_JA
            /// <summary>ジェスチャ検出の状態をリセットする。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Resets the state of gesture detection.</summary>
            /// @endif
            protected internal abstract void OnResetState();

            /// @if LANG_JA
            /// <summary>GestureDetectorの状態を取得する。</summary>
            /// <remarks>前回のOnTouchEventで返却されたGestureDetectorResponseを取得することができる。OnResetStateが呼ばれるとNoneが設定される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the state of GestureDetector.</summary>
            /// <remarks>It is possible to obtain GestureDetectorResponse returned with the previous OnTouchEvent. When OnResetState is called, None is set.</remarks>
            /// @endif
            public GestureDetectorResponse State
            {
                get;
                internal set;
            }

            /// @if LANG_JA
            /// <summary>ジェスチャ検出対象のウィジェットを取得する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the widget applicable for detecting gesture</summary>
            /// @endif
            public Widget TargetWidget
            {
                get
                {
                    return targetWidget;
                }
                internal set
                {
                    UIDebug.Assert(targetWidget == null || value == null, "Target widget can be set only once.");
                    targetWidget = value;
                }
            }
            private Widget targetWidget;
        }

        /// @if LANG_JA
        /// <summary>ジェスチャイベント引数の基底クラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Base class of gesture event argument</summary>
        /// @endif
        public abstract class GestureEventArgs : EventArgs
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public GestureEventArgs(Widget source)
            {
                this.Source = source;
            }

            /// @if LANG_JA
            /// <summary>イベント発生元ウィジェット</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event source widget</summary>
            /// @endif
            public Widget Source
            {
                get;
                private set;
            }
        }
    }
}
