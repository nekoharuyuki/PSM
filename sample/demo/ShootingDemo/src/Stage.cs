/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using DemoGame;
using DemoModel;

namespace ShootingDemo
{

/**
 * Stageクラス
 */
public class Stage : Mono
{
    /// Z値
    public override float ZParam {
        get {return 10000;}
    }

    private delegate void Action0();

    private class BgAction
    {
        public long timeMillis;
        public Action0 action;

        public BgAction(float timeMills, Action0 action)
        {
            this.timeMillis = (long)(timeMills * 1000);
            this.action = action;
        }
    }

    private ModelContainer modelContainer;
    private TexContainer texContainer;
    private List<BgAction> bgList;
    private int bgIndex;
    private Light light = new Light();
    private LightStatus lightStatus = 0;
    private float lightIntensity = 1f;
    private Matrix4 projectionMatrix;
    private Matrix4 viewMatrix;
    private Matrix4 viewProjectionMatrix;
    private Vector4 eye;
    private long animTimeMillis;

	private String shaderName = "BGShader";
	public String ShaderName
	{
		set {shaderName = value;}
	}

	/// ライトステータス
    private enum LightStatus
    {
        Default = 0,
        DefToLight,
        LightToDef
    }


    /// コンストラクタ
    public Stage(string name = null) : base(name)
    {
    }

    /// デストラクタ
    ~Stage()
    {
    }

