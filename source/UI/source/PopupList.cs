/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>複数の選択肢から１つを選択するためのウィジェット</summary>
        /// <remarks>モーダルダイアログでリストが表示される</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Widget for selecting one option from among multiple options</summary>
        /// <remarks>Displays a list in the modal dialog</remarks>
        /// @endif
        public class PopupList : Widget
        {
            private const float defaultWidth = 360.0f;
            private const float defaultHeight = 56.0f;

            private const float selectedLabelMargin = 10.0f;
            private const float dialogMargin = 30.0f;
            private const float listItemLabelLeftMargin = 40.0f;
            private const float listItemLabelRightMargin = 10.0f;
            private const float listItemHeightRate = 2.0f;
            private const float titleHorizontalMargin = 10.0f;

            private const float dialogFadeEffectTime = 300.0f;

            private ImageAsset[] backgroundImages;
            private NinePatchMargin backgroundNinePatch;
            private ImageAsset itemSelectedImageAsset;
            private UIColor itemSelectedImageColor;
            private NinePatchMargin itemSelectedImageNinePatch;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PopupList()
            {
                backgroundImages = new ImageAsset[3];
                backgroundImages[0] = new ImageAsset(SystemImageAsset.PopupListBackgroundNormal);
                backgroundImages[1] = new ImageAsset(SystemImageAsset.PopupListBackgroundPressed);
                backgroundImages[2] = new ImageAsset(SystemImageAsset.PopupListBackgroundDisabled);
                backgroundNinePatch = AssetManager.GetNinePatchMargin(SystemImageAsset.PopupListBackgroundNormal);

                itemSelectedImageAsset = new ImageAsset(SystemImageAsset.PopupListItemFocus);
                itemSelectedImageColor = new UIColor(0.9f, 0.9f, 0.9f, 1.0f);
                itemSelectedImageNinePatch = AssetManager.GetNinePatchMargin(SystemImageAsset.PopupListItemFocus);

                
                backGroundImage = new ImageBox();
                backGroundImage.Image = backgroundImages[0];
                backGroundImage.NinePatchMargin = backgroundNinePatch;
                backGroundImage.ImageScaleType = ImageScaleType.NinePatch;
                backGroundImage.TouchResponse = false;
                AddChildLast(backGroundImage);

                selectedLabel = new Label();
                selectedLabel.X = selectedLabelMargin;
                selectedLabel.HorizontalAlignment = HorizontalAlignment.Left;
                selectedLabel.TextTrimming = TextTrimming.EllipsisCharacter;
                selectedLabel.LineBreak = LineBreak.AtCode;
                selectedLabel.Text = "";
                selectedLabel.TouchResponse = false;
                AddChildLast(selectedLabel);

                ListTitle = "";
                ListTitleFont = new UIFont();
                ListItemFont = new UIFont();
                listTitleTextColor = new UIColor(1, 1, 1, 1f);
                listItemTextColor = new UIColor(1, 1, 1, 1f);


                listItems = new PopupListItemCollection();
                listItems.ItemChanged += HandleListItemsItemChanged;

                this.Width = defaultWidth;
                this.Height = defaultHeight;

                this.Pressable = true;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                foreach (var image in backgroundImages)
                {
                    image.Dispose();
                }

                if (itemSelectedImageAsset != null)
                {
                    itemSelectedImageAsset.Dispose();
                }

                base.DisposeSelf();
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

                    if (backGroundImage != null)
                    {
                        backGroundImage.Width = value;
                    }

                    if (selectedLabel != null)
                    {
                        float w = value - selectedLabel.X - backgroundNinePatch.Right;
                        selectedLabel.Width = w > 0 ? w : 0;
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

                    if (backGroundImage != null)
                    {
                        backGroundImage.Height = value;
                    }

                    if (selectedLabel != null)
                    {
                        selectedLabel.Height = value;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>リストのタイトルを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the list title.</summary>
            /// @endif
            public string ListTitle
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>文字列のフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the font of the character string.</summary>
            /// @endif
            public UIFont Font
            {
                get
                {
                    return selectedLabel.Font;
                }
                set
                {
                    selectedLabel.Font = value;
                }
            }

            /// @if LANG_JA
            /// <summary>文字列の色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the character string.</summary>
            /// @endif
            public UIColor TextColor
            {
                get
                {
                    return selectedLabel.TextColor;
                }
                set
                {
                    selectedLabel.TextColor = value;
                }
            }

            /// @if LANG_JA
            /// <summary>文字列の影の情報を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the information of the shadow of the character string.</summary>
            /// @endif
            public TextShadowSettings TextShadow
            {
                get
                {
                    return selectedLabel.TextShadow;
                }
                set
                {
                    selectedLabel.TextShadow = value;
                }
            }

            /// @if LANG_JA
            /// <summary>文字列のトリミング方法を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the cropping method of the character string.</summary>
            /// @endif
            public TextTrimming TextTrimming
            {
                get
                {
                    return selectedLabel.TextTrimming;
                }
                set
                {
                    selectedLabel.TextTrimming = value;
                }
            }

            private UIFont listItemFont;

            /// @if LANG_JA
            /// <summary>リストアイテムのフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the list item font.</summary>
            /// @endif
            public UIFont ListItemFont
            {
                get { return listItemFont; }
                set { listItemFont = value; }
            }

            private UIColor listItemTextColor;

            /// @if LANG_JA
            /// <summary>リストアイテムのテキストカラーを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the list item text color.</summary>
            /// @endif
            public UIColor ListItemTextColor
            {
                get { return listItemTextColor; }
                set { listItemTextColor = value; }
            }

            private UIFont listTitleFont;

            /// @if LANG_JA
            /// <summary>リストタイトルのフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the list title font.</summary>
            /// @endif
            public UIFont ListTitleFont
            {
                get { return listTitleFont; }
                set { listTitleFont = value; }
            }

            private UIColor listTitleTextColor;

            /// @if LANG_JA
            /// <summary>リストタイトルのテキストカラーを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the list title text color.</summary>
            /// @endif
            public UIColor ListTitleTextColor
            {
                get { return listTitleTextColor; }
                set { listTitleTextColor = value; }
            }


            private int selectedIndex = 0;

            /// @if LANG_JA
            /// <summary>選択されたListItemのIndex</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Index of selected ListItem</summary>
            /// @endif
            public int SelectedIndex
            {
                get { return selectedIndex; }
                set
                {
                    if (value >= 0 && value < listItems.Count)
                    {
                        selectedIndex = value;
                        selectedLabel.Text = listItems[selectedIndex];
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>リストのアイテムの文字列を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the character string of a list item.</summary>
            /// @endif
            public PopupListItemCollection ListItems
            {
                get
                {
                    return listItems;
                }
                set
                {
                    listItems.ItemChanged -= HandleListItemsItemChanged;
                    if (value == null)
                    {
                        listItems.Clear();
                    }
                    else
                    {
                        listItems = value;
                    }
                    listItems.ItemChanged += HandleListItemsItemChanged;
                    HandleListItemsItemChanged(this, EventArgs.Empty);
                }
            }

            private PopupListItemCollection listItems;

            /// @if LANG_JA
            /// <summary>プレス状態が変化したときに呼び出される</summary>
            /// <param name="e">イベント引数</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Called when the press status changes</summary>
            /// <param name="e">Event argument</param>
            /// @endif
            protected override void OnPressStateChanged(PressStateChangedEventArgs e)
            {
                backGroundImage.Image = backgroundImages[(int)PressState];

                if (e.checkDetectedButtonAction())
                {
                    ShowDialog();
                }
            }


            private void ShowDialog()
            {
                if (dialog == null)
                {
                    previousSelectedIndex = SelectedIndex;
                    listSelectedIndex = SelectedIndex;

                    currentListItemFont = listItemFont;
                    currentListItemTextColor = listItemTextColor;
                    currentListItemHeight = (int)(currentListItemFont.Size * listItemHeightRate);

                    // dialog
                    dialog = new Dialog();
                    dialog.HideOnTouchOutside = true;

                    dialog.ShowEffect = new FadeInEffect() { Time = dialogFadeEffectTime };
                    dialog.HideEffect = new FadeOutEffect() { Time = dialogFadeEffectTime };

                    dialog.Hidden += dialog_Hidden;
                    dialog.Shown += dialog_Shown; 


                    // title
                    Label titleLabel = new Label();
                    titleLabel.X = titleHorizontalMargin;
                    titleLabel.Y = 0.0f;
                    titleLabel.Text = ListTitle;
                    titleLabel.Font = listTitleFont ?? new UIFont();
                    titleLabel.TextColor = listTitleTextColor;
                    titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    dialog.AddChildLast(titleLabel);
                    
                    // calculate dialog size
                    var coreTitleFont = (listTitleFont ?? new UIFont()).GetFont();
                    var coreItemFont = (listItemFont ?? new UIFont()).GetFont();
                    int maxTextWidth = coreTitleFont.GetTextWidth(ListTitle ?? "");
                    foreach (string item in ListItems)
                    {
                        int textWidth = coreItemFont.GetTextWidth(item);
                        if (maxTextWidth < textWidth)
                        {
                            maxTextWidth = textWidth;
                        }
                    }
                    coreTitleFont.Dispose();
                    coreItemFont.Dispose();

                    var scene = this.ParentScene;
                    float width, height;
                    if (scene != null)
                    {
                        width = scene.RootWidget.Width;
                        height = scene.RootWidget.Height;
                    }
                    else
                    {
                        width = UISystem.FramebufferWidth;
                        height = UISystem.FramebufferHeight;
                    }

                    var dialogMinWidth = width * 0.5f;
                    var dialogMaxWidth = width * 0.9f;
                    var dialogMinHeight = currentListItemHeight * 2 + 10.0f;
                    var dialogMaxHeight = height * 0.9f;
                    dialog.Width = (int)FMath.Clamp(maxTextWidth * 1.5f, dialogMinWidth, dialogMaxWidth);
                    dialog.Height = (int)FMath.Clamp(currentListItemHeight * (listItems.Count + 1) + 10.0f, dialogMinHeight, dialogMaxHeight);
                    titleLabel.Width = dialog.Width - titleHorizontalMargin;
                    titleLabel.Height = (int)(titleLabel.Font.Size * listItemHeightRate);

                    // list
                    ListSectionCollection section = new ListSectionCollection { 
                        new ListSection("", ListItems.Count) 
                    };

                    listPanel = new ListPanel();
                    listPanel.X = titleLabel.X;
                    listPanel.Y = titleLabel.Y + titleLabel.Height;
                    listPanel.Width = titleLabel.Width;
                    listPanel.Height = dialog.Height - (titleLabel.Y + titleLabel.Height) - 10.0f;
                    listPanel.SetListItemCreator(ListItemCreator);
                    listPanel.SetListItemUpdater(ListItemUpdater);
                    listPanel.ShowSection = false;
                    listPanel.Sections = section;
                    listPanel.SelectItemChanged += listPanel_SelectItemChanged;
                    DragGestureDetector drag = new DragGestureDetector();
                    drag.DragDetected += new EventHandler<DragEventArgs>(dragEventHandler);
                    listPanel.AddGestureDetector(drag);
                    listPanel.PriorityHit = this.PriorityHit;
                    dialog.AddChildLast(listPanel);

                    // move to selected item
                    int displayedItemCount = (int)(listPanel.Height / currentListItemHeight);
                    float displaydTopItemIndex = listSelectedIndex - displayedItemCount / 2;
                    float moveDistance = -(displaydTopItemIndex * currentListItemHeight);
                    if (displayedItemCount % 2 == 0)
                    {
                        moveDistance -= currentListItemHeight / 2.0f;
                    }
                    listPanel.Move(moveDistance);

                    var firstFocusItem = listPanel.GetListItem(listSelectedIndex);
                    dialog.DefaultFocusWidget = firstFocusItem;

                    dialog.Show();
                }
            }
            ListPanel listPanel;
            void dialog_Shown(object sender, EventArgs e)
            {
                listPanel.UpdateItems();
            }

            void dialog_Hidden(object sender, DialogEventArgs e)
            {
                dialog.Dispose();
                dialog = null;
            }

            void listPanel_SelectItemChanged(object sender, ListPanelItemSelectChangedEventArgs e)
            {
                SelectedIndex = e.Index;
                dialog.Result = DialogResult.Ok;
                dialog.Hide();
                if (SelectionChanged != null && previousSelectedIndex != selectedIndex)
                {
                    SelectionChanged(this, new PopupSelectionChangedEventArgs(previousSelectedIndex, SelectedIndex));
                }
            }

            UIFont currentListItemFont;
            UIColor currentListItemTextColor;
            float currentListItemHeight;

            private ListPanelItem ListItemCreator()
            {
                PopupListPanelItem item = new PopupListPanelItem();

                item.Label.Font = currentListItemFont;
                item.Label.TextColor = currentListItemTextColor;
                item.Height = currentListItemHeight;

                item.selectedSprite.Image = this.itemSelectedImageAsset;
                item.selectedSprite.ShaderType = ShaderType.Texture;
                for (int i = 0; i < item.selectedSprite.UnitCount; i++)
                {
                    item.selectedSprite.GetUnit(i).Color = this.itemSelectedImageColor;
                }

                item.selectedImageNinePatch = this.itemSelectedImageNinePatch;

                item.HookChildTouchEvent = true;
                return item;
            }

            private void ListItemUpdater(ListPanelItem item)
            {
                if (item is PopupListPanelItem)
                {
                    PopupListPanelItem updateItem = (item as PopupListPanelItem);
                    updateItem.Label.Text = ListItems[updateItem.Index];
                    updateItem.IsSelected = (item.Index == listSelectedIndex);
                }
            }

            private int previousSelectedIndex = 0;
            private int listSelectedIndex = 0;


            private void dragEventHandler(object sender, DragEventArgs e)
            {
                ResetState(false);
            }

            void HandleListItemsItemChanged (object sender, EventArgs e)
            {
                if(selectedIndex >= listItems.Count)
                {
                    selectedIndex = listItems.Count - 1;
                    if(selectedIndex <= 0)
                    {
                        selectedIndex = 0;
                    }
                }
                
                selectedLabel.Text = listItems.Count > 0 ? listItems[selectedIndex] : "";

                if(this.ListItemsChanged != null)
                {
                    ListItemsChanged(this, new PopupListItemsChangedEventArgs());
                }
            }
            
            /// @if LANG_JA
            /// <summary>選択中のアイテムが変わったときに呼ばれるイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event called when selected item changes</summary>
            /// @endif
            public event EventHandler<PopupSelectionChangedEventArgs> SelectionChanged;
            
            /// @if LANG_JA
            /// <summary>ListItemsプロパティの内容が変更されたときに呼ばれるイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event called when the content of the ListItems properties is updated</summary>
            /// @endif
            public event EventHandler<PopupListItemsChangedEventArgs> ListItemsChanged;

            private class PopupListPanelItem : ListPanelItem
            {
                private readonly float selectItemDelay = 100.0f;

                public PopupListPanelItem()
                {
                    selectedSprite = new UISprite(3);
                    selectedSprite.Visible = false;
                    RootUIElement.AddChildLast(selectedSprite);

                    label = new Label();
                    label.X = listItemLabelLeftMargin;
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    AddChildLast(label);
                }

                void updateSprite()
                {
                    if (selectedSprite != null)
                    {
                        UISpriteUtility.SetupHorizontalThreePatch(selectedSprite, this.Width, this.Height,
                            selectedImageNinePatch.Left, selectedImageNinePatch.Right);
                    }
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
                            label.Width = value - listItemLabelLeftMargin - listItemLabelRightMargin;
                        }

                        updateSprite();
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
                            label.Height = value;
                        }

                        updateSprite();
                    }
                }

                public Label Label
                {
                    get { return label; }
                    set { label = value; }
                }

                public bool IsSelected
                {
                    get
                    {
                        return isSelected;
                    }
                    set
                    {
                        isSelected = value;
                        updateSelectSprite();
                    }
                }
                private bool isSelected;

                /// @if LANG_JA
                /// <summary>プレス状態が変化したときに呼び出される</summary>
                /// <param name="e">イベント引数</param>
                /// @endif
                /// @if LANG_EN
                /// <summary>Called when the press status changes</summary>
                /// <param name="e">Event argument</param>
                /// @endif
                protected override void OnPressStateChanged(PressStateChangedEventArgs e)
                {
                    isPressed = false;
                    if (e.NewState == PressState.Pressed)
                    {
                        pressTime = UISystem.CurrentTime;
                        prePress = true;
                    }
                    else
                    {
                        prePress = false;
                    }

                    updateSelectSprite();

                    base.OnPressStateChanged(e);
                }

                protected override void OnUpdate(float elapsedTime)
                {
                    base.OnUpdate(elapsedTime);

                    if (prePress && (UISystem.CurrentTime - pressTime).TotalMilliseconds > selectItemDelay)
                    {
                        isPressed = true;
                        prePress = false;
                        updateSelectSprite();
                    }
                }

                private void updateSelectSprite()
                {
                    selectedSprite.Visible = IsSelected || isPressed;
                    selectedSprite.Alpha = isPressed ? 1.0f : 0.5f;
                }

                private TimeSpan pressTime;
                bool prePress = false;
                bool isPressed = false;
                private Label label;
                public UISprite selectedSprite;
                public NinePatchMargin selectedImageNinePatch;
            }

            private ImageBox backGroundImage;
            private Label selectedLabel;
            private Dialog dialog;

        }
  
        
        /// @if LANG_JA
        /// <summary>PopupListのListItems用コレクション</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Collection for ListItems of PopupList</summary>
        /// @endif
        public class PopupListItemCollection : IList<String>
        {
            private List<string> list;
            
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PopupListItemCollection ()
            {
                list = new List<string>();
            }
            
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name='items'>初期値</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="items">Initial value</param>
            /// @endif
            public PopupListItemCollection (IEnumerable<string> items)
            {
                list = new List<string>(items);
            }
            
            /// @if LANG_JA
            /// <summary>複数の項目を末尾に追加する</summary>
            /// <param name="items">追加する項目のリスト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds multiple items to the end</summary>
            /// <param name="items">Added item list</param>
            /// @endif
            public void AddRange(IEnumerable<String> items)
            {
                list.AddRange(items);
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }

            /// @if LANG_JA
            /// <summary>指定したインデックスの位置に複数の項目を挿入する。</summary>
            /// <param name="index">挿入する位置のインデックス。</param>
            /// <param name="items">挿入する項目のリスト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts multiple items in a specified index position.</summary>
            /// <param name="index">Index of position to insert.</param>
            /// <param name="items">List of inserted items</param>
            /// @endif
            public void InsertRange(int index, IEnumerable<String> items)
            {
                list.InsertRange(index, items);
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }
            
            
            #region IList[String] implementation
            /// @if LANG_JA
            /// <summary>指定した項目のインデックスを取得する。</summary>
            /// <param name="item">検索する文字列</param>
            /// <returns>検索する文字列が存在する場合はそのインデックス。それ以外の場合は -1。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index of a specified item.</summary>
            /// <param name="item">Text to be searched</param>
            /// <returns>If the text to search exists, the index of that text. Otherwise, -1.</returns>
            /// @endif
            public int IndexOf (String item)
            {
                return this.list.IndexOf(item);
            }
   
            /// @if LANG_JA
            /// <summary>指定したインデックスの位置に項目を挿入する。</summary>
            /// <param name="index">挿入する位置のインデックス。</param>
            /// <param name="item">挿入する項目</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts the item in a specified index position.</summary>
            /// <param name="index">Index of position to insert.</param>
            /// <param name="item">Item to insert</param>
            /// @endif
            public void Insert (int index, String item)
            {
                this.list.Insert(index, item);
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }
   
            /// @if LANG_JA
            /// <summary>指定したインデックスにある項目を削除する。</summary>
            /// <param name="index">削除する項目のインデックス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the item in a specified index.</summary>
            /// <param name="index">Index of item to be deleted</param>
            /// @endif
            public void RemoveAt (int index)
            {
                this.list.RemoveAt(index);
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }

            
            /// @if LANG_JA
            /// <summary>指定したインデックスにある項目を取得・設定する。</summary>
            /// <param name="index">項目のインデックス</param>
            /// <returns>指定したインデックスにある項目</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the item in a specified index.</summary>
            /// <param name="index">Index of item</param>
            /// <returns>Item in a specified index</returns>
            /// @endif
            public String this[int index]
            {
                get
                {
                    return list[index];
                }
                set
                {
                    list[index] = value;
                    if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
                }
            }
            #endregion

            #region IEnumerable[String] implementation
            /// @if LANG_JA
            /// <summary>コレクションを反復処理する列挙子を返す。</summary>
            /// <returns>コレクションを反復処理するために使用できるIEnumeratorオブジェクト</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns the enumerator that repetitively handles the collection.</summary>
            /// <returns>IEnumerator object that can be used to repetitively handle a collection</returns>
            /// @endif
            public IEnumerator<String> GetEnumerator ()
            {
                return list.GetEnumerator();
            }
            #endregion

            #region IEnumerable implementation
            /// @if LANG_JA
            /// <summary>コレクションを反復処理する列挙子を返す。</summary>
            /// <returns>コレクションを反復処理するために使用できるIEnumeratorオブジェクト</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Returns the enumerator that repetitively handles the collection.</summary>
            /// <returns>IEnumerator object that can be used to repetitively handle a collection</returns>
            /// @endif
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
            {
                return ((System.Collections.IEnumerable)list).GetEnumerator();
            }
            #endregion

            #region ICollection[String] implementation
            /// @if LANG_JA
            /// <summary>項目</summary>
            /// <param name="item">項目</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Item</summary>
            /// <param name="item">Item</param>
            /// @endif
            public void Add (String item)
            {
                list.Add(item);
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }
   
            /// @if LANG_JA
            /// <summary>すべての項目を削除する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes all items.</summary>
            /// @endif
            public void Clear ()
            {
                list.Clear();
                if(ItemChanged != null) ItemChanged(this, EventArgs.Empty);
            }
   
            /// @if LANG_JA
            /// <summary>すべての項目を削除する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes all items.</summary>
            /// @endif
            public bool Contains (String item)
            {
                return list.Contains(item);
            }
   
            /// @if LANG_JA
            /// <summary>項目を配列にコピーする。</summary>
            /// <param name="array">項目がコピーされるString配列</param>
            /// <param name="arrayIndex">コピーの開始位置となる配列のインデックス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Copies an item to an array.</summary>
            /// <param name="array">String array where an item will be copied</param>
            /// <param name="arrayIndex">Index of an array that is the start position for copying</param>
            /// @endif
            public void CopyTo (String[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }
   
            /// @if LANG_JA
            /// <summary>最初に見つかった特定の項目を削除する。</summary>
            /// <param name="item">削除する項目</param>
            /// <returns>削除するセクションが存在する場合はtrue。それ以外の場合はfalse。</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes a specific item found first.</summary>
            /// <param name="item">Item to be deleted</param>
            /// <returns>If the section to be deleted exists, then true. Otherwise, false.</returns>
            /// @endif
            public bool Remove (String item)
            {
                bool success = list.Remove(item);
                if(success && ItemChanged != null) ItemChanged(this, EventArgs.Empty);
                return success;
            }
   
            /// @if LANG_JA
            /// <summary>項目数を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the number of items.</summary>
            /// @endif
            public int Count
            {
                get
                {
                    return list.Count;
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
            #endregion
            
            internal event EventHandler ItemChanged;
        }
        
        
        /// @if LANG_JA
        /// <summary>PopupList の選択中のアイテムが変わったときに呼ばれるイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event argument called when item selected in PopupList changes</summary>
        /// @endif
        public class PopupSelectionChangedEventArgs : EventArgs
        {
            int oldIndex;
            int newIndex;

            /// @if LANG_JA
            /// <summary>PopupList の選択中のアイテムが変わったときのイベント</summary>
            /// <param name="oldIndex">前回選択されていたアイテムのインデックス</param>
            /// <param name="newIndex">今回選択されていたアイテムのインデックス</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event when item selected in PopupList changes</summary>
            /// <param name="oldIndex">Index of item previously selected</param>
            /// <param name="newIndex">Index of item currently selected</param>
            /// @endif
            public PopupSelectionChangedEventArgs(int oldIndex, int newIndex)
            {
                this.oldIndex = oldIndex;
                this.newIndex = newIndex;
            }

            /// @if LANG_JA
            /// <summary>前回選択されていたアイテムのインデックス</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Index of item previously selected</summary>
            /// @endif
            public int OldIndex
            {
                get { return this.oldIndex; }
            }

            /// @if LANG_JA
            /// <summary>今回選択されていたアイテムのインデックス</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Index of item currently selected</summary>
            /// @endif
            public int NewIndex
            {
                get { return this.newIndex; }
            }
        }

        /// @if LANG_JA
        /// <summary>PopupListのListItemsChangedイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event argument for ListItemsChanged of PopupList</summary>
        /// @endif
        public class PopupListItemsChangedEventArgs : EventArgs
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public PopupListItemsChangedEventArgs ()
            {
             
            }
        }
    }
}
