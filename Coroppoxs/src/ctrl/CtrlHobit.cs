/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;


namespace AppRpg {

///***************************************************************************
/// 敵制御
///***************************************************************************
public class CtrlHobit
{
    private List< ActorChHobit >      	actorChList;
    private List< ActorChHobit >      	activeList;
    public  List< ActorChHobit >      	FarmerList;
    public  List< ActorChHobit >      	WarriorList;
    public  List< ActorChHobit >      	PriestList;
    public  List< ActorChHobit >      	ZonbiList;
    public  float   	              	EntryAreaDis = (float)Data.SetupValue.CharAppearArea;
    public  int                 	  	EntryStayMax = (int)Data.SetupValue.CharAppearTime;
    public  const int                 	EntryStayRot = (int)Data.SetupValue.CharAppearRot;
	public  const int                 	EntryAreaDisLookSelfcam = 60;
	public  bool				 	  	changeStateFlag ;
	private const float       		  	moveSpeed = 0.104f;
	private Vector3					  	EnemyPos;
	private Vector3					  	WarriorPos;
	private Vector3					  	FarmerPos;
	private Vector3					  	PriestPos;
	private Vector3					  	EnemyCenterPos;
	private Vector3					  	WarriorCenterPos;
	private Vector3					  	FarmerCenterPos;
	private Vector3					  	PriestCenterPos;		
	private int 					  	towerAttackId;
	private bool						EnemyDispearFlag;
	public int 					 	 	speakCount;	
	private const float	   		  	  	attackPlusEffectDis = (float)Data.SetupValue.ToBattleMode;
	GameCtrlManager            		  	ctrlResMgr    = GameCtrlManager.GetInstance();

    public  enum CtrlStateId{
        Move = 0,
        BattleMove,
        Battle,
		MurderMove,
		Murder,
		BreakTowerAndWall,
		Eat,
    }
    public CtrlStateId    stateId;	
    public CtrlStateId    preStateId;	
		
	
/// public メソッド
///---------------------------------------------------------------------------
		
    /// 初期化
    public bool Init()
    {
        actorChList = new List< ActorChHobit >();
        if( actorChList == null ){
            return false;
        }

        activeList = new List< ActorChHobit >();
        if( activeList == null ){
            return false;
        }
			
		FarmerList = new List<ActorChHobit>();
		WarriorList = new List<ActorChHobit>();
		PriestList = new List<ActorChHobit>();
		ZonbiList = new List<ActorChHobit>();
        
		FarmerPos = new Vector3(0.0f,0.0f,0.0f);
		WarriorPos = new Vector3(0.0f,0.0f,0.0f);
		PriestPos = new Vector3(0.0f,0.0f,0.0f);
		EnemyPos = new Vector3(0.0f,0.0f,0.0f);
			
		stateId = CtrlStateId.Move;
		preStateId = CtrlStateId.Move;
		changeStateFlag = false;
		EnemyDispearFlag = false;
		speakCount = 0;
        return true;
    }

    /// 破棄
    public void Term()
    {
        if( activeList != null ){
            activeList.Clear();
        }
        if( actorChList != null ){
            for( int i=0; i<actorChList.Count; i++ ){
                actorChList[i].Term();
            }
            actorChList.Clear();
        }

		activeList       = null;
        actorChList      = null;
    }

    /// 開始
    public bool Start()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Start();
        }

