/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;

using Sce.PlayStation.Core;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {

        /// @if LANG_JA
        /// <summary>複数のタッチイベントをまとめて保持するクラス</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Class that holds multiple touch events at the same time</summary>
        /// @endif
        public class TouchEventCollection : CollectionBase
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TouchEventCollection()
            {
                Forward = false;
                PrimaryTouchEvent = null;
            }

            /// @if LANG_JA
            /// <summary>インデクサー</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Indexer</summary>
            /// @endif
            public TouchEvent this[int index]
            {
                get
                {
                    return (TouchEvent)List[index];
                }
                set
                {
                    List[index] = value;
                }
            }

            /// @if LANG_JA
            /// <summary>指定した指のIDのタッチイベントを取得する。</summary>
            /// <param name="id">指のID</param>
            /// <returns>タッチイベント</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the touch event of the specified finger ID.</summary>
            /// <param name="id">Finger ID</param>
            /// <returns>Touch event</returns>
            /// @endif
            public TouchEvent GetTouchEventByID(int id)
            {
                foreach (TouchEvent e in List)
                {
                    if (e.FingerID == id)
                    {
                        return e;
                    }
                }
                return null;
            }

            /// @if LANG_JA
            /// <summary>タッチイベントを追加する。</summary>
            /// <param name="touchEvent">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds a touch event.</summary>
            /// <param name="touchEvent">Touch event</param>
            /// @endif
            public int Add(TouchEvent touchEvent)
            {
                return List.Add(touchEvent);
            }

            /// @if LANG_JA
            /// <summary>指定したインデックス位置にタッチイベントを追加する。</summary>
            /// <param name="index">インデックス</param>
            /// <param name="touchEvent">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Adds a touch event to the specified index position.</summary>
            /// <param name="index">Index</param>
            /// <param name="touchEvent">Touch event</param>
            /// @endif
            public void Insert(int index, TouchEvent touchEvent)
            {
                List.Insert(index, touchEvent);
            }

            /// @if LANG_JA
            /// <summary>指定したタッチイベントのインデックスを取得する。</summary>
            /// <param name="touchEvent">タッチイベント</param>
            /// <returns>インデックス</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the index of a specified touch event.</summary>
            /// <param name="touchEvent">Touch event</param>
            /// <returns>Index</returns>
            /// @endif
            public int IndexOf(TouchEvent touchEvent)
            {
                return List.IndexOf(touchEvent);
            }

            /// @if LANG_JA
            /// <summary>指定したタッチイベントを削除する。</summary>
            /// <param name="touchEvent">タッチイベント</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Deletes the specified touch event.</summary>
            /// <param name="touchEvent">Touch event</param>
            /// @endif
            public void Remove(TouchEvent touchEvent)
            {
                List.Remove(touchEvent);
            }

            /// @if LANG_JA
            /// <summary>指定したタッチイベントが含まれているかどうかを取得する。</summary>
            /// <param name="touchEvent">タッチイベント</param>
            /// <returns>含まれているかどうか</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains whether or not the specified touch event is included.</summary>
            /// <param name="touchEvent">Touch event</param>
            /// <returns>Whether included or not</returns>
            /// @endif
            public bool Contains(TouchEvent touchEvent)
            {
                return List.Contains(touchEvent);
            }

            /// @if LANG_JA
            /// <summary>ウィジェットに最初に触れた指のタッチイベントを取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the touch event of the widget that was first touched by a finger.</summary>
            /// @endif
            public TouchEvent PrimaryTouchEvent
            {
                get;
                internal set;
            }

            /// @if LANG_JA
            /// <summary>コレクションに含まれるタッチイベントをフォワードするかどうかを取得・設定する。初期値はFalse。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to forward a touch event included in a collection. Default value is False.</summary>
            /// @endif
            public bool Forward
            {
                get;
                set;
            }
        }
    }
}

