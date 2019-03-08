// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SampleDialog
    {
        Label Label_1;
        ImageBox ImageBox_1;
        Button button1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            button1 = new Button();
            button1.Name = "button1";

            // SampleDialog
            this.AddChildLast(Label_1);
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(button1);
            this.HideEffect = new TiltDropEffect();

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/stephanie_275x196.png");
            ImageBox_1.ImageScaleType = ImageScaleType.AspectInside;

            // button1
            button1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            button1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;

                    Label_1.SetPosition(122, 43);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    ImageBox_1.SetPosition(31, 111);
                    ImageBox_1.SetSize(426, 566);
                    ImageBox_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    ImageBox_1.Visible = true;

                    button1.SetPosition(243, 774);
                    button1.SetSize(214, 56);
                    button1.Anchors = Anchors.Bottom | Anchors.Height | Anchors.Right | Anchors.Width;
                    button1.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(700, 350);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(100, 27);
                    Label_1.SetSize(500, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    ImageBox_1.SetPosition(100, 72);
                    ImageBox_1.SetSize(500, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    button1.SetPosition(465, 272);
                    button1.SetSize(200, 56);
                    button1.Anchors = Anchors.Bottom | Anchors.Height | Anchors.Right;
                    button1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "Press Hide button to hide this dialog.";

            button1.Text = "Hide dialog";
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
