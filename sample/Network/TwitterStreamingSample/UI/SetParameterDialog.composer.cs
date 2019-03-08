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
    partial class SetParameterDialog
    {
        EditableText m_ParamTrack;
        Label Label_2;
        Label Label_3;
        Label Label_5;
        Button m_ButtonOk;
        Button m_ButtonCancel;
        PopupList m_PopupListLocation;
        PopupList m_PopupListWarning;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_ParamTrack = new EditableText();
            m_ParamTrack.Name = "m_ParamTrack";
            Label_2 = new Label();
            Label_2.Name = "Label_2";
            Label_3 = new Label();
            Label_3.Name = "Label_3";
            Label_5 = new Label();
            Label_5.Name = "Label_5";
            m_ButtonOk = new Button();
            m_ButtonOk.Name = "m_ButtonOk";
            m_ButtonCancel = new Button();
            m_ButtonCancel.Name = "m_ButtonCancel";
            m_PopupListLocation = new PopupList();
            m_PopupListLocation.Name = "m_PopupListLocation";
            m_PopupListWarning = new PopupList();
            m_PopupListWarning.Name = "m_PopupListWarning";

            // SetParameterDialog
            this.AddChildLast(m_ParamTrack);
            this.AddChildLast(Label_2);
            this.AddChildLast(Label_3);
            this.AddChildLast(Label_5);
            this.AddChildLast(m_ButtonOk);
            this.AddChildLast(m_ButtonCancel);
            this.AddChildLast(m_PopupListLocation);
            this.AddChildLast(m_PopupListWarning);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            // m_ParamTrack
            m_ParamTrack.TextColor = new UIColor(252f / 255f, 252f / 255f, 252f / 255f, 255f / 255f);
            m_ParamTrack.Font = new UIFont(FontAlias.System, 20, FontStyle.Regular);
            m_ParamTrack.LineBreak = LineBreak.Character;

            // Label_2
            Label_2.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_2.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
            Label_2.LineBreak = LineBreak.Character;

            // Label_3
            Label_3.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_3.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
            Label_3.LineBreak = LineBreak.Character;

            // Label_5
            Label_5.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            Label_5.Font = new UIFont(FontAlias.System, 22, FontStyle.Bold);
            Label_5.LineBreak = LineBreak.Character;

            // m_ButtonOk
            m_ButtonOk.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonOk.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_ButtonCancel
            m_ButtonCancel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_ButtonCancel.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_PopupListLocation
            m_PopupListLocation.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListLocation.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupListLocation.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListLocation.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupListLocation.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListLocation.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_PopupListWarning
            m_PopupListWarning.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListWarning.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupListWarning.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListWarning.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupListWarning.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupListWarning.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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
                    this.SetSize(520, 520);
                    this.Anchors = Anchors.None;

                    m_ParamTrack.SetPosition(16, 146);
                    m_ParamTrack.SetSize(360, 56);
                    m_ParamTrack.Anchors = Anchors.None;
                    m_ParamTrack.Visible = true;

                    Label_2.SetPosition(16, 110);
                    Label_2.SetSize(147, 36);
                    Label_2.Anchors = Anchors.None;
                    Label_2.Visible = true;

                    Label_3.SetPosition(16, 208);
                    Label_3.SetSize(147, 36);
                    Label_3.Anchors = Anchors.None;
                    Label_3.Visible = true;

                    Label_5.SetPosition(16, 307);
                    Label_5.SetSize(200, 36);
                    Label_5.Anchors = Anchors.None;
                    Label_5.Visible = true;

                    m_ButtonOk.SetPosition(272, 443);
                    m_ButtonOk.SetSize(214, 56);
                    m_ButtonOk.Anchors = Anchors.None;
                    m_ButtonOk.Visible = true;

                    m_ButtonCancel.SetPosition(28, 443);
                    m_ButtonCancel.SetSize(214, 56);
                    m_ButtonCancel.Anchors = Anchors.None;
                    m_ButtonCancel.Visible = true;

                    m_PopupListLocation.SetPosition(-80, 224);
                    m_PopupListLocation.SetSize(360, 56);
                    m_PopupListLocation.Anchors = Anchors.Height;
                    m_PopupListLocation.Visible = true;

                    m_PopupListWarning.SetPosition(-112, 310);
                    m_PopupListWarning.SetSize(360, 56);
                    m_PopupListWarning.Anchors = Anchors.Height;
                    m_PopupListWarning.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(700, 480);
                    this.Anchors = Anchors.None;

                    m_ParamTrack.SetPosition(45, 66);
                    m_ParamTrack.SetSize(620, 55);
                    m_ParamTrack.Anchors = Anchors.Height;
                    m_ParamTrack.Visible = true;

                    Label_2.SetPosition(45, 30);
                    Label_2.SetSize(291, 36);
                    Label_2.Anchors = Anchors.Height;
                    Label_2.Visible = true;

                    Label_3.SetPosition(45, 151);
                    Label_3.SetSize(291, 36);
                    Label_3.Anchors = Anchors.Height;
                    Label_3.Visible = true;

                    Label_5.SetPosition(46, 273);
                    Label_5.SetSize(290, 36);
                    Label_5.Anchors = Anchors.Height;
                    Label_5.Visible = true;

                    m_ButtonOk.SetPosition(386, 392);
                    m_ButtonOk.SetSize(214, 56);
                    m_ButtonOk.Anchors = Anchors.Height;
                    m_ButtonOk.Visible = true;

                    m_ButtonCancel.SetPosition(100, 392);
                    m_ButtonCancel.SetSize(214, 56);
                    m_ButtonCancel.Anchors = Anchors.Height;
                    m_ButtonCancel.Visible = true;

                    m_PopupListLocation.SetPosition(45, 186);
                    m_PopupListLocation.SetSize(620, 56);
                    m_PopupListLocation.Anchors = Anchors.Height;
                    m_PopupListLocation.Visible = true;

                    m_PopupListWarning.SetPosition(46, 309);
                    m_PopupListWarning.SetSize(618, 56);
                    m_PopupListWarning.Anchors = Anchors.Height;
                    m_PopupListWarning.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_ParamTrack.Text = "soccer";

            Label_2.Text = "track";

            Label_3.Text = "locations";

            Label_5.Text = "stall_warnings";

            m_ButtonOk.Text = UIStringTable.Get(UIStringID.RESID_OK);

            m_ButtonCancel.Text = UIStringTable.Get(UIStringID.RESID_CANCEL);

            m_PopupListLocation.ListItems.Clear();
            m_PopupListLocation.ListItems.AddRange(new String[]
            {
                "Empty",
                "San Francisco",
                "New York City",
            });
            m_PopupListLocation.SelectedIndex = 1;

            m_PopupListWarning.ListItems.Clear();
            m_PopupListWarning.ListItems.AddRange(new String[]
            {
                "Empty",
                "True",
            });
            m_PopupListWarning.SelectedIndex = 0;
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
