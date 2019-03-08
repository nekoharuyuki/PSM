/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【空をタッチ】
///***************************************************************************
public class TutorialDataP03 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p03_01.png",
			"tutorial_p03_02.png",
		};

		pageMessStrTbl = new string[]{
			"Touch sky.",

			"The player moves",
			"to straight.",
		};

		pageMessNumTbl = new int[]{
			1, 2
		};
		return true;
	}

}

} // namespace
