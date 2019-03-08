/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;
using System.Diagnostics;

namespace Sce.PlayStation.HighLevel.UI
{
    internal class InternalSpinBox : Widget
    {
        internal const float defaultWidth = 80.0f;
        internal const float defaultHeight = 204.0f;
        internal const float centerHeight = 54.0f;
        internal const float bgWidthGap = 30.0f;
        internal const float listVerticalMargin = 2.0f;
        internal const float unitWidth = 80.0f;
        internal const float unitHeight = 40.0f;
        internal const float unitWidthPower = 1.6f;
        internal SpinList spinList;
        static internal readonly int visibleCount = 5;
        
        ImageAsset backgraundImage;
        ImageAsset centerImage;
        NinePatchMargin backgroundImageNinePatch;
        NinePatchMargin centerImageNinePatch;
        private UIPrimitive backPrim;
        private UIPrimitive centerPrim;

        public InternalSpinBox()
        {
            base.Width = defaultWidth;
            base.Height = defaultHeight;
            
            backgraundImage = new ImageAsset (SystemImageAsset.SpinBoxBase);
            centerImage = new ImageAsset (SystemImageAsset.SpinBoxCenter);
            backgroundImageNinePatch = AssetManager.GetNinePatchMargin(SystemImageAsset.SpinBoxBase);
            centerImageNinePatch = AssetManager.GetNinePatchMargin(SystemImageAsset.SpinBoxCenter);

            backPrim = new UIPrimitive (DrawMode.TriangleStrip, 16, 28);
            backPrim.Image = backgraundImage;
            backPrim.ShaderType = ShaderType.Texture;
            UIPrimitiveUtility.SetupNinePatch(backPrim, defaultWidth, defaultHeight, 0, 0, backgroundImageNinePatch);
            backPrim.SetPosition(0.0f, 0.0f);
            this.RootUIElement.AddChildLast(backPrim);

            centerPrim = new UIPrimitive (DrawMode.TriangleStrip, 16, 28);
            centerPrim.Image = centerImage;
            centerPrim.ShaderType = ShaderType.Texture;
            UIPrimitiveUtility.SetupNinePatch(centerPrim, defaultWidth, centerHeight, 0, 0, centerImageNinePatch);
            centerPrim.SetPosition(0.0f, (defaultHeight - centerHeight) / 2);
            this.RootUIElement.AddChildLast(centerPrim);


            spinList = new SpinList ();
            spinList.Width = defaultWidth;
            spinList.Height = unitHeight * visibleCount;
            spinList.X = 0.0f;
            spinList.Y = listVerticalMargin;
            spinList.ItemGapLine = unitHeight;
            spinList.ScrollAreaLineNum = visibleCount;
            AddChildLast(spinList);

            this.PriorityHit = true;
            Focusable = true;
        }

        protected override void DisposeSelf()
        {
            if(backgraundImage != null)
                backgraundImage.Dispose();
            if (centerImage != null)
                centerImage.Dispose();

            base.DisposeSelf();
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
                if (backPrim != null && centerPrim != null && spinList != null)
                {
                    UIPrimitiveUtility.SetupNinePatch(backPrim, base.Width, base.Height, 0, 0, backgroundImageNinePatch);
                    UIPrimitiveUtility.SetupNinePatch(centerPrim, base.Width, centerHeight, 0, 0, centerImageNinePatch);
                    spinList.Width = base.Width;
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
                if (backPrim != null && centerPrim != null && spinList != null)
                {
                    base.Height = value;
                    UIPrimitiveUtility.SetupNinePatch(backPrim, base.Width, base.Height, 0, 0, backgroundImageNinePatch);
                    UIPrimitiveUtility.SetupNinePatch(centerPrim, base.Width, centerHeight, 0, 0, centerImageNinePatch);
                    centerPrim.SetPosition(0.0f, (base.Height - centerHeight) / 2);
                }
            }
        }

        protected internal override void OnPreviewKeyEvent(KeyEvent keyEvent)
        {
            base.OnPreviewKeyEvent(keyEvent);

            if ((keyEvent.KeyEventType == KeyEventType.Down || keyEvent.KeyEventType == KeyEventType.Repeat) &&
                (keyEvent.KeyType == KeyType.Up || keyEvent.KeyType == KeyType.Down))
            {
                spinList.ScrollOneLine(keyEvent.KeyType == KeyType.Up);
                keyEvent.Handled = true;
            }
        }
    
    internal class SpinItem : Widget
    {
        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public SpinItem()
        {
            this.sprt = new UISprite (1);
            RootUIElement.AddChildLast(this.sprt);
            this.sprt.ShaderType = ShaderType.SolidFill;

            this.IsFocused = false;
            this.IsSelected = false;

            this.ItemId = 0;
            this.isDown = false;
            this.PriorityHit = true;
        }

        /// <summary>Disposes the self.</summary>
        protected override void DisposeSelf()
        {
            if(this.ImageAsset != null)
            {
                this.ImageAsset.Dispose();
            }
            base.DisposeSelf();
        }

        /// @if LANG_JA
        /// <summary>アイテムを生成する。</summary>
        /// <returns>SpinBoxItemPanelクラスのインスタンス</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Generates an item.</summary>
        /// <returns>SpinBoxItemPanel class instance</returns>
        /// @endif
        public static SpinItem FactoryMethod()
        {
            return new SpinItem ();
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

                if (this.sprt != null)
                {
                    UISpriteUnit unit = this.sprt.GetUnit(0);
                    unit.Width = value;
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

                if (this.sprt != null)
                {
                    UISpriteUnit unit = this.sprt.GetUnit(0);
                    unit.Height = value;
                }
            }
        }


