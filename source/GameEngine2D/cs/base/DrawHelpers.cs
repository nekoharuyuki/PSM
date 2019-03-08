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
	/// Some basic drawing functionalities (2D/3D).
	/// This class is mostly an ImmediateMode object coupled with a debug shader. 
	/// You shouldn't use DrawHelpers for anything else than visual debugging, 
	/// as by nature it is not performance friendly at all.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>2D/3Dの基本的な描画関数を提供します。このクラスは主にデバッグシェーダーを使った ImmediateMode のオブジェクトです。
	/// パフォーマンスが良くないため、デバッグ以外の用途で DrawHelpers を使用することは推奨しません。
	/// </summary>
	/// @endif
	public class DrawHelpers : System.IDisposable
	{
		/// @if LANG_EN
		/// <summary>
		/// The vertex type used by DrawHelpers (V4F_C4F)
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>DrawHelpers で使用される頂点 (V4F_C4F)。
		/// </summary>
		/// @endif
		public struct Vertex
		{
			/// @if LANG_EN
			/// <summary>The vertex position. 2D positions should have (z,w) set to (0,1).</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>頂点位置。2D位置は、(z,w) を (0,1)に設定します。
			/// </summary>
			/// @endif
			public Vector4 Position;

			/// @if LANG_EN
			/// <summary>Color, each element in 0,1 range (but values don't get clamped).</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>色。各要素は 0,1の範囲で設定してください(クランプ処理は行われません)。
			/// </summary>
			/// @endif
			public Vector4 Color;

			/// @if LANG_EN
			/// <summary>Constructor.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>コンストラクタ。</summary>
			/// @endif
			public Vertex( Vector4 pos, Vector4 col )
			{
				Position = pos;
				Color = col;
			}

			/// @if LANG_EN
			/// <summary>Constructor.
			/// </summary>
			/// <param name="pos">The position is expended to 3d by setting (z,w) to (0,1).</param>
			/// <param name="col">The color.</param>
			/// @endif
			/// @if LANG_JA
			/// <summary>コンストラクタ。
			/// </summary>
			/// <param name="pos">(z,w)を(0,1)の設定し、位置を3Dに拡張します。</param>
			/// <param name="col">色。</param>
			/// @endif
			public Vertex( Vector2 pos, Vector4 col )
			{
				Position = pos.Xy01;
				Color = col;
			}
		}

		GraphicsContextAlpha GL;
		ImmediateMode< Vertex > m_imm;
		ShaderProgram m_shader_program; // a simple shader for debug primitives
		Vector4 m_current_color; // the last color set with .SetColor
		uint m_shader_depth; // allow nested shader push/pop 
		uint m_view_matrix_tag;	// check for ViewMatrix update
		uint m_model_matrix_tag; // check for ModelMatrix update
		uint m_projection_matrix_tag; // check for ProjectionMatrix update

		bool m_disposed = false;
		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このオブジェクトが破棄されていれば、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		/// @if LANG_EN
		/// <summary>
		/// </summary>
		/// <param name="gl">The core graphics context.</param>
		/// <param name="max_vertices">The maximum number of vertices you will be able to
		/// write in one frame with this DrawHelpers object.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// <param name="gl">コアグラフィックスのコンテキスト。</param>
		/// <param name="max_vertices">この DrawHelpers オブジェクトを使って、1フレームで描画する頂点の最大数。</param>
		/// @endif
		public DrawHelpers( GraphicsContextAlpha gl, uint max_vertices )
		{
			GL = gl;

			{
				m_shader_program = Common.CreateShaderProgram("cg/default.cgx");

				m_shader_program.SetUniformBinding( 0, "MVP" ) ;
				m_shader_program.SetAttributeBinding( 0, "p" ) ;
				m_shader_program.SetAttributeBinding( 1, "vin_color" ) ;
			}

			m_current_color = Colors.Magenta;
			m_shader_depth = 0;
			m_view_matrix_tag = unchecked((uint)-1);
			m_model_matrix_tag = unchecked((uint)-1);
			m_projection_matrix_tag = unchecked((uint)-1);

			m_imm = new ImmediateMode<Vertex>( gl, max_vertices, null, 0, 0, VertexFormat.Float4, VertexFormat.Float4 );
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
				m_imm.Dispose();

				Common.DisposeAndNullify< ShaderProgram >( ref m_shader_program );
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Start a new immediate primitive. 
		/// </summary>
		/// <param name="mode">The draw primive type.</param>
		/// <param name="max_vertices_intended">You must specify the maximum number of 
		/// vertices you intend to write with ImmVertex(): the number of ImmVertex() calls 
		/// following this function must be inferior or equal to 'max_vertices_intended'.
		/// </param>
		/// @endif
		/// @if LANG_JA
		/// <summary>新しいイミーディエイトプリミティブを開始します。
		/// </summary>
		/// @endif
		public void ImmBegin( DrawMode mode, uint max_vertices_intended )
		{
			m_imm.ImmBegin( mode, max_vertices_intended );
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a vertex to current primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>頂点を現在のプリミティブに追加します。
		/// </summary>
		/// @endif
		public void ImmVertex( Vertex v )
		{
			m_imm.ImmVertex( v );
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a vertex to current primitive, using the most recent color set by SetColor().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>直近のSetColor() で設定された色を使い、現在のプリミティブに頂点を追加します。
		/// </summary>
		/// @endif
		public void ImmVertex( Vector4 pos )
		{
			m_imm.ImmVertex( new Vertex( pos, m_current_color ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a vertex to current primitive, using the most recent color set by SetColor().
		/// (z,w) is set to (0,1).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>直近のSetColor() で設定された色を使い、現在のプリミティブに頂点を追加します。
		/// (z,w) は (0,1)でセットします。
		/// </summary>
		/// @endif
		public void ImmVertex( Vector2 pos )
		{
			m_imm.ImmVertex( new Vertex( pos.Xy01, m_current_color ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Finish current primitive (this function triggers the actual draw call).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 現在のプリミティブを終了します。この関数は、実際の描画呼び出しをトリガー(引き起こし)します。
		/// </summary>
		/// @endif
		public void ImmEnd()
		{
			m_imm.ImmEnd();
		}

		/// @if LANG_EN
		/// <summary>
		/// ShaderPush() reads current MVP matrix and sets the current shader.
		/// For DrawHelpers we allow nesting (shader parameters get updated
		/// internally accordingly).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ShaderPush() は現在のMVP行列を読み、現在のシェーダーをセットします。
		/// DrawHelpers ではネストで処理できます。シェーダーのパラメータは内部的に更新されます。
		/// </summary>
		/// @endif
		public void ShaderPush()
		{
			// check if MVP needs update... not sure how useful that actually is
			if ( m_view_matrix_tag != GL.ViewMatrix.Tag
				 || m_model_matrix_tag != GL.ModelMatrix.Tag 
				 || m_projection_matrix_tag != GL.ProjectionMatrix.Tag )
			{
				m_model_matrix_tag = GL.ModelMatrix.Tag;
				m_view_matrix_tag = GL.ViewMatrix.Tag;
				m_projection_matrix_tag = GL.ProjectionMatrix.Tag;

				Matrix4 mvp = GL.GetMVP();

				m_shader_program.SetUniformValue( 0, ref mvp );
			}

			if ( m_shader_depth++ != 0 )
				return;

			GL.Context.SetShaderProgram( m_shader_program );
		}

		/// @if LANG_EN
		/// <summary>
		/// "Pop" the shader. Number of ShaderPush() calls must match the number of ShaderPush().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// シェーダーをポップします。 ShaderPush()の回数は、ShaderPush()の回数に一致しなければなりません。
		/// </summary>
		/// @endif
		public void ShaderPop()
		{
			Common.Assert( m_shader_depth > 0 );

			if ( --m_shader_depth != 0 )
				return;

//			GL.Context.SetShaderProgram( null ); // would like to go back to clean state but this asserts
		}

		/// @if LANG_EN
		/// <summary>
		/// Set the color to be used by the next calls to ImmVertex.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 次の ImmVertex の呼び出しで使われる色をセットします。
		/// </summary>
		/// @endif
		public void SetColor( Vector4 value )
		{
			m_current_color = value;
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a filled axis aligned rectangle.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>四角形を描画します。
		/// </summary>
		/// @endif
		public void DrawBounds2Fill( Bounds2 bounds )
		{
			ShaderPush();

			ImmBegin( DrawMode.TriangleStrip, 4 );
//			ImmVertex( new Vertex( bounds.Point01.Xy01, Colors.Green ) ); // debug
//			ImmVertex( new Vertex( bounds.Point00.Xy01, Colors.Black ) ); // debug
//			ImmVertex( new Vertex( bounds.Point11.Xy01, Colors.Yellow ) ); // debug
//			ImmVertex( new Vertex( bounds.Point10.Xy01, Colors.Red ) ); // debug
			ImmVertex( new Vertex( bounds.Point01.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point11.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point10.Xy01, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a wireframe axis aligned rectangle.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>線で四角形を描画します。
		/// </summary>
		/// @endif
		public void DrawBounds2( Bounds2 bounds )
		{
			ShaderPush();

			ImmBegin( DrawMode.LineStrip, 5 );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point10.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point11.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point01.Xy01, m_current_color ) );
			ImmVertex( new Vertex( bounds.Point00.Xy01, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw convex polygon.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>凸ポリゴンを描画します。
		/// </summary>
		/// @endif
		public void DrawConvexPoly2( ConvexPoly2 convex_poly )
		{
			if ( convex_poly.Planes.Length == 0 )
				return;

			ShaderPush();
 
			ImmBegin( DrawMode.LineStrip, (uint)convex_poly.Planes.Length + 1 );
			foreach ( Plane2 p in convex_poly.Planes )
				ImmVertex( new Vertex( p.Base, m_current_color ) );
			ImmVertex( new Vertex( convex_poly.Planes[0].Base, m_current_color ) );
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a filled disk.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="radius">The radius.</param>
		/// <param name="n">Tesselation.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>塗りつぶした円盤を描画します。
		/// </summary>
		/// <param name="center">中心。</param>
		/// <param name="radius">半径。</param>
		/// <param name="n">分割数。</param>
		/// @endif
		public void DrawDisk( Vector2 center, float radius, uint n )
		{
			ShaderPush();

			ImmBegin( DrawMode.TriangleFan, n );
			for ( uint i=0;i<n;i++ )
			{
				Vector2 u = Vector2.Rotation( ((float)i/(float)(n-1)) * Math.TwicePi );
				ImmVertex( new Vertex( ( center + u * radius ).Xy01, m_current_color ) );
			}
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a filled circle.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="radius">The radius.</param>
		/// <param name="n">Tesselation.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>塗りつぶした円を描画します。
		/// </summary>
		/// <param name="center">中心。</param>
		/// <param name="radius">半径。</param>
		/// <param name="n">分割数。</param>
		/// @endif
		public void DrawCircle( Vector2 center, float radius, uint n )
		{
			ShaderPush();

			ImmBegin( DrawMode.LineStrip, n );
			for ( uint i=0;i<n;i++ )
			{
				Vector2 u = Vector2.Rotation( ((float)i/(float)(n-1)) * Math.TwicePi );
				ImmVertex( new Vertex( ( center + u * radius ).Xy01, m_current_color ) );
			}
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw the coordinate system represented by a transformation matrix 
		/// using arrows. The x vector is represented by a red arrow, and the y 
		/// vector is represented by a green arrow.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>矢印を使用し、変換行列であらわされる座標系を描画します。
		/// xのベクトルは赤色の矢印で表され、yのベクトルは、緑の矢印で表されます。
		/// </summary>
		/// @endif
		public void DrawCoordinateSystem2D( Matrix3 mat, ArrowParams ap = null )
		{
			if ( ap == null ) 
				ap = new ArrowParams();

			ShaderPush();

			ImmBegin( DrawMode.Triangles, 9 * 2 );

			SetColor( Colors.Red );
			DrawArrow( mat.Z.Xy, mat.Z.Xy + mat.X.Xy, ap );

			SetColor( Colors.Green );
			DrawArrow( mat.Z.Xy, mat.Z.Xy + mat.Y.Xy, ap );

			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a unit len arrow on x and y axis. Color is set to vector
		/// coordinates, so the x arrow is red (1,0,0), and the y arrow is
		/// green (0,1,0).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>x軸とy軸に矢印を描画します。xの矢印は赤 (1,0,0)であり、yの矢印は緑 (0,1,0)です。
		/// </summary>
		/// @endif
		public void DrawCoordinateSystem2D()
		{
			DrawCoordinateSystem2D( Matrix3.Identity );
		}

		/// @if LANG_EN
		/// <summary>
		/// Arrow parameters passed to DrawArrow.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>DrawArrow に渡される矢印のパラメータ。
		/// </summary>
		/// @endif
		public class ArrowParams
		{
			/// @if LANG_EN
			/// <summary>
			/// Length of the base of the arrow's head.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>矢印の頭部のベースの長さ。
			/// </summary>
			/// @endif
			public float HeadRadius;

			/// @if LANG_EN
			/// <summary>
			/// Length of the arrow's head.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>矢印の頭部の長さ。
			/// </summary>
			/// @endif
			public float HeadLen;
			
			/// @if LANG_EN
			/// <summary>
			/// Arrow's body's radius.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>矢印の本体の半径。
			/// </summary>
			/// @endif
			public float BodyRadius;
			
			/// @if LANG_EN
			/// <summary>
			/// A scale factor applied to HeadRadius, HeadLen, BodyRadius.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>HeadRadius、HeadLen、BodyRadiusに適用されるスケール要素。
			/// </summary>
			/// @endif
			public float Scale;

			/// @if LANG_EN
			/// <summary>
			/// You can display half of the arrow (and select which side) handy for debugging half edge graphs for example.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>矢印の半分を表示します。
			/// </summary>
			/// @endif
			public uint HalfMask;

			/// @if LANG_EN
			/// <summary>
			/// Arrow end points can be offset along the perpendicular direction.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>矢印の終点は、垂直方向に沿ってオフセットすることができます。
			/// </summary>
			/// @endif
			public float Offset;

			/// @if LANG_EN
			/// <summary>
			/// ArrowParams's constructor.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>コンストラクタ。
			/// </summary>
			/// @endif
			public ArrowParams( float r = 1.0f )
			{
				HeadRadius = 0.06f * r;
				HeadLen = 0.2f * r;
				BodyRadius = 0.02f * r;
				Scale = 1.0f;                   
				HalfMask = 3;
				Offset = 0.0f;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a 2d arrow. This function can be wrapped by ImmBegin()/ImmEnd(), 
		/// if you need to draw several arrows but want to limit the number of 
		/// draw calls. Each arrow consumes at most 9 vertices.
		/// </summary>
		/// <param name="start_point">Arrow's start point.</param>
		/// <param name="end_point">Arrow's tip.</param>
		/// <param name="ap">Arrow geometry parameters.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// 2Dの矢印を描画します。
		/// いくつかの矢印を描画する必要があり、かつ、描画呼び出しの数を制限したい場合には、この関数は ImmBegin()/ ImmEnd()でラップすることができます。
		/// 各矢印は、多くとも9頂点を消費します。
		/// </summary>
		/// <param name="start_point">矢印の開始位置。</param>
		/// <param name="end_point">矢印の先端。</param>
		/// <param name="ap">矢印の幾何情報。</param>
		/// @endif
		public void DrawArrow( Vector2 start_point, Vector2 end_point, ArrowParams ap )
		{
			Vector2 x = end_point - start_point;
			Vector2 x1 = x.Normalize();
			Vector2 y1 = Math.Perp( x1 );

			start_point += y1 * ap.Offset;
			end_point += y1 * ap.Offset;

			ap.BodyRadius *= ap.Scale;
			ap.HeadRadius *= ap.Scale;
			ap.HeadLen *= ap.Scale;

			float r2p = ap.HeadRadius;
			float r2n = ap.HeadRadius;
			float r1p = ap.BodyRadius;
			float r1n = ap.BodyRadius;

			if ( ( ap.HalfMask & 1 ) == 0 )
			{
				r2n=0.0f;
				r1n=0.0f;
			}

			if ( ( ap.HalfMask & 2 ) == 0 )
			{
				r2p=0.0f;
				r1p=0.0f;
			}

			ShaderPush();

			bool imm_was_active = m_imm.ImmActive;

			if ( ap.BodyRadius == 0.0f 
				 && !imm_was_active )
			{
				ImmBegin( DrawMode.Lines, 2 );
				ImmVertex( start_point );
				ImmVertex( end_point );
				ImmEnd();
			}

			if ( !imm_was_active ) 
				ImmBegin( DrawMode.Triangles, 9 );

			if ( ap.BodyRadius != 0.0f )
			{
				Vector2 p01 = start_point + r1p * y1;
				Vector2 p00 = start_point - r1n * y1;
				Vector2 p11 = end_point - x1 * ap.HeadLen + r1p * y1;
				Vector2 p10 = end_point - x1 * ap.HeadLen - r1n * y1;

				ImmVertex( p01 );
				ImmVertex( p00 );
				ImmVertex( p11 );
				ImmVertex( p00 );
				ImmVertex( p10 );
				ImmVertex( p11 );
			}

			ImmVertex( end_point - x1 * ap.HeadLen - r2n * y1 );
			ImmVertex( end_point );
			ImmVertex( end_point - x1 * ap.HeadLen + r2p * y1 );

			if ( !imm_was_active ) 
				ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a single line segment.
		/// This is expensive, if you draw many lines, don't use this function.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ひとつの線分を描画します。処理が重いので、多くの線を描画する場合、この関数は使用しないでください。
		/// </summary>
		/// @endif
		public void DrawLineSegment( Vector2 A, Vector2 B )
		{
			ShaderPush();

			ImmBegin( DrawMode.Lines, 2 );
			ImmVertex( A );
			ImmVertex( B );
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a single line segment.
		/// This is expensive, if you draw many lines, don't use this function.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>単一の線分を描画します。この関数は処理が重いため、多くの線を描画する場合、この関数の使用は避けてください。
		/// </summary>
		/// @endif
		public void DrawInfiniteLine( Vector2 A, Vector2 v )
		{
			ShaderPush();

			v *= 10000.0f;

			ImmBegin( DrawMode.Lines, 2 );
			ImmVertex( A - v );
			ImmVertex( A + v );
			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw all the vertical and horizontal lines in a given rectangle, regularly spaced.
		/// Since the smaller step_x or step_y are, the more lines primitives are generated, 
		/// it is easy to overflow the immediate draw mode vertex buffer. For that reason care 
		/// must be taken when setting the step values respective to the the bounds clip area.
		/// </summary>
		/// <param name="step_x">X spacing (starts at 0).</param>
		/// <param name="step_y">Y spacing (starts at 0).</param> 
		/// <param name="bounds">Clipping rectangle.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>指定された矩形内に、垂直および水平の線を等間隔で描画します。
		/// step_x, step_y を小さくすると、より多くのラインプリミティブが生成されるため、イミーディエイト描画モードの頂点バッファが容易にあふれてしまします。そのため、引数の指定には注意してください。
		/// </summary>
		/// @endif
		public void DrawRulers( Bounds2 bounds, float step_x, float step_y )
		{
			step_x = FMath.Max( step_x, 0.0f );
			step_y = FMath.Max( step_y, 0.0f );

			if ( step_x < float.Epsilon ) return;
			if ( step_y < float.Epsilon ) return;

			float left   = bounds.Min.X;
			float right  = bounds.Max.X;
			float bottom = bounds.Min.Y;
			float top    = bounds.Max.Y;

			int l = (int)( left   / step_x );
			int r = (int)( right  / step_x );
			int b = (int)( bottom / step_y );
			int t = (int)( top    / step_y );

			ShaderPush();

			bool safe_x = (r-l+1)<1000;
			bool safe_y = (t-b+1)<1000;

			ImmBegin( DrawMode.Lines, 
					  ( safe_x ? (uint)((r-l+1)*2) : 0 ) + 
					  ( safe_y ? (uint)((t-b+1)*2) : 0 ) );

			if ( safe_x )
			{
				for ( int i=l;i<=r;++i )
				{
					ImmVertex( new Vector2( (float)i * step_x, bottom ) );
					ImmVertex( new Vector2( (float)i * step_x, top ) );
				}
			}
//			else
//			{
//				System.Console.WriteLine( "skip drawing of x rulers lines, too many lines" );
//			}

			if ( safe_y )
			{
				for ( int i=b;i<=t;++i )
				{
					ImmVertex( new Vector2( left, (float)i * step_y ) );
					ImmVertex( new Vector2( right, (float)i * step_y ) );
				}
			}
//  		else
//  		{
//  			System.Console.WriteLine( "skip drawing of y rulers lines, too many lines" );
//  		}

			ImmEnd();

			ShaderPop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw axis lines (x=0 and y=0 lines) with a thickness.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>厚みのある軸線（x = 0 と y= 0 ライン）を描画します。
		/// </summary>
		/// @endif
		public void DrawAxis( Bounds2 clipping_bounds, float thickness )
		{
			GL.Context.SetLineWidth( thickness );

			float x = 0.0f;
			float y = 0.0f;

			ShaderPush();

			ImmBegin( DrawMode.Lines, 4 );

			// x=0
			ImmVertex( new Vector2( clipping_bounds.Min.X /*left*/, y ) );
			ImmVertex( new Vector2( clipping_bounds.Max.X/*right*/, y ) );

			// y=0
			ImmVertex( new Vector2( x, clipping_bounds.Min.Y/*bottom*/ ) );
			ImmVertex( new Vector2( x, clipping_bounds.Max.Y/*top*/ ) );

			ImmEnd();

			ShaderPop();

			GL.Context.SetLineWidth( 1.0f );
		}

		/// @if LANG_EN
		/// <summary>
		/// This function draws all the vertical and horizontal lines (rulers) regularly placed 
		/// at multiples of 'step' distances that are inside the rectangle 'clipping_bounds'. 
		/// It also draws the the 2 thick axis lines. All lines drawn are clipped again 
		/// 'clipping_bounds'. Blend mode is untouched when drawing the rulers, then blend is 
		/// disabled when drawing axis lines.
		/// </summary>
		/// <param name="clipping_bounds">Clipping rectangle.</param>
		/// <param name="step">Horizontal and vertical spacing between rulers.</param>
		/// <param name="rulers_color">Color of rulers lines.</param>
		/// <param name="axis_color">Color of axis lines.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>この関数は、引数'clipping_bounds'の内部に、'step'の値で規則的に垂直・水平の線を描画します。
		/// また、厚みのある2つの軸線を描画します。描画されるすべての線が'clipping_bounds'でクリップされます。
		/// ルーラーを描画するとき、ブレンドモードは変更されません。軸線を描画するとき、ブレンドはDisableになっています。
		/// </summary>
		/// <param name="clipping_bounds">クリッピングの矩形。</param>
		/// <param name="step">ルーラーの間隔。</param>
		/// <param name="rulers_color">ルーラーの色。</param>
		/// <param name="axis_color">軸線の色。</param>
		/// @endif
		public void DrawDefaultGrid( Bounds2 clipping_bounds, Vector2 step
									 , Vector4 rulers_color, Vector4 axis_color )
		{
			//Common.Profiler.Push("DrawHelpers.DrawDefaultGrid");

			ShaderPush();

			SetColor( rulers_color );
			DrawRulers( clipping_bounds, step.X, step.Y );

			GL.Context.Disable( EnableMode.Blend );

			SetColor( axis_color );
			DrawAxis( clipping_bounds, 2.0f );

			ShaderPop();

			//Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// DrawDefaultGrid with a default color/blend.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デフォルトの色/ブレンドを使ったDrawDefaultGrid()。
		/// </summary>
		/// @endif
		public void DrawDefaultGrid( Bounds2 clipping_bounds, float step )
		{
			GL.Context.Enable( EnableMode.Blend );
			GL.Context.SetBlendFunc( new BlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One ) );

			DrawDefaultGrid( clipping_bounds, new Vector2( step ), Colors.Grey30 * 0.5f, Colors.Black );
		}
	}
}

