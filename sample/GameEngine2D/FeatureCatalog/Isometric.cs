/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace FeatureCatalogIsometric
{
	// A quick isometric scene test with editor, using the data found there:
	// http://opengameart.org/content/isometric-64x64-outside-tileset
	// http://opengameart.org/sites/default/files/iso-64x64-outside.png

	public class IsometricScene
	: RawSpriteTileList
	, System.IDisposable
	{
		public TextureInfo TileMap;
		public TextureInfo ExampleImage;

		// each isometric cell can have at most 2 sprites
		public class Cell
		{
			public RawSpriteTile Data1; // bottom data
			public bool Data1Set;

			public RawSpriteTile Data2; // top data
			public bool Data2Set;

			public Cell()
			{
				Data1Set = false;
				Data2Set = false;
			}
		}

		Vector3i m_num_cells;
		public Vector3i NumCells { get { return m_num_cells;}}
		Cell[] m_cells;	// 3d voxel world, rendered with isometric tiles

		List< RawSpriteTile > m_blocks_list;

		public Vector2 X; // isometrix tile X support vector
		public Vector2 Y; // isometrix tile Y support vector
		public Matrix3 ToIso; // convert a world point to isometric coordinate (to find which cell it is on for example)
		public Matrix3 FromIso; // convert a point in isometric coordinates into world coordinates

		bool m_select_tile_screen = false;
		
		Vector2 m_world_edit_touch_pos; // mouse position in world edit screen
		Vector3i m_world_edit_cell { get { return new Vector3i( WhichIsoCell( m_world_edit_touch_pos ), m_edit_z ); } } 
		int m_edit_z; // edited z level
		bool m_second_object; // if true, set Data2 instead of Data1

		Camera2D m_tile_select_camera; // camera for tile select screen
		Vector2i m_selected_tile_unclamped;
		Vector2i m_selected_tile;
		bool is_erase_mode() { return m_selected_tile == -Math._11i;}
		void set_erase_mode() { m_selected_tile = -Math._11i;}
		bool navigating { get { return Input2.GamePad0.Triangle.Down || Input2.GamePad0.Square.Down; } }

		Camera2D m_gui_camera;
		
		public IsometricScene( Vector3i size )
		: base(null) 
		{
			TileMap = new TextureInfo( new Texture2D( "/Application/data/iso-64x64-outside.png", false ), new Vector2i(10,16) );
			TileMap.Texture.SetFilter( TextureFilterMode.Nearest );

			ExampleImage = new TextureInfo( new Texture2D( "/Application/data/iso-64x64-outside-example.png", false ) );
			ExampleImage.Texture.SetFilter( TextureFilterMode.Nearest );

			X = new Vector2( 2.0f,1.0f ).Normalize();
			Y = new Vector2( -2.0f,1.0f ).Normalize();

			// make sure width of a tile is 1
			float scale = Y.ProjectOnLine( Math._00, Math._10 ).Length() * 2.0f;
			X /= scale;
			Y /= scale;

			ToIso = new Matrix3( X.Xy0, Y.Xy0, Math._001 );
			FromIso = ToIso.Inverse();

			m_edit_z = 0;
			m_second_object = false;
			m_blocks_list = new List< RawSpriteTile >();

			m_num_cells = size;
			m_cells = new Cell[size.Product()];
			TextureInfo = TileMap;

			ScheduleUpdate();

			BlendMode = BlendMode.None;
			m_selected_tile = new Vector2i( 4, 10 );

			m_tile_select_camera = new Camera2D( Director.Instance.GL, Director.Instance.DrawHelpers );
			m_tile_select_camera.SetViewFromViewport();

			m_gui_camera = new Camera2D( Director.Instance.GL, Director.Instance.DrawHelpers );

//			Director.Instance.DebugFlags &= ~DebugFlags.DrawGrid; // shows the default grid (expensive)
		}

		public void Dispose()
		{
			Common.DisposeAndNullify< TextureInfo >( ref TileMap ); 
			Common.DisposeAndNullify< TextureInfo >( ref ExampleImage ); 
		}

		Cell CellAt( Vector3i index )
		{
			return m_cells[ index.X + m_num_cells.X * ( index.Y + index.Z * m_num_cells.Y ) ];
		}

		bool IsValidBlockIndex( Vector3i coord )
		{
			return coord == coord.Clamp( Math._000i, m_num_cells - Math._111i );
		}

		public void SetBlock( Vector3i coord, Vector2i tile_index, bool second )
		{
			Vector3i clamped_coord = coord.Clamp( Math._000i, m_num_cells - Math._111i );
			Common.Assert( clamped_coord == coord );

			int cell_index = coord.X + m_num_cells.X * ( coord.Y + coord.Z * m_num_cells.Y );

			if ( m_cells[ cell_index ] == null )
				m_cells[ cell_index ] = new Cell();

			TRS quad = TRS.Quad0_1;
			//each z level happens to corresponds to the iso tile neighbour at +(1,1), hence the +coord.Z below
			Vector2 p = X * ( coord.X + coord.Z ) + Y * ( coord.Y + coord.Z );
			quad.T = ( p + Y ).ProjectOnLine( p, Math._10 );
//			#if 0
			float d = ( p - quad.T ).Length() * 2.0f; // d should be =1
			quad.S = new Vector2( d, d );
//			#endif

			var data = new RawSpriteTile(quad,tile_index,false,false);

			if ( second )
			{
				m_cells[ cell_index ].Data2 = data;
				m_cells[ cell_index ].Data1Set = true;
			}
			else
			{
				m_cells[ cell_index ].Data1  = data;
				m_cells[ cell_index ].Data2Set = true;
			}
		}

		public void SetBlockRange( Vector3i coord, Vector2i tileindex, Vector2i tileoffset, Vector2i size )
		{
			for ( int y=0; y < size.Y; ++y )
			{
				for ( int x=0; x < size.X; ++x )
				{
					Vector2i delta = tileoffset + new Vector2i(x,y);
					// "hack" so that the flat tile indexes is superimposed with the iso layout
					// (and render the flat tiles in iso coords...) 
					// (won't work if trees overlap - although it will also save fillrate if you are 
					// ok with the artifacts)
					Vector3i coord_delta = coord + new Vector3i( new Vector2i(1,-1) * delta.X, delta.Y * 2 );

					// only add extra tiles that are within range
					if ( IsValidBlockIndex( coord_delta ) )
						SetBlock( coord_delta, tileindex + delta, false );
				}
			}
		}

		public void DeleteBlock( Vector3i coord )
		{
			Vector3i clamped_coord = coord.Clamp( Math._000i, m_num_cells - Math._111i );
			Common.Assert( clamped_coord == coord );

			int index = coord.X + m_num_cells.X * ( coord.Y + coord.Z * m_num_cells.Y );

			m_cells[ index ] = null;
		}

		public void DeleteAllBlocks()
		{
			for ( int i=0; i < m_cells.Length; ++i )
				m_cells[i] = null;
		}

		// this draws the grid at level zero
		void DrawIsometricTilesRulers()
		{
			Director.Instance.DrawHelpers.ShaderPush();

			{
				Director.Instance.GL.SetBlendMode( BlendMode.Additive );
				Vector4 col = Colors.Lime * 0.3f;

				Director.Instance.DrawHelpers.ImmBegin( DrawMode.Lines, (uint)(2*(m_num_cells.X+1+m_num_cells.Y+1)));
				for ( int y=0; y <= m_num_cells.Y; ++y )
				{
					Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( Math._00 + Y * y, col ) );
					Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( Math._00 + Y * y + X * m_num_cells.Y, col ) );
				}
				for ( int x=0; x <= m_num_cells.X; ++x )
				{
					Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( Math._00 + X * x, col ) );
					Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( Math._00 + X * x + Y * m_num_cells.X, col ) );
				}
				Director.Instance.DrawHelpers.ImmEnd();
			}

			DrawIsometricTileHighlight( WhichIsoCell( m_world_edit_touch_pos ), m_edit_z, is_erase_mode() ? Colors.White : Colors.Pink );

			Director.Instance.DrawHelpers.ShaderPop();
		}

		// draw the tiles that we are about to "put", with transparency
		void DrawBrush()
		{
			// for objects larger than one tile, you have to select the middle cell of the first, bottom row of the object

			Vector3i coord = m_world_edit_cell;

			if ( !IsValidIndex( coord ) )
				return;

			TRS quad = TRS.Quad0_1;
			//each z level happens to corresponds to the iso tile neighbour at +(1,1), hence the +coord.Z below
			Vector2 p = X * ( coord.X + coord.Z ) + Y * ( coord.Y + coord.Z );
			quad.T = ( p + Y ).ProjectOnLine( p, Math._10 );
//			#if 0
			float d = ( p - quad.T ).Length() * 2.0f; // d should be =1
			quad.S = new Vector2( d, d );
//			#endif
			var data = new RawSpriteTile(quad,m_selected_tile,false,false);

			m_blocks_list.Add( data );

			BlendMode = BlendMode.Normal;
			Color = Math.SetAlpha( Colors.White, Input2.GamePad0.Circle.Down ? 1.0f : 0.6f );

			flush_draw_blocks_list();
		}

		static Vector2 m_offset = new Vector2(0.246f,0.238f);