        /// @if LANG_JA
        /// <summary>ターゲット状態を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the target status.</summary>
        /// @endif
        public bool IsFocused
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>選択状態を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the selection status.</summary>
        /// @endif
        public bool IsSelected
        {
            get;
            set;
        }

        private bool enabled = true;

        public override bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (enabled)
                {
                    this.sprt.Alpha = 1.0f;
                    //this.sprt.GetUnit(0).Color = new UIColor(1f, 1f, 1f, 1f);
                }
                else
                {
                    this.sprt.Alpha = 0.3f;
                    //this.sprt.GetUnit(0).Color = new UIColor(1f, 0f, 0f, 1f);
                }
            }
        }

        /// @if LANG_JA
        /// <summary>表示する画像を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the image to display.</summary>
        /// @endif
        public ImageAsset ImageAsset
        {
            get
            {
                if (sprt != null)
                {
                    return this.sprt.Image;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (sprt != null)
                {
                    this.sprt.Image = value;
                }
            }
        }


        /// @if LANG_JA
        /// <summary>リストアイテムのIDを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the list item ID.</summary>
        /// @endif
        public int ItemId
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>タッチイベントのハンドラ</summary>
        /// <param name="touchEvents">タッチイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Touch event handler</summary>
        /// <param name="touchEvents">Touch event</param>
        /// @endif
        protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
        {
            base.OnTouchEvent(touchEvents);

            switch (touchEvents.PrimaryTouchEvent.Type)
            {
                case TouchEventType.Down:
                    this.isDown = true;
                    break;

                case TouchEventType.Up:
                    if (this.isDown)
                    {
                        FireSelect();
                        this.isDown = false;
                    }
                    break;
            }
            return;
        }


        /// @if LANG_JA
        /// <summary>使用するShaderProgramの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Type of ShaderProgram to use</summary>
        /// @endif
        public ShaderType ShaderType
        {
            get
            {
                if (sprt != null)
                {
                    return sprt.ShaderType;
                }
                else
                {
                    return ShaderType.SolidFill;
                }
            }
            set
            {
                if (sprt != null)
                {
                    sprt.ShaderType = value;
                }
            }
        }

        private void FireSelect()
        {
            if (ProtectedSelectAction != null)
            {
                ProtectedSelectAction(this, ItemId);
            }
        }


        /// @if LANG_JA
        /// <summary>リセットステートハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Reset state handler</summary>
        /// @endif
        protected internal override void OnResetState()
        {
            base.OnResetState();

            this.isDown = false;
        }


        /// @if LANG_JA
        /// <summary>リストアイテム選択時に呼ばれるハンドラのデリゲート</summary>
        /// <param name="sender">リストアイテム</param>
        /// <param name="itemId">リストアイテムのID</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Delegates the handler to be called when a list item is selected</summary>
        /// <param name="sender">List item</param>
        /// <param name="itemId">List item ID</param>
        /// @endif
        public delegate void ProtectedSelectActionHandler(object sender,int itemId);

        /// @if LANG_JA
        /// <summary>リストアイテム選択時に呼ばれるハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler to be called when a list item is selected</summary>
        /// @endif
        public event ProtectedSelectActionHandler ProtectedSelectAction;

        private UISprite sprt;
        private bool isDown;
    }

    internal class SpinList : Widget
    {

        public static int itemIdNoFocus = -1;
        public static int itemIdNoSelect = -1;
        private static int noCache = -1;


        /// @if LANG_JA
        /// <summary>アニメーションの状態</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Animation status</summary>
        /// @endif
        protected enum AnimationState
        {
            None = 0,
            Drag,
            Flick,
            Fit,
        }


        /// @if LANG_JA
        /// <summary>コンストラクタ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Constructor</summary>
        /// @endif
        public SpinList()
        {
            this.basePanel = new Panel ();
            this.AddChildLast(this.basePanel);

            this.HookChildTouchEvent = true;
            this.IsLoop = true;
            this.scrollAreaLineNum = 1;
            this.listItemNum = 0;
            this.ItemGapLine = 0.0f;
            this.cacheLineCount = 1;
            this.IsItemFit = true;
            this.ScrollAreaFirstLine = 0;
            this.focusIndex = itemIdNoFocus;
            this.lastSelectIndex = itemIdNoSelect;
            this.cacheStartIndex = noCache;
            this.cacheEndIndex = noCache;
            this.isSetup = false;
            this.animationState = AnimationState.None;
            this.flickDistance = 0.0f;
            this.flickInterpRate = 0.09f;
            this.flickSpeedCoeff = 0.03f;
            this.fitTargetIndex = 0;
            this.fitTargetOffsetPixel = 0.0f;
            this.fitInterpRate = 0.2f;
            this.isPress = false;
            this.startTouchPos = Vector2.Zero;

            this.listItem = new System.Collections.Generic.List<ListContainer> ();
            this.poolItem = new System.Collections.Generic.List<ListContainer> ();

            this.minDelayDragDist = 5.0f;
            this.Clip = true;

            this.FocusChanged = null;

            DragGestureDetector drag = new DragGestureDetector ();
            drag.Direction = DragDirection.Vertical;
            drag.DragDetected += new EventHandler<DragEventArgs> (DragEventHandler);
            this.AddGestureDetector(drag);

            FlickGestureDetector flick = new FlickGestureDetector ();
            flick.Direction = FlickDirection.Vertical;
            flick.FlickDetected += new EventHandler<FlickEventArgs> (FlickEventHandler);
            this.AddGestureDetector(flick);
        }


