/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class PINDialog : Dialog
    {
        public EditableText EditableTextPIN { get { return this.m_EditableTextPIN; } }
        public Button ButtonOK { get { return this.m_ButtonOK; } }
        public Button ButtonCancel { get { return this.m_ButtonCancel; } }
        
        public PINDialog() : base(null, null)
        {
            this.InitializeWidget();
        }
    }
}
