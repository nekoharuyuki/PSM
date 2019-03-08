/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UIUnitTest")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UITestLib")]

namespace Sce.PlayStation.HighLevel.UI
{


    /// @if LANG_JA
    /// <summary>トランジション中の描画順序</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Rendering order during a transition</summary>
    /// @endif
    public enum TransitionDrawOrder
    {
        /// @if LANG_JA
        /// <summary>CurrentSceneのみを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders only CurrentScene.</summary>
        /// @endif
        CurrentScene = 0,

        /// @if LANG_JA
        /// <summary>NextSceneのみを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders only NextScene.</summary>
        /// @endif
        NextScene,

        /// @if LANG_JA
        /// <summary>TransitionUIElementのみを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders only TransitionUIElement.</summary>
        /// @endif
        TransitionUIElement,

        /// @if LANG_JA
        /// <summary>CurrentScene、NextSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders CurrentScene and NextScene in order.</summary>
        /// @endif
        CS_NS,

        /// @if LANG_JA
        /// <summary>CurrentScene、TransitionUIElementの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders CurrentScene and TransitionUIElement in order.</summary>
        /// @endif
        CS_TE,

        /// @if LANG_JA
        /// <summary>NextScene、CurrentSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders NextScene and CurrentScene in order.</summary>
        /// @endif
        NS_CS,

        /// @if LANG_JA
        /// <summary>NextScene、TransitionUIElementの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders NextScene and TransitionUIElement in order.</summary>
        /// @endif
        NS_TE,

        /// @if LANG_JA
        /// <summary>TransitionUIElement、CurrentSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders TransitionUIElement and CurrentScene in order.</summary>
        /// @endif
        TE_CS,

        /// @if LANG_JA
        /// <summary>TransitionUIElement、NextSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders TransitionUIElement and NextScene in order.</summary>
        /// @endif
        TE_NS,

        /// @if LANG_JA
        /// <summary>CurrentScene、NextScene、TransitionUIElementの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders CurrentScene, NextScene, and TransitionUIElement in order.</summary>
        /// @endif
        CS_NS_TE,

        /// @if LANG_JA
        /// <summary>CurrentScene、TransitionUIElement、NextSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders CurrentScene, TransitionUIElement, and NextScene in order.</summary>
        /// @endif
        CS_TE_NS,

        /// @if LANG_JA
        /// <summary>NextScene、CurrentScene、TransitionUIElementの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders NextScene, CurrentScene, and TransitionUIElement in order.</summary>
        /// @endif
        NS_CS_TE,

        /// @if LANG_JA
        /// <summary>NextScene、TransitionUIElement、CurrentSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders NextScene, TransitionUIElement, and CurrentScene in order.</summary>
        /// @endif
        NS_TE_CS,

        /// @if LANG_JA
        /// <summary>TransitionUIElement、CurrentScene、NextSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders TransitionUIElement, CurrentScene, and NextScene in order.</summary>
        /// @endif
        TE_CS_NS,

        /// @if LANG_JA
        /// <summary>TransitionUIElement、NextScene、CurrentSceneの順に描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders TransitionUIElement, NextScene, and CurrentScene in order.</summary>
        /// @endif
        TE_NS_CS
    }


    /// @if LANG_JA
    /// <summary>レイアウトの方向</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Layout direction</summary>
    /// @endif
    public enum LayoutOrientation
    {
        /// @if LANG_JA
        /// <summary>水平方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Horizontal</summary>
        /// @endif
        Horizontal = 0,

        /// @if LANG_JA
        /// <summary>垂直方向</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Vertical</summary>
        /// @endif
        Vertical
    }

    /// @if LANG_JA
    /// <summary>画面の回転方法</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Screen rotation method</summary>
    /// @endif
#if !DISABLE_SCREEN_ORIENTATION
    public enum ScreenOrientation
#else
    internal enum ScreenOrientation
#endif
    {
        Landscape = 0,
        Portrait,
        ReverseLandscape,
        ReversePortrait,
    }


    /// @if LANG_JA
    /// <summary>UI Toolkitのシステム全体の管理を行うクラス</summary>
    /// @endif
    /// @if LANG_EN
    /// <summary>Class managing the entire UI Toolkit system</summary>
    /// @endif
    public static class UISystem
    {
        internal const float PerspectiveFar = 100000.0f;
        internal const float PerspectiveNear = 10.0f;
        internal const float PerspectiveFocus = 1000.0f;

        /// @if LANG_JA
        /// <summary>初期化する。</summary>
        /// <param name="graphics">グラフィックスコンテキスト</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Initializes.</summary>
        /// <param name="graphics">Graphics Context</param>
        /// @endif
        public static void Initialize(GraphicsContext graphics)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            CurrentTime = TimeSpan.Zero;
            elapsedTime = 0.0f;

            if (IsScaledPixelDensity)
            {
                FramebufferWidth = (int)(graphics.Screen.Width / pixelDensity);
                FramebufferHeight = (int)(graphics.Screen.Height / pixelDensity);
            }
            else
            {
                FramebufferWidth = graphics.Screen.Width;
                FramebufferHeight = graphics.Screen.Height;
            }


            var dpix = SystemParameters.DisplayDpiX / pixelDensity;
            var dpiy = SystemParameters.DisplayDpiY / pixelDensity;
            dpi = (dpix + dpiy) / 2f;

            hitTestPointOffsets = new Vector2[]{
                    new Vector2(0.0f, 0.0f),
                    new Vector2(-0.08f * dpix, -0.14f * dpiy),
                    new Vector2(0.08f * dpix, -0.14f * dpiy),
                    new Vector2(-0.04f * dpix, -0.07f * dpiy),
                    new Vector2(0.04f * dpix, -0.07f * dpiy),
                };

            GraphicsContext = graphics;
            StoredGraphicsState = new GraphicsState();
            SetClipRegionFull();
            GraphicsContext.SetFrameBuffer(null);
            GraphicsContext.Enable(EnableMode.Blend, true);
            GraphicsContext.Enable(EnableMode.ScissorTest, true);
            GraphicsContext.SetBlendFunc(StoredGraphicsState.DefaultBlendFunc);
            GraphicsContext.SetCullFace(StoredGraphicsState.DefaultCullFace);

            AssetManager.Initialize();

            zSortList = null;

            effects = new List<Effect>();
            addPendingEffects = new List<Effect>();
            removePendingEffects = new List<Effect>();
            effectUpdating = false;

            lowLevelTouchEvents = new TouchEventCollection();
            highLevelTouchEvents = new TouchEventCollection();
            touchEventSets = new Dictionary<Widget, TouchEventCollection>();
            primaryIDs = new Dictionary<Widget, int>();
            capturingWidgets = new Dictionary<int, Widget>();
            hitWidgets = new Dictionary<int, Widget>();

            keyEventDataList = new KeyEventData[KeyEvent.availableKeyTypes.Length];
            for (int i = 0; i < keyEventDataList.Length; i++)
            {
                keyEventDataList[i] = new KeyEventData() {
                    KeyType = KeyEvent.availableKeyTypes[i],
                    SingleGamePadButton = KeyEvent.availableGamePadButtons[i],
                };
            }

            offScreenFramebufferCache = new FrameBuffer();

            IsOffScreenRendering = false;

            ShaderProgramManager.Initialize();

            setupViewProjectionMatrix(ScreenOrientation.Landscape);

            blankScene = new Scene();
            blankScene.Focusable = false;
            CurrentScene = blankScene;
            NavigationScene = new NavigationScene();
            sceneLayers = new ReversibleList<Scene>();
            modalSceneList = new List<ModalScene>(2);
            needUpdateSceneLayersOrder = true;

            focusScene = new FocusScene();
            EnabledFocus = true;

            navigationSceneStack = new Stack<Scene>();
            ScenePushTransition = new DefaultNavigationTransition(DefaultNavigationTransition.TransitionType.Push);
            ScenePopTransition = new DefaultNavigationTransition(DefaultNavigationTransition.TransitionType.Pop);

            if (showHitTestDebugInfo == true)
            {
                DebugInfoScene = new Scene();
                DebugInfoScene.Focusable = false;

                hitTestPointSprt = new UISprite(hitTestPointOffsets.Length);
                for (int i = 0; i < hitTestPointOffsets.Length; i++)
                {
                    UISpriteUnit unit = hitTestPointSprt.GetUnit(i);
                    unit.SetSize(3, 3);
                    unit.SetPosition(hitTestPointOffsets[i].X - 1, hitTestPointOffsets[i].Y - 1);
                }
                DebugInfoScene.RootWidget.RootUIElement.AddChildLast(hitTestPointSprt);
            }

            IsDialogEffect = false;

            dummySendTouch();
        }

        /// @if LANG_JA
        /// <summary>初期化する。</summary>
        /// <param name="graphics">グラフィックスコンテキスト</param>
        /// <param name="pixelDensity">ピクセル密度</param>
        /// <exception cref="ArgumentOutOfRangeException">pixelDensity が 0.5～4 の範囲外です。</exception>
        /// @endif
        /// @if LANG_EN
        /// <summary>Initializes.</summary>
        /// <param name="graphics">Graphics Context</param>
        /// <param name="pixelDensity">Pixel density</param>
        /// <exception cref="ArgumentOutOfRangeException">pixelDensity is outside the range of 0.5 to 4.</exception>
        /// @endif
        public static void Initialize(GraphicsContext graphics, float pixelDensity)
        {
            if (pixelDensity < 0.5f || pixelDensity > 4.0f)
                throw new ArgumentOutOfRangeException("pixelDensity");

            UISystem.pixelDensity = pixelDensity;

            Initialize(graphics);
        }

        static void dummySendTouch()
        {
            // preload the touch process for first touch performance
            try
            {
                //var start = stopwatch.Elapsed;
                var widget = new Widget() { Width = FramebufferWidth, Height = FramebufferWidth };
                widget.SetupClipArea(true);
                CurrentScene.RootWidget.AddChildLast(widget);
                var touch = new TouchData() { ID = 0, Status = TouchStatus.Down };
                SendTouchDataList(new List<TouchData>() { touch });
                touch.Status = TouchStatus.Up;
                SendTouchDataList(new List<TouchData>() { touch });
                CurrentScene.RootWidget.RemoveChild(widget);
                //Debug.WriteLine("dummy touch: {0}ms\n", (stopwatch.Elapsed - start).TotalMilliseconds);
            }
            catch (Exception ex) { UIDebug.Assert(false, ex.ToString()); }
        }

