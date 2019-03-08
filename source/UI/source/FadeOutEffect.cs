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
        /// <summary>FadeOutEffect用の補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function for FadeOutEffect</summary>
        /// @endif
        public enum FadeOutEffectInterpolator
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
        /// <summary>フェードアウトエフェクト</summary>
        /// <remarks>不透明（設定されているアルファ値）から透明まで変化する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Fade-out effect</summary>
        /// <remarks>Changes from opaque (set alpha value) to transparent.</remarks>
        /// @endif
        public class FadeOutEffect : Effect
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public FadeOutEffect()
            {
                Widget = null;
                Time = 1000.0f;
                Interpolator = FadeOutEffectInterpolator.EaseOutQuad;
                CustomInterpolator = null;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクトの対象となるウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public FadeOutEffect(Widget widget, float time, FadeOutEffectInterpolator interpolator)
            {
                Widget = widget;
                Time = time;
                Interpolator = FadeOutEffectInterpolator.EaseOutQuad;
                CustomInterpolator = null;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクトの対象となるウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public static FadeOutEffect CreateAndStart(Widget widget, float time, FadeOutEffectInterpolator interpolator)
            {
                FadeOutEffect effect = new FadeOutEffect(widget, time, interpolator);
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
                        case FadeOutEffectInterpolator.Linear:
                            this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                            break;
                        case FadeOutEffectInterpolator.EaseOutQuad:
                            this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                            break;
                        case FadeOutEffectInterpolator.Custom:
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
                    Widget.Visible = true;
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
                if (this.TotalElapsedTime < this.Time)
                {
                    float alpha = this.interpolatorCallback(1.0f, 0.0f, this.TotalElapsedTime / this.Time);
                    Widget.Alpha = (alpha > 0.0f) ? alpha : 0.0f;
                    return EffectUpdateResponse.Continue;
                }
                else
                {
                    Widget.Alpha = 0.0f;
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
            public FadeOutEffectInterpolator Interpolator
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

            private AnimationInterpolator interpolatorCallback;

        }


    }
}
