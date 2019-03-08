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
        /// <summary>動作対象のシーン</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Scene for operation</summary>
        /// @endif
        public enum MoveTarget
        {
            /// @if LANG_JA
            /// <summary>次のシーン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Next scene</summary>
            /// @endif
            NextScene = 0,

            /// @if LANG_JA
            /// <summary>現在のシーン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Current scene</summary>
            /// @endif
            CurrentScene
        }


        /// @if LANG_JA
        /// <summary>補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function</summary>
        /// @endif
        public enum SlideTransitionInterpolator
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
        /// <summary>平行移動してシーンを切り替えるトランジション</summary>
        /// <remarks>MoveTargetプロパティにNextSceneを指定した場合、NextSceneが前面、CurrentSceneが背面になり、NextSceneがMoveDirectionプロパティで指定した方向にスライドインします。
        /// MoveTargetプロパティにCurrentSceneを指定した場合、CurrentSceneが前面、NextSceneが背面になり、CurrentSceneがMoveDirectionプロパティで指定した方向にスライドアウトします。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Transition in which the object moves laterally and the scene changes</summary>
        /// <remarks>When NextScene is specified for the MoveTarget property, NextScene becomes the foreground, CurrentScene becomes the background, and NextScene slides in in the direction specified with the MoveDirection property.
        /// When CurrentScene is specified for the MoveTarget property, CurrentScene becomes the foreground, NextScene becomes the background, and CurrentScene slides out in the direction specified with the MoveDirection property.</remarks>
        /// @endif
        public class SlideTransition : Transition
        {
            const float defaultTime = 300.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public SlideTransition()
            {
                Time = defaultTime;
                MoveDirection = FourWayDirection.Left;
                MoveTarget = MoveTarget.NextScene;
                Interpolator = SlideTransitionInterpolator.EaseOutQuad;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="direction">トランジションの移動方向</param>
            /// <param name="moveTarget">重ね合わせの優先度</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="time">Duration (ms)</param>
            /// <param name="direction">Transition travel direction</param>
            /// <param name="moveTarget">Priority of superposition</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public SlideTransition(float time, FourWayDirection direction,
                MoveTarget moveTarget, SlideTransitionInterpolator interpolator)
            {
                Time = time;
                MoveDirection = direction;
                MoveTarget = moveTarget;
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
                UISpriteUnit unit;

                switch (MoveTarget)
                {
                    case MoveTarget.NextScene:
                        // make next scene texture
                        ImageAsset nextImage = GetNextSceneRenderedImage();
                        nextSprt = new UISprite(1);
                        nextSprt.ShaderType = ShaderType.Texture;
                        nextSprt.BlendMode = BlendMode.Premultiplied;
                        nextSprt.Image = nextImage;

                        unit = nextSprt.GetUnit(0);
                        unit.Width = UISystem.FramebufferWidth;
                        unit.Height = UISystem.FramebufferHeight;
                        TransitionUIElement.AddChildLast(nextSprt);

                        to = 0.0f;

                        switch (MoveDirection)
                        {
                            case FourWayDirection.Left:
                                nextSprt.X = from = UISystem.FramebufferWidth;
                                nextSprt.Y = 0.0f;
                                break;
                            case FourWayDirection.Right:
                                nextSprt.X = from = -UISystem.FramebufferWidth;
                                nextSprt.Y = 0.0f;
                                break;
                            case FourWayDirection.Up:
                                nextSprt.X = 0.0f;
                                nextSprt.Y = from = UISystem.FramebufferHeight;
                                break;
                            case FourWayDirection.Down:
                                nextSprt.X = 0.0f;
                                nextSprt.Y = from = -UISystem.FramebufferHeight;
                                break;
                        }
                        break;

                    case MoveTarget.CurrentScene:
                        // make current scene texture
                        ImageAsset currentImage = GetCurrentSceneRenderedImage();
                        currentSprt = new UISprite(1);
                        currentSprt.ShaderType = ShaderType.Texture;
                        currentSprt.BlendMode = BlendMode.Premultiplied;
                        currentSprt.Image = currentImage;

                        unit = currentSprt.GetUnit(0);
                        unit.Width = UISystem.FramebufferWidth;
                        unit.Height = UISystem.FramebufferHeight;
                        TransitionUIElement.AddChildLast(currentSprt);

                        from = 0.0f;

                        switch (MoveDirection)
                        {
                            case FourWayDirection.Left:
                                to = -UISystem.FramebufferWidth;
                                break;
                            case FourWayDirection.Right:
                                to = UISystem.FramebufferWidth;
                                break;
                            case FourWayDirection.Up:
                                to = -UISystem.FramebufferHeight;
                                break;
                            case FourWayDirection.Down:
                                to = UISystem.FramebufferHeight;
                                break;
                        }
                        break;
                }

                switch (this.Interpolator)
                {
                    case SlideTransitionInterpolator.Linear:
                        this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                        break;
                    case SlideTransitionInterpolator.EaseOutQuad:
                        this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                        break;
                    case SlideTransitionInterpolator.Overshoot:
                        this.interpolatorCallback = AnimationUtility.OvershootInterpolator;
                        break;
                    case SlideTransitionInterpolator.Elastic:
                        this.interpolatorCallback = AnimationUtility.ElasticInterpolator;
                        break;
                    case SlideTransitionInterpolator.Custom:
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
                    switch (MoveTarget)
                    {
                        case MoveTarget.NextScene:
                            switch (MoveDirection)
                            {
                                case FourWayDirection.Left:
                                case FourWayDirection.Right:
                                    nextSprt.X = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                                    break;
                                case FourWayDirection.Up:
                                case FourWayDirection.Down:
                                    nextSprt.Y = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                                    break;
                            }
                            break;

                        case MoveTarget.CurrentScene:
                            switch (MoveDirection)
                            {
                                case FourWayDirection.Left:
                                case FourWayDirection.Right:
                                    currentSprt.X = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                                    break;
                                case FourWayDirection.Up:
                                case FourWayDirection.Down:
                                    currentSprt.Y = this.interpolatorCallback(from, to, TotalElapsedTime / Time);
                                    break;
                            }
                            break;
                    }
                    return TransitionUpdateResponse.Continue;
                }
                else
                {
                    switch (MoveTarget)
                    {
                        case MoveTarget.NextScene:
                            switch (MoveDirection)
                            {
                                case FourWayDirection.Left:
                                case FourWayDirection.Right:
                                    nextSprt.X = to;
                                    break;
                                case FourWayDirection.Up:
                                case FourWayDirection.Down:
                                    nextSprt.Y = to;
                                    break;
                            }
                            break;

                        case MoveTarget.CurrentScene:
                            switch (MoveDirection)
                            {
                                case FourWayDirection.Left:
                                case FourWayDirection.Right:
                                    currentSprt.X = to;
                                    break;
                                case FourWayDirection.Up:
                                case FourWayDirection.Down:
                                    currentSprt.Y = to;
                                    break;
                            }
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
            /// <summary>スライドするシーンがCurrentSceneかNextSceneかを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether the sliding scene is CurrentScene or NextScene</summary>
            /// @endif
            public MoveTarget MoveTarget
            {
                get
                {
                    return moveTarget;
                }
                set
                {
                    moveTarget = value;

                    if (moveTarget == MoveTarget.NextScene)
                    {
                        DrawOrder = TransitionDrawOrder.CS_TE;
                    }
                    else
                    {
                        DrawOrder = TransitionDrawOrder.NS_TE;
                    }
                }
            }
            private MoveTarget moveTarget;

            /// @if LANG_JA
            /// <summary>補間関数の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of interpolation function.</summary>
            /// @endif
            public SlideTransitionInterpolator Interpolator
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


            private float from;
            private float to;
            private UISprite currentSprt;
            private UISprite nextSprt;
            private AnimationInterpolator interpolatorCallback;

        }


    }
}
