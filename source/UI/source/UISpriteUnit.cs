/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>UISpriteが管理する矩形データ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Rectangle data managed by UISprite</summary>
        /// @endif
        public class UISpriteUnit
        {

            internal bool NeedUpdatePosition = true;
            internal bool NeedUpdateTexcoord = true;
            internal bool NeedUpdateColor = true;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public UISpriteUnit()
            {
                this.U2 = 1.0f;
                this.V2 = 1.0f;
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのX座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the X coordinate in the parent coordinate system.</summary>
            /// @endif
            public float X
            {
                get { return this.position3D.X; }
                set
                {
                    this.position3D.X = value;
                    NeedUpdatePosition = true;
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのY座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Y coordinate in the parent coordinate system.</summary>
            /// @endif
            public float Y
            {
                get { return this.position3D.Y; }
                set
                {
                    this.position3D.Y = value;
                    NeedUpdatePosition = true;
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系でのZ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Z coordinate in the parent coordinate system.</summary>
            /// @endif
            public float Z
            {
                get { return this.position3D.Z; }
                set
                {
                    this.position3D.Z = value;
                    NeedUpdatePosition = true;
                }
            }

            /// @if LANG_JA
            /// <summary>親の座標系での位置を設定する。</summary>
            /// <param name="x">親の座標系でのX座標</param>
            /// <param name="y">親の座標系でのY座標</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the position in the parent coordinate system.</summary>
            /// <param name="x">X coordinate in the parent coordinate system</param>
            /// <param name="y">Y coordinate in the parent coordinate system</param>
            /// @endif
            public void SetPosition(float x, float y)
            {
                this.position3D.X = x;
                this.position3D.Y = y;
                NeedUpdatePosition = true;
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// @endif
            public float Width
            {
                get { return width; }
                set
                {
                    width = value;
                    NeedUpdatePosition = true;
                }
            }
            float width;

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// @endif
            public float Height
            {
                get { return height; }
                set
                {
                    height = value;
                    NeedUpdatePosition = true;
                }
            }
            float height;

            /// @if LANG_JA
            /// <summary>サイズを設定する。</summary>
            /// <param name="width">幅</param>
            /// <param name="height">高さ</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the size.</summary>
            /// <param name="width">Width</param>
            /// <param name="height">Height</param>
            /// @endif
            public void SetSize(float width, float height)
            {
                this.width = width;
                this.height = height;
                NeedUpdatePosition = true;
            }

            /// @if LANG_JA
            /// <summary>左位置のテクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates of the left position.</summary>
            /// @endif
            public float U1
            {
                get { return u1; }
                set
                {
                    u1 = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float u1;

            /// @if LANG_JA
            /// <summary>上位置のテクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates of the top position.</summary>
            /// @endif
            public float V1
            {
                get { return v1; }
                set
                {
                    v1 = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float v1;

            /// @if LANG_JA
            /// <summary>右位置のテクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates of the right position.</summary>
            /// @endif
            public float U2
            {
                get { return u2; }
                set
                {
                    u2 = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float u2;


            /// @if LANG_JA
            /// <summary>下位置テクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates of the bottom position.</summary>
            /// @endif
            public float V2
            {
                get { return v2; }
                set
                {
                    v2 = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float v2;

            /// @if LANG_JA
            /// <summary>頂点の色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the vertex.</summary>
            /// @endif
            public UIColor Color
            {
                get { return color; }
                set
                {
                    color = value;
                    NeedUpdateColor = true;
                }
            }
            private UIColor color = new UIColor(1.0f, 1.0f, 1.0f, 1.0f);

            /// @if LANG_JA
            /// <summary>位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the position.</summary>
            /// @endif
            public Vector3 Position3D
            {
                get { return position3D; }
                set
                {
                    position3D = value;
                    NeedUpdatePosition = true;
                }
            }
            Vector3 position3D = new Vector3();
        }


    }
}
