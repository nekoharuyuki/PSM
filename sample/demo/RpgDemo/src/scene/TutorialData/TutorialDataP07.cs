/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【(FPSモードで)フリック】
///***************************************************************************
public class TutorialDataP07 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p07_01.png",
			"tutorial_p07_02.png",
		};

		pageMessStrTbl = new string[]{
			"Flick the screen",
			"in the FPS mode.",

			"The player shoot",
			"the magic bullet.",
		};

		pageMessNumTbl = new int[]{
			2, 2
		};
		return true;
	}

}

} // namespace
