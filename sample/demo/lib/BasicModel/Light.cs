// -*- mode: csharp; coding: utf-8-dos; tab-width: 4; -*-

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace DemoModel{

/// 点光源
public class Light
{
    public Vector4 Position;
    public Vector4 KDiffuse;
    public Vector4 KSpecular;
    
    /// コンストラクタ
    public Light()
    {
        Position = new Vector4( 0.0f, 0.0f, 0.0f, 1.0f );
        KDiffuse = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        KSpecular = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
    }

    /// コンストラクタ
    public Light( Vector4 pos )
    {
        Position = pos;
        KDiffuse = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        KSpecular = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
    }
}

} // end ns SceneScript
//===
// EOF
//===
