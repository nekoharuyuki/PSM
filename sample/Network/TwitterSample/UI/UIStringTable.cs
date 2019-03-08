// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using System.Text;
using Sce.PlayStation.Core.Environment;

namespace TwitterSample.UserInterface
{
    public static class UIStringTable
    {
        static string[] currentTable;
        static UILanguage currentLanguageId;
        static UILanguage defaultLanguageId;
        static string[][] textTable;

        public static string Get(UIStringID id)
        {
            return currentTable[(int)id];
        }

        public static UILanguage UILanguage
        {
            get { return currentLanguageId; }
            set
            {
                if (currentLanguageId != value)
                {
                    currentLanguageId = value;
                    currentTable = textTable[(int)currentLanguageId];
                }
            }
        }

        static UIStringTable()
        {
            textTable = new string[][]
            {
                new string[]
                {
                    "Login",
                    "Home",
                    "Cancel",
                    "OK",
                    "Search in the following characters",
                    "Tweets",
                    "Tweet",
                    "Search",
                    "Connect",
                    "Discover",
                    "Me",
                    "Option",
                    "TWEETS",
                    "FOLLOWING",
                    "FOLLOWERS",
                    "「TWITTER_CONSUMER_KEY」 or 「TWITTER CONSUMER_SECRET」 is not defined of Application.cs",
                    "SIGN OUT",
                },
                new string[]
                {
                    "ログイン",
                    "ホーム",
                    "Cancel",
                    "OK",
                    "以下の文字列で検索します",
                    "ツイート",
                    "ツイート",
                    "検索",
                    "つながり",
                    "見つける",
                    "アカウント",
                    "オプション",
                    "ツイート",
                    "フォロー",
                    "フォロワー",
                    "Application.cs の 「TWITTER_CONSUMER_KEY」 または 「TWITTER_CONSUMER_SECRET」が定義されていません",
                    "ログアウト",
                },
            };
            defaultLanguageId = UILanguage.English;
            currentLanguageId = defaultLanguageId;
            currentTable = textTable[(int)currentLanguageId];
        }
    }

    public enum UIStringID : int
    {
        RESID_LOGIN = 0,
        RESID_HOME = 1,
        RESID_CANCEL = 2,
        RESID_OK = 3,
        RESID_SEARCH_OPT = 4,
        RESID_TWEETS = 5,
        RESID_WRITE = 6,
        RESID_SEARCH = 7,
        RESID_CONNECT = 8,
        RESID_DISCOVER = 9,
        RESID_ME = 10,
        RESID_OPTION = 11,
        RESID_TWEET = 12,
        RESID_FOLLOWING = 13,
        RESID_FOLLOWER = 14,
        RESID_CONSUMER = 15,
        RESID_LOGOUT = 16,
    }

    public enum UILanguage : int
    {
        English = 0,
        Japanese = 1,
    }
}

