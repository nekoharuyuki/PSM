/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System ;
using System.Collections.Generic ;
using Sce.PlayStation.Core  ;
using Sce.PlayStation.Core.Graphics ;

namespace Sce.PlayStation.HighLevel.Model
{
	//------------------------------------------------------------
	//  BasicProgramContainer
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>プログラムコンテナを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a program container</summary>
	/// @endif
	public class BasicProgramContainer : IDisposable {

		/// @if LANG_JA
		/// <summary>プログラムコンテナを作成する</summary>
		/// <param name="parameters">共有パラメータ (nullならば共有をおこなわない)</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a program container</summary>
		/// <param name="parameters">Shared parameter (not shared when null)</param>
		/// @endif
		public BasicProgramContainer( BasicParameters parameters = null ) {
			programs = new Dictionary<string, BasicProgram>() ;
			Parameters = parameters ;
		}
		/// @if LANG_JA
		/// <summary>プログラムコンテナのアンマネージドリソースを解放する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frees the unmanaged resources of the program container</summary>
		/// @endif
		public void Dispose() {
			Dispose( true ) ;
		}
		protected virtual void Dispose( bool disposing ) {
			if ( disposing ) {
				if ( programs != null ) {
					foreach ( var pair in programs ) pair.Value.Dispose() ;
				}
				programs = null ;
			}
		}

		/// @if LANG_JA
		/// <summary>プログラムを登録する</summary>
		/// <param name="key">検索キー</param>
		/// <param name="program">プログラム</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Registers a program</summary>
		/// <param name="key">Search key</param>
		/// <param name="program">Program</param>
		/// @endif
		public void Add( string key, BasicProgram program ) {
			if ( key == null ) key = "" ;
			if ( Parameters != null ) program.Parameters = Parameters ;
			programs[ key ] = program ;
		}
		/// @if LANG_JA
		/// <summary>プログラムを検索する</summary>
		/// <param name="key">検索キー</param>
		/// <returns>プログラム (nullならば検索失敗)</returns>
		/// @endif
		/// @if LANG_EN
		/// <summary>Searches for a program</summary>
		/// <param name="key">Search key</param>
		/// <returns>Program (search fails when null)</returns>
		/// @endif
		public BasicProgram Find( string key ) {
			if ( key == null ) key = "" ;
			return !programs.ContainsKey( key ) ? null : programs[ key ] ;
		}

		/// @if LANG_JA
		/// <summary>共有パラメータ</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Shared parameters</summary>
		/// @endif
		public BasicParameters Parameters ;

		Dictionary<string, BasicProgram> programs ;
	}

} // namespace
