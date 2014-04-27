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
/// ステージ制御
///***************************************************************************
public class CtrlStage
{

    private ActorStgNormal            actorStg;
    private ActorDestinationMark      actorDestination;
    public  GameActorEventContainer   EventCntr;
    GameCtrlManager          		  ctrlResMgr    = GameCtrlManager.GetInstance();
	public  float                     newTowerDis  =  4.0f;
	public  float                     newEffectDis =  2.0f;
		
    private bool                      destinationFlg;
    private Vector3                   destinationPos;
    private ActorUnitCollLook         calCollLook;
    private GameActorProduct          destinationTrgActor;
    private Vector3   			      towerPos;
    private Vector3   			      effectPos;
    private Vector3   			      towerNowPos;
	private Vector3 				  TouchPostion;
	private int					  	  TouchCount;
    private bool   			     	  makeTowerFlag = true;
	public  float				  	  TowerAreaNorth;
	public  float				  	  TowerAreaSouth;
	public  float				  	  TowerAreaWest;
	public  float				  	  TowerAreaEast;
	public  bool					  MonumentSetFlag; 	
	private int 					  makeEnemyMonument;

	private int 					  mode;

/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorStg = new ActorStgNormal();
        actorStg.Init();

        actorDestination = new ActorDestinationMark();
        actorDestination.Init();

        calCollLook    = new ActorUnitCollLook();
        calCollLook.Init();
		
		EventCntr = new GameActorEventContainer();
        EventCntr.Init();
			
		towerPos.Xyz = StaticDataList.getVectorZero();
		effectPos.Xyz = StaticDataList.getVectorZero();
		mode = 0;
		TouchCount = 0;	
		MonumentSetFlag = false;	
		makeEnemyMonument = 0;
		
		TowerAreaNorth = -114.4f;
		TowerAreaSouth = -120.4f;
		TowerAreaEast = 109.0f;
		TowerAreaWest = 103.0f;

		return true;
    }

    /// 破棄
    public void Term()
    {
        if( calCollLook != null ){
            calCollLook.Term();
        }
        if( EventCntr != null ){
            EventCntr.Clear();
            EventCntr.Term();
        }
        actorStg.Term();
        actorDestination.Term();
			
		EventCntr		  = null;
        calCollLook       = null;
        actorStg          = null;
        actorDestination  = null;
    }

    /// 開始
    public bool Start()
    {
        actorStg.Start();
        ClearDestination();
        EventCntr.Clear();
        return true;
    }

    /// 終了
    public void End()
    {
        actorStg.End();
        actorDestination.End();
        EventCntr.Clear();
        destinationTrgActor = null;
    }


    /// フレーム処理
    public bool Frame()
    {
        actorStg.Frame();

		/// 目標地点が消失5
		if( destinationTrgActor != null && destinationTrgActor.Enable == false ){
			ClearDestination();
		}

			

        /// 目的地のセット
        if( (AppInput.GetInstance().Event & AppInput.EventId.DestinationMove) != 0 && makeTowerFlag == true){
			if((AppInput.GetInstance().Event & AppInput.EventId.buttonPress) == 0){
				//範囲拡張アイテム設置モード
				if(MonumentSetFlag == true){
					setMonument(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);
					mode = 3;
				}else{
					if(mode == 0){
						//最初に触ったオブジェクト探し
						findingTower(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);	
					}else if(mode == 2 && makeTowerFlag == true){
						//壁の生成
						makeWall(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);
					}
				}				
			}
			if((AppInput.GetInstance().Event & AppInput.EventId.buttonPress) != 0){
				setEatTarget(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);
			}
			TouchCount ++;
			/*
			if(mode == 0){
				findingMode(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);
			}else if(mode == 1){
				setEatTarget(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);
			}else if(mode == 2){
				makeWall(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);				
			}else if(mode == 3){
				setBreakTarget(AppInput.GetInstance().SingleScrTouchPosX, AppInput.GetInstance().SingleScrTouchPosY);					
			}
			*/
			//Console.WriteLine(mode);

//            destinationFlg = true;
        }
        else if( (AppInput.GetInstance().Event & AppInput.EventId.DestinationCancel) != 0 ){
			ClearMakeWall();
//            ClearDestination();
        }

        if( actorDestination.Enable ){
//            actorDestination.Frame();
        }
        return true;
    }

    /// フレーム処理（タイトル画面時）
    public bool FrameTitle()
    {
        actorStg.Frame();
        ClearDestination();
        return true;
    }

    /// フレーム処理（リザルト画面時）
    public bool FrameResult()
    {
        actorStg.Frame();
        ClearDestination();
        return true;
    }

    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        actorStg.Draw( graphDev );

        return true;
    }

    /// 目的地マーカーの描画
    public bool DrawDestinationMark( DemoGame.GraphicsDevice graphDev )
    {
        if( actorDestination.Enable ){
            actorDestination.Draw( graphDev );
        }
        return true;
    }

    /// 衝突対象をコンテナへ登録
    public bool SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        for( int i=0; i<actorStg.GetUseObjNum(); i++ ){
            container.Add( actorStg, actorStg.GetUseObj(i) );
        }
        return true;
    }

    /// 外部から割り込みを受ける対象をコンテナへ登録
    public bool SetinterfereActor( GameActorContainer container )
    {
        return true;
    }

    /// 目的地クリア
    public void ClearDestination()
    {
        destinationFlg          = false;
        actorDestination.Enable = false;
        destinationTrgActor     = null;
    }
		
    public void ClearMakeWall()
    {
		towerPos.Xyz = StaticDataList.getVectorZero();
		effectPos.Xyz = StaticDataList.getVectorZero();
		if(mode == 1){
//			ctrlResMgr.CtrlHobit.SetCtrlStateId(CtrlHobitemy.CtrlStateId.Eat);
//			ctrlResMgr.CtrlCam.SetCamMode( CtrlCamera.ModeId.LookMyself );
		}
		mode = 0;
		TouchCount = 0;
		makeTowerFlag 			= true;
        destinationFlg          = false;
        actorDestination.Enable = false;
        destinationTrgActor     = null;
    }

    /// 目的地が有効かのフラグ
    public bool CheckDestination()
    {
        return destinationFlg;
    }
    /// 目的地の座標がセットされているかのフラグ
    public bool CheckDestinationTrg()
    {
        return actorDestination.Enable;
    }
    /// 目的地の座標を取得
    public Vector3 GetDestinationPos()
    {
        return destinationPos;
    }
    /// 目的地のOBJを取得
    public GameActorProduct GetDestinationActor()
    {
        return destinationTrgActor;
    }
		
	public bool FirstTouch(){
		if(TouchCount == 0){
			return true;						
		}else{
			return false;									
		}	
	}

	public Vector3 TouchPosition(){
		return TouchPostion;
	}
		
	public void setBrightness(float brightness){
		actorStg.SetBrightness(brightness);
	}


