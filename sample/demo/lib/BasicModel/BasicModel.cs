//----------------------------------------------------------------
//  BasicModel
//----------------------------------------------------------------

using System ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics;
using System.Collections;
using System.Collections.Generic;


namespace DemoModel {

//----------------------------------------------------------------
//  BasicModel
//----------------------------------------------------------------

public class BasicModel {
	public BasicModel() {
		WorldMatrix = Matrix4.Identity ;
	}
	public BasicModel( string fileName, int index ) {
		WorldMatrix = Matrix4.Identity ;

		var loader = new MdxLoader() ;
		loader.Load( this, fileName, index ) ;
        SortDrawParts();
	}
	public void Dispose() {
		;
	}

    /// (Update()後に) Bone の姿勢を取得する
    public Matrix4 GetBoneMatrix( int idx )
    {
        return this.Bones[ idx ].WorldMatrix;
    }

    /// 指定秒のアニメーションを適用する
	public void SetAnimTime( int idx, float time )
    {
		if ( Motions.Length > idx ){
            BasicMotion motion = Motions[ idx ];
            motion.Frame = time * motion.FrameRate ;

            if ( motion.Frame > motion.FrameEnd ){
                motion.Frame = motion.FrameStart ;
            }
            UpdateBone( motion );
        }
	}

    /// 指定フレームのアニメーションを適用する
	public void SetAnimFrame( int idx, float frame )
    {
		if ( Motions.Length > idx ){
            BasicMotion motion = Motions[ idx ];
            motion.Frame = frame;

            if ( motion.Frame > motion.FrameEnd ){
                motion.Frame = motion.FrameStart ;
            }
            UpdateBone( motion );
        }
	}

    /// 保持している Motion の数
    public int GetMotionCount()
    {
        return Motions.Length;
    }

    /// step秒 分だけアニメーションを進める
	public void Animate( int idx, float step )
    {
		if ( Motions.Length > idx ){
            BasicMotion motion = Motions[ idx ];
            motion.Frame += step * motion.FrameRate ;

            if ( motion.Frame > motion.FrameEnd ){
                motion.Frame = motion.FrameStart ;
            }
            UpdateBone( motion );
        }
	}


    /// Motion の長さをフレームで取得
    public float GetMotionFrameMax( int idx )
    {
		if( Motions.Length > idx ){
            BasicMotion motion = Motions[ idx ];
            return motion.FrameEnd;
        }
        return 0.0f;
    }

	public void Update() {
		foreach ( var bone in Bones ) {
			Matrix4 local = Matrix4.Transformation( bone.Translation, bone.Rotation, bone.Scaling ) ;
			BasicBone parent = ( bone.ParentBone < 0 ) ? null : Bones[ bone.ParentBone ] ;
			if ( parent == null ) {
				bone.WorldMatrix = WorldMatrix * local ;
			} else {
				bone.WorldMatrix = parent.WorldMatrix * local ;
			}
		}
	}

    // START============================================================

    /// TexContainer から利用するテクスチャをバインドする
    /**
     * texture の所有権は持たない(TexContainer 側で管理すること)
     */
    public void BindTextures( TexContainer texContainer )
    {
        for( int i = 0; i < Textures.Length; i++ ){
            if( Textures[ i ].Texture == null ){
                Textures[ i ].Texture = texContainer.Find( Textures[ i ].FileName );
            }
        }
    }

    /// Material::Layer が保持する TexAnim用Matrix の対象 Bone を探して index を格納しておく
    public void BindTexAnimBone()
    {
        foreach(  BasicPart part in Parts ){
            foreach( var mesh in part.Meshes ){
                if( mesh.Material >= 0 ){
                    BasicMaterial material = Materials[ mesh.Material ] ;
                    for( int i = 0; i < material.Layers.Length; i++ ){
                        BasicLayer layer = material.Layers[ i ];

                        // 隠しておいた名前から、Bone の index を確定させる
                        if( layer.TexAnimBoneName != "" ){
                            for( int bi = 0; bi < Bones.Length; bi++ ){
                                if( layer.TexAnimBoneName == Bones[bi].Name ){
                                    layer.TexAnimBoneIdx = bi;
                                    material.UseTexAnim = true;
                                }
                            }
                        }else{
                            layer.TexAnimBoneIdx = -1;
                        }
                    }
                }
            }
		}
    }


