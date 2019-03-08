/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : モンスターの操作
///***************************************************************************
public class ActorChMonster : ActorChBase
{
    private const int hpMax = 3;

    private ObjChMonster            objCh;
    private int                     deadCnt;
    private bool                    isSuperArm;
    public  bool                    ActiveFlg;
    public  int                     ActiveCnt;
    public  float                   ActiveDis;

    public  int                     AiAttackCnt;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objCh = new ObjChMonster();
        objCh.Init();

        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        if( objCh != null ){
            objCh.Term();
        }
        objCh    = null;
    }

    /// 開始
    protected override bool DoStart()
    {
        objCh.Start();

        ActiveFlg    = false;
        ActiveCnt    = 0;
        ActiveDis    = 0;
        hpNow        = hpMax;
        AiAttackCnt  = 0;
        isSuperArm   = false;
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        ActiveFlg    = false;
        mvtHdl.End();
        objCh.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
        isSuperArm = false;

        switch( stateIsPlayId ){
        case StateId.Stand:     statePlayStand();       break;
        case StateId.Move:      statePlayMove();        break;
        case StateId.Turn:      statePlayTurn();        break;
        case StateId.Attack:    statePlayAttack();      break;
        case StateId.Damage:    statePlayDamage();      break;
        case StateId.Dead:      statePlayDead();        break;
        }

        mvtHdl.Frame();
        unitCmnPlay.Frame();
///     unitCmnPlay.FrameGravity();        /// 敵は衝突判定を行わない


        /// OBJの姿勢を更新
        if( unitCmnPlay.IsUpdateMtx() ){
            updateMatrix( unitCmnPlay.Mtx );
        }

        objCh.Frame();
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCh.Draw( graphDev );
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objCh;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
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


/// アクタイベント
///---------------------------------------------------------------------------

    /// ダメージ
    public override void SetEventDamage( GameObjProduct trgObj, Data.AttackTypeId dmgId )
    {
        if( stateIsPlayId != StateId.Damage && stateIsPlayId != StateId.Dead ){

            dmgTrgObj    = trgObj;

            hpNow --;

            Vector3 effPos = new Vector3( objCh.Mtx.M41, objCh.Mtx.M42+0.8f, objCh.Mtx.M43 );

            switch( dmgId ){
            case Data.AttackTypeId.VerticalUD:
            case Data.AttackTypeId.VerticalDU:
                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff03, effPos );
                AppSound.GetInstance().PlaySe( AppSound.SeId.EnHit );
                break;
            case Data.AttackTypeId.HorizontalLR:
            case Data.AttackTypeId.HorizontalRL:
                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff04, effPos );
                AppSound.GetInstance().PlaySe( AppSound.SeId.EnHit );
                break;
            }


            if( hpNow <= 0 ){
                ChangeState( StateId.Dead );
            }
            else {
                /// 攻撃中はダメージへ遷移しない（魔法攻撃は無条件）
                if( isSuperArm == false || dmgId == Data.AttackTypeId.Magic ){
                    ChangeState( StateId.Damage );
                }
            }
        }
    }


    /// 対象の座標へ振り向く
    public override void SetEventTurnPos( Vector3 trgPos, int rot )
    {
        float trgRot = Common.MatrixUtil.GetPointRotY( BaseMtx, BasePos, trgPos );

        if( trgRot > (float)rot ){
            unitCmnPlay.SetTurn( (float)rot, 1 );
        }
        else if( trgRot < (float)-rot ){
            unitCmnPlay.SetTurn( (float)-rot, 1 );
        }
        else {
            unitCmnPlay.SetTurn( trgRot, 1 );
        }
    }


    /// スーパーアーマー化
    public override void SetEventSuperArm()
    {
        isSuperArm = true;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルの登録
    public void SetMdlHandle( Data.ChTypeId chTypeId )
    {
        this.chTypeId = chTypeId;
        objCh.SetMdlHandle( chTypeId );

        mvtHdl.Start( unitCmnPlay, Data.CharParamDataManager.GetInstance().GetData( (int)chTypeId ) );
    }

    /// 体の中心座標の取得
    public Vector3 GetBodyPos()
    {
        return objCh.BodyPos;
    }

    /// 体の半径取得
    public float GetBodyWidth()
    {
        float width = 1.0f;
        switch(chTypeId){
        case Data.ChTypeId.MonsterA:        width = 1.15f;           break;
        case Data.ChTypeId.MonsterB:        width = 0.5f;            break;
        case Data.ChTypeId.MonsterC:        width = 1.0f;            break;
        }
        return width;
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
        return true;
    }

    /// 歩く
    private bool statePlayMove()
    {
        movePow = 0.0f;
        moveTurn = 0.0f;
        return true;
    }

    /// 旋回
    private bool statePlayTurn()
    {
        float turnRot = 0.0f;
        switch(chTypeId){
        case Data.ChTypeId.MonsterA:        turnRot = 3.0f;            break;
        case Data.ChTypeId.MonsterB:        turnRot = 5.0f;            break;
        case Data.ChTypeId.MonsterC:        turnRot = 2.0f;            break;
        }

        setTurnRot( turnRot );

        if( moveTurn > turnRot*6 || moveTurn < -turnRot*6 ){
            if( chTypeId ==  Data.ChTypeId.MonsterC ){
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Turn, false );
            }
            else{
                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
            }
        }
        else{
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
        }

        return true;
    }


    /// 攻撃
    private bool statePlayAttack()
    {
        switch(statePlayTask){

        /// 攻撃セット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackLR, false );
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
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Damage, false );

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

            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            if( mvtHdl.IsActive() == false ){
                switch(chTypeId){
                case Data.ChTypeId.MonsterA:        AiAttackCnt -= 10;      break;
                case Data.ChTypeId.MonsterB:        AiAttackCnt -= 5;       break;
                case Data.ChTypeId.MonsterC:        AiAttackCnt = 0;        break;
                }

                ChangeState( StateId.Stand );
            }
            break;
        }

        return true;
    }



    /// 死亡
    private bool statePlayDead()
    {
        Vector3 effPos = new Vector3(0,0,0);

        switch(statePlayTask){

        /// 死亡セット
        case 0:
            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Dead, false );

            /// 攻撃対象の方向へ向く
            if( dmgTrgObj != null ){
				Vector4 x = new Vector4(0,0,0,0);
				Vector4 y = new Vector4(0,0,0,0);
				Vector4 z = new Vector4(0,0,0,0);
				Vector4 w = new Vector4(0,0,0,0);
	            Matrix4 mtx = new Matrix4(x ,y, z, w);
                Vector3 vec;
                vec.X = dmgTrgObj.Mtx.M41 - objCh.Mtx.M41;
                vec.Y = 0.0f;
                vec.Z = dmgTrgObj.Mtx.M43 - objCh.Mtx.M43;
                Common.MatrixUtil.LookTrgVec( ref mtx, vec ); 
                Common.MatrixUtil.SetTranslate( ref mtx, BasePos );
                this.SetPlace( mtx );
            }

            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            if( mvtHdl.IsActive() == false ){

                Common.VectorUtil.Set( ref effPos, objCh.BodyPos.X, objCh.BodyPos.Y, objCh.BodyPos.Z );

                switch(chTypeId){
                case Data.ChTypeId.MonsterA:        effPos.Y -= 0.6f;            break;
                case Data.ChTypeId.MonsterB:        effPos.Y -= 0.1f;            break;
                case Data.ChTypeId.MonsterC:        effPos.Y -= 0.1f;            break;
                }

                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, effPos );

                AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.EnDead, BasePos );
                deadCnt = 0;
                statePlayTask ++;
            }
            break;

        /// エフェクト余韻
        case 2:
            deadCnt ++;
            if( deadCnt >= 15 ){
                Enable = false;
            }
            break;
        }

        return true;
    }



    /// 旋回値をセットする
    private void setTurnRot( float turnRot )
    {
        if( moveTurn > turnRot ){
            unitCmnPlay.SetTurn( turnRot, 1 );
        }
        else if( moveTurn < -turnRot ){
            unitCmnPlay.SetTurn( -turnRot, 1 );
        }
        else {
            unitCmnPlay.SetTurn( moveTurn, 1 );
        }
    }
}

} // namespace
