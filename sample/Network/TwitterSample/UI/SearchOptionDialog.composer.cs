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
    partial class SearchOptionDialog
    {
        Button m_Button_SearchOk;
        PopupList m_PopupList_Search;
        Label m_Label_SearchString;
        Label m_Label_Caution;
        Button m_Button_Cancel;

        private void InitializeWidget()
        {
            InitializeWidget(LayoutOrientation.Horizontal);
        }

        private void InitializeWidget(LayoutOrientation orientation)
        {
            m_Button_SearchOk = new Button();
            m_Button_SearchOk.Name = "m_Button_SearchOk";
            m_PopupList_Search = new PopupList();
            m_PopupList_Search.Name = "m_PopupList_Search";
            m_Label_SearchString = new Label();
            m_Label_SearchString.Name = "m_Label_SearchString";
            m_Label_Caution = new Label();
            m_Label_Caution.Name = "m_Label_Caution";
            m_Button_Cancel = new Button();
            m_Button_Cancel.Name = "m_Button_Cancel";

            // SearchOptionDialog
            this.AddChildLast(m_Button_SearchOk);
            this.AddChildLast(m_PopupList_Search);
            this.AddChildLast(m_Label_SearchString);
            this.AddChildLast(m_Label_Caution);
            this.AddChildLast(m_Button_Cancel);
            this.ShowEffect = new BunjeeJumpEffect()
            {
            };
            this.HideEffect = new TiltDropEffect();

            // m_Button_SearchOk
            m_Button_SearchOk.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Button_SearchOk.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_PopupList_Search
            m_PopupList_Search.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupList_Search.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupList_Search.ListItemTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupList_Search.ListItemFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_PopupList_Search.ListTitleTextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_PopupList_Search.ListTitleFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

            // m_Label_SearchString
            m_Label_SearchString.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Label_SearchString.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_Label_SearchString.LineBreak = LineBreak.Character;
            m_Label_SearchString.HorizontalAlignment = HorizontalAlignment.Center;

            // m_Label_Caution
            m_Label_Caution.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Label_Caution.Font = new UIFont(FontAlias.System, 25, FontStyle.Regular);
            m_Label_Caution.LineBreak = LineBreak.Character;
            m_Label_Caution.HorizontalAlignment = HorizontalAlignment.Center;

            // m_Button_Cancel
            m_Button_Cancel.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
            m_Button_Cancel.TextFont = new UIFont(FontAlias.System, 25, FontStyle.Regular);

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

                    m_Button_SearchOk.SetPosition(277, 316);
                    m_Button_SearchOk.SetSize(214, 56);
                    m_Button_SearchOk.Anchors = Anchors.None;
                    m_Button_SearchOk.Visible = true;

                    m_PopupList_Search.SetPosition(79, 178);
                    m_PopupList_Search.SetSize(360, 56);
                    m_PopupList_Search.Anchors = Anchors.Height;
                    m_PopupList_Search.Visible = true;

                    m_Label_SearchString.SetPosition(60, 78);
                    m_Label_SearchString.SetSize(399, 85);
                    m_Label_SearchString.Anchors = Anchors.None;
                    m_Label_SearchString.Visible = true;

                    m_Label_Caution.SetPosition(59, 43);
                    m_Label_Caution.SetSize(399, 35);
                    m_Label_Caution.Anchors = Anchors.None;
                    m_Label_Caution.Visible = true;

                    m_Button_Cancel.SetPosition(24, 316);
                    m_Button_Cancel.SetSize(214, 56);
                    m_Button_Cancel.Anchors = Anchors.None;
                    m_Button_Cancel.Visible = true;

                    break;

                default:
                    this.SetPosition(0, 0);
                    this.SetSize(854, 480);
                    this.Anchors = Anchors.None;

                    m_Button_SearchOk.SetPosition(488, 337);
                    m_Button_SearchOk.SetSize(214, 56);
                    m_Button_SearchOk.Anchors = Anchors.Height;
                    m_Button_SearchOk.Visible = true;

                    m_PopupList_Search.SetPosition(226, 205);
                    m_PopupList_Search.SetSize(400, 56);
                    m_PopupList_Search.Anchors = Anchors.Height;
                    m_PopupList_Search.Visible = true;

                    m_Label_SearchString.SetPosition(13, 111);
                    m_Label_SearchString.SetSize(827, 83);
                    m_Label_SearchString.Anchors = Anchors.Height;
                    m_Label_SearchString.Visible = true;

                    m_Label_Caution.SetPosition(45, 55);
                    m_Label_Caution.SetSize(748, 36);
                    m_Label_Caution.Anchors = Anchors.Height;
                    m_Label_Caution.Visible = true;

                    m_Button_Cancel.SetPosition(136, 337);
                    m_Button_Cancel.SetSize(214, 56);
                    m_Button_Cancel.Anchors = Anchors.Height;
                    m_Button_Cancel.Visible = true;

                    break;
            }
            _currentLayoutOrientation = orientation;
        }

        public void UpdateLanguage()
        {
            m_Button_SearchOk.Text = UIStringTable.Get(UIStringID.RESID_OK);

            m_PopupList_Search.ListTitle = UIStringTable.Get(UIStringID.RESID_OPTION);
            m_PopupList_Search.ListItems.Clear();
            m_PopupList_Search.ListItems.AddRange(new String[]
            {
                "User Search",
                "Tweet Search",
            });
            m_PopupList_Search.SelectedIndex = 0;

            m_Label_Caution.Text = UIStringTable.Get(UIStringID.RESID_SEARCH_OPT);

            m_Button_Cancel.Text = UIStringTable.Get(UIStringID.RESID_CANCEL);
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
