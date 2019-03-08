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
/// アクタ基底
///***************************************************************************
public class GameActorProduct
{
    public Vector3     BasePos;
    public Matrix4     BaseMtx;
    public bool        Enable;
    public bool        Visible;
    public int         LodLev;

    public  GameActorEventContainer    EventCntr;


/// 仮想メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public virtual bool Init()
    {
        return false;
    }

    /// 破棄
    public virtual void Term()
    {
        Enable = false;
        Visible = false;
    }

    /// 開始
    public virtual bool Start()
    {
        LodLev    = 0;
        Enable    = false;
        Visible   = false;
        return false;
    }

    /// 破棄
    public virtual void End()
    {
    }

    /// フレーム
    public virtual bool Frame()
    {
        return false;
    }

    /// 描画
    public virtual bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        return false;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// アクタの配置
    public void SetPlace( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );

        DoSetMatrix( mtx );
    }


    /// 使用するOBJの取得
    public GameObjProduct GetUseObj( int index )
    {
        return( DoGetUseObj( index ) );
    }

    /// 使用するOBJの数を取得
    public int GetUseObjNum()
    {
        return( DoGetUseObjNum() );
    }

    /// 境界ボリュームの取得
    public ShapeSphere GetBoundingShape()
    {
        return( DoGetBoundingShape() );
    }

    /// 対象の座標を向く
    public void SetLookTrgPos( Vector3 trgPos )
    {
        Vector3 look = (trgPos-BasePos);
        look.Y = 0.0f;
        Common.MatrixUtil.LookTrgVec( ref BaseMtx, look );
        SetPlace( BaseMtx );
    }



/// 仮想メソッド
///---------------------------------------------------------------------------

    protected virtual GameObjProduct DoGetUseObj( int index )
    {
        return null;
    }
    protected virtual int DoGetUseObjNum()
    {
        return 0;
    }
    protected virtual void DoSetMatrix( Matrix4 mtx )
    {
    }
    protected virtual ShapeSphere DoGetBoundingShape()
    {
        return null;
    }


/// アクタイベント
///---------------------------------------------------------------------------

    /// ダメージ
    public virtual void SetEventDamage( GameObjProduct trgObj, Data.AttackTypeId dmgId )
    {
    }

    /// 対象の座標へ振り向く
    public virtual void SetEventTurnPos( Vector3 trgPos, int rot )
    {
    }

    /// スーパーアーマー化
    public virtual void SetEventSuperArm()
    {
    }

    /// 動作キャンセル
    public virtual void SetEventMvtCancel()
    {
    }

    /// 移動対象IDの取得
    public virtual int CheckMoveTrgId()
    {
        if( DoGetUseObj(0) != null ){
            return DoGetUseObj(0).CheckMoveTrgId();
        }
        return 0;
    }

}

} // namespace
