/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.UI
{

    /// @if LANG_JA
    /// <summary>エレメントツリーのルート</summary>
    /// <remarks>各ウィジェットが１つ持つ。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Root of the element tree</summary>
    /// <remarks>Each widget has one.</remarks>
    /// @endif
    public class RootUIElement : UIElement
    {

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public RootUIElement(Widget parentWidget)
        {
            UIDebug.Assert(parentWidget != null);
            this.parentWidget = parentWidget;
        }

        /// @if LANG_JA
        /// <summary>最終的なアルファ値を計算する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Calculates the final alpha value.</summary>
        /// @endif
        protected internal override void SetupFinalAlpha()
        {
        }

        internal Widget parentWidget;

        internal override void updateLocalToWorld()
        {
            if (NeedUpdateLocalToWorld)
            {
                localToWorld = parentWidget.LocalToWorld;
                NeedUpdateLocalToWorld = false;
            }
        }

    }


}
