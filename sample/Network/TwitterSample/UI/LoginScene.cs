/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Timers;
using System.ComponentModel;
using Sce.PlayStation.HighLevel.UI;
using TweetSharp;

namespace TwitterSample.UserInterface
{
    public partial class LoginScene : SceneBase
    {
        private bool m_IsProcessing = false;
		private Timer timer = new Timer();

		public LoginScene()
        {
            this.InitializeWidget();
			
            this.m_TitlePanel.ButtonLogin.ButtonAction +=
                (object sender, TouchEventArgs e) =>
                {
                    if (this.m_IsProcessing)
                    {
                    	return;
                    }
                    this.m_IsProcessing = true;
					timer.Interval = 30000;
					timer.Enabled = true;
					timer.AutoReset = false;
					timer.Elapsed += (object sender_t, ElapsedEventArgs e_t) =>
					{
						this.m_IsProcessing = false;
					};
					timer.Start();
                
                    if ((String.IsNullOrEmpty(TwitterSample.Application.TWITTER_CONSUMER_KEY)) || 
                        (String.IsNullOrEmpty(TwitterSample.Application.TWITTER_CONSUMER_SECRET)))
                    {
                        var consumerDialog = new TwitterSample.UserInterface.ConsumerDialog();
                    
                        this.IsDialogShown = true;
                    
                        consumerDialog.ButtonOK.ButtonAction +=
                            (object sender_ex, TouchEventArgs ex) =>
                            {
                                consumerDialog.Hide();
                                this.IsDialogShown = false;
                            };
                        consumerDialog.Show();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(this.m_TitlePanel.SettingProxy.Text))
                        {
                            System.Net.WebRequest.DefaultWebProxy = new System.Net.WebProxy(this.m_TitlePanel.SettingProxy.Text, true);
                        }
                        Application.Current.TwitterService.Proxy = this.m_TitlePanel.SettingProxy.Text;

                        this.RunAuthorization(true, true, true);
                    }
                };
        }
        
        protected override void OnShown ()
        {
            base.OnShown ();
        }
        
        private void RunAuthorization(
            bool isRunAuthWithAccessToken,
            bool isRunAuthWithRequestToken,
            bool isRunAuthWithNothing)
        {
            // RunAuthWithAccessToken
            if (isRunAuthWithAccessToken)
            {
                var accessToken = Application.Current.Settings.Model.AccessToken;
                
                var isValidAccessToken =
                    null != accessToken &&
                    !string.IsNullOrWhiteSpace(accessToken.Token) &&
                    !string.IsNullOrWhiteSpace(accessToken.TokenSecret);
                
                if (isValidAccessToken)
                {
                    Application.Current.TwitterService.AuthenticateWith(
                        accessToken.Token,
                        accessToken.TokenSecret);
                    
                    var scene = new MainScene();
                    UISystem.SetScene(scene);
                    
                    return;
                }
            }
            
            // RunAuthWithRequestToken
            if (isRunAuthWithRequestToken)
            {
                var requestToken = Application.Current.Settings.Model.RequestToken;
                
                var isValidRequestToken =
                    null != requestToken &&
                    !string.IsNullOrWhiteSpace(requestToken.Token) &&
                    !string.IsNullOrWhiteSpace(requestToken.TokenSecret);
                
                if (isValidRequestToken)
                {
                    var pinDialog = new PINDialog();
                    pinDialog.ButtonOK.ButtonAction +=
                        (object sender, TouchEventArgs e) =>
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    pinDialog.Hide();
                                    this.IsDialogShown = false;
                                
                                    var pin = pinDialog.EditableTextPIN.Text;

                                    var waitDialog = new WaitingDialog();
                                    this.IsDialogShown = true;
                                    waitDialog.Show();
                                    
                                    var bw = new BackgroundWorker();
                                    bw.DoWork +=
                                        (object bw_sender, DoWorkEventArgs bw_e) =>
                                        {
                                            var accessToken =
                                                Application.Current.TwitterService.GetAccessToken(
                                                    requestToken,
                                                    pin);
                                    
                                            bw_e.Result = accessToken;
                                        };
                                    bw.RunWorkerCompleted +=
                                        (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                                        {
                                            Application.Current.Dispatcher.BeginInvoke(
                                                () =>
                                                {
                                                    waitDialog.Hide();
                                                    this.IsDialogShown = false;
                                    
		                                            if (null != bw_e.Error || bw_e.Cancelled == true || 
                                                   !(bw_e.Result is OAuthAccessToken))
                                                    {
                                                        Application.Current.Settings.Delete();
                                                        Application.Current.Settings.Model.AccessToken = null;
                                                        Application.Current.Settings.Model.RequestToken = null;
                                                        return;
                                                    }
                                                var accessToken = bw_e.Result as OAuthAccessToken;
                                            
                                                    if (0 >= accessToken.UserId)
                                                    {
                                                        Application.Current.Settings.Delete();
                                                        Application.Current.Settings.Model.AccessToken = null;
                                                        Application.Current.Settings.Model.RequestToken = null;
                                                        return;
                                                    }
                                            
                                                    Application.Current.Settings.Model.AccessToken = accessToken;
                                                    Application.Current.Settings.Save();
                                                
                                                    this.RunAuthorization(true, false, false);
                                                });
                                        };
                                    bw.RunWorkerAsync();
                                });
                        };
                    pinDialog.ButtonCancel.ButtonAction +=
                        (object sender, TouchEventArgs e) =>
                        {
                            Application.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    pinDialog.Hide();
                                    this.IsDialogShown = false;
                                    Application.Current.Settings.Model.RequestToken = null;
                                    Application.Current.Settings.Save();
                                });
                        };
                    this.IsDialogShown = true;
                    pinDialog.Show();
                    
                    return;
                }
            }
            
            // RunAuthWithNothing
            if (isRunAuthWithNothing)
            {
                var waitDialog = new WaitingDialog();
                this.IsDialogShown = true;
                waitDialog.Show();
                
                var bw = new BackgroundWorker();
                bw.DoWork +=
                    (object bw_sender, DoWorkEventArgs bw_e) =>
                    {
                        var requestToken = Application.Current.TwitterService.GetRequestToken();
                        bw_e.Result = requestToken;
						this.m_IsProcessing = false;
						timer.Stop();
					};
                bw.RunWorkerCompleted +=
                    (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            () =>
                            {
                                waitDialog.Hide();
                                this.IsDialogShown = false;
                        
                                if (null != bw_e.Error || bw_e.Cancelled == true ||
                                   !(bw_e.Result is OAuthRequestToken))
                                {
                                    return;
                                }
                        
                                var requestToken = bw_e.Result as OAuthRequestToken;
                                Application.Current.Settings.Model.RequestToken = requestToken;
                                Application.Current.Settings.Save();
                        
                                var uri = Application.Current.TwitterService.GetAuthorizationUri(requestToken);
                        
                                var browserAction = Sce.PlayStation.Core.Environment.Shell.Action.BrowserAction(uri.ToString());
                                Sce.PlayStation.Core.Environment.Shell.Execute(ref browserAction);
                                
                                this.RunAuthorization(true, true, false);
                            });
                    };
                bw.RunWorkerAsync();
            }
        }
    }
}