        return true;
    }

    /// 終了
    public void End()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].End();
        }
        actorChList.Clear();
        activeList.Clear();
		FarmerList.Clear();
		WarriorList.Clear();
		PriestList.Clear();
		ZonbiList.Clear();
			
    }


    /// フレーム処理
    public bool Frame()
    {
		//Console.WriteLine(actorChList.Count);
			
//		if(stateId == CtrlStateId.Move && ZonbiList.Count == 0){		//戦闘開始まで数える
		if(ctrlResMgr.countTime == (int)Data.SetupValue.EnemyAppearTime){
			EnemyDispearFlag = false;
			SetCtrlStateId(CtrlStateId.BattleMove);
			ctrlResMgr.battleStartFlag = true;
		}else if(ctrlResMgr.countTime == (int)Data.SetupValue.EnemyAppearTime+(int)Data.SetupValue.EnemyAppearingTime){
			if(stateId != CtrlStateId.Move){
				SetCtrlStateId(CtrlStateId.Move);
			}
		}else if(ctrlResMgr.countTime > (int)Data.SetupValue.EnemyAppearTime+(int)Data.SetupValue.EnemyAppearingTime && EnemyDispearFlag == false){			
			float dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos,ctrlResMgr.EnemyMoPos);
			if(dis < (int)Data.SetupValue.ToBattleMode*3){
				EnemyDispearFlag = true;
				ctrlResMgr.ZonbiNumber = ZonbiList.Count;
			}
		}
			
		makeList();
		changeState();
        for( int i=0; i<actorChList.Count; i++ ){				
			GameActorCollManager    useCollMgr    = actorChList[i].GetMoveCollManager();
		           /// 他アクタからのイベントをチェック
	            ///-------------------------------------	
			if(actorChList[i].MovedFlag == true){
				Vector3 tempVector3 = actorChList[i].GetBodyPos()- new Vector3(ctrlResMgr.CtrlCam.GetCamPos().X, ctrlResMgr.CtrlPl.GetPos().Y, ctrlResMgr.CtrlCam.GetCamPos().X);
				if(tempVector3.Cross(new Vector3( actorChList[i].BaseMtx.M31,actorChList[i].BaseMtx.M33,actorChList[i].BaseMtx.M33)).Y > 0){
					actorChList[i].moveAngle = true;
				}else{
					actorChList[i].moveAngle = false;
				}
				actorChList[i].MovedFlag = false;
			}
							
            /// フレーム処理
	            ///-------------------------------------
			if(actorChList[i].GetStateId() != ActorChBase.StateId.Dead && actorChList[i].GetStateId() != ActorChBase.StateId.Eat){
				switch(stateId){
					case CtrlStateId.Move: 				frameMove(actorChList[i]); 			break;
					case CtrlStateId.BattleMove: 		frameBattleMove( actorChList[i] ); 	break;
					case CtrlStateId.Battle: 			frameBattle( actorChList[i] ); 		break;
					case CtrlStateId.MurderMove:		frameMurderMove( actorChList[i] );	break;
					case CtrlStateId.Murder:			frameMurder( actorChList[i] );		break;
					case CtrlStateId.BreakTowerAndWall:	frameBreakTower( actorChList[i] );	break;
					case CtrlStateId.Eat:				frameEat(actorChList[i]); 			break;
				}
			}
			useCollMgr.SetMoveShape( GetUseActorBaseObj(i).GetMoveShape() ); 	
			ctrlResMgr.SetCollisionActorEn( useCollMgr.TrgContainer, actorChList[i].BasePos );
			actorChList[i].Frame();
		}
		return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<activeList.Count; i++ ){
			if(activeList[i].ActiveFlg == true){
	            activeList[i].Draw( graphDev);
			}
        }
        return true;
    }
		
	public bool DrawIdx(DemoGame.GraphicsDevice graphDev , int idx){
		if(activeList.Count != 0) {
			if(activeList[idx].deadFlag == true || activeList[idx].ActiveFlg == false){
				return true;
			}
			activeList[idx].Draw( graphDev);
		}		
        return true;
	}		
	
	public bool DrawText(DemoGame.GraphicsDevice graphDev){
        for( int i=0; i<activeList.Count; i++ ){
			if(activeList[i].ActiveFlg == true){
				activeList[i].DrawText(graphDev);
			}
		}
		return true;
	}
		

    /// 描画処理
    public bool DrawAlpha( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<activeList.Count; i++ ){

            Matrix4 mtx = activeList[i].BaseMtx;
            mtx.M41 = activeList[i].GetBodyPos().X;
            mtx.M43 = activeList[i].GetBodyPos().Z;
        }
        return true;
    }


    /// 描画処理（デバック用）
    public bool DrawDebug( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Frame();
            actorChList[i].Draw( graphDev );
        }
    
        return true;
    }


    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        for( int i=0; i<actorChList.Count; i++ ){
            for( int j=0; j<actorChList[i].GetUseObjNum(); j++ ){
				if(actorChList[i].deadFlag == true || actorChList[i].FrameFlag == false || actorChList[i].ActiveFlg == false){
					return true;		
				}
				if(actorChList[i].ActiveDis < (int)Data.SetupValue.TitanSize+(int)Data.SetupValue.AddContainorArea){
	        		container.Add( actorChList[i], actorChList[i].GetUseObj(j) );
	            }
			}
        }

        return true;
    }

    /// 移動目標対象をコンテナへ登録
    public bool SetDestinationActor( GameActorCollObjContainer container )
    {
        for( int i=0; i<actorChList.Count; i++ ){
			if(actorChList[i].deadFlag == true || actorChList[i].FrameFlag == false || actorChList[i].ActiveFlg == false){
				return true;		
			}
            for( int j=0; j<actorChList[i].GetUseObjNum(); j++ ){
				if(ctrlResMgr.CtrlStg.FirstTouch() == true){	
	                container.Add( actorChList[i], actorChList[i].GetUseObj(j) );
				}else{
					float dis = Common.VectorUtil.DistanceXZ(activeList[i].GetBodyPos(), ctrlResMgr.CtrlStg.TouchPosition());
					if( dis < (float)Data.SetupValue.TouchNextActiveArea){
						container.Add( actorChList[i], actorChList[i].GetUseObj(j) );
					}
				}
            }
        }
        return true;
    }


    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetinterfereActor( GameActorContainer container )
    {
        for( int i=0; i<actorChList.Count; i++ ){
			if(actorChList[i].deadFlag == true || actorChList[i].FrameFlag == false || actorChList[i].ActiveFlg == false){
				return true;
			}
			if(actorChList[i].ActiveDis < (int)Data.SetupValue.TitanSize+(int)Data.SetupValue.AddContainorArea){
	            container.Add( actorChList[i] );
			}
        }
        return true;
    }


    /// ベースOBJの取得
    public GameObjProduct GetUseActorBaseObj( int idx )
    {
        return actorChList[idx].GetUseObj(0);
    }
		
	public void SetCtrlStateId(CtrlStateId id){
		changeStateFlag = true;
		preStateId = stateId;
		stateId = id;
	}

    /// 敵の登録
    public void EntryAddEnemy( int TexId, Vector3 pos)
    {
        ActorChHobit actorCh = new ActorChHobit(TexId);
        actorCh.Init();
        actorCh.Start();

        actorChList.Add( actorCh );

        SetPlace( (actorChList.Count-1), pos );
    }

    /// 敵の登録削除
    public void DeleteEntryEnemy( int idx )
    {
        actorChList.RemoveAt( idx );
    }

    /// 敵の配置
    public void SetPlace( int idx, Vector3 pos )
    {
        Matrix4 mtx = Matrix4.RotationY( StaticDataList.getRandom(0,360) );
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        actorChList[idx].SetPlace( mtx );
    }

    /// 敵の座標を取得
    public Vector3 GetPos( int idx )
    {
        return new Vector3( actorChList[idx].BaseMtx.M41, actorChList[idx].BaseMtx.M42, actorChList[idx].BaseMtx.M43 );
    }
    /// 敵の向きを取得
    public float GetRotY( int idx )
    {
        float angleY = (float)Math.Atan2( actorChList[idx].BaseMtx.M31, actorChList[idx].BaseMtx.M33 );
        float a_Cal  = (float)(angleY / (3.141593f / 180.0));
        return a_Cal;
    }
    /// 登録タイプの取得
    public int GetChTypeId( int idx )
    {
        return (int)actorChList[idx].GetChTypeId();
    }
    /// 登録数の取得
    public int GetEntryNum()
    {
        return actorChList.Count;
    }
    /// 登録数の取得
    public int GetActiveNum()
    {
        return activeList.Count;
    }
    /// 距離
    public float Distance( int idx )
    {
		if(activeList.Count == 0 || activeList.Count == idx){
			return 0.0f;				
		}
		return activeList[idx].ActiveDis;
    }


