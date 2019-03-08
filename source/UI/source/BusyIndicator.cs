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
        /// <summary>進行状況を表示するウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A widget used to display the progress status</summary>
        /// @endif
        public class BusyIndicator : Widget
        {

            private const int defaultBusyIndicatorWidth = 48;
            private const int defaultBusyIndicatorHeight = 48;

            private const int frameCount = 8;
            private const float frameInterval = 66.7f;
            private const float fadeAnimationTime = 200.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public BusyIndicator() : this(false)
            {
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="autoStart">初期化直後にアニメーションを開始するかどうか。初期値はFalse。</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="autoStart">Whether to start the animation immediately after initialization. Default value is False.</param>
            /// @endif
            public BusyIndicator(bool autoStart)
            {
                base.Width = defaultBusyIndicatorWidth;
                base.Height = defaultBusyIndicatorHeight;

                image = new AnimationImageBox();
                image.Image = new ImageAsset(SystemImageAsset.BusyIndicator);
                image.FrameWidth = defaultBusyIndicatorWidth;
                image.FrameHeight = defaultBusyIndicatorHeight;
                image.FrameCount = frameCount;
                image.FrameInterval = frameInterval;
                AddChildLast(image);

                fadeInEffect = new FadeInEffect(image, fadeAnimationTime, FadeInEffectInterpolator.Linear);
                fadeOutEffect = new FadeOutEffect(image, fadeAnimationTime, FadeOutEffectInterpolator.Linear);
                fadeOutEffect.EffectStopped += new EventHandler<EventArgs>(fadeOutEffect_EffectStopped);

                Focusable = false;

                if (autoStart)
                {
                    image.Visible = true;
                    image.Start();
                }
                else
                {
                    image.Visible = false;
                    image.Stop();
                }
            }

            void fadeOutEffect_EffectStopped(object sender, EventArgs e)
            {
                image.Stop();
                image.Visible = false;
            }

            /// @if LANG_JA
            /// <summary>幅を取得する。</summary>
            /// <remarks>設定された値は無視される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the width.</summary>
            /// <remarks>The set value is ignored.</remarks>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得する。</summary>
            /// <remarks>設定された値は無視される。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the height.</summary>
            /// <remarks>The set value is ignored.</remarks>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                }
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (fadeInEffect != null)
                {
                    fadeInEffect.Stop();
                }
                if (fadeOutEffect != null)
                {
                    fadeOutEffect.Stop();
                }

                if (image != null && image.Image != null)
                {
                    image.Image.Dispose();
                }

                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>アニメーションを開始する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Starts the animation.</summary>
            /// @endif
            public void Start()
            {
                fadeOutEffect.Stop();
                fadeInEffect.Start();
                image.Visible = true;
                image.Start();
            }

            /// @if LANG_JA
            /// <summary>アニメーションを停止する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Stops the animation.</summary>
            /// @endif
            public void Stop()
            {
                fadeInEffect.Stop();
                fadeOutEffect.Start();
            }

            private AnimationImageBox image;
            private FadeInEffect fadeInEffect;
            private FadeOutEffect fadeOutEffect;

        }


    }
}
