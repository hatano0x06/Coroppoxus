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
public class CtrlEnemy
{
    private List< ActorChMonster >    actorChList;
    private List< ActorChMonster >    activeList;
    public  float   	              EntryAreaDis = (float)Data.SetupValue.CharAppearArea;
    public  int                 	  EntryStayMax = (int)Data.SetupValue.CharAppearTime;
    public  const int                 EntryStayRot = (int)Data.SetupValue.CharAppearRot;
	public  const int                 EntryAreaDisLookSelfcam = 60;
	public  bool				 	  changeFlag ;
	public  int				      	  AttackMoveCount;
	private int					  	  towerNumber;
	private const float       		  moveSpeed = 0.104f;
	private Vector3					  EnemyCenterPos;
	private Vector3					  WarriorCenterPos;
	private Vector3					  FarmerCenterPos;
	private Vector3					  PriestCenterPos;
    private int          		  	  EnemyCount;
    private int          		  	  WarriorCount;		
	private int					  	  FarmerCount;
	private int					  	  PriestCount;
	private int 					  towerAttackId;
	private int 					  battleCount;
	public int 					 	 speakCount;	
	private const float	   		  	  attackPlusEffectDis = (float)Data.SetupValue.ToBattleMode;
	GameCtrlManager            		  ctrlResMgr    = GameCtrlManager.GetInstance();

    public  enum CtrlStateId{
        Move = 0,
        BattleMove,
        Battle,
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
        actorChList = new List< ActorChMonster >();
        if( actorChList == null ){
            return false;
        }

        activeList = new List< ActorChMonster >();
        if( activeList == null ){
            return false;
        }
        
		FarmerCenterPos = new Vector3(0.0f,0.0f,0.0f);
		WarriorCenterPos = new Vector3(0.0f,0.0f,0.0f);
		PriestCenterPos = new Vector3(0.0f,0.0f,0.0f);
		EnemyCenterPos = new Vector3(0.0f,0.0f,0.0f);
		FarmerCount = 0;
		WarriorCount = 0;
		PriestCount = 0;
		EnemyCount = 0;
		towerNumber = 0;
		AttackMoveCount = 0;
		battleCount = 0;
		stateId = CtrlStateId.Move;
		preStateId = CtrlStateId.Move;
		changeFlag = false;
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
    }


