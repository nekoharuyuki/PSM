/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// SpriteRenderer wraps batch rendering of sprites in a simple BeginSprites / AddSprite x N / EndSprite API.
	/// It also provides some text rendering functions that uses FontMap.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>SpriteRendererは、単純なBeginSprites / AddSprite X N / EndSprite APIでのスプライトの一括描画をラップします。
	/// また、FontMap を使ったテキスト描画関数を提供します。
	/// </summary>
	/// @endif
	public class SpriteRenderer : System.IDisposable
	{
		GraphicsContextAlpha GL;
		ImmediateModeQuads< Vector4 > m_imm_quads; // V2F_T2F 
		Vector4 m_v0;
		Vector4 m_v1;
		Vector4 m_v2;
		Vector4 m_v3;
		TextureInfo m_current_texture_info;
		TextureInfo m_embedded_font_texture_info; // used by DrawTextDebug

		/// @if LANG_EN
		/// <summary>
		/// Flag that will swap the U coordinates (horizontally) of all rendered sprites/quads.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画される全てのスプライト/クワッドのU座標を水平に反転するフラグ。
		/// </summary>
		/// @endif
		public bool FlipU = false;

		/// @if LANG_EN
		/// <summary>
		/// Flag that will swap the V coordinates (vertically) of all rendered sprites/quads.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画される全てのスプライト/クワッドのV座標を垂直に反転するフラグ。
		/// </summary>
		/// @endif
		public bool FlipV = false;

		/// @if LANG_EN
		/// <summary>
		/// That's all the interface we require from the shaders set by user.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>開発者によって設定されるシェーダープログラムのインターフェース。
		/// </summary>
		/// @endif
		public interface ISpriteShader
		{
			/// @if LANG_EN
			/// <summary>
			/// The Projection * View * Model matrix.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>プロジェクション行列 * ビュー行列 * モデル行列。
			/// </summary>
			/// @endif
			void SetMVP( ref Matrix4 value );

			/// @if LANG_EN
			/// <summary>
			/// Global color.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>グローバルカラー。
			/// </summary>
			/// @endif
			void SetColor( ref Vector4 value );
			
			/// @if LANG_EN
			/// <summary>
			/// Set the uv transform: offset in Xy and scale in Zw, (0,0,1,1) means UV is unchanged.
			/// Shader code example: transformed_uv = UVTransform.xy + uv * UVTransform.zw
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>UVの変換を設定します。Xy内のオフセット、Zwのスケール、(0,0,1,1)はUVが変更されないことを意味します。
			/// シェーダーコードの例：transformed_uv = UVTransform.xy + UV * UVTransform.zw
			/// </summary>
			/// @endif
			void SetUVTransform( ref Vector4 value );

			/// @if LANG_EN
			/// <summary>
			/// Get the ShaderProgram object.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>ShaderProgram オブジェクトを取得します。
			/// </summary>
			/// @endif
			ShaderProgram GetShaderProgram();
		}

		/// @if LANG_EN
		/// <summary>
		/// Sprites's default shader: texture modulated by a color.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトのデフォルトのシェーダー：色によって変調されたテクスチャ。
		/// </summary>
		/// @endif
		public class DefaultShader_ : ISpriteShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public DefaultShader_()
			{
				m_shader_program = Common.CreateShaderProgram( "cg/sprite.cgx" );
				
				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
				m_shader_program.SetUniformBinding( 2, "UVTransform" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
				SetUVTransform( ref Math.UV_TransformFlipV );
			}

			/// @if LANG_EN
			/// <summary>
			/// Dispose implementation.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Disposeの実装。
			/// </summary>
			/// @endif
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{
					Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				}
			}

			public ShaderProgram GetShaderProgram() { return m_shader_program;}
			public void SetMVP( ref Matrix4 value ) { m_shader_program.SetUniformValue( 0, ref value );}
			public void SetColor( ref Vector4 value ) { m_shader_program.SetUniformValue( 1, ref value );}
			public void SetUVTransform( ref Vector4 value ) { m_shader_program.SetUniformValue( 2, ref value );}
		}

		DefaultShader_ m_default_shader = new DefaultShader_();
		/// @if LANG_EN
		/// <summary>The default shader used by SpriteRenderer.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SpriteRenderer によって使用されるデフォルトのシェーダー。
		/// </summary>
		/// @endif
		public DefaultShader_ DefaultShader { get { return m_default_shader; } }

		/// @if LANG_EN
		/// <summary>
		/// Font's default shader.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Fontのデフォルトのシェーダー。
		/// </summary>
		/// @endif
		public class DefaultFontShader_ : ISpriteShader, System.IDisposable
		{
			public ShaderProgram m_shader_program;

			public DefaultFontShader_()
			{
				m_shader_program = Common.CreateShaderProgram("cg/font.cgx");

				m_shader_program.SetUniformBinding( 0, "MVP" );
				m_shader_program.SetUniformBinding( 1, "Color" );
				m_shader_program.SetUniformBinding( 2, "UVTransform" );

				m_shader_program.SetAttributeBinding( 0, "vin_data" );

				Matrix4 identity = Matrix4.Identity;
				SetMVP( ref identity );
				SetColor( ref Colors.White );
				SetUVTransform( ref Math.UV_TransformFlipV );
			}

			/// @if LANG_EN
			/// <summary>
			/// Dispose implementation.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>Disposeの実装。
			/// </summary>
			/// @endif
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if(disposing)
				{
					Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				}
			}

			public ShaderProgram GetShaderProgram() { return m_shader_program;}
			public void SetMVP( ref Matrix4 value ) { m_shader_program.SetUniformValue( 0, ref value );}
			public void SetColor( ref Vector4 value ) { m_shader_program.SetUniformValue( 1, ref value );}
			public void SetUVTransform( ref Vector4 value ) { m_shader_program.SetUniformValue( 2, ref value );}
		}

		DefaultFontShader_ m_default_font_shader = new DefaultFontShader_();
		/// @if LANG_EN
		/// <summary>The default font shader used by SpriteRenderer.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SpriteRenderer によって使用される、Fontのためのデフォルトのシェーダー。
		/// </summary>
		/// @endif
		public DefaultFontShader_ DefaultFontShader { get { return m_default_font_shader; } }

		bool m_disposed = false;

		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>オブジェクトが破棄されている場合、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		/// @if LANG_EN
		/// <summary>SpriteRenderer constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public SpriteRenderer( GraphicsContextAlpha gl, uint max_sprites )
		{
			GL = gl;

			m_imm_quads = new ImmediateModeQuads< Vector4 >( GL, (uint)max_sprites, VertexFormat.Float4 );
		
			{
				// init the font texture used by DrawTextDebug

				Texture2D font_texture = EmbeddedDebugFontData.CreateTexture();

				m_embedded_font_texture_info = new TextureInfo();

				m_embedded_font_texture_info.Initialize( font_texture, new Vector2i( EmbeddedDebugFontData.NumChars, 1 )
										  , new TRS( new Bounds2( new Vector2( 0.0f, 0.0f )
																  , new Vector2( ( EmbeddedDebugFontData.CharSizei.X * EmbeddedDebugFontData.NumChars ) / (float)font_texture.Width
																				 , ( EmbeddedDebugFontData.CharSizei.Y / (float)font_texture.Height ) ) ) ) );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Dispose の実装。
		/// </summary>
		/// @endif
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				m_imm_quads.Dispose();
				m_embedded_font_texture_info.Dispose();

				Common.DisposeAndNullify< DefaultFontShader_ >( ref m_default_font_shader );
				Common.DisposeAndNullify< DefaultShader_ >( ref m_default_shader );
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Debug text draw function, using a small embedded font suitable for on screen debug prints.
		/// Since DrawTextDebug uses very small font data, it only work with ascii text (characters
		/// from ' ' to '~', 32 to 126) Any character outside this range will be displayed as '?'.
		/// For instance "こんにちは" will be displayed as "?????".
		/// </summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="shader">If no shader is specified, DefaultFontShader is used.</param>
		/// <returns>The rectangle area covered by rendered text (call with draw=false when you want to know the covered area before actually drawing it).</returns>
		/// @endif
		/// @if LANG_JA
		/// <summary>DrawDebugTextは、埋め込まれた小さなフォントを使って、スクリーン上に文字を表示します。
		/// DrawDebugTextは簡易的なフォントデータであるため、アスキー文字にしか対応していません ( ' '(32) から '~'(126)までの文字)。
		/// アスキー文字以外の文字は'?'で表示されます。例えば「こんにちは」は "?????" と表示されます。
		/// </summary>
		/// <param name="str">描画するテキスト。</param>
		/// <param name="bottom_left_start_pos">ワールド空間/ユニット内での、テキスト矩形の左下の点。</param>
		/// <param name="char_height">ワールド空間/ユニット内での文字の高さ。</param>
		/// <param name="draw">falseを指定した場合、何も描画せず、テキストによって使用される Bound2 を返します。</param>
		/// <param name="shader">シェーダーを指定しない場合、DefaultFontShader が使用されます。</param>
		/// <returns>描画されたテキストでカバーされた矩形領域。実際に描画する前に矩形領域を知りたい場合、draw=trueで呼び出してください。</returns>
		/// @endif
		public Bounds2 DrawTextDebug( string str, Vector2 bottom_left_start_pos, float char_height
									  , bool draw = true
									  , ISpriteShader shader = null )
		{
			if ( null == shader ) shader = (ISpriteShader)m_default_font_shader;

			float scale = ( char_height / EmbeddedDebugFontData.CharSizei.Y );

			Vector2 spacing_in_pixels = new Vector2( -1.0f, 1.0f );
			Vector2 spacing = spacing_in_pixels * scale;

			TRS char_box; // 1 character sprite bounding box in world space, could use Bouds2 instead really 
						  // R and S stay unchaged, T gets incrementd as we draw chars

			char_box.R = Math._10;
			char_box.S = EmbeddedDebugFontData.CharSizef * scale;
			char_box.T = bottom_left_start_pos;

			Vector2 max = bottom_left_start_pos;
			float left = char_box.T.X;

			if ( draw )
			{
				shader.SetUVTransform( ref Math.UV_TransformFlipV );
				BeginSprites( m_embedded_font_texture_info, shader, str.Length );
			}

			for ( int i=0; i < str.Length; ++i )
			{
				if ( str[i] == '\n' )
				{
					char_box.T -= new Vector2( 0.0f, char_box.S.Y + spacing.Y );
					char_box.T.X = left;
					continue;
				}

				if ( draw )
				{
					int char_index = (int)str[i] - 32;

					if ( char_index < 0 || char_index >= EmbeddedDebugFontData.NumChars )
						char_index = (int)'?' - 32;

					AddSprite( ref char_box, new Vector2i( char_index, 0 ) );
				}

				char_box.T += new Vector2( char_box.S.X + spacing.X, 0.0f );

				max.X = FMath.Max( char_box.T.X, max.X );
				max.Y = FMath.Min( char_box.T.Y, max.Y );
			}

			if ( draw )
				EndSprites();

			return Bounds2.SafeBounds( bottom_left_start_pos + new Vector2( 0.0f, char_box.S.Y ), max );
		}

		/// @if LANG_EN
		/// <summary>This text draw function uses a FontMap object.</summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="fontmap">the fontmap object (that holds the texture).</param>
		/// <param name="shader">The shader defaults to SpriteRenderer.DefaultFontShader.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>FontMapオブジェクトを使ったテキスト描画関数。
		/// </summary>
		/// <param name="str">描画するテキスト。</param>
		/// <param name="bottom_left_start_pos">ワールド空間/units内でのテキスト矩形の左下の点。</param>
		/// <param name="char_height">ワールド空間/units内での文字の高さ。</param>
		/// <param name="draw">falseを指定した場合、何も描画せず、テキストによって使用された Bound2 を返します。</param>
		/// <param name="fontmap">フォントマップのオブジェクト (テクスチャを保持します)。</param>
		/// <param name="shader">SpriteRenderer.DefaultFontShader のデフォルトシェーダー。</param>
		/// @endif
		public Bounds2 DrawTextWithFontMap( string str, Vector2 bottom_left_start_pos, float char_height
											, bool draw, FontMap fontmap, ISpriteShader shader )
		{
			float scale = ( char_height / fontmap.CharPixelHeight );

			Vector2 spacing_in_pixels = new Vector2( 1.0f, 1.0f );
			Vector2 spacing = spacing_in_pixels * scale;

			Vector2 turtle = bottom_left_start_pos;

			Vector2 max = bottom_left_start_pos;
			float left = bottom_left_start_pos.X;

			if ( draw )
			{
				shader.SetUVTransform( ref Math.UV_TransformFlipV );
				BeginSprites( new TextureInfo( fontmap.Texture ), shader, str.Length );
			}

			for ( int i=0; i < str.Length; ++i )
			{
				if ( str[i] == '\n' )
				{
					turtle -= new Vector2( 0.0f, char_height + spacing.Y );
					turtle.X = left;
					continue;
				}

				FontMap.CharData cdata;
				if ( !fontmap.TryGetCharData( str[i], out cdata ) )
					continue;

				Vector2 scaled_char_size = cdata.PixelSize * scale;

				if ( draw )
					AddSprite( turtle, new Vector2(scaled_char_size.X,0.0f), cdata.UV );

				turtle += new Vector2( scaled_char_size.X + spacing.X, 0.0f );

				max.X = FMath.Max( turtle.X, max.X );
				max.Y = FMath.Min( turtle.Y, max.Y );
			}

			if ( draw )
				EndSprites();

			return Bounds2.SafeBounds( bottom_left_start_pos + new Vector2( 0.0f, char_height ), max );
		}

		/// @if LANG_EN
		/// <summary>This text draw function uses a FontMap object and SpriteRenderer's DefaultShader.</summary>
		/// <param name="str">The text to draw.</param>
		/// <param name="bottom_left_start_pos">The bottom left of the text rectangle, in world space/units.</param>
		/// <param name="char_height">The character height in world space/units.</param>
		/// <param name="draw">If false, don't draw anything, just return the Bounds2 used by the text.</param>
		/// <param name="fontmap">the fontmap object (that holds the texture).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>このテキスト描画関数は FontMap オブジェクトと SpriteRenderer の DefaultShader を使用します。
		/// </summary>
		/// <param name="str">描画するテキスト。</param>
		/// <param name="bottom_left_start_pos">ワールド空間/ユニット内での、テキスト矩形の左下。</param>
		/// <param name="char_height">ワールド空間/ユニット内での文字の高さ。</param>
		/// <param name="draw">falseを指定した場合、何も描画せず、テキストによって使用された Bound2 を返します。</param>
		/// <param name="fontmap">フォントマップのオブジェクト (テクスチャを保持します)。</param>
		/// @endif
		public Bounds2 DrawTextWithFontMap( string str, Vector2 bottom_left_start_pos, float char_height
											, bool draw, FontMap fontmap )
		{
			return DrawTextWithFontMap( str, bottom_left_start_pos, char_height, draw, fontmap
										, (ISpriteShader)DefaultFontShader );
		}

		/// @if LANG_EN
		/// <summary>Start batch rendering of sprites.</summary>
		/// <param name="texture_info">The texture object.</param>
		/// <param name="shader">The shader object.</param>
		/// <param name="num_sprites">The maximum number of sprite you intend to draw.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画を開始します。
		/// </summary>
		/// <param name="texture_info">テクスチャオブジェクト。</param>
		/// <param name="shader">シェーダーオブジェクト。</param>
		/// <param name="num_sprites">描画するスプライトの最大数。</param>
		/// @endif
		public void BeginSprites( TextureInfo texture_info, ISpriteShader shader, int num_sprites )
		{
			////Common.Profiler.Push("SpriteRenderer.BeginSprites");

			Matrix4 mvp = GL.GetMVP();
			shader.SetMVP( ref mvp );

			GL.Context.SetShaderProgram( shader.GetShaderProgram() );

			Common.Assert( !texture_info.Disposed, "This TextureInfo oject has been disposed" );
			GL.Context.SetTexture( 0, texture_info.Texture );

			m_current_texture_info = texture_info;

			m_imm_quads.ImmBeginQuads( (uint)num_sprites );

			////Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>Start batch rendering of sprites.</summary>
		/// <param name="texture_info">The texture object.</param>
		/// <param name="num_sprites">The maximum number of sprite you intend to draw.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画を開始します。
		/// </summary>
		/// <param name="texture_info">テクスチャオブジェクト。</param>
		/// <param name="num_sprites">描画するスプライトの最大数。</param>
		/// @endif
		public void BeginSprites( TextureInfo texture_info, int num_sprites  )
		{
			BeginSprites( texture_info, (ISpriteShader)DefaultShader, num_sprites );
		}

		/// @if LANG_EN
		/// <summary>
		/// End batch rendering of sprites.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画を終了します。
		/// </summary>
		/// @endif
		public void EndSprites()
		{
			m_imm_quads.ImmEndQuads();

//			GL.Context.SetShaderProgram( null );

			m_current_texture_info = null;
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="tile_index">Sprite UVs are specified by a tile index.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// </summary>
		/// <param name="quad">スプライトの幾何情報。</param>
		/// <param name="tile_index">タイルインデックスで指定する、スプライトのUV。</param>
		/// @endif
		public void AddSprite( ref TRS quad, Vector2i tile_index )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			TextureInfo.CachedTileData uvs = m_current_texture_info.GetCachedTiledData( ref tile_index );

			m_v0 = new Vector4( quad.T               , uvs.UV_00 );
			m_v1 = new Vector4( quad.T + posX        , uvs.UV_10 );
			m_v2 = new Vector4( quad.T + posY        , uvs.UV_01 );
			m_v3 = new Vector4( quad.T + posX + posY , uvs.UV_11 );

			add_quad();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="tile_index">Sprite UVs are specified by a tile index.</param>
		/// <param name="mat">A per sprite transform matrix.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// </summary>
		/// <param name="quad">スプライトの幾何情報。</param>
		/// <param name="tile_index">タイルインデックスで指定する、スプライトのUV。</param>
		/// <param name="mat">スプライトごとの変換行列。</param>
		/// @endif
		public void AddSprite( ref TRS quad, Vector2i tile_index, ref Matrix3 mat )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			TextureInfo.CachedTileData uvs = m_current_texture_info.GetCachedTiledData( ref tile_index );

			m_v0 = new Vector4( transform_point( ref mat, quad.T               ), uvs.UV_00 );
			m_v1 = new Vector4( transform_point( ref mat, quad.T + posX        ), uvs.UV_10 );
			m_v2 = new Vector4( transform_point( ref mat, quad.T + posY        ), uvs.UV_01 );
			m_v3 = new Vector4( transform_point( ref mat, quad.T + posX + posY ), uvs.UV_11 );

			add_quad();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="uv">Sprite UVs are specified directly using a TRS object.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// </summary>
		/// <param name="quad">スプライトの幾何情報。</param>
		/// <param name="uv">TRSオブジェクトで指定するスプライトのUV。</param>
		/// @endif
		public void AddSprite( ref TRS quad, ref TRS uv )
		{
			Vector2 posX = quad.X;
			Vector2 posY = quad.Y;

			Vector2 uvX = uv.X;   
			Vector2 uvY = uv.Y;

			m_v0 = new Vector4( quad.T               , uv.T             );
			m_v1 = new Vector4( quad.T + posX        , uv.T + uvX       );
			m_v2 = new Vector4( quad.T + posY        , uv.T + uvY       );
			m_v3 = new Vector4( quad.T + posX + posY , uv.T + uvX + uvY );

			add_quad();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// </summary>
		/// <param name="quad">The sprite geometry.</param>
		/// <param name="uv">Sprite UVs are specified directly using a TRS object.</param>
		/// <param name="mat">A per sprite transform matrix.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// </summary>
		/// <param name="quad">スプライトの幾何情報。</param>
		/// <param name="uv">TRSオブジェクトで指定するスプライトのUV。</param>
		/// <param name="mat">スプライトごとの変換行列。</param>
		/// @endif
		public void AddSprite( ref TRS quad, ref TRS uv, ref Matrix3 mat )
		{
			Vector2 quadX = quad.X;
			Vector2 quadY = quad.Y;

			Vector2 uvX = uv.X;   
			Vector2 uvY = uv.Y;

			m_v0 = new Vector4( transform_point( ref mat, quad.T               ), uv.T             );
			m_v1 = new Vector4( transform_point( ref mat, quad.T + quadX        ), uv.T + uvX       );
			m_v2 = new Vector4( transform_point( ref mat, quad.T + quadY        ), uv.T + uvY       );
			m_v3 = new Vector4( transform_point( ref mat, quad.T + quadX + quadY ), uv.T + uvX + uvY );

			add_quad();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// One vector is enough to determine the orientation and scale of the sprite. The aspect ratio is
		/// by default the same was the size of the 'uv' domain covered (in texels).
		/// </summary>
		/// <param name="x">The len and direction of the bottom edge of the sprite.</param>
		/// <param name="bottom_left_start_pos">The bottom left point of the sprite.</param>
		/// <param name="uv_bounds">The uv bounds (Bounds2 in uv domain).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// ひとつのベクトルでスプライトの向きとスケールを決定します。
		/// </summary>
		/// <param name="x">スプライトの下端の長さと方向。</param>
		/// <param name="bottom_left_start_pos">スプライトの左下の点。</param>
		/// <param name="uv_bounds">UVの境界 ( UV領域内のBounds2)。</param>
		/// @endif
		public void AddSprite( Vector2 bottom_left_start_pos, Vector2 x, Bounds2 uv_bounds )
		{
			Vector2 ssize = uv_bounds.Size.Abs() * m_current_texture_info.TextureSizef;	// sprite size in texel units - if bounds is invalid, use uv_bounds.Size.Abs()

			Vector2 y = Math.Perp( x ) * ssize.Y / ssize.X;

			m_v0 = new Vector4( bottom_left_start_pos         , uv_bounds.Point00 );
			m_v1 = new Vector4( bottom_left_start_pos + x     , uv_bounds.Point10 );
			m_v2 = new Vector4( bottom_left_start_pos + y     , uv_bounds.Point01 );
			m_v3 = new Vector4( bottom_left_start_pos + x + y , uv_bounds.Point11 );

			add_quad();
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a sprite to batch rendering of sprites, must be called between BeginSprites and EndSprites.
		/// In this version user specify 4 vertices as Vector4, where each Vector4's xy is the position of 
		/// the vertex, and zw is the UV.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの一括描画処理にスプライトを追加します。 BeginSprites と EndSprites の間で呼び出してください。
		/// このバージョンでは、開発者は Vector4 で4つの頂点を指定します。Vector4の xy は 頂点の位置であり、 zw は UV です。
		/// </summary>
		/// @endif
		public void AddSprite( Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3 )
		{
			m_v0 = v0;
			m_v1 = v1;
			m_v2 = v2;
			m_v3 = v3;

			add_quad();
		}

		void add_quad()
		{
		    if ( FlipU )
		    {
		        Common.Swap( ref m_v0.X, ref m_v1.X );
		        Common.Swap( ref m_v0.Y, ref m_v1.Y );
		        Common.Swap( ref m_v2.X, ref m_v3.X );
		        Common.Swap( ref m_v2.Y, ref m_v3.Y );
		    }
		
		    if ( FlipV )
		    {
		        Common.Swap( ref m_v0.Y, ref m_v2.Y );
		        Common.Swap( ref m_v0.X, ref m_v2.X );
		        Common.Swap( ref m_v1.Y, ref m_v3.Y );
		        Common.Swap( ref m_v1.X, ref m_v3.X );
		    }
		
		    m_imm_quads.ImmAddQuad( m_v0, m_v1, m_v2, m_v3 );
		}		

		
		Vector2 transform_point( ref Matrix3 mat, Vector2 pos )
		{
			return 
				mat.X.Xy * pos.X + 
				mat.Y.Xy * pos.Y + 
				mat.Z.Xy;
		}
	}
}

