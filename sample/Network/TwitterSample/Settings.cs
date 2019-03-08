/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System.Xml.Serialization;

namespace TwitterSample
{
    public class Settings
    {
        public static readonly string SETTINGS_FILE_PATH = "/Documents/setting.xml";
        
        public SettingsModel Model { get; set; }
        
        public Settings ()
        {
            this.Model = new SettingsModel();
        }
        
        public void Load()
        {
            if (!System.IO.File.Exists(SETTINGS_FILE_PATH))
            {
                this.Model = new SettingsModel();
                return;
            }
            
            var serializer = new XmlSerializer(typeof(SettingsModel)); 
            using (var file = System.IO.File.OpenRead(SETTINGS_FILE_PATH))
            {
                this.Model = serializer.Deserialize(file) as SettingsModel;
            }
        }
        
        public void Save()
        {
            var serializer = new XmlSerializer(typeof(SettingsModel)); 
            using (var file = System.IO.File.Create(SETTINGS_FILE_PATH))
            {
                serializer.Serialize(file, this.Model);
            }
        }
        
        public void Delete()
        {
            var file = new System.IO.FileInfo(SETTINGS_FILE_PATH);
            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
    
    public class SettingsModel
    {
        public TweetSharp.OAuthRequestToken RequestToken { get; set; }
        public TweetSharp.OAuthAccessToken AccessToken { get; set; }
    }
}

