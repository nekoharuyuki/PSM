/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class HeaderPanel : Panel
    {
        public Label LabelTitle { get { return this.m_LabelTitle; } }
        public Button ButtonWrite { get { return this.m_ButtonWrite; } }
        public Button ButtonSearch { get { return this.m_ButtonSearch; } }
        public EditableText TweetText { get { return this.m_TweetTexts; } }
        public Label LabelCount { get { return this.m_LabelCount;} }
        
        public HeaderPanel()
        {
            this.InitializeWidget();
        }
    }
}
