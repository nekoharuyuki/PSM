/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// TRS comes from scenegraph terminology and is used to store a
	/// Translate/Rotate/Scale 2d transform in a canonical way. It also
	/// defines an oriented bounding box. We use it for storing both 
	/// sprite positionning/size and sprite UV.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>TRSはシーングラフの専門用語です。TRSは平行移動/回転/スケールの2D変換行列を標準的な方法で格納するのために使用されています。
	/// また、向きをもった境界ボックスを定義します。
	/// TRSはスプライトの配置/サイズとスプライトUVの両方を格納するためにも使用します。
	/// </summary>
	/// @endif
	public struct TRS
	{
		/// @if LANG_EN
		/// <summary>Translation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>平行移動。
		/// </summary>
		/// @endif
		public Vector2 T;

		/// @if LANG_EN
		/// <summary>Rotation - stored as a unit vector (cos,sin).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>回転。単位ベクトル (cos,sin) として格納します。
		/// </summary>
		/// @endif
		public Vector2 R;

		/// @if LANG_EN
		/// <summary>Scale (or Size).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スケール (またはサイズ)。
		/// </summary>
		/// @endif
		public Vector2 S;

		/// @if LANG_EN
		/// <summary>The support X vector, which goes from bottom left to bottom right.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>左下から右下に向かう、サポートXベクトル。
		/// </summary>
		/// @endif
		public Vector2 X { get { return R * S.X;}}

		/// @if LANG_EN
		/// <summary>The support Y vector, which goes from bottom left to top left.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>左下から左上に向かう、サポートYベクトル。
		/// </summary>
		/// @endif
		public Vector2 Y { get { return Math.Perp( R ) * S.Y;}}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// @if LANG_EN
		/// <summary>The bottom left point (the base point), (0,0) in 'local' coordinates.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロカール座標での左下の点 (基準点), (0,0)。
		/// </summary>
		/// @endif
		public Vector2 Point00 { get { return T;}}

		/// @if LANG_EN
		/// <summary>The bottom right point, (1,0) in 'local' coordinates.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロカール座標での右下の点 (1,0)。
		/// </summary>
		/// @endif
		public Vector2 Point10 { get { return T + X;}}

		/// @if LANG_EN
		/// <summary>The top left point, (0,1) in 'local' coordinates.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロカール座標での左上の点 (0,1)。
		/// </summary>
		/// @endif
		public Vector2 Point01 { get { return T + Y;}}

		/// @if LANG_EN
		/// <summary>The top right point, (1,1) in 'local' coordinates.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロカール座標での右上の点 (1,1)。
		/// </summary>
		/// @endif
		public Vector2 Point11 { get { return T + X + Y;}}

		/// @if LANG_EN
		/// <summary>Return the center of the oriented box defined by this TRS.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このTRSによって定義された向きを持ったボックスの中心を返します。
		/// </summary>
		/// @endif
		public Vector2 Center { get { return T + ( X + Y ) * 0.5f;}}

		/// @if LANG_EN
		/// <summary>
		/// RotationNormalize is like Rotation, but it normalizes on set,
		/// to prevent the unit vector from drifting because of accumulated numerical imprecision.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>RotationNormalize は Rotation に似ています。しかし RotationNormalize は誤差の蓄積を防ぐために、単位ベクトルを正規化します。
		/// </summary>
		/// @endif
		public Vector2 RotationNormalize { get { return R ;} set { R = value.Normalize(); }}

		/// @if LANG_EN
		/// <summary>Rotate the object by an angle.</summary>
		/// <param name="angle">Rotation angle in radian.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>角度を指定してオブジェクトを回転させます。
		/// </summary>
		/// <param name="angle">Rotation angle in radian.</param>
		/// @endif
		public void Rotate( float angle ) { R = R.Rotate( angle );}

		/// @if LANG_EN
		/// <summary>Rotate the object by an angle.</summary>
		/// <param name="rotation">The (cos(angle),sin(angle)) unit vector representing the rotation.</param>
		/// This lets you precompute the cos,sin needed during rotation.
		/// @endif
		/// @if LANG_JA
		/// <summary>角度を指定してオブジェクトを回転させます。
		/// </summary>
		/// <param name="rotation">回転を表す(cos(angle),sin(angle))の単位ベクトル。</param>
		/// This lets you precompute the cos,sin needed during rotation.
		/// @endif
		public void Rotate( Vector2 rotation ) { R = R.Rotate( rotation );}

		/// @if LANG_EN
		/// <summary>
		/// This property lets you set/get rotation as a angle. This is expensive and brings the usual
		/// angle discontinuity problems. The angle is always stored and returned in the the range -pi,pi.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このプロパティは角度で回転を設定/取得します。これは重い処理であり、通常角度の不連続性の問題をもたらします。角度は常に格納され、-pi,piの範囲で値を返します。
		/// </summary>
		/// @endif
		public float Angle { get { return Math.Angle( R );} set { R = Vector2.Rotation( value );}}

		/// @if LANG_EN
		/// <summary>A TRS that covers the unit quad that goes from (0,0) to (1,1).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>（0,0）（1,1）からなる単位クワッドをカバーするTRS。
		/// </summary>
		/// @endif
		public static TRS Quad0_1 = new TRS() 
		{ 
			T = Math._00, 
			R = Math._10, 
			S = Math._11
		};

		/// @if LANG_EN
		/// <summary>A TRS that covers the quad that goes from (-1,-1) to (1,1).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>（-1,-1）（1,1）からなるクワッドをカバーするTRS。
		/// </summary>
		/// @endif
		public static TRS QuadMinus1_1 = new TRS() 
		{ 
			T = -Math._11, 
			R = Math._10, 
			S = Math._11*2
		};

		/// @if LANG_EN
		/// <summary>Convert from Bounds2: a_bounds.Min becomes T and a_bounds.Size becomes S (no rotation).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Bounds2からの変換 : a_bounds.Min は T になり、a_bounds.Size は Sになります(回転なし)。
		/// </summary>
		/// @endif
		public TRS( Bounds2 a_bounds )
		{
			T = a_bounds.Min;
			R = Math._10;
			S = a_bounds.Size;
		}

		/// @if LANG_EN
		/// <summary>
		/// Convert to Bounds2. Note that end points won't match if there is a Rotation,
		/// but in all cases the returned bounds fully contains the TRS.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Bounds2に変換します。回転がある場合は、エンドポイントが一致しないことに注意してください。しかし、すべてのケースで、返された境界は完全にTRSを含んでいます。
		/// </summary>
		/// @endif
		public Bounds2 Bounds2()
		{
			Bounds2 ret = new Bounds2( Point00, Point00 );

			ret.Add( Point10 );
			ret.Add( Point01 );
			ret.Add( Point11 );

			return ret;
		}

		//	use new TRS { T=a_T, R=a_S, S=a_S } instead?
//		public TRS( Vector2 a_T ,
//					Vector2 a_R ,
//					Vector2 a_S )
//		{
//			T = a_T;
//			R = a_R;
//			S = a_S;
//		}

		/// @if LANG_EN
		/// <summary>
		/// Get a subregion from source_area, given a number of tiles and a tile index,
		/// assuming evenly spaced subdivision. Typically source_area will be Quad0_1
		/// (the unit quad, means the whole texture) and we return the uv info for a 
		/// given tile in the tiled texture.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>タイル数とタイルのインデックスを指定し、引数 source_area からサブ領域を取得します。
		/// 一般的にsource_areaはQuad0_1（単位クワッド、全体のテクスチャを意味する）になります。
		/// タイルのテクスチャ内で、指定されたタイルのUV情報を返します。
		/// </summary>
		/// @endif
		static public TRS Tile( Vector2i num_tiles, Vector2i tile_index, TRS source_area )
		{
			Vector2 num_tiles_f = num_tiles.Vector2();
			Vector2 tile_index_f = tile_index.Vector2();

			Vector2 tile_size = source_area.S / num_tiles_f;

			Vector2 X = source_area.X;
			Vector2 Y = source_area.Y;

			TRS ret = new TRS();

			ret.T = source_area.T + tile_index_f * tile_size;
			ret.R = source_area.R;
			ret.S = tile_size;

			return ret;
		}


/// <summary>
//		/// Make the 'bottom left' base point T be "upper left".
//		/// This requires to flip the sign of S.Y.
//
//		public TRS OutrageousYTopBottomSwap()
//		{
//			TRS ret = this;
//  	
//			ret.T += Y; // move base point to upper left
//			ret.S.Y = -S.Y;
//  	
//			return ret;
//		}

		/// @if LANG_EN
		/// <summary>Some aliases for commonly used points that can be passed to Centering.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>エイリアス。一般的にCenteringに渡すために使用します。
		/// </summary>
		/// @endif
		static public class Local 
		{
			/// <summary></summary>
			static public Vector2 TopLeft = new Vector2(0.0f,1.0f);
			/// <summary></summary>
			static public Vector2 MiddleLeft = new Vector2(0.0f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomLeft = new Vector2(0.0f,0.0f);
			/// <summary></summary>
			static public Vector2 TopCenter = new Vector2(0.5f,1.0f);
			/// <summary></summary>
			static public Vector2 Center = new Vector2(0.5f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomCenter = new Vector2(0.5f,0.0f);
			/// <summary></summary>
			static public Vector2 TopRight = new Vector2(1.0f,1.0f);
			/// <summary></summary>
			static public Vector2 MiddleRight = new Vector2(1.0f,0.5f);
			/// <summary></summary>
			static public Vector2 BottomRight = new Vector2(1.0f,0.0f);
		}

		/// @if LANG_EN
		/// <summary>
		/// Translate the TRS so that the normalized point given in input becomes (0,0). 
		/// There are a few predefined normalized points in TRS.Local.
		/// </summary>
		/// <param name="normalized_pos">The normalized position that will become the new center. 
		/// For example (0.5,0.5) represents the center of the TRS, regardless of the actual size, 
		/// position and orientation of the TRS. (0,0) is the bottom left point, (1,1) is the top
		/// right point etc.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>入力で与えられる、正規化された点が（0,0）になるようにTRSを変換します。TRS.Localで事前に定義され、正規化された点です。
		/// </summary>
		/// <param name="normalized_pos">新しい中心になる、正規化された位置。たとえば、実際のサイズ、位置、TRSの向きに関係なく、( 0.5, 0.5 ) はTRSの中心を表しています。 ( 0, 0 )は左下の点、( 1, 1 )は右上の点です。</param>
		/// @endif
		public void Centering( Vector2 normalized_pos )
		{
			T = - X * normalized_pos.X 
				- Y * normalized_pos.Y;
		}
		
		/// <summary></summary>
		public override string ToString() 
		{
			if ( R.X == 0.0f && R.Y == 0.0f )
				return string.Format("Invalid TRS (R lenght is zero)");

			return string.Format("(T={0},R={1}={2} degrees,S={3})", T, R, Math.Rad2Deg(Angle), S);
//			return string.Format("(T={0},R={1},S={2})", T, R, S);
		}
	}
}

