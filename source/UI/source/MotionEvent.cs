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
        /// <summary>モーションイベントクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Motion event class</summary>
        /// @endif
        public class MotionEvent
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public MotionEvent()
            {
                this.Acceleration = Vector3.Zero;
                this.AngularVelocity = Vector3.Zero;
                this.Time = TimeSpan.Zero;
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
                set;
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
                set;
            }

            /// @if LANG_JA
            /// <summary>イベントが発生した時刻を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the time an event occurred.</summary>
            /// @endif
            public TimeSpan Time
            {
                get;
                set;
            }

        }


    }
}
