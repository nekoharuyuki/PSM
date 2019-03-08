/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using DemoGame;

namespace Physics2dDemo
{

///
/// InputManagerクラス
///
public static class InputManager
{
    private static InputGamePad inputGamePad;
    private static InputTouch inputTouch;

    /// 初期化
    public static bool Init(InputGamePad inputGamePad, InputTouch inputTouch)
    {
        InputManager.inputGamePad = inputGamePad;
        InputManager.inputTouch = inputTouch;

        return true;
    }

    /// 解放
    public static void Term()
    {
        InputManager.inputGamePad = null;
        InputManager.inputTouch = null;
    }

    /// InputGamePad
    public static InputGamePad InputGamePad
    {
        get {return inputGamePad;}
    }

    ///　InputTouch
    public static InputTouch InputTouch
    {
        get {return inputTouch;}
    }
}

} // Physics2dDemo