        /// @if LANG_JA
        /// <summary>終了する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Terminates.</summary>
        /// @endif
        public static void Terminate()
        {
            UpdateSceneLayersOrder();
            foreach (var scene in sceneLayers)
            {
                if (scene != null)
                {
                    for (LinkedTree<Widget> tree = scene.RootWidget.LinkedTree;
                         tree != null;
                         tree = tree.NextAsList)
                    {
                        tree.Value.Dispose();
                    }
                }
            }
            sceneLayers.Clear();
            sceneLayers = null;

            if (transitionCurrentSceneTextureCache != null)
            {
                transitionCurrentSceneTextureCache.Dispose();
                transitionCurrentSceneTextureCache = null;
            }
            if (transitionNextSceneTextureCache != null)
            {
                transitionNextSceneTextureCache.Dispose();
                transitionNextSceneTextureCache = null;
            }


            ShaderProgramManager.Terminate(GraphicsContext);
            if (offScreenFramebufferCache != null)
            {
                offScreenFramebufferCache.Dispose();
                offScreenFramebufferCache = null;
            }

            AssetManager.Terminate();
            UIFontManager.ClearCache();

            zSortList = null;
            effects = null;
            addPendingEffects = null;
            removePendingEffects = null;
            capturingWidgets = null;
            hitWidgets = null;
            keyEventDataList = null;

            blankScene = null;
            currentScene = null;
            nextScene = null;
            transitionUIElementScene = null;
            navigationScene = null;
            modalSceneList = null;
            debugInfoScene = null;

            if (focusScene != null)
                focusScene.Dispose();
            focusScene = null;

            GraphicsContext = null;

            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch = null;

            hitTestPointOffsets = null;

            pixelDensity = 1.0f;
        }

        /// @if LANG_JA
        /// <summary>UIシステム全体を更新する。</summary>
        /// <param name="touchDataList">タッチ情報のリスト</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the entire UI system.</summary>
        /// <param name="touchDataList">Touch information list</param>
        /// @endif
        static public void Update(List<TouchData> touchDataList)
        {
            enabledFocusAtCurrentFrame = false;

            UpdateTime();
            SendTouchDataList(touchDataList);
            UpdateSystem();
        }

        /// @if LANG_JA
        /// <summary>UIシステム全体を更新する。</summary>
        /// <param name="touchDataList">タッチ情報のリスト</param>
        /// <param name="gamePadData">ゲームパッド情報</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the entire UI system.</summary>
        /// <param name="touchDataList">Touch information list</param>
        /// <param name="gamePadData">Gamepad information</param>
        /// @endif
        static public void Update(List<TouchData> touchDataList, ref GamePadData gamePadData)
        {
            enabledFocusAtCurrentFrame = true;

            UpdateTime();
            SendTouchDataList(touchDataList);
            SendGamePadData(ref gamePadData);
            UpdateSystem();
        }

        /// @if LANG_JA
        /// <summary>UIシステム全体を更新する。</summary>
        /// <param name="touchDataList">タッチ情報のリスト</param>
        /// <param name="motionData">モーションセンサー情報</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the entire UI system.</summary>
        /// <param name="touchDataList">Touch information list</param>
        /// <param name="motionData">Motion sensor information</param>
        /// @endif
        static public void Update(List<TouchData> touchDataList, ref MotionData motionData)
        {
            enabledFocusAtCurrentFrame = false;

            UpdateTime();
            SendTouchDataList(touchDataList);
            SendMotionData(ref motionData);
            UpdateSystem();
        }

        /// @if LANG_JA
        /// <summary>UIシステム全体を更新する。</summary>
        /// <param name="touchDataList">タッチ情報のリスト</param>
        /// <param name="gamePadData">ゲームパッド情報</param>
        /// <param name="motionData">モーションセンサー情報</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the entire UI system.</summary>
        /// <param name="touchDataList">Touch information list</param>
        /// <param name="gamePadData">Gamepad information</param>
        /// <param name="motionData">Motion sensor information</param>
        /// @endif
        static public void Update(List<TouchData> touchDataList, ref GamePadData gamePadData, ref MotionData motionData)
        {
            enabledFocusAtCurrentFrame = true;

            UpdateTime();
            SendTouchDataList(touchDataList);
            SendGamePadData(ref gamePadData);
            SendMotionData(ref motionData);
            UpdateSystem();
        }

        static private void UpdateTime()
        {
            TimeSpan previousTime = CurrentTime;
            CurrentTime = stopwatch.Elapsed;
            elapsedTime = (float)(CurrentTime - previousTime).TotalMilliseconds;

            // bug
            if (elapsedTime <= 0f)
                elapsedTime = 0.1f;
        }

        static private void UpdateSystem()
        {
            // focus timeout
            if (FocusActive)
            {
                if (focusTimeoutTimer > FocusTimeout)
                {
                    UIDebug.LogFocus("Timeout");
                    FocusActive = false;
                }
                else if(!IsTransition && !IsDialogEffect)
                {
                    focusTimeoutTimer += elapsedTime;
                }
            }

            UpdateFrame();
            UpdateEffect();
            UpdateTransition();
        }

        private static void SendTouchDataList(List<TouchData> touchDataList)
        {
            if (IsTransition || IsDialogEffect)
            {
                for (int j = 0; j < touchDataList.Count; j++)
                {
                    TouchData data = touchDataList[j];
                    data.Skip = true;
                    touchDataList[j] = data;
                }
                capturingWidgets.Clear();
                hitWidgets.Clear();
                primaryIDs.Clear();
                return;
            }

            if (touchDataList.Count == 0)
            {
                capturingWidgets.Clear();
                hitWidgets.Clear();
                primaryIDs.Clear();
            }

            lowLevelTouchEvents.Clear();
            highLevelTouchEvents.Clear();

            for (int i = 0; i < touchDataList.Count; i++)
            {
                FocusActive = false;

                TouchData touchData = touchDataList[i];

                Vector2 worldTouchPosition;
                switch (CurrentScene.ScreenOrientation)
                {
                    case ScreenOrientation.Portrait:
                        worldTouchPosition = new Vector2(
                            (0.5f - touchData.Y) * FramebufferHeight,
                            (0.5f + touchData.X) * FramebufferWidth);
                        break;
                    case ScreenOrientation.ReverseLandscape:
                        worldTouchPosition = new Vector2(
                            (0.5f - touchData.X) * FramebufferWidth,
                            (0.5f - touchData.Y) * FramebufferHeight);
                        break;
                    case ScreenOrientation.ReversePortrait:
                        worldTouchPosition = new Vector2(
                            (0.5f + touchData.Y) * FramebufferHeight,
                            (0.5f - touchData.X) * FramebufferWidth);
                        break;
                    case ScreenOrientation.Landscape:
                    default:
                        worldTouchPosition = new Vector2(
                            (0.5f + touchData.X) * FramebufferWidth,
                            (0.5f + touchData.Y) * FramebufferHeight);
                        break;
                }

                int id = touchData.ID;

                // find widget
                Widget previousHitWidget = hitWidgets.ContainsKey(id) ? hitWidgets[id] : null;
                Widget capturingWidget = capturingWidgets.ContainsKey(id) ? capturingWidgets[id] : null;
                Widget hitWidget = FindHitWidgetByNPoint(worldTouchPosition);

                switch (touchData.Status)
                {
                    case TouchStatus.Down:
                        if (hitWidget != null)
                        {
                            hitWidgets[id] = hitWidget;
                            capturingWidgets[id] = hitWidget;
                            lowLevelTouchEvents.Add(CreateTouchEvent(hitWidget, TouchEventType.Down, worldTouchPosition, id));

                            touchData.Skip = true;
                        }
                        break;

                    case TouchStatus.Up:
                        hitWidgets.Remove(id);

                        if (capturingWidget != null)
                        {
                            capturingWidgets.Remove(id);
                            lowLevelTouchEvents.Add(CreateTouchEvent(capturingWidget, TouchEventType.Up, worldTouchPosition, id));

                            touchData.Skip = true;
                        }
                        break;

                    case TouchStatus.Move:
                        if (hitWidget != null)
                        {
                            hitWidgets[id] = hitWidget;
                        }
                        else
                        {
                            hitWidgets.Remove(id);
                        }

                        if (capturingWidget != null)
                        {
                            lowLevelTouchEvents.Add(CreateTouchEvent(capturingWidget, TouchEventType.Move, worldTouchPosition, id));
                            if (hitWidget != previousHitWidget)
                            {
                                if (hitWidget == capturingWidget)
                                {
                                    highLevelTouchEvents.Add(CreateTouchEvent(capturingWidget, TouchEventType.Enter, worldTouchPosition, id));
                                }
                                else if (previousHitWidget == capturingWidget)
                                {
                                    highLevelTouchEvents.Add(CreateTouchEvent(capturingWidget, TouchEventType.Leave, worldTouchPosition, id));
                                }
                            }

                            touchData.Skip = true;
                        }
                        break;

                    case TouchStatus.Canceled:
                        ResetStateAll();
                        return;
                }

                if (IsModalScene)
                {
                    touchData.Skip = true;
                }

                if (touchData.Skip)
                {
                    // because TouchData is struct
                    touchDataList[i] = touchData;
                }
            }

            if (lowLevelTouchEvents.Count > 0)
            {
                CallTouchEventHandlers(lowLevelTouchEvents, true);
            }

            if (highLevelTouchEvents.Count > 0)
            {
                CallTouchEventHandlers(highLevelTouchEvents, false);
            }
        }

        /// @if LANG_JA
        /// <summary>指定した座標に存在するウィジェットを探す。</summary>
        /// <returns>見つかったウィジェット。見つからなかった場合はnull。</returns>
        /// <param name='screenPoint'>スクリーン座標系での座標</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Searches for the widget that exists at the specified coordinates.</summary>
        /// <returns>The found widget. If not found, null.</returns>
        /// <param name="screenPoint">Coordinates in the screen coordinate system</param>
        /// @endif
        public static Widget FindHitWidget(Vector2 screenPoint)
        {
            UpdateSceneLayersOrder();
            foreach (var scene in sceneLayers.GetReverseOrder())
            {
                if (scene == null || !scene.Visible)
                {
                    continue;
                }

                Widget retWidget = FindHitWidgetRecursively(scene.RootWidget, screenPoint);

                if (retWidget != null || scene.ConsumeAllTouchEvent)
                {
                    return retWidget;
                }
            }

            return null;
        }

