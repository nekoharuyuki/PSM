/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;


namespace Sce.PlayStation
{
    namespace HighLevel.UI
    {


        /// @if LANG_JA
        /// <summary>メッセージダイアログの種類</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Types of message dialogs</summary>
        /// @endif
        public enum MessageDialogStyle
        {
            /// @if LANG_JA
            /// <summary>OKボタンのメッセージダイアログ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Message dialog of the OK button</summary>
            /// @endif
            Ok = 0,

            /// @if LANG_JA
            /// <summary>OK・Cancelボタンのメッセージダイアログ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Message dialog of the OK/Cancel button</summary>
            /// @endif
            OkCancel
        }


        /// @if LANG_JA
        /// <summary>メッセージダイアログのボタンの種別</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Types of message dialog buttons</summary>
        /// @endif
        public enum MessageDialogResult
        {
            /// @if LANG_JA
            /// <summary>OKボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>OK button</summary>
            /// @endif
            Ok = 0,

            /// @if LANG_JA
            /// <summary>Cancelボタン</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Cancel button</summary>
            /// @endif
            Cancel
        }


        /// @if LANG_JA
        /// <summary>警告のメッセージ等を表示するための特殊化されたモーダルダイアログ</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Specialized modal dialog for displaying warning messages</summary>
        /// @endif
        public class MessageDialog : Dialog
        {
            const float minimumWidth = 500f;
            const float minimumHeight = 260f;
            const float margin = 15f;
            const float titleLabelHeight = 70f;

            private bool needUpdateMessageSize = true;

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public MessageDialog()
                : base(null, null)
            {
                base.Width = minimumWidth;
                base.Height = minimumHeight;

                base.X = (int)(UISystem.FramebufferWidth * 0.1f);
                base.Y = (int)(UISystem.FramebufferHeight * 0.05f);


                // Title
                this.title = new Label();
                this.title.SetPosition(margin, margin);
                this.title.SetSize(minimumWidth - margin * 2, titleLabelHeight);
                this.title.HorizontalAlignment = HorizontalAlignment.Center;
                this.title.VerticalAlignment = VerticalAlignment.Middle;
                this.title.TextColor = new UIColor(1.0f, 1.0f, 1.0f, 1.0f);
                this.title.Font = new UIFont(FontAlias.System, 28, FontStyle.Regular);
                this.title.Text = "";
                this.title.Anchors =
                    Anchors.Top | Anchors.Height |
                    Anchors.Left | Anchors.Right;
                this.AddChildLast(this.title);

                // Separator Image
                this.separatorImage = new ImageBox();
                this.separatorImage.X = 35f;
                this.separatorImage.Y = margin + titleLabelHeight;
                this.separatorImage.Width = minimumWidth - 35f * 2f;
                this.separatorImage.Height = 2.0f;
                this.separatorImage.Image = new ImageAsset(SystemImageAsset.MessageDialogSeparator);
                this.separatorImage.NinePatchMargin = AssetManager.GetNinePatchMargin(SystemImageAsset.MessageDialogSeparator);
                this.separatorImage.ImageScaleType = ImageScaleType.NinePatch;
                this.separatorImage.Anchors =
                    Anchors.Top | Anchors.Height |
                    Anchors.Left | Anchors.Right;
                this.AddChildLast(this.separatorImage);

                // Ok Button
                this.okButton = new Button();
                this.okButton.Text = "OK";
                this.okButton.ButtonAction += okButtonAction;

                // Cancel Button
                this.cancelButton = new Button();
                this.cancelButton.Text = "Cancel";
                this.cancelButton.ButtonAction += cancelButtonAction;

                // button backgraund panel
                this.buttonPanel = new Panel();
                this.buttonPanel.SetPosition(margin, minimumHeight - margin * 2 - okButton.Height);
                this.buttonPanel.SetSize(minimumWidth - margin * 2, okButton.Height);
                this.buttonPanel.BackgroundColor = new UIColor();
                this.buttonPanel.Anchors =
                    Anchors.Height | Anchors.Bottom |
                    Anchors.Width;
                this.AddChildLast(this.buttonPanel);
                this.buttonPanel.AddChildLast(this.okButton);
                this.buttonPanel.AddChildLast(this.cancelButton);

                // Message
                this.messageScrollPanel = new ScrollPanel();
                this.messageScrollPanel.SetPosition(35f,margin + titleLabelHeight + 20f);
                this.messageScrollPanel.SetSize(minimumWidth - 55f,58f);
                this.messageScrollPanel.PanelWidth = this.messageScrollPanel.Width;
                this.messageScrollPanel.PanelHeight = this.messageScrollPanel.Height;
                this.messageScrollPanel.PanelColor = new UIColor(0f, 0f, 0f, 0f);
                this.messageScrollPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
                this.messageScrollPanel.Anchors =
                    Anchors.Top | Anchors.Bottom |
                    Anchors.Left | Anchors.Right;
                this.AddChildLast(this.messageScrollPanel);

                this.message = new Label();
                this.message.Width = this.messageScrollPanel.PanelWidth - 15.0f;
                this.message.Height = this.messageScrollPanel.PanelHeight;
                this.message.TextColor = new UIColor(1.0f, 1.0f, 1.0f, 1.0f);
                this.message.Font = new UIFont(FontAlias.System, 24, FontStyle.Regular);
                this.message.HorizontalAlignment = HorizontalAlignment.Left;
                this.message.VerticalAlignment = VerticalAlignment.Top;
                this.message.Text = "";
                this.message.Anchors =
                    Anchors.Top | Anchors.Bottom |
                    Anchors.Left | Anchors.Right;
                this.messageScrollPanel.AddChildLast(this.message);

                this.Style = MessageDialogStyle.OkCancel;

                this.ShowTitle = true;

                this.PriorityHit = true;

                this.Width = (int)(UISystem.FramebufferWidth * 0.8f);
                this.Height = (int)(UISystem.FramebufferHeight * 0.9f);

                this.Showing += showingHandler;
                this.Hiding += hidingHandler;

                this.DefaultFocusWidget = this.okButton;
            }

