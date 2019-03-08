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
        /// <summary>リストのアイテム用のコンテナウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Container widget for list item</summary>
        /// @endif
        public class ListPanelItem : Panel
        {

            private const float edgeHeight = 1.0f;
            private const float defaultWidth = 200.0f;
            private const float defaultHeight = 60.0f;
            static NinePatchMargin margin = AssetManager.GetNinePatchMargin(SystemImageAsset.ListPanelSeparatorTop);

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ListPanelItem()
            {
                edgeContainer = new ContainerWidget();
                edgeContainer.Visible = false;
                edgeContainer.TouchResponse = false;
                base.AddChildLast(edgeContainer);

                index = -1;
                SectionIndex = -1;
                IndexInSection = -1;

                this.Width = defaultWidth;
                this.Height = defaultHeight;
                this.PriorityHit = true;
                this.Focusable = true;
                this.Pressable = true;
                this.FocusStyle = FocusStyle.Rectangle;
                this.HookChildTouchEvent = true;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (upperEdgeSprt != null && upperEdgeSprt.Image != null)
                {
                    upperEdgeSprt.Image.Dispose();
                }
                if (lowerEdgeSprt != null && lowerEdgeSprt.Image != null)
                {
                    lowerEdgeSprt.Image.Dispose();
                }

                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>幅を取得・設定する。</summary>
            /// <remarks>ListPanelクラスからのみ値を変更することができる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the width.</summary>
            /// <remarks>Values can be changed only from the ListPanel class.</remarks>
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

                    if (edgeContainer != null)
                    {
                        edgeContainer.Width = value;
                    }

                    if (hideEdge == false)
                    {
                        SetupSprite();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>高さを取得・設定する。</summary>
            /// <remarks>具象クラスまたはコールバックからのみ値を変更することができる。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the height.</summary>
            /// <remarks>Values can be changed only from a concrete class or callback.</remarks>
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

                    if (edgeContainer != null)
                    {
                        edgeContainer.Height = value;
                    }

                    if (hideEdge == false)
                    {
                        SetupSprite();
                    }
                }
            }

            private int index;

            /// @if LANG_JA
            /// <summary>リスト全体でのインデックスを取得する。</summary>
            /// <remarks>初期値は-1</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index for all lists.</summary>
            /// <remarks>Default value is -1</remarks>
            /// @endif
            public virtual int Index
            {
                get { return index; }
                set { index = value; }
            }

            /// @if LANG_JA
            /// <summary>セクション内でのインデックスを取得する。</summary>
            /// <remarks>ListPanelでのみ使用する。初期値は-1</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index within a section.</summary>
            /// <remarks>Uses only with ListPanel. Default value is -1</remarks>
            /// @endif
            public virtual int IndexInSection
            {
                get;
                set;
            }

            /// @if LANG_JA
            /// <summary>セクションのインデックスを取得する。</summary>
            /// <remarks>ListPanelでのみ使用する。初期値は-1</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index of a section.</summary>
            /// <remarks>Uses only with ListPanel. Default value is -1</remarks>
            /// @endif
            public int SectionIndex
            {
                get;
                set;
            }


            internal float ContainEdgeHeight
            {
                get
                {
                    if (HideEdge == true)
                    {
                        return edgeContainer.Height;
                    }
                    else
                    {
                        return base.Height;
                    }
                }
            }

            internal bool HideEdge
            {
                get
                {
                    return hideEdge;
                }
                set
                {
                    if (hideEdge != value)
                    {
                        hideEdge = value;

                        edgeContainer.Visible = !hideEdge;

                        if (hideEdge == false)
                        {
                            SetupSprite();
                        }
                    }
                }
            }
            private bool hideEdge = true;

            private void SetupSprite()
            {
                if (upperEdgeSprt == null || lowerEdgeSprt == null)
                {
                    upperEdgeSprt = new UISprite(3);
                    edgeContainer.RootUIElement.AddChildLast(upperEdgeSprt);
                    upperEdgeSprt.Image = new ImageAsset(SystemImageAsset.ListPanelSeparatorTop);
                    upperEdgeSprt.ShaderType = ShaderType.Texture;

                    lowerEdgeSprt = new UISprite(3);
                    edgeContainer.RootUIElement.AddChildLast(lowerEdgeSprt);
                    lowerEdgeSprt.Image = new ImageAsset(SystemImageAsset.ListPanelSeparatorBottom);
                    lowerEdgeSprt.ShaderType = ShaderType.Texture;
                }

                lowerEdgeSprt.Y = base.Height - edgeHeight;

                UISpriteUtility.SetupHorizontalThreePatch(upperEdgeSprt, edgeContainer.Width, edgeHeight, margin.Left, margin.Right);
                UISpriteUtility.SetupHorizontalThreePatch(lowerEdgeSprt, edgeContainer.Width, edgeHeight, margin.Left, margin.Right);
            }

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
                if (PressStateChanged != null)
                {
                    PressStateChanged(this, e);
                }
            }

            internal event EventHandler<PressStateChangedEventArgs> PressStateChanged;

            private UISprite upperEdgeSprt;
            private UISprite lowerEdgeSprt;
            private ContainerWidget edgeContainer;
        }
    }
}
