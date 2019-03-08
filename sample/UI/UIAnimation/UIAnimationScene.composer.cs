// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UIAnimationSample
{
    partial class UIAnimationScene
    {
        Panel bgPanel;
        PopupList fileSelectPopupList;
        Button pauseButton;
        Button playButton;
        Label repeatLabel;
        CheckBox repeatCheckBox;
        Panel repeatGroup;
        Panel controlePanel;
        Panel contentsPanel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            bgPanel = new Panel();
            bgPanel.Name = "bgPanel";
            fileSelectPopupList = new PopupList();
            fileSelectPopupList.Name = "fileSelectPopupList";
            pauseButton = new Button();
            pauseButton.Name = "pauseButton";
            playButton = new Button();
            playButton.Name = "playButton";
            repeatLabel = new Label();
            repeatLabel.Name = "repeatLabel";
            repeatCheckBox = new CheckBox();
            repeatCheckBox.Name = "repeatCheckBox";
            repeatGroup = new Panel();
            repeatGroup.Name = "repeatGroup";
            controlePanel = new Panel();
            controlePanel.Name = "controlePanel";
            contentsPanel = new Panel();
            contentsPanel.Name = "contentsPanel";

            // bgPanel
            bgPanel.BackgroundColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            bgPanel.Clip = true;

            // fileSelectPopupList
            fileSelectPopupList.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            fileSelectPopupList.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            fileSelectPopupList.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            fileSelectPopupList.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            fileSelectPopupList.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            fileSelectPopupList.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // pauseButton
            pauseButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            pauseButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // playButton
            playButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            playButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // repeatLabel
            repeatLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            repeatLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            repeatLabel.LineBreak = LineBreak.Character;

            // repeatGroup
            repeatGroup.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            repeatGroup.Clip = true;
            repeatGroup.AddChildLast(repeatLabel);
            repeatGroup.AddChildLast(repeatCheckBox);

            // controlePanel
            controlePanel.BackgroundColor = new UIColor(34f / 255f, 34f / 255f, 34f / 255f, 153f / 255f);
            controlePanel.Clip = true;
            controlePanel.AddChildLast(fileSelectPopupList);
            controlePanel.AddChildLast(pauseButton);
            controlePanel.AddChildLast(playButton);
            controlePanel.AddChildLast(repeatGroup);

            // contentsPanel
            contentsPanel.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            contentsPanel.Clip = true;

            // UIAnimationScene
            this.RootWidget.AddChildLast(bgPanel);
            this.RootWidget.AddChildLast(controlePanel);
            this.RootWidget.AddChildLast(contentsPanel);

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

                    bgPanel.SetPosition(215, 133);
                    bgPanel.SetSize(100, 100);
                    bgPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    bgPanel.Visible = true;

                    fileSelectPopupList.SetPosition(-27, 394);
                    fileSelectPopupList.SetSize(360, 56);
                    fileSelectPopupList.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    fileSelectPopupList.Visible = true;

                    pauseButton.SetPosition(146, 381);
                    pauseButton.SetSize(214, 56);
                    pauseButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    pauseButton.Visible = true;

                    playButton.SetPosition(146, 381);
                    playButton.SetSize(214, 56);
                    playButton.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    playButton.Visible = true;

                    repeatLabel.SetPosition(547, 401);
                    repeatLabel.SetSize(214, 36);
                    repeatLabel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    repeatLabel.Visible = true;

                    repeatCheckBox.SetPosition(549, 410);
                    repeatCheckBox.SetSize(56, 56);
                    repeatCheckBox.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    repeatCheckBox.Visible = true;

                    repeatGroup.SetPosition(419, 203);
                    repeatGroup.SetSize(100, 100);
                    repeatGroup.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    repeatGroup.Visible = true;

                    controlePanel.SetPosition(355, 270);
                    controlePanel.SetSize(100, 100);
                    controlePanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    controlePanel.Visible = true;

                    contentsPanel.SetPosition(91, 99);
                    contentsPanel.SetSize(100, 100);
                    contentsPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    contentsPanel.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    bgPanel.SetPosition(0, 0);
                    bgPanel.SetSize(854, 480);
                    bgPanel.Anchors = Anchors.None;
                    bgPanel.Visible = true;

                    fileSelectPopupList.SetPosition(20, 22);
                    fileSelectPopupList.SetSize(320, 56);
                    fileSelectPopupList.Anchors = Anchors.Height;
                    fileSelectPopupList.Visible = true;

                    pauseButton.SetPosition(519, 22);
                    pauseButton.SetSize(142, 56);
                    pauseButton.Anchors = Anchors.Height;
                    pauseButton.Visible = true;

                    playButton.SetPosition(360, 22);
                    playButton.SetSize(142, 56);
                    playButton.Anchors = Anchors.Height;
                    playButton.Visible = true;

                    repeatLabel.SetPosition(56, 22);
                    repeatLabel.SetSize(100, 58);
                    repeatLabel.Anchors = Anchors.Height | Anchors.Left | Anchors.Width;
                    repeatLabel.Visible = true;

                    repeatCheckBox.SetPosition(0, 22);
                    repeatCheckBox.SetSize(56, 56);
                    repeatCheckBox.Anchors = Anchors.Height | Anchors.Left | Anchors.Width;
                    repeatCheckBox.Visible = true;

                    repeatGroup.SetPosition(683, 0);
                    repeatGroup.SetSize(156, 101);
                    repeatGroup.Anchors = Anchors.None;
                    repeatGroup.Visible = true;

                    controlePanel.SetPosition(0, 0);
                    controlePanel.SetSize(854, 101);
                    controlePanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Right;
                    controlePanel.Visible = true;

                    contentsPanel.SetPosition(0, 100);
                    contentsPanel.SetSize(854, 379);
                    contentsPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    contentsPanel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            fileSelectPopupList.ListTitle = "Select animation / motion file";
            fileSelectPopupList.ListItems.Clear();
            fileSelectPopupList.ListItems.AddRange(new String[]
            {
                "UIASample.uia",
                "UIMSample.uim",
            });
            fileSelectPopupList.SelectedIndex = 0;

            pauseButton.Text = "Pause";

            playButton.Text = "Play";

            repeatLabel.Text = "Repeat";
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