    /// DrawOrder による、描画準制御に用いるための SubMesh
    class LocalPart
        : IComparable
    {
        public LocalPart()
        {
            matrixPalette = new Matrix4[ 4 ];
        }

        public int worldCount;
        public VertexBuffer vertexBuffer;
        public Primitive[] primitives;
        public BasicMaterial material;

        // 重複があるので最適化の対象となるもの
        public Matrix4[] matrixPalette;

        /// ソート用比較オペレータ
        public int CompareTo( object obj )
        {
            LocalPart lhs = this;
            LocalPart rhs = (LocalPart)obj;

            if( lhs.material == null || rhs.material == null ){
                return -1;
            }

            return lhs.material.DrawOrder - rhs.material.DrawOrder;
        }
    }

    /// SortDrawParts でソートに利用するクラス
    class Part{
        public Part( BasicMesh mesh, int drawOrder )
        {
            this.mesh = mesh;
            this.drawOrder = drawOrder;
        }
        public BasicMesh mesh;
        public int drawOrder;
    };

    /// DrawParts 内の、mesh を DrawOrder ごとに並び替える
    /**
     * note : DrawPart をまたぐようなソートは行わない点に注意すること.
     */
    public void SortDrawParts()
    {
		foreach ( var bone in Bones ) {
            List< Part > partList = new List< Part >();
			foreach ( var index in bone.DrawParts ) {
				BasicPart part = Parts[ index ] ;
				foreach ( var mesh in part.Meshes ) {
                    BasicMaterial material = Materials[ mesh.Material ];

                    partList.Add( new Part( mesh, material.DrawOrder ) );
                }

                // パーツを DrawOrder 順にソート
                partList.Sort( delegate( Part lhs, Part rhs ){ return lhs.drawOrder - rhs.drawOrder; } );

                int i = 0;
                foreach( var sortedPart in partList ){
                    part.Meshes[ i ] = sortedPart.mesh;
                    i++;
                }
            }
        }
    }

    /// DrawOrder の実装のため、 SubMesn を LocalPart としてリストに積みなおす
    private List< LocalPart > updateLocalPart()
    {
        List< LocalPart > localPartList = new List< LocalPart >();

		foreach ( var bone in Bones ) {
            int worldCount = 0;
            int[] blendBones = bone.BlendBones ;
			if ( blendBones == null ) {
                worldCount = 1;
				matrixBuffer = new Matrix4[ 1 ];
                matrixBuffer[ 0 ] = bone.WorldMatrix;
			}else{
				int blendCount = blendBones.Length ;
				if ( blendCount > matrixBuffer.Length ) matrixBuffer = new Matrix4[ blendCount ] ;
				for ( int i = 0 ; i < blendCount ; i ++ ) {
					matrixBuffer[ i ] = Bones[ blendBones[ i ] ].WorldMatrix * bone.BlendOffsets[ i ] ;
				}
			}
			int[] blendSubset = defaultBlendSubset ;
			foreach ( var index in bone.DrawParts ) {
				BasicPart part = Parts[ index ] ;
				foreach ( var mesh in part.Meshes ) {
					if ( blendBones != null && blendSubset != mesh.BlendSubset ) {
						blendSubset = ( mesh.BlendSubset != null ) ? mesh.BlendSubset : defaultBlendSubset ;
                        worldCount = blendSubset.Length ;
						if ( worldCount > blendBones.Length ) worldCount = blendBones.Length ;
					}

                    // パーツの保存
                    LocalPart localPart = new LocalPart();

                    if( mesh.Material >= 0 ){
                        localPart.material = Materials[ mesh.Material ];
                    }else{
                        localPart.material = null;
                    }
                    localPart.worldCount = worldCount;
                    localPart.vertexBuffer = mesh.VertexBuffer;
                    localPart.primitives = mesh.Primitives;

                    for( int i = 0; i < worldCount; i++ ){
                        localPart.matrixPalette[ i ] = matrixBuffer[ blendSubset[ i ] ];
                    }
                    localPartList.Add( localPart );
                }
            }
        }
        return localPartList;
    }

    /// 有効にするライトの数を設定する
    public void SetLightCount( int count )
    {
        if( lights.Length >= count ){
            lightCount = count;
        }
    }

    /// ライトの設定
    public void SetLight( int idx, Light light )
    {
        if( idx < lights.Length ){
            this.lights[ idx ] = light;
        }
    }

