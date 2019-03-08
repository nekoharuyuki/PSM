/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 追尾カメラの操作
///***************************************************************************
public class ActorCamBehind : ActorCamBase
{

    private Vector3    m_stRot;
    private Vector3    m_stPos;
    private float      m_stDis;

    private Vector3    m_nxRot;
    private Vector3    m_nxPos;
    private float      m_nxDis;

    private int        m_MoveSpd;
    private int        m_MoveCnt;




/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
    }

    /// 開始
    public override bool DoStart()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        objCam.SetPerspective( ctrlResMgr.GraphDev.DisplayWidth,
                                  ctrlResMgr.GraphDev.DisplayHeight,
                                  37.5f, 1.0f, 50000.0f );

        objCam.SetLookAt( new Vector3( 20.0f, 0.0f, 0.0f ),
                             new Vector3( 0.0f, 1.25f, 0.0f ),
                             6.0f );
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
    }

    /// フレーム処理
    public override bool DoFrame()
    {

        if( m_MoveSpd <= 0 ){
            return false;
        }

        if( m_MoveCnt < m_MoveSpd ){
            m_MoveCnt ++;
        }

        Vector3 isRot, isPos;
        float    isDis;

        isRot.X    = (float)_toCalc(( m_stRot.X + ( (m_nxRot.X * m_MoveCnt) / m_MoveSpd ) ) );
        isRot.Y    = (float)_toCalc(( m_stRot.Y + ( (m_nxRot.Y * m_MoveCnt) / m_MoveSpd ) ) );
        isRot.Z    = (float)_toCalc(( m_stRot.Z + ( (m_nxRot.Z * m_MoveCnt) / m_MoveSpd ) ) );

        isDis      = (float)( m_stDis + ( (m_nxDis * m_MoveCnt) / m_MoveSpd ) );

        isPos.X    = (float)( m_stPos.X + ( (m_nxPos.X * m_MoveCnt) / m_MoveSpd ) );
        isPos.Y    = (float)( m_stPos.Y + ( (m_nxPos.Y * m_MoveCnt) / m_MoveSpd ) );
        isPos.Z    = (float)( m_stPos.Z + ( (m_nxPos.Z * m_MoveCnt) / m_MoveSpd ) );

        objCam.SetLookAt( isRot, isPos, isDis );

        objCam.Frame();

        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCam.Draw( graphDev );
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objCam;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// カメラパラメータのセット
    public void SetMoveParam( Matrix4 trgMtx, Vector3 movePos, Vector3 moveRot, float moveDis, int moveSpd )
    {
        /// 移動カメラ情報
        ///--------------------------------------------
        float cal = (float)Math.Atan2( trgMtx.M31*-1, trgMtx.M33*-1 ) * 180.0f / 3.141593f;

        m_nxRot.X = moveRot.X - objCam.TrgRot.X;
        m_nxRot.Y = cal + moveRot.Y - objCam.TrgRot.Y;
        m_nxRot.Z = moveRot.Z - objCam.TrgRot.Z;

        if( m_nxRot.Y >= 180.0f ){
            m_nxRot.Y = m_nxRot.Y-360.0f;
        }
        if( m_nxRot.Y <= -180.0f ){
            m_nxRot.Y = 360.0f+m_nxRot.Y;
        }


        /// 背後にカメラの装着
        ///--------------------------------------------
        Vector4 a_calPos;
        a_calPos  = trgMtx * (new Vector4(movePos.X, movePos.Y, movePos.Z, 1.0f));
        m_nxPos.X = a_calPos.X - objCam.TrgPos.X;
        m_nxPos.Y = a_calPos.Y - objCam.TrgPos.Y;
        m_nxPos.Z = a_calPos.Z - objCam.TrgPos.Z;

        m_nxDis   = moveDis - objCam.TrgDis;


        /// カメラ移動開始前情報
        ///--------------------------------------------
        m_stRot.X = objCam.TrgRot.X;
        m_stRot.Y = objCam.TrgRot.Y;
        m_stRot.Z = objCam.TrgRot.Z;

        m_stPos.X = objCam.TrgPos.X;
        m_stPos.Y = objCam.TrgPos.Y;
        m_stPos.Z = objCam.TrgPos.Z;

        m_stDis   = objCam.TrgDis;


        /// その他
        ///--------------------------------------------
        m_MoveSpd = moveSpd;
        m_MoveCnt = 0;
    }

        
/// private メソッド
///---------------------------------------------------------------------------

    /// 角度の丸め込み -360.0f < x < 360.0f内に収める
    private float _toCalc( float val )
    {
        if( val >= 360.0f ){
            val = val-360.0f;
        }
        if( val <= -360.0f ){
            val = 360.0f+val;
        }
        return( val );
    }
}

} // namespace