/// private メソッド
///---------------------------------------------------------------------------
    /// フレーム：移動
    private bool frameMove( ActorChHobit actorCh)
    {
		
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0 ){

			actorCh.MovedFlag = true;
			if(actorCh.texId < (int)Data.Tex2dResId.NormalCharMax){
				int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum() + ctrlResMgr.CtrlWall.GetEntryNum();
				if(ToNumber == 0){			
					actorCh.AiMoveRot = (int)StaticDataList.getRandom(0,360);
				}else{
					int towerNumber = (int)StaticDataList.getRandom(0,ToNumber);				
					glowTowerWall(actorCh,towerNumber);
					moveToTowerWall(actorCh,towerNumber);
				}
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,ctrlResMgr.EnemyMoPos) + StaticDataList.getRandom(-10,10);
			}
				
			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));

			actorSpeek(actorCh);
		}
		moveCh(actorCh);
        return true;
    }

    /// フレーム：移動
    private bool frameBattleMove( ActorChHobit actorCh )
	{
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
			actorCh.MovedFlag = true;
				
			if(actorCh.texId == (int)Data.Tex2dResId.Noumin1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Senshi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Souryo1){
				randomMoveToTowerWall(actorCh);
			}				
			if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,false)+ StaticDataList.getRandom(-20,20));
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Necromancer1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyPos/ZonbiList.Count) + StaticDataList.getRandom(-20,20);
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
				ctrlResMgr.AddEffectFromEnemy = true;
				ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
			}

			actorSpeek(actorCh);
		}
		moveCh(actorCh);
			
        return true;
    }
		
	private bool FindTower(){		
		for(int i=0; i<ctrlResMgr.CtrlTo.GetEntryNum() + ctrlResMgr.CtrlWall.GetEntryNum(); i++){
			if(i<ctrlResMgr.CtrlTo.GetEntryNum()){
				if(ctrlResMgr.CtrlTo.GetUseActorBaseObj(i).glowFinish == true && ctrlResMgr.CtrlTo.GetUseActorBaseObj(i).GetTexId() != 0){
					float dis = Common.VectorUtil.DistanceXZ(EnemyPos/ZonbiList.Count,ctrlResMgr.CtrlTo.GetTowerPos(i));
					if(dis < attackPlusEffectDis){
						towerAttackId = i;
						SetCtrlStateId(CtrlStateId.BreakTowerAndWall);					
						break;
					}
				}
			}else{
				if(ctrlResMgr.CtrlWall.GetUseActorBaseObj(i-ctrlResMgr.CtrlTo.GetEntryNum()).glowFinish == true  && ctrlResMgr.CtrlWall.GetUseActorBaseObj(i-ctrlResMgr.CtrlTo.GetEntryNum()).GetTexId() != 0){
					float dis = Common.VectorUtil.DistanceXZ(EnemyPos/ZonbiList.Count,ctrlResMgr.CtrlWall.GetWallPos(i-ctrlResMgr.CtrlTo.GetEntryNum()));
					if(dis < attackPlusEffectDis){
						towerAttackId = i;
						SetCtrlStateId(CtrlStateId.BreakTowerAndWall);					
						break;
					}
				}
			}
		}
		return true;			
	}
		
	private bool frameBreakTower(ActorChHobit actorCh){
			
		return true;			
	}
		
	private bool frameBattle(ActorChHobit actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
			actorCh.MovedFlag = true;
			if(actorCh.texId == (int)Data.Tex2dResId.Noumin1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleNouminDeadRand*2*rateCh(false,false,false,true)) actorCh.changeId((int)Data.Tex2dResId.Zonbi1);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Senshi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleSenshiDeadRand*2*rateCh(false,false,false,true)) actorCh.changeId((int)Data.Tex2dResId.Zonbi1);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Souryo1){
				randomMoveToTowerWall(actorCh);
			}				
			if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleZonbiDeadRand*2*rateCh(true,true,false,false)) actorCh.setdeadFlag(true);
			}

			battleEffect(actorCh);				
			actorSpeek(actorCh);
		}
		moveCh(actorCh);
			
        return true;
	}
		
	private bool frameMurderMove(ActorChHobit actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
			if(actorCh.texId == (int)Data.Tex2dResId.Noumin1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Senshi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Souryo1){
				randomMoveToTowerWall(actorCh);
			}				
			if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,PriestCenterPos+ StaticDataList.getRandom(-20,20));
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Necromancer1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyPos/ZonbiList.Count) + StaticDataList.getRandom(-20,20);
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
				ctrlResMgr.AddEffectFromEnemy = true;
				ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
			}

			
			actorSpeek(actorCh);
		}
		moveCh(actorCh);
        return true;
	}

	private bool frameMurder(ActorChHobit actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
			actorCh.MovedFlag = true;
				
			if(actorCh.texId == (int)Data.Tex2dResId.Noumin1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleNouminDeadRand) actorCh.changeId((int)Data.Tex2dResId.Zonbi1);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Senshi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleSenshiDeadRand) actorCh.changeId((int)Data.Tex2dResId.Zonbi1);
			}
			if(actorCh.texId == (int)Data.Tex2dResId.Souryo1){
				randomMoveToTowerWall(actorCh);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleSouryoDeadRand*2*rateCh(false,false,false,true)) actorCh.changeId((int)Data.Tex2dResId.Zonbi1);					
			}				
			if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,meanChPos(true,true,false,true)) + StaticDataList.getRandom(-20,20);
				if(StaticDataList.random100() < (int)Data.SetupValue.BattleZonbi2DeadRand*2*rateCh(false,false,true,false)) actorCh.setdeadFlag(true);
			}
					
			battleEffect(actorCh);			
			actorSpeek(actorCh);
		}
		moveCh(actorCh);
        return true;
	}

	private bool frameEat(ActorChHobit actorCh){
		GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
		if(actorCh.GetStateId() != ActorChBase.StateId.Eat){
			actorCh.AiMoveCount--;
			if(actorCh.AiMoveCount < 0){
				actorCh.AiMoveRot = (int)StaticDataList.getRandom(360);
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));					
			}							
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
   		                          moveSpeed, actorCh.AiMoveRot, false );
		}
        return true;
	}

    /// アクティブなキャラクタのセット
    public void setActiveChList()
    {
        Vector3 plPos = GameCtrlManager.GetInstance().CtrlCam.GetCamPos();
	
        activeList.Clear();
        for( int i=0; i<actorChList.Count; i++ ){

				/// 登録削除
            ///-------------------------------------
            if( !actorChList[i].Enable){
                actorChList[i].End();
                actorChList.RemoveAt(i);     // 要素の削除
                i --;
                continue ;
            }
				
			if(EnemyDispearFlag == true && actorChList[i].texId == (int)Data.Tex2dResId.Zonbi1)  actorChList[i].setdeadFlag(true);
				
            float dis = Common.VectorUtil.DistanceXZ( actorChList[i].BasePos, GameCtrlManager.GetInstance().CtrlCam.GetCamPos() );
			float rot = Common.VectorUtil.GetRotY( GameCtrlManager.GetInstance().CtrlCam.GetCamRot().Y, GameCtrlManager.GetInstance().CtrlCam.GetCamPos(), actorChList[i].BasePos);
            if( actorChList[i].ActiveFlg ){
                actorChList[i].ActiveCnt --;
                if( actorChList[i].ActiveCnt <= 0 ){
					if( dis >= EntryAreaDis){
						if(actorChList[i].farFlag == false){
							actorChList[i].farFlag = true;
							actorChList[i].ActiveFlg    = true;
		                    actorChList[i].ActiveCnt    = (int)Data.SetupValue.AppearAndLeaveTime;
		                    actorChList[i].ActiveDis    = dis;
							actorChList[i].ActiveRot	= rot;		
							activeList.Add( actorChList[i] );
						}else{
							actorChList[i].setAppearCount((int)Data.SetupValue.AppearAndLeaveTime);
		                    actorChList[i].ActiveFlg = false;
						}
					}else{
						actorChList[i].farFlag = false;
						actorChList[i].ActiveFlg = false;
						actorChList[i].ActiveCnt    = EntryStayMax;
					}
                }
                else{
                    activeList.Add( actorChList[i] );
                    continue ;
                }
            }
				
            if( actorChList[i].ActiveFlg == false ){
                actorChList[i].ActiveFlg    = true;
              	actorChList[i].ActiveCnt    = EntryStayMax;
                actorChList[i].ActiveDis    = dis;
				actorChList[i].ActiveRot	= rot;		
				activeList.Add( actorChList[i] );
            }
        }
    }
	
    /// 距離が近い順にソート
    public void SortNear()
    {
        /// 描画されなかった樹を距離が近い順にソート
        activeList.Sort((x, y) => {
                if (x.ActiveDis > y.ActiveDis) {
                    return -1;
                }
                else if (x.ActiveDis < y.ActiveDis) {
                    return 1;
                }
                else {
                    return 0;
                }			
        	}
		);
    }
		
    /// 距離が近い順にソート
    public void Sortactor()
    {
        /// 描画されなかった樹を距離が近い順にソート
        actorChList.Sort((x, y) => {
                if (x.ActiveDis > y.ActiveDis) {
                    return -1;
                }
                else if (x.ActiveDis < y.ActiveDis) {
                    return 1;
                }
                else {
                    return 0;
                }			
        	}
		);
    }	
	
	private void moveCh(ActorChHobit actorCh){	
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
	}
		
	private void actorSpeek(ActorChHobit actorCh){
		actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
		if(actorCh.texId != (int)Data.Tex2dResId.Zonbi1 && actorCh.texId != (int)Data.Tex2dResId.Necromancer1){
			if(StaticDataList.getRandom(0,100) < (int)Data.SetupValue.BaloonAppearRand && actorCh.deadFlagSp == true && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){
					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;
				}
			}
		}else if(actorCh.texId == (int)Data.Tex2dResId.Zonbi1){
			if(StaticDataList.getRandom(0,100) < (int)Data.SetupValue.GionAppearRand && actorCh.deadFlagSp == true && speakCount < 6 ){
				actorCh.setAppearCountSp((int)Data.SetupValue.GionAppearSpeed);
				speakCount++;
				AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ZonbiVo3,actorCh.GetBodyPos() );
			}
		}
	}
		
	private void glowTowerWall(ActorChHobit actorCh, int towerNumber){
		float dis;
		if(towerNumber < ctrlResMgr.CtrlTo.GetEntryNum()){
			dis = Common.VectorUtil.DistanceXZ(actorCh.BasePos,ctrlResMgr.CtrlTo.GetPos(towerNumber));
			if(dis < (int)Data.SetupValue.TowerGlowArea/10.0f && ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).GetTexId() != 0){
				ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).glowCounter++;
//				actorCh.setdeadFlag(true);
			}
		}else{
			dis = Common.VectorUtil.DistanceXZ(actorCh.BasePos,ctrlResMgr.CtrlWall.GetPos(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()));
			if(dis < (int)Data.SetupValue.TowerGlowArea/10.0f && ctrlResMgr.CtrlWall.GetUseActorBaseObj(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()).GetTexId() != 0){
				ctrlResMgr.CtrlWall.GetUseActorBaseObj(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()).glowCounter++;
//				actorCh.setdeadFlag(true);
			}
		}			
	}
		
	private void moveToTowerWall(ActorChHobit actorCh , int towerNumber){
		float tempRot;
		if(towerNumber < ctrlResMgr.CtrlTo.GetEntryNum()){
			tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,ctrlResMgr.CtrlTo.GetPos(towerNumber));					
		}else{
			tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,ctrlResMgr.CtrlWall.GetPos(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()));
		}
		actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-3,3);
	}
		
	private void randomMoveToTowerWall(ActorChHobit actorCh){		
		int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum() + ctrlResMgr.CtrlWall.GetEntryNum();
		if(ToNumber == 0){			
			actorCh.AiMoveRot = (int)StaticDataList.getRandom(0,360);
		}else{
			int towerNumber = (int)StaticDataList.getRandom(0,ToNumber);				
			moveToTowerWall(actorCh,towerNumber);
		}
	}
		
	private void battleEffect(ActorChHobit actorCh)
	{
		if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleEffectRand+1) == 0){
	         AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.PlDamage,actorCh.GetBodyPos() );
			//ctrlResMgr.CtrlPl.Addeffect(new Vector3( actorCh.BasePos.X + StaticDataList.getRandom(-15,15)/5.0f ,actorCh.BasePos.Y + 0.2f ,actorCh.BasePos.Z+ StaticDataList.getRandom(-15,15)/5.0f));
				ctrlResMgr.AddEffectFromEnemy = true;
				ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
		}
	}
		
	private void makeList(){
		FarmerPos = new Vector3(0.0f,0.0f,0.0f);
		WarriorPos = new Vector3(0.0f,0.0f,0.0f);
		PriestPos = new Vector3(0.0f,0.0f,0.0f);
		EnemyPos = new Vector3(0.0f,0.0f,0.0f);
		FarmerList.Clear();
		WarriorList.Clear();
		PriestList.Clear();
		ZonbiList.Clear();
			
        for( int i=0; i<actorChList.Count; i++ ){
			if(actorChList[i].texId == (int)Data.Tex2dResId.Noumin1 && actorChList[i].eatFlag == false){
				FarmerList.Add(actorChList[i]);
				FarmerPos += actorChList[i].GetBodyPos(); 					
			}else if(actorChList[i].texId == (int)Data.Tex2dResId.Senshi1 && actorChList[i].eatFlag == false){
				WarriorList.Add(actorChList[i]);
				WarriorPos += actorChList[i].GetBodyPos();
			}else if(actorChList[i].texId == (int)Data.Tex2dResId.Souryo1 && actorChList[i].eatFlag == false){
				PriestList.Add(actorChList[i]);
				PriestPos += actorChList[i].GetBodyPos();		
			}else if(actorChList[i].texId == (int)Data.Tex2dResId.Zonbi1 && actorChList[i].eatFlag == false){
				ZonbiList.Add(actorChList[i]);
				EnemyPos += actorChList[i].GetBodyPos();
			}
		}			
		if(FarmerList.Count > 0){
			FarmerCenterPos = FarmerPos/FarmerList.Count;
		}else if(FarmerList.Count == 0){
			FarmerCenterPos = FarmerPos;
		}
		if(WarriorList.Count > 0){
			WarriorCenterPos = WarriorPos/WarriorList.Count;
		}else if(WarriorList.Count == 0){
			WarriorCenterPos = WarriorPos;
		}
		if(PriestList.Count > 0){
			PriestCenterPos = PriestPos/PriestList.Count;
		}else if(PriestList.Count == 0){
			PriestCenterPos = PriestPos;
		}
		if(ZonbiList.Count > 0){
			EnemyCenterPos = EnemyPos/ZonbiList.Count;
		}else if(ZonbiList.Count == 0){
			EnemyCenterPos = EnemyPos;
		}
	}
		
	private Vector3 meanChPos(bool farmerPosFlag,bool warriorPosFlag,bool priestPosFlag,bool zonbiPosFlag){
		Vector3 SumPos;
		SumPos.X = 0;
		SumPos.Y = 0;
		SumPos.Z = 0;
		int SumCounter = 0;
		if(farmerPosFlag == true){
			SumPos += FarmerPos;
			SumCounter += FarmerList.Count;
		}
		if(warriorPosFlag == true){
			SumPos += WarriorPos;
			SumCounter += WarriorList.Count;
		}
		if(priestPosFlag == true){
			SumPos += PriestPos;
			SumCounter += PriestList.Count;
		}
		if(zonbiPosFlag == true){
			SumPos += EnemyPos;
			SumCounter += ZonbiList.Count;
		}
		return SumPos/SumCounter;
	}
		
	private float rateCh(bool farmerFlag,bool warriorFlag,bool priestFlag,bool zonbiFlag){
		int molecule = 0;
		if(farmerFlag == true){
			molecule += FarmerList.Count;
		}
		if(warriorFlag == true){
			molecule += WarriorList.Count;
		}
		if(priestFlag == true){
			molecule += PriestList.Count;
		}
		if(zonbiFlag == true){
			molecule += ZonbiList.Count;
		}
		return (float)molecule/(FarmerList.Count+WarriorList.Count+PriestList.Count+ZonbiList.Count);
	}		
		
	private void changeState(){
		float dis;
		if(stateId == CtrlStateId.BattleMove){
			dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos,meanChPos(true,true,false,false));
			if(dis < (int)Data.SetupValue.ToBattleMode) SetCtrlStateId(CtrlStateId.Battle);	
		}else if(stateId == CtrlStateId.Battle){
			if(WarriorList.Count+FarmerList.Count == 0) SetCtrlStateId(CtrlStateId.MurderMove);
			else if(ZonbiList.Count == 0) SetCtrlStateId(CtrlStateId.Move);
			else if(WarriorList.Count+FarmerList.Count+PriestList.Count == 0) SetCtrlStateId(CtrlStateId.BreakTowerAndWall);
		}else if(stateId == CtrlStateId.MurderMove){
			dis = Common.VectorUtil.DistanceXZ(PriestCenterPos,EnemyCenterPos);
			if(WarriorList.Count+FarmerList.Count > 0) SetCtrlStateId(CtrlStateId.Battle);
			else if(ZonbiList.Count == 0) SetCtrlStateId(CtrlStateId.Move);
			else if(dis < (int)Data.SetupValue.ToBattleMode)  SetCtrlStateId(CtrlStateId.Murder);
		}else if(stateId == CtrlStateId.Murder){
			if(WarriorList.Count+FarmerList.Count > 0) SetCtrlStateId(CtrlStateId.Battle);
			else if(ZonbiList.Count == 0) SetCtrlStateId(CtrlStateId.Move);
			else if(WarriorList.Count+FarmerList.Count+PriestList.Count == 0) SetCtrlStateId(CtrlStateId.BreakTowerAndWall);
		}
	}
}

} // namespace
