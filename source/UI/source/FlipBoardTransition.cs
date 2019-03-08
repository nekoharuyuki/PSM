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
        /// <summary>FlipBoardTransition用の補間関数の種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of interpolation function for FlipBoardTransition</summary>
        /// @endif
        public enum FlipBoardTransitionInterpolator
        {
            /// @if LANG_JA
            /// <summary>線形補間</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Linear interpolation</summary>
            /// @endif
            Default = 0,

            /// @if LANG_JA
            /// <summary>カスタム</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Custom</summary>
            /// @endif
            Custom,
        }


        /// @if LANG_JA
        /// <summary>真ん中で折れるトランジション</summary>
        /// <remarks>裏に新しいシーンが現れる。空港の掲示板風表現</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Transition that folds in the middle</summary>
        /// <remarks>A new scene appears in the back. Airport bulletin board-like expression</remarks>
        /// @endif
        public class FlipBoardTransition : Transition
        {
            private float time = 720;
            private AnimationInterpolator interpolatorCallback;


            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public FlipBoardTransition()
            {
                DrawOrder = TransitionDrawOrder.TransitionUIElement;
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                if (this.Interpolator == FlipBoardEffectInterpolator.Custom && this.CustomInterpolator != null)
                {
                    this.interpolatorCallback = this.CustomInterpolator;
                }
                else
                {
                    this.interpolatorCallback = AnimationUtility.FlipBounceInterpolator;
                }


                // scene offscreen render image
                ImageAsset currentImage = GetCurrentSceneRenderedImage();
                ImageAsset nextImage = GetNextSceneRenderedImage();


                // divide texture
                float currentDivideWidth = UISystem.FramebufferWidth;
                float currentDivideHeight = UISystem.FramebufferHeight / 2.0f;
                float nextDivideWidth = UISystem.FramebufferWidth;
                float nextDivideHeight = UISystem.FramebufferHeight / 2.0f;

                currentUpperSprt = new UISprite(1);
                currentUpperSprt.X = 0.0f;
                currentUpperSprt.Y = 0.0f;
                currentUpperSprt.Image = currentImage;
                currentUpperSprt.ShaderType = ShaderType.Texture;
                currentUpperSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit currentUpperUnit = currentUpperSprt.GetUnit(0);
                currentUpperUnit.Width = currentDivideWidth;
                currentUpperUnit.Height = currentDivideHeight;
                currentUpperUnit.U1 = 0.0f;
                currentUpperUnit.V1 = 0.0f;
                currentUpperUnit.U2 = 1.0f;
                currentUpperUnit.V2 = 0.5f;

                currentLowerSprt = new UISprite(1);
                currentLowerSprt.X = 0.0f;
                currentLowerSprt.Y = currentDivideHeight;
                currentLowerSprt.Image = currentImage;
                currentLowerSprt.ShaderType = ShaderType.Texture;
                currentLowerSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit currentLowerUnit = currentLowerSprt.GetUnit(0);
                currentLowerUnit.Width = currentDivideWidth;
                currentLowerUnit.Height = currentDivideHeight;
                currentLowerUnit.U1 = 0.0f;
                currentLowerUnit.V1 = 0.5f;
                currentLowerUnit.U2 = 1.0f;
                currentLowerUnit.V2 = 1.0f;

                nextUpperSprt = new UISprite(1);
                nextUpperSprt.X = 0.0f;
                nextUpperSprt.Y = 0.0f;
                nextUpperSprt.Image = nextImage;
                nextUpperSprt.ShaderType = ShaderType.Texture;
                nextUpperSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit nextUpperUnit = nextUpperSprt.GetUnit(0);
                nextUpperUnit.Width = nextDivideWidth;
                nextUpperUnit.Height = nextDivideHeight;
                nextUpperUnit.U1 = 0.0f;
                nextUpperUnit.V1 = 0.0f;
                nextUpperUnit.U2 = 1.0f;
                nextUpperUnit.V2 = 0.5f;

                nextLowerSprt = new UISprite(1);
                nextLowerSprt.X = 0.0f;
                nextLowerSprt.Y = UISystem.FramebufferHeight / 2.0f;
                nextLowerSprt.ShaderType = ShaderType.Texture;
                nextLowerSprt.BlendMode = BlendMode.Premultiplied;
                nextLowerSprt.Image = nextImage;
                UISpriteUnit nextLowerUnit = nextLowerSprt.GetUnit(0);
                nextLowerUnit.Width = nextDivideWidth;
                nextLowerUnit.Height = nextDivideHeight;
                nextLowerUnit.U1 = 0.0f;
                nextLowerUnit.V1 = 0.5f;
                nextLowerUnit.U2 = 1.0f;
                nextLowerUnit.V2 = 1.0f;

                TransitionUIElement.AddChildLast(currentLowerSprt);
                TransitionUIElement.AddChildLast(nextUpperSprt);
                TransitionUIElement.AddChildLast(currentUpperSprt);
                TransitionUIElement.AddChildLast(nextLowerSprt);

                // setup visible
                currentUpperSprt.Visible = true;
                currentLowerSprt.Visible = true;
                nextUpperSprt.Visible = false;
                nextLowerSprt.Visible = false;

                degree = 0.0f;
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// <returns>トランジションの更新の応答</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// <returns>Response of transition update</returns>
            /// @endif
            protected override TransitionUpdateResponse OnUpdate(float elapsedTime)
            {
                degree = this.interpolatorCallback(0, 180, TotalElapsedTime / Time);
                degree = FMath.Clamp(degree, 0.0f, 180.0f);


                if (degree < 90.0)
                {
                    currentUpperSprt.Visible = true;
                    currentLowerSprt.Visible = true;
                    nextUpperSprt.Visible = true;
                    nextLowerSprt.Visible = false;
                    RotateCurrentUpperSprite();
                }
                else
                {
                    currentUpperSprt.Visible = false;
                    currentLowerSprt.Visible = true;
                    nextUpperSprt.Visible = true;
                    nextLowerSprt.Visible = true;
                    RotateNextLowerSprite();
                }


                if (TotalElapsedTime >= time)
                {
                    return TransitionUpdateResponse.Finish;
                }
                else
                {
                    return TransitionUpdateResponse.Continue;
                }
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
                if (currentUpperSprt != null)
                {
                    currentUpperSprt.Image.Dispose();
                    currentUpperSprt.Dispose();
                    currentUpperSprt = null;
                }
                if (currentLowerSprt != null)
                {
                    currentLowerSprt.Image.Dispose();
                    currentLowerSprt.Dispose();
                    currentLowerSprt = null;
                }
                if (nextUpperSprt != null)
                {
                    nextUpperSprt.Image.Dispose();
                    nextUpperSprt.Dispose();
                    nextUpperSprt = null;
                }
                if (nextLowerSprt != null)
                {
                    nextLowerSprt.Image.Dispose();
                    nextLowerSprt.Dispose();
                    nextLowerSprt = null;
                }
            }

            /// @if LANG_JA
            /// <summary>補間関数の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of interpolation function.</summary>
            /// @endif
            public FlipBoardEffectInterpolator Interpolator
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>カスタムの補間関数を設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets a custom interpolation function.</summary>
            /// @endif
            public AnimationInterpolator CustomInterpolator
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>アニメーションの時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the animation time. (ms)</summary>
            /// @endif
            public float Time
            {
                get { return time; }
                set { time = value; }
            }

            private void RotateCurrentUpperSprite()
            {
                float radian = degree / 360.0f * 2.0f * (float)Math.PI;
                UISpriteUnit unit = currentUpperSprt.GetUnit(0);

                Matrix4 mat1;
                Matrix4 trans1 = Matrix4.Translation(new Vector3(0.0f, unit.Height, 0.0f));
                Matrix4 rotate = Matrix4.RotationX(radian);
                trans1.Multiply(ref rotate, out mat1);

                Matrix4 mat2;
                Matrix4 trans2 = Matrix4.Translation(new Vector3(0.0f, -unit.Height, 0.0f));
                mat1.Multiply(ref trans2, out mat2);

                currentUpperSprt.Transform3D = mat2;
            }

            private void RotateNextLowerSprite()
            {
                float radian = (degree - 180.0f) / 360.0f * 2.0f * (float)Math.PI;
                UISpriteUnit unit = nextLowerSprt.GetUnit(0);

                Matrix4 mat;
                Matrix4 trans = Matrix4.Translation(new Vector3(0.0f, unit.Height, 0.0f));
                Matrix4 rotate = Matrix4.RotationX(radian);
                trans.Multiply(ref rotate, out mat);

                nextLowerSprt.Transform3D = mat;
            }

            private UISprite currentUpperSprt;
            private UISprite currentLowerSprt;
            private UISprite nextUpperSprt;
            private UISprite nextLowerSprt;
            private float degree;

        }


    }
}
