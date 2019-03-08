/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using DemoModel;
using Sce.PlayStation.Core.Graphics;
	
namespace DefenseDemo {

/// リソースデータクラス
/**
 * @version 0.1, 2011/06/23
 */
public class ResourceDataContainer {

	/// リソースデータクラス
	private static ResourceDataContainer resourceDataContainer;
	/// 3Dデータ読込み用クラス
	public ModelContainer modelContainer;
	/// 2Dデータ読込み用クラス
	public TexContainer texContainer;
	/// シェーダ読込み用クラス
	public ShaderContainer shaderContainer;

	/// コンストラクタ
	/**
	 */
	public ResourceDataContainer()
	{
		modelContainer = new ModelContainer();
		texContainer = new TexContainer();
		shaderContainer = new ShaderContainer();
		
		modelContainer.SetResPath( "" );
		texContainer.SetResPath( "/Application/res/3d/" );
	}

	/// デストラクタ
	/**
	 */
	~ResourceDataContainer()
	{
		modelContainer = null;
		texContainer = null;
	}

	/// インスタンスを取得する。
	/**
	 * インスタンスを生成していなければ生成する。
	 */
	public static ResourceDataContainer Inst()
	{
		if( resourceDataContainer == null ) {
			return resourceDataContainer = new ResourceDataContainer();
		} else {
			return resourceDataContainer;
		}
	}

	/// 初期化
	/**
	 */
	public void Init()
	{
	}

	/// 解放
	/**
	 */
	public void Term()
	{
	}

	/// 3Dデータの読み込み
	/**
	 * モデルデータ(.mdx)を読み込む。
	 * @param [in] tag : インデックスタグ
	 * @param [in] filename : 読み込みファイル名
	 */
	public void Load3D( string tag, string filename )
	{
		modelContainer.Load( tag, filename );
	}

	/// 3Dデータの解放
	/**
	 * 読み込んだモデルデータ(.mdx)を解放する。
	 * @param [in] tag : インデックスタグ
	 */
	public void Release3D( string tag )
	{
	}

	/// 全ての3Dデータの解放
	/**
	 */
	public void Release3DAll()
	{
	}

	/// 3Dテクスチャデータの読み込み
	/**
	 * モデルに貼り付けるテクスチャデータ(.png)を読み込む。
	 * @param [in] tag : インデックスタグ
	 * @param [in] filename : 読み込みファイル名
	 */

	/// 2Dデータの読み込み
	/**
	 * 2Dデータ(.png)を読み込む
	 * @param [in] tag : インデックスタグ
	 * @param [in] filename : 読み込みファイル名
	 */
	public void Load2D( string tag, string filename )
	{
		texContainer.Load( tag, filename );
	}

	/// 2Dデータの解放
	/**
	 * @param [in] tag : インデックスタグ
	 */
	public void Release2D( string tag )
	{
	}

	/// 全ての2Dデータの解放
	/**
	 */
	public void Release2DAll()
	{
	}

	/// 指定したタグの3DObjectを取得
	/**
	 * @return Object3D : 3Dデータ情報
	 */
	public Object3D GetObject3D( string tag )
	{
		Object3D obj = new Object3D();
		obj.model = modelContainer.Find( tag );
		return obj;
	}

	/// 指定した3DObjectを解放する
	/**
	 * @param [out] obj 3Dデータ情報
	 */
	public void LostObject3D( string tag, Object3D obj )
	{
		obj = null;
	}

	/// 指定したタグのTexture2Dを取得
	/**
	 * 指定したtagのTexture2Dのポインタを取得
	 * 対象Texture2Dへの参照カウントを加算
	 * @return Object3DTexture : 3DTextureデータ情報
	 */
	public Texture2D GetObject3DTexture( string tag )
	{
		Texture2D texture2d = texContainer.Find( tag );
		return texture2d;
	}

	/// 指定したTexture2Dへの参照を解放する
	/**
	 * 対象Texture2Dへの参照カウントを減算
	 */
	public void LostObject3DTexture( string tag )
	{
	}

	/// 指定したタグの2DObjectを取得
	/**
	 * 指定したtagのTexture2Dから指定範囲のSpriteを作成
	 * 対象Texture2Dへの参照カウントを加算
	 * @return Object2D : 2Dデータ情報
	 */
	public Object2D GetObject2D( string tag, int x, int y, int w, int h )
	{
		return new Object2D();
	}

	/// 指定した2DObjectを解放する
	/**
	 * 対象Texture2Dへの参照カウントを減算
	 * @param [out] obj 2Dデータ情報
	 */
	public void LostObject2D( string tag, Object2D obj )
	{
		obj = null;
	}

}

} // end ns DefenseDemo
