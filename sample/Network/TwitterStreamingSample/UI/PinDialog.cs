/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.HighLevel.UI;

namespace TwitterStreamingSample
{
    public partial class PinDialog : Dialog
    {
		public Button ButtonOk { get{return this.m_ButtonOK;}}
		public Button ButtonCancel { get{return this.m_ButtonCancel;}}
		public EditableText PinNum{ get{return this.m_EditableTextPIN;}}
		
        public PinDialog()
            : base(null, null)
        {
            InitializeWidget();
        }
    }
}
