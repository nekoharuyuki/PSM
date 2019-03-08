/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【敵や箱をタッチ】
///***************************************************************************
public class TutorialDataP02 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p02_01.png",
			"tutorial_p02_02.png",
			"tutorial_p02_03.png",
		};

		pageMessStrTbl = new string[]{
			"Touch enemies or objects.",

			"The marker appear",
			"on the target.",

			"The player moves",
			"to the marker",
			"and attack",
			"the target once.",
		};

		pageMessNumTbl = new int[]{
			1, 2, 4
		};
		return true;
	}

}

} // namespace
