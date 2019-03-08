// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace RssApp
{
    partial class AddFeedDialog
    {
        Label addFeedTitleLabel;
        EditableText addFeedEditableText;
        Button addFeedOkButton;
        Button addFeedCancelButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            addFeedTitleLabel = new Label();
            addFeedTitleLabel.Name = "addFeedTitleLabel";
            addFeedEditableText = new EditableText();
            addFeedEditableText.Name = "addFeedEditableText";
            addFeedOkButton = new Button();
            addFeedOkButton.Name = "addFeedOkButton";
            addFeedCancelButton = new Button();
            addFeedCancelButton.Name = "addFeedCancelButton";

            // addFeedTitleLabel
            addFeedTitleLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            addFeedTitleLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            addFeedTitleLabel.LineBreak = LineBreak.Character;

            // addFeedEditableText
            addFeedEditableText.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            addFeedEditableText.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            addFeedEditableText.LineBreak = LineBreak.Character;

            // addFeedOkButton
            addFeedOkButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            addFeedOkButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // addFeedCancelButton
            addFeedCancelButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            addFeedCancelButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // AddFeedDialog
            this.AddChildLast(addFeedTitleLabel);
            this.AddChildLast(addFeedEditableText);
            this.AddChildLast(addFeedOkButton);
            this.AddChildLast(addFeedCancelButton);
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
                    this.SetSize(300, 700);
                    this.Anchors = Anchors.None;

                    addFeedTitleLabel.SetPosition(13, 6);
                    addFeedTitleLabel.SetSize(214, 36);
                    addFeedTitleLabel.Anchors = Anchors.None;
                    addFeedTitleLabel.Visible = true;

                    addFeedEditableText.SetPosition(20, 67);
                    addFeedEditableText.SetSize(360, 56);
                    addFeedEditableText.Anchors = Anchors.None;
                    addFeedEditableText.Visible = true;

                    addFeedOkButton.SetPosition(336, 220);
                    addFeedOkButton.SetSize(214, 56);
                    addFeedOkButton.Anchors = Anchors.None;
                    addFeedOkButton.Visible = true;

                    addFeedCancelButton.SetPosition(448, 220);
                    addFeedCancelButton.SetSize(214, 56);
                    addFeedCancelButton.Anchors = Anchors.None;
                    addFeedCancelButton.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(700, 250);
                    this.Anchors = Anchors.Height | Anchors.Width;

                    addFeedTitleLabel.SetPosition(20, 20);
                    addFeedTitleLabel.SetSize(400, 30);
                    addFeedTitleLabel.Anchors = Anchors.None;
                    addFeedTitleLabel.Visible = true;

                    addFeedEditableText.SetPosition(20, 74);
                    addFeedEditableText.SetSize(660, 60);
                    addFeedEditableText.Anchors = Anchors.None;
                    addFeedEditableText.Visible = true;

                    addFeedOkButton.SetPosition(112, 168);
                    addFeedOkButton.SetSize(229, 56);
                    addFeedOkButton.Anchors = Anchors.None;
                    addFeedOkButton.Visible = true;

                    addFeedCancelButton.SetPosition(357, 168);
                    addFeedCancelButton.SetSize(229, 56);
                    addFeedCancelButton.Anchors = Anchors.None;
                    addFeedCancelButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            addFeedTitleLabel.Text = "Add new feed.";

            addFeedEditableText.DefaultText = "Please input new feed URL.";

            addFeedOkButton.Text = "OK";

            addFeedCancelButton.Text = "Cancel";
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
