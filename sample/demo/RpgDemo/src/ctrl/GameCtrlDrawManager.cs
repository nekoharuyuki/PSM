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
/// 衝突判定用OBJをまとめるコンテナ
///***************************************************************************
public class GameCtrlDrawParam
{
    public GameActorProduct     Actor;
    public float                Dis;
    public int                  LodLev;

    public void Clear()
    {
        Actor    = null;
    }
}


///***************************************************************************
/// シングルトン：描画対象のアクタを管理
///***************************************************************************
public class GameCtrlDrawManager
{
    private static GameCtrlDrawManager instance = new GameCtrlDrawManager();

    private List< GameCtrlDrawParam >    objParamList;
    private List< GameCtrlDrawParam >    objWoodParamList;

    private        ShapeFrustum          cullingShape;

    private        const int             lodLevelMax = 4;
    private        float[]               cullingDis;
    private        Vector3               camPos;
    public         int                   woodCnt;
    private        const int             woodMax = 16;
    


    /// コンストラクタ
    private GameCtrlDrawManager()
    {
    }

    /// インスタンスの取得
    public static GameCtrlDrawManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        objParamList = new List< GameCtrlDrawParam >();
        if( objParamList == null ){
            return false;
        }
        objWoodParamList = new List< GameCtrlDrawParam >();
        if( objWoodParamList == null ){
            return false;
        }

        cullingShape = new ShapeFrustum();
        cullingShape.Init(1);

