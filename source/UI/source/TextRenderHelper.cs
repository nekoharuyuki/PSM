/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @internal
        /// @if LANG_JA
        /// <summary>画像の中に文字列の描画を行う。</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Renders a string within an image.</summary>
        /// @endif
        internal class TextRenderHelper
        {

            /// @if LANG_JA
            /// <summary>デフォルトのフォント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default font</summary>
            /// @endif
            public const FontAlias DefaultFontAlias = FontAlias.System;

            /// @if LANG_JA
            /// <summary>デフォルトのフォントサイズ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default font size</summary>
            /// @endif
            public const int DefaultFontSize = 25;

            /// @if LANG_JA
            /// <summary>デフォルトのフォントの種類</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default font type</summary>
            /// @endif
            public const FontStyle DefaultFontStyle = FontStyle.Regular;

            /// @if LANG_JA
            /// <summary>デフォルトの文字列の色</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default character string color</summary>
            /// @endif
            public static readonly UIColor DefaultTextColor = new UIColor(1f, 1f, 1f, 1f);

            /// @if LANG_JA
            /// <summary>デフォルトの文字列の背景色</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default character string background color</summary>
            /// @endif
            public static readonly UIColor DefaultBackgroundColor = new UIColor(0f, 0f, 0f, 0f);

            /// @if LANG_JA
            /// <summary>デフォルトの水平方向のアラインメント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default horizontal alignment</summary>
            /// @endif
            public const HorizontalAlignment DefaultHorizontalAlignment = HorizontalAlignment.Center;

            /// @if LANG_JA
            /// <summary>デフォルトの垂直方向のアラインメント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default vertical alignment</summary>
            /// @endif
            public const VerticalAlignment DefaultVerticalAlignment = VerticalAlignment.Middle;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TextRenderHelper()
            {
                this.Font = new UIFont();
                this.horizontalAlignment = DefaultHorizontalAlignment;
                this.verticalAlignment = DefaultVerticalAlignment;
                this.horizontalOffset = 0.0f;
                this.verticalOffset = 0.0f;
                this.lineBreak = LineBreak.Character;
                this.textTrimming = TextTrimming.None;
                this.lineGap = 0.0f;

                this.ReflectLineBreakFuncs = new Dictionary<LineBreak, ReflectLineBreak>()
                {
                    {LineBreak.Character,       ReflectLineBreakCharacter},
                    {LineBreak.Word,            ReflectLineBreakWord},
                    {LineBreak.Hyphenation,     ReflectLineBreakHyphenation},
                    {LineBreak.AtCode,          ReflectLineBreakAtCode}
                };

                this.ReflectTextTrimmingFuncs = new Dictionary<TextTrimming, ReflectTextTrimming>()
                {
                    {TextTrimming.None,                 ReflectTextTrimmingNone},
                    {TextTrimming.Character,            ReflectTextTrimmingCharacter},
                    {TextTrimming.Word,                 ReflectTextTrimmingWord},
                    {TextTrimming.EllipsisCharacter,    ReflectTextTrimmingEllipsisCharacter},
                    {TextTrimming.EllipsisWord,         ReflectTextTrimmingEllipsisWord},
                };
            }

            /// @if LANG_JA
            /// <summary>文字列のフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the font of the character string.</summary>
            /// @endif
            public UIFont Font
            {
                // TODO: handle UIFont member change event and update core font.
                get { return font; }
                set
                {
                    if (value == null)
                        font = new UIFont();
                    else
                        font = value;
                    coreFont = null;
                }
            }
            private UIFont font;

            private Font coreFont;

            private Font CoreFont
            {
                get
                {
                    if (coreFont == null)
                        coreFont = UIFontManager.GetFontWithoutClone(font);
                    return coreFont;
                }
            }
            
            /// @if LANG_JA
            /// <summary>水平方向のアラインメント取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the horizontal alignment.</summary>
            /// @endif
            public HorizontalAlignment HorizontalAlignment
            {
                get { return this.horizontalAlignment; }
                set { this.horizontalAlignment = value; }
            }
            private HorizontalAlignment horizontalAlignment;

            /// @if LANG_JA
            /// <summary>垂直方向のアラインメントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the vertical alignment.</summary>
            /// @endif
            public VerticalAlignment VerticalAlignment
            {
                get { return this.verticalAlignment; }
                set { this.verticalAlignment = value; }
            }
            private VerticalAlignment verticalAlignment;

            /// @if LANG_JA
            /// <summary>水平方向のオフセット位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the horizontal offset position.</summary>
            /// @endif
            public float HorizontalOffset
            {
                get { return this.horizontalOffset / UISystem.PixelDensity; }
                set { this.horizontalOffset = value * UISystem.PixelDensity; }
            }
            private float horizontalOffset;

            /// @if LANG_JA
            /// <summary>垂直方向のオフセット位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the vertical offset position.</summary>
            /// @endif
            public float VerticalOffset
            {
                get { return this.verticalOffset / UISystem.PixelDensity; }
                set { this.verticalOffset = value * UISystem.PixelDensity; }
            }
            private float verticalOffset;

            /// @if LANG_JA
            /// <summary>文字列の改行方法を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the break method of the character string.</summary>
            /// @endif
            public LineBreak LineBreak
            {
                get { return this.lineBreak; }
                set { this.lineBreak = value; }
            }
            private LineBreak lineBreak;

            /// @if LANG_JA
            /// <summary>文字列のトリミング方法を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the cropping method of the character string.</summary>
            /// @endif
            public TextTrimming TextTrimming
            {
                get { return this.textTrimming; }
                set { this.textTrimming = value; }
            }
            private TextTrimming textTrimming;

            /// @if LANG_JA
            /// <summary>行間を取得・設定する。</summary>
            /// <remarks>初期値は０</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the row spacing.</summary>
            /// <remarks>Default value is 0</remarks>
            /// @endif
            public float LineGap
            {
                get { return this.lineGap / UISystem.PixelDensity; }
                set { this.lineGap = value * UISystem.PixelDensity; }
            }
            private float lineGap;

            private float LineHeight
            {
                get
                {
                    return this.CoreFont.Metrics.Ascent + this.CoreFont.Metrics.Descent + this.CoreFont.Metrics.Leading;
                }
            }

            private float GetMultiLineHeight(int lineCount)
            {
                if (lineCount == 0)
                {
                    return 0.0f;
                }

                float height = (this.LineHeight * lineCount - this.CoreFont.Metrics.Leading) + (this.lineGap * (lineCount - 1));
                height += this.verticalOffset;
                height += textShadowMarginBottom;
                return height;
            }

            private int GetMaxLineCount(float height)
            {
                int count = (int)((height - this.verticalOffset - textShadowMarginBottom) / (this.LineHeight + this.lineGap));
                int odd = (int)((height - this.verticalOffset - textShadowMarginBottom) % (this.LineHeight + this.lineGap));

                if (count == 0 || odd > this.LineHeight + textShadowMarginBottom)
                {
                    count++;
                }

                return count;
            }

            private float GetLineWidth(CharMetrics[] metrics, int startIndex, int length)
            {
                if (length == 0)
                {
                    return 0.0f;
                }

                int start = startIndex;
                int end = startIndex + length - 1;

                float width = metrics[end].X + metrics[end].HorizontalAdvance - metrics[start].X;
                width += this.horizontalOffset;
                width += textShadowMarginLeft;
                return width;
            }

            private float GetLineWidth(string text)
            {
                float width = this.CoreFont.GetTextWidth(text);
                width += this.horizontalOffset;
                width += textShadowMarginLeft;
                return width;
            }

            /// @if LANG_JA
            /// <summary>文字列を描画するために必要な画像のサイズを取得する。</summary>
            /// <param name="text">文字列</param>
            /// <returns>文字列を描画するために必要な画像のサイズ</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the size of the image required for rendering a string.</summary>
            /// <param name="text">Character string</param>
            /// <returns>Size of the image required for rendering a string</returns>
            /// @endif
            public ImageSize GetImageSize(string text)
            {
                if (!String.IsNullOrEmpty(text))
                {
                    ImageSize size = new ImageSize(0, 0);

                    List<string> lines = SplitLineBreakeCode(new StringBuilder(text));
                    foreach (string line in lines)
                    {
                        float lineWidth = GetLineWidth(line);
                        if (lineWidth > size.Width)
                        {
                            size.Width = (int)(lineWidth / UISystem.PixelDensity);
                        }
                    }
                    size.Height = (int)(GetMultiLineHeight(lines.Count) / UISystem.PixelDensity);

                    return size;
                }
                else
                {
                    return new ImageSize(0, 0);
                }
            }

            /// @if LANG_JA
            /// <summary>文字列を指定した画像の幅で描画するときに必要な画像の高さを取得する。</summary>
            /// <param name="text">文字列</param>
            /// <param name="width">画像の幅</param>
            /// <returns>文字列を描画するために必要な画像の高さ</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the height of the image required for rendering a string at a specified image width.</summary>
            /// <param name="text">Character string</param>
            /// <param name="width">Image width</param>
            /// <returns>Height of the image required for rendering a string</returns>
            /// @endif
            public float GetTotalHeight(string text, float width)
            {
                if (!String.IsNullOrEmpty(text) && (width > 0.0f))
                {
                    width *= UISystem.PixelDensity;

                    List<string> lines = new List<string>();

                    List<string> tmpLines = SplitLineBreakeCode(new StringBuilder(text));
                    foreach (string line in tmpLines)
                    {
                        CharMetrics[] metrics = this.CoreFont.GetTextMetrics(line);
                        float lineWidth = GetLineWidth(metrics, 0, metrics.Length);

                        List<string> addLines = new List<string>();
                        if (lineWidth > width)
                        {
                            addLines = ReflectLineBreakFuncs[this.LineBreak](new StringBuilder(line), metrics, width, int.MaxValue);
                        }
                        else
                        {
                            addLines.Add(line);
                        }

                        foreach (string addLine in addLines)
                        {
                            lines.Add(addLine);
                        }
                    }

                    return GetMultiLineHeight(lines.Count) / UISystem.PixelDensity;
                }
                else
                {
                    return 0;
                }
            }

            /// @if LANG_JA
            /// <summary>画像の中に文字列を描画する。</summary>
            /// <param name="text">文字列</param>
            /// <param name="width">画像の幅</param>
            /// <param name="height">画像の高さ</param>
            /// <returns>文字列を描画した画像</returns>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a string within an image.</summary>
            /// <param name="text">Character string</param>
            /// <param name="width">Image width</param>
            /// <param name="height">Image height</param>
            /// <returns>Image with rendered string</returns>
            /// @endif
            public ImageAsset DrawText(ref string text, int width, int height)
            {
                ImageColor backgroundColor = new ImageColor(
                    (int)(DefaultBackgroundColor.R * 255.0f),
                    (int)(DefaultBackgroundColor.G * 255.0f),
                    (int)(DefaultBackgroundColor.B * 255.0f),
                    (int)(DefaultBackgroundColor.A * 255.0f));

                ImageColor textColor = new ImageColor(
                    (int)(DefaultTextColor.R * 255.0f),
                    (int)(DefaultTextColor.G * 255.0f),
                    (int)(DefaultTextColor.B * 255.0f),
                    (int)(DefaultTextColor.A * 255.0f));


                bool IsEmpty = String.IsNullOrEmpty(text);

                if(UISystem.IsScaledPixelDensity)
                {
                    width = (int)(width * UISystem.PixelDensity + 0.5f);
                    height = (int)(height * UISystem.PixelDensity + 0.5f);
                }

                if (width <= 0)
                {
                    width = 1;
                    IsEmpty = true;
                }
                if (height <= 0)
                {
                    height = 1;
                    IsEmpty = true;
                }
                int maxsize = UISystem.GraphicsContext.Caps.MaxTextureSize;
                if(width > maxsize)
                    width = maxsize;
                if(height > maxsize)
                    height = maxsize;

                Image image = new Image(ImageMode.A, new ImageSize(width, height), backgroundColor);
                ImageAsset asset = null;

                if (IsEmpty)
                {
                    asset = new ImageAsset(image);
                    image.Dispose();
                    return asset;
                }

                List<string> lines = SplitText(new StringBuilder(text), width, height);

                // vertical position
                float textHeight = GetMultiLineHeight(lines.Count) - this.verticalOffset;
                float posY = 0.0f;
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        posY = 0.0f;
                        break;
                    case VerticalAlignment.Middle:
                        posY = (height - textHeight + this.CoreFont.Metrics.Descent) / 2.0f - 0.001f;
                        break;
                    case VerticalAlignment.Bottom:
                        posY = height - textHeight;
                        break;
                }

                foreach (string line in lines)
                {
                    // horizontal position
                    float lineWidth = GetLineWidth(line) - this.horizontalOffset;
                    float posX = 0.0f;
                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            posX = 0.0f;
                            break;
                        case HorizontalAlignment.Center:
                            posX = (width - lineWidth) / 2.0f;
                            break;
                        case HorizontalAlignment.Right:
                            posX = width - lineWidth;
                            break;
                    }

                    // draw
                    image.DrawText(line, textColor, this.CoreFont, new ImagePosition((int)(posX + this.horizontalOffset), (int)(posY + this.verticalOffset)));

                    posY += (this.LineHeight + this.lineGap);
                }

                asset = new ImageAsset(image, UISystem.IsScaledPixelDensity);

                image.Dispose();

                return asset;
            }

            private List<string> SplitText(StringBuilder text, float width, float height)
            {
                List<string> lines = new List<string>();
                List<string> tmpLines = SplitLineBreakeCode(text);
                int maxLineCount = GetMaxLineCount(height);

                if (this.LineBreak == LineBreak.AtCode)
                {
                    for (int i = 0; i < tmpLines.Count && i < maxLineCount; i++)
                    {
                        // TextTrimming
                        var metrics = this.CoreFont.GetTextMetrics(tmpLines[i]);
                        if (GetLineWidth(metrics, 0, metrics.Length) > width)
                        {
                            lines.Add(ReflectTextTrimmingFuncs[this.TextTrimming](new StringBuilder(tmpLines[i]), metrics, width));
                        }
                        else
                        {
                            lines.Add(tmpLines[i]);
                        }
                    }
                }
                else
                {
                    foreach (string line in tmpLines)
                    {
                        // LineBreak (without linefeed code)
                        CharMetrics[] metrics = this.CoreFont.GetTextMetrics(line);
                        float lineWidth = GetLineWidth(metrics, 0, metrics.Length);
                        if (lineWidth > width)
                        {
                            lines.AddRange(ReflectLineBreakFuncs[this.LineBreak](new StringBuilder(line), metrics, width, maxLineCount - lines.Count));
                        }
                        else
                        {
                            lines.Add(line);
                        }

                        UIDebug.Assert(lines.Count <= maxLineCount);

                        // TextTrimming
                        if (lines.Count >= maxLineCount)
                        {
                            string lastLine = lines[lines.Count - 1];
                            var lastLineMetrics = this.CoreFont.GetTextMetrics(lastLine);
                            if (GetLineWidth(lastLineMetrics, 0, lastLineMetrics.Length) > width)
                            {
                                lines[lines.Count - 1] = ReflectTextTrimmingFuncs[this.TextTrimming](new StringBuilder(lastLine), lastLineMetrics, width);
                            }

                            break;// foreach (string line in tmpLines)
                        }
                    }
                }

                return lines;
            }

            private List<string> SplitLineBreakeCode(StringBuilder text)
            {
                List<string> lines = new List<string>();
                int length = 0;
                int leftIndex = 0;
                bool isPrevCR = false;

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].Equals('\r'))
                    {
                        lines.Add(text.ToString(leftIndex, length));
                        length = 0;
                        leftIndex = i + 1;
                        isPrevCR = true;
                    }
                    else if (text[i].Equals('\n'))
                    {
                        if (!isPrevCR)
                        {
                            lines.Add(text.ToString(leftIndex, length));
                        }
                        length = 0;
                        leftIndex = i + 1;
                        isPrevCR = false;
                    }
                    else
                    {
                        length++;
                        isPrevCR = false;
                    }
                }

                if (length != 0)
                {
                    lines.Add(text.ToString(leftIndex, length));
                }

                return lines;
            }


            // LineBreak
            private delegate List<string> ReflectLineBreak(StringBuilder text, CharMetrics[] metrics, float width, int maxLineCount);
            private Dictionary<LineBreak, ReflectLineBreak> ReflectLineBreakFuncs;

            private List<string> ReflectLineBreakCharacter(StringBuilder text, CharMetrics[] metrics, float width, int maxLineCount)
            {
                List<string> lines = new List<string>();
                int length = 0;
                int leftIndex = 0;

                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, leftIndex, length + 1);
                    if (lineWidth > width && length != 0)
                    {
                        if (lines.Count + 1 >= maxLineCount)
                        {
                            lines.Add(text.ToString(leftIndex, text.Length - leftIndex));
                            return lines;
                        }
                        else if (length == 1)
                        {
                            lines.Add(text.ToString(leftIndex, length));
                            length = 0;
                        }
                        else
                        {
                            if (this.delimiters.Contains(text[i]))
                            {
                                lines.Add(text.ToString(leftIndex, length - 1));
                                length = 1;
                            }
                            else
                            {
                                lines.Add(text.ToString(leftIndex, length));
                                length = 0;
                            }
                        }
                        leftIndex = i - length;
                        i--;
                        continue;
                    }
                    length++;
                }

                if (length != 0)
                {
                    lines.Add(text.ToString(leftIndex, length));
                }

                return lines;
            }

            private List<string> ReflectLineBreakWord(StringBuilder text, CharMetrics[] metrics, float width, int maxLineCount)
            {
                List<string> lines = new List<string>();
                int length = 0;
                int worldLength = 0;
                int leftIndex = 0;

                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, leftIndex, length + 1);
                    if (lineWidth > width && length != 0)
                    {
                        if (lines.Count + 1 >= maxLineCount)
                        {
                            lines.Add(text.ToString(leftIndex, text.Length - leftIndex));
                            return lines;
                        }
                        else if (worldLength != 0)
                        {
                            lines.Add(text.ToString(leftIndex, worldLength));
                            length -= worldLength;
                            worldLength = 0;
                        }
                        else
                        {
                            if (length == 1)
                            {
                                lines.Add(text.ToString(leftIndex, length));
                                length = 0;
                            }
                            else
                            {
                                if (this.delimiters.Contains(text[i]))
                                {
                                    lines.Add(text.ToString(leftIndex, length - 1));
                                    length = 1;
                                }
                                else
                                {
                                    lines.Add(text.ToString(leftIndex, length));
                                    length = 0;
                                }
                            }
                        }
                        leftIndex = i - length;
                        i--;
                        continue;
                    }
                    length++;

                    if (this.wordSeparator.Equals(text[i]))
                    {
                        worldLength = length;
                    }
                }

                if (length != 0)
                {
                    lines.Add(text.ToString(leftIndex, length));
                }

                return lines;
            }

            private List<string> ReflectLineBreakHyphenation(StringBuilder text, CharMetrics[] metrics, float width, int maxLineCount)
            {
                List<string> lines = new List<string>();
                int length = 0;
                int leftIndex = 0;
                char preChar = ' ';

                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, leftIndex, length + 1) + this.CoreFont.GetTextWidth(this.hyphen.ToString());
                    if (lineWidth > width && length != 0)
                    {
                        if (lines.Count + 1 >= maxLineCount)
                        {
                            lines.Add(text.ToString(leftIndex, text.Length - leftIndex));
                            return lines;
                        }
                        else if (length == 1)
                        {
                            if (preChar.Equals(this.wordSeparator) || text[i].Equals(this.wordSeparator))
                            {
                                lines.Add(text.ToString(leftIndex, length));
                            }
                            else
                            {
                                lines.Add(text.ToString(leftIndex, length) + this.hyphen);
                            }
                            length = 0;
                        }
                        else
                        {
                            if (this.delimiters.Contains(text[i]))
                            {
                                if (preChar.Equals(this.wordSeparator) || text[i].Equals(this.wordSeparator))
                                {
                                    lines.Add(text.ToString(leftIndex, length - 1));
                                }
                                else
                                {
                                    lines.Add(text.ToString(leftIndex, length - 1) + this.hyphen);
                                }
                                length = 1;
                            }
                            else
                            {
                                if (preChar.Equals(this.wordSeparator) || text[i].Equals(this.wordSeparator))
                                {
                                    lines.Add(text.ToString(leftIndex, length));
                                }
                                else
                                {
                                    lines.Add(text.ToString(leftIndex, length) + this.hyphen);
                                }
                                length = 0;
                            }
                        }
                        leftIndex = i - length;
                        i--;
                        continue;
                    }
                    preChar = text[i];
                    length++;
                }

                if (length != 0)
                {
                    lines.Add(text.ToString(leftIndex, length));
                }

                return lines;
            }

            private List<string> ReflectLineBreakAtCode(StringBuilder text, CharMetrics[] metrics, float width, int maxLineCount)
            {
                List<string> lines = new List<string>();
                lines.Add(text.ToString());
                return lines;
            }


            // TextTrimming
            private delegate string ReflectTextTrimming(StringBuilder text, CharMetrics[] metrics, float width);
            private Dictionary<TextTrimming, ReflectTextTrimming> ReflectTextTrimmingFuncs;

            private string ReflectTextTrimmingNone(StringBuilder text, CharMetrics[] metrics, float width)
            {
                return text.ToString();
            }

            private string ReflectTextTrimmingCharacter(StringBuilder text, CharMetrics[] metrics, float width)
            {
                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, 0, i + 1);
                    if (lineWidth > width && i != 0)
                    {
                        return text.ToString(0, i);
                    }
                }

                return text.ToString();
            }

            private string ReflectTextTrimmingWord(StringBuilder text, CharMetrics[] metrics, float width)
            {
                int wordLength = 0;

                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, 0, i + 1);
                    if (lineWidth > width && i != 0)
                    {
                        if (wordLength != 0)
                        {
                            return text.ToString(0, wordLength);
                        }
                        else
                        {
                            return text.ToString(0, i);
                        }
                    }

                    if (text[i].Equals(this.wordSeparator))
                    {
                        wordLength = i + 1;
                    }
                }

                return text.ToString();
            }

            private string ReflectTextTrimmingEllipsisCharacter(StringBuilder text, CharMetrics[] metrics, float width)
            {
                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, 0, i + 1) + this.CoreFont.GetTextWidth(this.elipsis);
                    if (lineWidth > width)
                    {
                        return text.ToString(0, i) + this.elipsis;
                    }
                }

                return text.ToString() + this.elipsis;
            }

            private string ReflectTextTrimmingEllipsisWord(StringBuilder text, CharMetrics[] metrics, float width)
            {
                int wordLength = 0;

                for (int i = 0; i < metrics.Length; i++)
                {
                    float lineWidth = GetLineWidth(metrics, 0, i + 1) + this.CoreFont.GetTextWidth(this.elipsis);
                    if (lineWidth > width)
                    {
                        if (wordLength != 0)
                        {
                            return text.ToString(0, wordLength) + this.elipsis;
                        }
                        else
                        {
                            return text.ToString(0, i) + this.elipsis;
                        }
                    }

                    if (text[i].Equals(this.wordSeparator))
                    {
                        wordLength = i + 1;
                    }
                }

                return text.ToString() + this.elipsis;
            }


            private char wordSeparator = ' ';
            private char hyphen = '-';
            private string elipsis = "...";
            private List<char> delimiters = new List<char> { ',', '.', '。', '、' };

            private const float textShadowMarginLeft = 2.0f;
            private const float textShadowMarginBottom = 2.0f;

        }


    }
}