        /// @if LANG_JA
        /// <summary>アイテムの行間の距離を取得・取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the distance between rows of an item.</summary>
        /// @endif
        public float ItemGapLine
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>アイテムの列間の距離を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the distance between columns of an item.</summary>
        /// @endif
        public float ItemGapStep
        {
            get { return this.Width; }
        }


        /// @if LANG_JA
        /// <summary>フォーカス中アイテムのインデックス値を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the index value of the item in focus.</summary>
        /// @endif
        public int FocusIndex
        {
            get
            {
                return this.focusIndex;
            }
            set
            {
                if (focusIndex != value)
                {
                    if (isSetup)
                    {
                        if (this.IsLoop)
                        {
                            focusIndex = CalcItemIndex(value);
                            UpdateFocus();
                        }
                        else
                        {
                            if (value < listItemNum && value >= 0)
                            {
                                focusIndex = value;
                                UpdateFocus();
                            }
                        }
                    }
                    else
                    {
                        focusIndex = value;
                    }
                }
            }
        }


        /// @if LANG_JA
        /// <summary>選択状態を取得する。</summary>
        /// <param name="itemId">アイテムのインデックス値</param>
        /// <returns>選択状態</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the selection status.</summary>
        /// <param name="itemId">Item index value</param>
        /// <returns>Selection status</returns>
        /// @endif
        public bool IsSelect(int itemId)
        {
            if (itemId != itemIdNoSelect)
            {
                SpinItem item = GetListItem(itemId);

                if (item != null)
                {
                    return item.IsSelected;
                }
            }

            return false;
        }


        /// @if LANG_JA
        /// <summary>指定したアイテムを選択する。</summary>
        /// <param name="itemId">選択したいアイテムのインデックス値</param>
        /// <param name="isSelect">選択状態</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Selects the specified item.</summary>
        /// <param name="itemId">Index value of item to be selected</param>
        /// <param name="isSelect">Selection status</param>
        /// @endif
        public void SetSelect(int itemId, bool isSelect)
        {
            int prevSelectIndex = lastSelectIndex;

            if (isSelect)
            {
                if (prevSelectIndex != itemId)
                {
                    if (prevSelectIndex != itemIdNoSelect)
                    {
                        SetSelectStatus(prevSelectIndex, false);
                    }

                    if (itemId != itemIdNoSelect)
                    {
                        SetSelectStatus(itemId, true);

                        lastSelectIndex = itemId;
                    }
                }
            }
            else
            {
                if (itemId != itemIdNoSelect)
                {
                    SetSelectStatus(itemId, false);
                }
            }
        }


        /// @if LANG_JA
        /// <summary>リストのループを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the loop of a list.</summary>
        /// @endif
        public bool IsLoop
        {
            get
            {
                return isLoop;
            }
            set
            {
                if (isLoop != value)
                {
                    isLoop = value;

                    if (isSetup)
                    {
                        UpdateListItem(false);
                    }
                }
            }
        }

        private bool isLoop;


        /// @if LANG_JA
        /// <summary>アイテムごとに停止するかを取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets whether to stop for each item.</summary>
        /// @endif
        public bool IsItemFit
        {
            get;
            set;
        }


        /// @if LANG_JA
        /// <summary>List操作を開始する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Starts the List operation.</summary>
        /// @endif
        public void StartItemRequest()
        {
            if (this.ScrollAreaFirstLine > CalcMaxLineIndex())
            {
                SetScrollAreaLineIndex(CalcMaxLineIndex(), 0.0f);
            }
            else
            {
                SetScrollAreaLineIndex(this.ScrollAreaFirstLine, 0.0f);
            }

            listItem.Clear();
            poolItem.Clear();

            cacheStartIndex = 0;
            cacheEndIndex = 0;

            isSetup = true;

            UpdateListItem(false);
            UpdateFocus();
        }


        /// @if LANG_JA
        /// <summary>ListItemを取得する。</summary>
        /// <param name="index">アイテム番号</param>
        /// <returns>ListItemの取得（nullが返ることがある）</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains ListItem.</summary>
        /// <param name="index">Item number</param>
        /// <returns>Obtains ListItem (null may be returned)</returns>
        /// @endif
        public SpinItem GetListItem(int index)
        {
            if (index < 0)
            {
                return null;
            }

            for (int i = 0; i < listItem.Count; i++)
            {
                int itemIndex = listItem[i].listItem.ItemId;

                if (itemIndex == index)
                {
                    return (listItem[i].listItem);
                }
            }

            return null;
        }


        /// @if LANG_JA
        /// <summary>表示領域トップの行番号を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the row number of the display area top.</summary>
        /// @endif
        public int ScrollAreaFirstLine
        {
            get;
            private set;
        }


        /// @if LANG_JA
        /// <summary>表示領域トップの行番号から何ピクセルずれているかを取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the number of pixels deviating from the row number of the display area top.</summary>
        /// @endif
        public float ScrollAreaPixelOffset
        {
            get
            {
                return -this.basePanel.Y;
            }
        }


        /// @if LANG_JA
        /// <summary>表示領域トップの行番号とのずれをピクセル数で設定する。</summary>
        /// <param name="firstLine">トップの行番号</param>
        /// <param name="pixelOffset">行番号からのピクセル数</param>
        /// <param name="withAnimate">true:アニメーション付き  false:即時</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the deviation from the row number of the display area top in pixels.</summary>
        /// <param name="firstLine">Top row number</param>
        /// <param name="pixelOffset">Number of pixels from the row number</param>
        /// <param name="withAnimate">true: with animation, false: immediately</param>
        /// @endif
        public void SetScrollAreaPos(int firstLine, float pixelOffset, bool withAnimate)
        {
            ScrollTo(firstLine, pixelOffset, withAnimate);
        }


