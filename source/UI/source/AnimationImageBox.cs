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
        /// <summary>画像切り替えによるアニメーションの表示ウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Animation display widgets by image switching</summary>
        /// @endif
        public class AnimationImageBox : Widget
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public AnimationImageBox()
            {
                sprt = new UISprite(1);
                RootUIElement.AddChildLast(sprt);
                sprt.ShaderType = ShaderType.Texture;

                frameWidth = 0;
                frameHeight = 0;

                frameCount = 0;
                frameIndex = 0;

                frameInterval = 33.3f;
                animateElapsedTime = 0.0f;

                Focusable = false;
            }


            /// @if LANG_JA
            /// <summary>１フレームあたりの画像の幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the image width per frame.</summary>
            /// @endif
            public int FrameWidth
            {
                get
                {
                    return frameWidth;
                }
                set
                {
                    Stop();
                    frameWidth = value;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>１フレームあたりの画像の高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the image height per frame.</summary>
            /// @endif
            public int FrameHeight
            {
                get
                {
                    return frameHeight;
                }
                set
                {
                    Stop();
                    frameHeight = value;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>フレームの総数を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the total number of frames.</summary>
            /// @endif
            public int FrameCount
            {
                get
                {
                    return frameCount;
                }
                set
                {
                    Stop();
                    frameCount = (value > 0) ? value : 0;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>フレーム更新間隔を取得・設定する。（ミリ秒）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the frame refresh interval. (ms)</summary>
            /// @endif
            public float FrameInterval
            {
                get
                {
                    return frameInterval;
                }
                set
                {
                    Stop();
                    frameInterval = (value != 0) ? value : 1.0f;
                }
            }

            /// @if LANG_JA
            /// <summary>画像を取得・設定する。</summary>
            /// <remarks>画像はフレーム順に「左から右へ」「上から下へ」並べて指定する。２次元で並べる場合は横方向優先とする。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the image.</summary>
            /// <remarks>Sorts and specifies images in the frame order "from left to right" and "from top to bottom". When sorting in two dimensions, priority is given to the horizontal direction.</remarks>
            /// @endif
            public ImageAsset Image
            {
                get
                {
                    return sprt.Image;
                }
                set
                {
                    Stop();
                    sprt.Image = value;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>アニメーションを開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the animation.</summary>
            /// @endif
            public void Start()
            {
                animation = true;
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the animation.</summary>
            /// @endif
            public void Stop()
            {
                animation = false;
            }


            /// @if LANG_JA
            /// <summary>更新処理</summary>
            /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Update processing</summary>
            /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
            /// @endif
            protected override void OnUpdate(float elapsedTime)
            {
                base.OnUpdate(elapsedTime);

                if (animation)
                {
                    this.animateElapsedTime += elapsedTime;

                    if (this.animateElapsedTime >= this.frameInterval)
                    {
                        int elapsedFrame = (int)(this.animateElapsedTime / this.frameInterval);
                        int newFrameIndex = this.frameIndex + elapsedFrame;

                        this.animateElapsedTime -= (this.frameInterval * elapsedFrame);

                        newFrameIndex -= (newFrameIndex / frameCount) * frameCount;

                        if (newFrameIndex != this.frameIndex)
                        {
                            this.frameIndex = newFrameIndex;
                            needUpdateSprite = true;
                        }
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (needUpdateSprite)
                {
                    UpdateUISpriteBeforeRender();
                }
                base.Render();
            }

            private void UpdateUISpriteBeforeRender()
            {
                if (frameWidth > 0 && frameHeight > 0 && frameCount > 0 && sprt.Image != null)
                {
                    if (sprt.Image.Ready)
                    {
                        sprt.Visible = true;

                        int unitCol = sprt.Image.Width / frameWidth;

                        UISpriteUnit unit = sprt.GetUnit(0);
                        unit.X = 0;
                        unit.Y = 0;
                        unit.Width = frameWidth;
                        unit.Height = frameHeight;
                        unit.U1 = (frameWidth * (frameIndex % unitCol)) / (float)sprt.Image.Width;
                        unit.V1 = (frameHeight * (frameIndex / unitCol)) / (float)sprt.Image.Height;
                        unit.U2 = (frameWidth * (frameIndex % unitCol) + frameWidth) / (float)sprt.Image.Width;
                        unit.V2 = (frameHeight * (frameIndex / unitCol) + frameHeight) / (float)sprt.Image.Height;

                        needUpdateSprite = false;
                    }
                }
                else
                {
                    sprt.Visible = false;
                    needUpdateSprite = false;
                }
            }


            private UISprite sprt;

            private int frameWidth;
            private int frameHeight;

            private int frameCount;
            private int frameIndex;

            private float frameInterval;
            private float animateElapsedTime;
            private bool animation = false;

            private bool needUpdateSprite = true;
        }


    }
}
