/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.ComponentModel;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Imaging;

namespace TwitterSample
{
    public class TwitterUserProfileImageCacheStore
    {
        private object m_SyncObjectForDictionary = new object();
        private System.Collections.Generic.Dictionary<string, object> m_Dictionary = new System.Collections.Generic.Dictionary<string, object>();
        
        private Image m_DefaultImage = null;
        public Image DefaultImage
        {
            get
            {
                if (null == this.m_DefaultImage)
                {
                    this.m_DefaultImage = new Image("/Application/assets/default_profile_0_normal.png");
					this.m_DefaultImage.Decode();
                }
                return this.m_DefaultImage;
            }
        }
        
        public void RequestCache(TweetSharp.TwitterUser user, Action<Image> response)
        {
            var screenName = user.ScreenName;
            var extension = System.IO.Path.GetExtension(user.ProfileImageUrl);
            var fileName = screenName + extension;
            var filePath = "/Temp/" + fileName;
            
            var isExist = System.IO.File.Exists(filePath);
            if (isExist)
            {
                try
                {
                    var image = new Image(filePath);
					image.Decode();
                    response.Invoke(image);
                }
                catch (Exception)
                {
                    response.Invoke(this.DefaultImage);
                }
                
                return;
            }
            else
            {
                response.Invoke(this.DefaultImage);
            }
            
            lock (this.m_SyncObjectForDictionary)
            {
                if (this.m_Dictionary.ContainsKey(filePath)) { return; }
                this.m_Dictionary.Add(filePath, new object());
            }
            
            var bw = new BackgroundWorker();
            bw.DoWork +=
                (object bw_sender, DoWorkEventArgs bw_e) =>
                {	
                    var request = new System.Net.HttpWebRequest(new System.Uri(user.ProfileImageUrl));
                    using (var stream = request.GetResponse().GetResponseStream())
                    using (var file = System.IO.File.Create(filePath))
                    {
                        stream.CopyTo(file);
                    }               
                };
            bw.RunWorkerCompleted +=
                (object bw_sender, RunWorkerCompletedEventArgs bw_e) =>
                {
                    lock (this.m_SyncObjectForDictionary)
                    {
                        if (!this.m_Dictionary.ContainsKey(filePath)) { return; }
                        this.m_Dictionary.Remove(filePath);
                    }
                
                    Application.Current.Dispatcher.BeginInvoke(
                        () =>
                        {
                            try
                            {
                                var image = new Image(filePath);
								image.Decode();
                                response.Invoke(image);
                            }
                            catch (Exception)
                            {
                                response.Invoke(this.DefaultImage);
                            }
                        });
                };
            bw.RunWorkerAsync();
        }       
    }
}

