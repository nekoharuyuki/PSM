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
        /// <summary>MoveEffect用の補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function for MoveEffect</summary>
        /// @endif
        public enum MoveEffectInterpolator
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
        /// <summary>現在の位置から指定した位置に移動するエフェクト</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect that moves from the current position to a specified location</summary>
        /// @endif
        public class MoveEffect : Effect
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public MoveEffect()
            {
                Widget = null;
                Time = 1000.0f;
                X = 0.0f;
                Y = 0.0f;
                Interpolator = MoveEffectInterpolator.Linear;
                CustomInterpolator = null;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクトの対象となるウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="x">親の座標系での移動先のX座標</param>
            /// <param name="y">親の座標系での移動先のY座標</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="x">X coordinate of destination in the parent coordinate system</param>
            /// <param name="y">Y coordinate of destination in the parent coordinate system</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public MoveEffect(Widget widget, float time, float x, float y, MoveEffectInterpolator interpolator)
            {
                Widget = widget;
                Time = time;
                X = x;
                Y = y;
                Interpolator = interpolator;
                CustomInterpolator = null;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="x">親の座標系での移動先のX座標</param>
            /// <param name="y">親の座標系での移動先のY座標</param>
            /// <param name="interpolator">補間関数</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="x">X coordinate of destination in the parent coordinate system</param>
            /// <param name="y">Y coordinate of destination in the parent coordinate system</param>
            /// <param name="interpolator">Interpolation function</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static MoveEffect CreateAndStart(Widget widget, float time, float x, float y, MoveEffectInterpolator interpolator)
            {
                MoveEffect effect = new MoveEffect(widget, time, x, y, interpolator);
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
                        case MoveEffectInterpolator.Linear:
                            this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                            break;
                        case MoveEffectInterpolator.EaseOutQuad:
                            this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                            break;
                        case MoveEffectInterpolator.Overshoot:
                            this.interpolatorCallback = AnimationUtility.OvershootInterpolator;
                            break;
                        case MoveEffectInterpolator.Elastic:
                            this.interpolatorCallback = AnimationUtility.ElasticInterpolator;
                            break;
                        case MoveEffectInterpolator.Custom:
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

                    fromX = Widget.X;
                    fromY = Widget.Y;
                    toX = X;
                    toY = Y;
                    Widget.Visible = true;
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
            protected override EffectUpdateResponse OnUpdate(float elapsedTime)
            {
                if (TotalElapsedTime < Time)
                {
                    Widget.X = this.interpolatorCallback(fromX, toX, TotalElapsedTime / Time);
                    Widget.Y = this.interpolatorCallback(fromY, toY, TotalElapsedTime / Time);
                    return EffectUpdateResponse.Continue;
                }
                else
                {
                    Widget.X = toX;
                    Widget.Y = toY;
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
            /// <summary>親の座標系での移動先のX座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the X coordinate of the destination in the parent coordinate system.</summary>
            /// @endif
            public float X
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>親の座標系での移動先のY座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Y coordinate of the destination in the parent coordinate system.</summary>
            /// @endif
            public float Y
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
            public MoveEffectInterpolator Interpolator
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

            private float fromX;
            private float fromY;
            private float toX;
            private float toY;
            private AnimationInterpolator interpolatorCallback;

        }


    }
}
