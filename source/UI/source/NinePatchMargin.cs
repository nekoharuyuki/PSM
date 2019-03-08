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
        /// <summary>９パッチ情報を格納するための構造体</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Structure for storing 9-patch information</summary>
        /// @endif
        public struct NinePatchMargin
        {

            /// @if LANG_JA
            /// <summary>各メンバの値が0のインスタンス</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Instance when the value of each member is 0</summary>
            /// @endif
            public static readonly NinePatchMargin Zero = new NinePatchMargin();

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="left">左方向のマージン</param>
            /// <param name="top">上方向のマージン</param>
            /// <param name="right">右方向のマージン</param>
            /// <param name="bottom">下方向のマージン</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="left">Left margin</param>
            /// <param name="top">Top margin</param>
            /// <param name="right">Right margin</param>
            /// <param name="bottom">Bottom margin</param>
            /// @endif
            public NinePatchMargin(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            /// @if LANG_JA
            /// <summary>左方向のマージン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Left margin</summary>
            /// @endif
            public int Left;

            /// @if LANG_JA
            /// <summary>上方向のマージン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Top margin</summary>
            /// @endif
            public int Top;

            /// @if LANG_JA
            /// <summary>右方向のマージン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Right margin</summary>
            /// @endif
            public int Right;

            /// @if LANG_JA
            /// <summary>下方向のマージン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Bottom margin</summary>
            /// @endif
            public int Bottom;

        }


    }
}
