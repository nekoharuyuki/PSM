/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>モーションイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Motion event argument</summary>
        /// @endif
        public class MotionEventArgs : EventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public MotionEventArgs()
            {
                this.Acceleration = Vector3.Zero;
                this.AngularVelocity = Vector3.Zero;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="motionEvent">モーションセンサー情報</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="motionEvent">Motion sensor information</param>
            /// @endif
            public MotionEventArgs(MotionEvent motionEvent)
            {
                this.Time = motionEvent.Time;
                this.Acceleration = motionEvent.Acceleration;
                this.AngularVelocity = motionEvent.AngularVelocity;
            }

            /// @if LANG_JA
            /// <summary>イベントが発生した時刻を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the time an event occurred.</summary>
            /// @endif
            internal TimeSpan Time
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>加速度を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the acceleration.</summary>
            /// @endif
            public Vector3 Acceleration
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>角速度を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the angular velocity.</summary>
            /// @endif
            public Vector3 AngularVelocity
            {
                get;
                private set;
            }

        }


    }
}
