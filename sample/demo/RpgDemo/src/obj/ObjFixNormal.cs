/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

/// 備品:基本タイプ
public class ObjFixNormal : GameObjProduct
{
    /// 形状
    private ShapeTriangles          shapeColl;

    private Common.ModelHandle      useMdlHdl;
    private float                   objWidth;
    private int                     nowLodLev;
    private bool                    drawFlg;

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        shapeColl = new ShapeTriangles();

        useMdlHdl = new Common.ModelHandle();
        useMdlHdl.Init();
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( shapeColl != null ){
            shapeColl.Term();
        }
        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }
        shapeColl = null;
        useMdlHdl = null;
    }

    /// 開始
    public override bool DoStart()
    {
        DoUpdateMatrix();
        nowLodLev = -1;
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        useMdlHdl.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        if( drawFlg ){
            useMdlHdl.RenderNoAnim( graphDev, baseMtx );
        }
        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        shapeColl.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }

    /// モデルハンドルを返す
    public override Common.ModelHandle GetModelHdl()
    {
        return useMdlHdl;
    }

    /// 移動対象かどうか
    public override int CheckMoveTrgId()
    {
        return 1;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// モデルのセット
    public void SetMdlHandle( Data.FixTypeId fixTypeId, int lodLv )
    {
        if( nowLodLev != lodLv ){

            int fixTypeIdx  = (int)fixTypeId;
            int mdlResId    = (int)ObjFixLodData.LodModelTbl[fixTypeIdx, lodLv];
            int texResId    = (int)Data.ModelTexResId.Fix00 + fixTypeIdx;
            int shaResId    = (int)Data.ModelShaderReslId.Normal;

            if( mdlResId >= (int)Data.ModelResId.Fix00 ){
                Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

                /// 速度アップのため、事前にテクスチャをバインドしておく
                useMdlHdl.StartNoBindTex( resMgr.GetModel( mdlResId ),
                                          resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );
                drawFlg = true;
            }
            else{
                drawFlg = false;
            }

            nowLodLev = lodLv;
        }
    }

    /// モデルのセット
    public void SetBeforehandMdlBindTex( Data.FixTypeId fixTypeId, int lodLv )
    {
        int fixTypeIdx  = (int)fixTypeId;
        int mdlResId    = (int)ObjFixLodData.LodModelTbl[fixTypeIdx, lodLv];
        int texResId    = (int)Data.ModelTexResId.Fix00 + fixTypeIdx;
        int shaResId    = (int)Data.ModelShaderReslId.Normal;

        if( mdlResId >= (int)Data.ModelResId.Fix00 ){
            Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
            useMdlHdl.Start( resMgr.GetModel( mdlResId ),
                             resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );
        }
    }


    /// 衝突形状のセット
    public void SetShapeColl( Data.FixTypeId fixTypeId )
    {
        float[] posData = null;

        /// 当たりデータの展開
        ///-------------------------------
        switch( fixTypeId ){
        case Data.FixTypeId.Fix00:        posData = ShapeObj00Data.Positions;    break;
        case Data.FixTypeId.Fix01:        posData = ShapeObj01Data.Positions;    break;
        case Data.FixTypeId.Fix02:        posData = ShapeObj02Data.Positions;    break;
///     case Data.FixTypeId.Fix03:        posData = ShapeObj03Data.Positions;    break;
        case Data.FixTypeId.Fix04:        posData = ShapeObj04Data.Positions;    break;
        case Data.FixTypeId.Fix05:        posData = ShapeObj05Data.Positions;    break;
        case Data.FixTypeId.Fix06:        posData = ShapeObj06Data.Positions;    break;
        case Data.FixTypeId.Fix07:        posData = ShapeObj07Data.Positions;    break;
        case Data.FixTypeId.Fix08:        posData = ShapeObj08Data.Positions;    break;
        case Data.FixTypeId.Fix09:        posData = ShapeObj09Data.Positions;    break;
        case Data.FixTypeId.Fix10:        posData = ShapeObj10Data.Positions;    break;
        case Data.FixTypeId.Fix11:        posData = ShapeObj11Data.Positions;    break;
        case Data.FixTypeId.Fix12:        posData = ShapeObj12Data.Positions;    break;
        case Data.FixTypeId.Fix13:        posData = ShapeObj13Data.Positions;    break;
        case Data.FixTypeId.Fix14:        posData = ShapeObj14Data.Positions;    break;
        case Data.FixTypeId.Fix15:        posData = ShapeObj15Data.Positions;    break;
        case Data.FixTypeId.Fix16:        posData = ShapeObj16Data.Positions;    break;
        case Data.FixTypeId.Fix17:        posData = ShapeObj17Data.Positions;    break;
        case Data.FixTypeId.Fix18:        posData = ShapeObj18Data.Positions;    break;
        case Data.FixTypeId.Fix19:        posData = ShapeObj19Data.Positions;    break;
        }

        shapeColl.Term();

        if( posData != null ){
            shapeColl.Init( posData.Length/9 );
            for( int i=0; i<posData.Length/9; i++ ){
                Vector3 pos1 = new Vector3( posData[i*9+0], posData[i*9+1], posData[i*9+2] );
                Vector3 pos2 = new Vector3( posData[i*9+3], posData[i*9+4], posData[i*9+5] );
                Vector3 pos3 = new Vector3( posData[i*9+6], posData[i*9+7], posData[i*9+8] );
                shapeColl.SetType( 1 );
                shapeColl.Add( pos1, pos2, pos3 );
            }
        }
    

        ///-------------------------------
        SetBoundingShape( fixTypeId, new Vector3( 1.0f, 1.0f, 1.0f ) );
    }

    /// 境界ボリュームの作成
    public void SetBoundingShape( Data.FixTypeId fixTypeId, Vector3 scale )
    {
        float val = 1.0f;
        if( scale.X > scale.Y ){
            val = ( scale.X > scale.Z )?    scale.X : scale.Z;
        }
        else{
            val = ( scale.Y > scale.Z )?    scale.Y : scale.Z;
        }
            
        Vector3 cpos    = ObjFixLodData.GetBoundingCenterPos( (int)fixTypeId );
        objWidth        = ObjFixLodData.GetBoundingCenterWidth( (int)fixTypeId ) * val;

        boundingShape.Set( 0, cpos, objWidth );
        boundingShape.SetMult( this.baseMtx );
    }
        

/// 形状関連
///---------------------------------------------------------------------------

    /// 衝突形状
    public override GameShapeProduct GetCollisionShape( Data.CollTypeId type, int no )
    {
        return shapeColl;
    }
    public override int GetCollisionShapeMax( Data.CollTypeId type )
    {
        return 1;
    }

/// プロパティ
///---------------------------------------------------------------------------
    public bool DrawFlg
    {
        get {return this.drawFlg;}
    }


}


} // namespace
