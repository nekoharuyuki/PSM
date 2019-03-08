/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>水平方向のアラインメント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Horizontal alignment</summary>
        /// @endif
        public enum HorizontalAlignment
        {
            /// @if LANG_JA
            /// <summary>左揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Align left</summary>
            /// @endif
            Left = 0,

            /// @if LANG_JA
            /// <summary>中央揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Center</summary>
            /// @endif
            Center,

            /// @if LANG_JA
            /// <summary>右揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Align right</summary>
            /// @endif
            Right
        }


        /// @if LANG_JA
        /// <summary>垂直方向のアラインメント</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Vertical alignment</summary>
        /// @endif
        public enum VerticalAlignment
        {
            /// @if LANG_JA
            /// <summary>上揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Align top</summary>
            /// @endif
            Top = 0,

            /// @if LANG_JA
            /// <summary>中央揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Center</summary>
            /// @endif
            Middle,

            /// @if LANG_JA
            /// <summary>下揃え</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Align bottom</summary>
            /// @endif
            Bottom
        }


        /// @if LANG_JA
        /// <summary>文字列のトリミング方法</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Cropping method of character string</summary>
        /// @endif
        public enum TextTrimming
        {
            /// @if LANG_JA
            /// <summary>トリミングなし（文字の途中で切る）</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>No cropping (cut text in the middle)</summary>
            /// @endif
            None = 0,

            /// @if LANG_JA
            /// <summary>文字単位でトリミング</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Crop by individual characters</summary>
            /// @endif
            Character,

            /// @if LANG_JA
            /// <summary>単語単位でトリミング</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Crop by individual words</summary>
            /// @endif
            Word,

            /// @if LANG_JA
            /// <summary>文字単位でトリミングして省略記号（...）を挿入</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Crop by individual characters and insert ellipsis (...)</summary>
            /// @endif
            EllipsisCharacter,

            /// @if LANG_JA
            /// <summary>単語単位でトリミングして省略記号（...）を挿入</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Crop by individual words and insert ellipsis (...)</summary>
            /// @endif
            EllipsisWord
        }


        /// @if LANG_JA
        /// <summary>文字列の改行方法</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Break method of character string</summary>
        /// @endif
        public enum LineBreak
        {
            /// @if LANG_JA
            /// <summary>文字単位</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>By individual characters</summary>
            /// @endif
            Character = 0,

            /// @if LANG_JA
            /// <summary>単語単位</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>By individual words</summary>
            /// @endif
            Word,

            /// @if LANG_JA
            /// <summary>文字単位でハイフンを入れる。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Break by individual characters and insert a hyphen.</summary>
            /// @endif
            Hyphenation,

            /// @if LANG_JA
            /// <summary>改行文字で改行する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Break with a line feed character.</summary>
            /// @endif
            AtCode
        }



        /// @if LANG_JA
        /// <summary>文字列を表示するウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>A widget that displays a character string</summary>
        /// @endif
        public class Label : Widget, IWeakEventListener
        {

            private const float defaultLabelWidth = 214.0f;
            private const float defaultLabelHeight = 27.0f;

            [Flags]
            private enum UpdateFlags
            {
                Background = 0x01,
                Text = 0x02
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Label()
            {
                this.backgroundSprt = new UISprite(1);
                this.RootUIElement.AddChildLast(this.backgroundSprt);
                this.backgroundSprt.ShaderType = ShaderType.SolidFill;
                this.BackgroundColor = TextRenderHelper.DefaultBackgroundColor;

                this.textSprt = new UISprite(1);
                this.RootUIElement.AddChildLast(textSprt);
                this.textSprt.ShaderType = ShaderType.TextTexture;

                this.Text = "";
                this.Font = new UIFont();
                this.TextColor = TextRenderHelper.DefaultTextColor;

                this.TextShadow = null;

                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Middle;

                this.LineBreak = LineBreak.Character;
                this.TextTrimming = TextTrimming.EllipsisCharacter;
                this.LineGap = 0.0f;

                this.Width = defaultLabelWidth;
                this.Height = defaultLabelHeight;
                this.Focusable = false;

                this.updateFlags = UpdateFlags.Background | UpdateFlags.Text;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if(this.textSprt != null && this.textSprt.Image != null)
                {
                    this.textSprt.Image.Dispose();
                    this.textSprt.Image = null;
                }
                this.Font = null;
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
                    if (base.Width != value)
                    {
                        base.Width = value;
                        this.updateFlags = UpdateFlags.Background | UpdateFlags.Text;
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
                    if (base.Height != value)
                    {
                        base.Height = value;
                        this.updateFlags = UpdateFlags.Background | UpdateFlags.Text;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>文字列を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the character string.</summary>
            /// @endif
            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    if (this.text != value)
                    {
                        this.text = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private string text;

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
                    return this.font;
                }
                set
                {
                    if (this.font != value)
                    {
                        if (this.font != null)
                            this.font.RemoveListener(this);

                        this.font = value;

                        if (this.font != null)
                            this.font.AddListener(this);

                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private UIFont font;

            bool IWeakEventListener.ReceiveWeakEvent (Type managerType, object sender, EventArgs e)
            {
                //System.Diagnostics.Debug.WriteLine("ReceiveWeakEvent type:{0} sender:{1} EventArgs:{2}", managerType, sender, e);
                if (managerType == typeof(UIFont) && this.font != null && this.font == sender)
                {
                    this.updateFlags |= UpdateFlags.Text;
                    return true;
                }
                return false;
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
                    return this.textColor;
                }
                set
                {
                    if (this.textColor.R != value.R ||
                        this.textColor.G != value.G ||
                        this.textColor.B != value.B ||
                        this.textColor.A != value.A)
                    {
                        this.textColor = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private UIColor textColor;

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
                    return this.textShadow;
                }
                set
                {
                    if (this.textShadow != value)
                    {
                        this.textShadow = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private TextShadowSettings textShadow;

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
                    return this.backgroundColor;
                }
                set
                {
                    if (this.backgroundColor.R != value.R ||
                        this.backgroundColor.G != value.G ||
                        this.backgroundColor.B != value.B ||
                        this.backgroundColor.A != value.A)
                    {
                        this.backgroundColor = value;
                        this.updateFlags |= UpdateFlags.Background;
                    }
                }
            }
            private UIColor backgroundColor;

            /// @if LANG_JA
            /// <summary>水平方向のアラインメント取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the horizontal alignment.</summary>
            /// @endif
            public HorizontalAlignment HorizontalAlignment
            {
                get
                {
                    return this.horizontalAlignment;
                }
                set
                {
                    if (this.horizontalAlignment != value)
                    {
                        this.horizontalAlignment = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
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
                get
                {
                    return this.verticalAlignment;
                }
                set
                {
                    if (this.verticalAlignment != value)
                    {
                        this.verticalAlignment = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private VerticalAlignment verticalAlignment;

            /// @if LANG_JA
            /// <summary>文字列の改行方法を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the break method of the character string.</summary>
            /// @endif
            public LineBreak LineBreak
            {
                get
                {
                    return this.lineBreak;
                }
                set
                {
                    if (this.lineBreak != value)
                    {
                        this.lineBreak = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
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
                get
                {
                    return this.textTrimming;
                }
                set
                {
                    if (this.textTrimming != value)
                    {
                        this.textTrimming = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
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
                get
                {
                    return this.lineGap;
                }
                set
                {
                    if (this.lineGap != value)
                    {
                        this.lineGap = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private float lineGap;

            /// @if LANG_JA
            /// <summary>テキストの高さ</summary>
            /// <remarks>現在の設定でテキストを描画した場合の複数行文字列の高さ(pixel)です。描画される文字列は現在の幅、LineBreakで折り返されます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Test height</summary>
            /// <remarks>Height (pixels) of the multiple-line text rendered with current settings. The rendered text will be wrapped around at the current width, LineBreak.</remarks>
            /// @endif
            public float TextHeight
            {
                get
                {
                    TextRenderHelper textRenderHelper = new TextRenderHelper();
                    textRenderHelper.Font = this.Font;
                    textRenderHelper.HorizontalAlignment = this.HorizontalAlignment;
                    textRenderHelper.VerticalAlignment = this.VerticalAlignment;
                    textRenderHelper.LineBreak = this.LineBreak;
                    textRenderHelper.TextTrimming = this.TextTrimming;
                    textRenderHelper.LineGap = this.LineGap;
                    return textRenderHelper.GetTotalHeight(this.Text, this.Width);
                }
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (this.Width != 0 && this.Height != 0)
                {
                    UpdateBackgroundSprite();

                    if (String.IsNullOrEmpty(this.Text))
                    {
                        this.textSprt.Visible = false;
                    }
                    else
                    {
                        UpdateTextSprite();
                    }
                }
                this.updateFlags = 0;

                base.Render();
            }

            private void UpdateBackgroundSprite()
            {
                if ((this.updateFlags & UpdateFlags.Background) == UpdateFlags.Background)
                {
                    UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                    unit.Width = this.Width;
                    unit.Height = this.Height;
                    unit.Color = this.BackgroundColor;

                    this.updateFlags &= ~UpdateFlags.Background;
                }
            }

            private void UpdateTextSprite()
            {
                if ((this.updateFlags & UpdateFlags.Text) == UpdateFlags.Text)
                {
                    UISpriteUnit unit = this.textSprt.GetUnit(0);
                    unit.Width = this.Width;
                    unit.Height = this.Height;
                    unit.Color = this.TextColor;

                    TextRenderHelper textRenderHelper = new TextRenderHelper();
                    textRenderHelper.Font = this.Font;
                    textRenderHelper.HorizontalAlignment = this.HorizontalAlignment;
                    textRenderHelper.VerticalAlignment = this.VerticalAlignment;
                    textRenderHelper.LineBreak = this.LineBreak;
                    textRenderHelper.TextTrimming = this.TextTrimming;
                    textRenderHelper.LineGap = this.LineGap;

                    this.textSprt.Visible = true;
                    this.textSprt.ShaderType = ShaderType.TextTexture;
                    if (this.textSprt.Image != null)
                    {
                        this.textSprt.Image.Dispose();
                    }
                    this.textSprt.Image = textRenderHelper.DrawText(ref text, (int)unit.Width, (int)unit.Height);

                    if (TextShadow != null)
                    {
                        this.textSprt.InternalShaderType = InternalShaderType.TextureAlphaShadow;
                        if(textSprt.ShaderUniforms == null)
                            textSprt.ShaderUniforms = new Dictionary<string, float[]>(2);
                        this.textSprt.ShaderUniforms["u_ShadowColor"] = new float[] {
                            this.TextShadow.Color.R,
                            this.TextShadow.Color.G,
                            this.TextShadow.Color.B,
                            this.TextShadow.Color.A
                        };
                        this.textSprt.ShaderUniforms["u_ShadowOffset"] = new float[] {
                            this.TextShadow.HorizontalOffset / this.textSprt.Image.Width,
                            this.TextShadow.VerticalOffset / this.textSprt.Image.Height
                        };
                    }

                    this.updateFlags &= ~UpdateFlags.Text;
                }
            }

            private UISprite backgroundSprt;
            private UISprite textSprt;
            private UpdateFlags updateFlags;

        }


    }
}
