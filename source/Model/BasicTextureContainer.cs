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
	//  BasicTextureContainer
	//------------------------------------------------------------

	/// @if LANG_JA
	/// <summary>テクスチャコンテナを表すクラス</summary>
	/// @endif
	/// @if LANG_EN
	/// <summary>Class representing a texture container</summary>
	/// @endif
	public class BasicTextureContainer : IDisposable {

		/// @if LANG_JA
		/// <summary>テクスチャコンテナを作成する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Creates a texture container</summary>
		/// @endif
		public BasicTextureContainer() {
			textures = new Dictionary<string, Texture>() ;
		}
		/// @if LANG_JA
		/// <summary>テクスチャコンテナのアンマネージドリソースを解放する</summary>
		/// @endif
		/// @if LANG_EN
		/// <summary>Frees the unmanaged resources of the texture container</summary>
		/// @endif
		public void Dispose() {
			Dispose( true ) ;
		}
		protected virtual void Dispose( bool disposing ) {
			if ( disposing ) {
				if ( textures != null ) {
					foreach ( var pair in textures ) pair.Value.Dispose() ;
				}
				textures = null ;
			}
		}

		/// @if LANG_JA
		/// <summary>テクスチャを登録する</summary>
		/// <param name="key">検索キー</param>
		/// <param name="texture">テクスチャ</param>
		/// @endif
		/// @if LANG_EN
		/// <summary>Registers a texture</summary>
		/// <param name="key">Search key</param>
		/// <param name="texture">Textures</param>
		/// @endif
		public void Add( string key, Texture texture ) {
			if ( key == null ) key = "" ;
			textures[ key ] = texture ;
		}
		/// @if LANG_JA
		/// <summary>テクスチャを検索する</summary>
		/// <param name="key">検索キー</param>
		/// <returns>テクスチャ (nullならば検索失敗)</returns>
		/// @endif
		/// @if LANG_EN
		/// <summary>Searches for a texture</summary>
		/// <param name="key">Search key</param>
		/// <returns>Texture (search fails when null)</returns>
		/// @endif
		public Texture Find( string key ) {
			if ( key == null ) key = "" ;
			return !textures.ContainsKey( key ) ? null : textures[ key ] ;
		}

		Dictionary<string, Texture> textures ;
	}

} // namespace
