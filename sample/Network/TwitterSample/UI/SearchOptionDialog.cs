/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class SearchOptionDialog : Dialog
    {
        public Button ButtonOK { get { return this.m_Button_SearchOk; } }
        public Button ButtonCancel { get { return this.m_Button_Cancel; } }
        public PopupList SearchList { get { return this.m_PopupList_Search; } }
        public Label SearchString { get { return this.m_Label_SearchString; } }
        
        public SearchOptionDialog() : base(null, null)
        {
            this.InitializeWidget();
        }
    }
}
