/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;


namespace AppRpg{  namespace Data {


///***************************************************************************
/// モデルデータのリソースID
///***************************************************************************

/// モデルのリソースID
public enum ModelResId {
    Hero = 0,      /// 英雄
    MonsterA,      /// 怪物A
    MonsterB,      /// 怪物B
    MonsterC,      /// 怪物C
    Stage,         /// ステージ
    Sky,           /// ステージ空
    Sword,         /// 剣
    Eff00,         /// エフェクト：剣オーラ
    Eff01,         /// エフェクト：剣の軌跡（縦切り）
    Eff02,         /// エフェクト：剣の軌跡（横切り）
    Eff03,         /// エフェクト：剣ヒット（縦切り）
    Eff04,         /// エフェクト：剣ヒット（横切り）
    Eff05,         /// エフェクト：敵消滅
    Eff06,         /// エフェクト：足跡
    Eff07,         /// エフェクト：魔法攻撃
    Eff08,         /// エフェクト：魔法ヒット
    Eff09,         /// エフェクト：オブジェクト破壊
    Eff10,         /// エフェクト：影
    Eff11,         /// エフェクト：草刈
    Eff12,         /// エフェクト：プレイヤーダメージ
    Eff13,         /// エフェクト：敵＆アイテム選択カーソル
    Eff14,         /// エフェクト：移動位置カーソル
    Fix00,         /// 木箱
    Fix01,         /// 木樽
    Fix02,         /// 木の柵
    Fix03,         /// 低い木
    Fix04,         /// 立札
    Fix05,         /// 石柱
    Fix06,         /// 家Type2
    Fix07,         /// 家Type3
    Fix08,         /// 家Type4
    Fix09,         /// 荷車
    Fix10,         /// 薪山
    Fix11,         /// 小舟
    Fix12,         /// アーチ
    Fix13,         /// 木Type0
    Fix14,         /// 木Type1
    Fix15,         /// 石柱Type0
    Fix16,         /// 石柱Type1
    Fix17,         /// 石柱Type2
    Fix18,         /// 家Type0
    Fix19,         /// 家Type1
    Fix01_l1,      /// LOD:木樽
    Fix02_l1,      /// LOD:木の柵
    Fix03_l1,      /// LOD:低い木
    Fix04_l1,      /// LOD:立札
    Fix05_l1,      /// LOD:石柱
    Fix06_l1,      /// LOD:家Type2
    Fix07_l1,      /// LOD:家Type3
    Fix08_l1,      /// LOD:家Type4
    Fix09_l1,      /// LOD:荷車
    Fix10_l1,      /// LOD:薪山
    Fix11_l1,      /// LOD:小舟
    Fix12_l1,      /// LOD:アーチ
    Fix13_l1,      /// LOD:木Type0
    Fix13_l2,      /// LOD:木Type0
    Fix14_l1,      /// LOD:木Type1
    Fix14_l2,      /// LOD:木Type1
    Fix15_l1,      /// LOD:石柱Type0
    Fix16_l1,      /// LOD:石柱Type1
    Fix17_l1,      /// LOD:石柱Type2
    Fix18_l1,      /// LOD:家Type0
    Fix19_l1,      /// LOD:家Type1
    Max            ///
}

/// テクスチャのリソースID
public enum ModelTexResId {
    Hero = 0,      /// 英雄
    MonsterA,      /// 怪物A
    MonsterB,      /// 怪物B
    MonsterC,      /// 怪物C
    Stage,         /// ステージ
    Sky,           /// ステージ空
    Sword,         /// 剣
    EffA,          /// エフェクト
    EffB,          /// エフェクト
    EffC,          /// エフェクト
    EffD,          /// エフェクト
    EffE,          /// エフェクト
    EffF,          /// エフェクト
    EffG,          /// エフェクト
    EffH,          /// エフェクト
    EffI,          /// エフェクト
    Fix00,         /// 木箱
    Fix01,         /// 木樽
    Fix02,         /// 木の柵
    Fix03,         /// 低い木
    Fix04,         /// 立札
    Fix05,         /// 石柱
    Fix06,         /// 家Type2
    Fix07,         /// 家Type3
    Fix08,         /// 家Type4
    Fix09,         /// 荷車
    Fix10,         /// 薪山
    Fix11,         /// 小舟
    Fix12,         /// アーチ
    Fix13,         /// 木Type0
    Fix14,         /// 木Type1
    Fix15,         /// 石柱Type0
    Fix16,         /// 石柱Type1
    Fix17,         /// 石柱Type2
    Fix18,         /// 家Type0
    Fix19,         /// 家Type1
    Max         ///
}

/// シェーダのリソースID
public enum ModelShaderReslId {
    Normal = 0,    /// 通常
    Max         ///
}


/// エフェクトのテクスチャの管理ID
public enum ModelEffTexId {
    EffA = 0,    ///
    EffB,        ///
    EffC,        ///
    EffD,        ///
    EffE,        ///
    EffF,        ///
    EffG,        ///
    EffH,        ///
    EffI,        ///
    Max         ///
}
				
public enum Tex2dResId {
	CharStart = 0,
	Noumin1,			//1		
	Senshi1,			//2		
	Souryo1,			//3					
	NormalCharMax,
			
	Zonbi1,				//4			
	Necromancer1,		//5
			
	CharMax,			//
	HouseStart,			//
			
