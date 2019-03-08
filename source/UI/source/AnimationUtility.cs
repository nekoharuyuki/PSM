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
        /// <summary>Effect、Transition用アニメーション補間関数のデリゲート</summary>
        /// <remarks>開始値(from)から終了値(to)までを割合(ratio)を用いて補間する。戻り値は、ratio = 0のときfrom、ratio = 1のときtoである必要がある。ただし、0 &lt; ratio &lt; 1では戻り値が必ずしもfromからtoの間に収まっている必要はない。</remarks>
        /// <param name="from">開始値</param>
        /// <param name="to">終了値</param>
        /// <param name="ratio">割合（０～１）</param>
        /// <returns>補間結果</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Delegate the animation interpolation function for Effect and Transition</summary>
        /// <remarks>Interpolate using the ratio from the start value (from) to the end value (to). The return value must be from when ratio = 0 and to when ratio = 1. However, when 0 < ratio < 1, the return value does not have to be between from and to.</remarks>
        /// <param name="from">Start value</param>
        /// <param name="to">End value</param>
        /// <param name="ratio">Ratio (0 to 1)</param>
        /// <returns>Interpolation result</returns>
        /// @endif
        public delegate float AnimationInterpolator(float from, float to, float ratio);

        /// @if LANG_JA
        /// <summary>アニメーション用のユーティリティ</summary>
        /// <remarks>EffectおよびTransitionに使用できるアニメーション補間関数</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Utility for animation</summary>
        /// <remarks>Animation interpolation function usable for Effect and Transition</remarks>
        /// @endif
        public static class AnimationUtility
        {
            /// @if LANG_JA
            /// <summary>線形補間関数</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Linear interpolation function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float LinearInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (to - from) * ratio + from;
            }

            /// @if LANG_JA
            /// <summary>二次関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using quadratic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInQuadInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (to - from) * ratio * ratio + from;
            }

            /// @if LANG_JA
            /// <summary>二次関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using quadratic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutQuadInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return -(to - from) * ratio * (ratio - 2) + from;
            }

            /// @if LANG_JA
            /// <summary>二次関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using quadratic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutQuadInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInQuadInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutQuadInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>二次関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using quadratic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInQuadInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutQuadInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInQuadInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>三次関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using cubic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInCubicInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (to - from) * ratio * ratio * ratio + from;
            }

            /// @if LANG_JA
            /// <summary>三次関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using cubic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutCubicInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                ratio -= 1;
                return (to - from) * ratio * ratio * ratio + to;
            }

            /// @if LANG_JA
            /// <summary>三次関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using cubic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutCubicInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInCubicInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutCubicInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>三次関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using cubic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInCubicInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutCubicInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInCubicInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>四次関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using quartic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInQuartInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                ratio *= ratio;
                return (to - from) * ratio * ratio + from;
            }

            /// @if LANG_JA
            /// <summary>四次関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using quartic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutQuartInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                ratio -= 1;
                ratio *= ratio;
                return (from - to) * ratio * ratio + to;
            }

            /// @if LANG_JA
            /// <summary>四次関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using quartic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutQuartInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInQuartInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutQuartInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>四次関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using quartic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInQuartInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutQuartInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInQuartInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>五次関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using quintic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInQuintInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                float ratio2 = ratio * ratio;
                return (to - from) * ratio2 * ratio2 * ratio + from;
            }

            /// @if LANG_JA
            /// <summary>五次関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合(0.0f～1.0f)</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using quintic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0.0f to 1.0f)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutQuintInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                ratio -= 1;
                float ratio2 = ratio * ratio;
                return (to - from) * ratio2 * ratio2 * ratio + to;
            }

            /// @if LANG_JA
            /// <summary>五次関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using quintic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutQuintInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInQuintInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutQuintInterpolator((to + from) / 2, to, (ratio * 2) - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>五次関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using quintic function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInQuintInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutQuintInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInQuintInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>サイン関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using sine function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInSineInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (float)(-(to - from) * Math.Cos(ratio * Math.PI / 2) + to);
            }

            /// @if LANG_JA
            /// <summary>サイン関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using sine function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutSineInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (float)((to - from) * Math.Sin(ratio * (Math.PI / 2)) + from);
            }

            /// @if LANG_JA
            /// <summary>サイン関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using sine function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutSineInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (float)(-(to - from) / 2 * (Math.Cos(Math.PI * ratio) - 1) + from);
            }

            /// @if LANG_JA
            /// <summary>サイン関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using sine function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInSineInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutSineInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInSineInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>累乗関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using power function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInExpoInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (ratio <= 0) ? from : (to - from) * (float)(Math.Pow(2, 10 * (ratio - 1)) - 0.001f) + from;
            }

            /// @if LANG_JA
            /// <summary>累乗関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using power function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutExpoInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (ratio >= 1) ? to : (to - from) * 1.001f * (1 - (float)Math.Pow(2, -10 * ratio)) + from;
            }

            /// @if LANG_JA
            /// <summary>累乗関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using power function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutExpoInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInExpoInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutExpoInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>累乗関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using power function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInExpoInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutExpoInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInExpoInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>円関数によるイーズイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in interpolation using circular function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInCircInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (float)(-(to - from) * (Math.Sqrt(1 - ratio * ratio) - 1) + from);
            }

            /// @if LANG_JA
            /// <summary>円関数によるイーズアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out interpolation using circular function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutCircInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (float)((to - from) * Math.Sqrt(1 - (ratio = ratio - 1) * ratio) + from);
            }

            /// @if LANG_JA
            /// <summary>円関数によるイーズインアウト補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-in/out interpolation using circular function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseInOutCircInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseInCircInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseOutCircInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>円関数によるイーズアウトイン補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Ease-out/in interpolation using circular function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float EaseOutInCircInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 0.5f)
                {
                    return EaseOutCircInterpolator(from, (to + from) / 2, ratio * 2);
                }
                else
                {
                    return EaseInCircInterpolator((to + from) / 2, to, ratio * 2 - 1);
                }
            }

            /// @if LANG_JA
            /// <summary>弾性(Elastic)関数による補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Interpolation using elasticity (Elastic) function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float ElasticInterpolator(float from, float to, float ratio)
            {
                float p = .3f;
                float s = p / 4;
                float a = to - from;
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio == 0) return from;
                if (ratio == 1) return to;
                return (float)((a * Math.Pow(2, -10 * ratio) * Math.Sin((ratio - s) * (2 * Math.PI) / p) + to));
            }

            /// @if LANG_JA
            /// <summary>アンダーシュート関数による補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Interpolation using undershoot function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float UndershootInterpolator(float from, float to, float ratio)
            {
                float s = 1.70158f;
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (to - from) * ratio * ratio * ((s + 1) * ratio - s) + from;
            }

            /// @if LANG_JA
            /// <summary>オーバーシュート関数による補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Interpolation using overshoot function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float OvershootInterpolator(float from, float to, float ratio)
            {
                float s = 1.70158f;
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                return (to - from) * ((ratio -= 1) * ratio * ((s + 1) * ratio + s) + 1) + from;
            }

            /// @if LANG_JA
            /// <summary>跳ね返り(Bounce)関数による補間</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Interpolation using bounce-back (Bounce) function</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float BounceInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < (1 / 2.75))
                {
                    return (to - from) * (7.5625f * ratio * ratio) + from;
                }
                else if (ratio < (2f / 2.75f))
                {
                    return (to - from) * (7.5625f * (ratio -= (1.5f / 2.75f)) * ratio + .75f) + from;
                }
                else if (ratio < (2.5f / 2.75f))
                {
                    return (to - from) * (7.5625f * (ratio -= (2.25f / 2.75f)) * ratio + .9375f) + from;
                }
                else
                {
                    return (to - from) * (7.5625f * (ratio -= (2.625f / 2.75f)) * ratio + .984375f) + from;
                }
            }

            /// @if LANG_JA
            /// <summary>FlipBoardの標準的な補間関数</summary>
            /// <param name="from">開始値</param>
            /// <param name="to">終了値</param>
            /// <param name="ratio">割合（０～１）</param>
            /// <returns>補間結果</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Standard interpolation function of FlipBoard</summary>
            /// <param name="from">Start value</param>
            /// <param name="to">End value</param>
            /// <param name="ratio">Ratio (0 to 1)</param>
            /// <returns>Interpolation result</returns>
            /// @endif
            public static float FlipBounceInterpolator(float from, float to, float ratio)
            {
                ratio = FMath.Clamp(ratio, 0.0f, 1.0f);
                if (ratio < 1.0f / 3.0f)
                {
                    ratio *= 3;
                    return (to - from) * ratio * ratio + from;
                }
                else
                {
                    ratio = (ratio - (1.0f / 3.0f)) * 1.5f;
                    return to + (from - to) / 15f * ((1.0f - ratio) * (1.0f - (float)Math.Cos(ratio * (2f * 3f * Math.PI))));
                }
            }
   
            // Following interpolators are for UIMotion/UIAnimationPlayer
            private static Func<float, float> _linear = (x) => x;
            private static Func<float, float> _quadCurve = (x) => x * x;
            private static Func<float, float> _cubicCurve = (x) => x * x * x;
            private static Func<float, float> _quarticCurve = (x) => x * x * x * x;
            private static Func<float, float> _quinticCurve = (x) => x * x * x * x * x;
            
            private static float GetFunctionInterpolateRatio(int strength)
            {
                float a = -strength / 100.0f;
                return Math.Abs(FMath.Max(-1.0f, FMath.Min(1.0f, a)));
            }

            private static Func<float, float> InterpolateFunctions(Func<float, float> func1, Func<float, float> func2, float ratio)
            {
                return (x) => ratio * func1(x) + (1 - ratio) * func2(x);
            }
            
            private static Func<float, float> QuadCurve(float a, float b, float c)
            {
                return x => a * (x * x) + b * x + c;
            }
    
            private static Func<float, float> XShift(Func<float, float> f, float x_shift)
            {
                return x => f(x - x_shift);
            }
    
            private static AnimationInterpolator GetAnimationInterpolator(Func<float, float> ratioConverter)
            {
                return (from, to, ratio) => from + (to - from) * ratioConverter(ratio);
            }
    
            private static Func<float, float> GetRatioInputInverse(Func<float, float> origFunc)
            {
                return r => origFunc(1.0f - r);
            }
    
            private static Func<float, float> GetRatioOutputInverse(Func<float, float> origFunc)
            {
                return r => 1.0f - origFunc(r);
            }
    
            private static float GetBounceFirstDuration(int strength)
            {
                float sum = 0;
                for (int i = 1; i <= strength; i++)
                {
                    sum += 1.0f / i;
                }
                return 1.0f / sum;
            }
    
            private static int GetBounceCount(float T0, float ratio)
            {
                float d = T0;
                int count = 0;
                while (d < ratio)
                {
                    count++;
                    d += T0 / (count + 1);
                }
                return count;
            }
    
            private static float GetBounceOffset(int totalBound, int boundCount, float T0)
            {
                float offset = 0;
                for (int c = 0; c < boundCount; c++)
                {
                    offset += T0 / (c + 1);
                }
                return offset;
            }
            internal static AnimationInterpolator GetQuadInterpolator(int strength)
            {
                Func<float, float> rc = _quadCurve;

                if (strength > 0)
                {
                    rc = (r) => 1 - _quadCurve(r - 1);
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }

            internal static AnimationInterpolator GetCubicInterpolator(int strength)
            {
                Func<float, float> rc = _cubicCurve;

                if (strength > 0)
                {
                    rc = (r) => 1 + _cubicCurve(r - 1);
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }
            
            internal static AnimationInterpolator GetQuarticInterpolator(int strength)
            {
                Func<float, float> rc = _quarticCurve;

                if (strength > 0)
                {
                    rc = (r) => 1 - _quarticCurve(r - 1);
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }

            internal static AnimationInterpolator GetQuinticInterpolator(int strength)
            {
                Func<float, float> rc = _quinticCurve;

                if (strength > 0)
                {
                    rc = (r) => 1 + _quinticCurve(r - 1);
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }
            
            internal static AnimationInterpolator GetDualQuadInterpolator(int strength)
            {
                Func<float, float> rc;

                if (strength > 0)
                {
                    rc = (r) => r < 0.5f ?
                        0.5f - _quadCurve(2 * (r - 0.5f)) / 2
                        :
                        _quadCurve(2 * (r - 0.5f)) / 2 + 0.5f;
                }
                else
                {
                    rc = (r) => r < 0.5f ?
                        _quadCurve(2 * r) / 2
                        :
                        1.0f - _quadCurve(2 * (r - 1)) / 2;
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }

            internal static AnimationInterpolator GetDualCubicInterpolator(int strength)
            {
                Func<float, float> rc;

                if (strength > 0)
                {
                    rc = (r) => r < 0.5f ?
                        0.5f + _cubicCurve(2 * (r - 0.5f)) / 2
                        :
                        _cubicCurve(2 * (r - 0.5f)) / 2 + 0.5f;
                }
                else
                {
                    rc = (r) => r < 0.5f ?
                        _cubicCurve(2 * r) / 2
                        :
                        1.0f + _cubicCurve(2 * (r - 1)) / 2;
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }

            internal static AnimationInterpolator GetDualQuarticInterpolator(int strength)
            {
                Func<float, float> rc;

                if (strength > 0)
                {
                    rc = (r) => r < 0.5f ?
                        0.5f - _quarticCurve(2 * (r - 0.5f)) / 2
                        :
                        _quarticCurve(2 * (r - 0.5f)) / 2 + 0.5f;
                }
                else
                {
                    rc = (r) => r < 0.5f ?
                        _quarticCurve(2 * r) / 2
                        :
                        1.0f - _quarticCurve(2 * (r - 1)) / 2;
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }

            internal static AnimationInterpolator GetDualQuinticInterpolator(int strength)
            {
                Func<float, float> rc;

                if (strength > 0)
                {
                    rc = (r) => r < 0.5f ?
                        0.5f + _quinticCurve(2 * (r - 0.5f)) / 2
                        :
                        _quinticCurve(2 * (r - 0.5f)) / 2 + 0.5f;
                }
                else
                {
                    rc = (r) => r < 0.5f ?
                        _quinticCurve(2 * r) / 2
                        :
                        1.0f + _quinticCurve(2 * (r - 1)) / 2;
                }

                float a = GetFunctionInterpolateRatio(strength);

                return GetAnimationInterpolator(InterpolateFunctions(rc, _linear, a));
            }
            
            internal static AnimationInterpolator GetBounceInterpolator(int strength)
            {
                float T0 = GetBounceFirstDuration(Math.Abs(strength));
                var ratioConverter = GetBounceRatioConverter(Math.Abs(strength), T0);
                if (strength < 0)
                {
                    return GetAnimationInterpolator(GetRatioInputInverse(ratioConverter));
                }
                else
                {
                    return GetAnimationInterpolator(ratioConverter);
                }
            }
    
            private static Func<float, float> GetBounceRatioConverter(int strength, float T0)
            {
                return (ratio) =>
                {
                    int boundCount = GetBounceCount(T0, ratio);
                    var curve = SelectBounceCurve(strength, boundCount, T0);
    
                    return curve(ratio);
                };
            }
    
            private static Func<float, float> SelectBounceCurve(int totalBound, int boundCount, float T0)
            {
                float Tn = GetBounceFirstDuration(totalBound) / (boundCount + 1);
                float offset = GetBounceOffset(totalBound, boundCount, T0);
    
                float height = FMath.Exp(-boundCount);
                float a = -4.0f * height / Tn / Tn;
                float b = 4.0f * height / Tn;
                float c = 0;
    
                return XShift(QuadCurve(a, b, c), offset);
            }
            
            internal static AnimationInterpolator GetBounceInInterpolator(int strength)
            {
                float T0 = GetBounceFirstDuration(Math.Abs(strength));
                var ratioConverter = GetBounceInRatioConverter(Math.Abs(strength), T0);
    
                if (strength < 0)
                {
                    return GetAnimationInterpolator(GetRatioOutputInverse(GetRatioInputInverse(ratioConverter)));
                }
                else
                {
                    return GetAnimationInterpolator(ratioConverter);
                }
            }
    
            private static Func<float, float> GetBounceInRatioConverter(int strength, float T0)
            {
                return (ratio) =>
                {
                    int count = GetBounceCount(T0, ratio);
                    var curve = SelectBounceInCurve(strength, count, T0);
    
                    return curve(ratio);
                };
            }
    
            private static Func<float, float> SelectBounceInCurve(int totalBound, int boundCount, float T0)
            {
                float Tn = T0 / (boundCount + 1);
                float offset = GetBounceOffset(totalBound, boundCount, T0);
                float a = 0, b = 0, c = 0;
    
                if (boundCount == 0)
                {
                    a = 1.0f / Tn / Tn;
                    b = 0.0f;
                    c = 0.0f;
                }
                else
                {
                    float depth = FMath.Exp(-boundCount);
                    a = 4.0f * depth / Tn / Tn;
                    b = -4.0f * depth / Tn;
                    c = 1;
                }
                return XShift(QuadCurve(a, b, c), offset);
            }
            
            internal static AnimationInterpolator GetSpringInterpolator(int strength)
            {
                if (strength > 0)
                {
                    strength = Math.Min(100, strength);
                    float startupTime = 0.4f / strength;
                    float cycleTime = (1.0f - startupTime) / strength;

                    float angularFrequency = 2 * FMath.PI / cycleTime;

                    var ratioConverter = new Func<float, float>((t) =>
                        t < startupTime ?
                            FMath.Sin(t * FMath.PI / 2 / startupTime)
                        :
                            (0.7f + 0.3f * FMath.Exp(-(t - startupTime) / cycleTime) * FMath.Cos(angularFrequency * (t - startupTime))));

                    return GetAnimationInterpolator(ratioConverter);
                }
                else
                {
                    return AnimationUtility.LinearInterpolator;
                }
            }
            
            internal static AnimationInterpolator GetSineWaveInterpolator(int strength)
            {
                strength = (int)FMath.Clamp(strength, 0, 100);
                
                return GetAnimationInterpolator(
                            (ratio) =>
                            {
                                if (strength == 0)
                                {
                                    return ratio;
                                }
                                else
                                {
                                    return (1.0f + FMath.Sin(ratio * FMath.PI * strength - FMath.PI / 2.0f)) / 2.0f;
                                }
                            }
                        );
            }
            
            internal static AnimationInterpolator GetSawtoothWaveInterpolator(int strength)
            {
                strength = (int)FMath.Clamp(strength, 1, 100);
                
                return GetAnimationInterpolator(
                            (ratio) =>
                            {
                                float val = FMath.Repeat(ratio * strength, 0.0f, 2.0f);
                                if (1.0f < val)
                                {
                                    return 2.0f - val;
                                }
                                else
                                {
                                    return val;
                                }
                            }
                        );
            }
            
            internal static AnimationInterpolator GetSquareWaveInterpolator(int strength)
            {
                strength = (int)FMath.Clamp(strength, 2, 100);

                return GetAnimationInterpolator(
                            (ratio) =>
                            {
                                float val = FMath.Repeat(ratio * strength, 0.0f, 2.0f);
                                if (val <= 1.0f)
                                {
                                    return 0.0f;
                                }
                                else
                                {
                                    return 1.0f;
                                }
                            }
                        );
            }
            
            internal static AnimationInterpolator GetRandomSquareWaveInterpolator(int strength)
            {
                AnimationInterpolator square = GetSquareWaveInterpolator(strength);
                Random random = new Random();
                float prevVal = 0;
                float randomFacor = 0;
                
                return (from, to, ratio) => 
                {
                    float val = square(from, to, ratio);
                    if (prevVal != val && prevVal <= 0)
                    {
                        randomFacor = (float)random.NextDouble();
                    }
                    return prevVal = randomFacor * square(from, to, ratio);
                };
            }

            internal static AnimationInterpolator GetDampedWaveInterpolator(int strength)
            {
                strength = (int)FMath.Clamp(strength, 0, 100);
                
                return GetAnimationInterpolator(
                            (ratio) =>
                            {
                                if (strength == 0)
                                {
                                    return ratio;
                                }
                                else
                                {
                                    return (-FMath.Cos(ratio * FMath.PI * strength * 2) * FMath.Exp(-ratio * strength)) / 2.0f;
                                }
                            }
                        );
            }

            internal class CubicBezierCurveSequence
            {
                public CubicBezierCurveSequence(float startX, float startY)
                {
                    this.firstX = this.lastX = startX;
                    this.firstY = this.lastY = startY;
                }

                public void AppendSegment(float control1X, float control1Y, float control2X, float control2Y, float nextAnchorX, float nextAnchorY)
                {
                    if (!IsValidArguments(control1X, control2X, nextAnchorX))
                    {
                        throw new ArgumentException("anchor X of Bezier curve segment must be monotonically increase.");
                    }
                    segments.Add(new Segment(this.lastX, this.lastY, control1X, control1Y, control2X, control2Y, nextAnchorX, nextAnchorY));

                    this.lastX = nextAnchorX;
                    this.lastY = nextAnchorY;
                }

                public AnimationInterpolator EasingCurve
                {
                    get;
                    set;
                }

                internal float GetValue(float x)
                {
                    if (EasingCurve != null && lastX != firstX)
                    {
                        x = EasingCurve(firstX, lastX, (x - firstX) / (lastX - firstX));
                    }
                    Segment s = FindSegment(x);
                    if (s != null)
                    {
                        s.X = x;
                        return s.Y;
                    }
                    else
                    {
                        if (x >= this.lastX)
                            return this.lastY;
                        else if (x < this.firstX)
                            return this.firstY;
                        else
                            throw new System.Exception("internal error");
                    }
                }

                private bool IsValidArguments(float control1X, float control2X, float nextAnchorX)
                {
                    if (nextAnchorX < this.lastX)
                        return false;
                    if (nextAnchorX < control2X || nextAnchorX < control1X)
                        return false;
                    if (this.lastX > nextAnchorX || this.lastX > control1X || this.lastX > control2X)
                        return false;
                    return true;
                }

                private readonly List<Segment> segments = new List<Segment>();
                private float firstX;
                private float firstY;
                private float lastX;
                private float lastY;

                internal class Segment
                {
                    public Segment(float anchor1X, float anchor1Y, float control1X, float control1Y, float control2X, float control2Y, float anchor2X, float anchor2Y)
                    {
                        t = 0.0f;
                        x = anchor1X;
                        y = anchor1Y;
                        startX = anchor1X;
                        endX = anchor2X;

                        a3x = -anchor1X + 3 * control1X - 3 * control2X + anchor2X;
                        a2x = 3 * anchor1X - 6 * control1X + 3 * control2X;
                        a1x = -3 * anchor1X + 3 * control1X;
                        a0x = anchor1X;

                        a3y = -anchor1Y + 3 * control1Y - 3 * control2Y + anchor2Y;
                        a2y = 3 * anchor1Y - 6 * control1Y + 3 * control2Y;
                        a1y = -3 * anchor1Y + 3 * control1Y;
                        a0y = anchor1Y;
                    }

                    override public bool Equals(Object obj)
                    {
                        var rhs = obj as Segment;
                        return this.startX == rhs.startX
                            && this.endX == rhs.endX
                            && this.a3x == rhs.a3x
                            && this.a2x == rhs.a2x
                            && this.a1x == rhs.a1x
                            && this.a0x == rhs.a0x
                            && this.a3y == rhs.a3y
                            && this.a2y == rhs.a2y
                            && this.a1y == rhs.a1y
                            && this.a0y == rhs.a0y;
                    }

                    override public int GetHashCode()
                    {
                        return (int)(this.startX + this.endX + this.a3x + this.a2x + this.a1x + this.a0x + this.a3y + this.a2y + this.a1y + this.a0y);
                    }
                    public int RefineRepeatCount
                    {
                        get
                        {
                            return refineRepeatCount;
                        }
                        set
                        {
                            if (value > 0)
                            {
                                refineRepeatCount = value;
                            }
                        }
                    }

                    // repeat Newton's method to refine the solution of a cubic equation of t.
                    private int refineRepeatCount = 10; // 

                    public float T
                    {
                        get
                        {
                            return t;
                        }
                        set
                        {
                            if (t != value)
                            {
                                t = value;
                                Update();
                            }
                        }
                    }
                    public float X
                    {
                        get
                        {
                            return x;
                        }
                        set
                        {
                            if (x != value)
                            {
                                T = SolveEquationByNewtonsMethod((_t) => a3x * _t * _t * _t + a2x * _t * _t + a1x * _t + a0x - value,
                                                                 (_t) => 3 * a3x * _t * _t + 2 * a2x * _t + a1x,
                                                                 (value - startX) / (endX - startX), RefineRepeatCount);
                            }
                        }
                    }
                    public float Y
                    {
                        get
                        {
                            return y;
                        }
                    }

                    public float StartX
                    {
                        get
                        {
                            return startX;
                        }
                    }

                    public float EndX
                    {
                        get
                        {
                            return endX;
                        }
                    }

                    private void Update()
                    {
                        var t2 = t * t;
                        var t3 = t2 * t;
                        x = a3x * t3 + a2x * t2 + a1x * t + a0x;
                        y = a3y * t3 + a2y * t2 + a1y * t + a0y;
                    }

                    // solve equasion
                    private static float SolveEquationByNewtonsMethod(Func<float, float> equation, Func<float, float> derivative, float initial_root, int refineCount)
                    {
                        float root = initial_root;
                        float eq;
                        for (int i = 0; i < refineCount; i++)
                        {
                            eq = equation(root);
                            if (Math.Abs(eq) < 0.1f)
                                break;

                            float d = derivative(root);
                            while (Math.Abs(d) < 0.1f)
                            {
                                root += 0.001f;
                                d = derivative(root);
                            }
                            root = FMath.Clamp(root - eq / d, 0.0f, 1.0f);
                        }
                        return root;
                    }

                    private float t;
                    private float x;
                    private float y;

                    private float a3x;
                    private float a2x;
                    private float a1x;
                    private float a0x;
                    private float a3y;
                    private float a2y;
                    private float a1y;
                    private float a0y;

                    private readonly float startX;
                    private readonly float endX;
                }


                private Segment FindSegment(float x)
                {
                    foreach (var segment in segments)
                    {
                        if (segment.StartX <= x && x < segment.EndX)
                        {
                            return segment;
                        }
                    }
                    return null;
                }

            }
        }
    }
}
