/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System.ComponentModel;
using System.Collections.Generic;

using System;
using Sce.PlayStation.Core.Imaging;

namespace Sce.PlayStation.HighLevel.UI
{
    /// @if LANG_JA
    /// <summary>UIで使用するフォントクラス</summary>
    /// <remarks>システム内で Core.Imaging.Font クラスのオブジェクトを生成しキャッシュします。
    /// UISystem にピクセル密度が設定されている場合はフォントサイズもピクセル密度に応じてスケールされます。</remarks>
    /// @endif
    /// @if LANG_EN
    /// <summary>Font class used by UI</summary>
    /// <remarks>Generates and caches an object of Core.Imaging.Font class in the system.
    /// When the pixel density is set in UISystem, the font size is also scaled according to the pixel density.</remarks>
    /// @endif
    public class UIFont
    {
        /// @if LANG_JA
        /// <summary>UIFont のコンストラクタ (UIのデフォルト値から作成)</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>UIFont constructor (created from default UI value)</summary>
        /// @endif
        public UIFont()
        {
            // UI Default Font Setting
            this.fileName = null;
            this.aliasName =  TextRenderHelper.DefaultFontAlias;
            this.size = TextRenderHelper.DefaultFontSize;
            this.style = TextRenderHelper.DefaultFontStyle;
        }

        /// @if LANG_JA
        /// <summary>UIFont のコンストラクタ(ファイル名とサイズとスタイルから)</summary>
        /// <param name="filename">フォントのファイル名</param>
        /// <param name="size">サイズ</param>
        /// <param name="style">スタイル</param>
        /// <exception cref="ArgumentOutOfRangeException">size が 1～1024 の範囲外です。</exception>
        /// <remarks>引数 size は 1～1024 の範囲に収まっている必要があります。またフォントによって小さすぎるサイズで生成できない場合があります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>UIFont constructor (from filename, size, and style)</summary>
        /// <param name="filename">Font filename</param>
        /// <param name="size">Size</param>
        /// <param name="style">Style</param>
        /// <exception cref="ArgumentOutOfRangeException">size is outside the range of 1 to 1024.</exception>
        /// <remarks>The argument size must be between 1 and 1024. Also, depending on the font, the font may not be generated because its size is too small.</remarks>
        /// @endif
        public UIFont(string filename, int size, FontStyle style)
        {
            if (size < 1 || size > 1024)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            this.fileName = filename;
            this.aliasName = TextRenderHelper.DefaultFontAlias;
            this.size = size;
            this.style = style;
        }

        /// @if LANG_JA
        /// <summary>UIFont のコンストラクタ(別名とサイズとスタイルから)</summary>
        /// <param name="alias">フォントの別名</param>
        /// <param name="size">サイズ</param>
        /// <param name="style">スタイル</param>
        /// <exception cref="ArgumentOutOfRangeException">size が 1～1024 の範囲外です。</exception>
        /// <remarks>引数 size は 1～1024 の範囲に収まっている必要があります。またフォントによって小さすぎるサイズで生成できない場合があります。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>UIFont constructor (from separate name, size, and style)</summary>
        /// <param name="alias">Font separate name</param>
        /// <param name="size">Size</param>
        /// <param name="style">Style</param>
        /// <exception cref="ArgumentOutOfRangeException">size is outside the range of 1 to 1024.</exception>
        /// <remarks>The argument size must be between 1 and 1024. Also, depending on the font, the font may not be generated because its size is too small.</remarks>
        /// @endif
        public UIFont(FontAlias alias, int size, FontStyle style)
        {
            if (size < 1 || size > 1024)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            this.fileName = null;
            this.aliasName = alias;
            this.size = size;
            this.style = style;
       }


