/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>プリミティブのユーティリティ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Primitive utility</summary>
        /// @endif
        public static class UIPrimitiveUtility
        {

            /// @if LANG_JA
            /// <summary>画像の９パッチ情報をプリミティブに適用する。</summary>
            /// <param name="primitive">プリミティブ</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="offsetX">描画先のX座標のオフセット位置</param>
            /// <param name="offsetY">描画先のY座標のオフセット位置</param>
            /// <param name="ninePatchMargin">９パッチ情報</param>
            /// <exception cref="ArgumentOutOfRangeException">プリミティブの最大頂点数、もしくは最大インデックス数が足りない。</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 9-patch information of an image to a primitive.</summary>
            /// <param name="primitive">Primitive</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="offsetX">Offset position of the X coordinate of the rendering destination</param>
            /// <param name="offsetY">Offset position of the Y coordinate of the rendering destination</param>
            /// <param name="ninePatchMargin">9-patch information</param>
            /// <exception cref="ArgumentOutOfRangeException">The maximum number of vertices of the primitive or the maximum number of indices is insufficient.</exception>
            /// @endif
            public static void SetupNinePatch(UIPrimitive primitive,
                float width, float height, float offsetX, float offsetY, NinePatchMargin ninePatchMargin)
            {
                if (ninePatchMargin.Top == 0 && ninePatchMargin.Bottom == 0)
                {
                    SetupHorizontalThreePatch(primitive, width, height, offsetX, offsetY,ninePatchMargin.Left,ninePatchMargin.Right);
                }
                else if (ninePatchMargin.Left == 0 && ninePatchMargin.Right == 0)
                {
                    SetupVerticalThreePatch(primitive, width, height, offsetX, offsetY, ninePatchMargin.Top, ninePatchMargin.Bottom);
                }
                else
                {
                    SetupNinePatch(primitive, width, height, offsetX, offsetY, new ImageRect(), ninePatchMargin);
                }
            }

            /// @if LANG_JA
            /// <summary>画像の９パッチ情報をプリミティブに適用する。</summary>
            /// <param name="primitive">プリミティブ</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="offsetX">描画先のX座標のオフセット位置</param>
            /// <param name="offsetY">描画先のY座標のオフセット位置</param>
            /// <param name="imageRect">画像の表示領域</param>
            /// <param name="ninePatchMargin">９パッチ情報</param>
            /// <exception cref="ArgumentOutOfRangeException">プリミティブの最大頂点数、もしくは最大インデックス数が足りない。</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 9-patch information of an image to a primitive.</summary>
            /// <param name="primitive">Primitive</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="offsetX">Offset position of the X coordinate of the rendering destination</param>
            /// <param name="offsetY">Offset position of the Y coordinate of the rendering destination</param>
            /// <param name="imageRect">Display area of the image</param>
            /// <param name="ninePatchMargin">9-patch information</param>
            /// <exception cref="ArgumentOutOfRangeException">The maximum number of vertices of the primitive or the maximum number of indices is insufficient.</exception>
            /// @endif
            public static void SetupNinePatch(UIPrimitive primitive,
                float width, float height, float offsetX, float offsetY, ImageRect imageRect, NinePatchMargin ninePatchMargin)
            {
                if (primitive.MaxVertexCount < 16)
                {
                    throw new ArgumentOutOfRangeException("primitive", "UIPrimitive MaxVertexCount is out of range.");
                }
                if (primitive.MaxIndexCount < 28)
                {
                    throw new ArgumentOutOfRangeException("primitive", "UIPrimitive MaxIndexCount is out of range.");
                }

                if (primitive.Image != null)
                {
                    // vertex coord
                    float[] gridPosX = new float[4];
                    gridPosX[0] = 0.0f;
                    gridPosX[1] = ninePatchMargin.Left;
                    gridPosX[2] = width - ninePatchMargin.Right;
                    gridPosX[3] = width;

                    float[] gridPosY = new float[4];
                    gridPosY[0] = 0.0f;
                    gridPosY[1] = ninePatchMargin.Top;
                    gridPosY[2] = height - ninePatchMargin.Bottom;
                    gridPosY[3] = height;

                    if (gridPosX[1] > width) gridPosX[1] = width;
                    if (gridPosX[2] < 0) gridPosX[2] = 0;
                    if (gridPosX[1] > gridPosX[2]) gridPosX[1] = gridPosX[2] = (gridPosX[1] + gridPosX[2]) / 2f;

                    if (gridPosY[1] > height) gridPosY[1] = height;
                    if (gridPosY[2] < 0) gridPosY[2] = 0;
                    if (gridPosY[1] > gridPosY[2]) gridPosY[1] = gridPosY[2] = (gridPosY[1] + gridPosY[2]) / 2f;

                    // texture coord
                    int imgLeft = 0;
                    int imgTop = 0;
                    int tmpImageWidth = primitive.Image.Width;
                    int tmpImageHeight = primitive.Image.Height;
                    int imgRight = tmpImageWidth;
                    int imgBottom = tmpImageHeight;

                    if (imageRect.X < tmpImageWidth && imageRect.Y < tmpImageHeight && imageRect.Width > 0 && imageRect.Height > 0)
                    {
                        if (imageRect.X > 0) imgLeft = imageRect.X;
                        imgRight = imgLeft + imageRect.Width;
                        if (imgRight > tmpImageWidth) imgRight = tmpImageWidth;
                        UIDebug.Assert(imgRight - imgLeft > 0);

                        if (imageRect.Y > 0) imgTop = imageRect.Y;
                        imgBottom = imgTop + imageRect.Height;
                        if (imgBottom > tmpImageHeight) imgBottom = tmpImageHeight;
                        UIDebug.Assert(imgBottom - imgTop > 0);
                    }

                    float imageWidthF = (float)tmpImageWidth;
                    float imageHeightF = (float)tmpImageHeight;
                    
                    float imgMarginLeft = ninePatchMargin.Left < gridPosX[1] - gridPosX[0] ?
                        imgLeft + ninePatchMargin.Left : imgLeft + gridPosX[1] - gridPosX[0] ;
                    float imgMarginRight = ninePatchMargin.Right < gridPosX[3] - gridPosX[2] ?
                        imgRight - ninePatchMargin.Right : imgRight - (gridPosX[3] - gridPosX[2]);

                    float imgMarginTop = ninePatchMargin.Top < gridPosY[1] - gridPosY[0] ?
                        imgTop + ninePatchMargin.Top : imgTop + gridPosY[1] - gridPosY[0] ;
                    float imgMarginBottom = ninePatchMargin.Bottom < gridPosY[3] - gridPosY[2] ?
                        imgBottom - ninePatchMargin.Bottom : imgBottom - (gridPosY[3] - gridPosY[2]);

                    float[] gridU = new float[4];
                    gridU[0] = imgLeft / imageWidthF;
                    gridU[1] = imgMarginLeft / imageWidthF;
                    gridU[2] = imgMarginRight / imageWidthF;
                    gridU[3] = imgRight / imageWidthF;

                    float[] gridV = new float[4];
                    gridV[0] = imgTop / imageHeightF;
                    gridV[1] = imgMarginTop / imageHeightF;
                    gridV[2] = imgMarginBottom / imageHeightF;
                    gridV[3] = imgBottom / imageHeightF;

                    if (gridU[2] < gridU[0]) gridU[2] = gridU[0];
                    if (gridV[2] < gridV[0]) gridV[2] = gridV[0];
                    if (gridU[1] > gridU[3]) gridU[1] = gridU[3];
                    if (gridV[1] > gridV[3]) gridV[1] = gridV[3];

                    // set UIPrimitive and UIPrimitiveVertex
                    primitive.VertexCount = 16;
                    primitive.SetIndices(new ushort[] { 0, 4, 1, 5, 2, 6, 3, 7, 7,
                                                         4, 4, 8, 5, 9, 6, 10, 7, 11, 11,
                                                         8, 8, 12, 9, 13, 10, 14, 11, 15 });
                    primitive.IndexCount = 28;

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            UIPrimitiveVertex vertex = primitive.GetVertex(i * 4 + j);

                            vertex.X = gridPosX[j] + offsetX;
                            vertex.Y = gridPosY[i] + offsetY;
                            vertex.U = gridU[j];
                            vertex.V = gridV[i];
                        }

                    }
                }
            }

            /// @if LANG_JA
            /// <summary>画像の水平方向の３パッチ情報をプリミティブに適用する。</summary>
            /// <param name="primitive">プリミティブ</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="offsetX">描画先のX座標のオフセット位置</param>
            /// <param name="offsetY">描画先のY座標のオフセット位置</param>
            /// <param name="leftMargin">画像の左端のマージン</param>
            /// <param name="rightMargin">画像の右端のマージン</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 3-patch information in the horizontal direction of an image to a primitive.</summary>
            /// <param name="primitive">Primitive</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="offsetX">Offset position of the X coordinate of the rendering destination</param>
            /// <param name="offsetY">Offset position of the Y coordinate of the rendering destination</param>
            /// <param name="leftMargin">Left margin of the image</param>
            /// <param name="rightMargin">Right margin of the image</param>
            /// @endif
            public static void SetupHorizontalThreePatch(UIPrimitive primitive,
                float width, float height, float offsetX, float offsetY, float leftMargin, float rightMargin)
            {
                if (primitive.MaxVertexCount < 8)
                {
                    throw new ArgumentOutOfRangeException("primitive", "UIPrimitive MaxVertexCount is out of range.");
                }

                if (primitive.Image != null)
                {
                    int tempImageWidth = primitive.Image.Width;

                    if (tempImageWidth == 0.0f)
                    {
                        UIDebug.Assert(tempImageWidth != 0.0f);
                        return;
                    }

                    float imageWidth = (float)tempImageWidth;

                    float[] gridPosX = new float[4];
                    gridPosX[0] = 0.0f;
                    gridPosX[1] = leftMargin < width ? leftMargin : width;
                    gridPosX[2] = rightMargin < width ? width - rightMargin : 0.0f;
                    gridPosX[3] = width;

                    if (gridPosX[1] > gridPosX[2]) gridPosX[1] = gridPosX[2] = (gridPosX[1] + gridPosX[2]) / 2f;

                    float[] gridPosY = new float[2];
                    gridPosY[0] = 0.0f;
                    gridPosY[1] = height;

                    float imgMarginLeft = leftMargin < gridPosX[1] ? leftMargin : gridPosX[1];
                    float imgMarginRight = rightMargin < width - gridPosX[2] ? rightMargin : width - gridPosX[2];

                    float[] gridU = new float[4];
                    gridU[0] = 0.0f;
                    gridU[1] = imgMarginLeft / imageWidth;
                    gridU[2] = (imageWidth - imgMarginRight) / imageWidth;
                    gridU[3] = 1.0f;

                    float[] gridV = new float[2];
                    gridV[0] = 0.0f;
                    gridV[1] = 1.0f;

                    if (gridU[2] < 0.0f) gridU[2] = 0.0f;
                    if (gridU[1] > 1.0f) gridU[1] = 1.0f;

                    primitive.VertexCount = 8;
                    primitive.SetIndices(new ushort[] { 0, 4, 1, 5, 2, 6, 3, 7 });
                    primitive.IndexCount = 8;

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            UIPrimitiveVertex vertex = primitive.GetVertex(i * 4 + j);

                            vertex.X = gridPosX[j] + offsetX;
                            vertex.Y = gridPosY[i] + offsetY;
                            vertex.U = gridU[j];
                            vertex.V = gridV[i];
                        }
                    }

                }
            }

            /// @if LANG_JA
            /// <summary>画像の垂直方向の３パッチ情報をプリミティブに適用する。</summary>
            /// <param name="primitive">プリミティブ</param>
            /// <param name="width">描画先の幅</param>
            /// <param name="height">描画先の高さ</param>
            /// <param name="offsetX">描画先のX座標のオフセット位置</param>
            /// <param name="offsetY">描画先のY座標のオフセット位置</param>
            /// <param name="topMargin">画像の上端のマージン</param>
            /// <param name="bottomMargin">画像の下端のマージン</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Applies the 3-patch information in the vertical direction of an image to a primitive.</summary>
            /// <param name="primitive">Primitive</param>
            /// <param name="width">Width of the rendering destination</param>
            /// <param name="height">Height of the rendering destination</param>
            /// <param name="offsetX">Offset position of the X coordinate of the rendering destination</param>
            /// <param name="offsetY">Offset position of the Y coordinate of the rendering destination</param>
            /// <param name="topMargin">Top margin of the image</param>
            /// <param name="bottomMargin">Bottom margin of the image</param>
            /// @endif
            public static void SetupVerticalThreePatch(UIPrimitive primitive,
                float width, float height, float offsetX, float offsetY, float topMargin, float bottomMargin)
            {
                if (primitive.MaxVertexCount < 8)
                {
                    throw new ArgumentOutOfRangeException("primitive", "UIPrimitive MaxVertexCount is out of range.");
                }

                if (primitive.Image != null)
                {
                    int tempImageHeight = primitive.Image.Height;

                    if (tempImageHeight == 0.0f)
                    {
                        UIDebug.Assert(tempImageHeight != 0.0f);
                        return;
                    }

                    float imageHeight = (float)tempImageHeight;

                    float[] gridPosX = new float[2];
                    gridPosX[0] = 0.0f;
                    gridPosX[1] = width;

                    float[] gridPosY = new float[4];
                    gridPosY[0] = 0.0f;
                    gridPosY[1] = topMargin < height ? topMargin : height;
                    gridPosY[2] = bottomMargin < height ? height - bottomMargin : 0.0f;
                    gridPosY[3] = height;

                    if (gridPosY[1] > gridPosY[2]) gridPosY[1] = gridPosY[2] = (gridPosY[1] + gridPosY[2]) / 2f;

                    float[] gridU = new float[2];
                    gridU[0] = 0.0f;
                    gridU[1] = 1.0f;

                    float imgTopMargin = topMargin < gridPosY[1] ? topMargin : gridPosY[1];
                    float imgBottomMargin = bottomMargin < height - gridPosY[2] ? bottomMargin : height - gridPosY[2];

                    float[] gridV = new float[4];
                    gridV[0] = 0.0f;
                    gridV[1] = imgTopMargin / imageHeight;
                    gridV[2] = (imageHeight - imgBottomMargin) / imageHeight;
                    gridV[3] = 1.0f;

                    if (gridV[2] < 0.0f) gridV[2] = 0.0f;
                    if (gridV[1] > 1.0f) gridV[1] = 1.0f;

                    primitive.VertexCount = 8;
                    primitive.SetIndices(new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7 });
                    primitive.IndexCount = 8;

                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 1; j >= 0; j--)
                        {
                            UIPrimitiveVertex vertex = primitive.GetVertex(i * 2 + j);

                            vertex.X = gridPosX[j] + offsetX;
                            vertex.Y = gridPosY[i] + offsetY;
                            vertex.U = gridU[j];
                            vertex.V = gridV[i];
                        }
                    }
                }
            }

        }

    }
}