        static Widget FindHitWidgetByNPoint(Vector2 screenPoint)
        {
            UpdateSceneLayersOrder();
            foreach (var scene in sceneLayers.GetReverseOrder())
            {
                if (scene == null || !scene.Visible)
                {
                    continue;
                }

                int hitTestNum = hitTestPointOffsets.Length;
                Widget[] hitWidgets = new Widget[hitTestNum];
                int[] hitCounts = new int[hitTestNum];

                for (int i = 0; i < hitTestNum; i++)
                {
                    Vector2 hitTestPosition = screenPoint + hitTestPointOffsets[i];

                    Widget hitWidget = FindHitWidgetRecursively(scene.RootWidget, hitTestPosition);

                    if (hitWidget != null)
                    {
                        hitWidgets[i] = hitWidget;

                        // Check duplication and countup
                        for (int j = 0; j <= i; j++)
                        {
                            if (hitWidgets[j] == hitWidget)
                            {
                                hitCounts[j]++;
                                break;
                            }
                        }
                    }
                }

                // Select hit widget from priority widgets by a majority
                Widget retWidget = null;
                int minCount = 0;
                for (int i = 0; i < hitTestNum; i++)
                {
                    if (hitWidgets[i] != null && hitWidgets[i].PriorityHit == true && minCount < hitCounts[i])
                    {
                        retWidget = hitWidgets[i];
                        minCount = hitCounts[i];
                    }
                }

                // Select hit widget from normal widgets by a majority
                if (retWidget == null)
                {
                    for (int i = 0; i < hitTestNum; i++)
                    {
                        if (hitWidgets[i] != null && minCount < hitCounts[i])
                        {
                            retWidget = hitWidgets[i];
                            minCount = hitCounts[i];
                        }
                    }
                }

                if (showHitTestDebugInfo == true)
                {
                    hitTestPointSprt.SetPosition(screenPoint.X, screenPoint.Y);
                    for (int i = 0; i < hitTestNum; i++)
                    {
                        if (hitWidgets[i] == retWidget)
                        {
                            hitTestPointSprt.GetUnit(i).Color = new UIColor(1.0f, 0.3f, 0.3f, 1.0f);
                        }
                        else
                        {
                            hitTestPointSprt.GetUnit(i).Color = new UIColor(1.0f, 1.0f, 0.0f, 1.0f);
                        }
                    }
                }

                if (retWidget != null || scene.ConsumeAllTouchEvent)
                {
                    return retWidget;
                }
            }

            return null;
        }

        private static Widget FindHitWidgetRecursively(Widget widget, Vector2 touchPosition)
        {
            if (widget.TouchResponse == false ||
                widget.Visible == false)
            {
                return null;
            }

            for (LinkedTree<Widget> tree = widget.LinkedTree.LastChild;
                 tree != null;
                 tree = tree.PreviousSibling)
            {
                Widget hitWidget = FindHitWidgetRecursively(tree.Value, touchPosition);
                if (hitWidget != null)
                {
                    return hitWidget;
                }
            }

            return widget.HitTest(touchPosition) ? widget : null;
        }

        private static TouchEvent CreateTouchEvent(Widget widget, TouchEventType eventType, Vector2 worldTouchPosition, int id)
        {
            TouchEvent touchEvent = new TouchEvent();
            touchEvent.Time = CurrentTime;
            touchEvent.Type = eventType;
            touchEvent.WorldPosition = worldTouchPosition;
            touchEvent.FingerID = id;
            touchEvent.Source = widget;

            return touchEvent;
        }

        private static void CallTouchEventHandlers(TouchEventCollection allTouchEvents, bool isUpdatePrimaryTouchEvent)
        {
            Stack<KeyValuePair<Widget, TouchEventCollection>> touchEventsStack = new Stack<KeyValuePair<Widget, TouchEventCollection>>();
            touchEventsStack.Push(new KeyValuePair<Widget, TouchEventCollection>(null, allTouchEvents));

            while (touchEventsStack.Count > 0)
            {
                KeyValuePair<Widget, TouchEventCollection> keyValuePair = touchEventsStack.Pop();
                Widget deliveredWidget = keyValuePair.Key;
                TouchEventCollection touchEvents = keyValuePair.Value;

                // Create the touch event sets based on a destination widget
                CreateTouchEventSets(touchEvents, deliveredWidget, isUpdatePrimaryTouchEvent);

                // Call OnTouchEvent
                foreach (KeyValuePair<Widget, TouchEventCollection> pair in touchEventSets)
                {
                    if (IsTransition || IsDialogEffect)
                    {
                        return;
                    }

                    Widget dstWidget = pair.Key;
                    TouchEventCollection touchEventSet = pair.Value;

                    dstWidget.OnTouchEvent(touchEventSet);

                    foreach (GestureDetector detector in dstWidget.GestureDetectors)
                    {
                        if (detector.State == GestureDetectorResponse.DetectedAndStop ||
                            detector.State == GestureDetectorResponse.FailedAndStop)
                        {
                            detector.OnResetState();
                            detector.State = GestureDetectorResponse.None;
                        }
                    }

                    // Foward the touch events
                    if (touchEventSet.Forward && touchEventSet.Count > 0)
                    {
                        touchEventsStack.Push(pair);
                    }
                }
            }
        }

        private static void CreateTouchEventSets(TouchEventCollection touchEvents, Widget deliveredWidget, bool isUpdatePrimaryTouchEvent)
        {
            touchEventSets.Clear();
            foreach (TouchEvent touchEvent in touchEvents)
            {
                // Don't pack the touch events which are already delivered to OriginalSource
                if (touchEvent.Source == deliveredWidget || touchEvent.Source == null)
                {
                    continue;
                }

                // Search a widget which hooks this event
                Widget dstWidget = touchEvent.Source;
                for (Widget parentWidget = dstWidget.Parent;
                     parentWidget != deliveredWidget && parentWidget != null;
                     parentWidget = parentWidget.Parent)
                {
                    if (parentWidget.HookChildTouchEvent == true)
                    {
                        dstWidget = parentWidget;
                    }
                }

                // Set a local position for a destination widget
                touchEvent.LocalPosition =
                    dstWidget.ConvertScreenToLocal(touchEvent.WorldPosition);

                // Add a event to touchEventSet
                TouchEventCollection touchEventSet;
                if (touchEventSets.ContainsKey(dstWidget))
                {
                    touchEventSets[dstWidget].Add(touchEvent);
                }
                else
                {
                    touchEventSet = new TouchEventCollection();
                    touchEventSet.Add(touchEvent);
                    touchEventSets.Add(dstWidget, touchEventSet);
                }
            }

            SetPrimaryTouchEvent(isUpdatePrimaryTouchEvent);
        }

