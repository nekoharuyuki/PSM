/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【(通常モードで)横にフリック】
///***************************************************************************
public class TutorialDataP06 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p06_01.png",
			"tutorial_p06_02.png",
		};

		pageMessStrTbl = new string[]{
			"Flick the screen",
			"horizontally",
			"in the normal mode.",

			"The player swing",
			"the sword horizontally.",
		};

		pageMessNumTbl = new int[]{
			3, 2
		};
		return true;
	}

}

} // namespace