    /// 開始処理
    public override bool Start(MonoManager monoManager)
    {
        int width = Renderer.DisplayWidth;
        int height = Renderer.DisplayHeight;
        float angle = 48.6f;
        float near = 100.0f;
        float far = 250000.0f;

        projectionMatrix = Matrix4.Perspective(FMath.Radians(angle),
                                               (float)width / height,
                                               near,
                                               far);

        modelContainer = new ModelContainer();
        modelContainer.SetResPath("/Application/res/data/");
        modelContainer.Load("_6DEG", "sht_bg/deg_6.mdx");
        modelContainer.Load("_12DEG", "sht_bg/deg_12.mdx");
        modelContainer.Load("_12DEG_B", "sht_bg/deg_12_b.mdx");
        modelContainer.Load("_20DEG", "sht_bg/deg_20.mdx");
        modelContainer.Load("_60DEG", "sht_bg/deg_60.mdx");
        modelContainer.Load("_90DEG", "sht_bg/deg_90.mdx");
        modelContainer.Load("_1RING", "sht_bg/ring_1.mdx");
        modelContainer.Load("_2RING", "sht_bg/ring_2.mdx");
        modelContainer.Load("SPHERE", "sht_bg/sht_sphere.mdx");
        modelContainer.Load("CAMERA", "sht_bg/camera.mdx");

        texContainer = new TexContainer();
        texContainer.SetResPath("/Application/res/data/");
        texContainer.Load("deg_10_albedo.png", "sht_bg/deg_10_albedo.png");
        texContainer.Load("deg_10_normalM.png", "sht_bg/deg_10_normalM.png");
        texContainer.Load("deg_1_albedo.png", "sht_bg/deg_1_albedo.png");
        texContainer.Load("deg_1_normalM.png", "sht_bg/deg_1_normalM.png");
        texContainer.Load("deg1_4_albedo.png", "sht_bg/deg1_4_albedo.png");
        texContainer.Load("deg1_4_normalM.png", "sht_bg/deg1_4_normalM.png");
        texContainer.Load("deg3_4_albedo.png", "sht_bg/deg3_4_albedo.png");
        texContainer.Load("deg3_4_normalM.png", "sht_bg/deg3_4_normalM.png");
        texContainer.Load("deg2_4_albedo.png", "sht_bg/deg2_4_albedo.png");
        texContainer.Load("deg2_4_normalM.png", "sht_bg/deg2_4_normalM.png");
        texContainer.Load("deg_12_a_albedo.png", "sht_bg/deg_12_a_albedo.png");
        texContainer.Load("deg_12_a_normalM.png", "sht_bg/deg_12_a_normalM.png");
        texContainer.Load("deg_12_b_albedo.png", "sht_bg/deg_12_b_albedo.png");
        texContainer.Load("deg_12_b_normalM.png", "sht_bg/deg_12_b_normalM.png");
        texContainer.Load("deg_20_2_albedo.png", "sht_bg/deg_20_2_albedo.png");
        texContainer.Load("deg_20_2_normalM.png", "sht_bg/deg_20_2_normalM.png");
        texContainer.Load("deg_60_albedo.png", "sht_bg/deg_60_albedo.png");
        texContainer.Load("deg_60_normalM.png", "sht_bg/deg_60_normalM.png");
        texContainer.Load("deg_6_albedo.png", "sht_bg/deg_6_albedo.png");
        texContainer.Load("deg_6_normalM.png", "sht_bg/deg_6_normalM.png");
        texContainer.Load("deg_90_albedo.png", "sht_bg/deg_90_albedo.png");
        texContainer.Load("deg_90_normalM.png", "sht_bg/deg_90_normalM.png");
        texContainer.Load("sht_sphere.png", "sht_bg/sht_sphere.png");

        var shaderContainer = ShootingData.ShaderContainer;
        shaderContainer.Load("BGShader", "/Application/shaders/cbasic/W1C0L1FVA0T2.vp.cgx", "/Application/shaders/cbasic/C0L1FVT2V0RI0MN0NA0.fp.cgx");
        shaderContainer.Load("BGTexShader", "/Application/shaders/cbasic/W1C0L1FVA0T2.vp.cgx", "/Application/shaders/cbasic/SHOOTINGTEX.fp.cgx");
        modelContainer.BindTextures(texContainer);

        // 描画リストの生成
        bgList = new List<BgAction> {
            new BgAction(  2.7639f, () => {sector_0a();    sector_0aa();}),
            new BgAction( 4.49955f, () => {sector_0aa();   sector_0aaa();}),
            new BgAction(   6.666f, () => {sector_0aaa();  sector_0aaaa();}),
            new BgAction(  7.6659f, () => {sector_0aaaa();}),
            new BgAction(  9.3324f, () => {sector_0aaaa(); sector_0b();}),
            new BgAction( 11.6655f, () => {sector_0b();    sector_0bb();}),
            new BgAction(13.43199f, () => {sector_0bb();   sector_0bbb();}),
            new BgAction( 14.3319f, () => {sector_0bbb();}),
            new BgAction( 15.6651f, () => {sector_0bbb();  sector_0bbbb();}),
            new BgAction(  16.665f, () => {sector_0bbbb();}),
            new BgAction( 18.3315f, () => {sector_0bbbb(); sector_0c();}),
            new BgAction(19.16475f, () => {sector_0c();    sector_0cc();}),
            new BgAction( 20.6646f, () => {sector_0cc();}),
            new BgAction( 21.6645f, () => {sector_0cc();   sector_0ccc();}),
            new BgAction(22.89771f, () => {sector_0ccc();}),
            new BgAction(24.23091f, () => {sector_0ccc();  sector_0cccc();}),
            new BgAction(25.83075f, () => {sector_0cccc();}),
            new BgAction(27.83055f, () => {sector_0cccc(); sector_0d();}),
            new BgAction( 28.3305f, () => {sector_0d();}),
            new BgAction(29.86368f, () => {sector_0d();    sector_0dd();}),
            new BgAction( 30.9969f, () => {sector_0dd();}),
            new BgAction( 32.6634f, () => {sector_0dd();   sector_0ddd();}),
            new BgAction(34.49655f, () => {sector_0ddd();}),
            new BgAction( 36.3297f, () => {sector_0ddd();  sector_0e();}),
            new BgAction(38.26284f, () => {sector_0e();    sector_0ee();}),
            new BgAction(38.82945f, () => {sector_0ee();}),
            new BgAction(40.82925f, () => {sector_0ee();   sector_0eee();}),
            new BgAction( 42.9957f, () => {sector_0eee();  sector_0eeee();}),
            new BgAction( 44.3289f, () => {sector_0eeee();}),
            new BgAction( 45.9954f, () => {sector_0eeee(); sector_1a();}),
            new BgAction( 47.9952f, () => {sector_1a();    sector_1aa();}),
            new BgAction( 48.3285f, () => {sector_1aa();}),
            new BgAction( 50.3283f, () => {sector_1aa();   sector_1aaa();}),
            new BgAction(51.39486f, () => {sector_1aaa();  sector_1aaaa();}),
            new BgAction(52.49475f, () => {sector_1aaaa();}),
            new BgAction( 54.3279f, () => {sector_1aaaa(); sector_1b();}),
            new BgAction( 56.3277f, () => {sector_1b();    sector_1bb();}),
            new BgAction(59.16075f, () => {sector_1bb();   sector_1bbb();}),
            new BgAction( 60.6606f, () => {sector_1bbb();  sector_1bbbb();}),
            new BgAction( 61.6605f, () => {sector_1bbbb();}),
            new BgAction( 62.9937f, () => {sector_1bbbb(); sector_1c();}),
            new BgAction(63.89361f, () => {sector_1c();    sector_1cc();}),
            new BgAction(65.49345f, () => {sector_1cc();}),
            new BgAction( 66.3267f, () => {sector_1cc();   sector_1ccc();}),
            new BgAction(67.52658f, () => {sector_1ccc();}),
            new BgAction(68.82645f, () => {sector_1ccc();  sector_1cccc();}),
            new BgAction( 71.9928f, () => {sector_1cccc(); sector_1d();}),
            new BgAction( 74.6592f, () => {sector_1d();    sector_1dd();}),
            new BgAction(77.15895f, () => {sector_1dd();   sector_1ddd();}),
            new BgAction(79.49205f, () => {sector_1ddd();}),
            new BgAction(81.39186f, () => {sector_1ddd();  sector_1e();}),
            new BgAction(  83.325f, () => {sector_1e();    sector_1ee();}),
            new BgAction( 85.3248f, () => {sector_1ee();   sector_1eee();}),
            new BgAction(87.49125f, () => {sector_1eee();  sector_1eeee();}),
            new BgAction(88.85778f, () => {sector_1eeee();}),
            new BgAction(90.49095f, () => {sector_1eeee(); sector_2a();}),
            new BgAction( 92.9907f, () => {sector_2a();    sector_2aa();}),
            new BgAction( 94.3239f, () => {sector_2aa();   sector_2aaa();}),
            new BgAction( 95.9904f, () => {sector_2aaa();  sector_2aaaa();}),
            new BgAction( 98.6568f, () => {sector_2aaaa(); sector_2b();}),
            new BgAction(100.8233f, () => {sector_2b();    sector_2bb();}),
            new BgAction( 103.323f, () => {sector_2bb();   sector_2bbb();}),
            new BgAction(105.9894f, () => {sector_2bbb();  sector_2bbbb();}),
            new BgAction(107.8226f, () => {sector_2bbbb(); sector_2c();}),
            new BgAction(108.9891f, () => {sector_2c();    sector_2cc();}),
            new BgAction( 109.989f, () => {sector_2cc();}),
            new BgAction(111.1556f, () => {sector_2cc();   sector_2ccc();}),
            new BgAction(112.6554f, () => {sector_2ccc();}),
            new BgAction(113.4887f, () => {sector_2ccc();  sector_2cccc();}),
            new BgAction(114.6552f, () => {sector_2cccc();}),
            new BgAction( 117.055f, () => {sector_2cccc(); sector_2d();}),
            new BgAction(117.6549f, () => {sector_2d();}),
            new BgAction(119.3881f, () => {sector_2d();    sector_2dd();}),
            new BgAction(120.0547f, () => {sector_2dd();}),
            new BgAction(122.3211f, () => {sector_2dd();   sector_2ddd();}),
            new BgAction(124.2209f, () => {sector_2ddd();}),
            new BgAction(125.6541f, () => {sector_2ddd();  sector_2e();}),
            new BgAction(128.3205f, () => {sector_2e();    sector_2ee();}),
            new BgAction(130.3203f, () => {sector_2ee();   sector_2eee();}),
            new BgAction(132.3201f, () => {sector_2eee();  sector_2eeee();}),
            new BgAction(134.6532f, () => {sector_2eeee(); sector_3a();}),
            new BgAction(136.9863f, () => {sector_3a();    sector_3aa();}),
            new BgAction(138.6528f, () => {sector_3aa();   sector_3aaa();}),
            new BgAction(140.9859f, () => {sector_3aaa();  sector_3aaaa();}),
            new BgAction(143.3188f, () => {sector_3aaaa(); sector_3b();}),
            new BgAction(145.3188f, () => {sector_3b();    sector_3bb();}),
            new BgAction(147.6519f, () => {sector_3bb();   sector_3bbb();}),
            new BgAction(148.1519f, () => {sector_3bbb();}),
            new BgAction(149.8184f, () => {sector_3bbb();  sector_3bbbb();}),
            new BgAction(151.3182f, () => {sector_3bbbb();}),
            new BgAction(152.1181f, () => {sector_3bbbb(); sector_3c();}),
            new BgAction(152.9847f, () => {sector_3c();    sector_3cc();}),
            new BgAction(154.8179f, () => {sector_3cc();}),
            new BgAction(155.9844f, () => {sector_3cc();   sector_3ccc();}),
            new BgAction( 157.151f, () => {sector_3ccc();}),
            new BgAction(158.3175f, () => {sector_3ccc();  sector_3cccc();}),
            new BgAction( 159.984f, () => {sector_3cccc();}),
            new BgAction(161.9838f, () => {sector_3cccc(); sector_3d();}),
            new BgAction(164.6502f, () => {sector_3d();    sector_3dd();}),
            new BgAction(166.9833f, () => {sector_3dd();   sector_3ddd();}),
            new BgAction(168.9831f, () => {sector_3ddd();}),
            new BgAction(170.6496f, () => {sector_3ddd();  sector_3e();}),
            new BgAction( 173.316f, () => {sector_3e();    sector_3ee();}),
            new BgAction(175.6491f, () => {sector_3ee();   sector_3eee();}),
            new BgAction(177.6489f, () => {sector_3eee();  sector_3eeee();}),
            new BgAction(178.6488f, () => {sector_3eeee();}),
            new BgAction(180.3f,    () => {sector_3eeee(); sector_0a();})
        };

        animTimeMillis = 0;
        bgIndex = 0;

        lightIntensity = 1f;

        return true;
    }

