/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class Model
{
    private static MeshData mesh;

    private static Matrix4 posture;
    private static Vector4 colorAmbient;
    private static Vector4 colorDiffuse;
    private static Vector4 colorSpecular;

    public Model( MeshData meshData )
    {
        mesh = meshData;
        posture = Matrix4.Translation( new Vector3( 0.0f, 0.0f, 0.0f ) );
        colorAmbient = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        colorDiffuse = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
        colorSpecular = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
    }

    public void Dispose()
    {
    }

    public void RotateY( float rot )
    {
        posture = posture * Matrix4.RotationY( rot );
    }

    public void RotateX( float rot )
    {
        posture = posture * Matrix4.RotationX( rot );
    }

    public MeshData Mesh
    {
        get{ return mesh; }
    }
    public Vector3 Position
    {
        set{
            posture = Matrix4.Translation( value );
        }
        get{ return new Vector3( posture.M41, posture.M42, posture.M43 ); }
    }
    public Matrix4 Posture
    {
        get{ return posture; }
    }
    public Vector4 DiffuseColor
    {
        set{ colorDiffuse = value; }
        get{ return colorDiffuse; }
    }
    public Vector4 AmbientColor
    {
        set{ colorAmbient = value; }
        get{ return colorAmbient; }
    }
    public Vector4 SpecularColor
    {
        set{ colorSpecular = value; }
        get{ return colorSpecular; }
    }

}

} // end ns Sample
