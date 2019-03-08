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
        /// <summary>ウィジェットが上から落ちてきて少し跳ねて止まるエフェクト</summary>
        /// <remarks>"Little Big Planet"のメニュー画面風エフェクト</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect in which the widget falls from above, hops a little, and then stops</summary>
        /// <remarks>Effect in which the menu screen is similar to "Little Big Planet"</remarks>
        /// @endif
        public class BunjeeJumpEffect : Effect
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public BunjeeJumpEffect()
            {
                Widget = null;
                Elasticity = 0.4f;
                TargetX = 0.0f;
                TargetY = 0.0f;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="elasticity">弾性率（０～１）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="elasticity">Elasticity (0 - 1)</param>
            /// @endif
            public BunjeeJumpEffect(Widget widget, float elasticity)
            {
                Widget = widget;
                Elasticity = elasticity;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <param name="elasticity">弾性率（０～１）</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Create an instance and start the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <param name="elasticity">Elasticity (0 - 1)</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static BunjeeJumpEffect CreateAndStart(Widget widget, float elasticity)
            {
                BunjeeJumpEffect effect = new BunjeeJumpEffect(widget, elasticity);
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
                if (Widget == null)
                {
                    return;
                }

                orgPivotType = Widget.PivotType;
                Widget.PivotType = PivotType.TopLeft;

                TargetX = Widget.X;
                TargetY = Widget.Y;



                x = 0.0f;
                y = 0.0f;
                x0 = Widget.X;
                //todo:
                y0 = -480;
                v0 = CalcFirstVelocity();
                turnTime = 0.0f;
                rotateDegreeZ = 0.0f;
                rotateDegreeDelta = 0.35f;
                Random rnd = new Random();
                int randomNumber = rnd.Next(2);
                switch (randomNumber)
                {
                    case 0:
                        turn = 1.0f;
                        break;
                    case 1:
                        turn = -1.0f;
                        break;
                    default:
                        turn = 1.0f;
                        break;
                }
                bounceCount = 0;

                Widget.Y = y0;
                Widget.Visible = true;
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
                if (Widget == null)
                {
                    return EffectUpdateResponse.Finish;
                }

                float t = TotalElapsedTime / timeStep;
                bool end = false;

                CalcPosition(t - turnTime);

                Widget.X = x0 + x;
                Widget.Y = y0 + y;

                rotateDegreeZ += turn * rotateDegreeDelta * elapsedTime / timeStep;
                
                if (Widget.Y >= TargetY)
                {
                    Widget.Y = TargetY;
                    turnTime = t;
                    x0 = Widget.X;
                    y0 = Widget.Y;
                    x = 0.0f;
                    y = 0.0f;
                    turn *= -1.0f;
                    bounceCount++;

                    if (bounceCount == 1)
                    {
                        v0 = g * t * Elasticity;
                        if (v0 > 35.0f)
                        {
                            v0 = 35.0f;
                        }
                        rotateDegreeDelta = (1.7f * Math.Abs(rotateDegreeZ)) / (2.0f * v0 / g);
                    }
                    else
                    {
                        v0 *= Elasticity;
                        rotateDegreeDelta = (1.25f * Math.Abs(rotateDegreeZ)) / (2.0f * v0 / g);

                        if (bounceCount >= 4)
                        {
                            v0 *= Elasticity;
                            rotateDegreeDelta = (Math.Abs(rotateDegreeZ)) / (2.0f * v0 / g);
                            if (rotateDegreeZ < 0.5f)
                            {
                                rotateDegreeZ = 0.0f;
                                end = true;
                            }
                        }
                    }
                }

                RotateWidget();

                return end ? EffectUpdateResponse.Finish : EffectUpdateResponse.Continue;
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
                if (Widget == null)
                {
                    return;
                }

                Widget.PivotType = orgPivotType;
            }

            /// @if LANG_JA
            /// <summary>弾性率を取得・設定する。（０～１）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the elasticity. (0 - 1)</summary>
            /// @endif
            public float Elasticity
            {
                get
                {
                    return elasticity;
                }
                set
                {
                    elasticity = FMath.Clamp(value, 0.0f, 1.0f);
                }
            }
            private float elasticity;

            /// @if LANG_JA
            /// <summary>親の座標系での落下位置のX座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the X coordinate of the drop position in the parent coordinate system.</summary>
            /// @endif
            private float TargetX
            {
                get;
                set;
            }


            /// @if LANG_JA
            /// <summary>親の座標系での落下位置のY座標を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the Y coordinate of the drop position in the parent coordinate system.</summary>
            /// @endif
            private float TargetY
            {
                get;
                set;
            }


            private void CalcPosition(float t)
            {
                y = 0.5f * g * t * t - v0 * t;
                x = 0.0f;
            }


            private float CalcFirstVelocity()
            {
                if(TargetY == Widget.Y)
                {
                    return 0.0f;
                }
                return (TargetX - Widget.X) / (float)Math.Sqrt(2.0f * (TargetY - Widget.Y) / g);
            }


            private void RotateWidget()
            {
                Matrix4 matA;
                Matrix4 mat1 = Matrix4.Translation(new Vector3(Widget.X, Widget.Y, 0.0f));
                Matrix4 mat2 = Matrix4.Translation(new Vector3(Widget.Width / 2.0f, Widget.Height / 2.0f, 0.0f));
                mat1.Multiply(ref mat2, out matA);

                Matrix4 matB;
                Matrix4 mat3 = Matrix4.RotationZ(rotateDegreeZ / 360.0f * 2.0f * (float)Math.PI);
                matA.Multiply(ref mat3, out matB);

                Matrix4 matC;
                Matrix4 mat4 = Matrix4.Translation(new Vector3(-Widget.Width / 2.0f, -Widget.Height / 2.0f, 0.0f));
                matB.Multiply(ref mat4, out matC);

                Widget.Transform3D = matC;
            }


            private float x;
            private float y;
            private float x0;
            private float y0;
            private float v0;
            private float turnTime;
            private float rotateDegreeZ;
            private float rotateDegreeDelta;
            private float turn;
            private PivotType orgPivotType;
            private int bounceCount;

            const float g = 9.8f;
            const float vLimit = 0.3f;
            const float timeStep = 50.0f;

        }


    }
}
