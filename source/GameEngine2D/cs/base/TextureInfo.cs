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
	/// TextureInfo holds a Texture2D object and caches associated tile UV data. 
	/// The source region for tiling is not necesseraly the entire texture, it 
	/// can be any oriented box in UV domain. 
	/// TextureInfo takes ownership of the Texture2D object passed to it, and 
	/// disposes of it in its Dispose function.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>TextureInfoは、Texture2Dオブジェクトを保持し、関連付けられたタイルのUVデータをキャッシュします。
	/// タイルの転送元領域はテクスチャ全体である必要はなく、UV領域内の任意の大きさのボックスで指定することができます。
	/// TextureInfoは、渡されるTexture2Dオブジェクトの所有権を取得し、Dispose関数で破棄します。
	/// </summary>
	/// @endif
	public class TextureInfo : System.IDisposable
	{
		/// @if LANG_EN
		/// <summary>Cached UV information for each tile.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>各タイルのキャッシュされたUV情報。
		/// </summary>
		/// @endif
		public class CachedTileData
		{
			/// @if LANG_EN
			/// <summary>Bottom left point UV.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>左下のUVポイント。
			/// </summary>
			/// @endif
			public Vector2 UV_00;
			
			/// @if LANG_EN
			/// <summary>Bottom right point UV.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>右下のUVポイント。
			/// </summary>
			/// @endif
			public Vector2 UV_10;
			
			/// @if LANG_EN
			/// <summary>Top left point UV.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>左上のUVポイント。
			/// </summary>
			/// @endif
			public Vector2 UV_01;
			
			/// @if LANG_EN
			/// <summary>Top right point UV.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>右上のUVポイント。
			/// </summary>
			/// @endif
			public Vector2 UV_11;
		}
		/// @if LANG_EN
		/// <summary>The texture object.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>テクスチャのオブジェクト。
		/// </summary>
		/// @endif
		public Texture2D Texture;
		
		/// @if LANG_EN
		/// <summary>Return texture size in pixels as a Vector2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセル単位のテクスチャのサイズをVector2で返します。
		/// </summary>
		/// @endif
		public Vector2 TextureSizef { get { return new Vector2( Texture.Width, Texture.Height ); } } 
		
		/// @if LANG_EN
		/// <summary>Return texture size in pixels as a Vector2i.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセル単位のテクスチャのサイズをVector2iで返します。
		/// </summary>
		/// @endif
		public Vector2i TextureSizei { get { return new Vector2i( Texture.Width, Texture.Height ); } } 
		
		/// @if LANG_EN
		/// <summary>Return tile size in uv units.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>UV単位でタイルのサイズを返します。
		/// </summary>
		/// @endif
		public Vector2 TileSizeInUV;
		
		/// @if LANG_EN
		/// <summary>Return tile size in pixels as a Vector2. All tiles have the same size.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセル単位のタイルサイズを、Vector2で返します。すべてのタイルは同じサイズを持っています。
		/// </summary>
		/// @endif
		public Vector2 TileSizeInPixelsf { get { Common.Assert( Texture != null ); return TextureSizef * TileSizeInUV; } }
		
		/// @if LANG_EN
		/// <summary>Return the dimensions of the tiles grid.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>タイルグリッドの数を返します。
		/// </summary>
		/// @endif
		public Vector2i NumTiles; // default (1,1) for no tiles

		bool m_disposed = false;
		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このオブジェクトが破棄されている場合、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		CachedTileData[] m_tiles_uvs; // cache tiles uvs

		/// @if LANG_EN
		/// <summary>Return the CachedTileData (which contains tile UV information) for a given tile.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数で指定されたタイルのCachedTileData（タイルのUV情報が含まれている）を返します。
		/// </summary>
		/// @endif
		public CachedTileData GetCachedTiledData( ref Vector2i tile_index ) 
		{ 
//			Common.Assert( 0 <= tile_index.X && tile_index.X < NumTiles.X );
//			Common.Assert( 0 <= tile_index.Y && tile_index.Y < NumTiles.Y );
			return m_tiles_uvs[ tile_index.X + NumTiles.X * tile_index.Y ];
		}

		/// @if LANG_EN
		/// <summary>TextureInfo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public TextureInfo()
		{
		}

		/// @if LANG_EN
		/// <summary>TextureInfo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public TextureInfo( string filename )
		{
			Initialize( new Texture2D( filename, false ), Math._11i, TRS.Quad0_1 );
		}

		/// @if LANG_EN
		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// 注意: TextureInfoは、このコンストラクタに渡されるTexture2Dの所有権を取得し、Disposeでそれを破棄します。
		/// </summary>
		/// @endif
		public TextureInfo( Texture2D texture )
		{
			Initialize( texture, Math._11i, TRS.Quad0_1 );
		}

		/// @if LANG_EN
		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tile subdivisions on x and y.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// 注意: TextureInfoは、このコンストラクタに渡されるTexture2Dの所有権を取得し、Disposeでそれを破棄します。
		/// </summary>
		/// <param name="texture">テクスチャ。</param>
		/// <param name="num_tiles">xとyのタイル分割数。</param>
		/// @endif
		public TextureInfo( Texture2D texture, Vector2i num_tiles )
		{
			Initialize( texture, num_tiles, TRS.Quad0_1 );
		}

		/// @if LANG_EN
		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tile subdivisions on x and y.</param>
		/// <param name="source_area">The source rectangle, in UV domain, on which we are going to build the tiles (bottom left is 0,0).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// 注意: TextureInfoは、このコンストラクタに渡されるTexture2Dの所有権を取得し、Disposeでそれを破棄します。
		/// </summary>
		/// <param name="texture">テクスチャ。</param>
		/// <param name="num_tiles">xとyのタイル分割数。</param>
		/// <param name="source_area">UV領域内の転送元矩形。この矩形内を分割してタイルを構築します。左下が(0,0)になります。</param>
		/// @endif
		public TextureInfo( Texture2D texture, Vector2i num_tiles, TRS source_area  )
		{
			Initialize( texture, num_tiles, source_area );
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>オブジェクトの破棄。
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
//				System.Console.WriteLine( "TextureInfo Dispose() called!" );
				Common.DisposeAndNullify< Texture2D >( ref Texture );
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>The actual init function called by TextureInfo constructors.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tiles/cells, horitonally and vertically.</param>
		/// <param name="source_area">The source rectangle, in UV domain, on which we are going to build the tiles (bottom left is 0,0).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>TextureInfoコンストラクタによって呼び出される、実質的な初期化関数。
		/// </summary>
		/// <param name="texture">テクスチャ。</param>
		/// <param name="num_tiles">セル内のタイル数。水平方向と垂直方向の数を指定します。</param>
		/// <param name="source_area">UV領域内の転送元矩形。この矩形内を分割してタイルを構築します。左下が(0,0)になります。</param>
		/// @endif
		public void Initialize( Texture2D texture, Vector2i num_tiles, TRS source_area )
		{
			Texture = texture;
			TileSizeInUV = source_area.S / num_tiles.Vector2();  
			NumTiles = num_tiles;
			m_tiles_uvs = new CachedTileData[ num_tiles.Product() ];

			for ( int y=0; y < NumTiles.Y; ++y )
			{
				for ( int x=0; x < NumTiles.X; ++x )
				{
					Vector2i tile_index = new Vector2i( x, y );
					TRS tile = TRS.Tile( NumTiles, tile_index, source_area ); // lots of calculation duplicated, but this is an init function

					int index = tile_index.X + tile_index.Y * NumTiles.X;

					m_tiles_uvs[ index ] = new CachedTileData()
					{
						UV_00 = tile.Point00 ,
						UV_10 = tile.Point10 ,
						UV_01 = tile.Point01 ,
						UV_11 = tile.Point11 
					};
				}
			}
		}
	}
}

