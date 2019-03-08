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
        /// <summary>FlipBoardEffect用の補間関数の種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of interpolation function for FlipBoardEffect</summary>
        /// @endif
        public enum FlipBoardEffectInterpolator
        {
            /// @if LANG_JA
            /// <summary>線形補間</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Linear interpolation</summary>
            /// @endif
            Default = 0,

            /// @if LANG_JA
            /// <summary>カスタム</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Custom</summary>
            /// @endif
            Custom,
        }


        /// @if LANG_JA
        /// <summary>真ん中で折れるエフェクト</summary>
        /// <remarks>裏に新しいウィジェットが現れる。空港の掲示板風表現</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect that folds in the middle</summary>
        /// <remarks>A new widget appears in the back. Airport bulletin board-like expression</remarks>
        /// @endif
        public class FlipBoardEffect : Effect
        {
            private float time = 720;
            private AnimationInterpolator interpolatorCallback;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public FlipBoardEffect()
            {
                Widget = null;
                NextWidget = null;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="currentWidget">表示中のウィジェット</param>
            /// <param name="nextWidget">次に表示するウィジェット</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="currentWidget">Widget being displayed</param>
            /// <param name="nextWidget">Next widget to be displayed</param>
            /// @endif
            public FlipBoardEffect(Widget currentWidget, Widget nextWidget)
            {
                Widget = currentWidget;
                NextWidget = nextWidget;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="currentWidget">表示中のウィジェット</param>
            /// <param name="nextWidget">次に表示するウィジェット</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="currentWidget">Widget being displayed</param>
            /// <param name="nextWidget">Next widget to be displayed</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static FlipBoardEffect CreateAndStart(Widget currentWidget, Widget nextWidget)
            {
                FlipBoardEffect effect = new FlipBoardEffect(currentWidget, nextWidget);
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
                if (this.Interpolator == FlipBoardEffectInterpolator.Custom && this.CustomInterpolator != null)
                {
                    this.interpolatorCallback = this.CustomInterpolator;
                }
                else
                {
                    this.interpolatorCallback = AnimationUtility.FlipBounceInterpolator;
                }

                int currentTexWidth, currentTexHeight, nextTexWidth, nextTexHeight;

                currentTexWidth = (int)(Widget.Width * UISystem.PixelDensity);
                currentTexHeight = (int)(Widget.Height * UISystem.PixelDensity);
                nextTexWidth = (int)(NextWidget.Width * UISystem.PixelDensity);
                nextTexHeight = (int)(NextWidget.Height * UISystem.PixelDensity);

                if (!UISystem.CheckTextureSizeCapacity(currentTexWidth, currentTexHeight)
                    || !UISystem.CheckTextureSizeCapacity(nextTexWidth, nextTexHeight))
                {
                    throw new ArgumentOutOfRangeException();
                }
                // widget to texture
                Texture2D currentTexture = new Texture2D(currentTexWidth, currentTexHeight, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
                Widget.RenderToTexture(currentTexture);
                ImageAsset currentImageAsset = new ImageAsset(currentTexture, true);
                currentTexture.Dispose();
                Texture2D nextTexture = new Texture2D(nextTexWidth, nextTexHeight, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
                NextWidget.RenderToTexture(nextTexture);
                ImageAsset nextImageAsset = new ImageAsset(nextTexture, true);
                nextTexture.Dispose();


                // divide texture
                float currentDivideWidth = Widget.Width;
                float currentDivideHeight = Widget.Height / 2.0f;
                float nextDivideWidth = NextWidget.Width;
                float nextDivideHeight = NextWidget.Height / 2.0f;

                var orgPivot = Widget.PivotType;
                Widget.PivotType = PivotType.MiddleCenter;
                centerPosition = new Vector2(Widget.X, Widget.Y);
                Widget.PivotType = orgPivot;
                orgPivot = NextWidget.PivotType;
                NextWidget.PivotType = PivotType.MiddleCenter;
                NextWidget.Transform3D = Matrix4.Translation(centerPosition.X, centerPosition.Y, 0);
                NextWidget.PivotType = orgPivot;


                currentUpperSprt = new UISprite(1);
                currentUpperSprt.X = centerPosition.X;
                currentUpperSprt.Y = centerPosition.Y - currentDivideHeight * 0.5f;
                currentUpperSprt.Image = currentImageAsset;
                currentUpperSprt.ShaderType = ShaderType.Texture;
                currentUpperSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit currentUpperUnit = currentUpperSprt.GetUnit(0);
                currentUpperUnit.X = -currentDivideWidth * 0.5f;
                currentUpperUnit.Y = -currentDivideHeight * 0.5f;
                currentUpperUnit.Width = currentDivideWidth;
                currentUpperUnit.Height = currentDivideHeight;
                currentUpperUnit.V2 = 0.5f;

                currentLowerSprt = new UISprite(1);
                currentLowerSprt.X = centerPosition.X;
                currentLowerSprt.Y = centerPosition.Y + currentDivideHeight * 0.5f;
                currentLowerSprt.Image = currentImageAsset;
                currentLowerSprt.ShaderType = ShaderType.Texture;
                currentLowerSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit currentLowerUnit = currentLowerSprt.GetUnit(0);
                currentLowerUnit.X = -currentDivideWidth * 0.5f;
                currentLowerUnit.Y = -currentDivideHeight * 0.5f;
                currentLowerUnit.Width = currentDivideWidth;
                currentLowerUnit.Height = currentDivideHeight;
                currentLowerUnit.V1 = 0.5f;

                nextUpperSprt = new UISprite(1);
                nextUpperSprt.X = centerPosition.X;
                nextUpperSprt.Y = centerPosition.Y - nextDivideHeight * 0.5f;
                nextUpperSprt.Image = nextImageAsset;
                nextUpperSprt.ShaderType = ShaderType.Texture;
                nextUpperSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit nextUpperUnit = nextUpperSprt.GetUnit(0);
                nextUpperUnit.X = -nextDivideWidth * 0.5f;
                nextUpperUnit.Y = -nextDivideHeight * 0.5f;
                nextUpperUnit.Width = nextDivideWidth;
                nextUpperUnit.Height = nextDivideHeight;
                nextUpperUnit.V2 = 0.5f;

                nextLowerSprt = new UISprite(1);
                nextLowerSprt.X = centerPosition.X;
                nextLowerSprt.Y = centerPosition.Y + nextDivideHeight * 0.5f;
                nextLowerSprt.Image = nextImageAsset;
                nextLowerSprt.ShaderType = ShaderType.Texture;
                nextLowerSprt.BlendMode = BlendMode.Premultiplied;
                UISpriteUnit nextLowerUnit = nextLowerSprt.GetUnit(0);
                nextLowerUnit.X = -nextDivideWidth * 0.5f;
                nextLowerUnit.Y = -nextDivideHeight * 0.5f;
                nextLowerUnit.Width = nextDivideWidth;
                nextLowerUnit.Height = nextDivideHeight;
                nextLowerUnit.V1 = 0.5f;

                Widget.Parent.RootUIElement.AddChildLast(currentLowerSprt);
                NextWidget.Parent.RootUIElement.AddChildLast(nextUpperSprt);
                Widget.Parent.RootUIElement.AddChildLast(currentUpperSprt);
                NextWidget.Parent.RootUIElement.AddChildLast(nextLowerSprt);

                degree = 0.0f;
                turnupTime = this.Time;
                secondZOffset = currentDivideHeight / 4f;

                Widget.Visible = false;
                NextWidget.Visible = false;

                currentUpperSprt.Culling = true;
                nextLowerSprt.Culling = true;
                
                currentUpperSprt.ZSort = true;
                currentLowerSprt.ZSort = true;
                nextUpperSprt.ZSort = true;
                nextLowerSprt.ZSort = true;

                currentUpperSprt.ZSortOffset = -secondZOffset - firstZOffset;
                currentLowerSprt.ZSortOffset = 0.0f;
                nextUpperSprt.ZSortOffset = -firstZOffset;
                nextLowerSprt.ZSortOffset = -secondZOffset - firstZOffset;


                this.RotateNextLowerSprite();
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
                float previousDegree = degree;
                degree = this.interpolatorCallback(0, 180, TotalElapsedTime / Time);
                degree = FMath.Clamp(degree, 0.0f, 180.0f);

                if (degree < previousDegree && turnupTime > TotalElapsedTime)
                {
                    turnupTime = TotalElapsedTime;
                }

                UpdateWidgetVisible();


                if (TotalElapsedTime >= time)
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
                NextWidget.Visible = true;
                Widget.Visible = false;

                if (currentUpperSprt != null)
                {
                    currentUpperSprt.Image.Dispose();
                    currentUpperSprt.Dispose();
                    currentUpperSprt = null;
                }
                if (currentLowerSprt != null)
                {
                    currentLowerSprt.Image.Dispose();
                    currentLowerSprt.Dispose();
                    currentLowerSprt = null;
                }
                if (nextUpperSprt != null)
                {
                    nextUpperSprt.Image.Dispose();
                    nextUpperSprt.Dispose();
                    nextUpperSprt = null;
                }
                if (nextLowerSprt != null)
                {
                    nextLowerSprt.Image.Dispose();
                    nextLowerSprt.Dispose();
                    nextLowerSprt = null;
                }
            }
            
            private void UpdateWidgetVisible()
            {
                RotateCurrentUpperSprite();
                RotateNextLowerSprite();
                
                currentLowerSprt.ZSortOffset = -TotalElapsedTime / Time * firstZOffset;
                nextUpperSprt.ZSortOffset = -firstZOffset - currentLowerSprt.ZSortOffset;

                if (TotalElapsedTime > turnupTime)
                {
                    nextLowerSprt.ZSortOffset = -nextLowerSprt.Transform3D.M43 - ((Time - TotalElapsedTime) / (Time - turnupTime) * secondZOffset + firstZOffset);
                }
            }

            /// @if LANG_JA
            /// <summary>次に表示するウィジェットを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the next widget to be displayed.</summary>
            /// @endif
            public Widget NextWidget
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
            public FlipBoardEffectInterpolator Interpolator
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

            private void RotateCurrentUpperSprite()
            {
                float radian = degree / 360.0f * 2.0f * (float)Math.PI;
                UISpriteUnit unit = currentUpperSprt.GetUnit(0);

                Matrix4 trans1 = Matrix4.Translation(0.0f, -unit.Height * 0.5f, 0.0f);
                Matrix4 rotate = Matrix4.RotationX(radian);
                Matrix4 trans2 = Matrix4.Translation(centerPosition.X, centerPosition.Y, 0.0f);

                currentUpperSprt.Transform3D = trans2 * rotate * trans1;
            }

            private void RotateNextLowerSprite()
            {
                float radian = (degree - 180.0f) / 360.0f * 2.0f * (float)Math.PI;
                UISpriteUnit unit = nextLowerSprt.GetUnit(0);

                Matrix4 trans1 = Matrix4.Translation(0.0f, unit.Height * 0.5f, 0.0f);
                Matrix4 rotate = Matrix4.RotationX(radian);
                Matrix4 trans2 = Matrix4.Translation(centerPosition.X, centerPosition.Y, 0.0f);

                nextLowerSprt.Transform3D = trans2 * rotate * trans1;
            }

            private UISprite currentUpperSprt;
            private UISprite currentLowerSprt;
            private UISprite nextUpperSprt;
            private UISprite nextLowerSprt;
            private float degree;
            private Vector2 centerPosition;
            private float firstZOffset= 0.001f;
            private float secondZOffset;
            private float turnupTime;
        }


    }
}
