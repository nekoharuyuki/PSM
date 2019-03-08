/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class SceneGlossMapShader
    : IScene
{
    private static VertexBuffer vbTeapot;
    private static ShaderProgram shaderGlossMap;
    private static Texture2D glossmap;

    /// コンストラクタ
    public SceneGlossMapShader()
    {
    }

    /// Name used for display
    public string Name
    {
        get{ return @"< Glossmap >"; }
    }

    /// Initialization
    public void Setup( GraphicsContext graphics, Model model )
    {
        MeshData meshTeapot = model.Mesh;
        vbTeapot = new VertexBuffer( meshTeapot.VertexCount,
                                     meshTeapot.IndexCount,
                                     VertexFormat.Float3,
                                     VertexFormat.Float3,
                                     VertexFormat.Float2 );
        vbTeapot.SetVertices( 0, meshTeapot.Positions );
        vbTeapot.SetVertices( 1, meshTeapot.Normals );
        vbTeapot.SetVertices( 2, meshTeapot.TexCoords );

        vbTeapot.SetIndices( meshTeapot.Indices );

        shaderGlossMap = new ShaderProgram( "/Application/shaders/GlossMap.cgx" );
        shaderGlossMap.SetAttributeBinding( 0, "a_Position" ) ;
        shaderGlossMap.SetAttributeBinding( 1, "a_Normal" ) ;
        shaderGlossMap.SetAttributeBinding( 2, "a_TexCoord" );

        glossmap = new Texture2D( "/Application/data/glossmap.png", false );
        glossmap.SetWrap( TextureWrapMode.Repeat );
    }

    /// Release unmanaged resources
    public void Dispose()
    {
        vbTeapot.Dispose();
        shaderGlossMap.Dispose();
        glossmap.Dispose();
    }

    public void Update( float delta )
    {
    }
    
    public void Render( 
                       GraphicsContext graphics,
                       Camera camera,
                       LightModel light,
                       Model model,
                       BgModel bg
                        )
    {
        graphics.Clear();

        drawModel( graphics, camera, light, model );
        light.Render( graphics, camera );
        bg.Render( graphics, camera );
    }

    void drawModel( 
                   GraphicsContext graphics,
                   Camera camera,
                   LightModel light,
                   Model model
                    )
    {
        Matrix4 world = model.Posture;
        Matrix4 worldViewProj = camera.Projection * camera.View * world;

        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "WorldViewProj" ), ref worldViewProj );

        // model local Light Direction
        Matrix4 worldInverse = world.Inverse();

        Vector4 localLightDir4 = worldInverse * (new Vector4( light.Position, 0 ));
        Vector3 localLightDir = new Vector3( -localLightDir4.X, -localLightDir4.Y, -localLightDir4.Z );
        localLightDir = localLightDir.Normalize();

        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "LocalLightDirection" ), ref localLightDir );

        // model local eye
        Vector4 localEye4 = worldInverse * (new Vector4( camera.Position, 1.0f ));
        Vector3 localEye = new Vector3( localEye4.X, localEye4.Y, localEye4.Z);

        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "EyePosition" ), ref localEye );

        // light
        Vector4 i_a = light.Ambient;
        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "IAmbient" ), ref i_a );
        
        Vector4 i_d = light.Color;
        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "IDiffuse" ), ref i_d );

        // material
        Vector4 k_a = model.AmbientColor;
        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "KAmbient" ), ref k_a );

        Vector4 k_d = model.DiffuseColor;
        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "KDiffuse" ), ref k_d );

        Vector4 k_s = model.SpecularColor;
        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "KSpecular" ), ref k_s );

        shaderGlossMap.SetUniformValue( shaderGlossMap.FindUniform( "Shininess" ), 100.0f );

        graphics.SetShaderProgram( shaderGlossMap );
        graphics.SetTexture( 0, glossmap );
        graphics.SetVertexBuffer( 0, vbTeapot );
        graphics.DrawArrays( model.Mesh.Prim, 0, model.Mesh.IndexCount );
    }
}

} // end ns Sample
