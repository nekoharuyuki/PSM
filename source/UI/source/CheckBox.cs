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
using System.Diagnostics;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>チェックボックスの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box type</summary>
        /// @endif
        public enum CheckBoxStyle
        {
            /// @if LANG_JA
            /// <summary>チェックボックス</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Check box</summary>
            /// @endif
            CheckBox = 0,

            /// @if LANG_JA
            /// <summary>ラジオボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Radio button</summary>
            /// @endif
            RadioButton,

            /// @if LANG_JA
            /// <summary>カスタマイズ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Customize</summary>
            /// @endif
            Custom
        }


        /// @if LANG_JA
        /// <summary>チェックボックスウィジェット</summary>
        /// <remarks>形状のカスタマイズが可能。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Check box widget</summary>
        /// <remarks>The shape can be customized.</remarks>
        /// @endif
        public class CheckBox : Widget
        {

            private const float defaultCheckBoxWidth = 56.0f;
            private const float defaultCheckBoxHeight = 56.0f;

            private const float defaultRadioButtonWidth = 39.0f;
            private const float defaultRadioButtonHeight = 39.0f;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public CheckBox()
            {
                base.Width = defaultCheckBoxWidth;
                base.Height = defaultCheckBoxHeight;

                this.sprt = new UISprite(1);
                this.sprt.ShaderType = ShaderType.Texture;
                this.RootUIElement.AddChildLast(this.sprt);

                this.Checked = false;
                this.CustomCheckBoxImage = null;
                this.Style = CheckBoxStyle.CheckBox;
                this.needUpdateFlag = true;
                this.Pressable = true;

                this.images = new ImageAsset[
                    Enum.GetValues(typeof(CheckBoxStyle)).Length,
                    PressStateChangedEventArgs.PressStateCount, 2];

                // setup CheckBox images
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Normal, 0] =
                    new ImageAsset(SystemImageAsset.CheckBoxUncheckedNormal);
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Normal, 1] =
                    new ImageAsset(SystemImageAsset.CheckBoxCheckedNormal);
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Pressed, 0] =
                    new ImageAsset(SystemImageAsset.CheckBoxUncheckedPressed);
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Pressed, 1] =
                    new ImageAsset(SystemImageAsset.CheckBoxCheckedPressed);
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Disabled, 0] =
                    new ImageAsset(SystemImageAsset.CheckBoxUncheckedDisabled);
                this.images[(int)CheckBoxStyle.CheckBox, (int)PressState.Disabled, 1] =
                    new ImageAsset(SystemImageAsset.CheckBoxCheckedDisabled);

                // setup RadioButton images
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Normal, 0] =
                    new ImageAsset(SystemImageAsset.RadioButtonUncheckedNormal);
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Normal, 1] =
                    new ImageAsset(SystemImageAsset.RadioButtonCheckedNormal);
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Pressed, 0] =
                    new ImageAsset(SystemImageAsset.RadioButtonUncheckedPressed);
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Pressed, 1] =
                    new ImageAsset(SystemImageAsset.RadioButtonCheckedPressed);
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Disabled, 0] =
                    new ImageAsset(SystemImageAsset.RadioButtonUncheckedDisabled);
                this.images[(int)CheckBoxStyle.RadioButton, (int)PressState.Disabled, 1] =
                    new ImageAsset(SystemImageAsset.RadioButtonCheckedDisabled);
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
                this.needUpdateFlag = true;

                if (e.checkDetectedButtonAction())
                {
                    if (this.style != CheckBoxStyle.RadioButton || !this.Checked)
                    {
                        this.Checked = !this.Checked;
                        if (this.CheckedChanged != null)
                        {
                            this.CheckedChanged(this, TouchEventArgs.CreateDummy());
                        }
                    }
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
                for (int i = 0; i < images.GetLength(1); i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (images[(int)CheckBoxStyle.CheckBox, i, j] != null)
                            images[(int)CheckBoxStyle.CheckBox, i, j].Dispose();
                        if (images[(int)CheckBoxStyle.RadioButton, i, j] != null)
                            images[(int)CheckBoxStyle.RadioButton, i, j].Dispose();
                    }
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
                    if (this.Style == CheckBoxStyle.Custom)
                    {
                        base.Width = value;
                        this.needUpdateFlag = true;
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
                    if (this.Style == CheckBoxStyle.Custom)
                    {
                        base.Height = value;
                        this.needUpdateFlag = true;
                    }
                }
            }


            /// @if LANG_JA
            /// <summary>チェックの状態を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the check status.</summary>
            /// @endif
            public bool Checked
            {
                get
                {
                    return this.checkedValue;
                }
                set
                {
                    if (this.checkedValue != value)
                    {
                        this.checkedValue = value;
                        this.needUpdateFlag = true;
                    }
                }
            }
            private bool checkedValue;


            /// @if LANG_JA
            /// <summary>チェックボックスの種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the check box type.</summary>
            /// @endif
            public CheckBoxStyle Style
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
                        switch (this.style)
                        {
                            case CheckBoxStyle.CheckBox:
                                base.Width = defaultCheckBoxWidth;
                                base.Height = defaultCheckBoxHeight;
                                FocusStyle = FocusStyle.RoundedCorner;
                                break;
                            case CheckBoxStyle.RadioButton:
                                base.Width = defaultRadioButtonWidth;
                                base.Height = defaultRadioButtonHeight;
                                FocusStyle = FocusStyle.Circle;
                                break;
                        }
                        this.needUpdateFlag = true;
                    }
                }
            }
            private CheckBoxStyle style;


            /// @if LANG_JA
            /// <summary>チェックの状態が変化したときに呼ばれるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the check status changes</summary>
            /// @endif
            public event EventHandler<TouchEventArgs> CheckedChanged;


            /// @if LANG_JA
            /// <summary>カスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets custom images.</summary>
            /// @endif
            public CustomCheckBoxImageSettings CustomCheckBoxImage
            {
                get
                {
                    return this.customCheckBoxImage;
                }
                set
                {
                    if (this.customCheckBoxImage != value)
                    {
                        if (this.customCheckBoxImage != null)
                        {
                            this.customCheckBoxImage.ValueChanged -= customImageChanged;
                        }

                        this.customCheckBoxImage = value;

                        if (this.customCheckBoxImage != null)
                        {
                            customImageChanged();
                            this.customCheckBoxImage.ValueChanged += customImageChanged;
                        }

                        this.needUpdateFlag = true;
                    }
                }
            }
            private CustomCheckBoxImageSettings customCheckBoxImage;

            private void customImageChanged()
            {
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Normal, 0] = this.customCheckBoxImage.NormalUncheckedImage;
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Normal, 1] = this.customCheckBoxImage.NormalCheckedImage;
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Pressed, 0] = this.customCheckBoxImage.PressedUncheckedImage;
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Pressed, 1] = this.customCheckBoxImage.PressedCheckedImage;
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Disabled, 0] = this.customCheckBoxImage.DisabledUncheckedImage;
                this.images[(int)CheckBoxStyle.Custom, (int)PressState.Disabled, 1] = this.customCheckBoxImage.DisabledCheckedImage;
                this.needUpdateFlag = true;
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (this.needUpdateFlag)
                {
                    UISpriteUnit unit = sprt.GetUnit(0);
                    unit.Width = this.Width;
                    unit.Height = this.Height;
                    this.sprt.Image = this.images[(int)Style, (int)PressState, Checked ? 1 : 0];
                    this.needUpdateFlag = false;
                }

                base.Render();
            }

            private UISprite sprt;
            private ImageAsset[, ,] images;
            private bool needUpdateFlag;

        }


        /// @if LANG_JA
        /// <summary>カスタマイズ画像の設定情報</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Setting information of custom image</summary>
        /// @endif
        public class CustomCheckBoxImageSettings
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public CustomCheckBoxImageSettings()
            {
                this.NormalUncheckedImage = null;
                this.NormalCheckedImage = null;
                this.PressedUncheckedImage = null;
                this.PressedCheckedImage = null;
                this.DisabledUncheckedImage = null;
                this.DisabledCheckedImage = null;
                this.ValueChanged = null;
            }

            /// @if LANG_JA
            /// <summary>通常時の未チェックのカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the unchecked custom image to always be displayed.</summary>
            /// @endif
            public ImageAsset NormalUncheckedImage
            {
                get
                {
                    return this.normalUncheckedImage;
                }
                set
                {
                    this.normalUncheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>通常時のチェック済のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the checked custom image to always be displayed.</summary>
            /// @endif
            public ImageAsset NormalCheckedImage
            {
                get
                {
                    return this.normalCheckedImage;
                }
                set
                {
                    this.normalCheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>押下時の未チェックのカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the unchecked custom image when pressed.</summary>
            /// @endif
            public ImageAsset PressedUncheckedImage
            {
                get
                {
                    return this.pressedUncheckedImage;
                }
                set
                {
                    this.pressedUncheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>押下時のチェック済のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the checked custom image when pressed.</summary>
            /// @endif
            public ImageAsset PressedCheckedImage
            {
                get
                {
                    return this.pressedCheckedImage;
                }
                set
                {
                    this.pressedCheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>無効時の未チェックのカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the unchecked custom image when disabled.</summary>
            /// @endif
            public ImageAsset DisabledUncheckedImage
            {
                get
                {
                    return this.disabledUncheckedImage;
                }
                set
                {
                    this.disabledUncheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            /// @if LANG_JA
            /// <summary>無効時のチェック済のカスタマイズ画像を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the checked custom image when disabled.</summary>
            /// @endif
            public ImageAsset DisabledCheckedImage
            {
                get
                {
                    return this.disabledCheckedImage;
                }
                set
                {
                    this.disabledCheckedImage = value;
                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged();
                    }
                }
            }

            internal delegate void CustomCheckBoxImageSettingsChanged();
            internal CustomCheckBoxImageSettingsChanged ValueChanged;

            private ImageAsset normalUncheckedImage;
            private ImageAsset normalCheckedImage;
            private ImageAsset pressedUncheckedImage;
            private ImageAsset pressedCheckedImage;
            private ImageAsset disabledUncheckedImage;
            private ImageAsset disabledCheckedImage;

        }


    }
}