    /// フレーム処理
    public bool Frame()
    {
		//Console.WriteLine(actorChList.Count);
			
		if(stateId == CtrlStateId.BattleMove || stateId == CtrlStateId.Battle || stateId == CtrlStateId.BreakTowerAndWall || stateId == CtrlStateId.Murder){
//			battleCount = 0;
			AttackMoveCount++;	
			if(AttackMoveCount > 10){
				AttackMoveCount = 0;
				FarmerCenterPos = new Vector3(0.0f,0.0f,0.0f);
				WarriorCenterPos = new Vector3(0.0f,0.0f,0.0f);
				PriestCenterPos = new Vector3(0.0f,0.0f,0.0f);
				EnemyCenterPos = new Vector3(0.0f,0.0f,0.0f);
				FarmerCount = 0;
				WarriorCount = 0;
				PriestCount = 0;
				EnemyCount = 0;	
					
				for( int i=0; i<actorChList.Count; i++ ){
					if(actorChList[i].TexId == 0 && actorChList[i].eatFlag == false){
						FarmerCenterPos += actorChList[i].GetBodyPos()/100; 
						FarmerCount++;
					}else if(actorChList[i].TexId == 1  && actorChList[i].eatFlag == false){
						WarriorCenterPos += actorChList[i].GetBodyPos()/100;
						WarriorCount++;
					}else if(actorChList[i].TexId == 2  && actorChList[i].eatFlag == false){
						PriestCenterPos += actorChList[i].GetBodyPos()/100;
						PriestCount++;
					}else if((actorChList[i].TexId == 3 || actorChList[i].TexId == 4) && actorChList[i].eatFlag == false){
						if(actorChList[i].TexId == 3){
							EnemyCenterPos += actorChList[i].GetBodyPos()/100;
						}
						EnemyCount++;
					}
				}
		//		attackPos = new Vector3(attackPosX/attackNumber,attackPosY/attackNumber,attackPosZ/attackNumber);
				FarmerCenterPos = FarmerCenterPos/FarmerCount*100;
				WarriorCenterPos = WarriorCenterPos/WarriorCount*100;
				PriestCenterPos = PriestCenterPos/PriestCount*100;
				EnemyCenterPos = EnemyCenterPos/(EnemyCount-1)*100;
					
				if( stateId == CtrlStateId.BattleMove ||stateId == CtrlStateId.Murder ){
					FindTower();
				}
				
				if( stateId == CtrlStateId.BattleMove){
					if(WarriorCount != 0){
						float dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos , WarriorCenterPos);
						if(dis < attackPlusEffectDis)SetCtrlStateId(CtrlStateId.Battle);
					}
					else if(WarriorCount == 0){
						float dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos , (PriestCenterPos+FarmerCenterPos)/2);
						if(dis < attackPlusEffectDis)  SetCtrlStateId(CtrlStateId.Murder);
					}

					if(EnemyCount == 0)	SetCtrlStateId(CtrlStateId.Move);					
				}else if(stateId == CtrlStateId.BreakTowerAndWall){
					if(WarriorCount != 0){
						float dis = Common.VectorUtil.DistanceXZ(WarriorCenterPos , EnemyCenterPos);
						if(dis < attackPlusEffectDis)  SetCtrlStateId(CtrlStateId.Battle);
					}
						
					if(EnemyCount == 0) 	SetCtrlStateId(CtrlStateId.Move);					
				}else if( stateId == CtrlStateId.Battle){
					if(EnemyCount == 0){
						SetCtrlStateId(CtrlStateId.Move);
					}else if(WarriorCount == 0){
						SetCtrlStateId(CtrlStateId.BattleMove);
					}
				}else if(stateId == CtrlStateId.Murder){
					if(EnemyCount == 0){
						SetCtrlStateId(CtrlStateId.Move);
					}else if(WarriorCount != 0){
						SetCtrlStateId(CtrlStateId.Battle);					
					}
				}
			}
		}else{
			AttackMoveCount = 0;
			FarmerCount = 0;
			WarriorCount = 0;
			PriestCount = 0;
			EnemyCount = 0;	
			battleCount++;
			if(battleCount == (int)Data.SetupValue.EnemyAppearTime){
				SetCtrlStateId(CtrlEnemy.CtrlStateId.BattleMove);
				ctrlResMgr.battleStartFlag = true;
//				battleCount = 0;
			}
		}
			
			
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
		changeFlag = true;
		if(id == CtrlStateId.BattleMove){
			AttackMoveCount = 20;				
		}
		preStateId = stateId;
		stateId = id;
	}

    /// 敵の登録
    public void EntryAddEnemy( int chResId, float rotY, Vector3 pos ,int TexId )
    {
        ActorChMonster actorCh = new ActorChMonster(TexId);
        actorCh.Init();
        actorCh.Start();

        actorChList.Add( actorCh );

        SetPlace( (actorChList.Count-1), rotY, pos );
    }

    /// 敵の登録削除
    public void DeleteEntryEnemy( int idx )
    {
        actorChList.RemoveAt( idx );
    }

    /// 敵の配置
    public void SetPlace( int idx, float rotY, Vector3 pos )
    {
        float a_Cal     = (float)(3.141593f / 180.0);
        float angleY    = rotY * a_Cal;

        Matrix4 mtx = Matrix4.RotationY( angleY );
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
    private bool frameMove( ActorChMonster actorCh)
    {
		
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0 ){

			actorCh.MovedFlag = true;
			int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum() + ctrlResMgr.CtrlWall.GetEntryNum();
			if(ToNumber == 0){			
				actorCh.AiMoveRot = (int)StaticDataList.getRandom(0,360);
			}else{
				float dis;
				if(towerNumber < ctrlResMgr.CtrlTo.GetEntryNum()){
					dis = Common.VectorUtil.DistanceXZ(actorCh.BasePos,ctrlResMgr.CtrlTo.GetPos(towerNumber));
					if(dis < (int)Data.SetupValue.TowerGlowArea/10.0f && ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).TexId+(int)Data.Tex2dResId.TowerStart != (int)Data.Tex2dResId.Bosstower){
						ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).glowCounter++;
					}
				}else{
					dis = Common.VectorUtil.DistanceXZ(actorCh.BasePos,ctrlResMgr.CtrlWall.GetPos(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()));
					if(dis < (int)Data.SetupValue.TowerGlowArea/10.0f && ctrlResMgr.CtrlWall.GetUseActorBaseObj(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()).TexId == 0){
						ctrlResMgr.CtrlWall.GetUseActorBaseObj(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()).glowCounter++;
					}
				}
				
				int tempcount = 0;
				while(true){
					towerNumber = (int)StaticDataList.getRandom(0,ToNumber);
					tempcount++;
					if(tempcount > 2){
						break;						
					}
					if(actorCh.TexId == 0){
						if(towerNumber < ctrlResMgr.CtrlTo.GetEntryNum()){
							if(ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).glowFinish == false)  break;									
						}else{
							if(ctrlResMgr.CtrlWall.GetUseActorBaseObj(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()).glowFinish == false)	break;									
						}
					}else{
						break;							
					}
				}
				float tempRot;
				if(towerNumber < ctrlResMgr.CtrlTo.GetEntryNum()){
					tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,ctrlResMgr.CtrlTo.GetPos(towerNumber));					
				}else{
					tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,ctrlResMgr.CtrlWall.GetPos(towerNumber-ctrlResMgr.CtrlTo.GetEntryNum()));
				}
				actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-3,3);
			}
				
			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));

			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0 && actorCh.deadFlagSp == true && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){

					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;

				}
			}
		}
			
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
        return true;
    }

    /// フレーム：移動
    private bool frameBattleMove( ActorChMonster actorCh )
	{
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
				
			/*
			if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 0 && FarmerCount > (int)Data.SetupValue.BattleMoveFarmerDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 2 && PriestCount > (int)Data.SetupValue.BattleMovePriestDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else{
				actorCh.FrameFlag = true;
			}
			*/
			actorCh.MovedFlag = true;
				
			if(actorCh.TexId == 1 || actorCh.TexId == 3){
				if(WarriorCount != 0){
					if(actorCh.TexId == 1){
						actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
					}
					if(actorCh.TexId == 3){
						actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,WarriorCenterPos) + StaticDataList.getRandom(-20,20);
					}
				}
				else if(WarriorCount == 0){
					if(actorCh.TexId == 3){
						actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,(EnemyCenterPos+PriestCenterPos+FarmerCenterPos)/3) + StaticDataList.getRandom(-20,20);
					}					
				}
			}else if(actorCh.TexId == 4){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
				ctrlResMgr.AddEffectFromEnemy = true;
				ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
			}else{
				int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum();
				if(ToNumber == 0){
					actorCh.AiMoveRot = (int)StaticDataList.getRandom(360);
				}else{
					Vector3 TargetPos = ctrlResMgr.CtrlTo.GetPos(StaticDataList.getRandom(0,ToNumber));
					float tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,TargetPos);
					actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-10,10);
				}
			}

			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0 && actorCh.deadFlagSp == true && actorCh.TexId < 3 && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){
					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;
				}
			}
		}
			
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
			
        return true;
    }
		
	private bool FindTower(){		
		for(int i=0; i<ctrlResMgr.CtrlTo.GetEntryNum() + ctrlResMgr.CtrlWall.GetEntryNum(); i++){
			if(i<ctrlResMgr.CtrlTo.GetEntryNum()){
				if(ctrlResMgr.CtrlTo.GetUseActorBaseObj(i).glowFinish == true && ctrlResMgr.CtrlTo.GetUseActorBaseObj(towerNumber).TexId+(int)Data.Tex2dResId.TowerStart != (int)Data.Tex2dResId.Bosstower){
					float dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos,ctrlResMgr.CtrlTo.GetTowerPos(i));
					if(dis < attackPlusEffectDis){
						towerAttackId = i;
						SetCtrlStateId(CtrlStateId.BreakTowerAndWall);					
						break;
					}
				}
			}else{
				if(ctrlResMgr.CtrlWall.GetUseActorBaseObj(i-ctrlResMgr.CtrlTo.GetEntryNum()).glowFinish == true  && ctrlResMgr.CtrlWall.GetUseActorBaseObj(i-ctrlResMgr.CtrlTo.GetEntryNum()).TexId == 0){
					float dis = Common.VectorUtil.DistanceXZ(EnemyCenterPos,ctrlResMgr.CtrlWall.GetWallPos(i-ctrlResMgr.CtrlTo.GetEntryNum()));
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
	
	private bool frameBreakTower(ActorChMonster actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
			/*
			if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 0 && FarmerCount > (int)Data.SetupValue.BattleMoveFarmerDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 2 && PriestCount > (int)Data.SetupValue.BattleMovePriestDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 3 && EnemyCount > (int)Data.SetupValue.BattleMoveEnemyDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else{
				actorCh.FrameFlag = true;
			}
			*/				
			actorCh.MovedFlag = true;
				
			if(actorCh.TexId == 3 ){
				if(towerAttackId < ctrlResMgr.CtrlTo.GetEntryNum()){
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlTo.GetPos(towerAttackId)) + StaticDataList.getRandom(-30,30);
				}else{
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlWall.GetPos(towerAttackId-ctrlResMgr.CtrlTo.GetEntryNum())) + StaticDataList.getRandom(-30,30);
				}
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleEffectRand+1) == 0){
					//ctrlResMgr.CtrlPl.Addeffect(new Vector3( actorCh.BasePos.X + StaticDataList.getRandom(-15,15)/5.0f ,actorCh.BasePos.Y + 0.2f ,actorCh.BasePos.Z+ StaticDataList.getRandom(-15,15)/5.0f));
					ctrlResMgr.AddEffectFromEnemy = true;
					ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
				}
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleTowerDeadRand+1) == 0){
					if(towerAttackId < ctrlResMgr.CtrlTo.GetEntryNum()){
						ctrlResMgr.CtrlTo.killTower(towerAttackId);
					}else{
						ctrlResMgr.CtrlWall.killTower(towerAttackId-ctrlResMgr.CtrlTo.GetEntryNum());
					}
					SetCtrlStateId(CtrlStateId.BattleMove);
				}							
			}
			else if(actorCh.TexId == 1 ){
				if(towerAttackId < ctrlResMgr.CtrlTo.GetEntryNum()){
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlTo.GetPos(towerAttackId)) + StaticDataList.getRandom(-10,10);
				}else{
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlWall.GetPos(towerAttackId-ctrlResMgr.CtrlTo.GetEntryNum())) + StaticDataList.getRandom(-10,10);												
				}
			}else if(actorCh.TexId == 4){
				if(towerAttackId < ctrlResMgr.CtrlTo.GetEntryNum()){
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlTo.GetPos(towerAttackId)) + StaticDataList.getRandom(-10,10);
				}else{
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,ctrlResMgr.CtrlWall.GetPos(towerAttackId-ctrlResMgr.CtrlTo.GetEntryNum())) + StaticDataList.getRandom(-10,10);												
				}
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
			}
			else{
				int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum();
				if(ToNumber == 0){			
					actorCh.AiMoveRot = (int)StaticDataList.getRandom(360);
				}else{
					Vector3 TargetPos = ctrlResMgr.CtrlTo.GetPos(StaticDataList.getRandom(0,ToNumber));
					float tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,TargetPos);
					actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-10,10);
				}
			}
			
			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0 && actorCh.deadFlagSp == true && actorCh.TexId < 3 && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){
					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;
				}
			}
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.GionAppearRand+1) == 0 && actorCh.deadFlagSp == true && actorCh.TexId == 3){
				actorCh.setAppearCountSp((int)Data.SetupValue.GionAppearSpeed);
				AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ZonbiVo3,actorCh.GetBodyPos() );
			}
		}
			
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
        return true;
	}
		
	private bool frameBattle(ActorChMonster actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
				/*
			if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 0 && FarmerCount > (int)Data.SetupValue.BattleMoveFarmerDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 2 && PriestCount > (int)Data.SetupValue.BattleMovePriestDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else{
				actorCh.FrameFlag = true;
			}	
			*/		
			actorCh.MovedFlag = true;
				
			if(actorCh.TexId == 1 || actorCh.TexId == 3){
				if(EnemyCount != 0 && WarriorCount != 0){
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,(EnemyCenterPos+WarriorCenterPos)/2) + StaticDataList.getRandom(-30,30);
				}
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleEffectRand+1) == 0){
					//ctrlResMgr.CtrlPl.Addeffect(new Vector3( actorCh.BasePos.X + StaticDataList.getRandom(-15,15)/5.0f ,actorCh.BasePos.Y + 0.2f ,actorCh.BasePos.Z+ StaticDataList.getRandom(-15,15)/5.0f));
	                AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.PlDamage,actorCh.GetBodyPos() );
					ctrlResMgr.AddEffectFromEnemy = true;
					ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
				}
			}else if(actorCh.TexId == 4){
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos) + StaticDataList.getRandom(-20,20);
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
			}else{
				int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum();
				if(ToNumber == 0){			
					actorCh.AiMoveRot = StaticDataList.getRandom(360);
				}else{
					Vector3 TargetPos = ctrlResMgr.CtrlTo.GetPos(StaticDataList.getRandom(0,ToNumber));
					float tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,TargetPos);
					actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-10,10);
				}
			}
			
			if(actorCh.TexId == 1){
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleSenshiDeadRand+1) == 0){
					actorCh.setdeadFlag(true);
				}
			}
			else if(actorCh.TexId == 3){
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleZonbiDeadRand+1) == 0){
					actorCh.setdeadFlag(true);
				}							
			}

			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0 && actorCh.deadFlagSp == true && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){
					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;
				}
			}
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.GionAppearRand+1) == 0 && actorCh.deadFlagSp == true && actorCh.TexId == 3){
				actorCh.setAppearCountSp((int)Data.SetupValue.GionAppearSpeed);
				AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ZonbiVo3,actorCh.GetBodyPos() );
			}
		}
			
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
        return true;
	}

	private bool frameMurder(ActorChMonster actorCh){
		actorCh.AiMoveCount--;
		if(actorCh.AiMoveCount < 0){
				/*
			if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 0 && FarmerCount > (int)Data.SetupValue.BattleMoveFarmerDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 2 && PriestCount > (int)Data.SetupValue.BattleMovePriestDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else if(StaticDataList.getRandom(0,4) == 0 && actorCh.TexId == 3 && EnemyCount > (int)Data.SetupValue.BattleMoveEnemyDisCount){
				actorCh.FrameFlag = false;
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));
				return true;
			}else{
				actorCh.FrameFlag = true;
			}	
			*/			
			actorCh.MovedFlag = true;
				
			if(actorCh.TexId == 3){
				if(EnemyCount != 0 && WarriorCount == 0){
					actorCh.AiMoveRot += Common.VectorUtil.GetRotY(actorCh.AiMoveRot ,actorCh.BasePos,(EnemyCenterPos+PriestCenterPos+FarmerCenterPos)/3) + StaticDataList.getRandom(-30,30);
				}
			}else if(actorCh.TexId == 4){
//				Vector3 tempvector = EnemyCenterPos -WarriorCenterPos;
//				tempvector.Normalize();
//				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos ) + StaticDataList.getRandom(-20,20);
				actorCh.AiMoveRot += Common.VectorUtil.GetRotY( actorCh.AiMoveRot,actorCh.BasePos,EnemyCenterPos ) + StaticDataList.getRandom(-20,20);
				ctrlResMgr.AddEnemyFromNecro = true;
				ctrlResMgr.AddEnemyPos = actorCh.BasePos;
			}else{
				int ToNumber = ctrlResMgr.CtrlTo.GetEntryNum();
				if(ToNumber == 0){			
					actorCh.AiMoveRot = (int)StaticDataList.getRandom(360);
				}else{
					Vector3 TargetPos = ctrlResMgr.CtrlTo.GetPos(StaticDataList.getRandom(0,ToNumber));
					float tempRot = Common.VectorUtil.GetRotY(actorCh.AiMoveRot, actorCh.BasePos ,TargetPos);
					actorCh.AiMoveRot += tempRot+StaticDataList.getRandom(-10,10);
				}
			}

			if(actorCh.TexId == 0){
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleNouminDeadRand +1) == 0){
					actorCh.setdeadFlag(true);
				}
			}
			else if(actorCh.TexId == 2){
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleSouryoDeadRand+1) == 0){
					actorCh.setdeadFlag(true);
				}							
			}
			else if(actorCh.TexId == 3){
				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleZonbi2DeadRand+1) == 0){
					actorCh.setdeadFlag(true);
				}							
			}
				
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BattleEffectRand+1) == 0){
               AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.PlDamage,actorCh.GetBodyPos() );
				//ctrlResMgr.CtrlPl.Addeffect(new Vector3( actorCh.BasePos.X + StaticDataList.getRandom(-15,15)/5.0f ,actorCh.BasePos.Y + 0.2f ,actorCh.BasePos.Z+ StaticDataList.getRandom(-15,15)/5.0f));
					ctrlResMgr.AddEffectFromEnemy = true;
					ctrlResMgr.AddEnemyEffectPos = actorCh.BasePos;
			}
			
			actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));

			if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0 && actorCh.deadFlagSp == true && speakCount < 6){
				actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				speakCount++;
				int charVoNumber = StaticDataList.getRandom(3);
				switch(charVoNumber){
					case 0: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo1,actorCh.GetBodyPos() ); break;
					case 1: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo2,actorCh.GetBodyPos() ); break;
					case 2: AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.CharVo3,actorCh.GetBodyPos() ); break;
				}
			}
			if(StaticDataList.getRandom(0,(int)Data.SetupValue.GionAppearRand+1) == 0 && actorCh.deadFlagSp == true && actorCh.TexId == 3){
				actorCh.setAppearCountSp((int)Data.SetupValue.GionAppearSpeed);
				AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ZonbiVo3,actorCh.GetBodyPos() );
			}				
		}
		if(actorCh.FrameFlag == true){
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             moveSpeed, actorCh.AiMoveRot, false );
		}else{				
	        actorCh.SetStateMove( new Vector3( actorCh.BaseMtx.M31,actorCh.BaseMtx.M32,actorCh.BaseMtx.M33 ),
	                             0, actorCh.AiMoveRot, false );	
		}
        return true;
	}

	private bool frameEat(ActorChMonster actorCh){
		GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
		if(actorCh.GetStateId() != ActorChBase.StateId.Eat){
			actorCh.AiMoveCount--;
			if(actorCh.AiMoveCount < 0){
				actorCh.AiMoveRot = (int)StaticDataList.getRandom(360);
				actorCh.AiMoveCount = (int)(Data.SetupValue.CharMoveChangeTime + StaticDataList.getRandom(-(int)(Data.SetupValue.CharMoveChangeRandTime),(int)Data.SetupValue.CharMoveChangeRandTime));

				if(StaticDataList.getRandom(0,(int)Data.SetupValue.BaloonAppearRand+1) == 0){
					actorCh.setAppearCountSp((int)Data.SetupValue.AppearAndLeaveTime);
				}
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
				
            float dis = Common.VectorUtil.DistanceXZ( actorChList[i].BasePos, plPos );
			float rot = Common.VectorUtil.GetRotY( GameCtrlManager.GetInstance().CtrlCam.GetCamRot().Y, GameCtrlManager.GetInstance().CtrlCam.GetCamPos(), actorChList[i].BasePos);
            if( actorChList[i].ActiveFlg ){
                actorChList[i].ActiveCnt --;
                if( actorChList[i].ActiveCnt <= 0 ){
					if( dis >= EntryAreaDis){
						if(actorChList[i].changeFlag == false){
							actorChList[i].changeFlag = true;								
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
						actorChList[i].changeFlag = false;
						actorChList[i].ActiveFlg = false;							
						actorChList[i].ActiveCnt    = EntryStayMax;
					}
                }
                else{
                    activeList.Add( actorChList[i] );
                    continue ;
                }
            }
				
			if(ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.LookMyself ||
				ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.NormalToLookMyself ||	   
				ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.LookMyselfToNormal   ){
	            if( actorChList[i].ActiveFlg == false ){
	                actorChList[i].ActiveFlg    = true;
	              	actorChList[i].ActiveCnt    = EntryStayMax;
	                actorChList[i].ActiveDis    = dis;
					actorChList[i].ActiveRot	= rot;		
					activeList.Add( actorChList[i] );
	            }
			}else{
	            if( actorChList[i].ActiveFlg == false ){
					if( dis < EntryAreaDis){
	                    actorChList[i].ActiveFlg    = true;
	                    actorChList[i].ActiveCnt    = EntryStayMax;
	                    actorChList[i].ActiveDis    = dis;
						actorChList[i].ActiveRot	= 0;
	                    activeList.Add( actorChList[i] );
	                }
				}
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


}

} // namespace