    /// 指定 BasicProgram を必ず利用して描画する
    /**
     * Material は、BasicModel に設定された Material を利用する
     */
    public void Draw( GraphicsContext graphics,
                      BasicProgram program,
                      Matrix4 viewProj,
                      Vector4 eye )
    {
        List< LocalPart > localPartList = updateLocalPart();
        
        // パーツを DrawOrder 順にソート
        localPartList.Sort();

        // パーツの描画
        foreach( LocalPart localPart in localPartList ){
            if( localPart.material != null ){
                setPolygonMode( graphics, localPart.material );

                // [note] local 参照なしの変数(だが、debug用途に残しておきたい)
                /*
                 * int worldCount = localPart.worldCount;
                 * bool texAnim = localPart.material.UseTexAnim;
                 *int texCount = (localPart.material.Layers.Length <= 3) ? localPart.material.Layers.Length : 3;
                 */

                graphics.SetShaderProgram( program );

                program.SetViewProj( viewProj );

                program.SetMaterial( ref localPart.material );

                if( localPart.material.LightEnable != 0 ){
                    program.SetLightCount( lightCount );
					program.SetLights( ref localPart.matrixPalette[ 0 ], ref eye, ref lights );
                }else{
                    program.SetLightCount( 0 );
                }

                setLayers( graphics, program, localPart.material );

                program.SetMatrixPalette( localPart.worldCount, ref localPart.matrixPalette );
                program.Update();
            }

            graphics.SetVertexBuffer( 0, localPart.vertexBuffer ) ;
            graphics.DrawArrays( localPart.primitives ) ;
        }
    }
    /// 指定 BasicProgram と、指定 BasicMaterial を必ず利用して描画する
    public void Draw( GraphicsContext graphics,
                      BasicProgram program,
                      BasicMaterial material,
                      Matrix4 viewProj )
    {
        List< LocalPart > localPartList = updateLocalPart();
        
        // パーツを DrawOrder 順にソート
        localPartList.Sort();

        setPolygonMode( graphics, material );

        graphics.SetShaderProgram( program );
        program.SetViewProj( viewProj );

        program.SetMaterial( ref material );
        setLayers( graphics, program, material );

        // パーツの描画
        foreach( LocalPart localPart in localPartList ){

            program.SetMatrixPalette( localPart.worldCount, ref localPart.matrixPalette );
            program.Update();

            graphics.SetVertexBuffer( 0, localPart.vertexBuffer ) ;
            graphics.DrawArrays( localPart.primitives ) ;
        }
    }

    /// Matrial + WorldCount + LightCount から利用するシェーダを検索する
    public BasicProgram findBasicProgram( ShaderContainer shaderContainer,
                                          BasicMaterial material,
                                          int worldCount,
                                          int lightCount )
    {
        BasicProgram program;

        if( material.BasicShader[ worldCount, lightCount] == null ){
            if( material.ShaderName == "" || material.ShaderName == "basic"){
                if( material.ShaderNameObj == null ){
                    material.ShaderNameObj = new BasicShaderName( worldCount, this.lightCount, material );
                }else{
                    material.ShaderNameObj.SetWorldAndLight( worldCount, this.lightCount );
                }
                
                program = shaderContainer.Find( material.ShaderNameObj );
                                
                material.BasicShader[ worldCount, lightCount] = program;
            }else{
                // 特殊
                program = shaderContainer.Find( material.ShaderName );
                material.BasicShader[ worldCount, lightCount] = program;
                                
                // uniform
                foreach( KeyValuePair< string, int > entry in material.BlindData ){
                    int id = program.FindUniform( entry.Key );
                    if( id >= 0 ){
                        program.SetUniformValue( id, entry.Value / 1000.0f );
                    }
                }
            }
        }else{
            program = material.BasicShader[ worldCount, lightCount ];
        }
        return program;
    }

