/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using Sce.PlayStation.Core.Graphics ;

namespace DemoGame
{

public interface IScene {

    /// シーンの初期化
    bool Init(SceneManager useSceneMgr);

    /// シーンの破棄
    void Term();

    /// シーンの継続切り替え時の再開処理
    bool Restart();

    /// シーンの継続切り替え時の停止処理
    bool Pause();

    /// サスペンド＆レジューム処理
    void Suspend();
    void Resume();

    /// フレーム処理
    bool Update();

    /// 描画処理
    bool Render();

} // IScene

}
