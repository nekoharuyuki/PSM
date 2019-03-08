/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sce.PlayStation.HighLevel.Physics2D
{
	public partial class PhysicsScene
    {	
		/// @if LANG_EN
		/// <summary> PhysicsStopwatch </summary>
		/// <remarks> timer class only used inside PhysicsScene class </remarks>
		/// @endif
		/// @if LANG_JA
		/// <summary> PhysicsStopwatch </summary>
		/// <remarks> タイマークラス、PhysicsSceneクラス内部のみで使われる</remarks>
		/// @endif 
		private class PhysicsStopwatch : Stopwatch
	    {
			public bool printPerf = false;
			
			/// @if LANG_EN
			/// <summary> Print </summary>
	        /// <param name="name"> string of simulation phase name</param>
			/// @endif
			/// @if LANG_JA
			/// <summary> Print </summary>
	        /// <param name="name"> シミュレーションのフェーズ名の文字列 </param>
			/// @endif
	        public void Print(string name)
	        {	
				if(printPerf)
				{
	            	double sec = (double)ElapsedTicks / (double)Frequency * 1000.0f;
	            	Console.WriteLine(name + " = " + sec + " [ms] ");
				}
	        }
				
			/// @if LANG_EN
			/// <summary> PrintNoOutPut </summary>
			/// <remarks> if you do not need to have the printout and just need to get performance </remarks>
	        /// <param name="name"> string of simulation phase name </param>
	        /// <returns> performance value</returns>
			/// @endif
			/// @if LANG_JA
			/// <summary> PrintNoOutPut </summary>
			/// <remarks> 出力が必要なく、パフォーマンスを計測するだけ </remarks>
	        /// <param name="name"> シミュレーションのフェーズ名の文字列 </param>
	        /// <returns> パフォーマンス値 </returns>
			/// @endif
	        public double PrintNoOutPut(string name)
	        {
	            double sec = (double)ElapsedTicks / (double)Frequency * 1000.0f;
	            return sec;
	        }
	    }
	}
}