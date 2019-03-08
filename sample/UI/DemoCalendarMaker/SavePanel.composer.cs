// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace CalendarMaker
{
    partial class SavePanel
    {
        ImageBox ImageBox_1;
        Label Label_1;
        BusyIndicator BusyIndicator_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            BusyIndicator_1 = new BusyIndicator();
            BusyIndicator_1.Name = "BusyIndicator_1";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/menu_window_2.png");
            ImageBox_1.ImageScaleType = ImageScaleType.NinePatch;
            ImageBox_1.NinePatchMargin = new NinePatchMargin(16, 16, 16, 16);

            // Label_1
            Label_1.TextColor = new UIColor(224f / 255f, 255f / 255f, 240f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // SavePanel
            this.BackgroundColor = new UIColor(224f / 255f, 232f / 255f, 235f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(Label_1);
            this.AddChildLast(BusyIndicator_1);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(300, 300);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(55, 0);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    Label_1.SetPosition(44, 77);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    BusyIndicator_1.SetPosition(127, 130);
                    BusyIndicator_1.SetSize(48, 48);
                    BusyIndicator_1.Anchors = Anchors.None;
                    BusyIndicator_1.Visible = true;

                    break;

                default:
                    this.SetSize(250, 68);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(250, 60);
                    ImageBox_1.Anchors = Anchors.Height | Anchors.Width;
                    ImageBox_1.Visible = true;

                    Label_1.SetPosition(24, 0);
                    Label_1.SetSize(176, 54);
                    Label_1.Anchors = Anchors.Height | Anchors.Width;
                    Label_1.Visible = true;

                    BusyIndicator_1.SetPosition(188, 4);
                    BusyIndicator_1.SetSize(48, 48);
                    BusyIndicator_1.Anchors = Anchors.Height | Anchors.Width;
                    BusyIndicator_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "label";
        }

        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public void StartDefaultEffect()
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
