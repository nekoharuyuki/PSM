/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class TitlePanel : Panel
    {
        public Button ButtonLogin { get { return this.m_ButtonLogin; } }
        public EditableText SettingProxy { get { return this.m_Proxy; } }
        
        public TitlePanel()
        {
            this.InitializeWidget();
        }
    }
}
