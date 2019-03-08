/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class ConsumerDialog : Dialog
    {
        public Button ButtonOK { get { return this.m_ButtonOk; } }
        
        public ConsumerDialog() : base(null, null)
        {
            this.InitializeWidget();
        }
    }
}
