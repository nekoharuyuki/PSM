/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;


namespace AppRpg {

///***************************************************************************
/// 備品制御
///***************************************************************************
public class CtrlFixture
{
    const int gridNum = 120;
    const int gridMax = gridNum*gridNum;
    const float gridAreaDis = 3.0f;
    const float gridWidth = 5.0f;

    private List< ActorFixBase >    actorFixList;
    private List< int >             fixFragileIdxList;

    private List< int >[]           gridActorIdxList;

    private Common.ModelHandle[]    plantModelHdl;            /// 速度向上のためアニメーションを共有する



/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorFixList = new List< ActorFixBase >();
        if( actorFixList == null ){
            return false;
        }

        fixFragileIdxList = new List< int >();
        if( fixFragileIdxList == null ){
            return false;
        }

        gridActorIdxList = new List< int >[gridMax];
        for( int i=0; i<gridMax; i++ ){
            gridActorIdxList[i] = new List< int >();
        }

        plantModelHdl = new Common.ModelHandle[(int)Data.PlantTypeId.Max];
        for( int i=0; i<(int)Data.PlantTypeId.Max; i++ ){
            plantModelHdl[i] = new Common.ModelHandle();
            plantModelHdl[i].Init();
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( actorFixList != null ){
            for( int i=0; i<actorFixList.Count; i++ ){
                actorFixList[i].Term();
                actorFixList[i] = null;
            }
            actorFixList.Clear();
        }

        if( fixFragileIdxList != null ){
            fixFragileIdxList.Clear();
        }

        for( int i=0; i<gridMax; i++ ){
            gridActorIdxList[i].Clear();
        }

        for( int i=0; i<(int)Data.PlantTypeId.Max; i++ ){
            plantModelHdl[i].Term();
            plantModelHdl[i] = null;
        }

        actorFixList        = null;
        fixFragileIdxList   = null;
        gridActorIdxList    = null;
        plantModelHdl       = null;
    }

    /// 開始
    public bool Start()
    {
        for( int i=0; i<actorFixList.Count; i++ ){
            actorFixList[i].Start();
        }

        setBeforehandMdlBindTex();
        setPlantModel( 0, (int)Data.FixTypeId.Fix03 );
        setPlantModel( 1, (int)Data.FixTypeId.Fix13 );
        setPlantModel( 2, (int)Data.FixTypeId.Fix14 );
        return true;
    }

    /// 終了
    public void End()
    {
        for( int i=actorFixList.Count-1; i>=0; i-- ){
            actorFixList[i].End();
        }

        for( int i=0; i<gridMax; i++ ){
            gridActorIdxList[i].Clear();
        }
        for( int i=0; i<(int)Data.PlantTypeId.Max; i++ ){
            plantModelHdl[i].End();
        }
        actorFixList.Clear();
        fixFragileIdxList.Clear();
    }


    /// フレーム処理
    public bool Frame()
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        int actorIdx;
        for( int i=0; i<fixFragileIdxList.Count; i++ ){

            actorIdx = fixFragileIdxList[i];

            /// 破棄された備品はスルーする
            ///-------------------------------------
            if( !actorFixList[actorIdx].Enable ){
                continue ;
            }

            /// 他アクタからのイベントをチェック
            ///-------------------------------------
            if( actorFixList[actorIdx].EventCntr.Num > 0 ){
                ctrlResMgr.CtrlEvent.Play( actorFixList[actorIdx], actorFixList[actorIdx].EventCntr );
            }

            /// フレーム処理
            ///-------------------------------------
            actorFixList[actorIdx].Frame();

            /// 自身発生のイベントをチェック
            ///-------------------------------------
            if( actorFixList[actorIdx].EventCntr.Num > 0 ){
                ctrlResMgr.CtrlEvent.Play( actorFixList[actorIdx], actorFixList[actorIdx].EventCntr );
            }
        }


