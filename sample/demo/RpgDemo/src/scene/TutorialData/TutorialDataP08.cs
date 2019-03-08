/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【２本指でドラッグ】
///***************************************************************************
public class TutorialDataP08 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p08_01.png",
			"tutorial_p08_02.png",
		};

		pageMessStrTbl = new string[]{
			"Rub the screen",
			"by two fingers.",

			"The orientation",
			"of the camera is changed.",
		};

		pageMessNumTbl = new int[]{
			2, 2
		};
		return true;
	}

}

} // namespace
