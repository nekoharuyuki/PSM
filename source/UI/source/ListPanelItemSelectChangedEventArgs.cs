/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

namespace Sce.PlayStation.HighLevel.UI
{
    /// @if LANG_JA
    /// <summary>リストアイテムが選択された場合に呼び出されるイベント引数</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Event argument called when list item is selected</summary>
    /// @endif
    public class ListPanelItemSelectChangedEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public ListPanelItemSelectChangedEventArgs()
        {
        }
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="item">アイテム</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="item">Item</param>
        /// @endif
        public ListPanelItemSelectChangedEventArgs(ListPanelItem item)
        {
            this.Index = item.Index;
            this.IndexInSection = item.IndexInSection;
            this.SectionIndex = item.SectionIndex;
        }

        /// @if LANG_JA
        /// <summary>選択されたアイテムのインデックス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Index of selected items</summary>
        /// @endif
        public int Index { get; private set; }

        /// @if LANG_JA
        /// <summary>セクションのインデックス</summary>
        /// <remarks>このプロパティはListPanelの時にのみ有効です。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Index of section</summary>
        /// <remarks>This property is enabled only for ListPanel.</remarks>
        /// @endif
        public virtual int IndexInSection { get; private set; }

        /// @if LANG_JA
        /// <summary>セクション内のインデックス</summary>
        /// <remarks>このプロパティはListPanelの時にのみ有効です。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Index within section</summary>
        /// <remarks>This property is enabled only for ListPanel.</remarks>
        /// @endif
        public int SectionIndex { get; private set; }
    }
}

