/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.Collections.Generic ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace Sce.PlayStation.HighLevel.Model {

	//------------------------------------------------------------
	//  BasicModel
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model</summary>
	/// @endif
	public class BasicModel : IDisposable {
		/// @if LANG_JA
		/// <summary>モデルを作成する(ファイルから)</summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="index">ファイル内のモデル番号</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a model (from a file)</summary>
		/// <param name="fileName">Filename</param>
		/// <param name="index">Model number in a file</param>
		/// @endif
		public BasicModel( string fileName, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;

			loadDefaultTexture() ;
			var loader = new MdxLoader() ;
			loader.Load( this, fileName, index ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルを作成する(ファイルイメージから)</summary>
		/// <param name="fileImage">ファイルイメージ</param>
		/// <param name="index">ファイル内のモデル番号</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a model (from a file image)</summary>
		/// <param name="fileImage">File image</param>
		/// <param name="index">Model number in a file</param>
		/// @endif
		public BasicModel( byte[] fileImage, int index = 0 ) {
			WorldMatrix = Matrix4.Identity ;

			loadDefaultTexture() ;
			var loader = new MdxLoader() ;
			loader.Load( this, fileImage, index ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルのアンマネージドリソースを解放する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frees unmanaged resources of a model</summary>
		/// @endif
		public void Dispose() {
			Dispose( true ) ;
		}
		protected virtual void Dispose( bool disposing ) {
			if ( disposing ) {
				if ( Parts != null ) {
					foreach ( var part in Parts ) {
						foreach ( var arrays in part.Arrays ) {
							if ( arrays.VertexBuffer != null ) arrays.VertexBuffer.Dispose() ;
							arrays.VertexBuffer = null ;
						}
					}
				}
				if ( Textures != null ) {
					foreach ( var texture in Textures ) {
						if ( texture.Texture != null ) texture.Texture.Dispose() ;
						texture.Texture = null ;
					}
				}
				if ( Programs != null ) {
					foreach ( var program in Programs ) {
						if ( program != null ) program.Dispose() ;
					}
				}
				Bones = null ;
				Parts = null ;
				Materials = null ;
				Textures = null ;
				Motions = null ;
				Programs = null ;
			}
		}

		/// @if LANG_JA
		/// <summary>モデルにプログラムを関連づける</summary>
		/// <param name="container">プログラムコンテナ</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Links a program to a model</summary>
		/// <param name="container">Program Container</param>
		/// @endif
		public void BindPrograms( BasicProgramContainer container ) {
			var programs = new List<BasicProgram>( Programs ) ;
			foreach ( var material in Materials ) {
				if ( material.Program < 0 ) {
					BasicProgram p = container.Find( material.FileName ) ;
					if ( p == null ) p = container.Find( null ) ;
					if ( p == null ) continue ;
					material.Program = programs.Count ;
					programs.Add( p.ShallowClone() as BasicProgram ) ;
				}
			}
			Programs = programs.ToArray() ;
		}
		/// @if LANG_JA
		/// <summary>モデルにテクスチャを関連づける</summary>
		/// <param name="container">テクスチャコンテナ</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Links a texture to a model</summary>
		/// <param name="container">Texture Container</param>
		/// @endif
		public void BindTextures( BasicTextureContainer container ) {
			foreach ( var texture in Textures ) {
				if ( texture.Texture == null ) {
					Texture t = container.Find( texture.FileName ) ;
					if ( t == null ) t = container.Find( null ) ;
					if ( t == null ) continue ;
					if ( t is Texture2D ) texture.Texture = ( t as Texture2D ).ShallowClone() as Texture;
					if ( t is TextureCube ) texture.Texture =  ( t as TextureCube ).ShallowClone() as Texture;
				}
			}
		}

		/// @if LANG_JA
		/// <summary>ワールド行列を設定する</summary>
		/// <param name="world">ワールド行列</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets a world matrix</summary>
		/// <param name="world">World matrix</param>
		/// @endif
		public void SetWorldMatrix( ref Matrix4 world ) {
			WorldMatrix = world ;
		}
		/// @if LANG_JA
		/// <summary>カレントモーションを設定する</summary>
		/// <param name="index">モーション番号</param>
		/// <param name="delay">遅延時間 (単位＝秒)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the current motion</summary>
		/// <param name="index">Motion number</param>
		/// <param name="delay">Delay time (unit = s)</param>
		/// @endif
		public void SetCurrentMotion( int index, float delay = 0.0f ) {
			CurrentMotion = index ;
			TransitionDelay = delay ;
			if ( index >= 0 ) Motions[ index ].Frame = Motions[ index ].FrameStart ;
			if ( delay <= 0.0f ) AnimateTransition( TransitionDelay = 1.0f ) ;
		}
		/// @if LANG_JA
		/// <summary>モデルのアニメーションを計算する</summary>
		/// <param name="step">ステップ時間 (単位＝秒)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Calculates the model animation</summary>
		/// <param name="step">Step time (unit = s)</param>
		/// @endif
		public void Animate( float step ) {
			if ( Motions.Length == 0 ) return ;
			if ( !AnimateTransition( step ) ) {
				if ( CurrentMotion >= 0 ) AnimateMotion( Motions[ CurrentMotion ], step, 0 ) ;
			} else {
				foreach ( var motion in Motions ) AnimateMotion( motion, step, 1 ) ;
				foreach ( var motion in Motions ) AnimateMotion( motion, step, -1 ) ;
			}
		}
		/// @if LANG_JA
		/// <summary>モデルの行列を更新する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Updates the model matrix</summary>
		/// @endif
		public void Update() {
			foreach ( var bone in Bones ) {
				// Matrix4 local = Matrix4.Transformation( bone.Translation, bone.Rotation, bone.Scaling ) ;
				// local.AxisW += bone.Pivot - local.TransformVector( bone.Pivot ) ;
				Matrix4 local ;
				Vector3 pivot ;
				Matrix4.Transformation( ref bone.Translation, ref bone.Rotation, ref bone.Scaling, out local ) ;
				local.TransformVector( ref bone.Pivot, out pivot ) ;
				local.M41 += bone.Pivot.X - pivot.X ;
				local.M42 += bone.Pivot.Y - pivot.Y ;
				local.M43 += bone.Pivot.Z - pivot.Z ;
				BasicBone parent = ( bone.ParentBone < 0 ) ? null : Bones[ bone.ParentBone ] ;
				if ( parent == null ) {
					// bone.WorldMatrix = WorldMatrix * local ;
					WorldMatrix.Multiply( ref local, out bone.WorldMatrix ) ;
				} else {
					// bone.WorldMatrix = parent.WorldMatrix * local ;
					parent.WorldMatrix.Multiply( ref local, out bone.WorldMatrix ) ;
				}
			}
		}
		/// @if LANG_JA
		/// <summary>モデルを描画する</summary>
		/// <param name="graphics">グラフィックスコンテキスト</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Renders a model</summary>
		/// <param name="graphics">Graphics Context</param>
		/// @endif
		public void Draw( GraphicsContext graphics ) { Draw( graphics, null ) ; }
		/// @if LANG_JA
		/// <summary>モデルを描画する (指定されたプログラムで)</summary>
		/// <param name="graphics">グラフィックスコンテキスト</param>
		/// <param name="program">指定されたプログラム</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Renders a model (with the specified program)</summary>
		/// <param name="graphics">Graphics Context</param>
		/// <param name="program">Specified program</param>
		/// @endif
		public void Draw( GraphicsContext graphics, BasicProgram program ) {
			BasicProgram baseProgram = program ;
			BasicParameters parameters = ( program == null ) ? null : program.Parameters ;
			graphics.SetShaderProgram( program ) ;
			int oldMaterial = 0x7fffffff ;
			int oldTexture = 0x7fffffff ;

			bool worldDirty = false ;
			foreach ( var bone in Bones ) {
				if ( bone.Visibility == 0 ) continue ;
				if ( bone.DrawParts == null ) continue ;
				int[] blendBones = bone.BlendBones ;
				if ( blendBones != null ) {
					int blendCount = blendBones.Length ;
					if ( blendCount > matrixBuffer.Length ) matrixBuffer = new Matrix4[ blendCount ] ;
					for ( int i = 0 ; i < blendCount ; i ++ ) {
						// matrixBuffer[ i ] = Bones[ blendBones[ i ] ].WorldMatrix * bone.BlendOffsets[ i ] ;
						Bones[ blendBones[ i ] ].WorldMatrix.Multiply( ref bone.BlendOffsets[ i ], out matrixBuffer[ i ] ) ;
					}
				}
				int[] blendIndices = defaultBlendIndices ;
				worldDirty = true ;
				foreach ( var index in bone.DrawParts ) {
					BasicPart part = Parts[ index ] ;
					foreach ( var mesh in part.Meshes ) {
						if ( oldMaterial != mesh.Material ) {
							BasicMaterial material = Materials[ mesh.Material ] ;
							if ( baseProgram == null ) {
								if ( material.Program < 0 ) continue ;
								program = Programs[ material.Program ] ;
								parameters = program.Parameters ;
								if ( program != graphics.GetShaderProgram() ) {
									graphics.SetShaderProgram( program ) ;
									worldDirty = true ;
								}
							}
							parameters.SetMaterialDiffuse( ref material.Diffuse ) ;
							parameters.SetMaterialSpecular( ref material.Specular ) ;
							parameters.SetMaterialEmission( ref material.Emission ) ;
							parameters.SetMaterialAmbient( ref material.Ambient ) ;
							parameters.SetMaterialOpacity( material.Opacity ) ;
							parameters.SetMaterialShininess( material.Shininess ) ;
							int newTexture = ( material.Layers.Length == 0 ) ? -1 : material.Layers[ 0 ].Texture ;
							if ( oldTexture != newTexture ) {
								BasicTexture texture = ( newTexture < 0 ) ? defaultTexture : Textures[ newTexture ] ;
								graphics.SetTexture( 0, texture.Texture ?? defaultTexture.Texture ) ;
								parameters.SetTexCoordOffset( 0, texture.UVTranslation.X, texture.UVTranslation.Y, texture.UVScaling.X, texture.UVScaling.Y ) ;
								oldTexture = newTexture ;
							}
							oldMaterial = mesh.Material ;
						}
						if ( blendBones == null ) {
							if ( worldDirty ) {
								parameters.SetWorldCount( 1 ) ;
								parameters.SetWorldMatrix( 0, ref bone.WorldMatrix ) ;
								worldDirty = false ;
							}
						} else if ( blendIndices != mesh.BlendIndices ) {
							blendIndices = mesh.BlendIndices ;
							int[] indices = blendIndices ?? defaultBlendIndices ;
							int count = indices.Length ;
							if ( count > blendBones.Length ) count = blendBones.Length ;
							parameters.SetWorldCount( count ) ;
							for ( int i = 0 ; i < count ; i ++ ) {
								parameters.SetWorldMatrix( i, ref matrixBuffer[ indices[ i ] ] ) ;
							}
							worldDirty = true ;
						}
						var vbuffer = part.Arrays[ mesh.Arrays ].VertexBuffer ;
						int n_weights = ( (int)vbuffer.Formats[ 4 ] & 3 ) + 1 ;
						parameters.SetVertexWeightCount( n_weights ) ;
						graphics.SetVertexBuffer( 0, vbuffer ) ;
						graphics.DrawArrays( mesh.Primitives ) ;
					}
				}
			}
		}

		//  Subroutines

		bool AnimateTransition( float step ) {
			if ( TransitionDelay <= 0.0f ) {
				float active = 0.0f ;
				foreach ( var motion in Motions ) active += motion.Weight ;
				float current = ( CurrentMotion < 0 ) ? 0.0f : Motions[ CurrentMotion ].Weight ;
				return ( active != current ) ;
			}
			float t = FMath.Max( TransitionDelay - FMath.Abs( step ), 0.0f ) ;
			float f = t / TransitionDelay ;
			foreach ( var motion in Motions ) motion.Weight *= f ;
			if ( CurrentMotion >= 0 ) Motions[ CurrentMotion ].Weight += 1.0f - f ;
			TransitionDelay = t ;
			return true ;
		}
		void AnimateMotion( BasicMotion motion, float step, int mode ) {
			if ( motion.Weight <= 0.0f ) return ;
			if ( mode >= 0 ) {
				motion.Frame += step * motion.FrameRate ;
				if ( motion.FrameRepeat == BasicMotionRepeatMode.Hold ) {
					motion.Frame = FMath.Clamp( motion.Frame, motion.FrameStart, motion.FrameEnd ) ;
				} else {
					motion.Frame = FMath.Repeat( motion.Frame, motion.FrameStart, motion.FrameEnd ) ;
				}
			}
			float weight = motion.Weight ;
			float[] value = new float[ 4 ] ;
			foreach ( var fcurve in motion.FCurves ) {
				if ( mode >= 0 ) EvalFCurve( fcurve, motion.Frame, value ) ;
				switch ( fcurve.TargetType ) {
					case BasicFCurveTargetType.Bone :
						AnimateBone( Bones[ fcurve.TargetIndex ], fcurve, value, weight, mode ) ;
						break ;
					case BasicFCurveTargetType.Material :
						AnimateMaterial( Materials[ fcurve.TargetIndex ], fcurve, value, weight, mode ) ;
						break ;
					case BasicFCurveTargetType.Texture :
						AnimateTexture( Textures[ fcurve.TargetIndex ], fcurve, value, weight, mode ) ;
						break ;
				}
			}
		}
		void AnimateBone( BasicBone bone, BasicFCurve fcurve, float[] value, float weight, int mode ) {
			float blend = bone.AnimationBlendingData.GetBlending( fcurve, value, weight, mode ) ;
			switch ( fcurve.ChannelType ) {
				case BasicFCurveChannelType.Visibility :
					if ( blend >= 0.5f ) bone.Visibility = (int)( value[ 0 ] + 0.5f ) ;
					break ;
				case BasicFCurveChannelType.Translation :
					Vector3 translation = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					bone.Translation = bone.Translation.Lerp( translation, blend ) ;
					break ;
				case BasicFCurveChannelType.Rotation :
					Quaternion rotation ;
					if ( fcurve.ChannelOption == 0 || mode < 0 ) {
						rotation = new Quaternion( value[ 0 ], value[ 1 ], value[ 2 ], value[ 3 ] ) ;
					} else {
						var angles = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) * FMath.DegToRad ;
						switch ( fcurve.ChannelOption ) {
							case 1 : rotation = Quaternion.RotationXyz( angles ) ; break ;
							case 2 : rotation = Quaternion.RotationYzx( angles ) ; break ;
							case 3 : rotation = Quaternion.RotationZxy( angles ) ; break ;
							case 4 : rotation = Quaternion.RotationXzy( angles ) ; break ;
							case 5 : rotation = Quaternion.RotationYxz( angles ) ; break ;
							default : rotation = Quaternion.RotationZyx( angles ) ; break ;
						}
					}
					bone.Rotation = bone.Rotation.Slerp( rotation, blend ) ;
					break ;
				case BasicFCurveChannelType.Scaling :
					Vector3 scaling = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					bone.Scaling = bone.Scaling.Lerp( scaling, blend ) ;
					break ;
			}
		}
		void AnimateMaterial( BasicMaterial material, BasicFCurve fcurve, float[] value, float weight, int mode ) {
			float blend = material.AnimationBlendingData.GetBlending( fcurve, value, weight, mode ) ;
			switch ( fcurve.ChannelType ) {
				case BasicFCurveChannelType.Diffuse :
					var diffuse = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					material.Diffuse = material.Diffuse.Lerp( diffuse, blend ) ;
					break ;
				case BasicFCurveChannelType.Ambient :
					var ambient = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					material.Ambient = material.Ambient.Lerp( ambient, blend ) ;
					break ;
				case BasicFCurveChannelType.Specular :
					var specular = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					material.Specular = material.Specular.Lerp( specular, blend ) ;
					break ;
				case BasicFCurveChannelType.Emission :
					var emission = new Vector3( value[ 0 ], value[ 1 ], value[ 2 ] ) ;
					material.Emission = material.Emission.Lerp( emission, blend ) ;
					break ;
				case BasicFCurveChannelType.Opacity :
					material.Opacity = FMath.Lerp( material.Opacity, value[ 0 ], blend ) ;
					break ;
				case BasicFCurveChannelType.Shininess :
					material.Shininess = FMath.Lerp( material.Shininess, value[ 0 ], blend ) ;
					break ;
			}
		}
		void AnimateTexture( BasicTexture texture, BasicFCurve fcurve, float[] value, float weight, int mode ) {
			float blend = texture.AnimationBlendingData.GetBlending( fcurve, value, weight, mode ) ;
			switch ( fcurve.ChannelType ) {
				case BasicFCurveChannelType.UVTranslation :
					var transition = new Vector2( value[ 0 ], value[ 1 ] ) ;
					texture.UVTranslation = texture.UVTranslation.Lerp( transition, blend ) ;
					break ;
				case BasicFCurveChannelType.UVScaling :
					var scaling = new Vector2( value[ 0 ], value[ 1 ] ) ;
					texture.UVScaling = texture.UVScaling.Lerp( scaling, blend ) ;
					break ;
			}
		}
		void EvalFCurve( BasicFCurve fcurve, float frame, float[] result ) {
			float[] data = fcurve.KeyFrames ;
			int lower = 0 ;
			int upper = fcurve.KeyCount - 1 ;
			int stride = fcurveStrides[ (int)fcurve.InterpType ] * fcurve.DimCount + 1 ;
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
			int n = fcurve.DimCount ;
			switch ( interp ) {
				case BasicFCurveInterpType.Constant :
					for ( int i = 0 ; i < n ; i ++ ) {
						result[ i ] = data[ k1 ++ ] ;
					}
					break ;
				case BasicFCurveInterpType.Linear :
					for ( int i = 0 ; i < n ; i ++ ) {
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
					for ( int i = 0 ; i < n ; i ++ ) {
						result[ i ] = data[ k1 ] * b0 + data[ k1 + n * 2 ] * b1 + data[ k2 + n ] * b2 + data[ k2 ] * b3 ;
						k1 ++ ;
						k2 ++ ;
					}
					break ;
				case BasicFCurveInterpType.Cubic :
					for ( int i = 0 ; i < n ; i ++ ) {
						float fa = f1 + data[ k1 + n * 4 ] ;
						float fb = f2 + data[ k2 + n * 3 ] ;
						float u = FindCubicRoot( f1, fa, fb, f2, frame ) ;
						float v = 1.0f - u ;
						float c1 = v * v * u * 3.0f ;
						float c2 = u * u * v * 3.0f ;
						float c0 = v * v * v + c1 ;
						float c3 = u * u * u + c2 ;
						result[ i ] = data[ k1 ] * c0 + data[ k1 + n * 2 ] * c1 + data[ k2 + n ] * c2 + data[ k2 ] * c3 ;
						k1 ++ ;
						k2 ++ ;
					}
					break ;
				case BasicFCurveInterpType.Slerp :
					var q1 = new Quaternion( data[ k1 + 0 ], data[ k1 + 1 ], data[ k1 + 2 ], data[ k1 + 3 ] ) ;
					var q2 = new Quaternion( data[ k2 + 0 ], data[ k2 + 1 ], data[ k2 + 2 ], data[ k2 + 3 ] ) ;
					var q3 = q1.Slerp( q2, t ) ;
					result[ 0 ] = q3.X ;
					result[ 1 ] = q3.Y ;
					result[ 2 ] = q3.Z ;
					result[ 3 ] = q3.W ;
					break ;
			}
		}
		float FindCubicRoot( float f0, float f1, float f2, float f3, float f ) {
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

		/// @if LANG_JA
		/// <summary>モデルの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Model name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>バウンディングスフィア</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bounding sphere</summary>
		/// @endif
		public Vector4 BoundingSphere ;
		/// @if LANG_JA
		/// <summary>ワールド行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>World matrix</summary>
		/// @endif
		public Matrix4 WorldMatrix ;
		/// @if LANG_JA
		/// <summary>カレントモーション番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current motion number</summary>
		/// @endif
		public int CurrentMotion ;

		/// @if LANG_JA
		/// <summary>モーション遷移遅延時間 (単位＝秒)</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Motion transition delay time (units = seconds)</summary>
		/// @endif
		public float TransitionDelay ;

		/// @if LANG_JA
		/// <summary>モデルに含まれるボーンの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone array included in a model</summary>
		/// @endif
		public BasicBone[] Bones ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるパートの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Part array included in a model</summary>
		/// @endif
		public BasicPart[] Parts ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるマテリアルの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material array included in a model</summary>
		/// @endif
		public BasicMaterial[] Materials ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるテクスチャの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture array included in a model</summary>
		/// @endif
		public BasicTexture[] Textures ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるモーションの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Motion array included in a model</summary>
		/// @endif
		public BasicMotion[] Motions ;
		/// @if LANG_JA
		/// <summary>モデルに含まれるプログラムの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program array included in a model</summary>
		/// @endif
		public BasicProgram[] Programs ;

		//  load default texture

		static BasicTexture defaultTexture ;
		static BasicTexture loadDefaultTexture() {
			if ( defaultTexture == null ) {
				Texture2D texture2D = new Texture2D( 1, 1, false, PixelFormat.Rgba ) ;
				var pixels = new byte[] { 255, 255, 255, 255 } ;
				texture2D.SetPixels( 0, pixels ) ;
				defaultTexture = new BasicTexture() ;
				defaultTexture.Texture = texture2D ;
			}
			return defaultTexture ;
		}

		static Matrix4[] matrixBuffer = new Matrix4[ 0 ] ;
		static int[] defaultBlendIndices = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } ;
		static int[] fcurveStrides = { 1, 1, 3, 5, 1 } ;
	}

	//----------------------------------------------------------------
	//  BasicBone
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのボーンを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model bone</summary>
	/// @endif
	public class BasicBone {
		/// @if LANG_JA
		/// <summary>ボーンを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a bone</summary>
		/// @endif
		public BasicBone() {
			ParentBone = -1 ;
			Visibility = 1 ;
			DrawParts = null ;
			BlendBones = null ;
			BlendOffsets = null ;
			Pivot = Vector3.Zero ;
			Translation = Vector3.Zero ;
			Rotation = Quaternion.Identity ;
			Scaling = Vector3.One ;
			WorldMatrix = Matrix4.Identity ;
		}
		/// @if LANG_JA
		/// <summary>ボーンの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>バウンディングスフィア</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bounding sphere</summary>
		/// @endif
		public Vector4 BoundingSphere ;
		/// @if LANG_JA
		/// <summary>親ボーンの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Parent bone number</summary>
		/// @endif
		public int ParentBone ;
		/// @if LANG_JA
		/// <summary>ビジビリティ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Visibility</summary>
		/// @endif
		public int Visibility ;
		/// @if LANG_JA
		/// <summary>描画パートの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Render part number</summary>
		/// @endif
		public int[] DrawParts ;
		/// @if LANG_JA
		/// <summary>スキニングの影響ボーンの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning effect bone number</summary>
		/// @endif
		public int[] BlendBones ;
		/// @if LANG_JA
		/// <summary>スキニングのオフセット行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning offset matrix</summary>
		/// @endif
		public Matrix4[] BlendOffsets ;
		/// @if LANG_JA
		/// <summary>ピボット位置</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Pivot position</summary>
		/// @endif
		public Vector3 Pivot ;
		/// @if LANG_JA
		/// <summary>移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Translation</summary>
		/// @endif
		public Vector3 Translation ;
		/// @if LANG_JA
		/// <summary>回転量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Rotation amount</summary>
		/// @endif
		public Quaternion Rotation ;
		/// @if LANG_JA
		/// <summary>スケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Scaling</summary>
		/// @endif
		public Vector3 Scaling ;
		/// @if LANG_JA
		/// <summary>ワールド行列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>World matrix</summary>
		/// @endif
		public Matrix4 WorldMatrix ;

		internal BasicAnimationBlendingData AnimationBlendingData ;
	}

	//----------------------------------------------------------------
	//  BasicPart / BasicMesh / BasicArrays
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのパートを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model part</summary>
	/// @endif
	public class BasicPart {
		/// @if LANG_JA
		/// <summary>パートを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a part</summary>
		/// @endif
		public BasicPart() {
			Meshes = null ;
		}
		/// @if LANG_JA
		/// <summary>パートの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Part name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>パートに含まれるメッシュの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Mesh array included in a part</summary>
		/// @endif
		public BasicMesh[] Meshes ;
		/// @if LANG_JA
		/// <summary>パートに含まれる頂点配列の配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Array of vertex array included in a part</summary>
		/// @endif
		public BasicArrays[] Arrays ;
	}

	/// @if LANG_JA
	/// <summary>モデルのメッシュを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model mesh</summary>
	/// @endif
	public class BasicMesh {
		/// @if LANG_JA
		/// <summary>メッシュを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a mesh</summary>
		/// @endif
		public BasicMesh() {
			Material = -1 ;
			Arrays = -1 ;
			BlendIndices = null ;
			Primitives = null ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material number</summary>
		/// @endif
		public int Material ;
		/// @if LANG_JA
		/// <summary>頂点配列の番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Number of the vertex array</summary>
		/// @endif
		public int Arrays ;
		/// @if LANG_JA
		/// <summary>スキニングの行列番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Skinning matrix number</summary>
		/// @endif
		public int[] BlendIndices ;
		/// @if LANG_JA
		/// <summary>プリミティブの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Primitive array</summary>
		/// @endif
		public Primitive[] Primitives ;
	}

	/// @if LANG_JA
	/// <summary>モデルの頂点配列を表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model vertex array</summary>
	/// @endif
	public class BasicArrays {
		/// @if LANG_JA
		/// <summary>頂点配列を作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Create vertex array</summary>
		/// @endif
		public BasicArrays() {
			VertexBuffer = null ;
		}
		/// @if LANG_JA
		/// <summary>頂点バッファ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Vertex buffer</summary>
		/// @endif
		public VertexBuffer VertexBuffer ;
	}

	//----------------------------------------------------------------
	//  BasicMaterial / BasicLayer
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのマテリアルを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model material</summary>
	/// @endif
	public class BasicMaterial {
		/// @if LANG_JA
		/// <summary>マテリアルを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a material</summary>
		/// @endif
		public BasicMaterial() {
			Program = -1 ;
			FileName = null ;
			Diffuse = Vector3.One ;
			Specular = Vector3.Zero ;
			Ambient = Vector3.One ;
			Emission = Vector3.Zero ;
			Opacity = 1.0f ;
			Shininess = 0.0f ;
			Layers = null ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>プログラムの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program number</summary>
		/// @endif
		public int Program ;
		/// @if LANG_JA
		/// <summary>プログラムのファイル名</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Program filename</summary>
		/// @endif
		public string FileName ;
		/// @if LANG_JA
		/// <summary>ディフューズカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Diffuse color</summary>
		/// @endif
		public Vector3 Diffuse ;
		/// @if LANG_JA
		/// <summary>スペキュラーカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Specular color</summary>
		/// @endif
		public Vector3 Specular ;
		/// @if LANG_JA
		/// <summary>アンビエントカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Ambient color</summary>
		/// @endif
		public Vector3 Ambient ;
		/// @if LANG_JA
		/// <summary>エミッションカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Emission color</summary>
		/// @endif
		public Vector3 Emission ;
		/// @if LANG_JA
		/// <summary>不透明度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Opacity</summary>
		/// @endif
		public float Opacity ;
		/// @if LANG_JA
		/// <summary>輝度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Luminance</summary>
		/// @endif
		public float Shininess ;
		/// @if LANG_JA
		/// <summary>マテリアルに含まれるレイヤーの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Layer array included in a material</summary>
		/// @endif
		public BasicLayer[] Layers ;

		internal BasicAnimationBlendingData AnimationBlendingData ;
	}

	/// @if LANG_JA
	/// <summary>モデルのレイヤーを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model layer</summary>
	/// @endif
	public class BasicLayer {
		/// @if LANG_JA
		/// <summary>レイヤーを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a layer</summary>
		/// @endif
		public BasicLayer() {
			Texture = -1 ;
		}
		/// @if LANG_JA
		/// <summary>テクスチャの番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture number</summary>
		/// @endif
		public int Texture ;
	}

	//----------------------------------------------------------------
	//  BasicTexture
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのテクスチャを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model texture</summary>
	/// @endif
	public class BasicTexture {
		/// @if LANG_JA
		/// <summary>テクスチャを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a texture</summary>
		/// @endif
		public BasicTexture() {
			Texture = null ;
			FileName = null ;
			UVTranslation = new Vector2( 0.0f, 0.0f ) ;
			UVScaling = new Vector2( 1.0f, 1.0f ) ;
		}
		/// @if LANG_JA
		/// <summary>テクスチャの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>使用するテクスチャ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture to be used</summary>
		/// @endif
		public Texture Texture ;
		/// @if LANG_JA
		/// <summary>使用するテクスチャのファイル名</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Filename of texture to be used</summary>
		/// @endif
		public string FileName ;
		/// @if LANG_JA
		/// <summary>UV 座標の移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Translation of UV coordinates</summary>
		/// @endif
		public Vector2 UVTranslation ;
		/// @if LANG_JA
		/// <summary>UV 座標のスケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>UV coordinate scaling</summary>
		/// @endif
		public Vector2 UVScaling ;

		internal BasicAnimationBlendingData AnimationBlendingData ;
	}

	//----------------------------------------------------------------
	//  BasicMotion / BasicFCurve
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モデルのモーションを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model motion</summary>
	/// @endif
	public class BasicMotion {
		/// @if LANG_JA
		/// <summary>モーションを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a motion</summary>
		/// @endif
		public BasicMotion() {
			FrameStart = 0.0f ;
			FrameEnd = 1000000.0f ;
			FrameRate = 60.0f ;
			FrameRepeat = BasicMotionRepeatMode.Cycle ;
			Frame = 0.0f ;
			Weight = 0.0f ;
			FCurves = null ;
		}
		/// @if LANG_JA
		/// <summary>モーションの名前</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Motion name</summary>
		/// @endif
		public string Name ;
		/// @if LANG_JA
		/// <summary>開始フレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Start frame</summary>
		/// @endif
		public float FrameStart ;
		/// @if LANG_JA
		/// <summary>終了フレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>End frame</summary>
		/// @endif
		public float FrameEnd ;
		/// @if LANG_JA
		/// <summary>フレームレート</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frame rate</summary>
		/// @endif
		public float FrameRate ;
		/// @if LANG_JA
		/// <summary>繰り返しモード</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Repeat mode</summary>
		/// @endif
		public BasicMotionRepeatMode FrameRepeat ;
		/// @if LANG_JA
		/// <summary>現在のフレーム</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current frame</summary>
		/// @endif
		public float Frame ;
		/// @if LANG_JA
		/// <summary>現在のブレンド係数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Current blend coefficient</summary>
		/// @endif
		public float Weight ;
		/// @if LANG_JA
		/// <summary>モーションに含まれる関数カーブの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Function curve array included in a motion</summary>
		/// @endif
		public BasicFCurve[] FCurves ;
	}

	/// @if LANG_JA
	/// <summary>モデルの関数カーブを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a model function curve</summary>
	/// @endif
	public class BasicFCurve {
		/// @if LANG_JA
		/// <summary>関数カーブを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a function curve</summary>
		/// @endif
		public BasicFCurve() {
			DimCount = 0 ;
			KeyCount = 0 ;
			InterpType = BasicFCurveInterpType.Constant ;
			TargetType = BasicFCurveTargetType.None ;
			ChannelType = BasicFCurveChannelType.None ;
			ChannelOption = 0 ;
			TargetIndex = -1 ;
			KeyFrames = null ;
		}
		/// @if LANG_JA
		/// <summary>次元数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Dimensionality</summary>
		/// @endif
		public int DimCount ;
		/// @if LANG_JA
		/// <summary>キー数</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Number of keys</summary>
		/// @endif
		public int KeyCount ;
		/// @if LANG_JA
		/// <summary>アニメーションの補間タイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation interpolation type</summary>
		/// @endif
		public BasicFCurveInterpType InterpType ;
		/// @if LANG_JA
		/// <summary>アニメーションのターゲットタイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation target type</summary>
		/// @endif
		public BasicFCurveTargetType TargetType ;
		/// @if LANG_JA
		/// <summary>アニメーションのチャンネルタイプ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation channel type</summary>
		/// @endif
		public BasicFCurveChannelType ChannelType ;
		/// @if LANG_JA
		/// <summary>アニメーションのチャンネルオプション</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation channel option</summary>
		/// @endif
		public int ChannelOption ;
		/// @if LANG_JA
		/// <summary>アニメーションのターゲット番号</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Animation target number</summary>
		/// @endif
		public int TargetIndex ;
		/// @if LANG_JA
		/// <summary>キーフレームの配列</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Key frame array</summary>
		/// @endif
		public float[] KeyFrames ;
	} ;

	//----------------------------------------------------------------
	//  Public Enums
	//----------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>モーションの繰り返しモード</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Motion repeat mode</summary>
	/// @endif
	public enum BasicMotionRepeatMode {
		/// @if LANG_JA
		/// <summary>単発再生</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Individual playback</summary>
		/// @endif
		Hold,
		/// @if LANG_JA
		/// <summary>繰り返し再生</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Repeat playback</summary>
		/// @endif
		Cycle
	}

	/// @if LANG_JA
	/// <summary>アニメーションの補間タイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation interpolation type</summary>
	/// @endif
	public enum BasicFCurveInterpType {
		/// @if LANG_JA
		/// <summary>定数補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Constant interpolation</summary>
		/// @endif
		Constant,
		/// @if LANG_JA
		/// <summary>線形補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Linear interpolation</summary>
		/// @endif
		Linear,
		/// @if LANG_JA
		/// <summary>エルミート補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Hermite interpolation</summary>
		/// @endif
		Hermite,
		/// @if LANG_JA
		/// <summary>キュービック補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Cubic interpolation</summary>
		/// @endif
		Cubic,
		/// @if LANG_JA
		/// <summary>球面線形補間</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Spherical linear interpolation</summary>
		/// @endif
		Slerp
	} ;

	/// @if LANG_JA
	/// <summary>アニメーションのターゲットタイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation target type</summary>
	/// @endif
	public enum BasicFCurveTargetType {
		/// @if LANG_JA
		/// <summary>なし</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>None</summary>
		/// @endif
		None,
		/// @if LANG_JA
		/// <summary>ボーン</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone</summary>
		/// @endif
		Bone,
		/// @if LANG_JA
		/// <summary>マテリアル</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material</summary>
		/// @endif
		Material,
		/// @if LANG_JA
		/// <summary>テクスチャ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Textures</summary>
		/// @endif
		Texture
	} ;

	/// @if LANG_JA
	/// <summary>アニメーションのチャンネルタイプ</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Animation channel type</summary>
	/// @endif
	public enum BasicFCurveChannelType {
		/// @if LANG_JA
		/// <summary>なし</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>None</summary>
		/// @endif
		None,
		/// @if LANG_JA
		/// <summary>ボーンのビジビリティ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone visibility</summary>
		/// @endif
		Visibility,
		/// @if LANG_JA
		/// <summary>ボーンの移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone translation</summary>
		/// @endif
		Translation,
		/// @if LANG_JA
		/// <summary>ボーンの回転量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone rotation amount</summary>
		/// @endif
		Rotation,
		/// @if LANG_JA
		/// <summary>ボーンのスケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Bone scaling</summary>
		/// @endif
		Scaling,
		/// @if LANG_JA
		/// <summary>マテリアルのディフューズカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material diffuse color</summary>
		/// @endif
		Diffuse,
		/// @if LANG_JA
		/// <summary>マテリアルのスペキュラーカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material specular color</summary>
		/// @endif
		Specular,
		/// @if LANG_JA
		/// <summary>マテリアルのエミッションカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material emission color</summary>
		/// @endif
		Emission,
		/// @if LANG_JA
		/// <summary>マテリアルのアンビエントカラー</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material ambient color</summary>
		/// @endif
		Ambient,
		/// @if LANG_JA
		/// <summary>マテリアルの不透明度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material opacity</summary>
		/// @endif
		Opacity,
		/// @if LANG_JA
		/// <summary>マテリアルの輝度</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Material luminance</summary>
		/// @endif
		Shininess,
		/// @if LANG_JA
		/// <summary>テクスチャ座標の移動量</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Translation of texture coordinates</summary>
		/// @endif
		UVTranslation,
		/// @if LANG_JA
		/// <summary>テクスチャ座標のスケーリング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Texture coordinate scaling</summary>
		/// @endif
		UVScaling
	} ;

	//----------------------------------------------------------------
	//  Internal Types
	//----------------------------------------------------------------

	internal struct BasicAnimationBlendingData {
		public BasicFCurveChannelType BaseChannel ;
		public float[][] InitialValues ;
		public float[] TotalWeights ;

		public BasicAnimationBlendingData( BasicBone bone ) {
			BaseChannel = BasicFCurveChannelType.Visibility ;
			InitialValues = new float[ 4 ][] ;
			TotalWeights = new float[ 4 ] ;

			SetValue( BasicFCurveChannelType.Visibility, (float)(int)bone.Visibility ) ;
			SetValue( BasicFCurveChannelType.Translation, bone.Translation ) ;
			SetValue( BasicFCurveChannelType.Rotation, bone.Rotation ) ;
			SetValue( BasicFCurveChannelType.Scaling, bone.Scaling ) ;
		}
		public BasicAnimationBlendingData( BasicMaterial material ) {
			BaseChannel = BasicFCurveChannelType.Diffuse ;
			InitialValues = new float[ 6 ][] ;
			TotalWeights = new float[ 6 ] ;

			SetValue( BasicFCurveChannelType.Diffuse, material.Diffuse ) ;
			SetValue( BasicFCurveChannelType.Ambient, material.Ambient ) ;
			SetValue( BasicFCurveChannelType.Specular, material.Specular ) ;
			SetValue( BasicFCurveChannelType.Emission, material.Emission ) ;
			SetValue( BasicFCurveChannelType.Opacity, material.Opacity ) ;
			SetValue( BasicFCurveChannelType.Shininess, material.Shininess ) ;
		}
		public BasicAnimationBlendingData( BasicTexture texture ) {
			BaseChannel = BasicFCurveChannelType.UVTranslation ;
			InitialValues = new float[ 2 ][] ;
			TotalWeights = new float[ 2 ] ;

			SetValue( BasicFCurveChannelType.UVTranslation, texture.UVTranslation ) ;
			SetValue( BasicFCurveChannelType.UVScaling, texture.UVScaling ) ;
		}

		public float GetBlending( BasicFCurve fcurve, float[] value, float weight, int mode ) {
			if ( mode == 0 ) return 1.0f ;
			int channel = fcurve.ChannelType - BaseChannel ;
			if ( channel < 0 ) return 0.0f ;
			if ( mode > 0 ) {
				float total = TotalWeights[ channel ] + weight ;
				TotalWeights[ channel ] = total ;
				return ( total == 0.0f ) ? 0.0f : weight / total ;
			} else {
				var initial = InitialValues[ channel ] ;
				for ( int i = 0 ; i < initial.Length ; i ++ ) value[ i ] = initial[ i ] ;
				float total = TotalWeights[ channel ] ;
				TotalWeights[ channel ] = 0.0f ;
				return ( total == 0.0f ) ? 0.0f : 1.0f - total ;
			}
		}

		void SetValue( BasicFCurveChannelType channel, Vector3 value ) {
			InitialValues[ channel - BaseChannel ] = new float[]{ value.X, value.Y, value.Z } ;
		}
		void SetValue( BasicFCurveChannelType channel, Vector2 value ) {
			InitialValues[ channel - BaseChannel ] = new float[]{ value.X, value.Y } ;
		}
		void SetValue( BasicFCurveChannelType channel, float value ) {
			InitialValues[ channel - BaseChannel ] = new float[]{ value } ;
		}
		void SetValue( BasicFCurveChannelType channel, Quaternion value ) {
			InitialValues[ channel - BaseChannel ] = new float[]{ value.X, value.Y, value.Z, value.W } ;
		}
	}


} // namespace
