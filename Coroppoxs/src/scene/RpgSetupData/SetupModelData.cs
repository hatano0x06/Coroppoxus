/* PlayStation(R)Mobile SDK 1.11.01
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
        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();

        /// 英雄
        /*
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
        */

		resMgr.Load2dTexture();
        return true;
    }


    /// 装備品モデルデータの読み込み
    public bool SetEquipData()
    {
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