        /// @if LANG_JA
        /// <summary>表示位置を全体の割合（０～１）で設定する。</summary>
        /// <param name="ratio">全体に対する表示位置の割合</param>
        /// <param name="withAnimate">true:アニメーション付き  false:即時</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Sets the display position with the overall ratio (0 - 1).</summary>
        /// <param name="ratio">Ratio of display position to overall</param>
        /// <param name="withAnimate">true: with animation, false: immediately</param>
        /// @endif
        public void SetScrollAreaPos(float ratio, bool withAnimate)
        {
            ScrollTo(ratio, withAnimate);
        }


        /// @if LANG_JA
        /// <summary>表示位置を全体の割合（０～１）で取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the display position with the overall ratio (0 - 1).</summary>
        /// @endif
        public float ScrollAreaRatio
        {
            get
            {
                int tmpMaxLine = CalcMaxLineIndex();

                if (tmpMaxLine == 0)
                {
                    return 0.0f;
                }
                else
                {
                    int tmpCurrentLine = this.ScrollAreaFirstLine;
                    float tmpCurrentOffset = this.ScrollAreaPixelOffset;

                    float tmpMaxLineReal = tmpMaxLine;
                    float tmpCurrentLineReal = tmpCurrentLine;

                    return (tmpCurrentLineReal + tmpCurrentOffset / ItemGapLine) / tmpMaxLineReal;
                }
            }
        }


        /// @if LANG_JA
        /// <summary>表示領域の行数を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the number of rows of the display area.</summary>
        /// @endif
        public int ScrollAreaLineNum
        {
            get
            {
                return scrollAreaLineNum;
            }
            set
            {
                scrollAreaLineNum = (value > 0) ? (UInt16)value : (UInt16)0;

                if (isSetup)
                {
                    UpdateListItem(false);
                }
            }
        }


        /// @if LANG_JA
        /// <summary>リストの総項目数を取得・設定する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains and sets the total number of items in a list.</summary>
        /// @endif
        public int ListItemNum
        {
            get
            {
                return this.listItemNum;
            }
            set
            {
                listItemNum = value;

                if (focusIndex >= listItemNum)
                {
                    this.FocusIndex = listItemNum - 1;
                }

                if (this.ScrollAreaFirstLine > CalcMaxLineIndex())
                {
                    SetScrollAreaLineIndex(CalcMaxLineIndex(), 0.0f);
                }

                if (isSetup)
                {
                    UpdateListItem(false);
                }
            }
        }


        /// @if LANG_JA
        /// <summary>リストの総行数を取得する。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains the total number of rows in a list.</summary>
        /// @endif
        public int TotalLineNum
        {
            get
            {
                return listItemNum;
            }
        }

        public void ScrollOneLine( bool up)
        {
            var index = this.focusIndex;

            // check end
            if (!isLoop)
            {
                if (up)
                {
                    if (index > this.listItemNum - (visibleCount / 2) - 2)
                        return;
                }
                else
                {
                    if (index < (visibleCount / 2) + 1)
                        return;
                }
            }

            // increment or decrement (and skip disable items)
            for (int i = 0; i < 4; i++)
            {  // skippable 3 or less items. e.g. 2/29,30,31
                index = CalcItemIndex(up ? index + 1 : index - 1);
                var item = GetListItem(index);
                if (item == null || item.Enabled) // if current index is 0 then getListItem(27) return null
                {
                    break;
                }
            }

            // scroll
            ScrollTo(index - (visibleCount / 2), 0.0f, true);
            this.FocusIndex = index;
        }

        public class ListContainer
        {
            public ListContainer next;
            public ListContainer prev;
            public SpinItem listItem;
            public int totalId;
            public bool updateFlag;

            public ListContainer()
            {
                next = null;
                prev = null;
                listItem = null;
                totalId = 0;
                updateFlag = false;
            }
        };

        private void NormalizeLineIndex(ref int lineIndex, ref float offsetPixel)
        {
            while (offsetPixel < 0.0f)
            {
                offsetPixel += ItemGapLine;
                lineIndex--;
            }

            while (offsetPixel > ItemGapLine)
            {
                offsetPixel -= ItemGapLine;
                lineIndex++;
            }
        }

        private void SetScrollAreaLineIndex(int lineIndex, float offsetPixel)
        {
            NormalizeLineIndex(ref lineIndex, ref offsetPixel);
            if (this.IsLoop)
            {
                while (lineIndex < 0)
                {
                    lineIndex += this.TotalLineNum;
                    cacheStartIndex += listItemNum;
                    cacheEndIndex += listItemNum;
                    fitTargetIndex += this.TotalLineNum;

                    for (int i = 0; i < listItem.Count; i++)
                    {
                        listItem[i].totalId += listItemNum;
                    }
                }
                while (lineIndex > this.TotalLineNum - 1)
                {
                    lineIndex -= this.TotalLineNum;
                    cacheStartIndex -= listItemNum;
                    cacheEndIndex -= listItemNum;
                    fitTargetIndex -= this.TotalLineNum;

                    for (int i = 0; i < listItem.Count; i++)
                    {
                        listItem[i].totalId -= listItemNum;
                    }
                }
                this.ScrollAreaFirstLine = lineIndex;
                SetNodeLinePos(offsetPixel);

                if (isSetup)
                {
                    UpdateListItem(false);
                }
            }
            else
            {
                if (lineIndex <= CalcMaxLineIndex() &&
                            lineIndex >= CalcMinLineIndex())
                {
                    bool isMod = false;
                    if (this.ScrollAreaFirstLine != lineIndex)
                    {
                        this.ScrollAreaFirstLine = lineIndex;
                        isMod = true;
                    }

                    SetNodeLinePos(offsetPixel);

                    if (isSetup)
                    {
                        UpdateListItem(isMod);
                    }
                }
            }
        }

