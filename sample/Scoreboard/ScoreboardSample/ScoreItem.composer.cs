// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScoreboardSample
{
    partial class ScoreItem
    {
        Label rank;
        Label name;
        Label score;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            rank = new Label();
            rank.Name = "rank";
            name = new Label();
            name.Name = "name";
            score = new Label();
            score.Name = "score";

            // rank
            rank.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            rank.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            rank.LineBreak = LineBreak.Character;

            // name
            name.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            name.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            name.LineBreak = LineBreak.Character;

            // score
            score.TextColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 255f / 255f);
            score.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            score.LineBreak = LineBreak.Character;

            // ScoreItem
            this.BackgroundColor = new UIColor(196f / 255f, 195f / 255f, 197f / 255f, 127f / 255f);
            this.AddChildLast(rank);
            this.AddChildLast(name);
            this.AddChildLast(score);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(50, 960);
                    this.Anchors = Anchors.None;

                    rank.SetPosition(7, 7);
                    rank.SetSize(214, 36);
                    rank.Anchors = Anchors.None;
                    rank.Visible = true;

                    name.SetPosition(281, 7);
                    name.SetSize(214, 36);
                    name.Anchors = Anchors.None;
                    name.Visible = true;

                    score.SetPosition(620, 7);
                    score.SetSize(214, 36);
                    score.Anchors = Anchors.None;
                    score.Visible = true;

                    break;

                default:
                    this.SetSize(917, 50);
                    this.Anchors = Anchors.None;

                    rank.SetPosition(8, 7);
                    rank.SetSize(300, 36);
                    rank.Anchors = Anchors.None;
                    rank.Visible = true;

                    name.SetPosition(308, 7);
                    name.SetSize(300, 36);
                    name.Anchors = Anchors.None;
                    name.Visible = true;

                    score.SetPosition(608, 8);
                    score.SetSize(300, 34);
                    score.Anchors = Anchors.None;
                    score.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
        }

        public void InitializeDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public void StartDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                    break;

                default:
                    break;
            }
        }

        public static ListPanelItem Creator()
        {
            return new ScoreItem();
        }

    }
}
