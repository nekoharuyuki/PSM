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
/// OBJ:カメラ
///***************************************************************************
public class ObjCamera : GameObjProduct
{
    DemoGame.Camera    camCore;
    private Vector3    trgPos;
    private Vector3    trgRot;
    private float      trgDis;

    private int        dspWidth;
    private int        dspHeight;
    private float      camAngle;
    private float      camNear;
    private float      camFar;


/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        camCore = new DemoGame.Camera();
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        camCore = null;
    }

    /// 開始
    public override bool DoStart()
    {
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
    }

    /// フレーム処理
    public override bool DoFrame()
    {
        camCore.SetPerspective( dspWidth, dspHeight, camAngle, camNear, camFar );
        camCore.SetLookAt( trgRot, trgPos, trgDis );
        return true;
    }

    /// 描画処理
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        return true;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 射影行列のパラメータ登録
    public void SetPerspective( int dspWidth, int dspHeight, float angle, float near, float far )
    {
        this.dspWidth   = dspWidth;
        this.dspHeight  = dspHeight;
        this.camAngle   = angle;
        this.camNear    = near;
        this.camFar     = far;
            
        camCore.SetPerspective( dspWidth, dspHeight, camAngle, camNear, camFar );
    }

    /// Look Atのパラメータセット
    public void SetLookAt( Vector3 trgRot, Vector3 trgPos, float trgDis )
    {
        this.trgPos = trgPos;
        this.trgRot = trgRot;
        this.trgDis = trgDis;
    }

    /// ターゲットとの距離をたす
    public void AddTrgDis( float dis )
    {
        trgDis += dis;
        if( trgRot.X <= 0.0f ){
            trgDis = 0.0001f;
        }
    }

    /// ターゲットとの角度をたす
    public void AddTrgRotX( float rot )
    {
        trgRot.X += rot;
        if( trgRot.X > 360.0f ){
            trgRot.X -= 360.0f;
        }
        else if( trgRot.X < -360.0f ){
            trgRot.X += 360.0f;
        }
    }
    public void AddTrgRotY( float rot )
    {
        trgRot.Y += rot;
        if( trgRot.Y > 360.0f ){
            trgRot.Y -= 360.0f;
        }
        else if( trgRot.Y < -360.0f ){
            trgRot.Y += 360.0f;
        }
    }
    public void AddTrgRotZ( float rot )
    {
        trgRot.Z += rot;
        if( trgRot.Z > 360.0f ){
            trgRot.Z -= 360.0f;
        }
        else if( trgRot.Z < -360.0f ){
            trgRot.Z += 360.0f;
        }
    }


/// プロパティ
///---------------------------------------------------------------------------

    /// カメラ座標
    public DemoGame.Camera CameraCore
    {
        get {return camCore;}
    }

    /// カメラ座標
    public Vector3 CamPos
    {
        get {return camCore.Pos;}
    }

    /// 視野角
    public float CamAngle
    {
        set {camAngle = value;}
        get {return camAngle;}
    }

    /// 対象の座標
    public Vector3 TrgPos
    {
        set {trgPos = value;}
        get {return trgPos;}
    }

    /// 対象との角度
    public Vector3 TrgRot
    {
        set {trgRot = value;}
        get {return trgRot;}
    }

    /// 対象との距離
    public float TrgDis
    {
        set {trgDis = value;}
        get {return trgDis;}
    }

}

} // namespace
