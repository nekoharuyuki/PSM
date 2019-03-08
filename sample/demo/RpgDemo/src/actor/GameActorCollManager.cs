/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;

namespace AppRpg {

///***************************************************************************
/// 衝突パラメータ
///***************************************************************************
public class GameActorCollManager
{

    private bool isCollisionHitFlg;



/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public void Init()
    {
        TrgContainer    = new GameActorCollObjContainer();
        TrgContainer.Init();
    }

    /// 破棄
    public void Term()
    {
        if( TrgContainer != null ){
            TrgContainer.Term();
        }
        MoveShape       = null;
        TrgContainer    = null;
    }

    /// 移動対象の形状を登録
    public void SetMoveShape( GameShapeProduct shape )
    {
        MoveShape       = shape;
    }


    /// 衝突判定の開始
    public void StartCollision()
    {
        TrgContainer.StartCollision();
        isCollisionHitFlg = false;
    }

    /// 衝突パラメータの登録
    public void SetCollParam( int entIdx, int shapeId, int primId, float dis )
    {
        TrgContainer.SetCollParam( entIdx, shapeId, primId, dis );
        isCollisionHitFlg = true;
    }

    /// 衝突したかのチェック
    public bool CheckCollHit()
    {
        return isCollisionHitFlg;
    }

    /// 衝突したOBJのソート
    public void SortCollisionTrg()
    {
        TrgContainer.SortNearDis();
    }




/// private メソッド
///---------------------------------------------------------------------------


/// プロパティ
///---------------------------------------------------------------------------
    public GameShapeProduct             MoveShape;
    public GameActorCollObjContainer    TrgContainer;
}

} // namespace
