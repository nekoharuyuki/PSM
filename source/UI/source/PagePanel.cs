/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>ページ単位でスクロールするコンテナウィジェット</summary>
        /// <remarks>複数のパネルで構成される。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>A container widget that scrolls by page</summary>
        /// <remarks>Made up of multiple panels.</remarks>
        /// @endif
        public class PagePanel : Widget
        {

            private const float defaultPagePanelPointWidth = 24.0f;
            private const float defaultPagePanelPointHeight = 24.0f;

            private enum AnimationState
            {
                None = 0,
                Drag,
                Flick
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PagePanel()
            {
                base.Width = UISystem.FramebufferWidth;
                base.Height = UISystem.FramebufferHeight;

                this.panelContainer = new ContainerWidget();
                this.panelContainer.Clip = false;
                this.panelContainer.EnabledChildrenAnchors = false;
                this.AddChildLast(this.panelContainer);
                this.sprtContainer = new ContainerWidget();
                this.sprtContainer.Clip = false;
                this.sprtContainer.EnabledChildrenAnchors = false;
                this.AddChildLast(this.sprtContainer);
                this.panelList = new List<Panel>();
                this.sprtList = new List<UISprite>();
                this.pageCount = 0;
                this.pageIndex = -1;
                this.Clip = true;
                this.HookChildTouchEvent = true;
                this.Focusable = true;
                this.avoidFocusFromChildren = true;
                this.state = AnimationState.None;
                this.activeImage = new ImageAsset(SystemImageAsset.PagePanelActive);
                this.normalImage = new ImageAsset(SystemImageAsset.PagePanelNormal);
                this.FocusStyle = FocusStyle.Rectangle;

                DragGestureDetector drag = new DragGestureDetector();
                drag.Direction = DragDirection.Horizontal;
                drag.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(drag);

                FlickGestureDetector flick = new FlickGestureDetector();
                flick.Direction = FlickDirection.Horizontal;
                flick.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flick);

                updateSize();
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (this.activeImage != null)
                {
                    this.activeImage.Dispose();
                    this.activeImage = null;
                }

                if (this.normalImage != null)
                {
                    this.normalImage.Dispose();
                    this.normalImage = null;
                }

                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>ページ数を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the number of pages.</summary>
            /// @endif
            public int PageCount
            {
                get { return pageCount; }
            }

            /// @if LANG_JA
            /// <summary>現在のページ番号を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the current page number.</summary>
            /// @endif
            public int CurrentPageIndex
            {
                get
                {
                    return pageIndex;
                }
                set
                {
                    ScrollTo(value, false);
                }
            }

            /// @if LANG_JA
            /// <summary>末尾のページを追加する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds an end page.</summary>
            /// @endif
            public int AddPage()
            {
                return InsertPage(pageCount, new Panel());
            }

            /// @if LANG_JA
            /// <summary>末尾のページを追加する。</summary>
            /// <param name="panel">追加するPanelオブジェクト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds an end page.</summary>
            /// <param name="panel">Panel object to be added</param>
            /// @endif
            public int AddPage(Panel panel)
            {
                return InsertPage(pageCount, panel);
            }

            /// @if LANG_JA
            /// <summary>指定したページ番号の位置にパネルを挿入する。</summary>
            /// <param name="index">ページを挿入する位置（０～）</param>
            /// <param name="panel">挿入するページのPanelオブジェクト</param>
            /// <exception cref="ArgumentOutOfRangeException">indexが0未満かPageCountより大きい</exception>
            /// <exception cref="ArgumentNullException">panelがnull</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts a panel at the position of the specified page number.</summary>
            /// <param name="index">Position for inserting a page (0 - )</param>
            /// <param name="panel">Panel object of page to be inserted</param>
            /// <exception cref="ArgumentOutOfRangeException">Index is less than 0 or greater than PageCount</exception>
            /// <exception cref="ArgumentNullException">Panel is null</exception>
            /// @endif
            public int InsertPage(int index, Panel panel)
            {
                if (index < 0 || index > this.pageCount)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (panel == null)
                {
                    throw new ArgumentNullException("panel");
                }

                panel.Y = 0.0f;
                panel.Width = this.Width;
                panel.Height = this.Height;
                panel.FocusStyle = FocusStyle.Rectangle;
                if (index > 0)
                {
                    this.panelContainer.InsertChildBefore(panel, this.panelList[index - 1]);
                }
                else
                {
                    this.panelContainer.AddChildFirst(panel);
                }
                this.panelList.Insert(index, panel);

                updatePagePos();

                UISprite sprt = new UISprite(1);
                sprt.ShaderType = ShaderType.Texture;
                sprt.Image = this.normalImage;
                UISpriteUnit unit = sprt.GetUnit(0);
                unit.X = defaultPagePanelPointWidth * this.pageCount;
                unit.Y = 0.0f;
                unit.Width = defaultPagePanelPointWidth;
                unit.Height = defaultPagePanelPointHeight;
                this.sprtContainer.RootUIElement.AddChildLast(sprt);
                this.sprtList.Add(sprt);

                this.pageCount++;
                if (this.pageIndex >= index || this.pageIndex == -1)
                {
                    this.CurrentPageIndex++;
                }

                UpdateSprite();
                
                return index;
            }

            /// @if LANG_JA
            /// <summary>指定したページ番号の位置にページを挿入する。</summary>
            /// <param name="index">ページを挿入する位置（０～）</param>
            /// <exception cref="ArgumentOutOfRangeException">indexが0未満かPageCountより大きい</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts a page at the position of the specified page number.</summary>
            /// <param name="index">Position for inserting a page (0 - )</param>
            /// <exception cref="ArgumentOutOfRangeException">Index is less than 0 or greater than PageCount</exception>
            /// @endif
            public void InsertPage(int index)
            {
                InsertPage(index, new Panel());
            }

            /// @if LANG_JA
            /// <summary>指定したページを削除する</summary>
            /// <param name="panel">削除するページ</param>
            /// <returns>panelが正常に削除された場合はtrue。それ以外はfalse。panelが見つからなかった場合もfalseを返す。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified page</summary>
            /// <param name="panel">Page to be deleted</param>
            /// <returns>If panel is deleted correctly, then true. Otherwise, false. If panel cannot be found, returns false.</returns>
            /// @endif
            public bool RemovePage(Panel panel)
            {
                int index = this.panelList.IndexOf(panel);

                if (index < 0)
                {
                    return false;
                }
                else
                {
                    RemovePageAt(index);
                    return true;
                }
            }

            /// @if LANG_JA
            /// <summary>指定したページ番号のページを削除する。</summary>
            /// <param name="index">削除するページ番号(０～)</param>
            /// <exception cref="ArgumentOutOfRangeException">indexが0未満かPageCount以上</exception>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the page of the specified page number.</summary>
            /// <param name="index">Page number to be deleted (0 - )</param>
            /// <exception cref="ArgumentOutOfRangeException">Index is less than 0 or equal to or greater than PageCount</exception>
            /// @endif
            public void RemovePageAt(int index)
            {
                if (index < 0 || index >= this.pageCount)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                Panel panel = this.panelList[index];
                this.panelContainer.RemoveChild(panel);
                this.panelList.RemoveAt(index);

                updatePagePos();

                this.sprtList[this.pageCount - 1].Dispose();
                this.sprtList.RemoveAt(this.pageCount - 1);

                this.pageCount--;
                if (this.pageIndex > index || this.pageIndex == this.pageCount)
                {
                    this.CurrentPageIndex--;
                }

                UpdateSprite();
            }

            private void UpdateSprite()
            {
                for (int i = 0; i < this.pageCount; i++)
                {
                    this.sprtList[i].Image = (this.pageIndex == i) ? activeImage : normalImage;
                }

                this.sprtContainer.X = (this.Width - defaultPagePanelPointWidth * this.pageCount) / 2.0f;
                this.sprtContainer.Y = this.Height - defaultPagePanelPointHeight;
            }

            private void updatePagePos()
            {
                panelContainer.SetSize(this.Width * this.panelList.Count, this.Height);
                for (int i = 0; i < this.panelList.Count; i++)
                {
                    this.panelList[i].SetPosition(this.Width * i, 0.0f);
                }
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// @endif
            public override float Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;
                    updateSize();
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// @endif
            public override float Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;
                    updateSize();
                }
            }

            void updateSize()
            {
                if (panelList != null && sprtContainer != null)
                {
                    for (int i = 0; i < this.panelList.Count; i++)
                    {
                        this.panelList[i].SetSize(this.Width, this.Height);
                        this.panelList[i].SetPosition(this.Width * i, 0.0f);
                    }
                    updatePagePos();
                    this.panelContainer.X = -(this.Width * this.pageIndex);
                    this.sprtContainer.X = (this.Width - defaultPagePanelPointWidth * this.pageCount) / 2.0f;
                    this.sprtContainer.Y = this.Height - defaultPagePanelPointHeight;
                }
            }

            /// @if LANG_JA
            /// <summary>指定したページのパネルを取得する。</summary>
            /// <param name="index">取得するページのインデックス</param>
            /// <returns>パネル</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the panel of a specified page.</summary>
            /// <param name="index">Index of page to be obtained</param>
            /// <returns>Panel</returns>
            /// @endif
            public Panel GetPage(int index)
            {
                return this.panelList[index];
            }

            private void setCurrentPos(float distance)
            {
                this.panelContainer.X = this.startPos + distance;

                float firstPos = 0.0f;
                float lastPos = -this.Width * (this.pageCount - 1);

                if (this.panelContainer.X > firstPos)
                {
                    this.panelContainer.X = firstPos;
                }
                else if (this.panelContainer.X < lastPos)
                {
                    this.panelContainer.X = lastPos;
                }

                int leftIndex = -(int)(this.panelContainer.X / this.Width);

                int prevIndex = this.pageIndex;
                this.pageIndex = (int)(0.5f - (this.panelContainer.X / this.Width));

                if (prevIndex != this.pageIndex)
                {
                    UpdateSprite();
                }

                for (int i = 0; i < this.pageCount; i++)
                {
                    this.panelList[i].Visible = (i == leftIndex || i == (leftIndex + 1));
                }
            }

            /// @if LANG_JA
            /// <summary>指定したページの位置までスクロールする。</summary>
            /// <param name="index">ページのインデックス</param>
            /// <param name="withAnimation">アニメーションするかどうか</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Scrolls to the specified page position.</summary>
            /// <param name="index">Index of page</param>
            /// <param name="withAnimation">Whether to execute the animation</param>
            /// @endif
            public void ScrollTo(int index, bool withAnimation)
            {
                if (index >= this.pageCount)
                {
                    index = this.pageCount - 1;
                }
                else if (index < 0)
                {
                    index = 0;
                }

                if (withAnimation)
                {
                    this.animationStartPos = this.panelContainer.X;
                    this.animationElapsedTime = 0f;
                    this.nextPos = -(this.Width * index);
                    this.animation = true;
                }
                else
                {
                    this.pageIndex = index;
                    this.panelContainer.X = -(this.Width * this.pageIndex);
                    this.animation = false;
                    SetupPageVisible();
                    UpdateSprite();

                    OnPageIndexChanged();
                }
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
                    animationElapsedTime += elapsedTime;

                    this.panelContainer.X = (animationStartPos - nextPos) * (float)Math.Exp(-animationElapsedTime * (0.2f / 16.67f)) + nextPos;

                    int leftIndex = -(int)(this.panelContainer.X / this.Width);
                    for (int i = 0; i < this.pageCount; i++)
                    {
                        this.panelList[i].Visible = (i == leftIndex || i == (leftIndex + 1));
                    }

                    float offsetX = this.nextPos - this.panelContainer.X;

                    this.pageIndex = (int)(0.5f - (this.panelContainer.X / this.Width));
                    UpdateSprite();

                    if (offsetX < 1.0f && offsetX > -1.0f)
                    {
                        this.panelContainer.X = this.nextPos;
                        this.animation = false;
                        this.state = AnimationState.None;

                        SetupPageVisible();

                        OnPageIndexChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>タッチイベントハンドラ</summary>
            /// <param name="touchEvents">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Touch event handler</summary>
            /// <param name="touchEvents">Touch event</param>
            /// @endif
            protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
            {
                base.OnTouchEvent(touchEvents);

                TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

                if (touchEvent.Type == TouchEventType.Down)
                {
                    this.animation = false;
                    this.startPos = this.panelContainer.X;
                    this.touchDownLocalPos = touchEvent.LocalPosition.X;
                    this.state = AnimationState.None;
                }

                if (touchEvent.Type == TouchEventType.Up && this.state != AnimationState.Flick)
                {
                    ScrollTo(this.pageIndex, true);
                }

                if (this.state != AnimationState.Drag &&
                    this.state != AnimationState.Flick)
                {
                    touchEvents.Forward = true;
                }
            }

            /// @if LANG_JA
            /// <summary>状態リセットハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Status reset handler</summary>
            /// @endif
            protected internal override void OnResetState()
            {
                base.OnResetState();
                ScrollTo(this.pageIndex, true);
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                ResetState(false);
                if(this.state != AnimationState.Drag)
                {
                    this.touchDownLocalPos = e.LocalPosition.X;
                    this.state = AnimationState.Drag;
                }
                else
                {
                    setCurrentPos(e.LocalPosition.X - this.touchDownLocalPos);
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                ResetState(false);
                this.state = AnimationState.Flick;

                int leftIndex = -(int)(this.panelContainer.X / this.Width);
                if (e.Speed.X < 0)
                {
                    ScrollTo(leftIndex + 1, true);
                }
                else
                {
                    ScrollTo(leftIndex, true);
                }
                this.animation = true;
            }

            private void SetupPageVisible()
            {
                for (int i = 0; i < this.pageCount; i++)
                {
                    this.panelList[i].Visible = this.pageIndex == i;
                }
            }

            /// @if LANG_JA
            /// <summary>ページ番号がかわり、新しいページが表示されきった時に呼ばれる</summary>
            /// <remarks>連続でフリックした場合などは、アニメーションが完了した最後のタイミングで呼ばれる</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Called when the page number changes and a new page is completely displayed</summary>
            /// <remarks>Called when the animation is completed, such as when flicking continuously</remarks>
            /// @endif
            private void OnPageIndexChanged()
            {
                if (lastPageIndex != pageIndex)
                {
                    var scene = ParentScene;
                    if (scene != null)
                    {
                        var w = scene.FocusWidget;
                        int focusPage = -1;
                        if (w != null)
                        {
                            for (int i = 0; i < this.panelList.Count; i++)
                            {
                                if (w.belongTo(this.panelList[i]) || w == this.panelList[i])
                                {
                                    focusPage = i;
                                    break;
                                }
                            }
                        }

                        if (focusPage >= 0 && focusPage != this.pageIndex)
                        {
                            FourWayDirection direction = focusPage < this.pageIndex ? FourWayDirection.Right : FourWayDirection.Left;
                            var nextFocusWidget = this.panelList[this.pageIndex].SearchNextFocusFromChild(direction, w);
                            if(UISystem.FocusActive)
                                UISystem.setFocusVisible(true, true);
                            if (nextFocusWidget != null)
                            {
                                nextFocusWidget.SetFocus(false);
                            }
                            else
                            {
                                this.panelList[pageIndex].SetFocus(false);
                            }
                        }
                    }

                    lastPageIndex = pageIndex;
                }
            }

            /// @if LANG_JA
            /// <summary>事前キーイベントのハンドラ</summary>
            /// <param name="keyEvent">キーイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Advance key event handler</summary>
            /// <param name="keyEvent">Key event</param>
            /// @endif
            internal protected override void OnPreviewKeyEvent(KeyEvent keyEvent)
            {
                // TODO:FOCUS merge ScrollPanel and GridListPanel impl
                if (keyEvent.KeyEventType == KeyEventType.Down ||
                    keyEvent.KeyEventType == KeyEventType.Repeat)
                {
                    FourWayDirection direction = FourWayDirection.Down;
                    if (KeyEvent.TryConvertToDirection(keyEvent.KeyType, ref direction))
                    {
                        var scene = ParentScene;
                        if (scene == null)
                            return;

                        var w = this.SearchNextFocusFromChild(direction, scene.FocusWidget);

                        if (w != null && w.getClippedState() == ClippedState.NoClipped)
                        {
                            w.SetFocus(true);
                            keyEvent.Handled = true;
                        }
                        else
                        {
                            if (this.Focused && this.pageIndex >= 0 && this.PageCount > 0)
                            {
                                this.panelList[this.pageIndex].SetFocus(false);
                            }

                            bool moved = scrollByFourWayKey(direction);
                            if (moved)
                            {
                                UISystem.SuppressFocusKeyEvent = true;
                                UISystem.setFocusVisible(false, true);

                                keyEvent.Handled = true;
                            }
                        }
                    }
                }

                base.OnPreviewKeyEvent(keyEvent);
            }

            bool scrollByFourWayKey(FourWayDirection direction)
            {
                switch (direction)
                {
                    case FourWayDirection.Left:
                        if (this.CurrentPageIndex > 0)
                        {
                            ScrollTo(this.CurrentPageIndex - 1, true);
                            return true;
                        }
                        break;
                    case FourWayDirection.Right:
                        if (this.CurrentPageIndex < this.PageCount - 1)
                        {
                            ScrollTo(this.CurrentPageIndex + 1, true);
                            return true;
                        }
                        break;
                }
                return false;
            }

            /// @if LANG_JA
            /// <summary>フォーカスが当たるかどうかを取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to set the focus</summary>
            /// @endif
            public override bool Focusable
            {
                get
                {
                    return base.Focusable && PageCount > 1;
                }
                set
                {
                    base.Focusable = value;
                }
            }


            private ContainerWidget panelContainer;
            private ContainerWidget sprtContainer;
            private List<Panel> panelList;
            private List<UISprite> sprtList;
            private int pageCount;
            private int pageIndex;
            private int lastPageIndex;
            private float startPos;
            private float nextPos;
            private float touchDownLocalPos;
            private AnimationState state;
            private ImageAsset activeImage;
            private ImageAsset normalImage;
            private bool animation = false;
            private float animationElapsedTime;
            private float animationStartPos;
        }


    }
}
