/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>編集可能なテキストウィジェット</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Editable text widget</summary>
        /// @endif
        public class EditableText : Widget, IWeakEventListener
        {

            private const float defaultEditableTextWidth = 360.0f;
            private const float defaultEditableTextHeight = 56.0f;

            private const float textVerticalOffset = 4.0f;
            private const float textHorizontalOffset = 10.0f;

            [Flags]
            private enum UpdateFlags
            {
                Background = 0x01,
                Text = 0x02
            }

            ImageAsset[] bgImages;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public EditableText()
            {
                bgImages = new ImageAsset[PressStateChangedEventArgs.PressStateCount];
                bgImages[(int)PressState.Normal] = new ImageAsset(SystemImageAsset.EditableTextBackgroundNormal);
                bgImages[(int)PressState.Pressed] = bgImages[(int)PressState.Normal];
                bgImages[(int)PressState.Disabled] = new ImageAsset(SystemImageAsset.EditableTextBackgroundDisabled);



                this.backgroundSprt = new UISprite(9);
                this.RootUIElement.AddChildLast(this.backgroundSprt);
                this.backgroundSprt.ShaderType = ShaderType.Texture;
                this.backgroundSprt.Image = bgImages[(int)PressState.Normal];
                this.backgroundNinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.EditableTextBackgroundNormal);

                this.textSprt = new UISprite(1);
                this.RootUIElement.AddChildLast(this.textSprt);
                this.textSprt.ShaderType = ShaderType.TextTexture;

                this.DefaultText = "Please input the text.";
                this.DefaultFont = new UIFont();
                this.DefaultTextColor = new UIColor(0.75f, 0.75f, 0.75f, 0.75f);

                this.Text = "";
                this.Font = this.DefaultFont;
                this.TextColor = TextRenderHelper.DefaultTextColor;

                this.TextShadow = null;

                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Middle;

                this.LineBreak = LineBreak.Character;
                this.TextTrimming = TextTrimming.EllipsisCharacter;
                this.LineGap = 0.0f;

                this.Width = defaultEditableTextWidth;
                this.Height = defaultEditableTextHeight;

                this.TextInputMode = TextInputMode.Normal;
                this.Pressable = true;

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
                if (backgroundSprt != null && backgroundSprt.Image != null)
                {
                    backgroundSprt.Image.Dispose();
                    backgroundSprt.Image = null;
                }
                if(this.textSprt != null && this.textSprt.Image != null)
                {
                    this.textSprt.Image.Dispose();
                    this.textSprt.Image = null;
                }
                this.Font = null;
                this.DefaultFont = null;
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
                    return this.text;
                }
                set
                {
                    if (this.text != value)
                    {
                        var args = new TextChangedEventArgs(this.text, value);
                        this.text = value;
                        this.updateFlags |= UpdateFlags.Text;

                        if (TextChanged != null)
                        {
                            TextChanged(this, args);
                        }
                    }
                }
            }
            private string text;

            /// @if LANG_JA
            /// <summary>Textプロパティが変更された場合に発行するイベント</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event issued when Text property is changed</summary>
            /// @endif
            public event EventHandler<TextChangedEventArgs> TextChanged;

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

                        if (!String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                    }
                }
            }
            private UIFont font;

            bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                if (managerType == typeof(UIFont) && sender != null &&
                    (this.font == sender || this.defaultFont == sender))
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
                        if (!String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                    }

                }
            }
            private UIColor textColor;

            /// @if LANG_JA
            /// <summary>デフォルトの文字列を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the default character string.</summary>
            /// @endif
            public string DefaultText
            {
                get
                {
                    return this.defaultText;
                }
                set
                {
                    if (this.defaultText != value)
                    {
                        this.defaultText = value;
                        if (String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                    }
                }
            }
            private string defaultText;

            /// @if LANG_JA
            /// <summary>デフォルトの文字列のフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the font of the default character string.</summary>
            /// @endif
            public UIFont DefaultFont
            {
                get
                {
                    return this.defaultFont;
                }
                set
                {
                    if (this.defaultFont != value)
                    {
                        if (this.defaultFont != null)
                            this.defaultFont.RemoveListener(this);

                        this.defaultFont = value;

                        if (this.defaultFont != null)
                            this.defaultFont.AddListener(this);

                        if (String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                    }
                }
            }
            private UIFont defaultFont;

            /// @if LANG_JA
            /// <summary>デフォルトの文字列の色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the default character string.</summary>
            /// @endif
            public UIColor DefaultTextColor
            {
                get
                {
                    return this.defaultTextColor;
                }
                set
                {
                    if (this.defaultTextColor.R != value.R ||
                        this.defaultTextColor.G != value.G ||
                        this.defaultTextColor.B != value.B ||
                        this.defaultTextColor.A != value.A)
                    {
                        this.defaultTextColor = value;
                        if (String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                    }
                }
            }
            private UIColor defaultTextColor;

            /// @if LANG_JA
            /// <summary>文字列の影を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the shadow of the character string.</summary>
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
            /// <summary>文字列またはアイコン画像の水平方向のアライメントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the horizontal alignment for character strings or icon images.</summary>
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
            /// <summary>文字列またはアイコン画像の垂直方向のアライメントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the vertical alignment for character strings or icon images.</summary>
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
            /// <summary>テキスト入力の動作モードを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the operation mode for text input.</summary>
            /// @endif
            public TextInputMode TextInputMode
            {
                get
                {
                    return this.textInputMode;
                }
                set
                {
                    if (this.textInputMode != value)
                    {
                        this.textInputMode = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private TextInputMode textInputMode;

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
                this.updateFlags = UpdateFlags.Background;

                this.textSprt.Alpha = e.NewState == UI.PressState.Disabled ? 0.3f : 1.0f;
                this.backgroundSprt.Image = bgImages[(int)e.NewState];

                if (e.NewState == UI.PressState.Pressed)
                {
                    this.dialog = new TextInputDialog();
                    this.dialog.Mode = this.TextInputMode;
                    this.dialog.Text = this.Text;
                    this.dialog.Open();
                }
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

                if (this.dialog != null)
                {
                    if (this.dialog.State == CommonDialogState.Finished)
                    {
                        if (this.dialog.Result == CommonDialogResult.OK)
                        {
                            this.Text = this.dialog.Text;
                        }
                        this.dialog.Dispose();
                        this.dialog = null;
                    }
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
                    UpdateTextSprite();
                }
                this.updateFlags = 0;

                base.Render();
            }

            private void UpdateBackgroundSprite()
            {
                if ((this.updateFlags & UpdateFlags.Background) != 0)
                {
                    UISpriteUnit unit = this.backgroundSprt.GetUnit(0);
                    unit.Width = this.Width;
                    unit.Height = this.Height;

                    UISpriteUtility.SetupNinePatch(
                        this.backgroundSprt,
                        this.Width, this.Height,
                        0.0f, 0.0f,
                        this.backgroundNinePatchMargin);

                    this.updateFlags &= ~UpdateFlags.Background;
                }
            }

            private void UpdateTextSprite()
            {
                if ((this.updateFlags & UpdateFlags.Text) != 0)
                {
                    UISpriteUnit unit = this.textSprt.GetUnit(0);
                    unit.X = textHorizontalOffset;
                    unit.Y = textVerticalOffset;
                    unit.Width = this.Width - (textHorizontalOffset * 2.0f);
                    unit.Height = this.Height - (textVerticalOffset * 2.0f);

                    TextRenderHelper textRenderHelper = new TextRenderHelper();
                    textRenderHelper.HorizontalAlignment = this.HorizontalAlignment;
                    textRenderHelper.VerticalAlignment = this.VerticalAlignment;
                    textRenderHelper.LineBreak = this.LineBreak;
                    textRenderHelper.TextTrimming = this.TextTrimming;
                    textRenderHelper.LineGap = this.LineGap;

                    if (String.IsNullOrEmpty(this.Text))
                    {
                        unit.Color = DefaultTextColor;
                        textRenderHelper.Font = this.DefaultFont;
                        this.textSprt.ShaderType = ShaderType.TextTexture;
                        if (this.textSprt.Image != null)
                        {
                            this.textSprt.Image.Dispose();
                        }
                        this.textSprt.Image = textRenderHelper.DrawText(ref this.defaultText, (int)unit.Width, (int)unit.Height);
                    }
                    else
                    {
                        string displayText;
                        if (this.TextInputMode == TextInputMode.Password)
                        {
                            displayText = new string('*', this.Text.Length);
                        }
                        else
                        {
                            displayText = this.Text;
                        }

                        unit.Color = TextColor;
                        textRenderHelper.Font = this.Font;
                        this.textSprt.ShaderType = ShaderType.TextTexture;
                        if (this.textSprt.Image != null)
                        {
                            this.textSprt.Image.Dispose();
                        }
                        this.textSprt.Image = textRenderHelper.DrawText(ref displayText, (int)unit.Width, (int)unit.Height);
                    }

                    if (TextShadow != null)
                    {
                        textSprt.InternalShaderType = InternalShaderType.TextureAlphaShadow;
                        if(textSprt.ShaderUniforms == null)
                            textSprt.ShaderUniforms = new Dictionary<string, float[]>(2);
                        textSprt.ShaderUniforms["u_ShadowColor"] = new float[] {
                            TextShadow.Color.R,
                            TextShadow.Color.G,
                            TextShadow.Color.B,
                            TextShadow.Color.A
                        };
                        textSprt.ShaderUniforms["u_ShadowOffset"] = new float[] {
                            TextShadow.HorizontalOffset / textSprt.Image.Width,
                            TextShadow.VerticalOffset / textSprt.Image.Height
                        };
                    }

                    this.updateFlags &= ~UpdateFlags.Text;
                }
            }


            private UISprite textSprt;
            private UISprite backgroundSprt;
            private NinePatchMargin backgroundNinePatchMargin;
            private TextInputDialog dialog;
            private UpdateFlags updateFlags;
        }

        /// @if LANG_JA
        /// <summary>テキストを変更したときのイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event argument when the text has changed</summary>
        /// @endif
        public class TextChangedEventArgs : EventArgs
        {
            private string newText;
            private string oldText;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="oldText">変更前のテキスト</param>
            /// <param name="newText">変更後のテキスト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="oldText">Text before change</param>
            /// <param name="newText">Text after change</param>
            /// @endif
            public TextChangedEventArgs(string oldText, string newText)
            {
                this.newText = newText;
                this.oldText = oldText;
            }

            /// @if LANG_JA
            /// <summary>変更後のテキスト</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Text after change</summary>
            /// @endif
            public string NewText
            {
                get { return newText; }
            }

            /// @if LANG_JA
            /// <summary>変更前のテキスト</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Text before change</summary>
            /// @endif
            public string OldText
            {
                get { return oldText; }
            }
        }
    }
}
