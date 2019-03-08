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
        /// <summary>回転の速度</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Rotation speed</summary>
        /// @endif
        public enum JumpFlipTransitionSpeed
        {
            /// @if LANG_JA
            /// <summary>高速</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>High speed</summary>
            /// @endif
            Fast = 0,

            /// @if LANG_JA
            /// <summary>低速</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Low speed</summary>
            /// @endif
            Slow
        }


        /// @if LANG_JA
        /// <summary>JumpFlipの回転方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>JumpFlip rotation direction</summary>
        /// @endif
        public enum JumpFlipTransitionRotateDirection
        {
            /// @if LANG_JA
            /// <summary>時計回り</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Clock-wise</summary>
            /// @endif
            ClockWise = 0,

            /// @if LANG_JA
            /// <summary>反時計回り</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Counter clock-wise</summary>
            /// @endif
            CounterClockWise
        }


        /// @if LANG_JA
        /// <summary>画面手前にジャンプし反転して切り替わるトランジション</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Transition that jumps forward, flips, and then changes</summary>
        /// @endif
        public class JumpFlipTransition : Transition
        {

            const float fastTime = 500.0f;
            const float slowTime = 1000.0f;

            const float fromPosZ = 0.0f;
            const float toPosZ = -600.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public JumpFlipTransition()
            {
                Speed = JumpFlipTransitionSpeed.Fast;
                RotateDirection = JumpFlipTransitionRotateDirection.ClockWise;
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
                // CurrentScene
                ImageAsset currentImage = GetCurrentSceneRenderedImage();

                this.currentSprt = new UISprite(1);
                this.currentSprt.Image = currentImage;
                this.currentSprt.ShaderType = ShaderType.Texture;
                this.currentSprt.BlendMode = BlendMode.Premultiplied;
                TransitionUIElement.AddChildLast(this.currentSprt);

                UISpriteUnit unit = this.currentSprt.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;

                // NextScene
                ImageAsset nextImage = GetNextSceneRenderedImage();

                this.nextSprt = new UISprite(1);
                this.nextSprt.Image = nextImage;
                this.nextSprt.ShaderType = ShaderType.Texture;
                this.nextSprt.BlendMode = BlendMode.Premultiplied;
                TransitionUIElement.AddChildLast(this.nextSprt);

                unit = this.nextSprt.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;

                this.currentSprt.Visible = true;
                this.nextSprt.Visible = false;

                this.time = (Speed == JumpFlipTransitionSpeed.Fast) ? fastTime : slowTime;

                this.fromDegree = 0.0f;
                this.toDegree = (RotateDirection == JumpFlipTransitionRotateDirection.ClockWise) ? 180.0f : -180.0f;
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
                if (TotalElapsedTime >= time)
                {
                    this.currentSprt.Transform3D = GetTransform3D(fromPosZ, 180.0f);
                    this.nextSprt.Transform3D = GetTransform3D(fromPosZ, 0.0f);

                    this.currentSprt.Visible = false;
                    this.nextSprt.Visible = true;

                    return TransitionUpdateResponse.Finish;
                }
                else
                {
                    float transZ = FMath.Lerp(fromPosZ, toPosZ * 2.0f, TotalElapsedTime / time);
                    float degree = FMath.Lerp(fromDegree, toDegree, TotalElapsedTime / time);

                    transZ = (transZ <= toPosZ) ? (toPosZ * 2.0f - transZ) : transZ;

                    if (this.RotateDirection == JumpFlipTransitionRotateDirection.ClockWise)
                    {
                        if (degree >= 90.0f)
                        {
                            this.currentSprt.Visible = false;
                            this.nextSprt.Visible = true;
                        }
                    }
                    else
                    {
                        if (degree <= -90.0f)
                        {
                            this.currentSprt.Visible = false;
                            this.nextSprt.Visible = true;
                        }
                    }

                    this.currentSprt.Transform3D = GetTransform3D(transZ, degree);
                    this.nextSprt.Transform3D = GetTransform3D(transZ, degree + 180.0f);

                    return TransitionUpdateResponse.Continue;
                }
            }

            private Matrix4 GetTransform3D(float transZ, float degree)
            {
                float centerX = UISystem.FramebufferWidth / 2.0f;
                float centerY = UISystem.FramebufferHeight / 2.0f;

                Matrix4 matA;
                Matrix4 mat1 = Matrix4.Translation(new Vector3(centerX, centerY, transZ));
                Matrix4 mat2 = Matrix4.RotationY(degree / 360.0f * 2.0f * (float)Math.PI);
                mat1.Multiply(ref mat2, out matA);

                Matrix4 matB;
                Matrix4 mat3 = Matrix4.Translation(new Vector3(-centerX, -centerY, 0.0f));
                matA.Multiply(ref mat3, out matB);

                return matB;
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
                if (this.currentSprt != null)
                {
                    this.currentSprt.Image.Dispose();
                    this.currentSprt.Dispose();
                    this.currentSprt = null;
                }
                if (this.nextSprt != null)
                {
                    this.nextSprt.Image.Dispose();
                    this.nextSprt.Dispose();
                    this.nextSprt = null;
                }
            }

            /// @if LANG_JA
            /// <summary>回転の速度を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the rotation speed.</summary>
            /// @endif
            public JumpFlipTransitionSpeed Speed
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>回転の方向を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the rotation direction.</summary>
            /// @endif
            public JumpFlipTransitionRotateDirection RotateDirection
            {
                get;
                set;
            }

            private UISprite currentSprt;
            private UISprite nextSprt;

            private float fromDegree;
            private float toDegree;

            private float time;

        }


    }
}
