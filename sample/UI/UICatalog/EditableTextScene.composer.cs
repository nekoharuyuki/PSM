// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class EditableTextScene
    {
        Panel contentPanel;
        EditableText edit1;
        Button buttonClear;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            contentPanel = new Panel();
            contentPanel.Name = "contentPanel";
            edit1 = new EditableText();
            edit1.Name = "edit1";
            buttonClear = new Button();
            buttonClear.Name = "buttonClear";

            // EditableTextScene
            this.RootWidget.AddChildLast(contentPanel);

            // contentPanel
            contentPanel.BackgroundColor = new UIColor(153f / 255f, 153f / 255f, 153f / 255f, 0f / 255f);
            contentPanel.Clip = true;
            contentPanel.AddChildLast(edit1);
            contentPanel.AddChildLast(buttonClear);

            // edit1
            edit1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            edit1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            edit1.LineBreak = LineBreak.Character;

            // buttonClear
            buttonClear.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonClear.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    contentPanel.SetPosition(0, 209);
                    contentPanel.SetSize(480, 644);
                    contentPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentPanel.Visible = true;

                    edit1.SetPosition(80, 109);
                    edit1.SetSize(360, 56);
                    edit1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    edit1.Visible = true;

                    buttonClear.SetPosition(133, 241);
                    buttonClear.SetSize(214, 56);
                    buttonClear.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    buttonClear.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    contentPanel.SetPosition(0, 110);
                    contentPanel.SetSize(854, 370);
                    contentPanel.Anchors = Anchors.Top | Anchors.Bottom;
                    contentPanel.Visible = true;

                    edit1.SetPosition(246, 51);
                    edit1.SetSize(300, 56);
                    edit1.Anchors = Anchors.Height;
                    edit1.Visible = true;

                    buttonClear.SetPosition(246, 143);
                    buttonClear.SetSize(200, 56);
                    buttonClear.Anchors = Anchors.Height;
                    buttonClear.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            edit1.Text = "Press lower buttons.";

            buttonClear.Text = "Clear";
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
