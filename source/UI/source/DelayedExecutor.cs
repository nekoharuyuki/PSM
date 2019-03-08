/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {
        /// @if LANG_JA
        /// <summary>ある時間が経過した後に処理を行うエフェクト</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Effect processed after a certain time has elapsed</summary>
        /// @endif
        public class DelayedExecutor : Effect
        {
            TimeSpan startTime;//todo: see OnUpdate()


            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public DelayedExecutor()
            {
                Widget = null;
                Time = 0.0f;
                Action = null;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="time">遅延時間（ミリ秒）</param>
            /// <param name="action">動作させる処理</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="time">Delay time (ms)</param>
            /// <param name="action">Running process</param>
            /// @endif
            public DelayedExecutor(float time, Action action)
            {
                Widget = null;
                Time = time;
                this.action = action;
            }

            /// @if LANG_JA
            /// <summary>インスタンスを作成しエフェクトを開始する。</summary>
            /// <param name="time">遅延時間（ミリ秒）</param>
            /// <param name="action">動作させる処理</param>
            /// <returns>エフェクトのインスタンス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates an instance and starts the effect.</summary>
            /// <param name="time">Delay time (ms)</param>
            /// <param name="action">Running process</param>
            /// <returns>Effect instance</returns>
            /// @endif
            public static DelayedExecutor CreateAndStart(float time, Action action)
            {
                var effect = new DelayedExecutor(time, action);
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
                if (action == null)
                {
                    this.Stop();
                }
                startTime = UISystem.CurrentTime;
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
                if (startTime == UISystem.CurrentTime)
                {
                    // TODO: want to move to superclass
                    return EffectUpdateResponse.Continue;
                }
                else if (this.TotalElapsedTime > this.Time)
                {
                    if (action != null)
                    {
                        action();
                    }
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
            }

            /// @if LANG_JA
            /// <summary>遅延時間を取得・設定する。（ミリ秒）</summary>
            /// <remarks>処理の実行タイミングは、設定した時間が経過した後の最初のフレームになる。0の場合は次のフレームとなる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the delay time. (ms)</summary>
            /// <remarks>The process is executed at the first frame after the set time has elapsed. When 0, it is executed at the next frame.</remarks>
            /// @endif
            public float Time
            {
                get { return time; }
                set { time = value; }
            }
            private float time;


            /// @if LANG_JA
            /// <summary>動作させる処理のデリゲートを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the delegate of the running process.</summary>
            /// @endif
            public Action Action
            {
                get { return action; }
                set { action = value; }
            }
            private Action action;

        }

    }
}
