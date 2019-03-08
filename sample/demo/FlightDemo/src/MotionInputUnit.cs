/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using DemoGame;

namespace FlightDemo{

public class MotionInputUnit
    : FlightUnit
{
    private Vector3 baseAccel;      ///< 基準加速度(法線)

    private Vector3 baseNormal;     ///< 法線ベクトル(基準加速度の正規化)
    private Vector3 baseTangent;    ///< 接ベクトル
    private Vector3 baseBinormal;   ///< 従法線ベクトル
    private Vector3 localN;

    public float inputLeft = 0.0f;
    public float inputRight = 0.0f;
    public float inputUp = 0.0f;
    public float inputDown = 0.0f;


    public Vector3 BaseAccel{ get{ return baseAccel; } }
    public Vector3 BaseNormal{ get{ return baseNormal; } }
    public Vector3 BaseTangent{ get{ return baseTangent; } }
    public Vector3 BaseBinormal{ get{ return baseBinormal; } }
    public Vector3 LocalN{ get{ return localN; } }
    /// コンストラクタ
    public MotionInputUnit()
    //    : base( new MotionInputHandle(), new MotionInputModel() )
    : base( new MotionInputHandle(), null )
    {
    }

    /// デストラクタ
    ~MotionInputUnit()
    {
    }

    protected override bool onStart( FlightUnitManager unitMng )
    {
        SetBaseAccel();
        return true;
    }

    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }

    public void Update()
    {
        var motionData = Motion.GetData(0);
        Vector3 nrm = motionData.Acceleration.Normalize() * -1.0f;

        //        Matrix4 mtx = new Matrix4();

        //        mtx.ColumnX = new Vector4( baseTangent.X, baseTangent.Y, baseTangent.Z, 0.0f );
        //        mtx.ColumnY = new Vector4( baseNormal.X, baseNormal.Y, baseNormal.Z, 0.0f );
        //        mtx.ColumnZ = new Vector4( baseBinormal.X, baseBinormal.Y, baseBinormal.Z, 0.0f );

        //        mtx = mtx.Transpose();

        //        Vector4 ln = mtx * (new Vector4( nrm.X, nrm.Y, nrm.Z, 0.0f ));
        //        localN.X = ln.X;
        //        localN.Y = ln.Y;
        //        localN.Z = ln.Z;
        localN.X = nrm.Dot( baseTangent );
        localN.Y = nrm.Dot( baseBinormal );
        localN.Z = 0.0f;

        inputLeft = 0.0f;
        inputRight = 0.0f;
        inputUp = 0.0f;
        inputDown = 0.0f;

        const float kMIN_X = 5.0f;
        const float kMAX_X = 30.0f;
        const float kMIN_Y = 3.0f;
        const float kMAX_Y = 20.0f;

        if( localN.X > 0 ){
            inputLeft = 90.0f - DemoUtil.Rad2Deg( (float)Math.Acos( localN.X ) );
            inputLeft = DemoUtil.Clamp( inputLeft - kMIN_X, kMAX_X, 0.0f );
            inputLeft /= kMAX_X;
        }
        if( localN.X < 0 ){
            inputRight = 90.0f - DemoUtil.Rad2Deg( (float)Math.Acos( -localN.X ) );
            inputRight = DemoUtil.Clamp( inputRight - kMIN_X, kMAX_X, 0.0f );
            inputRight /= kMAX_X;
        }

        if( localN.Y > 0 ){
            inputUp = 90.0f - DemoUtil.Rad2Deg( (float)Math.Acos( localN.Y ) );
            inputUp = DemoUtil.Clamp( inputUp - kMIN_Y, kMAX_Y, 0.0f );
            inputUp /= kMAX_Y;
        }
        if( localN.Y < 0 ){
            inputDown = 90.0f - DemoUtil.Rad2Deg( (float)Math.Acos( -localN.Y ) );
            inputDown = DemoUtil.Clamp( inputDown - kMIN_Y, kMAX_Y, 0.0f );
            inputDown /= kMAX_Y;
        }
            
    }

    public void SetBaseAccel()
    {
        var motionData = Motion.GetData(0);
        baseAccel = motionData.Acceleration;

        baseNormal = baseAccel.Normalize() * -1.0f; // ディスプレイ方向にする

        // <<< 注意 >>> 左右方向は、特異点となっているので方向が定まらない
        Vector3 tmpRight = new Vector3( 1.0f, 0.0f, 0.0f ); // ワールドの右

        baseBinormal = tmpRight.Cross( baseNormal );
        baseTangent = baseNormal.Cross( baseBinormal );

    }
}

} // end ns FlightDemo
//===
// EOF
//===