        internal void ScrollTo(int scrollAreaLineIndex, float offsetPixel, bool withAnimate)
        {
            if (scrollAreaLineIndex < 0)
            {
                scrollAreaLineIndex += TotalLineNum;
            }
            NormalizeLineIndex(ref scrollAreaLineIndex, ref offsetPixel);

            if (this.IsItemFit)
            {
                if (offsetPixel > ItemGapLine * 0.5f)
                {
                    scrollAreaLineIndex += 1;
                }

                offsetPixel = 0.0f;
            }

            if (!this.IsLoop)
            {
                if (scrollAreaLineIndex >= CalcMaxLineIndex())
                {
                    scrollAreaLineIndex = CalcMaxLineIndex();
                    offsetPixel = 0.0f;
                }

                if (scrollAreaLineIndex < CalcMinLineIndex())
                {
                    scrollAreaLineIndex = CalcMinLineIndex();
                    offsetPixel = 0.0f;
                }
            }
            else
            {
                if (Math.Abs(scrollAreaLineIndex - scrollAreaLineIndex) > (this.TotalLineNum - 1) / 2)
                {
                    if (scrollAreaLineIndex - scrollAreaLineIndex > 0)
                    {
                        scrollAreaLineIndex -= this.TotalLineNum;
                    }
                    else
                    {
                        scrollAreaLineIndex += this.TotalLineNum;
                    }
                }
            }

            if (isSetup && withAnimate)
            {
                fitTargetIndex = scrollAreaLineIndex;
                fitTargetOffsetPixel = offsetPixel;

                animationState = AnimationState.Fit;
                fitInterpRate = 0.2f;
            }
            else
            {
                SetScrollAreaLineIndex(scrollAreaLineIndex, offsetPixel);
            }
        }

        private void ScrollTo(float ratio, bool withAnimate)
        {
            int tmpMaxLine = CalcMaxLineIndex(); //this.TotalLineNum;
            if (ratio == 1.0f)
            {
                ScrollTo(tmpMaxLine, 0.0f, withAnimate);
            }
            else
            {
                float tmpMaxLineReal = tmpMaxLine;
                float targetLineIndexReal = tmpMaxLineReal * ratio;
                int targetLineIndex = (int)targetLineIndexReal;
                float targetLineOffset = (targetLineIndexReal - targetLineIndex) * ItemGapLine;

                ScrollTo(targetLineIndex, targetLineOffset, withAnimate);
            }
        }

        private void OnSelectHandler(object sender, int index)
        {
            ToggleSelectItemID(index);
            this.FocusIndex = index;
        }

        private void ToggleSelectItemID(int itemId)
        {
            if (itemId != itemIdNoSelect)
            {
                SpinItem item = GetListItem(itemId);

                if (item != null)
                {
                    SetSelect(itemId, !item.IsSelected);
                    ScrollTo(itemId - (visibleCount / 2), 0.0f, true);
                }
            }
        }

        private void SetSelectStatus(int index, bool isSelect)
        {
            SpinItem item = GetListItem(index);

            if (item != null)
            {
                item.IsSelected = isSelect;
            }
        }

        private void UpdateFocus()
        {
            if (!isSetup)
            {
                return;
            }

            if (focusIndex < 0)
            {
                return;
            }

            for (int i = 0; i < listItem.Count; i++)
            {
                int index = listItem[i].listItem.ItemId;

                if (focusIndex == index)
                {
                    if (!listItem[i].listItem.IsFocused)
                    {
                        listItem[i].listItem.IsFocused = true;
                        listItem[i].updateFlag = true;
                    }
                }
                else if (listItem[i].listItem.IsFocused)
                {
                    listItem[i].listItem.IsFocused = false;
                    listItem[i].updateFlag = true;
                }
            }
        }

