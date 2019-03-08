/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;


namespace Sce.PlayStation.HighLevel.UI
{
    /// @if LANG_JA
    /// <summary>プレス状態</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Press status</summary>
    /// @endif
    public enum PressState : int
    {
        /// @if LANG_JA
        /// <summary>通常</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Normal</summary>
        /// @endif
        Normal = 0,
        /// @if LANG_JA
        /// <summary>プレス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Press</summary>
        /// @endif
        Pressed = 1,
        /// @if LANG_JA
        /// <summary>無効</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Disabled</summary>
        /// @endif
        Disabled = 2,
    }

    /// @if LANG_JA
    /// <summary>プレス状態の変更理由</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Reason for changing press status</summary>
    /// @endif
    public enum PressStateChangedReason
    {
        /// @if LANG_JA
        /// <summary>タッチダウン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch down</summary>
        /// @endif
        TouchDown,
        /// @if LANG_JA
        /// <summary>タッチが領域の内側から外側に移動した</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch moved from the inside of the area to the outside</summary>
        /// @endif
        TouchLeave,
        /// @if LANG_JA
        /// <summary>タッチが領域の外側から内側に移動した</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch moved from the outside of the area to the inside</summary>
        /// @endif
        TouchEnter,
        /// @if LANG_JA
        /// <summary>タッチアップ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch up</summary>
        /// @endif
        TouchUp,
        /// @if LANG_JA
        /// <summary>キーダウン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Key down</summary>
        /// @endif
        KeyDown,
        /// @if LANG_JA
        /// <summary>キーアップ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Key up</summary>
        /// @endif
        KeyUp,
        /// @if LANG_JA
        /// <summary>キャンセルされた、または ResetState が呼ばれた</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Canceled or ResetState called</summary>
        /// @endif
        Cancel,
        /// @if LANG_JA
        /// <summary>PressStateプロパティが変更された</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>PressState property changed</summary>
        /// @endif
        ChangePressStateProperty,
        /// @if LANG_JA
        /// <summary>Enabledプロパティが変更された</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Enabled property changed</summary>
        /// @endif
        ChangeEnabledProperty,
    }

    /// @if LANG_JA
    /// <summary>タッチが領域の内側から外側に移動した場合のプレス状態の挙動</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Behavior of the press status when the touch is moved from the inside of the area to the outside</summary>
    /// @endif
    public enum PressStateTouchLeaveBehavior
    {
        /// @if LANG_JA
        /// <summary>指が範囲外にでるとNormalになるが、指を離さずに範囲外に戻るとまたPress状態に戻る</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the finger goes outside the range, the status becomes Normal, and if the finger returns outside the range without being lifted, it returns to Press status again</summary>
        /// @endif
        ResumeByTouchEnter = 0,
        /// @if LANG_JA
        /// <summary>指が範囲外にドラッグされてもPress状態を維持する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Keeps the Press status even when the finger drags outside the range</summary>
        /// @endif
        KeepPressed,
        /// @if LANG_JA
        /// <summary>指が一度範囲外にでるとNormalになり、再タッチするまでPress状態にはならない</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>When the finger goes outside the range once, the status becomes Normal and does not change to Press status until touched again</summary>
        /// @endif
        End,
    }

    /// @if LANG_JA
    /// <summary>PressStateChangedイベント引数</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>PressStateChanged event argument</summary>
    /// @endif
    public class PressStateChangedEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="reason">変更理由</param>
        /// <param name="newState">変更後のプレス状態</param>
        /// <param name="oldState">変更前のプレス状態</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="reason">Reason for change</param>
        /// <param name="newState">Press status after change</param>
        /// <param name="oldState">Press status before change</param>
        /// @endif
        public PressStateChangedEventArgs(PressStateChangedReason reason, PressState newState, PressState oldState)
        {
            UIDebug.Assert(newState != oldState);

            this.ChangedReason = reason;
            this.NewState = newState;
            this.OldState = oldState;
        }

        /// @if LANG_JA
        /// <summary>変更後のプレス状態</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Press status after change</summary>
        /// @endif
        public PressState NewState { get; private set; }

        /// @if LANG_JA
        /// <summary>変更前のプレス状態</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Press status before change</summary>
        /// @endif
        public PressState OldState { get; private set; }

        /// @if LANG_JA
        /// <summary>変更理由</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Reason for change</summary>
        /// @endif
        public PressStateChangedReason ChangedReason { get; private set; }

        internal bool checkDetectedButtonAction()
        {
            if(ChangedReason == PressStateChangedReason.KeyDown)
                return this.OldState == PressState.Normal && this.NewState == PressState.Pressed; 
            if (ChangedReason == PressStateChangedReason.TouchUp)
                return this.OldState == PressState.Pressed && this.NewState == PressState.Normal; 
            return false;
        }

        internal static int PressStateCount = Enum.GetValues(typeof(PressState)).Length;
    }
}

