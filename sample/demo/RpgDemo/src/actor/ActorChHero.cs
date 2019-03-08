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
/// ACTOR : 英雄の操作
///***************************************************************************
public class ActorChHero : ActorChBase
{
    private const int hpMax = 3;

    private ObjChHero                objCh;
    private ObjEqpSword              objEqpSword;
    private int                      jumpCnt;
    private int                      moveCnt;
    private bool                     isMvtCancel;
        


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objCh = new ObjChHero();
        objCh.Init();

        objEqpSword = new ObjEqpSword();
        objEqpSword.Init();

        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        if( objCh != null ){
            objCh.Term();
        }
        if( objEqpSword != null ){
            objEqpSword.Term();
        }
        objCh        = null;
        objEqpSword  = null;
    }

    /// 開始
    protected override bool DoStart()
    {
        mvtHdl.Start( unitCmnPlay,
                      Data.CharParamDataManager.GetInstance().GetData( (int)Data.ChTypeId.Hero ) );


        hpNow = hpMax;

        objCh.Start();
        objEqpSword.Start();
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        mvtHdl.End();
        objEqpSword.End();
        objCh.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
        isMvtCancel = false;

        switch( stateIsPlayId ){
        case StateId.Stand:     statePlayStand();       break;
        case StateId.Move:      statePlayMove();        break;
        case StateId.Jump:      statePlayJump();        break;
        case StateId.Attack:    statePlayAttack();      break;
        case StateId.Damage:    statePlayDamage();      break;
        case StateId.Dead:      statePlayDead();        break;
        case StateId.Victory:   statePlayVictory();     break;
        }

        mvtHdl.Frame();
        unitCmnPlay.Frame();
 
        if( AppDebug.GravityFlg ){
            unitCmnPlay.FrameGravity();
        }

        /// OBJの姿勢を更新
        if( unitCmnPlay.IsUpdateMtx() ){
            updateMatrix( unitCmnPlay.Mtx );
        }

        objCh.Frame();
        objEqpSword.Frame();
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCh.Draw( graphDev );

        /// 装備品を付属
        objEqpSword.SetMatrix( objCh.GetBoneMatrix( objCh.GetHandBoneId() ) );

        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        if( index == 0 ){
            return objCh;
        }
        return( objEqpSword );
    }
    protected override int DoGetUseObjNum()
    {
        return 2;
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
        unitCmnPlay.SetPlace( mtx );
        updateMatrix( mtx );
    }

    /// 境界ボリュームの取得
    protected override ShapeSphere DoGetBoundingShape()
    {
        return objCh.GetBoundSphere();
    }


/// public
///---------------------------------------------------------------------------

    /// 体の中心座標の取得
    public Vector3 GetBodyPos()
    {
        return objCh.BodyPos;
    }

    /// 動作キャンセル可能かのチェック
    public bool CheckMvtCancel()
    {
        return isMvtCancel;
    }




/// アクタイベント
///---------------------------------------------------------------------------

    /// ダメージ
    public override void SetEventDamage( GameObjProduct trgObj, Data.AttackTypeId dmgId )
    {
        if( stateIsPlayId != StateId.Dead ){
            dmgTrgObj    = trgObj;

            if( stateIsPlayId != StateId.Damage ){
                hpNow --;
            }

            if( hpNow <= 0 ){
                hpNow = 1;        /// 死亡のタイミングをずらす
                ChangeState( StateId.Dead );
            }
            else {
                ChangeState( StateId.Damage );
            }
        }
    }

    /// 動作キャンセル
    public override void SetEventMvtCancel()
    {
        isMvtCancel = true;
    }


