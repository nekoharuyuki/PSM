/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 追従エフェクトの操作
///***************************************************************************
public class ActorEffAttach : ActorEffBase
{
    private GameObjProduct   objTrg;
    private ObjEffect        objEff;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        objEff = new ObjEffect();
        objEff.Init();
        objTrg = null;

        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        objEff.Term();
        objEff    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        objEff.Start();

        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        objTrg = null;
        objEff.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        if( objEff.IsAnimation() == false ){
            Enable = false;
            return false;
        }

        objEff.Frame();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        if( objTrg != null ){
            BaseMtx        = objTrg.Mtx;
            Common.VectorUtil.Set( ref BasePos, objTrg.Mtx );
            objEff.SetMatrix( objTrg.Mtx );

            objEff.Draw( graphDev );
        }
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objEff;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
    }

    /// 境界ボリュームの取得
    protected override ShapeSphere DoGetBoundingShape()
    {
        return objEff.GetBoundSphere();
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルの登録
    public void SetMdlHandle( Data.EffTypeId effTypeId )
    {
        objEff.SetMdlHandle( effTypeId );
        objEff.SetScale( 1.0f );

        this.effTypeId    = effTypeId;
    }

    /// 追従対象のOBJをセット
    public void SetTrgObj( GameObjProduct trgObj )
    {
        objTrg = trgObj;
    }

}

} // namespace