        /// @if LANG_JA
        /// <summary>フォントのファイル名</summary>
        /// <remarks>null が設定されている場合は AliasName の値が使用されます。
        /// フォントファイルが正しいかどうかの評価は初めて描画される時に行われます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Font filename</summary>
        /// <remarks>When null is set, the AliasName value is used.
        /// The evaluation of whether the font file is correct is performed when first rendered.</remarks>
        /// @endif
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    if (fileName == "") fileName = null;
                    NotifyPropertyChanged("FileName");
                }
            }
        }
        private string fileName;


        private FontAlias aliasName;

        /// @if LANG_JA
        /// <summary>フォントの別名</summary>
        /// <remarks>FileName が null の場合のみこの値が使用されます。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Font separate name</summary>
        /// <remarks>This value is used only when FileName is null.</remarks>
        /// @endif
        public FontAlias AliasName
        {
            get { return aliasName; }
            set
            {
                if (aliasName != value)
                {
                    aliasName = value;
                    NotifyPropertyChanged("AliasName");
                }
            }
        }

        private int size;

        /// @if LANG_JA
        /// <summary>フォントサイズ</summary>
        /// <remarks>引数 size は 1～1024 の範囲に収まっている必要があります。またフォントによって小さすぎるサイズで生成できない場合があります。</remarks>
        /// <exception cref="ArgumentOutOfRangeException">size が 1～1024 の範囲外です。</exception>
        /// @endif
        /// @if LANG_EN
        /// <summary>Font size</summary>
        /// <remarks>The argument size must be between 1 and 1024. Also, depending on the font, the font may not be generated because its size is too small.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">size is outside the range of 1 to 1024.</exception>
        /// @endif
        public int Size
        {
            get { return size; }
            set {
                if (value < 1 || value > 1024)
                {
                    throw new ArgumentOutOfRangeException("Size");
                }

                if (size != value)
                {
                    size = value;
                    NotifyPropertyChanged("Size");
                }
            }
        }

        private FontStyle style;

        /// @if LANG_JA
        /// <summary>フォントスタイル</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Font style</summary>
        /// @endif
        public FontStyle Style
        {
            get { return style; }
            set
            {
                if (style != value)
                {
                    style = value;
                    NotifyPropertyChanged("Style");
                }
            }
        }


        /// @if LANG_JA
        /// <summary>UIFont から Core.Imaging.Font クラスを取得します</summary>
        /// <remarks>キャッシュされている Core.Imaging.Font の ShallowClone を返します。</remarks>
        /// <returns>Font オブジェクト</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Obtains Core.Imaging.Font class from UIFont</summary>
        /// <remarks>Returns ShallowClone of cached Core.Imaging.Font.</remarks>
        /// <returns>Font object</returns>
        /// @endif
        public Font GetFont()
        {
            return UIFontManager.GetFontWithoutClone(this).ShallowClone() as Font;
        }

        /// @if LANG_JA
        /// <summary>キャッシュされている Core.Imaging.Font クラスのオブジェクトをすべて解放します。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Frees all cached objects of Core.Imaging.Font class.</summary>
        /// @endif
        public static void ClearCache()
        {
            UIFontManager.ClearCache();
        }

        /// @internal
        /// @if LANG_JA
        /// <summary></summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// @endif
        /// @if LANG_EN
        /// <summary></summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// @endif
        [Obsolete("use UIFont class or UIFont.GetFont()")]
        public static implicit operator Font(UIFont font)
        {
            return font.GetFont();
        }

        /// @internal
        /// @if LANG_JA
        /// <summary></summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// @endif
        /// @if LANG_EN
        /// <summary></summary>
        /// <param name="font"></param>
        /// <returns></returns>
        /// @endif
        [Obsolete("use UIFont class.")]
        public static implicit operator UIFont(Font font)
        {
            if (!String.IsNullOrEmpty(font.Name) && !font.Name.StartsWith("__"))
                return new UIFont(font.Name, (int)(font.Size / UISystem.PixelDensity + 0.5f), font.Style);
            else
                return new UIFont(FontAlias.System, (int)(font.Size / UISystem.PixelDensity + 0.5f), font.Style);
                //return new UIFont(FontAlias.System, font.Size, font.Style);
        }

        private void NotifyPropertyChanged(String info)
        {
            for (int i = WeakEventListeners.Count - 1; i >= 0; i--)
            {
                var listener = WeakEventListeners[i].Target as IWeakEventListener;
                if (listener == null)
                    WeakEventListeners.RemoveAt(i);
                else
                    listener.ReceiveWeakEvent(typeof(UIFont), this, new PropertyChangedEventArgs(info));
            }
        }

        internal void AddListener(IWeakEventListener listener)
        {
            WeakEventListeners.Add(new WeakReference(listener));
        }
        internal bool RemoveListener(IWeakEventListener listener)
        {
            bool result = false;
            for (int i = WeakEventListeners.Count-1; i >= 0; i--)
            {
                var ev = WeakEventListeners[i].Target;
                if (ev == null)
                {
                    WeakEventListeners.RemoveAt(i);
                }
                else if (ev == listener)
                {
                    result = true;
                    WeakEventListeners.RemoveAt(i);
                }
            }
            return result;
        }
        List<WeakReference> WeakEventListeners = new List<WeakReference>();

        /// @if LANG_JA
        /// <summary>ハッシュコードを返す </summary>
        /// <returns>ハッシュコード</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Returns a hash code. </summary>
        /// <returns>Hash code</returns>
        /// @endif
        public override int GetHashCode()
        {
            return (this.fileName != null ? this.fileName.GetHashCode() : (int)this.aliasName) ^
                ((int)this.style << 16) ^ (this.size << 20);
        }

        /// @if LANG_JA
        /// <summary>対象と自分自身が等価かどうか</summary>
        /// <param name="obj">比較対象</param>
        /// <returns>比較結果</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Whether the target and self are equivalent</summary>
        /// <param name="obj">Comparison target</param>
        /// <returns>Comparison result</returns>
        /// @endif
        public override bool Equals(object obj)
        {
            return this.Equals(obj as UIFont);
        }

        /// @if LANG_JA
        /// <summary>対象と自分自身が等価かどうか</summary>
        /// <param name="uifont">比較対象</param>
        /// <returns>比較結果</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Whether the target and self are equivalent</summary>
        /// <param name="uifont">Comparison target</param>
        /// <returns>Comparison result</returns>
        /// @endif
        public bool Equals(UIFont uifont)
        {
            if (uifont == null)
                return false;
            return this.size == uifont.size &&
                this.style == uifont.style &&
                this.fileName == uifont.fileName &&
                (this.fileName != null || this.aliasName == uifont.aliasName);
        }

        /// @if LANG_JA
        /// <summary>文字列を返す</summary>
        /// <returns>"フォント名, フォントスタイル, フォントサイズ" 形式の文字列</returns>
        /// @endif
        /// @if LANG_EN
        /// <summary>Returns a character string</summary>
        /// <returns>Character string of "font name, font style, font size" format</returns>
        /// @endif
        public override string ToString()
        {
            if (this.fileName != null)
                return String.Format("{0}, {1}, {2}", this.fileName, this.style, this.size);
            else
                return String.Format("{0}, {1}, {2}", this.aliasName, this.style, this.size);
        }
    }

    internal interface IWeakEventListener
    {
        bool ReceiveWeakEvent( Type managerType, Object sender, EventArgs e);
    }

    static internal class UIFontManager
    {
        // this class is thread safe, but text render or others are not guaranteed to be thread safe...
        static Dictionary<UIFont, Font> syncCache = new Dictionary<UIFont, Font>();

        internal static Font GetFontWithoutClone(UIFont uiFont)
        {
            Font font;
            lock (syncCache)
            {
                if (!syncCache.TryGetValue(uiFont, out font))
                {
                    // create font and cache
                    if (uiFont.FileName != null)
                    {
                        font = new Font(uiFont.FileName,
                            (int)(uiFont.Size * UISystem.PixelDensity + 0.5f), uiFont.Style);
                    }
                    else
                    {
                        font = new Font(uiFont.AliasName,
                            (int)(uiFont.Size * UISystem.PixelDensity + 0.5f), uiFont.Style);
                    }
                    syncCache.Add(uiFont, font);
                }
            }
            return font;
        }

        internal static void ClearCache()
        {
            lock (syncCache)
            {
                foreach (var font in syncCache.Values)
                {
                    font.Dispose();
                }
                syncCache.Clear();
            }
        }
    }
}

