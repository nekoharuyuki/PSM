// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace UICatalog
{
    partial class SamplePagePanelInnerPanel
    {
        Label pageTitle;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            pageTitle = new Label();

            SetWidgetLayout(orientation);

            // pageTitle
            pageTitle.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            pageTitle.Font = new UIFont( FontAlias.System, 25, FontStyle.Regular);
            pageTitle.LineBreak = LineBreak.Character;
            pageTitle.HorizontalAlignment = HorizontalAlignment.Center;

            // Scene
            this.RootWidget.AddChildLast(pageTitle);

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

                pageTitle.SetPosition(338, 199);
                pageTitle.SetSize(214, 36);
                pageTitle.AnchorFlags = AnchorFlags.Top | AnchorFlags.Height | AnchorFlags.Left | AnchorFlags.Width;
                pageTitle.Visible = true;

                break;

            default:
                this.DesignWidth = 854;
                this.DesignHeight = 480;

                pageTitle.SetPosition(320, 222);
                pageTitle.SetSize(214, 36);
                pageTitle.AnchorFlags = AnchorFlags.Top | AnchorFlags.Height | AnchorFlags.Left | AnchorFlags.Width;
                pageTitle.Visible = true;

                break;
            }
            _currentLayoutOrientation = orientation;
        }
        public void UpdateLanguage()
        {
            pageTitle.Text = "Title";
        }
        internal void StartDefaultEffect()
        {
            switch (_currentLayoutOrientation)
            {
                case LayoutOrientation.Vertical:
                {
                }
                break;

                default:
                {
                }
                break;
            }
        }
    }
}
