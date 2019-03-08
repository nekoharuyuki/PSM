/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace FeatureCatalogPlanetCute
{
	// see http://www.lostgarden.com/2007/05/dancs-miraculously-flexible-game.html 

	public class PlanetCute
		: RawSpriteTileList
		, System.IDisposable
	{
		public TextureInfo ObjectsMap = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/Objects.png", false ), new Vector2i(7,3) );
		public TextureInfo ShadowsMap = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/Shadows.png", false ), new Vector2i(3,3) );
		public TextureInfo BlocksMap = new TextureInfo( new Texture2D( "/Application/data/PlanetCute/Blocks.png", false ), new Vector2i(5,5) );

		// Blocks.png
		public static Vector2i BrownBlock = new Vector2i(0,0);
		public static Vector2i DirtBlock = new Vector2i(1,0);
		public static Vector2i DoorTallClosed = new Vector2i(2,0);
		public static Vector2i DoorTallOpen = new Vector2i(3,0);
		public static Vector2i GrassBlock = new Vector2i(4,0);
		public static Vector2i PlainBlock = new Vector2i(0,1);
		public static Vector2i RampEast = new Vector2i(1,1);
		public static Vector2i RampNorth = new Vector2i(2,1);
		public static Vector2i RampSouth = new Vector2i(3,1);
		public static Vector2i RampWest = new Vector2i(4,1);
		public static Vector2i RoofEast = new Vector2i(0,2);
		public static Vector2i RoofNorth = new Vector2i(1,2);
		public static Vector2i RoofNorthEast = new Vector2i(2,2);
		public static Vector2i RoofNorthWest = new Vector2i(3,2);
		public static Vector2i RoofSouth = new Vector2i(4,2);
		public static Vector2i RoofSouthEast = new Vector2i(0,3);
		public static Vector2i RoofSouthWest = new Vector2i(1,3);
		public static Vector2i RoofWest = new Vector2i(2,3);
		public static Vector2i StoneBlock = new Vector2i(3,3);
		public static Vector2i StoneBlockTall = new Vector2i(4,3);
		public static Vector2i WallBlock = new Vector2i(0,4);
		public static Vector2i WallBlockTall = new Vector2i(1,4);
		public static Vector2i WaterBlock = new Vector2i(2,4);
		public static Vector2i WindowTall = new Vector2i(3,4);
		public static Vector2i WoodBlock = new Vector2i(4,4);
		// Objects.png
		public static Vector2i CharacterBoy = new Vector2i(0,0);
		public static Vector2i CharacterCatGirl = new Vector2i(1,0);
		public static Vector2i CharacterHornGirl = new Vector2i(2,0);
		public static Vector2i CharacterPinkGirl = new Vector2i(3,0);
		public static Vector2i CharacterPrincessGirl = new Vector2i(4,0);
		public static Vector2i ChestClosed = new Vector2i(5,0);
		public static Vector2i ChestLid = new Vector2i(6,0);
		public static Vector2i ChestOpen = new Vector2i(5,1);
		public static Vector2i EnemyBug = new Vector2i(1,1);
		public static Vector2i GemBlue = new Vector2i(2,1);
		public static Vector2i GemGreen = new Vector2i(3,1);
		public static Vector2i GemOrange = new Vector2i(4,1);
		public static Vector2i Heart = new Vector2i(0,1);
		public static Vector2i Key = new Vector2i(6,1);
		public static Vector2i Rock = new Vector2i(0,2);
		public static Vector2i Selector = new Vector2i(1,2);
		public static Vector2i SpeechBubble = new Vector2i(2,2);
		public static Vector2i Star = new Vector2i(3,2);
		public static Vector2i TreeShort = new Vector2i(4,2);
		public static Vector2i TreeTall = new Vector2i(5,2);
		public static Vector2i TreeUgly = new Vector2i(6,2);
		// Shadows.png
		public static Vector2i ShadowEast = new Vector2i(0,0);
		public static Vector2i ShadowNorth = new Vector2i(1,0);
		public static Vector2i ShadowNorthEast = new Vector2i(2,0);
		public static Vector2i ShadowNorthWest = new Vector2i(0,1);
		public static Vector2i ShadowSideWest = new Vector2i(1,1);
		public static Vector2i ShadowSouth = new Vector2i(2,1);
		public static Vector2i ShadowSouthEast = new Vector2i(0,2);
		public static Vector2i ShadowSouthWest = new Vector2i(1,2);
		public static Vector2i ShadowWest = new Vector2i(2,2);

		public class Cell
		{
			public RawSpriteTile Data;
			public bool IsTopMost = false;
		}

		Vector3i m_num_cells;
		Cell[] m_cells; // just brute force a simple 3d grid (voxel world)
		int[] m_z_count;

		public Vector2 SpriteSize;
		Vector2 m_cell_size;
		float m_stack_height;

		List< RawSpriteTile > m_shadows_list = new List< RawSpriteTile >();
		List< RawSpriteTile > m_blocks_list = new List< RawSpriteTile >();

		public PlanetCute( Vector3i size )
		: base(null) 
		{
			m_num_cells = size;
			m_cells = new Cell[size.Product()];
			m_z_count = new int[size.Xy.Product()];
			for ( int i=0; i < m_z_count.Length; ++i )
				m_z_count[i]=0;
			TextureInfo = BlocksMap;
			Vector2 pixel_size = CalcSizeInPixels();
			SpriteSize = pixel_size / pixel_size.X;
			m_cell_size = SpriteSize;
			m_cell_size.Y *= (80.0f/pixel_size.Y);
			m_stack_height = SpriteSize.Y * (40.0f/pixel_size.Y);
		}

		public void Dispose()
		{
//			System.Console.WriteLine( "PlanetCute.Dispose got called!" );
			ObjectsMap.Dispose();
			ShadowsMap.Dispose();
			BlocksMap.Dispose();
		}

		public Vector2 GetCellPos( Vector3 coord )
		{
			return m_cell_size * coord.Xy + new Vector2( 0, coord.Z * m_stack_height );
		}

		Cell CellAt( Vector3i index )
		{
			return m_cells[ index.X + m_num_cells.X * ( index.Y + index.Z * m_num_cells.Y ) ];
		}

		Cell SafeCellAt( Vector3i index )
		{
			Vector3i clamped_index = index.Clamp( Math._000i, m_num_cells - Math._111i );
			if ( index != clamped_index )
				return null;

			return CellAt( index );
		}

		void update_top_most_flag( Vector3i index )
		{
			int index1d = index.X + m_num_cells.X * ( index.Y + 0 * m_num_cells.Y );
			int zincr = m_num_cells.X * m_num_cells.Y;
			int last_valid = -1;

			for ( index.Z=0; index.Z< m_num_cells.Z; ++index.Z )
			{
				if ( m_cells[ index1d ] != null )
				{
					m_cells[ index1d ].IsTopMost = false;
					last_valid = index1d;
				}

				index1d += zincr;
			}

			if ( last_valid != -1 )
				m_cells[ last_valid ].IsTopMost = true;
		}

		public void SetBlock( Vector3i coord, Vector2i tile_index )
		{
			Vector3i clamped_coord = coord.Clamp( Math._000i, m_num_cells - Math._111i );
			Common.Assert( clamped_coord == coord );

			int cell_index = coord.X + m_num_cells.X * ( coord.Y + coord.Z * m_num_cells.Y );

			if ( m_cells[ cell_index ] == null )
				m_cells[ cell_index ] = new Cell();

			TRS trs = TRS.Quad0_1;
			trs.S = SpriteSize;
			trs.T = GetCellPos( coord.Vector3() );

			m_z_count[ coord.X + coord.Y * m_num_cells.Y ]++;
			m_cells[ cell_index ].Data = new RawSpriteTile(trs,tile_index,false,false);
			update_top_most_flag( coord );
		}

		public void DeleteBlock( Vector3i coord )
		{
			Vector3i clamped_coord = coord.Clamp( Math._000i, m_num_cells - Math._111i );
			Common.Assert( clamped_coord == coord );

			int index = coord.X + m_num_cells.X * ( coord.Y + coord.Z * m_num_cells.Y );

			if ( m_cells[ index ] != null )
				m_z_count[ coord.X + coord.Y * m_num_cells.Y ]--;

			m_cells[ index ] = null;
			update_top_most_flag( coord );
		}

		public override void Draw()
		{
			m_shadows_list.Clear();
			m_blocks_list.Clear();

			bool do_shadows = true;
			
			Vector3i index;
			for ( index.Z=0; index.Z< m_num_cells.Z; ++index.Z )
			{
				for ( index.Y=m_num_cells.Y-1; index.Y>=0; --index.Y )
				{
					for ( index.X=0; index.X < m_num_cells.X; ++index.X )
					{
						Cell cell = CellAt( index );

						if ( cell == null ) 
							continue;
						
						m_blocks_list.Add( cell.Data );

						if ( !do_shadows )
							continue;

//						if ( SafeCellAt( index + new Vector3i( 0,0,1 ) ) == null )
						if ( cell.IsTopMost )
						{
							uint surround=0;

							if ( SafeCellAt( index + new Vector3i( -1,-1,1 ) ) != null ) surround |= 1;
							if ( SafeCellAt( index + new Vector3i( 0,-1,1 ) ) != null )	surround |= 2;
							if ( SafeCellAt( index + new Vector3i( 1,-1,1 ) ) != null )	surround |= 4;
							if ( SafeCellAt( index + new Vector3i( 1,0,1 ) ) != null ) surround |= 8;
							if ( SafeCellAt( index + new Vector3i( 1,1,1 ) ) != null ) surround |= 16;
							if ( SafeCellAt( index + new Vector3i( 0,1,1 ) ) != null ) surround |= 32;
							if ( SafeCellAt( index + new Vector3i( -1,1,1 ) ) != null )	surround |= 64;
							if ( SafeCellAt( index + new Vector3i( -1,0,1 ) ) != null )	surround |= 128;

							if ( (surround&(4|8))==4 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowSouthEast ) );
							if ( (surround&(1|128))==1 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowSouthWest ) );
							if ( (surround&(8|16|32))==16 )	m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowNorthEast ) );
							if ( (surround&(32|64|128))==64 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowNorthWest ) );
							if ( (surround&(2))==2 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowSouth ) );
							if ( (surround&(8))==8 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowEast ) );
							if ( (surround&(32))==32 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowNorth ) );
							if ( (surround&(128))==128 ) m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowWest ) );
						}

						if ( ( SafeCellAt( index + new Vector3i( -1,-1,0 ) ) != null ) 
							 && ( SafeCellAt( index + new Vector3i( 0,-1,0 ) ) == null ) )
							m_shadows_list.Add( new RawSpriteTile( cell.Data.Quad, ShadowSideWest ) );
					}
				}

				if ( do_shadows )
				{
					flush_draw_blocks_list();
					flush_draw_shadows_list();
				}
			}

			if ( !do_shadows )
			{
				flush_draw_blocks_list();
				flush_draw_shadows_list();
			}
		}

		// draw all world blocks and clear the list
		public void flush_draw_blocks_list()
		{
			Common.Swap< List< RawSpriteTile > >( ref Sprites, ref m_blocks_list );
			TextureInfo = BlocksMap;
			BlendMode = BlendMode.Normal;
			base.Draw();
			m_blocks_list.Clear();
		}

		// draw all shadow sprites and clear the list
		public void flush_draw_shadows_list()
		{
			Common.Swap< List< RawSpriteTile > >( ref Sprites, ref m_shadows_list );
			TextureInfo = ShadowsMap;
			BlendMode = BlendMode.Multiplicative;
			base.Draw();
			m_shadows_list.Clear();
		}

		public SpriteTile NewCharater( Vector2i index, Vector3 start_pos )
		{
			SpriteTile sprite = new SpriteTile();
			sprite.TextureInfo = ObjectsMap;
			sprite.TileIndex2D = index;
			sprite.Scale = SpriteSize;
			sprite.Position = GetCellPos( start_pos );
			return sprite;
		}

		public SpriteTile NewObject( Vector2i index, Vector3 start_pos )
		{
			SpriteTile sprite = new SpriteTile();
			sprite.TextureInfo = ObjectsMap;
			sprite.TileIndex2D = index;
			sprite.Scale = SpriteSize;
			sprite.Position = GetCellPos( start_pos );
			return sprite;
		}

		public SpriteTile NewTree( Vector2i index, Vector3 start_pos )
		{
			SpriteTile sprite = NewObject( index, start_pos );

			// make the sprite bottom center point be the new pivot (for Skew)
			sprite.Quad.Centering( TRS.Local.BottomCenter );

			// make the trees move in the wind
			Schedule( (dt) => { 

				int hc = sprite.GetHashCode();
				System.Random rnd2 = new System.Random(hc);

				sprite.Skew = new Vector2( Math.Deg2Rad( 1.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 1.0f * rnd2.Next(4096)/4096.0f ), 
										   Math.Deg2Rad( 2.0f ) * FMath.Sin( (float)Director.Instance.DirectorTime * 3.0f * rnd2.Next(4096)/4096.0f ) );
			} );

			return sprite;
		}

		// Recreated the test scene there:
		// http://www.lostgarden.com/2007/05/dancs-miraculously-flexible-game.html

		public void CreateRPGTestScene()
		{
			// layer y=0

			SetBlock( new Vector3i(0,0,0), DirtBlock );
			SetBlock( new Vector3i(1,0,0), DirtBlock );
			SetBlock( new Vector3i(2,0,0), DirtBlock );
			SetBlock( new Vector3i(3,0,0), DirtBlock );
			SetBlock( new Vector3i(4,0,0), DirtBlock );
			SetBlock( new Vector3i(5,0,0), DirtBlock );
			SetBlock( new Vector3i(6,0,0), DirtBlock );

			SetBlock( new Vector3i(0,0,1), GrassBlock );
			SetBlock( new Vector3i(1,0,1), GrassBlock );
			SetBlock( new Vector3i(2,0,1), GrassBlock );
			SetBlock( new Vector3i(3,0,1), StoneBlock );
			SetBlock( new Vector3i(4,0,1), StoneBlock );
			SetBlock( new Vector3i(5,0,1), DirtBlock );
			SetBlock( new Vector3i(6,0,1), DirtBlock );

			SetBlock( new Vector3i(0,0,2), GrassBlock );
			SetBlock( new Vector3i(1,0,2), WaterBlock );
			SetBlock( new Vector3i(2,0,2), WaterBlock );
			SetBlock( new Vector3i(3,0,2), GrassBlock );

			// layer y=1

			SetBlock( new Vector3i(4,1,1), StoneBlock );
			SetBlock( new Vector3i(5,1,1), DirtBlock );
			SetBlock( new Vector3i(6,1,1), DirtBlock );

			SetBlock( new Vector3i(0,1,2), GrassBlock );
			SetBlock( new Vector3i(1,1,2), WaterBlock );
			SetBlock( new Vector3i(2,1,2), WaterBlock );
			SetBlock( new Vector3i(3,1,2), GrassBlock );

			// layer y=2

			SetBlock( new Vector3i(0,2,2), StoneBlock );
			SetBlock( new Vector3i(1,2,2), WaterBlock );
			SetBlock( new Vector3i(2,2,2), WaterBlock );
			SetBlock( new Vector3i(3,2,2), StoneBlock );
			SetBlock( new Vector3i(4,2,2), StoneBlock );
			SetBlock( new Vector3i(5,2,2), StoneBlock );
			SetBlock( new Vector3i(6,2,2), RampSouth );

			// layer y=3

			SetBlock( new Vector3i(0,3,3), RampWest );
			SetBlock( new Vector3i(1,3,3), StoneBlock );
			SetBlock( new Vector3i(2,3,3), StoneBlock );
			SetBlock( new Vector3i(3,3,3), StoneBlock );
			SetBlock( new Vector3i(4,3,3), RampEast );
			SetBlock( new Vector3i(5,3,2), StoneBlock );
			SetBlock( new Vector3i(6,3,2), StoneBlock );

			// layer y=4

			SetBlock( new Vector3i(0,4,2), StoneBlock );
			SetBlock( new Vector3i(1,4,2), WaterBlock );
			SetBlock( new Vector3i(2,4,2), WaterBlock );
			SetBlock( new Vector3i(3,4,2), PlainBlock );
			SetBlock( new Vector3i(4,4,3), WallBlock );
			SetBlock( new Vector3i(4,4,4), WallBlock );
			SetBlock( new Vector3i(4,4,5), WallBlock );
			SetBlock( new Vector3i(5,4,2), StoneBlock );
			SetBlock( new Vector3i(5,4,3), DoorTallClosed );
			SetBlock( new Vector3i(6,4,3), WallBlock );
			SetBlock( new Vector3i(6,4,4), WallBlock );
			SetBlock( new Vector3i(6,4,5), WallBlock );

			// try put as many sprites in SpriteList for performance

			SpriteList sprite_list = new SpriteList( ObjectsMap );

			sprite_list.AddChild( NewCharater( CharacterBoy, new Vector3(3,1.5f,3) ) );
			sprite_list.AddChild( NewCharater( CharacterHornGirl, new Vector3(5,2.6f,3) ) );
			sprite_list.AddChild( NewCharater( CharacterPrincessGirl, new Vector3(6,1,2) ) );

			sprite_list.AddChild( NewObject( Rock, new Vector3(3,0,3) ) );
			sprite_list.AddChild( NewObject( Rock, new Vector3(6,0,2) ) );
			sprite_list.AddChild( NewObject( Key, new Vector3(5,0,2) ) );

			sprite_list.AddChild( NewTree( TreeShort, new Vector3(0.5f,1,3) ) );

			AddChild( sprite_list );

			{
				var bug = NewObject( EnemyBug, new Vector3(2,3,4) );

				bug.Schedule( ( dt ) => { 

					float p = 1.0f;

					bug.Position = this.GetCellPos( Math.Lerp( new Vector3(1,3,4), new Vector3(3,3,4)
															   , ( 1.0f + FMath.Sin( (float)Director.Instance.DirectorTime * p ) ) * 0.5f ) );

					// we just care about the sign of the derivative
					bug.FlipU = ( FMath.Cos( (float)Director.Instance.DirectorTime * p ) < 0.0f );
				} );

				AddChild( bug );
			}
		}
	}
}

