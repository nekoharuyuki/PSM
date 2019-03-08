/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sample
{

class BasicMeshFactory
{
    /// Torus mesh
    static public MeshData CreateTorus( 
                                       float M,   ///< Radius, from the center point to the the middle of the tube
                                       float N,   ///< Tube's radius
                                       int divM,  ///< Outer circumferenc's tessellation
                                       int divN   ///< Tube tesselation
                                       )
    {
        int vtxNum = (divM + 1) * (divN + 1);   // Number of vertices
        int indexNum = ((divM + 0) * (divN + 0)) * 6; // Number of indices

        MeshData mesh = new MeshData( DrawMode.Triangles, vtxNum, indexNum );

        createTorusVertex( mesh, M, N, divM, divN );
        createTorusIndices( mesh, divM, divN );
        
        return mesh;
    }

    static private bool createTorusVertex( MeshData mesh,
                                           float M,
                                           float N,
                                           int divM,
                                           int divN )
    {
        float s = 0.0f;

        float sStep = (float)(FMath.PI * 2 / divM);
        float tStep = (float)(FMath.PI * 2 / divN);

        for( int m = 0; m < (divM+1); m++ ){
            float t = 0.0f;

            for( int n = 0; n < (divN+1); n++ ){
                float x = (M + N * (float)FMath.Cos(FMath.PI * 2 - t)) * (float)FMath.Cos(s);
                float y = (M + N * (float)FMath.Cos(FMath.PI * 2 - t)) * (float)FMath.Sin(s);
                float z = N * (float)FMath.Sin(FMath.PI * 2 - t);

                float nx = (float)(FMath.Cos(s) * FMath.Cos(FMath.PI * 2 - t));
                float ny = (float)(FMath.Sin(s) * FMath.Cos(FMath.PI * 2 - t));
                float nz = (float)(FMath.Sin(FMath.PI * 2 - t));

                float len = (float)FMath.Sqrt( x * x + y * y );
                float tx = -y / len;
                float ty = x / len;
                float tz = 0.0f;

                int idx = (m * (divN+1)) + n;
                mesh.Positions[ (idx * 3) + 0 ] = x;
                mesh.Positions[ (idx * 3) + 1 ] = y;
                mesh.Positions[ (idx * 3) + 2 ] = z;

                mesh.Normals[ (idx * 3) + 0 ] = nx;
                mesh.Normals[ (idx * 3) + 1 ] = ny;
                mesh.Normals[ (idx * 3) + 2 ] = nz;

                mesh.Tangents[ (idx * 3) + 0 ] = tx;
                mesh.Tangents[ (idx * 3) + 1 ] = ty;
                mesh.Tangents[ (idx * 3) + 2 ] = tz;


                mesh.TexCoords[ (idx * 2) + 0 ] = m / (float)divM;
                mesh.TexCoords[ (idx * 2) + 1 ] = n / (float)divN;

                t+=tStep;
            }
            s+=sStep;
        }
        
        return true;
    }

    static private bool createTorusIndices( MeshData mesh,
                                            int divM,
                                            int divN )
    {
        int a_pv = 0;
        for( int j = 0; j < divM; j++ ){
            int ofst = j * (divN+1);

            for( int i = 0; i < divN; i++ ){
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i       + (divN+1)); // 1
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i);                  // 2
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) + (divN+1)); // 3

                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i);                  // 2
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) );           // 4
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) + (divN+1)); // 3
            }
        }

        return true;
    }

    /// Sphere mesh
    static public MeshData CreateSphere(
                                        float radius,
                                        int div     ///< Tessellation along the circumference
                                        )
    {
        int vtxNum = (div + 0) * (div + 0);         // Number of vertices
        int indexNum = ((div + 0) * (div + 0)) * 6; // Number of indices
        
        MeshData mesh = new MeshData( DrawMode.Triangles, vtxNum, indexNum );

        createSphereVertex( mesh, radius, div );
        createSpherePolygonIndices( mesh, div );

        return mesh;
    }

    static private bool createSphereVertex(
                                           MeshData mesh,
                                           float radius,
                                           int div
                                           )
    {
        if( div <= 0 || radius <= 0 ){
            return false;
        }

        int i_stacks = div;
        int i_slices = div;
        float i_radius = radius;

        float a_st_now = 0;
        float a_stack_step = (float)(FMath.PI / (i_stacks - 1));

        int idx = 0;
        for( int i = 0; i < i_stacks; i++ ){
            float a_s = (float)FMath.Sin( a_st_now );
            float ny = (float)FMath.Cos( a_st_now );
            float py = i_radius * ny;

            float a_sl_now = 0;
            float a_slice_step = (float)((FMath.PI * 2) / (i_slices - 1));

            for( int j = 0; j < i_slices; j++ ){
                float nx = (float)FMath.Cos( FMath.PI * 2 - a_sl_now ) * a_s;
                float nz = (float)FMath.Sin( FMath.PI * 2 - a_sl_now ) * a_s;

                float px = i_radius * nx;
                float pz = i_radius * nz;

                mesh.Positions[ (idx * 3) + 0 ] = px;
                mesh.Positions[ (idx * 3) + 1 ] = py;
                mesh.Positions[ (idx * 3) + 2 ] = pz;

                // Normals
                mesh.Normals[ (idx * 3) + 0 ] = nx;
                mesh.Normals[ (idx * 3) + 1 ] = ny;
                mesh.Normals[ (idx * 3) + 2 ] = nz;

                // Tangents
                float len = (float)FMath.Sqrt( nx * nx + nz * nz );
                mesh.Tangents[ (idx * 3) + 0 ] = nz / len;
                mesh.Tangents[ (idx * 3) + 1 ] = 0;
                mesh.Tangents[ (idx * 3) + 2 ] = -nx / len;

                // Singularities
                if( nz == 0.0f && nx == 0.0f ){
                    mesh.Tangents[ (idx * 3) + 0 ] = (float)FMath.Cos( a_sl_now );
                    mesh.Tangents[ (idx * 3) + 1 ] = 0;
                    mesh.Tangents[ (idx * 3) + 2 ] = (float)FMath.Sin( a_sl_now );
                }

                float u = 1.0f / (i_slices - 1)* j;
                float v = 1.0f / (i_stacks - 1) * i;

                mesh.TexCoords[ (idx * 2) + 0 ] = u;
                mesh.TexCoords[ (idx * 2) + 1 ] = v;

                a_sl_now += a_slice_step;
                idx++;
            }
            a_st_now += a_stack_step;
        }
        return true;
    }

    static private bool createSpherePolygonIndices( 
                                                   MeshData mesh,
                                                   int div
                                                    )
    {
        if( div <= 0 ){
            return false;
        }

        int a_pv = 0;
        for( int j = 0; j < div - 1; j++ ){
            int ofst = j * div;

            for( int i = 0; i < div - 1; i++ ){
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i + div ); // 1
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) % div + div); //3
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i); // 2

                mesh.Indices[ a_pv++ ] = (ushort)(ofst + i); // 2
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) % div + div); //3
                mesh.Indices[ a_pv++ ] = (ushort)(ofst + (i + 1) % div); //4
            }
        }

        return true;
    }

    /// Plane mesh
    static public MeshData CreatePlane(
                                       float width,   
                                       float depth,   
                                       int divW,      ///< Tessellation along width direction
                                       int divD,      ///< Tessellation along depth direction
                                       float i_u,     ///< UV subdivision for 1 patch (1.0f = 1 patch)(1.0f/divW = 1 face)
                                       float i_v      ///< UV subdivision for 1 patch (1.0f = 1 patch)(1.0f/divD = 1 face)
                                       )
    {
        int vtxNum = (divW + 1) * (divD + 1);
        int polyNum = divW * divD * 2;
        float xstep = (width / divW);
        float zstep = (depth / divD);
        float mx = -(width / 2.0f);
        float mz = -(depth / 2.0f);

        MeshData mesh = new MeshData( DrawMode.Triangles, vtxNum, polyNum * 3 );

        for( int d = 0; d < (divD + 1); d++ ){
            for( int w = 0; w < (divW + 1); w++ ){
                int idx = ((d * (divW + 1)) + w) * 3;
                int uvidx = ((d * (divW + 1)) + w) * 2;

                mesh.Positions[ idx + 0 ] = mx + w * xstep;
                mesh.Positions[ idx + 1 ] = 0.0f;
                mesh.Positions[ idx + 2 ] = mz + d * zstep;

                mesh.TexCoords[ uvidx + 0 ] = w * i_u;
                mesh.TexCoords[ uvidx + 1 ] = d * i_v;

                mesh.Normals[ idx + 0 ] = 0.0f;
                mesh.Normals[ idx + 1 ] = 1.0f;
                mesh.Normals[ idx + 2 ] = 0.0f;

                mesh.Tangents[ idx + 0 ] = 1.0f;
                mesh.Tangents[ idx + 1 ] = 0.0f;
                mesh.Tangents[ idx + 2 ] = 0.0f;
            }
        }

        // index
        for( int d = 0; d < divD; d++ ){
            for( int w = 0; w < divW; w++ ){
                int planeId = (d * divW) + w;

                mesh.Indices[ (planeId * 6) + 0 ] = (ushort)((d * (divW+1)) + w); // 0
                mesh.Indices[ (planeId * 6) + 1 ] = (ushort)(((d+1) * (divW+1)) + w); // 1
                mesh.Indices[ (planeId * 6) + 2 ] = (ushort)((d * (divW+1)) + w + 1); // 2

                mesh.Indices[ (planeId * 6) + 3 ] = (ushort)(((d+1) * (divW+1)) + w + 1); // 3
                mesh.Indices[ (planeId * 6) + 4 ] = (ushort)((d * (divW+1)) + w + 1); // 2
                mesh.Indices[ (planeId * 6) + 5 ] = (ushort)(((d+1) * (divW+1)) + w); // 1

            }
        }
        return mesh;
    }

    /// Grid mesh
    static public MeshData CreateGrid(
                                      float width,   
                                      float depth,
                                      int divW,      ///< Tesselation along width
                                      int divD,      ///< Tesselation along depth
                                      Rgba colorG,   ///< Color of grid lines
                                      Rgba colorW,  ///< Color of center line, x axis direction
                                      Rgba colorD   ///< Color of center line, z axis direction
                                      )
    {
        int lineNum = (divW + divD + 2);    // Number of lines
        int vtxNum  = 2 * lineNum;              // Number of vertices

        MeshData mesh = new MeshData( DrawMode.Lines, vtxNum, vtxNum );
        
        float mx = -(width / 2.0f);
        float mz = -(depth / 2.0f);

        float xstep = (width / divW);
        float zstep = (depth / divD);

        int i;
        int idx;
        float x = mx;
        // Draw a line on the z direction, starting from -x and incrementing x
        for(i = 0; i < (divW+1); i++){
            idx = i*2;
            mesh.Positions[ (idx * 3) + 0 ] = x;
            mesh.Positions[ (idx * 3) + 1 ] = 0.0f;
            mesh.Positions[ (idx * 3) + 2 ] = -mz;

            mesh.Positions[ ((idx+1) * 3) + 0 ] = x;
            mesh.Positions[ ((idx+1) * 3) + 1 ] = 0.0f;
            mesh.Positions[ ((idx+1) * 3) + 2 ] = mz;
            x += xstep;
        }

        // Draw a line on the x direction, starting from -z and incrementing z
        float z = mz;
        for(; i < lineNum; i++){
            idx = i*2;
            mesh.Positions[ (idx * 3) + 0 ] = -mx;
            mesh.Positions[ (idx * 3) + 1 ] = 0.0f;
            mesh.Positions[ (idx * 3) + 2 ] = z;

            mesh.Positions[ ((idx+1) * 3) + 0 ] = mx;
            mesh.Positions[ ((idx+1) * 3) + 1 ] = 0.0f;
            mesh.Positions[ ((idx+1) * 3) + 2 ] = z;
            z += zstep;
        }

        for(i = 0; i < vtxNum; i++){
            mesh.Normals[ (i*3) + 0 ] = 0.0f;
            mesh.Normals[ (i*3) + 1 ] = 1.0f;
            mesh.Normals[ (i*3) + 2 ] = 0.0f;
            mesh.Colors[ (i*4) + 0 ] = colorG.R / 255.0f;
            mesh.Colors[ (i*4) + 1 ] = colorG.G / 255.0f;
            mesh.Colors[ (i*4) + 2 ] = colorG.B / 255.0f;
            mesh.Colors[ (i*4) + 3 ] = colorG.A / 255.0f;
            
        }

        // Change the color of the center lines
        if( (divW & 1) == 0 && (divD & 1) == 0 ){
            mesh.Colors[ (divW*4) + 0 ] = colorW.R / 255.0f;
            mesh.Colors[ (divW*4) + 1 ] = colorW.G / 255.0f;
            mesh.Colors[ (divW*4) + 2 ] = colorW.B / 255.0f;
            mesh.Colors[ (divW*4) + 3 ] = colorW.A / 255.0f;

            mesh.Colors[ ((divW+1)*4) + 0 ] = colorW.R / 255.0f;
            mesh.Colors[ ((divW+1)*4) + 1 ] = colorW.G / 255.0f;
            mesh.Colors[ ((divW+1)*4) + 2 ] = colorW.B / 255.0f;
            mesh.Colors[ ((divW+1)*4) + 3 ] = colorW.A / 255.0f;

            int ofst = (divW + 1) * 2;
            mesh.Colors[ ((ofst + divD) * 4) + 0 ] = colorD.R / 255.0f;
            mesh.Colors[ ((ofst + divD) * 4) + 1 ] = colorD.G / 255.0f;
            mesh.Colors[ ((ofst + divD) * 4) + 2 ] = colorD.B / 255.0f;
            mesh.Colors[ ((ofst + divD) * 4)+ 3 ] = colorD.A / 255.0f;

            mesh.Colors[ ((ofst + divD + 1) * 4) + 0 ] = colorD.R / 255.0f;
            mesh.Colors[ ((ofst + divD + 1) * 4) + 1 ] = colorD.G / 255.0f;
            mesh.Colors[ ((ofst + divD + 1) * 4) + 2 ] = colorD.B / 255.0f;
            mesh.Colors[ ((ofst + divD + 1) * 4) + 3 ] = colorD.A / 255.0f;
        }

        for(i = 0; i < vtxNum; i++){
            mesh.Indices[ i ] = (ushort)i;
        }

        return mesh;
    }
}

} // end ns Sample
