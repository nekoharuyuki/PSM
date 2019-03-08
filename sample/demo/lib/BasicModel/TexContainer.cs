//#define SHARED_RESOURCES


using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace DemoModel
{

/// テクスチャ格納用コンテナ
public class TexContainer
{
    string resPath = "/Application/res/data";
    
#if SHARED_RESOURCES
    static Dictionary< string, Texture2D > texTable = new Dictionary< string, Texture2D>();
#else
    Dictionary< string, Texture2D > texTable = new Dictionary< string, Texture2D>();
#endif

    /// コンストラクタ
    public TexContainer()
    {
    }

    public void Dispose()
    {
#if SHARED_RESOURCES
		// no need to dispose
#else
        foreach( var entry in texTable ){
            entry.Value.Dispose();
        }
        texTable.Clear();
#endif
    }
    
    public void SetResPath( string path )
    {
        resPath = path;
    }

    /// コンテナに Texture2D の読み込みを行う
    /**
     * すでに読み込み済みのテクスチャが存在する場合には、読み込み済みのテクスチャを返す
     */
    public Texture2D Load( string key, string filename )
    {
        Texture2D ret = this.Find( key );
        if( ret == null ){
            ret = this.Regist( key, new Texture2D( resPath + filename, false ) );
        }
        return ret;
    }

    /// Texture2D の検索
    public Texture2D Find( string key )
    {
        if( texTable.ContainsKey( key ) ){
            return texTable[ key ];
        }
        return null;
    }

    /// Texture2D の登録
    /**
     * @return : 登録に失敗した場合、null が返る
     */
    public Texture2D Regist( string key, Texture2D tex )
    {
        if( Find( key ) != null ){
            return null;
        }
        texTable[ key ] = tex;
        return tex;
    }
}

} // end ns SceneScript
