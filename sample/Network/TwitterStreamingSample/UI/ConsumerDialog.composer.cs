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

namespace TwitterStreamingSample
{
    partial class ConsumerDialog
    {
        Label Label_1;
        Button m_ButtonOk;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            Label_1 = new Label();
            Label_1.Name = "Label_1";
            m_ButtonOk = new Button();
            m_ButtonOk.Name = "m_ButtonOk";

            // ConsumerDialog
            this.AddChildLast(Label_1);
            this.AddChildLast(m_ButtonOk);
            this.ShowEffect = new FadeInEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            // Label_1
            Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_1.Font = new UIFont(FontAlias.System, 30, FontStyle.Regular);
            Label_1.LineBreak = LineBreak.Character;

            // m_ButtonOk
            m_ButtonOk.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonOk.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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
                    this.SetSize(520, 400);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(36, 57);
                    Label_1.SetSize(443, 188);
                    Label_1.Anchors = Anchors.None;
                    Label_1.Visible = true;

                    m_ButtonOk.SetPosition(151, 301);
                    m_ButtonOk.SetSize(214, 56);
                    m_ButtonOk.Anchors = Anchors.None;
                    m_ButtonOk.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(680, 350);
                    this.Anchors = Anchors.None;

                    Label_1.SetPosition(34, 45);
                    Label_1.SetSize(627, 206);
                    Label_1.Anchors = Anchors.Height;
                    Label_1.Visible = true;

                    m_ButtonOk.SetPosition(230, 268);
                    m_ButtonOk.SetSize(214, 56);
                    m_ButtonOk.Anchors = Anchors.Height;
                    m_ButtonOk.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            Label_1.Text = UIStringTable.Get(UIStringID.RESID_CONSUMER);

            m_ButtonOk.Text = UIStringTable.Get(UIStringID.RESID_OK);
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
