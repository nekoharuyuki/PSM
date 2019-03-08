/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>コンテナ機能を持つウィジェット</summary>
        /// <remarks>子ウィジェットを追加することができ、ウィジェットの親子関係を形成することができる。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Widget which has the container feature</summary>
        /// <remarks>A child widget can be added and the parent-child relationship of the widget can be formed.</remarks>
        /// @endif
        public class ContainerWidget : Widget
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public ContainerWidget()
            {
                EnabledChildrenAnchors = true;
            }


            /// @if LANG_JA
            /// <summary>子ウィジェットを取得する。</summary>
            /// <remarks>コレクションを反復処理する列挙子を返す。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the child widget.</summary>
            /// <remarks>Returns the enumerator that repetitively handles the collection.</remarks>
            /// @endif
            public new virtual IEnumerable<Widget> Children
            {
                get
                {
                    return base.Children;
                }
            }


            /// @if LANG_JA
            /// <summary>子ウィジェットを先頭に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は先頭に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the beginning.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the beginning if it has already been added.</remarks>
            /// @endif
            public new virtual void AddChildFirst(Widget child)
            {
                base.AddChildFirst(child);
            }


            /// @if LANG_JA
            /// <summary>子ウィジェットを末尾に追加する。</summary>
            /// <param name="child">追加する子ウィジェット</param>
            /// <remarks>既に追加されている場合は末尾に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds the child widget to the end.</summary>
            /// <param name="child">Child widget to be added</param>
            /// <remarks>Moves it to the end if it has already been added.</remarks>
            /// @endif
            public new virtual void AddChildLast(Widget child)
            {
                base.AddChildLast(child);
            }


            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直前に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="nextChild">挿入する子ウィジェットの直後となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直前に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately in front of the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="nextChild">Child widget immediately after the inserted child widget</param>
            /// <remarks>Moves it immediately in front of the specified child widget if it has already been added.</remarks>
            /// @endif
            public new virtual void InsertChildBefore(Widget child, Widget nextChild)
            {
                base.InsertChildBefore(child, nextChild);
            }


            /// @if LANG_JA
            /// <summary>指定した子ウィジェットの直後に挿入する。</summary>
            /// <param name="child">挿入する子ウィジェット</param>
            /// <param name="prevChild">挿入する子ウィジェットの直前となる子ウィジェット</param>
            /// <remarks>既に追加されている場合は指定した子ウィジェットの直後に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Inserts it immediately after the specified child widget.</summary>
            /// <param name="child">Child widget to be inserted</param>
            /// <param name="prevChild">Child widget immediately in front of the inserted child widget</param>
            /// <remarks>Moves it immediately after the specified child widget if it has already been added.</remarks>
            /// @endif
            public new virtual void InsertChildAfter(Widget child, Widget prevChild)
            {
                base.InsertChildAfter(child, prevChild);
            }


            /// @if LANG_JA
            /// <summary>指定された子ウィジェットを削除する。</summary>
            /// <param name="child">削除するウィジェット</param>
            /// <remarks>存在しないウィジェットを指定した場合は何も行われない。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified child widget.</summary>
            /// <param name="child">Widget to be deleted</param>
            /// <remarks>Nothing will be performed if a widget that does not exist is specified.</remarks>
            /// @endif
            public new virtual void RemoveChild(Widget child)
            {
                if (child != null)
                {
                    base.RemoveChild(child);
                }
            }

            /// @if LANG_JA
            /// <summary>子ウィジェットをAnchorsに従ってリサイズするかどうかの値を設定・取得する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets and obtains the value for whether to resize the child widget according to Anchors</summary>
            /// @endif
            public bool EnabledChildrenAnchors { get; set; }

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
                    if (EnabledChildrenAnchors)
                    {
                        updateWidth(base.Width, value);
                    }
                    base.Width = value;
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
                    if (EnabledChildrenAnchors)
                    {
                        updateHeight(base.Height, value);
                    }
                    base.Height = value;
                }
            }

            internal void updateWidth(float srcWidth, float dstWidth)
            {
                const float minimumSize = 1;
                if (srcWidth < minimumSize) return;
                if (dstWidth < minimumSize) dstWidth = minimumSize;

                float rate = dstWidth / srcWidth;

                for (LinkedTree<Widget> tree = this.LinkedTree.FirstChild; tree != null; tree = tree.NextSibling)
                {
                    Widget child = tree.Value;
     
                    switch ((int)child.Anchors & 0xf0)
                    {
                        case (int)Anchors.None:
                            float newCenterX = child.CenterX * rate;
                            child.Width *= rate;
                            child.CenterX = newCenterX;
                            break;
                        case (int)Anchors.Left:
                            child.Width *= Math.Max(dstWidth - child.X, minimumSize) /
                                Math.Max(srcWidth - child.X, minimumSize);
                            break;
                        case (int)Anchors.Right:
                            float newRight = child.X + child.Width + dstWidth - srcWidth;
                            float newX = child.X * Math.Max(newRight, minimumSize) /
                                Math.Max(child.X + child.Width, minimumSize);
                            child.Width = newRight - newX;
                            child.X = newRight - child.Width;
                            break;
                        case (int)Anchors.Width:
                            child.CenterX *= rate;
                            break;
                        case (int)(Anchors.Left | Anchors.Right):
                            child.Width = Math.Max(child.Width + dstWidth - srcWidth, minimumSize);
                            break;
                        case (int)(Anchors.Right | Anchors.Width):
                            child.X += dstWidth - srcWidth;
                            break;
                        default:
                            break;
                    }
                }
            }


            internal void updateHeight(float srcHeight, float dstHeight)
            {
                const float minimumSize = 1;
                if (srcHeight < minimumSize) return;
                if (dstHeight < minimumSize) dstHeight = minimumSize;

                float rate = dstHeight / srcHeight;

                for (LinkedTree<Widget> tree = this.LinkedTree.FirstChild; tree != null; tree = tree.NextSibling)
                {
                    Widget child = tree.Value;

                    switch ((int)child.Anchors & 0x0f)
                    {
                        case (int)Anchors.None:
                            float newCenterY = child.CenterY * rate;
                            child.Height *= rate;
                            child.CenterY = newCenterY;
                            break;
                        case (int)Anchors.Top:
                            child.Height *= Math.Max(dstHeight - child.Y, minimumSize) /
                                Math.Max(srcHeight - child.Y, minimumSize);
                            break;
                        case (int)Anchors.Bottom:
                            float newBottom = child.Y + child.Height + dstHeight - srcHeight;
                            float newY = child.Y * Math.Max(newBottom, minimumSize) /
                                Math.Max(child.Y + child.Height, minimumSize);
                            child.Height = newBottom - newY;
                            child.Y = newBottom - child.Height;
                            break;
                        case (int)Anchors.Height:
                            child.CenterY *= rate;
                            break;
                        case (int)(Anchors.Top | Anchors.Bottom):
                            child.Height = Math.Max(child.Height + dstHeight - srcHeight, minimumSize);
                            break;
                        case (int)(Anchors.Bottom | Anchors.Height):
                            child.Y += dstHeight - srcHeight;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
}
