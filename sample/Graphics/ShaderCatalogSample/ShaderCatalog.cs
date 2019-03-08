/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Sample
{

class ShaderCatalog
{
    static public int DspWidth;
    static public int DspHeight;

    private static Stopwatch stopwatch;
    private static GraphicsContext graphics;
    private static List< IScene > sceneList;

    private static Model model;
    private static BgModel modelBg;
    private static LightModel light;
    private static Camera camera;
    private static Vector2 ptPrev = new Vector2();
    private static int idTouch = -1;
    private static bool isMove = false;
    private static int id = 0;

    static bool loop = true;

    public static void Main( string[] args )
    {
        Init();
        while(loop){
            SystemEvents.CheckEvents();
            Update();
            Render();
        }
        Term();
    }

    static void Init()
    {
        stopwatch = new Stopwatch();
        graphics = new GraphicsContext();
        SampleDraw.Init(graphics);

        DspWidth = graphics.Screen.Width;
        DspHeight = graphics.Screen.Height;

        sceneList = new List< IScene >();

        sceneList.Add( new SceneSimpleShader() );
        sceneList.Add( new SceneVertexLightingShader() );
        sceneList.Add( new SceneGouraudShader() );
        sceneList.Add( new ScenePhongShader() );
        sceneList.Add( new SceneGlossMapShader() );
        sceneList.Add( new SceneTextureShader() );
        sceneList.Add( new SceneMultiTextureShader() );
        sceneList.Add( new SceneToonShader() );
        sceneList.Add( new SceneFogShader() );
        sceneList.Add( new SceneBumpMapShader() );
        sceneList.Add( new SceneProjectionShadow() );

        // graphics context
        graphics.SetViewport( 0, 0, DspWidth, DspHeight ) ;
        graphics.SetClearColor( 0.0f, 0.5f, 1.0f, 1.0f ) ;
        graphics.Enable( EnableMode.DepthTest ) ;
        graphics.Enable( EnableMode.CullFace ) ;
        graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;

        // camera
        float aspect = DspWidth / (float)DspHeight;
        float fov = FMath.Radians( 45.0f );
        camera = new Camera( fov, aspect, 10.0f, 100.0f );

        // light
        light = new LightModel();
        light.Color = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        light.Ambient = new Vector4( 0.3f, 0.3f, 0.3f, 1.0f );

        // model
        model = new Model( BasicMeshFactory.CreateTorus( 3.0f, 1.0f, 40, 12 ) );
        model.Position = new Vector3( 0.0f, 4.0f, 0.0f );
        model.DiffuseColor = new Vector4( 0.5f, 0.5f, 1.0f, 1.0f );
        model.AmbientColor = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        model.SpecularColor = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );

        // Bg
        modelBg = new BgModel( 40.0f, 40.0f, 10, 10 );

        id = 0;
        sceneList[ id ].Setup( graphics, model );

        stopwatch.Start();
    }

    /// Terminate
    static void Term()
    {
        graphics.Dispose();
    }

    static void Update()
    {
        SampleDraw.Update();

        ctrlCamera();
        ctrlScene();

    }

    static void Render()
    {
        float delta = (float)stopwatch.ElapsedMilliseconds / 1000.0f;
        stopwatch = Stopwatch.StartNew();

        model.RotateX( 0.25f * delta );
        model.RotateY( -0.25f * delta );
        light.Update( delta );

        sceneList[ id ].Update( delta );
        sceneList[ id ].Render( graphics, camera, light, model, modelBg );

        graphics.Enable( EnableMode.CullFace, false );

        SampleDraw.DrawText("ShaderCatalog Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText( sceneList[ id ].Name, 0xffffffff, 0, 48 );

        graphics.Enable( EnableMode.CullFace, true );
        graphics.SwapBuffers() ;
    }

    /// Scene control
    static void ctrlScene()
    {
        var gamePadData = GamePad.GetData(0);

        if( (gamePadData.ButtonsUp & GamePadButtons.Left) != 0 ){
            callNextScene( -1 );
        }else if( (gamePadData.ButtonsUp & GamePadButtons.Right) != 0 ){
            callNextScene( 1 );
        }
    }

    /// Go to the next shader 
    static void callNextScene( int i )
    {
        int prev = id;
        id += i;
        if( id >= sceneList.Count ){
            id = 0;
        }
        if( id < 0 ){
            id = sceneList.Count - 1;
        }
        if( prev != id ){
            sceneList[ prev ].Dispose();
            sceneList[ id ].Setup( graphics, model );
        }
    }

    /// Camera control
    static void ctrlCamera()
    {
        foreach (var touchData in Touch.GetData(0)) {
            if( idTouch == -1 ){
                if( touchData.Status == TouchStatus.Down ){
                    idTouch = touchData.ID;
                    float pointX = (touchData.X + 0.5f) * DspWidth;
                    float pointY = (touchData.Y + 0.5f) * DspHeight;

                    ptPrev.X = pointX;
                    ptPrev.Y = pointY;
                    isMove = false;
                }
            }else if( idTouch == touchData.ID ){
                if( touchData.Status == TouchStatus.Up ){
                    idTouch = -1;
                    if( isMove == false ){
                        callNextScene( 1 );
                    }
                }else if( touchData.Status == TouchStatus.Move ){
                    float pointX = (touchData.X + 0.5f) * DspWidth;
                    float pointY = (touchData.Y + 0.5f) * DspHeight;
                    Vector2 ptNow = new Vector2( pointX, pointY );

                    float dx = (ptPrev.X - ptNow.X) / (float)DspWidth;
                    float dy = (ptPrev.Y - ptNow.Y) / (float)DspHeight;

                    // input on X axis become the amount of rotation around Y axis
                    if( FMath.Abs( dx ) >= (1.0 / (float)DspWidth) || FMath.Abs( dy ) >= (1.0 / (float)DspHeight) ){
                        isMove = true;
                        camera.OnRotate( ((float)FMath.PI * 2.0f) * dy, ((float)FMath.PI * 2.0f) * dx );
                        ptPrev = ptNow;
                    }
                }
            }
        }
    }
}

} // end ns Sample