    /// BasicMaterial に設定された ShaderName から、ShaderProgram を探して描画する
    public void Draw(
                     GraphicsContext graphics,
                     ShaderContainer shaderContainer,
                     Matrix4 viewProj,
                     Vector4 eye
                     )
    {
		foreach ( var bone in Bones ) {
            int worldCount = 0;
            int[] blendBones = bone.BlendBones ;
			if ( blendBones == null ) {
                worldCount = 1;
				matrixBuffer = new Matrix4[ 1 ] ;
                matrixBuffer[ 0 ] = bone.WorldMatrix;
			}else{
				int blendCount = blendBones.Length ;
				if ( blendCount > matrixBuffer.Length ) matrixBuffer = new Matrix4[ blendCount ] ;
				for ( int i = 0 ; i < blendCount ; i ++ ) {
					matrixBuffer[ i ] = Bones[ blendBones[ i ] ].WorldMatrix * bone.BlendOffsets[ i ] ;
				}
			}
			int[] blendSubset = defaultBlendSubset ;
			foreach ( var index in bone.DrawParts ) {
				BasicPart part = Parts[ index ] ;
				foreach ( var mesh in part.Meshes ) {
					if ( blendBones != null && blendSubset != mesh.BlendSubset ) {
						blendSubset = ( mesh.BlendSubset != null ) ? mesh.BlendSubset : defaultBlendSubset ;
                        worldCount = blendSubset.Length ;
						if ( worldCount > blendBones.Length ) worldCount = blendBones.Length ;
					}
                    BasicMaterial material = Materials[ mesh.Material ];

                    if( material != null ){
                        setPolygonMode( graphics, material );

                        BasicProgram program = findBasicProgram( shaderContainer, material, worldCount, this.lightCount );
                        graphics.SetShaderProgram( program );

                        program.SetViewProj( viewProj );
                        
                        program.SetMaterial( ref material );
                        if( material.LightEnable != 0 ){
                            program.SetLightCount( lightCount );
                            //                            program.SetLights( ref this.WorldMatrix, ref eye, ref lights );
                            program.SetLights( ref matrixBuffer[ 0 ], ref eye, ref lights );
                        }else{
                            program.SetLightCount( 0 );
                        }
                        
                        setLayers( graphics, program, material );
                        
                        program.SetWorldCount( worldCount );
                        for( int i = 0; i < worldCount; i++ ){
                            program.SetMatrixPalette( i, ref matrixBuffer[ blendSubset[ i ] ] );
                        }
                        
                        program.Update();
                    }
                    
                    graphics.SetVertexBuffer( 0, mesh.VertexBuffer ) ;
                    graphics.DrawArrays( mesh.Primitives ) ;
                }
            }
        }
    }

    /// BasicProgram を指定した描画(一番オリジナルに近い形)
	public void Draw( 
                     GraphicsContext graphics, 
                     BasicProgram program
                      )
    {
        List< LocalPart > localPartList = updateLocalPart();

        // パーツを DrawOrder 順にソート
        localPartList.Sort();

        // パーツの描画
        foreach( LocalPart localPart in localPartList ){
            if( localPart.material != null ){
                setPolygonMode( graphics, localPart.material );
                program.SetMaterial( ref localPart.material );
                setLayers( graphics, program, localPart.material );
            }

            program.SetMatrixPalette( localPart.worldCount, ref localPart.matrixPalette );
            program.Update();

            graphics.SetVertexBuffer( 0, localPart.vertexBuffer ) ;
            graphics.DrawArrays( localPart.primitives ) ;
        }
    }


    /// Material::Layer の設定を Shader に作用する
    void setLayers( 
                    GraphicsContext graphics,
                    BasicProgram program,
                    BasicMaterial material
                     )
    {
        if( material.Layers == null ){
            return ;
        }

        Texture texture = null ;

        int layerCount = (material.Layers.Length <= 3) ? material.Layers.Length : 3;

        program.SetTexCount( layerCount );

        int texID = 0;
        if( NormalizeCubeMap != null ){
            graphics.SetTexture( texID, NormalizeCubeMap );
            texID++;
        }

        for( int i = 0; i < layerCount; i++ ){
            BasicLayer layer = material.Layers[ i ];
            texture = Textures[ layer.Texture ].Texture ;

            if( texture != null ){
                // TexAnimation
                if( layer.TexAnimBoneIdx >= 0 ){
                    Vector3 st = new Vector3();

                    st.X = 1.0f - Bones[ layer.TexAnimBoneIdx ].Scaling.X;
                    st.Y = 1.0f - Bones[ layer.TexAnimBoneIdx ].Scaling.Y;
                    st.Z = 1.0f - Bones[ layer.TexAnimBoneIdx ].Scaling.Z;
					texAnims[ i ] = Matrix4.Transformation( Bones[ layer.TexAnimBoneIdx ].Translation + st,
                                                            Bones[ layer.TexAnimBoneIdx ].Rotation,
                                                            Bones[ layer.TexAnimBoneIdx ].Scaling ) ;
                }else{
                    texAnims[ i ] = Matrix4.Identity;
                }
                texture.SetWrap( (TextureWrapMode)layer.TexWrapU,
                                 (TextureWrapMode)layer.TexWrapV );
                texture.SetFilter( (TextureFilterMode)layer.magFilter,
                                   (TextureFilterMode)layer.minFilter,
                                   (TextureFilterMode)layer.mipFilter );
                //                graphics.SetTexture( i + 1, texture );
                graphics.SetTexture( texID, texture );
                texID++;
            }
            program.SetTexMtx( texAnims );
            program.SetTexCoord( i, material.Layers[ i ].TexCoord );
            program.SetTangentID( material.Layers[ i ].TangentID );
            program.SetTexType( i, material.Layers[ i ].TexType );
            program.SetTexBlendFunc( i, material.Layers[ i ].TexBlendFunc );
        }
    }