        private void UpdateListItem(bool isMod)
        {
            int cacheStartIndex = this.ScrollAreaFirstLine - cacheLineCount;
            int cacheEndIndex = this.ScrollAreaFirstLine + scrollAreaLineNum + cacheLineCount + 1;

            if (!this.IsLoop)
            {
                if (cacheStartIndex < 0)
                {
                    cacheStartIndex = 0;
                }
                if (cacheEndIndex > listItemNum)
                {
                    cacheEndIndex = listItemNum;
                }
            }

            bool isModify = isMod;

            ListContainer container = new ListContainer ();

            // all reset
            if (this.cacheEndIndex - 1 < cacheStartIndex || cacheEndIndex - 1 < this.cacheStartIndex)
            {
                while (listItem.Count != 0)
                {
                    container = listItem[0];
                    listItem.RemoveAt(0);
                    PoolContainer(ref container);
                    isModify = true;
                }

                for (int diffIndex = cacheStartIndex;
                            diffIndex < cacheEndIndex;
                            diffIndex++)
                {
                    container = GetContainer();
                    CreateListItem(ref container, diffIndex);
                    listItem.Add(container);
                    isModify = true;
                }
            }
            else if (this.cacheStartIndex != cacheStartIndex || this.cacheEndIndex != cacheEndIndex)
            {
                if (this.cacheStartIndex < cacheStartIndex)
                {
                    for (int diffIndex = cacheStartIndex - 1;
                                diffIndex >= this.cacheStartIndex;
                                diffIndex--)
                    {
                        container = listItem[0];
                        listItem.RemoveAt(0);
                        PoolContainer(ref container);
                        isModify = true;
                    }
                }

                if (this.cacheEndIndex > cacheEndIndex)
                {
                    for (int diffIndex = cacheEndIndex;
                                diffIndex < this.cacheEndIndex;
                                diffIndex++)
                    {
                        container = listItem[listItem.Count - 1];
                        listItem.RemoveAt(listItem.Count - 1);
                        PoolContainer(ref container);
                        isModify = true;
                    }
                }

                if (this.cacheStartIndex > cacheStartIndex)
                {
                    for (int diffIndex = this.cacheStartIndex - 1;
                                diffIndex >= cacheStartIndex;
                                diffIndex--)
                    {
                        container = GetContainer();
                        CreateListItem(ref container, diffIndex);
                        listItem.Insert(0, container);
                        isModify = true;
                    }
                }

                if (this.cacheEndIndex < cacheEndIndex)
                {
                    for (int diffIndex = this.cacheEndIndex;
                                diffIndex < cacheEndIndex;
                                diffIndex++)
                    {
                        container = GetContainer();
                        CreateListItem(ref container, diffIndex);
                        listItem.Add(container);
                        isModify = true;
                    }
                }
            }

            this.cacheStartIndex = cacheStartIndex;
            this.cacheEndIndex = cacheEndIndex;

            if (isModify)
            {
                for (int i = 0; i < listItem.Count; i++)
                {
                    int currentId = listItem[i].totalId;
                    listItem[i].listItem.X = CalcItemPosOnScrollNodeX(currentId);
                    listItem[i].listItem.Y = CalcItemPosOnScrollNodeY(currentId);

                    if (listItem[i].updateFlag)
                    {
                        if (ItemRequestAction != null)
                        {
                            SpinItemRequestEventArgs args = new SpinItemRequestEventArgs ();
                            args.Index = listItem[i].totalId;
                            ItemRequestAction(this, args);

                            listItem[i].updateFlag = false;
                        }
                    }

                }
            }


            // Alpha value of the item is changed according as Y of item and basePanel is changed.
            // (Notes: Item get on basePanel. When flick or drag, basePanel move) 
            for (int i = 0; i < listItem.Count; i++)
            {
                float yDash = listItem[i].listItem.Y + listItem[i].listItem.Height + basePanel.Y;
                int centerIndex = ScrollAreaLineNum / 2 + 1;

                if (yDash <= listItem[i].listItem.Height * centerIndex)
                {
                    listItem[i].listItem.Alpha = yDash / (listItem[i].listItem.Height * centerIndex);
                }
                else
                {
                    listItem[i].listItem.Alpha = 2.0f - (yDash / (listItem[i].listItem.Height * centerIndex));
                }
            }
        }

        private ListContainer GetContainer()
        {
            ListContainer container = new ListContainer ();

            if (poolItem.Count != 0)
            {
                container = poolItem[0];
                poolItem.RemoveAt(0);
                container.listItem.Visible = true;
                container.listItem.TouchResponse = true;
            }
            else
            {
                container = new ListContainer ();
                container.listItem = null;
            }

            return container;
        }

        private void PoolContainer(ref ListContainer container)
        {
            if (container != null)
            {
                poolItem.Add(container);
                container.listItem.Visible = false;
            }
        }

        private void CreateListItem(ref ListContainer listContainer, int index)
        {
            SpinItem item;

            if (listContainer.listItem == null)
            {
                item = new SpinItem ();
            }
            else
            {
                item = listContainer.listItem;
            }

            listContainer.listItem = item;
            listContainer.totalId = index;
            listContainer.updateFlag = true;

            item.ItemId = CalcItemIndex(index);
            this.basePanel.AddChildLast(item);
            item.ProtectedSelectAction += new SpinItem.ProtectedSelectActionHandler (OnSelectHandler);

            item.IsFocused = (focusIndex == CalcItemIndex(index));
        }

        private int CalcMinLineIndex()
        {
            return 0;
        }

        private int CalcMaxLineIndex()
        {
            return Math.Max(this.TotalLineNum - scrollAreaLineNum, 0);
        }

        private int CalcStepIndex(int index)
        {
            return 0;
        }

        private int CalcLineIndex(int index)
        {
            return index;
        }

        private int CalcItemIndex(int totalId)
        {
            if (totalId < 0)
            {
                int round;
                round = (Math.Abs(totalId) - 1) / listItemNum;
                round = -round - 1;
                totalId = totalId + (-round * listItemNum);
                totalId = totalId % listItemNum;
            }
            else
            {
                totalId = totalId % listItemNum;
            }

            return totalId;
        }

        private float CalcItemPosOnScrollNodeX(int totalId)
        {
            float x = CalcStepIndex(totalId) * ItemGapStep;
            float y = (CalcLineIndex(totalId) - this.ScrollAreaFirstLine) * ItemGapLine;
            return GetRealPosX(x, y);
        }

        private float CalcItemPosOnScrollNodeY(int totalId)
        {
            float x = CalcStepIndex(totalId) * ItemGapStep;
            float y = (CalcLineIndex(totalId) - this.ScrollAreaFirstLine) * ItemGapLine;
            return GetRealPosY(x, y);
        }

        private void SetNodeLinePos(float pos)
        {
            this.basePanel.Y = -pos;
        }

        private float GetRealPosX(float virtualPosX, float virtualPosY)
        {
            return virtualPosX;
        }

        private float GetRealPosY(float virtualPosX, float virtualPosY)
        {
            return virtualPosY;
        }

        private float GetRealVecX(float virtualPosX, float virtualPosY)
        {
            return virtualPosX;
        }

        private float GetRealVecY(float virtualPosX, float virtualPosY)
        {
            return virtualPosY;
        }

        private float GetVirtualVec(Vector2 realVec)
        {
            return -realVec.Y;
        }


