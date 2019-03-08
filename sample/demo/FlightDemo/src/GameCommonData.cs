/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoModel;
using UnitSys;
using DemoGame;

namespace FlightDemo{

public class GameCommonData
    : UnitCommonData
{
    private static GameCommonData inst;     ///< シングルトンのインスタンス
    private FlightRoute route;
    private TexContainer texContainer;
    private ModelContainer modelContainer;
    private ShaderContainer shaderContainer;


    private Matrix4 proj; ///< 共通で利用するプロジェクションマトリックス
    private Matrix4 view; ///< 共通で利用するビューマトリックス

    /// FlightRoute へのアクセッサ
    public FlightRoute FlightRoute
    {
        get{ return this.route; }
    }

    /// TexContainer へのアクセッサ
    public TexContainer TexContainer{
        get{ return this.texContainer; }
    }

    /// ModelContainer へのアクセッサ
    public ModelContainer ModelContainer{
        get{
            return this.modelContainer;
        }
    }

    /// ShaderContainer へのアクセッサ
    public ShaderContainer ShaderContainer{
        get{
            return this.shaderContainer;
        }
    }

    // SceneTitle / SceneGameMain で共通して利用するためのシングルトン
    public static GameCommonData Inst()
    {
        return inst;
    }

    /// インスタンスの初期化
    public static bool Init()
    {
        GameCommonData.inst = new GameCommonData();
        return true;
    }

    /// インスタンスの解放
    public static void Term()
    {
        inst.Dispose();
        inst = null;
    }

    /// コンストラクタ
    private GameCommonData()
    {
        this.route = new FlightRoute();
        route.Load( "/Application/res/data/plane/pass_plane.mdx");

        texContainer = new TexContainer();
        modelContainer = new ModelContainer();
        shaderContainer = new ShaderContainer();

        // 2D リソース
        TexContainer.SetResPath( "" );

        TexContainer.Load( "2d_2.png", "/Application/res/2d/2d_2.png" );
        TexContainer.Load( "2d_3.png", "/Application/res/2d/2d_3.png" );
        // 3Dリソース
        TexContainer.SetResPath( "/Application/res/data/" );
        modelContainer.SetResPath( "/Application/res/data/" );

        TexContainer.Load( "airship00_00.png", "png/airship00_00.png" );
        TexContainer.Load( "airship00_01.png", "png/airship00_01.png" );

        TexContainer.Load( "RenderMap.png", "png/RenderMap.png" );
        TexContainer.Load( "RenderMap2.png", "png/RenderMap2.png" );
        TexContainer.Load( "RenderMap3.png", "png/RenderMap3.png" );
        TexContainer.Load( "balloon_albedo.png", "png/balloon_albedo.png" );
        TexContainer.Load( "balloon_normalM.png", "png/balloon_normalM.png" );
        TexContainer.Load( "efx_bad.png", "png/efx_bad.png" );
        TexContainer.Load( "efx_good.png", "png/efx_good.png" );
        TexContainer.Load( "enemy00_00.png", "png/enemy00_00.png" );
        TexContainer.Load( "enemy00_01.png", "png/enemy00_01.png" );
        TexContainer.Load( "enemy00_02.png", "png/enemy00_02.png" );
        TexContainer.Load( "plane_00.png", "png/plane_00.png" );
        TexContainer.Load( "plane_01.png", "png/plane_01.png" );
        TexContainer.Load( "plane_02.png", "png/plane_02.png" );
        TexContainer.Load( "plane_enemy.png", "png/plane_enemy.png" );
        TexContainer.Load( "plane_normalM.png", "png/plane_normalM.png" );
        TexContainer.Load( "plane_tex01.png", "png/plane_tex01.png" );
        TexContainer.Load( "plane_tex03.png", "png/plane_tex03.png" );
        TexContainer.Load( "sea.png", "png/sea.png" );
        TexContainer.Load( "sea_alp.png", "png/sea_alp.png" );
        TexContainer.Load( "stg_tex01.png", "png/stg_tex01.png" );
        TexContainer.Load( "stg_tex03.png", "png/stg_tex03.png" );
        TexContainer.Load( "stg_tex05.png", "png/stg_tex05.png" );
        TexContainer.Load( "stg_tex06.png", "png/stg_tex06.png" );
        TexContainer.Load( "stg_tex07.png", "png/stg_tex07.png" );
        TexContainer.Load( "stg_tex08.png", "png/stg_tex08.png" );
        TexContainer.Load( "stg_tex09.png", "png/stg_tex09.png" );
        TexContainer.Load( "stg_tex10.png", "png/stg_tex10.png" );
        TexContainer.Load( "stg_tex11.png", "png/stg_tex11.png" );
        TexContainer.Load( "stg_tex12.png", "png/stg_tex12.png" );
        TexContainer.Load( "stg_tex13.png", "png/stg_tex13.png" );
        TexContainer.Load( "stg_tex14.png", "png/stg_tex14.png" );
        TexContainer.Load( "stg_tex15.png", "png/stg_tex15.png" );
        TexContainer.Load( "stg_tex16.png", "png/stg_tex16.png" );
        TexContainer.Load( "stg_tex17.png", "png/stg_tex17.png" );
        TexContainer.Load( "stg_tex18.png", "png/stg_tex18.png" );
        TexContainer.Load( "time_deduct_00.png", "png/time_deduct_00.png" );
        TexContainer.Load( "time_deduct_01.png", "png/time_deduct_01.png" );
        TexContainer.Load( "time_add_00.png", "png/time_add_00.png" );
        TexContainer.Load( "time_add_01.png", "png/time_add_01.png" );
        TexContainer.Load( "speed_l_00.png", "png/speed_l_00.png" );
        TexContainer.Load( "speed_l_01.png", "png/speed_l_01.png" );
        TexContainer.Load( "speed_up_00.png", "png/speed_up_00.png" );
        TexContainer.Load( "speed_up_01.png", "png/speed_up_01.png" );

 
        ModelContainer.Load( "POINT",    "point/point.mdx" );
        ModelContainer.Load( "GATE",     "gate/gate.mdx" );
        ModelContainer.Load( "PIPE",     "area/area.mdx" );
        ModelContainer.Load( "START-3",  "start/start_3.mdx" );
        ModelContainer.Load( "START-2",  "start/start_2.mdx" );
        ModelContainer.Load( "START-1",  "start/start_1.mdx" );
        ModelContainer.Load( "START-0",  "start/start.mdx" );
        ModelContainer.Load( "BG",       "flt/flt_bg.mdx" );
        ModelContainer.Load( "GOAL",     "goal/goal.mdx" );

        // 自機
        ModelContainer.Load( "PLANE",    "plane/plane00.mdx");
        ModelContainer.Load( "TURN",     "plane/plane01.mdx");
        ModelContainer.Load( "UPDOWN-L", "plane/plane02.mdx");
        ModelContainer.Load( "UPDOWN-R", "plane/plane03.mdx");
        ModelContainer.Load( "PROPERA",   "plane/plane04.mdx");

        // アイテムの演出
        ModelContainer.Load( "EFX-BAD",    "effect/efx_bad.mdx" );
        ModelContainer.Load( "EFX-GOOD-A",    "effect/efx_good_a.mdx" );
        ModelContainer.Load( "EFX-GOOD-B",    "effect/efx_good_b.mdx" );

        // 敵
        ModelContainer.Load( "ENEMY-PLANE","plane_enemy/enemy00.mdx" ); 
        ModelContainer.Load( "AIRSHIP",    "airship/airship00.mdx" );
        ModelContainer.Load( "BALLOON",    "balloon/balloon.mdx" );

        // アイテム
        ModelContainer.Load( "SPEED-UP", "speed_up/speed_up.mdx" );
        ModelContainer.Load( "SPEED-L",  "speed_l/speed_l.mdx" );
        ModelContainer.Load( "TIME-ADD", "time_add/time_add.mdx" );
        ModelContainer.Load( "TIME-DEC", "time_deduct/time_deduct.mdx" );

        ModelContainer.Load( "OPENING", "opening/opening.mdx" );

        ModelContainer.BindTextures( TexContainer );

    }

    /// デストラクタ
    ~GameCommonData()
    {
    }

    private float ms;
    /// 速度表示用
    public void SetMs( float ms )
    {
        this.ms = ms;
    }

    /// 速度表示用
    public float GetMs()
    {
        return this.ms;
    }

    public void Dispose()
    {
        if( texContainer != null ){
            texContainer.Dispose();
            texContainer = null;
        }

        if( modelContainer != null ){
            modelContainer.Dispose();
            modelContainer = null;
        }
        if( shaderContainer != null ){
            shaderContainer.Dispose();
            shaderContainer = null;
        }
    }

    /// プロジェクションマトリックスの設定
    public void SetProjection( Matrix4 proj )
    {
        this.proj = proj;
    }

    /// ビューマトリックスの設定
    public void SetView( Matrix4 view )
    {
        this.view = view;
    }

    /// Projection * View の取得
    public Matrix4 GetViewProj()
    {
        return this.proj * this.view;
    }

    /// View の取得
    public Matrix4 GetView()
    {
        return this.view;
    }

    /// カメラの位置取得
    public Vector4 GetEyePos()
    {
        Matrix4 invView = this.GetView().Inverse();
        Vector4 eye = invView * new Vector4( 0.0f, 0.0f, 0.0f, 1.0f);
        return eye;
    }

}
} // end ns FlightDemo
//===
// EOF
//===
