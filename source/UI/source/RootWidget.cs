/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>ウィジェットツリーのルート</summary>
    /// <remarks>各シーンが１つ持つ。コンテナ機能を持つ。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Root of the widget tree</summary>
    /// <remarks>Each scene has one. Has the container feature.</remarks>
    /// @endif
    public class RootWidget : ContainerWidget
    {

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="scene">シーン</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="scene">Scene</param>
        /// @endif
        internal RootWidget(Scene scene)
        {
            this.parentScene = scene;
        }

        internal void SetSizeInternal(float width, float height)
        {
            base.Width = width;
            base.Height = height;
        }

        /// @if LANG_JA
        /// <summary>親の座標系でのX座標を取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the X coordinate in the parent coordinate system.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float X
        {
            get
            {
                return 0.0f;
            }
            set
            {
            }
        }


        /// @if LANG_JA
        /// <summary>親の座標系でのY座標を取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the Y coordinate in the parent coordinate system.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float Y
        {
            get
            {
                return 0.0f;
            }
            set
            {
            }
        }


        /// @if LANG_JA
        /// <summary>幅を取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the width.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float Width
        {
            get
            {
                return base.Width;
            }
            set { }
        }


        /// @if LANG_JA
        /// <summary>高さを取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the height.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override float Height
        {
            get
            {
                return base.Height;
            }
            set { }
        }


        /// @if LANG_JA
        /// <summary>親の座標系への変換行列を取得する。</summary>
        /// <remarks>設定された値は無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the transformation matrix to the parent coordinate system.</summary>
        /// <remarks>The set value is ignored.</remarks>
        /// @endif
        public override Matrix4 Transform3D
        {
            get
            {
                return base.Transform3D;
            }
            set
            {
            }
        }


        /// @if LANG_JA
        /// <summary>通常のシーンのRootWidgetはヒットしないため常にfalseを返す</summary>
        /// <param name="screenPoint">スクリーン座標系での位置</param>
        /// <returns>通常はfalse</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Always returns false since RootWidget of a normal scene does not hit</summary>
        /// <param name="screenPoint">Position in the screen coordinate system</param>
        /// <returns>Always false</returns>
        /// @endif
        public override bool HitTest(Vector2 screenPoint)
        {
            UIDebug.Assert(this.parentScene != null);
            return this.parentScene.ConsumeAllTouchEvent;
        }

        internal Scene parentScene;

    }


}
