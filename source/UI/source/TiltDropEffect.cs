/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>下方向へ落ちるエフェクト</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect that falls in a downward direction</summary>
        /// @endif
        public class TiltDropEffect : Effect
        {

            enum RotateDirection
            {
                ClockWise = 0,
                CounterClockWise
            }

            enum LeftPosition
            {
                LeftTop,
                RightTop,
                RightBottom,
                LeftBottom,
            }


            const float timeStep = 40.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TiltDropEffect()
            {
                init(null);
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="widget">Effect-target widget</param>
            /// @endif
            public TiltDropEffect(Widget widget)
            {
                init(widget);
            }

            private void init(Widget widget)
            {
                Widget = widget;
                Speed = speed;
                rotateDirection = RotateDirection.ClockWise;
                speed = 1.0f;
                this.rand = new Random();
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="widget">エフェクト対象のウィジェット</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="widget">Effect-target widget</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static TiltDropEffect CreateAndStart(Widget widget)
            {
                TiltDropEffect effect = new TiltDropEffect(widget);
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

                // reset transform3D
                Matrix4 matrix = Matrix4.Identity;
                matrix.M41 = Widget.X;
                matrix.M42 = Widget.Y;
                Widget.Transform3D = matrix;
                originalPosition = matrix;

                this.originalPivotType = Widget.PivotType;
                Widget.PivotType = PivotType.MiddleCenter;

                this.pivotOffsetX = Widget.Width / 2.0f;
                this.pivotOffsetY = Widget.Height / 2.0f;

                this.fromX = Widget.X;
                this.fromY = Widget.Y;
                this.outsideMargin = (float)Math.Sqrt(pivotOffsetX * pivotOffsetX + pivotOffsetY * pivotOffsetY);
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

                this.targetRadianSpeed = 0.0f;
                this.targetRadian = 0.0f;
                this.targetOffsetXSpeed = 0.0f;
                this.targetOffsetX = 0.0f;
                this.targetOffsetYSpeed = 0.0f;
                this.targetOffsetY = 0.0f;

                this.T = 40.0f;
                this.leftTime = T / (14.0f + 4.0f * (float)rand.NextDouble());

                setupLeftPos();

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

                CalcPosition(TotalElapsedTime / timeStep * speed, elapsedTime / timeStep * speed);

                Widget.X = targetOffsetX + fromX;
                Widget.Y = targetOffsetY + fromY;
                CalcMatrix();

                if (Widget.X < -outsideMargin || Widget.X > screenWidth + outsideMargin ||
                    Widget.Y < -outsideMargin || Widget.Y > screenHeight + outsideMargin)
                {
                    return EffectUpdateResponse.Finish;
                }
                else
                {
                    return EffectUpdateResponse.Continue;
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
                Widget.Visible = false;
                Widget.PivotType = originalPivotType;
                Widget.Transform3D = originalPosition;
            }

            /// @if LANG_JA
            /// <summary>落ちる方向（ラジアン）を取得・設定する。</summary>
            /// <remarks>下方向が0。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the falling direction (radian).</summary>
            /// <remarks>Downward direction is 0.</remarks>
            /// @endif
            public float DropDirection
            {
                get { return dropDirection; }
                set { dropDirection = value; }
            }

            /// @if LANG_JA
            /// <summary>エフェクトの速度を取得・設定する。1.0 が標準速度。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the effect speed. 1.0 is the standard speed.</summary>
            /// @endif
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }

            private void setupLeftPos()
            {
                float rad = (float)Math.Atan2(Widget.Height, Widget.Width);

                float dropDirection = this.dropDirection + (float)(rand.NextDouble() - 0.5) / 100;
                while (dropDirection < 0f) dropDirection += (float)Math.PI * 2f;
                while (dropDirection > (float)Math.PI * 2f) dropDirection -= (float)Math.PI * 2f;


                if (Math.PI / 2.0 - rad > dropDirection)
                {
                    leftPosition = LeftPosition.LeftTop;
                    rotateDirection = RotateDirection.ClockWise;
                }
                else if (Math.PI / 2.0 > dropDirection)
                {
                    leftPosition = LeftPosition.RightBottom;
                    rotateDirection = RotateDirection.CounterClockWise;
                }
                else if (Math.PI / 2.0 + rad > dropDirection)
                {
                    leftPosition = LeftPosition.RightTop;
                    rotateDirection = RotateDirection.ClockWise;
                }
                else if (Math.PI * 3.0 / 2.0 > dropDirection)
                {
                    leftPosition = LeftPosition.LeftBottom;
                    rotateDirection = RotateDirection.CounterClockWise;
                }
                else if (Math.PI * 3.0 / 2.0 - rad > dropDirection)
                {
                    leftPosition = LeftPosition.RightBottom;
                    rotateDirection = RotateDirection.ClockWise;
                }
                else if (Math.PI * 3.0 / 2.0 > dropDirection)
                {
                    leftPosition = LeftPosition.LeftTop;
                    rotateDirection = RotateDirection.CounterClockWise;
                }
                else if (Math.PI * 3.0 / 2.0 + rad > dropDirection)
                {
                    leftPosition = LeftPosition.LeftBottom;
                    rotateDirection = RotateDirection.ClockWise;
                }
                else
                {
                    leftPosition = LeftPosition.RightTop;
                    rotateDirection = RotateDirection.CounterClockWise;
                }
                
                float leftRadian;
                calcRotaitonOffset(leftTime, out leftRadian, out this.leftOffsetX, out this.leftOffsetY);
            }

            void calcRotaitonOffset(float time, out float radian, out float offsetX, out float offsetY)
            {
                radian = (1.0f + (float)Math.Sin(time * (2.0f * (float)Math.PI) / T - (float)Math.PI / 2.0f)) / 2.0f * (float)Math.PI;
                offsetX = -(float)Math.Sin(radian) * pivotOffsetY;
                offsetY = (float)Math.Sin(radian) * pivotOffsetX;


                switch (rotateDirection)
                {
                    case RotateDirection.ClockWise:
                        switch (leftPosition)
                        {
                            case LeftPosition.LeftTop:
                                break;
                            case LeftPosition.RightTop:
                                offsetY = -offsetY;
                                break;
                            case LeftPosition.RightBottom:
                                offsetX = -offsetX;
                                offsetY = -offsetY;
                                break;
                            case LeftPosition.LeftBottom:
                                offsetX = -offsetX;
                                break;
                            default:
                                break;
                        }
                        break;
                    case RotateDirection.CounterClockWise:
                        radian = -radian;
                        switch (leftPosition)
                        {
                            case LeftPosition.LeftTop:
                                offsetX = -offsetX;
                                offsetY = -offsetY;
                                break;
                            case LeftPosition.RightTop:
                                offsetX = -offsetX;
                                break;
                            case LeftPosition.RightBottom:
                                break;
                            case LeftPosition.LeftBottom:
                                offsetY = -offsetY;
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }

            private void CalcPosition(float t, float elapsedTime)
            {
                if (t < leftTime)
                {
                    float currentRadian;
                    float currentOffsetX;
                    float currentOffsetY;
                    calcRotaitonOffset(t, out currentRadian, out currentOffsetX, out currentOffsetY);

                    targetRadianSpeed = (currentRadian - targetRadian) / elapsedTime;
                    targetRadian = currentRadian;
                    targetOffsetXSpeed = (currentOffsetX - targetOffsetX) / elapsedTime;
                    targetOffsetX = currentOffsetX;
                    targetOffsetYSpeed = (currentOffsetY - targetOffsetY) / elapsedTime;
                    targetOffsetY = currentOffsetY;
                }
                else
                {
                    float diff_time = (t - leftTime);
                    float distance = 0.5f * 9.8f * diff_time * diff_time;
                    targetOffsetX = -distance * (float)Math.Sin(dropDirection) + targetOffsetXSpeed * diff_time + leftOffsetX;
                    targetOffsetY = distance * (float)Math.Cos(dropDirection) + targetOffsetYSpeed * diff_time + leftOffsetY;
                    targetRadian += targetRadianSpeed * elapsedTime;
                }
            }

            private void CalcMatrix()
            {
                Matrix4 matA = Matrix4.Translation(new Vector3(Widget.X, Widget.Y, 0.0f));

                Matrix4 matB;
                Matrix4 mat3 = Matrix4.RotationZ(targetRadian);
                matA.Multiply(ref mat3, out matB);

                Widget.Transform3D = matB;
            }


            private RotateDirection rotateDirection;
            private float speed;

            private Random rand;

            private PivotType originalPivotType;
            private Matrix4 originalPosition;

            private float leftTime;
            private float T;

            private float fromX;
            private float fromY;
            private float outsideMargin;

            private float dropDirection;

            private float pivotOffsetX;
            private float pivotOffsetY;

            private float targetRadianSpeed;
            private float targetRadian;

            private float targetOffsetXSpeed;
            private float targetOffsetX;

            private float targetOffsetYSpeed;
            private float targetOffsetY;

            private float screenWidth;
            private float screenHeight;

            LeftPosition leftPosition;
            float leftOffsetX;
            float leftOffsetY;

        }


    }
}
