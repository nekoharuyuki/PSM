/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>コンテナの機能を持つウィジェット</summary>
        /// <remarks>背景色を設定することができる</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Widget which has the container feature</summary>
        /// <remarks>Background color can be set</remarks>
        /// @endif
        public class Panel : ContainerWidget
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Panel()
            {
                sprt = new UISprite(1);
                RootUIElement.AddChildLast(sprt);
                sprt.ShaderType = ShaderType.SolidFill;

                Width = 0.0f;
                Height = 0.0f;

                BackgroundColor = new UIColor(0f, 0f, 0f, 0f);
                Clip = false;
            }


            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;

                    if (sprt != null)
                    {
                        UISpriteUnit unit = sprt.GetUnit(0);
                        unit.Width = value;
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;

                    if (sprt != null)
                    {
                        UISpriteUnit unit = sprt.GetUnit(0);
                        unit.Height = value;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);
                touchEvents.Forward = true;
            }

            /// @if LANG_JA
            /// <summary>背景色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background color.</summary>
            /// @endif
            public virtual UIColor BackgroundColor
            {
                get
                {
                    UISpriteUnit unit = sprt.GetUnit(0);
                    return unit.Color;
                }
                set
                {
                    if (sprt != null)
                    {
                        UISpriteUnit unit = sprt.GetUnit(0);
                        unit.Color = value;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェエットをクリップするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to clip the child widget.</summary>
            /// @endif
            public new bool Clip
            {
                get
                {
                    return base.Clip;
                }
                set
                {
                    base.Clip = value;
                }
            }

            internal UISprite BackgroundUISprite
            {
                get
                {
                    return sprt;
                }
            }

            private UISprite sprt;

        }


    }
}