        /// @if LANG_JA
        /// <summary>タッチイベント時に呼び出されるハンドラ</summary>
        /// <param name="touchEvents">タッチイベント</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Handler called when there is a touch event</summary>
        /// <param name="touchEvents">Touch event</param>
        /// @endif
        protected internal override void OnTouchEvent(TouchEventCollection touchEvents)
        {
            base.OnTouchEvent(touchEvents);

            TouchEvent touchEvent = touchEvents.PrimaryTouchEvent;

            switch (touchEvent.Type)
            {
                case TouchEventType.Down:
                    animationState = AnimationState.None;
                    this.startTouchPos = touchEvent.LocalPosition;
                    isPress = true;
                    break;

                case TouchEventType.Move:
                    if (isPress)
                    {
                        Vector2 diff = startTouchPos - touchEvent.LocalPosition;
                        if (Math.Abs(GetVirtualVec(diff)) > minDelayDragDist)
                        {
                            animationState = AnimationState.Drag;
                        }
                    }
                    break;

                case TouchEventType.Up:
                    isPress = false;
                    if (animationState != AnimationState.Flick)
                    {
                        if (this.IsItemFit)
                        {
                            if (this.ScrollAreaPixelOffset > ItemGapLine * 0.5f)
                            {
                                fitTargetIndex = this.ScrollAreaFirstLine + 1;
                            }
                            else
                            {
                                fitTargetIndex = this.ScrollAreaFirstLine;
                            }
                            fitTargetOffsetPixel = 0.0f;

                            animationState = AnimationState.Fit;
                            fitInterpRate = 0.2f;
                        }
                    }
                    break;
            }

            if (animationState != AnimationState.Drag &&
                        animationState != AnimationState.Flick)
            {
                touchEvents.Forward = true;
                ;
            }
        }

        private void DragEventHandler(object sender, DragEventArgs e)
        {
            if (animationState == AnimationState.Drag)
            {
                float currentOffset = GetVirtualVec(e.Distance) + this.ScrollAreaPixelOffset;

                int currentIndex = this.ScrollAreaFirstLine;
                NormalizeLineIndex(ref currentIndex, ref currentOffset);

                if (currentIndex < CalcMinLineIndex() && !this.IsLoop)
                {
                    // under
                    SetScrollAreaLineIndex(CalcMinLineIndex(), 0.0f);
                }
                else if (currentOffset > 0.0f && currentIndex >= CalcMaxLineIndex() && !this.IsLoop)
                {
                    // over
                    SetScrollAreaLineIndex(CalcMaxLineIndex(), 0.0f);
                }
                else
                {
                    SetScrollAreaLineIndex(currentIndex, currentOffset);
                }
            }
        }

        private void FlickEventHandler(object sender, FlickEventArgs e)
        {
            animationState = AnimationState.Flick;
            {
                float flickSpeed = GetVirtualVec(e.Speed);
                flickDistance = flickSpeed * flickSpeedCoeff;
            }

            if (this.IsItemFit)
            {
                if (flickDistance > 0.0f)
                {
                    if (ItemGapLine * 2.0 - this.ScrollAreaPixelOffset - flickDistance / flickInterpRate > 0.0f)
                    {
                        animationState = AnimationState.Fit;
                        fitInterpRate = flickDistance / (ItemGapLine - this.ScrollAreaPixelOffset);
                        if (this.ScrollAreaFirstLine >= CalcMaxLineIndex() && !this.IsLoop)
                        {
                            fitTargetIndex = this.ScrollAreaFirstLine;
                        }
                        else
                        {
                            fitTargetIndex = this.ScrollAreaFirstLine + 1;
                        }
                        fitTargetOffsetPixel = 0.0f;
                    }
                }
                else
                {
                    if (ItemGapLine + this.ScrollAreaPixelOffset + flickDistance / flickInterpRate > 0.0)
                    {
                        animationState = AnimationState.Fit;
                        fitInterpRate = -flickDistance / this.ScrollAreaPixelOffset;
                        fitTargetIndex = this.ScrollAreaFirstLine;
                        fitTargetOffsetPixel = 0.0f;
                    }
                }
            }
        }


        /// @if LANG_JA
        /// <summary>自分自身と、自分以下の全ウィジェットの状態をリセットする。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Resets the status of all widgets for self and under self.</summary>
        /// @endif
        protected internal override void OnResetState()
        {
            base.OnResetState();

            isPress = false;
            if (this.ScrollAreaPixelOffset > ItemGapLine * 0.5f)
            {
                fitTargetIndex = this.ScrollAreaFirstLine + 1;
            }
            else
            {
                fitTargetIndex = this.ScrollAreaFirstLine;
            }
            fitTargetOffsetPixel = 0.0f;
            animationState = AnimationState.Fit;
            fitInterpRate = 0.2f;
        }


