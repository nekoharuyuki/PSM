// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class PopupListScene
    {
        Panel contentPanel;
        PopupList popupList;
        Label label;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            popupList = new PopupList();
            popupList.Name = "popupList";
            label = new Label();
            label.Name = "label";

            // PopupListScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(popupList);
            contentPanel.AddChildLast(label);

            // popupList
            popupList.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupList.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupList.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupList.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            popupList.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            popupList.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // label
            label.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            label.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            label.LineBreak = LineBreak.Character;
            label.VerticalAlignment = VerticalAlignment.Bottom;

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

                    contentPanel.SetPosition(6, 224);
                    contentPanel.SetSize(474, 638);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    popupList.SetPosition(77, 267);
                    popupList.SetSize(360, 56);
                    popupList.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    popupList.Visible = true;

                    label.SetPosition(77, 124);
                    label.SetSize(214, 36);
                    label.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    label.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    popupList.SetPosition(246, 114);
                    popupList.SetSize(300, 56);
                    popupList.Anchors = Anchors.Height;
                    popupList.Visible = true;

                    label.SetPosition(247, 51);
                    label.SetSize(300, 36);
                    label.Anchors = Anchors.Height;
                    label.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            popupList.ListTitle = "Language";
            popupList.ListItems.Clear();
            popupList.ListItems.AddRange(new String[]
            {
                "Arabic",
                "中国語",
                "English",
                "French",
                "German",
                "Italian",
                "日本語",
                "韓国語",
                "Russian",
                "Swedish",
            });
            popupList.SelectedIndex = 2;

            label.Text = "Select language";
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
