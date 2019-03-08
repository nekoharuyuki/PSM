/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace TwitterStreamingSample
{
	public class StreamInfo
	{
		private string m_Id;
		private string m_Name;
		private string m_ScreenName;
		private string m_Time;
		private string m_Text;
		private string m_ProfileImgUrl;
		
		public string Id
		{
			get{ return m_Id; }
			set{ this.m_Id = value; }
		}
		
		public string Name
		{
			get{ return m_Name; }
			set{ this.m_Name = value; }
		}
		
		public string ScreenName
		{
			get{ return m_ScreenName; }
			set{ this.m_ScreenName = value; }
		}
		
		public string Time
		{
			get{ return m_Time; }
			set{ this.m_Time = value; }
		}
		
		public string Text
		{
			get{ return m_Text; }
			set{ this.m_Text = value; }
		}
		
		public string ProfileImgUrl
		{
			get{ return m_ProfileImgUrl; }
			set{ this.m_ProfileImgUrl = value; }
		}
		
		public StreamInfo ()
		{
		}
	}
}

