/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @internal
        /// @if LANG_JA
        /// <summary>リストとしてツリー全体を走査することが可能なツリー</summary>
        /// <remarks>内部でnewを一切行わない再帰処理に頼らず全要素の処理が可能</remarks>
        /// <typeparam name="T">要素の型</typeparam>
        /// @endif
        /// @if LANG_EN
        /// <summary>Tree that enables scanning of the entire tree as a list</summary>
        /// <remarks>Enables processing of all elements that do not rely on recursive processing that never performs new inside</remarks>
        /// <typeparam name="T">Element type</typeparam>
        /// @endif
        internal class LinkedTree<T>
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="value">関連づける値</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="value">Linked value</param>
            /// @endif
            public LinkedTree(T value)
            {
                UIDebug.Assert(value != null, "invalid argument");

                this.value = value;
                this.parent = null;
                this.lastChild = null;
                this.previous = this.next = null;
            }


            /// @if LANG_JA
            /// <summary>関連づけられた値を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the linked value.</summary>
            /// @endif
            public T Value
            {
                get
                {
                    UIDebug.Assert(this.value != null, "not initialized");
                    return this.value;
                }
            }


            /// @if LANG_JA
            /// <summary>親を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the parent.</summary>
            /// @endif
            public LinkedTree<T> Parent
            {
                get
                {
                    return this.parent;
                }
            }


            /// @if LANG_JA
            /// <summary>ツリー全体をリストと見なした時の直前の要素を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the element before the entire tree was treated as a list.</summary>
            /// @endif
            public LinkedTree<T> PreviousAsList
            {
                get
                {
                    return this.previous;
                }
            }


            /// @if LANG_JA
            /// <summary>ツリー全体をリストと見なした時の直後の要素を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the element after the entire tree was treated as a list.</summary>
            /// @endif
            public LinkedTree<T> NextAsList
            {
                get
                {
                    return this.next;
                }
            }


            /// @if LANG_JA
            /// <summary>直前の兄弟を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the previous brother.</summary>
            /// @endif
            public LinkedTree<T> PreviousSibling

            {
                get
                {
                    UIDebug.Assert(this.parent != null, "invalid call");

                    if (this.previous != this.parent)
                    {
                        LinkedTree<T> previous = this.previous;

                        while (previous.parent != this.parent)
                        {
                            previous = previous.parent;
                        }

                        return previous;
                    }
                    else
                    {
                        return null;
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>直後の兄弟を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the next brother.</summary>
            /// @endif
            public LinkedTree<T> NextSibling
            {
                get
                {
                    UIDebug.Assert(this.parent != null, "invalid call");

                    LinkedTree<T> desc = this.LastDescendant;
                    LinkedTree<T> next = desc.next;

                    return (next != null && next.parent == this.parent) ? next : null;
                }
            }


            /// @if LANG_JA
            /// <summary>先頭の子供を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the start child.</summary>
            /// @endif
            public LinkedTree<T> FirstChild
            {
                get
                {
                    return this.lastChild != null ? this.next : null;
                }
            }


            /// @if LANG_JA
            /// <summary>末尾の子供を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the end child.</summary>
            /// @endif
            public LinkedTree<T> LastChild
            {
                get
                {
                    return this.lastChild;
                }
            }


            /// @if LANG_JA
            /// <summary>自分の子孫の末尾の要素を取得する。</summary>
            /// <remarks>リスト処理の終端条件として使用する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the end element of one's own descendant.</summary>
            /// <remarks>This is used as the termination condition of list processing.</remarks>
            /// @endif
            public LinkedTree<T> LastDescendant
            {
                get
                {
                    LinkedTree<T> desc = this;

                    while (desc.lastChild != null)
                    {
                        desc = desc.lastChild;
                    }

                    return desc;
                }
            }


            /// @if LANG_JA
            /// <summary>子要素を先頭に追加する。</summary>
            /// <param name="child">追加する要素</param>
            /// <remarks>既に追加されている場合は先頭に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds a child element to the beginning.</summary>
            /// <param name="child">Element to be added</param>
            /// <remarks>Moves it to the beginning if it has already been added.</remarks>
            /// @endif
            public void AddChildFirst(LinkedTree<T> child)
            {
                assertCanAddChild(child, this);

                child.RemoveChild();

                LinkedTree<T> childDesc = child.LastDescendant;

                child.parent = this;
                child.previous = this;
                childDesc.next = this.next;

                child.previous.next = child;

                if (childDesc.next != null)
                {
                    childDesc.next.previous = childDesc;
                }

                if (this.lastChild == null)
                {
                    this.lastChild = child;
                }
            }

            [System.Diagnostics.Conditional("DEBUG")]
            private static void assertCanAddAsSibling(LinkedTree<T> target, LinkedTree<T> sibling)
            {
                UIDebug.Assert(sibling != null, "invalid argument");
                UIDebug.Assert(sibling != target, "invalid argument");
                assertCanAddChild(sibling.parent, target);
            }

            [System.Diagnostics.Conditional("DEBUG")]
            private static void assertCanAddChild(LinkedTree<T> target, LinkedTree<T> parent)
            {
                UIDebug.Assert(parent != null, "invalid argument");
                UIDebug.Assert(target != null, "invalid argument");
                UIDebug.Assert(target != parent, "invalid argument");

                for (LinkedTree<T> node = parent.parent; node != null; node = node.parent)
                {
                    UIDebug.Assert(node != target, "invalid argument");
                }
            }

            /// @if LANG_JA
            /// <summary>子要素を末尾に追加する。</summary>
            /// <param name="child">追加する要素</param>
            /// <remarks>既に追加されている場合は末尾に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds a child element to the end.</summary>
            /// <param name="child">Element to be added</param>
            /// <remarks>Moves it to the end if it has already been added.</remarks>
            /// @endif
            public void AddChildLast(LinkedTree<T> child)
            {
                assertCanAddChild(child, this);

                child.RemoveChild();

                LinkedTree<T> thisDesc = LastDescendant;
                LinkedTree<T> childDesc = child.LastDescendant;

                child.parent = this;
                child.previous = thisDesc;
                childDesc.next = thisDesc.next;

                child.previous.next = child;

                if (childDesc.next != null)
                {
                    childDesc.next.previous = childDesc;
                }

                this.lastChild = child;
            }


            /// @if LANG_JA
            /// <summary>自身を指定した子要素の直前の要素として追加する。</summary>
            /// <param name="tree">直後となる子要素</param>
            /// <remarks>既に追加されている場合は指定した子要素の直前に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds itself as an element immediately before the specified child element.</summary>
            /// <param name="tree">Child element to come immediately after</param>
            /// <remarks>Moves it immediately in front of the specified child element if it has already been added.</remarks>
            /// @endif
            public void InsertChildBefore(LinkedTree<T> tree)
            {
                assertCanAddAsSibling(this, tree);

                RemoveChild();

                LinkedTree<T> desc = this.LastDescendant;

                this.parent = tree.parent;
                this.previous = tree.previous;
                desc.next = tree;

                this.previous.next = this;
                desc.next.previous = desc;
            }


            /// @if LANG_JA
            /// <summary>自身を指定した子要素の直後の要素として追加する。</summary>
            /// <param name="tree">直前となる子要素</param>
            /// <remarks>既に追加されている場合は指定した子要素の直後に移動する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds itself as an element immediately after the specified child element.</summary>
            /// <param name="tree">Child element to come immediately before</param>
            /// <remarks>Moves it immediately after the specified child element if it has already been added.</remarks>
            /// @endif
            public void InsertChildAfter(LinkedTree<T> tree)
            {
                assertCanAddAsSibling(this, tree);

                RemoveChild();

                LinkedTree<T> treeDesc = tree.LastDescendant;
                LinkedTree<T> thisDesc = this.LastDescendant;

                this.parent = tree.parent;
                this.previous = treeDesc;
                thisDesc.next = treeDesc.next;

                this.previous.next = this;

                if (thisDesc.next != null)
                {
                    thisDesc.next.previous = thisDesc;
                }

                if (this.parent.lastChild == tree)
                {
                    this.parent.lastChild = this;
                }
            }


            /// @if LANG_JA
            /// <summary>自身を親から削除する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes itself from the parent.</summary>
            /// @endif
            public void RemoveChild()
            {
                if (this.parent != null)
                {
                    LinkedTree<T> desc = this.LastDescendant;

                    if (this.parent.lastChild == this)
                    {
                        this.parent.lastChild = PreviousSibling;
                    }

                    this.previous.next = desc.next;

                    if (desc.next != null)
                    {
                        desc.next.previous = this.previous;
                    }

                    this.parent = null;
                    this.previous = desc.next = null;
                }
            }


            /// @if LANG_JA
            /// <summary>すべての子要素を削除する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes all child elements.</summary>
            /// @endif
            public void Clear()
            {
                while (this.lastChild != null)
                {
                    this.FirstChild.RemoveChild();
                }
            }


            private T value;
            private LinkedTree<T> parent;
            private LinkedTree<T> lastChild;
            private LinkedTree<T> previous;
            private LinkedTree<T> next;

        }


    }
}
