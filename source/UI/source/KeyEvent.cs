/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Input;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>キーイベントの種別</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of key event</summary>
        /// @endif
        public enum KeyEventType
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
            /// <summary>長押し</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Long press</summary>
            /// @endif
            LongPress,

            /// @if LANG_JA
            /// <summary>連続押し</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Continuous press</summary>
            /// @endif
            Repeat,

            /// @if LANG_JA
            /// <summary>毎フレームごとのリピート</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Repeat for each frame</summary>
            /// @endif
            EachFrameRepeat,

            /// @if LANG_JA
            /// <summary>キャンセル</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Cancel</summary>
            /// @endif
            Cancel,
        }

        /// @if LANG_JA
        /// <summary>キーの種別</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of key</summary>
        /// @endif
        public enum KeyType : int
        {
            // if you will modify, check KeyEvent.TryConvertFromSingleGamePadButton, KeyEvent.availableGamePadButtons
            // this int value is used as array index.

            /// @if LANG_JA
            /// <summary>None</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>None</summary>
            /// @endif
            None = 0,

            /// @if LANG_JA
            /// <summary>方向キーの左</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Left directional key</summary>
            /// @endif
            Left,


            /// @if LANG_JA
            /// <summary>方向キーの上</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Up directional key</summary>
            /// @endif
            Up,

            /// @if LANG_JA
            /// <summary>方向キーの右</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Right directional key</summary>
            /// @endif
            Right,

            /// @if LANG_JA
            /// <summary>方向キーの下</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Down directional key</summary>
            /// @endif
            Down,

            /// @if LANG_JA
            /// <summary>Enter ボタン</summary>
            /// <remarks>○または×ボタン</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Enter button</summary>
            /// <remarks>Circle or cross button</remarks>
            /// @endif
            Enter,

            /// @if LANG_JA
            /// <summary>Back ボタン</summary>
            /// <remarks>○または×ボタン</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Back button</summary>
            /// <remarks>Circle or cross button</remarks>
            /// @endif
            Back,
            
            /// @if LANG_JA
            /// <summary>□ ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Square button</summary>
            /// @endif
            Square,

            /// @if LANG_JA
            /// <summary>△ ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Triangle button</summary>
            /// @endif
            Triangle,

            /// @if LANG_JA
            /// <summary>STARTボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start button</summary>
            /// @endif
            Start,

            /// @if LANG_JA
            /// <summary>SELECTボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Select button</summary>
            /// @endif
            Select,

            /// @if LANG_JA
            /// <summary>L ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>L button</summary>
            /// @endif
            L,

            /// @if LANG_JA
            /// <summary>R ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>R button</summary>
            /// @endif
            R,
            
            
            /// @internal
            /// @if LANG_JA
            /// <summary>○ ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Circle button</summary>
            /// @endif
            [Obsolete("Use Enter or Back.")]
            Circle = Back,

            
            /// @internal
            /// @if LANG_JA
            /// <summary>× ボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Cross button</summary>
            /// @endif
            [Obsolete("Use Enter or Back.")]
            Cross = Enter,

        }

        /// @if LANG_JA
        /// <summary>キーイベント情報</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Key event information</summary>
        /// @endif
        public class KeyEvent
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public KeyEvent()
            {
                this.KeyEventType = KeyEventType.Up;
                this.KeyType = KeyType.Left;
                this.Time = TimeSpan.Zero;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="keyType">キーの種別</param>
            /// <param name="keyEventType">キーイベントの種別</param>
            /// <param name="time">イベントが発生した時刻</param>
            /// <param name="downTime">キーが押された時刻</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="keyType">Type of key</param>
            /// <param name="keyEventType">Type of key event</param>
            /// <param name="time">Time event occurred</param>
            /// <param name="downTime">Time key was pressed</param>
            /// @endif
            public KeyEvent(KeyType keyType, KeyEventType keyEventType, TimeSpan time, TimeSpan downTime)
            {
                this.KeyEventType = keyEventType;
                this.KeyType = keyType;
                this.Time = time;
                this.DownTime = downTime;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="keyType">キーの種別</param>
            /// <param name="keyEventType">キーイベントの種別</param>
            /// <param name="time">イベントが発生した時刻</param>
            /// <param name="downTime">キーが押された時刻</param>
            /// <param name="enabledEachFrame"></param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="keyType">Type of key</param>
            /// <param name="keyEventType">Type of key event</param>
            /// <param name="time">Time event occurred</param>
            /// <param name="downTime">Time key was pressed</param>
            /// <param name="enabledEachFrame"></param>
            /// @endif
            public KeyEvent(KeyType keyType, KeyEventType keyEventType, TimeSpan time, TimeSpan downTime, bool enabledEachFrame)
                : this(keyType,keyEventType,time,downTime)
            {
                this.EnabledEachFrameRepeat = enabledEachFrame;
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
            /// <summary>イベントをフォワードするかどうかを取得・設定する。初期値はFalse。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to forward the event. Default value is False.</summary>
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


            #region internal utility

            internal bool IsFocusKey
            {
                get
                {
                    var dummy = FourWayDirection.Down;
                    return KeyEvent.TryConvertToDirection(this.KeyType, ref dummy);
                }
            }

            internal static GamePadButtons[] availableGamePadButtons =
            {
                GamePadButtons.Left,     
                GamePadButtons.Up,       
                GamePadButtons.Right,    
                GamePadButtons.Down,     
                GamePadButtons.Enter,    
                GamePadButtons.Back,     
                GamePadButtons.Square,   
                GamePadButtons.Triangle, 
                GamePadButtons.Start,    
                GamePadButtons.Select,   
                GamePadButtons.L,        
                GamePadButtons.R,        
            };
            internal static KeyType[] availableKeyTypes =
            {
                KeyType.Left,
                KeyType.Up,
                KeyType.Right,
                KeyType.Down,
                KeyType.Enter,
                KeyType.Back,
                KeyType.Square,
                KeyType.Triangle,
                KeyType.Start,
                KeyType.Select,
                KeyType.L,
                KeyType.R,
            };

            internal static bool TryConvertToDirection(KeyType keyType, ref FourWayDirection direction)
            {
                switch (keyType)
                {
                    case KeyType.Left:
                        direction = FourWayDirection.Left;
                        return true;
                    case KeyType.Up:
                        direction = FourWayDirection.Up;
                        return true;
                    case KeyType.Right:
                        direction = FourWayDirection.Right;
                        return true;
                    case KeyType.Down:
                        direction = FourWayDirection.Down;
                        return true;
                    default:
                        return false;
                }
            }
            #endregion
        }


    }
}
