
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;

namespace DemoModel{

public class ShaderContainer
{
    Dictionary< string, BasicProgram > shaderTable = new Dictionary< string, BasicProgram>();
    string shaderRoot = "/Application/shaders/cbasic/";

    /// コンストラクタ
    public ShaderContainer()
    {
        Load( "ASSERT", shaderRoot + "ASSERT.cgx", null );

    }

    public void Dispose()
    {
        foreach( var entry in shaderTable ){
            entry.Value.Dispose();
        }
        shaderTable.Clear();
    }

    /// コンテナに BasicProgram の読み込みを行う
    public BasicProgram Load( string key, string vsfilename, string fsfilename )
    {
        BasicProgram ret = this.Find( key );
        if( ret == null ){
            ret = this.Regist( key, new BasicProgram( vsfilename, fsfilename ) );
        }
        return ret;
    }

    /// BasicProgram の検索
    public BasicProgram Find( string key )
    {
        if( shaderTable.ContainsKey( key ) ){
            return shaderTable[ key ];
        }
        return null;
    }

    /// BasicProgram の登録
    public BasicProgram Regist( string key, BasicProgram shader )
    {
        if( Find( key ) != null ){
            return null;
        }
        shaderTable[ key ] = shader;

        return shader;
    }

    public BasicProgram Find( BasicShaderName name )
    {
        string key = name.Key();
        
        BasicProgram ret = Find( key );
        if( ret == null ){
            string nameVS = name.VSName();
            string nameFS = name.FSName();
            try{
                ret = Load( key, shaderRoot +  nameVS + ".vp.cgx", shaderRoot + nameFS + ".fp.cgx" );
                // 開発用シェーダ
                // ret = Load( key, shaderRoot +  "_testVSH.vp.cgx", shaderRoot + "_testFSH.fp.cgx" );
            }catch( System.IO.FileLoadException ){
                ret = Find( "ASSERT" );
            }
        }
        return ret;
    }

    public bool LoadBasicProgram()
    {
        return true;
    }

    
}

} // end ns SceneScript