/// private メソッド
///---------------------------------------------------------------------------

    /// 姿勢の更新
    private void updateMatrix( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );

        objCh.SetMatrix( mtx );
    }


    /// 立ち
    private bool statePlayStand()
    {
        mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );

        /// 旋回
        if( moveTurn != 0.0f ){
            unitCmnPlay.SetRot( moveTurn );
        }
        moveCnt = 5;
        return true;
    }

    /// 歩く
    private bool statePlayMove()
    {
        if( moveCnt >= 12 ){
            AppSound.GetInstance().PlaySe( AppSound.SeId.PlFoot );
            moveCnt = 0;
        }
        moveCnt ++;

        mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Run, false );

        /// 移動
        if( movePow != 0.0f ){
            unitCmnPlay.SetMove( moveVec, movePow );
        }

        /// 旋回
        if( moveTurn != 0.0f ){
            unitCmnPlay.SetRot( moveTurn );
        }

        movePow     = 0.0f;
        moveTurn    = 0.0f;
        return true;
    }

    /// ジャンプ
    private bool statePlayJump()
    {
        float pow;

        switch(statePlayTask){

        /// ジャンプ開始セット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.JumpStart, false );
            jumpCnt = 0;
            statePlayTask ++;
            break;

        /// ジャンプ開始終了待ち
        case 1:
            jumpCnt ++;
            if( jumpCnt >= 5 || mvtHdl.IsActive() == false ){
                jumpCnt = 5;
                statePlayTask = 10;
            }
            break;

        /// ジャンプ中
        case 10:
            if( mvtHdl.IsActive() == false ){
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.JumpLoop, false );
            }

            /// 旋回
            if( moveTurn != 0.0f ){
                unitCmnPlay.SetRot( moveTurn );
            }
                
            /// 上昇中
            if( jumpCnt < 16 ){
                unitCmnPlay.ResetGroudParam();
                jumpCnt ++;
                    
                pow = FMath.Sin( (3.14f*(jumpCnt+5)/30) ) * 0.15f + 0.5f;
                unitCmnPlay.SetMove( new Vector3( moveVec.X*movePow, pow, moveVec.Z*movePow ), 1.0f );
            }
            /// 下降中
            else {
                jumpCnt += 2;
                    
                pow = ((jumpCnt-15)*-0.03f) + 0.5f;
                unitCmnPlay.SetMove( new Vector3( moveVec.X*movePow, pow, moveVec.Z*movePow ), 1.0f );
                    
                if( unitCmnPlay.CheckTouchGround() == true ){
                    mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.JumpEnd, false );
                    AppSound.GetInstance().PlaySe( AppSound.SeId.PlFoot );
                    statePlayTask = 30;
                }
            }
            break;

        /// ジャンプ終了待ち
        case 30:
            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
            }
            break;
        }

        movePow     = 0.0f;
        moveTurn    = 0.0f;
        return true;
    }

    /// 攻撃
    private bool statePlayAttack()
    {
        switch(statePlayTask){

        /// 攻撃セット
        case 0:
			{
			switch( attackType ){
			case Data.AttackTypeId.VerticalUD:
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackUD, false );
				break;
			case Data.AttackTypeId.VerticalDU:
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackDU, false );
				break;
			case Data.AttackTypeId.HorizontalLR:
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackLR, false );
				break;
			case Data.AttackTypeId.HorizontalRL:
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackRL, false );
				break;
            }
			}
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
            }
            break;
        }

        return true;
    }



    /// ダメージ
    private bool statePlayDamage()
    {
        switch(statePlayTask){

        /// ダメージセット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Damage, true );

            setDamageEff();

            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
            }
            break;
        }

        return true;
    }


    /// 死亡
    private bool statePlayDead()
    {
        switch(statePlayTask){

        /// 死亡セット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Dead, false );
            setDamageEff();

            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            hpNow = 0;
            break;
        }

        return true;
    }


    /// 勝利
    private bool statePlayVictory()
    {
        switch(statePlayTask){

        /// ダメージセット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Victory, false );
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
            }
            break;
        }

        return true;
    }


    /// ダメージ演出
    private void setDamageEff()
    {

        /// 攻撃対象の方向へ向く
        if( dmgTrgObj != null ){
			Vector4 x = new Vector4(0,0,0,0);
			Vector4 y = new Vector4(0,0,0,0);
			Vector4 z = new Vector4(0,0,0,0);
			Vector4 w = new Vector4(0,0,0,0);
            Matrix4 mtx = new Matrix4(x ,y, z, w);
            Vector3 vec = new Vector3( (dmgTrgObj.Mtx.M41 - objCh.Mtx.M41), 0.0f, (dmgTrgObj.Mtx.M43 - objCh.Mtx.M43) );
            Common.MatrixUtil.LookTrgVec( ref mtx, vec ); 
            Common.MatrixUtil.SetTranslate( ref mtx, BasePos );
            this.SetPlace( mtx );
        }

        /// エフェクト表示
        Vector3 effPos = new Vector3( objCh.Mtx.M41+objCh.Mtx.M31*0.25f, objCh.Mtx.M42+0.9f, objCh.Mtx.M43+objCh.Mtx.M33*0.25f );
        EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff12, effPos );
        AppSound.GetInstance().PlaySe( AppSound.SeId.PlDamage );
    }



}

} // namespace
