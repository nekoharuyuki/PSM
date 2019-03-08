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
        /// <summary>下方向へ落ちるトランジション</summary>
        /// <remarks>新しいシーンが後ろから現れる</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Transition that falls in a downward direction</summary>
        /// <remarks>New scene appears from the background</remarks>
        /// @endif
        public class TiltDropTransition : Transition
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
            public TiltDropTransition()
            {
                DrawOrder = TransitionDrawOrder.NS_TE;
                rotateDirection = RotateDirection.CounterClockWise;
                speed = 1.0f;
                this.rand = new Random();
            }

            /// @if LANG_JA
            /// <summary>開始処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Start processing</summary>
            /// @endif
            protected override void OnStart()
            {
                ImageAsset currentImage = GetCurrentSceneRenderedImage();

                this.currentSprt = new UISprite(1);
                this.currentSprt.ShaderType = ShaderType.Texture;
                this.currentSprt.BlendMode = BlendMode.Premultiplied;
                this.currentSprt.Image = currentImage;

                UISpriteUnit unit = this.currentSprt.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;
                unit.X = -UISystem.FramebufferWidth / 2.0f;
                unit.Y = -UISystem.FramebufferHeight / 2.0f;
                this.currentSprt.X = UISystem.FramebufferWidth / 2.0f;
                this.currentSprt.Y = UISystem.FramebufferHeight / 2.0f;
                TransitionUIElement.AddChildLast(this.currentSprt);

                this.pivotOffsetX = UISystem.FramebufferWidth / 2.0f;
                this.pivotOffsetY = UISystem.FramebufferHeight / 2.0f;

                this.fromX = 0.0f;
                this.fromY = 0.0f;
                this.outsideMargin = (float)Math.Sqrt(pivotOffsetX * pivotOffsetX + pivotOffsetY * pivotOffsetY);

                this.targetRadianSpeed = 0.0f;
                this.targetRadian = 0.0f;
                this.targetOffsetXSpeed = 0.0f;
                this.targetOffsetX = 0.0f;
                this.targetOffsetYSpeed = 0.0f;
                this.targetOffsetY = 0.0f;

                this.T = 40.0f;
                this.leftTime = T / (14.0f + 4.0f * (float)rand.NextDouble());

                setupLeftPos();
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

                CalcPosition(TotalElapsedTime / timeStep * speed, elapsedTime / timeStep * speed);

                this.currentSprt.X = targetOffsetX + fromX;
                this.currentSprt.Y = targetOffsetY + fromY;
                CalcMatrix();

                if (this.currentSprt.X < -outsideMargin || this.currentSprt.X > UISystem.FramebufferWidth + outsideMargin ||
                    this.currentSprt.Y < -outsideMargin || this.currentSprt.Y > UISystem.FramebufferHeight + outsideMargin)
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
                if (currentSprt != null)
                {
                    currentSprt.Image.Dispose();
                    currentSprt.Dispose();
                }
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
            /// <summary>落とす速度を取得・設定する。1.0 が標準速度。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the deceleration speed 1.0 is the standard speed.</summary>
            /// @endif
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }

            private void setupLeftPos()
            {
                float rad = (float)Math.Atan2(UISystem.FramebufferHeight, UISystem.FramebufferWidth);

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
                Matrix4 matA = Matrix4.Translation(new Vector3(pivotOffsetX + targetOffsetX, pivotOffsetY + targetOffsetY, 0.0f));

                Matrix4 matB;
                Matrix4 mat3 = Matrix4.RotationZ(targetRadian);
                matA.Multiply(ref mat3, out matB);

                currentSprt.Transform3D = matB;
            }


            private RotateDirection rotateDirection;
            private float speed;

            private Random rand;

            private UISprite currentSprt;

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

            LeftPosition leftPosition;
            float leftOffsetX;
            float leftOffsetY;

        }


    }
}
