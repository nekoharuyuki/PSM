/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Xml;


namespace RssApp
{

    public class RssFeed
    {
        private const string xmlTagTitle = "title";
        private const string xmlTagItem = "item";
        private const int itemNodeIndex = 0;

        private string rssTitle;
        private List<RssArticle> rssArticleList;
        private int unreadCount;

        public RssFeed()
        {
            rssTitle = "";
            rssArticleList = null;
            unreadCount = 0;
        }

        public void Load(string uri)
        {
            int rssItemCount;
            XmlDocument rssXml = new XmlDocument();
            XmlNodeList rssNodeList;

            rssXml.Load(uri);

            rssNodeList = rssXml.GetElementsByTagName(xmlTagTitle);
            rssTitle = FeedManager.ParseInnerText(rssNodeList[itemNodeIndex].InnerText);

            rssNodeList = rssXml.GetElementsByTagName(xmlTagItem);
            rssItemCount = rssNodeList.Count;

            rssArticleList = new List<RssArticle>();
            for (int i = 0; i < rssItemCount; i++)
            {
                RssArticle rssItem
                    = new RssArticle((XmlElement)rssNodeList.Item(i));
                rssArticleList.Add(rssItem);
            }

            unreadCount = rssItemCount;
        }

        public string RssTitle
        {
            get
            {
                return rssTitle;
            }
        }

        // get only
        public int ArticleCount
        {
            get
            {
                return rssArticleList.Count;
            }
        }

        public int UnreadCount
        {
            get
            {
                return unreadCount;
            }
        }

        // return instance of article class
        public RssArticle GetArticle(int index)
        {
            if (index < ArticleCount)
            {
                return rssArticleList[index];
            }
            else
            {
                return null;
            }
        }

        public void SetArticleRead(int index)
        {
            if (rssArticleList[index].AlreadyRead == false)
            {
                rssArticleList[index].AlreadyRead = true;
                unreadCount--;
            }
        }
    }
}

