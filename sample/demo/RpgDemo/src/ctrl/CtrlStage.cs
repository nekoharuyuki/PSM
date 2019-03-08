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
/// ステージ制御
///***************************************************************************
public class CtrlStage
{

    private ActorStgNormal            actorStg;
    private ActorDestinationMark      actorDestination;

    private bool                      destinationFlg;
    private Vector3                   destinationPos;
    private ActorUnitCollLook         calCollLook;
    private GameActorProduct          destinationTrgActor;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorStg = new ActorStgNormal();
        actorStg.Init();

        actorDestination = new ActorDestinationMark();
        actorDestination.Init();

        calCollLook    = new ActorUnitCollLook();
        calCollLook.Init();

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( calCollLook != null ){
            calCollLook.Term();
        }
        actorStg.Term();
        actorDestination.Term();

        calCollLook       = null;
        actorStg          = null;
        actorDestination  = null;
    }

    /// 開始
    public bool Start()
    {
        actorStg.Start();

        ClearDestination();
        return true;
    }

    /// 終了
    public void End()
    {
        actorStg.End();
        actorDestination.End();
        destinationTrgActor = null;
    }


    /// フレーム処理
    public bool Frame()
    {
        actorStg.Frame();

		/// 目標地点が消失
		if( destinationTrgActor != null && destinationTrgActor.Enable == false ){
			ClearDestination();
		}


        /// 目的地のセット
        if( (AppInput.GetInstance().Event & AppInput.EventId.DestinationMove) != 0 ){
            setDestination( AppInput.GetInstance().SingleScrPosX, AppInput.GetInstance().SingleScrPosY );
            destinationFlg = true;
        }
        else if( (AppInput.GetInstance().Event & AppInput.EventId.DestinationCancel) != 0 ){
            ClearDestination();
        }

        if( actorDestination.Enable ){
            actorDestination.Frame();
        }
        return true;
    }

    /// フレーム処理（タイトル画面時）
    public bool FrameTitle()
    {
        actorStg.Frame();
        ClearDestination();
        return true;
    }

    /// フレーム処理（リザルト画面時）
    public bool FrameResult()
    {
        actorStg.Frame();
        ClearDestination();
        return true;
    }

    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        actorStg.Draw( graphDev );

        return true;
    }

    /// 目的地マーカーの描画
    public bool DrawDestinationMark( DemoGame.GraphicsDevice graphDev )
    {
        if( actorDestination.Enable ){
            actorDestination.Draw( graphDev );
        }
        return true;
    }

    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        for( int i=0; i<actorStg.GetUseObjNum(); i++ ){
            container.Add( actorStg, actorStg.GetUseObj(i) );
        }
        return true;
    }

    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetInterfereActor( GameActorContainer container )
    {
        return true;
    }

    /// 目的地クリア
    public void ClearDestination()
    {
        destinationFlg          = false;
        actorDestination.Enable = false;
        destinationTrgActor     = null;
    }

    /// 目的地が有効かのフラグ
    public bool CheckDestination()
    {
        return destinationFlg;
    }
    /// 目的地の座標がセットされているかのフラグ
    public bool CheckDestinationTrg()
    {
        return actorDestination.Enable;
    }
    /// 目的地の座標を取得
    public Vector3 GetDestinationPos()
    {
        return destinationPos;
    }
    /// 目的地のOBJを取得
    public GameActorProduct GetDestinationActor()
    {
        return destinationTrgActor;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 目的地セット
    public void setDestination( int scrPosX, int scrPosY )
    {
        GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlFix.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlEn.SetDestinationActor( useCollMgr.TrgContainer );
        useCollMgr.TrgContainer.Add( actorStg, actorStg.GetUseObj(0) );



        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            /// マーカーのセット
            actorDestination.Start();

            Matrix4 mtx = Matrix4.RotationY( 0 );

            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
            ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();

            if( destinationTrgActor.CheckMoveTrgId() == 1 ){
                if( bndSph != null ){
                    destinationPos = bndSph.Sphre.Pos;
                }
                else{
                    destinationPos = destinationTrgActor.BasePos;
                }
                actorDestination.SetType( 0 );
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 2 ){
                destinationPos = destinationTrgActor.BasePos;
                if( bndSph != null ){
                    destinationPos.Y += bndSph.Sphre.R;
                }
                actorDestination.SetType( 0 );
            }
            else{
                destinationPos = calCollLook.NextPos;
                destinationPos.Y += 0.02f;
                actorDestination.SetType( 1 );
            }

            Common.MatrixUtil.SetTranslate( ref mtx, destinationPos );
            actorDestination.SetPlace( mtx );

            actorDestination.Enable = true;
        }
        else{
            actorDestination.Enable = false;
        }
    }


}

} // namespace
