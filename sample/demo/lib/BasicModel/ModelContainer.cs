//#define SHARED_RESOURCES


using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace DemoModel
{

/// テクスチャ格納用コンテナ
public class ModelContainer
{
    string resPath = "/Application/res/data";
#if SHARED_RESOURCES
	static Dictionary< string, BasicModel > modelTable = new Dictionary< string, BasicModel >();
#else
	Dictionary< string, BasicModel > modelTable = new Dictionary< string, BasicModel >();
#endif

    /// コンストラクタ
    public ModelContainer()
    {
    }

    public void Dispose()
    {
#if SHARED_RESOURCES
		// don't need to release
#else
        foreach( var entry in modelTable ){
            entry.Value.Dispose();
        }
        modelTable.Clear();
#endif
    }

    public void SetResPath( string path )
    {
        resPath = path;
    }

    /// コンテナに Model の読み込みを行う
    /**
     * すでに読み込み済みのモデルが存在する場合には、読み込み済みのモデルを返す
     */
    public BasicModel Load( string key, string filename )
    {
        BasicModel ret = this.Find( key );
        if( ret == null ){
            ret = this.Regist( key, new BasicModel( resPath + filename, 0 ) );
        }
        return ret;
    }

    /// Model の検索
    public BasicModel Find( string key )
    {
		if( modelTable.ContainsKey( key ) ){
        	return modelTable[ key ] as BasicModel;
		}
		return null;
    }

    /// Model の登録
    /**
     * @return : 登録に失敗した場合、null が返る
     */
    public BasicModel Regist( string key, BasicModel model )
    {
        if( Find( key ) != null ){
            return null;
        }
        modelTable[ key ] = model;
        return model;
    }

    public bool BindTextures( TexContainer texContainer )
    {
        foreach( var entry in modelTable ){
            BasicModel model = entry.Value as BasicModel;
            model.BindTextures( texContainer );
        }
        return true;
    }

    /// 有効なライト数の設定
    public void SetLightCount( int count )
    {
        foreach( var entry in modelTable ){
            BasicModel model = entry.Value as BasicModel;
            model.SetLightCount( count );
        }
    }

    /// ライトの設定
    public bool SetLight( int idx, Light light )
    {
        foreach( var entry in modelTable ){
            BasicModel model = entry.Value as BasicModel;
            model.SetLight( idx, light );
        }

        return false;
    }

}

} // end ns SceneScript