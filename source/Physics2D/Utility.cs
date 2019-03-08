/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Text;

using Sce.PlayStation.Core;

//
// typedef (of original C code) and
// #define MACRO were changed to PhysicsUtility Class for C# implementation
//
namespace Sce.PlayStation.HighLevel.Physics2D
{
	/// @if LANG_EN
	/// <summary> Utility class for Physics2D </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary> Physics2Dのためのユーティリティクラス </summary>
	/// @endif
	public class PhysicsUtility
    {
		#region Fields
		/// @if LANG_EN
		/// <summary> Internal Pi used for Physics2D simulation </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> Physics2D計算内で使用するPi </summary>
		/// @endif
        public const float Pi = 3.14159265f;
		
		/// @if LANG_EN
		/// <summary> Internal FltMax used for Physics2D simulation </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> Physics2D計算内で使用するFltMax </summary>
		/// @endif
        public const float FltMax = 1E+37f;
		#endregion
		
		#region Methods
		/// @if LANG_EN
		/// <summary> Get one of the pair index, totalIndex -> index1 </summary>
		/// <param name="val"> given totalIndex of PhysicsSolverPair etc </param>
		/// <returns> index1 </returns>
		/// @endif
		/// @if LANG_JA
		/// <summary> 合成のインデックスから１番目のインデックスを取り出す </summary>
		/// <param name="val"> PhysicsSolverPairなどの合成のインデックス </param>
		/// <returns> １番目のインデックス </returns>
		/// @endif
        public static uint UnpackIdx1(uint val)
        {
            return ((uint)(((val) & 0xFFFF0000) >> 16));
        }
		
		/// @if LANG_EN
		/// <summary> Get one of the pair index, totalIndex -> index2 </summary>	
		/// <param name="val"> given totalIndex of PhysicsSolverPair etc </param>
		/// <returns> index2 </returns>
		/// @endif
		/// @if LANG_JA
		/// <summary> 合成のインデックスから２番目のインデックスを取り出す </summary>	
		/// <param name="val"> PhysicsSolverPairなどの合成のインデックス </param>
		/// <returns> ２番目のインデックス </returns>
		/// @endif
        public static uint UnpackIdx2(uint val)
        {
            return ((uint)(((val) & 0x0000FFFF)));
        }
		
		/// @if LANG_EN
		/// <summary> Pack two indices to one index by shifting and summing </summary>	
		/// <param name="val1"> given first index </param>
		/// <param name="val2"> given second index </param>
		/// <returns> totalIndex </returns>
		/// @endif
		/// @if LANG_JA
		/// <summary> ２つのインデックスからシフト演算と和を取って合成のインデックスを計算する </summary>	
		/// <param name="val1"> １番目のインデックス </param>
		/// <param name="val2"> ２番目のインデックス </param>
		/// <returns> 合成のインデックス </returns>
		/// @endif
        public static uint PackIdx(uint val1, uint val2)
        {
            return ((((uint)val1) << 16 & 0xFFFF0000) | ((uint)val2 & 0x0000FFFF));
        }
		
		/// @if LANG_EN
		/// <summary> Swap index1 and index2 </summary>	
		/// <param name="val"> given totalIndex of PhysicsSolverPair etc </param>
		/// <returns> totalIndex by swapping </returns>
		/// @endif 
		/// @if LANG_JA
		/// <summary> 合成のインデックスの１番目と２番目を入れ替える </summary>	
		/// <param name="val"> PhysicsSolverPairなどの合成のインデックス </param>
		/// <returns> 入れ替えた合成インデックス </returns>
		/// @endif 
        public static uint SwapIdx(uint val)
        {
            return ((uint)(((val) & 0xFFFF0000) >> 16) | (uint)((val) & 0x0000FFFF) << 16);
        }
		
		/// @if LANG_EN  
		/// <summary> angle in degree -> radian conversion </summary>
        /// <param name="angle"> given angle in degree </param>
		/// @endif
		/// @if LANG_JA  
		/// <summary> Degreeからラジアンへの変換 </summary>
        /// <param name="angle"> Degreeでの角度 </param>
		/// @endif
		public static float GetRadian(float angle)
		{
			return (angle/180.0f * Pi);
		}
		
