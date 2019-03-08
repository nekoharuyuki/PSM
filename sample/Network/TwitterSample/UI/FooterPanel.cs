/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class FooterPanel : Panel
    {
        public Button ButtonHome { get { return this.m_ButtonHome; } }
        public Button ButtonMentions { get { return this.m_ButtonMentions; } }
        public Button ButtonTrend { get { return this.m_ButtonTrend; } }
        public Button ButtonAccount { get { return this.m_ButtonAccount; } }
        public Button ButtonLogOut { get { return this.m_ButtonLogout;} }
        
        public FooterPanel()
        {
            this.InitializeWidget();
        }
    }
}
