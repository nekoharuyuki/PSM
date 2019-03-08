/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>A simple timer.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>シンプルなタイマー。
	/// </summary>
	/// @endif
	public class Timer
	{
//		System.DateTime m_start; // DateTime can't be trusted for profiling
		System.Diagnostics.Stopwatch m_stop_watch = new System.Diagnostics.Stopwatch();
		
		/// @if LANG_EN
		/// <summary>Timer constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Timerのコンストラクタ。
		/// </summary>
		/// @endif
		public Timer()
		{
			Reset();
		}
		
		/// @if LANG_EN
		/// <summary>
		/// Reset the timer.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>タイマーをリセットします。
		/// </summary>
		/// @endif
		public void Reset()
		{
//			m_start = System.DateTime.Now;
			m_stop_watch.Reset();
			m_stop_watch.Start();
		}
		
		/// @if LANG_EN
		/// <summary>
		/// Return time elapsed (in milliseconds) since constructor as called or since last call to Reset().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタが呼び出された瞬間、または Reset()が呼び出された瞬間からの経過時間をミリ秒単位で返します。
		/// </summary>
		/// @endif
		public double Milliseconds()
		{
//			return( System.DateTime.Now - m_start ).TotalMilliseconds;
//			return ( ( m_stop_watch.ElapsedTicks * 1000 ) / System.Diagnostics.Stopwatch.Frequency );
			return m_stop_watch.Elapsed.TotalMilliseconds;
		}
		
		/// @if LANG_EN
		/// <summary>
		/// Return time elapsed (in seconds) since constructor as called or since last call to Reset().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタが呼び出された瞬間、または Reset()が呼び出された瞬間からの経過時間を秒単位で返します。
		/// </summary>
		/// @endif
		public double Seconds()
		{
//			return( System.DateTime.Now - m_start ).TotalSeconds;
			return m_stop_watch.Elapsed.TotalSeconds;
		}
	}
}

