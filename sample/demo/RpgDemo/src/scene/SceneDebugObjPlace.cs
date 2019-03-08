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
/// シーン：デバックOBJ配置
///***************************************************************************
public class SceneDebugObjPlace : DemoGame.IScene
{
    private enum debugMenuTaskId{
        SelectObj,
        MoveObj,
        GravityObj,
        DeleteObj
    };
    private string[] menuStringList = {
        "Move",
        "Gravity",
        "Delete"
    };

    private DemoGame.SceneManager      useSceneMgr;
    private debugMenuTaskId            nowTaskId;
    private debugMenuTaskId            nextTaskId;

    private int                        topMenuId;
        
    private int                        trgObjType = 0;
    private int                        trgObjMax;

    private int                        trgObjIdx;
    private Vector3                    trgObjPos;
    private Vector3                    trgObjRot;
    private float                      trgObjMoveSpd;

    private ActorUnitCollGravity       calCollGrav;
    GameActorCollManager               moveCollMgr;
    private ShapeSphere                shapeMove;

    private DemoGame.RenderGeometry    renderSph;
///    private GameObjProduct          trgObj;



/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public void SetTrgType( int type )
    {
        trgObjType = type;
    }

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        useSceneMgr = sceneMgr;

        calCollGrav    = new ActorUnitCollGravity();
        calCollGrav.Init();

        moveCollMgr = new GameActorCollManager();
        moveCollMgr.Init();

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, new Vector3(0.0f, 0.0f, 0.0f), 0.001f );

        renderSph = new DemoGame.RenderGeometry();
        renderSph.MakeSphere();
///        trgObj        = null;

        /// 移動する自身のOBJを登録
        moveCollMgr.SetMoveShape( shapeMove ); 


        trgObjMoveSpd = 1.0f;
        setPlaceTypeParam( trgObjType );

        nowTaskId    = debugMenuTaskId.SelectObj;
        changeTask( debugMenuTaskId.SelectObj );
        return true;
    }

    /// シーンの破棄
    public void Term()
    {
        if( calCollGrav != null ){
            calCollGrav.Term();
        }
        if( moveCollMgr != null ){
            moveCollMgr.Term();
        }
        if( shapeMove != null ){
            shapeMove.Term();
        }
        if( renderSph != null ){
            renderSph.Dispose();
        }

///        trgObj            = null;
        renderSph        = null;
        shapeMove        = null;
        moveCollMgr        = null;
        calCollGrav        = null;
        useSceneMgr        = null;
    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }
    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
        switch( nowTaskId ){
        case debugMenuTaskId.SelectObj:        frameSelectObj();    break;
        case debugMenuTaskId.MoveObj:        frameMoveObj();        break;
        case debugMenuTaskId.GravityObj:    frameGravityObj();    break;
        case debugMenuTaskId.DeleteObj:        frameDeleteObj();    break;
        }


        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        ctrlResMgr.CtrlCam.FramePlace( trgObjPos );

        return true;
    }


    /// 描画処理
    public bool Render()
    {
        if( nowTaskId != nextTaskId ){
            nowTaskId    = nextTaskId;
            return true;
        }

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.0f, 0.025f, 0.25f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;

        ctrlResMgr.DrawDebug();

        switch( nowTaskId ){
        case debugMenuTaskId.SelectObj:        renderSelectObj();    break;
        case debugMenuTaskId.MoveObj:        renderMoveObj();    break;
        case debugMenuTaskId.GravityObj:    renderGravityObj();    break;
        case debugMenuTaskId.DeleteObj:        renderDeleteObj();    break;
        }

/// 境界ボリュームの表示
///        if( trgObj != null ){
///            trgObj.GetBoundSphere().Draw( useGraphDev, 0, new Rgba(0xff, 0x00, 0x00, 0x80), new Rgba(0xff, 0x00, 0x00, 0x80) );
///        }

        /// 描画
        useGraphDev.Graphics.SwapBuffers();

        return true;
    }


    /// タスクの変更
    private void changeTask( debugMenuTaskId task)
    {
        nextTaskId        = task;
    }



    /// 初期配置情報のセット
    private void setPlaceTypeParam( int type )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        trgObjIdx    = 0;
        trgObjMax    = 0;
        trgObjPos    = ctrlResMgr.CtrlPl.GetPos();

        /// 敵
        if( type == 0 ){
            trgObjMax    = ctrlResMgr.CtrlEn.GetEntryNum();
            if( trgObjMax > 0 ){
                setPlaceTrgParam( 0 );
            }
        }
        else {
            trgObjMax    = ctrlResMgr.CtrlFix.GetEntryNum();
            if( trgObjMax > 0 ){
                setPlaceTrgParam( 0 );
            }
        }
    }

    private void setPlaceTrgObj( int idx )
    {
//        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
//        if( trgObjType == 0 ){
//            trgObj    = ctrlResMgr.CtrlEn.GetUseActorBaseObj( idx );
//        }
//        else {
//            trgObj    = ctrlResMgr.CtrlFix.GetUseFixActorBaseObj( idx );
//        }
    }

        
    /// 配置対象の情報をセット
    private void setPlaceTrgParam( int idx )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        if( trgObjMax > 0 ){
            /// 敵
            if( trgObjType == 0 ){
                trgObjPos    = ctrlResMgr.CtrlEn.GetPos( idx );
                trgObjRot.Y    = ctrlResMgr.CtrlEn.GetRotY( idx );
            }
            else {
                trgObjPos    = ctrlResMgr.CtrlFix.GetPos( idx );
                trgObjRot    = ctrlResMgr.CtrlFix.GetRot( idx );
            }
            setPlaceTrgObj( idx );
        }
    }


    /// 配置対象の移動
    private void setTrgObjMove( Vector3 move )
    {
        GameCtrlManager        ctrlResMgr    = GameCtrlManager.GetInstance();

        float a_Cal        = (float)(3.141593f / 180.0);
        float angleY    = ctrlResMgr.CtrlCam.GetCamRotY() * a_Cal;
        Matrix4 mtx = Matrix4.RotationY( angleY );

        trgObjPos.X = trgObjPos.X + mtx.M11 * move.X;
        trgObjPos.Z = trgObjPos.Z + mtx.M13 * move.X;
        trgObjPos.X = trgObjPos.X + mtx.M31 * move.Z;
        trgObjPos.Z = trgObjPos.Z + mtx.M33 * move.Z;
        trgObjPos.Y = trgObjPos.Y + move.Y;

        /// 敵
        if( trgObjType == 0 ){
            ctrlResMgr.CtrlEn.SetPlace( trgObjIdx, trgObjRot.Y, trgObjPos );
        }
        else {
            ctrlResMgr.CtrlFix.SetPlace( trgObjIdx, trgObjRot, new Vector3(1.0f,1.0f,1.0f), trgObjPos );
        }
    }


