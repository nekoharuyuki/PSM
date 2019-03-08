/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

namespace Sce.PlayStation.HighLevel.UI
{
    /// @if LANG_JA
    /// <summary>FocusChangedイベント引数</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>FocusChanged event argument</summary>
    /// @endif
    public class FocusChangedEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="focused">フォーカスが設定されたかどうか</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="focused">Whether the focus is set</param>
        /// @endif
        public FocusChangedEventArgs(bool focused)
        {
            Focused = focused;
        }

        internal FocusChangedEventArgs(bool focused, Widget newWidget, Widget oldWidget)
            : this(focused)
        {
            this.NewFocusWidget = newWidget;
            this.OldFocusWidget = oldWidget;
        }

        internal Widget OldFocusWidget { get; private set; }
        internal Widget NewFocusWidget { get; private set; }

        /// @if LANG_JA
        /// <summary>フォーカスが設定されたかどうかを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether the focus is set</summary>
        /// @endif
        public bool Focused { get; private set; }
    }
}

