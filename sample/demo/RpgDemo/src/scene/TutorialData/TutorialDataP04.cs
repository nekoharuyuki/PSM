/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using System.IO;

namespace AppRpg {


///***************************************************************************
//【移動中に画面を２度連続でタッチ】
///***************************************************************************
public class TutorialDataP04 : TutorialData
{

    /// 作成
    public override bool Init()
    {
		pageImgTbl = new string[]{
			"tutorial_p04_01.png",
			"tutorial_p04_02.png",
		};

		pageMessStrTbl = new string[]{
			"While the player is moving",
			"touch the screen twice.",

			"The player stop and wait.",
		};

		pageMessNumTbl = new int[]{
			2, 1
		};
		return true;
	}

}

} // namespace