		/// @if LANG_EN  
		/// <summary> angle-> rearranged to [-Pi, Pi] conversion </summary>
        /// <param name="angle"> given angle in radian </param>
		/// @endif
		/// @if LANG_JA  
		/// <summary> ラジアン角度を[-Pi, Pi]の範囲に変換する  </summary>
        /// <param name="angle"> ラジアンでの角度 </param>
		/// @endif
		public static float GetRadianMod(float angle)
		{
			if(angle > Pi)
			{ 
				int div_mod = (int)Math.Floor((angle - (-Pi))/(2.0f*Pi));
				angle -= div_mod * (2.0f * Pi);
			}
			
			if(angle < -Pi)
			{
				int div_mod = (int)Math.Floor((Pi - angle)/(2.0f*Pi));
				angle += div_mod * (2.0f * Pi);
			}
		
			return angle;
		}
		#endregion

    }
	
		
	public partial class PhysicsJoint
    {		
		/// @if LANG_EN		
		/// <summary> 2d vector internal math function </summary>
		/// @endif
		/// @if LANG_JA	
		/// <summary> 2d vector 計算内部用関数 </summary>
		/// @endif
		private class Vector2Util
		{ 
			#region Methods
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
		    /// <param name="vec1"> 2d vector </param>
		    /// <param name="vec2"> 2d vector </param>
		    /// <returns> cross product of 2d vector and 2d vector </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
		    /// <param name="vec1"> ２次元ベクトル </param>
		    /// <param name="vec2"> ２次元ベクトル </param>
		    /// <returns> ２次元ベクトルと２次元ベクトルの外積 </returns>
			/// @endif
		    public static float Cross(Vector2 vec1, Vector2 vec2)
		    {
				return vec1.X * vec2.Y - vec1.Y * vec2.X;
			}	
				
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
			/// <param name="val"> scalar </param>
		    /// <param name="vec"> 2d vector </param>
		    /// <returns> cross product of scalar and 2d vector </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
		    /// <param name="val"> スカラー </param>
		    /// <param name="vec"> ２次元ベクトル </param>
		    /// <returns> スカラーと２次元ベクトルの外積 </returns>
			/// @endif
		    public static Vector2 Cross(float val, Vector2 vec)
			{
				Vector2 result = new Vector2(-val * vec.Y, val * vec.X);
				return result;
			}
				
			/// @if LANG_EN
			/// <summary> Cross Product </summary>
			/// <param name="vec"> 2d vector </param>
			/// <param name="val"> scalar </param>
			/// <returns> cross product of 2d vector and scalar </returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> 外積計算 </summary>
			/// <param name="vec"> ２次元ベクトル </param>
			/// <param name="val"> スカラー </param>
			/// <returns> ２次元ベクトルとスカラーの外積 </returns>
			/// @endif
			public static Vector2 Cross(Vector2 vec, float val)
			{
				Vector2 result = new Vector2(val * vec.Y, -val * vec.X);
				return result;
			}
			#endregion
		}
		
		/// @if LANG_EN	
		/// <summary> Mat2 </summary>
	    /// <remarks> 2d matrix internal math function </remarks>
		/// @endif
		/// @if LANG_JA	
		/// <summary> Mat2 </summary>
	    /// <remarks> 2d matrix 計算内部用関数 </remarks>
		/// @endif
	    private struct Mat2
	    {
	
	        #region Fields
			/// @if LANG_EN	
			/// <summary> col1 </summary>
			/// <remarks> column of 2d matrix </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary> col1 </summary>
			/// <remarks> ２次元行列のcolumn </remarks>
			/// @endif
	        public Vector2 col1;
			
			/// @if LANG_EN	
			/// <summary> col2 </summary>
			/// <remarks> column of 2d matrix </remarks>
			/// @endif
			/// @if LANG_JA
			/// <summary> col2 </summary>
			/// <remarks> ２次元行列のcolumn </remarks>
			/// @endif
			public Vector2 col2;
	        
	        #endregion
	
	        #region Methods
			/// @if LANG_EN	
			/// <summary> Mat2 Constructor </summary>
			/// <remarks> initialize all elements by val </remarks>
			/// <param name="val"> value used for initialization of elements </param>
			/// @endif
			/// @if LANG_JA	
			/// <summary> Mat2 Constructor </summary>
			/// <remarks> 全ての要素をvalで初期化 </remarks>
			/// <param name="val"> すべての要素をvalで初期化します </param>
			/// @endif
			public Mat2(float val)
			{
				col1.X = col1.Y = col2.X = col2.Y = val;
			}
			
			/// @if LANG_EN	
			/// <summary> MakeInvert </summary>
			/// <remarks> create the inverse of 2d matrix </remarks>
			/// <param name="mat"> 2d matrix </param>
			/// <returns> inverse of 2d matrix </returns>
			/// @endif
			/// @if LANG_JA	
			/// <summary> MakeInvert </summary>
			/// <remarks> ２次元行列の逆行列を作ります </remarks>
			/// <param name="mat"> ２次元行列 </param>
			/// <returns> ２次元行列の逆行列 </returns>
			/// @endif
	        public static Mat2 MakeInvert(Mat2 mat)
	        {
	            Mat2 result = new Mat2(0);
	            float a = mat.col1.X;
	            float b = mat.col1.Y;
	            float c = mat.col2.X;
	            float d = mat.col2.Y;
	            float det = a * d - b * c;
	
	            //if(det == 0.0f) 
	            // may cause the program crash
	
	            det = 1.0f / det;
	            result.col1.X = det * d;
	            result.col1.Y = det * -b;
	            result.col2.X = det * -c;
	            result.col2.Y = det * a;
	
	            return result;
	        }
			
