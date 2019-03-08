/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterStreamingSample
{
    public partial class SetParameterDialog : Dialog
    {
		public EditableText ParamTrack{ get {return this.m_ParamTrack;}}
		public PopupList ParamLocation { get { return this.m_PopupListLocation;}}
		public PopupList ParamWarning { get { return this.m_PopupListWarning;}}
		public Button ButtonOk{ get{return this.m_ButtonOk;}}
		public Button ButtonCancel{ get{return this.m_ButtonCancel;}}
		
        public SetParameterDialog()
            : base(null, null)
        {
            InitializeWidget();
        }
    }
}
