/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Xml;


namespace RssApp
{
    public class RssArticle
    {
        private const string xmlTagTitle = "title";
        private const string xmlTaglink = "link";
        private const string xmlTagDescription = "description";
        private const int itemNodeIndex = 0;
        private const int noItemCount = 0;

        private string rssItemTitle;
        private string rssItemLink;
        private string rssItemDescription;
        private bool rssAlreadyRead;

        public RssArticle(System.Xml.XmlElement elm)
        {
            XmlNodeList rssNodeList;

            // ItemTitle
            rssNodeList = elm.GetElementsByTagName(xmlTagTitle);
            rssItemTitle = FeedManager.ParseInnerText(rssNodeList[itemNodeIndex].InnerText);

            // ItemLink
            rssNodeList = elm.GetElementsByTagName(xmlTaglink);
            rssItemLink = FeedManager.ParseInnerText(rssNodeList[itemNodeIndex].InnerText);

            // ItemDescription
            rssNodeList = elm.GetElementsByTagName(xmlTagDescription);
            if (rssNodeList.Count != noItemCount)
            {
                rssItemDescription = FeedManager.ParseInnerText(rssNodeList[itemNodeIndex].InnerText);
            }
            else
            {
                rssItemDescription = "";
            }

            AlreadyRead = false;
        }

        public string ArticleTitle
        {
            get
            {
                return rssItemTitle;
            }
        }

        public string ArticleDescription
        {
            get
            {
                return rssItemDescription;
            }
        }

        public string ArticleLink
        {
            get
            {
                return rssItemLink;
            }
        }

        public bool AlreadyRead
        {
            set
            {
                rssAlreadyRead = value;
            }
            get
            {
                return rssAlreadyRead;
            }
        }
    }
}

