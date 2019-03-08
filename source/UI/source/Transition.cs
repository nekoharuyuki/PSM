/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>トランジションの更新の応答</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Response of transition update</summary>
    /// @endif
    public enum TransitionUpdateResponse
    {
        /// @if LANG_JA
        /// <summary>トランジションの更新を継続する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Continues to update the transition.</summary>
        /// @endif
        Continue = 0,

        /// @if LANG_JA
        /// <summary>トランジションの更新を終了する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Ends updating the transition.</summary>
        /// @endif
        Finish
    }


    /// @if LANG_JA
    /// <summary>シーンに適用するアニメーションの基底クラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Base class of animation to be applied to scene</summary>
    /// @endif
    public abstract class Transition
    {

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public Transition()
        {
        }

        /// @if LANG_JA
        /// <summary>トランジションを開始する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Starts the transition.</summary>
        /// @endif
        internal void Start()
        {
            if (UISystem.NextScene == null)
            {
                return;
            }

            isFristUpdate = true;
            TotalElapsedTime = 0.0f;

            OnStart();

            UISystem.TransitionDrawOrder = DrawOrder;
        }

        /// @if LANG_JA
        /// <summary>トランジションを停止する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Stops the transition.</summary>
        /// @endif
        internal void Stop()
        {
            OnStop();

            if (UISystem.NextScene != null)
            {
                UISystem.SetSceneInternal(UISystem.NextScene);
                UISystem.NextScene = null;
            }

            if (TransitionStopped != null)
            {
                TransitionStopped(this, EventArgs.Empty);
            }
        }

        /// @if LANG_JA
        /// <summary>トランジションを開始してからの経過時間を取得する。（ミリ秒）</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the time elapsed from the start of the transition. (ms)</summary>
        /// @endif
        public float TotalElapsedTime
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>トランジションを更新する。</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the transition.</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// @endif
        internal void Update(float elapsedTime)
        {
            // #5566
            if (isFristUpdate && UISystem.CurrentTime < startedTime)
            {
                return;
            }
            if (isFristUpdate)
            {
                isFristUpdate = false;
                elapsedTime = (float)(UISystem.CurrentTime - startedTime).TotalMilliseconds;
            }

            TotalElapsedTime += elapsedTime;

            TransitionUpdateResponse response = OnUpdate(elapsedTime);
            if (response == TransitionUpdateResponse.Finish)
            {
                Stop();
            }
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
        /// <returns>トランジションの更新の応答</returns>
        /// <remarks>派生クラスで更新処理を実装する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Update processing</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// <returns>Response of transition update</returns>
        /// <remarks>Implements the update processing with the derived class.</remarks>
        /// @endif
        abstract protected TransitionUpdateResponse OnUpdate(float elapsedTime);

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
        /// <summary>トランジション中の描画順序を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the rendering order during a transition.</summary>
        /// @endif
        protected TransitionDrawOrder DrawOrder
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>トランジション終了時に呼び出されるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when a transition ends</summary>
        /// @endif
        public event EventHandler<EventArgs> TransitionStopped;

        /// @if LANG_JA
        /// <summary>次に表示するシーンを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the next scene to be displayed.</summary>
        /// @endif
        protected Scene NextScene
        {
            get
            {
                return UISystem.NextScene;
            }
        }

        /// @if LANG_JA
        /// <summary>トランジション中にのみ使用可能なエレメントツリーのルートを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the root of the element tree that can only be used during a transition.</summary>
        /// @endif
        protected RootUIElement TransitionUIElement
        {
            get
            {
                return UISystem.TransitionUIElement;
            }
        }

        /// @if LANG_JA
        /// <summary>CurrentScene をオフスクリーンレンダリングしたImageAssetを生成する。</summary>
        /// <returns>オフスクリーンレンダリングしたImageAsset</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Generates ImageAsset used for offscreen rendering of CurrentScene.</summary>
        /// <returns>Offscreen rendered ImageAsset</returns>
        /// @endif
        protected ImageAsset GetCurrentSceneRenderedImage()
        {
            if (UISystem.transitionCurrentSceneTextureCache == null)
            {
                UISystem.transitionCurrentSceneTextureCache = new Texture2D(
                    UISystem.GraphicsContext.Screen.Width,
                    UISystem.GraphicsContext.Screen.Height,
                    false, PixelFormat.Rgba,
                    PixelBufferOption.Renderable);
            }

            return getOffscreenImage(UISystem.CurrentScene, UISystem.transitionCurrentSceneTextureCache);
        }

        /// @if LANG_JA
        /// <summary>NextScene をオフスクリーンレンダリングしたImageAssetを生成する。</summary>
        /// <returns>オフスクリーンレンダリングしたImageAsset</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Generates ImageAsset used for offscreen rendering of NextScene.</summary>
        /// <returns>Offscreen rendered ImageAsset</returns>
        /// @endif
        protected ImageAsset GetNextSceneRenderedImage()
        {
            if (UISystem.transitionNextSceneTextureCache == null)
            {
                UISystem.transitionNextSceneTextureCache = new Texture2D(
                    UISystem.GraphicsContext.Screen.Width,
                    UISystem.GraphicsContext.Screen.Height,
                    false, PixelFormat.Rgba,
                    PixelBufferOption.Renderable);
            }
            return getOffscreenImage(UISystem.NextScene, UISystem.transitionNextSceneTextureCache);
        }

        private ImageAsset getOffscreenImage(Scene scene, Texture2D texture)
        {
            var frameBuffer = UISystem.offScreenFramebufferCache;
            frameBuffer.SetColorTarget(texture, 0);
            //frameBuffer.SetDepthTarget(null);
            
            scene.Update(0);
            var mat = Matrix4.Identity;
            scene.RootWidget.RenderToFrameBuffer(frameBuffer, ref mat, false);

            ImageAsset asset = new ImageAsset(texture, UISystem.IsScaledPixelDensity);

            return asset;
        }

        internal TimeSpan startedTime;
        bool isFristUpdate;
    }


}