        cullingDis    = new float[lodLevelMax];
        return true;
    }

    /// 破棄
    public void Term()
    {
        clear();

        if( objParamList != null ){
            for( int i=0; i<objParamList.Count; i++ ){
                objParamList[i].Clear();
                objParamList[i] = null;
            }
            objParamList.Clear();
        }

        if( objWoodParamList != null ){
            for( int i=0; i<objWoodParamList.Count; i++ ){
                objWoodParamList[i].Clear();
                objWoodParamList[i] = null;
            }
            objWoodParamList.Clear();
        }

        if( cullingShape != null ){
            cullingShape.Term();
        }

        cullingShape     = null;
        objParamList     = null;
        objWoodParamList = null;
        cullingDis       = null;
    }

    /// 開始
    public void Start()
    {
        DemoGame.Camera camCore = GameCtrlManager.GetInstance().CtrlCam.GetCurrentCameraCore();
        cullingShape.Set( camCore.Projection.Inverse() );
        SetLodParam( 30.0f, 40.0f, 80.0f, 120.0f );
    }

    /// 描画
    public void Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].Actor.Draw( graphDev );
        }

        for( int i=0; i<objWoodParamList.Count; i++ ){
            /// 樹は最低woodMax個描画される
            if( (i+woodCnt) >= woodMax ){
                objWoodParamList[i].Actor.Visible = false;
                continue ;
            }
            objWoodParamList[i].Actor.LodLev = 2;
            objWoodParamList[i].Actor.Draw( graphDev );
        }
    }


    /// 登録開始
    public void EntryStart()
    {
        DemoGame.Camera camCore = GameCtrlManager.GetInstance().CtrlCam.GetCurrentCameraCore();
        Matrix4            mtx        = camCore.View.Inverse();

        Common.MatrixUtil.SetTranslate( ref mtx, camCore.Pos );
        cullingShape.SetMult( mtx );

        camPos = GameCtrlManager.GetInstance().CtrlCam.GetCamTrgPos();

        woodCnt = 0;

        clear();
    }


    /// キャラクタの登録
    public void EntryCharacter( GameActorProduct actor, bool cullingFlg )
    {
        ShapeSphere bndSph = actor.GetBoundingShape();

        if( cullingFlg == false || cullingShape.CheckNearDis( bndSph.Sphre.Pos ) < bndSph.Sphre.R ){
            float dis = Common.VectorUtil.Distance( actor.BasePos, GameCtrlManager.GetInstance().CtrlCam.GetCamPos() );
            entryActor( actor, dis );
        }
    }


    /// 備品の登録
    public void EntryFixture( ActorFixBase actor, bool cullingFlg )
    {
        ShapeSphere bndSph = actor.GetBoundingShape();

        if( cullingFlg == false || cullingShape.CheckNearDis( bndSph.Sphre.Pos ) < bndSph.Sphre.R ){
            float dis = Common.VectorUtil.Distance( actor.BasePos, camPos );
            entryActor( actor, dis );
        }
        else{
            actor.Visible = false;
        }
    }


    /// エフェクトの登録
    public void EntryEffect( GameActorProduct actor, bool cullingFlg )
    {
        float dis = Common.VectorUtil.Distance( actor.BasePos, camPos );
        entryActor( actor, dis );
    }


    /// LODパラメータのセット
    public void SetLodParam( float disLv1, float disLv2, float disLv3, float disLv4 )
    {
        cullingDis[0] = disLv1;
        cullingDis[1] = disLv2;
        cullingDis[2] = disLv3;
        cullingDis[3] = disLv4;
    }
    public void SetLodParam( int lv, float val )
    {
        cullingDis[lv] = val;
    }
    public float GetLodParam( int lv )
    {
        return cullingDis[lv];
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 登録
    public void entryActor( GameActorProduct actor, float dis )
    {
        GameCtrlDrawParam    drawParam = new GameCtrlDrawParam();

        int lv = 0;
        for( lv=0; lv<lodLevelMax; lv++ ){
            if( dis < cullingDis[lv] ){
                break;
            }
        }

        actor.LodLev        = lv;

        drawParam.Actor     = actor;
        drawParam.Dis       = dis;
        drawParam.LodLev    = lv;

        objParamList.Add( drawParam );
    }


    /// 登録
    public void entryActor( ActorFixBase actor, float dis )
    {
        GameCtrlDrawParam    drawParam = new GameCtrlDrawParam();

        int lv = 0;
        for( lv=0; lv<lodLevelMax; lv++ ){
            if( dis < cullingDis[lv] ){
                break;
            }
        }

        /// 一度描画されたものはレベル２の補正値が付く
        if( actor.GetFixTypeId() == Data.FixTypeId.Fix13 || actor.GetFixTypeId() == Data.FixTypeId.Fix14 ){
            if( actor.Visible ){
				/// 描画中の場合、描画されてからの距離補正が付く
                if( lv >= 3 && dis < (actor.DrawCamDis+1.0f) ){
                    lv = 2;
                }
            }
			else{
	            actor.DrawCamDis = dis;
			}
        }

        actor.LodLev        = lv;
        drawParam.Actor     = actor;
        drawParam.Dis       = dis;
        drawParam.LodLev    = lv;

        /// 樹はLodによって描画されない場合、敗者復活の機会がある
        if( actor.GetFixTypeId() == Data.FixTypeId.Fix13 || actor.GetFixTypeId() == Data.FixTypeId.Fix14 ){
            if( lv >= 3 ){
                objWoodParamList.Add( drawParam );            /// 敗者復活分のアクタを登録
            }
            else{
                objParamList.Add( drawParam );
                woodCnt ++;
            }
        }
        else{
            objParamList.Add( drawParam );
        }
    }


    /// 登録のクリア
    public void clear()
    {
        for( int i=0; i<objWoodParamList.Count; i++ ){
            objWoodParamList[i].Clear();
        }
        objWoodParamList.Clear();

        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].Clear();
        }
        objParamList.Clear();
    }


    /// 距離が近い順にソート
    public void SortNear()
    {
        /// 描画されなかった樹を距離が近い順にソート
        objWoodParamList.Sort( (x, y) => {
                if (x.Dis > y.Dis) {
                    return 1;
                }
                else if (x.Dis < y.Dis) {
                    return -1;
                }
                else {
                    return 0;
                }
               } );

        /// 描画対象の不透明OBJをカメラから近い順にソートを行い描画をしても
        /// 動作速度に変化がなかったため、ソート処理を行わない
    }


    /// 距離が遠い順にソート
    public void SortFar()
    {
        /// 描画対象のリスト
        objParamList.Sort( (x, y) => {
                if (x.Dis < y.Dis) {
                    return 1;
                }
                else if (x.Dis > y.Dis) {
                    return -1;
                }
                else {
                    return 0;
                }
            } );
    }


/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数
    public int Num
    {
        get {return objParamList.Count;}
    }

    /// LODのレベル上限
    public int LodLevMax
    {
        get {return lodLevelMax;}
    }
}

} // namespace