    /// 終了処理
    public override bool End(MonoManager monoManager)
    {
        if (modelContainer != null) {
            modelContainer.Dispose();
            modelContainer = null;
        }

        if (texContainer != null) {
            texContainer.Dispose();
            texContainer = null;
        }

        if (bgList != null) {
            bgList.Clear();
            bgList = null;
        }

        return true;
    }

    /// 更新処理
    public override bool Update(MonoManager monoManager)
    {
        var cameraModel = modelContainer.Find("CAMERA");
        long worldTimeLength = (long)(cameraModel.GetMotionLength(0) * 1000);

        // アニメーション更新
        animTimeMillis += GameData.FrameTimeMillis;

        if (animTimeMillis >= worldTimeLength) {
            animTimeMillis -= worldTimeLength;
            bgIndex = 0;
        }

        while (bgList[bgIndex].timeMillis < animTimeMillis) {
            bgIndex++;
            if (bgIndex >= bgList.Count) {
                bgIndex = 0;
                break;
            }
        }

        float animTime = (float)animTimeMillis / 1000;
        modelContainer.Find("_2RING").SetAnimTime(0, animTime);
        modelContainer.Find("_1RING").SetAnimTime(0, animTime);
        modelContainer.Find("SPHERE").SetAnimTime(0, animTime);
        cameraModel.SetAnimTime(0, animTime);

        // カメラの更新
        cameraModel.Update();
        viewMatrix = cameraModel.Bones[0].WorldMatrix.Inverse();
        viewProjectionMatrix = projectionMatrix * viewMatrix;
        eye = viewMatrix.Inverse() * new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        // ライトの更新
        light.Position = eye;
        lightControl();

        return true;
    }

