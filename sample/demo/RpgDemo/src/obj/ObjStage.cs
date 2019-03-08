/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {

///***************************************************************************
/// OBJ:ステージ
///***************************************************************************
public class ObjStage : GameObjProduct
{
    /// 形状ID
    ///------------------------------
    public enum ShapeTypeId{
        MoveWall = 0,
        MoveGround,
        MoveGround2,
        MoveMagic,
        Max
    };

    private ShapeTriangles[]    shapeColl;

    private Common.ModelHandle  useMdlHdl;
    private Common.ModelHandle  useMdlSkyHdl;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        /// 形状生成
        shapeColl = new ShapeTriangles[(int)ShapeTypeId.Max];

        float[] posData;

        /// 地面の登録
        posData = ShapeStageData.Positions00;
        makeShapeColl( (int)ShapeTypeId.MoveGround, posData );

        /// 砂地地面の登録
        posData = ShapeStageData.Positions01;
        makeShapeColl( (int)ShapeTypeId.MoveGround2, posData );

        /// キャラ移動壁の登録
        posData = ShapeStageData.Positions50;
        makeShapeColl( (int)ShapeTypeId.MoveWall, posData );

        /// 魔法移動壁の登録
        posData = ShapeStageData.Positions51;
        makeShapeColl( (int)ShapeTypeId.MoveMagic, posData );

        useMdlHdl = new Common.ModelHandle();
        useMdlHdl.Init();
        useMdlSkyHdl = new Common.ModelHandle();
        useMdlSkyHdl.Init();
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        for( int i=0; i<(int)ShapeTypeId.Max; i++ ){
            if( shapeColl[i] != null ){
                shapeColl[i].Term();
            }
            shapeColl[i] = null;
        }

        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }
        if( useMdlSkyHdl != null ){
            useMdlSkyHdl.Term();
        }

        useMdlHdl       = null;
        useMdlSkyHdl    = null;
    }

    /// 開始
    public override bool DoStart()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        useMdlHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Stage ),
                         resMgr.GetTextureContainer( (int)Data.ModelTexResId.Stage ),
                         resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                        );
        useMdlSkyHdl.Start( resMgr.GetModel( (int)Data.ModelResId.Sky ),
                         resMgr.GetTextureContainer( (int)Data.ModelTexResId.Sky ),
                         resMgr.GetShaderContainer( (int)Data.ModelShaderReslId.Normal )
                        );
        useMdlHdl.SetPlayAnim( 0, true );
        useMdlSkyHdl.SetPlayAnim( 0, true );
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
        useMdlSkyHdl.End();
        useMdlHdl.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        useMdlHdl.UpdateAnim();
        useMdlSkyHdl.UpdateAnim();
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        useMdlHdl.Render( graphDev, baseMtx );
        useMdlSkyHdl.Render( graphDev, baseMtx );

///       shapeColl[(int)ShapeTypeId.MoveGround2].Draw( graphDev, 0, new Rgba(0xff,0x00,0xff,0x80), new Rgba(0xff,0x00,0xff,0x80) );
       return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
    }



/// 形状関連
///---------------------------------------------------------------------------

    /// 衝突形状
    public override GameShapeProduct GetCollisionShape( Data.CollTypeId type, int no )
    {
        /// キャラの移動
        if( type == Data.CollTypeId.ChMove ){
            return shapeColl[no];
        }

        /// 目的地チェック
        else if( type == Data.CollTypeId.ChDestination ){
            return shapeColl[(int)ShapeTypeId.MoveGround+no];
        }

        return shapeColl[(int)ShapeTypeId.MoveMagic];
    }
    public override int GetCollisionShapeMax( Data.CollTypeId type )
    {
        /// キャラの移動
        if( type == Data.CollTypeId.ChMove ){
            return 3;
        }

        /// 目的地チェック
        else if( type == Data.CollTypeId.ChDestination ){
            return 2;
        }
        return 1;
    }





/// private メソッド
///---------------------------------------------------------------------------

    /// 衝突形状の作成
    private void makeShapeColl( int id, float[] posList )
    {
        shapeColl[id] = new ShapeTriangles();
        shapeColl[id].Init( posList.Length/9 );

        for( int i=0; i<posList.Length/9; i++ ){
            Vector3 pos1 = new Vector3( posList[i*9+0], posList[i*9+1], posList[i*9+2] );
            Vector3 pos2 = new Vector3( posList[i*9+3], posList[i*9+4], posList[i*9+5] );
            Vector3 pos3 = new Vector3( posList[i*9+6], posList[i*9+7], posList[i*9+8] );
            shapeColl[id].SetType( 0 );
            shapeColl[id].AddLight( pos1, pos2, pos3 );
        }
    }

}

} // namespace
