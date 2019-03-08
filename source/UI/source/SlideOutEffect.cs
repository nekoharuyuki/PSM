/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;

namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>SlideOutEffect用の補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function for SlideOutEffect</summary>
        /// @endif
        public enum SlideOutEffectInterpolator
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
        /// <summary>現在の位置から画面の外まで平行移動するエフェクト</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect in which the object slides laterally out of view from the current position</summary>
        /// @endif
        public class SlideOutEffect : Effect
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public SlideOutEffect()
            {
                Widget = null;
                Time = 1000.0f;
                Interpolator = SlideOutEffectInterpolator.EaseOutQuad;
                CustomInterpolator = null;
                MoveDirection = FourWayDirection.Left;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="direction">エフェクトの移動方向</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="direction">Effect travel direction</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public SlideOutEffect(Widget widget, float time,
                FourWayDirection direction, SlideOutEffectInterpolator interpolator)
            {
                Widget = widget;
                Time = time;
                Interpolator = interpolator;
                CustomInterpolator = null;
                MoveDirection = direction;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="direction">エフェクトの移動方向</param>
            /// <param name="interpolator">補間関数</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="direction">Effect travel direction</param>
            /// <param name="interpolator">Interpolation function</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static SlideOutEffect CreateAndStart(Widget widget, float time,
                FourWayDirection direction, SlideOutEffectInterpolator interpolator)
            {
                SlideOutEffect effect = new SlideOutEffect(widget, time, direction, interpolator);
                effect.Start();
                return effect;
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                if (this.Widget != null)
                {
                    switch (this.Interpolator)
                    {
                        case SlideOutEffectInterpolator.Linear:
                            this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                            break;
                        case SlideOutEffectInterpolator.EaseOutQuad:
                            this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                            break;
                        case SlideOutEffectInterpolator.Overshoot:
                            this.interpolatorCallback = AnimationUtility.OvershootInterpolator;
                            break;
                        case SlideOutEffectInterpolator.Elastic:
                            this.interpolatorCallback = AnimationUtility.ElasticInterpolator;
                            break;
                        case SlideOutEffectInterpolator.Custom:
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

                    float screenWidth, screenHeight;
                    var scene = Widget.ParentScene;
                    if (scene != null)
                    {
                        screenWidth = scene.RootWidget.Width;
                        screenHeight = scene.RootWidget.Height;
                    }
                    else
                    {
                        screenWidth = UISystem.CurrentScreenWidth;
                        screenHeight = UISystem.CurrentScreenHeight;
                    }

                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                            from = Widget.X;
                            to = Widget.X - screenWidth;
                            orgWidgetX = Widget.X;
                            Widget.Visible = true;
                            break;
                        case FourWayDirection.Right:
                            from = Widget.X;
                            to = Widget.X + screenWidth;
                            orgWidgetX = Widget.X;
                            Widget.Visible = true;
                            break;
                        case FourWayDirection.Up:
                            from = Widget.Y;
                            to = Widget.Y - screenHeight;
                            orgWidgetY = Widget.Y;
                            Widget.Visible = true;
                            break;
                        case FourWayDirection.Down:
                            from = Widget.Y;
                            to = Widget.Y + screenHeight;
                            orgWidgetY = Widget.Y;
                            Widget.Visible = true;
                            break;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// <returns>エフェクトの更新の応答</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// <returns>Response of effect update</returns>
            /// @endif
            protected override EffectUpdateResponse OnUpdate(float elapsedTime)
            {
                float move = this.interpolatorCallback(from, to, TotalElapsedTime / Time);

                if (this.TotalElapsedTime < this.Time)
                {
                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                        case FourWayDirection.Right:
                            Widget.X = move;
                            break;
                        case FourWayDirection.Up:
                        case FourWayDirection.Down:
                            Widget.Y = move;
                            break;
                    }
                    return EffectUpdateResponse.Continue;
                }
                else
                {
                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                        case FourWayDirection.Right:
                            Widget.X = to;
                            break;
                        case FourWayDirection.Up:
                        case FourWayDirection.Down:
                            Widget.Y = to;
                            break;
                    }
                    return EffectUpdateResponse.Finish;
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
                if (this.Widget != null)
                {
                    switch (MoveDirection)
                    {
                        case FourWayDirection.Left:
                        case FourWayDirection.Right:
                            Widget.X = orgWidgetX;
                            Widget.Visible = false;
                            break;
                        case FourWayDirection.Up:
                        case FourWayDirection.Down:
                            Widget.Y = orgWidgetY;
                            Widget.Visible = false;
                            break;
                    }
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
            /// <summary>エフェクトの移動方向を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the effect travel direction.</summary>
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
            public SlideOutEffectInterpolator Interpolator
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
            private AnimationInterpolator interpolatorCallback;
            private float orgWidgetX;
            private float orgWidgetY;

        }


    }
}
