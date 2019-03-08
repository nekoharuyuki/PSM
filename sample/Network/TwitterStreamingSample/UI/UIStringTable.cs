// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using System.Text;
using Sce.PlayStation.Core.Environment;

namespace TwitterStreamingSample
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
                    "Setting",
                    "「TWITTER_CONSUMER_KEY」 or 「TWITTER CONSUMER_SECRET」 is not defined of Application.cs",
                    "SIGN OUT",
                },
                new string[]
                {
                    "ログイン",
                    "ホーム",
                    "Cancel",
                    "OK",
                    "設定",
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
        RESID_SETTING = 4,
        RESID_CONSUMER = 5,
        RESID_LOGOUT = 6,
    }

    public enum UILanguage : int
    {
        English = 0,
        Japanese = 1,
    }
}