//		static Vector2 m_offset = new Vector2(0.066f,0.224f);

		// draw example source image underneath the editor to help positionning tiles
		void DrawExampleSourceImage()
		{ 
			Director.Instance.GL.SetBlendMode( BlendMode.Normal );
			Vector4 col = Math.SetAlpha( Colors.White, 0.5f );
			Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref col );
			Director.Instance.SpriteRenderer.DefaultShader.SetUVTransform( ref Math.UV_TransformFlipV );
			Director.Instance.SpriteRenderer.BeginSprites( ExampleImage, 1 );
			Director.Instance.SpriteRenderer.FlipU = false;
			Director.Instance.SpriteRenderer.FlipV = false;
			TRS quad = TRS.Quad0_1;
			quad.S = ExampleImage.TextureSizef / 64.0f;
//			System.Console.WriteLine( m_offset );
//			m_offset += Input2.GamePad0.Dpad * 0.002f;
			quad.T = new Vector2(-5.0f,3.0f) + m_offset;
			Director.Instance.SpriteRenderer.AddSprite( ref quad, new Vector2i(0,0) );
			Director.Instance.SpriteRenderer.EndSprites();
		}

		// draw the world in its current state
		void DrawWorld()
		{
			m_blocks_list.Clear();

			Vector3i index;
			for ( index.Z=0; index.Z< m_num_cells.Z; ++index.Z )
			{
				for ( index.Y=m_num_cells.Y-1; index.Y>=0; --index.Y )
				{
					for ( index.X=m_num_cells.X-1; index.X>=0; --index.X )
					{
						Cell cell = CellAt( index );

						if ( cell == null )
							continue;

						if ( cell.Data2Set ) m_blocks_list.Add( cell.Data1 );
						if ( cell.Data1Set ) m_blocks_list.Add( cell.Data2 );
					}
				}
			}

			BlendMode = BlendMode.Normal;
			Color = Colors.White;
			flush_draw_blocks_list();
		}

		// draw all style and highlight the selected one
		// clicking in empty space triggers erase mode
		void DrawTilesForSelection()
		{
			m_tile_select_camera.Push();

			Director.Instance.GL.SetBlendMode( BlendMode.None );
			Vector4 col = Colors.White;
			Director.Instance.SpriteRenderer.DefaultShader.SetColor( ref col );
			Director.Instance.SpriteRenderer.DefaultShader.SetUVTransform( ref Math.UV_TransformFlipV );
			Director.Instance.SpriteRenderer.BeginSprites( TileMap, TextureInfo.NumTiles.Product() );

			for ( int j=0; j < TextureInfo.NumTiles.Y; ++j )
			{
				for ( int i=0; i < TextureInfo.NumTiles.X; ++i )
				{
					Director.Instance.SpriteRenderer.FlipU = false;
					Director.Instance.SpriteRenderer.FlipV = false;
					TRS quad = TRS.Quad0_1;
					quad.T = new Vector2((float)i,(float)j) * TextureInfo.TileSizeInPixelsf;
					quad.S = TextureInfo.TileSizeInPixelsf;
					Director.Instance.SpriteRenderer.AddSprite( ref quad, new Vector2i(i,j) );
				}
			}

			Director.Instance.SpriteRenderer.EndSprites();

			if ( !is_erase_mode() )
			{
				// highlight selected tile
				TRS quad = TRS.Quad0_1;
				quad.T = m_selected_tile.Vector2() * TextureInfo.TileSizeInPixelsf;
				quad.S = TextureInfo.TileSizeInPixelsf;
				Director.Instance.DrawHelpers.SetColor( Colors.Pink );
				Director.Instance.DrawHelpers.DrawBounds2( quad.Bounds2() ) ;
			}

			{
				Director.Instance.GL.SetBlendMode( BlendMode.Normal );
				Director.Instance.DrawHelpers.DrawDefaultGrid( m_tile_select_camera.CalcBounds()
															   , TextureInfo.TileSizeInPixelsf, Math.SetAlpha( Colors.LightBlue, 0.2f ), Colors.LightBlue );
			}

			m_tile_select_camera.Pop();

		}

		public override void Draw()
		{
			DepthFunc depth_func = Director.Instance.GL.Context.GetDepthFunc();
			Director.Instance.GL.Context.Disable( EnableMode.DepthTest );

			if ( !m_select_tile_screen )
			{
//				DrawExampleSourceImage();

				DrawWorld();

				if ( !is_erase_mode() && !navigating )
					DrawBrush();

				DrawIsometricTilesRulers();

				{
					m_gui_camera.SetViewFromViewport();
					m_gui_camera.Push();

					float h = m_gui_camera.GetPixelSize() * EmbeddedDebugFontData.CharSizei.Y;
					Vector2 line_incr = new Vector2( 0, ( EmbeddedDebugFontData.CharSizei.Y + 1 ) * m_gui_camera.GetPixelSize() );
					Vector2 pos = line_incr * 9;

					Vector3i coord = m_world_edit_cell;
					
					string text="";
					text += "commands:\n";
					text += "- triangle/W + drag          : zoom\n";
					text += "- square/A + drag            : translate\n";
					text += "- hold cross/S               : bring tile image select screen\n";
					text += "- hold circle/D + touch/drag : put tile at selected world cell\n";
					text += "- up                         : increment edit z\n";
					text += "- down                       : decrement edit z\n";
					text += "- right                      : toggle secondary object\n";

					text += "SECOND OBJECT=" + m_second_object + "\n";

					if ( IsValidIndex( coord ) ) 
						text += "SELECTED TILE=" + coord + "\n";

					Director.Instance.GL.SetBlendMode( BlendMode.Normal );
					Director.Instance.SpriteRenderer.DrawTextDebug(text, pos, h );
					m_gui_camera.Pop();
				}
			}
			else
			{
				DrawTilesForSelection();

				{
					m_gui_camera.SetViewFromViewport();
					m_gui_camera.Push();

					float h = m_gui_camera.GetPixelSize() * EmbeddedDebugFontData.CharSizei.Y;
					Vector2 line_incr = new Vector2( 0, ( EmbeddedDebugFontData.CharSizei.Y + 1 ) * m_gui_camera.GetPixelSize() );
					Vector2 pos = line_incr * 4;

					Director.Instance.GL.SetBlendMode( BlendMode.Normal );

					string text="";
					text += "commands:\n";
					text += "- touch             : scroll the tile map and select a tile\n";
					text += "- touch empty space : set erase mode\n";
					text += "- release cross/S   : back to world edit screen\n";

					text += "SELECTED TILE=" + m_selected_tile + "\n";

					Director.Instance.SpriteRenderer.DrawTextDebug( text, pos, h );
					m_gui_camera.Pop();
				}
			}

			Director.Instance.GL.Context.SetDepthFunc( depth_func );
		}

		Vector2i WhichCell( Vector2 value )
		{
			int x = (int)value.X;
			int y = (int)value.Y;
			if ( value.X < 0 ) --x;
			if ( value.Y < 0 ) --y;
			return new Vector2i( x, y );
		}

		Vector2i WhichIsoCell( Vector2 value )
		{
			value = ( FromIso * value.Xy1 ).Xy;

			int x = (int)value.X;
			int y = (int)value.Y;
			if ( value.X < 0 ) --x;
			if ( value.Y < 0 ) --y;
			return new Vector2i( x, y );
		}

		bool IsValidIndex( Vector3i index )
		{
			return index.Xy.ClampIndex( m_num_cells.Xy ) == index.Xy;
		}

		void DrawIsometricTileHighlight( Vector2i index, int z, Vector4 col )
		{
			Director.Instance.DrawHelpers.SetColor( col );

			Vector4 colz = Math.SetAlpha( col, 0.5f );

			Vector2 p0 = index.X * X + index.Y * Y;
			Vector2 p1 = p0 + X;
			Vector2 p2 = p0 + X+Y;
			Vector2 p3 = p0 + Y;

			Vector2 p0z = ( index.X + z ) * X + ( index.Y + z ) * Y;
			Vector2 p1z = p0z + X;
			Vector2 p2z = p0z + X+Y;
			Vector2 p3z = p0z + Y;

			Director.Instance.GL.SetBlendMode( BlendMode.Normal );

			//ground highlight
			Director.Instance.DrawHelpers.ImmBegin( DrawMode.Lines, (uint)3*2*4 );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0, col ) );

			//top highlight at altitude z
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0z, colz ) );

			// vertical lines that connect ground and top highlights
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p0z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p1z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p2z, colz ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3, col ) );
			Director.Instance.DrawHelpers.ImmVertex( new DrawHelpers.Vertex( p3z, colz ) );

			Director.Instance.DrawHelpers.ImmEnd();
		}

		public override void Update( float dt )
		{
			m_select_tile_screen = Input2.GamePad0.Cross.Down;

			if ( m_select_tile_screen )
			{
				if ( Input2.Touch00.Down )
					m_selected_tile_unclamped = WhichCell( m_tile_select_camera.GetTouchPos() / TextureInfo.TileSizeInPixelsf );

				if ( m_selected_tile_unclamped == m_selected_tile_unclamped.ClampIndex( TextureInfo.NumTiles ) )
					m_selected_tile = m_selected_tile_unclamped;
				else set_erase_mode(); // invalid tile => erase mode

				bool zoom = false;//Input2.GamePad0.Triangle.Down; // zoom = A key on PC
				bool pan = true;//Input2.GamePad0.Square.Down; // pan = W key on PC

				m_tile_select_camera.Navigate( pan ? 1 : zoom ? 2 : 0 );
			}
			else
			{
				if ( Input2.Touch00.Down && !navigating )
				{
					m_world_edit_touch_pos = GetTouchPos();

					if ( Input2.GamePad0.Circle.Down )
					{
						Vector3i coord = m_world_edit_cell;

						if ( IsValidIndex(coord) )
						{
							if ( is_erase_mode() ) DeleteBlock( coord );
							else
							{
								// for objects larger than one tile, you have to select the middle cell of the first, bottom row of the object

								if ( m_selected_tile == new Vector2i(0,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,3) );
								if ( m_selected_tile == new Vector2i(1,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,3) );
								if ( m_selected_tile == new Vector2i(2,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,2) );
								if ( m_selected_tile == new Vector2i(3,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,2) );

								if ( m_selected_tile == new Vector2i(5,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(-1,0), new Vector2i(3,3) );
								if ( m_selected_tile == new Vector2i(8,0) )	SetBlockRange( coord, m_selected_tile, new Vector2i(-1,0), new Vector2i(3,3) );

								if ( m_selected_tile == new Vector2i(2,2) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,2) );
								if ( m_selected_tile == new Vector2i(3,2) )	SetBlockRange( coord, m_selected_tile, new Vector2i(0,0), new Vector2i(1,2) );

								else SetBlock( coord, m_selected_tile, m_second_object );
							}
						}
					}
				}

				// some exciting game pad editing controls
				if ( Input2.GamePad0.Up.Press )	m_edit_z = Common.Min( ( m_num_cells.Z - 1 ), ( m_edit_z + 1 ) );
				if ( Input2.GamePad0.Down.Press ) m_edit_z = Common.Max( 0, m_edit_z-1);
				if ( Input2.GamePad0.Right.Press ) m_second_object=!m_second_object;

				if ( Input2.GamePad0.L.Press )
				{
					System.Console.WriteLine( "---" );

					// serialize as C# we can copy paste in the init function

					Vector3i index;
					for ( index.Z=0; index.Z< m_num_cells.Z; ++index.Z )
					{
						for ( index.Y=m_num_cells.Y-1; index.Y>=0; --index.Y )
						{
							for ( index.X=m_num_cells.X-1; index.X>=0; --index.X )
							{
								Cell cell = CellAt( index );

								if ( cell == null )
									continue;

								if ( cell.Data2Set )
									System.Console.WriteLine( "SetBlock( new Vector3i("
															  + index.X + ","
															  + index.Y + ","
															  + index.Z + "), new Vector2i(" 
															  + cell.Data1.TileIndex2D.X + "," 
															  + cell.Data1.TileIndex2D.Y + "), false );" );

								if ( cell.Data1Set )
									System.Console.WriteLine( "SetBlock( new Vector3i("
															  + index.X + ","
															  + index.Y + ","
															  + index.Z + "), new Vector2i(" 
															  + cell.Data2.TileIndex2D.X + "," 
															  + cell.Data2.TileIndex2D.Y + "), true );" );
							}
						}
					}
				}
			}
		}

		// draw all world blocks and clear the list
		public void flush_draw_blocks_list()
		{
			Common.Swap< List< RawSpriteTile > >( ref Sprites, ref m_blocks_list );
			TextureInfo = TileMap;
			base.Draw();
			m_blocks_list.Clear();
		}

		public void CreateTestScene()
		{
			SetBlock( new Vector3i(5,14,0), new Vector2i(4,10), true );
			SetBlock( new Vector3i(4,14,0), new Vector2i(4,10), true );
			SetBlock( new Vector3i(16,13,0), new Vector2i(3,14), false );
			SetBlock( new Vector3i(7,13,0), new Vector2i(4,10), true );
			SetBlock( new Vector3i(6,13,0), new Vector2i(5,9), true );
			SetBlock( new Vector3i(5,13,0), new Vector2i(5,10), false );
			SetBlock( new Vector3i(4,13,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,13,0), new Vector2i(2,10), true );
			SetBlock( new Vector3i(3,13,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(2,13,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(17,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(16,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(15,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(14,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(13,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(12,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(11,12,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(10,12,0), new Vector2i(5,10), true );
			SetBlock( new Vector3i(9,12,0), new Vector2i(5,10), true );
			SetBlock( new Vector3i(8,12,0), new Vector2i(4,10), true );
			SetBlock( new Vector3i(7,12,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(7,12,0), new Vector2i(1,10), true );
			SetBlock( new Vector3i(6,12,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(6,12,0), new Vector2i(1,6), true );
			SetBlock( new Vector3i(5,12,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,12,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(3,12,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(2,12,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(17,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(16,11,0), new Vector2i(3,14), false );
			SetBlock( new Vector3i(15,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(14,11,0), new Vector2i(3,0), false );
			SetBlock( new Vector3i(13,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(12,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(11,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(10,11,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(9,11,0), new Vector2i(5,10), true );
			SetBlock( new Vector3i(8,11,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(8,11,0), new Vector2i(2,10), true );
			SetBlock( new Vector3i(7,11,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(7,11,0), new Vector2i(6,3), true );
			SetBlock( new Vector3i(6,11,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(6,11,0), new Vector2i(4,6), true );
			SetBlock( new Vector3i(5,11,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(5,11,0), new Vector2i(3,6), true );
			SetBlock( new Vector3i(4,11,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(3,11,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(3,11,0), new Vector2i(0,0), true );
			SetBlock( new Vector3i(2,11,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(17,10,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(17,10,0), new Vector2i(1,4), true );
			SetBlock( new Vector3i(16,10,0), new Vector2i(8,12), false );
			SetBlock( new Vector3i(15,10,0), new Vector2i(3,12), false );
			SetBlock( new Vector3i(14,10,0), new Vector2i(3,2), false );
			SetBlock( new Vector3i(13,10,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(12,10,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(11,10,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(10,10,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(9,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(9,10,0), new Vector2i(0,8), true );
			SetBlock( new Vector3i(8,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(8,10,0), new Vector2i(6,3), true );
			SetBlock( new Vector3i(7,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(6,10,0), new Vector2i(0,15), false );
			SetBlock( new Vector3i(5,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(5,10,0), new Vector2i(1,6), true );
			SetBlock( new Vector3i(4,10,0), new Vector2i(4,15), false );
			SetBlock( new Vector3i(3,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(2,10,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(18,9,0), new Vector2i(2,4), true );
			SetBlock( new Vector3i(17,9,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(17,9,0), new Vector2i(0,4), true );
			SetBlock( new Vector3i(16,9,0), new Vector2i(5,12), false );
			SetBlock( new Vector3i(15,9,0), new Vector2i(0,12), false );
			SetBlock( new Vector3i(14,9,0), new Vector2i(3,13), false );
			SetBlock( new Vector3i(14,9,0), new Vector2i(2,2), true );
			SetBlock( new Vector3i(12,9,0), new Vector2i(0,14), false );
			SetBlock( new Vector3i(11,9,0), new Vector2i(0,14), false );
			SetBlock( new Vector3i(10,9,0), new Vector2i(0,14), false );
			SetBlock( new Vector3i(9,9,0), new Vector2i(0,14), false );
			SetBlock( new Vector3i(8,9,0), new Vector2i(1,14), false );
			SetBlock( new Vector3i(7,9,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(6,9,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(6,9,0), new Vector2i(9,7), true );
			SetBlock( new Vector3i(5,9,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(5,9,0), new Vector2i(6,6), true );
			SetBlock( new Vector3i(4,9,0), new Vector2i(4,15), false );
			SetBlock( new Vector3i(4,9,0), new Vector2i(7,7), true );
			SetBlock( new Vector3i(3,9,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(2,9,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(15,8,0), new Vector2i(0,0), false );
			SetBlock( new Vector3i(15,8,0), new Vector2i(0,0), true );
			SetBlock( new Vector3i(14,8,0), new Vector2i(6,12), false );
			SetBlock( new Vector3i(13,8,0), new Vector2i(3,12), false );
			SetBlock( new Vector3i(12,8,0), new Vector2i(3,15), false );
			SetBlock( new Vector3i(11,8,0), new Vector2i(0,14), false );
			SetBlock( new Vector3i(10,8,0), new Vector2i(8,12), false );
			SetBlock( new Vector3i(9,8,0), new Vector2i(6,12), false );
			SetBlock( new Vector3i(8,8,0), new Vector2i(3,12), false );
			SetBlock( new Vector3i(7,8,0), new Vector2i(3,14), false );
			SetBlock( new Vector3i(6,8,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(6,8,0), new Vector2i(6,7), true );
			SetBlock( new Vector3i(5,8,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(5,8,0), new Vector2i(5,7), true );
			SetBlock( new Vector3i(4,8,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,8,0), new Vector2i(5,7), true );
			SetBlock( new Vector3i(3,8,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(2,8,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(16,7,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(14,7,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(13,7,0), new Vector2i(3,11), false );
			SetBlock( new Vector3i(12,7,0), new Vector2i(6,12), false );
			SetBlock( new Vector3i(11,7,0), new Vector2i(6,12), false );
			SetBlock( new Vector3i(10,7,0), new Vector2i(0,11), false );
			SetBlock( new Vector3i(9,7,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(8,7,0), new Vector2i(1,12), false );
			SetBlock( new Vector3i(7,7,0), new Vector2i(3,14), false );
			SetBlock( new Vector3i(6,7,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(5,7,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(5,7,0), new Vector2i(8,7), true );
			SetBlock( new Vector3i(4,7,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,7,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(3,7,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(16,6,0), new Vector2i(2,12), false );
			SetBlock( new Vector3i(16,6,0), new Vector2i(2,12), true );
			SetBlock( new Vector3i(13,6,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(12,6,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(11,6,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(10,6,0), new Vector2i(2,11), false );
			SetBlock( new Vector3i(9,6,0), new Vector2i(2,12), false );
			SetBlock( new Vector3i(8,6,0), new Vector2i(0,12), false );
			SetBlock( new Vector3i(8,6,0), new Vector2i(7,3), true );
			SetBlock( new Vector3i(7,6,0), new Vector2i(0,13), false );
			SetBlock( new Vector3i(6,6,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(5,6,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(4,6,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,6,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(3,6,0), new Vector2i(1,15), true );
			SetBlock( new Vector3i(13,5,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(12,5,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(11,5,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(10,5,0), new Vector2i(0,12), false );
			SetBlock( new Vector3i(9,5,0), new Vector2i(4,14), false );
			SetBlock( new Vector3i(9,5,0), new Vector2i(9,7), true );
			SetBlock( new Vector3i(8,5,0), new Vector2i(3,14), false );
			SetBlock( new Vector3i(8,5,0), new Vector2i(2,6), true );
			SetBlock( new Vector3i(7,5,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(7,5,0), new Vector2i(3,6), true );
			SetBlock( new Vector3i(6,5,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(4,5,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(13,4,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(12,4,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(11,4,0), new Vector2i(2,11), false );
			SetBlock( new Vector3i(10,4,0), new Vector2i(2,15), false );
			SetBlock( new Vector3i(10,4,0), new Vector2i(9,7), true );
			SetBlock( new Vector3i(9,4,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(9,4,0), new Vector2i(0,6), true );
			SetBlock( new Vector3i(8,4,0), new Vector2i(5,15), true );
			SetBlock( new Vector3i(7,4,0), new Vector2i(5,15), true );
			SetBlock( new Vector3i(13,3,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(12,3,0), new Vector2i(4,12), false );
			SetBlock( new Vector3i(11,3,0), new Vector2i(1,12), false );
			SetBlock( new Vector3i(10,3,0), new Vector2i(1,15), false );
			SetBlock( new Vector3i(10,3,0), new Vector2i(8,7), true );
			SetBlock( new Vector3i(9,3,0), new Vector2i(0,14), true );
			SetBlock( new Vector3i(8,3,0), new Vector2i(5,15), true );
			SetBlock( new Vector3i(7,3,0), new Vector2i(5,15), true );
			SetBlock( new Vector3i(13,2,0), new Vector2i(2,11), false );
			SetBlock( new Vector3i(12,2,0), new Vector2i(0,12), false );
			SetBlock( new Vector3i(11,2,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(11,2,0), new Vector2i(9,7), true );
			SetBlock( new Vector3i(10,2,0), new Vector2i(6,6), true );
			SetBlock( new Vector3i(9,2,0), new Vector2i(6,7), false );
			SetBlock( new Vector3i(9,2,0), new Vector2i(0,14), true );
			SetBlock( new Vector3i(12,1,0), new Vector2i(2,13), false );
			SetBlock( new Vector3i(12,1,0), new Vector2i(9,7), true );
			SetBlock( new Vector3i(11,1,0), new Vector2i(6,6), true );
			SetBlock( new Vector3i(10,1,0), new Vector2i(4,7), true );
			SetBlock( new Vector3i(5,14,1), new Vector2i(4,10), true );
			SetBlock( new Vector3i(4,14,1), new Vector2i(3,10), true );
			SetBlock( new Vector3i(7,13,1), new Vector2i(4,10), true );
			SetBlock( new Vector3i(6,13,1), new Vector2i(2,9), true );
			SetBlock( new Vector3i(5,13,1), new Vector2i(1,10), false );
			SetBlock( new Vector3i(10,12,1), new Vector2i(4,10), true );
			SetBlock( new Vector3i(9,12,1), new Vector2i(5,10), true );
			SetBlock( new Vector3i(8,12,1), new Vector2i(5,10), true );
			SetBlock( new Vector3i(8,8,1), new Vector2i(7,0), false );
			SetBlock( new Vector3i(15,7,1), new Vector2i(0,10), false );
			SetBlock( new Vector3i(9,7,1), new Vector2i(8,0), false );
			SetBlock( new Vector3i(14,6,1), new Vector2i(5,10), false );
			SetBlock( new Vector3i(13,6,1), new Vector2i(1,10), false );
			SetBlock( new Vector3i(12,6,1), new Vector2i(7,9), false );
			SetBlock( new Vector3i(10,6,1), new Vector2i(9,0), false );
			SetBlock( new Vector3i(14,5,1), new Vector2i(0,12), false );
			SetBlock( new Vector3i(13,5,1), new Vector2i(0,8), false );
			SetBlock( new Vector3i(12,5,1), new Vector2i(0,3), false );
			SetBlock( new Vector3i(13,4,1), new Vector2i(1,3), false );
			SetBlock( new Vector3i(12,4,1), new Vector2i(1,3), false );
			SetBlock( new Vector3i(13,3,1), new Vector2i(7,14), false );
			SetBlock( new Vector3i(7,16,2), new Vector2i(5,10), true );
			SetBlock( new Vector3i(7,15,2), new Vector2i(5,10), true );
			SetBlock( new Vector3i(6,15,2), new Vector2i(5,10), true );
			SetBlock( new Vector3i(7,14,2), new Vector2i(3,9), true );
			SetBlock( new Vector3i(6,14,2), new Vector2i(5,9), true );
			SetBlock( new Vector3i(5,14,2), new Vector2i(4,10), true );
			SetBlock( new Vector3i(8,13,2), new Vector2i(5,10), true );
			SetBlock( new Vector3i(7,13,2), new Vector2i(4,10), true );
			SetBlock( new Vector3i(6,13,2), new Vector2i(4,9), true );
			SetBlock( new Vector3i(9,12,2), new Vector2i(4,10), true );
			SetBlock( new Vector3i(8,12,2), new Vector2i(0,10), true );
			SetBlock( new Vector3i(14,11,2), new Vector2i(3,1), false );
			SetBlock( new Vector3i(3,11,2), new Vector2i(0,1), false );
			SetBlock( new Vector3i(14,10,2), new Vector2i(3,3), false );
			SetBlock( new Vector3i(14,9,2), new Vector2i(2,3), false );
			SetBlock( new Vector3i(15,8,2), new Vector2i(0,1), false );
			SetBlock( new Vector3i(5,14,3), new Vector2i(2,10), true );
			SetBlock( new Vector3i(8,8,3), new Vector2i(7,1), false );
			SetBlock( new Vector3i(9,7,3), new Vector2i(8,1), false );
			SetBlock( new Vector3i(10,6,3), new Vector2i(9,1), false );
			SetBlock( new Vector3i(3,11,4), new Vector2i(0,2), false );
			SetBlock( new Vector3i(15,8,4), new Vector2i(0,2), false );
			SetBlock( new Vector3i(8,8,5), new Vector2i(7,2), false );
			SetBlock( new Vector3i(9,7,5), new Vector2i(8,2), false );
			SetBlock( new Vector3i(10,6,5), new Vector2i(9,2), false );
		}
	}
}

