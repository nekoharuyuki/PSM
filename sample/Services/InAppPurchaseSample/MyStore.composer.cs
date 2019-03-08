// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace Sample
{
    partial class MyStore
    {
        Label Label_1;
        Label Label_2;
        Label Label_3;
        Label Label_4;
        Label Label_5;
        ListPanel ListPanel_1;
        Button Button_1;
        Button Button_2;
        Button Button_3;
        Button Button_4;
        Panel Panel_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Label_4 = new Label();
            Label_4.Name = "Label_4";
            Label_5 = new Label();
            Label_5.Name = "Label_5";
            ListPanel_1 = new ListPanel();
            ListPanel_1.Name = "ListPanel_1";
            Button_1 = new Button();
            Button_1.Name = "Button_1";
            Button_2 = new Button();
            Button_2.Name = "Button_2";
            Button_3 = new Button();
            Button_3.Name = "Button_3";
            Button_4 = new Button();
            Button_4.Name = "Button_4";
            Panel_1 = new Panel();
            Panel_1.Name = "Panel_1";

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;

            // Label_4
            Label_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_4.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_4.LineBreak = LineBreak.Character;
            Label_4.HorizontalAlignment = HorizontalAlignment.Right;

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_5.LineBreak = LineBreak.Character;
            Label_5.HorizontalAlignment = HorizontalAlignment.Right;

            // ListPanel_1
            ListPanel_1.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            ListPanel_1.ShowSection = false;
            ListPanel_1.ShowEmptySection = false;
            ListPanel_1.SetListItemCreator(MyStoreItem.Creator);

            // Button_1
            Button_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_1.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Button_2
            Button_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_2.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Button_3
            Button_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_3.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Button_4
            Button_4.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Button_4.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Panel_1
            Panel_1.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            Panel_1.Clip = true;
            Panel_1.AddChildLast(Label_2);
            Panel_1.AddChildLast(Label_3);
            Panel_1.AddChildLast(Label_4);
            Panel_1.AddChildLast(Label_5);
            Panel_1.AddChildLast(ListPanel_1);
            Panel_1.AddChildLast(Button_1);
            Panel_1.AddChildLast(Button_2);
            Panel_1.AddChildLast(Button_3);
            Panel_1.AddChildLast(Button_4);

            // MyStore
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(Panel_1);

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

                    Label_1.SetPosition(17, 18);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    Label_2.SetPosition(100, 80);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    Label_3.SetPosition(277, 78);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_3.Visible = true;

                    Label_4.SetPosition(500, 78);
                    Label_4.SetSize(214, 36);
                    Label_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_4.Visible = true;

                    Label_5.SetPosition(500, 78);
                    Label_5.SetSize(214, 36);
                    Label_5.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_5.Visible = true;

                    ListPanel_1.SetPosition(40, 120);
                    ListPanel_1.SetSize(854, 400);
                    ListPanel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    ListPanel_1.Visible = true;

                    Button_1.SetPosition(40, 384);
                    Button_1.SetSize(214, 56);
                    Button_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_1.Visible = true;

                    Button_2.SetPosition(280, 384);
                    Button_2.SetSize(214, 56);
                    Button_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_2.Visible = true;

                    Button_3.SetPosition(520, 384);
                    Button_3.SetSize(214, 56);
                    Button_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_3.Visible = true;

                    Button_4.SetPosition(520, 384);
                    Button_4.SetSize(214, 56);
                    Button_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Button_4.Visible = true;

                    Panel_1.SetPosition(227, 29);
                    Panel_1.SetSize(100, 100);
                    Panel_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Panel_1.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    Label_1.SetPosition(20, 20);
                    Label_1.SetSize(320, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height;
                    Label_1.Visible = true;

                    Label_2.SetPosition(114, 0);
                    Label_2.SetSize(152, 36);
                    Label_2.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_2.Visible = true;

                    Label_3.SetPosition(268, 0);
                    Label_3.SetSize(203, 36);
                    Label_3.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Right;
                    Label_3.Visible = true;

                    Label_4.SetPosition(473, 0);
                    Label_4.SetSize(164, 36);
                    Label_4.Anchors = Anchors.Top | Anchors.Height | Anchors.Right | Anchors.Width;
                    Label_4.Visible = true;

                    Label_5.SetPosition(639, 0);
                    Label_5.SetSize(101, 36);
                    Label_5.Anchors = Anchors.Top | Anchors.Height | Anchors.Right | Anchors.Width;
                    Label_5.Visible = true;

                    ListPanel_1.SetPosition(0, 40);
                    ListPanel_1.SetSize(780, 264);
                    ListPanel_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    ListPanel_1.Visible = true;

                    Button_1.SetPosition(0, 316);
                    Button_1.SetSize(192, 56);
                    Button_1.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_1.Visible = true;

                    Button_2.SetPosition(196, 316);
                    Button_2.SetSize(192, 56);
                    Button_2.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_2.Visible = true;

                    Button_3.SetPosition(392, 316);
                    Button_3.SetSize(192, 56);
                    Button_3.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_3.Visible = true;

                    Button_4.SetPosition(588, 316);
                    Button_4.SetSize(192, 56);
                    Button_4.Anchors = Anchors.Bottom | Anchors.Height;
                    Button_4.Visible = true;

                    Panel_1.SetPosition(35, 67);
                    Panel_1.SetSize(780, 372);
                    Panel_1.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    Panel_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "InAppPurchaseSample";

            Label_2.Text = "Label";

            Label_3.Text = "Name";

            Label_4.Text = "Price";

            Label_5.Text = "Ticket";

            Button_1.Text = "GetProductInfo";

            Button_2.Text = "GetTicketInfo";

            Button_3.Text = "Purchase";

            Button_4.Text = "Consume";
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