/// メニュートップ
///---------------------------------------------------------------------------

    public bool frameSelectObj()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// 戻る
        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            useSceneMgr.Prev();
            return true;
        }

        /// 未登録
        if( trgObjMax <= 0 ){
            return true;
        }


        if( (pad.Trig & DemoGame.InputGamePadState.Left) != 0 ){
            trgObjIdx --;
            if( trgObjIdx < 0 ){
                trgObjIdx = trgObjMax-1;
            }
            setPlaceTrgParam( trgObjIdx );
        }

        else if( (pad.Trig & DemoGame.InputGamePadState.Right) != 0 ){
            trgObjIdx = (trgObjIdx+1)%trgObjMax;
            setPlaceTrgParam( trgObjIdx );
        }

        else if( (pad.Trig & DemoGame.InputGamePadState.Up) != 0 ){
            topMenuId --;
            if( topMenuId < 0 ){
                topMenuId = menuStringList.Length-1;
            }
        }

        else if( (pad.Trig & DemoGame.InputGamePadState.Down) != 0 ){
            topMenuId = (topMenuId+1)%menuStringList.Length;
        }


        /// アクションの決定
        else if( (pad.Trig & DemoGame.InputGamePadState.Circle) != 0 ){
            switch( topMenuId ){
            case 0:     changeTask( debugMenuTaskId.MoveObj );        break;
            case 1:     changeTask( debugMenuTaskId.GravityObj );    break;
            case 2:        changeTask( debugMenuTaskId.DeleteObj );    break;
            }
        }
        return true;
    }
    public bool renderSelectObj()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, useGraphDev.DisplayWidth, useGraphDev.DisplayHeight );

        if( trgObjType == 0 ){
            DemoGame.Graphics2D.AddSprite( "Mess", "Enemy Place TrgObj "+trgObjIdx, 0xff40ff40, 2, 2 );
        }
        else{
            DemoGame.Graphics2D.AddSprite( "Mess", "Fix Place TrgObj "+trgObjIdx, 0xff40ff40, 2, 2 );
        }

        /// 未登録
        if( trgObjMax <= 0 ){
            DemoGame.Graphics2D.AddSprite( "Mess1", "OBJ No Entry", 0xffff0000, 30, 36 );
        }

        /// 選択中のOBJ
        else{
            for( int i=0; i<menuStringList.Length; i++ ){
                if( topMenuId == i ){
                    DemoGame.Graphics2D.AddSprite( "Mess"+i, menuStringList[i], 0xffff0000, 30, 36+i*28 );
                }
                else{
                    DemoGame.Graphics2D.AddSprite( "Mess"+i, menuStringList[i], 0xffffffff, 30, 36+i*28 );
                }
                DemoGame.Graphics2D.DrawSprites();
            }
        }

        /// スプライトの描画
        DemoGame.Graphics2D.DrawSprites();

        /// スプライトの削除
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        for( int i=0; i<menuStringList.Length; i++ ){
            DemoGame.Graphics2D.RemoveSprite( "Mess"+i );
        }

        return true;
    }
        