/// private メソッド
///---------------------------------------------------------------------------

    /// 目的地セット
    public void setDestination( int scrPosX, int scrPosY )
    {

        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlHobit.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlWall.SetDestinationActor( useCollMgr.TrgContainer );
        useCollMgr.TrgContainer.Add( actorStg, actorStg.GetUseObj(0) );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            /// マーカーのセット
            actorDestination.Start();

            Matrix4 mtx = Matrix4.RotationY( 0 );

            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
            ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();
				
            if( destinationTrgActor.CheckMoveTrgId() == 1 ){
                if( bndSph != null ){
                    destinationPos = bndSph.Sphre.Pos;
                }
                else{
                    destinationPos = destinationTrgActor.BasePos;
                }
                actorDestination.SetType( 0 );
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 2 ){
                destinationPos = destinationTrgActor.BasePos;
                if( bndSph != null ){
                    destinationPos.Y += bndSph.Sphre.R;
                }
                actorDestination.SetType( 0 );
            }
            else{
                destinationPos = calCollLook.NextPos;
                destinationPos.Y += 0.02f;
                actorDestination.SetType( 1 );
            }

            Common.MatrixUtil.SetTranslate( ref mtx, destinationPos );
            actorDestination.SetPlace( mtx );

            actorDestination.Enable = true;
        }
        else{
            actorDestination.Enable = false;
        }
    }
		
    /// 継続タッチのとき　最初に触ったオブジェクトで処理の変化
    public void findingMode( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlHobit.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlWall.SetDestinationActor( useCollMgr.TrgContainer );
        useCollMgr.TrgContainer.Add( actorStg, actorStg.GetUseObj(0) );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
            ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();
				
            if( destinationTrgActor.CheckMoveTrgId() == 1 ){
				mode = 1;
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 2 ){
				mode = 2;
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
			}
			else if( destinationTrgActor.CheckMoveTrgId() == 3 ){
				mode = 3;
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }					
			}
			else{
				TouchPostion = calCollLook.NextPos;	
			}
		}
    }		

    public void findingTower( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
            ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();
				
			if( destinationTrgActor.CheckMoveTrgId() == 2 ){
				mode= 2;
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
			}
			else{
				TouchPostion = calCollLook.NextPos;	
			}
		}
    }		
		
		
    /// 食べる目標決め
    public void setEatTarget( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlHobit.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlWall.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlHouse.SetDestinationActor( useCollMgr.TrgContainer );
		
//        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
//        ctrlResMgr.CtrlWall.SetDestinationActor( useCollMgr.TrgContainer );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            /// マーカーのセット
            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
			ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();

            if( destinationTrgActor.CheckMoveTrgId() == 1 ){
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
				destinationTrgActor.SetEatFlag();
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 2 ){
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
				destinationTrgActor.SetEatFlag();
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 3 ){
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
				destinationTrgActor.SetEatFlag();
            }else if( destinationTrgActor.CheckMoveTrgId() == 4 ){
                if( bndSph != null ){
                    TouchPostion = bndSph.Sphre.Pos;
                }
                else{
                    TouchPostion = destinationTrgActor.BasePos;
                }
				destinationTrgActor.SetEatFlag();
            }else{
				TouchPostion = calCollLook.NextPos;		
			}
        }
    }

    /// 破壊対象
    public void setBreakTarget( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        ctrlResMgr.CtrlHobit.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        ctrlResMgr.CtrlWall.SetDestinationActor( useCollMgr.TrgContainer );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
            /// マーカーのセット
            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
			destinationTrgActor.SetDeadFlag();
            if( destinationTrgActor.CheckMoveTrgId() == 1 ){
				TouchPostion = destinationTrgActor.BasePos;
				destinationTrgActor.SetDeadFlag();
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 2 ){
				TouchPostion = destinationTrgActor.BasePos;
				destinationTrgActor.SetDeadFlag();
            }
            else if( destinationTrgActor.CheckMoveTrgId() == 3 ){
				TouchPostion = destinationTrgActor.BasePos;
				destinationTrgActor.SetDeadFlag();
            }
			else{
				TouchPostion = calCollLook.NextPos;	
			}
        }
    }
		
    /// 敵陣地の拡大
    public void setMonument( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();

        useCollMgr.TrgContainer.Add( actorStg, actorStg.GetUseObj(0) );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );

        if( checkBound ){
			TouchPostion = calCollLook.NextPos;	
			if(ctrlResMgr.CtrlMo.inDistance(TouchPostion) == true){
				ctrlResMgr.CtrlMo.EntryAddMonument(1,0,new Vector3(TouchPostion.X,30.0f,TouchPostion.Z));
				ctrlResMgr.CtrlPl.Addeffect(towerNowPos);
		        AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeMo, towerNowPos );
				//makeEnemyTower

				if(makeEnemyMonument == 0){
						//+-3
					ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3	));
					ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3	));
					ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3	));
					ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3	));
			
					ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3),new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3));
					ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3));
					ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+3),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3));
					ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3),new Vector3((int)Data.SetupValue.EnemyMonumentPosX-3,30.0f,(int)Data.SetupValue.EnemyMonumentPosY-3));
				}else if(makeEnemyMonument == 1){
					int prePosX = 0,prePosY = 0;
					int i=-2;
					int j=-2;
					for(j=-2; j<=2; j++){
						ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						if(j!=-2){
							ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3(prePosX,30.0f,prePosY),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						}
						prePosX = (int)Data.SetupValue.EnemyMonumentPosX+6*i;
						prePosY = (int)Data.SetupValue.EnemyMonumentPosY+6*j;
					}
					
					i=2;
					for(j=-2; j<=2; j++){
						ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						if(j!=-2){
							ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3(prePosX,30.0f,prePosY),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						}
						prePosX = (int)Data.SetupValue.EnemyMonumentPosX+6*i;
						prePosY = (int)Data.SetupValue.EnemyMonumentPosY+6*j;
					}

					j=-2;
					for(i=-2; i<=2; i++){
						ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						if(i!=-2){
							ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3(prePosX,30.0f,prePosY),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						}
						prePosX = (int)Data.SetupValue.EnemyMonumentPosX+6*i;
						prePosY = (int)Data.SetupValue.EnemyMonumentPosY+6*j;
					}

					j=2;
					for(i=-2; i<=2; i++){
						ctrlResMgr.CtrlTo.EntryAddTower((int)Data.Tex2dResId.Bosstower,new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						if(i!=-2){
							ctrlResMgr.CtrlWall.EntryAddWall((int)Data.Tex2dResId.BossWall,new Vector3(prePosX,30.0f,prePosY),new Vector3((int)Data.SetupValue.EnemyMonumentPosX+6*i,30.0f,(int)Data.SetupValue.EnemyMonumentPosY+6*j));
						}
						prePosX = (int)Data.SetupValue.EnemyMonumentPosX+6*i;
						prePosY = (int)Data.SetupValue.EnemyMonumentPosY+6*j;
					}
				}
				makeEnemyMonument++;
				MonumentSetFlag = false;					
			}
        }
    }

		
    /// 壁の生成
    public void makeWall( int scrPosX, int scrPosY )
    {
        GameActorCollManager       useCollMgr    = actorDestination.GetMoveCollManager();

        Vector3 posStart = new Vector3(0,0,0);
        Vector3 posEnd = new Vector3(0,0,0);

        /// チェックする開始座標と終了座標のセット
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 0.0f, ref posStart );
        ctrlResMgr.GraphDev.GetScreenToWorldPos( scrPosX, scrPosY, 1.0f, ref posEnd );


        DemoGame.GeometryLine moveMoveLine = new DemoGame.GeometryLine( posStart, posEnd );

        /// 衝突対象の登録
        useCollMgr.TrgContainer.Clear();
        ctrlResMgr.CtrlTo.SetDestinationActor( useCollMgr.TrgContainer );
        useCollMgr.TrgContainer.Add( actorStg, actorStg.GetUseObj(0) );


        /// 衝突判定
        calCollLook.SetMoveType( Data.CollTypeId.ChDestination );

        bool checkBound = calCollLook.Check( useCollMgr, moveMoveLine );
        if( checkBound ){
            /// マーカーのセット
//            actorDestination.Start();

            Matrix4 mtx = Matrix4.RotationY( 0 );
            destinationTrgActor = useCollMgr.TrgContainer.GetEntryObjParent(0);
            ShapeSphere bndSph  = destinationTrgActor.GetBoundingShape();
			if( destinationTrgActor.CheckMoveTrgId() == 2){	//始点の建物に接触
                towerNowPos = destinationTrgActor.BasePos;
				TouchPostion = destinationTrgActor.BasePos;
				if(towerPos.X != towerNowPos.X && towerPos.Z != towerNowPos.Z && !(towerPos.X == 0.0f &&  towerPos.Y == 0.0f && towerPos.Z == 0.0f)){ //最初のタッチ以外もしくは前のタワー以外との接触の場合
	                if( bndSph != null ){
	                    towerNowPos.Y += bndSph.Sphre.R;
	                }
					towerNowPos.Y += 0.1f;
					ctrlResMgr.AddWall = true;
					ctrlResMgr.AddWallPos1 = towerPos;
					ctrlResMgr.AddWallPos2 = towerNowPos;
					makeTowerFlag = false;
					Console.WriteLine("asdf");
				}
            }else{
				Console.WriteLine("fdsa");
	            towerNowPos = calCollLook.NextPos;
				TouchPostion = calCollLook.NextPos;
				towerNowPos.Y += 0.1f;
				if(towerPos.X == 0.0f &&  towerPos.Y == 0.0f && towerPos.Z == 0.0f){
					towerPos = towerNowPos;
					effectPos = towerNowPos;
					//ctrlResMgr.CtrlTo.EntryAddTower( 1, 0.0f, towerNowPos, (int)StaticDataList.getRandom(0,5));
				}
	            double dis2 = Common.VectorUtil.Distance( towerPos , towerNowPos );
				if(dis2 > newTowerDis){
					ctrlResMgr.AddWall = true;
					ctrlResMgr.AddWallPos1 = towerPos;
					ctrlResMgr.AddWallPos2 = towerNowPos;
							
					ctrlResMgr.AddTower = true;
					ctrlResMgr.AddTowerPos = towerNowPos;
							
					TowerAreaNorth = FMath.Max(TowerAreaNorth , towerNowPos.X);
					TowerAreaSouth = FMath.Min(TowerAreaSouth , towerNowPos.X);
					TowerAreaEast = FMath.Max(TowerAreaEast , towerNowPos.Z);
					TowerAreaWest = FMath.Min(TowerAreaWest , towerNowPos.Z);
	                //AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeMo, towerNowPos );

					towerPos = towerNowPos;
					//sumPos += towerPos;
					//sumcount++;
				}
		        dis2 = Common.VectorUtil.Distance( effectPos , towerNowPos );
				if(dis2 > newEffectDis){
					ctrlResMgr.AddEffectFromEnemy = true;
					ctrlResMgr.AddEnemyEffectPos = towerNowPos;
					effectPos = towerNowPos;					
				}
			}				
		}
	}
}

} // namespace
