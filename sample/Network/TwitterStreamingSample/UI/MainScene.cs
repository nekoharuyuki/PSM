/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.IO;
using System.Collections.Generic;
using Sce.PlayStation.HighLevel.UI;
using System.ComponentModel;
using TweetSharp;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwitterStreamingSample
{
    public partial class MainScene : SceneBase
    {
		public Button SetButton { get{return this.m_SetButton;}}
		public Button LogOutButton { get{return this.m_ButtonLogOut;}}
		public BusyIndicator BusyIndicator{ get{return this.m_BusyIndicator;}}

		private List<StreamInfo> StreamData = new List<StreamInfo>();
		private SetParameterDialog SetDialog = new SetParameterDialog();
		private bool m_IsRejectFlg;
		private static TwitterUser profile;
		private static TwitterUser sm_usr = null;
		private System.Timers.Timer timer = new System.Timers.Timer();
		
		public TwitterUser usr
		{
			get
			{
				if(sm_usr == null)
				{
					sm_usr = Application.Current.TwitterService.GetUserProfile(new GetUserProfileOptions());
				}
				return sm_usr;
			}
		}
		
        public MainScene()
        {
            this.InitializeWidget();
			this.BusyIndicator.Visible = false;
			
			profile = usr;
			
			this.SetButton.ButtonAction +=
				(object sender, TouchEventArgs Environment) =>
				{
					if (String.IsNullOrEmpty(Application.Current.TwitterService.Proxy))
					{
						SetDialog.ButtonOk.Enabled = true;
					}
					else
					{
						SetDialog.ButtonOk.Enabled = false;
						timer.Interval = 30000;
						timer.Enabled = true;
						timer.AutoReset = false;
						timer.Elapsed += (object sender_t, System.Timers.ElapsedEventArgs e_t) =>
						{
							this.SetDialog.ButtonOk.Enabled = true;
						};
						timer.Start();
					}
					this.SetParam();
				};
			this.LogOutButton.ButtonAction +=
				(object sender, TouchEventArgs Environment) =>
				{
					this.LogOut();
				};
        }
		
		protected override void OnShown()
		{
			base.OnShown();
		}
		
		public void SetParam()
		{
			SetDialog.ButtonOk.ButtonAction +=
				(object sender, TouchEventArgs Environment) =>
			{
				Application.Current.TwitterService.CancelStreaming();
				Thread.Sleep(2000);

				SetDialog.Hide();
				this.IsDialogShown = false;
				this.BusyIndicator.Visible = true;
				
				List<string> Segment = new List<string>();
				Segment.Add(".json");
				
				Segment.Add("?follow=");
				if(profile == null)
				{
					Segment.Add("");
				}
				else
				{
					Segment.Add(profile.Id.ToString());
				}
				
				if(!String.IsNullOrWhiteSpace(SetDialog.ParamTrack.Text))
				{
					Segment.Add("&track=");
					Segment.Add(SetDialog.ParamTrack.Text);
				}
				
				string locations = "";
				if(SetDialog.ParamLocation.SelectedIndex != 0)
				{
					if(SetDialog.ParamLocation.SelectedIndex == 1)
					{
						locations = "-122.75,36.8,-121.75,37.8";
					}
					else
					{
						locations = "-74,40,-73,41";
					}
					Segment.Add("&locations=");
					Segment.Add(locations);
				}
				
				Segment.Add("&delimited=");
				Segment.Add("length");
				
				if(SetDialog.ParamWarning.SelectedIndex == 1)
				{
					Segment.Add("&stall_warnings=");
					Segment.Add("true");
				}
				
				string[] segments = Segment.ToArray();
				Application.Current.TwitterService.StreamFilter((streamEvent, response) =>
	            {
					if((!String.IsNullOrEmpty(response.Response)) &&
					   (response.Response.Contains("{\"created_at\":")) &&
					   (response.Response.Contains(",\"text\":")) &&
					   (response.Response.Contains(",\"screen_name\":")) &&
					   (response.Response.Contains(",\"profile_image_url\":")))
					{
						System.Threading.Thread.Sleep(1800);
						Application.Current.Dispatcher.BeginInvoke(
    		               	() =>
            		       	{
								this.BusyIndicator.Visible = false;
							});
						try
						{
							JObject jo = JObject.Parse(response.Response);
	
							this.m_IsRejectFlg = false;
	
							foreach(StreamInfo st in StreamData)
							{
								if(st.Id == jo["user"]["id"].ToString())
								{
									this.m_IsRejectFlg = true;
									break;
								}
							}
							
							if(this.m_IsRejectFlg == false || StreamData.Count == 0)
							{
								StreamInfo StInfo = new StreamInfo();
								
								StInfo.Id = jo["user"]["id"].ToString();
								StInfo.Name = jo["user"]["name"].ToString();
								StInfo.ScreenName = jo["user"]["screen_name"].ToString();
								StInfo.Time = jo["created_at"].ToString();
								StInfo.Text = jo["text"].ToString();
								StInfo.ProfileImgUrl = jo["user"]["profile_image_url"].ToString();
								StreamData.Insert(0, StInfo);
								
								Application.Current.Dispatcher.BeginInvoke(
		    		               	() =>
		            		       	{
										this.m_FeedsPanel.StreamList = StreamData;
									});
							}
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex);
							Application.Current.Dispatcher.BeginInvoke(
	    		               	() =>
	            		       	{
									this.BusyIndicator.Visible = false;
								});
						}
					}
	            }, segments);
				
			};
			SetDialog.ButtonCancel.ButtonAction +=
				(object sender, TouchEventArgs Environment) =>
			{
				SetDialog.Hide();
				this.IsDialogShown = false;
			};
			this.IsDialogShown = true;
			SetDialog.Show();
		}
		public void LogOut()
		{
			if (File.Exists(TwitterStreamingSample.Settings.SETTINGS_FILE_PATH))
            {
				FileInfo file = new FileInfo(TwitterStreamingSample.Settings.SETTINGS_FILE_PATH);
				
				if((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
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
    }
}
