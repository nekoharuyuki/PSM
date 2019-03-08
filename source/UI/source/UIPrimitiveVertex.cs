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
        /// <summary>UIPrimitiveが管理する頂点データ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Vertex data managed by UIPrimitive.</summary>
        /// @endif
        public class UIPrimitiveVertex
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
            public UIPrimitiveVertex()
            {
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
            /// <summary>親の座標系での位置を設定する。</summary>
            /// <param name="x">親の座標系でのX座標</param>
            /// <param name="y">親の座標系でのY座標</param>
            /// <param name="z">親の座標系でのZ座標</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets the position in the parent coordinate system.</summary>
            /// <param name="x">X coordinate in the parent coordinate system</param>
            /// <param name="y">Y coordinate in the parent coordinate system</param>
            /// <param name="z">Z coordinate in the parent coordinate system</param>
            /// @endif
            public void SetPosition(float x, float y, float z)
            {
                this.position3D.X = x;
                this.position3D.Y = y;
                this.position3D.Z = z;
                NeedUpdatePosition = true;
            }

            /// @if LANG_JA
            /// <summary>横方向のテクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates in the horizontal direction.</summary>
            /// @endif
            public float U
            {
                get { return u; }
                set
                {
                    u = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float u;

            /// @if LANG_JA
            /// <summary>縦方向のテクスチャ座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the texture coordinates in the vertical direction.</summary>
            /// @endif
            public float V
            {
                get { return v; }
                set
                {
                    v = value;
                    NeedUpdateTexcoord = true;
                }
            }
            float v;

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
