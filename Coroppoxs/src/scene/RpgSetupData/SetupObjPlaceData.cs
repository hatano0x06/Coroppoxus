/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.IO;
using Sce.PlayStation.Core;

namespace AppRpg {


///***************************************************************************
/// 配置物のセットアップ
///***************************************************************************
public static class SetupObjPlaceData
{
    private const string FILE_NAME = "/Application/res/data/ObjPlaceData.bin";
    private const string FILE_ENLIST_NAME = "/Application/res/data/SetupEnPlaceData.cs";

/// public メソッド
///---------------------------------------------------------------------------

    /// 配置情報を書き込む
    public static bool Save()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        // 既にファイルが存在する
        if( File.Exists( FILE_NAME ) ){
            File.Delete( FILE_NAME );
        }

        FileStream fs = new FileStream( FILE_NAME, FileMode.CreateNew );

        // ファイルを新規作成
        BinaryWriter w = new BinaryWriter( fs );

        Vector3        trgPos;

        /// プレイヤー情報の書き込み
        ///-----------------------------------------------------------
        trgPos = ctrlResMgr.CtrlPl.GetPos();
        w.Write( (float) trgPos.X );
        w.Write( (float) trgPos.Y );
        w.Write( (float) trgPos.Z );
        w.Write( ctrlResMgr.CtrlPl.GetRotY() );


        /// 敵情報の書き込み
        ///-----------------------------------------------------------
        int enNum = ctrlResMgr.CtrlHobit.GetEntryNum();
        w.Write( (int) enNum );

        for( int i=0; i<enNum; i++ ){
            w.Write( (int) ctrlResMgr.CtrlHobit.GetChTypeId(i) );

            trgPos = ctrlResMgr.CtrlHobit.GetPos( i );
            w.Write( (float) trgPos.X );
            w.Write( (float) trgPos.Y );
            w.Write( (float) trgPos.Z );
            w.Write( ctrlResMgr.CtrlHobit.GetRotY( i ) );
        }





        w.Close();
        fs.Close();

        saveEnList();

        return true;
    }


