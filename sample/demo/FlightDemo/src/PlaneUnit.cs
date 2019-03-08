/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using DemoGame;

namespace FlightDemo{

public enum PlaneState{
    Sleep,
    Normal,
    Uncontrollable,
}

public class PlaneUnit
    : FlightUnit
{
    // 最低速度        60km/h
    // 通常時最高速度   220km/h
    // 最高速度        260km/h

    private const float kMIN_UISPEED   =  60.0f;
    //    private const float kMIN_UISPEED   =   0.0f;
    private const float kSTART_UISPEED = 100.0f;
    private const float kHIGH_UISPEED  = 220.0f;
    private const float kMAX_UISPEED   = 260.0f;
    private const float kEX_UIACCELL   =  50.0f;

    private const float kMAX_SPEED     = 21.0f;

    private const float kMIN_SPEED     = kMAX_SPEED * (kMIN_UISPEED  / kMAX_UISPEED);
    private const float kSTART_SPEED   = kMAX_SPEED * (kSTART_UISPEED  / kMAX_UISPEED);
    private const float kHIGH_SPEED    = kMAX_SPEED * (kHIGH_UISPEED / kMAX_UISPEED);
    private const float kEX_ACCELL     = kMAX_SPEED * (kEX_UIACCELL / kMAX_UISPEED);


    private const float kACCELERATION  = 4.0f;
    private const float kYAW_ACCEL     = 22.5f * 1.5f * 0.65f;
    private const float kPITCH_ACCEL   = 22.5f * 1.5f * 0.65f;
    private const float kROLL_ACCEL    = 22.5f;
    private const float kROUTE_DELTA = 1.0f;

	private const float kENGINE_VOLUME = 0.4f;

    private float exAccell = 0.0f;
    private float exAccellTime = 0.0f;

    private PlaneState state;
    private float speed = 0.0f;
    private float routeTime = 0.0f;
    private float prevSpeed = 0.0f;

    // パラメータ
    private float rudder    = 0.0f;
    private float aileronL  = 0.0f;
    private float aileronR = 0.0f;
    private float uncontrollableTime = 0.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;
    private GeometrySphere collision;

    /// あたり判定の取得
    public GeometrySphere GetCollision()
    {
        return collision;
    }

    public PlaneState State
    {
        get{ return state; }
    }

    public float Rudder
    {
        get{ return rudder; }
    }

    public float AileronL
    {
        get{ return aileronL; }
    }

    public float AileronR
    {
        get{ return aileronR; }
    }

    private void setState( PlaneState state )
    {
        this.state = state;
    }

    /// コンストラクタ
    public PlaneUnit( FlightUnitHandle handle, FlightUnitModel model )
    : base( handle, model )
    {
    }

    /// デストラクタ
    ~PlaneUnit()
    {
    }

    Matrix4 ComputeYPR( float yaw, float pitch, float roll )
    {
        Matrix4 yawM = Matrix4.RotationY( yaw );
        Matrix4 pitchM = Matrix4.RotationX( pitch );
        Matrix4 rollM = Matrix4.RotationZ( roll );
        return rollM * pitchM * yawM;
    }

    /// UnitManager に登録されたときに呼び出されるハンドラ
    FlightUnitManager parentUnitMng;
    protected override bool onStart( FlightUnitManager unitMng )
    {
        routeTime = 0.0f;
        // スタート位置の姿勢
        yaw = 0.0f;
        pitch = 0.0f;
        roll = 0.0f;
			
		this.prevSpeed = kSTART_SPEED;
        this.speed = kSTART_SPEED;
        Matrix4 basePosture = unitMng.GameCommonData.FlightRoute.BasePosture( this.GetRouteTime() );
        this.SetPosture( basePosture );
        this.setState( PlaneState.Sleep ); // timer がスタートを管理してくれる
			
		// 初期エンジン音の再生
		if( speed >= 0.67 ) AudioManager.PlaySound( "EngineH", true, kENGINE_VOLUME );
		else if( speed >= 0.33 ) AudioManager.PlaySound( "EngineM", true, kENGINE_VOLUME );
		else AudioManager.PlaySound( "EngineL", true, kENGINE_VOLUME );

        collision = new GeometrySphere( new Vector3( 0.0f, 0.0f, 0.0f ), 0.08f );

        this.parentUnitMng = unitMng;

        return true;
    }

    /// UnitManager の登録から削除されたときに呼び出されるハンドラ
    protected override bool onEnd( FlightUnitManager unitMng )
    {
        return true;
    }


    /// ルート上の時間
    public float GetRouteTime()
    {
        return routeTime;
    }

    /// 左右に舵を取る
    public void Yaw( float pow, float delta )
    {
        if( this.State == PlaneState.Normal ){
            rudder = DemoUtil.Clamp( rudder + (1.0f * pow * delta), 1.0f, -1.0f );
            yaw += DemoUtil.Deg2Rad( kYAW_ACCEL ) * (1.0f * pow * delta);
        }
    }

    /// 機首を上下にする
    public void Pitch( float pow, float delta )
    {
        if( this.State == PlaneState.Normal ){
            aileronR = DemoUtil.Clamp( aileronR + (1.0f * pow * delta), 1.0f, -1.0f );
            aileronL = DemoUtil.Clamp( aileronL + (1.0f * pow * delta), 1.0f, -1.0f );
            pitch += DemoUtil.Deg2Rad( kPITCH_ACCEL ) * (1.0f * pow * delta);
        }
    }

    /// 左右にロールする
    public void Roll( float delta )
    {
        if( this.State == PlaneState.Normal ){
            aileronR = DemoUtil.Clamp( aileronR + delta, 1.0f, -1.0f );
            aileronL = DemoUtil.Clamp( aileronL - delta, 1.0f, -1.0f );
            roll += DemoUtil.Deg2Rad( kROLL_ACCEL ) * delta;
        }
    }

    /// スピードを上げる
    public void AddSpeed( float delta )
    {
        if( this.State == PlaneState.Normal ){
            if( speed < kHIGH_SPEED ){
                speed = DemoUtil.Clamp( speed + kACCELERATION * delta, kHIGH_SPEED, kMIN_SPEED );
            }
        }
    }

    /// 良いアイテムを取った時のエフェクト再生
    public void EfxGoodA()
    {
        PlaneModel myModel = this.model as PlaneModel;
        myModel.EfxGoodA();
    }

    /// 良いアイテムを取った時のエフェクト再生
    public void EfxGoodB()
    {
        PlaneModel myModel = this.model as PlaneModel;
        myModel.EfxGoodB();
    }

    /// 悪いアイテムを取った時のエフェクト再生
    public void EfxBad()
    {
        PlaneModel myModel = this.model as PlaneModel;
        myModel.EfxBad();
    }

    /// (アイテム効果によって)スピードを上げる
    public void SpeedUp()
    {
        exAccell = kEX_ACCELL;
        exAccellTime = 1.0f;
        AudioManager.PlaySound( "SpeedUp" );
        EfxGoodB();
    }

    /// (アイテム効果によって)スピードを下げる
    public void SpeedDown()
    {
        exAccell = -kEX_ACCELL;
        exAccellTime = 1.0f;
        AudioManager.PlaySound( "SpeedDown" );
        EfxBad();
    }

    private float lerp( float rudder, float delta )
    {
        if( rudder > 0.0f ){
            rudder -= delta * 0.2f;
        }else if( rudder < 0.0f ){
            rudder += delta * 0.2f;
        }

        DemoUtil.Clamp( rudder, 1.0f, -1.0f );
        return rudder;
    }

    /// 飛行機の向いている方向
    private Vector3 Direction( Matrix4 mtx )
    {
        // モデルが逆を向いているので-1する
        return new Vector3( mtx.M31,
                            mtx.M32,
                            mtx.M33 ) * -1.0f;
    }

    /// 行列のZ軸を合わせる
    private Matrix4 look( Matrix4 org, Vector3 lookDir )
    {
        Matrix4 posture = org;

        lookDir = lookDir.Normalize();
        Vector3 look = -lookDir;
        Vector3 up = posture.AxisY;
        Vector3 right = up.Cross( look );
        up = look.Cross( right );

        posture.AxisX = right.Normalize();
        posture.AxisY = up.Normalize();
        posture.AxisZ = look.Normalize();

        return posture;
    }

    /// 
    public void Move( FlightRoute route, float delta )
    {
        if( this.State != PlaneState.Sleep ){

            if( exAccellTime >= 0 ){
                // アイテム加速
                speed = DemoUtil.Clamp( speed + exAccell * delta, kMAX_SPEED, kMIN_SPEED );
                exAccellTime -= delta;
            }else{
                // 減速
                if( speed > kMIN_SPEED ){
                    speed -= (speed * 0.1f) * delta;
                }
            }

            if( uncontrollableTime > 0.0f ){
                uncontrollableTime -= delta;
                if( uncontrollableTime <= 0.0f ){
                    uncontrollableTime = 0.0f;
                    this.setState( PlaneState.Normal );
                }
            }
        }

        moveNormal( route, delta );

		// エンジン音の切り替え
		var currentSpeed = Speed();
		// 今は低回転
		if( prevSpeed < 0.33f ){
			// L→Hに切り替え
			if( currentSpeed >= 0.67 ){
				AudioManager.StopSound( "EngineL" );
				AudioManager.PlaySound( "EngineH", true, kENGINE_VOLUME );
			}
			// L→Mに切り替え
			else if( currentSpeed >= 0.33 )
			{
				AudioManager.StopSound( "EngineL" );
				AudioManager.PlaySound( "EngineM", true, kENGINE_VOLUME );
			}
		}
		// 今は中回転
		else if( prevSpeed < 0.67 ){
			// M→Lに切り替え
			if( currentSpeed < 0.33 ){
				AudioManager.StopSound( "EngineM" );
				AudioManager.PlaySound( "EngineL", true, kENGINE_VOLUME );
			}
			// M→Hに切り替え
			else if( currentSpeed >= 0.67 )
			{
				AudioManager.StopSound( "EngineM" );
				AudioManager.PlaySound( "EngineH", true, kENGINE_VOLUME );
			}
		}
		// 今は高回転
		else{
			// H→Lに切り替え
			if( currentSpeed < 0.33 ){
				AudioManager.StopSound( "EngineH" );
				AudioManager.PlaySound( "EngineL", true, kENGINE_VOLUME );
			}
			// H→Mに切り替え
			else if( currentSpeed < 0.67 )
			{
				AudioManager.StopSound( "EngineH" );
				AudioManager.PlaySound( "EngineM", true, kENGINE_VOLUME );
			}
		}
		prevSpeed = currentSpeed;

        // ypr は、累積せずに初期化する
        yaw = 0.0f;
        pitch = 0.0f;
        roll = 0.0f;

    }

    /// 通常移動
    private void moveNormal( FlightRoute route, float delta )
    {
        Matrix4 nextPosture = this.GetPosture() * ComputeYPR( yaw, pitch, roll );

        // コースの進行方向ベクトルを作る
        Vector3 routeDir = route.Direction( this.GetRouteTime(), kROUTE_DELTA );
        float routeLen = routeDir.Length();
        routeDir.X /= routeLen;
        routeDir.Y /= routeLen;
        routeDir.Z /= routeLen;


        // 自分の現在の姿勢から、進もうとしている方向のベクトルを作る
        Vector3 planeDir = this.Direction( nextPosture );

        if( this.State == PlaneState.Uncontrollable || this.State == PlaneState.Sleep ){
            Vector3 pos = new Vector3( nextPosture.M41,
                                       nextPosture.M42,
                                       nextPosture.M43 );
            Vector3 nextPos = route.BasePos( this.GetRouteTime() + 0.5f );
            Vector3 nextDir = nextPos - pos;

            // 中心に向ける
            planeDir = planeDir.Lerp( nextDir, delta / 2.0f ).Normalize();
            nextPosture = this.look( nextPosture, planeDir );
        }

        // 自分の向いている方向に対する移動距離
        float dist = (speed * delta);

        // ルート上の進行方向に向けた時間を取得する
        float dt = 0.0f;
        {
            float dot = routeDir.Dot( planeDir );
            float routeDist = dot * dist;// ルート上の進行距離
            dt = routeDist / (routeLen / kROUTE_DELTA); // このdtをそのまま計算に使ってしまうと計算誤差がたまってしまう
        }

        // 移動
        if( dt > 0.0001f ){
            // 平面上での平行移動を求める
            // dt の計算誤差を解消するのは手間がかかるので、dtを正しいものとしてしまう
            // routetime + dt の平面に拘束してしまう
            Matrix4 nextBase = route.BasePosture( this.GetRouteTime() + dt );

            // 平面と移動線の交点を求める(= 平面上の位置)
            Vector3 baseDZ = new Vector3( nextBase.M31, nextBase.M32, nextBase.M33 );
            Vector3 basePos = new Vector3( nextBase.M41, nextBase.M42, nextBase.M43 );
            GeometryPlane basePlane = new GeometryPlane( baseDZ, basePos );

            Vector3 pos = new Vector3( nextPosture.M41,
                                       nextPosture.M42,
                                       nextPosture.M43 );


            Vector3 pt = new Vector3();

            if( DemoUtil.IntersectRayPlane( ref pt, basePlane, pos, planeDir ) ){
                nextPosture.M41 = pt.X;
                nextPosture.M42 = pt.Y;
                nextPosture.M43 = pt.Z;
            }
        }

        // 中心に向かって拘束する
        {
            Vector3 pos = new Vector3( nextPosture.M41, nextPosture.M42, nextPosture.M43 );
            Vector3 centerPos = route.BasePos( this.GetRouteTime() + dt );

            // 中心から離れている方向
            Vector3 centerDir = pos - centerPos;

            float len = centerDir.Length();
            centerDir = centerDir.Normalize();

            // ルート外に出てしまっている
            if( len > FlightRoute.kROUTE_RADIUS ){
                if( this.State != PlaneState.Uncontrollable && this.State != PlaneState.Sleep ){
                    uncontrollableTime = 2.0f;
                    this.setState( PlaneState.Uncontrollable );
                    AudioManager.PlaySound( "Boundary" );
                    this.parentUnitMng.Regist( "Eff", -1, new UncontrollableEffect() );

                }

                pos = centerPos + centerDir * FlightRoute.kROUTE_RADIUS;
                nextPosture.M41 = pos.X;
                nextPosture.M42 = pos.Y;
                nextPosture.M43 = pos.Z;
            }

        }

        // 時間/姿勢のアップデート
        routeTime += dt;

        // 正規化
        nextPosture = this.look( nextPosture, planeDir );
        this.SetPosture( nextPosture );

        rudder = lerp( rudder, delta );
        aileronL = lerp( aileronL, delta );
        aileronR = lerp( aileronR, delta );
    }
    /// 現在の速度
    public float Speed()
    {
        return speed / kMAX_SPEED;
    }

    /// スリープ状態の機体をアクティブにする
    public void Active()
    {
        if( this.state == PlaneState.Sleep ){
            this.state = PlaneState.Normal;
        }
    }

    /// 強制的に機体をスリープにする
    public void Sleep()
    {
        this.state = PlaneState.Sleep;
    }

    /// 現在の高度
    public float HighDegree()
    {
        return this.GetPosture().M42 / 15.0f;
    }

    /// 敵とのあたり判定が起きた
    public void ForceUncontrollable()
    {
        if( this.State != PlaneState.Uncontrollable && this.State != PlaneState.Sleep ){
            uncontrollableTime = 2.0f;
            this.setState( PlaneState.Uncontrollable );
            AudioManager.PlaySound( "Crash" );
            this.parentUnitMng.Regist( "Eff", -1, new UncontrollableEffect() );
        }
    }

}

} // end ns FlightDemo
//===
// EOF
//===
