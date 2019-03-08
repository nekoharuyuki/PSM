/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using Network;
using System.IO;
using Sce.PlayStation.Core.PSNServices;
using System.Net;

namespace SirAwesome
{

    public partial class UISettingDialog : Dialog
    {
		private bool m_isVisible = false;
		private bool m_loaded = false;
		private List<object> m_data;
		private NetworkService m_networkService;
		private LeaderboardCategory m_currentCategory;
		private enum LeaderboardCategory
		{
			kDaily,
			kWeekly,
			kMonthly,
			kAllTime,
			kFriends,
			kMax
		}
		private ImageAsset imgPanel;
		List<object> curLeaderboardData = null;
		private static int m_gameID = 1;
		private static int m_leaderboardID = 1;

		public bool IsVisible
        {
			get
            {
                return m_isVisible;
            }
            set
            {
                m_isVisible = value;
            }
        }
		
		public bool Loaded
        {
			get
            {
                return m_loaded;
            }
            set
            {
                m_loaded = value;
			}
		}
		// Setting the size of the listitem
		ListSectionCollection section = new ListSectionCollection {
            new ListSection("Section1", 200)
            };

        public UISettingDialog()
        {
            InitializeWidget();
			//Sce.PlayStation.Core.PSNServices.PSNRequestHeaderParam.RequestURL = "http://10.98.35.70:18000";
			PSNRequest.UseCson = true;
			// Setup ImageAsset
            imgPanel = new ImageAsset("/Application/assets/ui_panel.png");
			listPanel.SetListItemCreator(ListItemCreator);
			listPanel.SetListItemUpdater(ListItemUpdater);
			listPanel.Sections = section;
            buttonOK.ButtonAction += new EventHandler<TouchEventArgs>(Button_OK_ButtonAction);
			buttonDaily.ButtonAction += new EventHandler<TouchEventArgs>(Button_Daily_ButtonAction);
			buttonWeekly.ButtonAction += new EventHandler<TouchEventArgs>(Button_Weekly_ButtonAction);
			buttonMonthly.ButtonAction += new EventHandler<TouchEventArgs>(Button_Monthly_ButtonAction);
			buttonAllTime.ButtonAction += new EventHandler<TouchEventArgs>(Button_AllTime_ButtonAction);
			buttonFriends.ButtonAction += new EventHandler<TouchEventArgs>(Button_Friends_ButtonAction);
			this.Showing += new EventHandler(OnShowing);
			this.Shown += new EventHandler(OnShown);
			this.Hidden += new EventHandler<DialogEventArgs>(OnHidden);
			ReadSavedata();			
			//InitNetworkService();
        }
		
		const string SAVE_FILE = "/Documents/savedata";
		string m_ip = null;
		public string Ip { get { return m_ip; } }
		
		public void ReadSavedata()
		{
			bool exists = System.IO.File.Exists(SAVE_FILE);
			if (exists)
			{
				try 
				{
					TextReader reader = new StreamReader(SAVE_FILE);
					m_ip = reader.ReadLine();
					reader.Close();
					
					try
					{
						IPAddress addr = IPAddress.Parse(m_ip);
					}
					catch (Exception e)
					{
						Console.WriteLine("Invalid ip in savedata: {0}", m_ip);
						m_ip = null;
					}					
					
					if (m_ip != null)
						PSNRequestHeaderParam.RequestURL = "http://"+m_ip+":18000";
				}
				catch (Exception e)
				{
					Console.WriteLine("Read savedata failed: {0}", e);
				}
			}		
		}
		
		public void WriteSavedata()
		{
			try
			{			
				TextWriter writer = new StreamWriter(SAVE_FILE);
				writer.WriteLine(m_ip);
				writer.Close();	
			}
			catch (Exception e)
			{
				Console.WriteLine("Write savedata failed: {0}", e);
			}	
		}
		
