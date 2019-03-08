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
        /// <summary>タッチイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch event argument</summary>
        /// @endif
        public class TouchEventArgs : EventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TouchEventArgs(TouchEventCollection touchEvents)
            {
                this.TouchEvents = touchEvents;
            }

            /// @if LANG_JA
            /// <summary>ウィジェットに配信された全てのタッチイベントを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains all of the touch events distributed to the widget.</summary>
            /// @endif
            public TouchEventCollection TouchEvents
            {
                get;
                private set;
            }

            /// @if LANG_JA
            /// <summary>タッチイベント引数に含まれるタッチイベントをフォワードするかどうかを取得・設定する。初期値はFalse。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to forward a touch event included in a touch event argument. Default value is False.</summary>
            /// @endif
            public bool Forward
            {
                get{ return TouchEvents.Forward; }
                set{ TouchEvents.Forward = value; }
            }

            internal static TouchEventArgs CreateDummy()
            {
                var touchEventSet = new TouchEventCollection();
                var e = new TouchEvent();
                touchEventSet.Add(e);
                touchEventSet.PrimaryTouchEvent = e;
                return new TouchEventArgs(touchEventSet);
            }
        }
    }
}
