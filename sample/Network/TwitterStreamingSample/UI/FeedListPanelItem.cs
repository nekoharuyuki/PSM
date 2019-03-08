/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.HighLevel.UI;
using System.ComponentModel;
using TweetSharp;
using Sce.PlayStation.Core.Imaging;

namespace TwitterStreamingSample
{
    public partial class FeedListPanelItem :  ListPanelItem, INotifyPropertyChanged
    {
		private StreamInfo sm_ModeSt;
		public StreamInfo ModeSt
		{
			get
			{
				return this.sm_ModeSt;
			}
			set
			{
				this.sm_ModeSt = value;
				OnPropertyChanged("ModeSt");
			}
		}
		
		private static string sm_UtcOffset = null;
		public string UtcOffset{
			get{
				if(sm_UtcOffset == null)
				{
					var user = Application.Current.TwitterService.GetUserProfile(new TweetSharp.GetUserProfileOptions());
					sm_UtcOffset = user.UtcOffset;
				}
				return sm_UtcOffset;
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
						switch(e.PropertyName)
						{
							case "ModeSt":
							{
								var mode = this.ModeSt;
						
								Application.Current.TwitterUserProfileImageCacheStore.RequestCache(
									this.ModeSt,
									(l_image) =>
									{
										if(mode != this.ModeSt){ return; }
										var img = new ImageAsset(l_image);
										this.m_ImageBox.Image = img;
									});
								
								this.m_LabelUserName.Text = this.ModeSt.Name;
			                    this.m_LabelUserScreenName.Text = "@" + this.ModeSt.ScreenName;
								
								TimeSpan offset = new TimeSpan(0,0, int.Parse(this.UtcOffset));
								var date = this.ModeSt.Time.Split(' ');
								var postDate = date[1] + " " + date[2] + ", " + date[5] + " " + date[3];
						
			                    this.m_LabelDateTime.Text = (DateTime.Parse (postDate) + offset).ToString("yyyy/MM/dd HH:mm:ss");
			                    this.m_LabelText.Text = this.ModeSt.Text;
			                }
								break;
							default:
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