    /// 配置情報を読み込む
    public static bool Load()
    {
        // ファイルが存在しない場合は内部リストから生成する
 		if( File.Exists( FILE_NAME ) == false ){
            loadFirstSetup();
            return false;
        }

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        FileStream fs = new FileStream( FILE_NAME, FileMode.Open, FileAccess.Read );
			
		// ファイルを新規作成
        BinaryReader r = new BinaryReader( fs );

        Vector3        trgPos = new Vector3();
        Vector3        trgScale = new Vector3();
        Vector3        trgRot = new Vector3();
        int            trgType;

        /// プレイヤー情報の読み取り
        ///-----------------------------------------------------------
        trgPos.X    = r.ReadSingle();
        trgPos.Y    = r.ReadSingle();
        trgPos.Z    = r.ReadSingle();
        trgRot.Y    = r.ReadSingle();
//        ctrlResMgr.CtrlPl.SetPlace( trgRot.Y, trgPos );
//        ctrlResMgr.CtrlCam.SetCamRotY( trgRot.Y );

        	/// 敵情報の読み取り
        ///-----------------------------------------------------------
        int enNum = r.ReadInt32();

        for( int i=0; i<enNum; i++ ){
            trgType        = r.ReadInt32();
            trgPos.X    = r.ReadSingle();
            trgPos.Y    = r.ReadSingle();
            trgPos.Z    = r.ReadSingle();
            trgRot.Y    = r.ReadSingle();

            //ctrlResMgr.CtrlHobit.EntryAddEnemy( trgType, trgRot.Y, trgPos,(int)rand.Next(0,4) );
        }


        /// 備品情報の読み取り
        ///-----------------------------------------------------------
        r.ReadInt32();    /// バージョン
        int fixNum = r.ReadInt32();

        for( int i=0; i<fixNum; i++ ){
            trgType        = r.ReadInt32();
            trgPos.X    = r.ReadSingle();
            trgPos.Y    = r.ReadSingle();
            trgPos.Z    = r.ReadSingle();
            trgRot.X    = r.ReadSingle();
            trgRot.Y    = r.ReadSingle();
            trgRot.Z    = r.ReadSingle();
            trgScale.X    = r.ReadSingle();
            trgScale.Y    = r.ReadSingle();
            trgScale.Z    = r.ReadSingle();

 //           ctrlResMgr.CtrlFix.EntryAddFix( trgType, trgRot, trgScale, trgPos );
        }

        r.Close();
        fs.Close();

		trgPos.X    = -63.2f;
        trgPos.Y    = 18.0f;
        trgPos.Z    = 100.2f;
        trgRot.Y    = 265.0f;
        ctrlResMgr.CtrlPl.SetPlace( trgRot.Y, trgPos );
        ctrlResMgr.CtrlCam.SetCamRotY( trgRot.Y );
			
		ctrlResMgr.CtrlMo.EntryAddMonument(1,0,new Vector3(-117.4f,20.0f,106.0f));
		ctrlResMgr.CtrlHobit.EntryAddEnemy(StaticDataList.getRandom((int)Data.Tex2dResId.CharStart+1,(int)Data.Tex2dResId.NormalCharMax),new Vector3(-117.4f,20.0f,106.0f));
		ctrlResMgr.CtrlTo.EntryAddTower(StaticDataList.getRandom((int)Data.Tex2dResId.TowerStart+1,(int)Data.Tex2dResId.TowerMax),new Vector3(-120.4f,30.0f,103.0f));
		ctrlResMgr.CtrlTo.EntryAddTower(StaticDataList.getRandom((int)Data.Tex2dResId.TowerStart+1,(int)Data.Tex2dResId.TowerMax),new Vector3(-120.4f,30.0f,109.0f));
		ctrlResMgr.CtrlTo.EntryAddTower(StaticDataList.getRandom((int)Data.Tex2dResId.TowerStart+1,(int)Data.Tex2dResId.TowerMax),new Vector3(-114.4f,30.0f,109.0f));
		ctrlResMgr.CtrlTo.EntryAddTower(StaticDataList.getRandom((int)Data.Tex2dResId.TowerStart+1,(int)Data.Tex2dResId.TowerMax),new Vector3(-114.4f,30.0f,103.0f));
		ctrlResMgr.CtrlWall.EntryAddWall(StaticDataList.getRandom((int)Data.Tex2dResId.WallStart+1,(int)Data.Tex2dResId.WallMax),new Vector3(-120.4f,30.0f,103.0f),new Vector3(-120.4f,30.0f,109.0f));
		ctrlResMgr.CtrlWall.EntryAddWall(StaticDataList.getRandom((int)Data.Tex2dResId.WallStart+1,(int)Data.Tex2dResId.WallMax),new Vector3(-120.4f,30.0f,109.0f),new Vector3(-114.4f,30.0f,109.0f));
		ctrlResMgr.CtrlWall.EntryAddWall(StaticDataList.getRandom((int)Data.Tex2dResId.WallStart+1,(int)Data.Tex2dResId.WallMax),new Vector3(-114.4f,30.0f,109.0f),new Vector3(-114.4f,30.0f,103.0f));
		ctrlResMgr.CtrlWall.EntryAddWall(StaticDataList.getRandom((int)Data.Tex2dResId.WallStart+1,(int)Data.Tex2dResId.WallMax),new Vector3(-114.4f,30.0f,103.0f),new Vector3(-120.4f,30.0f,103.0f));

		ctrlResMgr.CtrlHouse.EntryAddHouse((int)Data.Tex2dResId.BossMonument,new Vector3((int)Data.SetupValue.EnemyMonumentPosX,20.0f,(int)Data.SetupValue.EnemyMonumentPosY));
		return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 配置情報を書き込む
    private static void saveEnList()
    {
        // 既にファイルが存在する
        if( File.Exists( FILE_ENLIST_NAME ) ){
            File.Delete( FILE_ENLIST_NAME );
        }

        StreamWriter sw = new StreamWriter(
                FILE_ENLIST_NAME,
                false,
                System.Text.Encoding.GetEncoding( "utf-8" ) );


        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        int enNum = ctrlResMgr.CtrlHobit.GetEntryNum();

/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
        sw.Write("using System;\n\n");
        sw.Write("namespace AppRpg {\n\n");
        sw.Write("///***************************************************************************\n");
        sw.Write("/// 敵配置リスト\n");
        sw.Write("///***************************************************************************\n");
        sw.Write("public class SetupEnPlaceData\n{\n\n");

        sw.Write("\tpublic static int Version = 1;\n\n");
        sw.Write("\tpublic float[,] PlaceData = {\n");

        for( int i=0; i<enNum; i++ ){
            Vector3 trgPos = ctrlResMgr.CtrlHobit.GetPos( i );
            sw.Write("\t\t{ "+ctrlResMgr.CtrlHobit.GetChTypeId(i)+", "+trgPos.X+"f, "+trgPos.Y+"f, "+trgPos.Z+"f, "+ctrlResMgr.CtrlHobit.GetRotY( i )+"f },\n");
        }
        sw.Write("\t};\n\n");
        sw.Write("}\n} // namespace\n");

        sw.Close();
    }



    /// 配置情報を書き込む
    private static void loadFirstSetup()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
			
        ctrlResMgr.CtrlPl.SetPlace( 0.0f, new Vector3( -10.0f, 15.0f, 0.0f) );
        int            trgType;
        Vector3        trgPos = new Vector3();
        Vector3        trgRot = new Vector3();


        /// 敵情報の読み取り
        ///-----------------------------------------------------------
        if( SetupEnPlaceData.Version > 0 ){
            SetupEnPlaceData data = new SetupEnPlaceData();
            int enNum = data.PlaceData.GetLength(0);
            for( int i=0; i<enNum; i++ ){
                trgType        = (int)data.PlaceData[i,0];
                trgPos.X    = data.PlaceData[i,1];
                trgPos.Y    = data.PlaceData[i,2];
                trgPos.Z    = data.PlaceData[i,3];
                trgRot.Y    = data.PlaceData[i,4];
                //ctrlResMgr.CtrlHobit.EntryAddEnemy( trgType, trgRot.Y, trgPos,(int)rand.Next(0,4) );
            }
        }    

            
    }

}

} // namespace
