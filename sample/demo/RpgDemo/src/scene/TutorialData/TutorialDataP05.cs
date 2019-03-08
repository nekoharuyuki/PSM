/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【(通常モードで)縦にフリック】
///***************************************************************************
public class TutorialDataP05 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p05_01.png",
			"tutorial_p05_02.png",
		};

		pageMessStrTbl = new string[]{
			"Flick the screen vertically",
			"in the normal mode.",

			"The player swing",
			"the sword vertically.",
		};

		pageMessNumTbl = new int[]{
			2, 2
		};
		return true;
	}

}

} // namespace