            void showingHandler(object sender, EventArgs e)
            {
                if (UISystem.IsCurrentVerticalLayout && this.Width > UISystem.FramebufferHeight)
                {
                    this.X = (int)(UISystem.FramebufferHeight * 0.05f);
                    this.Width = (int)(UISystem.FramebufferHeight * 0.9f);
                }
            }

            void hidingHandler(object sender, DialogEventArgs e)
            {
                if (this.ButtonPressed != null)
                {
                    if (this.Result == DialogResult.Ok)
                    {
                        MessageDialogButtonEventArgs args = new MessageDialogButtonEventArgs(MessageDialogResult.Ok);
                        this.ButtonPressed(this, args);
                    }
                    else
                    {
                        MessageDialogButtonEventArgs args = new MessageDialogButtonEventArgs(MessageDialogResult.Cancel);
                        this.ButtonPressed(this, args);
                    }
                }
            }

            private void okButtonAction(object sender, TouchEventArgs e)
            {
                this.Result = DialogResult.Ok;

                this.Hide();
            }

            private void cancelButtonAction(object sender, TouchEventArgs e)
            {
                this.Result = DialogResult.Cancel;

                this.Hide();
            }




            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// <param name="style">メッセージダイアログの種類</param>
            /// <param name="title">タイトルの文字列</param>
            /// <param name="message">メッセージの文字列</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// <param name="style">Types of message dialogs</param>
            /// <param name="title">Title character string</param>
            /// <param name="message">Message character string</param>
            /// @endif
            public MessageDialog(MessageDialogStyle style, string title, string message)
                : this()
            {
                this.Message = message;
                this.Title = title;
                this.Style = style;
            }

            /// @if LANG_JA
            /// <summary>MessageDialog のインスタンスを生成し表示する。</summary>
            /// <param name="style">メッセージダイアログの種類</param>
            /// <param name="title">タイトルの文字列</param>
            /// <param name="message">メッセージの文字列</param>
            /// @endif
            /// @if LANG_EN
            /// <summary>Creates and displays a MessageDialog instance.</summary>
            /// <param name="style">Types of message dialogs</param>
            /// <param name="title">Title character string</param>
            /// <param name="message">Message character string</param>
            /// @endif
            public static MessageDialog CreateAndShow(MessageDialogStyle style, string title, string message)
            {
                MessageDialog msg = new MessageDialog(style, title, message);
                if (UISystem.IsCurrentVerticalLayout)
                {
                    msg.X = (int)(UISystem.FramebufferHeight * 0.05f);
                    msg.Width = (int)(UISystem.FramebufferHeight * 0.9f);
                }
                msg.Show();
                return msg;
            }

