// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class LoadingScene
    {
        ProgressBar ProgressBar_load;
        Label Label_1;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            ProgressBar_load = new ProgressBar();
            ProgressBar_load.Name = "ProgressBar_load";
            Label_1 = new Label();
            Label_1.Name = "Label_1";

            // LoadingScene
            this.RootWidget.AddChildLast(ProgressBar_load);
            this.RootWidget.AddChildLast(Label_1);

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;
            Label_1.HorizontalAlignment = HorizontalAlignment.Center;

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

                    ProgressBar_load.SetPosition(59, 445);
                    ProgressBar_load.SetSize(362, 16);
                    ProgressBar_load.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    ProgressBar_load.Visible = true;

                    Label_1.SetPosition(133, 392);
                    Label_1.SetSize(214, 36);
                    Label_1.Anchors = Anchors.Top | Anchors.Height | Anchors.Left | Anchors.Width;
                    Label_1.Visible = true;

                    break;

                default:
                    this.DesignWidth = 854;
                    this.DesignHeight = 480;

                    ProgressBar_load.SetPosition(246, 266);
                    ProgressBar_load.SetSize(362, 16);
                    ProgressBar_load.Anchors = Anchors.Height;
                    ProgressBar_load.Visible = true;

                    Label_1.SetPosition(320, 176);
                    Label_1.SetSize(214, 44);
                    Label_1.Anchors = Anchors.Height | Anchors.Width;
                    Label_1.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = "loading";
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
