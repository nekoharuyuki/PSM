/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function</summary>
        /// @endif
        public enum CrossFadeTransitionInterpolator
        {
            /// @if LANG_JA
            /// <summary>線形補間</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Linear interpolation</summary>
            /// @endif
            Linear = 0,

            /// @if LANG_JA
            /// <summary>イージング（二次）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Easing (2D)</summary>
            /// @endif
            EaseOutQuad,

            /// @if LANG_JA
            /// <summary>カスタム</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Custom</summary>
            /// @endif
            Custom,
        }


        /// @if LANG_JA
        /// <summary>クロスフェードトランジション</summary>
        /// <remarks>古いシーンの上で、新しいシーンが透明から不透明へと変化する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Cross-fade transition</summary>
        /// <remarks>On the old scene, a new scene changes from transparent to opaque.</remarks>
        /// @endif
        public class CrossFadeTransition : Transition
        {
            const float defaultTime = 300.0f;
            private UISprite currentSprt;
            private UISprite nextSprt;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public CrossFadeTransition()
            {
                Time = defaultTime;
                Interpolator = CrossFadeTransitionInterpolator.EaseOutQuad;
                DrawOrder = TransitionDrawOrder.CS_NS;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="time">Duration (ms)</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public CrossFadeTransition(float time, CrossFadeTransitionInterpolator interpolator)
            {
                Time = time;
                Interpolator = interpolator;
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                // set draw order
                if (texturize)
                {
                    DrawOrder = TransitionDrawOrder.TransitionUIElement;
                }
                else if (nextSceneForward)
                {
                    DrawOrder = TransitionDrawOrder.CS_NS;
                }
                else
                {
                    DrawOrder = TransitionDrawOrder.NS_CS;
                }

                switch (this.Interpolator)
                {
                    case CrossFadeTransitionInterpolator.EaseOutQuad:
                        if (nextSceneForward)
                        {
                            this.nextInterpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                            this.currentInterpolatorCallback = constInterpolator;
                        }
                        else
                        {
                            this.nextInterpolatorCallback = constInterpolator;
                            this.currentInterpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                        }
                        break;
                    case CrossFadeTransitionInterpolator.Custom:
                        if (this.customNextSceneInterpolator != null)
                        {
                            this.nextInterpolatorCallback = this.customNextSceneInterpolator;
                        }
                        else
                        {
                            this.nextInterpolatorCallback = constInterpolator;
                        }
                        if (this.customCurrentSceneInterpolator != null)
                        {
                            this.currentInterpolatorCallback = this.customCurrentSceneInterpolator;
                        }
                        else
                        {
                            this.currentInterpolatorCallback = constInterpolator;
                        }
                        break;
                    case CrossFadeTransitionInterpolator.Linear:
                    default:
                        if (nextSceneForward)
                        {
                            this.nextInterpolatorCallback = AnimationUtility.LinearInterpolator;
                            this.currentInterpolatorCallback = constInterpolator;
                        }
                        else
                        {
                            this.nextInterpolatorCallback = constInterpolator;
                            this.currentInterpolatorCallback = AnimationUtility.LinearInterpolator;
                        }
                        break;
                }

                if (texturize)
                {
                    // current scene
                    ImageAsset currentImage = GetCurrentSceneRenderedImage();

                    if (currentSprt == null)
                    {
                        currentSprt = new UISprite(1);
                        currentSprt.ShaderType = ShaderType.Texture;
                        currentSprt.BlendMode = BlendMode.Premultiplied;
                    }
                    currentSprt.Image = currentImage;

                    UISpriteUnit unit = currentSprt.GetUnit(0);
                    unit.Width = UISystem.FramebufferWidth;
                    unit.Height = UISystem.FramebufferHeight;

                    // next scene
                    ImageAsset nextImage = GetNextSceneRenderedImage();

                    if (nextSprt == null)
                    {
                        nextSprt = new UISprite(1);
                        nextSprt.ShaderType = ShaderType.Texture;
                        nextSprt.BlendMode = BlendMode.Premultiplied;
                    }
                    nextSprt.Image = nextImage;

                    unit = nextSprt.GetUnit(0);
                    unit.Width = UISystem.FramebufferWidth;
                    unit.Height = UISystem.FramebufferHeight;

                    if (nextSceneForward)
                    {
                        TransitionUIElement.AddChildLast(currentSprt);
                        TransitionUIElement.AddChildLast(nextSprt);
                    }
                    else
                    {
                        TransitionUIElement.AddChildLast(nextSprt);
                        TransitionUIElement.AddChildLast(currentSprt);
                    }

                    nextSprt.Alpha = 0f;
                }

                NextScene.RootWidget.Alpha = 0.0f;
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
                if (TotalElapsedTime < Time)
                {
                    float nextAlpha = this.nextInterpolatorCallback(0.0f, 1.0f, TotalElapsedTime / Time);
                    nextAlpha = FMath.Clamp(nextAlpha, 0.0f, 1.0f);
                    float currentAlpha = this.currentInterpolatorCallback(1.0f, 0.0f, TotalElapsedTime / Time);
                    currentAlpha = FMath.Clamp(currentAlpha, 0.0f, 1.0f);

                    if (nextSprt != null && currentSprt != null)
                    {
                        nextSprt.Alpha = nextAlpha;
                        currentSprt.Alpha = currentAlpha;
                    }
                    else
                    {
                        NextScene.RootWidget.Alpha = nextAlpha;
                        UISystem.CurrentScene.RootWidget.Alpha = currentAlpha;
                    }
                    return TransitionUpdateResponse.Continue;
                }
                else
                {
                    return TransitionUpdateResponse.Finish;
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
                if (nextSprt != null)
                {
                    TransitionUIElement.RemoveChild(nextSprt);
                    nextSprt.Image.Dispose();
                    nextSprt.Dispose();
                    nextSprt = null;
                }
                if (currentSprt != null)
                {
                    TransitionUIElement.RemoveChild(currentSprt);
                    currentSprt.Image.Dispose();
                    currentSprt.Dispose();
                    currentSprt = null;
                }

                NextScene.RootWidget.Alpha = 1.0f;
                UISystem.CurrentScene.RootWidget.Alpha = 1.0f;
            }

            /// @if LANG_JA
            /// <summary>持続時間を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the duration. (ms)</summary>
            /// @endif
            public float Time
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>補間関数の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of interpolation function.</summary>
            /// @endif
            public CrossFadeTransitionInterpolator Interpolator
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>NextScene カスタムの補間関数を設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets a NextScene custom interpolation function.</summary>
            /// @endif
            public AnimationInterpolator CustomNextSceneInterpolator
            {
                get { return customNextSceneInterpolator; }
                set { customNextSceneInterpolator = value; }
            }
            private AnimationInterpolator customNextSceneInterpolator;

            /// @if LANG_JA
            /// <summary>CurrentScene のカスタム補間関数を設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets a CurrentScene custom interpolation function.</summary>
            /// @endif
            public AnimationInterpolator CustomCurrentSceneInterpolator
            {
                get { return customCurrentSceneInterpolator; }
                set { customCurrentSceneInterpolator = value; }
            }
            private AnimationInterpolator customCurrentSceneInterpolator;

            /// @if LANG_JA
            /// <summary>NextScene を前面に表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display NextScene at the front.</summary>
            /// @endif
            public bool NextSceneFront
            {
                get { return nextSceneForward; }
                set { nextSceneForward = value; }
            }
            private bool nextSceneForward = false;

            /// @if LANG_JA
            /// <summary>トランジションの際にテクスチャ化をするかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to create a texture during the transition.</summary>
            /// @endif
            internal bool Texturize
            {
                get { return texturize; }
                set { texturize = value; }
            }
            private bool texturize = false;

            float constInterpolator(float from, float to, float ratio)
            {
                return 1.0f;
            }

            private AnimationInterpolator nextInterpolatorCallback;
            private AnimationInterpolator currentInterpolatorCallback;

        }


    }
}
