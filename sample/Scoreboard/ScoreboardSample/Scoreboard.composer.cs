// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScoreboardSample
{
    partial class Scoreboard
    {
        Panel sceneBackgroundPanel;
        Button setScore;
        EditableText scoreText;
        ListPanel listPanel;
        PopupList typePopup;
        Label Label_1;
        Label Label_2;
        Label Label_3;
        Label connectionText;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            sceneBackgroundPanel = new Panel();
            sceneBackgroundPanel.Name = "sceneBackgroundPanel";
            setScore = new Button();
            setScore.Name = "setScore";
            scoreText = new EditableText();
            scoreText.Name = "scoreText";
            listPanel = new ListPanel();
            listPanel.Name = "listPanel";
            typePopup = new PopupList();
            typePopup.Name = "typePopup";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            connectionText = new Label();
            connectionText.Name = "connectionText";

            // sceneBackgroundPanel
            sceneBackgroundPanel.BackgroundColor = new UIColor(128f / 255f, 152f / 255f, 180f / 255f, 255f / 255f);

            // setScore
            setScore.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            setScore.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // scoreText
            scoreText.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            scoreText.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            scoreText.LineBreak = LineBreak.Character;
            scoreText.HorizontalAlignment = HorizontalAlignment.Right;

            // listPanel
            listPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            listPanel.SetListItemCreator(ScoreItem.Creator);

            // typePopup
            typePopup.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            typePopup.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // Label_1
            Label_1.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_3.LineBreak = LineBreak.Character;

            // connectionText
            connectionText.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            connectionText.Font = new UIFont(FontAlias.System, 15, FontStyle.Regular);
            connectionText.LineBreak = LineBreak.Character;

            // Scoreboard
            this.RootWidget.AddChildLast(sceneBackgroundPanel);
            this.RootWidget.AddChildLast(setScore);
            this.RootWidget.AddChildLast(scoreText);
            this.RootWidget.AddChildLast(listPanel);
            this.RootWidget.AddChildLast(typePopup);
            this.RootWidget.AddChildLast(Label_1);
            this.RootWidget.AddChildLast(Label_2);
            this.RootWidget.AddChildLast(Label_3);
            this.RootWidget.AddChildLast(connectionText);

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

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(544, 960);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    setScore.SetPosition(240, 86);
                    setScore.SetSize(214, 56);
                    setScore.Anchors = Anchors.None;
                    setScore.Visible = true;

                    scoreText.SetPosition(56, 42);
                    scoreText.SetSize(360, 56);
                    scoreText.Anchors = Anchors.None;
                    scoreText.Visible = true;

                    listPanel.SetPosition(40, 110);
                    listPanel.SetSize(854, 400);
                    listPanel.Anchors = Anchors.None;
                    listPanel.Visible = true;

                    typePopup.SetPosition(586, 25);
                    typePopup.SetSize(360, 56);
                    typePopup.Anchors = Anchors.Height;
                    typePopup.Visible = true;

                    Label_1.SetPosition(7, 7);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(7, 7);
                    Label_2.SetSize(214, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(7, 7);
                    Label_3.SetSize(214, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    connectionText.SetPosition(-70, 501);
                    connectionText.SetSize(214, 36);
                    connectionText.Anchors = Anchors.None;
                    connectionText.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    sceneBackgroundPanel.SetPosition(0, 0);
                    sceneBackgroundPanel.SetSize(960, 544);
                    sceneBackgroundPanel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    sceneBackgroundPanel.Visible = true;

                    setScore.SetPosition(229, 16);
                    setScore.SetSize(214, 56);
                    setScore.Anchors = Anchors.None;
                    setScore.Visible = true;

                    scoreText.SetPosition(21, 16);
                    scoreText.SetSize(200, 56);
                    scoreText.Anchors = Anchors.None;
                    scoreText.Visible = true;

                    listPanel.SetPosition(21, 134);
                    listPanel.SetSize(917, 379);
                    listPanel.Anchors = Anchors.None;
                    listPanel.Visible = true;

                    typePopup.SetPosition(578, 16);
                    typePopup.SetSize(360, 56);
                    typePopup.Anchors = Anchors.Height;
                    typePopup.Visible = true;

                    Label_1.SetPosition(29, 95);
                    Label_1.SetSize(300, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    Label_2.SetPosition(329, 95);
                    Label_2.SetSize(300, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(630, 95);
                    Label_3.SetSize(300, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    connectionText.SetPosition(21, 513);
                    connectionText.SetSize(329, 30);
                    connectionText.Anchors = Anchors.None;
                    connectionText.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            setScore.Text = "Set Score";

            scoreText.Text = "1";

            typePopup.ListItems.Clear();
            typePopup.ListItems.AddRange(new String[]
            {
                "Friends",
                "Daily",
                "Weekly",
                "Monthly",
                "All Time",
            });
            typePopup.SelectedIndex = 4;

            Label_1.Text = "Rank";

            Label_2.Text = "Player";

            Label_3.Text = "Score";

            connectionText.Text = "Server status: not connected.";
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
