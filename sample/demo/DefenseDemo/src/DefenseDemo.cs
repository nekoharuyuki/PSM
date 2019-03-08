///
/// SCE COMMENT
/// 

using System;

namespace DefenseDemo {

/// エントリクラス
static class DefenseDemo {

	static GameMain gameMain;

	/// エントリポイント
	static void Main( string[] args )
	{
		gameMain = new GameMain();
		gameMain.SetUpperLimitFps( 33 );
		gameMain.Run( args );
	}
}

} // end ns DefenseDemo
