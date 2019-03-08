/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;

namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        internal class DefaultNavigationTransition : Transition
        {
            internal enum TransitionType
            {
                Push,
                Pop
            }

            private static readonly float animationTime = 500.0f;
            private static readonly float minAlpha = 0.0f;
            private static readonly float maxAlpha = 1.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public DefaultNavigationTransition(TransitionType type)
            {
                transitionType = type;
                DrawOrder = TransitionDrawOrder.TransitionUIElement;

                float leftPosX = -UISystem.FramebufferWidth * 0.121f;
                float leftPosZ = UISystem.FramebufferWidth * 0.156f;
                float leftDegree = - (float)Math.PI / 2.0f;

                float rightPosX = UISystem.FramebufferWidth;
                float rightPosZ = -UISystem.FramebufferWidth * 0.344f;
                float rightDegree = (float)Math.PI / 2.0f;

                if (transitionType == TransitionType.Push)
                {
                    fromPosXCurrent = 0.0f;
                    toPosXCurrent = leftPosX;
                    fromPosZCurrent = 0.0f;
                    toPosZCurrent = leftPosZ;
                    fromDegreeCurrent = 0.0f;
                    toDegreeCurrent = leftDegree;

                    fromPosXNext = rightPosX;
                    toPosXNext = 0.0f;
                    fromPosZNext = rightPosZ;
                    toPosZNext = 0.0f;
                    fromDegreeNext = rightDegree;
                    toDegreeNext = 0.0f;
                }
                else
                {
                    fromPosXCurrent = 0.0f;
                    toPosXCurrent = rightPosX;
                    fromPosZCurrent = 0.0f;
                    toPosZCurrent = rightPosZ;
                    fromDegreeCurrent = 0.0f;
                    toDegreeCurrent = rightDegree;

                    fromPosXNext = leftPosX;
                    toPosXNext = 0.0f;
                    fromPosZNext = leftPosZ;
                    toPosZNext = 0.0f;
                    fromDegreeNext = leftDegree;
                    toDegreeNext = 0.0f;
                }
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

                this.sprtCurrent = new UISprite(1);
                this.sprtCurrent.Image = currentImage;
                this.sprtCurrent.ShaderType = ShaderType.Texture;
                this.sprtCurrent.BlendMode = BlendMode.Premultiplied;
                this.sprtCurrent.Alpha = maxAlpha;
                this.sprtCurrent.Visible = true;
                TransitionUIElement.AddChildLast(this.sprtCurrent);

                UISpriteUnit unit = this.sprtCurrent.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;

                // NextScene
                ImageAsset nextImage = GetNextSceneRenderedImage();

                this.sprtNext = new UISprite(1);
                this.sprtNext.Image = nextImage;
                this.sprtNext.ShaderType = ShaderType.Texture;
                this.sprtNext.BlendMode = BlendMode.Premultiplied;
                this.sprtNext.Alpha = minAlpha;
                this.sprtNext.Visible = true;
                TransitionUIElement.AddChildLast(this.sprtNext);

                unit = this.sprtNext.GetUnit(0);
                unit.Width = UISystem.FramebufferWidth;
                unit.Height = UISystem.FramebufferHeight;
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
                if (TotalElapsedTime >= animationTime)
                {
                    this.sprtCurrent.Transform3D = GetTransform3D(toPosXCurrent, toPosZCurrent, toDegreeCurrent);
                    this.sprtNext.Transform3D = GetTransform3D(toPosXNext, toPosZNext, toDegreeNext);

                    this.sprtCurrent.Visible = false;
                    this.sprtNext.Visible = true;

                    return TransitionUpdateResponse.Finish;
                }
                else
                {
                    float transXCurrent = AnimationUtility.EaseOutQuartInterpolator(fromPosXCurrent, toPosXCurrent, TotalElapsedTime / animationTime);
                    float transZCurrent = AnimationUtility.EaseOutQuartInterpolator(fromPosZCurrent, toPosZCurrent, TotalElapsedTime / animationTime);
                    float degreeCurrent = AnimationUtility.EaseOutQuartInterpolator(fromDegreeCurrent, toDegreeCurrent, TotalElapsedTime / animationTime);
                    float alphaCurrent = AnimationUtility.LinearInterpolator(maxAlpha, minAlpha, TotalElapsedTime / animationTime);

                    this.sprtCurrent.Transform3D = GetTransform3D(transXCurrent, transZCurrent, degreeCurrent);
                    this.sprtCurrent.Alpha = alphaCurrent;

                    float transXNext = AnimationUtility.EaseOutQuartInterpolator(fromPosXNext, toPosXNext, TotalElapsedTime / animationTime);
                    float transZNext = AnimationUtility.EaseOutQuartInterpolator(fromPosZNext, toPosZNext, TotalElapsedTime / animationTime);
                    float degreeNext = AnimationUtility.EaseOutQuartInterpolator(fromDegreeNext, toDegreeNext, TotalElapsedTime / animationTime);
                    float alphaNext = AnimationUtility.LinearInterpolator(minAlpha, maxAlpha, TotalElapsedTime / animationTime);

                    this.sprtNext.Transform3D = GetTransform3D(transXNext, transZNext, degreeNext);
                    this.sprtNext.Alpha = alphaNext;

                    return TransitionUpdateResponse.Continue;
                }
            }

            private Matrix4 GetTransform3D(float transX, float transZ, float degree)
            {
                Matrix4 matA;
                Matrix4 mat1 = Matrix4.Translation(new Vector3(transX, 0.0f, transZ));
                Matrix4 mat2 = Matrix4.RotationY(degree);
                mat1.Multiply(ref mat2, out matA);

                return matA;
            }

            /// @if LANG_JA
            /// <summary>停止処理</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stop processing</summary>
            /// @endif
            protected override void OnStop()
            {
                if (this.sprtCurrent != null)
                {
                    this.sprtCurrent.Image.Dispose();
                    this.sprtCurrent.Dispose();
                }
                if (this.sprtNext != null)
                {
                    this.sprtNext.Image.Dispose();
                    this.sprtNext.Dispose();
                }
            }

            private TransitionType transitionType;

            private UISprite sprtCurrent;
            private float fromPosXCurrent;
            private float toPosXCurrent;
            private float fromPosZCurrent;
            private float toPosZCurrent;
            private float fromDegreeCurrent;
            private float toDegreeCurrent;

            private UISprite sprtNext;
            private float fromPosXNext;
            private float toPosXNext;
            private float fromPosZNext;
            private float toPosZNext;
            private float fromDegreeNext;
            private float toDegreeNext;
        }
    }
}
