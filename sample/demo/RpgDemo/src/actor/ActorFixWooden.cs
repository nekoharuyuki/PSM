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
/// ACTOR : 木製の備品（破壊可能な備品）
///***************************************************************************
public class ActorFixWooden : ActorFixBase
{


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objFix = new ObjFixNormal();
        objFix.Init();

        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        objFix.Term();
        objFix    = null;
    }

    /// 開始
    protected override bool DoStart()
    {
        objFix.Start();

        breakFlg = true;

        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        objFix.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
        /// 壊れる
        if( brokenFlg == true ){

            /// 草を刈る
            if( fixTypeId == Data.FixTypeId.Fix03 ){
                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff11, BasePos );
            }

            /// 木製の備品の破壊
            else{
                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff09, BasePos );
                AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ObjBreak, BasePos );
            }

            Enable = false;
            return false;
        }

        objFix.Frame();
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objFix.Draw( graphDev );
        return true;
    }




/// private メソッド
///---------------------------------------------------------------------------

    /// Y軸ビルボード用姿勢セット
    protected void setBillboardMatrixY( DemoGame.Camera camera )
    {
        Vector3 look = (camera.Pos - BasePos);
        look.Y = 0.0f;

        Common.MatrixUtil.LookTrgVec( ref BaseMtx, look );
        objFix.SetMatrixNoUpdate( BaseMtx );
    }


}

} // namespace
