/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>色を扱う構造体</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Structure for handling color</summary>
        /// @endif
        public struct UIColor
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="r">R値(0.0f-1.0f)</param>
            /// <param name="g">G値(0.0f-1.0f)</param>
            /// <param name="b">B値(0.0f-1.0f)</param>
            /// <param name="a">アルファ値(0.0f-1.0f)</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="r">R value (0.0f-1.0f)</param>
            /// <param name="g">G value (0.0f-1.0f)</param>
            /// <param name="b">B value (0.0f-1.0f)</param>
            /// <param name="a">Alpha value (0.0f-1.0f)</param>
            /// @endif
            public UIColor( float r, float g, float b, float a ) 
            {
                R = r ;
                G = g ;
                B = b ;
                A = a ;
            }

            /// @if LANG_JA
            /// <summary>UIColorからVector4への明示的型変換</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Explicit conversion from UIColor to Vector4</summary>
            /// @endif
            public static explicit operator Sce.PlayStation.Core.Vector4(UIColor color)
            {
                return new Sce.PlayStation.Core.Vector4(color.R, color.G, color.B, color.A);
            }

            /// @if LANG_JA
            /// <summary>Vector4からUIColorへの明示的型変換</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Explicit conversion from Vector4 to UIColor</summary>
            /// @endif
            public static explicit operator UIColor(Sce.PlayStation.Core.Vector4 vec)
            {
                return new UIColor(vec.X, vec.Y, vec.Z, vec.W);
            }

            /// @if LANG_JA
            /// <summary>比較演算子==</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Comparative operator==</summary>
            /// @endif
            public static bool operator ==(UIColor color1, UIColor color2)
            {
                return (color1.R == color2.R) && (color1.G == color2.G) && (color1.B == color2.B) && (color1.A == color2.A);
            }

            /// @if LANG_JA
            /// <summary>比較演算子!=</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Comparative operator!=</summary>
            /// @endif
            public static bool operator !=(UIColor color1, UIColor color2)
            {
                return !(color1 == color2);
            }

            /// @if LANG_JA
            /// <summary>対象と自分自身が等価かどうか</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Whether the target and self are equivalent</summary>
            /// @endif
            public override bool Equals(object obj)
            {
                if (!(obj is UIColor))
                {
                    return false;
                }

                return this.Equals((UIColor)obj);
            }

            /// @if LANG_JA
            /// <summary>対象と自分自身が等価かどうか</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Whether the target and self are equivalent</summary>
            /// @endif
            public bool Equals(UIColor color)
            {
                return (this.R == color.R) && (this.G == color.G) && (this.B == color.B) && (this.A == color.A);
            }

            /// @if LANG_JA
            /// <summary>ハッシュコードを返す</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns a hash code.</summary>
            /// @endif
            public override int GetHashCode()
            {
                return this.A.GetHashCode()^this.R.GetHashCode()^this.G.GetHashCode()^this.B.GetHashCode();
            }

            /// @if LANG_JA
            /// <summary>R値</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>R value</summary>
            /// @endif
            public float R;
            /// @if LANG_JA
            /// <summary>G値</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>G value</summary>
            /// @endif
            public float G;
            /// @if LANG_JA
            /// <summary>B値</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>B value</summary>
            /// @endif
            public float B;
            /// @if LANG_JA
            /// <summary>アルファ値</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Alpha value</summary>
            /// @endif
            public float A;

            /// @if LANG_JA
            /// <summary>文字列を返す</summary>
            /// <returns>"R=0.0000 G=0.0000 B=0.0000 A=0.0000" 形式の文字列</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns a character string</summary>
            /// <returns>Characters string of "R=0.0000 G=0.0000 B=0.0000 A=0.0000" format</returns>
            /// @endif
            public override string ToString()
            {
                return string.Format("R={0:F4} G={1:F4} B={2:F4} A={3:F4}", R, G, B, A);
            }
        }
    }
}