            /// @if LANG_JA
            /// <summary>シーングラフを描画する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Renders a scene graph.</summary>
            /// @endif
            protected internal override void Render()
            {
                if (needUpdateMessageSize)
                {
                    this.messageScrollPanel.PanelHeight = this.message.TextHeight;
                    needUpdateMessageSize = false;
                }

                base.Render();
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
                    if (value > minimumWidth)
                        base.Width = value;
                    else
                        base.Width = minimumWidth;

                    if (this.messageScrollPanel != null)
                    {
                        this.messageScrollPanel.PanelWidth = this.messageScrollPanel.Width;
                        needUpdateMessageSize = true;
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
                    if (value > minimumHeight)
                        base.Height = value;
                    else
                        base.Height = minimumHeight;
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
                if (this.separatorImage != null && this.separatorImage.Image != null)
                {
                    this.separatorImage.Image.Dispose();
                }

                base.DisposeSelf();
            }

            /// @if LANG_JA
            /// <summary>メッセージを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the message.</summary>
            /// @endif
            public String Message
            {
                get
                {
                    return this.message.Text;
                }
                set
                {
                    this.message.Text = value;

                    needUpdateMessageSize = true;
                }
            }

            /// @if LANG_JA
            /// <summary>タイトルを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the title.</summary>
            /// @endif
            public String Title
            {
                get
                {
                    return this.title.Text;
                }
                set
                {
                    this.title.Text = value;
                }
            }

            /// @if LANG_JA
            /// <summary>タイトルを表示するかどうかを取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets whether to display the title.</summary>
            /// @endif
            public bool ShowTitle
            {
                get
                {
                    return this.showTitle;
                }
                set
                {
                    this.showTitle = value;

                    if (this.title != null)
                    {
                        this.title.Visible = this.showTitle;
                    }
                    if (this.separatorImage != null)
                    {
                        this.separatorImage.Visible = this.showTitle;
                    }
                }
            }
            private bool showTitle;

            /// @if LANG_JA
            /// <summary>メッセージダイアログの種類を取得・設定する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains and sets the type of message dialog.</summary>
            /// @endif
            public MessageDialogStyle Style
            {
                get
                {
                    return this.style;
                }
                set
                {
                    this.style = value;
                    UpdateLayout();
                }
            }
            private MessageDialogStyle style;

            /// @if LANG_JA
            /// <summary>メッセージダイアログのボタンの押下時に呼び出されるハンドラ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Handler called when the message dialog button is pressed</summary>
            /// @endif
            public event EventHandler<MessageDialogButtonEventArgs> ButtonPressed;


            private void UpdateLayout()
            {
                if (this.Style == MessageDialogStyle.Ok)
                {
                    this.leftButton = this.okButton;
                    this.rightButton = this.cancelButton;

                    this.okButton.X = (buttonPanel.Width - this.okButton.Width) / 2f;

                    this.cancelButton.Visible = false;
                    this.cancelButton.TouchResponse = false;
                }
                else if (this.Style == MessageDialogStyle.OkCancel)
                {
                    if (SystemParameters.YesNoLayout == YesNoLayout.YesIsLeft)
                    {
                        this.leftButton = this.okButton;
                        this.rightButton = this.cancelButton;
                    }
                    else
                    {
                        this.leftButton = this.cancelButton;
                        this.rightButton = this.okButton;
                    }

                    this.buttonPanel.X = (this.Width - this.buttonPanel.Width) / 2.0f;

                    this.leftButton.X = 0f;
                    this.rightButton.X = this.buttonPanel.Width-this.rightButton.Width;

                    this.cancelButton.Visible = true;
                    this.cancelButton.TouchResponse = true;
                }
            }

            private ScrollPanel messageScrollPanel;
            private Label message;
            private Label title;
            private ImageBox separatorImage;
            private Button okButton;
            private Button cancelButton;
            private Button leftButton;
            private Button rightButton;
            private Panel buttonPanel;

        }


        /// @if LANG_JA
        /// <summary>メッセージダイアログのイベント引数</summary>
        /// @endif
        /// @if LANG_EN
        /// <summary>Event argument of message dialog</summary>
        /// @endif
        public class MessageDialogButtonEventArgs : EventArgs
        {

            /// @if LANG_JA
            /// <summary>コンストラクタ</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Constructor</summary>
            /// @endif
            public MessageDialogButtonEventArgs(MessageDialogResult result)
            {
                this.result = result;
            }

            /// @if LANG_JA
            /// <summary>メッセージダイアログのボタンの種別を取得する。</summary>
            /// @endif
            /// @if LANG_EN
            /// <summary>Obtains the type of button of the message dialog.</summary>
            /// @endif
            public MessageDialogResult Result
            {
                get { return result; }
            }
            MessageDialogResult result;
        }


    }
}