	Monument,			//1
	NouminHouse1,		//2
	NouminHouse2,		//3
	NouminHouse3,		//4
	NouminHouse4,		//5
	NouminHouse5,		//6
	SenshiHouse1,		//7
	SenshiHouse2,		//8
	SenshiHouse3,		//9
	SenshiHouse4,		//10
	SenshiHouse5,		//11
	SouryoHouse1,		//21
	SouryoHouse2,		//22
	SouryoHouse3,		//23
	SouryoHouse4,		//24
	SouryoHouse5,		//25
			
	HouseMax,			//26
	TowerStart,			//27
			
	Noumintower1,		//28
	SenshiTower1,		//29
	SenshiTower2,		//30
	SouryoTower1,		//31
	SouryoTower2,		//32
			
	TowerMax,		
	WallStart,			

	Wall1,				//34
	Wall2,				//34
	Wall3,				//34
			
	WallMax,			
	BalloonStart,			

	Gareki,				//32
	GarekiWall,			//33
	MakingWall1,		//35
	MakingWall2,		//36
	MakingWall3,		//37
	SpeechBalloon1,		//38
	SpeechBalloon2,		//39
	SpeechBalloon3,		//40
	Uppertooth,			//41
	Undertooth,			//42
	Life,			//42
			
	BalloonMax,
	EnemyStart,

	BossMonument,			//10
	Bosstower,			//33
	BossWall,				//34
	EnemyMax
}
public enum SetupValue {
	//リアル時間が知りたいなら数字に×0.032　確率は少なければ少ないほど高い　0だと100%死ぬ
			
	EnemyMonumentPosX = -75,	
	EnemyMonumentPosY = 65,	

	CharScale = 5,					//小人の大きさ
	HouseScale = 5,					//家の大きさ
	TowerScale = 5,					//砦の大きさ
	WallScale = 5,					//城壁の大きさ
	BallonScale = 5,				//吹き出しの大きさ
	MonumentScale = 7,				//モミュメントの大きさ	
	GionScale = 20,					//擬音の大きさ
	TexScale = 3,					//文字の大きさ
	
	TitanSize = 20,					//巨人のあたり判定
	TouchSize = 2,					//タッチするときの判定
	TouchNextActiveArea = 2,		//画面をタッチしたときに指を中心に判定が有効になる範囲　指の周りにだけ次の判定がされる　処理を軽くする用
	
	CharMoveChangeTime = 80,		//次に小人が次の方向転換するまでの時間
	CharMoveChangeRandTime = 60,		//次に小人が次の方向転換するまでの時間を多少ずらす用　ぶれ幅

	TowerGlowArea = 500,				//小人が城壁に接近したときに成長するあたり範囲
	ToNextStageCount = 1,			//画像が進化するのにかかる回数
	AppearAndLeaveTime = 20,		//めくれて出てくるのにかかる時間

	BaloonAppearRand = 5,			//吹き出しがでる確率
	BaloonAppearTime = 60,			//吹き出しがでている時間
	GionAppearRand = 10,				//擬音が出る確率
	GionAppearSpeed = 6,			//擬音が出る確率
	GionAppearTime = 10,			//擬音が出る確率
			
	EatingSpeed = 10,				//食べるときに小人が飛んでくるときの速さ
	EatingDeadArea　= 20,			//食べる時死ぬモーションに入る距離

	EnemyAppearTime = 1300,			//次にゾンビが攻めてくるまでの時間
	EnemyAppearingTime = 1000,		//次にゾンビが攻めてくるまでの時間
	ToBattleMode = 6,				//戦士とゾンビの距離　この値以下になると殺し合いが始まる　タワーとゾンビの中心位置も同じ
	NecromancerDistance = 100,		//マンサーがゾンビから離れる距離
	BattleSenshiDeadRand = 30,		//戦闘で戦士が死ぬ確率
	BattleZonbiDeadRand = 30,		//戦闘（対戦士,農民）でゾンビが死ぬ確率
	BattleZonbi2DeadRand = 20,		//戦闘（対村人、僧侶）でゾンビが死ぬ確率
	BattleNouminDeadRand = 60,		//戦闘で農民が死ぬ確率
	BattleSouryoDeadRand = 70,		//戦闘で僧侶が死ぬ確率
	BattleTowerDeadRand = 10,		//タワーが倒れる確率
	BattleMonumentDeadRand = 10,	//モニュメントが倒れる確率
	BattleMoveFarmerDisCount = 15,	//農民の消える処理　この数を農民が超えると消える
	BattleMovePriestDisCount = 15,	//僧侶の消える処理　この数を僧侶が超えると消える
	BattleMoveEnemyDisCount = 15,	//ゾンビの消える処理　この数をゾンビが超えると消える
	BattleEffectRand = 5,			//エフェクトが発生する確率
			
	NewMonumentAppearTime = 100,	//新しい家ができる、小人がわくまでの時間
	NewMonumentAppearArea = 2,		//モニュメントと新しい家の間の距離がこの値以下
	NewEnemyMultipleTower = 15,		//小人の限界人口を決める　城壁の数の倍数
	BonusNewMoEnemyLimit = 14,		//新しいモニュメントが経ったときの恩恵　人口限界解除
	BonusNewMoHouseLimit = 11,		//新しいモニュメントが経ったときの恩恵　家の数
	BonusNewMoNewEnemy   = 2,		//新しいモニュメントが経ったときの恩恵　増加量
			
	CharAppearArea = 130,			//描画する範囲
	CharAppearRot = 60,				//描画する角度
	CharAppearTime = 20,			//一回描画するのを保持する
			
	ChangeCamSpeed = 15,
			
	AddContainorArea = 1,			//あたり判定を実装する距離
	TweetTime = 300				//twitterで呟く周期
}

		

}} // namespace