    private void setPolygonMode( GraphicsContext graphics, BasicMaterial material )
    {
        graphics.SetBlendFunc( (BlendFuncMode)material.BlendFuncMode,
                               (BlendFuncFactor)material.SrcBlendFactor,
                               (BlendFuncFactor)material.DstBlendFactor );
        graphics.SetPolygonOffset( material.PolyOffsetFactor / 1000.0f,
                                   material.PolyOffsetUnits / 1000.0f );

        // Depth Test
        graphics.Enable( EnableMode.DepthTest, material.DepthTest != 0 );
        graphics.SetDepthFunc( (DepthFuncMode)material.DepthMode, material.DepthWrite != 0 );

        graphics.Enable( EnableMode.CullFace, (CullFaceMode)material.CullFaceMode != CullFaceMode.None);
        graphics.SetCullFace( (CullFaceMode)material.CullFaceMode,
                              (CullFaceDirection)material.CullFaceDirection );
    }

	//  Subroutines
    public float GetFrame()
    {
        if( Motions.Length > 0 ){
            return getFrame( Motions[ 0 ] );
        }
        return 0.0f;
    }
    private float getFrame( BasicMotion motion )
    {
        return motion.Frame;
    }

    /// Motion の長さを秒で取得
    public float GetMotionLength( int idx )
    {
		if( Motions.Length > idx ){
            BasicMotion motion = Motions[ idx ];
            return motion.FrameEnd / motion.FrameRate;
        }
        return 0.0f;
    }


