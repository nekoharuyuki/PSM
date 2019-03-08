/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Sce.PlayStation.HighLevel.UI;
using TweetSharp;

namespace TwitterSample.UserInterface
{
    public partial class MainScene : SceneBase
    {
        public static readonly int WOEID_WORLD_WIDE = 1;
        public static readonly  int TWITTER_MAX_CHAR = 140;
		private bool m_IsProcessing = false;
        public FooterPanel FooterPanel { get { return this.m_FooterPanel; } }
        public HeaderPanel HeaderPanel { get { return this.m_HeaderPanel; } }
        public FeedsPanel FeedsPanel { get { return this.m_FeedsPanel; } }
        public AccountPanel AccountPanel { get { return this.m_AccountPanel; } }
        
        public MainScene()
        {
            this.InitializeWidget();
            
            this.FooterPanel.ButtonHome.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Home
                    this.ShowHome();
                };
            
            this.FooterPanel.ButtonMentions.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Connect
                    this.ShowMentions();
                };
            
            this.FooterPanel.ButtonTrend.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Trend
                    this.ShowTrends();
                };
            
            this.FooterPanel.ButtonAccount.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Account
                    this.ShowAccount();
                };
            
            this.HeaderPanel.ButtonSearch.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Search
                    this.SearchUser();
                };
            
            this.HeaderPanel.ButtonWrite.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.DenyOperation();
                    //Tweet
                    this.WriteTweet();
                };
            
            this.FooterPanel.ButtonLogOut.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    //SignOut
                    this.SignOut();
                };
            
            this.HeaderPanel.TweetText.TextChanged +=
                (object sender, TextChangedEventArgs e) =>
                {
                    var newText = e.NewText;
                    int charCount = TWITTER_MAX_CHAR - newText.Length;
    
                    this.HeaderPanel.LabelCount.Text = charCount.ToString();
                    
                    if (charCount < 0)
                    {
                        this.HeaderPanel.ButtonWrite.Enabled = false;
                    }
                    else
                    {
						if(this.m_IsProcessing == false)
						{
	                        this.HeaderPanel.ButtonWrite.Enabled = true;
	                    }
					}
                };
        }
        
        protected override void OnShown()
        {
            base.OnShown();
            
			if(!this.IsScreenOrientationChanging)
			{
            	this.DenyOperation();
            	this.ShowHome();
        	}
        }
        
        //Home
        private void ShowHome()
        {
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_HOME);
            
            this.FeedsPanel.BusyIndicator.Visible = true;
            
            this.AccountPanel.Visible = false;
            this.FeedsPanel.Visible = true;
            
            var bw = new BackgroundWorker();
            bw.DoWork +=
                (object bw_sender, DoWorkEventArgs bw_e) =>
                {
                    var tweets = Application.Current.TwitterService.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
                    if (null == tweets){
						this.PermitOperation();
                        return;
                    }
                
                    bw_e.Result = tweets;
                    
                };
            bw.RunWorkerCompleted +=    
                (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                {
                    var tweets = bw_e.Result as IEnumerable<TwitterStatus>;
                    
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            try
                            {
								if (null == tweets){
									this.PermitOperation();
                                    return;
                                }
                                this.m_FeedsPanel.TwitterStatusList = new List<TwitterStatus>(tweets);
                            }
                            finally
                            {
                                this.FeedsPanel.BusyIndicator.Visible = false;
                                this.HeaderPanel.TweetText.Text = "";
                                this.HeaderPanel.LabelCount.Text = "";
								this.PermitOperation();
                            }
                        });
                };
            bw.RunWorkerAsync();
        }
        
        //Connect
        private void ShowMentions()
        {
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_CONNECT);
            
            this.FeedsPanel.BusyIndicator.Visible = true;
            
            this.AccountPanel.Visible = false;
            this.FeedsPanel.Visible = true;
            
            var bw = new BackgroundWorker();
            bw.DoWork +=
                (object bw_sender, DoWorkEventArgs bw_e) =>
                {
                    var tweets = Application.Current.TwitterService.ListTweetsMentioningMe(new ListTweetsMentioningMeOptions());
                    if (null == tweets) {
						this.PermitOperation();
                        return;
                    }
                
                    bw_e.Result = tweets;
                };
            bw.RunWorkerCompleted +=
                (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                {
                    var tweets = bw_e.Result as IEnumerable<TwitterStatus>;
                    
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            try
                            {
                                if (null == tweets){
									this.PermitOperation();
                                    return;
                                }
                                this.m_FeedsPanel.TwitterStatusList = new List<TwitterStatus>(tweets);
                            }
                            finally
                            {
                                this.FeedsPanel.BusyIndicator.Visible = false;
                                this.HeaderPanel.TweetText.Text = "";
                                this.HeaderPanel.LabelCount.Text = "";
								this.PermitOperation();
                            }
                        });
                };
            bw.RunWorkerAsync();
        }
        
        //Trend
        private void ShowTrends()
        {
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_DISCOVER);
            
            this.FeedsPanel.BusyIndicator.Visible = true;
            this.AccountPanel.Visible = false;
            this.FeedsPanel.Visible = true;
            
            var bw = new BackgroundWorker();
            bw.DoWork +=
                (object bw_sender, DoWorkEventArgs bw_e) =>
                {
                    var user = Application.Current.TwitterService.GetUserProfile(new GetUserProfileOptions());
					if (null == user){
						this.PermitOperation();
                        return;
                    }
                
                    var userLocation = user.Location;
                    long? userLocationWoeId = null;
                    var locations = Application.Current.TwitterService.ListAvailableTrendsLocations();

                    foreach (var location in locations)
                    {
                        if (location.Name == userLocation)
                        {
                            userLocationWoeId = location.WoeId;
                            break;
                        }
                    }
                
                    if (!userLocationWoeId.HasValue)
                    {
                        userLocationWoeId = WOEID_WORLD_WIDE;
                    }
                
                    var trends = Application.Current.TwitterService.ListLocalTrendsFor(
                        new ListLocalTrendsForOptions()
                        { 
                            Id = (int)userLocationWoeId.Value, 
                        });
                    
                    if (null == trends || null == trends.Trends || 0 >= trends.Trends.Count)
                    {
                        this.PermitOperation();
                        return;
                    }
                
                    bw_e.Result = trends.Trends;
                };
            bw.RunWorkerCompleted +=
                (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                {
                    var userTrends = default(List<TwitterTrend>);
                    try
                    {
                        userTrends = bw_e.Result as List<TwitterTrend>;
                    }
                    catch
                    {
						this.PermitOperation();
                        this.FeedsPanel.BusyIndicator.Visible = false;
                        return;
                    }
                
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            try
                            {
                                if (null == userTrends)
                                {
                                    this.PermitOperation();
                                    return;
                                }
                        
                                this.m_FeedsPanel.TwitterTrendsList = new List<TwitterTrend>(userTrends);
                            }
                            finally
                            {
                                this.FeedsPanel.BusyIndicator.Visible = false;
                                this.HeaderPanel.TweetText.Text = "";
                                this.HeaderPanel.LabelCount.Text = "";
								this.PermitOperation();
                            }
                        });
                };
            bw.RunWorkerAsync();
        }
        
        //Account
        private void ShowAccount()
        {
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_ME);
            
            this.AccountPanel.Visible = true;
            this.FeedsPanel.Visible = false;
            this.AccountPanel.BusyIndicator.Visible = true;
            
            {
                var user = Application.Current.TwitterService.GetUserProfile(new GetUserProfileOptions());
                
                if (null == user)
                {
                	this.PermitOperation();
                    this.AccountPanel.BusyIndicator.Visible = false;
                    return; 
                }
    
                this.AccountPanel.AccountName.Text = user.Name.ToString();
                this.AccountPanel.AccountScreenName.Text = "@" + user.ScreenName;
                this.AccountPanel.AccountProfile.Text = user.Description;
                this.AccountPanel.AccountLocation.Text = user.Location;
                this.AccountPanel.AccountTweets.Text = user.StatusesCount.ToString();
                this.AccountPanel.AccountFollow.Text = user.FriendsCount.ToString();
                this.AccountPanel.AccountFollower.Text = user.FollowersCount.ToString();
                
                var filePath = "/Temp/" + user.ScreenName + ".png";
                var request = new System.Net.HttpWebRequest(new System.Uri(user.ProfileImageUrl));
                
                using (var stream = request.GetResponse().GetResponseStream())              
                using (var file = System.IO.File.Create(filePath))
                {
                    stream.CopyTo(file);
                }
                
                var imageAsset = new ImageAsset(filePath, false);
                this.AccountPanel.AccountImg.Image = imageAsset;

                var bw = new BackgroundWorker();
                bw.DoWork +=
                    (object bw_sender, DoWorkEventArgs bw_e) =>
                    {
                        var userTweets =
                            Application.Current.TwitterService.ListTweetsOnUserTimeline(
                                new ListTweetsOnUserTimelineOptions
                                { 
                                    UserId = user.Id
                                });
                        bw_e.Result = userTweets;
                    };
                bw.RunWorkerCompleted +=
                    (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                var tweets = bw_e.Result as IEnumerable<TwitterStatus>;
                                if (null == tweets)
                                {
                                    this.PermitOperation();
                                    return;
                                }
                        
                                try
                                {
                                    this.AccountPanel.TwitterStatusList = new List<TwitterStatus>(tweets);
                                }
                                finally
                                {
                                    this.AccountPanel.BusyIndicator.Visible = false;
                                    this.HeaderPanel.TweetText.Text = "";
                                    this.HeaderPanel.LabelCount.Text = "";
									this.PermitOperation();
                                }
                            });
                    };
                bw.RunWorkerAsync();
            }
        }
        
        //Tweet
        private void WriteTweet()
        {
            if (string.IsNullOrEmpty(this.HeaderPanel.TweetText.Text))
            {
                this.PermitOperation();
                return;
            }
            
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_TWEET);
            
            this.FeedsPanel.BusyIndicator.Visible = true;
            this.AccountPanel.Visible = false;
            this.FeedsPanel.Visible = true;
            
            var status = this.HeaderPanel.TweetText.Text;
            
            var bw = new BackgroundWorker();
            bw.DoWork +=
                (object bw_sender, DoWorkEventArgs bw_e) =>
                {
                    Application.Current.TwitterService.SendTweet(
                        new SendTweetOptions
                        {
                            Status = status
                        });

                    var tweets = Application.Current.TwitterService.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
                    if (null == tweets)
                    {
                        this.PermitOperation();
                        return;
                    }
                
                    bw_e.Result = tweets;
                };
            bw.RunWorkerCompleted +=
                (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                {
                    var tweets = bw_e.Result as IEnumerable<TwitterStatus>;
                    
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            try
                            {
                                if (null == tweets)
                                {
                                    this.PermitOperation();
                                    return;
                                }
                                this.m_FeedsPanel.TwitterStatusList = new List<TwitterStatus>(tweets);
                            }
                            finally
                            {
                                this.FeedsPanel.BusyIndicator.Visible = false;
                                this.HeaderPanel.TweetText.Text = "";
                                this.HeaderPanel.LabelCount.Text = "";
								this.PermitOperation();
                            }
                        });
                };
            bw.RunWorkerAsync();
        }
        
        //Search
        private void SearchUser()
        {
            this.HeaderPanel.LabelTitle.Text = UIStringTable.Get(UIStringID.RESID_SEARCH);

            this.AccountPanel.Visible = false;
            this.FeedsPanel.Visible = true;
            
            var searchOptionDialog = new TwitterSample.UserInterface.SearchOptionDialog();
            searchOptionDialog.SearchString.Text = this.HeaderPanel.TweetText.Text.ToString();
            searchOptionDialog.ButtonOK.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.FeedsPanel.BusyIndicator.Visible = true;
            
                    var bw = new BackgroundWorker();
                    bw.DoWork +=
                        (object bw_sender, DoWorkEventArgs bw_e) =>
                        {
                            if (!String.IsNullOrEmpty(this.HeaderPanel.TweetText.Text.ToString()))
                            {
                                if (searchOptionDialog.SearchList.SelectedIndex == 0)
                                {
                                    //User Search
                                    var users =
                                        Application.Current.TwitterService.SearchForUser(
                                            new SearchForUserOptions
                                            {
                                                Q = this.HeaderPanel.TweetText.Text
                                            });
                                    if (null == users)
                                    {
                                        this.PermitOperation();
                                        return;
                                    }
                            
                                    bw_e.Result = users;
                                }
                                else
                                {
                                    //Tweet Search
                                    var tweets =
                                        Application.Current.TwitterService.Search(
                                            new SearchOptions
                                            {
                                                Q = this.HeaderPanel.TweetText.Text
                                            });
                                    if (null == tweets)
                                    {
                                        this.PermitOperation();
                                        return;
                                    }
                            
                                    bw_e.Result = tweets;
                                }
                            }
                        };
                    bw.RunWorkerCompleted +=
                        (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                        {
                            var resultUser = default(IEnumerable<TwitterUser>);
                            var resultTweet = default(TwitterSearchResult);
                    
                            if (bw_e.Result is List<TwitterUser>)
                            {
                                resultUser = bw_e.Result as IEnumerable<TwitterUser>;
                            }
                            else
                            {
                                resultTweet = (TwitterSearchResult)bw_e.Result;
                            }
                    
                            Application.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    try
                                    {
                                        if ((null == resultUser) && (null == resultTweet ))
                                        {
                                        	this.PermitOperation();
                                        	return;
                                        }
                                        if (null != resultUser)
                                        {
                                            this.m_FeedsPanel.TwitterUserList = new List<TwitterUser>(resultUser);
                                        }
                                        else
                                        {
                                            this.m_FeedsPanel.TwitterStatusList = new List<TwitterStatus>(resultTweet.Statuses);
                                        }
                                    }
                                    finally
                                    {
                                        this.FeedsPanel.BusyIndicator.Visible = false;
                                        this.HeaderPanel.TweetText.Text = "";
                                        this.HeaderPanel.LabelCount.Text = "";
										this.PermitOperation();
                                    }
                                });
                        };
                    bw.RunWorkerAsync();
                
                    this.IsDialogShown = false;
                    searchOptionDialog.Hide();
                };
            searchOptionDialog.ButtonCancel.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    this.IsDialogShown = false;
                    searchOptionDialog.Hide();
                
                    this.FeedsPanel.BusyIndicator.Visible = false;
                    this.HeaderPanel.TweetText.Text = "";
                    this.HeaderPanel.LabelCount.Text = "";
					this.PermitOperation();
                };
            
            this.IsDialogShown = true;
            searchOptionDialog.Show();
        }
        
        //SignOut
        private void SignOut()
        {
            if (File.Exists(TwitterSample.Settings.SETTINGS_FILE_PATH))
            {
                FileInfo file = new FileInfo(TwitterSample.Settings.SETTINGS_FILE_PATH);
                
                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    file.Attributes = FileAttributes.Normal;
                }
                file.Delete();
            }
            Application.Current.Settings.Delete();
            Application.Current.Settings.Model.AccessToken = null;
            Application.Current.Settings.Model.RequestToken = null;
            
            var scene = new LoginScene();
            UISystem.SetScene(scene);
        }
		
		private void PermitOperation()
		{
			this.m_IsProcessing = false;
			this.HeaderPanel.ButtonSearch.Enabled = true;
			this.HeaderPanel.ButtonWrite.Enabled = true;
			this.FooterPanel.ButtonHome.Enabled = true;
			this.FooterPanel.ButtonMentions.Enabled = true;
			this.FooterPanel.ButtonTrend.Enabled = true;
			this.FooterPanel.ButtonAccount.Enabled = true;
		}
		
		private void DenyOperation()
		{
			this.m_IsProcessing = true;
			this.HeaderPanel.ButtonSearch.Enabled = false;
			this.HeaderPanel.ButtonWrite.Enabled = false;
			this.FooterPanel.ButtonHome.Enabled = false;
			this.FooterPanel.ButtonMentions.Enabled = false;
			this.FooterPanel.ButtonTrend.Enabled = false;
			this.FooterPanel.ButtonAccount.Enabled = false;
		}
    }
}