    ///
    public void setLightStatus()
    {
        lightStatus = LightStatus.DefToLight;
    }

    /// ライト制御
    private void lightControl()
    {
        light.KDiffuse = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
        light.KSpecular = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);

        /// ライトON
        if ( lightStatus == LightStatus.DefToLight ) {

            lightIntensity += 1.0f;

            if ( lightIntensity >= 3.0f ) {
                lightIntensity = 3.0f;
                lightStatus = LightStatus.LightToDef;
            }

            light.KSpecular *= lightIntensity;
            light.KDiffuse *= lightIntensity;
        }

        /// ライトOFF
        else if ( lightStatus == LightStatus.LightToDef ) {

            lightIntensity -= 0.15f;

            if ( lightIntensity <= 1f ) {
                lightIntensity = 1f;
                lightStatus = LightStatus.Default;
            }

            light.KSpecular *= lightIntensity;
            light.KDiffuse *= lightIntensity;
        }
    }

    /// 描画処理
    public override bool Render(MonoManager monoManager)
    {
        if (bgIndex >= 0 && bgIndex < bgList.Count) {
            bgList[bgIndex].action();
        }

        draw("SPHERE");

        return true;
    }

    /// 移動
    private Matrix4 translate(float x, float y, float z)
    {
        return Matrix4.Translation(new Vector3( x, y, z ));
    }

    /// 回転
    private Matrix4 rotXyzDeg(float x, float y, float z)
    {
        float PI2 = (float)Math.PI * 2.0f;

        Matrix4 mx = Matrix4.RotationX( PI2 * x / 360.0f );
        Matrix4 my = Matrix4.RotationY( PI2 * y / 360.0f );
        Matrix4 mz = Matrix4.RotationZ( PI2 * z / 360.0f );

        return mz * my * mx;
    }

    /// 描画
    private void draw(string modelName, string shaderName, float transX, float transY, float transZ, float rotX, float rotY, float rotZ)
    {
        var graphics = Renderer.GetGraphicsContext();
        var shaderContainer = ShootingData.ShaderContainer;

        BasicModel model = modelContainer.Find(modelName);
        model.SetLightCount(1);
        model.SetLight(0, light);
        model.WorldMatrix = translate(transX, transY, transZ) * rotXyzDeg(rotX, rotY, rotZ);
        model.Update();
        model.Draw(graphics, shaderContainer.Find(shaderName), viewProjectionMatrix, eye);
    }

    /// 描画
    private void draw(string modelName)
    {
        var graphics = Renderer.GetGraphicsContext();
        var shaderContainer = ShootingData.ShaderContainer;

        BasicModel model = modelContainer.Find(modelName);
        model.SetLightCount(1);
        model.SetLight(0, light);
        model.Update();
        model.Draw(graphics, shaderContainer, viewProjectionMatrix, eye);
    }