        private static void SetPrimaryTouchEvent(bool isUpdatePrimaryTouchEvent)
        {
            foreach (KeyValuePair<Widget, TouchEventCollection> pair in touchEventSets)
            {
                Widget widget = pair.Key;
                TouchEventCollection touchEventSet = pair.Value;

                foreach (TouchEvent touchEvent in touchEventSet)
                {
                    if (touchEventSet.PrimaryTouchEvent != null)
                    {
                        break;
                    }

                    if (primaryIDs.ContainsKey(widget))
                    {
                        if (primaryIDs[widget] == touchEvent.FingerID)
                        {
                            touchEventSet.PrimaryTouchEvent = touchEvent;
                            if (isUpdatePrimaryTouchEvent && touchEvent.Type == TouchEventType.Up)
                            {
                                primaryIDs.Remove(widget);
                            }
                        }
                    }
                    else
                    {
                        if (touchEvent.Type == TouchEventType.Down)
                        {
                            touchEventSet.PrimaryTouchEvent = touchEvent;
                            if (isUpdatePrimaryTouchEvent)
                            {
                                primaryIDs.Add(widget, touchEvent.FingerID);
                            }
                        }
                    }
                }

                if (touchEventSet.PrimaryTouchEvent == null)
                {
                    if (isUpdatePrimaryTouchEvent && primaryIDs.ContainsKey(widget))
                    {
                        primaryIDs.Remove(widget);
                    }

                    // Create dummy touch event, if there is no primary touch event
                    TouchEvent e = new TouchEvent();
                    e.FingerID = -1;
                    e.Time = touchEventSet[0].Time;
                    touchEventSet.PrimaryTouchEvent = e;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>キー入力イベントを受けるウィジェットを取得・設定する。</summary>
        /// <remarks>nullを指定した場合、キーイベントは無視される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the widget receiving a key-input event.</summary>
        /// <remarks>If null is specified, the key event is ignored.</remarks>
        /// @endif
        [Obsolete("use focus system")]
        public static Widget KeyReceiverWidget
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>ゲームパッド情報を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the gamepad information</summary>
        /// @endif
        public static GamePadData GamePadData
        {
            get;
            private set;
        }

#pragma warning disable 612, 618
        private static bool obsoleteSendGamePadData(List<KeyEvent> keyEvents)
        {
            if (KeyReceiverWidget != null)
            {
                foreach (KeyEvent keyEvent in keyEvents)
                {
                    obsoleteCallKeyEventHandlerRecursively(KeyReceiverWidget, keyEvent);
                }

                return true;
            }
            return false;
        }
        
        private static void obsoleteCallKeyEventHandlerRecursively(Widget widget, KeyEvent keyEvent)
        {
            Widget localKeyReceiverWidget = widget.LocalKeyReceiverWidget;
            keyEvent.Handled = true;
            if (localKeyReceiverWidget != null)
            {
                obsoleteCallKeyEventHandlerRecursively(localKeyReceiverWidget, keyEvent);
                if (!keyEvent.Handled)
                {
                    keyEvent.Handled = true;
                    widget.OnKeyEvent(keyEvent);
                }
            }
            else
            {
                widget.OnKeyEvent(keyEvent);
            }
        }
#pragma warning restore 612, 618

        private static void SendGamePadData(ref GamePadData gamePadData)
        {
            // UISystem.GamePadData is always new.
            GamePadData = gamePadData;

            var keyEvents = GamePadDataToKeyEventList(gamePadData);

            if (keyEventCancelled)
            {
                if ((gamePadData.Buttons | gamePadData.ButtonsPrev) == 0)
                {
                    keyEventCancelled = false;
                }
                gamePadData.Skip = true;
                return;
            }
            if(IsDialogEffect || IsTransition)
            {
                gamePadData.Skip = true;
                return;
            }
            if(keyEvents.Count == 0)
            {
                return;
            }

            // reset focus timeout
            focusTimeoutTimer = 0;

            convertFourWayKeyByScreenOrientation(keyEvents, CurrentScreenOrientation);

            /////////
            // obsolete support
            bool skip = obsoleteSendGamePadData(keyEvents);
            if (skip)
            {
                gamePadData.Skip = true;
                return;
            }
            ////////

            var scene = FocusActiveScene;

            if (scene != null)
            {
                if (!FocusActive || !EnabledFocus)
                {
                    bool resetFocus = false;
                    foreach (var keyEvent in keyEvents)
                    {
                        if (suppressFocusKeyEvent && keyEvent.IsFocusKey)
                        {
                            keyEvent.Handled = true;
                        }

                        if (!keyEvent.Handled)
                        {
                            scene.RootWidget.OnPreviewKeyEvent(keyEvent);

                            if (!keyEvent.Handled)
                            {
                                scene.RootWidget.OnKeyEvent(keyEvent);

                                //TODO: send key event to other scene
                                if(!keyEvent.Handled && NavigationScene != null && NavigationScene.Visible)
                                {
                                    NavigationScene.RootWidget.OnKeyEvent(keyEvent);
                                }

                                if (!keyEvent.Handled && keyEvent.KeyEventType == KeyEventType.Down)
                                {
                                    resetFocus = true;
                                }
                            }
                        }

                        if (keyEvent.Handled)
                        {
                            gamePadData.Skip = true;
                        }
                    }

                    if (resetFocus)
                        scene.ResetFocus(true, false);
                }
                else
                {
                    List<Widget> widgetChain = null;
                    bool needUpdateWidgetChain = true;

                    foreach (var keyEvent in keyEvents)
                    {
                        if (needUpdateWidgetChain)
                        {
                            previousFocusWidgt = scene.FocusWidget;
                            widgetChain = createFocusWidgetChain(scene);
                            if (previousFocusWidgt != scene.FocusWidget)
                            {
                                CancelKeyEvents();
                                return;
                            }
                        }

                        // OnPreviewKeyEvent
                        for (int i = widgetChain.Count - 1; i >= 0 && !keyEvent.Handled; i--)
                        {
                            widgetChain[i].OnPreviewKeyEvent(keyEvent);
                        }

                        // handle focus key
                        if (EnabledFocus && !keyEvent.Handled && (
                               keyEvent.KeyEventType == KeyEventType.Down ||
                               keyEvent.KeyEventType == KeyEventType.Repeat)
                               )
                        {
                            Widget w = null;
                            FourWayDirection direction = FourWayDirection.Down;
                            if (KeyEvent.TryConvertToDirection(keyEvent.KeyType, ref direction))
                            {
                                w = scene.FocusWidget.SearchNextFocus(direction);
                                keyEvent.Handled = true;
                            }
                            //// test
                            //else if (keyEvent.KeyType == KeyType.R)
                            //    w = scene.FocusWidget.SearchNextTreeNodeFocus(false);
                            //else if (keyEvent.KeyType == KeyType.L)
                            //    w = scene.FocusWidget.SearchNextTreeNodeFocus(true);

                            if (w != null)
                            {
                                w.SetFocus(true);
                                keyEvent.Handled = true;
                                needUpdateWidgetChain = true;
                            }
                        }

                        // OnKeyEvent
                        for (int i = 0; i < widgetChain.Count && !keyEvent.Handled; i++)
                        {
                            widgetChain[i].OnKeyEvent(keyEvent);
                        }

                        //TODO: send key event to other scene
                        if(!keyEvent.Handled && NavigationScene != null && NavigationScene.Visible)
                        {
                            NavigationScene.RootWidget.OnKeyEvent(keyEvent);
                        }

                        //todo:enabledEachFrameRepeats[(int)(keyEvent.KeyType)] = keyEvent.Handled;
                        if (keyEvent.Handled)
                        {
                            gamePadData.Skip = true;
                        }
                    }
                }
            }
        }

        private static List<Widget> createFocusWidgetChain(Scene scene)
        {
            var widget = scene.FocusWidget;

            var widgetChain = new List<Widget>(16);
            while (widget != null)
            {
                widgetChain.Add(widget);
                widget = widget.Parent;
            }

            if (widgetChain.Count > 0)
            {
                var root = widgetChain[widgetChain.Count - 1] as RootWidget;
                if (root == null)
                {
                    widgetChain.Clear();
                    widgetChain.Add(scene.RootWidget);
                    scene.FocusWidget = scene.RootWidget;
                }
            }
            else
            {
                widgetChain.Add(scene.RootWidget);
                scene.FocusWidget = scene.RootWidget;
            }

            return widgetChain;
        }

        private static List<KeyEvent> GamePadDataToKeyEventList(GamePadData gamePadData)
        {
            List<KeyEvent> keyEvents = new List<KeyEvent>(4);
            var time = UISystem.CurrentTime;
            var upButtons = gamePadData.ButtonsPrev & gamePadData.ButtonsUp;
            var downButtons = gamePadData.Buttons & gamePadData.ButtonsDown;
            var secondButtons = gamePadData.Buttons & ~gamePadData.ButtonsDown;

            foreach (var eventData in keyEventDataList)
            {
                if (eventData.Active)
                {
                    if ((upButtons & eventData.SingleGamePadButton) != 0)
                    {
                        //up
                        keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.Up, time, eventData.KeyDownTime));
                        eventData.Active = false;
                    }
                    else if ((secondButtons & eventData.SingleGamePadButton) != 0)
                    {
                        // repeat each frame
                        if (eventData.EnabledEachFrameRepeat)
                        {
                            keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.EachFrameRepeat,
                                time, eventData.KeyDownTime, eventData.EnabledEachFrameRepeat));
                        }

                        // long press
                        if (time >= eventData.NextKeyLongPressTime)
                        {
                            keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.LongPress,
                                time, eventData.KeyDownTime, eventData.EnabledEachFrameRepeat));
                            eventData.NextKeyLongPressTime = TimeSpan.MaxValue;
                        }

                        // repeat
                        if (time >= eventData.NextKeyRepeatTime)
                        {
                            keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.Repeat,
                                time, eventData.KeyDownTime, eventData.EnabledEachFrameRepeat));
                            eventData.NextKeyRepeatTime += keyRepeatIntervalTime;
                        }
                    }
                    else
                    {
                        // cancel
                        keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.Cancel, time, eventData.KeyDownTime));
                        eventData.Active = false;
                    }
                }
                else
                {
                    if ((downButtons & eventData.SingleGamePadButton) != 0)
                    {
                        //down
                        keyEvents.Add(new KeyEvent(eventData.KeyType, KeyEventType.Down, time, time));
                        eventData.Active = true;
                        eventData.KeyDownTime = time;
                        eventData.NextKeyLongPressTime = time + keyLongPressTime;
                        eventData.NextKeyRepeatTime = time + keyRepeatStartTime;
                        eventData.EnabledEachFrameRepeat = false;
                    }
                }
            }

            return keyEvents;
        }

        static void convertFourWayKeyByScreenOrientation(List<KeyEvent> keyEvents, ScreenOrientation orientation)
        {
            if (orientation != ScreenOrientation.Landscape)
            {
                //foreach (var item in keyEvents)
                //{
                //    int key = (int)item.KeyType;
                //    if (key >= (int)KeyType.Left && key <= (int)KeyType.Down)
                //        item.KeyType = (KeyType)((key + (int)orientation - (int)KeyType.Left) % 4 + (int)KeyType.Left);
                //}
                switch (orientation)
                {
                    case ScreenOrientation.Portrait:
                        foreach (var item in keyEvents)
                        {
                            switch (item.KeyType)
                            {
                                case KeyType.Left:
                                    item.KeyType = KeyType.Up;
                                    break;
                                case KeyType.Up:
                                    item.KeyType = KeyType.Right;
                                    break;
                                case KeyType.Right:
                                    item.KeyType = KeyType.Down;
                                    break;
                                case KeyType.Down:
                                    item.KeyType = KeyType.Left;
                                    break;
                            }
                        }
                        break;
                    case ScreenOrientation.ReverseLandscape:
                        foreach (var item in keyEvents)
                        {
                            switch (item.KeyType)
                            {
                                case KeyType.Left:
                                    item.KeyType = KeyType.Right;
                                    break;
                                case KeyType.Up:
                                    item.KeyType = KeyType.Down;
                                    break;
                                case KeyType.Right:
                                    item.KeyType = KeyType.Left;
                                    break;
                                case KeyType.Down:
                                    item.KeyType = KeyType.Up;
                                    break;
                            }
                        }
                        break;
                    case ScreenOrientation.ReversePortrait:
                        foreach (var item in keyEvents)
                        {
                            switch (item.KeyType)
                            {
                                case KeyType.Left:
                                    item.KeyType = KeyType.Down;
                                    break;
                                case KeyType.Up:
                                    item.KeyType = KeyType.Left;
                                    break;
                                case KeyType.Right:
                                    item.KeyType = KeyType.Up;
                                    break;
                                case KeyType.Down:
                                    item.KeyType = KeyType.Right;
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        /// @if LANG_JA
        /// <summary>現在のキーイベントをすべてキャンセルする</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Cancel all current key events</summary>
        /// @endif
        public static void CancelKeyEvents()
        {
            keyEventCancelled = true;
        }

        /// @if LANG_JA
        /// <summary>モーションセンサーの情報を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains motion sensor information.</summary>
        /// @endif
        public static MotionData MotionData
        {
            get;
            private set;
        }

        private static void SendMotionData(ref MotionData motionData)
        {
            MotionData = motionData;

            MotionEvent motionEvent = new MotionEvent();
            motionEvent.Time = CurrentTime;

            var a = motionData.Acceleration;
            var v = motionData.AngularVelocity;
            switch (CurrentScene.ScreenOrientation)
            {
                case ScreenOrientation.Portrait:
                    motionEvent.Acceleration = new Vector3(a.Y, -a.X, a.Z);
                    motionEvent.AngularVelocity = new Vector3(v.Y, -v.X, v.Z);
                    break;
                case ScreenOrientation.ReverseLandscape:
                    motionEvent.Acceleration = new Vector3(-a.X, -a.Y, a.Z);
                    motionEvent.AngularVelocity = new Vector3(-v.X, -v.Y, v.Z);
                    break;
                case ScreenOrientation.ReversePortrait:
                    motionEvent.Acceleration = new Vector3(-a.Y, a.X, a.Z);
                    motionEvent.AngularVelocity = new Vector3(-v.Y, v.X, v.Z);
                    break;
                case ScreenOrientation.Landscape:
                default:
                    motionEvent.Acceleration = a;
                    motionEvent.AngularVelocity = v;
                    break;
            }

            CallMotionEventHandler(motionEvent);
        }

        private static void CallMotionEventHandler(MotionEvent motionEvent)
        {
            Widget rootWidget = CurrentScene.RootWidget;
            LinkedTree<Widget> end = rootWidget.LinkedTree.LastDescendant.NextAsList;
            for (LinkedTree<Widget> widgetTree = rootWidget.LinkedTree; widgetTree != end; widgetTree = widgetTree.NextAsList)
            {
                Widget widget = widgetTree.Value;
                if (widget != null)
                {
                    widget.OnMotionEvent(motionEvent);
                }
            }
        }

        /// @if LANG_JA
        /// <summary>UIシステム全体を１フレーム分進める。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Advances the entire UI system one frame.</summary>
        /// @endif
        private static void UpdateFrame()
        {
            UpdateSceneLayersOrder();
            foreach (var scene in sceneLayers)
            {
                if (scene == null)
                {
                    continue;
                }

                // call scene.OnUpdate and each Widget.OnUpdate in scene's widget tree.
                scene.Update(elapsedTime);
            }
        }

        /// @if LANG_JA
        /// <summary>UIを描画する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders the UI.</summary>
        /// @endif
        public static void Render()
        {
            SetupGraphics();

            UpdateSceneLayersOrder();
            foreach (var scene in sceneLayers)
            {
                if (scene != null)
                {
                    SetCurrentScreenOrientation(scene.ScreenOrientation);
                    Render(scene.RootWidget);
                }
            }

            RestoreGraphics();
        }

        internal static void Render(Widget widget)
        {
            RenderWidgets(widget);
            RenderZSortList();
        }

        private static void SetupGraphics()
        {
            // save state
            StoredGraphicsState.EnableModeBlend = GraphicsContext.IsEnabled(EnableMode.Blend); // change by UIElement
            StoredGraphicsState.EnableModeCullFace = GraphicsContext.IsEnabled(EnableMode.CullFace); // change by UIElement
            StoredGraphicsState.EnableModeDepthTest = GraphicsContext.IsEnabled(EnableMode.DepthTest);
            StoredGraphicsState.EnableModeScissorTest = GraphicsContext.IsEnabled(EnableMode.ScissorTest);
            StoredGraphicsState.EnableModeStencilTest = GraphicsContext.IsEnabled(EnableMode.StencilTest);
            StoredGraphicsState.BlendFuncRgb = GraphicsContext.GetBlendFuncRgb(); // change by UIElement
            StoredGraphicsState.BlendFuncAlpha = GraphicsContext.GetBlendFuncAlpha(); // change by UIElement
            StoredGraphicsState.ClearColor = GraphicsContext.GetClearColor(); // change by RenderToTexture
            StoredGraphicsState.ColorMask = GraphicsContext.GetColorMask(); // change by RenderToTexture
            StoredGraphicsState.CullFace = GraphicsContext.GetCullFace(); // change by UIElement
            // EnableMode.Dither is unchange


            // set state

            // enabled change is checked at Core.
            GraphicsContext.Enable(EnableMode.DepthTest, false);
            GraphicsContext.Enable(EnableMode.ScissorTest, true);
            GraphicsContext.Enable(EnableMode.StencilTest, false);
            if (StoredGraphicsState.ColorMask != ColorMask.Rgb)
            {
                GraphicsContext.SetColorMask(ColorMask.Rgb);
            }
            GraphicsContext.SetCullFace(StoredGraphicsState.DefaultCullFace);


            // Dont restore state
            var currentFBO = GraphicsContext.GetFrameBuffer();
            if (currentFBO != GraphicsContext.Screen)
            {
                GraphicsContext.SetFrameBuffer(null);
            }
            SetClipRegionFull();
            var currentViewport = GraphicsContext.GetViewport();
            var newViewport = new ImageRect(0, 0, GraphicsContext.Screen.Width, GraphicsContext.Screen.Height);
            if (!newViewport.Equals(currentViewport))
            {
                GraphicsContext.SetViewport(newViewport);
            }

            // for debug
            if(UIDebug.WidgetClipping == UIDebug.ForceMode.ForceDisabled)
                GraphicsContext.Enable(EnableMode.ScissorTest, false);
        }

        private static void RestoreGraphics()
        {
            // enabled change is checked at Core.
            GraphicsContext.Enable(EnableMode.Blend, StoredGraphicsState.EnableModeBlend);
            GraphicsContext.Enable(EnableMode.CullFace, StoredGraphicsState.EnableModeCullFace);
            GraphicsContext.Enable(EnableMode.DepthTest, StoredGraphicsState.EnableModeDepthTest);
            GraphicsContext.Enable(EnableMode.ScissorTest, StoredGraphicsState.EnableModeScissorTest);
            GraphicsContext.Enable(EnableMode.StencilTest, StoredGraphicsState.EnableModeStencilTest);

            if (!StoredGraphicsState.BlendFuncRgb.Equals(GraphicsContext.GetBlendFuncRgb()))
            {
                GraphicsContext.SetBlendFuncRgb(StoredGraphicsState.BlendFuncRgb);
            }
            if (!StoredGraphicsState.BlendFuncAlpha.Equals(GraphicsContext.GetBlendFuncAlpha()))
            {
                GraphicsContext.SetBlendFuncAlpha(StoredGraphicsState.BlendFuncAlpha);
            }
            if (!StoredGraphicsState.ClearColor.Equals(GraphicsContext.GetClearColor()))
            {
                GraphicsContext.SetClearColor(StoredGraphicsState.ClearColor);
            }
            if (!StoredGraphicsState.ColorMask.Equals(GraphicsContext.GetColorMask()))
            {
                GraphicsContext.SetColorMask(StoredGraphicsState.ColorMask);
            }
            if (!StoredGraphicsState.CullFace.Equals(GraphicsContext.GetCullFace()))
            {
                GraphicsContext.SetCullFace(StoredGraphicsState.CullFace);
            }

            SetClipRegionFull();
        }

        internal static void RenderWidgets(Widget rootWidget)
        {
            LinkedTree<Widget> end = rootWidget.LinkedTree.LastDescendant.NextAsList;
            for (LinkedTree<Widget> widgetTree = rootWidget.LinkedTree; widgetTree != end; widgetTree = widgetTree.NextAsList)
            {
                Widget widget = widgetTree.Value;

                if (widget.Visible)
                {
                    for (LinkedTree<UIElement> tree = widget.RootUIElement.linkedTree; tree != null; tree = tree.NextAsList)
                    {
                        UIElement element = tree.Value;

                        if (element.Visible)
                        {
                            element.SetupFinalAlpha();
                        }
                        else
                        {
                            tree = tree.LastDescendant;
                        }
                    }
                }
                else
                {
                    widgetTree = widgetTree.LastDescendant;
                }
            }

            zSortList = null;

            end = rootWidget.LinkedTree.LastDescendant.NextAsList;
            for (LinkedTree<Widget> tree = rootWidget.LinkedTree; tree != end; tree = tree.NextAsList)
            {
                Widget widget = tree.Value;

                if (widget.Visible && widget.SetupClipArea(widget == rootWidget))
                {
                    widget.AddZSortUIElements(ref zSortList);

                    if (widget.ZSort)
                    {
                        tree = tree.LastDescendant;
                    }
                    else
                    {
                        widget.SetupFinalAlpha();
                        widget.Render();
                    }
                }
                else
                {
                    tree = tree.LastDescendant;
                }

            }
        }

        internal static void RenderZSortList()
        {
            SetClipRegionFull();

            if (zSortList != null)
            {
                zSortList = SortDrawSequence(zSortList, null, null);
            }

            for (UIElement element = zSortList; element != null; element = element.nextZSortElement)
            {
                if (element.Parent == null) // RootUIElement
                {
                    Widget widget = ((RootUIElement)element).parentWidget;

                    widget.ZSort = false;
                    widget.Render();
                    widget.ZSort = true;

                    LinkedTree<Widget> end = widget.LinkedTree.LastDescendant.NextAsList;

                    for (LinkedTree<Widget> tree = widget.LinkedTree.NextAsList; tree != end; tree = tree.NextAsList)
                    {
                        Widget widget2 = tree.Value;

                        widget2.Render();
                    }
                }
                else
                {
                    element.SetupFinalAlpha();
                    element.SetupDrawState();
                    element.Render();

                    LinkedTree<UIElement> end = element.linkedTree.LastDescendant.NextAsList;

                    for (LinkedTree<UIElement> tree2 = element.linkedTree.NextAsList; tree2 != end; tree2 = tree2.NextAsList)
                    {
                        UIElement element2 = tree2.Value;

                        if (element2.ZSort)
                        {
                            tree2 = tree2.LastDescendant;
                        }
                        else
                        {
                            element2.SetupFinalAlpha();
                            element2.SetupDrawState();
                            element2.Render();
                        }
                    }
                }
            }
        }

        private static UIElement SortDrawSequence(UIElement targetList, UIElement prevTargetList, UIElement nextTargetList)
        {
            UIElement center = targetList;
            targetList = targetList.nextZSortElement;

            UIElement leftList = null;
            UIElement rightList = null;

            UIElement nextSort;

            for (UIElement element = targetList; element != nextTargetList; element = nextSort)
            {
                nextSort = element.nextZSortElement;

                if (element.zSortValue >= center.zSortValue)
                {
                    element.nextZSortElement = (leftList != null) ? leftList : center;
                    leftList = element;
                }
                else
                {
                    element.nextZSortElement = (rightList != null) ? rightList : nextTargetList;
                    rightList = element;
                }
            }

            if (leftList != null)
            {
                if (prevTargetList != null)
                {
                    prevTargetList.nextZSortElement = leftList;
                }

                targetList = SortDrawSequence(leftList, prevTargetList, center);
            }
            else
            {
                if (prevTargetList != null)
                {
                    prevTargetList.nextZSortElement = center;
                }

                targetList = center;
            }

            if (rightList != null)
            {
                center.nextZSortElement = rightList;

                SortDrawSequence(rightList, center, nextTargetList);
            }
            else
            {
                center.nextZSortElement = nextTargetList;
            }

            return targetList;
        }

        /// @if LANG_JA
        /// <summary>フレームバッファの幅を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the width of the frame buffer.</summary>
        /// @endif
        public static int FramebufferWidth
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>フレームバッファの高さを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the height of the frame buffer.</summary>
        /// @endif
        public static int FramebufferHeight
        {
            get;
            private set;
        }

        internal static ScreenOrientation CurrentScreenOrientation
        {
            get { return currentScreenOrientation;}
        }
        internal static void SetCurrentScreenOrientation(ScreenOrientation orientation)
        {
            if (orientation != currentScreenOrientation)
            {
                currentScreenOrientation = orientation;
                setupViewProjectionMatrix(orientation);
            }
        }
        static ScreenOrientation currentScreenOrientation;

        internal static bool IsCurrentVerticalLayout
        {
            get
            {
                return (currentScreenOrientation == ScreenOrientation.Portrait ||
                        currentScreenOrientation == ScreenOrientation.ReversePortrait);
            }
        }

        internal static int CurrentScreenWidth
        {
            get
            {
                if (IsCurrentVerticalLayout)
                {
                    return FramebufferHeight;
                }
                return FramebufferWidth;
            }
        }
        internal static int CurrentScreenHeight
        {
            get
            {
                if (IsCurrentVerticalLayout)
                {
                    return FramebufferWidth;
                }
                return FramebufferHeight;
            }
        }

        static float pixelDensity = 1.0f;


        /// @if LANG_JA
        /// <summary>ピクセル密度を取得する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the pixel density</summary>
        /// @endif
        public static float PixelDensity
        {
            get { return pixelDensity; }
        }

        internal static bool IsScaledPixelDensity
        {
            get { return pixelDensity != 1.0f;}
        }

        /// @internal
        /// @if LANG_JA
        /// <summary>DPI (SystemParameters.DisplayDpiX + SystemParameters.DisplayDpiY) / 2f</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>DPI (SystemParameters.DisplayDpiX + SystemParameters.DisplayDpiY) / 2f</summary>
        /// @endif
        internal static float Dpi
        {
            get { return dpi; }
            set { dpi = value; }
        }

        private static float dpi;
        private static Scene blankScene;

        /// @if LANG_JA
        /// <summary>現在表示中のシーンを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the currently displayed scene.</summary>
        /// @endif
        public static Scene CurrentScene
        {
            get
            {
                return currentScene;
            }
            private set
            {
                currentScene = value;
                needUpdateSceneLayersOrder = true;
            }
        }

        private static Scene currentScene;

        /// @if LANG_JA
        /// <summary>次に表示するシーンを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the next scene to be displayed.</summary>
        /// @endif
        internal static Scene NextScene
        {
            get
            {
                return nextScene;
            }
            set
            {
                nextScene = value;

                if (nextScene != null)
                {
                    TransitionUIElementScene = new Scene();
                    TransitionUIElementScene.Focusable = false;
                }
                else
                {
                    if (TransitionUIElementScene != null && TransitionUIElementScene.RootWidget != null)
                    {
                        TransitionUIElementScene.RootWidget.Dispose();
                    }
                    TransitionUIElementScene = null;
                    TransitionDrawOrder = TransitionDrawOrder.CurrentScene;
                }

                needUpdateSceneLayersOrder = true;
            }
        }

        private static Scene nextScene;

        private static Scene TransitionUIElementScene
        {
            get
            {
                return transitionUIElementScene;
            }
            set
            {
                transitionUIElementScene = value;
                needUpdateSceneLayersOrder = true;
            }
        }

        private static Scene transitionUIElementScene;

        private static NavigationScene NavigationScene
        {
            get
            {
                return navigationScene;
            }
            set
            {
                navigationScene = value;
                needUpdateSceneLayersOrder = true;
            }
        }

        private static NavigationScene navigationScene;

        private static Scene DebugInfoScene
        {
            get
            {
                return debugInfoScene;
            }
            set
            {
                debugInfoScene = value;
                needUpdateSceneLayersOrder = true;
            }
        }

        private static Scene debugInfoScene;
        static Transition currentTransition = null;

        /// @if LANG_JA
        /// <summary>シーンがプッシュされる時のトランジションを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transition when a scene is pushed.</summary>
        /// @endif
        public static Transition ScenePushTransition
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>シーンがポップされる時のトランジションを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the transition when a scene pops.</summary>
        /// @endif
        public static Transition ScenePopTransition
        {
            get;
            set;
        }

        /// @if LANG_JA
        /// <summary>トランジション中のみ使用可能なエレメントツリーのルートを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the root of the element tree that can only be used during a transition.</summary>
        /// @endif
        internal static RootUIElement TransitionUIElement
        {
            get
            {
                return TransitionUIElementScene.RootWidget.RootUIElement;
            }
        }

        /// @if LANG_JA
        /// <summary>シーンを切り替える。</summary>
        /// <param name="newScene">新しいシーン</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Changes the scene.</summary>
        /// <param name="newScene">New scene</param>
        /// @endif
        public static void SetScene(Scene newScene)
        {
            Transition trans = newScene == null ? null : newScene.Transition;
            SetScene(newScene, trans);
        }

        /// @if LANG_JA
        /// <summary>シーンを切り替える。</summary>
        /// <param name="newScene">新しいシーン</param>
        /// <param name="transition">トランジション</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Changes the scene.</summary>
        /// <param name="newScene">New scene</param>
        /// <param name="transition">Transition</param>
        /// @endif
        public static void SetScene(Scene newScene, Transition transition)
        {
            navigationSceneStack.Clear();
            SetSceneInternal(newScene, transition);
        }

        /// @if LANG_JA
        /// <summary>現在のシーンをシーンスタックにプッシュし、シーンを切り替える。</summary>
        /// <param name="newScene">新しいシーン</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Pushes the current scene to the scene stack and changes the scene.</summary>
        /// <param name="newScene">New scene</param>
        /// @endif
        public static void PushScene(Scene newScene)
        {
            navigationSceneStack.Push(CurrentScene);
            SetSceneInternal(newScene, ScenePushTransition, true);
            NavigationScene.StartPushAnimation(0 < navigationSceneStack.Count ? navigationSceneStack.Peek().Title : null, newScene.Title);
        }

        /// @if LANG_JA
        /// <summary>シーンスタックに最後にプッシュしたシーンに切り替える。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Changes to the scene that was last pushed to the scene stack.</summary>
        /// @endif
        public static void PopScene()
        {
            if (0 < navigationSceneStack.Count)
            {
                Scene newScene = navigationSceneStack.Pop();
                SetSceneInternal(newScene, ScenePopTransition, true);
                NavigationScene.StartPopAnimation(0 < navigationSceneStack.Count ? navigationSceneStack.Peek().Title : null, newScene.Title);
            }
        }

        internal static void SetSceneInternal(Scene newScene, Transition transition)
        {
            SetSceneInternal(newScene, transition, false);
        }

        internal static void SetSceneInternal(Scene newScene, Transition transition, bool withNavigationAnimation)
        {
            if (newScene == null)
            {
                newScene = blankScene;
            }

            setFocusVisible(false, false);

            CurrentScene.OnHiding();
            newScene.OnShowing();

            currentTransition = transition;

            if (currentTransition == null)
            {
                SetSceneInternal(newScene);
            }
            else
            {
                NextScene = newScene;
                currentTransition.Start();
                currentTransition.startedTime = stopwatch.Elapsed;
            }

            if (newScene.ShowNavigationBar)
            {
                if (withNavigationAnimation)
                {
                    // New scene title is set in PosScene or PushScene
                    NavigationScene.Show(true, null);
                }
                else
                {
                    NavigationScene.Show(true, newScene.Title);
                }
            }
            else
            {
                NavigationScene.Hide(true);
            }
        }

        internal static void SetSceneInternal(Scene newScene)
        {
            UIDebug.Assert(newScene != null);

            ResetStateAll();

            var lastScene = CurrentScene;

            CurrentScene = newScene;
            NextScene = null;
            currentTransition = null;

            lastScene.OnHidden();
            newScene.OnShown();
            OnFocusActiveSceneChanged(newScene, lastScene);
        }

        /// @if LANG_JA
        /// <summary>トランジション中の描画順序を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the rendering order during a transition.</summary>
        /// @endif
        internal static TransitionDrawOrder TransitionDrawOrder
        {
            get
            {
                return transitionDrawOrder;
            }
            set
            {
                transitionDrawOrder = value;
                needUpdateSceneLayersOrder = true;
            }
        }

        private static TransitionDrawOrder transitionDrawOrder;

        private static void UpdateSceneLayersOrder()
        {
            if (!needUpdateSceneLayersOrder)
            {
                return;
            }

            if (sceneLayers == null)
            {
                sceneLayers = new ReversibleList<Scene>();
            }
            else
            {
                sceneLayers.Clear();
            }

            switch (transitionDrawOrder)
            {
                case TransitionDrawOrder.CurrentScene:
                    sceneLayers.Add(CurrentScene);
                    break;
                case TransitionDrawOrder.NextScene:
                    sceneLayers.Add(NextScene);
                    break;
                case TransitionDrawOrder.TransitionUIElement:
                    sceneLayers.Add(TransitionUIElementScene);
                    break;
                case TransitionDrawOrder.CS_NS:
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(NextScene);
                    break;
                case TransitionDrawOrder.CS_TE:
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    break;
                case TransitionDrawOrder.NS_CS:
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(CurrentScene);
                    break;
                case TransitionDrawOrder.NS_TE:
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    break;
                case TransitionDrawOrder.TE_CS:
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(CurrentScene);
                    break;
                case TransitionDrawOrder.TE_NS:
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(NextScene);
                    break;
                case TransitionDrawOrder.CS_NS_TE:
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    break;
                case TransitionDrawOrder.CS_TE_NS:
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(NextScene);
                    break;
                case TransitionDrawOrder.NS_CS_TE:
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    break;
                case TransitionDrawOrder.NS_TE_CS:
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(CurrentScene);
                    break;
                case TransitionDrawOrder.TE_CS_NS:
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(CurrentScene);
                    sceneLayers.Add(NextScene);
                    break;
                case TransitionDrawOrder.TE_NS_CS:
                    sceneLayers.Add(TransitionUIElementScene);
                    sceneLayers.Add(NextScene);
                    sceneLayers.Add(CurrentScene);
                    break;
                default:
                    sceneLayers.Add(CurrentScene);
                    break;
            }

            sceneLayers.Add(NavigationScene);
            foreach (var modalScene in modalSceneList)
            {
                sceneLayers.Add(modalScene);
            }
            sceneLayers.Add(focusScene);
            sceneLayers.Add(DebugInfoScene);
            needUpdateSceneLayersOrder = false;
        }

        /// @if LANG_JA
        /// <summary>グラフィックスコンテキストを取得する。</summary>
        /// <returns>グラフィックスコンテキスト</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the graphics context.</summary>
        /// <returns>Graphics Context</returns>
        /// @endif
        public static GraphicsContext GraphicsContext
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>シェーダープログラムを取得する。</summary>
        /// <param name="type">シェーダープログラムの種類</param>
        /// <returns>シェーダープログラム</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the shader program.</summary>
        /// <param name="type">Type of shader program</param>
        /// <returns>Shader Program</returns>
        /// @endif
        public static ShaderProgram GetShaderProgram(ShaderType type)
        {
            return ShaderProgramManager.GetShaderProgram((InternalShaderType)type);
        }

        internal static int GetUniformLocation(ShaderType type, string name)
        {
            return ShaderProgramManager.GetUniforms((InternalShaderType)type)[name];
        }

        /// @if LANG_JA
        /// <summary>プロジェクション行列を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the projection matrix.</summary>
        /// @endif
        public static Matrix4 ViewProjectionMatrix
        {
            get
            {
                return viewProjectionMatrix;
            }
            internal set
            {
                viewProjectionMatrix = value;
            }
        }

        internal static Matrix4 viewProjectionMatrix;
        internal static Vector4 viewpoint;
        internal static Matrix4 screenMatrix;

        static void setupViewProjectionMatrix(ScreenOrientation Orientation)
        {
            Matrix4 frustum, view, r, ri;
            int w, h;
            switch (Orientation)
            {
                case ScreenOrientation.Portrait:
                    w = FramebufferHeight;
                    h = FramebufferWidth;
                    r = Matrix4.RotationZ(FMath.PI / 2f);
                    ri = Matrix4.RotationZ(-FMath.PI / 2f);
                    break;
                case ScreenOrientation.ReverseLandscape:
                    w = FramebufferWidth;
                    h = FramebufferHeight;
                    ri = r = Matrix4.RotationZ(FMath.PI);
                    break;
                case ScreenOrientation.ReversePortrait:
                    w = FramebufferHeight;
                    h = FramebufferWidth;
                    r = Matrix4.RotationZ(-FMath.PI / 2f);
                    ri = Matrix4.RotationZ(FMath.PI / 2f);
                    break;
                default:
                    w = FramebufferWidth;
                    h = FramebufferHeight;
                    ri = r = Matrix4.Identity;
                    break;
            }

            viewpoint = new Vector4(w / 2, h / 2, -PerspectiveFocus, 1.0f);
            frustum = new Matrix4(
                (2.0f * PerspectiveFocus) / w, 0.0f, 0.0f, 0.0f,
                0.0f, (2.0f * PerspectiveFocus) / h, 0.0f, 0.0f,
                0.0f, 0.0f, (PerspectiveFar + PerspectiveNear) / (PerspectiveFar - PerspectiveNear), -1.0f,
                0.0f, 0.0f, (2.0f * PerspectiveFar * PerspectiveNear) / (PerspectiveFar - PerspectiveNear), 0.0f
            );
            view = new Matrix4(
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, -1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, -1.0f, 0.0f,
                -viewpoint.X, viewpoint.Y, viewpoint.Z, viewpoint.W
            );
            frustum.Multiply(ref view, out viewProjectionMatrix);

            screenMatrix = new Matrix4(
                w / 2.0f, 0, 0, 0,
                0, -h / 2.0f, 0, 0,
                0, 0, -1, 0,
                w / 2.0f, h / 2.0f, 0, 1
                );

            viewProjectionMatrix = r * viewProjectionMatrix;
            screenMatrix = screenMatrix * ri;
        }


        internal static bool removeModalScene(ModalScene scene)
        {
            var index = modalSceneList.IndexOf(scene);
            Debug.Assert(index >= 0);
            if (index < 0)
            {
                return false;
            }

            modalSceneList.RemoveAt(index);
            if (index == modalSceneList.Count)
            {
                if (index == 0)
                    OnFocusActiveSceneChanged(CurrentScene, scene);
                else
                    OnFocusActiveSceneChanged(modalSceneList[modalSceneList.Count - 1], scene);
            }
            needUpdateSceneLayersOrder = true;
            return true;
        }

        internal static void addModalScnene(ModalScene scene)
        {
            Debug.Assert(!modalSceneList.Contains(scene));

            modalSceneList.Add(scene);
            needUpdateSceneLayersOrder = true;
            Scene oldScene = CurrentScene;
            if (modalSceneList.Count > 1)
                oldScene = modalSceneList[modalSceneList.Count - 2];
            OnFocusActiveSceneChanged(scene, oldScene);
        }

        /// @if LANG_JA
        /// <summary>エフェクトを登録する。</summary>
        /// <param name="effect">登録するエフェクト</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Registers the effect.</summary>
        /// <param name="effect">Effect to register</param>
        /// @endif
        internal static void RegisterEffect(Effect effect)
        {
            if (effectUpdating)
            {
                addPendingEffects.Add(effect);
            }
            else
            {
                effects.Add(effect);
            }
            effect.startedTime = stopwatch.Elapsed;
        }

        /// @if LANG_JA
        /// <summary>エフェクトの登録を解除する。</summary>
        /// <param name="effect">解除するエフェクト</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Cancels registration of the effect.</summary>
        /// <param name="effect">Effect to cancel</param>
        /// @endif
        internal static void UnregisterEffect(Effect effect)
        {
            if (effectUpdating)
            {
                removePendingEffects.Add(effect);
            }
            else
            {
                effects.Remove(effect);
            }
        }

        /// @if LANG_JA
        /// <summary>エフェクトを更新する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the effect.</summary>
        /// @endif
        private static void UpdateEffect()
        {
            effectUpdating = true;
            foreach (Effect effect in effects)
            {
                effect.Update(elapsedTime);
            }
            effectUpdating = false;
            foreach (Effect effect in addPendingEffects)
            {
                effects.Add(effect);
            }
            addPendingEffects.Clear();
            foreach (Effect effect in removePendingEffects)
            {
                effects.Remove(effect);
            }
            removePendingEffects.Clear();
        }

        /// @if LANG_JA
        /// <summary>トランジションを更新する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Updates the transition.</summary>
        /// @endif
        private static void UpdateTransition()
        {
            if (IsTransition)
            {
                currentTransition.Update(elapsedTime);
            }
        }

        /// @if LANG_JA
        /// <summary>システムの経過時間を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the elapsed time of the system.</summary>
        /// @endif
        public static TimeSpan CurrentTime
        {
            get;
            private set;
        }

        /// @if LANG_JA
        /// <summary>全てのウィジェットの状態をリセットする。</summary>
        /// <remarks>ウィジェットに登録しているGestureDetectorの状態もリセットされる。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resets the states of all widgets.</summary>
        /// <remarks>The state of GestureDetector registered to a widget is also reset.</remarks>
        /// @endif
        public static void ResetStateAll()
        {
            if (CurrentScene != null)
            {
                CurrentScene.RootWidget.ResetState(true);
            }
            foreach (var scene in modalSceneList)
            {
                scene.RootWidget.ResetState(true);
            }
            capturingWidgets.Clear();
            hitWidgets.Clear();
            primaryIDs.Clear();
        }

        /// @if LANG_JA
        /// <summary>PixelBufferのキャッシュをクリアする。対象はTransition用のテクスチャ。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Clears the PixelBuffer cache. The target is the texture of Transition.</summary>
        /// @endif
        internal static void ClearBuffer()
        {
            if (transitionCurrentSceneTextureCache != null)
            {
                transitionCurrentSceneTextureCache.Dispose();
                transitionCurrentSceneTextureCache = null;
            }
            if (transitionNextSceneTextureCache != null)
            {
                transitionNextSceneTextureCache.Dispose();
                transitionNextSceneTextureCache = null;
            }
        }

        private static bool IsTransition
        {
            get { return NextScene != null && currentTransition != null; }
        }

        private static bool IsModalScene
        {
            get { return modalSceneList != null && modalSceneList.Count > 0; }
        }

        internal static bool IsDialogEffect
        {
            get;
            set;
        }

        internal static bool CheckTextureSizeCapacity(int width, int height)
        {
            return width <= GraphicsContext.Caps.MaxTextureSize && height <= GraphicsContext.Caps.MaxTextureSize;
        }

        /// @if LANG_JA
        /// <summary>クリップ（シザー）の領域を全画面（FrameBufferのサイズ）に設定する</summary>
        /// <remarks>クリップ領域のサイズが0以下の場合はクリップ領域は変更されない。x,yは左上原点。</remarks>
        /// <param name="x">x</param>
        /// <param name="y">y(左上原点)</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>false: クリップ領域のサイズが0</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the clip (scissor) area to full screen (FrameBuffer size)</summary>
        /// <remarks>The clip area will not be changed when the clip area size is 0 or less. The upper left is the origin of x,y.</remarks>
        /// <param name="x">x</param>
        /// <param name="y">y (upper left is the origin)</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>false: size of the clip area is 0</returns>
        /// @endif
        internal static bool SetClipRegion(float x, float y, float w, float h)
        {
            ImageRect orgRect = GraphicsContext.GetScissor();
            ImageRect newRect;

            FrameBuffer frameBuffer = UISystem.GraphicsContext.GetFrameBuffer();

            newRect = new ImageRect(
                (int)(x * PixelDensity + 0.5f),
                (int)(y * PixelDensity + 0.5f),
                (int)(w * PixelDensity + 0.5f),
                (int)(h * PixelDensity + 0.5f));

            if (newRect.Width < 1 || newRect.Height < 1)
            {
                return false;
            }

            switch (CurrentScreenOrientation)
            {
                case ScreenOrientation.Portrait:
                    newRect = new ImageRect(
                        newRect.Y,
                        frameBuffer.Height - newRect.X - newRect.Width,
                        newRect.Height,
                        newRect.Width);
                    break;
                case ScreenOrientation.ReverseLandscape:
                    newRect = new ImageRect(
                        frameBuffer.Width - newRect.X - newRect.Width,
                        frameBuffer.Height - newRect.Y - newRect.Height,
                        newRect.Width,
                        newRect.Height);
                    break;
                case ScreenOrientation.ReversePortrait:
                    newRect = new ImageRect(
                        frameBuffer.Width - newRect.Y - newRect.Height,
                        newRect.X,
                        newRect.Height,
                        newRect.Width);
                    break;
                case ScreenOrientation.Landscape:
                default:
                    break;
            }

            // invert y-axis
            if (!IsOffScreenRendering)
            {
                newRect.Y = frameBuffer.Height - newRect.Y - newRect.Height;
            }

            if (!newRect.Equals(orgRect))
            {
                GraphicsContext.SetScissor(newRect);
            }

            return true;
        }

        /// @if LANG_JA
        /// <summary>クリップ（シザー）の領域を全画面（FrameBufferのサイズ）に設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the clip (scissor) area to full screen (FrameBuffer size)</summary>
        /// @endif
        internal static void SetClipRegionFull()
        {
            FrameBuffer frameBuffer = UISystem.GraphicsContext.GetFrameBuffer();
            var orgRect = GraphicsContext.GetScissor();
            var newRect = new ImageRect(0, 0, frameBuffer.Width, frameBuffer.Height);

            if (!newRect.Equals(orgRect))
            {
                GraphicsContext.SetScissor(newRect);
            }
        }

        internal class ReversibleList<T> : List<T>
        {
            public IEnumerable<T> GetReverseOrder()
            {
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    yield return this[i];
                }
            }
        }

        /// @if LANG_JA
        /// <summary>キーが押されてからLongPressキーイベントが発行されるまでの時間（ミリ秒）を取得、設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the time (ms) from when a key is pressed to when a LongPress key event is issued</summary>
        /// @endif
        static public float KeyLongPressTime
        {
            get { return (float)keyLongPressTime.TotalMilliseconds; }
            set
            {
                if (value > 0.0f)
                    keyLongPressTime = TimeSpan.FromMilliseconds(value);
                else
                    keyLongPressTime = new TimeSpan();
            }
        }

        /// @if LANG_JA
        /// <summary>キーが押されてからRepeatキーイベントが初めて発行されるまでの時間（ミリ秒）を取得、設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the time (ms) from when a key is pressed to when a Repeat key event is first issued</summary>
        /// @endif
        static public float KeyRepeatStartTime
        {
            get { return (float)keyRepeatStartTime.TotalMilliseconds; }
            set
            {
                if (value > 0.0f)
                    keyRepeatStartTime = TimeSpan.FromMilliseconds(value);
                else
                    keyRepeatStartTime = new TimeSpan();
            }
        }

        /// @if LANG_JA
        /// <summary>Repeatキーイベントが発行される間隔（ミリ秒）を取得、設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the interval time (ms) when a Repeat key event is issued</summary>
        /// @endif
        static public float KeyRepeatIntervalTime
        {
            get { return (float)keyRepeatIntervalTime.TotalMilliseconds; }
            set
            {
                if (value > 0.0f)
                    keyRepeatIntervalTime = TimeSpan.FromMilliseconds(value);
                else
                    keyRepeatIntervalTime = new TimeSpan();
            }
        }

        /// @if LANG_JA
        /// <summary>無操作時にフォーカスを非アクティブにするまでの時間（ミリ秒）を取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the time (ms) taken to deactivate focus when not operating</summary>
        /// @endif
        static public float FocusTimeout
        {
            get { return focusTimeout; }
            set
            {
                if (value > 0.0f)
                    focusTimeout = value;
                else
                    focusTimeout = float.Epsilon;
            }
        }
        static private float focusTimeout = 5000;

        internal static bool AsyncLoadingSystemAsset
        {
            get { return AssetManager.AsyncLoadingSystemAsset; }
            set { AssetManager.AsyncLoadingSystemAsset = value; }
        }

        internal static ReversibleList<Scene> sceneLayers;
        private static List<ModalScene> modalSceneList;
        private static bool needUpdateSceneLayersOrder;
        private static Stack<Scene> navigationSceneStack;
        private static Stopwatch stopwatch;
        private static float elapsedTime;
        private static UIElement zSortList;
        private static List<Effect> effects;
        private static List<Effect> addPendingEffects;
        private static List<Effect> removePendingEffects;
        private static TouchEventCollection lowLevelTouchEvents;
        private static TouchEventCollection highLevelTouchEvents;
        private static Dictionary<Widget, TouchEventCollection> touchEventSets;
        private static Dictionary<Widget, int> primaryIDs;
        private static bool effectUpdating;
        private static Dictionary<int, Widget> capturingWidgets;
        private static Dictionary<int, Widget> hitWidgets;
        private static Widget previousFocusWidgt;
        private static KeyEventData[] keyEventDataList;
        private static TimeSpan keyLongPressTime = TimeSpan.FromMilliseconds(300);
        private static TimeSpan keyRepeatStartTime = TimeSpan.FromMilliseconds(200);
        private static TimeSpan keyRepeatIntervalTime = TimeSpan.FromMilliseconds(50);
        private static float focusTimeoutTimer;
        internal static FrameBuffer offScreenFramebufferCache;
        internal static bool IsOffScreenRendering;
        internal static Texture2D transitionNextSceneTextureCache;
        internal static Texture2D transitionCurrentSceneTextureCache;
        private static GraphicsState StoredGraphicsState;
        private static bool keyEventCancelled;

        // For N point hit test
        private static Vector2[] hitTestPointOffsets;
        private static UISprite hitTestPointSprt;
        private static readonly bool showHitTestDebugInfo = false;

        /// @if LANG_JA
        /// <summary>RenderのときのGraphicsContextのStateを覚えておくためのクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Class for remembering the State of the GraphicsContext at the time of Render</summary>
        /// @endif
        private class GraphicsState
        {
            public bool EnableModeBlend;
            public bool EnableModeCullFace;
            public bool EnableModeDepthTest;
            //public bool EnableModeDither;
            //public bool EnableModePolygonOffsetFill;
            public bool EnableModeScissorTest;
            public bool EnableModeStencilTest;
            public ColorMask ColorMask;
            public CullFace CullFace;
            public BlendFunc BlendFuncRgb;
            public BlendFunc BlendFuncAlpha;
            public Vector4 ClearColor;
            public readonly CullFace DefaultCullFace =
                new CullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
            public readonly BlendFunc DefaultBlendFunc =
                new BlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
        }

        private class KeyEventData
        {
            public bool Active;
            public KeyType KeyType;
            public GamePadButtons SingleGamePadButton;
            public TimeSpan KeyDownTime;
            public TimeSpan NextKeyLongPressTime;
            public TimeSpan NextKeyRepeatTime;
            public bool EnabledEachFrameRepeat;
        }

        #region focus

        private static bool focusActive;

        /// @if LANG_JA
        /// <summary>フォーカスが現在アクティブかどうかを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether the focus is currently active</summary>
        /// @endif
        public static bool FocusActive
        {
            get { return focusActive; }
            set
            {
                UIDebug.LogFocusIf(focusActive != value, "FocusActive=" + value.ToString());

                focusActive = value;
                setFocusVisible(value, true);
                SuppressFocusKeyEvent = false;

                // reset focus timeout
                if(value == true)
                    focusTimeoutTimer = 0;
            }
        }

        internal static Scene FocusActiveScene
        {
            get
            {
                // TODO:FOCUS only CurrentScene and ModalScene?
                if(IsModalScene)
                    return modalSceneList[modalSceneList.Count - 1];
                else
                    return currentScene;
            }
        }

        private static void OnFocusActiveSceneChanged(Scene newScene, Scene oldScene)
        {
            focusScene.ScreenOrientation = newScene.ScreenOrientation;
            if (newScene.FocusWidget != null || !(newScene.FocusWidget is RootWidget))
            {
                newScene.FocusWidget.SetFocus(false);
                oldScene.FocusWidget.OnFocusChanged(
                    new FocusChangedEventArgs(false, newScene.FocusWidget, oldScene.FocusWidget));
                newScene.FocusWidget.OnFocusChanged(
                    new FocusChangedEventArgs(true, newScene.FocusWidget, oldScene.FocusWidget));
            }
        }

        /// @if LANG_JA
        /// <summary>フォーカス機能を有効にするかどうかを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to enable the focus feature</summary>
        /// @endif
        public static bool EnabledFocus
        {
            get { return enabledFocus && enabledFocusAtCurrentFrame; }
            set { enabledFocus = value; }
        }
        private static bool enabledFocus;
        internal static bool enabledFocusAtCurrentFrame;

        private static Widget getCurrentFocusWidget()
        {
            return getCurrentFocusableScene().FocusWidget;
        }

        private static Scene getCurrentFocusableScene()
        {
            foreach (var scene in sceneLayers)
            {
                if (scene != null && scene.Focusable)
                    return scene;
            }
            return blankScene;
        }

        internal static void setFocusVisible(bool visible, bool animation)
        {
            if(!visible || (EnabledFocus && focusActive))
            {
                focusScene.SetVisible(visible, animation);
            }
        }

        internal static bool focusVisible
        {
            get
            {
                return focusScene.FocusVisible;
            }
        }

        internal static void setFocusSizeAndImage(Rectangle rect,FocusStyle style,
            FocusCustomSettings settings, bool animation)
        {
            if (EnabledFocus)
            {
                focusScene.SetPosition(rect, animation);

                var image = (settings == null) ? null : settings.FocusImage;
                var ninepatch = (image == null) ? NinePatchMargin.Zero : settings.FocusImageNinePatchMargin;
                var padding = (image == null) ? 0 : settings.FocusImagePadding;

                focusScene.SetImage(style, image, ninepatch, padding, animation);
                
                //var visible = (settings == null) ? true : !settings.HideFocusImage;
                //if (!visible || focusActive)
                //    focusScene.SetVisible(visible, animation);
            }
        }

        private static bool suppressFocusKeyEvent;

        /// @if LANG_JA
        /// <summary>フォーカスのキーイベント配信を一時停止させるかどうかを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to pause the key event distribution of the focus</summary>
        /// @endif
        public static bool SuppressFocusKeyEvent
        {
            get { return suppressFocusKeyEvent; }
            set { suppressFocusKeyEvent = value; }
        }


        static FocusScene focusScene;

        /// @if LANG_JA
        /// <summary>フォーカスイメージに乗算する色を取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the color that multiplies in the focus image</summary>
        /// @endif
        public static UIColor FocusFilterColor
        {
            get
            {
                if (focusScene == null)
                    return FocusScene.defaultFilterColor;
                return focusScene.FilterColor;
            }
            set
            {
                if (focusScene != null)
                    focusScene.FilterColor = value;
            }
        }

        /// @if LANG_JA
        /// <summary>フォーカスイメージのブレンドモードを取得・設定する</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the blend mode of the focus image</summary>
        /// @endif
        public static BlendMode FocusBlendMode
        {
            get
            {
                if (focusScene == null)
                    return FocusScene.defaultBlendMode;
                return focusScene.BlendMode;
            }
            set
            {
                if (focusScene != null)
                    focusScene.BlendMode = value;
            }
        }

        #endregion

    }
}