		public void InitNetworkService()
		{
			if (labelPlayerName.Text.ToString() == "")
			{
				labelPlayerName.Text = Sce.PlayStation.Core.PSNServices.PSN.GetOnlineId;
				// Set player name
				if (labelPlayerName.Text.ToString().Length > 0)
					labelPlayerName.Text = labelPlayerName.Text + "'s";
			}

			//Request leaderboard data
			m_networkService = new NetworkService();
			if (m_networkService != null)
			{
				m_networkService.onFinished += OnGetLeaderboard;
				m_networkService.GetLeaderboard(m_gameID, m_leaderboardID);
			}
			m_currentCategory = LeaderboardCategory.kDaily;
		}
		
		public void ChangeIp(string ip)
		{
			try
			{
				IPAddress addr = IPAddress.Parse(ip);
			}
			catch (Exception e)
			{
				Console.WriteLine("Invalid ip: {0}", ip);
				ip = null;
			}
			
			if (ip != null)
			{
				m_ip = ip;
				WriteSavedata();
				PSNRequestHeaderParam.RequestURL = "http://"+m_ip+":18000";
				m_networkService.GetLeaderboard(1, 1);
			}
		}
		
		private ListPanelItem ListItemCreator()
        {
            //return new MyListPanelItem(ListPanel_1.Width, ListPanel_1.Height / 8.0f);
			return new MyListPanelItem(listPanel.Width, imgPanel.Height + 20, imgPanel);
        }

        public void ListItemUpdater(ListPanelItem item)
        {
            if (item is MyListPanelItem)
            {
                MyListPanelItem myItem = (item as MyListPanelItem);
                //myItem.BackgroundColor = new UIColor(0.12f, 0.12f, 0.12f, 1.0f);
				if (m_data != null)
				{
					if (curLeaderboardData != null) // double check logic
					{
						List<object> entry = (List<object>)curLeaderboardData[item.Index];
						if (entry != null)
						{
  	              			myItem.Image = imgPanel;
							myItem.Text = entry[0].ToString();
							//if (m_currentCategory == LeaderboardCategory.kFriends)
							//	myItem.Text = "Hello";
							List<object> score = (List<object>)entry[1];
							myItem.Score = score[0].ToString();
							myItem.Rank = (item.Index + 1).ToString();
							listPanel.Visible = true;
							labelPosition.Visible = true;
							labelPSNID.Visible = true;
							labelScore.Visible = true;
							buttonDaily.Visible = true;
							buttonWeekly.Visible = true;
							buttonMonthly.Visible = true;
							buttonAllTime.Visible = true;
							buttonFriends.Visible = true;
							labelLoading.Visible = false;
							indicator.Visible = false;
						}
					}
				}
				else
				{
					listPanel.Visible = false;
					labelPosition.Visible = false;
					labelPSNID.Visible = false;
					labelScore.Visible = false;
					buttonDaily.Visible = false;
					buttonWeekly.Visible = false;
					buttonMonthly.Visible = false;
					buttonAllTime.Visible = false;
					buttonFriends.Visible = false;
					labelLoading.Visible = true;
					indicator.Visible = true;
				}
            }
        }
		
		private class MyListPanelItem : ListPanelItem 
        {
            const float margin = 5.0f;

