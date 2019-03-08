/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using System.ComponentModel;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class AccountPanel : Panel, INotifyPropertyChanged
    {
        private List<TweetSharp.TwitterStatus> m_TwitterStatusList;
        public List<TweetSharp.TwitterStatus> TwitterStatusList
        {
            get { return this.m_TwitterStatusList; }
            set
            {
                this.m_TwitterStatusList = value;
                this.OnPropertyChanged("TwitterStatusList");
            }
        }
        
        public BusyIndicator BusyIndicator { get { return this.m_BusyIndicator; } }
        public ImageBox AccountImg { get { return this.m_AccountImg; } }
        public Label AccountScreenName { get { return this.m_AccountScreenName; } }
        public Label AccountName { get { return this.m_AccountName; } }
        public Label AccountProfile { get { return this.m_AccountProfile; } }
        public Label AccountLocation { get { return this.m_AccountLocattion; } }
        public Label AccountTweets { get { return this.m_AccountTweets; } }
        public Label AccountFollow { get { return this.m_AccountFollow; } }
        public Label AccountFollower { get { return this.m_AccountFollower; } }

        public AccountPanel()
        {
            this.InitializeWidget();
            
            this.PropertyChanged +=
                (object sender, PropertyChangedEventArgs e) =>
                {
                    switch (e.PropertyName)
                    {
                        case "TwitterStatusList":
                            {
                                this.ListPanelFeeds.Sections = new ListSectionCollection();
                                this.ListPanelFeeds.Sections.Add(new ListSection("statusList", this.TwitterStatusList.Count));
                                this.ListPanelFeeds.UpdateItems();
                            }
                            break;
                     };
                };
            
            this.ListPanelFeeds.SetListItemUpdater(
                (ListPanelItem item) =>
                {
                    var listPanelItem = item as FeedListPanelItem;
                    if (null == listPanelItem) { return; }
                
                    var sectionIndex = item.SectionIndex;
                    var indexInSection = item.IndexInSection;
                
                    switch (this.ListPanelFeeds.Sections[0].Title)
                    {
                        case "statusList":
                            {
                                listPanelItem.Model = this.TwitterStatusList[indexInSection];
                            }
                            break;
                    }
                });
        }
        
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