    void UpdateBone( BasicMotion motion )
    {
		float weight = 1.0f ;

		float[] value = new float[ 4 ] ;
		foreach ( var fcurve in motion.FCurves ) {
			if ( fcurve.TargetType == BasicFCurveTargetType.Bone ){
                EvalFCurve( fcurve, motion.Frame, value ) ;
                BasicBone bone = Bones[ fcurve.TargetIndex ] ;
                switch ( fcurve.ChannelType ) {
                  case BasicFCurveChannelType.Translation :
					Vector3 translation = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					bone.Translation = bone.Translation.Lerp( translation, weight ) ;
					break ;
                  case BasicFCurveChannelType.Rotation :
					Quaternion rotation ;
					if ( fcurve.DimCount == 4 ) {
						rotation = new Quaternion( value[ 0 ], value[ 1 ], value[ 2 ], value[ 3 ] ) ;
					} else {
						Vector3 angles = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
                        // [note] 旧API
						// rotation = Quaternion.FromEulerAnglesZYX( angles ) ;
						rotation = Quaternion.RotationZyx( angles ) ;
					}
                    // [note] 旧API
					// bone.Rotation = Quaternion.Slerp( bone.Rotation, rotation, weight, 0.0f ) ;
                    bone.Rotation = bone.Rotation.Slerp( rotation, weight ) ;
					break ;
                  case BasicFCurveChannelType.Scaling :
					Vector3 scaling = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					bone.Scaling = bone.Scaling.Lerp( scaling, weight ) ;
					break ;
                }
            }else if( fcurve.TargetType == BasicFCurveTargetType.Material ){
                EvalFCurve( fcurve, motion.Frame, value ) ;
                BasicMaterial material = Materials[ fcurve.TargetIndex ];

                switch ( fcurve.ChannelType ) {
                    // 追加
                  case BasicFCurveChannelType.Diffuse:
                    material.Diffuse.X = value[ 0 ];
                    material.Diffuse.Y = value[ 1 ];
                    material.Diffuse.Z = value[ 2 ];
                    material.Diffuse.W = value[ 3 ];
                    break;
                }
            }
		}
	}
	void EvalFCurve( BasicFCurve fcurve, float frame, float[] result ) {
		float[] data = fcurve.KeyFrames ;
		int lower = 0 ;
		int upper = fcurve.KeyCount - 1 ;
		int elementStride = fcurveElementStrides[ (int)fcurve.InterpType ] ;
		int stride = elementStride * fcurve.DimCount + 1 ;
		while ( upper - lower > 1 ) {
			int center = ( upper + lower ) / 2 ;
			float frame2 = data[ stride * center ] ;
			if ( frame < frame2 ) {
				upper = center ;
			} else {
				lower = center ;
			}
		}
		int k1 = stride * lower ;
		int k2 = stride * upper ;
		float f1 = data[ k1 ++ ] ;
		float f2 = data[ k2 ++ ] ;
		float t = f2 - f1 ;
		if ( t != 0.0f ) t = ( frame - f1 ) / t ;

		BasicFCurveInterpType interp = fcurve.InterpType ;
		if ( t <= 0.0f ) interp = BasicFCurveInterpType.Constant ;
		if ( t >= 1.0f ) {
			interp = BasicFCurveInterpType.Constant ;
			k1 = k2 ;
		}
		switch ( interp ) {
			case BasicFCurveInterpType.Constant :
				for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
					result[ i ] = data[ k1 ] ;
					k1 += elementStride ;
				}
				break ;
			case BasicFCurveInterpType.Linear :
				for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
					float v1 = data[ k1 ++ ] ;
					float v2 = data[ k2 ++ ] ;
					result[ i ] = ( v2 - v1 ) * t + v1 ;
				}
				break ;
			case BasicFCurveInterpType.Hermite :
				float s = 1.0f - t ;
				float b1 = s * s * t * 3.0f ;
				float b2 = t * t * s * 3.0f ;
				float b0 = s * s * s + b1 ;
				float b3 = t * t * t + b2 ;
				for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
					result[ i ] = data[ k1 ] * b0 + data[ k1 + 2 ] * b1 + data[ k2 + 1 ] * b2 + data[ k2 ] * b3 ;
					k1 += 3 ;
					k2 += 3 ;
				}
				break ;
			case BasicFCurveInterpType.Cubic :
				for ( int i = 0 ; i < fcurve.DimCount ; i ++ ) {
					float fa = f1 + data[ k1 + 3 ] ;
					float fb = f2 + data[ k2 + 1 ] ;
					float u = FindCubicRoot( f1, fa, fb, f2, frame ) ;
					float v = 1.0f - u ;
					float c1 = v * v * u * 3.0f ;
					float c2 = u * u * v * 3.0f ;
					float c0 = v * v * v + c1 ;
					float c3 = u * u * u + c2 ;
					result[ i ] = data[ k1 ] * c0 + data[ k1 + 4 ] * c1 + data[ k2 + 2 ] * c2 + data[ k2 ] * c3 ;
					k1 += 5 ;
					k2 += 5 ;
				}
				break ;
			case BasicFCurveInterpType.Spherical :
				var q1 = new Quaternion( data[ k1 + 0 ], data[ k1 + 1 ], data[ k1 + 2 ], data[ k1 + 3 ] ) ;
				var q2 = new Quaternion( data[ k2 + 0 ], data[ k2 + 1 ], data[ k2 + 2 ], data[ k2 + 3 ] ) ;
                // [note] 旧API
				// var q3 = Quaternion.Slerp( q1, q2, t, 0.0f ) ;
                var q3 = q1.Slerp( q2, t ) ;
				result[ 0 ] = q3.X ;
				result[ 1 ] = q3.Y ;
				result[ 2 ] = q3.Z ;
				result[ 3 ] = q3.W ;
				break ;
		}
	}
	float FindCubicRoot( float f0, float f1, float f2, float f3, float f )
	{
		float E = ( f3 - f0 ) * 0.01f ;
		if ( E < 0.0001f ) E = 0.0001f ;
		float t0 = 0.0f ;
		float t3 = 1.0f ;
		for ( int i = 0 ; i < 8 ; i ++ ) {
			float d = f3 - f0 ;
			if ( d > -E && d < E ) break ;
			float r = ( f2 - f1 ) / d - ( 1.0f / 3.0f ) ;
			if ( r > -0.01f && r < 0.01f ) break ;
			float fc = ( f0 + f1 * 3.0f + f2 * 3.0f + f3 ) / 8.0f ;
			if ( f < fc ) {
				f3 = fc ;
				f2 = ( f1 + f2 ) * 0.5f ;
				f1 = ( f0 + f1 ) * 0.5f ;
				f2 = ( f1 + f2 ) * 0.5f ;
				t3 = ( t0 + t3 ) * 0.5f ;
			} else {
				f0 = fc ;
				f1 = ( f1 + f2 ) * 0.5f ;
				f2 = ( f2 + f3 ) * 0.5f ;
				f1 = ( f1 + f2 ) * 0.5f ;
				t0 = ( t0 + t3 ) * 0.5f ;
			}
		}
		float c = f0 - f ;
		float b = 3.0f * ( f1 - f0 ) ;
		float a = f3 - f0 - b ;
		float x ;
		if ( a == 0.0f ) {
			x = ( b == 0.0f ) ? 0.5f : -c / b ;
		} else {
			float D2 = b * b - 4.0f * a * c ;
			if ( D2 < 0.0f ) D2 = 0.0f ;
			D2 = (float)Math.Sqrt( D2 ) ;
			if ( a + b < 0.0f ) D2 = -D2 ;
			x = ( -b + D2 ) / ( 2.0f * a ) ;
		}
		return ( t3 - t0 ) * x + t0 ;
	}

	public Matrix4 WorldMatrix ;
	public BasicBone[] Bones ;
	public BasicPart[] Parts ;
	public BasicMaterial[] Materials ;
	public BasicTexture[] Textures ;
	public BasicMotion[] Motions ;

	static Matrix4[] matrixBuffer = new Matrix4[ 0 ] ;
	static int[] defaultBlendSubset = { 0, 1, 2, 3 } ;
	static int[] fcurveElementStrides = { 1, 1, 3, 5, 1 } ;


    public static TextureCube NormalizeCubeMap;
    public static int MaxLightCount = 3;
    private int lightCount = 0;
    private Light[] lights = new Light[ MaxLightCount ];
    private Matrix4[] texAnims = new Matrix4[ 3 ];
}

