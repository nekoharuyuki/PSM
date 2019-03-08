/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg
{

///***************************************************************************
/// 形状基底
///***************************************************************************
public class GameShapeProduct
{
    /// 形状タイプID
    ///--------------------------------
    public enum TypeId{
        Non,
        Capsule,
        Fan,
        Frustum,
        Sphere,
        Triangles
    };

    protected TypeId        ShapeType;


        
/// public メソッド
///---------------------------------------------------------------------------

    /// 形状のタイプIDを返す
    public TypeId GetShapeType()
    {
        return ShapeType;
    }


/// 仮想メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public virtual bool Init( int num )
    {
        ShapeType = TypeId.Non;
        return false;
    }

    /// 破棄
    public virtual void Term()
    {
    }

    /// 行列変換
    public virtual void SetMult( Matrix4 mtx )
    {
    }
}

} // namespace
