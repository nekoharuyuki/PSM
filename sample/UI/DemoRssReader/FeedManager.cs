/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace RssApp
{
    class FeedManager
    {
        private List<RssFeed> rssFeedList;

        public FeedManager()
        {
            // instead of regisration, use fixed uri
            string[] feedsFiles = Directory.GetFiles("/Application/rss_1", "*.xml");

            rssFeedList = new List<RssFeed>();
            foreach (string s in feedsFiles)
            {
                RssFeed rssFeed = new RssFeed();
                try
                {
                    rssFeed.Load(s);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("exception = " + e.Message);
                    throw e;
                }
                rssFeedList.Add(rssFeed);
            }
        }

        public void AddFeed(string uri)
        {
            // TODO : later...
        }

        public void RemoveFeed(string uri)
        {
            // TODO : later...
        }

        // get only
        public int FeedCount
        {
            get
            {
                return rssFeedList.Count;
            }
        }

        // return instance of RssFeed class
        public RssFeed GetRssFeed(int index)
        {
            if (index < FeedCount)
            {
                return rssFeedList[index];
            }
            else
            {
                return null;
            }
        }

        public void Register()
        {
            // TODO : later...
            //        register purchased feed uris.
        }
		
		public static string ParseInnerText(string innerText)
		{
            string result = "";
			var reader = new XmlTextReader(new StringReader("<html>" + innerText + "</html>"));
			while(reader.Read ())
			{
                if (reader.NodeType == XmlNodeType.Text)
                {
                    result += reader.Value;
                }
			}
            return result;
		}
    }
}

