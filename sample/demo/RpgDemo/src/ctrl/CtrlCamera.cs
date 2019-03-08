/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {

///***************************************************************************
/// カメラ制御
///***************************************************************************
public class CtrlCamera
{
    ///-------------------------------------
    /// カメラモードID
    ///-------------------------------------
    public enum ModeId {
        Normal = 0,     /// 通常カメラ
        Fps,            /// FPSカメラ
    };


    private ActorCamBehind          actorBehind;
    private Matrix4                 worldMtx;
    private Vector3                 worldRot;
    private float                   worldDis;
    private int                     autoCamCnt;
    private int                     autoCamFrame;
    private ModeId                  camModeId;
    private bool                    modeChangeFlg;

    private const float             camBehindY = 1.4f;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorBehind = new ActorCamBehind();
        actorBehind.Init();

		worldMtx	= Matrix4.Identity;
        worldRot    = new Vector3( 0.0f, 0.0f, 0.0f );
        worldDis    = 4.0f;
        return true;
    }

    /// 破棄
    public void Term()
    {
        actorBehind.Term();

        actorBehind        = null;
    }

    /// 開始
    public bool Start()
    {
        autoCamCnt      = 0;
        autoCamFrame    = 0;

        worldMtx    = Matrix4.Identity;
        worldRot    = new Vector3( 0.0f, 0.0f, 0.0f );
        worldDis    = 5.0f;
        modeChangeFlg = false;

        ResetCamMode();

        actorBehind.Start();

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        GameObjProduct  trgObj     = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3         movePos    = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 5.0f, 1 );
        actorBehind.Frame();
        return true;
    }

    /// 終了
    public void End()
    {
        actorBehind.End();
    }


    /// フレーム処理
    public bool Frame()
    {
        switch( camModeId ){
        case ModeId.Normal:     frameFieldBehind();     break;        /// 通常カメラ
        case ModeId.Fps:        frameFieldFps();        break;        /// １人称カメラ
        }
        return true;
    }

    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        actorBehind.Draw( graphDev );

        return true;
    }

    /// 通常状態開始
    public void ResetCamMode()
    {
        worldDis    = 5.0f;
        SetCamMode( ModeId.Normal );
    }

    /// カメラモードのセット
    public void SetCamMode( ModeId id )
    {
        camModeId = id;
        modeChangeFlg = false;
    }


    /// カメラの世界に対するY軸回転のみの行列を返す
    public Matrix4 GetCamMatrixRotY()
    {
        ObjCamera camObj = actorBehind.Obj;

        float a_Cal     = (float)(3.141593f / 180.0);
        float angleY    = camObj.TrgRot.Y * a_Cal;
        return Matrix4.RotationY( angleY );
    }

    /// カメラのY軸回転をセット
    public void SetCamRotY( float rotY )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        GameObjProduct  trgObj     = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3         movePos    = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
            
        worldRot.Y = rotY;
            
        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 4.0f, 1 );
        actorBehind.Frame();
        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );
    }

    /// カメラのY軸回転値を返す
    public float GetCamRotY()
    {
        return actorBehind.Obj.TrgRot.Y;
    }


    /// カメラの取得
    public DemoGame.Camera GetCurrentCameraCore()
    {
        return actorBehind.Obj.CameraCore;
    }

    /// カメラの位置を取得
    public Vector3 GetCamPos()
    {
        return actorBehind.Obj.CamPos;
    }

    /// カメラの注視点の取得
    public Vector3 GetCamTrgPos()
    {
        return actorBehind.Obj.TrgPos;
    }

    /// 注視点との距離
    public bool CheckModeChange()
    {
        return modeChangeFlg;
    }


    /// フレーム処理（配置用）
    public bool FramePlace( Vector3 trgPos )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Scan & DemoGame.InputGamePadState.L) != 0 ){
            worldDis -= 0.1f;
            if( worldDis < 1.0f ){
                worldDis = 1.0f;
            }
        }
        else if( (pad.Scan & DemoGame.InputGamePadState.R) != 0 ){
            worldDis += 0.1f;
        }
        else {
            worldRot.X += AppInput.GetInstance().CamRotX;
            worldRot.Y += AppInput.GetInstance().CamRotY;
        }

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, trgPos, worldRot, worldDis, 5 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }


    /// フレーム処理（タイトル用）
    public bool FrameTitle()
    {
        float[,] autoCamTrgPos = {
            { -105.5112f, 3.7772f, -84.7431f },
            { 58.3641f, 25.4343f, 61.3083f },
            { -24.0761f, 12.7641f, -115.3390f },
        };
        float[,] autoCamRot = {
            { 2.1f, -105.1f, 0.0f },
            { -12.1f, 128.9f, 0.0f },
            { -2.0f, -150.0f, 0.0f },
        };
        float[] autoCamDis = {
            17.1f, 19.0f, 10.3f
        };

        Vector3 pos = new Vector3( autoCamTrgPos[autoCamCnt,0], autoCamTrgPos[autoCamCnt,1], autoCamTrgPos[autoCamCnt,2] );
        Vector3 rot = new Vector3( autoCamRot[autoCamCnt,0], autoCamRot[autoCamCnt,1], autoCamRot[autoCamCnt,2] );
        float   dis = autoCamDis[autoCamCnt];
        rot.Y += (autoCamFrame/10.0f);

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, pos, rot, dis, 1 );
        actorBehind.Frame();

        autoCamFrame ++;
        if( autoCamFrame >= 120 ){
            autoCamFrame    = 0;
            autoCamCnt        = (autoCamCnt+1)%autoCamDis.Length;
        }
        return true;
    }



    /// フレーム処理（リザルト用）
    public bool FrameResult()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        if( worldRot.X > 30.0f ){
            worldRot.X = 30.0f;
        }
        else if( worldRot.X < 0.0f ){
            worldRot.X = 0.0f;
        }

        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 5.0f, 4 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 通常カメラ
    private bool frameFieldBehind()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;
        worldDis   += AppInput.GetInstance().CamDis;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        if( worldRot.X > 30.0f ){
            worldRot.X = 30.0f;
        }
        else if( worldRot.X < 0.0f ){
            worldRot.X = 0.0f;
        }

        if( worldDis < 1.4f ){
            if( worldDis < 0.1f ){
                worldDis = 0.1f;
            }

            /// 一定距離近づくとモードチェンジを要請
			worldDis		= 1.4f;
            modeChangeFlg	= true;
        }
        else if( worldDis > 10.0f ){
            worldDis = 10.0f;
        }

    
        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, worldDis, 4 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }


    /// FPSカメラ    
    private bool frameFieldFps()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;
        worldDis   += AppInput.GetInstance().CamDis;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }
            
        if( worldRot.X > 80.0f ){
            worldRot.X = 80.0f;
        }
        else if( worldRot.X < -80.0f ){
            worldRot.X = -80.0f;
        }
            
        if( worldDis > 2.0f ){
            worldDis      = 2.0f;

            /// 一定距離近づくとモードチェンジを要請
            modeChangeFlg = true;
        }
       else if( worldDis < 1.4f ){
           worldDis      = 1.4f;
       }

                
        GameObjProduct    trgObj  = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41+trgObj.Mtx.M31*-1.0f, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43+trgObj.Mtx.M33*-1.0f );

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 0.1f, 4 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }


/// プロパティ
///---------------------------------------------------------------------------
}

} // namespace
