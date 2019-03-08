/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using UnitSys;
using Sce.PlayStation.Core;
using DemoModel;

namespace FlightDemo{


/// 環境のライティングを行うためのユニット
/**
 * PlaneUnit のルート上の時間を参照しながら、ライティングを決定する
 */
public class EnvLightingUnit
    : Unit
{
    class LightData
    {
        public float StartTime;
        public float EndTime;
        public Light Light = new Light();
        
        public LightData( float start, float end )
        {
            this.StartTime = start;
            this.EndTime = end;
        }
    }

    private ModelContainer modelContainer;
    private LightData[] lights;


    /// コンストラクタ
    public EnvLightingUnit()
    : base( new EnvLightingHandle(), null )
    {
    }

    /// デストラクタ
    ~EnvLightingUnit()
    {
    }

    public override bool OnStart( UnitManager unitMng )
    {
        GameCommonData commonData = unitMng.CommonData as GameCommonData;
        FlightRoute route = commonData.FlightRoute;
        modelContainer = commonData.ModelContainer;

        lights = new LightData[3];

        lights[ 1 ] = new LightData( 51.0f, 52.0f );
        lights[ 1 ].Light.Position  = new Vector4( 0.0f, 120.0f, 0.0f, 1.0f );
        lights[ 1 ].Light.KDiffuse  = new Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
        lights[ 1 ].Light.KSpecular = new Vector4( 0.5f, 0.5f, 0.5f, 1.0f );

        lights[ 2 ] = new LightData( 54.0f, 56.0f );
        lights[ 2 ].Light.Position  = new Vector4( 0.0f, 120.0f, 0.0f, 1.0f );
        lights[ 2 ].Light.KDiffuse  = new Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
        lights[ 2 ].Light.KSpecular = new Vector4( 0.5f, 0.5f, 0.5f, 1.0f );

        // デフォルト
        lights[ 0 ] = new LightData( 0.0f, route.Length() );
        lights[ 0 ].Light.Position  = new Vector4( 0.0f, 120.0f, 0.0f, 1.0f );
        lights[ 0 ].Light.KDiffuse  = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        lights[ 0 ].Light.KSpecular = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );


        return true;
    }

    public override bool OnEnd( UnitManager unitMng )
    {
        // 所有権なし
        this.modelContainer = null;

        return true;
    }

    public void Lighting( float time )
    {
        modelContainer.SetLightCount( 1 );
        modelContainer.SetLight( 0, lights[ 0 ].Light );

        for( int i = 1; i < lights.Length; i++ ){
            if( lights[ i ].StartTime <= time && time <= lights[ i ].EndTime ){
                float len = lights[ i ].EndTime - lights[ i ].StartTime;
                float center = lights[ i ].StartTime + len / 2.0f;

                float ratio = (float)Math.Abs( center - time ) / (len / 2.0f);
                Light light = new Light();
                light.Position = lights[ 0 ].Light.Position;
                light.KDiffuse  = lights[ 0 ].Light.KDiffuse.Lerp( lights[ i ].Light.KDiffuse, ratio );
                light.KSpecular = lights[ 0 ].Light.KSpecular.Lerp( lights[ i ].Light.KSpecular, ratio );
                
                modelContainer.SetLight( 0, light );
                break;
            }
        }
    }


}

} // end ns FlightDemo
//===
// EOF
//===
