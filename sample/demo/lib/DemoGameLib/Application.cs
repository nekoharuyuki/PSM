/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System.Threading ;
using Sce.PlayStation.Core.Environment ;
using Sce.PlayStation.Core.Graphics;

using System;
using System.Diagnostics;

using System.Collections;
using Sce.PlayStation.Core.Input;

namespace DemoGame
{


public class Application {

    private long prevFpsTime;
    private long waitFpsTime;
	private float nowFps;
	private float nowMs;

	protected GraphicsDevice	graphicsDevice;
	protected InputTouch		inputTouch;
	protected InputGamePad		inputGPad;

	private	Stopwatch stopwatch = new Stopwatch();


    /// コンストラクタ
    public Application()
    {
        SetUpperLimitFps( 60 );

		graphicsDevice		= new GraphicsDevice();
    }
    

/// 仮想メソッド
///---------------------------------------------------------------------------

    public virtual bool DoInit()
    {
        return true;
    }
    public virtual bool DoTerm()
    {
        return true;
    }
    public virtual bool DoUpdate()
    {
        return true;
    }
    public virtual bool DoRender()
    {
        return true;
    }
    public virtual void DoSuspend()
    {
    }
    public virtual void DoResume()
    {
    }


/// public メソッド
///---------------------------------------------------------------------------
	
	static bool loop = true;
		
    /// 実行メイン
    public void Run( string[] args )
    {
        /// 初期化
        init();

        while( loop ){

            /// アプリケーションの状態を更新
            SystemEvents.CheckEvents();

	        /// Fps制御
			long currTime = stopwatch.ElapsedMilliseconds;
			long diffTime	= currTime - prevFpsTime;

			if( diffTime < waitFpsTime && (waitFpsTime-diffTime) < waitFpsTime ){
				Thread.Sleep( (int)(waitFpsTime-diffTime) );
			}

			/// FPSを計測
			currTime	= stopwatch.ElapsedMilliseconds;
			if( (currTime-prevFpsTime) > 0 ){
				nowMs		= (currTime-prevFpsTime);
				nowFps		= 1000.0f / nowMs;
			}
			else{
				nowMs		= (currTime-prevFpsTime);
				nowFps		= 0.0f;
			}

			prevFpsTime	= currTime;

			// 入力情報更新
			inputTouch.Update();
			inputGPad.Update();

            /// 更新＆描画
            update();
            render();
        }

        /// 破棄
        term();
    }


    /// Fpsの上限値の設定
    public void SetUpperLimitFps( int LimitFps )
    {
        if( LimitFps > 0 ){
            waitFpsTime   = 1000 / LimitFps;
        }
        else{
            waitFpsTime = 0;
        }
    }
	/// 現在のFps値の取得
    public float GetFps()
    {
		return nowFps;
	}

	/// 動作速度（ミリ秒）の取得
    public float GetMs()
    {
		return nowMs;
	}

    /// タッチ入力取得
    public InputTouch GetInputTouch()
    {
		return inputTouch;
    }
    /// ゲームパッド入力取得
    public InputGamePad GetInputGamePad()
    {
		return inputGPad;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 初期化
    private bool init()
    {
		graphicsDevice.Init();

		/// インプット系クラス初期化
		inputTouch		= new InputTouch();
		inputGPad		= new InputGamePad();
		inputTouch.Init( 0, 2, graphicsDevice.DisplayWidth, graphicsDevice.DisplayHeight );
		inputGPad.Init( 0 );

		stopwatch.Start();
        prevFpsTime     = 0;

        return DoInit();
    }

    /// 破棄
    private void term()
    {
		DoTerm();

		/// インプット系クラス破棄
		inputTouch.Term();
		inputGPad.Term();

		inputGPad	= null;
		inputTouch	= null;

		stopwatch.Stop();

		graphicsDevice.Term();
		graphicsDevice = null;
    }

    /// フレーム処理
    private bool update()
    {
        return DoUpdate();
    }

    /// 描画処理
    private bool render()
    {
        return DoRender();
    }
}

} // Application
