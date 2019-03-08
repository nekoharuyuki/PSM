// AUTOMATICALLY GENERATED CODE

using System;
using System.Collections.Generic;
using System.Text;
using Sce.PlayStation.Core.Environment;

namespace CalendarMaker
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
                    "Make Calendar",
                    "See Wallpaper",
                    "Selection",
                    "Resize",
                    "Type",
                    "Positon",
                    "Close",
                    "Save",
                    "Cancel",
                    "Do you want to save?",
                    "OK",
                    "Please select an image.",
                    "Please adjust the size and position of the image.",
                    "Please select the type of a calendar.",
                    "Please adjust the position of the calendar.",
                },
                new string[]
                {
                    "カレンダーを作る",
                    "作成した壁紙を見る",
                    "選択",
                    "サイズ",
                    "タイプ",
                    "位置",
                    "閉じる",
                    "保存する",
                    "キャンセル",
                    "保存しますか？",
                    "決定",
                    "イメージを選択してください",
                    "イメージを調整してください",
                    "カレンダーの種類を選択してください",
                    "カレンダーの位置を決めてください",
                },
            };
            defaultLanguageId = UILanguage.English;
            currentLanguageId = defaultLanguageId;
            currentTable = textTable[(int)currentLanguageId];
        }
    }

    public enum UIStringID : int
    {
        RESID1 = 0,
        RESID2 = 1,
        RESID3 = 2,
        RESID4 = 3,
        RESID5 = 4,
        RESID6 = 5,
        RESID7 = 6,
        RESID8 = 7,
        RESID9 = 8,
        RESID10 = 9,
        RESID11 = 10,
        RESID12 = 11,
        RESID13 = 12,
        RESID14 = 13,
        RESID15 = 14,
    }

    public enum UILanguage : int
    {
        English = 0,
        Japanese = 1,
    }
}