			/// @if LANG_EN	
			/// <summary> operator + </summary>
			/// <param name="mat1"> 2d matrix </param>
			/// <param name="mat2"> 2d matrix </param>
			/// <returns> sum of 2d matrix </returns>
			/// @endif
			/// @if LANG_JA	
			/// <summary> operator + </summary>
			/// <param name="mat1"> ２次元行列</param>
			/// <param name="mat2"> ２次元行列 </param>
			/// <returns> ２次元行列の和 </returns>
			/// @endif
	        public static Mat2 operator +(Mat2 mat1, Mat2 mat2)
	        {
	            Mat2 result = new Mat2(0);
	            result.col1 = mat1.col1 + mat2.col1;
	            result.col2 = mat1.col2 + mat2.col2;
	            return result;
	        }
			
			/// @if LANG_EN	
			/// <summary> operator - </summary>
			/// <param name="mat1"> 2d matrix </param>
			/// <param name="mat2"> 2d matrix </param>
			/// <returns> difference of 2d matrix and 2d matrix </returns>
			/// @endif
			/// @if LANG_JA	
			/// <summary> operator - </summary>
			/// <param name="mat1"> ２次元行列 </param>
			/// <param name="mat2"> ２次元行列 </param>
			/// <returns> ２次元行列の差 </returns>
			/// @endif
	        public static Mat2 operator -(Mat2 mat1, Mat2 mat2)
	        {
	            Mat2 result = new Mat2(0);
	            result.col1 = mat1.col1 - mat2.col1;
	            result.col2 = mat1.col2 - mat2.col2;
	            return result;
	        }
			/// @if LANG_EN		
			/// <summary> operator * </summary>
			/// <param name="mat"> 2d matrix </param>
			/// <param name="vec"> 2d vector </param>
			/// <returns> multiply of 2d matrix and 2d vector </returns>
			/// @endif
			/// @if LANG_JA		
			/// <summary> operator * </summary>
			/// <param name="mat"> ２次元行列 </param>
			/// <param name="vec"> ２次元ベクトル </param>
			/// <returns> ２次元行列と２次元ベクトルの積 </returns>
			/// @endif
	        public static Vector2 operator *(Mat2 mat, Vector2 vec)
	        {
	            return new Vector2(
	                mat.col1.X * vec.X + mat.col1.Y * vec.Y,
	                mat.col2.X * vec.X + mat.col2.Y * vec.Y);
	        }
			
			/// @if LANG_EN	
			/// <summary> operator * </summary>
			/// <param name="mat1"> 2d matrix </param>
			/// <param name="mat2"> 2d matrix </param>
			/// <returns> multiply of 2d matrix and 2d matrix </returns>
			/// @endif
			/// @if LANG_JA	
			/// <summary> operator * </summary>
			/// <param name="mat1"> ２次元行列 </param>
			/// <param name="mat2"> ２次元行列 </param>
			/// <returns> ２次元行列と２次元行列の積 </returns>
			/// @endif
	        public static Mat2 operator *(Mat2 mat1, Mat2 mat2)
	        {
	
	            Mat2 result = new Mat2(0);
	
	            result.col1 = new Vector2(
	                mat1.col1.X * mat2.col1.X + mat1.col1.Y * mat2.col2.X,
	                mat1.col1.X * mat2.col1.Y + mat1.col1.Y * mat2.col2.Y);
	
	            result.col2 = new Vector2(
	                mat1.col2.X * mat2.col1.X + mat1.col2.Y * mat2.col2.X,
	                mat1.col2.X * mat2.col1.Y + mat1.col2.Y * mat2.col2.Y);
	
	            return result;
	        }
	        #endregion
	    }
	}
	
	public partial class PhysicsShape
    {
		private class Vector2Util
		{ 

		    public static float Cross(Vector2 vec1, Vector2 vec2)
		    {
				return vec1.X * vec2.Y - vec1.Y * vec2.X;
			}	
				
		    public static Vector2 Cross(float val, Vector2 vec)
			{
				Vector2 result = new Vector2(-val * vec.Y, val * vec.X);
				return result;
			}
				
			public static Vector2 Cross(Vector2 vec, float val)
			{
				Vector2 result = new Vector2(val * vec.Y, -val * vec.X);
				return result;
			}
		}	
	}
	
	public partial class PhysicsCollision
    {
		private class Vector2Util
		{ 

		    public static float Cross(Vector2 vec1, Vector2 vec2)
		    {
				return vec1.X * vec2.Y - vec1.Y * vec2.X;
			}	
				
		    public static Vector2 Cross(float val, Vector2 vec)
			{
				Vector2 result = new Vector2(-val * vec.Y, val * vec.X);
				return result;
			}
				
			public static Vector2 Cross(Vector2 vec, float val)
			{
				Vector2 result = new Vector2(val * vec.Y, -val * vec.X);
				return result;
			}
		}	
	}
	
	public partial class PhysicsScene
    {
		private class Vector2Util
		{ 

		    public static float Cross(Vector2 vec1, Vector2 vec2)
		    {
				return vec1.X * vec2.Y - vec1.Y * vec2.X;
			}	
				
		    public static Vector2 Cross(float val, Vector2 vec)
			{
				Vector2 result = new Vector2(-val * vec.Y, val * vec.X);
				return result;
			}
				
			public static Vector2 Cross(Vector2 vec, float val)
			{
				Vector2 result = new Vector2(val * vec.Y, -val * vec.X);
				return result;
			}
		}	
	}	
}
