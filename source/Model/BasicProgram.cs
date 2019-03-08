/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.IO ;
using System.Reflection ;

using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace Sce.PlayStation.HighLevel.Model {

	//------------------------------------------------------------
	//  BasicProgram
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>基本プログラムを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a basic program</summary>
	/// @endif
	public class BasicProgram : ShaderProgram, IShallowCloneable {

		/// @if LANG_JA
		/// <summary>プログラムを作成する</summary>
		/// <param name="parameters">パラメータ (nullならば新規作成)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a program</summary>
		/// <param name="parameters">Parameters (creates new when null)</param>
		/// @endif
		public BasicProgram( BasicParameters parameters = null ) : base( loadDefaultProgram() ) {
			BasicParameters.InitProgram( this ) ;
			Parameters = ( parameters != null ) ? parameters : new BasicParameters() ;
		}
		/// @if LANG_JA
		/// <summary>プログラムを作成する (ファイルから)</summary>
		/// <param name="vpFileName">頂点シェーダーのファイル名</param>
		/// <param name="fpFileName">フラグメントシェーダーのファイル名（nullならばvpFileNameを使用）</param>
		/// <param name="parameters">パラメータ (nullならば新規作成)</param>
		/// <remarks>指定されたファイルからシェーダープログラムを作成します。利用可能なファイルフォーマットは CGX です。なお、指定するシェーダーは、HighLevel.Modelのソースコードに含まれる既定のシェーダーと互換性をもち、同じユニフォーム変数やアトリビュート変数が宣言されている必要があります。</remarks>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a program (from a file)</summary>
		/// <param name="vpFileName">Vertex shader filename</param>
		/// <param name="fpFileName">Filename of the fragment shader (vpFileName used when NULL)</param>
		/// <param name="parameters">Parameters (creates new when null)</param>
		/// <remarks>Creates a shader program from a specified file. The usable file format is CGX. The shader to be specified must be compatible to the existing shader included in the HighLevel.Model source code, and have the same uniform and attribute variables declared.</remarks>
		/// @endif
		public BasicProgram( string vpFileName, string fpFileName = null, BasicParameters parameters = null	 ) : base( vpFileName, fpFileName ) {
			BasicParameters.InitProgram( this ) ;
			Parameters = ( parameters != null ) ? parameters : new BasicParameters() ;
		}
		/// @if LANG_JA
		/// <summary>プログラムを複製する</summary>
		/// <param name="program">プログラム</param>
		/// <remarks>プログラムを複製します。複製されたプログラムはアンマネージドリソースを共有します。すべての複製に対して Dispose() が呼び出されたとき、共有されたアンマネージドリソースが解放されます。</remarks>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a copy of a program</summary>
		/// <param name="program">Program</param>
		/// <remarks>Creates a copy of the program. The 2 programs will then share unmanaged resources. When Dispose() is called for all copies, the shared unmanaged resources will be freed.</remarks>
		/// @endif
		protected BasicProgram( BasicProgram program ) : base( program ) {
			Parameters = program.parameters ;
		}
		/// @if LANG_JA
		/// <summary>プログラムを複製する</summary>
		/// <returns>プログラムの複製</returns>
		/// <remarks>プログラムを複製します。複製されたプログラムはアンマネージドリソースを共有します。すべての複製に対して Dispose() が呼び出されたとき、共有されたアンマネージドリソースが解放されます。</remarks>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a copy of a program</summary>
		/// <returns>Program copy</returns>
		/// <remarks>Creates a copy of the program. The 2 programs will then share unmanaged resources. When Dispose() is called for all copies, the shared unmanaged resources will be freed.</remarks>
		/// @endif
		public override Object ShallowClone() { return new BasicProgram( this ) ; }

		/// @if LANG_JA
		/// <summary>プログラムのアンマネージメントリソースを解放する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frees the unmanaged resources of the program</summary>
		/// @endif
		public new void Dispose() {
			Dispose( true ) ;
		}
		protected override void Dispose( bool disposing ) {
			if ( disposing ) {
				parameters = null ;
			}
			base.Dispose( disposing ) ;
		}

		/// @if LANG_JA
		/// <summary>シェーダーの状態を更新する</summary>
		/// <remarks>プリミティブ描画前にグラフィックスコンテキストから呼び出される仮想関数です。この関数はクラス内にキャッシュされた状態をシェーダーに反映する場合に使用します。</remarks>
		/// @endif
		/// @if LANG_EN
		/// <summary>Updates the shader state</summary>
		/// <remarks>A virtual function that is called from the graphics context before primitive rendering. This function is used when a cached state in a class is applied to the shader.</remarks>
		/// @endif
		protected override void UpdateShader() {
			Parameters.UpdateProgram( this ) ;
		}

		/// @if LANG_JA
		/// <summary>パラメータ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Parameters</summary>
		/// @endif
		public BasicParameters Parameters {
			get { return parameters ; }
			set { parameters = value ; parameters.TouchProgram( this ) ; }
		}

		//  load default program

		static byte[] defaultProgram ;
		static byte[] loadDefaultProgram() {
			if ( defaultProgram == null ) {
				Assembly assembly = Assembly.GetExecutingAssembly() ;
				string resname = "Sce.PlayStation.HighLevel.Model.shaders.Basic.cgx" ;
				if ( assembly.GetManifestResourceInfo( resname ) == null ) {
					throw new FileNotFoundException( "Resource not found.", resname ) ;
				}
				Stream stream = assembly.GetManifestResourceStream( resname ) ;
				defaultProgram = new Byte[ stream.Length ] ;
				stream.Read( defaultProgram, 0, defaultProgram.Length ) ;
			}
			return defaultProgram ;
		}

		internal BasicParameters parameters ;
		internal int updateMode ;
		internal int updateView ;
		internal int updateWorld ;
		internal int updateLight ;
		internal int updateMaterial ;
		internal int updateDeformation ;
	}

	//------------------------------------------------------------
	//  BasicParameters
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>基本プログラムのパラメータを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing basic program parameters</summary>
	/// @endif
	public class BasicParameters {

		/// @if LANG_JA
		/// <summary>パラメータを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a parameter</summary>
		/// @endif
		public BasicParameters() {
			;
		}

		//  Enable

		/// @if LANG_JA
		/// <summary>指定されたシェーダー機能を有効または無効にする</summary>
		/// <param name="mode">有効または無効にするシェーダー機能</param>
		/// <param name="status">有効ならばtrue</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Enables or disables the specified shader feature</summary>
		/// <param name="mode">Shader feature to enable or disable</param>
		/// <param name="status">Specify true to enable</param>
		/// @endif
		public void Enable( BasicEnableMode mode, bool status ) {
			BasicEnableMode oldMode = enableMode ;
			enableMode = status ? ( enableMode | mode ) : ( enableMode & ~mode ) ;
			if ( oldMode != enableMode ) updateMode ++ ;
		}
		/// @if LANG_JA
		/// <summary>指定されたシェーダー機能が有効かどうかを取得する</summary>
		/// <param name="mode">有効または無効にするシェーダー機能</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Obtains whether or not the specified shader feature is enabled</summary>
		/// <param name="mode">Shader feature to enable or disable</param>
		/// @endif
		public bool IsEnabled( BasicEnableMode mode ) {
			return ( enableMode & mode ) != 0 ;
		}

		//  Transform

		/// @if LANG_JA
		/// <summary>プロジェクション行列を設定する</summary>
		/// <param name="projection">プロジェクション行列</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the projection matrix</summary>
		/// <param name="projection">Projection matrix</param>
		/// @endif
		public void SetProjectionMatrix( ref Matrix4 projection ) {
			projMatrix = projection ;
			viewProjection = projMatrix * viewMatrix ;
			updateView ++ ;
		}
		/// @if LANG_JA
		/// <summary>ビュー行列を設定する</summary>
		/// <param name="view">ビュー行列</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets a view matrix</summary>
		/// <param name="view">View matrix</param>
		/// @endif
		public void SetViewMatrix( ref Matrix4 view ) {
			viewMatrix = view ;
			viewProjection = projMatrix * viewMatrix ;

			// - viewMatrix.Transpose().TransformVector( view.AxisW )
			eyePosition.X = -( viewMatrix.M11 * viewMatrix.M41 + viewMatrix.M12 * viewMatrix.M42 + viewMatrix.M13 * viewMatrix.M43 ) ;
			eyePosition.Y = -( viewMatrix.M21 * viewMatrix.M41 + viewMatrix.M22 * viewMatrix.M42 + viewMatrix.M23 * viewMatrix.M43 ) ;
			eyePosition.Z = -( viewMatrix.M31 * viewMatrix.M41 + viewMatrix.M32 * viewMatrix.M42 + viewMatrix.M33 * viewMatrix.M43 ) ;

			// ( viewMatrix.RowZ + ( 0, 0, 0, fs ) ) / ( fs - fe )
			float fs = fogRange.X ;
			float fe = fogRange.Y ;
			float fr = 1.0f / ( fs - fe ) ;
			fogVector.X = viewMatrix.M13 * fr ;
			fogVector.Y = viewMatrix.M23 * fr ;
			fogVector.Z = viewMatrix.M33 * fr ;
			fogVector.W = ( viewMatrix.M43 + fs ) * fr ;

			updateView ++ ;
		}
		/// @if LANG_JA
		/// <summary>ワールド行列の数を設定する</summary>
		/// <param name="count">ワールド行列の数 (1～16)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the number of world matrices</summary>
		/// <param name="count">Number of world matrices (1 - 16)</param>
		/// @endif
		public void SetWorldCount( int count ) {
			if ( worldCount != count ) {
				worldCount = count ;
				updateWorld ++ ;
			}
			//  default weight count
			SetVertexWeightCount( count ) ;
		}
		/// @if LANG_JA
		/// <summary>ワールド行列を設定する</summary>
		/// <param name="index">ワールド行列の番号 (0～15)</param>
		/// <param name="world">ワールド行列</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets a world matrix</summary>
		/// <param name="index">World matrix number (0 - 15)</param>
		/// <param name="world">World matrix</param>
		/// @endif
		public void SetWorldMatrix( int index, ref Matrix4 world ) {
			worldMatrix[ index ] = world ;
			updateWorld ++ ;
		}

		//  Lighting

		/// @if LANG_JA
		/// <summary>ライトの数を設定する</summary>
		/// <param name="count">ライトの数 (0～3)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the number of lights</summary>
		/// <param name="count">Number of lights (0 - 3)</param>
		/// @endif
		public void SetLightCount( int count ) {
			if ( lightCount == count ) return ;
			lightCount = count ;
			updateLight ++ ;
		}
		/// @if LANG_JA
		/// <summary>ライトの方向ベクトルを設定する</summary>
		/// <param name="index">ライトの番号 (0～2)</param>
		/// <param name="direction">方向ベクトル</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the light direction vector</summary>
		/// <param name="index">Light number (0 - 2)</param>
		/// <param name="direction">Direction vector</param>
		/// @endif
		public void SetLightDirection( int index, ref Vector3 direction ) {
			lightDirection[ index ] = direction ;
			updateLight ++ ;
		}
		/// @if LANG_JA
		/// <summary>ライトのディフューズカラーを設定する</summary>
		/// <param name="index">ライトの番号 (0～2)</param>
		/// <param name="diffuse">ディフューズカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the light diffuse color</summary>
		/// <param name="index">Light number (0 - 2)</param>
		/// <param name="diffuse">Diffuse color</param>
		/// @endif
		public void SetLightDiffuse( int index, ref Vector3 diffuse ) {
			lightDiffuse[ index ] = diffuse ;
			updateLight ++ ;
		}
		/// @if LANG_JA
		/// <summary>ライトのスペキュラカラーを設定する</summary>
		/// <param name="index">ライトの番号 (0～2)</param>
		/// <param name="specular">スペキュラカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the light specular color</summary>
		/// <param name="index">Light number (0 - 2)</param>
		/// <param name="specular">Specular color</param>
		/// @endif
		public void SetLightSpecular( int index, ref Vector3 specular ) {
			lightSpecular[ index ] = specular ;
			updateLight ++ ;
		}
		/// @if LANG_JA
		/// <summary>ライトのアンビエントカラーを設定する</summary>
		/// <param name="ambient">アンビエントカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the light ambient color</summary>
		/// <param name="ambient">Ambient color</param>
		/// @endif
		public void SetLightAmbient( ref Vector3 ambient ) {
			lightAmbient = ambient ;
			updateLight ++ ;
		}

		//  Fog

		/// @if LANG_JA
		/// <summary>フォグの範囲を設定する</summary>
		/// <param name="start">開始距離</param>
		/// <param name="end">終了距離</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the fog range</summary>
		/// <param name="start">Start distance</param>
		/// <param name="end">End distance</param>
		/// @endif
		public void SetFogRange( float start, float end ) {
			fogRange.X = start ;
			fogRange.Y = end ;

			// ( viewMatrix.RowZ + ( 0, 0, 0, fs ) ) / ( fs - fe )
			float fs = fogRange.X ;
			float fe = fogRange.Y ;
			float fr = 1.0f / ( fs - fe ) ;
			fogVector.X = viewMatrix.M13 * fr ;
			fogVector.Y = viewMatrix.M23 * fr ;
			fogVector.Z = viewMatrix.M33 * fr ;
			fogVector.W = ( viewMatrix.M43 + fs ) * fr ;

			updateView ++ ;
		}
		/// @if LANG_JA
		/// <summary>フォグのカラーを設定する</summary>
		/// <param name="color">カラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the fog color</summary>
		/// <param name="color">Color</param>
		/// @endif
		public void SetFogColor( ref Vector3 color ) {
			fogColor = color ;
			updateView ++ ;
		}

		//  Material

		/// @if LANG_JA
		/// <summary>マテリアルのディフューズカラーを設定する</summary>
		/// <param name="diffuse">ディフューズカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material diffuse color</summary>
		/// <param name="diffuse">Diffuse color</param>
		/// @endif
		public void SetMaterialDiffuse( ref Vector3 diffuse ) {
			materialDiffuse = diffuse ;
			updateMaterial ++ ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルのスペキュラーカラーを設定する</summary>
		/// <param name="specular">スペキュラーカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material specular color</summary>
		/// <param name="specular">Specular color</param>
		/// @endif
		public void SetMaterialSpecular( ref Vector3 specular ) {
			materialSpecular = specular ;
			updateMaterial ++ ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルのアンビエントカラーを設定する</summary>
		/// <param name="ambient">アンビエントカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material ambient color</summary>
		/// <param name="ambient">Ambient color</param>
		/// @endif
		public void SetMaterialAmbient( ref Vector3 ambient ) {
			materialAmbient = ambient ;
			updateMaterial ++ ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルのエミッションカラーを設定する</summary>
		/// <param name="emission">エミッションカラー</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material emission color</summary>
		/// <param name="emission">Emission color</param>
		/// @endif
		public void SetMaterialEmission( ref Vector3 emission ) {
			materialEmission = emission ;
			updateMaterial ++ ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルの不透明度を設定する</summary>
		/// <param name="opacity">不透明度</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material opacity</summary>
		/// <param name="opacity">Opacity</param>
		/// @endif
		public void SetMaterialOpacity( float opacity ) {
			materialFactor.X = opacity ;
			updateMaterial ++ ;
		}
		/// @if LANG_JA
		/// <summary>マテリアルの輝度を設定する</summary>
		/// <param name="shininess">輝度</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the material luminance</summary>
		/// <param name="shininess">Luminance</param>
		/// @endif
		public void SetMaterialShininess( float shininess ) {
			materialFactor.Y = shininess ;
			updateMaterial ++ ;
		}

		/// @if LANG_JA
		/// <summary>頂点ポジション座標のオフセットを設定する</summary>
		/// <param name="transX">X 移動量</param>
		/// <param name="transY">Y 移動量</param>
		/// <param name="transZ">Z 移動量</param>
		/// <param name="scale">スケーリング</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Set offset of vertex position coordinates</summary>
		/// <param name="transX">Translation of X</param>
		/// <param name="transY">Translation of Y</param>
		/// <param name="transZ">Translation of Z</param>
		/// <param name="scale">Scaling</param>
		/// @endif
		public void SetPositionOffset( float transX, float transY, float transZ, float scale ) {
			vertexOffsets[ 0 ].X = transX ;
			vertexOffsets[ 0 ].Y = transY ;
			vertexOffsets[ 0 ].Z = transZ ;
			vertexOffsets[ 0 ].W = scale ;
			updateDeformation ++ ;
		}
		/// @if LANG_JA
		/// <summary>頂点テクスチャ座標のオフセットを設定する</summary>
		/// <param name="index">テクスチャ座標の番号 (つねに 0)</param>
		/// <param name="transU">U 移動量</param>
		/// <param name="transV">V 移動量</param>
		/// <param name="scaleU">U スケーリング</param>
		/// <param name="scaleV">V スケーリング</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Set offset of vertex texture coordinates</summary>
		/// <param name="index">Number of the texture coordinate (always 0)</param>
		/// <param name="transU">Translation of U</param>
		/// <param name="transV">Translation of V</param>
		/// <param name="scaleU">U scaling</param>
		/// <param name="scaleV">V scaling</param>
		/// @endif
		public void SetTexCoordOffset( int index, float transU, float transV, float scaleU, float scaleV ) {
			if ( index >= 0 ) index = 1 + index ;
			vertexOffsets[ index ].X = transU ;
			vertexOffsets[ index ].Y = transV ;
			vertexOffsets[ index ].Z = scaleU ;
			vertexOffsets[ index ].W = scaleV ;
			updateDeformation ++ ;
		}
		/// @if LANG_JA
		/// <summary>頂点ウェイトの数を設定する</summary>
		/// <param name="count">頂点ウェイトの数 (0～4)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Sets the number of vertex weights</summary>
		/// <param name="count">Number of vertex weights (0 - 4)</param>
		/// @endif
		public void SetVertexWeightCount( int count ) {
			if ( vertexWeightCount == count ) return ;
			vertexWeightCount = count ;
			updateDeformation ++ ;
		}

		internal static void InitProgram( BasicProgram program ) {
			program.SetUniformBinding( idEnableLighting, "EnableLighting" ) ;
			program.SetUniformBinding( idEnableFog, "EnableFog" ) ;
			program.SetUniformBinding( idLightCount, "LightCount" ) ;
			program.SetUniformBinding( idViewProjection, "ViewProjection" ) ;
			program.SetUniformBinding( idEyePosition, "EyePosition" ) ;
			program.SetUniformBinding( idFogVector, "FogVector" ) ;
			program.SetUniformBinding( idFogColor, "FogColor" ) ;
			program.SetUniformBinding( idWorldMatrix, "WorldMatrix" ) ;
			program.SetUniformBinding( idLightDirection, "LightDirection" ) ;
			program.SetUniformBinding( idLightDiffuse, "LightDiffuse" ) ;
			program.SetUniformBinding( idLightSpecular, "LightSpecular" ) ;
			program.SetUniformBinding( idLightAmbient, "LightAmbient" ) ;
			program.SetUniformBinding( idMaterialDiffuse, "MaterialDiffuse" ) ;
			program.SetUniformBinding( idMaterialSpecular, "MaterialSpecular" ) ;
			program.SetUniformBinding( idMaterialAmbient, "MaterialAmbient" ) ;
			program.SetUniformBinding( idMaterialEmission, "MaterialEmission" ) ;
			program.SetUniformBinding( idMaterialFactor, "MaterialFactor" ) ;
			program.SetUniformBinding( idVertexOffsets, "VertexOffsets" ) ;
			program.SetUniformBinding( idVertexWeightCount, "VertexWeightCount" ) ;

			program.SetAttributeBinding( idAttribPosition, "a_Position" ) ;
			program.SetAttributeBinding( idAttribNormal, "a_Normal" ) ;
			program.SetAttributeBinding( idAttribColor, "a_Color" ) ;
			program.SetAttributeBinding( idAttribTexCoord, "a_TexCoord" ) ;
			program.SetAttributeBinding( idAttribWeight, "a_Weight" ) ;
			program.SetAttributeBinding( idAttribIndices, "a_Indices" ) ;

			program.SetAttributeValue( idAttribColor, 1.0f, 1.0f, 1.0f, 1.0f ) ;
			program.SetAttributeValue( idAttribWeight, 1.0f, 0.0f, 0.0f, 0.0f ) ;
			program.SetAttributeValue( idAttribIndices, 0.0f, 1.0f, 2.0f, 3.0f ) ;
		}
		internal void TouchProgram( BasicProgram program ) {
			program.updateMode = updateMode - 1 ;
			program.updateView = updateView - 1 ;
			program.updateWorld = updateWorld - 1 ;
			program.updateLight = updateLight - 1 ;
			program.updateMaterial = updateMaterial - 1 ;
			program.updateDeformation = updateDeformation - 1 ;
		}
		internal void UpdateProgram( BasicProgram program ) {
			if ( program.updateMode != updateMode ) { 
				program.updateMode = updateMode ;
				program.SetUniformValue( idEnableLighting, ( enableMode & BasicEnableMode.Lighting ) == 0 ? 0 : 1 ) ;
				program.SetUniformValue( idEnableFog, ( enableMode & BasicEnableMode.Fog ) == 0 ? 0 : 1 ) ;
			}
			if ( program.updateView != updateView ) {
				program.updateView = updateView ;
				program.SetUniformValue( idViewProjection, ref viewProjection ) ;
				program.SetUniformValue( idEyePosition, ref eyePosition ) ;
				program.SetUniformValue( idFogVector, ref fogVector ) ;
				program.SetUniformValue( idFogColor, ref fogColor ) ;
			}
			if ( program.updateWorld != updateWorld ) {
				program.updateWorld = updateWorld ;
				program.SetUniformValue( idWorldMatrix, worldMatrix, 0, 0, worldCount ) ;
			}
			if ( program.updateLight != updateLight ) {
				program.updateLight = updateLight ;
				program.SetUniformValue( idLightCount, lightCount ) ;
				program.SetUniformValue( idLightDirection, lightDirection, 0, 0, lightCount ) ;
				program.SetUniformValue( idLightDiffuse, lightDiffuse, 0, 0, lightCount ) ;
				program.SetUniformValue( idLightSpecular, lightSpecular, 0, 0, lightCount ) ;
				program.SetUniformValue( idLightAmbient, ref lightAmbient ) ;
			}
			if ( program.updateMaterial != updateMaterial ) {
				program.updateMaterial = updateMaterial ;
				program.SetUniformValue( idMaterialDiffuse, ref materialDiffuse ) ;
				program.SetUniformValue( idMaterialSpecular, ref materialSpecular ) ;
				program.SetUniformValue( idMaterialAmbient, ref materialAmbient ) ;
				program.SetUniformValue( idMaterialEmission, ref materialEmission ) ;
				program.SetUniformValue( idMaterialFactor, ref materialFactor ) ;
			}
			if ( program.updateDeformation != updateDeformation ) {
				program.SetUniformValue( idVertexOffsets, vertexOffsets, 0, 0, 2 ) ;
				program.SetUniformValue( idVertexWeightCount, vertexWeightCount ) ;
			}
		}

		//  Const

		const int idEnableLighting = 0 ;
		const int idEnableFog = 1 ;
		const int idLightCount = 2 ;
		const int idViewProjection = 3 ;
		const int idEyePosition = 4 ;
		const int idFogVector = 5 ;
		const int idFogColor = 6 ;
		const int idWorldMatrix = 7 ;
		const int idLightDirection = 8 ;
		const int idLightDiffuse = 9 ;
		const int idLightSpecular = 10 ;
		const int idLightAmbient = 11 ;
		const int idMaterialDiffuse = 12 ;
		const int idMaterialSpecular = 13 ;
		const int idMaterialAmbient = 14 ;
		const int idMaterialEmission = 15 ;
		const int idMaterialFactor = 16 ;
		const int idVertexOffsets = 17 ;
		const int idVertexWeightCount = 18 ;

		const int idAttribPosition = 0 ;
		const int idAttribNormal = 1 ;
		const int idAttribColor = 2 ;
		const int idAttribTexCoord = 3 ;
		const int idAttribWeight = 4 ;
		const int idAttribIndices = 5 ;

		//  Status

		int updateMode ;
		int updateView ;
		int updateWorld ;
		int updateLight ;
		int updateMaterial ;
		int updateDeformation ;

		BasicEnableMode enableMode ;

		Matrix4 projMatrix = Matrix4.Identity ;
		Matrix4 viewMatrix = Matrix4.Identity ;
		Matrix4 viewProjection = Matrix4.Identity ;
		Vector3 eyePosition = Vector3.Zero ;
		Vector2 fogRange = new Vector2( 1000000.0f, 1000001.0f ) ;
		Vector3 fogColor = Vector3.Zero ;
		Vector4 fogVector = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ) ;

		int worldCount = 1 ;
		Matrix4[] worldMatrix = new Matrix4[ 16 ] ;

		int lightCount = 0 ;
		Vector3[] lightDirection = new Vector3[ 3 ] ;
		Vector3[] lightDiffuse = new Vector3[ 3 ] ;
		Vector3[] lightSpecular = new Vector3[ 3 ] ;
		Vector3 lightAmbient = Vector3.Zero ;

		Vector3 materialDiffuse = Vector3.One ;
		Vector3 materialSpecular = Vector3.Zero ;
		Vector3 materialAmbient = Vector3.One ;
		Vector3 materialEmission = Vector3.Zero ;
		Vector3 materialFactor = Vector3.One ;

		Vector4[] vertexOffsets = new Vector4[] {
			new Vector4( 0.0f, 0.0f, 0.0f, 1.0f ),
			new Vector4( 0.0f, 0.0f, 1.0f, 1.0f )
		} ;
		int vertexWeightCount = 1 ;
	}

	//------------------------------------------------------------
	//  Public Enums
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>有効または無効にするシェーダー機能</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Shader feature to enable or disable</summary>
	/// @endif
	[Flags]
	public enum BasicEnableMode : uint {
		/// @if LANG_JA
		/// <summary>ライティング</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Lighting</summary>
		/// @endif
		Lighting					= 0x00000001,
		/// @if LANG_JA
		/// <summary>フォグ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Fog</summary>
		/// @endif
		Fog							= 0x00000002
	} ;

} // namespace
