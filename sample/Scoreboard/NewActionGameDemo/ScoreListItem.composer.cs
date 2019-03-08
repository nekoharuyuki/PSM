// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    partial class ScoreListItem
    {
        ImageBox ImageBox_1;
        Label rank;
        Label score;
        Label name;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ImageBox_1 = new ImageBox();
            ImageBox_1.Name = "ImageBox_1";
            rank = new Label();
            rank.Name = "rank";
            score = new Label();
            score.Name = "score";
            name = new Label();
            name.Name = "name";

            // ImageBox_1
            ImageBox_1.Image = new ImageAsset("/Application/assets/ui_panel.png");

            // rank
            rank.TextColor = new UIColor(0f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            rank.Font = new UIFont(FontAlias.System, 20, FontStyle.Bold);
            rank.LineBreak = LineBreak.Character;

            // score
            score.TextColor = new UIColor(0f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            score.Font = new UIFont(FontAlias.System, 20, FontStyle.Bold);
            score.LineBreak = LineBreak.Character;
            score.HorizontalAlignment = HorizontalAlignment.Right;

            // name
            name.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
            name.Font = new UIFont(FontAlias.System, 20, FontStyle.Bold);
            name.LineBreak = LineBreak.Character;
            name.HorizontalAlignment = HorizontalAlignment.Center;

            // ScoreListItem
            this.BackgroundColor = new UIColor(0f / 255f, 0f / 255f, 0f / 255f, 0f / 255f);
            this.AddChildLast(ImageBox_1);
            this.AddChildLast(rank);
            this.AddChildLast(score);
            this.AddChildLast(name);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(544, 960);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(51, -50);
                    ImageBox_1.SetSize(200, 200);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    rank.SetPosition(0, 12);
                    rank.SetSize(214, 36);
                    rank.Anchors = Anchors.None;
                    rank.Visible = true;

                    score.SetPosition(155, 19);
                    score.SetSize(214, 36);
                    score.Anchors = Anchors.None;
                    score.Visible = true;

                    name.SetPosition(306, 19);
                    name.SetSize(214, 36);
                    name.Anchors = Anchors.None;
                    name.Visible = true;

                    break;

                default:
                    this.SetSize(544, 75);
                    this.Anchors = Anchors.None;

                    ImageBox_1.SetPosition(0, 0);
                    ImageBox_1.SetSize(544, 75);
                    ImageBox_1.Anchors = Anchors.None;
                    ImageBox_1.Visible = true;

                    rank.SetPosition(84, 19);
                    rank.SetSize(60, 36);
                    rank.Anchors = Anchors.None;
                    rank.Visible = true;

                    score.SetPosition(382, 19);
                    score.SetSize(84, 36);
                    score.Anchors = Anchors.None;
                    score.Visible = true;

                    name.SetPosition(174, 19);
                    name.SetSize(196, 36);
                    name.Anchors = Anchors.None;
                    name.Visible = true;

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
            return new ScoreListItem();
        }

    }
}
