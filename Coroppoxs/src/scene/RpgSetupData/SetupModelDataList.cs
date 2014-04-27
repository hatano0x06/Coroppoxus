/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg {


///***************************************************************************
/// モデルデータリスト
///***************************************************************************
public class SetupModelDataList
{
    /// モデルデータファイル名一覧
    ///-----------------------------------------------
    public string[] MdlFileNameList = {
        "char00.mdx",
        "mons00.mdx",
        "mons01.mdx",
        "mons02.mdx",
        "ground00.mdx",
        "sky00.mdx",
        "wep00.mdx",
        "eff00.mdx",
        "eff01.mdx",
        "eff02.mdx",
        "eff03.mdx",
        "eff04.mdx",
        "eff05.mdx",
        "eff06.mdx",
        "eff07.mdx",
        "eff08.mdx",
        "eff09.mdx",
        "eff10.mdx",
        "eff11.mdx",
        "eff12.mdx",
        "eff13.mdx",
        "eff14.mdx",
        "obj00.mdx",
        "obj01.mdx",
        "obj02.mdx",
        "obj03.mdx",
        "obj04.mdx",
        "obj05.mdx",
        "obj06.mdx",
        "obj07.mdx",
        "obj08.mdx",
        "obj09.mdx",
        "obj10.mdx",
        "obj11.mdx",
        "obj12.mdx",
        "obj13.mdx",
        "obj14.mdx",
        "obj15.mdx",
        "obj16.mdx",
        "obj17.mdx",
        "obj18.mdx",
        "obj19.mdx",
        "obj01_l1.mdx",
        "obj02_l1.mdx",
        "obj03_l1.mdx",
        "obj04_l1.mdx",
        "obj05_l1.mdx",
        "obj06_l1.mdx",
        "obj07_l1.mdx",
        "obj08_l1.mdx",
        "obj09_l1.mdx",
        "obj10_l1.mdx",
        "obj11_l1.mdx",
        "obj12_l1.mdx",
        "obj13_l1.mdx",
        "obj13_l2.mdx",
        "obj14_l1.mdx",
        "obj14_l2.mdx",
        "obj15_l1.mdx",
        "obj16_l1.mdx",
        "obj17_l1.mdx",
        "obj18_l1.mdx",
        "obj19_l1.mdx",
    };


    /// テクスチャデータファイル名一覧
    ///-----------------------------------------------
    public string[,] TexFileNameList = {
        { "char00_00.png","char00_01.png","char00_02.png","","","","","","","" },
        { "mons00_00.png","","","","","","","","","" },
        { "mons01_00.png","","","","","","","","","" },
        { "mons02_00.png","","","","","","","","","" },
        { "ground00_00.png","ground00_01.png","ground00_02.png","ground00_03.png","ground00_04.png","ground00_05.png","ground00_06.png","ground00_07.png","ground00_08.png", "ground00_09.png" },
        { "sky00_00.png","sky00_01.png","sky00_02.png","sky00_03.png","sky00_04.png","sky00_05.png","sky00_06.png","","","" },
        { "wep00_00.png","","","","","","","","","" },
        { "eff_00.png","","","","","","","","","" },
        { "eff_01.png","","","","","","","","","" },
        { "eff_02.png","","","","","","","","","" },
        { "eff_03.png","","","","","","","","","" },
        { "eff_04.png","","","","","","","","","" },
        { "eff_05.png","","","","","","","","","" },
        { "eff_06.png","","","","","","","","","" },
        { "eff_07.png","","","","","","","","","" },
        { "eff_08.png","","","","","","","","","" },
        { "obj00_01.png","","","","","","","","","" },
        { "obj01_01.png","","","","","","","","","" },
        { "obj02_01.png","","","","","","","","","" },
        { "obj03_00.png","","","","","","","","","" },
        { "obj04_01.png","","","","","","","","","" },
        { "obj05_00.png","","","","","","","","","" },
        { "obj06_00.png","","","","","","","","","" },
        { "obj07_00.png","","","","","","","","","" },
        { "obj08_00.png","","","","","","","","","" },
        { "obj09_01.png","","","","","","","","","" },
        { "obj10_01.png","obj10_l1_00.png","","","","","","","","" },
        { "obj11_01.png","","","","","","","","","" },
        { "obj12_01.png","","","","","","","","","" },
        { "obj13_00.png","obj13_l2_00.png","","","","","","","","" },
        { "obj14_00.png","obj14_l2_00.png","","","","","","","","" },
        { "obj15_00.png","","","","","","","","","" },
        { "obj16_00.png","","","","","","","","","" },
        { "obj17_00.png","","","","","","","","","" },
        { "obj18_00.png","","","","","","","","","" },
        { "obj19_00.png","","","","","","","","","" },
    };
		
		/*
         "NouminHouse1.png",	//2
         "NouminHouse2.png",	//3
         "NouminHouse3.png",	//4
         "NouminHouse4.png",	//5
         "NouminHouse5.png",	//6
         "SenshiHouse1.png",	//7
         "SenshiHouse2.png",	//8
         "SenshiHouse3.png",	//9
         "SenshiHouse4.png",	//10
         "SenshiHouse5.png",	//11
         "SouryoHouse1.png",	//12
         "SouryoHouse2.png",	//13
         "SouryoHouse3.png",	//14
         "SouryoHouse4.png",	//15
         "SouryoHouse5.png",	//16	
		*/		
		
    public string[] Tex2dFileNameList = {
         "",
         "Noumin.png",		//1
		 "Senshi.png",		//2
		 "Souryo.png",		//3
		 "",				//
		 "Zonbi.png",		//4
		 "Necromancer.png",	//5
		 "",				//
         "",				//
         "NouminHouse2.png",	//1
         "NouminHouse1.png",	//2
         "NouminHouse1.png",	//3
         "NouminHouse1.png",	//4
         "NouminHouse1.png",	//5
         "NouminHouse1.png",	//6
         "SenshiHouse1.png",	//7
         "SenshiHouse2.png",	//8
         "SenshiHouse1.png",	//9
         "SenshiHouse2.png",	//10
         "SenshiHouse1.png",	//11
         "SouryoHouse1.png",	//12
         "SouryoHouse1.png",	//13
         "SouryoHouse1.png",	//14
         "SouryoHouse1.png",	//15
         "SouryoHouse1.png",	//16	
         "",	//
         "",	//
         "NouminTower1.png",	//1
         "SenshiTower1.png",	//2
         "SenshiTower2.png",	//3
         "SouryoTower1.png",	//4
         "SouryoTower2.png",	//5
         "",	//
         "",	// 
         "wall.png",	//1
         "wall.png",	//2
         "wall.png",	//3
		 "",	//			
         "",	//
         "Gareki.png",			//1
         "Gareki_notloop.png",	//2
         "tower1.png",			//3
         "tower2.png",			//4
         "tower3.png",			//5
         "Fukidashi1.png",		//6
         "Fukidashi2.png",		//7
         "koukaon.png",			//8
         "uppertooth.png",		//9
         "undertooth.png",		//10
         "Life_Gauge.png",		//11
         "",	//		
         "",	//					
         "Boss_house.png",		//1
         "NouminTower1.png",	//2
         "wall.png",			//3
         ""						//					
	};		
/*		
    public string[,] Tex2dFileNameList = {
        { "","","","","","","","","","" },						//
			
        { "Noumin.png","","","","","","","","","" },			//1
        { "Noumin2.png","","","","","","","","","" },			//2

		{ "Senshi.png","","","","","","","","","" },			//3
        { "Senshi2.png","","","","","","","","","" },			//4

		{ "Souryo.png","","","","","","","","","" },			//5
        { "Souryo2.png","","","","","","","","","" },			//6

		{ "Zonbi.png","","","","","","","","","" },				//7
        { "Zonbi2.png","","","","","","","","","" },			//8
        
		{ "Necromancer.png","","","","","","","","","" },		//9
        { "Necromancer.png","","","","","","","","","" },		//10
        
		{ "","","","","","","","","","" },						//
        { "","","","","","","","","","" },						//
        { "Monument.png","","","","","","","","","" },			//11
        { "NouminHouse1.png","","","","","","","","","" },		//12
        { "NouminHouse2.png","","","","","","","","","" },		//13
        { "NouminHouse3.png","","","","","","","","","" },		//14
        { "NouminHouse4.png","","","","","","","","","" },		//15
        { "NouminHouse5.png","","","","","","","","","" },		//16
        { "SenshiHouse1.png","","","","","","","","","" },		//17
        { "SenshiHouse2.png","","","","","","","","","" },		//18
        { "SenshiHouse3.png","","","","","","","","","" },		//19
        { "SenshiHouse4.png","","","","","","","","","" },		//20
        { "SenshiHouse5.png","","","","","","","","","" },		//21
        { "SouryoHouse1.png","","","","","","","","","" },		//22
        { "SouryoHouse2.png","","","","","","","","","" },		//23
        { "SouryoHouse3.png","","","","","","","","","" },		//24
        { "SouryoHouse4.png","","","","","","","","","" },		//25
        { "SouryoHouse5.png","","","","","","","","","" },		//26
			
        { "","","","","","","","","","" },						//
        { "","","","","","","","","","" },						//
        { "NouminTower1.png","","","","","","","","","" },		//27
        { "SenshiTower1.png","","","","","","","","","" },		//28
        { "SenshiTower2.png","","","","","","","","","" },		//29
        { "SouryoTower1.png","","","","","","","","","" },		//30
        { "SouryoTower2.png","","","","","","","","","" },		//31
        { "","","","","","","","","","" },						//
        { "","","","","","","","","","" },						//
        { "Gareki.png","","","","","","","","","" },			//32
        { "Gareki_notloop.png","","","","","","","","","" },	//33
        { "wall.png","","","","","","","","","" },				//34
        { "tower1.png","","","","","","","","","" },			//35
        { "tower2.png","","","","","","","","","" },			//36
        { "tower3.png","","","","","","","","","" },			//37
        { "","","","","","","","","","" },						//			
        { "","","","","","","","","","" },						//
        { "Fukidashi1.png","","","","","","","","","" },		//38
        { "Fukidashi2.png","","","","","","","","","" },		//39
        { "koukaon.png","","","","","","","","","" },			//40
        { "","","","","","","","","","" },						//			
	};
*/		
	}
} // namespace