    /// 分割パーツ描画
    private void sector_0a()
    {
        draw("_12DEG", shaderName, -13404.133300f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        draw("_12DEG", shaderName, -10258.781500f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        draw("_12DEG", shaderName, -7113.429700f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
    }
    private void sector_0aa()
    {
        draw("_2RING", shaderName, -4320.158900f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        draw("_12DEG", shaderName, -1583.707100f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
    }
    private void sector_0aaa()
    {
        draw("_12DEG", shaderName, 1561.644700f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        draw("_2RING", shaderName, 4354.915500f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
        draw("_12DEG", shaderName, 7091.367300f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
    }
    private void sector_0aaaa()
    {
        draw("_12DEG", shaderName, 10236.719100f, 0.000000f, 0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG", shaderName, 13382.070900f, 0.000000f, 0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_90DEG", shaderName, 15298.533300f, 0.000000f, 0.000000f, 45.000000f, 90.000000f, -45.000000f);
    }
    private void sector_0b()
    {
        draw("_20DEG" ,shaderName, 17966.000000f, 0.000000f, -4766.426667f, 0.000000f, -90.000000f, 0.000000f);
        draw("_6DEG" ,shaderName, 18000.000000f, 0.000000f, -6775.674500f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, -9556.270000f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_0bb()
    {
        draw("_60DEG" ,shaderName, 18000.000000f, 0.000000f, -12407.355300f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, -15201.621700f, 0.000000f, 90.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 18000.000000f, 0.000000f, -18052.707100f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_0bbb()
    {
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, -20907.463209f, 0.000000f, 90.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 18000.000000f, 0.000000f, -23698.058900f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, -26492.325300f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_0bbbb()
    {
        draw("_6DEG" ,shaderName, 18000.000000f, 0.000000f, -29329.739900f, 0.000000f, -90.000000f, 0.000000f);
        draw("_20DEG" ,shaderName, 17966.000000f, 0.000000f, -31314.426667f, 0.000000f, 90.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 17997.000000f, 0.000000f, -33154.426667f, -90.000000f, 0.000000f, 90.000000f);
    }
    private void sector_0c()
    {
        draw("_1RING" ,shaderName, 17903.000000f, 5572.000000f, -36056.113333f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0cc()
    {
        draw("_12DEG" ,shaderName, 17882.000000f, 10752.000000f, -36167.113333f, 90.000000f, 0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 17903.000000f, 15482.000000f, -36056.113333f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0ccc()
    {
        draw("_12DEG" ,shaderName, 17882.000000f, 20558.000000f, -36167.113333f, 90.000000f, 0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 17903.000000f, 25378.000000f, -36056.113333f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0cccc()
    {
        draw("_12DEG" ,shaderName, 17882.000000f, 30395.000000f, -36167.113333f, 90.000000f, 0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, 17995.000000f, 33263.000000f, -35838.113333f, 0.000000f, 0.000000f, -0.000000f);
    }
    private void sector_0d()
    {
        draw("_2RING" ,shaderName, 22538.013700f, 36195.475000f, -35887.799963f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 25274.465500f, 36221.350000f, -35926.738333f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 28381.020900f, 36217.850000f, -35832.113333f, 0.000000f, 180.000000f, 0.000000f);
    }
    private void sector_0dd()
    {
        draw("_12DEG_B" ,shaderName, 33380.808700f, 36217.850000f, -35832.113333f, 12.000000f, -0.000000f, -0.000000f);
        draw("_2RING" ,shaderName, 36078.464100f, 36195.475000f, -35887.799963f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 38776.119500f, 36217.850000f, -35832.113333f, 0.000000f, -180.000000f, 0.000000f);
    }
    private void sector_0ddd()
    {
        draw("_12DEG_B" ,shaderName, 43775.907400f, 36217.850000f, -35832.113333f, 12.000000f, -0.000000f, -0.000000f);
        draw("_12DEG" ,shaderName, 46882.462800f, 36221.350000f, -35926.738333f, 0.000000f, 180.000000f, 0.000000f);
        draw("_2RING" ,shaderName, 49618.914600f, 36195.475000f, -35887.799963f, 0.000000f, 0.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 51111.100000f, 36278.100000f, -35844.113333f, 0.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0e()
    {
        draw("_2RING" ,shaderName, 54070.000000f, 31807.000000f, -35907.426704f, 0.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 54073.000000f, 29013.729200f, -35931.426667f, 0.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 54050.000000f, 26219.462800f, -35927.988333f, 0.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0ee()
    {
        draw("_12DEG" ,shaderName, 54073.000000f, 23368.377400f, -35931.426667f, 0.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 54050.000000f, 20574.111000f, -35927.988333f, 0.000000f, 0.000000f, -90.000000f);
        draw("_12DEG" ,shaderName, 54073.000000f, 17723.025600f, -35931.426667f, 0.000000f, 0.000000f, 90.000000f);
    }
    private void sector_0eee()
    {
        draw("_2RING" ,shaderName, 54070.000000f, 14986.573800f, -35907.426704f, 0.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 54073.000000f, 12193.303000f, -35931.426667f, 0.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 54050.000000f, 9399.036600f, -35927.988333f, 0.000000f, 0.000000f, -90.000000f);
    }
    private void sector_0eeee()
    {
        draw("_2RING" ,shaderName, 54070.000000f, 6956.851200f, -35907.426704f, 0.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 54073.000000f, 4163.580400f, -35931.426667f, 0.000000f, 0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, 53936.000000f, 3038.871290f, -36199.906197f, 90.000000f, -90.000000f, 90.000000f);
    }
    private void sector_1a()
    {
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -31307.646711f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -28234.679042f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -25113.429700f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_1aa()
    {
        draw("_2RING" ,shaderName, 54000.000000f, 0.000000f, -22277.348385f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -19583.707100f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -16438.355300f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_1aaa()
    {
        draw("_2RING" ,shaderName, 54000.000000f, 0.000000f, -13645.084500f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -10908.632700f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_1aaaa()
    {
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -7763.280900f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, -4617.510569f, 0.000000f, -90.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 54000.000000f, 0.000000f, -2701.466700f, 90.000000f, -0.000000f, -0.000000f);
    }
    private void sector_1b()
    {
        draw("_20DEG" ,shaderName, 58766.426667f, 0.000000f, -34.000000f, 0.000000f, -180.000000f, 0.000000f);
        draw("_6DEG"  ,shaderName, 60775.674500f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 63556.270000f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_1bb()
    {
        draw("_60DEG" ,shaderName, 66407.355300f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 69201.621700f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 72052.707100f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_1bbb()
    {
        draw("_12DEG" ,shaderName, 74846.973500f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 77698.058900f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 80492.325300f, 0.000000f, -0.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_1bbbb()
    {
        draw("_6DEG"  ,shaderName, 83329.739900f, 0.000000f, -0.000000f, 0.000000f, -180.000000f, 0.000000f);
        draw("_20DEG" ,shaderName, 85314.426667f, 0.000000f, -34.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 87154.426667f, 0.000000f, -3.000000f, -0.000000f, 180.000000f, -90.000000f);
    }
    private void sector_1c()
    {
        draw("_1RING" ,shaderName, 90056.113333f, 5572.000000f, -97.000000f, 0.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1cc()
    {
        draw("_12DEG" ,shaderName, 90167.113333f, 10752.000000f, -118.000000f, 0.000000f, -0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 90056.113333f, 15482.000000f, -97.000000f, 0.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1ccc()
    {
        draw("_12DEG" ,shaderName, 90167.113333f, 20558.000000f, -118.000000f, 0.000000f, -0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 90056.113333f, 25378.000000f, -97.000000f, 0.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1cccc()
    {
        draw("_12DEG" ,shaderName, 90167.113333f, 30395.000000f, -118.000000f, 0.000000f, -0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, 89838.113333f, 33263.000000f, -5.000000f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_1d()
    {
        draw("_2RING" ,shaderName, 89887.799963f, 36195.475000f, 4538.013700f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 89926.738333f, 36221.350000f, 7274.465500f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 89832.113333f, 36217.850000f, 10381.020900f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_1dd()
    {
        draw("_12DEG_B" ,shaderName, 89832.113333f, 36217.850000f, 15380.808700f, 6.000000f, -90.000000f, 6.000000f);
        draw("_2RING" ,shaderName, 89887.799963f, 36195.475000f, 18078.464100f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 89832.113333f, 36217.850000f, 20776.119500f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_1ddd()
    {
        draw("_12DEG_B" ,shaderName, 89832.113333f, 36217.850000f, 25775.907400f, 6.000000f, -90.000000f, 6.000000f);
        draw("_12DEG" ,shaderName, 89926.738333f, 36221.350000f, 28882.462800f, 0.000000f, 90.000000f, 0.000000f);
        draw("_2RING" ,shaderName, 89887.799963f, 36195.475000f, 31618.914600f, 0.000000f, -90.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 89844.113333f, 36278.100000f, 33111.100000f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1e()
    {
        draw("_2RING" ,shaderName, 89907.426704f, 31807.000000f, 36070.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 89931.426667f, 29013.729200f, 36073.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 89927.988333f, 26219.462800f, 36050.000000f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1ee()
    {
        draw("_12DEG" ,shaderName, 89931.426667f, 23368.377400f, 36073.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 89927.988333f, 20574.111000f, 36050.000000f, 90.000000f, -0.000000f, -90.000000f);
        draw("_12DEG" ,shaderName, 89931.426667f, 17723.025600f, 36073.000000f, -90.000000f, -0.000000f, 90.000000f);
    }
    private void sector_1eee()
    {
        draw("_2RING" ,shaderName, 89907.426704f, 14986.573800f, 36070.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 89931.426667f, 12193.303000f, 36073.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, 89927.988333f, 9399.036600f, 36050.000000f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_1eeee()
    {
        draw("_2RING" ,shaderName, 89907.426704f, 6956.851200f, 36070.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 89931.426667f, 4163.580400f, 36073.000000f, -90.000000f, -0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, 90199.906197f, 3038.871290f, 35936.000000f, -0.000000f, -0.000000f, -180.000000f);
    }
    private void sector_2a()
    {
        draw("_12DEG" ,shaderName, 85404.133300f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 82258.781500f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 79113.429700f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_2aa()
    {
        draw("_2RING" ,shaderName, 76320.158900f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 73583.707100f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 70438.355300f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_2aaa()
    {
        draw("_2RING" ,shaderName, 67645.084500f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 64908.632700f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_2aaaa()
    {
        draw("_12DEG" ,shaderName, 61763.280900f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 58617.929100f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 56701.466700f, 0.000000f, 36000.000000f, 45.000000f, -90.000000f, 45.000000f);
    }
    private void sector_2b()
    {
        draw("_20DEG" ,shaderName, 54000.000000f, 0.000000f, 40766.426667f, 0.000000f, 90.000000f, 0.000000f);
        draw("_6DEG" ,shaderName, 54000.000000f, 0.000000f, 42775.674500f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, 45556.270000f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_2bb()
    {
        draw("_60DEG" ,shaderName, 54000.000000f, 0.000000f, 48407.355300f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, 51201.621700f, 0.000000f, -90.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 54000.000000f, 0.000000f, 54052.707100f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_2bbb()
    {
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, 56846.973500f, 0.000000f, -90.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, 54000.000000f, 0.000000f, 59698.058900f, 0.000000f, -90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 54000.000000f, 0.000000f, 62492.325300f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_2bbbb()
    {
        draw("_6DEG" ,shaderName, 54000.000000f, 0.000000f, 65329.739900f, 0.000000f, 90.000000f, 0.000000f);
        draw("_20DEG" ,shaderName, 54034.000000f, 0.000000f, 67314.426667f, 0.000000f, -90.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 54003.000000f, 0.000000f, 69154.426667f, 90.000000f, 0.000000f, 90.000000f);
    }
    private void sector_2c()
    {
        draw("_1RING" ,shaderName, 54097.000000f, 5572.000000f, 72056.113333f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_2cc()
    {
        draw("_12DEG" ,shaderName, 54118.000000f, 10752.000000f, 72167.113333f, -90.000000f, -0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 54097.000000f, 15482.000000f, 72056.113333f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_2ccc()
    {
        draw("_12DEG" ,shaderName, 54118.000000f, 20558.000000f, 72167.113333f, -90.000000f, -0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, 54097.000000f, 25378.000000f, 72056.113333f, 90.000000f, -0.000000f, -90.000000f);
    }
    private void sector_2cccc()
    {
        draw("_12DEG" ,shaderName, 54118.000000f, 30395.000000f, 72167.113333f, -90.000000f, -0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, 54005.000000f, 33263.000000f, 71838.113333f, 0.000000f, -180.000000f, 0.000000f);
    }
    private void sector_2d()
    {
        draw("_2RING"   ,shaderName, 49461.986300f, 36195.475000f, 71887.799963f, 0.000000f, -180.000000f, 0.000000f);
        draw("_12DEG"   ,shaderName, 46725.534500f, 36221.350000f, 71926.738333f, 0.000000f, -180.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 43618.979100f, 36217.850000f, 71832.113333f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_2dd()
    {
        draw("_12DEG_B" ,shaderName, 38619.191300f, 36217.850000f, 71832.113333f, 12.000000f, 180.000000f, 0.000000f);
        draw("_2RING"   ,shaderName, 35921.535900f, 36195.475000f, 71887.799963f, 0.000000f, -180.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, 33223.880500f, 36217.850000f, 71832.113333f, 0.000000f, 0.000000f, 0.000000f);
    }
    private void sector_2ddd()
    {
        draw("_12DEG_B" ,shaderName, 28224.092600f, 36217.850000f, 71832.113333f, 12.000000f, 180.000000f, 0.000000f);
        draw("_12DEG"   ,shaderName, 25117.537200f, 36221.350000f, 71926.738333f, 0.000000f, -0.000000f, 0.000000f);
        draw("_2RING"   ,shaderName, 22381.085400f, 36195.475000f, 71887.799963f, 0.000000f, -180.000000f, 0.000000f);
        draw("_90DEG"   ,shaderName, 20888.900000f, 36278.100000f, 71844.113333f, 0.000000f, -180.000000f, 90.000000f);
    }
    private void sector_2e()
    {
        draw("_2RING" ,shaderName, 17930.000000f, 31807.000000f, 71907.426704f, 0.000000f, -180.000000f, -90.000000f);
        draw("_60DEG" ,shaderName, 17950.000000f, 26219.462800f, 71927.988333f, 0.000000f, -180.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 17927.000000f, 29013.729200f, 71931.426667f, 0.000000f, -180.000000f, -90.000000f);
    }
    private void sector_2ee()
    {
        draw("_12DEG" ,shaderName, 17927.000000f, 23368.377400f, 71931.426667f, 0.000000f, -180.000000f, -90.000000f);
        draw("_60DEG" ,shaderName, 17950.000000f, 20574.111000f, 71927.988333f, 0.000000f, -180.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, 17927.000000f, 17723.025600f, 71931.426667f, 0.000000f, -180.000000f, -90.000000f);
    }
    private void sector_2eee()
    {
        draw("_2RING" ,shaderName, 17930.000000f, 14986.573800f, 71907.426704f, 0.000000f, -180.000000f, -90.000000f);
        draw("_12DEG" ,shaderName, 17927.000000f, 12193.303000f, 71931.426667f, 0.000000f, -180.000000f, -90.000000f);
        draw("_60DEG" ,shaderName, 17950.000000f, 9399.036600f, 71927.988333f, 0.000000f, -180.000000f, 90.000000f);
    }
    private void sector_2eeee()
    {
        draw("_2RING" ,shaderName, 17930.000000f, 6956.851200f, 71907.426704f, 0.000000f, -180.000000f, -90.000000f);
        draw("_12DEG" ,shaderName, 17927.000000f, 4163.580400f, 71931.426667f, 0.000000f, -180.000000f, -90.000000f);
        draw("_90DEG" ,shaderName, 18064.000000f, 3038.871290f, 72199.906197f, 90.000000f, 90.000000f, -90.000000f);
    }
    private void sector_3a()
    {
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 67404.133300f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 64258.781500f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 61113.429700f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_3aa()
    {
        draw("_2RING" ,shaderName, 18000.000000f, 0.000000f, 58320.158900f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 55583.707100f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 52438.355300f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_3aaa()
    {
        draw("_2RING" ,shaderName, 18000.000000f, 0.000000f, 49645.084500f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 46908.632700f, 0.000000f, 90.000000f, 0.000000f);

    }
    private void sector_3aaaa()
    {
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 43763.280900f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 18000.000000f, 0.000000f, 40617.929100f, 0.000000f, 90.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, 18000.000000f, 0.000000f, 38701.466700f, 90.000000f, 180.000000f, 0.000000f);
    }
    private void sector_3b()
    {
        draw("_20DEG" ,shaderName, 13233.573333f, 0.000000f, 36034.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_6DEG"  ,shaderName, 11224.325500f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 8443.730000f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
    }
    private void sector_3bb()
    {
        draw("_60DEG" ,shaderName, 5592.644700f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, 2798.378300f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, -52.707100f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
    }
    private void sector_3bbb()
    {
        draw("_12DEG" ,shaderName, -2846.973500f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_60DEG" ,shaderName, -5698.058900f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_12DEG" ,shaderName, -8492.325300f, 0.000000f, 36000.000000f, 0.000000f, 180.000000f, 0.000000f);
    }
    private void sector_3bbbb()
    {
        draw("_6DEG"  ,shaderName, -11329.739900f, 0.000000f, 36000.000000f, 0.000000f, 0.000000f, 0.000000f);
        draw("_20DEG" ,shaderName, -13314.426667f, 0.000000f, 36034.000000f, 0.000000f, 180.000000f, 0.000000f);
        draw("_90DEG" ,shaderName, -15154.426667f, 0.000000f, 36003.000000f, -0.000000f, -0.000000f, 90.000000f);
    }
    private void sector_3c()
    {
        draw("_1RING" ,shaderName, -18056.113333f, 5572.000000f, 36097.000000f, -180.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3cc()
    {
        draw("_12DEG" ,shaderName, -18167.113333f, 10752.000000f, 36118.000000f, 180.000000f, 0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, -18056.113333f, 15482.000000f, 36097.000000f, -180.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3ccc()
    {
        draw("_12DEG" ,shaderName, -18167.113333f, 20558.000000f, 36118.000000f, 180.000000f, 0.000000f, 90.000000f);
        draw("_1RING" ,shaderName, -18056.113333f, 25378.000000f, 36097.000000f, -180.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3cccc()
    {
        draw("_12DEG"   ,shaderName, -18167.113333f, 30395.000000f, 36118.000000f, 180.000000f, 0.000000f, 90.000000f);
        draw("_90DEG"   ,shaderName, -17838.113333f, 33263.000000f, 36005.000000f, 0.000000f, 90.000000f, 0.000000f);
    }
    private void sector_3d()
    {
        draw("_2RING"   ,shaderName, -17887.799963f, 36195.475000f, 31461.986300f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG"   ,shaderName, -17926.738333f, 36221.350000f, 28725.534500f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, -17832.113333f, 36217.850000f, 25618.979100f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_3dd()
    {
        draw("_12DEG_B" ,shaderName, -17832.113333f, 36217.850000f, 20619.191300f, 6.000000f, 90.000000f, -6.000000f);
        draw("_2RING"   ,shaderName, -17887.799963f, 36195.475000f, 17921.535900f, 0.000000f, 90.000000f, 0.000000f);
        draw("_12DEG_B" ,shaderName, -17832.113333f, 36217.850000f, 15223.880500f, 0.000000f, -90.000000f, 0.000000f);
    }
    private void sector_3ddd()
    {
        draw("_12DEG_B" ,shaderName, -17832.113333f, 36217.850000f, 10224.092600f, 6.000000f, 90.000000f, -6.000000f);
        draw("_12DEG"   ,shaderName, -17926.738333f, 36221.350000f, 7117.537200f, 0.000000f, -90.000000f, 0.000000f);
        draw("_2RING"   ,shaderName, -17887.799963f, 36195.475000f, 4381.085400f, 0.000000f, 90.000000f, 0.000000f);
        draw("_90DEG"   ,shaderName, -17844.113333f, 36278.100000f, 2888.900000f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3e()
    {
        draw("_2RING" ,shaderName, -17907.426704f, 31807.000000f, -70.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, -17931.426667f, 29013.729200f, -73.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, -17927.988333f, 26219.462800f, -50.000000f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3ee()
    {
        draw("_12DEG" ,shaderName, -17931.426667f, 23368.377400f, -73.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, -17927.988333f, 20574.111000f, -50.000000f, -90.000000f, 0.000000f, -90.000000f);
        draw("_12DEG" ,shaderName, -17931.426667f, 17723.025600f, -73.000000f, 90.000000f, 0.000000f, 90.000000f);
    }
    private void sector_3eee()
    {
        draw("_2RING" ,shaderName, -17907.426704f, 14986.573800f, -70.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, -17931.426667f, 12193.303000f, -73.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_60DEG" ,shaderName, -17927.988333f, 9399.036600f, -50.000000f, -90.000000f, 0.000000f, -90.000000f);
    }
    private void sector_3eeee()
    {
        draw("_2RING" ,shaderName, -17907.426704f, 6956.851200f, -70.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_12DEG" ,shaderName, -17931.426667f, 4163.580400f, -73.000000f, 90.000000f, 0.000000f, 90.000000f);
        draw("_90DEG" ,shaderName, -18199.906197f, 3038.871290f, 64.000000f, 180.000000f, -0.000000f, -0.000000f);
    }

}

} // ShootingDemo