        /// 草木のアニメーション更新
        ///-------------------------------------
        for( int i=0; i<(int)Data.PlantTypeId.Max; i++ ){
            plantModelHdl[i].UpdateAnim();
            plantModelHdl[i].UpdateBindAnim();
        }

        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorFixList.Count; i++ ){

            /// 破棄された備品はスルーする
            ///-------------------------------------
            if( !actorFixList[i].Enable ){
                continue ;
            }
            
            GameCtrlDrawManager.GetInstance().EntryFixture( actorFixList[i], true );
        }

        return true;
    }




    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        int gridW = (int)( (300.0f+trgPos.X)/gridWidth );
        int gridH = (int)( (300.0f+trgPos.Z)/gridWidth );

        if( gridW < 0 || gridW >= gridNum || gridH < 0 || gridH >= gridNum ){
            return false;
        }
            
        int grid = ( gridW + gridH*gridNum );
        for( int i=0; i<gridActorIdxList[grid].Count; i++ ){
            int idx = gridActorIdxList[grid][i];

            /// 破棄された備品はスルーする
            ///-------------------------------------
            if( !actorFixList[idx].Enable ){
                continue ;
            }

            container.Add( actorFixList[idx], actorFixList[idx].GetUseObj(0) );
        }

        return true;
    }


    /// 目的地対象をコンテナへ登録
    public void SetDestinationActor( GameActorCollObjContainer container )
    {
        int actorIdx;
        for( int i=0; i<fixFragileIdxList.Count; i++ ){

            actorIdx = fixFragileIdxList[i];

            /// 破棄された備品はスルーする
            ///-------------------------------------
            if( !actorFixList[actorIdx].Enable ){
                continue ;
            }

            /// 非表示の備品はスルーする
            ///-------------------------------------
            else if( !actorFixList[actorIdx].Enable ){
                continue ;
            }

            container.Add( actorFixList[actorIdx], actorFixList[actorIdx].GetUseObj(0) );
        }
    }


    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetInterfereActor( GameActorContainer container )
    {
        int actorIdx;
        for( int i=0; i<fixFragileIdxList.Count; i++ ){
            actorIdx = fixFragileIdxList[i];

            /// 破棄された備品はスルーする
            ///-------------------------------------
            if( !actorFixList[actorIdx].Enable ){
                continue ;
            }

            container.Add( actorFixList[actorIdx] );
        }
        return true;
    }


    /// ベースOBJの取得
    public GameObjProduct GetUseFixActorBaseObj( int idx )
    {
        return actorFixList[idx].GetUseObj(0);
    }

    /// 備品の登録
    public void EntryAddFix( int fixResId, Vector3 rot, Vector3 scale, Vector3 pos )
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        if( resMgr.GetModel( (int)Data.ModelResId.Fix00 + fixResId ) == null ){
            return ;
        }

        ActorFixBase actorFix;

        Data.FixTypeId fixId = (Data.FixTypeId)fixResId;

        if( fixId == Data.FixTypeId.Fix00 || fixId == Data.FixTypeId.Fix01 || fixId == Data.FixTypeId.Fix03 || fixId == Data.FixTypeId.Fix04 ){
            actorFix = new ActorFixWooden();
        }
        else{
            actorFix = new ActorFixNormal();
        }

        actorFix.Init();
        actorFix.Start();
        actorFix.SetMdlHandle( fixId );

        actorFixList.Add( actorFix );

        SetPlace( (actorFixList.Count-1), rot, scale, pos );
    }

    /// 備品の登録削除
    public void DeleteEntryFix( int idx )
    {
        actorFixList.RemoveAt( idx );
    }

    /// 備品の配置位置を変更
    public void SetPlace( int idx, Vector3 rot, Vector3 scale, Vector3 pos )
    {
        actorFixList[idx].SetPlace( rot, scale, pos );
    }

    /// 備品の座標を取得
    public Vector3 GetPos( int idx )
    {
        return new Vector3( actorFixList[idx].BaseMtx.M41, actorFixList[idx].BaseMtx.M42, actorFixList[idx].BaseMtx.M43 );
    }
    /// 備品の向きを取得
    public Vector3 GetRot( int idx )
    {
        return actorFixList[idx].ObjAngle;
    }
    /// 備品のスケールを取得
    public Vector3 GetScale( int idx )
    {
        return actorFixList[idx].ObjScale;
    }
    /// 登録タイプの取得
    public int GetFixTypeId( int idx )
    {
        return (int)actorFixList[idx].GetFixTypeId();
    }
    /// 登録数の取得
    public int GetEntryNum()
    {
        return actorFixList.Count;
    }


    /// アクタの登録情報の更新
    public void UpdateFixEntry()
    {
        updateGridEntryList();
        UpdateFixIdxList();
    }

    public void UpdateFixIdxList()
    {
        updateFragileIdxList();
    }