        /// @if LANG_JA
        /// <summary>更新処理</summary>
        /// <param name="elapsedTime">前回のUpdateからの経過時間</param>
        /// @endif
        /// @if LANG_EN
        /// <summary>Update processing</summary>
        /// <param name="elapsedTime">Elapsed time from previous update</param>
        /// @endif
        protected override void OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            if (animationState == AnimationState.Flick)
            {
                if (0.5f > flickDistance
                            && -0.5f < flickDistance
                            && !this.IsItemFit)
                {
                    flickDistance = 0.0f;
                    animationState = AnimationState.None;
                }

                float currentOffset = flickDistance + this.ScrollAreaPixelOffset;

                int currentIndex = this.ScrollAreaFirstLine;
                NormalizeLineIndex(ref currentIndex, ref currentOffset);

                if (currentIndex < CalcMinLineIndex() && !this.IsLoop)
                {
                    // under
                    SetScrollAreaLineIndex(CalcMinLineIndex(), 0.0f);

                    animationState = AnimationState.None;
                }
                else if (currentOffset > 0.0f && currentIndex >= CalcMaxLineIndex() && !this.IsLoop)
                {
                    // over
                    SetScrollAreaLineIndex(CalcMaxLineIndex(), 0.0f);

                    animationState = AnimationState.None;
                }
                else
                {
                    SetScrollAreaLineIndex(currentIndex, currentOffset);
                }

                flickDistance = flickDistance * (1.0f - flickInterpRate);

                if (this.IsItemFit)
                {
                    if (flickDistance > 0.0f)
                    {
                        if (ItemGapLine - this.ScrollAreaPixelOffset - flickDistance < 0)
                        {
                            if (!this.IsLoop && this.ScrollAreaFirstLine + 2 > CalcMaxLineIndex())
                            {
                                float tmpDistance = ItemGapLine * (CalcMaxLineIndex() - this.ScrollAreaFirstLine) - this.ScrollAreaPixelOffset;
                                animationState = AnimationState.Fit;
                                if (tmpDistance < 0.5f)
                                {
                                    fitInterpRate = 0.2f;
                                }
                                else
                                {
                                    fitInterpRate = flickDistance / tmpDistance;
                                }
                                fitTargetIndex = CalcMaxLineIndex();
                                fitTargetOffsetPixel = 0.0f;
                            }
                            else if (ItemGapLine * 3.0f - this.ScrollAreaPixelOffset - flickDistance / flickInterpRate > 0.0f)
                            {
                                animationState = AnimationState.Fit;
                                fitInterpRate = flickDistance / (ItemGapLine * 2.0f - this.ScrollAreaPixelOffset);
                                fitTargetIndex = this.ScrollAreaFirstLine + 2;
                                fitTargetOffsetPixel = 0.0f;
                            }
                        }
                    }
                    else
                    {
                        if (this.ScrollAreaPixelOffset + flickDistance < 0.0f)
                        {
                            if (!this.IsLoop && this.ScrollAreaFirstLine - 1 < CalcMinLineIndex())
                            {
                                float tmpDistance = -(ItemGapLine * (this.ScrollAreaFirstLine - CalcMinLineIndex()) + this.ScrollAreaPixelOffset);
                                animationState = AnimationState.Fit;
                                if (tmpDistance < 0.5f)
                                {
                                    fitInterpRate = 0.8f;
                                }
                                else
                                {
                                    fitInterpRate = flickDistance / tmpDistance;
                                }
                                fitTargetIndex = CalcMinLineIndex();
                                fitTargetOffsetPixel = 0.0f;
                            }
                            else if (ItemGapLine * 2.0f + this.ScrollAreaPixelOffset + flickDistance / flickInterpRate > 0.0f)
                            {
                                animationState = AnimationState.Fit;
                                fitInterpRate = -flickDistance / (ItemGapLine * 1.0f + this.ScrollAreaPixelOffset);
                                fitTargetIndex = this.ScrollAreaFirstLine - 1;
                                fitTargetOffsetPixel = 0.0f;
                            }
                        }
                    }
                }
            }
            else if (animationState == AnimationState.Fit)
            {
                int diffIndex = this.ScrollAreaFirstLine - fitTargetIndex;
                if (this.isLoop)
                {
                    if (diffIndex > this.TotalLineNum / 2)
                    {
                        diffIndex -= this.TotalLineNum;
                    }
                    else if (diffIndex < -this.TotalLineNum / 2)
                    {
                        diffIndex += this.TotalLineNum;
                    }
                }

                float diffIndexReal = diffIndex * (1.0f - fitInterpRate);
                diffIndex = (int)(diffIndexReal);
                float diffPixel = (diffIndexReal - diffIndex) * ItemGapLine;
                diffPixel += (this.ScrollAreaPixelOffset - fitTargetOffsetPixel) * (1.0f - fitInterpRate);

                SetScrollAreaLineIndex(diffIndex + fitTargetIndex, diffPixel + fitTargetOffsetPixel);

                if (diffIndex == 0 && Math.Abs(diffPixel) <= 0.5f)
                {
                    animationState = AnimationState.None;
                    SetScrollAreaLineIndex(fitTargetIndex, fitTargetOffsetPixel);

                    if (FocusChanged != null)
                    {
                        FocusChanged(this, EventArgs.Empty);
                    }
                    this.focusIndex = CalcItemIndex(this.ScrollAreaFirstLine + visibleCount / 2); 
                }
            }
        }


        /// @if LANG_JA
        /// <summary>リストアイテムリクエストハンドラ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>List item request handler</summary>
        /// @endif
        public event EventHandler<SpinItemRequestEventArgs> ItemRequestAction;

        // TODO: rename member name
        public new event EventHandler<EventArgs> FocusChanged;

        private Panel basePanel;
        private UInt16 scrollAreaLineNum;
        private int listItemNum;
        private UInt16 cacheLineCount;
        private int focusIndex;
        private int lastSelectIndex;
        private int cacheStartIndex;
        private int cacheEndIndex;
        private bool isSetup;
        public System.Collections.Generic.List<ListContainer> listItem;
        private System.Collections.Generic.List<ListContainer> poolItem;
        private AnimationState animationState;
        private float flickDistance;
        public float flickInterpRate;
        public float flickSpeedCoeff;
        private int fitTargetIndex;
        private float fitTargetOffsetPixel;
        public float fitInterpRate;
        private bool isPress;
        private Vector2 startTouchPos;
        private float minDelayDragDist;

    }

    internal class SpinItemRequestEventArgs : EventArgs
    {
        public int Index
        {
            get;
            set;
        }
    }
    }

}

