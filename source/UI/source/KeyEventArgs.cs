/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>キーイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Key event argument</summary>
        /// @endif
        public class KeyEventArgs : EventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public KeyEventArgs(KeyEvent keyEvent)
            {
                this.Time = keyEvent.Time;
                this.DownTime = keyEvent.DownTime;
                this.KeyType = keyEvent.KeyType;
                this.KeyEventType = keyEvent.KeyEventType;
                this.Handled = keyEvent.Handled;
                this.EnabledEachFrameRepeat = keyEvent.EnabledEachFrameRepeat;
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
                set;
            }

            /// @if LANG_JA
            /// <summary>キーが押された時刻を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the time a key was pressed.</summary>
            /// @endif
            public TimeSpan DownTime
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>キーの種別を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of key.</summary>
            /// @endif
            public KeyType KeyType
            {
                get;
                internal set;
            }

            /// @if LANG_JA
            /// <summary>キーイベントの種別を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of key event.</summary>
            /// @endif
            public KeyEventType KeyEventType
            {
                get;
                internal set;
            }

            /// @if LANG_JA
            /// <summary>キーイベント引数に含まれるキーイベントをフォワードするかどうかを取得・設定する。初期値はFalse。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to forward a key event included in a key event argument. Default value is False.</summary>
            /// @endif
            [Obsolete("use Handled")]
            public bool Forward
            {
                get { return !handled; }
                set { handled = !value; }
            }

            private bool handled = false;

            /// @if LANG_JA
            /// <summary>イベントが処理済みかどうかを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether an event has been processed</summary>
            /// @endif
            public bool Handled
            {
                get { return handled; }
                set { handled = value; }
            }

            /// @if LANG_JA
            /// <summary>毎フレームごとのキーリピートイベントを有効にするかどうかを取得・設定する</summary>
            /// <remarks>この値を true に設定すると、次のフレームから KeyEventType が EachFrameRepeat のイベントが毎フレーム配信されるようになります。
            /// この値は現在の KeyType に対してのみ有効で、キーが押されている間は値が保持されますが、キーが離されると false にリセットされます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to enable a key repeat event for each frame</summary>
            /// <remarks>When this value is set to true, KeyEventType is such that, from the next frame, the EachFrameRepeat event is distributed for each frame.
            /// This value is enabled only for the current KeyType, and the value is retained while the key is pressed, but it is reset to false when the key is released.</remarks>
            /// @endif
            public bool EnabledEachFrameRepeat { get; set; }
        }


    }
}
