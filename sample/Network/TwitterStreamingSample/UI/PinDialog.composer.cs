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
    partial class PinDialog
    {
        Label m_Label_1;
        EditableText m_EditableTextPIN;
        Button m_ButtonCancel;
        Button m_ButtonOK;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_Label_1 = new Label();
            m_Label_1.Name = "m_Label_1";
            m_EditableTextPIN = new EditableText();
            m_EditableTextPIN.Name = "m_EditableTextPIN";
            m_ButtonCancel = new Button();
            m_ButtonCancel.Name = "m_ButtonCancel";
            m_ButtonOK = new Button();
            m_ButtonOK.Name = "m_ButtonOK";

            // PinDialog
            this.AddChildLast(m_Label_1);
            this.AddChildLast(m_EditableTextPIN);
            this.AddChildLast(m_ButtonCancel);
            this.AddChildLast(m_ButtonOK);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            // m_Label_1
            m_Label_1.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Label_1.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_Label_1.LineBreak = LineBreak.Character;
            m_Label_1.HorizontalAlignment = HorizontalAlignment.Center;

            // m_EditableTextPIN
            m_EditableTextPIN.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_EditableTextPIN.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_EditableTextPIN.LineBreak = LineBreak.Character;

            // m_ButtonCancel
            m_ButtonCancel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonCancel.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_ButtonOK
            m_ButtonOK.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonOK.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    m_Label_1.SetPosition(152, 93);
                    m_Label_1.SetSize(214, 36);
                    m_Label_1.Anchors = Anchors.None;
                    m_Label_1.Visible = true;

                    m_EditableTextPIN.SetPosition(79, 129);
                    m_EditableTextPIN.SetSize(360, 56);
                    m_EditableTextPIN.Anchors = Anchors.None;
                    m_EditableTextPIN.Visible = true;

                    m_ButtonCancel.SetPosition(20, 313);
                    m_ButtonCancel.SetSize(214, 56);
                    m_ButtonCancel.Anchors = Anchors.None;
                    m_ButtonCancel.Visible = true;

                    m_ButtonOK.SetPosition(289, 313);
                    m_ButtonOK.SetSize(214, 56);
                    m_ButtonOK.Anchors = Anchors.None;
                    m_ButtonOK.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(640, 480);
                    this.Anchors = Anchors.None;

                    m_Label_1.SetPosition(226, 60);
                    m_Label_1.SetSize(214, 36);
                    m_Label_1.Anchors = Anchors.None;
                    m_Label_1.Visible = true;

                    m_EditableTextPIN.SetPosition(153, 240);
                    m_EditableTextPIN.SetSize(360, 56);
                    m_EditableTextPIN.Anchors = Anchors.Height;
                    m_EditableTextPIN.Visible = true;

                    m_ButtonCancel.SetPosition(19, 399);
                    m_ButtonCancel.SetSize(214, 56);
                    m_ButtonCancel.Anchors = Anchors.Height;
                    m_ButtonCancel.Visible = true;

                    m_ButtonOK.SetPosition(405, 399);
                    m_ButtonOK.SetSize(214, 56);
                    m_ButtonOK.Anchors = Anchors.Height;
                    m_ButtonOK.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_Label_1.Text = "PIN";

            m_ButtonCancel.Text = UIStringTable.Get(UIStringID.RESID_CANCEL);

            m_ButtonOK.Text = UIStringTable.Get(UIStringID.RESID_OK);
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
