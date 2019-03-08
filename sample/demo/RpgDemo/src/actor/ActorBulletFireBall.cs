/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 魔法弾の操作
///***************************************************************************
public class ActorBulletFireBall : ActorBulletBase
{
    private ObjBulletFireBall    objBullet;
    private Vector3              movePos;
    private int                  moveLife;

    private const int lifeFrameMax = 40;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        objBullet = new ObjBulletFireBall();
        objBullet.Init();

        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        objBullet.Term();
        objBullet    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        objBullet.Start();
        objBullet.SetScale( 0.25f );

        calCollMove.SetMoveType( Data.CollTypeId.BullMove );

        moveLife = lifeFrameMax;        /// 移動寿命フレーム

        AppSound.GetInstance().PlaySe( AppSound.SeId.PlSpelAtk ); 
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        objBullet.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        movePos.X = BasePos.X + BaseMtx.M31 * 1.0f;
        movePos.Y = BasePos.Y + BaseMtx.M32 * 1.0f;
        movePos.Z = BasePos.Z + BaseMtx.M33 * 1.0f;

        moveLife --;

        // 移動寿命
        if( moveLife <= 0 ){
            EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff08, BasePos );
            this.Enable = false;
        }
        
        /// 移動
        else if( calCollMove.Check( moveCollMgr, movePos ) == true ){
            EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff08, BasePos );
                
            if( moveCollMgr.TrgContainer.GetEntryObjParent(0).EventCntr != null ){
                moveCollMgr.TrgContainer.GetEntryObjParent(0).EventCntr.Add( ActorEventId.Damage, (int)Data.AttackTypeId.Magic, null );
                moveCollMgr.TrgContainer.GetEntryObjParent(0).EventCntr.Add( ActorEventId.LookTrg, 0, BasePos );
            }

            AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.EnSpelHit, BasePos );
            this.Enable = false;
        }
        else{
            BasePos = calCollMove.NextPos;
        }

        Common.MatrixUtil.SetTranslate( ref BaseMtx, BasePos );
        objBullet.SetMatrix( BaseMtx );

        objBullet.Frame();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objBullet.Draw( graphDev );
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objBullet;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );
        objBullet.SetMatrix( mtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

}
} // namespace
