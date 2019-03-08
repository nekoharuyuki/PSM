using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Services;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.HighLevel.JsonHelper;

namespace ScoreboardSample
{
    public partial class Scoreboard : Scene
    {
		static NetworkResponseData scoreboardData = null;
		static NetworkResponseData friendScoreboardData = null;
		static string scoreboardToken = "dbf6101a-75f7-46f8-a880-63a99b5e833d";
				
        public Scoreboard()
        {
            InitializeWidget();
			
			listPanel.SetListItemUpdater(ListItemUpdater);
			ListSectionCollection section = new ListSectionCollection { new ListSection("", 0) };
			listPanel.Sections = section;
			listPanel.ShowItemBorder = false;
            listPanel.ShowSection = false;
            listPanel.ShowEmptySection = false;
			listPanel.UpdateItems();
						
			setScore.Enabled = false;
			setScore.ButtonAction += ButtonClick;
			
			typePopup.SelectionChanged += PopupChanged;
			
			GetScoreboard();
        }
		
		static bool init = false;
		protected override void OnUpdate(float elapsedTime)
		{
			if (Network.State == NetworkState.NetworkServerReady && init == false)
			{
				init = true;
				GetScoreboard();
			}
			
			if (Network.State != NetworkState.NetworkServerReady && setScore.Enabled == true)
			{
				setScore.Enabled = false;
				connectionText.Text = "Server status: not connected.";
			}
			else if (Network.State == NetworkState.NetworkServerReady && setScore.Enabled == false)
			{
				setScore.Enabled = true;
				connectionText.Text = "Server status: connected.";
			}
		}
		
		public void ButtonClick(object sender, TouchEventArgs e)
		{
			int n;
			bool isNumeric = int.TryParse(scoreText.Text, out n);
			if (isNumeric)
				SetScore(scoreText.Text);
			else
				MessageDialog.CreateAndShow(MessageDialogStyle.Ok, "Error", "Score must be an integer.");
		}
		
		public void PopupChanged(object sender, PopupSelectionChangedEventArgs e) 
		{
			int count = 0;
			if (typePopup.SelectedIndex > 0)
			{
				if (scoreboardData == null)
					return;
				count = scoreboardData.GetScoreCount(scoreboardToken, typePopup.SelectedIndex.ToString());
			}
			else
			{
				if (friendScoreboardData == null)
					return;
				count = friendScoreboardData.GetScoreCount(scoreboardToken, "f");	
			}
			ListSectionCollection section = new ListSectionCollection { new ListSection("", count) };
			listPanel.Sections = section;
			listPanel.UpdateItems();
		}
		
		private void ListItemUpdater(ListPanelItem item)
	    {
			if (scoreboardData == null || scoreboardData.scoreboard == null ||
			    friendScoreboardData == null || friendScoreboardData.scoreboard == null)
				return;
			
			ScoreItem score = (ScoreItem)item;
			
			NetworkResponseData.ScoreData data = new NetworkResponseData.ScoreData();
			if (typePopup.SelectedIndex > 0)
			{
				data = scoreboardData.GetScoreData(scoreboardToken, typePopup.SelectedIndex.ToString(), item.Index);
			}
			else
			{
				data = friendScoreboardData.GetScoreData(scoreboardToken, "f", item.Index);
			}

			
			if (data.user.Length > 0)
			{
				score.onUpdate((item.Index+1).ToString(), data.user, data.score);
			}			
		}
		
		void GetScoreboard()
		{
			if (Network.State == NetworkState.NetworkServerReady)
			{
				NetworkGetRequestData data = new NetworkGetRequestData();
				data.serviceType = NetworkServiceType.score_board;
				data.scoreboardTokens = new string[1] { scoreboardToken };
				data.scoreboardTypes = new NetworkScoreboardType[4] { NetworkScoreboardType.AllTime, NetworkScoreboardType.Monthly, NetworkScoreboardType.Weekly, NetworkScoreboardType.Daily };
				NetworkRequest request = JsonHelper.CreateRequest(data);
				request.BeginGetResponse(OnGetScoreboard);
			}
		}
				
		public void OnGetScoreboard( IAsyncResult rs )
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
	        NetworkResponse response = request.EndGetResponse(rs);
	        NetworkStreamReader reader = response.GetStreamReader();
	        string jsonResponse = reader.ReadToEnd();
			scoreboardData = JsonHelper.Parse(jsonResponse);
			reader.Close();
	        reader.Dispose();
	        response.Dispose();
	        request.Dispose();
			
			//////////////////////
			NetworkGetRequestData data = new NetworkGetRequestData();
			data.scoreboardTokens = new string[1] { scoreboardToken };
			data.serviceType = NetworkServiceType.friend_score_board;
			request = JsonHelper.CreateRequest(data);
			request.BeginGetResponse(OnGetFriendsScoreboard);
			
			//////////////////////
			if (typePopup.SelectedIndex > 0)
			{
				int count = scoreboardData.GetScoreCount(scoreboardToken, typePopup.SelectedIndex.ToString());
				ListSectionCollection section = new ListSectionCollection { new ListSection("", count) };
				listPanel.Sections = section;
				listPanel.UpdateItems();
			}
		}
		
		public void OnGetFriendsScoreboard( IAsyncResult rs)
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
	        NetworkResponse response = request.EndGetResponse(rs);
	        NetworkStreamReader reader = response.GetStreamReader();
	        string jsonResponse = reader.ReadToEnd();
			friendScoreboardData = JsonHelper.Parse(jsonResponse);
			reader.Close();
	        reader.Dispose();
	        response.Dispose();
	        request.Dispose();
			
			listPanel.UpdateItems();
			setScore.Enabled = true;
			connectionText.Text = "Server status: connected.";
		}
		
		static string previousScore = "";
		public void SetScore( string score )
		{
			if (previousScore == score)
				return;
			
			previousScore = score;
			if (Network.State == NetworkState.NetworkServerReady)
			{
				NetworkSetRequestData data = new NetworkSetRequestData();
				data.scoreboardToken = scoreboardToken;
				data.score = score;
				NetworkRequest request = JsonHelper.CreateRequest(data);
				request.BeginGetResponse(OnSetScore);	
			}
		}
		
		public void OnSetScore( IAsyncResult rs )
		{
			NetworkRequest request = rs.AsyncState as NetworkRequest;
			NetworkResponse response = request.EndGetResponse(rs);
			NetworkStreamReader reader = response.GetStreamReader();
	        string jsonResponse = reader.ReadToEnd();
	        reader.Dispose();
	        response.Dispose();
	        request.Dispose();
			
			// Pull updated scoreboards
			GetScoreboard();
		}		
    }
}
