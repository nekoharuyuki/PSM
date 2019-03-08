/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>スプライトのユーティリティ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sprite utility</summary>
        /// @endif
        public static class UISpriteUtility
        {

            /// @if LANG_JA
            /// <summary>画像の９パッチ情報をスプライトに適用する。</summary>
            /// <param name="sprite">スプライト</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="offsetX">描画先のX座標のオフセット位置</param>
            /// <param name="offsetY">描画先のY座標のオフセット位置</param>
            /// <param name="ninePatchMargin">９パッチ情報</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 9-patch information of an image to a sprite.</summary>
            /// <param name="sprite">Sprite</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="offsetX">Offset position of the X coordinate of the rendering destination</param>
            /// <param name="offsetY">Offset position of the Y coordinate of the rendering destination</param>
            /// <param name="ninePatchMargin">9-patch information</param>
            /// @endif
            public static void SetupNinePatch(UISprite sprite,
                float width, float height, float offsetX, float offsetY, NinePatchMargin ninePatchMargin)
            {
                if (sprite.Image != null)
                {
                    int tempImageWidth = sprite.Image.Width;
                    int tempImageHeight = sprite.Image.Height;

                    if (tempImageWidth == 0.0f || tempImageHeight == 0.0f)
                    {
                        UIDebug.Assert(tempImageWidth != 0.0f);
                        UIDebug.Assert(tempImageHeight != 0.0f);
                        return;
                    }

                    float imageWidth = (float)tempImageWidth;
                    float imageHeight = (float)tempImageHeight;

                    float centerWidth = width - (ninePatchMargin.Left + ninePatchMargin.Right);
                    float centerHeight = height - (ninePatchMargin.Top + ninePatchMargin.Bottom);

                    if (centerWidth < 0.0f)
                    {
                        centerWidth = 0.0f;
                    }

                    if (centerHeight < 0.0f)
                    {
                        centerHeight = 0.0f;
                    }

                    float[] rectPosX = new float[3];
                    rectPosX[0] = 0.0f;
                    rectPosX[1] = ninePatchMargin.Left;
                    rectPosX[2] = ninePatchMargin.Left + centerWidth;

                    float[] rectPosY = new float[3];
                    rectPosY[0] = 0.0f;
                    rectPosY[1] = ninePatchMargin.Top;
                    rectPosY[2] = ninePatchMargin.Top + centerHeight;

                    float[] rectWidth = new float[3];
                    rectWidth[0] = ninePatchMargin.Left;
                    rectWidth[1] = centerWidth;
                    rectWidth[2] = ninePatchMargin.Right;

                    float[] rectHeight = new float[3];
                    rectHeight[0] = ninePatchMargin.Top;
                    rectHeight[1] = centerHeight;
                    rectHeight[2] = ninePatchMargin.Bottom;

                    float[] rectImgOfsX = new float[3];
                    rectImgOfsX[0] = 0.0f;
                    rectImgOfsX[1] = ninePatchMargin.Left;
                    rectImgOfsX[2] = imageWidth - ninePatchMargin.Right;

                    float[] rectImgOfsY = new float[3];
                    rectImgOfsY[0] = 0.0f;
                    rectImgOfsY[1] = ninePatchMargin.Top;
                    rectImgOfsY[2] = imageHeight - ninePatchMargin.Bottom;

                    float[] rectImgW = new float[3];
                    rectImgW[0] = rectWidth[0];
                    rectImgW[1] = imageWidth - (ninePatchMargin.Left + ninePatchMargin.Right);
                    rectImgW[2] = rectWidth[2];

                    float[] rectImgH = new float[3];
                    rectImgH[0] = rectHeight[0];
                    rectImgH[1] = imageHeight - (ninePatchMargin.Top + ninePatchMargin.Bottom);
                    rectImgH[2] = rectHeight[2];

                    if (rectImgOfsX[2] < 0.0f)
                    {
                        rectImgOfsX[2] = 0.0f;
                    }

                    if (rectImgOfsY[2] < 0.0f)
                    {
                        rectImgOfsY[2] = 0.0f;
                    }

                    if (rectImgW[1] < 0.0f)
                    {
                        rectImgW[1] = 0.0f;
                    }

                    if (rectImgH[1] < 0.0f)
                    {
                        rectImgH[1] = 0.0f;
                    }

                    if (rectPosX[0] + rectWidth[0] > width)
                    {
                        if (rectPosX[0] > width)
                        {
                            rectWidth[0] = 0.0f;
                        }
                        else
                        {
                            rectWidth[0] = rectImgW[0] = width - rectPosX[0];
                        }
                    }

                    if (rectPosY[0] + rectHeight[0] > height)
                    {
                        if (rectPosY[0] > height)
                        {
                            rectHeight[0] = 0.0f;
                        }
                        else
                        {
                            rectHeight[0] = rectImgH[0] = height - rectPosY[0];
                        }
                    }

                    if (rectPosX[2] + rectWidth[2] > width)
                    {
                        if (rectPosX[2] > width)
                        {
                            rectWidth[2] = 0.0f;
                        }
                        else
                        {
                            rectWidth[2] = rectImgW[2] = width - rectPosX[2];
                        }
                    }

                    if (rectPosY[2] + rectHeight[2] > height)
                    {
                        if (rectPosY[2] > height)
                        {
                            rectHeight[2] = 0.0f;
                        }
                        else
                        {
                            rectHeight[2] = rectImgH[2] = height - rectPosY[2];
                        }
                    }

                    float[] rectLU = new float[3];
                    rectLU[0] = rectImgOfsX[0] / imageWidth;
                    rectLU[1] = rectImgOfsX[1] / imageWidth;
                    rectLU[2] = rectImgOfsX[2] / imageWidth;

                    float[] rectTV = new float[3];
                    rectTV[0] = rectImgOfsY[0] / imageHeight;
                    rectTV[1] = rectImgOfsY[1] / imageHeight;
                    rectTV[2] = rectImgOfsY[2] / imageHeight;

                    float[] rectRU = new float[3];
                    rectRU[0] = (rectImgOfsX[0] + rectImgW[0]) / imageWidth;
                    rectRU[1] = (rectImgOfsX[1] + rectImgW[1]) / imageWidth;
                    rectRU[2] = (rectImgOfsX[2] + rectImgW[2]) / imageWidth;

                    float[] rectBV = new float[3];
                    rectBV[0] = (rectImgOfsY[0] + rectImgH[0]) / imageHeight;
                    rectBV[1] = (rectImgOfsY[1] + rectImgH[1]) / imageHeight;
                    rectBV[2] = (rectImgOfsY[2] + rectImgH[2]) / imageHeight;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int index = i * 3 + j;

                            UISpriteUnit unit = sprite.GetUnit(index);
                            unit.X = rectPosX[j] + offsetX;
                            unit.Y = rectPosY[i] + offsetY;
                            unit.Width = rectWidth[j];
                            unit.Height = rectHeight[i];
                            unit.U1 = rectLU[j];
                            unit.V1 = rectTV[i];
                            unit.U2 = rectRU[j];
                            unit.V2 = rectBV[i];
                        }
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>画像の水平方向の３パッチ情報をスプライトに適用する。</summary>
            /// <param name="sprite">スプライト</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="leftMargin">画像の左端のマージン</param>
            /// <param name="rightMargin">画像の右端のマージン</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 3-patch information of a horizontal image to a sprite.</summary>
            /// <param name="sprite">Sprite</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="leftMargin">Left margin of the image</param>
            /// <param name="rightMargin">Right margin of the image</param>
            /// @endif
            public static void SetupHorizontalThreePatch(
                UISprite sprite, float width, float height, float leftMargin, float rightMargin)
            {
                if (sprite.Image != null)
                {
                    int tempImageWidth = sprite.Image.Width;
                    int tempImageHeight = sprite.Image.Height;

                    if (tempImageWidth == 0.0f || tempImageHeight == 0.0f)
                    {
                        return;
                    }

                    float imageWidth = sprite.Image.Width;
                    //float imageHeight = this.Image.Height;

                    float centerWidth = width - (leftMargin + rightMargin);
                    float centerHeight = height;

                    if (centerWidth < 0.0f)
                    {
                        centerWidth = 0.0f;
                    }

                    if (centerHeight < 0.0f)
                    {
                        centerHeight = 0.0f;
                    }

                    float[] rectPosX = new float[3];
                    rectPosX[0] = 0.0f;
                    rectPosX[1] = leftMargin;
                    rectPosX[2] = leftMargin + centerWidth;

                    float[] rectWidth = new float[3];
                    rectWidth[0] = leftMargin;
                    rectWidth[1] = centerWidth;
                    rectWidth[2] = rightMargin;

                    float[] rectImgOfsX = new float[3];
                    rectImgOfsX[0] = 0.0f;
                    rectImgOfsX[1] = leftMargin;
                    rectImgOfsX[2] = imageWidth - rightMargin;

                    float[] rectImgW = new float[3];
                    rectImgW[0] = rectWidth[0];
                    rectImgW[1] = imageWidth - (leftMargin + rightMargin);
                    rectImgW[2] = rectWidth[2];

                    if (rectImgOfsX[2] < 0.0f)
                    {
                        rectImgOfsX[2] = 0.0f;
                    }

                    if (rectImgW[1] < 0.0f)
                    {
                        rectImgW[1] = 0.0f;
                    }

                    if (rectPosX[0] + rectWidth[0] > width)
                    {
                        if (rectPosX[0] > width)
                        {
                            rectWidth[0] = 0.0f;
                        }
                        else
                        {
                            rectWidth[0] = rectImgW[0] = width - rectPosX[0];
                        }
                    }

                    if (rectPosX[2] + rectWidth[2] > width)
                    {
                        if (rectPosX[2] > width)
                        {
                            rectWidth[2] = 0.0f;
                        }
                        else
                        {
                            rectWidth[2] = rectImgW[2] = width - rectPosX[2];
                        }
                    }

                    float[] rectLU = new float[3];
                    rectLU[0] = rectImgOfsX[0] / imageWidth;
                    rectLU[1] = rectImgOfsX[1] / imageWidth;
                    rectLU[2] = rectImgOfsX[2] / imageWidth;

                    float[] rectRU = new float[3];
                    rectRU[0] = (rectImgOfsX[0] + rectImgW[0]) / imageWidth;
                    rectRU[1] = (rectImgOfsX[1] + rectImgW[1]) / imageWidth;
                    rectRU[2] = (rectImgOfsX[2] + rectImgW[2]) / imageWidth;

                    for (int i = 0; i < 3; i++)
                    {
                        UISpriteUnit unit = sprite.GetUnit(i);
                        unit.X = rectPosX[i];
                        unit.Y = 0.0f;
                        unit.Width = rectWidth[i];
                        unit.Height = centerHeight;
                        unit.U1 = rectLU[i];
                        unit.V1 = 0.0f;
                        unit.U2 = rectRU[i];
                        unit.V2 = 1.0f;
                    }
                }
            }

        }


    }
}