/// OBJの移動
///---------------------------------------------------------------------------

    public bool frameMoveObj()
    {
        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        /// 戻る
        if( (pad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
            changeTask( debugMenuTaskId.SelectObj );
            return true;
        }

        /// 速度変更
        if( (pad.Trig & DemoGame.InputGamePadState.Triangle) != 0 ){
            trgObjMoveSpd += 1.0f;
            if( trgObjMoveSpd >= 8.0f ){
                trgObjMoveSpd = 1.0f;
            }
        }

        Vector3 move = new Vector3( 0.0f, 0.0f, 0.0f );

        /// 回転
        if( (pad.Scan & DemoGame.InputGamePadState.Square) != 0 ){
            if( (pad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
                trgObjRot.Y -= (1.0f*trgObjMoveSpd);
                if( trgObjRot.Y <= 0.0f ){
                    trgObjRot.Y += 360.0f;
                }
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
                trgObjRot.Y += (1.0f*trgObjMoveSpd);
                if( trgObjRot.Y >= 360.0f ){
                    trgObjRot.Y -= 360.0f;
                }
            }
        }

        /// 移動
        else{
            if( (pad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
                move.X = -1.0f;
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
                move.X = 1.0f;
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Up) != 0 ){
                move.Z = -1.0f;
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Down) != 0 ){
                move.Z = 1.0f;
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Circle) != 0 ){
                move.Y = 1.0f;
            }
            if( (pad.Scan & DemoGame.InputGamePadState.Cross) != 0 ){
                move.Y = -1.0f;
            }
        }
        setTrgObjMove( (move*trgObjMoveSpd*0.01f) );
    
        return true;
    }
    public bool renderMoveObj()
    {
        DemoGame.Graphics2D.FillRect( 0x80000000, 0, 0, 300, 4+26*2 );

        int drawY = 2;

        DemoGame.Graphics2D.AddSprite( "Mess", "ObjMove TrgObj "+trgObjIdx, 0xff40ff40, 2, drawY );
        drawY += 26;
        DemoGame.Graphics2D.AddSprite( "Mess1", "Spd "+trgObjMoveSpd, 0xffa0ffa0, 2, drawY );
        drawY += 26;

        /// スプライトの描画
        DemoGame.Graphics2D.DrawSprites();

        /// スプライトの削除
        DemoGame.Graphics2D.RemoveSprite( "Mess" );
        DemoGame.Graphics2D.RemoveSprite( "Mess1" );

        return true;
    }


/// OBJの落下
///---------------------------------------------------------------------------

    public bool frameGravityObj()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        Vector3 movePos = trgObjPos;
        movePos.Y = 0.0f;

        /// 対象のOBJを登録
        moveCollMgr.TrgContainer.Clear();
        ctrlResMgr.CtrlStg.SetCollisionActor( moveCollMgr.TrgContainer, trgObjPos );
        ctrlResMgr.CtrlFix.SetCollisionActor( moveCollMgr.TrgContainer, trgObjPos );

        shapeMove.Set( 0, movePos, 0.001f );
        calCollGrav.GetMovePos( moveCollMgr, ref movePos );
        shapeMove.Set( 0, trgObjPos, 0.001f );

        calCollGrav.Check( moveCollMgr, movePos );


        trgObjPos = calCollGrav.NextPos;
        setTrgObjMove( new Vector3( 0.0f, 0.0f, 0.0f ) );

        changeTask( debugMenuTaskId.SelectObj );
        return true;
    }
    public bool renderGravityObj()
    {
        return true;
    }


/// OBJの削除
///---------------------------------------------------------------------------

    public bool frameDeleteObj()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        if( trgObjType == 0 ){
            ctrlResMgr.CtrlEn.DeleteEntryEnemy( trgObjIdx );
            trgObjMax = ctrlResMgr.CtrlEn.GetEntryNum();
        }
        else{
            ctrlResMgr.CtrlFix.DeleteEntryFix( trgObjIdx );
            trgObjMax = ctrlResMgr.CtrlFix.GetEntryNum();
        }

        trgObjIdx --;
        if( trgObjIdx < 0 ){
            trgObjIdx = trgObjMax - 1;
            if( trgObjIdx < 0 ){
                trgObjIdx = 0;
            }
        }
        setPlaceTrgParam( trgObjIdx );

        changeTask( debugMenuTaskId.SelectObj );
        return true;
    }
    public bool renderDeleteObj()
    {
        return true;
    }
}


} // namespace
