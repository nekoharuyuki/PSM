/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>複数の行を持つリストウィジェット</summary>
        /// <remarks>グループ化機能とインデックス機能がある。アイテムの内容を自由にカスタマイズできる。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>List widget with multiple rows</summary>
        /// <remarks>A grouping feature and index feature are available. Item details can be customized.</remarks>
        /// @endif
        public class ListPanel : ContainerWidget
        {
            private const float sectionHeightRate = 1.9f;// default section height is 47 
            private const float focusMoveMargin = 30;

            private enum AnimationState
            {
                None = 0,
                Drag,
                Flick
            }

            private enum TerminalState
            {
                None,
                Top,
                Bottom,
                TopReset,
                BottomReset
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ListPanel()
            {
                sectionBackgroundColor = new UIColor(0.5f, 0.5f, 0.5f, 0.8f);
                sectionFont = new UIFont();
                sectionTextColor = new UIColor(1f, 1f, 1f, 1f);
                
                backgroundSprt = new UISprite(1);
                RootUIElement.AddChildLast(backgroundSprt);
                UISpriteUnit unit = backgroundSprt.GetUnit(0);
                unit.Color = new UIColor(0.0f, 0.0f, 0.0f, 0.0f);

                scrollBar = new ScrollBar(ScrollBarOrientation.Vertical);
                AddChildLast(scrollBar);

                displayAllItems = new List<ListPanelItem>();
                displayListItems = new List<ListPanelItem>();
                cacheListItems = new List<ListPanelItem>();
                displaySectionItems = new List<ListPanelSectionItem>();
                cacheSectionItems = new List<ListPanelSectionItem>();

                animationState = AnimationState.None;
                terminalState = TerminalState.None;

                terminalDistance = 0.0f;

                Sections = null;
                ShowSection = true;
                ShowEmptySection = true;
                ShowItemBorder = true;

                Clip = true;
                HookChildTouchEvent = true;
                EnabledChildrenAnchors = false;

                DragGestureDetector drag = new DragGestureDetector();
                drag.Direction = DragDirection.Vertical;
                drag.DragDetected += new EventHandler<DragEventArgs>(DragEventHandler);
                this.AddGestureDetector(drag);

                FlickGestureDetector flick = new FlickGestureDetector();
                flick.Direction = FlickDirection.Vertical;
                flick.FlickDetected += new EventHandler<FlickEventArgs>(FlickEventHandler);
                this.AddGestureDetector(flick);
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

                animationState = AnimationState.None;

                if (terminalState == TerminalState.Top)
                {
                    terminalState = TerminalState.TopReset;
                }
                else if (terminalState == TerminalState.Bottom)
                {
                    terminalState = TerminalState.BottomReset;
                }

                animation = (terminalState != TerminalState.None);

                if (this.ScrollBarVisibility == ScrollBarVisibility.ScrollingVisible &&
                    animation == false)
                {
                    this.scrollBar.Visible = false;
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

                    if (backgroundSprt != null)
                    {
                        backgroundSprt.GetUnit(0).Width = value;
                    }

                    if (scrollBar != null)
                    {
                        scrollBar.X = value - scrollBar.Width;
                    }
                    
                    if (displayAllItems != null)
                    {
                        foreach (ListPanelItem item in displayAllItems)
                        {
                            if (item != null)
                            {
                                item.Width = value;
                            }
                        }
                    }
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

                    if (backgroundSprt != null)
                    {
                        backgroundSprt.GetUnit(0).Height = value;
                    }

                    if (scrollBar != null)
                    {
                        scrollBar.Height = value;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>背景色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background color.</summary>
            /// @endif
            public UIColor BackgroundColor
            {
                get
                {
                    return backgroundSprt.GetUnit(0).Color;
                }
                set
                {
                    backgroundSprt.GetUnit(0).Color = value;
                }
            }

            /// @if LANG_JA
            /// <summary>セクションのコレクションを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the collection of a section.</summary>
            /// @endif
            public ListSectionCollection Sections
            {
                get
                {
                    return sections;
                }
                set
                {
                    sections = value;
                    if (sections != null)
                    {
                        sections.ItemsChanged += new EventHandler<EventArgs>(SectionsItemChanged);
                        Refresh();
                    }
                }
            }
            private ListSectionCollection sections;

            private void SectionsItemChanged(object sender, EventArgs e)
            {
                Refresh();
            }

            /// @if LANG_JA
            /// <summary>アイテムの総数を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the total number of items.</summary>
            /// @endif
            public int AllItemCount
            {
                get
                {
                    if(sections != null)
                        return Sections.AllItemCount;
                    else
                        return 0;
                }
            }

            /// @if LANG_JA
            /// <summary>セクションを表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display a section.</summary>
            /// @endif
            public bool ShowSection
            {
                get
                {
                    return showSection;
                }
                set
                {
                    if (showSection != value)
                    {
                        showSection = value;
                        Refresh();
                    }
                }
            }
            private bool showSection;

            /// @if LANG_JA
            /// <summary>空のセクションを表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display empty sections.</summary>
            /// @endif
            public bool ShowEmptySection
            {
                get
                {
                    return showEmptySection;
                }
                set
                {
                    if (showEmptySection != value)
                    {
                        showEmptySection = value;
                        Refresh();
                    }
                }
            }
            private bool showEmptySection;

            /// @if LANG_JA
            /// <summary>アイテムの境界画像を表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display the border images of an item.</summary>
            /// @endif
            public bool ShowItemBorder
            {
                get
                {
                    return showItemBorder;
                }
                set
                {
                    if (showItemBorder != value)
                    {
                        showItemBorder = value;
                        Refresh();
                    }
                }
            }
            private bool showItemBorder;

            /// @if LANG_JA
            /// <summary>スクロールバーの見え方を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the scroll bar view.</summary>
            /// @endif
            public ScrollBarVisibility ScrollBarVisibility
            {
                get
                {
                    return this.scrollBarVisibility;
                }
                set
                {
                    this.scrollBarVisibility = value;
                    UpdateScrollBarVisible();
                }
            }
            private ScrollBarVisibility scrollBarVisibility = ScrollBarVisibility.ScrollableVisible;

            private UIFont sectionFont;

            /// @if LANG_JA
            /// <summary>セクションのフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the section font.</summary>
            /// @endif
            public UIFont SectionFont
            {
                get { return sectionFont; }
                set
                {
                    if (sectionFont != value)
                    {
                        sectionFont = value;
                        if (ShowSection)
                            Refresh();
                        else
                        {
                            foreach (var item in cacheSectionItems)
                            {
                                item.Label.Font = value ?? new UIFont();
                                item.Height = getSectionHeight();
                            }
                        }
                    }
                }
            }

            private UIColor sectionTextColor;

            /// @if LANG_JA
            /// <summary>セクションのテキストカラーを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the section text color.</summary>
            /// @endif
            public UIColor SectionTextColor
            {
                get { return sectionTextColor; }
                set
                {
                    if (sectionTextColor != value)
                    {
                        sectionTextColor = value;
                        foreach (var item in cacheSectionItems)
                            item.Label.TextColor = value;
                        foreach (var item in displaySectionItems)
                            item.Label.TextColor = value;
                    }
                }
            }

            private UIColor sectionBackgroundColor;

            /// @if LANG_JA
            /// <summary>セクションの背景色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the section background color.</summary>
            /// @endif
            public UIColor SectionBackgroundColor
            {
                get { return sectionBackgroundColor; }
                set
                {
                    if (sectionBackgroundColor != value)
                    {
                        sectionBackgroundColor = value;
                        foreach (var item in cacheSectionItems)
                            item.BackgroundColor = value;
                        foreach (var item in displaySectionItems)
                            item.BackgroundColor = value;
                    }
                }
            }

            private void Refresh()
            {
                if (displayAllItems.Count > 0)
                {
                    var temp = displayAllItems.ToArray();
                    foreach (var item in temp)
                    {
                        DestroyListItem(item);
                    }
                    UIDebug.Assert(displayAllItems.Count == 0, "displayAllItems was broken.");
                }

                animationState = AnimationState.None;
                terminalState = TerminalState.None;
                animation = false;
            }


            private void UpdateScrollBar()
            {
                if (displayListItems.Count > 0)
                {
                    // Set standard list item height, if standardItemHeight has been initialized. ( = -1)
                    if (standardItemHeight < 0f)
                        standardItemHeight = displayListItems[0].Height;

                    int curIndex = displayListItems[0].Index + (ShowSection == true ? displayListItems[0].SectionIndex + 1 : 0);
                    foreach (var item in displayAllItems)
                    {
                        if (item != null && IsSectionItem(item))
                        {
                            curIndex -= 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // To calcurate actual BarPosition, set Length and BarLength before set BarPosition.
                    scrollBar.Length = AllItemCount + (ShowSection == true ? Sections.Count : 0);
                    scrollBar.BarLength = this.Height / (standardItemHeight + Math.Abs(terminalDistance) * terminalDistanceRatio);
                    scrollBar.BarPosition = (float)curIndex - displayAllItems[0].Y / displayAllItems[0].Height;
                    AddChildLast(scrollBar);
                }
                UpdateScrollBarVisible();
            }

            private void UpdateScrollBarVisible()
            {
                switch (this.scrollBarVisibility)
                {
                    case ScrollBarVisibility.Visible:
                        this.scrollBar.Visible = true;
                        break;
                    case ScrollBarVisibility.ScrollableVisible:
                        if (displayListItems.Count < AllItemCount)
                        {
                            this.scrollBar.Visible = true;
                        }
                        else if (displayListItems.Count == AllItemCount)
                        {
                            // Calcurate sum of displaying items height, and compare it to scroll bar height.
                            // Because, displayListItems contains item that was displayed only part of it.
                            var displayItemsHeightSum = 0f;
                            foreach (var item in displayListItems)
                            {
                                displayItemsHeightSum += item.Height;
                            }
                            this.scrollBar.Visible = scrollBar.Height < displayItemsHeightSum;
                        }
                        else
                        {
                            this.scrollBar.Visible = false;
                        }
                        break;
                    case ScrollBarVisibility.ScrollingVisible:
                        this.scrollBar.Visible = (animationState != AnimationState.None) || (terminalState != TerminalState.None);
                        break;
                    case ScrollBarVisibility.Invisible:
                        this.scrollBar.Visible = false;
                        break;
                }
            }

            private ListPanelItem CreateListItem()
            {
                ListPanelItem item = null;

                if (cacheListItems.Count == 0)
                {
                    if (itemCreator != null)
                    {
                        item = itemCreator();
                    }
                    else
                    {
                        item = new ListPanelItem();
                    }

                    if (item != null)
                    {
                        item.PressStateChanged += itemPressStateChangedEventHandler;
                        item.IndexInSection = 0;
                        AddChildLast(item);
                    }
                }
                else
                {
                    item = cacheListItems[0];
                    cacheListItems.RemoveAt(0);
                }

                item.Width = Width;
                item.HideEdge = !ShowItemBorder;
                item.Visible = true;
                item.FocusStyle = FocusStyle.ListItem;

                return item;
            }

            void itemPressStateChangedEventHandler(object sender, PressStateChangedEventArgs e)
            {
                UIDebug.Assert(sender is ListPanelItem);

                var item = sender as ListPanelItem;
                if (itemUpdater != null)
                {
                    itemUpdater(item);
                }
                if (e.checkDetectedButtonAction() && SelectItemChanged != null)
                {
                    SelectItemChanged(this, new ListPanelItemSelectChangedEventArgs(item));
                }
            }

            /// @if LANG_JA
            /// <summary>アイテムが選択された場合に呼び出されるイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event called when item is selected</summary>
            /// @endif
            public event EventHandler<ListPanelItemSelectChangedEventArgs> SelectItemChanged;

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
                // TODO:Focus marge ScrollPanel, GridListPanel
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

                        if (w == null || w.getClippedState() != ClippedState.NoClipped)
                        {
                            bool moved = scrollByFourWayKey(direction);
                            if (moved)
                            {
                                w = this.SearchNextFocusFromChild(direction, scene.FocusWidget);
                                if (w != null)
                                {
                                    w.SetFocus(true);
                                }
                                keyEvent.Handled = true;
                            }
                        }
                    }
                }

                base.OnPreviewKeyEvent(keyEvent);
            }

            private bool scrollByFourWayKey(FourWayDirection direction)
            {
                if (direction == FourWayDirection.Down)
                {
                    ListPanelItem item;
                    bool moved = true;
                    do
                    {
                        item = displayAllItems[displayAllItems.Count - 1];

                        UpdateItems(this.Height - (item.Y + item.Height + focusMoveMargin));


                        if (this.terminalState == TerminalState.Bottom ||
                            this.terminalState == TerminalState.BottomReset)
                        {
                            ResetBottomTerminalItems();
                            moved = false;
                            break;
                        }
                    } while (IsSectionItem(item));
                    return moved;
                }
                else if (direction == FourWayDirection.Up)
                {
                    ListPanelItem item;
                    bool moved = true;
                    do
                    {
                        item = displayAllItems[0];

                        UpdateItems(-item.Y + focusMoveMargin);


                        if (this.terminalState == TerminalState.Top ||
                            this.terminalState == TerminalState.TopReset)
                        {
                            ResetTopTerminalItems();
                            moved = false;
                            break;
                        }
                    } while (IsSectionItem(item));
                    return moved;
                }
                return false;
            }

            internal override void OnFocusChangedChild(FocusChangedEventArgs args)
            {
                if (args.Focused && !args.OldFocusWidget.belongTo(this))
                {
                    var item = getFocusedItem();
                    if (item != null)
                    {
                        if (item.Y < 0)
                        {
                            for (int i = 0; i < displayListItems.Count; i++)
                            {
                                if (displayListItems[i].Y >= 0)
                                {
                                    displayListItems[i].SetFocus(false);
                                    break;
                                }
                            }
                        }
                        else if (item.Y + item.Height > this.Height)
                        {
                            for (int i = displayListItems.Count - 1; i >= 0; i--)
                            {
                                if (displayListItems[i].Y + displayListItems[i].Height <= this.Height)
                                {
                                    displayListItems[i].SetFocus(false);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            private ListPanelItem getFocusedItem()
            {
                var scene = this.ParentScene;
                if (scene != null && scene.FocusWidget != null)
                {
                    foreach (var item in displayAllItems)
                    {
                        if (item == scene.FocusWidget || scene.FocusWidget.belongTo(item))
                        {
                            return item;
                        }
                    }
                }
                return null;
            }

            private void DestroyListItem(ListPanelItem item)
            {
                if (IsSectionItem(item))
                {
                    var sectionItem = item as ListPanelSectionItem;
                    item.Visible = false;
                    displaySectionItems.Remove(sectionItem);
                    cacheSectionItems.Add(sectionItem);
                }
                else
                {
                    item.Visible = false;
                    displayListItems.Remove(item);
                    cacheListItems.Add(item);
                }
                displayAllItems.Remove(item);
            }

            private static bool IsSectionItem(ListPanelItem item)
            {
                UIDebug.Assert(item.IndexInSection != -1 || item is ListPanelSectionItem, "item.IndexInSection is invalid.");

                return item.IndexInSection == -1; // item is ListPanelSectionItem;
            }

            private void UpdateListItem(ListPanelItem item, int allIndex, int sectionIndex, int indexInSection)
            {
                item.Index = allIndex;
                item.SectionIndex = sectionIndex;
                item.IndexInSection = indexInSection;

                if (itemUpdater != null)
                {
                    itemUpdater(item);
                }

                if ((displayListItems.Count > 0) && (displayListItems[0].Index > item.Index))
                {
                    displayListItems.Insert(0, item);
                }
                else
                {
                    displayListItems.Add(item);
                }
            }


            private ListPanelSectionItem CreateSectionItem()
            {
                ListPanelSectionItem item = null;

                if (cacheSectionItems.Count == 0)
                {
                    item = new ListPanelSectionItem();
                    AddChildLast(item);
                }
                else
                {
                    item = cacheSectionItems[0];
                    cacheSectionItems.Remove(item);
                }

                item.LeftMargin = 2;
                item.RightMargin = scrollBar.Width;
                item.Label.Font = this.SectionFont ?? new UIFont();
                item.Label.TextColor = this.SectionTextColor;
                item.BackgroundColor = this.SectionBackgroundColor;
                item.Width = this.Width;
                item.Height = getSectionHeight();
                item.Visible = true;

                return item;
            }

            private float getSectionHeight()
            {
                int fontSize = sectionFont != null ? sectionFont.Size : TextRenderHelper.DefaultFontSize;
                return (int)(fontSize * sectionHeightRate);
            }

            private void UpdateSectionItem(ListPanelSectionItem item, int sectionIndex)
            {
                item.Title = Sections[sectionIndex].Title;
                item.SectionIndex = sectionIndex;

                if ((displaySectionItems.Count > 0) && (displaySectionItems[0].SectionIndex > item.SectionIndex))
                {
                    displaySectionItems.Insert(0, item);
                }
                else
                {
                    displaySectionItems.Add(item);
                }
            }

            private int GetNextSectionIndex(int currentIndex)
            {
                if (ShowEmptySection == false || ShowSection == false)
                {
                    for (int i = currentIndex + 1; i < Sections.Count; i++)
                    {
                        if (Sections[i].ItemCount != 0)
                        {
                            return i;
                        }
                    }
                    return Sections.Count;
                }
                else
                {
                    return currentIndex + 1;
                }
            }

            private int GetPrevSectionIndex(int currentIndex)
            {
                if (ShowEmptySection == false || ShowSection == false)
                {
                    for (int i = currentIndex - 1; i >= 0; i--)
                    {
                        if (Sections[i].ItemCount != 0)
                        {
                            return i;
                        }
                    }
                    return -1;
                }
                else
                {
                    return currentIndex - 1;
                }
            }

            private void SetNextItem(float nextPos)
            {
                if (displayAllItems.Count == 0)
                {
                    if (ShowSection == true)
                    {
                        SetNextSectionItem(nextPos, GetNextSectionIndex(-1));
                    }
                    else
                    {
                        SetNextListItem(nextPos, 0, GetNextSectionIndex(-1), 0);
                    }
                }
                else
                {
                    if (displayAllItems.Count > 1)
                    {
                        var topItem = displayAllItems[0];
                        if (topItem.Y + topItem.Height < 0)
                        {
                            DestroyListItem(topItem);
                        }
                    }

                    ListPanelItem lowerItem = displayAllItems[displayAllItems.Count - 1];
                    if (IsBottomTerminalItem(lowerItem) == false)
                    {
                        if (ShowSection == true)
                        {
                            if (Sections[lowerItem.SectionIndex].ItemCount == (lowerItem.IndexInSection + 1))
                            {
                                SetNextSectionItem(nextPos, GetNextSectionIndex(lowerItem.SectionIndex));
                            }
                            else
                            {
                                SetNextListItem(nextPos, MaxDisplayListItemAllIndex + 1, lowerItem.SectionIndex, lowerItem.IndexInSection + 1);
                            }
                        }
                        else
                        {
                            if (Sections[lowerItem.SectionIndex].ItemCount == (lowerItem.IndexInSection + 1))
                            {
                                SetNextListItem(nextPos, MaxDisplayListItemAllIndex + 1, GetNextSectionIndex(lowerItem.SectionIndex), 0);
                            }
                            else
                            {
                                SetNextListItem(nextPos, MaxDisplayListItemAllIndex + 1, lowerItem.SectionIndex, lowerItem.IndexInSection + 1);
                            }
                        }
                    }
                }
            }

            private void SetNextSectionItem(float nextPos, int nextIndex)
            {
                if (nextIndex != Sections.Count)
                {
                    ListPanelSectionItem sectionItem = CreateSectionItem();
                    UpdateSectionItem(sectionItem, nextIndex);
                    sectionItem.Y = nextPos;
                    displayAllItems.Add(sectionItem);
                }
            }

            private void SetNextListItem(float nextPos, int nextAllIndex, int nextSctionIndex, int nextIndexInSection)
            {
                if (nextSctionIndex != Sections.Count)
                {
                    ListPanelItem item = CreateListItem();
                    UpdateListItem(item, nextAllIndex, nextSctionIndex, nextIndexInSection);
                    item.Y = nextPos;
                    displayAllItems.Add(item);
                }
            }

            private void SetPrevItem(float currentPos)
            {
                if (displayAllItems.Count == 0)
                {
                    return;
                }

                if (displayAllItems.Count > 1)
                {
                    var bottomItem = displayAllItems[displayAllItems.Count - 1];
                    if (bottomItem.Y > this.Height)
                    {
                        DestroyListItem(bottomItem);
                    }
                }

                ListPanelItem upperItem = displayAllItems[0];
                if (IsTopTerminalItem(upperItem) == false)
                {
                    if (ShowSection == true)
                    {
                        if (IsSectionItem(upperItem))
                        {
                            int prevSectionIndex = GetPrevSectionIndex(upperItem.SectionIndex);
                            if (Sections[prevSectionIndex].ItemCount == 0)
                            {
                                SetPrevSectionItem(currentPos, prevSectionIndex);
                            }
                            else
                            {
                                SetPrevListItem(currentPos, MinDisplayListItemAllIndex - 1, prevSectionIndex, Sections[prevSectionIndex].ItemCount - 1);
                            }
                        }
                        else if (upperItem.IndexInSection == 0)
                        {
                            SetPrevSectionItem(currentPos, upperItem.SectionIndex);
                        }
                        else
                        {
                            SetPrevListItem(currentPos, MinDisplayListItemAllIndex - 1, upperItem.SectionIndex, upperItem.IndexInSection - 1);
                        }
                    }
                    else
                    {
                        if (upperItem.IndexInSection == 0)
                        {
                            int prevSectionIndex = GetPrevSectionIndex(upperItem.SectionIndex);
                            SetPrevListItem(currentPos, MinDisplayListItemAllIndex - 1, prevSectionIndex, Sections[prevSectionIndex].ItemCount - 1);
                        }
                        else
                        {
                            SetPrevListItem(currentPos, MinDisplayListItemAllIndex - 1, upperItem.SectionIndex, upperItem.IndexInSection - 1);
                        }
                    }
                }
            }

            private void SetPrevSectionItem(float currentPos, int prevIndex)
            {
                if (prevIndex != -1)
                {
                    ListPanelSectionItem sectionItem = CreateSectionItem();
                    UpdateSectionItem(sectionItem, prevIndex);
                    sectionItem.Y = currentPos - sectionItem.ContainEdgeHeight;
                    displayAllItems.Insert(0, sectionItem);
                }
            }

            private void SetPrevListItem(float currentPos, int prevAllIndex, int prevSctionIndex, int prevIndexInSection)
            {
                if (prevSctionIndex != -1)
                {
                    ListPanelItem item = CreateListItem();
                    UpdateListItem(item, prevAllIndex, prevSctionIndex, prevIndexInSection);
                    item.Y = currentPos - item.ContainEdgeHeight;
                    displayAllItems.Insert(0, item);
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

                if (IsScrollable() == true) {
                    if (animationState == AnimationState.None)
                    {
                        OnTouchEventImpl(touchEvents);
                        touchEvents.Forward = true;
                    }
                    else
                    {
                        OnTouchEventImpl(touchEvents);
                    }
                }
                else {
                    touchEvents.Forward = true;
                }
            }

            private void OnTouchEventImpl(TouchEventCollection touchEvents)
            {
                switch (touchEvents.PrimaryTouchEvent.Type)
                {
                    case TouchEventType.Down:
                        switch (terminalState)
                        {
                            case TerminalState.None:
                                animationState = AnimationState.None;
                                animation = false;
                                break;
                            case TerminalState.Top:
                            case TerminalState.TopReset:
                                ResetTopTerminalItems();
                                break;
                            case TerminalState.Bottom:
                            case TerminalState.BottomReset:
                                ResetBottomTerminalItems();
                                break;
                        }
                        break;

                    case TouchEventType.Up:
                        if (animationState == AnimationState.Drag)
                        {
                            animationState = AnimationState.None;
                            animation = false;
                            
                            switch (terminalState)
                            {
                                case TerminalState.Top:
                                    terminalState = TerminalState.TopReset;
                                    animation = true;
                                    break;
                            
                                case TerminalState.Bottom:
                                    terminalState = TerminalState.BottomReset;
                                    animation = true;
                                    break;
                            }
                        }
                        
                        if (this.ScrollBarVisibility == ScrollBarVisibility.ScrollingVisible &&
                            animation == false)
                        {
                            this.scrollBar.Visible = false;
                        }
                        break;
                }
            }

            private void DragEventHandler(object sender, DragEventArgs e)
            {
                if (IsScrollable() == true)
                {
                    ResetState(false);
                    animationState = AnimationState.Drag;
                    UpdateItems(e.Distance.Y);
                    animation = false;
                }
            }

            private void FlickEventHandler(object sender, FlickEventArgs e)
            {
                if (IsScrollable() == true)
                {
                    ResetState(false);
                    animationState = AnimationState.Flick;
                    animation = true;

                    switch (terminalState)
                    {
                        case TerminalState.Top:
                        case TerminalState.Bottom:
                            scrollVelocity = 0.0f;
                            break;

                        default :
                            scrollVelocity = e.Speed.Y * flickStartRatio;
                            break;
                    }
                }
            }


            private void UpdateTopTerminalItems()
            {
                if (displayAllItems.Count > 0)
                    {
                    if (terminalState == TerminalState.Top)
                    {
                        terminalDistance += displayAllItems[0].Y * 2.0f;
                    }
                    else
                    {
                        terminalDistance += displayAllItems[0].Y;
                    }
                }

                if (terminalDistance > maxTerminalDistance) {
                    terminalDistance = maxTerminalDistance;
                    scrollVelocity = 0.0f;
                    if (animationState == AnimationState.Flick) {
                        terminalState = TerminalState.TopReset;
                    }
                }

                if (terminalDistance > 0.0f)
                {
                    float nextPos = 0.0f;
                    float addY = terminalDistance * terminalDistanceRatio;
                    foreach (ListPanelItem item in displayAllItems)
                    {
                        item.Y = nextPos;
                        nextPos = item.Y + item.ContainEdgeHeight + addY;
                    }

                    if (terminalState == TerminalState.None)
                    {
                        terminalState = TerminalState.Top;
                    }
                }
                else
                {
                    ResetTopTerminalItems();
                }
            }

            private void UpdateBottomTerminalItems()
            {
                int bottomIndex = displayAllItems.Count - 1;
                ListPanelItem bottomItem = displayAllItems[bottomIndex];
                if (displayAllItems.Count > 0)
                    {
                    if (terminalState == TerminalState.Bottom)
                    {
                        terminalDistance -= (Height - (bottomItem.Y + bottomItem.ContainEdgeHeight)) * 2.0f;
                    }
                    else
                    {
                        terminalDistance -= Height - (bottomItem.Y + bottomItem.ContainEdgeHeight);
                    }
                }

                if (terminalDistance < -maxTerminalDistance) {
                    terminalDistance = -maxTerminalDistance;
                    scrollVelocity = 0.0f;
                    if (animationState == AnimationState.Flick) {
                        terminalState = TerminalState.BottomReset;
                    }
                }

                if (terminalDistance < 0.0f)
                {
                    float nextPos = Height;
                    float addY = terminalDistance * terminalDistanceRatio;
                    for (int i = bottomIndex; i >= 0; i--)
                    {
                        ListPanelItem item = displayAllItems[i];
                        item.Y = nextPos - item.ContainEdgeHeight;
                        nextPos = item.Y + addY;
                    }

                    if (terminalState == TerminalState.None)
                    {
                        terminalState = TerminalState.Bottom;
                    }
                }
                else
                {
                    ResetBottomTerminalItems();
                }
            }

            private void ResetTopTerminalItems()
            {
                float nextPos = 0.0f;
                foreach (ListPanelItem item in displayAllItems)
                {
                    item.Y = nextPos;
                    nextPos = item.Y + item.ContainEdgeHeight;
                }
                terminalState = TerminalState.None;
                terminalDistance = 0.0f;
                animationState = AnimationState.None;
                animation = false;
            }

            private void ResetBottomTerminalItems()
            {
                float nextPos = Height;
                for (int i = displayAllItems.Count - 1; i >= 0; i--)
                {
                    ListPanelItem item = displayAllItems[i];
                    item.Y = nextPos - item.ContainEdgeHeight;
                    nextPos = item.Y;
                }
                terminalState = TerminalState.None;
                terminalDistance = 0.0f;
                animationState = AnimationState.None;
                animation = false;
            }

            private void UpdateItems(float scrollDistance)
            {
                if (sections == null || sections.Count == 0)
                {
                    return;
                }

                if (displayAllItems.Count == 0)
                {
                    float nextPosY = 0.0f;
                    while (nextPosY < Height)
                    {
                        SetNextItem(nextPosY);
                        if (displayAllItems.Count == 0)
                        {
                            return;
                        }

                        ListPanelItem bottomItem = displayAllItems[displayAllItems.Count - 1];
                        nextPosY = bottomItem.Y + bottomItem.ContainEdgeHeight;
                        if (IsBottomTerminalItem(bottomItem))
                        {
                            break;
                        }
                    }
                }

                foreach (ListPanelItem item in displayAllItems)
                {
                    item.Y += scrollDistance;
                }

                // update items
                switch (terminalState)
                {
                    case TerminalState.None:
                        ListPanelItem topItem = displayAllItems[0];
                        ListPanelItem bottomItem = displayAllItems[displayAllItems.Count - 1];
                        float nextPosY = bottomItem.Y + bottomItem.ContainEdgeHeight;

                        while (topItem.Y > 0.0f)
                        {
                            if (IsTopTerminalItem(topItem))
                            {
                                UpdateTopTerminalItems();
                                break;
                            }

                            SetPrevItem(topItem.Y);
                            topItem = displayAllItems[0];
                        }

                        while (nextPosY < Height)
                        {
                            if (IsTopTerminalItem(topItem))
                            {
                                if (nextPosY - topItem.Y < Height)
                                {
                                    UpdateTopTerminalItems();
                                    break;
                                }
                            }

                            if (IsBottomTerminalItem(bottomItem))
                            {
                                UpdateBottomTerminalItems();
                                break;
                            }

                            SetNextItem(nextPosY);
                            bottomItem = displayAllItems[displayAllItems.Count - 1];
                            nextPosY = bottomItem.Y + bottomItem.ContainEdgeHeight;
                        }
                        break;

                    case TerminalState.Top:
                    case TerminalState.TopReset:
                        UpdateTopTerminalItems();
                        break;

                    case TerminalState.Bottom:
                    case TerminalState.BottomReset:
                        UpdateBottomTerminalItems();
                        break;
                }

                // delete undisplay items
                if (terminalState == TerminalState.None)
                {
                    List<ListPanelItem> removeItems = new List<ListPanelItem>();
                    foreach (ListPanelItem item in displayAllItems)
                    {
                        if ((item.Y + item.ContainEdgeHeight) < 0.0f || item.Y > Height)
                        {
                            removeItems.Add(item);
                        }
                    }
                    foreach (ListPanelItem item in removeItems)
                    {
                        DestroyListItem(item);
                    }
                }

                UpdateScrollBar();
            }

            private Boolean IsScrollable() {
                // Count valid sections
                int sectionCount = 0;
                if (ShowSection == true) {
                    sectionCount = Sections.Count;
                    if (ShowEmptySection == false) {
                        foreach(ListSection section in Sections) {
                            if (section.ItemCount <= 0) {
                                sectionCount--;
                            }
                        }
                    }
                }

                if (displayAllItems.Count >= AllItemCount + sectionCount) {
                    // Calculate all items height
                    float allHeight = 0.0f;
                    foreach (ListPanelItem item in displayAllItems) {
                        allHeight += item.ContainEdgeHeight;
                    }

                    // Check whether this list is scrollable or not
                    if (allHeight <= Height) {
                        return false;
                    }
                }

                return true;
            }

            private int MaxDisplayListItemAllIndex
            {
                get
                {
                    if (displayListItems.Count == 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return displayListItems[displayListItems.Count - 1].Index;
                    }
                }
            }

            private int MinDisplayListItemAllIndex
            {
                get
                {
                    if (displayListItems.Count == 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return displayListItems[0].Index;
                    }

                }
            }

            private bool IsTopTerminalItem(ListPanelItem item)
            {
                if (ShowSection == true)
                {
                    if (!IsSectionItem(item))
                    {
                        return false;
                    }

                    if (ShowEmptySection == false)
                    {
                        for (int i = 0; i < Sections.Count; i++)
                        {
                            if (Sections[i].ItemCount != 0)
                            {
                                return (i == item.SectionIndex) ? true : false;
                            }
                        }
                    }
                    else
                    {
                        if (item.SectionIndex == 0)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (item.Index == 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool IsBottomTerminalItem(ListPanelItem item)
            {
                if (ShowSection == true)
                {
                    if (ShowEmptySection == false)
                    {
                        for (int i = Sections.Count - 1; i >= 0; i--)
                        {
                            int lastSectionIndex = i;
                            int lastSectionItemCount = Sections[lastSectionIndex].ItemCount;
                            if (Sections[lastSectionIndex].ItemCount != 0)
                            {
                                if ((item.SectionIndex == lastSectionIndex) && ((item.IndexInSection + 1) == lastSectionItemCount))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        int lastSectionIndex = Sections.Count - 1;
                        int lastSectionItemCount = Sections[lastSectionIndex].ItemCount;
                        if ((item.SectionIndex == lastSectionIndex) && ((item.IndexInSection + 1) == lastSectionItemCount))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (AllItemCount == (item.Index + 1))
                    {
                        return true;
                    }
                }

                return false;
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

                if (displayAllItems.Count == 0) UpdateItems(0);
                if (!animation) return;

                switch (animationState)
                {
                    case AnimationState.Flick:
                        if (terminalState == TerminalState.None) {
                            scrollVelocity = scrollVelocity * (float)Math.Pow((1.0f - flickDecelerationRatio), elapsedTime / 16.6f);
                        }
                        else {
                            scrollVelocity = scrollVelocity * (float)Math.Pow((1.0f - terminalDecelerationRatio), elapsedTime / 16.6f);
                        }

                        if (Math.Abs(scrollVelocity * elapsedTime / 16.6f) < 0.5f / 16.6f || Math.Abs(terminalDistance) >= maxTerminalDistance)
                        {
                            scrollVelocity = 0.0f;
                            animationState = AnimationState.None;

                            switch (terminalState)
                            {
                                case TerminalState.Top:
                                    terminalState = TerminalState.TopReset;
                                    animation = true;
                                    break;

                                case TerminalState.Bottom:
                                    terminalState = TerminalState.BottomReset;
                                    animation = true;
                                    break;
                                default:
                                    animation = false;
                                    break;
                            }
                        }
                        else
                        {
                            UpdateItems(scrollVelocity);
                        }
                        break;
                }

                switch (terminalState)
                {
                    case TerminalState.TopReset:
                        if (terminalDistance < 1.0f)
                        {
                            ResetTopTerminalItems();
                        }
                        else
                        {
                            UpdateItems(terminalDistance * (float)Math.Pow((1.0f - terminalReturnRatio), elapsedTime / 16.6f) - terminalDistance);
                        }
                        break;

                    case TerminalState.BottomReset:
                        if (terminalDistance > -1.0f)
                        {
                            ResetBottomTerminalItems();
                        }
                        else
                        {
                            UpdateItems(terminalDistance * (float)Math.Pow((1.0f - terminalReturnRatio), elapsedTime/ 16.6f) - terminalDistance);
                        }
                        break;
                }

                UpdateScrollBarVisible();
            }

            /// @if LANG_JA
            /// <summary>アイテムを作成するメソッドを登録する。</summary>
            /// <param name="creator">アイテムを作成するメソッド</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Registers the method of creating items.</summary>
            /// <param name="creator">Method of creating items</param>
            /// @endif
            public void SetListItemCreator(ListItemCreator creator)
            {
                itemCreator = creator;
                standardItemHeight = -1.0f;

                Refresh();
                foreach (var item in this.cacheListItems)
                {
                    if(item != null) item.Dispose();
                }
                this.cacheListItems.Clear();
            }

            /// @if LANG_JA
            /// <summary>アイテムを更新するメソッドを登録する。</summary>
            /// <param name="updater">アイテムを更新するメソッド</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Registers the method of updating items.</summary>
            /// <param name="updater">Method of updating items</param>
            /// @endif
            public void SetListItemUpdater(ListItemUpdater updater)
            {
                itemUpdater = updater;
                standardItemHeight = -1.0f;
                Refresh();
            }

            /// @if LANG_JA
            /// <summary>指定した距離移動する。負数を指定した場合は上へ、正数を指定した場合は下へ移動する。</summary>
            /// <param name="moveDistance">移動距離(pixel)</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Moves a specified distance. Moves up when a negative number is specified, and moves down when a positive number is specified.</summary>
            /// <param name="moveDistance">Travel distance (pixels)</param>
            /// @endif
            public void Move(float moveDistance)
            {
                UpdateItems(moveDistance);

                switch (this.terminalState)
                {
                    case TerminalState.Top:
                    case TerminalState.TopReset:
                        ResetTopTerminalItems();
                        break;
                    case TerminalState.Bottom:
                    case TerminalState.BottomReset:
                        ResetBottomTerminalItems();
                        break;
                }
            }

            /// @if LANG_JA
            /// <summary>現在表示中のすべてのアイテムを更新する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Updates all currently displayed items.</summary>
            /// @endif
            public void UpdateItems()
            {
                if (itemUpdater != null)
                {
                    for (int i = 0; i < displayAllItems.Count; i++)
                    {
                        itemUpdater(displayAllItems[i]);
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスのアイテムを取得する</summary>
            /// <remarks>表示範囲内にないインデックスを指定した場合はnullが返されます。</remarks>
            /// <param name="index">リスト全体でのアイテムインデックス</param>
            /// <returns>アイテム</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the specified index item</summary>
            /// <remarks>If an index outside the display range is specified, null is returned.</remarks>
            /// <param name="index">Item index for all lists</param>
            /// <returns>Item</returns>
            /// @endif
            public ListPanelItem GetListItem(int index)
            {
                if (index < 0)
                {
                    return null;
                }

                foreach (var item in this.displayListItems)
                {
                    if (item.Index == index)
                    {
                        return item;
                    }
                }

                return null;
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスのアイテムを取得する</summary>
            /// <remarks>表示範囲内にないインデックスを指定した場合はnullが返されます。</remarks>
            /// <param name="sectionIndex">セクションのインデックス</param>
            /// <param name="indexInSection">セクション内でのアイテムインデックス</param>
            /// <returns>アイテム</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the specified index item</summary>
            /// <remarks>If an index outside the display range is specified, null is returned.</remarks>
            /// <param name="sectionIndex">Index of section</param>
            /// <param name="indexInSection">Item index within section</param>
            /// <returns>Item</returns>
            /// @endif
            public ListPanelItem GetListItem(int sectionIndex, int indexInSection)
            {
                if (sectionIndex < 0 || indexInSection < 0)
                {
                    return null;
                }

                foreach (var item in this.displayListItems)
                {
                    if (item.SectionIndex == sectionIndex && item.IndexInSection == indexInSection)
                    {
                        return item;
                    }
                }

                return null;
            }


            private class ListPanelSectionItem : ListPanelItem
            {

                public ListPanelSectionItem()
                {
                    label = new Label();
                    AddChildLast(label);
                    HideEdge = true;
                    Focusable = false;
                }

                public override float Width
                {
                    get
                    {
                        return base.Width;
                    }
                    set
                    {
                        base.Width = value;

                        if (label != null)
                        {
                            label.X = LeftMargin;
                            label.Width = value - LeftMargin - RightMargin;
                        }
                    }
                }

                public override float Height
                {
                    get
                    {
                        return base.Height;
                    }
                    set
                    {
                        base.Height = value;

                        if (label != null)
                        {
                            label.Y = 0;
                            label.Height = value;
                        }
                    }
                }

                public string Title
                {
                    get
                    {
                        return label.Text;
                    }
                    set
                    {
                        label.Text = value;
                    }
                }

                public Label Label
                {
                    get { return label; }
                    set { label = value; }
                }

                public float LeftMargin { get; set; }

                public float RightMargin { get; set; }

                private Label label;
            }

            private UISprite backgroundSprt;
            private ScrollBar scrollBar;

            private ListItemCreator itemCreator;
            private ListItemUpdater itemUpdater;

            private List<ListPanelItem> displayAllItems;
            private List<ListPanelItem> displayListItems;
            private List<ListPanelItem> cacheListItems;
            private List<ListPanelSectionItem> displaySectionItems;
            private List<ListPanelSectionItem> cacheSectionItems;

            private AnimationState animationState;
            private TerminalState terminalState;
            private bool animation = false;

            private float terminalDistance;
            //private float terminalAcceleration;
            private float scrollVelocity;
            private float standardItemHeight = -1.0f;
            

            // For parameter tuning, following parameters are temporary changed as public
            float flickStartRatio = 0.022f;
            float flickDecelerationRatio = 0.012f;
            float terminalDistanceRatio = 0.073f;
            float terminalDecelerationRatio = 0.48f;
            float maxTerminalDistance = 240.0f;
            float terminalReturnRatio = 0.09f;
        }

        /// @if LANG_JA
        /// <summary>アイテムを作成するメソッドのデリゲート</summary>
        /// <returns>アイテム</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Delegate the method of creating items</summary>
        /// <returns>Item</returns>
        /// @endif
        public delegate ListPanelItem ListItemCreator();

        /// @if LANG_JA
        /// <summary>アイテムを更新するメソッドのデリゲート</summary>
        /// <param name="item">アイテム</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Delegate the method of updating items</summary>
        /// <param name="item">Item</param>
        /// @endif
        public delegate void ListItemUpdater(ListPanelItem item);

        /// @if LANG_JA
        /// <summary>リストのセクション用のコンテナ部品</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Container part for list section</summary>
        /// @endif
        public class ListSection
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ListSection()
            {
                Title = "";
                ItemCount = 0;
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="title">セクションのタイトル</param>
            /// <param name="itemCount">セクションに登録されているアイテムの数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="title">Section title</param>
            /// <param name="itemCount">Number of items registered to a section</param>
            /// @endif
            public ListSection(string title, int itemCount)
            {
                Title = title;
                ItemCount = itemCount;
            }

            /// @if LANG_JA
            /// <summary>セクションのタイトルを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the section title.</summary>
            /// @endif
            public string Title
            {
                get
                {
                    return title;
                }
                set
                {
                    title = value;
                    if (ItemChanged != null)
                    {
                        ItemChanged(this, EventArgs.Empty);
                    }
                }
            }
            private string title;

            /// @if LANG_JA
            /// <summary>セクションに登録されているアイテムの数を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the number of items registered to a section.</summary>
            /// @endif
            public int ItemCount
            {
                get
                {
                    return itemCount;
                }
                set
                {
                    itemCount = value;
                    if (ItemChanged != null)
                    {
                        ItemChanged(this, EventArgs.Empty);
                    }
                }
            }
            private int itemCount;

            /// @if LANG_JA
            /// <summary>セクションに登録されているアイテムが変わった時のイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event when the item registered to a section changes</summary>
            /// @endif
            public event EventHandler<EventArgs> ItemChanged;

        }


        /// @if LANG_JA
        /// <summary>セクションのコレクション</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Collection of sections</summary>
        /// @endif
        public class ListSectionCollection : IList<ListSection>
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ListSectionCollection()
            {
                this.items = new List<ListSection>();
            }

            /// @if LANG_JA
            /// <summary>セクションに登録されているアイテムの総数を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the total number of items registered to a section.</summary>
            /// @endif
            public int AllItemCount
            {
                get
                {
                    int count = 0;
                    foreach (ListSection item in this.items)
                    {
                        count += item.ItemCount;
                    }
                    return count;
                }
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスにあるセクションを取得・設定する。</summary>
            /// <param name="index">取得するセクションのインデックス</param>
            /// <returns>指定したインデックスにあるセクション</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the section in a specified index.</summary>
            /// <param name="index">Index of section to be obtained</param>
            /// <returns>Section in a specified index</returns>
            /// @endif
            public ListSection this[int index]
            {
                get
                {
                    return this.items[index];
                }
                set
                {
                    this.items[index].ItemChanged += new EventHandler<EventArgs>(SectionItemChanged);
                    this.items[index] = value;
                }
            }

            /// @if LANG_JA
            /// <summary>指定したセクションのインデックスを取得する。</summary>
            /// <param name="section">検索するセクション</param>
            /// <returns>検索するセクションが存在する場合はそのインデックス。それ以外の場合は -1。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index of a specified section.</summary>
            /// <param name="section">Section to search for</param>
            /// <returns>If the section to search for exists, the index of that section. Otherwise, -1.</returns>
            /// @endif
            public int IndexOf(ListSection section)
            {
                return this.items.IndexOf(section);
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスの位置にセクションを挿入する。</summary>
            /// <param name="index">挿入する位置のインデックス。</param>
            /// <param name="section">挿入するセクション</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts the section in a specified index position.</summary>
            /// <param name="index">Index of position to insert.</param>
            /// <param name="section">Section to insert</param>
            /// @endif
            public void Insert(int index, ListSection section)
            {
                section.ItemChanged += new EventHandler<EventArgs>(SectionItemChanged);

                this.items.Insert(index, section);
                this.SectionItemChanged(this, EventArgs.Empty);
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスにあるセクションを削除する。</summary>
            /// <param name="index">削除するセクションのインデックス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the section in a specified index.</summary>
            /// <param name="index">Index of section to be deleted</param>
            /// @endif
            public void RemoveAt(int index)
            {
                this.items.RemoveAt(index);
                this.SectionItemChanged(this, EventArgs.Empty);
            }

            /// @if LANG_JA
            /// <summary>セクションの数を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the number of sections.</summary>
            /// @endif
            public int Count
            {
                get
                {
                    return items.Count;
                }
            }

            /// @if LANG_JA
            /// <summary>読み取り専用かどうかを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains whether to be read-only.</summary>
            /// @endif
            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// @if LANG_JA
            /// <summary>セクションを追加する。</summary>
            /// <param name="section">追加するセクション</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds a section.</summary>
            /// <param name="section">Section to be added</param>
            /// @endif
            public void Add(ListSection section)
            {
                section.ItemChanged += new EventHandler<EventArgs>(SectionItemChanged);

                this.items.Add(section);
                this.SectionItemChanged(this, EventArgs.Empty);
            }

            /// @if LANG_JA
            /// <summary>すべてのセクションを削除する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes all sections.</summary>
            /// @endif
            public void Clear()
            {
                this.items.Clear();
                this.SectionItemChanged(this, EventArgs.Empty);
            }

            /// @if LANG_JA
            /// <summary>特定のセクションが格納されているかどうかを判断する。</summary>
            /// <param name="section">検索するセクション</param>
            /// <returns>検索するセクションが存在する場合はtrueそれ以外の場合はfalse。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Determines whether a specific section is stored.</summary>
            /// <param name="section">Section to search for</param>
            /// <returns>If the section to search for exists, then true; otherwise, false.</returns>
            /// @endif
            public bool Contains(ListSection section)
            {
                return this.items.Contains(section);
            }

            /// @if LANG_JA
            /// <summary>セクションを配列にコピーする。</summary>
            /// <param name="array">セクションがコピーされる１次元の配列</param>
            /// <param name="arrayIndex">コピーの開始位置となる配列のインデックス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Copies a section to an array.</summary>
            /// <param name="array">1D array where a section will be copied</param>
            /// <param name="arrayIndex">Index of an array that is the start position for copying</param>
            /// @endif
            public void CopyTo(ListSection[] array, int arrayIndex)
            {
                this.items.CopyTo(array, arrayIndex);
            }

            /// @if LANG_JA
            /// <summary>最初に見つかった特定のセクションを削除する。</summary>
            /// <param name="section">削除するセクション</param>
            /// <returns>削除するセクションが存在する場合はtrue。それ以外の場合はfalse。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes a specific section found first.</summary>
            /// <param name="section">Section to be deleted</param>
            /// <returns>If the section to be deleted exists, then true. Otherwise, false.</returns>
            /// @endif
            public bool Remove(ListSection section)
            {
                bool ret = this.items.Remove(section);
                this.SectionItemChanged(this, EventArgs.Empty);
                return ret;
            }

            /// @if LANG_JA
            /// <summary>コレクションを反復処理する列挙子を返す。</summary>
            /// <returns>コレクションを反復処理するために使用できるIEnumeratorオブジェクト</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns the enumerator that repetitively handles the collection.</summary>
            /// <returns>IEnumerator object that can be used to repetitively handle a collection</returns>
            /// @endif
            IEnumerator<ListSection> IEnumerable<ListSection>.GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            /// @if LANG_JA
            /// <summary>コレクションを反復処理する列挙子を返す。</summary>
            /// <returns>コレクションを反復処理するために使用できるIEnumeratorオブジェクト</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns the enumerator that repetitively handles the collection.</summary>
            /// <returns>IEnumerator object that can be used to repetitively handle a collection</returns>
            /// @endif
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            private void SectionItemChanged(object sender, EventArgs e)
            {
                if (this.ItemsChanged != null)
                {
                    this.ItemsChanged(sender, e);
                }
            }

            /// @if LANG_JA
            /// <summary>内容が変化したときに呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the content has changed</summary>
            /// @endif
            public event EventHandler<EventArgs> ItemsChanged;

            private List<ListSection> items;

        }


    }
}
