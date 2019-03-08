// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ButtonScene
    {
        Panel contentPanel;
        Label Label_1;
        Button button1;
        Label label2;
        Button button2;
        Label label3;
        Button button3;
        Label label4;
        Button button4;
        Button enableButton;
        Label labelExecuteButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            button1 = new Button();
            button1.Name = "button1";
            label2 = new Label();
            label2.Name = "label2";
            button2 = new Button();
            button2.Name = "button2";
            label3 = new Label();
            label3.Name = "label3";
            button3 = new Button();
            button3.Name = "button3";
            label4 = new Label();
            label4.Name = "label4";
            button4 = new Button();
            button4.Name = "button4";
            enableButton = new Button();
            enableButton.Name = "enableButton";
            labelExecuteButton = new Label();
            labelExecuteButton.Name = "labelExecuteButton";

            // ButtonScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(Label_1);
            contentPanel.AddChildLast(button1);
            contentPanel.AddChildLast(label2);
            contentPanel.AddChildLast(button2);
            contentPanel.AddChildLast(label3);
            contentPanel.AddChildLast(button3);
            contentPanel.AddChildLast(label4);
            contentPanel.AddChildLast(button4);
            contentPanel.AddChildLast(enableButton);
            contentPanel.AddChildLast(labelExecuteButton);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.VerticalAlignment = VerticalAlignment.Bottom;

            // button1
            button1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // label2
            label2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label2.LineBreak = LineBreak.Character;
            label2.VerticalAlignment = VerticalAlignment.Bottom;

            // button2
            button2.IconImage = new ImageAsset("/Application/assets/next.png");

            // label3
            label3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label3.LineBreak = LineBreak.Character;
            label3.VerticalAlignment = VerticalAlignment.Bottom;

            // button3
            button3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button3.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            button3.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f),
                HorizontalOffset = 3f,
                VerticalOffset = 3f,
            };

            // label4
            label4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label4.LineBreak = LineBreak.Character;
            label4.VerticalAlignment = VerticalAlignment.Bottom;

            // button4
            button4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button4.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // enableButton
            enableButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            enableButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // labelExecuteButton
            labelExecuteButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            labelExecuteButton.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelExecuteButton.LineBreak = LineBreak.Character;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 480;
                    this.DesignHeight = 854;

                    contentPanel.SetPosition(0, 211);
                    contentPanel.SetSize(480, 641);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    Label_1.SetPosition(27, 0);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    button1.SetPosition(27, 57);
                    button1.SetSize(214, 56);
                    button1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button1.Visible = true;

                    label2.SetPosition(27, 120);
                    label2.SetSize(214, 36);
                    label2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label2.Visible = true;

                    button2.SetPosition(27, 156);
                    button2.SetSize(214, 56);
                    button2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button2.Visible = true;

                    label3.SetPosition(27, 236);
                    label3.SetSize(214, 36);
                    label3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label3.Visible = true;

                    button3.SetPosition(27, 272);
                    button3.SetSize(214, 56);
                    button3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button3.Visible = true;

                    label4.SetPosition(27, 353);
                    label4.SetSize(214, 36);
                    label4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label4.Visible = true;

                    button4.SetPosition(27, 401);
                    button4.SetSize(358, 56);
                    button4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    button4.Visible = true;

                    enableButton.SetPosition(171, 487);
                    enableButton.SetSize(214, 56);
                    enableButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    enableButton.Visible = true;

                    labelExecuteButton.SetPosition(133, 569);
                    labelExecuteButton.SetSize(214, 36);
                    labelExecuteButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    labelExecuteButton.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    Label_1.SetPosition(63, 81);
                    Label_1.SetSize(200, 36);
                    Label_1.Anchors = Anchors.Height;
                    Label_1.Visible = true;

                    button1.SetPosition(63, 117);
                    button1.SetSize(200, 56);
                    button1.Anchors = Anchors.Height;
                    button1.Visible = true;

                    label2.SetPosition(321, 81);
                    label2.SetSize(200, 36);
                    label2.Anchors = Anchors.Height;
                    label2.Visible = true;

                    button2.SetPosition(321, 117);
                    button2.SetSize(200, 56);
                    button2.Anchors = Anchors.Height;
                    button2.Visible = true;

                    label3.SetPosition(576, 81);
                    label3.SetSize(200, 36);
                    label3.Anchors = Anchors.Height;
                    label3.Visible = true;

                    button3.SetPosition(576, 117);
                    button3.SetSize(200, 56);
                    button3.Anchors = Anchors.Height;
                    button3.Visible = true;

                    label4.SetPosition(63, 228);
                    label4.SetSize(200, 36);
                    label4.Anchors = Anchors.Height;
                    label4.Visible = true;

                    button4.SetPosition(63, 264);
                    button4.SetSize(458, 56);
                    button4.Anchors = Anchors.Height;
                    button4.Visible = true;

                    enableButton.SetPosition(576, 264);
                    enableButton.SetSize(200, 56);
                    enableButton.Anchors = Anchors.Height;
                    enableButton.Visible = true;

                    labelExecuteButton.SetPosition(63, 21);
                    labelExecuteButton.SetSize(727, 34);
                    labelExecuteButton.Anchors = Anchors.Height;
                    labelExecuteButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Normal Button";

            button1.Text = "Button1";

            label2.Text = "Icon Image On";

            label3.Text = "Text Shadow On";

            button3.Text = "Button3";

            label4.Text = "Enable";

            button4.Text = "Change left button enable ->";

            enableButton.Text = "Disbale";
        }

        private void onShowing(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        private void onShown(object sender, EventArgs e)
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

    }
}
