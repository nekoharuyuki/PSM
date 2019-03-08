/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace Physics2dDemo
{

///
/// Physics2dDemo
///
public static class Physics2dDemo
{
	private static GameMain appMain;
		
	/// エントリーポイント
    public static void Main(string[] args)
    {
		appMain = new GameMain();
		appMain.SetUpperLimitFps(30);
		appMain.Run( args );
	}
}
	
} // Physics2dDemo