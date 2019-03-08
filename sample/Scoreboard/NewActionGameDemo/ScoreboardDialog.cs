using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Services;
using Sce.PlayStation.HighLevel.JsonHelper;
using Sce.PlayStation.HighLevel.UI;

namespace SirAwesome
{
    public partial class ScoreboardDialog : Dialog
    {
		Button currentButton;
		const string appToken = "af1c0a1b-a7b8-4597-a022-eee91e6735d1";
		const string scoreboardName = "7d3c213d-8c7b-48ce-90d9-e33b2f0bb9ab";
		static NetworkResponseData m_data = null;
		static NetworkResponseData m_friendData = null;
		static NetworkRequest m_request = null;
		DateTime m_timer = DateTime.MinValue;

        public ScoreboardDialog()
            : base(null, null)
        {
            InitializeWidget();
			
			InitNetworkService();
			
			buttonOK.ButtonAction += new EventHandler<TouchEventArgs>(Button_OK_ButtonAction);
			buttonDaily.ButtonAction += new EventHandler<TouchEventArgs>(Button_Daily_ButtonAction);
			buttonWeekly.ButtonAction += new EventHandler<TouchEventArgs>(Button_Weekly_ButtonAction);
			buttonMonthly.ButtonAction += new EventHandler<TouchEventArgs>(Button_Monthly_ButtonAction);
			buttonAllTime.ButtonAction += new EventHandler<TouchEventArgs>(Button_AllTime_ButtonAction);
			buttonFriends.ButtonAction += new EventHandler<TouchEventArgs>(Button_Friends_ButtonAction);
			
			currentButton = buttonAllTime;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			
			scoreboardListPanel.SetListItemUpdater(ListItemUpdater);
			ListSectionCollection section = new ListSectionCollection { new ListSection("", 0) };
			scoreboardListPanel.Sections = section;
        }
		
		void GetScoreboard()
		{
			if (Network.State == NetworkState.NetworkServerReady)
			{			
				NetworkGetRequestData data = new NetworkGetRequestData();
				data.serviceType = NetworkServiceType.score_board;
				data.scoreboardTokens = new string[1] { ScoreboardDialog.scoreboardName };
				data.scoreboardTypes = new NetworkScoreboardType[4] { NetworkScoreboardType.AllTime, NetworkScoreboardType.Monthly, NetworkScoreboardType.Weekly, NetworkScoreboardType.Daily };
				data.limit = 50;
				m_request = JsonHelper.CreateRequest(data);
				m_request.BeginGetResponse(OnGetScoreboard);
			}
		}
				
		public void OnGetScoreboard( IAsyncResult rs )
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
	        NetworkResponse response = request.EndGetResponse(rs);
	        NetworkStreamReader reader = response.GetStreamReader();
	        string jsonResponse = reader.ReadToEnd();
			m_data = JsonHelper.Parse(jsonResponse);
			reader.Close();
	        reader.Dispose();
	        response.Dispose();
	        request.Dispose();
			
			//////////////////////
			NetworkGetRequestData data = new NetworkGetRequestData();
			data.scoreboardTokens = new string[1] { ScoreboardDialog.scoreboardName };
			data.serviceType = NetworkServiceType.friend_score_board;
			m_request = JsonHelper.CreateRequest(data);
			m_request.BeginGetResponse(OnGetFriendsScoreboard);
			
			//////////////////////
			OnHaveData();
		}
		
		public void OnGetFriendsScoreboard( IAsyncResult rs)
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
	        NetworkResponse response = request.EndGetResponse(rs);
	        NetworkStreamReader reader = response.GetStreamReader();
	        string jsonResponse = reader.ReadToEnd();
			m_friendData = JsonHelper.Parse(jsonResponse);
			if (m_data.error.Length > 0)
				Console.WriteLine("OnGetScoreboard error: {0}", m_data.error);			
			reader.Close();
	        reader.Dispose();
	        response.Dispose();
	        request.Dispose();
			