//----------------------------------------------------------------
//  BasicBone
//----------------------------------------------------------------

public class BasicBone {
	public BasicBone() {
		ParentBone = -1 ;
		Visibility = ~0U ;
		DrawParts = null ;
		BlendBones = null ;
		BlendOffsets = null ;
		Pivot = Vector3.Zero ;
		Translation = Vector3.Zero ;
		Rotation = Quaternion.Identity ;
		Scaling = Vector3.One ;
		WorldMatrix = Matrix4.Identity ;
	}
    public string Name;
	public int ParentBone ;
	public uint Visibility ;
	public int[] DrawParts ;
	public int[] BlendBones ;
	public Matrix4[] BlendOffsets ;
	public Vector3 Pivot ;
	public Vector3 Translation ;
	public Quaternion Rotation ;
	public Vector3 Scaling ;
	public Matrix4 WorldMatrix ;
}

//----------------------------------------------------------------
//  BasicPart / BasicMesh
//----------------------------------------------------------------

public class BasicPart {
	public BasicPart() {
		Meshes = null ;
	}
	public BasicMesh[] Meshes ;
}

public class BasicMesh {
	public BasicMesh() {
		VertexBuffer = null ;
		Primitives = null ;
		BlendSubset = null ;
		Material = -1 ;
	}
	public VertexBuffer VertexBuffer ;
	public Primitive[] Primitives ;
	public int[] BlendSubset ;
	public int Material ;
}

//----------------------------------------------------------------
//  BasicMaterial / BasicLayer
//----------------------------------------------------------------

public class BasicMaterial {
	public BasicMaterial() {
		Layers = null ;
		Diffuse = Vector4.One ;
		Specular = Vector4.Zero ;
		Emission = Vector4.Zero ;
		Ambient = Vector4.One ;
		Opacity = 1.0f ;
		Shininess = 0.0f ;
        AlphaThreshold = 0.0f;
        LightEnable = 0;
        VertexColorEnable = 0;

        UseTexAnim = false;
        BlindData = new Dictionary< string, int >();
        ShaderName = "";
        for( int i = 0; i < 5; i++ ){
            for( int l = 0; l < BasicModel.MaxLightCount; l++ ){
                BasicShader[ i, l ] = null;
            }
        }
	}
	public BasicLayer[] Layers ;
    public string Name;
	public Vector4 Diffuse ;
	public Vector4 Specular ;
	public Vector4 Emission ;
	public Vector4 Ambient ;
	public float Opacity ;
	public float Shininess ;
    public float AlphaThreshold;

