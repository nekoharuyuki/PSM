// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScreenOrientationSample
{
    partial class ScreenOrientationScene
    {
        Panel PanelTitle;
        Label LabelTitle;
        Button ButtonBack;
        Button ButtonDummy1;
        Button ButtonDummy2;
        ImageBox ImageBox_1;
        Panel PanelPortrait;
        Label Label_1;
        Button ButtonDummy3;
        Button ButtonDummy4;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            PanelTitle = new Panel();
            PanelTitle.Name = "PanelTitle";
            LabelTitle = new Label();
            LabelTitle.Name = "LabelTitle";
            ButtonBack = new Button();
            ButtonBack.Name = "ButtonBack";
            ButtonDummy1 = new Button();
            ButtonDummy1.Name = "ButtonDummy1";
            ButtonDummy2 = new Button();
            ButtonDummy2.Name = "ButtonDummy2";
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            PanelPortrait = new Panel();
            PanelPortrait.Name = "PanelPortrait";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            ButtonDummy3 = new Button();
            ButtonDummy3.Name = "ButtonDummy3";
            ButtonDummy4 = new Button();
            ButtonDummy4.Name = "ButtonDummy4";

            // ScreenOrientationScene
            this.RootWidget.AddChildLast(PanelTitle);
            this.RootWidget.AddChildLast(ButtonBack);
            this.RootWidget.AddChildLast(ButtonDummy1);
            this.RootWidget.AddChildLast(ButtonDummy2);
            this.RootWidget.AddChildLast(ImageBox_1);
            this.RootWidget.AddChildLast(PanelPortrait);
            this.Transition = new PushTransition();

            // PanelTitle
            PanelTitle.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 255f / 255f);
            PanelTitle.Clip = true;
            PanelTitle.AddChildLast(LabelTitle);

            // LabelTitle
            LabelTitle.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            LabelTitle.Font = new UIFont(FontAlias.System, 40, FontStyle.Regular);
            LabelTitle.LineBreak = LineBreak.Character;

            // ButtonBack
            ButtonBack.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonBack.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonDummy1
            ButtonDummy1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonDummy1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonDummy2
            ButtonDummy2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonDummy2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/photo01.png");

            // PanelPortrait
            PanelPortrait.BackgroundColor = new UIColor(56f / 255f, 187f / 255f, 239f / 255f, 255f / 255f);
            PanelPortrait.Clip = true;
            PanelPortrait.AddChildLast(Label_1);
            PanelPortrait.AddChildLast(ButtonDummy3);
            PanelPortrait.AddChildLast(ButtonDummy4);

            // Label_1
            Label_1.TextColor = new UIColor(8f / 255f, 23f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // ButtonDummy3
            ButtonDummy3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonDummy3.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonDummy4
            ButtonDummy4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonDummy4.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.DesignWidth = 544;
                    this.DesignHeight = 960;

                    PanelTitle.SetPosition(0, 0);
                    PanelTitle.SetSize(544, 100);
                    PanelTitle.Anchors = Anchors.None;
                    PanelTitle.Visible = true;

                    LabelTitle.SetPosition(21, 16);
                    LabelTitle.SetSize(474, 68);
                    LabelTitle.Anchors = Anchors.None;
                    LabelTitle.Visible = true;

                    ButtonBack.SetPosition(301, 857);
                    ButtonBack.SetSize(214, 56);
                    ButtonBack.Anchors = Anchors.None;
                    ButtonBack.Visible = true;

                    ButtonDummy1.SetPosition(53, 182);
                    ButtonDummy1.SetSize(214, 56);
                    ButtonDummy1.Anchors = Anchors.None;
                    ButtonDummy1.Visible = true;

                    ButtonDummy2.SetPosition(301, 182);
                    ButtonDummy2.SetSize(214, 56);
                    ButtonDummy2.Anchors = Anchors.None;
                    ButtonDummy2.Visible = true;

                    ImageBox_1.SetPosition(42, 276);
                    ImageBox_1.SetSize(464, 339);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    PanelPortrait.SetPosition(21, 658);
                    PanelPortrait.SetSize(505, 138);
                    PanelPortrait.Anchors = Anchors.None;
                    PanelPortrait.Visible = true;

                    Label_1.SetPosition(11, 10);
                    Label_1.SetSize(260, 48);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    ButtonDummy3.SetPosition(31, 66);
                    ButtonDummy3.SetSize(214, 56);
                    ButtonDummy3.Anchors = Anchors.None;
                    ButtonDummy3.Visible = true;

                    ButtonDummy4.SetPosition(270, 66);
                    ButtonDummy4.SetSize(214, 56);
                    ButtonDummy4.Anchors = Anchors.None;
                    ButtonDummy4.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    PanelTitle.SetPosition(0, 0);
                    PanelTitle.SetSize(960, 100);
                    PanelTitle.Anchors = Anchors.None;
                    PanelTitle.Visible = true;

                    LabelTitle.SetPosition(21, 11);
                    LabelTitle.SetSize(920, 77);
                    LabelTitle.Anchors = Anchors.None;
                    LabelTitle.Visible = true;

                    ButtonBack.SetPosition(712, 445);
                    ButtonBack.SetSize(214, 56);
                    ButtonBack.Anchors = Anchors.None;
                    ButtonBack.Visible = true;

                    ButtonDummy1.SetPosition(86, 172);
                    ButtonDummy1.SetSize(214, 56);
                    ButtonDummy1.Anchors = Anchors.None;
                    ButtonDummy1.Visible = true;

                    ButtonDummy2.SetPosition(86, 290);
                    ButtonDummy2.SetSize(214, 56);
                    ButtonDummy2.Anchors = Anchors.None;
                    ButtonDummy2.Visible = true;

                    ImageBox_1.SetPosition(447, 135);
                    ImageBox_1.SetSize(429, 286);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    PanelPortrait.SetPosition(62, 420);
                    PanelPortrait.SetSize(345, 106);
                    PanelPortrait.Anchors = Anchors.None;
                    PanelPortrait.Visible = false;

                    Label_1.SetPosition(22, 12);
                    Label_1.SetSize(244, 33);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    ButtonDummy3.SetPosition(22, 45);
                    ButtonDummy3.SetSize(131, 54);
                    ButtonDummy3.Anchors = Anchors.None;
                    ButtonDummy3.Visible = true;

                    ButtonDummy4.SetPosition(184, 45);
                    ButtonDummy4.SetSize(131, 54);
                    ButtonDummy4.Anchors = Anchors.None;
                    ButtonDummy4.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            LabelTitle.Text = "Title";

            ButtonBack.Text = "Back";

            ButtonDummy1.Text = "Dummy 1";

            ButtonDummy2.Text = "Dummy 2";

            Label_1.Text = "Portrait only";

            ButtonDummy3.Text = "Dummy 3";

            ButtonDummy4.Text = "Dummy 4";
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