            public MyListPanelItem(float itemWidth, float itemHeight, ImageAsset imgPanel)
            {
                this.Width = itemWidth;
                this.Height = itemHeight;
                
                image = new ImageBox();
				image.Alpha = 5;
				image.X = 0;
				image.Y = 0;
                //image.X = margin;k
                //image.Y = margin;
                //image.Height = itemHeight - (margin * 2.0f);
				image.Height = imgPanel.Height;
                //image.Width = image.Height * 3.0f / 2.0f;
				image.Width = imgPanel.Width;
 
                this.AddChildLast(image);
				
				rank = new Label();
                //label.X = image.X + image.Width + margin;
                //label.Y = margin;
				rank.X = 100;
				rank.Y = 30;
				rank.Width = 250;
				rank.Height = 20;
				rank.TextColor = new UIColor(0, 1, 1, 1);
				rank.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
                //label.Width = (label.X * 3.0f) - (margin * 2.0f);
                //label.Height = itemHeight - (margin * 2.0f);
                rank.HorizontalAlignment = HorizontalAlignment.Left;
                this.AddChildLast(rank);
				
                label = new Label();
                //label.X = image.X + image.Width + margin;
                //label.Y = margin;
				label.X = 195;
				label.Y = 29;
				label.Width = 250;
				label.Height = 25;
				label.TextColor = new UIColor(1, 1, 0, 1);
				label.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
                //label.Width = (label.X * 3.0f) - (margin * 2.0f);
                //label.Height = itemHeight - (margin * 2.0f);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                this.AddChildLast(label);
				
				score = new Label();
                //label.X = image.X + image.Width + margin;
                //label.Y = margin;
				score.X = 400;
				score.Y = 30;
				score.Width = 250;
				score.Height = 20;
				score.TextColor = new UIColor(0, 1, 1, 1);
				score.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
                //label.Width = (label.X * 3.0f) - (margin * 2.0f);
                //label.Height = itemHeight - (margin * 2.0f);
                score.HorizontalAlignment = HorizontalAlignment.Left;
                this.AddChildLast(score);
            }

            public ImageAsset Image
            {
                get
                {
                    return image.Image;
                }
                set
                {
                    image.Image = value;
                }
            }
			public string Rank
            {
                get
                {
                    return rank.Text;
                }
                set
                {
                    rank.Text = value;
                }
            }
            public string Text
            {
                get
                {
                    return label.Text;
                }
                set
                {
                    label.Text = value;
                }
            }
			
			public string Score
            {
                get
                {
                    return score.Text;
                }
                set
                {
                    score.Text = value;
                }
            }
			private Label rank;
            private Label label;
			private Label score;
            private ImageBox image;
        }

        void Button_OK_ButtonAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }
		
		void Button_Daily_ButtonAction(object sender, TouchEventArgs e)
        {
			m_currentCategory = LeaderboardCategory.kDaily;
			SetLeaderboardCategoryData();
			listPanel.SetListItemUpdater(ListItemUpdater);
        }
		void Button_Weekly_ButtonAction(object sender, TouchEventArgs e)
        {
			m_currentCategory = LeaderboardCategory.kWeekly;
			SetLeaderboardCategoryData();
			listPanel.SetListItemUpdater(ListItemUpdater);
        }
		void Button_Monthly_ButtonAction(object sender, TouchEventArgs e)
        {
			m_currentCategory = LeaderboardCategory.kMonthly;
			SetLeaderboardCategoryData();
			listPanel.SetListItemUpdater(ListItemUpdater);
        }
		void Button_AllTime_ButtonAction(object sender, TouchEventArgs e)
        {
			m_currentCategory = LeaderboardCategory.kAllTime;
			SetLeaderboardCategoryData();
			listPanel.SetListItemUpdater(ListItemUpdater);
        }
		void Button_Friends_ButtonAction(object sender, TouchEventArgs e)
        {
			m_currentCategory = LeaderboardCategory.kFriends;
			SetLeaderboardCategoryData();
			listPanel.SetListItemUpdater(ListItemUpdater);
        }
		
