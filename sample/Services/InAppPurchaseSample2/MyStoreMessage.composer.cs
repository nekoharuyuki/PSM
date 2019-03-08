// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    partial class MyStoreMessage
    {
        Label Label_1;
        Button Button_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Button_1 = new Button();
            Button_1.Name = "Button_1";

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // Button_1
            Button_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Button_1.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 127f / 255f);

            // MyStoreMessage
            this.BackgroundFilterColor = new UIColor(75f / 255f, 128f / 255f, 255f / 255f, 191f / 255f);
            this.AddChildLast(Label_1);
            this.AddChildLast(Button_1);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetPosition(0, 0);
                    this.SetSize(320, 640);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(101, 78);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Button_1.SetPosition(286, 360);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_1.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(640, 320);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(38, 29);
                    Label_1.SetSize(562, 202);
                    Label_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    Label_1.Visible = true;

                    Button_1.SetPosition(212, 233);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Button_1.Text = "OK";
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
