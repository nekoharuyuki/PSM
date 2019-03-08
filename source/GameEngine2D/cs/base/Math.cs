/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>Some extensions to the math/vector lib</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>数学/ベクトルライブラリ用に拡張した Math クラス。
	/// </summary>
	/// @endif
	public static class Math
	{
		/// <summary></summary>
		public static float Pi { get { return 3.141592654f;}}

		/// <summary>2pi</summary>
		public static float TwicePi { get { return 6.283185307f;}}
		
		/// <summary>pi/2</summary>
		public static float HalfPi { get { return 1.570796327f;}}

		/// @if LANG_EN
		/// <summary>Wrap System.Random and extend with vector random generation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>System.Randomをラップし、ベクトルの乱数生成を拡張したクラス。
		/// </summary>
		/// @endif
		public 
		class RandGenerator
		{
			/// @if LANG_EN
			/// <summary>The raw random generator.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>生の乱数生成器。
			/// </summary>
			/// @endif
			public System.Random Random;
			

			/// <summary></summary>
			public RandGenerator( int seed = 0 )
			{
				Random = new System.Random( seed );
			}

			/// @if LANG_EN
			/// <summary>Return a random float in 0,1.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>0～1のランダムな浮動小数点数を返します。
			/// </summary>
			/// @endif
			public float NextFloat0_1()
			{
				return (float)Random.NextDouble();
			}

			/// @if LANG_EN
			/// <summary>Return a random float in -1,1.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>-1～1のランダムな浮動小数点数を返します。
			/// </summary>
			/// @endif
			public float NextFloatMinus1_1()
			{
				return ( NextFloat0_1() * 2.0f ) - 1.0f;
			}

			/// @if LANG_EN
			/// <summary>Return a random float in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな浮動小数点を返します。
			/// </summary>
			/// @endif
			public float NextFloat( float mi, float ma )
			{
				return mi + ( ma - mi ) * NextFloat0_1();
			}

			/// @if LANG_EN
			/// <summary>Return a random Vector2 -1,1.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>-1～1のランダムな Vector2 を返します。
			/// </summary>
			/// @endif
			public Vector2 NextVector2Minus1_1()
			{
				return new Vector2( NextFloat( -1.0f, 1.0f ), 
									NextFloat( -1.0f, 1.0f ) );
			}

			/// @if LANG_EN
			/// <summary>Return a random Vector2 in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな Vector2 を返します。
			/// </summary>
			/// @endif
			public Vector2 NextVector2( Vector2 mi, Vector2 ma )
			{
				return new Vector2( NextFloat( mi.X, ma.X ), 
									NextFloat( mi.Y, ma.Y ) );
			}

			/// @if LANG_EN
			/// <summary>Return a random Vector2 in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな Vector2 を返します。
			/// </summary>
			/// @endif
			public Vector2 NextVector2( float mi, float ma )
			{
				return new Vector2( NextFloat( mi, ma ), 
									NextFloat( mi, ma ) );
			}

			/// @if LANG_EN
			/// <summary>Return a random Vector3 in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな Vector3 を返します。
			/// </summary>
			/// @endif
			public Vector3 NextVector3( Vector3 mi, Vector3 ma )
			{
				return new Vector3( NextFloat( mi.X, ma.X ),
									NextFloat( mi.Y, ma.Y ),
									NextFloat( mi.Z, ma.Z ) );
			}
			/// @if LANG_EN
			/// <summary>Return a random Vector4 in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな Vector4 を返します。
			/// </summary>
			/// @endif
			public Vector4 NextVector4( Vector4 mi, Vector4 ma )
			{
				return new Vector4( NextFloat( mi.X, ma.X ),
									NextFloat( mi.Y, ma.Y ),
									NextFloat( mi.Z, ma.Z ), 
									NextFloat( mi.W, ma.W ) );
			}

			/// @if LANG_EN
			/// <summary>Return a random Vector4 in mi,ma.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>mi～maのランダムな Vector4 を返します。
			/// </summary>
			/// @endif
			public Vector4 NextVector4( float mi, float ma )
			{
				return new Vector4( NextFloat( mi, ma ),
									NextFloat( mi, ma ),
									NextFloat( mi, ma ), 
									NextFloat( mi, ma ) );
			}
		}                                                                                              

		/// @if LANG_EN
		/// <summary>
		/// Compute a lookat matrix for the camera. The vector (eye, center)
		/// maps to -z (since OpenGL looks downward z), and up maps to y.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>カメラのルックアット行列を計算します。ベクトル（視点、中央）を-zにマップし（OpenGLはZを下向きにみるので）、上方向をyにマップします。
		/// </summary>
		/// @endif
		public static 
		Matrix4 LookAt( Vector3 eye, Vector3 center, Vector3 _Up )
		{
			Vector3 up = _Up.Normalize();

			Common.Assert( up.IsUnit(1.0e-3f) );

			Vector3 x,y,z;

			float EPSILON=1.0e-5f;

			if ( ( eye - center ).Length() > EPSILON )
			{
				z = ( eye - center ).Normalize();

				if ( FMath.Abs( z.Dot( up ) ) > 0.9999f )
				{
					y = z.Perpendicular();
					x = y.Cross( z );
				}
				else
				{
					x = up.Cross( z ).Normalize();
					y = z.Cross( x );
				}
			}
			else
			{
				y = up;
				x = y.Perpendicular();	// use any vector perpendicular to y
				z = x.Cross( y );
			}

			Matrix4 retval = new Matrix4() 
			{ 
				ColumnX = x.Xyz0, 
				ColumnY = y.Xyz0, 
				ColumnZ = z.Xyz0, 
				ColumnW = eye.Xyz1
			};

			Common.Assert( retval.IsOrthonormal( 1.0e-3f ) );

			return retval;
		}

		/// @if LANG_EN
		/// <summary>Fast build of a Matrix3 TRS matrix.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Matrix3 TRS 行列の高速ビルド。
		/// </summary>
		/// @endif
		public static void TranslationRotationScale( ref Matrix3 ret
													 , Vector2 translation, Vector2 rotation, Vector2 scale )
		{
			ret.X = new Vector3( rotation.X * scale.X, rotation.Y * scale.X, 0.0f );
			ret.Y = new Vector3( -rotation.Y * scale.Y, rotation.X * scale.Y, 0.0f );
			ret.Z = translation.Xy1;
		}

		/// @if LANG_EN
		/// <summary>Return the determinant formed by 2 vectors.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>2つのベクトルの行列式を返します。
		/// @endif
		public static
		float Det( Vector2 value1, Vector2 value2 ) 
		{
			return value1.X * value2.Y - value1.Y * value2.X;
		}

		/// @if LANG_EN
		/// <summary>Return the sign of x (returns 0.0f is x=0.0f).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>xの符号を返します。xが0.0fなら0.0fを返します。
		/// </summary>
		/// @endif
		public static
		float Sign( float x ) 
		{
			if ( x < 0.0f ) return -1.0f;
			if ( x > 0.0f ) return 1.0f;
			return 0.0f;
		}

		/// @if LANG_EN
		/// <summary>Return value rotated by pi/2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>pi/2で回転した値を返します。
		/// </summary>
		/// @endif
		public static 
		Vector2 Perp( Vector2 value )
		{
			return new Vector2( -value.Y, value.X );
		}

		/// @if LANG_EN
		/// <summary>Set alpha (can be inlined in math expressions).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アルファ値を設定します。(数学式でインライン化することができます)
		/// </summary>
		/// @endif
		public static 
		Vector4 SetAlpha( Vector4 value, float w )
		{
			value.W = w;
			return value;
		}

		/// @if LANG_EN
		/// <summary>SafeAcos checks that x is in [-1,1], and if x is off by an epsilon it clamps it.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SafeAcos xが[-1,1]の範囲内にあることをチェックし、イプシロンによってオフになるなら、それをクランプします。
		/// </summary>
		/// @endif
		public static 
		float SafeAcos( float x )
		{
			Common.Assert( FMath.Abs( x ) - 1.0f < 1.0e-5f );
			return FMath.Acos( FMath.Clamp( x, -1.0f, 1.0f ) );	// clamp if necessary (we have checked that we are in in [-1,1] by an epsilon)
		}

		/// @if LANG_EN
		/// <summary>Return the absolute 2d angle formed by (1,0) and value, in range -pi,pi</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>(1,0)と引数 valueのなす2Dの絶対角を、-pi～piの範囲で返します。
		/// </summary>
		/// @endif
		public static 
		float Angle( Vector2 value )
		{
			float angle = SafeAcos( value.Normalize().X );
			return value.Y < 0.0f ? -angle : angle;
		}

		/// @if LANG_EN
		/// <summary>Rotate 'point' around rotation center 'pivot' by an angle 'angle' (radians).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>'pivot'を中心に、角度 ' angle'(ラジアン)で 'point'を回転します。
		/// </summary>
		/// @endif
		public static 
		Vector2 Rotate( Vector2 point, float angle, Vector2 pivot )
		{
			return pivot + ( point - pivot ).Rotate( angle );
		}

		/// @if LANG_EN
		/// <summary>Degree to radians.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>度数をラジアンに変換します。
		/// </summary>
		/// @endif
		public static 
		float Deg2Rad( float value )
		{
			return value * 0.01745329251f;
		}

		/// @if LANG_EN
		/// <summary>Radians to degrees.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ラジアンを度数に変換します。
		/// </summary>
		/// @endif
		public static 
		float Rad2Deg( float value )
		{
			return value * 57.29577951308f;
		}

		/// @if LANG_EN
		/// <summary>Element wise degree to radians.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>度数をラジアンに変換します。
		/// </summary>
		/// @endif
		public static 
		Vector2 Deg2Rad( Vector2 value )
		{
			return value * 0.01745329251f;
		}

		/// @if LANG_EN
		/// <summary>Element wise radians to degrees.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ラジアンを度数に変換します。
		/// </summary>
		/// @endif
		public static 
		Vector2 Rad2Deg( Vector2 value )
		{
			return value * 57.29577951308f;
		}

		/// @if LANG_EN
		/// <summary>Linear interpolation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>線形補間。</summary>
		/// @endif
		public static
		float Lerp( float a, float b, float x )
		{
			return a + x * ( b - a );
		}

		/// @if LANG_EN
		/// <summary>Linear interpolation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>線形補間。</summary>
		/// @endif
		public static
		Vector2 Lerp( Vector2 a, Vector2 b, float x )
		{
			return a + x * ( b - a );
		}

		/// @if LANG_EN
		/// <summary>Linear interpolation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>線形補間。</summary>
		/// @endif
		public static
		Vector3 Lerp( Vector3 a, Vector3 b, float x )
		{
			return a + x * ( b - a );
		}

		/// @if LANG_EN
		/// <summary>Linear interpolation.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>線形補間。</summary>
		/// @endif
		public static
		Vector4 Lerp( Vector4 a, Vector4 b, float x )
		{
			return a + x * ( b - a );
		}

		/// @if LANG_EN
		/// <summary>Lerp 2 (assumed) unit vectors (shortest path).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>2つの単位ベクトルで線形補間を行います。補間はパスの短い側で行います。</summary>
		/// @endif
		public static
		Vector2 LerpUnitVectors( Vector2 va, Vector2 vb, float x )
		{
			return va.Rotate( va.Angle( vb ) * x );
		}

		/// @if LANG_EN
		/// <summary>Lerp 2 angle values (shortest path).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数の2つの角度で線形補間を行います。補間はパスの短い側で行います。</summary>
		/// @endif
		public static
		float LerpAngles( float a, float b, float x )
		{
			return Angle( LerpUnitVectors( Vector2.Rotation( a ), 
										   Vector2.Rotation( b ), x )  );
		}

		/// @if LANG_EN
		/// <summary>A "safe" sine function taking uint values.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>uint型の値をとる、安全なサイン関数。</summary>
		/// @endif
		public static
		float Sin( uint period, float phase, uint mstime )
		{
			return FMath.Sin( ( ( ( (float)( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
		}

		/// @if LANG_EN
		/// <summary>A "safe" sine function taking ulong values.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ulong型の値をとる、安全なサイン関数。</summary>
		/// @endif
		public static
		float Sin( ulong period, float phase, ulong mstime )
		{
			return FMath.Sin( ( ( ( (float)( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
		}

		/// @if LANG_EN
		/// <summary>This is just f(x)=x, named so that code is more explicit when it is passed as a tween function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この関数は単なる f(x)=x です。Tween関数として渡すとき、コードをより明快にするため定義されています。</summary>
		/// @endif
		public static 
		float Linear( float x )
		{
			return x;
		}

		/// @if LANG_EN
		/// <summary>
		/// A very controlable s curve, lets you do polynomial ease in/out curves
		/// with little code.
		/// </summary>
		/// <param name="x">Asssumed to be in 0,1.</param>
		/// <param name="p1">Controls the ease in exponent (if >1).</param>
		/// <param name="p2">Controls the ease out exponent (if >1.,(p1,p2)=(1,1) just gives f(x)=x</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>制御可能な曲線であり、少ないコードでカーブのイン/アウト多項式を容易に行うことができます。
		/// </summary>
		/// <param name="x">Asssumed to be in 0,1.</param>
		/// <param name="p1">Controls the ease in exponent (if >1).</param>
		/// <param name="p2">Controls the ease out exponent (if >1.),(p1,p2)=(1,1) just gives f(x)=x</param>
		/// @endif
		public static
		float PowerfulScurve( float x, float p1, float p2 )
		{
			return FMath.Pow( 1.0f - FMath.Pow( 1.0f - x, p2 ), p1 );
		}

		/// @if LANG_EN
		/// <summary>Ease in curve using Pow.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Ease in curve using Pow.</summary>
		/// @endif
		public static 
		float PowEaseIn( float x, float p )
		{
			return FMath.Pow(x,p);
		}

		/// @if LANG_EN
		/// <summary>Ease out curve using Pow.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Ease out curve using Pow.</summary>
		/// @endif
		public static 
		float PowEaseOut( float x, float p )
		{
			return 1.0f - PowEaseIn( 1.0f - x, p );
		}

		/// @if LANG_EN
		/// <summary>
		/// PowEaseIn/PowEaseOut mirrored around 0.5,0.5.
		/// Same exponent in and out.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// PowEaseIn/PowEaseOut mirrored around 0.5,0.5.
		/// Same exponent in and out.
		/// </summary>
		/// @endif
		public static 
		float PowEaseInOut( float x, float p )
		{
			if ( x < 0.5f )	return 0.5f * PowEaseIn( x * 2.0f, p );
			return 0.5f + 0.5f * PowEaseOut( ( x - 0.5f ) * 2.0f, p );
		}

		/// @if LANG_EN
		/// <summary>
		/// Ease out curve using a 1-exp(-a*x) exponential,
		/// but normalized so that we reach 1 when x=1.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Ease out curve using a 1-exp(-a*x) exponential,
		/// but normalized so that we reach 1 when x=1.
		/// </summary>
		/// @endif
		public static 
		float ExpEaseOut( float x, float a )
		{
			return( 1.0f - FMath.Exp( - x * a ) ) / ( 1.0f - FMath.Exp( - a ) );
		}

		/// @if LANG_EN
		/// <summary>Ease in curve using an exponential.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Ease in curve using an exponential.</summary>
		/// @endif
		public static 
		float ExpEaseIn( float x, float a )
		{
			return 1.0f - ExpEaseOut( 1.0f - x, a );
		}

		/// @if LANG_EN
		/// <summary>BackEaseIn function (see  http://www.robertpenner.com)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>BackEaseIn function (see  http://www.robertpenner.com)</summary>
		/// @endif
		public static 
		float BackEaseIn( float x, float a )
		{
			return x * x * ( ( a + 1.0f ) * x - a );
		}

		/// @if LANG_EN
		/// <summary>BackEaseOut function (see  http://www.robertpenner.com)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>BackEaseOut function (see  http://www.robertpenner.com)</summary>
		/// @endif
		public static 
		float BackEaseOut( float x, float a )
		{
			return 1.0f - BackEaseIn( 1.0f - x, a );
		}

		/// @if LANG_EN
		/// <summary>BackEaseIn/BackEaseOut mirrored around 0.5,0.5.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>BackEaseIn/BackEaseOut mirrored around 0.5,0.5.</summary>
		/// @endif
		public static 
		float BackEaseInOut( float x, float p )
		{
			if ( x < 0.5f )	return 0.5f * BackEaseIn( x * 2.0f, p );
			return 0.5f + 0.5f * BackEaseOut( ( x - 0.5f ) * 2.0f, p );
		}

		/// @if LANG_EN
		/// <summary>Impulse function (source Inigo Quilez).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Impulse function (source Inigo Quilez).</summary>
		/// @endif
		public static 
		float Impulse( float x, float b )
		{
			float h = b * x;
			return h * FMath.Exp( 1.0f - h );
		}

		/// @if LANG_EN
		/// <summary>Travelling wave function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Travelling wave function.</summary>
		/// @endif
		public static 
		float ShockWave( float d
						 , float time
						 , float wave_half_width
						 , float wave_speed
						 , float wave_fade
						 , float d_scale )
		{
			d *= d_scale;
			float travelled = time * wave_speed;
			float x = FMath.Clamp( d - travelled, -wave_half_width, wave_half_width ) / wave_half_width; // -1,1 parameter
			float wave = ( 1.0f + FMath.Cos( Pi * x ) ) * 0.5f;
			return wave * FMath.Exp( -d * wave_fade );
		}

		/// @if LANG_EN
		/// <summary>Return the log of v in base 2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return the log of v in base 2.</summary>
		/// @endif
		public static 
			int Log2( int v )
		{
			int r;
			int shift;
			r =     (v > 0xFFFF?1:0) << 4; v >>= r;
			shift = (v > 0xFF  ?1:0) << 3; v >>= shift; r |= shift;
			shift = (v > 0xF   ?1:0) << 2; v >>= shift; r |= shift;
			shift = (v > 0x3   ?1:0) << 1; v >>= shift; r |= shift;
			r |= (v >> 1);
			return r;
		}

		/// @if LANG_EN
		/// <summary>Return true if 'i' is a power of 2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>'i'が2のべき乗である場合、trueを返します。</summary>
		/// @endif
		public static 
			bool IsPowerOf2( int i )
		{
			return ( 1 << Log2(i) ) == i;
		}

		/// @if LANG_EN
		/// <summary>Return the closest greater or equal power of 2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数 i に大きい側で最も近い、もしくは等しい2のべき乗を返します。</summary>
		/// @endif
		public static 
			int GreatestOrEqualPowerOf2( int i )
		{
			int p = ( 1 << Log2(i) );
			return p < i ? 2 * p : p;
		}

		// some constants


		/// <summary></summary>
		public static Vector2i _00i = new Vector2i(0,0);
		/// <summary></summary>
		public static Vector2i _10i = new Vector2i(1,0);
		/// <summary></summary>
		public static Vector2i _01i = new Vector2i(0,1);
		/// <summary></summary>
		public static Vector2i _11i = new Vector2i(1,1);
		/// <summary></summary>
		public static Vector3i _000i = new Vector3i(0,0,0);
		/// <summary></summary>
		public static Vector3i _100i = new Vector3i(1,0,0);
		/// <summary></summary>
		public static Vector3i _010i = new Vector3i(0,1,0);
		/// <summary></summary>
		public static Vector3i _110i = new Vector3i(1,1,0);
		/// <summary></summary>
		public static Vector3i _001i = new Vector3i(0,0,1);
		/// <summary></summary>
		public static Vector3i _101i = new Vector3i(1,0,1);
		/// <summary></summary>
		public static Vector3i _011i = new Vector3i(0,1,1);
		/// <summary></summary>
		public static Vector3i _111i = new Vector3i(1,1,1);
		
		/// <summary></summary>
		public static Vector2 _00 = new Vector2(0.0f,0.0f);
		/// <summary></summary>
		public static Vector2 _10 = new Vector2(1.0f,0.0f);
		/// <summary></summary>
		public static Vector2 _01 = new Vector2(0.0f,1.0f);
		/// <summary></summary>
		public static Vector2 _11 = new Vector2(1.0f,1.0f);
		
		/// <summary></summary>
		public static Vector3 _000 = new Vector3(0.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector3 _100 = new Vector3(1.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector3 _010 = new Vector3(0.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector3 _110 = new Vector3(1.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector3 _001 = new Vector3(0.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector3 _101 = new Vector3(1.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector3 _011 = new Vector3(0.0f,1.0f,1.0f);
		/// <summary></summary>
		public static Vector3 _111 = new Vector3(1.0f,1.0f,1.0f);
		
		/// <summary></summary>
		public static Vector4 _0000 = new Vector4(0.0f,0.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _1000 = new Vector4(1.0f,0.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _0100 = new Vector4(0.0f,1.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _1100 = new Vector4(1.0f,1.0f,0.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _0010 = new Vector4(0.0f,0.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _1010 = new Vector4(1.0f,0.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _0110 = new Vector4(0.0f,1.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _1110 = new Vector4(1.0f,1.0f,1.0f,0.0f);
		/// <summary></summary>
		public static Vector4 _0001 = new Vector4(0.0f,0.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _1001 = new Vector4(1.0f,0.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _0101 = new Vector4(0.0f,1.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _1101 = new Vector4(1.0f,1.0f,0.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _0011 = new Vector4(0.0f,0.0f,1.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _1011 = new Vector4(1.0f,0.0f,1.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _0111 = new Vector4(0.0f,1.0f,1.0f,1.0f);
		/// <summary></summary>
		public static Vector4 _1111 = new Vector4(1.0f,1.0f,1.0f,1.0f);

		/// @if LANG_EN
		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4.
		/// offset=0,0 scale=1,1 means identity.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4.
		/// offset=0,0 scale=1,1 means identity.
		/// </summary>
		/// @endif
		public static Vector4 UV_TransformIdentity = new Vector4( 0.0f, 0.0f, 1.0f, 1.0f ); 

		/// @if LANG_EN
		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4
		/// UV_TransformFlipV v into 1-v, and leaves u unchanged.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// UV transform stored as (offset, scale) in a Vector4
		/// UV_TransformFlipV v into 1-v, and leaves u unchanged.
		/// </summary>
		/// @endif
		public static Vector4 UV_TransformFlipV = new Vector4( 0.0f, 1.0f, 1.0f, -1.0f );

		/// @if LANG_EN
		/// <summary>Return the closest point to P that's on segment [A,B].</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return the closest point to P that's on segment [A,B].</summary>
		/// @endif
		public static 
		Vector2 ClosestSegmentPoint( Vector2 P, Vector2 A, Vector2 B )
		{
			Vector2 AB = B - A;

			if ( ( P - A ).Dot( AB ) <= 0.0f ) return A;
			if ( ( P - B ).Dot( AB ) >= 0.0f ) return B;

			return P.ProjectOnLine( A, AB );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// Integer version of Vector2.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// Vector2 のint版。
	/// </summary>
	/// @endif
	public struct Vector2i
	{
		/// <summary>X</summary>
		public int X;
		/// <summary>Y</summary>
		public int Y;

		/// @if LANG_EN
		/// <summary>Vector2i constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public Vector2i( int x, int y )  
		{
			X = x;
			Y = y;
		}

		/// @if LANG_EN
		/// <summary>Return this as a Vector2 (cast to float).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>floatのVector2 型に変換して値を返します。</summary>
		/// @endif
		public Vector2 Vector2()
		{
			return new Vector2( (float)X, (float)Y );
		}

		/// @if LANG_EN
		/// <summary>Element wise max.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の最大値。</summary>
		/// @endif
		public Vector2i Max( Vector2i value )
		{
			return new Vector2i( Common.Max( X, value.X ) ,
								 Common.Max( Y, value.Y ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise min.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の最小値。
		/// </summary>
		/// @endif
		public Vector2i Min( Vector2i value )
		{
			return new Vector2i( Common.Min( X, value.X ) ,
								 Common.Min( Y, value.Y ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise clamp.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数の範囲でクランプ処理を行います。</summary>
		/// @endif
		public Vector2i Clamp( Vector2i min, Vector2i max )
		{
			return new Vector2i( Common.Clamp( X, min.X, max.X ) ,
								 Common.Clamp( Y, min.Y, max.Y ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// <param name="n">The 2d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// <param name="n">The 2d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		public Vector2i ClampIndex( Vector2i n )
		{
			return new Vector2i( Common.ClampIndex( X, n.X ) ,
								 Common.ClampIndex( Y, n.Y ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// This's (X,Y) is assumed to be a 2d index in a 2d table of size (n.X,n.Y).
		/// If X or Y are not in the valid array range, they are wrapped around [0,n.X-1] and [0,n.Y-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// This's (X,Y) is assumed to be a 2d index in a 2d table of size (n.X,n.Y).
		/// If X or Y are not in the valid array range, they are wrapped around [0,n.X-1] and [0,n.Y-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X or n.Y negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		public Vector2i WrapIndex( Vector2i n )
		{
			return new Vector2i( Common.WrapIndex( X, n.X ) ,
								 Common.WrapIndex( Y, n.Y ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise addition.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の加算。
		/// </summary>
		/// @endif
		public static Vector2i operator + ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X + value.X , 
								 a.Y + value.Y );
		}

		/// @if LANG_EN
		/// <summary>Element wise subtraction.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の減算。
		/// </summary>
		/// @endif
		public static Vector2i operator - ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X - value.X , 
								 a.Y - value.Y );
		}

		/// @if LANG_EN
		/// <summary>Element wise multiplication.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の乗算。
		/// </summary>
		/// @endif
		public static Vector2i operator * ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X * value.X , 
								 a.Y * value.Y );
		}

		/// @if LANG_EN
		/// <summary>Element wise multiplication.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の乗算。
		/// </summary>
		/// @endif
		public static Vector2i operator * ( Vector2i a, int value )
		{
			return new Vector2i( a.X * value , 
								 a.Y * value );
		}

		/// @if LANG_EN
		/// <summary>Element wise multiplication.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の乗算。
		/// </summary>
		/// @endif
		public static Vector2i operator * ( int value, Vector2i a )
		{
			return new Vector2i( a.X * value , 
								 a.Y * value );
		}

		/// @if LANG_EN
		/// <summary>Element wise division.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の除算。
		/// </summary>
		/// @endif
		public static Vector2i operator / ( Vector2i a, Vector2i value )
		{
			return new Vector2i( a.X / value.X , 
								 a.Y / value.Y );
		}

		/// @if LANG_EN
		/// <summary>Unary minus operator.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>単項マイナス演算子。
		/// </summary>
		/// @endif
		public static Vector2i operator- ( Vector2i a )
		{
			return new Vector2i( -a.X , 
								 -a.Y );
		}

		/// @if LANG_EN
		/// <summary>Return true if all elements are equal.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>全ての要素が等しい場合、true を返します。
		/// </summary>
		/// @endif
		public static bool operator == ( Vector2i a, Vector2i value )
		{
			return( a.X == value.X ) && ( a.Y == value.Y );
		}

		/// @if LANG_EN
		/// <summary>Return true if at least one element is non equal.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>少なくとも1つの要素が等しくない場合、trueを返します。
		/// </summary>
		/// @endif
		public static bool operator != ( Vector2i a, Vector2i value )
		{
			return( a.X != value.X ) || ( a.Y != value.Y );
		}

		/// @if LANG_EN
		/// <summary>Return the product of elements, X * Y</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素の積、 X*Y を返します。
		/// </summary>
		/// @endif
		public int Product()
		{
			return X * Y;
		}

		/// @if LANG_EN
		/// <summary>Equality test.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>等価テスト。</summary>
		/// @endif
		public bool Equals( Vector2i v )  
		{
			return( X == v.X ) && ( Y == v.Y );
		}

		/// @if LANG_EN
		/// <summary>Equality test.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>等価テスト。</summary>
		/// @endif
		public override bool Equals( Object o ) 
		{
			return !(o is Vector2i) ? false : Equals((Vector2i)o);
		}

		/// @if LANG_EN
		/// <summary>Return the string representation of this vector.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このベクトルの文字列表現を返します。
		/// </summary>
		/// @endif
		public override string ToString() 
		{
			return string.Format("({0},{1})", X, Y);
		}

		/// @if LANG_EN
		/// <summary>Gets the hash code for this vector.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このベクトルのハッシュコードを返します。
		/// </summary>
		/// @endif
		public override int GetHashCode() 
		{
			return(int)(X.GetHashCode() ^ Y.GetHashCode());
		}

//		public static Vector2i operator >> ( Vector2i a, Vector2i value )
//		{
//			return new Vector2i( a.X >> value.X , 
//								 a.Y >> value.Y );
//		}
//  	
//		public static Vector2i operator << ( Vector2i a, Vector2i value )
//		{
//			return new Vector2i( a.X << value.X , 
//								 a.Y << value.Y );
//		}


		/// <summary></summary>
		public Vector2i Yx { get{ return new Vector2i( Y, X ); } }
	}

	/// @if LANG_EN
	/// <summary>
	/// Integer version of Vector3.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Vector3 の int版。
	/// </summary>
	/// @endif
	public struct Vector3i
	{
		/// <summary>X</summary>
		public int X;
		/// <summary>Y</summary>
		public int Y;
		/// <summary>Z</summary>
		public int Z;

		/// @if LANG_EN
		/// <summary>Vector3i constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public Vector3i( int x, int y, int z )  
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// @if LANG_EN
		/// <summary>Vector3i constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public Vector3i( Vector2i xy, int z )  
		{
			X = xy.X;
			Y = xy.Y;
			Z = z;
		}

		/// @if LANG_EN
		/// <summary>Return this as a Vector3 (cast to float).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>floatのVector3 型に変換して値を返します。
		/// </summary>
		/// @endif
		public Vector3 Vector3()
		{
			return new Vector3( (float)X, (float)Y, (float)Z );
		}

		/// @if LANG_EN
		/// <summary>Element wise max.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の最大値。
		/// </summary>
		/// @endif
		public Vector3i Max( Vector3i value )
		{
			return new Vector3i( Common.Max( X, value.X ) ,
								 Common.Max( Y, value.Y ) ,
								 Common.Max( Z, value.Z ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise min.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の最小値。
		/// </summary>
		/// @endif
		public Vector3i Min( Vector3i value )
		{
			return new Vector3i( Common.Min( X, value.X ) ,
								 Common.Min( Y, value.Y ),
								 Common.Min( Z, value.Z ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise clamp.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位のクランプ処理。
		/// </summary>
		/// @endif
		public Vector3i Clamp( Vector3i min, Vector3i max )
		{
			return new Vector3i( Common.Clamp( X, min.X, max.X ) ,
								 Common.Clamp( Y, min.Y, max.Y ),
								 Common.Clamp( Z, min.Z, max.Z ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// Z is clamped to [0,n.Z-1]
		/// <param name="n">The 3d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Element wise index clamp.
		/// X is clamped to [0,n.X-1]
		/// Y is clamped to [0,n.Y-1]
		/// Z is clamped to [0,n.Z-1]
		/// <param name="n">The 3d size "this" components must be clamped against. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		public Vector3i ClampIndex( Vector3i n )
		{
			return new Vector3i( Common.ClampIndex( X, n.X ) ,
								 Common.ClampIndex( Y, n.Y ),
								 Common.ClampIndex( Z, n.Z ) );
		}

		/// @if LANG_EN
		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// Z wraps around [0,n.Z-1]
		/// This's (X,Y,Z) is assumed to be a 3d index in a 3d table of size (n.X,n.Y.n.Z).
		/// If X, Y or Z are not in the valid array range, they are wrapped around [0,n.X-1], [0,n.Y-1], [0,n.Z-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Element wise index wrap. 
		/// X wraps around [0,n.X-1]
		/// Y wraps around [0,n.Y-1]
		/// Z wraps around [0,n.Z-1]
		/// This's (X,Y,Z) is assumed to be a 3d index in a 3d table of size (n.X,n.Y.n.Z).
		/// If X, Y or Z are not in the valid array range, they are wrapped around [0,n.X-1], [0,n.Y-1], [0,n.Z-1] respectively (-1 becomes n-1, n becomes 0, n+1 becomes 1 etc), else their value is unchanged.
		/// <param name="n">The 2d size "this" components must be wrapped around. The components of n are assumed to be positive (values of n.X, n.Y or n.Z negative or zero will result in undefined behaviour).</param>
		/// </summary>
		/// @endif
		public Vector3i WrapIndex( Vector3i n )
		{
			return new Vector3i( Common.WrapIndex( X, n.X ) ,
								 Common.WrapIndex( Y, n.Y ) ,
								 Common.WrapIndex( Z, n.Z ) );
		}

		/// @if LANG_EN
		/// <summary>Element wise addition.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の加算。
		/// </summary>
		/// @endif
		public static Vector3i operator + ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X + value.X , 
								 a.Y + value.Y, 
								 a.Z + value.Z );
		}

		/// @if LANG_EN
		/// <summary>Element wise subtraction.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の減算。
		/// </summary>
		/// @endif
		public static Vector3i operator - ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X - value.X , 
								 a.Y - value.Y, 
								 a.Z - value.Z );
		}

		/// @if LANG_EN
		/// <summary>Element wise multiplication.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の乗算。
		/// </summary>
		/// @endif
		public static Vector3i operator * ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X * value.X , 
								 a.Y * value.Y , 
								 a.Z * value.Z );
		}

		/// @if LANG_EN
		/// <summary>Element wise division.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素単位の除算。
		/// </summary>
		/// @endif
		public static Vector3i operator / ( Vector3i a, Vector3i value )
		{
			return new Vector3i( a.X / value.X , 
								 a.Y / value.Y, 
								 a.Z / value.Z );
		}

		/// @if LANG_EN
		/// <summary>Return true if all elements are equal.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>すべての要素が等しい場合、trueを返します。
		/// </summary>
		/// @endif
		public static bool operator == ( Vector3i a, Vector3i value )
		{
			return( a.X == value.X ) && ( a.Y == value.Y ) && ( a.Z == value.Z );
		}

		/// @if LANG_EN
		/// <summary>Return true if at least one element is non equal.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>少なくとも1つの要素が等しくない場合、trueを返します。
		/// </summary>
		/// @endif
		public static bool operator != ( Vector3i a, Vector3i value )
		{
			return( a.X != value.X ) || ( a.Y != value.Y ) || ( a.Z != value.Z );
		}

		/// @if LANG_EN
		/// <summary>Return the product of elements, X * Y * Z</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>要素の積、X * Y * Z を返します。
		/// </summary>
		/// @endif
		public int Product()
		{
			return X * Y * Z;
		}

		/// @if LANG_EN
		/// <summary>Equality test.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>等価テスト。
		/// </summary>
		/// @endif
		public bool Equals( Vector3i v )  
		{
			return( X == v.X ) && ( Y == v.Y ) && ( Z == v.Z );
		}

		/// @if LANG_EN
		/// <summary>Equality test.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>等価テスト。
		/// </summary>
		/// @endif
		public override bool Equals( Object o ) 
		{
			return !(o is Vector3i) ? false : Equals((Vector3i)o);
		}

		/// @if LANG_EN
		/// <summary>Return the string representation of this vector.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このベクトルの文字列表現を返します。
		/// </summary>
		/// @endif
		public override string ToString() 
		{
			return string.Format("({0},{1},{2})", X, Y,Z);
		}

		/// @if LANG_EN
		/// <summary>Gets the hash code for this vector.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このベクトルのハッシュコードを返します。
		/// </summary>
		/// @endif
		public override int GetHashCode() 
		{
			return(int)(X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
		}

//		public static Vector3i operator >> ( Vector3i a, Vector3i value )
//		{
//			return new Vector3i( a.X >> value.X , 
//								 a.Y >> value.Y , 
//								 a.Z >> value.Z );
//		}
//  	
//		public static Vector3i operator << ( Vector3i a, Vector3i value )
//		{
//			return new Vector3i( a.X << value.X , 
//								 a.Y << value.Y , 
//								 a.Z << value.Z );
//		}

		/// <summary></summary>
		public Vector2i Xx { get { return new Vector2i( X, X ); } }
		/// <summary></summary>
		public Vector2i Xy { get { return new Vector2i( X, Y ); } }
		/// <summary></summary>
		public Vector2i Xz { get { return new Vector2i( X, Z ); } }
		/// <summary></summary>
		public Vector2i Yx { get { return new Vector2i( Y, X ); } }
		/// <summary></summary>
		public Vector2i Yy { get { return new Vector2i( Y, Y ); } }
		/// <summary></summary>
		public Vector2i Yz { get { return new Vector2i( Y, Z ); } }
		/// <summary></summary>
		public Vector2i Zx { get { return new Vector2i( Z, X ); } }
		/// <summary></summary>
		public Vector2i Zy { get { return new Vector2i( Z, Y ); } }
		/// <summary></summary>
		public Vector2i Zz { get { return new Vector2i( Z, Z ); } }

		/// <summary></summary>
		public Vector3i Xxx { get { return new Vector3i( X, X, X ); } }
		/// <summary></summary>
		public Vector3i Xxy { get { return new Vector3i( X, X, Y ); } }
		/// <summary></summary>
		public Vector3i Xxz { get { return new Vector3i( X, X, Z ); } }
		/// <summary></summary>
		public Vector3i Xyx { get { return new Vector3i( X, Y, X ); } }
		/// <summary></summary>
		public Vector3i Xyy { get { return new Vector3i( X, Y, Y ); } }
		/// <summary></summary>
		public Vector3i Xyz { get { return new Vector3i( X, Y, Z ); } }
		/// <summary></summary>
		public Vector3i Xzx { get { return new Vector3i( X, Z, X ); } }
		/// <summary></summary>
		public Vector3i Xzy { get { return new Vector3i( X, Z, Y ); } }
		/// <summary></summary>
		public Vector3i Xzz { get { return new Vector3i( X, Z, Z ); } }
		/// <summary></summary>
		public Vector3i Yxx { get { return new Vector3i( Y, X, X ); } }
		/// <summary></summary>
		public Vector3i Yxy { get { return new Vector3i( Y, X, Y ); } }
		/// <summary></summary>
		public Vector3i Yxz { get { return new Vector3i( Y, X, Z ); } }
		/// <summary></summary>
		public Vector3i Yyx { get { return new Vector3i( Y, Y, X ); } }
		/// <summary></summary>
		public Vector3i Yyy { get { return new Vector3i( Y, Y, Y ); } }
		/// <summary></summary>
		public Vector3i Yyz { get { return new Vector3i( Y, Y, Z ); } }
		/// <summary></summary>
		public Vector3i Yzx { get { return new Vector3i( Y, Z, X ); } }
		/// <summary></summary>
		public Vector3i Yzy { get { return new Vector3i( Y, Z, Y ); } }
		/// <summary></summary>
		public Vector3i Yzz { get { return new Vector3i( Y, Z, Z ); } }
		/// <summary></summary>
		public Vector3i Zxx { get { return new Vector3i( Z, X, X ); } }
		/// <summary></summary>
		public Vector3i Zxy { get { return new Vector3i( Z, X, Y ); } }
		/// <summary></summary>
		public Vector3i Zxz { get { return new Vector3i( Z, X, Z ); } }
		/// <summary></summary>
		public Vector3i Zyx { get { return new Vector3i( Z, Y, X ); } }
		/// <summary></summary>
		public Vector3i Zyy { get { return new Vector3i( Z, Y, Y ); } }
		/// <summary></summary>
		public Vector3i Zyz { get { return new Vector3i( Z, Y, Z ); } }
		/// <summary></summary>
		public Vector3i Zzx { get { return new Vector3i( Z, Z, X ); } }
		/// <summary></summary>
		public Vector3i Zzy { get { return new Vector3i( Z, Z, Y ); } }
		/// <summary></summary>
		public Vector3i Zzz { get { return new Vector3i( Z, Z, Z ); } }
	}

	/// @if LANG_EN
	/// <summary>Common interface for Bounds2, Sphere2, ConvexPoly2.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Bounds2, Sphere2, ConvexPoly2 の共通インターフェース。
	/// </summary>
	/// @endif
	public interface ICollisionBasics
	{
		/// @if LANG_EN
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		bool IsInside( Vector2 point );
		/// @if LANG_EN
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign );	
		/// @if LANG_EN
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		float SignedDistance( Vector2 point );
		/// @if LANG_EN
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		bool NegativeClipSegment( ref Vector2 A, ref Vector2 B );
	}
/*
	/// @if LANG_EN
	/// <summary>
	/// An axis aligned box class in 2D (integers).
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// </summary>
	/// @endif
	public struct Bounds2i
	{
		/// @if LANG_EN
		/// <summary>Minimum point (lower left).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Min;

		/// @if LANG_EN
		/// <summary>Maximum point (upper right)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Max;

		/// @if LANG_EN
		/// <summary>The Width/Height ratio.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public float Aspect { get { Vector2 size = Size.Vector2(); return size.X / size.Y;}} 

		/// @if LANG_EN
		/// <summary>The center of the bounds (Max+Min)/2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2 Center { get { return( Max + Min ).Vector2() * 0.5f;}} 

		/// @if LANG_EN
		/// <summary>The Size the bounds (Max-Min).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Size { get { return( Max - Min );}} 

		/// @if LANG_EN
		/// <summary>Return true if the size is (0,0).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public bool IsEmpty()
		{
			return Max == Min;
		}

		/// @if LANG_EN
		/// <summary>
		/// Bounds2i constructor.
		/// All functions in Bounds2i assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
		/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
		/// takes anything the user passes without trying to fix it.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
		/// <param name="max">The top right point. Max is set to that value without further checking.</param>
		public Bounds2i( Vector2i min, Vector2i max )
		{
			Min = min; 
			Max = max;
		}

		/// @if LANG_EN
		/// <summary>
		/// Bounds2i constructor. 
		/// Return a zero size bounds. You can then use Add to expand it. 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		/// <param name="point">Location of the Bounds2.</param>
		public Bounds2i( Vector2i point )
		{
			Min = point; 
			Max = point;
		}

		/// @if LANG_EN
		/// <summary>
		/// Create a Bounds2i that goes through 2 points, the min and max are recalculated.
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		/// @endif
		static public Bounds2i SafeBounds( Vector2i min, Vector2i max )
		{
			return new Bounds2i( min.Min( max ), min.Max( max ) );
		}

		/// @if LANG_EN
		/// <summary>(0,0) -> (0,0) box.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		static public Bounds2i Zero = new Bounds2i( new Vector2i(0,0), new Vector2i(0,0) );

		/// @if LANG_EN
		/// <summary>Translate bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public static Bounds2i operator + ( Bounds2i bounds, Vector2i value )
		{
			return new Bounds2i( bounds.Min + value, 
								bounds.Max + value );
		}

		/// @if LANG_EN
		/// <summary>Translate bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public static Bounds2i operator - ( Bounds2i bounds, Vector2i value )
		{
			return new Bounds2i( bounds.Min - value, 
								bounds.Max - value );
		}

		/// @if LANG_EN
		/// <summary>Return true if this and 'bounds' overlap.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public bool Overlaps( Bounds2i bounds )
		{
			if ( Min.X > bounds.Max.X || bounds.Min.X > Max.X )	return false;
			if ( Min.Y > bounds.Max.Y || bounds.Min.Y > Max.Y )	return false;

			return true;
		}

		/// @if LANG_EN
		/// <summary>Return the Bounds2i resulting from the intersection of 2 bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Bounds2i Intersection( Bounds2i bounds ) 
		{
			Vector2i mi = Min.Max( bounds.Min );
			Vector2i ma = Max.Min( bounds.Max );

			Vector2i dim = ma - mi;

			if ( dim.X < 0.0f || dim.Y < 0.0f )
				return Zero;

			return new Bounds2i( mi, ma );
		} 

		/// @if LANG_EN
		/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public void Add( Vector2i point )
		{
			Min = Min.Min( point );
			Max = Max.Max( point );
		}

		/// @if LANG_EN
		/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public void Add( Bounds2i bounds )
		{
			Add( bounds.Min );
			Add( bounds.Max );
		}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// @if LANG_EN
		/// <summary>The bottom left point (which is also Min).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Point00 { get { return Min;}}
		/// @if LANG_EN
		/// <summary>The top right point (which is also Max).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Point11 { get { return Max;}}
		/// @if LANG_EN
		/// <summary>The bottom right point.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Point10 { get { return new Vector2i(Max.X,Min.Y);}}
		/// @if LANG_EN
		/// <summary>The top left point.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public Vector2i Point01 { get { return new Vector2i(Min.X,Max.Y);}}

		/// @if LANG_EN
		/// <summary>Return the string representation of this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public override string ToString()
		{
			return Min.ToString() + " " + Max.ToString();
		}

		/// @if LANG_EN
		/// <summary></summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public bool IsInside( Vector2i point )
		{
			return point == point.Max( Min ).Min( Max );
		}
	}
*/
	/// @if LANG_EN
	/// <summary>
	/// An axis aligned box class in 2D.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// An axis aligned box class in 2D.
	/// </summary>
	/// @endif
	public struct Bounds2 : ICollisionBasics
	{
		/// @if LANG_EN
		/// <summary>Minimum point (lower left).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Minimum point (lower left).</summary>
		/// @endif
		public Vector2 Min;

		/// @if LANG_EN
		/// <summary>Maximum point (upper right)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Maximum point (upper right)</summary>
		/// @endif
		public Vector2 Max;

		/// @if LANG_EN
		/// <summary>The Width/Height ratio.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The Width/Height ratio.</summary>
		/// @endif
		public float Aspect { get { Vector2 size = Size; return size.X / size.Y;}} 

		/// @if LANG_EN
		/// <summary>The center of the bounds (Max+Min)/2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The center of the bounds (Max+Min)/2.</summary>
		/// @endif
		public Vector2 Center { get { return( Max + Min ) * 0.5f;}} 

		/// @if LANG_EN
		/// <summary>The Size the bounds (Max-Min).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The Size the bounds (Max-Min).</summary>
		/// @endif
		public Vector2 Size { get { return( Max - Min );}} 

		/// @if LANG_EN
		/// <summary>Return true if the size is (0,0).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return true if the size is (0,0).</summary>
		/// @endif
		public bool IsEmpty()
		{
			return Max == Min;
		}

		/// @if LANG_EN
		/// <summary>
		/// Bounds2 constructor.
		/// All functions in Bounds2 assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
		/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
		/// takes anything the user passes without trying to fix it.
		/// </summary>
		/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
		/// <param name="max">The top right point. Max is set to that value without further checking.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Bounds2 constructor.
		/// All functions in Bounds2 assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
		/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
		/// takes anything the user passes without trying to fix it.
		/// </summary>
		/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
		/// <param name="max">The top right point. Max is set to that value without further checking.</param>
		/// @endif
		public Bounds2( Vector2 min, Vector2 max )
		{
			Min = min; 
			Max = max;
		}

		/// @if LANG_EN
		/// <summary>
		/// Bounds2 constructor. 
		/// Return a zero size bounds. You can then use Add to expand it. 
		/// </summary>
		/// <param name="point">Location of the Bounds2.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Bounds2 constructor. 
		/// Return a zero size bounds. You can then use Add to expand it. 
		/// </summary>
		/// <param name="point">Location of the Bounds2.</param>
		/// @endif
		public Bounds2( Vector2 point )
		{
			Min = point; 
			Max = point;
		}

		/// @if LANG_EN
		/// <summary>
		/// Create a Bounds2 that goes through 2 points, the min and max are recalculated.
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Create a Bounds2 that goes through 2 points, the min and max are recalculated.
		/// </summary>
		/// <param name="min">First point.</param>
		/// <param name="max">Second point.</param>
		/// @endif
		static public Bounds2 SafeBounds( Vector2 min, Vector2 max )
		{
			return new Bounds2( min.Min( max ), min.Max( max ) );
		}

		/// @if LANG_EN
		/// <summary>(0,0) -> (0,0) box.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>(0,0) -> (0,0) box.</summary>
		/// @endif
		static public Bounds2 Zero = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(0.0f,0.0f) );

		/// @if LANG_EN
		/// <summary>(0,0) -> (1,1) box.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>(0,0) -> (1,1) box.</summary>
		/// @endif
		static public Bounds2 Quad0_1 = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(1.0f,1.0f) );

		/// @if LANG_EN
		/// <summary>(-1,-1) -> (1,1) box.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>(-1,-1) -> (1,1) box.</summary>
		/// @endif
		static public Bounds2 QuadMinus1_1 = new Bounds2( new Vector2(-1.0f,-1.0f), new Vector2(1.0f,1.0f) );

		/// @if LANG_EN
		/// <summary>
		/// Return a box that goes from (-h,-h) to (h,h).
		/// We don't check for sign.
		/// </summary>
		/// <param name="h">Half size of the square.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return a box that goes from (-h,-h) to (h,h).
		/// We don't check for sign.
		/// </summary>
		/// <param name="h">Half size of the square.</param>
		/// @endif
		static public Bounds2 CenteredSquare( float h )
		{
			Vector2 half_vec = new Vector2( h, h );
			return new Bounds2( -half_vec, half_vec );
		}

		/// @if LANG_EN
		/// <summary>Translate bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Translate bounds.</summary>
		/// @endif
		public static Bounds2 operator + ( Bounds2 bounds, Vector2 value )
		{
			return new Bounds2( bounds.Min + value, 
								bounds.Max + value );
		}

		/// @if LANG_EN
		/// <summary>Translate bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Translate bounds.</summary>
		/// @endif
		public static Bounds2 operator - ( Bounds2 bounds, Vector2 value )
		{
			return new Bounds2( bounds.Min - value, 
								bounds.Max - value );
		}

		/// @if LANG_EN
		/// <summary>Return true if this and 'bounds' overlap.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return true if this and 'bounds' overlap.</summary>
		/// @endif
		public bool Overlaps( Bounds2 bounds )
		{
			if ( Min.X > bounds.Max.X || bounds.Min.X > Max.X )	return false;
			if ( Min.Y > bounds.Max.Y || bounds.Min.Y > Max.Y )	return false;

			return true;
		}

		/// @if LANG_EN
		/// <summary>Return the Bounds2 resulting from the intersection of 2 bounds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return the Bounds2 resulting from the intersection of 2 bounds.</summary>
		/// @endif
		public Bounds2 Intersection( Bounds2 bounds ) 
		{
			Vector2 mi = Min.Max( bounds.Min );
			Vector2 ma = Max.Min( bounds.Max );

			Vector2 dim = ma - mi;

			if ( dim.X < 0.0f || dim.Y < 0.0f )
				return Zero;

			return new Bounds2( mi, ma );
		} 

		/// @if LANG_EN
		/// <summary>
		/// Scale bounds around a given pivot.
		/// </summary>
		/// <param name="scale">Amount of scale.</param>
		/// <param name="center">Scale center.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Scale bounds around a given pivot.
		/// </summary>
		/// <param name="scale">Amount of scale.</param>
		/// <param name="center">Scale center.</param>
		/// @endif
		public Bounds2 Scale( Vector2 scale, Vector2 center )
		{
			return new Bounds2( ( Min - center ) * scale + center, 
								( Max - center ) * scale + center );
		}

		/// @if LANG_EN
		/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
		/// @endif
		public void Add( Vector2 point )
		{
			Min = Min.Min( point );
			Max = Max.Max( point );
		}

		/// @if LANG_EN
		/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
		/// @endif
		public void Add( Bounds2 bounds )
		{
			Add( bounds.Min );
			Add( bounds.Max );
		}

		// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')

		/// @if LANG_EN
		/// <summary>The bottom left point (which is also Min).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The bottom left point (which is also Min).</summary>
		/// @endif
		public Vector2 Point00 { get { return Min;}}
		/// @if LANG_EN
		/// <summary>The top right point (which is also Max).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The top right point (which is also Max).</summary>
		/// @endif
		public Vector2 Point11 { get { return Max;}}
		/// @if LANG_EN
		/// <summary>The bottom right point.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The bottom right point.</summary>
		/// @endif
		public Vector2 Point10 { get { return new Vector2(Max.X,Min.Y);}}
		/// @if LANG_EN
		/// <summary>The top left point.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The top left point.</summary>
		/// @endif
		public Vector2 Point01 { get { return new Vector2(Min.X,Max.Y);}}

		/// @if LANG_EN
		/// <summary>Return the string representation of this Bounds2.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return the string representation of this Bounds2.</summary>
		/// @endif
		public override string ToString()
		{
			return Min.ToString() + " " + Max.ToString();
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		public bool IsInside( Vector2 point )
		{
			return point == point.Max( Min ).Min( Max );
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			Vector2 closest = point.Max( Min ).Min( Max );

			if ( closest != point )
			{
				ret = closest;
				sign = 1.0f;
				return;
			}

			Vector2 l = closest; l.X = Min.X; float dl = closest.X - Min.X; 
			Vector2 r = closest; r.X = Max.X; float dr = Max.X - closest.X; 
			Vector2 t = closest; t.Y = Min.Y; float dt = closest.Y - Min.Y; 
			Vector2 b = closest; b.Y = Max.Y; float db = Max.Y - closest.Y; 

			ret = l; 
			float d = dl;

			if ( d > dr )
			{
				ret = r; 
				d = dr;
			}

			if ( d > dt )
			{
				ret = t; 
				d = dt;
			}

			if ( d > db )
			{
				ret = b; 
				d = db;
			}

			sign = -1.0f;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		public float SignedDistance( Vector2 point )
		{
			Vector2 p;
			float s = 0.0f;
			ClosestSurfacePoint( point, out p, out s );
			return s * ( p - point ).Length();
		}

		/// @if LANG_EN
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			bool ret = true;

			ret &= ( new Plane2( Min, -Math._10 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Min, -Math._01 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Max,  Math._10 ) ).NegativeClipSegment( ref A, ref B );
			ret &= ( new Plane2( Max,  Math._01 ) ).NegativeClipSegment( ref A, ref B );

			return ret;
		}

		/// @if LANG_EN
		/// <summary>
		/// Swap y coordinates for top and bottom, handy for hacking uvs
		/// in system that use 0,0 as top left. Also, this will generate
		/// an invalid Bounds2 and all functions in that class will break
		/// (intersections, add etc.)
		/// 
		/// Functions like Point00, Point10 etc can still be used.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Swap y coordinates for top and bottom, handy for hacking uvs
		/// in system that use 0,0 as top left. Also, this will generate
		/// an invalid Bounds2 and all functions in that class will break
		/// (intersections, add etc.)
		/// 
		/// Functions like Point00, Point10 etc can still be used.
		/// </summary>
		/// @endif
		public Bounds2 OutrageousYTopBottomSwap()
		{
			Bounds2 ret = this;
			float y = Min.Y;
			ret.Min.Y = ret.Max.Y;
			ret.Max.Y = y;
			return ret;
		}

		/// @if LANG_EN
		/// <summary>
		/// Similar to OutrageousYTopBottomSwap, but instead of
		/// swapping top and bottom y, it just does y=1-y. Same
		/// comment as OutrageousYTopBottomSwap.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Similar to OutrageousYTopBottomSwap, but instead of
		/// swapping top and bottom y, it just does y=1-y. Same
		/// comment as OutrageousYTopBottomSwap.
		/// </summary>
		/// @endif
		public Bounds2 OutrageousYVCoordFlip()
		{
			Bounds2 ret = this;
			ret.Min.Y = 1.0f - ret.Min.Y;
			ret.Max.Y = 1.0f - ret.Max.Y;
			return ret;
		}
	}

	/// @if LANG_EN
	/// <summary>A plane class in 2D.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>A plane class in 2D.</summary>
	/// @endif
	public struct Plane2 : ICollisionBasics
	{
		/// @if LANG_EN
		/// <summary>A base point on the plane.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>A base point on the plane.</summary>
		/// @endif
		public Vector2 Base;

		/// @if LANG_EN
		/// <summary>The plane normal vector, assumed to be unit length. If this is not the case, some functions will have undefined behaviour.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>The plane normal vector, assumed to be unit length. If this is not the case, some functions will have undefined behaviour.</summary>
		/// @endif
		public Vector2 UnitNormal;

		/// @if LANG_EN
		/// <summary>Plane2 constructor</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Plane2 constructor</summary>
		/// @endif
		public Plane2( Vector2 a_base, Vector2 a_unit_normal )
		{
			Base = a_base;
			UnitNormal = a_unit_normal;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		public bool IsInside( Vector2 point )
		{
			return SignedDistance( point ) <= 0.0f;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			float d = SignedDistance( point );
			ret = point - d * UnitNormal;
			sign = d > 0.0f ? 1.0f : -1.0f;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		public float SignedDistance( Vector2 point )
		{
			return( point - Base ).Dot( UnitNormal );
		}

		/// @if LANG_EN
		/// <summary>
		/// Project a point on this plane.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Project a point on this plane.
		/// </summary>
		/// @endif
		public Vector2 Project( Vector2 point )
		{
			return point - SignedDistance( point ) * UnitNormal;
		}

		/// @if LANG_EN
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			float dA = SignedDistance( A );
			float dB = SignedDistance( B );

			bool Ain = ( dA >= 0.0f );
			bool Bin = ( dB >= 0.0f );

			if ( Ain && Bin )
				return false;

			if ( Ain && (!Bin) )
			{
				Vector2 AB = B - A;
				float alpha = -dA / AB.Dot( UnitNormal );
				Vector2 I = A + alpha * AB;
				A = I;
			}
			else if ( (!Ain) && Bin )
			{
				Vector2 AB = B - A;
				float alpha = -dA / AB.Dot( UnitNormal );
				Vector2 I = A + alpha * AB;
				B = I;
			}

			return true;
		}
	}

	/// @if LANG_EN
	/// <summary>A sphere class in 2D.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>A sphere class in 2D.</summary>
	/// @endif
	public struct Sphere2 : ICollisionBasics
	{
		/// @if LANG_EN
		/// <summary>Sphere center.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Sphere center.</summary>
		/// @endif
		public Vector2 Center;

		/// @if LANG_EN
		/// <summary>Sphere radius.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Sphere radius.</summary>
		/// @endif
		public float Radius; 

		/// @if LANG_EN
		/// <summary>Sphere2 constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Sphere2 constructor.</summary>
		/// @endif
		public Sphere2( Vector2 center, float radius )
		{
			Center = center;
			Radius = radius;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		public bool IsInside( Vector2 point )
		{
			return( point - Center ).LengthSquared() <= Radius * Radius;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			Vector2 r = point - Center;
			float len = r.Length();
			float d = len - Radius;

			if ( len < 0.00001f )
			{
				ret = Center + new Vector2( 0.0f, Radius );	// degenerate case, pick any separation direction
				sign = -1.0f;
				return;
			}

			ret = point - d * ( r / len );
			sign = d > 0.0f ? 1.0f : -1.0f;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		public float SignedDistance( Vector2 point )
		{
			return( point - Center ).Length() - Radius;
		}

		/// @if LANG_EN
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
		{
			Vector2 AB = B - A;
			float r_sqr = Radius * Radius;

			float epsilon = 0.00000001f;
			if ( AB.LengthSquared() <= epsilon )
			{
				// A and B are the same point
				if ( ( A - Center ).LengthSquared() >= r_sqr )
					return false;
			}

			Vector2 p = Center.ProjectOnLine( A, AB );
			float d_sqr = ( p - Center ).LengthSquared();
			
			if ( d_sqr >= r_sqr )
				return false;

			float e = FMath.Sqrt( FMath.Max( 0.0f, r_sqr - d_sqr ) );
			Vector2 v = AB.Normalize();
			Vector2 A2 = p - e * v;
			Vector2 B2 = p + e * v;

			if ( ( A - B2 ).Dot( AB ) >= 0.0f ) return false;
			if ( ( B - A2 ).Dot( AB ) <= 0.0f ) return false;
			 				  
			if ( ( A - A2 ).Dot( AB ) < 0.0f ) A = A2;
			if ( ( B - B2 ).Dot( AB ) > 0.0f ) B = B2;

			return true;
		}
	}

	/// @if LANG_EN
	/// <summary>A convex polygon class in 2D.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>A convex polygon class in 2D.</summary>
	/// @endif
	public struct ConvexPoly2 : ICollisionBasics
	{
		/// @if LANG_EN
		/// <summary>
		/// The convex poly is stored as a list of planes assumed to define a 
		/// convex region. Plane base points are also polygon vertices.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// The convex poly is stored as a list of planes assumed to define a 
		/// convex region. Plane base points are also polygon vertices.
		/// </summary>
		/// @endif
		public Plane2[] Planes; 

		Sphere2 m_sphere;

		/// @if LANG_EN
		/// <summary>Bounding sphere, centered at center of mass.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Bounding sphere, centered at center of mass.</summary>
		/// @endif
		public Sphere2 Sphere{ get { return m_sphere; } }

		/// @if LANG_EN
		/// <summary>
		/// ConvexPoly2 constructor.
		/// Assumes input points define a convex region.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// ConvexPoly2 constructor.
		/// Assumes input points define a convex region.
		/// </summary>
		/// @endif
		public ConvexPoly2( Vector2[] points )
		{
			Vector2 center = GameEngine2D.Base.Math._00;
			Planes = new Plane2[ points.Length ];

			for ( int n=points.Length,i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 p1 = points[i];
				Vector2 p2 = points[i_next];

				Planes[i] = new Plane2( p1, -Math.Perp( p2 - p1 ).Normalize() );

				center += p1;
			}

			center /= (float)points.Length;

			float radius = 0.0f;
			for ( int i = 0; i != points.Length; ++i )
				radius = FMath.Max( radius, ( points[i] - center ).Length() );

			m_sphere = new Sphere2( center, radius );
		}


		/// <summary></summary>
		public void MakeBox( Bounds2 bounds )
		{
			Planes = new Plane2[ 4 ];

			Planes[0] = new Plane2( bounds.Point00, -Math._10 );
			Planes[1] = new Plane2( bounds.Point10, -Math._01 );
			Planes[2] = new Plane2( bounds.Point11,  Math._10 );
			Planes[3] = new Plane2( bounds.Point01,  Math._01 );

			m_sphere = new Sphere2( bounds.Center, bounds.Size.Length() * 0.5f );
		}

		/// <summary></summary>
		public void MakeRegular( uint num, float r )
		{
			Planes = new Plane2[ num ];

			float a2 = Math.TwicePi * 0.5f / (float)num;

			for ( uint i = 0; i != num; ++i )
			{
				float a = Math.TwicePi * (float)i / (float)num;
				Vector2 p = Vector2.Rotation( a + a2 );
				Vector2 n = Vector2.Rotation( a );
				Planes[i] = new Plane2( p * r, n );
			}

			m_sphere = new Sphere2( GameEngine2D.Base.Math._00, r );
		}

		/// @if LANG_EN
		/// <summary>Return the number of vertices (or faces)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Return the number of vertices (or faces)</summary>
		/// @endif
		public uint Size()
		{
			return(uint)Planes.Length;
		}

		/// @if LANG_EN
		/// <summary>Get a vertex position.</summary>
		/// <param name="index">The vertex index.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>Get a vertex position.</summary>
		/// <param name="index">The vertex index.</param>
		/// @endif
		public Vector2 GetPoint( int index )
		{
			return Planes[index].Base;
		}

		/// @if LANG_EN
		/// <summary>Get the normal vector of a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>Get the normal vector of a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		/// @endif
		public Vector2 GetNormal( int index )
		{
			return Planes[index].UnitNormal;
		}

		/// @if LANG_EN
		/// <summary>Get the plane formed by a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>Get the plane formed by a face of this poly.</summary>
		/// <param name="index">The face index.</param>
		/// @endif
		public Plane2 GetPlane( int index )
		{
			return Planes[index];
		}

		/// @if LANG_EN
		/// <summary>Calculate the bounds of this poly.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Calculate the bounds of this poly.</summary>
		/// @endif
		public Bounds2 CalcBounds()
		{
			if ( Size() == 0 )
				return Bounds2.Zero;

			Bounds2 retval = new Bounds2( GetPoint( 0 ), GetPoint( 0 ) );
			for ( int i = 1; i != (int)Size(); ++i )
				retval.Add( GetPoint( i ) );
			return retval;
		}

		/// @if LANG_EN
		/// <summary>Calculate the gravity center of this poly.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Calculate the gravity center of this poly.</summary>
		/// @endif
		public Vector2 CalcCenter()
		{
			Vector2 center = GameEngine2D.Base.Math._00;
			float area = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 A = GetPoint( i );
				Vector2 B = GetPoint( i_next );
				float det = Math.Det( A, B );
				area += det;
				center += det * ( A + B );
			}

			area /= 2.0f;
			center /= ( 6.0f * area );

			return center;
		}

		/// @if LANG_EN
		/// <summary>Calculate the area of this convex poly.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Calculate the area of this convex poly.</summary>
		/// @endif
		public float CalcArea()
		{
			float area = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
				area += Math.Det( GetPoint( i ), GetPoint( i_next ) );

			area /= 2.0f;

			return area;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return true if 'point' is inside the primitive (in its negative space).
		/// </summary>
		/// @endif
		public bool IsInside(  Vector2 point ) 
		{
			foreach ( Plane2 plane in Planes )
			{
				if ( plane.SignedDistance( point ) > 0.0f )
					return false;
			}

			return true;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the closest point to 'point' that lies on the surface of the primitive.
		/// If that point is inside the primitive, sign is negative.
		/// </summary>
		/// @endif
		public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
		{
			ret = GameEngine2D.Base.Math._00;

			float max_neg_d = -100000.0f;
			int max_neg_d_plane_index = -1;
			bool outside = false;

			for ( int i=0; i != Planes.Length; ++i )
			{
				float d = Planes[i].SignedDistance( point );

				if ( d > 0.0f )	outside = true;
				else if ( max_neg_d < d )
				{
					max_neg_d = d;
					max_neg_d_plane_index = i;
				}
			}

			if ( !outside )
			{
				sign = -1.0f;
				ret = point - max_neg_d * Planes[max_neg_d_plane_index].UnitNormal;
				return;
			}

			// brute force

			float d_sqr_min = 0.0f;

			for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
			{
				Vector2 p = Math.ClosestSegmentPoint( point, GetPoint( i ), GetPoint( i_next ) );
				float d_sqr = ( p - point ).LengthSquared();

				if ( i==n-1 || d_sqr < d_sqr_min )
				{
					ret = p;
					d_sqr_min = d_sqr;
				}
			}

			sign = 1.0f;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Return the signed distance (penetration distance) from 'point' 
		/// to the surface of the primitive.
		/// </summary>
		/// @endif
		public float SignedDistance( Vector2 point )
		{
			Vector2 p;
			float s = 0.0f;
			ClosestSurfacePoint( point, out p, out s );
			return s * ( p - point ).Length();
		}


		/// <summary></summary>
		public void Translate( Vector2 dx, ConvexPoly2 poly )
		{
			Planes = new Plane2[ poly.Planes.Length ];

			for ( int i=0; i != poly.Planes.Length; ++i )
			{
				Planes[i] = poly.Planes[i];
				Planes[i].Base += dx;
			}

			m_sphere = poly.m_sphere;
			m_sphere.Center += dx;
		}

		/// @if LANG_EN
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Assuming the primitive is convex, clip the segment AB against the primitive.
		/// Return false if AB is entirely in positive halfspace,
		/// else clip against negative space and return true.
		/// </summary>
		/// @endif
		public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B ) 
		{
			for ( int i=0; i != Planes.Length; ++i )
			{
				if ( !Planes[i].NegativeClipSegment( ref A, ref B ) )
					return false;
			}

			return true;
		}
	}
}

