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
        /// <summary>タッチイベントの種別</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of touch event</summary>
        /// @endif
        public enum TouchEventType
        {
            /// @if LANG_JA
            /// <summary>離された</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Released</summary>
            /// @endif
            Up = 0,

            /// @if LANG_JA
            /// <summary>押された</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Pressed</summary>
            /// @endif
            Down,

            /// @if LANG_JA
            /// <summary>移動した</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Moved</summary>
            /// @endif
            Move,

            /// @if LANG_JA
            /// <summary>領域の外側から内側に移動した</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Moved from the outside of the area to the inside</summary>
            /// @endif
            Enter,

            /// @if LANG_JA
            /// <summary>領域の内側から外側に移動した</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Moved from the inside of the area to the outside</summary>
            /// @endif
            Leave,

            /// @if LANG_JA
            /// <summary>無効</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Disabled</summary>
            /// @endif
            None
        }

        /// @if LANG_JA
        /// <summary>タッチイベントクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch event class</summary>
        /// @endif
        public class TouchEvent
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TouchEvent()
            {
                this.FingerID = -1;
                this.Time = TimeSpan.Zero;
                this.Type = TouchEventType.None;
                this.WorldPosition = Vector2.Zero;
                this.LocalPosition = Vector2.Zero;
                this.Source = null;
            }

            /// @if LANG_JA
            /// <summary>実際にタッチされたウィジェット。</summary>
            /// <remarks>子ウィジェットのタッチイベントをフックした場合に、どの子ウィジェットに対するタッチイベントだったのかを判定する場合などに用います。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>The widget that is actually touched.</summary>
            /// <remarks>Used for specifying, etc. the child widget in relation to the touch event when a child widget touch event is hooked.</remarks>
            /// @endif
            public Widget Source
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>ワールド（スクリーン）座標系での位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the position in the world (screen) coordinate system.</summary>
            /// @endif
            public Vector2 WorldPosition
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>ローカル座標系での位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the position in the local coordinate system.</summary>
            /// @endif
            public Vector2 LocalPosition
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

            /// @if LANG_JA
            /// <summary>タッチイベントの種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of touch event.</summary>
            /// @endif
            public TouchEventType Type
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>指のIDを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the finger ID.</summary>
            /// @endif
            public int FingerID
            {
                get;
                set;
            }
        }
    }
}
