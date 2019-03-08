/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Collections.Generic;
using System.ComponentModel;
using Sce.PlayStation.HighLevel.UI;

namespace TwitterSample.UserInterface
{
    public partial class FeedsPanel : Panel, INotifyPropertyChanged
    {
        public BusyIndicator BusyIndicator { get { return this.m_BusyIndicator; } }
        
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
        
        private List<TweetSharp.TwitterTrend> m_TwitterTrendsList;
        public List<TweetSharp.TwitterTrend> TwitterTrendsList
        {
            get { return this.m_TwitterTrendsList; }
            set
            {
                this.m_TwitterTrendsList = value;
                this.OnPropertyChanged("TwitterTrendsList");
            }
        }
        
        private List<TweetSharp.TwitterUser> m_TwitterUserList;
        public List<TweetSharp.TwitterUser> TwitterUserList
        {
            get { return this.m_TwitterUserList; }
            set
            {
                this.m_TwitterUserList = value;
                this.OnPropertyChanged("TwitterUserList");
            }
        }
        
        private List<TweetSharp.TwitterSearchResult> m_TwitterSearchResult;
        public List<TweetSharp.TwitterSearchResult> TwitterSearchResult
        {
            get { return this.m_TwitterSearchResult; }
            set
            {
                this.m_TwitterSearchResult = value;
                this.OnPropertyChanged("TwitterSearchResult");
            }
        }
        
        public FeedsPanel()
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
                    
                        case "TwitterUserList":
                            {
                                this.ListPanelFeeds.Sections = new ListSectionCollection();
                                this.ListPanelFeeds.Sections.Add(new ListSection("userList", this.TwitterUserList.Count));
                                this.ListPanelFeeds.UpdateItems();
                            }
                            break;
                    
                        case "TwitterTrendsList":
                            {
                                this.ListPanelFeeds.Sections = new ListSectionCollection();
                                this.ListPanelFeeds.Sections.Add(new ListSection("trendList", this.TwitterTrendsList.Count));
                                this.ListPanelFeeds.UpdateItems();
                            }
                            break;
                    
                        case "TwitterSearchResult":
                            {
                                this.ListPanelFeeds.Sections = new ListSectionCollection();
                                this.ListPanelFeeds.Sections.Add(new ListSection("searchResult", this.TwitterSearchResult.Count));
                                this.ListPanelFeeds.UpdateItems();
                            }
                            break;
                        default :
                            break;
                    }
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
                    
                        case "trendList":
                            {
                                listPanelItem.ModelTrends = this.TwitterTrendsList[indexInSection];
                            }
                            break;
                    
                        case "userList":
                            {
                                listPanelItem.ModelUser = this.TwitterUserList[indexInSection];
                            }
                            break;
                    
                        case "searchResult":
                            {
                                listPanelItem.ModelSearchResult = this.TwitterSearchResult[indexInSection];
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
