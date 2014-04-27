/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Tutorial.Utility;


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

	public Dictionary<string, UnifiedTextureInfo> dicTextureInfo;	


	SetupModelDataList            dataList = new SetupModelDataList();


    /// インスタンスの取得
    private static ModelDataManager instance = new ModelDataManager();

    public static ModelDataManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( int mdlMax, int texCnrMax, int shaderCnrMax)
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
				
		dicTextureInfo = UnifiedTexture.GetDictionaryTextureInfo("/Application/res/data/2Dtex/unifiedtexture.xml");
		//dicTextureInfo["wall.png"];		
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
				
		dicTextureInfo.Clear();
		dicTextureInfo = null;
	
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

    /// シェーダデータの読み込み
    public bool Load2dTexture()
    {
		/*
		texNoumin1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumin1,0],false);			//1
		texNoumin2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumin2,0],false);			//1
		texSenshi1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Senshi1,0],false);			//2
		texSenshi2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Senshi2,0],false);			//2
		texSouryo1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Souryo1,0],false);			//3
		texSouryo2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Souryo2,0],false);			//3
		texZonbi1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Zonbi1,0],false);				//4
		texZonbi2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Zonbi2,0],false);				//4
		texNecromancer1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Necromancer1,0],false);	//5
		texNecromancer2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Necromancer2,0],false);	//5
		
		texMonument  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Monument,0],false);			//6
		texNouminHouse1  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse1,0],false);	//7
		texNouminHouse2  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse2,0],false);	//8
		texNouminHouse3  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse3,0],false);	//9
		texNouminHouse4  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse4,0],false);	//10
		texNouminHouse5  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse5,0],false);	//11
		texSenshiHouse1  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse1,0],false);	//12
		texSenshiHouse2  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse2,0],false);	//13
		texSenshiHouse3  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse3,0],false);	//14
		texSenshiHouse4  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse4,0],false);	//15
		texSenshiHouse5  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse5,0],false);	//16
		texSouryoHouse1  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse1,0],false);	//17
		texSouryoHouse2  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse2,0],false);	//18
		texSouryoHouse3  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse3,0],false);	//19
		texSouryoHouse4  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse4,0],false);	//20
		texSouryoHouse5  = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse5,0],false);	//21
				
		texNoumintower1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumintower1,0],false);	//22
		texSenshiTower1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiTower1,0],false);	//23
		texSenshiTower2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiTower2,0],false);	//24
		texSouryoTower1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoTower1,0],false);	//25
		texSouryoTower2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoTower1,0],false);	//26
				
		texGareki = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Gareki,0],false);				//27
		texGarekiWall = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.GarekiWall,0],false);		//28
		texWall = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Wall,0],false);					//29
		texMakingWall1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall1,0],false);		//30
		texMakingWall2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall2,0],false);		//31
		texMakingWall3 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall3,0],false);		//32
				
		texFukidasi1 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon1,0],false);		//33
		texFukidasi2 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon2,0],false);		//34
		texFukidasi3 = new Texture2D("/Application/res/data/2Dtex/" + dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon3,0],false);		//34
		*/
		return true;
    }

	public UnifiedTextureInfo SetTexture2(int idx)
    {
		switch((Data.Tex2dResId)idx){
			case Data.Tex2dResId.Noumin1:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumin1]];			//1
			case Data.Tex2dResId.Senshi1:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Senshi1]];			//2
			case Data.Tex2dResId.Souryo1:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Souryo1]];			//3
			case Data.Tex2dResId.Zonbi1:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Zonbi1]];			//4					
			case Data.Tex2dResId.Necromancer1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Necromancer1]];		//5

			case Data.Tex2dResId.Monument:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Monument]];			//1
			case Data.Tex2dResId.NouminHouse1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse1]];		//2
			case Data.Tex2dResId.NouminHouse2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse2]];		//3
			case Data.Tex2dResId.NouminHouse3:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse3]];		//4
			case Data.Tex2dResId.NouminHouse4:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse4]];		//5
			case Data.Tex2dResId.NouminHouse5:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.NouminHouse5]];		//6
			case Data.Tex2dResId.SenshiHouse1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse1]];		//7
			case Data.Tex2dResId.SenshiHouse2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse2]];		//8
			case Data.Tex2dResId.SenshiHouse3:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse3]];		//9
			case Data.Tex2dResId.SenshiHouse4:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse4]];		//10
			case Data.Tex2dResId.SenshiHouse5:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiHouse5]];		//11
			case Data.Tex2dResId.SouryoHouse1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse1]];		//12
			case Data.Tex2dResId.SouryoHouse2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse2]];		//13
			case Data.Tex2dResId.SouryoHouse3:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse3]];		//14
			case Data.Tex2dResId.SouryoHouse4:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse4]];		//15
			case Data.Tex2dResId.SouryoHouse5:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoHouse5]];		//16
					
			case Data.Tex2dResId.Noumintower1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumintower1]];		//1
			case Data.Tex2dResId.SenshiTower1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiTower1]];		//2
			case Data.Tex2dResId.SenshiTower2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SenshiTower2]];		//3
			case Data.Tex2dResId.SouryoTower1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoTower1]];		//4
			case Data.Tex2dResId.SouryoTower2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SouryoTower2]];		//5

			case Data.Tex2dResId.Wall1:				return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Wall1]];				//1
			case Data.Tex2dResId.Wall2:				return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Wall2]];				//2
			case Data.Tex2dResId.Wall3:				return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Wall3]];				//3
					
			case Data.Tex2dResId.Gareki:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Gareki]];			//1
			case Data.Tex2dResId.GarekiWall:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.GarekiWall]];		//2
			case Data.Tex2dResId.MakingWall1:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall1]];		//3
			case Data.Tex2dResId.MakingWall2:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall2]];		//4
			case Data.Tex2dResId.MakingWall3:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.MakingWall3]];		//5
			case Data.Tex2dResId.SpeechBalloon1:	return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon1]];	//6
			case Data.Tex2dResId.SpeechBalloon2:	return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon2]];	//7
			case Data.Tex2dResId.SpeechBalloon3:	return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.SpeechBalloon3]];	//8
			case Data.Tex2dResId.Uppertooth:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Uppertooth]];		//9
			case Data.Tex2dResId.Undertooth:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Undertooth]];		//10
			case Data.Tex2dResId.Life:				return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Life]];				//11
					
			case Data.Tex2dResId.BossMonument:		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.BossMonument]];		//1
			case Data.Tex2dResId.BossWall:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.BossWall]];			//2
			case Data.Tex2dResId.Bosstower:			return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Bosstower]];		//3
					
		}
				
		return dicTextureInfo[dataList.Tex2dFileNameList[(int)Data.Tex2dResId.Noumin1]];
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
