// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace ScreenOrientationSample
{
    partial class TopScene
    {
        Button ButtonLandscape;
        Button ButtonReverseLandscape;
        Button ButtonPortrait;
        Button ButtonReversePortrait;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ButtonLandscape = new Button();
            ButtonLandscape.Name = "ButtonLandscape";
            ButtonReverseLandscape = new Button();
            ButtonReverseLandscape.Name = "ButtonReverseLandscape";
            ButtonPortrait = new Button();
            ButtonPortrait.Name = "ButtonPortrait";
            ButtonReversePortrait = new Button();
            ButtonReversePortrait.Name = "ButtonReversePortrait";

            // TopScene
            this.RootWidget.AddChildLast(ButtonLandscape);
            this.RootWidget.AddChildLast(ButtonReverseLandscape);
            this.RootWidget.AddChildLast(ButtonPortrait);
            this.RootWidget.AddChildLast(ButtonReversePortrait);
            this.Transition = new PushTransition();

            // ButtonLandscape
            ButtonLandscape.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonLandscape.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonReverseLandscape
            ButtonReverseLandscape.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonReverseLandscape.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonPortrait
            ButtonPortrait.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonPortrait.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // ButtonReversePortrait
            ButtonReversePortrait.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            ButtonReversePortrait.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    ButtonLandscape.SetPosition(105, 95);
                    ButtonLandscape.SetSize(214, 56);
                    ButtonLandscape.Anchors = Anchors.None;
                    ButtonLandscape.Visible = true;

                    ButtonReverseLandscape.SetPosition(105, 186);
                    ButtonReverseLandscape.SetSize(214, 56);
                    ButtonReverseLandscape.Anchors = Anchors.None;
                    ButtonReverseLandscape.Visible = true;

                    ButtonPortrait.SetPosition(105, 278);
                    ButtonPortrait.SetSize(214, 56);
                    ButtonPortrait.Anchors = Anchors.None;
                    ButtonPortrait.Visible = true;

                    ButtonReversePortrait.SetPosition(541, 285);
                    ButtonReversePortrait.SetSize(214, 56);
                    ButtonReversePortrait.Anchors = Anchors.None;
                    ButtonReversePortrait.Visible = true;

                    break;

                default:
                    this.DesignWidth = 960;
                    this.DesignHeight = 544;

                    ButtonLandscape.SetPosition(58, 174);
                    ButtonLandscape.SetSize(366, 56);
                    ButtonLandscape.Anchors = Anchors.None;
                    ButtonLandscape.Visible = true;

                    ButtonReverseLandscape.SetPosition(58, 315);
                    ButtonReverseLandscape.SetSize(366, 56);
                    ButtonReverseLandscape.Anchors = Anchors.None;
                    ButtonReverseLandscape.Visible = true;

                    ButtonPortrait.SetPosition(546, 174);
                    ButtonPortrait.SetSize(366, 56);
                    ButtonPortrait.Anchors = Anchors.None;
                    ButtonPortrait.Visible = true;

                    ButtonReversePortrait.SetPosition(546, 315);
                    ButtonReversePortrait.SetSize(366, 56);
                    ButtonReversePortrait.Anchors = Anchors.None;
                    ButtonReversePortrait.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            ButtonLandscape.Text = "Landscape";

            ButtonReverseLandscape.Text = "ReverseLandscape";

            ButtonPortrait.Text = "Portrait";

            ButtonReversePortrait.Text = "ReversePortrait";
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
