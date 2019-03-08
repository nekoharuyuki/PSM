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


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>ボタンの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Button type</summary>
        /// @endif
        public enum ButtonStyle
        {
            /// @if LANG_JA
            /// <summary>デフォルト</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Default</summary>
            /// @endif
            Default = 0,

            /// @if LANG_JA
            /// <summary>カスタマイズ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Customize</summary>
            /// @endif
            Custom
        }


        /// @if LANG_JA
        /// <summary>ボタンウィジェット</summary>
        /// <remarks>アイコン画像と１行テキストを設定することが可能。形状のカスタマイズが可能。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Button widget</summary>
        /// <remarks>An icon image and a single line of text can be set. The shape can be customized.</remarks>
        /// @endif
        public class Button : Widget, IWeakEventListener
        {

            private const float defaultButtonWidth = 214.0f;
            private const float defaultButtonHeight = 56.0f;

            [Flags]
            private enum UpdateFlags
            {
                Background = 0x01,
                Text = 0x02,
                Icon = 0x04
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Button()
            {
                backgroundPrim = new UIPrimitive(DrawMode.TriangleStrip, 16, 28);
                RootUIElement.AddChildLast(backgroundPrim);
                backgroundPrim.ShaderType = ShaderType.Texture;

                iconSprt = new UISprite(1);
                RootUIElement.AddChildLast(iconSprt);
                iconSprt.ShaderType = ShaderType.SolidFill;
                iconSprt.Visible = false;

                textSprt = new UISprite(1);
                RootUIElement.AddChildLast(textSprt);
                textSprt.ShaderType = ShaderType.TextTexture;
                textSprt.Visible = false;

                updateFlags = UpdateFlags.Background;
                int buttonStyleCount = Enum.GetValues(typeof(ButtonStyle)).Length;
                backgroundImages = new ImageAsset[buttonStyleCount, PressStateChangedEventArgs.PressStateCount];
                backgroundImages[(int)ButtonStyle.Default, (int)PressState.Normal] =
                    new ImageAsset(SystemImageAsset.ButtonBackgroundNormal);
                backgroundImages[(int)ButtonStyle.Default, (int)PressState.Pressed] =
                    new ImageAsset(SystemImageAsset.ButtonBackgroundPressed);
                backgroundImages[(int)ButtonStyle.Default, (int)PressState.Disabled] =
                    new ImageAsset(SystemImageAsset.ButtonBackgroundDisabled);

                backgroundNinePatchs = new NinePatchMargin[buttonStyleCount];
                backgroundNinePatchs[(int)ButtonStyle.Default] =
                    AssetManager.GetNinePatchMargin(SystemImageAsset.ButtonBackgroundNormal);

                CustomImage = null;

                IconImage = null;

                Text = "";
                TextFont = new UIFont();
                TextTrimming = TextTrimming.EllipsisCharacter;
                TextColor = new UIColor(1.0f, 1.0f, 1.0f, 1.0f);
                TextShadow = null;
                HorizontalAlignment = HorizontalAlignment.Center;
                VerticalAlignment = VerticalAlignment.Middle;
                Pressable = true;

                Style = ButtonStyle.Default;

                Width = defaultButtonWidth;
                Height = defaultButtonHeight;
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
                // update flag
                this.updateFlags |= UpdateFlags.Background;
                if (e.NewState == PressState.Disabled || e.OldState == PressState.Disabled)
                {
                    this.updateFlags |= (UpdateFlags.Text | UpdateFlags.Icon);
                }

                if (e.checkDetectedButtonAction() && ButtonAction != null)
                {
                    this.ButtonAction(this, TouchEventArgs.CreateDummy());
                }
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                for (int i = 0; i < backgroundImages.GetLength(1); i++)
                {
                    if (backgroundImages[(int)ButtonStyle.Default, i] != null)
                    {
                        backgroundImages[(int)ButtonStyle.Default, i].Dispose();
                    }
                }
                if(this.textSprt != null && this.textSprt.Image != null)
                {
                    this.textSprt.Image.Dispose();
                    this.textSprt.Image = null;
                }
                this.TextFont = null;
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

                        this.updateFlags |= UpdateFlags.Background;
                        if (!String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                        if (this.IconImage != null)
                        {
                            this.updateFlags |= UpdateFlags.Icon;
                        }
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

                        this.updateFlags |= UpdateFlags.Background;
                        if (!String.IsNullOrEmpty(this.Text))
                        {
                            this.updateFlags |= UpdateFlags.Text;
                        }
                        if (this.IconImage != null)
                        {
                            this.updateFlags |= UpdateFlags.Icon;
                        }
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
                        this.text = value;
                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private string text;

            /// @if LANG_JA
            /// <summary>ボタンの種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the button type.</summary>
            /// @endif
            public ButtonStyle Style
            {
                get
                {
                    return this.style;
                }
                set
                {
                    if (this.style != value)
                    {
                        this.style = value;
                        this.updateFlags |= UpdateFlags.Background;
                    }
                }
            }
            private ButtonStyle style;

            /// @if LANG_JA
            /// <summary>アイコン画像を取得・設定する。</summary>
            /// <remarks>nullが設定された場合、設定されている文字列を表示する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the icon image.</summary>
            /// <remarks>Displays the set character string when null is set.</remarks>
            /// @endif
            public ImageAsset IconImage
            {
                get
                {
                    return this.iconImage;
                }
                set
                {
                    if (this.iconImage != value)
                    {
                        this.iconImage = value;
                        this.updateFlags |= UpdateFlags.Icon;
                    }
                }
            }
            private ImageAsset iconImage;

            /// @if LANG_JA
            /// <summary>文字列のフォントを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the font of the character string.</summary>
            /// @endif
            public UIFont TextFont
            {
                get
                {
                    return this.textFont;
                }
                set
                {
                    if (this.textFont != value)
                    {
                        if(this.textFont != null)
                            this.textFont.RemoveListener(this);

                        this.textFont = value;

                        if (this.textFont != null)
                            this.textFont.AddListener(this);

                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private UIFont textFont;

            bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                if (managerType == typeof(UIFont) && this.textFont != null && this.textFont == sender)
                {
                    this.updateFlags |= UpdateFlags.Text;
                    return true;
                }
                return false;
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
                        if (this.textShadow != null)
                        {
                            this.textShadow.ValueChanged -= textShadowChanged;
                        }

                        this.textShadow = value;

                        if (this.textShadow != null)
                        {
                            this.textShadow.ValueChanged += textShadowChanged;
                        }

                        this.updateFlags |= UpdateFlags.Text;
                    }
                }
            }
            private TextShadowSettings textShadow;

            private void textShadowChanged()
            {
                this.updateFlags |= UpdateFlags.Text;
            }

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
                        this.updateFlags |= (UpdateFlags.Text | UpdateFlags.Icon);
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
                        this.updateFlags |= (UpdateFlags.Text | UpdateFlags.Icon);
                    }
                }
            }
            private VerticalAlignment verticalAlignment;

            /// @if LANG_JA
            /// <summary>カスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets custom images.</summary>
            /// @endif
            public CustomButtonImageSettings CustomImage
            {
                get
                {
                    return this.customImage;
                }
                set
                {
                    if (this.customImage != value)
                    {
                        if (this.customImage != null)
                        {
                            this.customImage.ValueChanged -= customImageChanged;
                        }

                        this.customImage = value;

                        if (this.customImage != null)
                        {
                            customImageChanged();
                            this.customImage.ValueChanged += customImageChanged;
                        }

                        this.updateFlags |= UpdateFlags.Background;
                    }
                }
            }
            private CustomButtonImageSettings customImage;

            private void customImageChanged()
            {
                backgroundImages[(int)ButtonStyle.Custom, (int)PressState.Normal] = this.customImage.BackgroundNormalImage;
                backgroundImages[(int)ButtonStyle.Custom, (int)PressState.Pressed] = this.customImage.BackgroundPressedImage;
                backgroundImages[(int)ButtonStyle.Custom, (int)PressState.Disabled] = this.customImage.BackgroundDisabledImage;
                backgroundNinePatchs[(int)ButtonStyle.Custom] = this.customImage.BackgroundNinePatchMargin;
                this.updateFlags |= UpdateFlags.Background;
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
                if (this.Width != 0 && this.Height != 0)
                {
                    UpdateBackgroundSprite();
                    UpdateTextSprite();
                    UpdateIconSprite();
                }

                base.OnUpdate(elapsedTime);
            }

            private void UpdateBackgroundSprite()
            {
                if ((this.updateFlags & UpdateFlags.Background) == UpdateFlags.Background)
                {
                    this.updateFlags &= ~UpdateFlags.Background;

                    ImageAsset imageAsset = this.backgroundImages[(int)this.Style, (int)this.PressState];
                    if (imageAsset == null)
                    {
                        this.backgroundPrim.Image = null;
                        this.backgroundPrim.Visible = true;
                    }
                    else if (imageAsset.Ready)
                    {
                        this.backgroundPrim.Image = imageAsset;
                        this.backgroundPrim.VertexCount = 9;
                        this.backgroundPrim.Visible = true;

                        UIPrimitiveUtility.SetupNinePatch(
                            this.backgroundPrim,
                            this.Width, this.Height,
                            0.0f, 0.0f,
                            this.backgroundNinePatchs[(int)this.Style]);
                    }
                    else
                    {
                        this.backgroundPrim.Visible = false;
                        this.updateFlags |= UpdateFlags.Background;
                    }
                }
            }

            private void UpdateTextSprite()
            {
                if (String.IsNullOrEmpty(this.Text))
                {
                    this.textSprt.Visible = false;
                    this.updateFlags &= ~UpdateFlags.Text;
                }

                if ((this.updateFlags & UpdateFlags.Text) == UpdateFlags.Text)
                {
                    TextRenderHelper textRenderHelper = new TextRenderHelper();
                    textRenderHelper.LineBreak = LineBreak.AtCode;
                    textRenderHelper.HorizontalAlignment = this.HorizontalAlignment;
                    textRenderHelper.VerticalAlignment = this.VerticalAlignment;
                    textRenderHelper.Font = this.TextFont;
                    textRenderHelper.TextTrimming = this.TextTrimming;

                    UISpriteUnit unit = this.textSprt.GetUnit(0);
                    unit.Width = this.Width;
                    unit.Height = this.Height;
                    unit.Color = this.TextColor;

                    if (this.textSprt.Image != null)
                    {
                        this.textSprt.Image.Dispose();
                    }
                    this.textSprt.Image = textRenderHelper.DrawText(ref this.text, (int)unit.Width, (int)unit.Height);
                    this.textSprt.Alpha = (this.Enabled) ? 1.0f : 0.3f;
                    this.textSprt.ShaderType = ShaderType.TextTexture;
                    this.textSprt.Visible = true;
                    this.iconSprt.Visible = false;

                    if (this.TextShadow != null)
                    {
                        this.textSprt.InternalShaderType = InternalShaderType.TextureAlphaShadow;
                        if(textSprt.ShaderUniforms == null)
                            textSprt.ShaderUniforms = new Dictionary<string, float[]>(2);
                        this.textSprt.ShaderUniforms["u_ShadowColor"] = new float[] {
                            TextShadow.Color.R,
                            TextShadow.Color.G,
                            TextShadow.Color.B,
                            TextShadow.Color.A
                        };
                        this.textSprt.ShaderUniforms["u_ShadowOffset"] = new float[] {
                            TextShadow.HorizontalOffset / textSprt.Image.Width,
                            TextShadow.VerticalOffset / textSprt.Image.Height
                        };
                    }

                    this.updateFlags &= ~UpdateFlags.Text;
                }
            }

            private void UpdateIconSprite()
            {
                if ((this.updateFlags & UpdateFlags.Icon) == UpdateFlags.Icon)
                {
                    this.updateFlags &= ~UpdateFlags.Icon;

                    if (this.IconImage == null)
                    {
                        this.iconSprt.Visible = false;
                    }
                    else if (this.IconImage.Ready)
                    {
                        UISpriteUnit unit = this.iconSprt.GetUnit(0);
                        unit.Width = this.IconImage.Width;
                        unit.Height = this.IconImage.Height;

                        if (unit.Width > this.Width)
                        {
                            // crop by this.Width
                            switch (this.HorizontalAlignment)
                            {
                                case HorizontalAlignment.Left:
                                    unit.U1 = 0.0f;
                                    unit.U2 = this.Width / unit.Width;
                                    break;
                                case HorizontalAlignment.Center:
                                    unit.U1 = (unit.Width - this.Width) / 2.0f / unit.Width;
                                    unit.U2 = 1.0f - unit.U1;
                                    break;
                                case HorizontalAlignment.Right:
                                    unit.U1 = 1.0f - this.Width / unit.Width;
                                    unit.U2 = 1.0f;
                                    break;
                            }
                            unit.X = 0.0f;
                            unit.Width = this.Width;
                        }
                        else
                        {
                            switch (this.HorizontalAlignment)
                            {
                                case HorizontalAlignment.Left:
                                    unit.X = 0.0f;
                                    break;
                                case HorizontalAlignment.Center:
                                    unit.X = (this.Width - unit.Width) / 2.0f;
                                    break;
                                case HorizontalAlignment.Right:
                                    unit.X = this.Width - unit.Width;
                                    break;
                            }
                            unit.U1 = 0.0f;
                            unit.U2 = 1.0f;
                        }

                        if (unit.Height > this.Height)
                        {
                            // crop by this.Height
                            switch (this.VerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    unit.V1 = 0.0f;
                                    unit.V2 = this.Height / unit.Height;
                                    break;
                                case VerticalAlignment.Middle:
                                    unit.V1 = (unit.Height - this.Height) / 2.0f / unit.Height;
                                    unit.V2 = 1.0f - unit.V1;
                                    break;
                                case VerticalAlignment.Bottom:
                                    unit.V1 = 1.0f - this.Height / unit.Height;
                                    unit.V2 = 1.0f;
                                    break;
                            }
                            unit.Y = 0.0f;
                            unit.Height = this.Height;
                        }
                        else
                        {
                            switch (this.VerticalAlignment)
                            {
                                case VerticalAlignment.Top:
                                    unit.Y = 0.0f;
                                    break;
                                case VerticalAlignment.Middle:
                                    unit.Y = (this.Height - unit.Height) / 2.0f;
                                    break;
                                case VerticalAlignment.Bottom:
                                    unit.Y = this.Height - unit.Height;
                                    break;
                            }
                            unit.V1 = 0.0f;
                            unit.V2 = 1.0f;
                        }

                        this.iconSprt.ShaderType = ShaderType.Texture;
                        this.iconSprt.Image = this.IconImage;
                        this.iconSprt.Visible = true;
                        this.textSprt.Visible = false;

                    }
                    else
                    {
                        this.iconSprt.Visible = false;
                        this.textSprt.Visible = false;
                        this.updateFlags |= UpdateFlags.Icon;
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>ボタンアクション発火時に呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when a button action is fired</summary>
            /// @endif
            public event EventHandler<TouchEventArgs> ButtonAction;

            /// @if LANG_JA
            /// <summary>背景画像に乗算する色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color that multiplies in the background image.</summary>
            /// @endif
            public UIColor BackgroundFilterColor {
                get{return backgroundFilterColor;}
                set{
                    backgroundFilterColor = value;
                    for (int i = 0; i < backgroundPrim.VertexCount; i++) {
                        var vrtx = backgroundPrim.GetVertex(i);
                        vrtx.Color = value;
                    }
                    this.updateFlags |= UpdateFlags.Background;
                }
            }

            private UIColor backgroundFilterColor = new UIColor(1.0f, 1.0f, 1.0f, 1.0f);
   
            private UISprite iconSprt;
            private UISprite textSprt;
            private UIPrimitive backgroundPrim;
            private ImageAsset[,] backgroundImages;
            private NinePatchMargin[] backgroundNinePatchs;
            private UpdateFlags updateFlags;

        }


        /// @if LANG_JA
        /// <summary>文字列の影の設定情報</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Setting information of shadow of character string</summary>
        /// @endif
        public class TextShadowSettings
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public TextShadowSettings()
            {
                this.color = new UIColor(0.5f, 0.5f, 0.5f, 0.5f);
                this.horizontalOffset = 2.0f;
                this.verticalOffset = 2.0f;
                this.ValueChanged = null;
           }

            /// @if LANG_JA
            /// <summary>文字列の影の色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color of the shadow of the character string.</summary>
            /// @endif
            public UIColor Color
            {
                get
                {
                    return this.color;
                }
                set
                {
                    this.color = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>文字列の影の水平方向のオフセット位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the horizontal offset position of the shadow of the character string.</summary>
            /// @endif
            public float HorizontalOffset
            {
                get
                {
                    return this.horizontalOffset;
                }
                set
                {
                    this.horizontalOffset = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>文字列の影の垂直方向のオフセット位置を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the vertical offset position of the shadow of the character string.</summary>
            /// @endif
            public float VerticalOffset
            {
                get
                {
                    return this.verticalOffset;
                }
                set
                {
                    this.verticalOffset = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            internal delegate void TextShadowSettingsChanged();
            internal TextShadowSettingsChanged ValueChanged;

            private UIColor color;
            private float horizontalOffset;
            private float verticalOffset;

        }


        /// @if LANG_JA
        /// <summary>ボタンのカスタマイズ画像の設定情報</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Setting information of button custom image</summary>
        /// @endif
        public class CustomButtonImageSettings
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public CustomButtonImageSettings()
            {
                this.backgroundNormalImage = null;
                this.backgroundPressedImage = null;
                this.backgroundDisabledImage = null;
                this.backgroundNinePatchMargin = NinePatchMargin.Zero;
                this.ValueChanged = null;
            }

            /// @if LANG_JA
            /// <summary>通常時の背景のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the custom image to always be displayed in the background.</summary>
            /// @endif
            public ImageAsset BackgroundNormalImage
            {
                get
                {
                    return this.backgroundNormalImage;
                }
                set
                {
                    this.backgroundNormalImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>押下時の背景のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the custom image displayed in the background when pressed.</summary>
            /// @endif
            public ImageAsset BackgroundPressedImage
            {
                get
                {
                    return this.backgroundPressedImage;
                }
                set
                {
                    this.backgroundPressedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>無効時の背景のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the custom image displayed in the background when disabled.</summary>
            /// @endif
            public ImageAsset BackgroundDisabledImage
            {
                get
                {
                    return this.backgroundDisabledImage;
                }
                set
                {
                    this.backgroundDisabledImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>背景のカスタマイズ画像の９パッチ情報を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the 9-patch information of the custom image displayed in the background.</summary>
            /// @endif
            public NinePatchMargin BackgroundNinePatchMargin
            {
                get
                {
                    return this.backgroundNinePatchMargin;
                }
                set
                {
                    this.backgroundNinePatchMargin = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            internal delegate void CustomButtonImageSettingsChanged();
            internal CustomButtonImageSettingsChanged ValueChanged;

            private ImageAsset backgroundNormalImage;
            private ImageAsset backgroundPressedImage;
            private ImageAsset backgroundDisabledImage;
            private NinePatchMargin backgroundNinePatchMargin;

        }


    }
}