    public BasicShaderName ShaderNameObj;
    public string ShaderName;
    public BasicProgram[,] BasicShader = new BasicProgram[5,BasicModel.MaxLightCount];
    public Dictionary< string, int > BlindData;
    public bool UseTexAnim;
    public int CullFaceMode;
    public int CullFaceDirection;
    public int DrawOrder;
    public int BlendFuncMode;
    public int SrcBlendFactor;
    public int DstBlendFactor;
    public int DepthWrite;
    public int DepthMode;
    public int DepthTest;
    public int PolyOffsetFactor;
    public int PolyOffsetUnits;
    public int LightEnable;
    public int VertexColorEnable;
}


public enum TexBlendFunc{
    kBLEND_NONE              = -1,
    kBLEND_REPLACE           = 0,
    kBLEND_ADD               = 1,
    kBLEND_BLEND             = 2,
    kBLEND_DECAL             = 3,
    kBLEND_MODULATE          = 4,
}
/// テクスチャの種別
public enum TexType{
    kTEXTYPE_NONE            = -1,
    kTEXTYPE_IMAGE           = 0,
    kTEXTYPE_VERTEXNORMAL    = 1,
    kTEXTYPE_IMAGENORMALL    = 2,
}


public class BasicLayer {
    /// テクスチャのブレンドファンクション

	public BasicLayer() {
		Texture = -1 ;
        TexAnimBoneName = "";
        TexAnimBoneIdx = -1;
        TexType = 0; // 通常
        TangentID = 0;
        //        BlindData = new Dictionary< string, int >();
	}
	public int Texture;
    public int TexAnimBoneIdx;
    public string TexAnimBoneName;
    //    public Dictionary< string, int > BlindData;
    public int TexCoord;
    public TexType TexType;
    public int TangentID;
    public TexBlendFunc TexBlendFunc;
    public int TexWrapU;
    public int TexWrapV;
    public int magFilter;
    public int minFilter;
    public int mipFilter;

}

//----------------------------------------------------------------
//  BasicTexture
//----------------------------------------------------------------

public class BasicTexture {
	public BasicTexture() {
		Texture = null ;
	}
	public Texture Texture ;
    public string FileName;
}

//----------------------------------------------------------------
//  BasicMotion / BasicFCurve
//----------------------------------------------------------------

public class BasicMotion {
	public BasicMotion() {
		FCurves = null ;
		FrameStart = 0.0f ;
		FrameEnd = 1000000.0f ;
		FrameRate = 60.0f ;
		FrameRepeat = BasicMotionRepeatMode.Cycle ;
		Frame = 0.0f ;
		Weight = 0.0f ;
	}
	public BasicFCurve[] FCurves ;
	public float FrameStart ;
	public float FrameEnd ;
	public float FrameRate ;
	public BasicMotionRepeatMode FrameRepeat ;
	public float Frame ;
	public float Weight ;
}

public class BasicFCurve {
	public BasicFCurve() {
		KeyFrames = null ;
		DimCount = 0 ;
		KeyCount = 0 ;
		InterpType = BasicFCurveInterpType.Constant ;
		TargetType = BasicFCurveTargetType.None ;
		ChannelType = BasicFCurveChannelType.None ;
		TargetIndex = -1 ;
	}
	public float[] KeyFrames ;
	public int DimCount ;
	public int KeyCount ;
	public BasicFCurveInterpType InterpType ;
	public BasicFCurveTargetType TargetType ;
	public BasicFCurveChannelType ChannelType ;
	public int TargetIndex ;
} ;

//----------------------------------------------------------------
//  Public Enums
//----------------------------------------------------------------

public enum BasicMotionRepeatMode {
	Hold,
	Cycle
}

public enum BasicFCurveInterpType {
	Constant,
	Linear,
	Hermite,
	Cubic,
	Spherical
} ;

public enum BasicFCurveTargetType {
	None,
	Bone,
	Material,
} ;

public enum BasicFCurveChannelType {
	None,
	Visibility,
	Translation,
	Rotation,
	Scaling,
	Diffuse,
	Specular,
	Emission,
	Ambient,
	Opacity,
	Shininess,
	TextureCrop
} ;


} // namespace
