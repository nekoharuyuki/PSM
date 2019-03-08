/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

#define IMMEDIATE_MODE_QUADS_USES_INDEXING
#define IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK // special hack for xepria, where it seems we need one VertexBuffer per DrawArray or performance is abysmal
#define IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK_DISCARD_UNUSED_VERTEX_BUFFER

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// An immediate mode vertex array that you can write everyframe
	/// using a ImmBegin()/ImmVertex()/ImmEnd() OpenGL style interface.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>イミーディエイトモードの頂点配列は、ImmBegin()/ImmVertex()/ImmEnd()などのOpenGLスタイルのインターフェースを使い、どのフレームでも描画できます。
	/// </summary>
	/// @endif
	public class ImmediateMode<T> : System.IDisposable
	{
		GraphicsContextAlpha GL;
		#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBufferPool m_vbuf_pool;
		#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBuffer[] m_vertex_buffers; // n buffer, to deal with xperia GL drivers(?) problems
		int m_current_vertex_buffer_index;
		#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
		VertexBuffer m_current_vertex_buffer;
		T[] m_vertices_tmp;
		uint m_max_vertices; // the maximum number of total vertices written within one frame between ImmBegin/ImmEnd
		uint m_max_indices;
		DrawMode m_prim; // current ImmBegin draw primitive
		uint m_prim_start; // m_pos when ImmBegin is called... ( m_pos - m_prim_start ) is the number of vertices sent
		uint m_pos; // current position in written array
		uint m_frame_count; // last frame when an ImmBegin was called
		uint m_max_vertices_intended; // maximum number of vertices expected in the current ImmBegin/ImmEnd
		bool m_active; // true if we are between an immBegin and an ImmEnd
		int m_vertices_per_primitive; // the number of vertices each ImmBegin is expected to have (relevant only if indices is set, 0 if not used)
		int m_indices_per_primitive; // the number of indices we want to draw for each primitive (relevant only if indices is set, 0 if not used)

		bool m_disposed = false;
		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このオブジェクトが破棄されている場合、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		/// @if LANG_EN
		/// <summary>
		/// ImmediateMode constructor.
		/// 
		/// If indices is not null, vertices_per_primitive and indices_per_primitive must follow the constraints below:
		/// 
		///  - vertices_per_primitive must not be 0
		///  - indices_per_primitive must not be 0
		///  - max_vertices must be a multiple of vertices_per_primitive
		///  - indices.Length must be a multiple of indices_per_primitive
		///	 - max_vertices / vertices_per_primitive must be equal to indices.Length / indices_per_primitive
		/// 
		/// If any of those constraints is not met, the constructor will assert.
		/// 
		/// Note that ImmediateMode relies on the frame counter incremented by Common.OnSwap().
		/// In the context of using GameEngine2D.Director, Common.OnSwap() is already called inside Director.Instance.PostSwap(). 
		/// 
		/// But if you use ImmediateMode "stand alone" then you will need to call Common.OnSwap() yourself everyframe to make 
		/// sure the GameEngine2D's frame counter gets increments (else you will get memory leaks.)
		/// </summary>
		/// <param name="gl">The core graphics context.</param> 
		/// <param name="max_vertices">The maximum number of vertices you can have per frame.</param>
		/// <param name="indices">The array of indices (can be null), assuming a static setup.</param>
		/// <param name="vertices_per_primitive">If indices is not null, this must be set to the number of vertices each ImmBegin is expected to have. If indices is null, just set to 0.</param>
		/// <param name="indices_per_primitive">If indices is not null, this must be set to the number of indices you want to draw for each primitive. If indices is null, just set to 0.</param>
		/// <param name="formats">The vertex format, passed to VertexBuffer as it is.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// ImmediateModeのコンストラクタです。
		/// 
		/// インデックスがnullでない場合は、vertices_per_primitiveとindices_per_primitiveは、以下の制約に従う必要があります。
		/// 
		/// - vertices_per_primitiveは0であってはならない
		/// - indices_per_primitiveは0であってはならない
		/// - max_verticesはvertices_per_primitiveの倍数でなければなりません
		/// - indices.Lengthはindices_per_primitiveの倍数でなければなりません
		/// - max_vertices/ vertices_per_primitiveはindices.Length/ indices_per_primitiveに等しくなければなりません
		/// 
		/// これらの制約のいずれかが満たされていない場合、コンストラクタは警告を出します。
		/// 
		/// ImmediateModeは、Common.OnSwap() でインクリメントされるフレームカウンタに依存していることに注意してください。
		/// GameEngine2D.Directorを使用する流れでは、Common.OnSwap()はDirector.Instance.PostSwap()の内部で呼ばれてるようになっています。
		/// ImmediateModeを "スタンドアローン"を使用する場合でも、GameEngine2Dのフレームカウンタがインクリメントされているか確認するため、毎フレームCommon.OnSwap()を呼び出す必要があります。（呼び出されないと、メモリリークが発生します)。
		/// 
		/// </summary>
		/// <param name="gl">コアグラフィックスのコンテキスト。</param> 
		/// <param name="max_vertices">1フレームあたりの最大頂点数。</param>
		/// <param name="indices">インデックスの配列 (nullでも可)。静的な設定を仮定しています。</param>
		/// <param name="vertices_per_primitive">インデックスがnullでない場合、これは各ImmBeginが持つ予定の頂点数に設定する必要があります。インデックスがnullの場合は、0に設定されています。</param>
		/// <param name="indices_per_primitive">インデックスがnullでない場合、これは各プリミティブごとに描画したいインデックスの数に設定する必要があります。インデックスがnullの場合は、0に設定されています。</param>
		/// <param name="formats">VertexBufferに渡される頂点フォーマット。</param>
		/// @endif
		public ImmediateMode( GraphicsContextAlpha gl
							  , uint max_vertices
							  , ushort[] indices
							  , int vertices_per_primitive
							  , int indices_per_primitive
							  , params VertexFormat[] formats )
		{
/*
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			{
				// clamp to next power of 2
				int p = Math.Log2( max_vertices );
				if ( ( 1 << p ) < max_vertices ) ++p;
				max_vertices = ( 1 << p );
			}
			#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
*/
			GL = gl;

			m_max_vertices = max_vertices;
			m_vertices_per_primitive = vertices_per_primitive;
			m_indices_per_primitive = indices_per_primitive;
			m_vertices_tmp = new T[ m_max_vertices ];
			m_frame_count = unchecked((uint)-1);
			m_pos = 0;
			
			if ( indices != null )
			{
				 m_max_indices = (uint)indices.Length;

				Common.Assert( m_vertices_per_primitive != 0 );
				Common.Assert( m_indices_per_primitive != 0 );
				Common.Assert( ( m_max_vertices / m_vertices_per_primitive ) * m_vertices_per_primitive == m_max_vertices );
				Common.Assert( ( m_max_indices / m_indices_per_primitive ) * m_indices_per_primitive == m_max_indices );
				Common.Assert( ( m_max_vertices / m_vertices_per_primitive ) == ( m_max_indices / m_indices_per_primitive ) );
			}
			else 
			{
				m_max_indices = 0;
			}

			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			m_vbuf_pool = new VertexBufferPool( indices, m_vertices_per_primitive, m_indices_per_primitive, formats );
			#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
//			m_vertex_buffers = new VertexBuffer[40]; // brute force test
			m_vertex_buffers = new VertexBuffer[2];
//			m_vertex_buffers = new VertexBuffer[1];

			for ( int i=0; i < m_vertex_buffers.Length; ++i )
			{
				m_vertex_buffers[i] = new VertexBuffer( (int)m_max_vertices, (int)m_max_indices, formats );

				if ( indices != null )
					m_vertex_buffers[i].SetIndices( indices, 0, 0, (int)m_max_indices );
			}
			#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
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
				#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				m_vbuf_pool.Dispose();
				#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				for ( int i=0; i < m_vertex_buffers.Length; ++i )
					m_vertex_buffers[i].Dispose();
				m_vertex_buffers.Clear();
				m_current_vertex_buffer_index = -1;
				#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>Begin a draw primitive.</summary>
		/// <param name="mode">The draw primitive type.</param>
		/// <param name="max_vertices_intended">The maximum number of vertices you intend to write.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画プリミティブの開始。
		/// </summary>
		/// <param name="mode">描画プリミティブのタイプ。</param>
		/// <param name="max_vertices_intended">描画予定の最大頂点数。</param>
		/// @endif
		public void ImmBegin( DrawMode mode, uint max_vertices_intended )
		{
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			if ( m_frame_count != Common.FrameCount )
			{
				m_frame_count = Common.FrameCount;
				m_vbuf_pool.OnFrameChanged();
			}
			// get a new VertexBuffer for each DrawArray
			m_current_vertex_buffer = m_vbuf_pool.GetAVertexBuffer( (int)max_vertices_intended );
			m_pos = 0;
			#else // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			if ( m_frame_count != Common.FrameCount )
			{
				m_frame_count = Common.FrameCount;
				m_current_vertex_buffer_index = ( m_current_vertex_buffer_index + 1 ) % m_vertex_buffers.Length;
				m_current_vertex_buffer = m_vertex_buffers[ m_current_vertex_buffer_index ];
				m_pos = 0;
			}
			#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK

			m_prim_start = m_pos;
			m_prim = mode;
			m_max_vertices_intended = max_vertices_intended;
			m_active = true;
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a vertex, must be called between ImmBegin and ImmEnd.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>頂点を追加します。ImmBeginとImmEndの間で呼ぶ必要があります。
		/// </summary>
		/// @endif
		public void ImmVertex( T vertex )
		{
			#if DEBUG
			Common.Assert(m_pos<m_max_vertices,"ImmediateMode capacity overflow (m_max_vertices="+m_max_vertices+")");
			#endif

			m_vertices_tmp[m_pos++] = vertex;
		}

		void imm_end_prelude()
		{
			Common.Assert( m_pos - m_prim_start <= m_max_vertices_intended, "You added more vertices than you said you would." ); 

//			if ( m_current_vertex_buffer != GL.GetVertexBuffer( 0 ) )
				GL.Context.SetVertexBuffer( 0, m_current_vertex_buffer );

//			System.Console.WriteLine( Common.FrameCount + " " + ( m_pos - m_prim_start ) );

			m_current_vertex_buffer.SetVertices(m_vertices_tmp,(int)m_prim_start,(int)m_prim_start,(int)(m_pos-m_prim_start));
		}

		/// @if LANG_EN
		/// <summary>
		/// End draw primitive and draw.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画プリミティブを終了し、描画処理を行います。
		/// </summary>
		/// @endif
		public void ImmEnd()
		{
			////Common.Profiler.Push("ImmediateMode<T>.ImmEnd");

			imm_end_prelude();

			GL.Context.DrawArrays( m_prim, (int)m_prim_start,(int)(m_pos-m_prim_start) );

			GL.DebugStats.OnDrawArray(); // count the number of DrawArrays per frame

			////Common.Profiler.Pop();

			m_active = false;
		}

		/// @if LANG_EN
		/// <summary>
		/// Special version of ImmEnd that uses the 'vertices_per_primitive' and 'indices_per_primitive' arguments
		/// passed to ImmediateMode's constructor. It is assumed that in that case all primitives consume the same
		/// amount of vertices.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ImmediateMode のコンストラクタに渡される引数'vertices_per_primitive' and 'indices_per_primitive' を使った、ImmEndの特別なバージョンです。すべてのプリミティブは同じ数の頂点を消費することを想定しています。
		/// </summary>
		/// @endif
		public void ImmEndIndexing()
		{
			////Common.Profiler.Push("ImmediateMode<T>.ImmEnd");

			#if DEBUG
			Common.Assert( ( ( m_pos - m_prim_start ) / m_vertices_per_primitive ) * m_vertices_per_primitive == ( m_pos - m_prim_start ) );
			Common.Assert( ( ( m_prim_start ) / m_vertices_per_primitive ) * m_vertices_per_primitive == m_prim_start );
			#endif

			imm_end_prelude();

			GL.Context.DrawArrays( m_prim, ((int)m_prim_start/m_vertices_per_primitive)*m_indices_per_primitive
							 ,((int)(m_pos-m_prim_start)/m_vertices_per_primitive)*m_indices_per_primitive );

			GL.DebugStats.OnDrawArray(); // count the number of DrawArrays per frame

			////Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if we are in the middle of an ImmBegin()/ImmEnd().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ImmBegin()/ImmEnd()の間で呼ばれると、trueを返します。
		/// </summary>
		/// @endif
		public bool ImmActive { get { return m_active;}}

		/// @if LANG_EN
		/// <summary>
		/// Return the maximum (total) number of vertices we can add per frame.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>フレームごとに追加可能な、最大(合計)頂点数を返します。
		/// </summary>
		/// @endif
		public uint MaxVertices { get { return m_max_vertices; } }
	}

	/// @if LANG_EN
	/// <summary>
	/// ImmediateModeQuads wraps ImmediateMode to deal with quad rendering only.
	/// This is used by SpriteRenderer and other places.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ImmediateModeQuads はクワッドの描画を扱うために、ImmediateModeをラップしています。
	/// このクラスはSpriteRenderer や他の場所で使用されます。
	/// </summary>
	/// @endif
	public class ImmediateModeQuads<T> : System.IDisposable
	{
		ImmediateMode<T> m_imm;
		uint m_max_quads;

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
		/// Return the maximum (total) number of quads we can add per frame.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>フレームごとに追加可能なクワッドの最大数(合計)を返します。
		/// </summary>
		/// @endif
		public uint MaxQuads { get { return m_max_quads; } }

		/// @if LANG_EN
		/// <summary>Constructor.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public ImmediateModeQuads( GraphicsContextAlpha gl, uint max_quads, params VertexFormat[] formats )
		{
			#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
			// clamp to next power of 2 so that indices get the right number of vertices in VertexBufferPool
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			{
				int p = Math.Log2( (int)max_quads * 4 );
				if ( ( 1 << p ) < max_quads * 4 ) ++p;
				max_quads = (uint)( ( 1 << p ) / 4 );
			}
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			{
				int p = Math.Log2( (int)max_quads * 6 );
				if ( ( 1 << p ) < max_quads * 6 ) ++p;
				max_quads = (uint)( ( 1 << p ) / 6 );
			}
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			#endif

			m_max_quads = max_quads;

			ushort[] indices = null;

			#if IMMEDIATE_MODE_QUADS_USES_INDEXING

			// note: the lack of a sharable IndexBuffer objects forces us to copy indices for all VertexBuffers here

			indices = new ushort[ m_max_quads * 6 ];
			ushort[] quad_indices = new ushort[6] { 0,1,3,0,3,2};

			// manually repeat quad indexes for now

			for ( int q=0,i=0; q < (int)max_quads; ++q )
			{
				Common.Assert( i + 6 <= indices.Length );
				for ( int k=0; k<6; ++k )
					indices[i++] = (ushort)( q * 4 + quad_indices[k] );
			}

			m_imm = new ImmediateMode< T >( gl, max_quads * 4, indices, 4, 6, formats );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm = new ImmediateMode< T >( gl, max_quads * 6, null, 0, 0, formats );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
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
				m_imm.Dispose();
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>Prepare for registering n quads for rendering.</summary>
		/// <param name="num_quads">The maximum number of quads you intend to add.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画のため、nクワッドを登録する準備をします。
		/// </summary>
		/// <param name="num_quads">追加する予定のクワッドの最大数。</param>
		/// @endif
		public void ImmBeginQuads( uint num_quads )
		{
			m_imm.ImmBegin( DrawMode.Triangles, 
							#if IMMEDIATE_MODE_QUADS_USES_INDEXING
							(uint)(num_quads * 4) );
							#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
							(uint)(num_quads * 6) );
							#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a quad
		/// 
		/// v2----v3
		/// 
		///  |    | 
		/// 
		/// v0----v1
		/// 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// クワッドの追加。
		/// 
		/// v2----v3
		/// 
		///  |    | 
		/// 
		/// v0----v1
		/// 
		/// </summary>
		/// @endif
		public void ImmAddQuad( T v0, T v1, T v2, T v3 )
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// indexing does 013-023
			// note: inlining didn't make much difference
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v1 );
			m_imm.ImmVertex( v2 );
			m_imm.ImmVertex( v3 );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// copy 2 more vertices to form 2 triangles
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v1 );
			m_imm.ImmVertex( v3 );
			m_imm.ImmVertex( v0 );
			m_imm.ImmVertex( v3 );
			m_imm.ImmVertex( v2 );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// @if LANG_EN
		/// <summary>
		/// Add a quad
		/// 
		/// v[2]----v[3]
		/// 
		///   |      | 
		/// 
		/// v[0]----v[1]
		/// 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// クワッドの追加
		/// 
		/// v[2]----v[3]
		/// 
		///   |      | 
		/// 
		/// v[0]----v[1]
		/// 
		/// </summary>
		/// @endif
		public void ImmAddQuad( T[] v )
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// indexing does 013-023
			// note: inlining didn't make much difference
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[1] );
			m_imm.ImmVertex( v[2] );
			m_imm.ImmVertex( v[3] );
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			// copy 2 more vertices to form 2 triangles
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[1] );
			m_imm.ImmVertex( v[3] );
			m_imm.ImmVertex( v[0] );
			m_imm.ImmVertex( v[3] );
			m_imm.ImmVertex( v[2] );
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw all the quads added since the last ImmBeginQuads
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>最後のImmBeginQuads以降に追加された全てのクワッドを描画します。
		/// </summary>
		/// @endif
		public void ImmEndQuads()
		{
			#if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm.ImmEndIndexing();
			#else // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
			m_imm.ImmEnd();
			#endif // #if IMMEDIATE_MODE_QUADS_USES_INDEXING
		}
	}

	#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK

	// As the name implies, VertexBufferPool is a pool of VertexBuffer objects, 
	// so we can allocate/reuse are runtime. The reason this class exists is 
	// because of a problem on xperia/OpenGL(?) where render seems to get stuck 
	// if we try to reuse a VertexBuffer for multipled DrawArrays calls (!). 
	// To work around that, we "simply" allocate a VertexBuffer object for 
	// each DrawArrays call(!) - in order to do that we must be able to quickly 
	// get a VertexBuffer of any size at anytime at runtime. In this pool 
	// each queried size is rounded to the next power of 2. Note that not 
	// only this is wasting lots of memory (because of the padding) and 
	// causing a slight fragmentation risk because of the runtime allocations, 
	// but on other platforms (Vita) it ruins some small optimizations that 
	// were relying on "1 VertexBuffer, N DrawArrays" situations. But the 
	// decision was made to have one code path for all patforms.

	internal class VertexBufferPool : System.IDisposable
	{
		bool m_disposed = false;
		//Return true if this object been disposed.
		public bool Disposed { get { return m_disposed; } }

		class Entry
		{
			internal VertexBuffer m_vertex_buffer;
			internal uint m_frame_count; // remember last used frame so we can discard old items
		}

		// A list of VertexBuffer objects of a given size.

		class PerSizeList
		{
			internal VertexBufferPool m_parent;
			internal int m_max_vertices; // this is a power of 2
			internal List< Entry > m_active_list;
			internal List< Entry > m_free_list;

			public PerSizeList( int max_vertices )
			{
				m_max_vertices = max_vertices;
				m_active_list = new List< Entry >();
				m_free_list = new List< Entry >();
			}

			public VertexBuffer GetAVertexBuffer()
			{
				if ( m_free_list.Count == 0 )
				{
					int num_indices = 0;

					VertexBuffer vb;

//					#ifdef DEBUG
//					System.Console.WriteLine( Common.FrameCount + " VertexBufferPool: create VertexBuffer, size=" + m_max_vertices );
//					#endif

					if ( null == m_parent.m_indices )
					{
						vb = new VertexBuffer( (int)m_max_vertices, 0, m_parent.m_format );
					}
					else
					{
						num_indices = ( m_max_vertices / m_parent.m_vertices_per_primitive ) * m_parent.m_indices_per_primitive;
						vb = new VertexBuffer( (int)m_max_vertices, num_indices, m_parent.m_format );

						// note: the real number of indices we are going to need is not the one based on the power of 2 snapped 
						// number of vertices, but SetIndices must be passed a size that matches the created VertexBuffer size 
						// I think? (or you get a native function error)
						Common.Assert( num_indices <= m_parent.m_indices.Length);

						vb.SetIndices( m_parent.m_indices, 0, 0, num_indices );
					}

					m_free_list.Add( new Entry() { m_vertex_buffer = vb, m_frame_count = Common.FrameCount } );
				}

				Entry ret = m_free_list[ m_free_list.Count - 1 ];
				m_free_list.RemoveAt( m_free_list.Count - 1 );
				m_active_list.Add( ret );
				return ret.m_vertex_buffer;
			}
		}

		List< PerSizeList > m_per_size_lists;

		ushort[] m_indices;
		int m_vertices_per_primitive; // the number of vertices each ImmBegin is expected to have (relevant only if indices is set, 0 if not used)
		int m_indices_per_primitive; // the number of indices we want to draw for each primitive (relevant only if indices is set, 0 if not used)
		VertexFormat[] m_format;
		public int DisposeInterval = 30 * 60;

		public VertexBufferPool( ushort[] indices_model
								 , int vertices_per_primitive
								 , int indices_per_primitive
								 , params VertexFormat[] formats )
		{
			m_per_size_lists = new List< PerSizeList >();

			m_indices = indices_model;
			m_vertices_per_primitive = vertices_per_primitive;
			m_indices_per_primitive = indices_per_primitive;
			m_format = formats;

			for ( int p=0; p < 20; ++p )
			{
				m_per_size_lists.Add( new PerSizeList(1<<p) );
				m_per_size_lists[ m_per_size_lists.Count - 1 ].m_parent = this;
			}
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
				foreach ( PerSizeList entry in m_per_size_lists )
				{
					foreach ( Entry v in entry.m_active_list )
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					entry.m_active_list.Clear();

					foreach ( Entry v in entry.m_free_list )
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					entry.m_free_list.Clear();
				}

				m_disposed = true;
			}
		}

		// Call this when you know the frame has changed, so we can free all VertexBuffer objects again.
		public void OnFrameChanged()
		{
			foreach ( PerSizeList entry in m_per_size_lists )
			{
				foreach ( Entry v in entry.m_active_list )
				{
					#if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK_DISCARD_UNUSED_VERTEX_BUFFER
					// Discard the older VerteBuffer objects that haven't been used in a long time. Note: 
					// we could spread those discard more across frames, or clamp them to n per frame etc
					// (if that ever becomes a problem).

					if ( Common.FrameCount - v.m_frame_count > DisposeInterval )
					{
//						System.Console.WriteLine( Common.FrameCount+ " Disposing of a " + entry.m_max_vertices + " VertexBuffer" );
						Common.DisposeAndNullify< VertexBuffer >( ref v.m_vertex_buffer );
					}
					else 
					#endif
					{
						entry.m_free_list.Add( v );
					}
				}
				entry.m_active_list.Clear();
			}

//			Dump();
		}

		public VertexBuffer GetAVertexBuffer( int max_vertices )
		{
			int p = Math.Log2( max_vertices );
			if ( ( 1 << p ) < max_vertices ) ++p;

//			#ifdef DEBUG
//			System.Console.WriteLine( "max_vertices=" + max_vertices + " next=" + (1<<p) + " index=" + p );
//			#endif

			return m_per_size_lists[ p ].GetAVertexBuffer();
		}

		public void Dump()
		{
			foreach ( PerSizeList entry in m_per_size_lists )
			{
				if ( entry.m_free_list.Count == 0 && entry.m_active_list.Count == 0 )
					continue;

				System.Console.WriteLine( Common.FrameCount + " " + entry.m_max_vertices + " vertices : " + entry.m_free_list.Count + entry.m_active_list.Count );
			}
		}
	}

	#endif // #if IMMEDIATE_MODE_XPERIA_WORKAROUND_HACK
}

