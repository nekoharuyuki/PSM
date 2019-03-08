// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class NavigationPanel
    {
        Panel backgroundPanel;
        Panel headerPanel;
        Label titleLabel;
        Panel controlPanel;
        Button transitionSettingButton;
        Button previousButton;
        Button topButton;
        Button nextButton;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            backgroundPanel = new Panel();
            backgroundPanel.Name = "backgroundPanel";
            headerPanel = new Panel();
            headerPanel.Name = "headerPanel";
            titleLabel = new Label();
            titleLabel.Name = "titleLabel";
            controlPanel = new Panel();
            controlPanel.Name = "controlPanel";
            transitionSettingButton = new Button();
            transitionSettingButton.Name = "transitionSettingButton";
            previousButton = new Button();
            previousButton.Name = "previousButton";
            topButton = new Button();
            topButton.Name = "topButton";
            nextButton = new Button();
            nextButton.Name = "nextButton";

            // NavigationPanel
            this.BackgroundColor = new UIColor(81f / 255f, 81f / 255f, 81f / 255f, 0f / 255f);
            this.Clip = true;
            this.AddChildLast(backgroundPanel);
            this.AddChildLast(headerPanel);
            this.AddChildLast(controlPanel);

            // backgroundPanel
            backgroundPanel.BackgroundColor = new UIColor(81f / 255f, 81f / 255f, 81f / 255f, 255f / 255f);
            backgroundPanel.Clip = true;

            // headerPanel
            headerPanel.BackgroundColor = new UIColor(17f / 255f, 17f / 255f, 17f / 255f, 255f / 255f);
            headerPanel.Clip = true;
            headerPanel.AddChildLast(titleLabel);

            // titleLabel
            titleLabel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            titleLabel.Font = new UIFont(FontAlias.System, 30, FontStyle.Regular);
            titleLabel.LineBreak = LineBreak.Character;

            // controlPanel
            controlPanel.BackgroundColor = new UIColor(34f / 255f, 34f / 255f, 34f / 255f, 255f / 255f);
            controlPanel.Clip = true;
            controlPanel.AddChildLast(transitionSettingButton);
            controlPanel.AddChildLast(previousButton);
            controlPanel.AddChildLast(topButton);
            controlPanel.AddChildLast(nextButton);

            // transitionSettingButton
            transitionSettingButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            transitionSettingButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // previousButton
            previousButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            previousButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // topButton
            topButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            topButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // nextButton
            nextButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            nextButton.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetSize(480, 854);
                    this.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;

                    backgroundPanel.SetPosition(0, 175);
                    backgroundPanel.SetSize(480, 678);
                    backgroundPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    backgroundPanel.Visible = true;

                    headerPanel.SetPosition(0, 0);
                    headerPanel.SetSize(480, 50);
                    headerPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Right;
                    headerPanel.Visible = true;

                    titleLabel.SetPosition(5, 5);
                    titleLabel.SetSize(465, 43);
                    titleLabel.Anchors = Anchors.Top | Anchors.Bottom | Anchors.Left | Anchors.Right;
                    titleLabel.Visible = true;

                    controlPanel.SetPosition(0, 50);
                    controlPanel.SetSize(480, 125);
                    controlPanel.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Right;
                    controlPanel.Visible = true;

                    transitionSettingButton.SetPosition(191, 69);
                    transitionSettingButton.SetSize(278, 56);
                    transitionSettingButton.Anchors = Anchors.Top | Anchors.Height;
                    transitionSettingButton.Visible = true;

                    previousButton.SetPosition(5, 5);
                    previousButton.SetSize(150, 56);
                    previousButton.Anchors = Anchors.Top | Anchors.Height;
                    previousButton.Visible = true;

                    topButton.SetPosition(165, 5);
                    topButton.SetSize(150, 56);
                    topButton.Anchors = Anchors.Top | Anchors.Height;
                    topButton.Visible = true;

                    nextButton.SetPosition(319, 5);
                    nextButton.SetSize(150, 56);
                    nextButton.Anchors = Anchors.Top | Anchors.Height;
                    nextButton.Visible = true;

                    break;

                default:
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    backgroundPanel.SetPosition(0, 0);
                    backgroundPanel.SetSize(854, 480);
                    backgroundPanel.Anchors = Anchors.None;
                    backgroundPanel.Visible = true;

                    headerPanel.SetPosition(0, 0);
                    headerPanel.SetSize(854, 45);
                    headerPanel.Anchors = Anchors.None;
                    headerPanel.Visible = true;

                    titleLabel.SetPosition(9, 0);
                    titleLabel.SetSize(840, 45);
                    titleLabel.Anchors = Anchors.Top | Anchors.Height;
                    titleLabel.Visible = true;

                    controlPanel.SetPosition(0, 45);
                    controlPanel.SetSize(854, 65);
                    controlPanel.Anchors = Anchors.Top | Anchors.Height;
                    controlPanel.Visible = true;

                    transitionSettingButton.SetPosition(629, 4);
                    transitionSettingButton.SetSize(220, 56);
                    transitionSettingButton.Anchors = Anchors.Height;
                    transitionSettingButton.Visible = true;

                    previousButton.SetPosition(5, 4);
                    previousButton.SetSize(120, 56);
                    previousButton.Anchors = Anchors.Height;
                    previousButton.Visible = true;

                    topButton.SetPosition(130, 4);
                    topButton.SetSize(120, 56);
                    topButton.Anchors = Anchors.Height;
                    topButton.Visible = true;

                    nextButton.SetPosition(255, 4);
                    nextButton.SetSize(120, 56);
                    nextButton.Anchors = Anchors.Height;
                    nextButton.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            titleLabel.Text = "label";

            transitionSettingButton.Text = "Transition Setting";

            previousButton.Text = "Previous";

            topButton.Text = "Top";

            nextButton.Text = "Next";
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

    }
}
