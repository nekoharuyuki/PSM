/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

namespace FlightDemo {

/// エントリクラス
static class FlightDemo {

	static GameMain gameMain;

	/// エントリポイント
	static void Main( string[] args )
	{
		gameMain = new GameMain();
		gameMain.SetUpperLimitFps( 30 );
		gameMain.Run( args );
	}
}

} // end ns FlightDemo