			m_request = null;
		}
		
		public void SetScore( string score )
		{
			if (Network.State == NetworkState.NetworkServerReady)
			{			
				NetworkSetRequestData data = new NetworkSetRequestData();
				data.scoreboardToken = ScoreboardDialog.scoreboardName;
				data.score = score;
				//data.metadata = "metadata";
				m_request = JsonHelper.CreateRequest(data);
				m_request.BeginGetResponse(OnSetScore);	
			}
		}
		
		public void OnSetScore( IAsyncResult rs )
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
			request.Dispose();
			m_request = null;
			
			// Pull updated scoreboards
			GetScoreboard();
		}		
		
		private void InitNetworkService()
		{
			if (m_data == null)
			{
				JsonHelper.Initialize(appToken);
				Network.AuthGetTicket();
				SetTimer();
			}
		}
		

		protected override void OnUpdate(float elapsedTime)
		{
			CheckGetScoreboard();
		}
		
		static bool init = false;
		public void CheckGetScoreboard()
		{
			if (Network.State == NetworkState.NetworkServerReady && init == false)
			{
				init = true;
				GetScoreboard();
			}
			else if (m_data == null && m_timer != DateTime.MinValue && busyAwaitingScores.Visible && (DateTime.Now - m_timer).TotalMilliseconds > 30000)
			{
				busyAwaitingScores.Stop();
				busyAwaitingScores.Visible = false;
				errorLabel.Visible = true;
			}
		}		
		
		public void SetTimer()
		{
			if (m_data == null && m_timer == DateTime.MinValue)
			{
				m_timer = DateTime.Now;
			}
		}
		
		private void OnHaveData()
		{
			if (m_data != null)
			{
				busyAwaitingScores.Stop();
				busyAwaitingScores.Visible = false;
				errorLabel.Visible = false;
				
				nameLabel.Text = m_data.username + "'s";
				
				if (currentButton == buttonDaily)
					Button_Daily_ButtonAction(this, null);
				else if (currentButton == buttonWeekly)
					Button_Weekly_ButtonAction(this, null);
				else if (currentButton == buttonMonthly)
					Button_Monthly_ButtonAction(this, null);
				else if (currentButton == buttonAllTime)
					Button_AllTime_ButtonAction(this, null);				
				else if (currentButton == buttonFriends)
					Button_Friends_ButtonAction(this, null);				
			}
		}
		
	    private void ListItemUpdater(ListPanelItem item)
	    {
			ScoreListItem score = (ScoreListItem)item;
			
			if (m_data == null)
			{
				if (m_request == null)
				{
					GetScoreboard();
				}
				return;
			}
			
			
			string sb = "";
			if (currentButton == buttonAllTime)
				sb = ((int)NetworkScoreboardType.AllTime).ToString();
			else if (currentButton == buttonMonthly)
				sb = ((int)NetworkScoreboardType.Monthly).ToString();
			else if (currentButton == buttonWeekly)
				sb = ((int)NetworkScoreboardType.Weekly).ToString();
			else if (currentButton == buttonDaily)
				sb = ((int)NetworkScoreboardType.Daily).ToString();
			else if (currentButton == buttonFriends)
				sb = "f";
			else
			{
				return;
			}
				
			NetworkResponseData.ScoreData data = GetItem(sb, item.Index);
			if (data.user.Length > 0)
			{
				score.onUpdate((item.Index+1).ToString(), data.user, data.score);
			}
	    }
		
		int GetItemCount(string scoreboardType)
		{
			if (scoreboardType != "f")
			{
				if (m_data != null && 
					m_data.scoreboard != null &&
					m_data.scoreboard.ContainsKey(ScoreboardDialog.scoreboardName) &&
					m_data.scoreboard[ScoreboardDialog.scoreboardName].ContainsKey(scoreboardType))
						return m_data.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType].Count;
			}
			else
			{
				if (m_friendData != null && 
					m_friendData.scoreboard != null &&
					m_friendData.scoreboard.ContainsKey(ScoreboardDialog.scoreboardName) &&
					m_friendData.scoreboard[ScoreboardDialog.scoreboardName].ContainsKey(scoreboardType))
						return m_friendData.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType].Count;				
			}
			return 0;
		}
		
		NetworkResponseData.ScoreData GetItem(string scoreboardType, int index)
		{
			if (scoreboardType != "f")
			{
				if (m_data != null &&
					m_data.scoreboard != null &&
					m_data.scoreboard.ContainsKey(ScoreboardDialog.scoreboardName) &&
					m_data.scoreboard[ScoreboardDialog.scoreboardName].ContainsKey(scoreboardType) &&
					m_data.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType].Count > index)
						return m_data.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType][index];
			}
			else
			{
				if (m_friendData != null &&
					m_friendData.scoreboard != null &&
					m_friendData.scoreboard.ContainsKey(ScoreboardDialog.scoreboardName) &&
					m_friendData.scoreboard[ScoreboardDialog.scoreboardName].ContainsKey(scoreboardType) &&
					m_friendData.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType].Count > index)
						return m_friendData.scoreboard[ScoreboardDialog.scoreboardName][scoreboardType][index];				
			}
			
			return new NetworkResponseData.ScoreData();
		}
		
		void Button_Daily_ButtonAction(object sender, TouchEventArgs e)
        {
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			currentButton = buttonDaily;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			scoreboardListPanel.Sections.Clear();
			ListSectionCollection section = new ListSectionCollection { new ListSection("", GetItemCount("1")) };
			scoreboardListPanel.Sections = section;	
			scoreboardListPanel.UpdateItems();
        }
		void Button_Weekly_ButtonAction(object sender, TouchEventArgs e)
        {
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			currentButton = buttonWeekly;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			scoreboardListPanel.Sections.Clear();
			ListSectionCollection section = new ListSectionCollection { new ListSection("", GetItemCount("2")) };
			scoreboardListPanel.Sections = section;	
			scoreboardListPanel.UpdateItems();
        }
		void Button_Monthly_ButtonAction(object sender, TouchEventArgs e)
        {
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			currentButton = buttonMonthly;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			scoreboardListPanel.Sections.Clear();
			ListSectionCollection section = new ListSectionCollection { new ListSection("", GetItemCount("3")) };
			scoreboardListPanel.Sections = section;	
			scoreboardListPanel.UpdateItems();
        }
		void Button_AllTime_ButtonAction(object sender, TouchEventArgs e)
        {
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			currentButton = buttonAllTime;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			scoreboardListPanel.Sections.Clear();
			ListSectionCollection section = new ListSectionCollection { new ListSection("", GetItemCount("4")) };
			scoreboardListPanel.Sections = section;	
			scoreboardListPanel.UpdateItems();
        }
		void Button_Friends_ButtonAction(object sender, TouchEventArgs e)
        {
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
			currentButton = buttonFriends;
			currentButton.TextColor = new UIColor(255f / 255f, 255f / 255f, 0f / 255f, 255f / 255f);
			scoreboardListPanel.Sections.Clear();
			ListSectionCollection section = new ListSectionCollection { new ListSection("", GetItemCount("f")) };
			scoreboardListPanel.Sections = section;	
			scoreboardListPanel.UpdateItems();
        }
		
		void Button_OK_ButtonAction(object sender, TouchEventArgs e)
        {
            this.Hide();
        }
    }
}
