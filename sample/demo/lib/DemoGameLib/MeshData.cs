/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using Sce.PlayStation.Core.Graphics;


namespace DemoGame
{

public class MeshData
{
    float[] positions;
    float[] normals;
    float[] tangents;
    float[] colors;
    float[] texcoords;
    ushort[] indices;
    DrawMode drawMode;

    public MeshData( DrawMode mode, int vertexCount, int indexCount )
    {
        drawMode = mode;
        positions = new float[ vertexCount * 3 ];
        normals = new float[ vertexCount * 3 ];
        tangents = new float[ vertexCount * 3 ];
        colors = new float[ vertexCount * 4 ];
        texcoords = new float[ vertexCount * 2 ];
        indices = new ushort[ indexCount ];
    }

    public void Dispose()
    {
    }

    public DrawMode Prim
    {
        get{ return drawMode; }
    }
    public int VertexCount
    {
        get{ return positions.Length / 3; }
    }
    public int IndexCount
    {
        get{ return indices.Length; }
    }
    public float[] Positions
    {
        get{ return positions; }
    }
    public float[] Colors
    {
        get{ return colors; }
    }
    public float[] Normals
    {
        get{ return normals; }
    }
    public float[] Tangents
    {
        get{ return tangents; }
    }
    public float[] TexCoords
    {
        get{ return texcoords; }
    }
    public ushort[] Indices
    {
        get{ return indices; }
    }
}

} // end ns Sample