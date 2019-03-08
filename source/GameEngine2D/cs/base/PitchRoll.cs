/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// Pitch/roll rotations helper object/functions.
	/// Pitch(x) -> roll(y) rotation order
	/// roll in -pi, pi
	/// pitch in -pi/2,pi/2
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ピッチ/ロール回転のヘルパーオブジェクト/関数。
	/// Pitch(x) -> roll(y)の順に回転します。
	/// ロールは-pi, pi、ピッチは-pi/2,pi/2です。
	/// </summary>
	/// @endif
	public class PitchRoll
	{
		/// @if LANG_EN
		/// <summary>
		/// Return pitch in x, roll in y.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピッチ x とロール y を返します。
		/// </summary>
		/// @endif
		public static
		Vector2 FromVector( Vector3 v )
		{
			float roll = Math.Angle( v.Zx );
			v = v.RotateY( -roll );
			Common.Assert( FMath.Abs( v.X ) < 1.0e-3f );
			float pitch = - Math.Angle( v.Zy );
			return new Vector2( pitch, roll );
		}

		/// @if LANG_EN
		/// <summary>
		/// Return z=(0,0,1) rotated by roll->pitch.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロール -> ピッチで回転させた z=(0,0,1)を返します。
		/// </summary>
		/// @endif
		public static
		Vector3 ToVector( Vector2 a )
		{
			return Math._001.RotateX( a.X ).RotateY( a.Y );
		}

		/// @if LANG_EN
		/// <summary>x: pitch  y: roll (radians)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>x: ピッチ  y: ロール (ラジアン)
		/// </summary>
		/// @endif
		public Vector2 Data;
		
		/// <summary></summary>
		public PitchRoll() { Data = GameEngine2D.Base.Math._00;}
		/// <summary></summary>
		public PitchRoll( Vector2 v ) { Data = v;}
		/// <summary></summary>
		public PitchRoll( Vector3 v ) { Data = FromVector( v );}
		/// <summary></summary>
		public Vector3 ToVector() { return ToVector( Data );}
		/// <summary></summary>
		public Matrix4 ToMatrix()
		{
			Vector3 z = ToVector();
			Vector2 tmp = Data;
			tmp.X -= Math.Pi * 0.5f;
			Vector3 y = new PitchRoll( tmp ).ToVector();
			Vector3 x = y.Cross(z).Normalize();
			y = z.Cross(x);

			return new Matrix4()
			{
				ColumnX = x.Xyz0 ,
				ColumnY = y.Xyz0 ,
				ColumnZ = z.Xyz0 ,
				ColumnW = Math._0001

			}.InverseOrthonormal();
		}
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Pitch/roll rotations helper object/functions.
	/// Roll(y) -> pitch(x) rotation order
	/// pitch in -pi, pi
	/// roll in -pi/2,pi/2
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Pitch/roll 回転のヘルパーオブジェクト/関数。
	/// Roll(y) -> pitch(x)の順に回転します。
	/// ピッチは-pi, pi、ロールは-pi/2,pi/2です。
	/// </summary>
	/// @endif
	public class RollPitch
	{
		/// @if LANG_EN
		/// <summary>
		/// Return pitch in x, roll in y.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピッチ x、ロール yを返します。
		/// </summary>
		/// @endif
		public static
		Vector2 FromVector( Vector3 v )
		{
			float pitch = - Math.Angle( v.Zy );
			v = v.RotateX( -pitch );
			Common.Assert( FMath.Abs( v.Y ) < 1.0e-3f );
			float roll =  Math.Angle( v.Zx );
			return new Vector2( pitch, roll );
		}

		/// @if LANG_EN
		/// <summary>
		/// Return z=(0,0,1) rotated by roll->pitch.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ロール -> ピッチで回転させる z=(0,0,1)を返します。
		/// </summary>
		/// @endif
		public static
		Vector3 ToVector( Vector2 a )
		{
			return Math._001.RotateY( a.Y ).RotateX( a.X );
		}

		/// @if LANG_EN
		/// <summary>x: pitch  y: roll (radians)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>x:ピッチ、y:ロール (ラジアン)。
		/// </summary>
		/// @endif
		public Vector2 Data;
		/// <summary></summary>
		public RollPitch() { Data = GameEngine2D.Base.Math._00;}
		/// <summary></summary>
		public RollPitch( Vector2 v ) { Data = v;}
		/// <summary></summary>
		public RollPitch( Vector3 v ) { Data = FromVector( v );}
		/// <summary></summary>
		public Vector3 ToVector() { return ToVector( Data );}
		/// <summary></summary>
		public Matrix4 ToMatrix()
		{
			Vector3 z = ToVector();
			Vector2 tmp = Data;
			tmp.X -= Math.Pi * 0.5f;
			Vector3 y = new RollPitch( tmp ).ToVector();
			Vector3 x = y.Cross(z).Normalize();
			y = z.Cross(x);

			return new Matrix4()
			{
				ColumnX = x.Xyz0 ,
				ColumnY = y.Xyz0 ,
				ColumnZ = z.Xyz0 ,
				ColumnW = Math._0001

			}.InverseOrthonormal();
		}
	}
}

