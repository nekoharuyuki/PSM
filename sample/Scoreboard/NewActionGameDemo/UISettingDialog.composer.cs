// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    partial class UISettingDialog
    {
        Button buttonOK;
        public ListPanel listPanel;
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
        Label labelPlayerName;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            buttonOK = new Button();
            buttonOK.Name = "buttonOK";
            listPanel = new ListPanel();
            listPanel.Name = "listPanel";
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
            labelPlayerName = new Label();
            labelPlayerName.Name = "labelPlayerName";

            // buttonOK
            buttonOK.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            buttonOK.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Bold);
            buttonOK.BackgroundFilterColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

            // listPanel
            listPanel.ScrollBarVisibility = ScrollBarVisibility.ScrollableVisible;
            listPanel.ShowItemBorder = false;
            listPanel.ShowSection = false;
            listPanel.ShowEmptySection = false;

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

            // labelPlayerName
            labelPlayerName.TextColor = new UIColor(184f / 255f, 6f / 255f, 6f / 255f, 255f / 255f);
            labelPlayerName.Font = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            labelPlayerName.LineBreak = LineBreak.Character;
            labelPlayerName.HorizontalAlignment = HorizontalAlignment.Center;
            labelPlayerName.TextShadow = new TextShadowSettings()
            {
                Color = new UIColor(128f / 255f, 128f / 255f, 128f / 255f, 127f / 255f),
                HorizontalOffset = 2f,
                VerticalOffset = 2f,
            };

            // UISettingDialog
            this.BackgroundStyle = DialogBackgroundStyle.Custom;
            this.CustomBackgroundImage = new ImageAsset("/Application/assets/ui_sa.png");
            this.CustomBackgroundNinePatchMargin = new NinePatchMargin(0, 0, 0, 0);
            this.CustomBackgroundColor = new UIColor(255f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(buttonOK);
            this.AddChildLast(listPanel);
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
            this.AddChildLast(labelPlayerName);
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

                    listPanel.SetPosition(-8, 35);
                    listPanel.SetSize(854, 400);
                    listPanel.Anchors = Anchors.None;
                    listPanel.Visible = true;

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

                    labelPlayerName.SetPosition(376, 0);
                    labelPlayerName.SetSize(214, 36);
                    labelPlayerName.Anchors = Anchors.None;
                    labelPlayerName.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(960, 544);
                    this.Anchors = Anchors.None;

                    buttonOK.SetPosition(417, 477);
                    buttonOK.SetSize(131, 56);
                    buttonOK.Anchors = Anchors.None;
                    buttonOK.Visible = true;

                    listPanel.SetPosition(207, 97);
                    listPanel.SetSize(544, 364);
                    listPanel.Anchors = Anchors.None;
                    listPanel.Visible = true;

                    labelPosition.SetPosition(290, 67);
                    labelPosition.SetSize(110, 36);
                    labelPosition.Anchors = Anchors.None;
                    labelPosition.Visible = true;

                    labelPSNID.SetPosition(400, 67);
                    labelPSNID.SetSize(198, 36);
                    labelPSNID.Anchors = Anchors.None;
                    labelPSNID.Visible = true;

                    labelScore.SetPosition(606, 67);
                    labelScore.SetSize(146, 36);
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

                    labelPlayerName.SetPosition(376, -3);
                    labelPlayerName.SetSize(214, 36);
                    labelPlayerName.Anchors = Anchors.None;
                    labelPlayerName.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            buttonOK.Text = "OK";

            labelPosition.Text = "Position";

            labelPSNID.Text = "PSN ID";

            labelScore.Text = "Score";

            buttonDaily.Text = "Daily";

            buttonWeekly.Text = "Weekly";

            buttonMonthly.Text = "Monthly";

            labelLoading.Text = "Loading...";

            buttonAllTime.Text = "All Time";

            buttonFriends.Text = "Friends";
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
