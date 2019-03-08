/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {


///***************************************************************************
/// アプリ依存の入力処理
///***************************************************************************
public class AppInput
{

    ///---------------------------------------------------------------------------
    /// 入力イベントID
    ///---------------------------------------------------------------------------
    public enum EventId{
        Move        = (1 << 0),        /// プレイヤー移動
        AttackLR    = (1 << 1),        /// プレイヤー攻撃１
        AttackRL    = (1 << 2),        /// プレイヤー攻撃２
        AttackUD    = (1 << 3),        /// プレイヤー攻撃２
        AttackDU    = (1 << 4),        /// プレイヤー攻撃２
        SpelAtk     = (1 << 5),        /// プレイヤー魔法
        Jump        = (1 << 6),        /// プレイヤージャンプ
        CamRot      = (1 << 7),        /// カメラの回転上
        CamZoomIn   = (1 << 8),        /// カメラのズームイン
        CamZoomOut  = (1 << 9),        /// カメラのズームアウト
        DestinationMove   = (1 <<10),  /// 移動先決定
        DestinationCancel = (1 <<11),  /// 移動先キャンセル
    };

    private struct cTapParam
    {
        public ImagePosition                 Pos;
        public int                           TouchId;
    }


    private static AppInput instance = new AppInput();

    private DemoGame.InputGamePad        useGPad;
    private DemoGame.InputTouch          useTouch;

    private int                          scrWidth;
    private int                          scrHeight;

    private EventId                      eventId;
    private float                        plRotY;
    private float                        camRotX;
    private float                        camRotY;
    private float                        camDis;

    private bool                         touchRelease;
    private int                          backTouchNum;

    /// シングルタッチ
    private bool                         singleTouchFlg;
    private bool                         singleFlicFlg;
    private int                          singleDTapCnt;
    public ImagePosition                 singleDTapBackPos;

    /// マルチタッチ
    private cTapParam                    singleTapParam;
    private cTapParam[]                  multiTapParam;
    private bool                         multiZoomModeFlg;
    private float                        multiBackDis;

    private bool                         plBehindFlg;


    /// インスタンスの取得
    public static AppInput GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( DemoGame.InputGamePad pad, DemoGame.InputTouch touch, DemoGame.GraphicsDevice gDev )
    {
        useGPad        = pad;
        useTouch       = touch;
        plRotY         = 0.0f;
        camRotX        = 0.0f;
        camRotY        = 0.0f;
        camDis         = 0.0f;
        eventId        = 0;
        touchRelease   = false;
        singleTouchFlg = false;
        singleDTapCnt  = 0;
        backTouchNum   = 0;
		plBehindFlg    = false;

        multiTapParam   = new cTapParam[2];
        singleTapParam.Pos.X = 0;
        singleTapParam.Pos.Y = 0;

        scrWidth     = gDev.DisplayWidth;
        scrHeight    = gDev.DisplayHeight;
        return true;
    }

    /// 破棄
    public void Term()
    {
        eventId     = 0;
        useGPad     = null;
        useTouch    = null;
        multiTapParam  = null;
    }

    /// フレーム処理
    public void Frame()
    {
        eventId = 0;

        camRotX = 0.0f;
        camRotY = 0.0f;
        camDis    = 0.0f;
        touchRelease = false;

        /// 移動判定
        checkPadPlMove();

        /// 攻撃判定
        checkPadAttack();

        /// ジャンプ判定
        checkPadJump();

        /// カメラの回転判定
        checkPadCamRot();


        /// タップ判定
        ///-------------------------------------------------------
        if( backTouchNum == 0 ){
            if( useTouch.GetInputNum() > 0 ){
                touchRelease = true;
            }
        }


        /// ニュートラル状態
        ///-------------------------------------------------------
        if( useTouch.GetInputNum() == 0 ){

            if( singleTouchFlg ){

                /// フリックしていたら無効
                if( singleFlicFlg ){
                    singleDTapCnt  = 0;
                }

                /// ダブルタップで移動キャンセル
                else if( checkDTouch() ){
                    singleDTapCnt  = 0;
                }

                /// 目的地セット
                else{
                     eventId |= EventId.DestinationMove;
                    singleDTapCnt  = 30;
                }

                singleTouchFlg = false;
            }
        }

        /// シングルタッチ系の判定
        ///-------------------------------------------------------
        else if( useTouch.GetInputNum() == 1 ){

            /// タッチした直後のパラメータセット
            if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Down ){
                setSingleTouchParam();
            }

            /// 攻撃判定
            if( singleTouchFlg && !singleFlicFlg ){
                checkTouchAttack();
            }
        }

        /// マルチタッチ系の判定
        ///-------------------------------------------------------
        else{
            /// マルチタッチした直後のパラメータセット
            if( backTouchNum != 2 ){
                setMultiTouchBackDis();
                setMultiTouchBackPos();
                multiZoomModeFlg = false;
                singleTouchFlg   = false;
            }

            /// ズーム判定
//            if( !multiZoomModeFlg ){
                checkTouchCamZoom();
//            }

            /// カメラの回転判定
            checkTouchCamRot();

            singleDTapCnt  = 0;
        }

        if( singleDTapCnt > 0 ){
            singleDTapCnt --;
        }
        backTouchNum  = useTouch.GetInputNum();
    }


    /// プレイヤーの向きの取得
    public float GetPlRotY()
    {
        return plRotY;
    }

    /// フリック入力逆転のセット
    public void SetPlBehind( bool flg )
    {
		plBehindFlg = flg;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouchUp()
    {
		if( useTouch.GetInputNum() == 1 ){
			if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Up ){
				return true;
			}
		}
		return false;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouching()
    {
		if( useTouch.GetInputNum() == 1 ){
			return true;
		}
		return false;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouchDown()
    {
		if( useTouch.GetInputNum() == 1 ){
			if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Down ){
				return true;
			}
		}
		return false;
	}
    
/// private メソッド
///---------------------------------------------------------------------------

    /// プレイヤーの移動チェック
    private void checkPadPlMove()
    {
        plRotY = 0.0f;
        eventId |= EventId.Move;

        /// ８方向の入力チェック
        if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 45.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 135.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 225.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 315.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 ){
            plRotY = 0.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 90.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 ){
            plRotY = 180.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 270.0f;
        }

        /// アナログパッド入力
        else if( useGPad.AnalogLeftX != 0.0f || useGPad.AnalogLeftY != 0.0f ){
            plRotY = (float)( Math.Atan2( useGPad.AnalogLeftX, useGPad.AnalogLeftY ) / (3.141593f / 180.0) );
            plRotY += 180.0f;
            if( plRotY >= 360.0f ){
                plRotY -= 360.0f;
            }
        }

        else{
            eventId &= ~EventId.Move;
        }
    }

    private void checkPadCamRot()
    {
        /// パッド入力チェック
        ///----------------------------------------------------------

        /// ８方向の入力チェック
        if( (useGPad.Scan & DemoGame.InputGamePadState.L) != 0 ){
            camRotY = 2.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.R) != 0 ){
            camRotY = -2.0f;
        }

        /// アナログパッド入力
        else if( (Math.Abs(useGPad.AnalogRightX) != 0.0f) || (Math.Abs(useGPad.AnalogRightY) != 0.0f) ){
            camRotX = 1.25f * useGPad.AnalogRightY;
            camRotY = -1.25f * useGPad.AnalogRightX;
        }


    }


    /// 攻撃チェック
    private void checkPadAttack()
    {
        if( (useGPad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            eventId |= EventId.AttackLR;
            eventId |= EventId.SpelAtk;
        }
    }


    /// ジャンプチェック
    private void checkPadJump()
    {
        if( (useGPad.Trig & DemoGame.InputGamePadState.Cross) != 0 ){
            eventId |= EventId.Jump;
        }
    }



    /// フリック攻撃チェック
    private void checkTouchAttack()
    {
        int moveX = 0;
        int moveY = 0;

        int j;


        for( j=0; j<useTouch.GetInputNum(); j++ ){

            if( singleTapParam.TouchId == useTouch.GetInputId( j ) ){
                moveX += (singleTapParam.Pos.X - useTouch.GetScrPosX( j ));
                moveY += (singleTapParam.Pos.Y - useTouch.GetScrPosY( j ));
                break;
            }
        }
        if( j >= useTouch.GetInputNum() ){
            return ;
        }

        float perX = ((float)moveX / (float)scrWidth);
        float perY = ((float)moveY / (float)scrHeight);

		if( plBehindFlg ){
			/// 右→左
			if( perX > 0.3f ){
	            eventId |= EventId.AttackRL;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
	        }
			/// 左→右
	        else if( perX < -0.3f ){
	            eventId |= EventId.AttackLR;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
			}
		}
		else{
			/// 右→左
			if( perX > 0.3f ){
	            eventId |= EventId.AttackRL;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
	        }
			/// 左→右
	        else if( perX < -0.3f ){
	            eventId |= EventId.AttackLR;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
			}
		}

		if( singleFlicFlg == false ){
			/// 上→下
	        if( perY < -0.3f ){
	            eventId |= EventId.AttackUD;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
	        }
			/// 下→上
			else if( perY > 0.3f ){
	            eventId |= EventId.AttackDU;
	            eventId |= EventId.SpelAtk;
	            singleFlicFlg = true;
			}
		}
    }


    /// カメラの回転チェック
    private void checkTouchCamRot()
    {
        int moveX = 0;
        int moveY = 0;

        for( int i=0; i<2; i++ ){
            int j;
            for( j=0; j<useTouch.GetInputNum(); j++ ){

                if( multiTapParam[i].TouchId == useTouch.GetInputId( j ) ){
                    moveX += (multiTapParam[i].Pos.X - useTouch.GetScrPosX( j ));
                    moveY += (multiTapParam[i].Pos.Y - useTouch.GetScrPosY( j ));
                    break;
                }
            }

            if( j >= useTouch.GetInputNum() ){
                setMultiTouchBackPos();
                return ;
            }
        }

        camRotY = ((float)moveX / (float)scrWidth) * 60.0f;
        camRotX = ((float)moveY / (float)scrHeight) * -60.0f;
        setMultiTouchBackPos();
    }



    /// カメラズームチェック
    private bool checkTouchCamZoom()
    {
        /// カメラズームインチェック
        if( (useGPad.Scan & DemoGame.InputGamePadState.Triangle) != 0 ){
            camDis = -0.2f;
            eventId |= EventId.CamZoomIn;
            return true;
        }

        /// カメラズームアウトチェック
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Square) != 0 ){
            camDis = 0.2f;
            eventId |= EventId.CamZoomOut;
            return true;
        }

        ImagePosition calPos;
        float disPer;

        calPos.X = useTouch.GetScrPosX( 0 ) - useTouch.GetScrPosX( 1 );
        calPos.Y = useTouch.GetScrPosY( 0 ) - useTouch.GetScrPosY( 1 );

        if( FMath.Abs( calPos.X ) > FMath.Abs( calPos.Y ) ){
            disPer = (multiBackDis - FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) )) / scrWidth;
        }
        else{
            disPer = (multiBackDis - FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) )) / scrHeight;
        }

        if( disPer < 0.2f || (multiZoomModeFlg && disPer < 0.0f) ){
	        camDis += (disPer*4);
	        multiBackDis    = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) );

			multiZoomModeFlg = true;
            eventId |= EventId.CamZoomIn;
            return true;
        }
        else if( disPer > 0.2f || (multiZoomModeFlg && disPer > 0.0f) ){
	        camDis += (disPer*4);
	        multiBackDis    = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) );

			multiZoomModeFlg = true;
            eventId |= EventId.CamZoomOut;
            return true;
        }

        return false;
    }


    /// ダブルタップチェック
    private bool checkDTouch()
    {
        ImagePosition calPos;
        float disPer;

        if( singleDTapCnt <= 0 ){
            return false;
        }
            
        calPos.X = useTouch.GetScrPosX( 0 ) - singleDTapBackPos.X;
        calPos.Y = useTouch.GetScrPosY( 0 ) - singleDTapBackPos.Y;

        if( FMath.Abs( calPos.X ) > FMath.Abs( calPos.Y ) ){
            disPer = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) ) / scrWidth;
        }
        else{
            disPer = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) ) / scrHeight;
        }

        if( FMath.Abs( disPer ) < 0.1f ){
            eventId |= EventId.DestinationCancel;
            return true;
        }        
        return false;
    }



    /// シングルタッチした際のパラメータセット
    private void setSingleTouchParam()
    {
        singleDTapBackPos    = singleTapParam.Pos;
        singleTapParam.Pos.X = useTouch.GetScrPosX( 0 );
        singleTapParam.Pos.Y = useTouch.GetScrPosY( 0 );
        singleTapParam.TouchId = useTouch.GetInputId( 0 );
        singleTouchFlg = true;
        singleFlicFlg  = false;
    }

    /// マルチタッチした際のパラメータセット
    private void setMultiTouchBackDis()
    {
        ImagePosition calPos;
        calPos.X = useTouch.GetScrPosX( 0 ) - useTouch.GetScrPosX( 1 );
        calPos.Y = useTouch.GetScrPosY( 0 ) - useTouch.GetScrPosY( 1 );
        multiBackDis    = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) );
    }



    /// マルチタッチした際のパラメータセット
    private void setMultiTouchBackPos()
    {
        for( int i=0; i<2; i++ ){
            multiTapParam[i].Pos.X = useTouch.GetScrPosX( i );
            multiTapParam[i].Pos.Y = useTouch.GetScrPosY( i );
            multiTapParam[i].TouchId = useTouch.GetInputId( i );
        }
    }




/// プロパティ
///---------------------------------------------------------------------------

    /// イベントIDの取得
    public EventId Event
    {
        get{return eventId;}
    }
        
    public float CamRotX
    {
        get {return camRotX;}
    }
    public float CamRotY
    {
        get {return camRotY;}
    }
    public float CamDis
    {
        get {return camDis;}
    }
    public float PlRotY
    {
        get {return plRotY;}
    }
    public bool TouchRelease
    {
        get {return touchRelease;}
    }

    public int SingleScrPosX
    {
        get {return singleTapParam.Pos.X;}
    }
    public int SingleScrPosY
    {
        get {return singleTapParam.Pos.Y;}
    }

    public int DeviceInputId
    {
        get {return useTouch.GetInputId( 0 );}
    }
    public int DevicePosX
    {
        get {return useTouch.GetScrPosX( 0 );}
    }
    public int DevicePosY
    {
        get {return useTouch.GetScrPosY( 0 );}
    }

    /// デバック用
    public DemoGame.InputGamePad Pad
    {
        get {return useGPad;}
    }
    public DemoGame.InputTouch Touch
    {
        get {return useTouch;}
    }



}

} // namespace
