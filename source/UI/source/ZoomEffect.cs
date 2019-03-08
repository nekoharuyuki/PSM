/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>ZoomEffect用の補間関数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Interpolation function for ZoomEffect</summary>
        /// @endif
        public enum ZoomEffectInterpolator
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
        /// <summary>現在のスケールから指定したスケールに拡大または縮小するエフェクト</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect that enlarges or reduces the current scale to the specified scale</summary>
        /// @endif
        public class ZoomEffect : Effect
        {
            private float time;
            private float targetScaleX;
            private float targetScaleY;
            private float targetScaleZ;
            private float startScaleX;
            private float startScaleY;
            private float startScaleZ;
            private Matrix4 baseTransformMat;
            private AnimationInterpolator interpolatorCallback;


            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ZoomEffect()
            {
                Widget = null;
                Time = 1000.0f;
                targetScaleX = 1.0f;
                targetScaleY = 1.0f;
                targetScaleZ = 1.0f;
                Interpolator = ZoomEffectInterpolator.EaseOutQuad;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクトの対象となるウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="scale">目標のスケール</param>
            /// <param name="interpolator">補間関数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="scale">Target scale</param>
            /// <param name="interpolator">Interpolation function</param>
            /// @endif
            public ZoomEffect(Widget widget, float time, float scale, ZoomEffectInterpolator interpolator)
            {
                Widget = widget;
                Time = time;
                this.targetScaleX = scale;
                this.targetScaleY = scale;
                this.targetScaleZ = scale;
                Interpolator = interpolator;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="time">持続時間（ミリ秒）</param>
            /// <param name="scale">目標のスケール</param>
            /// <param name="interpolator">補間関数</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="time">Duration (ms)</param>
            /// <param name="scale">Target scale</param>
            /// <param name="interpolator">Interpolation function</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static ZoomEffect CreateAndStart(Widget widget, float time, float scale, ZoomEffectInterpolator interpolator)
            {
                ZoomEffect effect = new ZoomEffect(widget, time, scale, interpolator);
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
                        case ZoomEffectInterpolator.Linear:
                            this.interpolatorCallback = AnimationUtility.LinearInterpolator;
                            break;
                        case ZoomEffectInterpolator.EaseOutQuad:
                            this.interpolatorCallback = AnimationUtility.EaseOutQuadInterpolator;
                            break;
                        case ZoomEffectInterpolator.Overshoot:
                            this.interpolatorCallback = ZoomOvershootInterpolator;
                            break;
                        case ZoomEffectInterpolator.Elastic:
                            this.interpolatorCallback = ZoomElasticInterpolator;
                            break;
                        case ZoomEffectInterpolator.Custom:
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

                    this.baseTransformMat = this.Widget.Transform3D;
                    this.startScaleX = this.baseTransformMat.ColumnX.Length();
                    this.startScaleY = this.baseTransformMat.ColumnY.Length();
                    this.startScaleZ = this.baseTransformMat.ColumnZ.Length();

                    this.baseTransformMat.ColumnX /= this.startScaleX;
                    this.baseTransformMat.ColumnY /= this.startScaleY;
                    this.baseTransformMat.ColumnZ /= this.startScaleZ;
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
                if (this.Widget == null) return EffectUpdateResponse.Finish;

                EffectUpdateResponse response = EffectUpdateResponse.Continue;

                float scaleX;
                float scaleY;
                float scaleZ;

                if (this.TotalElapsedTime < this.time)
                {
                    float ratio = this.TotalElapsedTime / this.time;

                    scaleX = this.interpolatorCallback(this.startScaleX, this.targetScaleX, ratio);
                    scaleY = this.interpolatorCallback(this.startScaleY, this.targetScaleY, ratio);
                    scaleZ = this.interpolatorCallback(this.startScaleZ, this.targetScaleZ, ratio);

                    //ratio = this.interpolatorCallback(0, (float)Math.Log(targetScaleZ), ratio);
                    //scaleX = scaleY = scaleZ = (float)Math.Exp(ratio);
                }
                else
                {
                    scaleX = this.targetScaleX;
                    scaleY = this.targetScaleY;
                    scaleZ = this.targetScaleZ;

                    response = EffectUpdateResponse.Finish;
                }

                Matrix4 mat;
                Matrix4 scaleMat = Matrix4.Scale(new Vector3(scaleX, scaleY, scaleZ));
                baseTransformMat.Multiply(ref scaleMat, out mat);
                this.Widget.Transform3D = mat;
                return response;
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
            /// <summary>目標のX方向のスケールを取得・設定する。（等倍＝１）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scale in the target X direction. (Magnification = 1)</summary>
            /// @endif
            public float TargetScaleX
            {
                get { return targetScaleX; }
                set { targetScaleX = value; }
            }


            /// @if LANG_JA
            /// <summary>目標のY方向のスケールを取得・設定する。（等倍＝１）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scale in the target Y direction. (Magnification = 1)</summary>
            /// @endif
            public float TargetScaleY
            {
                get { return targetScaleY; }
                set { targetScaleY = value; }
            }

            /// @if LANG_JA
            /// <summary>目標のZ方向のスケールを取得・設定する。（等倍＝１）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scale in the target Z direction. (Magnification = 1)</summary>
            /// @endif
            public float TargetScaleZ
            {
                get { return targetScaleZ; }
                set { targetScaleZ = value; }
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

            /// @if LANG_JA
            /// <summary>補間関数の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of interpolation function.</summary>
            /// @endif
            public ZoomEffectInterpolator Interpolator
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

            static float ZoomElasticInterpolator(float from, float to, float ratio)
            {
                ratio = AnimationUtility.ElasticInterpolator(0, (float)Math.Log(to / from), ratio);
                return (float)Math.Exp(ratio) * from;
            }

            static float ZoomOvershootInterpolator(float from, float to, float ratio)
            {
                ratio = AnimationUtility.OvershootInterpolator(0, (float)Math.Log(to/from), ratio);
                return (float)Math.Exp(ratio) * from;
            }

        }
    }
}
