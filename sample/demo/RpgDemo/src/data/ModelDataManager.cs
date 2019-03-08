/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg { namespace Data {

///***************************************************************************
/// モデルデータのコンテナ
///***************************************************************************
public class ModelDataManager
{
    private DemoModel.BasicModel[]          modelTbl;
    private DemoModel.TexContainer[]        textureCnrTbl;
    private DemoModel.ShaderContainer[]     shaderCnrTbl;

    private int                             mdlMax;
    private int                             texContMax;
    private int                             shaderContMax;


    /// インスタンスの取得
    private static ModelDataManager instance = new ModelDataManager();

    public static ModelDataManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( int mdlMax, int texCnrMax, int shaderCnrMax )
    {
        modelTbl        = new DemoModel.BasicModel[mdlMax];
        for( int i=0; i<mdlMax; i++ ){
            modelTbl[i]            = null;
        }

        textureCnrTbl    = new DemoModel.TexContainer[texCnrMax];
        for( int i=0; i<texCnrMax; i++ ){
            textureCnrTbl[i]    = new DemoModel.TexContainer();
        }

        shaderCnrTbl    = new DemoModel.ShaderContainer[shaderCnrMax];
        for( int i=0; i<shaderCnrMax; i++ ){
            shaderCnrTbl[i]        = new DemoModel.ShaderContainer();
            shaderSetUp( i );
        }

        this.mdlMax            = mdlMax;
        this.texContMax        = texCnrMax;
        this.shaderContMax     = shaderCnrMax;
        return true;
    }

    /// 破棄
    public void Term()
    {
        if( modelTbl != null ){
            for( int i=0; i<mdlMax; i++ ){
                modelTbl[i]            = null;
            }
        }
        if( textureCnrTbl != null ){
            for( int i=0; i<texContMax; i++ ){
                textureCnrTbl[i]    = null;
            }
        }
        if( shaderCnrTbl != null ){
            for( int i=0; i<shaderContMax; i++ ){
                shaderCnrTbl[i]        = null;
            }
        }
        modelTbl        = null;
        textureCnrTbl   = null;
        shaderCnrTbl    = null;
    }


    /// モデルデータの読み込み
    public bool LoadModel( int idx, String filename )
    {
        modelTbl[idx] = null;
        modelTbl[idx] = new DemoModel.BasicModel( filename, 0 );
        return true;
    }

    /// データの取得
    public DemoModel.BasicModel GetModel( int idx )
    {
        return modelTbl[idx];
    }

    /// テクスチャデータの読み込み
    public bool LoadTexture( int idx, String key, String filename )
    {
        textureCnrTbl[idx].Load( key, filename );
        return true;
    }

    /// テクスチャコンテナの取得
    public DemoModel.TexContainer GetTextureContainer( int idx )
    {
        return textureCnrTbl[idx];
    }

    /// シェーダデータの読み込み
    public bool LoadShader( int idx, String key, String vsfilename, String fsfilename )
    {
        shaderCnrTbl[idx].Load( key, vsfilename, fsfilename );
        return true;
    }

    /// シェーダコンテナの取得
    public DemoModel.ShaderContainer GetShaderContainer( int idx )
    {
        return shaderCnrTbl[idx];
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 基本シェーダを読み込む
    public void shaderSetUp( int idx )
    {
        shaderCnrTbl[idx].LoadBasicProgram();
    }


/// プロパティ
///---------------------------------------------------------------------------

}

}} // namespace
