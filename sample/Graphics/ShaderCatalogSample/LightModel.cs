/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment; // FIXME : for FMath

namespace Sample
{
    
class LightModel
{
    private static MeshData meshLight;
    private static VertexBuffer vbLight;
    private static ShaderProgram shaderSimple;

    private static Matrix4 posture;
    private static Vector4 color;
    private static Vector4 ambient;

    public Vector4 Ambient
    {
        set{ ambient = value;}
        get{ return ambient; }
    }

    public Vector4 Color
    {
        set{ color = value;}
        get{ return color; }
    }

    public Vector3 Position
    {
        get{ return new Vector3( posture.M41, posture.M42, posture.M43 ); }
    }

    public LightModel()
    {
        // Light
        meshLight = BasicMeshFactory.CreateSphere( 1.0f, 20 );
        vbLight = new VertexBuffer( meshLight.VertexCount,
                                    meshLight.IndexCount,
                                    VertexFormat.Float3 );
        vbLight.SetVertices( 0, meshLight.Positions );
        vbLight.SetIndices( meshLight.Indices );

        // simple shader
        shaderSimple = new ShaderProgram( "/Application/shaders/Simple.cgx" );
        shaderSimple.SetAttributeBinding( 0, "a_Position" );

        // default color
        ambient = new Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
        color = new Vector4( 1.0f, 1.0f, 0.0f, 1.0f );

        posture = Matrix4.Translation( new Vector3( 0.0f, 15.0f, 13.0f ) );
    }

    public void Update( float epsilon )
    {
        float anglePerSec = (float)(FMath.PI * 2) / 5.0f;
        Matrix4 rotation = Matrix4.RotationY( anglePerSec * epsilon );
        posture = rotation * posture;
    }

    /// Render a model representing the light
    public void Render( GraphicsContext graphics,
                        Camera camera )
    {
        Matrix4 projection = camera.Projection;
        Matrix4 view = camera.View;
        Matrix4 worldViewProj = projection * view * posture;

        shaderSimple.SetUniformValue( shaderSimple.FindUniform( "WorldViewProj" ), ref worldViewProj );
        shaderSimple.SetUniformValue( shaderSimple.FindUniform( "MaterialColor" ), ref color );

        graphics.SetShaderProgram( shaderSimple );
        graphics.SetVertexBuffer( 0, vbLight );

        graphics.DrawArrays( DrawMode.Triangles, 0, meshLight.IndexCount );
    }
}

} // end ns Sample