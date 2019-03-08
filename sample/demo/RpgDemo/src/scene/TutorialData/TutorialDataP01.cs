/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【地面をタッチ】
///***************************************************************************
public class TutorialDataP01 : TutorialData
{
    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p01_01.png",
			"tutorial_p01_02.png",
			"tutorial_p01_03.png",
		};

		pageMessStrTbl = new string[]{
			"Touch ground.",

			"The marker appear.",

			"The player moves",
			"to the marker.",
		};

		pageMessNumTbl = new int[]{
			1, 1, 2
		};
		return true;
	}
}

} // namespace
