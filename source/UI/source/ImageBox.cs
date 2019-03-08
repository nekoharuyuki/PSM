/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>画像のスケール</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Image scale</summary>
        /// @endif
        public enum ImageScaleType
        {
            /// @if LANG_JA
            /// <summary>スケールなしで画像を中央に表示する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Displays the image in the center without scaling.</summary>
            /// @endif
            Center = 0,

            /// @if LANG_JA
            /// <summary>アスペクト比を無視して埋める。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ignore the aspect ratio and fill the screen.</summary>
            /// @endif
            Stretch,

            /// @if LANG_JA
            /// <summary>アスペクト比固定で内側に接する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scale with a fixed aspect ratio until the interior edge.</summary>
            /// @endif
            AspectInside,

            /// @if LANG_JA
            /// <summary>アスペクト固定で外側に接する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scale with a fixed aspect ratio until the exterior edge.</summary>
            /// @endif
            AspectOutside,

            /// @if LANG_JA
            /// <summary>９パッチでスケーリングする。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Performs scaling using 9-patch.</summary>
            /// @endif
            NinePatch
        }


        /// @if LANG_JA
        /// <summary>画像を表示するウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A widget that displays an image</summary>
        /// @endif
        public class ImageBox : Widget
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ImageBox()
            {
                this.sprt = new UISprite(1);
                this.RootUIElement.AddChildLast(sprt);
                this.sprt.ShaderType = ShaderType.Texture;

                this.ninePatchMargin = NinePatchMargin.Zero;
                this.scale = ImageScaleType.Stretch;
                this.cropArea = new ImageRect(0, 0, 0, 0);
                this.clipImageX = 0.0f;
                this.clipImageY = 0.0f;
                this.clipImageWidth = 0.0f;
                this.clipImageHeight = 0.0f;

                Focusable = false;

                this.setupSpriteImageScale = new Dictionary<ImageScaleType, SetupUISpriteImageScale>
                {
                    {ImageScaleType.Stretch,         SetupUISpriteNormal},
                    {ImageScaleType.AspectInside,   SetupUISpriteAspectInside},
                    {ImageScaleType.AspectOutside,  SetupUISpriteAspectOutside},
                    {ImageScaleType.NinePatch,      SetupUIPrimitiveNinePatch},
                    {ImageScaleType.Center,           SetupUISpriteNone}
                };
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
                    needUpdateSprite = true;
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
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the image.</summary>
            /// @endif
            public ImageAsset Image
            {
                get
                {
                    return this.sprt.Image;
                }
                set
                {
                    this.sprt.Image = value;
                    if(this.ninePatchPrim != null)
                        this.ninePatchPrim.Image = value;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>画像の９パッチ情報を取得・設定する。</summary>
            /// <remarks>ImageScaleTypeがNinePatchの場合にのみ有効となる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the 9-patch information of the image.</summary>
            /// <remarks>Enabled only when ImageScaleType is NinePatch.</remarks>
            /// @endif
            public NinePatchMargin NinePatchMargin
            {
                get
                {
                    return ninePatchMargin;
                }
                set
                {
                    ninePatchMargin = value;
                    needUpdateSprite = true;
                }
            }
            private NinePatchMargin ninePatchMargin;

            /// @if LANG_JA
            /// <summary>画像のスケールを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scale of the image.</summary>
            /// @endif
            public ImageScaleType ImageScaleType
            {
                get
                {
                    return this.scale;
                }
                set
                {
                    this.scale = value;
                    needUpdateSprite = true;
                }
            }
            private ImageScaleType scale;

            /// @if LANG_JA
            /// <summary>画像のクリップ領域を取得・設定する。</summary>
            /// <remarks>全て０の場合はクリップしない。ImageScaleTypeと同時に設定されている場合クリップした画像をスケーリングする。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the clip area of the image.</summary>
            /// <remarks>No clipping is performed if everything is 0. The clipped image is scaled when set at the same time as ImageScaleType.</remarks>
            /// @endif
            public ImageRect CropArea
            {
                get
                {
                    return this.cropArea;
                }
                set
                {
                    this.cropArea = value;
                    needUpdateSprite = true;
                }
            }
            private ImageRect cropArea;

            /// @if LANG_JA
            /// <summary>ブレンドモードを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the blend mode.</summary>
            /// @endif
            public BlendMode BlendMode
            {
                get
                {
                    if (sprt != null)
                        return sprt.BlendMode;
                    else
                        return BlendMode.Half;
                }
                set
                {
                    if (sprt != null)
                        sprt.BlendMode = value;
                    if (ninePatchPrim != null)
                        ninePatchPrim.BlendMode = value;
                }
            }


            private void updateSprite()
            {
                if (this.sprt.Image == null)
                {
                    this.sprt.Visible = false;
                    if (this.ninePatchPrim != null)
                        this.ninePatchPrim.Visible = false;
                    needUpdateSprite = false;
                }
                else if (this.sprt.Image.Ready && this.sprt.Image.Width > 0 && this.sprt.Image.Height > 0)
                {
                    if (this.CropArea.X != 0 || this.CropArea.Y != 0 ||
                        this.CropArea.Width != 0 || this.CropArea.Height != 0)
                    {
                        this.clipImageX = this.CropArea.X;
                        this.clipImageY = this.CropArea.Y;
                        this.clipImageWidth = this.CropArea.Width;
                        this.clipImageHeight = this.CropArea.Height;
                    }
                    else
                    {
                        this.clipImageX = 0.0f;
                        this.clipImageY = 0.0f;
                        this.clipImageWidth = this.sprt.Image.Width;
                        this.clipImageHeight = this.sprt.Image.Height;
                    }

                    this.setupSpriteImageScale[this.scale]();

                    needUpdateSprite = false;
                }
            }

            void createNinePatchPrim()
            {
                UIDebug.Assert(this.ninePatchPrim == null);
                UIDebug.Assert(this.sprt != null);

                this.ninePatchPrim = new UIPrimitive(DrawMode.TriangleStrip, 16, 28);
                this.ninePatchPrim.Visible = false;
                this.ninePatchPrim.BlendMode = this.sprt.BlendMode;
                this.ninePatchPrim.ShaderType =  this.sprt.ShaderType;
                this.ninePatchPrim.Image = this.sprt.Image;
                this.RootUIElement.AddChildLast(ninePatchPrim);
            }

            void showSprtAndHide9PatchPrim()
            {
                this.sprt.Visible = true;
                if (this.ninePatchPrim != null)
                {
                    this.ninePatchPrim.Visible = false;
                }
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (needUpdateSprite)
                {
                    updateSprite();
                }
                base.Render();
            }

            private delegate void SetupUISpriteImageScale();
            private Dictionary<ImageScaleType, SetupUISpriteImageScale> setupSpriteImageScale;

            private void SetupUISpriteNormal()
            {
                showSprtAndHide9PatchPrim();

                float imageWidth = this.sprt.Image.Width;
                float imageHeight = this.sprt.Image.Height;

                UISpriteUnit unit = sprt.GetUnit(0);
                unit.X = 0.0f;
                unit.Y = 0.0f;
                unit.Width = this.Width;
                unit.Height = this.Height;
                unit.U1 = this.clipImageX / imageWidth;
                unit.V1 = this.clipImageY / imageHeight;
                unit.U2 = (this.clipImageX + this.clipImageWidth) / imageWidth;
                unit.V2 = (this.clipImageY + this.clipImageHeight) / imageHeight;
            }

            private void SetupUISpriteAspectInside()
            {
                showSprtAndHide9PatchPrim();

                float imageWidth = this.sprt.Image.Width;
                float imageHeight = this.sprt.Image.Height;

                float widthRatio = this.Width / this.clipImageWidth;
                float heightRatio = this.Height / this.clipImageHeight;
                float ratio = (widthRatio < heightRatio) ? widthRatio : heightRatio;

                float scaleClipImageWidth = this.clipImageWidth * ratio;
                float scaleClipImageHeight = this.clipImageHeight * ratio;

                UISpriteUnit unit = sprt.GetUnit(0);
                unit.X = (this.Width - scaleClipImageWidth) / 2.0f;
                unit.Y = (this.Height - scaleClipImageHeight) / 2.0f;
                unit.Width = scaleClipImageWidth;
                unit.Height = scaleClipImageHeight;
                unit.U1 = this.clipImageX / imageWidth;
                unit.V1 = this.clipImageY / imageHeight;
                unit.U2 = (this.clipImageX + this.clipImageWidth) / imageWidth;
                unit.V2 = (this.clipImageY + this.clipImageHeight) / imageHeight;
            }

            private void SetupUISpriteAspectOutside()
            {
                showSprtAndHide9PatchPrim();

                float imageWidth = this.sprt.Image.Width;
                float imageHeight = this.sprt.Image.Height;

                float widthRatio = this.Width / this.clipImageWidth;
                float heightRatio = this.Height / this.clipImageHeight;
                float ratio = (widthRatio > heightRatio) ? widthRatio : heightRatio;

                float scaleImageWidth = imageWidth * ratio;
                float scaleImageHeight = imageHeight * ratio;

                float scaleClipImageX = this.clipImageX * ratio;
                float scaleClipImageY = this.clipImageY * ratio;
                float scaleClipImageWidth = this.clipImageWidth * ratio;
                float scaleClipImageHeight = this.clipImageHeight * ratio;

                float scaleImageOffsetX = scaleClipImageX + ((scaleClipImageWidth - this.Width) / 2.0f);
                float scaleImageOffsetY = scaleClipImageY + ((scaleClipImageHeight - this.Height) / 2.0f);

                UISpriteUnit unit = sprt.GetUnit(0);
                unit.X = 0.0f;
                unit.Y = 0.0f;
                unit.Width = this.Width;
                unit.Height = this.Height;
                unit.U1 = scaleImageOffsetX / scaleImageWidth;
                unit.V1 = scaleImageOffsetY / scaleImageHeight;
                unit.U2 = (scaleImageOffsetX + this.Width) / scaleImageWidth;
                unit.V2 = (scaleImageOffsetY + this.Height) / scaleImageHeight;
            }

            private void SetupUIPrimitiveNinePatch()
            {
                if (this.ninePatchPrim == null)
                    createNinePatchPrim();

                this.sprt.Visible = false;
                this.ninePatchPrim.Visible = true;

                UIPrimitiveUtility.SetupNinePatch(this.ninePatchPrim, this.Width, this.Height, 0f, 0f,this.cropArea, this.ninePatchMargin);
            }

            private void SetupUISpriteNone()
            {
                showSprtAndHide9PatchPrim();

                float imageWidth = this.sprt.Image.Width;
                float imageHeight = this.sprt.Image.Height;

                float clipImageOffsetX = Math.Abs(this.Width - this.clipImageWidth) / 2.0f;
                float clipImageOffsetY = Math.Abs(this.Height - this.clipImageHeight) / 2.0f;

                UISpriteUnit unit = sprt.GetUnit(0);

                if (this.clipImageWidth > this.Width)
                {
                    unit.X = 0.0f;
                    unit.Width = this.Width;
                    unit.U1 = (this.clipImageX + clipImageOffsetX) / imageWidth;
                    unit.U2 = (this.clipImageX + clipImageOffsetX + this.Width) / imageWidth;
                }
                else
                {
                    unit.X = clipImageOffsetX;
                    unit.Width = this.clipImageWidth;
                    unit.U1 = this.clipImageX / imageWidth;
                    unit.U2 = (this.clipImageX + this.clipImageWidth) / imageWidth;
                }

                if (this.clipImageHeight > this.Height)
                {
                    unit.Y = 0.0f;
                    unit.Height = this.Height;
                    unit.V1 = (this.clipImageY + clipImageOffsetY) / imageHeight;
                    unit.V2 = (this.clipImageY + clipImageOffsetY + this.Height) / imageHeight;
                }
                else
                {
                    unit.Y = clipImageOffsetY;
                    unit.Height = this.clipImageHeight;
                    unit.V1 = this.clipImageY / imageHeight;
                    unit.V2 = (this.clipImageY + this.clipImageHeight) / imageHeight;
                }
            }

            private UISprite sprt;
            private UIPrimitive ninePatchPrim;
            private float clipImageX;
            private float clipImageY;
            private float clipImageWidth;
            private float clipImageHeight;
            private bool needUpdateSprite = true;
        }


    }
}
