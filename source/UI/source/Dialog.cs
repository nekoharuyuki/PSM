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
        /// <summary>ダイアログの背景の種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Dialog background type</summary>
        /// @endif
        public enum DialogBackgroundStyle
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
        /// <summary>モーダルダイアログ</summary>
        /// <remarks>任意の部品を載せることができる。</remarks>
        /// @endif
        /// @if LANG_EN
        /// <summary>Modal Dialog</summary>
        /// <remarks>Any part can be listed.</remarks>
        /// @endif
        public class Dialog : ContainerWidget
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Dialog()
            {
                init(new BunjeeJumpEffect(), new TiltDropEffect());
            }

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public Dialog(Effect showEffect, Effect hideEffect)
            {
                init(showEffect, hideEffect);
            }

            private void init(Effect showEffect, Effect hideEffect)
            {
                this.Width = UISystem.FramebufferWidth;
                this.Height = UISystem.FramebufferHeight;

                this.bgColorSprt = new UISprite(1);
                this.bgColorSprt.ShaderType = ShaderType.SolidFill;
                this.RootUIElement.AddChildLast(this.bgColorSprt);

                this.bgImageSprt = new UISprite(9);
                this.bgImageSprt.ShaderType = ShaderType.Texture;
                this.RootUIElement.AddChildLast(this.bgImageSprt);

                this.ShowEffect = showEffect;
                this.HideEffect = hideEffect;

                this.modalScene = new ModalScene(this);

                needUpdateSprite = true;
            }

            /// @if LANG_JA
            /// <summary>使用されているリソースを解放する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Frees used resources.</summary>
            /// @endif
            protected override void DisposeSelf()
            {
                if (defaultImageAsset != null)
                {
                    defaultImageAsset.Dispose();
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
                    base.Width = value;
                    needUpdateSprite = true;
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
                    base.Height = value;
                    needUpdateSprite = true;
                }
            }

            /// @if LANG_JA
            /// <summary>ダイアログを表示するときのエフェクトを取得・設定する。nullの場合はエフェクトを使用しない。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the effect when displaying a dialog. When there is null, the effect is not used.</summary>
            /// @endif
            public Effect ShowEffect
            {
                get
                {
                    return this.showEffect;
                }
                set
                {
                    if (this.showEffect != null)
                    {
                        this.showEffect.EffectStopped -= new EventHandler<EventArgs>(ShowEffectStopped);
                    }

                    this.showEffect = value;

                    if (this.showEffect != null)
                    {
                        this.showEffect.EffectStopped += new EventHandler<EventArgs>(ShowEffectStopped);
                    }
                }
            }
            private Effect showEffect;


            /// @if LANG_JA
            /// <summary>ダイアログを非表示にするときのエフェクトを取得・設定する。nullの場合はエフェクトを使用しない。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the effect when hiding a dialog. When there is null, the effect is not used.</summary>
            /// @endif
            public Effect HideEffect
            {
                get
                {
                    return this.hideEffect;
                }
                set
                {
                    if (this.hideEffect != null)
                    {
                        this.hideEffect.EffectStopped -= new EventHandler<EventArgs>(HideEffectStopped);
                    }

                    this.hideEffect = value;

                    if (this.hideEffect != null)
                    {
                        this.hideEffect.EffectStopped += new EventHandler<EventArgs>(HideEffectStopped);
                    }
                }
            }
            private Effect hideEffect;

            /// @if LANG_JA
            /// <summary>ダイアログの背景の種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background type of the dialog.</summary>
            /// @endif
            public DialogBackgroundStyle BackgroundStyle
            {
                get { return backgroundStyle; }
                set
                {
                    backgroundStyle = value;
                    needUpdateSprite = true;
                }
            }
            private DialogBackgroundStyle backgroundStyle;


            /// @if LANG_JA
            /// <summary>ダイアログの背景画像を取得・設定する。</summary>
            /// <remarks>BackgroundStyle が Custom の場合のみ有効。nullを設定した場合は背景画像を表示しない。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background image of the dialog.</summary>
            /// <remarks>This is enabled only when BackgroundStyle is Custom. When null is set, the background image is not displayed.</remarks>
            /// @endif
            public ImageAsset CustomBackgroundImage
            {
                get
                {
                    return customBackgroundImage;
                }
                set
                {
                    customBackgroundImage = value;
                    needUpdateSprite = true;
                }
            }
            ImageAsset customBackgroundImage = null;

            /// @if LANG_JA
            /// <summary>ダイアログの背景画像の9PatchMarginを取得・設定する。</summary>
            /// <remarks>BackgroundStyle が Custom で CustomBackgroundImage が設定されている場合のみ有効。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets 9PatchMargin of the background image of the dialog.</summary>
            /// <remarks>This is enabled only when BackgroundStyle is Custom and CustomBackgroundImage is set.</remarks>
            /// @endif
            public NinePatchMargin CustomBackgroundNinePatchMargin
            {
                get
                {
                    return this.customBackgroundNinePatchMargin;
                }
                set
                {
                    this.customBackgroundNinePatchMargin = value;
                    needUpdateSprite = true;
                }
            }
            private NinePatchMargin customBackgroundNinePatchMargin;

            /// @if LANG_JA
            /// <summary>ダイアログの背景色を取得・設定する。</summary>
            /// <remarks>BackgroundStyle が Custom の場合のみ有効。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the background color of the dialog.</summary>
            /// <remarks>This is enabled only when BackgroundStyle is Custom.</remarks>
            /// @endif
            public UIColor CustomBackgroundColor
            {
                get { return customBackgroundColor; }
                set
                {
                    customBackgroundColor = value;
                    needUpdateSprite = true;
                }
            }
            private UIColor customBackgroundColor = new UIColor();
            
            
            /// @if LANG_JA
            /// <summary>背景画像に乗算する色を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the color that multiplies in the background image.</summary>
            /// @endif
            public UIColor BackgroundFilterColor
            {
                get{return backgroundFilterColor;}
                set{
                    backgroundFilterColor = value;
                    needUpdateSprite = true;
                }
            }
            private UIColor backgroundFilterColor = new UIColor(1f, 1f, 1f, 1f);

            private void updateSprite()
            {
                if (this.bgImageSprt != null && this.bgColorSprt != null)
                {
                    needUpdateSprite = false;

                    if (this.backgroundStyle == DialogBackgroundStyle.Default)
                    {
                        this.bgColorSprt.Visible = false;
                        this.bgImageSprt.Visible = true;

                        if (defaultImageAsset == null)
                        {
                            this.defaultImageAsset = new ImageAsset(SystemImageAsset.DialogBackground);
                        }
                        this.bgImageSprt.Image = this.defaultImageAsset;
                        UISpriteUtility.SetupNinePatch(this.bgImageSprt, this.Width, this.Height, 0, 0,
                            AssetManager.GetNinePatchMargin(SystemImageAsset.DialogBackground));
                    }
                    else
                    {
                        var unit = this.bgColorSprt.GetUnit(0);
                        unit.Color = this.customBackgroundColor;
                        unit.SetSize(this.Width, this.Height);
                        this.bgColorSprt.Visible = true;

                        if (customBackgroundImage == null)
                        {
                            this.bgImageSprt.Image = null;
                            this.bgImageSprt.Visible = false;
                        }
                        else if (customBackgroundImage.Ready)
                        {
                            this.bgImageSprt.Image = customBackgroundImage;
                            this.bgImageSprt.Visible = true;
                            UISpriteUtility.SetupNinePatch(this.bgImageSprt, this.Width, this.Height, 0, 0, this.customBackgroundNinePatchMargin);
                        }
                        else
                        {
                            this.bgImageSprt.Visible = false;
                            needUpdateSprite = true;
                        }
                    }
                    
                    // set multiply color
                    for (int i = 0; i < bgImageSprt.UnitCount; i++)
                    {
                        var unit = bgImageSprt.GetUnit(i);
                        unit.Color = backgroundFilterColor;
                    }

                }
            }

            /// @if LANG_JA
            /// <summary>ダイアログを表示する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Displays the dialog box.</summary>
            /// @endif
            public void Show()
            {
                if (ShowState != ShowStateInternal.Hidden)
                    return;

                ShowState = ShowStateInternal.ShowingEffect;

                this.modalScene.ScreenOrientation = UISystem.CurrentScene.ScreenOrientation;

                if (Showing != null)
                {
                    Showing(this, EventArgs.Empty);
                }

                UISystem.setFocusVisible(false, true);
                UISystem.ResetStateAll();
                UISystem.addModalScnene(this.modalScene);

                this.Transform3D = Matrix4.Identity;

                this.PivotType = PivotType.TopLeft;
                this.X = (this.modalScene.RootWidget.Width - this.Width) / 2.0f;
                this.Y = (this.modalScene.RootWidget.Height - this.Height) / 2.0f;

                this.Alpha = 1.0f;
                this.Visible = true;

                if (this.ShowEffect == null)
                {
                    ShowEffectStopped(this, null);
                }
                else
                {
                    this.ResetState(false);
                    this.ShowEffect.Widget = this;
                    this.ShowEffect.Start();
                }
            }


            /// @if LANG_JA
            /// <summary>ダイアログを表示する。</summary>
            /// <param name="effect">ダイアログを表示する時のエフェクト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Displays the dialog box.</summary>
            /// <param name="effect">Effect when the dialog is displayed</param>
            /// @endif
            public void Show(Effect effect)
            {
                this.ShowEffect = effect;
                Show();
            }

            private void ShowEffectStopped(object sender, EventArgs e)
            {
                ResetFocus(true);

                if (Shown != null)
                {
                    Shown(this, EventArgs.Empty);
                }

                ShowState = ShowStateInternal.Shown;
            }

            /// @if LANG_JA
            /// <summary>ダイアログを非表示にする。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Hides the dialog box.</summary>
            /// @endif
            public void Hide()
            {
                if (ShowState != ShowStateInternal.Shown)
                    return;

                ShowState = ShowStateInternal.HidingEffect;

                if(Hiding != null)
                {
                    var args = new DialogEventArgs(this.Result);
                    Hiding(this, args);

                    if (args.CancelHide)
                    {
                        ShowState = ShowStateInternal.Shown;
                        return;
                    }
                }

                UISystem.setFocusVisible(false, true);

                if (this.HideEffect == null)
                {
                    HideEffectStopped(this, null);
                }
                else
                {
                    this.ResetState(false);
                    this.HideEffect.Widget = this;
                    this.HideEffect.Start();
                }
            }


            /// @if LANG_JA
            /// <summary>ダイアログを非表示にする。</summary>
            /// <param name="effect">ダイアログを表示する時のエフェクト</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Hides the dialog box.</summary>
            /// <param name="effect">Effect when the dialog is displayed</param>
            /// @endif
            public void Hide(Effect effect)
            {
                this.HideEffect = effect;
                Hide();
            }

            private void HideEffectStopped(object sender, EventArgs e)
            {
                this.Visible = false;
                this.Transform3D = Matrix4.Identity;

                UISystem.ResetStateAll();

                UISystem.removeModalScene(this.modalScene);

                UISystem.CurrentScene.ResetFocus(false, false);

                if (Hidden != null)
                {
                    Hidden(this, new DialogEventArgs(this.Result));
                }

                ShowState = ShowStateInternal.Hidden;
            }

            /// @if LANG_JA
            /// <summary>ダイアログが表示されるときに発行するイベント</summary>
            /// <remarks>ShowEffectが設定されている場合は、Effectが開始される前に発行する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event issued when a dialog is displayed</summary>
            /// <remarks>This is issued before an Effect is started when a ShowEffect is set.</remarks>
            /// @endif
            public event EventHandler Showing;

            /// @if LANG_JA
            /// <summary>ダイアログが表示されたときに発行するイベント</summary>
            /// <remarks>ShowEffectが設定されている場合は、Effectが終了したときに発行する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event issued when a dialog is displayed</summary>
            /// <remarks>This is issued at the time an Effect is terminated when a ShowEffect is set.</remarks>
            /// @endif
            public event EventHandler Shown;

            /// @if LANG_JA
            /// <summary>ダイアログが消えるときに発行するイベント</summary>
            /// <remarks>HideEffectが設定されている場合は、Effectが開始される前に発行する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event issued when a dialog disappears</summary>
            /// <remarks>This is issued before an Effect is started when a HideEffect is set.</remarks>
            /// @endif
            public event EventHandler<DialogEventArgs> Hiding;

            /// @if LANG_JA
            /// <summary>ダイアログが消えたときに発行するイベント</summary>
            /// <remarks>HideEffectが設定されている場合は、Effectが終了したときに発行する。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Event issued when a dialog has disappeared</summary>
            /// <remarks>This is issued at the time an Effect is terminated when a HideEffect is set.</remarks>
            /// @endif
            public event EventHandler<DialogEventArgs> Hidden;

            private bool hideOnTouchOutside = false;

            /// @if LANG_JA
            /// <summary>ダイアログの外をタッチされたときにダイアログを閉じるかどうか</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Whether to close a dialog when the outside of the dialog is touched</summary>
            /// @endif
            public bool HideOnTouchOutside
            {
                get { return hideOnTouchOutside; }
                set { hideOnTouchOutside = value; }
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (needUpdateSprite)
                {
                    updateSprite();
                }
                base.Render();
            }


            internal void ResetFocus(bool useFirstFocus)
            {
                bool focusActive = UISystem.FocusActive;
                Widget focus = null;

                if (defaultFocusWidget != null)
                {
                    bool belongToThis = false;
                    for (var node = defaultFocusWidget.LinkedTree; node != null; node = node.Parent)
                    {
                        if (node.Value == this)
                        {
                            belongToThis = true;
                            break;
                        }
                    }
                    if (belongToThis)
                    {
                        focus = defaultFocusWidget;
                    }
                }

                if (focus == null)
                {
                    for (var node = this.Parent.LinkedTree; node != null; node = node.NextAsList)
                    {
                       node.Value.updateFinalClip(false);
                    }
                    focus = this.SearchNextTreeNodeFocus(false);
                }

                if(focus != null && focus != this)
                {
                    focus.SetFocus(false);
                    if(focusActive)
                        UISystem.FocusActive = true;
                }

                if(!focusActive)
                {
                    UISystem.FocusActive = false;
                }
            }

            private Widget defaultFocusWidget;

            /// @if LANG_JA
            /// <summary>最初にフォーカスが当たるウィジェット</summary>
            /// <remarks>nullの場合は既定のウィジェットになります。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Widget in focus first</summary>
            /// <remarks>When there is null, the default widget is used.</remarks>
            /// @endif
            public Widget DefaultFocusWidget
            {
                get { return defaultFocusWidget; }
                set { defaultFocusWidget = value; }
            }

            /// @if LANG_JA
            /// <summary>ダイアログ結果を取得・設定する</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the dialog result</summary>
            /// @endif
            public DialogResult Result
            {
                get;
                set;
            }

            private ShowStateInternal ShowState
            {
                get { return _showState; }
                set
                {
                    switch (value)
                    {
                        case ShowStateInternal.ShowingEffect:
                        case ShowStateInternal.HidingEffect:
                            UISystem.IsDialogEffect = true;
                            break;
                        case ShowStateInternal.Hidden:
                        case ShowStateInternal.Shown:
                            UISystem.IsDialogEffect = false;
                            break;
                    }
                    _showState = value;
                }
            }
            private ShowStateInternal _showState;

            enum ShowStateInternal
            {
                Hidden = 0,
                ShowingEffect,
                Shown,
                HidingEffect,
            }

            private bool needUpdateSprite = true;
            private UISprite bgImageSprt;
            private UISprite bgColorSprt;
            private ImageAsset defaultImageAsset = null;
            private ModalScene modalScene;
        }

        /// @if LANG_JA
        /// <summary>ダイアログイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Dialog event argument</summary>
        /// @endif
        public class DialogEventArgs : EventArgs
        {
            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="result">ダイアログ結果</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="result">Dialog result</param>
            /// @endif
            public DialogEventArgs(DialogResult result)
            {
                this.CancelHide = false;
                this.Result = result;
            }

            /// @if LANG_JA
            /// <summary>Hide処理をキャンセルするかどうかを設定する</summary>
            /// <remarks>Hidingイベントでこのプロパティをtrueにすると、ダイアログの閉じる処理をキャンセルすることができます。
            /// このプロパティは、Hiddenイベントでは無視されます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Sets whether to cancel Hide processing</summary>
            /// <remarks>When this property is set to true at Hiding events, the dialog closing process can be canceled.
            /// This property is ignored at Hidden events.</remarks>
            /// @endif
            public bool CancelHide { get; set; }

            /// @if LANG_JA
            /// <summary>ダイアログ結果</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Dialog result</summary>
            /// @endif
            public DialogResult Result { get; private set; }
        }

        /// @if LANG_JA
        /// <summary>ダイアログ結果</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Dialog result</summary>
        /// @endif
        public enum DialogResult
        {
            /// @if LANG_JA
            /// <summary>OK</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>OK</summary>
            /// @endif
            Ok = 0,
            /// @if LANG_JA
            /// <summary>キャンセル</summary>
            /// <remarks>この値は、明示的に設定された場合以外に、Backボタンおよびダイアログの外をタッチしてダイアログを閉じた場合に設定されます。</remarks>
            /// @endif
            /// @if LANG_EN
            /// <summary>Cancel</summary>
            /// <remarks>This value is set when the dialog is closed by touching the Back button or the area outside the dialog, except when explicitly set.</remarks>
            /// @endif
            Cancel,
        }

        internal class ModalScene : Scene
        {
            public Dialog Dialog { get; private set; }
            public ModalScene(Dialog dialog)
            {
                this.ConsumeAllTouchEvent = true;

                this.Dialog = dialog;
                this.RootWidget.AddChildLast(dialog);

                this.RootWidget.TouchEventReceived += new EventHandler<TouchEventArgs>(RootWidget_TouchEventReceived);
                this.RootWidget.KeyEventReceived += new EventHandler<KeyEventArgs>(RootWidget_KeyEventReceived);
            }

            void RootWidget_KeyEventReceived(object sender, KeyEventArgs e)
            {
                UIDebug.Assert(Dialog != null);

                if (e.KeyType == KeyType.Back && e.KeyEventType == KeyEventType.Down)
                {
                    Dialog.Result = DialogResult.Cancel;
                    Dialog.Hide();
                    UISystem.CancelKeyEvents();
                    e.Handled = true;
                }
            }

            void RootWidget_TouchEventReceived(object sender, TouchEventArgs e)
            {
                UIDebug.Assert(Dialog != null);

                if (Dialog.HideOnTouchOutside)
                {
                    if (e.TouchEvents.Count == 1 && e.TouchEvents.PrimaryTouchEvent.Type == TouchEventType.Up)
                    {
                        Dialog.Result = DialogResult.Cancel;
                        Dialog.Hide();
                    }
                }
            }
        }

    }
}
