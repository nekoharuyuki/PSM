/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>プログレスバーの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of progress bar</summary>
        /// @endif
        public enum ProgressBarStyle
        {
            /// @if LANG_JA
            /// <summary>標準</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Standard</summary>
            /// @endif
            Normal = 0,

            /// @if LANG_JA
            /// <summary>アニメーションつき</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>With animation</summary>
            /// @endif
            Animation
        }


        /// @if LANG_JA
        /// <summary>進捗の度合いを表示するウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A widget used to display the progress level</summary>
        /// @endif
        public class ProgressBar : Widget
        {

            private const float defaultProgressBarWidth = 362.0f;
            private const float defaultProgressBarHeight = 16.0f;

            private const float acceleratorOffset = 3.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ProgressBar()
            {
                base.Width = defaultProgressBarWidth;
                base.Height = defaultProgressBarHeight;

                this.Focusable = false;

                this.progress = 0.0f;
                this.style = ProgressBarStyle.Normal;
                this.animationElapsedTime = 0.0f;

                this.baseImage = new ImageBox();
                this.baseImage.Image = new ImageAsset(SystemImageAsset.ProgressBarBase);
                this.baseImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ProgressBarBase);
                this.baseImage.ImageScaleType = ImageScaleType.NinePatch;
                this.baseImage.Width = defaultProgressBarWidth;
                this.baseImage.Height = defaultProgressBarHeight;
                this.AddChildLast(this.baseImage);

                this.barImage = new ImageBox();
                this.barImage.Image = new ImageAsset(SystemImageAsset.ProgressBarNormal);
                this.barImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.ProgressBarNormal);
                this.barImage.ImageScaleType = ImageScaleType.NinePatch;
                this.barImage.Width = 0.0f;
                this.barImage.Height = defaultProgressBarHeight;
                this.AddChildLast(this.barImage);

                this.acceleratorWidget = new Widget();
                this.acceleratorWidget.Width = defaultProgressBarWidth;
                this.acceleratorWidget.Height = defaultProgressBarHeight;
                this.AddChildLast(acceleratorWidget);

                this.acceleratorSprt = new UISprite(1);
                this.acceleratorSprt.X = acceleratorOffset;
                this.acceleratorSprt.Y = acceleratorOffset;
                this.acceleratorSprt.ShaderType = ShaderType.Texture;
                this.acceleratorSprt.Image = new ImageAsset(SystemImageAsset.ProgressBarAccelerator);
                this.acceleratorSprt.Visible = false;
                this.acceleratorSprt.TextureWrapMode = TextureWrapMode.Repeat;
                this.acceleratorSprt.BlendMode = BlendMode.Add;
                this.acceleratorSprt.Alpha = 0.5f;
                this.acceleratorWidget.RootUIElement.AddChildLast(this.acceleratorSprt);

                UISpriteUnit unit = this.acceleratorSprt.GetUnit(0);
                unit.Width = 0.0f;
                unit.Height = this.acceleratorSprt.Image.Height;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (this.baseImage != null)
                {
                    this.baseImage.Image.Dispose();
                }

                if (this.barImage != null)
                {
                    this.barImage.Image.Dispose();
                }

                if (this.acceleratorSprt != null)
                {
                    this.acceleratorSprt.Image.Dispose();
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

                    if (this.baseImage != null)
                    {
                        this.baseImage.Width = value;
                    }

                    if (this.barImage != null)
                    {
                        this.barImage.Width = this.Width * this.progress;
                    }

                    if(acceleratorSprt != null)
                    {
                        this.acceleratorWidget.Width = this.barImage.Width;
                        UISpriteUnit unit = this.acceleratorSprt.GetUnit(0);
                        unit.Width = this.barImage.Width - acceleratorOffset * 2.0f;
                    }
                }
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
                set
                {
                }
            }

            /// @if LANG_JA
            /// <summary>進捗の度合いを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the level of progress.</summary>
            /// @endif
            public float Progress
            {
                get
                {
                    return this.progress;
                }
                set
                {
                    if (this.progress != value)
                    {
                        this.progress = FMath.Clamp(value, 0.0f, 1.0f);

                        this.barImage.Width = this.Width * this.progress;

                        UISpriteUnit unit = this.acceleratorSprt.GetUnit(0);
                        unit.Width = this.barImage.Width - acceleratorOffset * 2.0f;
                        unit.U1 = 0.0f;
                        unit.U2 = this.barImage.Width / this.acceleratorSprt.Image.Width;
                    }
                }
            }
            private float progress;

            /// @if LANG_JA
            /// <summary>プログレスバーの種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of progress bar.</summary>
            /// @endif
            public ProgressBarStyle Style
            {
                get
                {
                    return this.style;
                }
                set
                {
                    if (this.style != value)
                    {
                        this.style = value;

                        if (this.Style == ProgressBarStyle.Animation)
                        {
                            this.animation = true;
                            this.acceleratorSprt.Visible = true;
                        }
                        else
                        {
                            this.animation = false;
                            this.acceleratorSprt.Visible = false;
                        }
                    }
                }
            }
            private ProgressBarStyle style;

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// @endif
            protected override void OnUpdate(float elapsedTime)
            {
                base.OnUpdate(elapsedTime);

                if (animation)
                {
                    animationElapsedTime += elapsedTime;
                    if (animationElapsedTime > animationTime)
                    {
                        animationElapsedTime -= animationTime;
                    }

                    float addU = animationElapsedTime / animationTime;

                    UISpriteUnit unit = this.acceleratorSprt.GetUnit(0);
                    unit.U1 = addU;
                    unit.U2 = this.barImage.Width / acceleratorScaledImageWidth + addU;
                }
            }


            private ImageBox baseImage;
            private ImageBox barImage;
            private Widget acceleratorWidget;
            private UISprite acceleratorSprt;

            private float animationElapsedTime;
            private const float animationTime = 500.0f;

            private const float acceleratorScaledImageWidth = 45.0f;

            private bool animation;
        }


    }
}
