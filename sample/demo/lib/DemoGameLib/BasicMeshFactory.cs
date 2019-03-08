/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace DemoGame{

public
class BasicMeshFactory
{
    static float Clamp( float value, float max, float  min )
    {     
        float result = value;
        if( value.CompareTo(max) > 0 ){
            result = max;
        }
        if( value.CompareTo(min) < 0 ){
            result = min;
        }
        return result;
    } 

    /// 正規化キューブマップの生成
    public static TextureCube CreateNormalizeCubeTexture( int width )
    {
        int height = width;
        TextureCube nc = new TextureCube( width, false, PixelFormat.Rgba );
        nc.SetFilter( TextureFilterMode.Linear,
                      TextureFilterMode.Linear,
                      TextureFilterMode.Linear );
        nc.SetWrap( TextureWrapMode.ClampToEdge, TextureWrapMode.ClampToEdge );

        // +X
        {
            byte[] pixsPX = new byte[ width * height * 4 ];
            byte[] pixsNX = new byte[ width * height * 4 ];

            byte[] pixsPY = new byte[ width * height * 4 ];
            byte[] pixsNY = new byte[ width * height * 4 ];

            byte[] pixsPZ = new byte[ width * height * 4 ];
            byte[] pixsNZ = new byte[ width * height * 4 ];

            for( int v = 0; v < width; v++ ){
                float y = (float)(v + v - height) / (float)height;
                for( int u = 0; u < width; u++ ){
                    float x = (float)(u + u - width) / (float)width;

                    float r = 1.0f / (float)Math.Sqrt( (x * x) + (y * y) + 1.0f);
                    float s = x * r;
                    float t = y * r;

                    Vector3 vn = new Vector3( r, s, t );
                    vn = vn.Normalize();

                    r = (r / 2.0f + 0.5f);
                    s = (s / 2.0f + 0.5f);
                    t = (t / 2.0f + 0.5f);

                    int ofst = (4 * u) + ((v * 4) * width);

                    pixsPX[ ofst + 0 ] = (byte)(255 * r);
                    pixsPX[ ofst + 1 ] = (byte)(255 * -t);
                    pixsPX[ ofst + 2 ] = (byte)(255 * -s);
                    pixsPX[ ofst + 3 ] = 0xFF;

                    pixsNX[ ofst + 0 ] = (byte)(255 * -r);
                    pixsNX[ ofst + 1 ] = (byte)(255 * -t);
                    pixsNX[ ofst + 2 ] = (byte)(255 * s);
                    pixsNX[ ofst + 3 ] = 0xFF;

                    pixsPY[ ofst + 0 ] = (byte)(255 * s);
                    pixsPY[ ofst + 1 ] = (byte)(255 * r);
                    pixsPY[ ofst + 2 ] = (byte)(255 * t);
                    pixsPY[ ofst + 3 ] = 0xFF;

                    pixsNY[ ofst + 0 ] = (byte)(255 * s);
                    pixsNY[ ofst + 1 ] = (byte)(255 * -r);
                    pixsNY[ ofst + 2 ] = (byte)(255 * -t);
                    pixsNY[ ofst + 3 ] = 0xFF;

                    pixsPZ[ ofst + 0 ] = (byte)(255 * s);
                    pixsPZ[ ofst + 1 ] = (byte)(255 * -t);
                    pixsPZ[ ofst + 2 ] = (byte)(255 * r);
                    pixsPZ[ ofst + 3 ] = 0xFF;

                    pixsNZ[ ofst + 0 ] = (byte)(255 * -s);
                    pixsNZ[ ofst + 1 ] = (byte)(255 * -t);
                    pixsNZ[ ofst + 2 ] = (byte)(255 * -r);
                    pixsNZ[ ofst + 3 ] = 0xFF;


                }
            }
            nc.SetPixels( 0, TextureCubeFace.PositiveX, pixsPX );
            nc.SetPixels( 0, TextureCubeFace.NegativeX, pixsNX );

            nc.SetPixels( 0, TextureCubeFace.PositiveY, pixsPY );
            nc.SetPixels( 0, TextureCubeFace.NegativeY, pixsNY );

            nc.SetPixels( 0, TextureCubeFace.PositiveZ, pixsPZ );
            nc.SetPixels( 0, TextureCubeFace.NegativeZ, pixsNZ );
        }


        return nc;
    }

    /// トーラスのメッシュ
    static public MeshData CreateTorus( 
                                       float M,   ///< 穴の中心からチューブの中心までの半径
                                       float N,   ///< チューブの半径
                                       int divM,  ///< 外周の分割数
                                       int divN   ///< チューブの分割数
                                       )
    {
        int vtxNum = (divM + 1) * (divN + 1);   // 頂点数
        int indexNum = ((divM + 0) * (divN + 0)) * 6; // インデックス数

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

        float sStep = (float)(Math.PI * 2 / divM);
        float tStep = (float)(Math.PI * 2 / divN);

        for( int m = 0; m < (divM+1); m++ ){
            float t = 0.0f;

            for( int n = 0; n < (divN+1); n++ ){
                float x = (M + N * (float)Math.Cos(Math.PI * 2 - t)) * (float)Math.Cos(s);
                float y = (M + N * (float)Math.Cos(Math.PI * 2 - t)) * (float)Math.Sin(s);
                float z = N * (float)Math.Sin(Math.PI * 2 - t);

                float nx = (float)(Math.Cos(s) * Math.Cos(Math.PI * 2 - t));
                float ny = (float)(Math.Sin(s) * Math.Cos(Math.PI * 2 - t));
                float nz = (float)(Math.Sin(Math.PI * 2 - t));

                float len = (float)Math.Sqrt( x * x + y * y );
                float tx = -y / len;
                float ty = x / len;
                //float tz = 0.0f;

                int idx = (m * (divN+1)) + n;
                mesh.Positions[ (idx * 3) + 0 ] = x;
                mesh.Positions[ (idx * 3) + 1 ] = y;
                mesh.Positions[ (idx * 3) + 2 ] = z;

                mesh.Normals[ (idx * 3) + 0 ] = nx;
                mesh.Normals[ (idx * 3) + 1 ] = ny;
                mesh.Normals[ (idx * 3) + 2 ] = nz;

                mesh.Tangents[ (idx * 3) + 0 ] = tx;
                mesh.Tangents[ (idx * 3) + 1 ] = ty;
                mesh.Tangents[ (idx * 3) + 2 ] = tx;


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

    /// 球のメッシュ
    static public MeshData CreateSphere(
                                        float radius, ///< 半径
                                        int div     ///< 円周に対する分割数
                                        )
    {
        int vtxNum = (div + 0) * (div + 0);         // 頂点数
        int indexNum = ((div + 0) * (div + 0)) * 6; // インデックス数
        
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
        //float a_c = 0.0f;
        //float a_ac = 1.0f / (div * 2);

//        float a_angle_step = 1.0f / (div + 1);

        int i_stacks = div;
        int i_slices = div;
        float i_radius = radius;
	
        float a_st_now = 0;
        float a_stack_step = (float)(Math.PI / (i_stacks - 1));

        int idx = 0;
        for( int i = 0; i < i_stacks; i++ ){
            float a_s = (float)Math.Sin( a_st_now );
            float ny = (float)Math.Cos( a_st_now );
            float py = i_radius * ny;
		
            float a_sl_now = 0;
            float a_slice_step = (float)((Math.PI * 2) / (i_slices - 1));
		
            for( int j = 0; j < i_slices; j++ ){
                float nx = (float)Math.Cos( Math.PI * 2 - a_sl_now ) * a_s;
                float nz = (float)Math.Sin( Math.PI * 2 - a_sl_now ) * a_s;

                float px = i_radius * nx;
                float pz = i_radius * nz;
			
                mesh.Positions[ (idx * 3) + 0 ] = px;
                mesh.Positions[ (idx * 3) + 1 ] = py;
                mesh.Positions[ (idx * 3) + 2 ] = pz;
                
                // 法線
                mesh.Normals[ (idx * 3) + 0 ] = nx;
                mesh.Normals[ (idx * 3) + 1 ] = ny;
                mesh.Normals[ (idx * 3) + 2 ] = nz;

                // 接ベクトル
                float len = (float)Math.Sqrt( nx * nx + nz * nz );
                mesh.Tangents[ (idx * 3) + 0 ] = nz / len;
                mesh.Tangents[ (idx * 3) + 1 ] = 0;
                mesh.Tangents[ (idx * 3) + 2 ] = -nx / len;

                // 特異点
                if( nz == 0.0f && nx == 0.0f ){
                    mesh.Tangents[ (idx * 3) + 0 ] = (float)Math.Cos( a_sl_now );
                    mesh.Tangents[ (idx * 3) + 1 ] = 0;
                    mesh.Tangents[ (idx * 3) + 2 ] = (float)Math.Sin( a_sl_now );
                    
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

    /// 平面のメッシュ
    static public MeshData CreatePlane(
                                       float width,   ///< 幅
                                       float depth,   ///< 奥行き
                                       int divW,      ///< 横幅の分割数
                                       int divD,      ///< 奥行きの分割数
                                       float i_u,     ///< 1マスあたりのUV分割数(1.0f = 1マス)(1.0f/divW = 1面)
                                       float i_v      ///< 1マスあたりのUV分割数(1.0f = 1マス)(1.0f/divD = 1面)
                                       )
    {
        int vtxNum = (divW + 1) * (divD + 1);
        int polyNum = divW * divD * 2;
		float xstep	= (width / divW);
		float zstep	= (depth / divD);
		//float ustep	= (1.0f / divW);
		//float vstep	= (1.0f / divD);
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

                mesh.TexCoords[ uvidx + 0 ] = w * i_u; //w * ustep;
                mesh.TexCoords[ uvidx + 1 ] = d * i_v; //d * vstep;

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

                                      
    /// グリッドのメッシュ
    static public MeshData CreateGrid(
                                      float width,   ///< 横幅
                                      float depth,   ///< 奥行き
                                      int divW,      ///< 横幅の分割数
                                      int divD,      ///< 奥行きの分割数
                                      Rgba colorG,   ///< グリッドの線の色
                                      Rgba colorW,  ///< 中心の線の色(x軸方向)
                                      Rgba colorD   ///< 中心の線の色(z軸方向)
                                      )
	{
		int lineNum = (divW + divD + 2);	// ラインの本数
		int vtxNum	= 2 * lineNum;				// 頂点数

        MeshData mesh = new MeshData( DrawMode.Lines, vtxNum, vtxNum );
        
		float mx = -(width / 2.0f);
		float mz = -(depth / 2.0f);

		float xstep	= (width / divW);
		float zstep	= (depth / divD);

		int i;
		int idx;
		float x = mx;
		// -xから始まって xを増やしながらz方向に線を引く
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

		// -zから始まって zを増やしながらx方向に線を引く
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

		// センター線の色を変える
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


    /// パイプのメッシュ
    static public MeshData CreatePipe(
                                        float radius, ///< 半径
                                        float height, ///< 高さ
                                        int div     ///< 円周に対する分割数
                                        )
    {
        int vtxNum = 4 * div;   // 頂点数
        int indexNum = 6 * div*2; // インデックス数
        
        MeshData mesh = new MeshData( DrawMode.Triangles, vtxNum, indexNum );

        createPipeVertex( mesh, radius, height, div );
        createPipePolygonIndices( mesh, div );

        return mesh;
    }

    static public bool createPipeVertex(
	                                    MeshData mesh,
                                        float radius,
                                        float height,
                                        int div
                                        )
    {
        int   a_pos_cnt		= 0;
        float a_st_now		= 0;
        float a_stack_step	= (float)(Math.PI / div);

        for( int j = 0; j < div*2; j++ ){
            float px = FMath.Cos( a_st_now ) * radius;
            float py = FMath.Sin( a_st_now ) * radius;

            mesh.Positions[ a_pos_cnt + 0 ] = px;
            mesh.Positions[ a_pos_cnt + 1 ] = py;
            mesh.Positions[ a_pos_cnt + 2 ] = height;
            mesh.Positions[ a_pos_cnt + 3 ] = px;
            mesh.Positions[ a_pos_cnt + 4 ] = py;
            mesh.Positions[ a_pos_cnt + 5 ] = 0;
			a_pos_cnt += 6;

            a_st_now += a_stack_step;
		}

		return true;
	}

    static private bool createPipePolygonIndices( 
                                                   MeshData mesh,
                                                   int div
                                                    )
    {
        int a_pv		= div*2*2;
		int a_pos_cnt	= 0;
		int a_idx_cnt	= 0;

        for( int i = 0; i < div*2; i++ ){

            mesh.Indices[ a_idx_cnt+0 ] = (ushort)( ((i*2)+0)%a_pv );
            mesh.Indices[ a_idx_cnt+1 ] = (ushort)( ((i*2)+1)%a_pv );
            mesh.Indices[ a_idx_cnt+2 ] = (ushort)( ((i*2)+3)%a_pv );
                          
            mesh.Indices[ a_idx_cnt+3 ] = (ushort)( ((i*2)+0)%a_pv );
            mesh.Indices[ a_idx_cnt+4 ] = (ushort)( ((i*2)+3)%a_pv );
            mesh.Indices[ a_idx_cnt+5 ] = (ushort)( ((i*2)+2)%a_pv );
			a_idx_cnt += 6;
			a_pos_cnt += 4;
        }
		return true;
	}

}
    

} // end ns Sample