/// アクタの所属マップ関連
///---------------------------------------------------------------------------

    /// アクタの所属マップ生成
    public void AddGridEntry( int grid, int val )
    {
        gridActorIdxList[grid].Add( val );
    }

    public int GetGridEntryListMax()
    {
        return gridMax;
    }
    public int GetGridEntryNum( int grid )
    {
        return gridActorIdxList[grid].Count;
    }
    public int GetGridEntryParam( int grid, int idx )
    {
        return gridActorIdxList[grid][idx];
    }




/// private
///---------------------------------------------------------------------------

    /// 破壊可能の備品リストを生成
    private void updateFragileIdxList()
    {
        fixFragileIdxList.Clear();
        for( int i=0; i<actorFixList.Count; i++ ){
            if( actorFixList[i].CheckBreakObj() ){
                fixFragileIdxList.Add( i );
            }
        }
    }


    /// アクタの所属マップ生成
    private void updateGridEntryList()
    {
        Vector3 pos = new Vector3(0,0,0);

        for( int i=0; i<gridMax; i++ ){
            gridActorIdxList[i].Clear();
        }

        for( int x=0; x<gridNum; x++ ){
            pos.X = -300.0f + 2.5f + x * 5.0f;

            for( int y=0; y<gridNum; y++ ){
                pos.Z = -300.0f + 2.5f + y * 5.0f;

                for( int i=0; i<actorFixList.Count; i++ ){
                    ShapeSphere bndSph = actorFixList[i].GetBoundingShape();
                    if( Common.VectorUtil.DistanceXZ( pos, GetPos(i) ) < gridAreaDis+bndSph.Sphre.R ){
                        gridActorIdxList[x+y*gridNum].Add( i );
                    }
                }
            }
        }
    }


    /// 草木のモデルセット
    private void setPlantModel( int id, int fixTypeIdx )
    {
        /// 草木のアニメーションは配置物個々で行うと処理の負荷がかかるため、
        /// CTRLにて全草木の配置物共通でアニメーションを行う

        int    mdlResId    = (int)Data.ModelResId.Fix00 + fixTypeIdx;
        int    texResId    = (int)Data.ModelTexResId.Fix00 + fixTypeIdx;
        int    shaResId    = (int)Data.ModelShaderReslId.Normal;

        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
        plantModelHdl[id].Start( resMgr.GetModel( mdlResId ),
                                 resMgr.GetTextureContainer( texResId ), resMgr.GetShaderContainer( shaResId )    );

        plantModelHdl[id].SetPlayAnim( 0, true );
    }


    /// モデルの事前セット
    private void setBeforehandMdlBindTex()
    {
        /// LOD処理の度にモデルにテクスチャをBindする速度を稼ぐため、
        /// 事前に全モデルに対してテクスチャをBindしておく

        ObjFixNormal fixObj = new ObjFixNormal();
        fixObj.Init();

        for( int i=0; i<(int)Data.FixTypeId.Max; i++ ){
            for( int lod=0; lod<(int)GameCtrlDrawManager.GetInstance().LodLevMax; lod++ ){
                fixObj.SetBeforehandMdlBindTex( (Data.FixTypeId)i, lod );
            }
        }

        fixObj.Term();
        fixObj = null;
    }
}

} // namespace
