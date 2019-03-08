// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    partial class ScoreboardDialog
    {
        Button buttonOK;
        ListPanel scoreboardListPanel;
        Label labelPosition;
        Label labelPSNID;
        Label labelScore;
        Button buttonDaily;
        Button buttonWeekly;
        Button buttonMonthly;
        BusyIndicator indicator;
        Label labelLoading;
        Button buttonAllTime;
        Button buttonFriends;
        Label nameLabel;
        Label Label_1;
        BusyIndicator busyAwaitingScores;
        Label errorLabel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            buttonOK = new Button();
            buttonOK.Name = "buttonOK";
            scoreboardListPanel = new ListPanel();
            scoreboardListPanel.Name = "scoreboardListPanel";
            labelPosition = new Label();
            labelPosition.Name = "labelPosition";
            labelPSNID = new Label();
            labelPSNID.Name = "labelPSNID";
            labelScore = new Label();
            labelScore.Name = "labelScore";
            buttonDaily = new Button();
            buttonDaily.Name = "buttonDaily";
            buttonWeekly = new Button();
            buttonWeekly.Name = "buttonWeekly";
            buttonMonthly = new Button();
            buttonMonthly.Name = "buttonMonthly";
            indicator = new BusyIndicator(true);
            indicator.Name = "indicator";
            labelLoading = new Label();
            labelLoading.Name = "labelLoading";
            buttonAllTime = new Button();
            buttonAllTime.Name = "buttonAllTime";
            buttonFriends = new Button();
            buttonFriends.Name = "buttonFriends";
            nameLabel = new Label();
            nameLabel.Name = "nameLabel";
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            busyAwaitingScores = new BusyIndicator(true);
            busyAwaitingScores.Name = "busyAwaitingScores";
            errorLabel = new Label();
            errorLabel.Name = "errorLabel";

            // buttonOK
            buttonOK.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            buttonOK.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Bold);
            buttonOK.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // scoreboardListPanel
            scoreboardListPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            scoreboardListPanel.ShowItemBorder = false;
            scoreboardListPanel.ShowSection = false;
            scoreboardListPanel.ShowEmptySection = false;
            scoreboardListPanel.SetListItemCreator(ScoreListItem.Creator);

            // labelPosition
            labelPosition.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            labelPosition.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelPosition.LineBreak = LineBreak.Character;
            labelPosition.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // labelPSNID
            labelPSNID.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            labelPSNID.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelPSNID.LineBreak = LineBreak.Character;
            labelPSNID.HorizontalAlignment = HorizontalAlignment.Center;
            labelPSNID.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // labelScore
            labelScore.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            labelScore.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelScore.LineBreak = LineBreak.Character;
            labelScore.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // buttonDaily
            buttonDaily.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonDaily.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            buttonDaily.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 178f / 255f);

            // buttonWeekly
            buttonWeekly.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonWeekly.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            buttonWeekly.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 178f / 255f);

            // buttonMonthly
            buttonMonthly.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonMonthly.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            buttonMonthly.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 178f / 255f);

            // labelLoading
            labelLoading.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            labelLoading.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            labelLoading.TextTrimming = TextTrimming.EllipsisWord;
            labelLoading.LineBreak = LineBreak.Word;
            labelLoading.HorizontalAlignment = HorizontalAlignment.Center;
            labelLoading.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // buttonAllTime
            buttonAllTime.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonAllTime.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            buttonAllTime.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 178f / 255f);

            // buttonFriends
            buttonFriends.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            buttonFriends.TextFont = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            buttonFriends.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 178f / 255f);

            // nameLabel
            nameLabel.TextColor = new UIColor(184f / 255f, 6f / 255f, 6f / 255f, 255f / 255f);
            nameLabel.Font = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            nameLabel.LineBreak = LineBreak.Character;
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // Label_1
            Label_1.TextColor = new UIColor(223f / 255f, 12f / 255f, 12f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 44, FontStyle.Bold);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;
            Label_1.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // errorLabel
            errorLabel.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            errorLabel.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            errorLabel.LineBreak = LineBreak.Character;
            errorLabel.HorizontalAlignment = HorizontalAlignment.Center;

            // ScoreboardDialog
            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = new ImageAsset("/Application/assets/ui_sa.png");
            this.CustomBackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0);
            this.CustomBackgroundColor = new UIColor(255f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(buttonOK);
            this.AddChildLast(scoreboardListPanel);
            this.AddChildLast(labelPosition);
            this.AddChildLast(labelPSNID);
            this.AddChildLast(labelScore);
            this.AddChildLast(buttonDaily);
            this.AddChildLast(buttonWeekly);
            this.AddChildLast(buttonMonthly);
            this.AddChildLast(indicator);
            this.AddChildLast(labelLoading);
            this.AddChildLast(buttonAllTime);
            this.AddChildLast(buttonFriends);
            this.AddChildLast(nameLabel);
            this.AddChildLast(Label_1);
            this.AddChildLast(busyAwaitingScores);
            this.AddChildLast(errorLabel);
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
                    this.SetPosition(1, 1);
                    this.SetSize(400, 640);
                    this.Anchors = Anchors.None;

                    buttonOK.SetPosition(144, 310);
                    buttonOK.SetSize(214, 56);
                    buttonOK.Anchors = Anchors.None;
                    buttonOK.Visible = true;

                    scoreboardListPanel.SetPosition(-8, 35);
                    scoreboardListPanel.SetSize(854, 400);
                    scoreboardListPanel.Anchors = Anchors.None;
                    scoreboardListPanel.Visible = true;

                    labelPosition.SetPosition(208, 64);
                    labelPosition.SetSize(214, 36);
                    labelPosition.Anchors = Anchors.None;
                    labelPosition.Visible = true;

                    labelPSNID.SetPosition(381, 64);
                    labelPSNID.SetSize(214, 36);
                    labelPSNID.Anchors = Anchors.None;
                    labelPSNID.Visible = true;

                    labelScore.SetPosition(605, 74);
                    labelScore.SetSize(214, 36);
                    labelScore.Anchors = Anchors.None;
                    labelScore.Visible = true;

                    buttonDaily.SetPosition(707, 119);
                    buttonDaily.SetSize(214, 56);
                    buttonDaily.Anchors = Anchors.None;
                    buttonDaily.Visible = true;

                    buttonWeekly.SetPosition(705, 203);
                    buttonWeekly.SetSize(214, 56);
                    buttonWeekly.Anchors = Anchors.None;
                    buttonWeekly.Visible = true;

                    buttonMonthly.SetPosition(704, 294);
                    buttonMonthly.SetSize(214, 56);
                    buttonMonthly.Anchors = Anchors.None;
                    buttonMonthly.Visible = true;

                    indicator.SetPosition(456, 232);
                    indicator.SetSize(48, 48);
                    indicator.Anchors = Anchors.Height | Anchors.Width;
                    indicator.Visible = true;

                    labelLoading.SetPosition(373, 300);
                    labelLoading.SetSize(214, 36);
                    labelLoading.Anchors = Anchors.None;
                    labelLoading.Visible = true;

                    buttonAllTime.SetPosition(716, 351);
                    buttonAllTime.SetSize(214, 56);
                    buttonAllTime.Anchors = Anchors.None;
                    buttonAllTime.Visible = true;

                    buttonFriends.SetPosition(676, 469);
                    buttonFriends.SetSize(214, 56);
                    buttonFriends.Anchors = Anchors.None;
                    buttonFriends.Visible = true;

                    nameLabel.SetPosition(376, 0);
                    nameLabel.SetSize(214, 36);
                    nameLabel.Anchors = Anchors.None;
                    nameLabel.Visible = true;

                    Label_1.SetPosition(365, 24);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    busyAwaitingScores.SetPosition(467, 252);
                    busyAwaitingScores.SetSize(48, 48);
                    busyAwaitingScores.Anchors = Anchors.Height | Anchors.Width;
                    busyAwaitingScores.Visible = true;

                    errorLabel.SetPosition(376, 248);
                    errorLabel.SetSize(214, 36);
                    errorLabel.Anchors = Anchors.None;
                    errorLabel.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(960, 544);
                    this.Anchors = Anchors.None;

                    buttonOK.SetPosition(417, 477);
                    buttonOK.SetSize(131, 56);
                    buttonOK.Anchors = Anchors.None;
                    buttonOK.Visible = true;

                    scoreboardListPanel.SetPosition(207, 97);
                    scoreboardListPanel.SetSize(544, 364);
                    scoreboardListPanel.Anchors = Anchors.None;
                    scoreboardListPanel.Visible = true;

                    labelPosition.SetPosition(288, 67);
                    labelPosition.SetSize(88, 36);
                    labelPosition.Anchors = Anchors.None;
                    labelPosition.Visible = true;

                    labelPSNID.SetPosition(374, 67);
                    labelPSNID.SetSize(212, 36);
                    labelPSNID.Anchors = Anchors.None;
                    labelPSNID.Visible = true;

                    labelScore.SetPosition(608, 67);
                    labelScore.SetSize(145, 36);
                    labelScore.Anchors = Anchors.None;
                    labelScore.Visible = true;

                    buttonDaily.SetPosition(759, 123);
                    buttonDaily.SetSize(86, 56);
                    buttonDaily.Anchors = Anchors.None;
                    buttonDaily.Visible = true;

                    buttonWeekly.SetPosition(754, 211);
                    buttonWeekly.SetSize(96, 56);
                    buttonWeekly.Anchors = Anchors.None;
                    buttonWeekly.Visible = true;

                    buttonMonthly.SetPosition(755, 300);
                    buttonMonthly.SetSize(102, 56);
                    buttonMonthly.Anchors = Anchors.None;
                    buttonMonthly.Visible = true;

                    indicator.SetPosition(456, 210);
                    indicator.SetSize(48, 48);
                    indicator.Anchors = Anchors.Height | Anchors.Width;
                    indicator.Visible = false;

                    labelLoading.SetPosition(290, 280);
                    labelLoading.SetSize(386, 131);
                    labelLoading.Anchors = Anchors.None;
                    labelLoading.Visible = false;

                    buttonAllTime.SetPosition(753, 388);
                    buttonAllTime.SetSize(102, 56);
                    buttonAllTime.Anchors = Anchors.None;
                    buttonAllTime.Visible = true;

                    buttonFriends.SetPosition(111, 477);
                    buttonFriends.SetSize(100, 56);
                    buttonFriends.Anchors = Anchors.None;
                    buttonFriends.Visible = true;

                    nameLabel.SetPosition(376, -3);
                    nameLabel.SetSize(214, 36);
                    nameLabel.Anchors = Anchors.None;
                    nameLabel.Visible = true;

                    Label_1.SetPosition(304, 23);
                    Label_1.SetSize(352, 56);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    busyAwaitingScores.SetPosition(456, 248);
                    busyAwaitingScores.SetSize(48, 48);
                    busyAwaitingScores.Anchors = Anchors.Height | Anchors.Width;
                    busyAwaitingScores.Visible = true;

                    errorLabel.SetPosition(317, 258);
                    errorLabel.SetSize(326, 28);
                    errorLabel.Anchors = Anchors.None;
                    errorLabel.Visible = false;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonOK.Text = "OK";

            labelPosition.Text = "Rank";

            labelPSNID.Text = "User";

            labelScore.Text = "Score";

            buttonDaily.Text = "Daily";

            buttonWeekly.Text = "Weekly";

            buttonMonthly.Text = "Monthly";

            labelLoading.Text = "Loading...";

            buttonAllTime.Text = "All Time";

            buttonFriends.Text = "Friends";

            Label_1.Text = "Scoreboard";

            errorLabel.Text = "Scoreboard is unavailable.";
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
