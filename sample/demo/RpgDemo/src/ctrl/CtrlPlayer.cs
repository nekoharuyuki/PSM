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
/// プレイヤー制御
///***************************************************************************
public class CtrlPlayer
{
    ///-------------------------------------
    /// 操作モードID
    ///-------------------------------------
    private enum PlayModeId {
        Normal = 0,     /// 通常カメラ
        Fps,            /// FPSカメラ
    };


    private ActorChHero         actorCh;
    private ActorWepSword       actorWep;
    private ObjEffect           objShadow;
    private PlayModeId          playModeId;
    private bool                plDrawFlg;
    private int                 spelDelayCnt;

	private const float         moveSpeed = 0.208f;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorCh = new ActorChHero();
        actorCh.Init();
        actorWep = new ActorWepSword();
        actorWep.Init();

        objShadow = new ObjEffect();
        objShadow.Init();
        return true;
    }

    /// 破棄
    public void Term()
    {
        objShadow.Term();
        actorCh.Term();
        actorWep.Term();

        actorWep    = null;
        actorCh     = null;
        objShadow   = null;
    }

    /// 開始
    public bool Start()
    {
		ObjChHero heroObj = (ObjChHero)actorCh.GetUseObj(0);

        actorCh.Start();
        actorWep.Start();
        actorWep.SetTrgObj( heroObj, heroObj.GetHandBoneId() );

        playModeId   = PlayModeId.Normal;
        plDrawFlg    = true;
        spelDelayCnt = 0;

        objShadow.SetMdlHandle( Data.EffTypeId.Eff10 );
        objShadow.Start();
        objShadow.SetScale( 0.12f );
        return true;
    }

    /// 終了
    public void End()
    {
        objShadow.End();
        actorWep.End();
        actorCh.End();
    }


    /// フレーム処理
    public bool Frame()
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();
            
        /// 他アクタからのイベントをチェック
        ///-------------------------------------
        if( actorCh.EventCntr.Num > 0 ){
            ctrlResMgr.CtrlEvent.Play( actorCh, actorCh.EventCntr );
        }

        /// フレーム処理
        ///-------------------------------------
        switch( playModeId ){
        case PlayModeId.Normal:     framePlayModeNormal();        break;
        case PlayModeId.Fps:        framePlayModeFps();           break;
        }

        /// 自身発生のイベントをチェック
        ///-------------------------------------
        if( actorCh.EventCntr.Num > 0 ){
            ctrlResMgr.CtrlEvent.Play( actorCh, actorCh.EventCntr );
        }

        return true;
    }


    /// フレーム処理（リザルト画面時）
    public bool FrameResult()
    {
        if( GetHp() > 0 && actorCh.GetStateId() != ActorChBase.StateId.Victory ){
            actorCh.SetStateVictory();
        }

        /// 通常カメラに戻す
        if( playModeId != PlayModeId.Normal ){
            setPlayMode( PlayModeId.Normal );
            GameCtrlManager.GetInstance().CtrlCam.ResetCamMode();
        }

        actorCh.Frame();
        actorWep.Frame();

        plDrawFlg = true;
        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        if( plDrawFlg ){
            actorCh.Draw( graphDev );
        }
        return true;
    }

    /// αの描画処理
    public bool DrawAlpha( DemoGame.GraphicsDevice graphDev )
    {
        if( plDrawFlg ){
            GameCtrlDrawManager.GetInstance().EntryEffect( actorWep, true );

            /// ステージの地面を踏んでいるときに影を表示
            if( actorCh.GetTouchGroundType() == 0 ){
                Matrix4 mtx = actorCh.BaseMtx;
                mtx.M41 = actorCh.GetBodyPos().X;
                mtx.M43 = actorCh.GetBodyPos().Z;

                objShadow.SetMatrix( mtx );
                objShadow.Draw( graphDev );
            }
        }
        return true;
    }


    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        for( int i=0; i<actorCh.GetUseObjNum(); i++ ){
            container.Add( actorCh, actorCh.GetUseObj(i) );
        }

        return true;
    }

    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetInterfereActor( GameActorContainer container )
    {
        container.Add( actorCh );
        return true;
    }


    /// ベースOBJの取得
    public GameObjProduct GetUseActorBaseObj()
    {
        return actorCh.GetUseObj(0);
    }


    /// プレイヤーの配置
    public void SetPlace( float rotY, Vector3 pos )
    {
        float a_Cal     = (float)(3.141593f / 180.0);
        float angleY    = rotY * a_Cal;

        Matrix4 mtx = Matrix4.RotationY( angleY );
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        actorCh.SetPlace( mtx );
    }

    /// プレイヤーの現在位置の取得
    public Vector3 GetPos()
    {
        return new Vector3( actorCh.BaseMtx.M41, actorCh.BaseMtx.M42, actorCh.BaseMtx.M43 );
    }
    /// プレイヤーの向きを取得
    public float GetRotY()
    {
        float angleY = (float)Math.Atan2( actorCh.BaseMtx.M31, actorCh.BaseMtx.M33 );
        float a_Cal     = (float)(angleY / (3.141593f / 180.0));
        return a_Cal;
    }

    /// 現在のHPを取得
    public int GetHp()
    {
        return actorCh.GetHp();
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 移動衝突対象OBJの登録
    private void setMoveCollTrgObj()
    {
        GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
        GameActorCollManager    useCollMgr    = actorCh.GetMoveCollManager();

        /// 移動する自身の形状を登録
        useCollMgr.SetMoveShape( GetUseActorBaseObj().GetMoveShape() ); 

        /// 対象を登録
        ctrlResMgr.SetCollisionActor( useCollMgr.TrgContainer, actorCh.BasePos );
    }

    /// 干渉対象の登録
    private void setInterfereActor()
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        /// 対象を登録
        ctrlResMgr.SetInterfereActorPl( actorCh.GetInterfereCntr() );
    }




    /// フレーム処理：通常視点
    public bool framePlayModeNormal()
    {
        plDrawFlg = true;

        /// フレーム処理
        ///-------------------------------------
        if( actorCh.GetStateId() == ActorChBase.StateId.Stand || actorCh.GetStateId() == ActorChBase.StateId.Move ){
            frameMove();
        }
        else if( actorCh.GetStateId() == ActorChBase.StateId.Jump ){
            frameJump();
        }
        else if( actorCh.GetStateId() == ActorChBase.StateId.Attack ){
            frameAttack();
        }

        setMoveCollTrgObj();
        setInterfereActor();
        actorCh.Frame();
        actorWep.Frame();

		setCamPlBehindFlg();

        return true;
    }


    /// フレーム処理：FPS視点
    public bool framePlayModeFps()
    {
        plDrawFlg = false;

        /// フレーム処理
        ///-------------------------------------
        if( actorCh.GetStateId() == ActorChBase.StateId.Stand || actorCh.GetStateId() == ActorChBase.StateId.Move ){
            frameMoveFps();
        }
        else if( actorCh.GetStateId() == ActorChBase.StateId.Jump ){
            frameJump();
        }
        else if( actorCh.GetStateId() == ActorChBase.StateId.Damage || actorCh.GetStateId() == ActorChBase.StateId.Dead){
            setPlayMode( PlayModeId.Normal );
            GameCtrlManager.GetInstance().CtrlCam.ResetCamMode();
        }

        setMoveCollTrgObj();
        setInterfereActor();
        actorCh.Frame();
        actorWep.Frame();

        if( spelDelayCnt > 0 ){
            spelDelayCnt --;
        }
        return true;
    }



    /// フレーム：移動
    private bool frameMove()
    {
        float tPow = 0.0f;

        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        /// カメラモード切替
        if( ctrlResMgr.CtrlCam.CheckModeChange() ){
            setPlayMode( PlayModeId.Fps );
            return true;
        }

        /// キャラのジャンプ
        if( (AppInput.GetInstance().Event & AppInput.EventId.Jump) != 0 ){
            actorCh.SetStateJump();
            return true;
        }

        /// 攻撃横斬り
        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackLR) != 0 ){
            actorCh.SetStateAttack( Data.AttackTypeId.HorizontalLR );
            ctrlResMgr.CtrlStg.ClearDestination();
            return true;
        }
        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackRL) != 0 ){
            actorCh.SetStateAttack( Data.AttackTypeId.HorizontalRL );
            ctrlResMgr.CtrlStg.ClearDestination();
            return true;
        }

        /// 攻撃縦斬り
        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackUD) != 0 ){
            actorCh.SetStateAttack( Data.AttackTypeId.VerticalUD );
            ctrlResMgr.CtrlStg.ClearDestination();
            return true;
        }
        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackDU) != 0 ){
            actorCh.SetStateAttack( Data.AttackTypeId.VerticalDU );
            ctrlResMgr.CtrlStg.ClearDestination();
            return true;
        }

        /// キャラの移動
        else if( (AppInput.GetInstance().Event & AppInput.EventId.Move) != 0 ){
            tPow = (180.0f+ctrlResMgr.CtrlCam.GetCamRotY() + AppInput.GetInstance().GetPlRotY());

            actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31, actorCh.BaseMtx.M32, actorCh.BaseMtx.M33 ),
                                moveSpeed, tPow, false );

            /// キー入力が入った場合には目的地をキャンセル
            ctrlResMgr.CtrlStg.ClearDestination();
        }

        /// 目的地への移動
        ///--------------------------------------------
        else if( ctrlResMgr.CtrlStg.CheckDestination() ){
            frameDestinationMove();
        }

        /// 待機
        else{
            actorCh.SetStateStand();
        }

        return true;
    }


    /// フレーム：ジャンプ
    private bool frameJump()
    {
        float    mPow;

        Matrix4  rotMtx;
        float    rotY;
        float    cal;
        float    angleY;


        if( (AppInput.GetInstance().Event & AppInput.EventId.Move) != 0 ){
            GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();
            mPow = 0.108f;

            rotY     = 180.0f + ctrlResMgr.CtrlCam.GetCamRotY() + AppInput.GetInstance().GetPlRotY();
            cal      = (float)(3.141593f / 180.0);
            angleY   = rotY * cal;

            rotMtx   = Matrix4.RotationY( angleY );

            actorCh.SetStateJumpMove( new Vector3( rotMtx.M31, rotMtx.M32, rotMtx.M33 ), mPow, 0.0f );
        }

        return true;
    }


    /// フレーム：剣攻撃
    private bool frameAttack()
    {
        if( actorCh.CheckMvtCancel() ){

	        /// 攻撃横斬り
	        if( (AppInput.GetInstance().Event & AppInput.EventId.AttackLR) != 0 ){
	            actorCh.SetStateAttack( Data.AttackTypeId.HorizontalLR );
	            return true;
	        }
	        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackRL) != 0 ){
	            actorCh.SetStateAttack( Data.AttackTypeId.HorizontalRL );
	            return true;
	        }

	        /// 攻撃縦斬り
	        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackUD) != 0 ){
	            actorCh.SetStateAttack( Data.AttackTypeId.VerticalUD );
	            return true;
	        }
	        else if( (AppInput.GetInstance().Event & AppInput.EventId.AttackDU) != 0 ){
	            actorCh.SetStateAttack( Data.AttackTypeId.VerticalDU );
	            return true;
	        }
        }
        return true;
    }



    /// フレーム：FPS移動
    private bool frameMoveFps()
    {
        Matrix4      rotMtx;
        float        rotY;
        float        cal;
        float        angleY;
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();


        /// カメラの方向を常に向く
        ///--------------------------------------------
        setCamVecTurn();


        /// カメラモード切替
        ///--------------------------------------------
        if( ctrlResMgr.CtrlCam.CheckModeChange() ){
            setPlayMode( PlayModeId.Normal );
            return true;
        }

        /// ジャンプ
        ///--------------------------------------------
        if( (AppInput.GetInstance().Event & AppInput.EventId.Jump) != 0 ){
            actorCh.SetStateJump();
            return true;
        }

        /// 魔法攻撃
        ///--------------------------------------------
        else if( (AppInput.GetInstance().Event & AppInput.EventId.SpelAtk) != 0 && spelDelayCnt <= 0 ){
            DemoGame.Camera camCore = ctrlResMgr.CtrlCam.GetCurrentCameraCore();
            
            ctrlResMgr.CtrlBullet.EntryEffect( Data.BulletTypeId.FireBall,
                        new Vector3( actorCh.BasePos.X, actorCh.BasePos.Y+1.0f, actorCh.BasePos.Z ),
                        camCore.LookVec );
            spelDelayCnt = 10;
            return true;
        }

        /// 移動
        ///--------------------------------------------
        else if( (AppInput.GetInstance().Event & AppInput.EventId.Move) != 0 ){
            rotY     = 180.0f + ctrlResMgr.CtrlCam.GetCamRotY() + AppInput.GetInstance().GetPlRotY();
            cal      = (float)(3.141593f / 180.0);
            angleY   = rotY * cal;

            rotMtx   = Matrix4.RotationY( angleY );

            actorCh.SetStateMove( new Vector3( rotMtx.M31, rotMtx.M32, rotMtx.M33 ), moveSpeed, 0.0f, false );

            /// キー入力が入った場合には目的地をキャンセル
            ctrlResMgr.CtrlStg.ClearDestination();
        }

        /// 目的地への移動
        ///--------------------------------------------
        else if( ctrlResMgr.CtrlStg.CheckDestination() ){
            frameDestinationMoveFps();
        }

        /// 待機
        ///--------------------------------------------
        else{
            actorCh.SetStateStand();
        }

        return true;
    }



    /// フレーム：目的地への移動
    private bool frameDestinationMove()
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        float        rotY;
        float        cal;
        float        angleY;

        float mPow = moveSpeed;


        /// 目的地が設定されている場合
        ///----------------------------------------------------
        if( ctrlResMgr.CtrlStg.CheckDestinationTrg() ){
            float trgDis =  Common.VectorUtil.DistanceXZ( actorCh.BasePos, ctrlResMgr.CtrlStg.GetDestinationPos() );
            GameActorProduct  trgActor        =  ctrlResMgr.CtrlStg.GetDestinationActor();
            ShapeSphere bndSph                = trgActor.GetBoundingShape();

            if( mPow > trgDis ){
                mPow = trgDis;
            }

            /// 目的地到着
            if( trgDis <= mPow || ( bndSph != null && trgDis <= bndSph.Sphre.R*1.5f ) ){
                ctrlResMgr.CtrlStg.ClearDestination();
                actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31, actorCh.BaseMtx.M32, actorCh.BaseMtx.M33 ), mPow, 0, false );

				if( bndSph != null ){
			        actorCh.Frame();
		            actorCh.SetStateAttack( 0 );
				}
                return false;
            }

            actorCh.SetLookTrgPos( ctrlResMgr.CtrlStg.GetDestinationPos() );
        }

        /// 直進移動
        ///----------------------------------------------------
        else{
            rotY     = 180.0f + ctrlResMgr.CtrlCam.GetCamRotY() + AppInput.GetInstance().GetPlRotY();
            cal      = (float)(3.141593f / 180.0);
            angleY   = rotY * cal;
            actorCh.BaseMtx = Matrix4.RotationY( angleY );
            Common.MatrixUtil.SetTranslate( ref actorCh.BaseMtx, actorCh.BasePos );
            actorCh.SetPlace( actorCh.BaseMtx );
        }

        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31, actorCh.BaseMtx.M32, actorCh.BaseMtx.M33 ), mPow, 0, false );
        return true;
    }



    /// フレーム：目的地への移動FPS視点
    private bool frameDestinationMoveFps()
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        float        rotY;
        float        cal;
        float        angleY;

        float mPow = moveSpeed;


        /// 目的地が設定されている場合
        ///----------------------------------------------------
        if( ctrlResMgr.CtrlStg.CheckDestinationTrg() ){
            float trgDis =  Common.VectorUtil.DistanceXZ( actorCh.BasePos, ctrlResMgr.CtrlStg.GetDestinationPos() );

            GameActorProduct  trgActor = ctrlResMgr.CtrlStg.GetDestinationActor();
            ShapeSphere bndSph         = trgActor.GetBoundingShape();


            if( mPow > trgDis ){
                mPow = trgDis;
            }

            /// 目的地到着
            if( trgDis <= mPow || ( bndSph != null && trgDis <= bndSph.Sphre.R*1.5f) ){
                ctrlResMgr.CtrlStg.ClearDestination();
                actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31, actorCh.BaseMtx.M32, actorCh.BaseMtx.M33 ), mPow, 0, false );

				if( bndSph != null ){
			        actorCh.Frame();
		            setPlayMode( PlayModeId.Normal );
		            actorCh.SetStateAttack( 0 );
				}
                return false;
            }

            actorCh.SetLookTrgPos( ctrlResMgr.CtrlStg.GetDestinationPos() );
        }

        /// 直進移動
        ///----------------------------------------------------
        else{
            rotY     = 180.0f + ctrlResMgr.CtrlCam.GetCamRotY() + AppInput.GetInstance().GetPlRotY();
            cal      = (float)(3.141593f / 180.0);
            angleY   = rotY * cal;
            actorCh.BaseMtx = Matrix4.RotationY( angleY );
            Common.MatrixUtil.SetTranslate( ref actorCh.BaseMtx, actorCh.BasePos );
            actorCh.SetPlace( actorCh.BaseMtx );
        }

        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31, actorCh.BaseMtx.M32, actorCh.BaseMtx.M33 ), mPow, 0, false );
        return true;
    }


    /// 操作モードのセット
    private void setPlayMode( PlayModeId id )
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        playModeId = id;

        /// カメラモードのセット
        if( id == PlayModeId.Normal ){
            ctrlResMgr.CtrlCam.SetCamMode( CtrlCamera.ModeId.Normal );
///            ctrlResMgr.CtrlCam.ResetCamMode();
        }
        else{
            ctrlResMgr.CtrlCam.SetCamMode( CtrlCamera.ModeId.Fps );
            plDrawFlg = false;
        }

        /// 通常視点に戻るときはカメラの方向を向く
        setCamVecTurn();
    }


    /// カメラの方向を向く
    private void setCamVecTurn()
    {
        Matrix4      rotMtx;
        float        rotY;
        float        cal;
        float        angleY;

        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        rotY     = 180.0f + ctrlResMgr.CtrlCam.GetCamRotY();
        cal      = (float)(3.141593f / 180.0);
        angleY   = rotY * cal;
        rotMtx   = Matrix4.RotationY( angleY );
        Common.MatrixUtil.SetTranslate( ref rotMtx, actorCh.BasePos );
        actorCh.SetPlace( rotMtx );
    }


    /// カメラとプレイヤーとの位置関係をセット
    private void setCamPlBehindFlg()
    {
        DemoGame.Camera camCore = GameCtrlManager.GetInstance().CtrlCam.GetCurrentCameraCore();

		Vector3 plVec; 
        plVec.X = actorCh.BaseMtx.M31;
        plVec.Y = actorCh.BaseMtx.M32;
        plVec.Z = actorCh.BaseMtx.M33;
		float dot = camCore.LookVec.Dot( plVec );

		if( dot > 0.0f ){
			AppInput.GetInstance().SetPlBehind( true );
		}
		else{
			AppInput.GetInstance().SetPlBehind( false );
		}
    }

}

} // namespace
