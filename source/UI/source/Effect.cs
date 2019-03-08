/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>上下左右の４つ方向</summary>
    /// <remarks>エフェクトの移動方向などに使用される。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Four directions up, down, left, and right</summary>
    /// <remarks>Used for the travel direction of the effect.</remarks>
    /// @endif
    public enum FourWayDirection
    {
        /// @if LANG_JA
        /// <summary>上方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Up</summary>
        /// @endif
        Up = 0,

        /// @if LANG_JA
        /// <summary>下方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Down</summary>
        /// @endif
        Down,

        /// @if LANG_JA
        /// <summary>左方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Left</summary>
        /// @endif
        Left,

        /// @if LANG_JA
        /// <summary>右方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Right</summary>
        /// @endif
        Right
    }

    /// @if LANG_JA
    /// <summary>エフェクトの更新の応答</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Response of effect update</summary>
    /// @endif
    public enum EffectUpdateResponse
    {
        /// @if LANG_JA
        /// <summary>エフェクトの更新を継続する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Continues updating the effect.</summary>
        /// @endif
        Continue = 0,

        /// @if LANG_JA
        /// <summary>エフェクトの更新を終了する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Stops updating the effect.</summary>
        /// @endif
        Finish
    }

    /// @if LANG_JA
    /// <summary>ウィジェットまたはエレメントに適用するアニメーションの基底クラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Base class of animation applying to widget or element</summary>
    /// @endif
    public abstract class Effect
    {

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public Effect()
        {
            Playing = false;
            Paused = false;
            Repeating = false;
            Widget = null;
        }

        /// @if LANG_JA
        /// <summary>エフェクトを開始する。</summary>
        /// <remarks>開始中(Playing==true)、一時停止中(Paused==true)は何も行いません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Starts the effect.</summary>
        /// <remarks>Nothing is performed when being started (Playing==true) and when paused (Paused==true).</remarks>
        /// @endif
        public void Start()
        {
            if (!Playing && !Paused)
            {
                TotalElapsedTime = 0.0f;
                isFristUpdate = true;
                UISystem.RegisterEffect(this);
                Playing = true;
                Paused = false;
                OnStart();
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを更新する。</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the effect.</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// @endif
        internal void Update(float elapsedTime)
        {
            if (isFristUpdate && UISystem.CurrentTime < startedTime)
            {
                return;
            }
            if (isFristUpdate)
            {
                isFristUpdate = false;
                elapsedTime = FMath.Clamp(elapsedTime, 0.0f, 1000.0f / 60.0f);
            }

            TotalElapsedTime += elapsedTime;

            EffectUpdateResponse response = OnUpdate(elapsedTime);
            if (response == EffectUpdateResponse.Finish)
            {
                if (Repeating)
                {
                    OnRepeat();
                    TotalElapsedTime = 0.0f;
                }
                else
                {
                    Stop();
                }
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを停止する</summary>
        /// <remarks>エフェクトが停止中の場合は何も行いません。
        /// 一時停止中の場合は停止させます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Stops the effect</summary>
        /// <remarks>Nothing is performed when the effect is stopped.
        /// If it is paused, it will be stopped.</remarks>
        /// @endif
        public void Stop()
        {
            if (Playing || Paused)
            {
                OnStop();
                if (Playing)
                {
                    UISystem.UnregisterEffect(this);
                }
                Playing = false;
                Paused = false;

                if (EffectStopped != null)
                {
                    EffectStopped(this, EventArgs.Empty);
                }

                TotalElapsedTime = 0.0f;
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを中断する</summary>
        /// <remarks>エフェクトが再生中(Playing==true)でなければ何も行いません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Pauses the effect</summary>
        /// <remarks>Nothing is performed when the effect is being played (Playing==true).</remarks>
        /// @endif
        public void Pause()
        {
            if (Playing)
            {
                OnPause();
                Paused = true;
                Playing = false;
                UISystem.UnregisterEffect(this);
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを再開する</summary>
        /// <remarks>エフェクトが一時停止中(Paused==true)でなければ何も行いません。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resumes the effect</summary>
        /// <remarks>Nothing is performed when the effect is paused (Paused==true).</remarks>
        /// @endif
        public void Resume()
        {
            if (Paused)
            {
                OnResume();
                Paused = false;
                Playing = true;
                UISystem.RegisterEffect(this);
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを開始してからの経過時間を取得する。（ミリ秒）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the time elapsed from the start of the effect. (ms)</summary>
        /// @endif
        public float TotalElapsedTime
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>エフェクト中かどうかを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether the effect is operating.</summary>
        /// @endif
        public bool Playing
        {
            get;
            private set;
        }


        /// @if LANG_JA
        /// <summary>エフェクトが中断されているかどうかを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether the effect is paused.</summary>
        /// @endif
        public bool Paused
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>エフェクトをリピート再生するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether the effect will be repeatedly played back.</summary>
        /// @endif
        public bool Repeating
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>エフェクト対象のウィジェットを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the effect-target widget.</summary>
        /// @endif
        public Widget Widget
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>開始処理</summary>
        /// <remarks>派生クラスで開始処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Start processing</summary>
        /// <remarks>Implements the start processing with the derived class.</remarks>
        /// @endif
        abstract protected void OnStart();

        /// @if LANG_JA
        /// <summary>更新処理</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
        /// <returns>エフェクトの更新の応答</returns>
        /// <remarks>派生クラスで更新処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Update processing</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// <returns>Response of effect update</returns>
        /// <remarks>Implements the update processing with the derived class.</remarks>
        /// @endif
        abstract protected EffectUpdateResponse OnUpdate(float elapsedTime);

        /// @if LANG_JA
        /// <summary>停止処理</summary>
        /// <remarks>派生クラスで停止処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Stop processing</summary>
        /// <remarks>Implements the stop processing with the derived class.</remarks>
        /// @endif
        abstract protected void OnStop();

        /// @if LANG_JA
        /// <summary>中断処理</summary>
        /// <remarks>派生クラスで中断処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Pause processing</summary>
        /// <remarks>Implements the pause processing with the derived class.</remarks>
        /// @endif
        protected virtual void OnPause()
        {
        }

        /// @if LANG_JA
        /// <summary>再開処理</summary>
        /// <remarks>派生クラスで再開処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resume processing</summary>
        /// <remarks>Implements the resume processing with the derived class.</remarks>
        /// @endif
        protected virtual void OnResume()
        {
        }

        /// @if LANG_JA
        /// <summary>リピート処理</summary>
        /// <remarks>派生クラスでリピート処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Repeat processing</summary>
        /// <remarks>Implements the repeat processing with the derived class.</remarks>
        /// @endif
        protected virtual void OnRepeat()
        {
        }

        /// @if LANG_JA
        /// <summary>エフェクトを停止したときに呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when the effect is stopped</summary>
        /// @endif
        public event EventHandler<EventArgs> EffectStopped;

        internal TimeSpan startedTime;
        bool isFristUpdate;
    }

}
