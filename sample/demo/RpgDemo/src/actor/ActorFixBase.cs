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
/// ACTOR : 備品操作基底
///***************************************************************************
public class ActorFixBase : GameActorProduct
{

    public Vector3                ObjScale;
    public Vector3                ObjAngle;
    public float                  DrawCamDis;

    protected ObjFixNormal        objFix;
    protected ActorUnitCommon     unitCmnPlay;
    protected Data.FixTypeId      fixTypeId;
    protected bool                breakFlg;        /// 破壊されたかのフラグ
    protected bool                brokenFlg;       /// 破壊可能かのフラグ



/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool Init()
    {
        unitCmnPlay = new ActorUnitCommon();

        EventCntr   = new GameActorEventContainer();
        EventCntr.Init();

        Visible          = false;
        return( DoInit() );
    }

    /// 破棄
    public override void Term()
    {
        DoTerm();

        if( unitCmnPlay != null ){
            unitCmnPlay.Term();
        }
        if( EventCntr != null ){
            EventCntr.Term();
        }

        EventCntr   = null;
        unitCmnPlay = null;
    }

    /// 開始
    public override bool Start()
    {
        unitCmnPlay.Start( this, null, null );

        brokenFlg   = false;        /// 破壊されたかのフラグ
        breakFlg    = false;        /// 破壊可能かのフラグ

        Enable = DoStart();
        return( Enable );
    }

    /// 終了
    public override void End()
    {
        DoEnd();
        unitCmnPlay.End();

        Enable = false;
    }

    /// フレーム
    public override bool Frame()
    {
        return( DoFrame() );
    }

    /// 描画
    public override bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        objFix.SetMdlHandle( fixTypeId, LodLev );

        return( DoDraw(graphDev) );
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objFix;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

    /// 境界ボリュームの取得
    protected override ShapeSphere DoGetBoundingShape()
    {
        return objFix.GetBoundSphere();
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
        updateMatrix( mtx );
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルの登録
    public bool SetMdlHandle( Data.FixTypeId fixId )
    {
        fixTypeId = fixId;

        objFix.SetMdlHandle( fixId, 0 );
        objFix.SetShapeColl( fixId );
        return true;
    }

    /// 破壊可能かのチェック
    public bool CheckBreakObj()
    {
        return breakFlg;
    }




/// アクタイベント
///---------------------------------------------------------------------------

    /// ダメージ
    public override void SetEventDamage( GameObjProduct trgObj, Data.AttackTypeId dmgId )
    {
        brokenFlg = true;
    }



/// 状態セット
///---------------------------------------------------------------------------

    /// 備品タイプIDの取得
    public Data.FixTypeId GetFixTypeId()
    {
        return fixTypeId;
    }

    /// 備品の配置
    public void SetPlace( Vector3 angle, Vector3 scale, Vector3 pos )
    {
        Matrix4 mtx = new Matrix4();
        float cal   = (float)(3.141593f / 180.0);
        Vector3 calAngle = new Vector3( angle.X * cal, angle.Y * cal, angle.Z * cal );

        Common.MatrixUtil.SetMtxRotateEulerXYZ( ref mtx, calAngle );
        mtx = mtx * Matrix4.Scale(new Vector3(scale.X, scale.Y, scale.Z));
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        ObjScale = scale;
        ObjAngle = angle;

        SetPlace( mtx );
    }




/// 仮想メソッド
///---------------------------------------------------------------------------

    protected virtual bool DoInit()
    {
        return true;
    }
    protected virtual void DoTerm()
    {
    }
    protected virtual bool DoStart()
    {
        return true;
    }
    protected virtual void DoEnd()
    {
    }
    protected virtual bool DoFrame()
    {
        return true;
    }
    protected virtual bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 姿勢の更新
    private void updateMatrix( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );

        objFix.SetMatrix( mtx );
        objFix.SetBoundingShape( fixTypeId, ObjScale );
    }


/// プロパティ
///---------------------------------------------------------------------------

}

} // namespace
