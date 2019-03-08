/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【ピンチアウト】
///***************************************************************************
public class TutorialDataP10 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p10_01.png",
			"tutorial_p10_02.png",
		};

		pageMessStrTbl = new string[]{
			"Spread two fingers",
			"on the screen.",

			"The camera moves",
			"closer to the player.",
		};

		pageMessNumTbl = new int[]{
			2, 2
		};
		return true;
	}

}

} // namespace
