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
        public enum PushTransitionInterpolator
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
            /// <summary>オーバーシュート</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Overshoot</summary>
            /// @endif
            Overshoot,

            /// @if LANG_JA
            /// <summary>振動</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Oscillation</summary>
            /// @endif
            Elastic,

            /// @if LANG_JA
            /// <summary>カスタム</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Custom</summary>
            /// @endif
            Custom,
        }


        /// @if LANG_JA
        /// <summary>新しいシーンが古いシーンを押しだすトランジション</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Transition in which the new scene pushes out the old scene</summary>
        /// @endif
        public class PushTransition : Transition
        {
            const float defaultTime = 300.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PushTransition()
            {
                Time = defaultTime;
                MoveDirection = FourWayDirection.Left;
                Interpolator = PushTransitionInterpolator.EaseOutQuad;
                DrawOrder = TransitionDrawOrder.TransitionUIElement;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="direction">トランジションの移動方向</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="time">Duration (ms)</param>
            /// <param name="direction">Transition travel direction</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public PushTransition(float time, FourWayDirection direction, PushTransitionInterpolator interpolator)
            {
                Time = time;
                MoveDirection = direction;
                Interpolator = interpolator;
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
                // current scene
                ImageAsset currentImage = GetCurrentSceneRenderedImage();

                currentSprt = new UISprite(1);
                TransitionUIElement.AddChildLast(currentSprt);
                currentSprt.ShaderType = ShaderType.Texture;
                currentSprt.BlendMode = BlendMode.Premultiplied;
                currentSprt.Image = currentImage;

                UISpriteUnit unit = currentSprt.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;

                // next scene
                ImageAsset nextImage = GetNextSceneRenderedImage();

                nextSprt = new UISprite(1);
                TransitionUIElement.AddChildLast(nextSprt);
                nextSprt.ShaderType = ShaderType.Texture;
                nextSprt.BlendMode = BlendMode.Premultiplied;
                nextSprt.Image = nextImage;

                unit = nextSprt.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;

                from = 0.0f;

                switch (MoveDirection)
                {
                    case FourWayDirection.Left:
                        nextSprt.X = to = -UISystem.FramebufferWidth;
                        nextSprt.Y = 0.0f;
                        break;
                    case FourWayDirection.Right:
                        nextSprt.X = to = UISystem.FramebufferWidth;
                        nextSprt.Y = 0.0f;
                        break;
                    case FourWayDirection.Up:
                        nextSprt.X = 0.0f;
                        nextSprt.Y = to = -UISystem.FramebufferHeight;
                        break;
                    case FourWayDirection.Down:
                        nextSprt.X = 0.0f;
                        nextSprt.Y = to = UISystem.FramebufferHeight;
                        break;
                }

                switch (this.Interpolator)
                {
                    case PushTransitionInterpolator.Linear:
                        this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                        break;
                    case PushTransitionInterpolator.EaseOutQuad:
                        this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                        break;
                    case PushTransitionInterpolator.Overshoot:
                        this.interpolatorCallback = AnimationUtility.OvershootInterpolator;
                        break;
                    case PushTransitionInterpolator.Elastic:
                        this.interpolatorCallback = AnimationUtility.ElasticInterpolator;
                        break;
                    case PushTransitionInterpolator.Custom:
                        if (this.CustomInterpolator != null)
                        {
                            this.interpolatorCallback = this.CustomInterpolator;
                        }
                        else
                        {
                            this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                        }
                        break;
                    default:
                        UIDebug.Assert(false);
                        this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                        break;
                }
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// @endif
            protected override TransitionUpdateResponse OnUpdate(float elapsedTime)
            {
                if (TotalElapsedTime < Time)
                {
                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                            currentSprt.X = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                            nextSprt.X = this.interpolatorCallback(from + UISystem.FramebufferWidth, to + UISystem.FramebufferWidth, TotalElapsedTime / Time);
                            break;
                        case FourWayDirection.Right:
                            currentSprt.X = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                            nextSprt.X = this.interpolatorCallback(from - UISystem.FramebufferWidth, to - UISystem.FramebufferWidth, TotalElapsedTime / Time);
                            break;
                        case FourWayDirection.Up:
                            currentSprt.Y = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                            nextSprt.Y = this.interpolatorCallback(from + UISystem.FramebufferHeight, to + UISystem.FramebufferHeight, TotalElapsedTime / Time);
                            break;
                        case FourWayDirection.Down:
                            currentSprt.Y = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                            nextSprt.Y = this.interpolatorCallback(from - UISystem.FramebufferHeight, to - UISystem.FramebufferHeight, TotalElapsedTime / Time);
                            break;
                    }
                    return TransitionUpdateResponse.Continue;
                }
                else
                {
                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                            currentSprt.X = to;
                            nextSprt.X = to + UISystem.FramebufferWidth;
                            break;
                        case FourWayDirection.Right:
                            currentSprt.X = to;
                            nextSprt.X = to - UISystem.FramebufferWidth;
                            break;
                        case FourWayDirection.Up:
                            currentSprt.Y = to;
                            nextSprt.Y = to + UISystem.FramebufferHeight;
                            break;
                        case FourWayDirection.Down:
                            currentSprt.Y = to;
                            nextSprt.Y = to - UISystem.FramebufferHeight;
                            break;
                    }
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
                if (currentSprt != null)
                {
                    currentSprt.Image.Dispose();
                    currentSprt.Dispose();
                }

                if (nextSprt != null)
                {
                    nextSprt.Image.Dispose();
                    nextSprt.Dispose();
                }
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
            /// <summary>トランジションの移動方向を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the transition travel direction.</summary>
            /// @endif
            public FourWayDirection MoveDirection
            {
                set;
                get;
            }

            /// @if LANG_JA
            /// <summary>補間関数の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of interpolation function.</summary>
            /// @endif
            public PushTransitionInterpolator Interpolator
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>カスタムの補間関数を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets a custom interpolation function.</summary>
            /// @endif
            public AnimationInterpolator CustomInterpolator
            {
                get;
                set;
            }


            private float from;
            private float to;
            private UISprite currentSprt;
            private UISprite nextSprt;
            private AnimationInterpolator interpolatorCallback;

        }


    }
}