		public List<object> Data
		{ 
			get
			{
				return m_data;
			}
			set
			{
				m_data = value;
				listPanel.SetListItemUpdater(ListItemUpdater);
				//UpdateData();
			}
		}
		public void OnGetLeaderboard( NetworkEventArgs args )
		{
			
			if (args.ErrorMessage != null )
			{
				Console.WriteLine("ERROR: " + args.ErrorMessage);
				//m_leaderboard.SetErrorMessage( args.ErrorMessage );
				//m_hasError = true;
				
				labelLoading.Text = args.ErrorMessage;
			}
			else
			{
				Console.WriteLine("LOADED!");
				this.Loaded = true;
				IDictionary<string, object> leaderboards = (IDictionary<string, object>)args.Data["1"];
				List<object> data = new List<object>();
				data.Add(leaderboards["1"]);
				data.Add(leaderboards["2"]);
				data.Add(leaderboards["3"]);
				data.Add(leaderboards["4"]);
				//m_leaderboard.Data = data;
				this.Data = data;
				SetLeaderboardCategoryData();
				//m_hasError = false;

				m_networkService.onFinished -= OnGetLeaderboard;
				m_networkService.onFinished += OnGetFriendsList;
				m_networkService.GetFriendsList();
			}
		}
		
		public void OnGetFriendsList( NetworkEventArgs args )
		{
			if( args.ErrorMessage != null )
			{
				//SetErrorMessage( args.ErrorMessage );
				Console.WriteLine(args.ErrorMessage);
				labelLoading.Text = args.ErrorMessage;
			}
			else
			{
				// Get Friends list
				m_networkService.onFinished -= OnGetFriendsList;
				m_networkService.onFinished += OnGetFriendsLeaderboard;
				m_networkService.GetFriendsLeaderboard( m_gameID, m_leaderboardID, (List<object>)args.Data["friends"] );
			}
		}
		
		public void OnGetFriendsLeaderboard( NetworkEventArgs args )
		{
			if( args.ErrorMessage != null )
			{
				//SetErrorMessage( args.ErrorMessage );
				labelLoading.Text = args.ErrorMessage;
				buttonFriends.Visible = false;
			}
			else
			{
				if (args.Data != null)
				{
					m_data.Add( args.Data[m_leaderboardID.ToString()] );
					buttonFriends.Visible = true;
					// Start leaderboard
					m_currentCategory = LeaderboardCategory.kFriends;
				}
			}
		}
		
		public void OnShown(object sender, object e)
		{
			if (!this.Loaded)
			{
				Console.WriteLine("Initializing Network Service!");
				InitNetworkService();
			}
		}
		
		public void OnShowing(object sender, object e)
		{
			this.IsVisible = true;
			buttonFriends.Visible = false;
		}

		public void OnHidden(object sender, object e)
		{
			indicator.Visible = true;
			labelLoading.Text = "Loading...";
			listPanel.Sections.Clear();
			listPanel.Sections = new ListSectionCollection {new ListSection("Section1", 0)};	
			this.Loaded = false;
			this.IsVisible = false;
			listPanel.Visible = false;
		}
		public void Timeout()
		{
			indicator.Visible = false;
			labelLoading.Text = "Timed out. Try again later.";
		}
		
		public void SetLeaderboardCategoryData()
		{
			switch (m_currentCategory)
			{
				case LeaderboardCategory.kDaily:
					curLeaderboardData = (List<object>)m_data[0];
					break;
				case LeaderboardCategory.kWeekly:
					curLeaderboardData = (List<object>)m_data[1];
					break;
				case LeaderboardCategory.kMonthly:
					curLeaderboardData = (List<object>)m_data[2];
					break;
				case LeaderboardCategory.kAllTime:
					curLeaderboardData = (List<object>)m_data[3];
					break;
				case LeaderboardCategory.kFriends:
					if (m_data.Count >= 5)
					{
						curLeaderboardData = (List<object>)m_data[4];
					}
					break;
				default:
					//curLeaderboardData = (List<object>)m_data[3];
					break;
			}
			//Console.WriteLine("COUNT" + curLeaderboardData.Count);
			listPanel.Sections.Clear();
			listPanel.Sections = new ListSectionCollection {new ListSection("Section1", curLeaderboardData.Count)};	
			if (curLeaderboardData.Count == 0)
			{
				listPanel.Visible = false;
				indicator.Visible = false;
			}
			else
			{
				listPanel.Visible = true;
			}
		}
	}
}
