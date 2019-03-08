/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.ComponentModel;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Imaging;

namespace TwitterSample.UserInterface
{
    public partial class FeedListPanelItem : ListPanelItem, INotifyPropertyChanged
    {
        private static string sm_UtcOffset = null;
		private static ImageAsset img = new ImageAsset("/Application/assets/default_profile_0_normal.png");
        public string UtcOffset
        {
            get
            {
                if (sm_UtcOffset == null)
                {
                    var user = Application.Current.TwitterService.GetUserProfile(new TweetSharp.GetUserProfileOptions());
                    if(user == null)
                    { 
                    	return "0";
                    }
                    if (user.UtcOffset == null)
                    {
                        user.UtcOffset = "0";
                    }
                    sm_UtcOffset = user.UtcOffset;
                }
                
                return sm_UtcOffset;
            }
        }
        
        private TweetSharp.TwitterStatus m_Model;
        public TweetSharp.TwitterStatus Model
        {
            get
            {
                return this.m_Model;
            }
            set
            {
                this.m_Model = value;
                this.OnPropertyChanged("Model");
            }
        }
        
        private TweetSharp.TwitterUser m_ModelUser;
        public TweetSharp.TwitterUser ModelUser
        {
            get
            {
                return this.m_ModelUser;
            }
            set
            {
                this.m_ModelUser = value;
                this.OnPropertyChanged("ModelUser");
            }
        }
        
        private TweetSharp.TwitterTrend m_ModelTrend;
        public TweetSharp.TwitterTrend ModelTrends
        {
            get
            {
                return this.m_ModelTrend;
            }
            set
            {
                this.m_ModelTrend = value;
                this.OnPropertyChanged("ModelTrends");
            }
        }
        
        private TweetSharp.TwitterSearchResult m_SearchResult;
        public TweetSharp.TwitterSearchResult ModelSearchResult
        {
            get
            {
                return this.m_SearchResult;
            }
            set
            {
                this.m_SearchResult = value;
                this.OnPropertyChanged("SearchResult");
            }
        }
        
        public FeedListPanelItem()
        {
            this.InitializeWidget();
            
            this.PropertyChanged +=
                (object sender, PropertyChangedEventArgs e) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            switch (e.PropertyName)
                            {
                                case "Model":
                                    {
                                        var model = this.Model;
                                        Application.Current.TwitterUserProfileImageCacheStore.RequestCache(
                                            this.Model.User, 
                                            (l_image) =>
                                            {
                                                if (model != this.Model) { return; }
                                           		var img = new ImageAsset(l_image);
												this.m_ImageBox.Image = img;
                                            });
                                        this.m_LabelUserName.Text = this.Model.User.Name;
                                        this.m_LabelUserScreenName.Text = "@" + this.Model.User.ScreenName;

                                        TimeSpan offset = new TimeSpan(0,0, int.Parse(UtcOffset));

                                        this.m_LabelDateTime.Text = (this.Model.CreatedDate + offset).ToString();
                                        this.m_LabelText.Text = this.Model.Text;
                                    }
                                    break;
                            
                                case "ModelTrends":
                                    {
                                        this.m_LabelUserName.Text = this.ModelTrends.Name;
                                        this.m_LabelUserScreenName.Text = "";
                                        this.m_LabelDateTime.Text = this.ModelTrends.TrendingAsOf.ToString();
                                        this.m_LabelText.Text = this.ModelTrends.Url;
                                    }
                                    break;
                            
                                case "ModelUser":
                                    {
                                        var modelUser = this.ModelUser;
                                        Application.Current.TwitterUserProfileImageCacheStore.RequestCache(
                                            this.ModelUser, 
                                            (l_image) =>
                                            {
                                                if (modelUser != this.ModelUser) { return; }
												var img = new ImageAsset(l_image);
												this.m_ImageBox.Image = img;
                                            });
                            
                                        this.m_LabelUserName.Text = this.ModelUser.Name;
                                        this.m_LabelUserScreenName.Text = "@" + this.ModelUser.ScreenName;
                                        this.m_LabelDateTime.Text = this.ModelUser.CreatedDate.ToString();
                            
                                        if (!String.IsNullOrEmpty(this.ModelUser.Description))
                                        {
                                            this.m_LabelText.Text = this.ModelUser.Description;
                                        }
                                        else
                                        {
                                            this.m_LabelText.Text = "";
                                        }
        
                                    }
                                    break;
                            
                                case "SearchResult":
                                    {
                                        this.m_LabelUserName.Text = "";
                                        this.m_LabelUserScreenName.Text = "";
                                        this.m_LabelDateTime.Text = "";
                                        this.m_LabelText.Text = "";
                                    }
                                    break;
                                default :
                                    break;
                            }
                        });
                };
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
