/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【ピンチイン】
///***************************************************************************
public class TutorialDataP09 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p09_01.png",
			"tutorial_p09_02.png",
		};

		pageMessStrTbl = new string[]{
			"Close to two fingers",
			"on the screen.",

			"The camera moves",
			"far from the player.",
		};

		pageMessNumTbl = new int[]{
			2, 2
		};
		return true;
	}

}

} // namespace
