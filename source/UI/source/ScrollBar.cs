/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>スクロールバーの方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scroll bar direction</summary>
        /// @endif
        public enum ScrollBarOrientation
        {
            /// @if LANG_JA
            /// <summary>水平方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Horizontal</summary>
            /// @endif
            Horizontal = 0,

            /// @if LANG_JA
            /// <summary>垂直方向</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Vertical</summary>
            /// @endif
            Vertical
        }

        /// @if LANG_JA
        /// <summary>画面のスクロール位置を把握するためのウィジェット</summary>
        /// <remarks>ユーザーが操作することはできない。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Widget for understanding the scroll position on the screen</summary>
        /// <remarks>User cannot operate this.</remarks>
        /// @endif
        public class ScrollBar : Widget
        {

            private float defaultScrollBarHorizontalWidth = 482.0f;
            private float defaultScrollBarHorizontalHeight = 10.0f;

            private float defaultScrollBarVerticalWidth = 10.0f;
            private float defaultScrollBarVerticalHeight = 260.0f;

            private float scrollBarMinWidth = 10.0f;
            private float scrollBarMinHeight = 10.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="orientation">スクロールバーの方向</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="orientation">Scroll bar direction</param>
            /// @endif
            public ScrollBar(ScrollBarOrientation orientation)
            {
                Orientation = orientation;

                Focusable = false;

                baseImage = new ImageBox();
                this.AddChildLast(baseImage);

                barImage = new ImageBox();
                this.AddChildLast(barImage);

                switch (Orientation)
                {
                    case ScrollBarOrientation.Horizontal:
                        base.Width = defaultScrollBarHorizontalWidth;
                        base.Height = defaultScrollBarHorizontalHeight;

                        baseImage.Image = new ImageAsset(SystemImageAsset.ScrollBarHorizontalBackground);
                        baseImage.Width = defaultScrollBarHorizontalWidth;
                        baseImage.Height = defaultScrollBarHorizontalHeight;
                        baseImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ScrollBarHorizontalBackground);
                        baseImage.ImageScaleType = ImageScaleType.NinePatch;

                        barImage.Image = new ImageAsset(SystemImageAsset.ScrollBarHorizontalBar);
                        barImage.Width = defaultScrollBarHorizontalWidth;
                        barImage.Height = defaultScrollBarHorizontalHeight;
                        barImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ScrollBarHorizontalBar);
                        barImage.ImageScaleType = ImageScaleType.NinePatch;
                        break;

                    case ScrollBarOrientation.Vertical:
                        base.Width = defaultScrollBarVerticalWidth;
                        base.Height = defaultScrollBarVerticalHeight;

                        baseImage.Image = new ImageAsset(SystemImageAsset.ScrollBarVerticalBackground);
                        baseImage.Width = defaultScrollBarVerticalWidth;
                        baseImage.Height = defaultScrollBarVerticalHeight;
                        baseImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ScrollBarVerticalBackground);
                        baseImage.ImageScaleType = ImageScaleType.NinePatch;

                        barImage.Image = new ImageAsset(SystemImageAsset.ScrollBarVerticalBar);
                        barImage.Width = defaultScrollBarVerticalWidth;
                        barImage.Height = defaultScrollBarVerticalHeight;
                        barImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ScrollBarVerticalBar);
                        barImage.ImageScaleType = ImageScaleType.NinePatch;
                        break;
                }
            }


            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (baseImage != null)
                {
                    baseImage.Image.Dispose();
                }

                if (barImage != null)
                {
                    barImage.Image.Dispose();
                }

                base.DisposeSelf();
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

                    if (this.Orientation == ScrollBarOrientation.Horizontal)
                    {
                        if (baseImage != null)
                        {
                            baseImage.Width = value;
                            UpdateView();
                        }
                    }
                    else
                    {
                        if (base.Width < scrollBarMinWidth)
                        {
                            base.Width = scrollBarMinWidth;
                        }
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

                    if (this.Orientation == ScrollBarOrientation.Vertical)
                    {
                        if (baseImage != null)
                        {
                            baseImage.Height = value;
                            UpdateView();
                        }
                    }
                    else
                    {
                        if (base.Height < scrollBarMinHeight)
                        {
                            base.Height = scrollBarMinHeight;
                        }
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>全体のサイズを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the entire size.</summary>
            /// @endif
            public float Length
            {
                get
                {
                    return length;
                }
                set
                {
                    length = value;
                    UpdateView();
                }
            }
            private float length;


            /// @if LANG_JA
            /// <summary>スクロールバーの方向を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the scroll bar direction.</summary>
            /// @endif
            public ScrollBarOrientation Orientation
            {
                get;
                private set;
            }


            /// @if LANG_JA
            /// <summary>バーの位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the bar position.</summary>
            /// @endif
            public float BarPosition
            {
                get
                {
                    return barPosition;
                }
                set
                {
                    barPosition = value;
                    UpdateView();
                }
            }
            private float barPosition;


            /// @if LANG_JA
            /// <summary>バーの長さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the bar length.</summary>
            /// @endif
            public float BarLength
            {
                get
                {
                    return barLength;
                }
                set
                {
                    barLength = value;
                    UpdateView();
                }
            }
            private float barLength;


            private void UpdateView()
            {
                if (barImage == null || baseImage == null)
                    return;

                if (Width <= 0.0f || Height <= 0.0f || Length <= 0.0f)
                {
                    barImage.Visible = false;
                    baseImage.Visible = false;
                }
                else
                {
                    barImage.Visible = true;
                    baseImage.Visible = true;

                    float tmpBarLength = FMath.Clamp(barLength, 0.0f, length);
                    
                    barPosition = FMath.Clamp(barPosition, 0.0f, length - tmpBarLength);

                    switch (Orientation)
                    {
                        case ScrollBarOrientation.Horizontal:
                            barImage.Width = Width * (tmpBarLength / length);
                            barImage.X = Width * (barPosition / length);
                            break;

                        case ScrollBarOrientation.Vertical:
                            barImage.Height = Height * (tmpBarLength / Length);
                            barImage.Y = Height * (barPosition / length);
                            break;
                    }
                }
            }


            private ImageBox baseImage;
            private ImageBox barImage;

        }


    }
}
