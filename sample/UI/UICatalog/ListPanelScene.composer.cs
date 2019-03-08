// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class ListPanelScene
    {
        Panel contentPanel;
        ListPanel listPanel;
        Label Label_SelectItemTitle;
        Label Label_SelectItem;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            listPanel = new ListPanel();
            listPanel.Name = "listPanel";
            Label_SelectItemTitle = new Label();
            Label_SelectItemTitle.Name = "Label_SelectItemTitle";
            Label_SelectItem = new Label();
            Label_SelectItem.Name = "Label_SelectItem";

            // ListPanelScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(listPanel);
            contentPanel.AddChildLast(Label_SelectItemTitle);
            contentPanel.AddChildLast(Label_SelectItem);

            // listPanel
            listPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;

            // Label_SelectItemTitle
            Label_SelectItemTitle.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_SelectItemTitle.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_SelectItemTitle.LineBreak = LineBreak.Word;
            Label_SelectItemTitle.VerticalAlignment = VerticalAlignment.Top;

            // Label_SelectItem
            Label_SelectItem.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_SelectItem.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            Label_SelectItem.LineBreak = LineBreak.Character;
            Label_SelectItem.VerticalAlignment = VerticalAlignment.Top;

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

                    contentPanel.SetPosition(0, 226);
                    contentPanel.SetSize(480, 628);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    listPanel.SetPosition(0, 29);
                    listPanel.SetSize(480, 583);
                    listPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    listPanel.Visible = true;

                    Label_SelectItemTitle.SetPosition(640, 334);
                    Label_SelectItemTitle.SetSize(214, 36);
                    Label_SelectItemTitle.Anchors = Anchors.None;
                    Label_SelectItemTitle.Visible = true;

                    Label_SelectItem.SetPosition(640, 334);
                    Label_SelectItem.SetSize(214, 36);
                    Label_SelectItem.Anchors = Anchors.None;
                    Label_SelectItem.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    listPanel.SetPosition(227, 1);
                    listPanel.SetSize(400, 370);
                    listPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    listPanel.Visible = true;

                    Label_SelectItemTitle.SetPosition(634, 214);
                    Label_SelectItemTitle.SetSize(166, 34);
                    Label_SelectItemTitle.Anchors = Anchors.None;
                    Label_SelectItemTitle.Visible = true;

                    Label_SelectItem.SetPosition(649, 248);
                    Label_SelectItem.SetSize(192, 113);
                    Label_SelectItem.Anchors = Anchors.None;
                    Label_SelectItem.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_SelectItemTitle.Text = "SelectItem:";
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
