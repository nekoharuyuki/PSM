/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using Sce.PlayStation.HighLevel.UI;
using System.ComponentModel;
using TweetSharp;

namespace TwitterStreamingSample
{
    public partial class FeedsPanel : Panel, INotifyPropertyChanged
    {
		private List<StreamInfo> m_StList;
		public List<StreamInfo> StreamList
		{
			get
			{
				return this.m_StList;
			}
			set
			{
				this.m_StList = value;
				this.OnPropertyChanged("TwitterStList");
			}
		}
		
		public FeedsPanel()
		{
			this.InitializeWidget();
			
			this.PropertyChanged +=
				(object sender, PropertyChangedEventArgs e) =>
			{
				switch(e.PropertyName)
				{
					case "TwitterStList":
						{
							this.ListPanelFeeds.Sections = new ListSectionCollection();
							this.ListPanelFeeds.Sections.Add(new ListSection("statusList", this.StreamList.Count));
						}
						break;
					default:
						break;
				}
			};
			this.ListPanelFeeds.SetListItemUpdater(
				(ListPanelItem item) =>
			{
				var listPanelItem = item as FeedListPanelItem;
				if(null == listPanelItem) { return; }

				var sectionIndex = item.SectionIndex;
				var indexInSection = item.IndexInSection;
				
				switch(this.ListPanelFeeds.Sections[0].Title)
				{
					case "statusList":
						{
							listPanelItem.ModeSt = this.StreamList[indexInSection];
						}
						break;
				}
			});
		}
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged = delegate {};
		public void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		
    }
}
