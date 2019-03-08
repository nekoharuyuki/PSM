/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg {


///***************************************************************************
/// モデルデータのセットアップ
///***************************************************************************
public class SetupModelData
{
    SetupModelDataList            dataList = new SetupModelDataList();


/// public メソッド
///---------------------------------------------------------------------------

    /// キャラモデルデータの読み込み
    public bool SetCharData()
    {
        int mdlResId, mdlTexId;

        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// 英雄
        mdlResId = (int)Data.ModelResId.Hero;
        mdlTexId = (int)Data.ModelTexResId.Hero;
        resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/char/"+dataList.MdlFileNameList[mdlResId] );
        for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
            if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                resMgr.LoadTexture( mdlTexId,
                                    dataList.TexFileNameList[mdlTexId,i],
                                    "/3D/char/" + dataList.TexFileNameList[mdlTexId,i] );
            }
        }

        /// 敵Ａ
        mdlResId = (int)Data.ModelResId.MonsterA;
        mdlTexId = (int)Data.ModelTexResId.MonsterA;
        resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/monster/"+dataList.MdlFileNameList[mdlResId] );
        for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
            if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                resMgr.LoadTexture( mdlTexId,
                                    dataList.TexFileNameList[mdlTexId,i],
                                    "/3D/monster/" + dataList.TexFileNameList[mdlTexId,i] );
            }
        }

        /// 敵Ｂ
        mdlResId = (int)Data.ModelResId.MonsterB;
        mdlTexId = (int)Data.ModelTexResId.MonsterB;
        resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/monster/"+dataList.MdlFileNameList[mdlResId] );
        for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
            if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                resMgr.LoadTexture( mdlTexId,
                                    dataList.TexFileNameList[mdlTexId,i],
                                    "/3D/monster/" + dataList.TexFileNameList[mdlTexId,i] );
            }
        }

        /// 敵Ｃ
        mdlResId = (int)Data.ModelResId.MonsterC;
        mdlTexId = (int)Data.ModelTexResId.MonsterC;
        resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/monster/"+dataList.MdlFileNameList[mdlResId] );
        for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
            if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                resMgr.LoadTexture( mdlTexId,
                                    dataList.TexFileNameList[mdlTexId,i],
                                    "/3D/monster/" + dataList.TexFileNameList[mdlTexId,i] );
            }
        }

        return true;
    }


    /// 装備品モデルデータの読み込み
    public bool SetEquipData()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// 武器モデルデータ
        for( int id=0; id<(int)Data.EquipTypeId.Max; id++ ){
            int mdlResId = (int)Data.ModelResId.Sword + id;

            if( dataList.MdlFileNameList[mdlResId] != "" ){
                resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/weapon/"+dataList.MdlFileNameList[mdlResId] );
            }
        }

        /// 武器テクスチャデータ
        for( int id=0; id<(int)Data.EquipTypeId.Max; id++ ){
            int mdlTexId = (int)Data.ModelTexResId.Sword + id;

            for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
                if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                    resMgr.LoadTexture( mdlTexId,
                                        dataList.TexFileNameList[mdlTexId,i],
                                        "/3D/weapon/" + dataList.TexFileNameList[mdlTexId,i] );
                }
            }
        }

        return true;
    }


    /// ステージモデルデータの読み込み
    public bool SetStgData()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// ステージモデルデータ
        for( int id=0; id<(int)Data.StageTypeId.Max; id++ ){
            int mdlResId = (int)Data.ModelResId.Stage + id;

            if( dataList.MdlFileNameList[mdlResId] != "" ){
                resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/field/"+dataList.MdlFileNameList[mdlResId] );
            }
        }

        /// ステージテクスチャデータ
        for( int id=0; id<(int)Data.StageTypeId.Max; id++ ){
            int mdlTexId = (int)Data.ModelTexResId.Stage + id;

            for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
                if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                    resMgr.LoadTexture( mdlTexId,
                                        dataList.TexFileNameList[mdlTexId,i],
                                        "/3D/field/" + dataList.TexFileNameList[mdlTexId,i] );
                }
            }
        }

        return true;
    }


    /// エフェクトモデルデータの読み込み
    public bool SetEffData()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// エフェクトモデルデータ
        for( int id=0; id<(int)Data.EffTypeId.Max; id++ ){
            int mdlResId = (int)Data.ModelResId.Eff00 + id;

            if( dataList.MdlFileNameList[mdlResId] != "" ){
                resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/effect/"+dataList.MdlFileNameList[mdlResId] );
            }
        }

        /// エフェクトテクスチャデータ
        for( int id=0; id<(int)Data.ModelEffTexId.Max; id++ ){
            int mdlTexId = (int)Data.ModelTexResId.EffA + id;

            for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
                if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                    resMgr.LoadTexture( mdlTexId,
                                        dataList.TexFileNameList[mdlTexId,i],
                                        "/3D/effect/" + dataList.TexFileNameList[mdlTexId,i] );
                }
            }
        }

        return true;
    }


    /// 備品モデルデータの読み込み
    public bool SetFixData()
    {
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// 備品モデルデータ
        for( int mdlResId=(int)Data.ModelResId.Fix00; mdlResId<(int)Data.ModelResId.Max; mdlResId++ ){

            if( dataList.MdlFileNameList[mdlResId] != "" ){
                resMgr.LoadModel( mdlResId,    "/Application/res/data/3D/field/"+dataList.MdlFileNameList[mdlResId] );
            }
        }

        /// 備品テクスチャデータ
        for( int id=0; id<(int)Data.FixTypeId.Max; id++ ){
            int mdlTexId = (int)Data.ModelTexResId.Fix00 + id;

            for( int i=0; i<dataList.TexFileNameList.GetLength(1); i++ ){
                if( dataList.TexFileNameList[mdlTexId,i] != "" ){
                    resMgr.LoadTexture( mdlTexId,
                                        dataList.TexFileNameList[mdlTexId,i],
                                        "/3D/field/" + dataList.TexFileNameList[mdlTexId,i] );
                }
            }
        }

        return true;
    }
}

} // namespace
