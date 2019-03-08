/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    partial class WaitingDialog
    {
        BusyIndicator m_BusyIndicator_1;
        Label m_LabelWaiting;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_BusyIndicator_1 = new BusyIndicator(true);
            m_BusyIndicator_1.Name = "m_BusyIndicator_1";
            m_LabelWaiting = new Label();
            m_LabelWaiting.Name = "m_LabelWaiting";

            // WaitingDialog
            this.AddChildLast(m_BusyIndicator_1);
            this.AddChildLast(m_LabelWaiting);
            this.HideEffect = new TiltDropEffect();

            // m_LabelWaiting
            m_LabelWaiting.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_LabelWaiting.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_LabelWaiting.LineBreak = LineBreak.Character;
            m_LabelWaiting.HorizontalAlignment = HorizontalAlignment.Center;

            SetWidgetLayout(orientation);

            UpdateLanguage();
        }

        private LayoutOrientation _currentLayoutOrientation;
        public void SetWidgetLayout(LayoutOrientation orientation)
        {
            switch (orientation)
            {
                case LayoutOrientation.Vertical:
                    this.SetPosition(0, 0);
                    this.SetSize(540, 100);
                    this.Anchors = Anchors.None;

                    m_BusyIndicator_1.SetPosition(140, 30);
                    m_BusyIndicator_1.SetSize(48, 48);
                    m_BusyIndicator_1.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator_1.Visible = true;

                    m_LabelWaiting.SetPosition(188, 36);
                    m_LabelWaiting.SetSize(214, 36);
                    m_LabelWaiting.Anchors = Anchors.None;
                    m_LabelWaiting.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(320, 72);
                    this.Anchors = Anchors.None;

                    m_BusyIndicator_1.SetPosition(31, 12);
                    m_BusyIndicator_1.SetSize(48, 48);
                    m_BusyIndicator_1.Anchors = Anchors.Height | Anchors.Width;
                    m_BusyIndicator_1.Visible = true;

                    m_LabelWaiting.SetPosition(79, 18);
                    m_LabelWaiting.SetSize(200, 36);
                    m_LabelWaiting.Anchors = Anchors.Height;
                    m_LabelWaiting.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_LabelWaiting.Text = "Waiting...";
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
