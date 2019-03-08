/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>画面</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Screen</summary>
    /// @endif
    public class Scene
    {

        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public Scene()
        {
            this.rootWidget = new RootWidget(this);
            this.rootWidget.SetSizeInternal(UISystem.FramebufferWidth, UISystem.FramebufferHeight);
            this.Transition = null;
            this.ShowNavigationBar = false;
            this.ConsumeAllTouchEvent = false;
            this.FocusWidget = this.rootWidget;
        }

        /// @if LANG_JA
        /// <summary>シーンのタイトルを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the scene title.</summary>
        /// @endif
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != null)
                {
                    title = value;
                }
                else
                {
                    title = "";
                }
            }
        }

        private string title;

        /// @if LANG_JA
        /// <summary>ウィジェットツリーのルートを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the root of the widget tree.</summary>
        /// @endif
        public RootWidget RootWidget
        {
            get
            {
                return this.rootWidget;
            }
        }

        private RootWidget rootWidget;

        /// @if LANG_JA
        /// <summary>トランジションを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transition.</summary>
        /// @endif
        public Transition Transition
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>ナビゲーションバーを表示するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to display the navigation bar.</summary>
        /// @endif
        public bool ShowNavigationBar
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>表示するかどうかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to display.</summary>
        /// @endif
        public bool Visible
        {
            get
            {
                return (this.RootWidget != null) ? this.RootWidget.Visible : false;
            }
            set
            {
                if (this.RootWidget != null)
                {
                    this.RootWidget.Visible = value;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>レイアウト用のシーンの初期幅を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the initial width of the scene for the layout.</summary>
        /// @endif
        public float DesignWidth
        {
            get { return designWidth; }
            set
            {
                designWidth = value;
                needDesignSizeReflect = true;
            }
        }

        private float designWidth;


        /// @if LANG_JA
        /// <summary>レイアウト用のシーンの初期高さを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the initial height of the scene for the layout.</summary>
        /// @endif
        public float DesignHeight
        {
            get { return designHeight; }
            set
            {
                designHeight = value;
                needDesignSizeReflect = true;
            }
        }

        private float designHeight;
        bool needDesignSizeReflect = false;

        internal bool ConsumeAllTouchEvent
        {
            set;
            get;
        }

        /// @if LANG_JA
        /// <summary>毎フレームの更新処理のハンドラ</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Update processing handler of each frame</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// @endif
        protected virtual void OnUpdate(float elapsedTime)
        {
            if (Updated != null)
            {
                Updated(this, new UpdateEventArgs(elapsedTime));
            }
        }

        internal void Update(float elapsedTime)
        {
            OnUpdate(elapsedTime);

            this.RootWidget.updateForEachDescendant(elapsedTime);
        }

        /// @if LANG_JA
        /// <summary>毎フレームの更新時のイベント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event when updating each frame</summary>
        /// @endif
        public event EventHandler<UpdateEventArgs> Updated;

        /// @if LANG_JA
        /// <summary>画面が表示されるときに呼ばれるイベントハンドラ</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが開始される前に呼ばれる。Overrideする場合はメソッド内で基本クラスのメソッドを呼び出すこと。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event handler called when the screen is displayed</summary>
        /// <remarks>This is called before a set Transition is started. When an Override occurs, the basic class method must be called within the method.</remarks>
        /// @endif
        protected internal virtual void OnShowing()
        {
            if (needDesignSizeReflect)
            {
                this.RootWidget.updateWidth(designWidth, this.RootWidget.Width);
                this.RootWidget.updateHeight(designHeight, this.RootWidget.Height);
                needDesignSizeReflect = false;
            }
            if (Showing != null)
            {
                Showing(this, EventArgs.Empty);
            }
        }


        /// @if LANG_JA
        /// <summary>画面が表示されたとき発行するイベントハンドラ</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが終了したときに呼ばれる。Overrideする場合はメソッド内で基本クラスのメソッドを呼び出すこと。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event handler issued when the screen is displayed</summary>
        /// <remarks>This is called when a set Transition is terminated. When an Override occurs, the basic class method must be called within the method.</remarks>
        /// @endif
        protected internal virtual void OnShown()
        {
            ResetFocus(false, true);

            if (Shown != null)
            {
                Shown(this, EventArgs.Empty);
            }
        }

        
        /// @internal
        /// @if LANG_JA
        /// <summary>画面が切り替わった時などにフォーカスをリセットする</summary>
        /// <param name="setFocusActive">true: 強制的にFocusをActiveにする, false:FocusActiveは変えない</param>
        /// <param name="useDefaultFocusWidget">true: DefaultFocusWidgetが設定されていたらフォーカスをDefaultFocusWidgetに設定する</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resets focus when screen is switched, for example</summary>
        /// <param name="setFocusActive">true: force Focus to be Active, false: do no change FocusActive</param>
        /// <param name="useDefaultFocusWidget">true: set focus to DefaultFocusWidget if DefaultFocusWidget is set</param>
        /// @endif
        internal void ResetFocus(bool setFocusActive, bool useDefaultFocusWidget)
        {
            Widget focusWidget = this.FocusWidget;

            // all widgets update final clipped
            // TODO: should update if needed
            for (var node = rootWidget.LinkedTree; node != null; node = node.NextAsList)
            {
                node.Value.updateFinalClip(false);
            }

            if (useDefaultFocusWidget && this.DefaultFocusWidget != null)
            {
                focusWidget = this.DefaultFocusWidget;
            }

            // check can focus
            if (focusWidget != null &&
                (focusWidget.ParentScene != this ||
                !focusWidget.getFinalVisible() ||
                focusWidget.getClippedState() == Widget.ClippedState.ClippedAll))
            {
                focusWidget = null;
            }

            // if none focus then set focus to first widget
            if (focusWidget == null || focusWidget == this.rootWidget)
            {
                focusWidget = this.rootWidget.SearchNextTreeNodeFocus(false);

                UIDebug.LogFocus("Scene.ResetFocus.SearchNextTreeNodeFocus = {0}", focusWidget != null ? focusWidget.ToString() : "null");
            }


            if (focusWidget == null)
            {
                this.rootWidget.SetFocus(false);
            }
            else
            {
                focusWidget.SetFocus(setFocusActive);
                if (UISystem.FocusActive)
                    UISystem.setFocusVisible(true, true);
            }
        }

        /// @if LANG_JA
        /// <summary>このシーンがアクティブかどうかを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains whether this scene is active</summary>
        /// @endif
        internal bool FocusActive
        {
            get
            {
                return (this == UISystem.FocusActiveScene);
            }
        }

        /// @if LANG_JA
        /// <summary>画面が消えるときに呼ばれるイベントハンドラ</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが開始される前に呼ばれる。Overrideする場合はメソッド内で基本クラスのメソッドを呼び出すこと。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event handler called when the screen is being turned off</summary>
        /// <remarks>This is called before a set Transition is started. When an Override occurs, the basic class method must be called within the method.</remarks>
        /// @endif
        protected internal virtual void OnHiding()
        {
            if (Hiding != null)
            {
                Hiding(this, EventArgs.Empty);
            }
        }

        /// @if LANG_JA
        /// <summary>画面が消えたときに呼ばれるイベントハンドラ</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが終了したときに呼ばれる。Overrideする場合はメソッド内で基本クラスのメソッドを呼び出すこと。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event handler called when the screen has been turned off</summary>
        /// <remarks>This is called when a set Transition is terminated. When an Override occurs, the basic class method must be called within the method.</remarks>
        /// @endif
        protected internal virtual void OnHidden()
        {
            if (Hidden != null)
            {
                Hidden(this, EventArgs.Empty);
            }
        }

        /// @if LANG_JA
        /// <summary>画面が表示されるときに発行するイベント</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが開始される前に発行する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the screen is displayed</summary>
        /// <remarks>This is issued before a set Transition is started.</remarks>
        /// @endif
        public event EventHandler Showing;

        /// @if LANG_JA
        /// <summary>画面が表示されたときに発行するイベント</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが終了したときに発行する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the screen is displayed</summary>
        /// <remarks>This is issued when a set Transition is terminated.</remarks>
        /// @endif
        public event EventHandler Shown;

        /// @if LANG_JA
        /// <summary>画面が消えるときに発行するイベント</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが開始される前に発行する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the screen is being turned off</summary>
        /// <remarks>This is issued before a set Transition is started.</remarks>
        /// @endif
        public event EventHandler Hiding;

        /// @if LANG_JA
        /// <summary>画面が消えたときに発行するイベント</summary>
        /// <remarks>Transitionが設定されている場合は、Transitionが終了したときに発行する。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event issued when the screen is turned off</summary>
        /// <remarks>This is issued when a set Transition is terminated.</remarks>
        /// @endif
        public event EventHandler Hidden;

        /// @if LANG_JA
        /// <summary>現在このシーンでフォーカスが当たっているウィジェットを取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the widget that currently has focus in this scene</summary>
        /// @endif
        public Widget FocusWidget { get; internal set; }

        internal bool Focusable = true;

        /// @if LANG_JA
        /// <summary>最初にフォーカスが当たるウィジェット</summary>
        /// <remarks>nullの場合は既定のウィジェットになります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Widget in focus first</summary>
        /// <remarks>When there is null, the default widget is used.</remarks>
        /// @endif
        public Widget DefaultFocusWidget { get; set; }

#if !DISABLE_SCREEN_ORIENTATION
        public virtual ScreenOrientation ScreenOrientation
#else
        internal virtual ScreenOrientation ScreenOrientation
#endif
        {
            get { return screenOrientation; }
            set
            {
                screenOrientation = value;
                switch (value)
                {
                    case ScreenOrientation.Portrait:
                    case ScreenOrientation.ReversePortrait:
                        this.rootWidget.SetSizeInternal(UISystem.FramebufferHeight, UISystem.FramebufferWidth);
                        break;
                    case ScreenOrientation.Landscape:
                    case ScreenOrientation.ReverseLandscape:
                    default:
                        this.rootWidget.SetSizeInternal(UISystem.FramebufferWidth, UISystem.FramebufferHeight);
                        break;
                }
            }
        }
        ScreenOrientation screenOrientation;
    }

    /// @if LANG_JA
    /// <summary>更新処理イベントのデータが格納されているクラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Class storing data of update processing events</summary>
    /// @endif
    public class UpdateEventArgs : EventArgs
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間（ミリ秒）</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// <param name="elapsedTime">Elapsed time from previous update (ms)</param>
        /// @endif
        public UpdateEventArgs(float elapsedTime)
        {
            this.elapsedTime = elapsedTime;
        }

        /// @if LANG_JA
        /// <summary>前回のUpdateからの経過時間（ミリ秒）を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the elapsed time from the previous update (ms).</summary>
        /// @endif
        public float ElapsedTime
        {
            get { return elapsedTime; }
        }

        private float elapsedTime;
    }
}
