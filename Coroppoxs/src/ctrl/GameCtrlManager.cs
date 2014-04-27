/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
//using System.Threading;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;



namespace AppRpg {

///***************************************************************************
/// シングルトン：CTRL間にて使用するリソースの管理
///***************************************************************************
public class GameCtrlManager
{

    private static GameCtrlManager instance = new GameCtrlManager();

    private DemoGame.GraphicsDevice        graphDev;
		

    private CtrlPlayer       		ctrlPl;
    private CtrlHobit       		ctrlHobit;
    private CtrlTower       	 	ctrlTo;
    private CtrlWall         		ctrlWall;
    private CtrlHouse         		ctrlHouse;
    private CtrlCamera      	 	ctrlCam;
    private CtrlStage       	 	ctrlStg;
    private CtrlEffect       		ctrlEffect;
    private CtrlEvent        		ctrlEvent;
    private CtrlDestinationMark 	ctrlDesMark;
    private CtrlMonument 			ctrlMo;
		
    private float            		nowFps;
    private float   		       	nowMs;
	public int				 		countTime;
		
	private int 					EnemyNumber;
	private int 					TowerNumber;
	private int 					WallNumber;
	private int 					HouseNumber;
	private int 					MonumentNumber;
		
	private bool 					EnemyDrawFlag;
	private bool 					TowerDrawFlag;
	private bool					WallDrawFlag;
	private bool 					HouseDrawFlag;
	private bool 					MonumentDrawFlag;
	public bool						EatingFlag;
		
//	private Thread 					threadEn;
//	private Thread 					threadOther;
		
		
	// threadした時用　
	public bool						AddEnemyFromMo;
	public bool						AddHouseFromMo;
	public int						EatCharNumber;
	public bool						changePlAni;
	public bool						changePlMode;
	public bool						eatingBoss;
		
	public bool						AddEnemyFromNecro;
	public Vector3					AddEnemyPos;
	
	public bool						AddEffectFromEnemy;
	public Vector3					AddEnemyEffectPos;

	public bool						AddTower;
	public Vector3					AddTowerPos;

	public bool						AddWall;
	public Vector3					AddWallPos1;
	public Vector3					AddWallPos2;

	public bool						battleStartFlag;
	public int 						SortNumber;
		
		
	public SpriteBatch batch ;
	public SpriteMaterial material ;
	public Sprite[] sprites ;
	public int spriteCount ;
	public float spriteSize = 24.0f ;
	public float spriteSpin = 1.0f ;
	public int spriteCounter = 0;
		
	public int ZonbiNumber;
		
	public Vector3 EnemyMoPos;	//あとで消す
		
	private bool					EatingNow;
	private int						EatingCount;
	private int						EatingStart;
	private bool					ToothVector = true;
		
		
	
	

	/// コンストラクタ
    private GameCtrlManager()
    {
    }

    /// インスタンスの取得
    public static GameCtrlManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {			  
        ctrlPl = new CtrlPlayer();
        ctrlPl.Init();

        ctrlHobit = new CtrlHobit();
        ctrlHobit.Init();		

        ctrlTo = new CtrlTower();
        ctrlTo.Init();

        ctrlWall = new CtrlWall();
        ctrlWall.Init();
			
        ctrlCam = new CtrlCamera();
        ctrlCam.Init();

        ctrlStg = new CtrlStage();
        ctrlStg.Init();

        ctrlEffect = new CtrlEffect();
        ctrlEffect.Init();
        
        ctrlEvent = new CtrlEvent();
        ctrlEvent.Init();
			
		ctrlDesMark = new CtrlDestinationMark();
		ctrlDesMark.Init();
			
		ctrlHouse = new CtrlHouse();
		ctrlHouse.Init();
			
		ctrlMo = new CtrlMonument();
		ctrlMo.Init();

		GameCtrlDrawManager.GetInstance().Init();
					
		EnemyNumber = 0;
		HouseNumber = 0;
		TowerNumber = 0;
		WallNumber = 0;
		MonumentNumber = 0;
		EnemyDrawFlag = false;
		TowerDrawFlag = false;
		WallDrawFlag = false;
		HouseDrawFlag = false;
		EatingFlag = false;
		MonumentDrawFlag = false;
		EatCharNumber = 0;
			
		AddEnemyFromMo = false;
		AddHouseFromMo = false;
		changePlAni = false;
		changePlMode = false;
		eatingBoss = false;
		AddEnemyFromNecro = false;
		battleStartFlag = false;
		AddEffectFromEnemy = false;
		AddTower = false;
			
		ZonbiNumber = 30;
			
		EnemyMoPos = new Vector3(-70.0f, 45.0f, 65.0f);

		countTime = 0;
		SortNumber = 0;
        nowFps = 0.0f;
			
		EatingNow = false;
		EatingCount = 0;
		EatingStart = 20;
			
        return true;
    }

    /// 破棄
    public void Term()
    {
        GameCtrlDrawManager.GetInstance().Term();

        ctrlPl.Term();
        ctrlHobit.Term();
        ctrlTo.Term();
        ctrlWall.Term();
		ctrlCam.Term();
        ctrlStg.Term();
        ctrlEffect.Term();
        ctrlEvent.Term();
		ctrlDesMark.Term();
		ctrlHouse.Term();
		ctrlMo.Term();

        ctrlPl        = null;
        ctrlHobit        = null;
        ctrlTo        = null;
        ctrlWall      = null;
        ctrlCam       = null;
        ctrlStg       = null;
        ctrlEffect    = null;
        ctrlEvent     = null;
		ctrlDesMark	  = null;
		ctrlHouse	  = null;
		ctrlMo		  = null;
        graphDev      = null;
    }


    /// 開始
    public bool Start()
    {
			
        ctrlPl.Start();
        ctrlHobit.Start();
        ctrlTo.Start();
        ctrlWall.Start();
        ctrlCam.Start();
        ctrlStg.Start();
        ctrlEffect.Start();
        ctrlEvent.Start();
		ctrlHouse.Start();
		ctrlMo.Start();
			
		InitSpriteBatch( 4096 , graphDev) ;
		SetSpriteCount( 4096 ) ;
			
//		threadEn.Start();
		Scene2dTex.GetInstance().init(graphDev.Graphics);
		Scene2dTex.GetInstance().SetUpperPos(-400);
		Scene2dTex.GetInstance().SetUnderPos(950);
//		Scene2dTex.GetInstance().SetUpperPos(950);
//		Scene2dTex.GetInstance().SetUnderPos(-400);
			

		
        GameCtrlDrawManager.GetInstance().Start();
        return true;
    }


    /// 終了
    public void End()
    {
        ctrlPl.End();
        ctrlHobit.End();
        ctrlTo.End();
        ctrlWall.End();
        ctrlCam.End();
        ctrlStg.End();
        ctrlEffect.End();
        ctrlEvent.End();
		ctrlDesMark.End();
		ctrlHouse.End();
		ctrlMo.End();
    }


    /// 使用するグラフィクスデバイスの登録
    public void SetGraphicsDevice( DemoGame.GraphicsDevice use )
    {
        graphDev    = use;
    }


    /// FPS
    public void SetFps( float fps )
    {
        nowFps    = fps;
    }
    public float GetFps()
    {
        return nowFps;
    }

    /// ミリ秒
    public void SetMs( float fps )
    {
        nowMs    = fps;
    }
    public float GetMs()
    {
        return nowMs;
    }
		

    /// 全コントロールのUpdate
    public void Frame()
    {	
		/*
		Console.WriteLine(GetMs());
		Console.WriteLine(ctrlHobit.GetEntryNum()+ctrlTo.GetEntryNum()+ctrlHouse.GetEntryNum()+ctrlWall.GetEntryNum());
		Console.WriteLine();
		*/
		countTime++;

		if(changePlAni == true){
			if(changePlMode == false){
//				ctrlPl.changeAnimation(true);								
				changePlMode = true;				
			}else if(changePlMode == true){
//				ctrlPl.changeAnimation(false);						
				changePlMode = false;				
			}
			changePlAni = false;
		}
			
		makeHouseAndEnTimer();
		if(AddEnemyFromMo == true){
			if(CtrlHobit.stateId == CtrlHobit.CtrlStateId.Move){
				AddEnemyFromMonument();
			}
			AddEnemyFromMo = false;
		}
			
		if(AddEnemyFromNecro == true){
			ctrlHobit.EntryAddEnemy( (int)Data.Tex2dResId.Necromancer1, new Vector3(AddEnemyPos.X , 34.0f , AddEnemyPos.Z));			
			AddEnemyFromNecro = false;
		}
			
		if(AddHouseFromMo == true){
			AddHouseFromMonument();	
			AddHouseFromMo = false;
		}
			
		if(battleStartFlag == true){
			battleStart();
			battleStartFlag	= false;	
		}

		if(AddEffectFromEnemy == true){
			ctrlPl.Addeffect(AddEnemyEffectPos);
			AddEffectFromEnemy = false;
		}
			
		if(AddTower == true){
//			ctrlTo.EntryAddTower(StaticDataList.getRandom((int)Data.Tex2dResId.TowerStart+1,(int)(Data.Tex2dResId.TowerMax)),AddTowerPos);
			ctrlTo.EntryAddTower((int)Data.Tex2dResId.SouryoTower1,AddTowerPos);
			AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeMo, AddTowerPos);
			AddTower = false;				
		}
				
		if(AddWall == true){
			CtrlWall.EntryAddWall(StaticDataList.getRandom((int)Data.Tex2dResId.WallStart+1,(int)Data.Tex2dResId.WallMax),AddWallPos1, AddWallPos2);	
			AddWall = false;				
		}
			
		if(EatCharNumber >0){
			Console.WriteLine(EatCharNumber);
			Scene2dTex.GetInstance().AddHp(EatCharNumber);
				switch(EatCharNumber){
				case (int)Data.Tex2dResId.Noumin1: 		CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.Senshi1: 		CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.Souryo1: 		CtrlPl.Hp = CtrlPl.Hp + 2;	break;
				case (int)Data.Tex2dResId.Zonbi1: 		CtrlPl.Hp = CtrlPl.Hp + 1;	ctrlPl.Poision += 70; break;
				case (int)Data.Tex2dResId.Necromancer1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
					
				case (int)Data.Tex2dResId.TowerStart:	CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiTower1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiTower2: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoTower1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoTower2: CtrlPl.Hp = CtrlPl.Hp + 1;	break;

				case (int)Data.Tex2dResId.NouminHouse1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.NouminHouse2: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.NouminHouse3: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.NouminHouse4: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.NouminHouse5: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiHouse1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiHouse2: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiHouse3: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiHouse4: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SenshiHouse5: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoHouse1: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoHouse2: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoHouse3: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoHouse4: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
				case (int)Data.Tex2dResId.SouryoHouse5: CtrlPl.Hp = CtrlPl.Hp + 1;	break;
					
				case (int)Data.Tex2dResId.Gareki:		CtrlPl.Hp = CtrlPl.Hp + 0.5f;	break;
				case (int)Data.Tex2dResId.GarekiWall:	CtrlPl.Hp = CtrlPl.Hp + 0.5f;	break;
				case (int)Data.Tex2dResId.MakingWall1:	CtrlPl.Hp = CtrlPl.Hp + 0.5f;	break;
				case (int)Data.Tex2dResId.MakingWall2:	CtrlPl.Hp = CtrlPl.Hp + 0.5f;	break;
				case (int)Data.Tex2dResId.MakingWall3:	CtrlPl.Hp = CtrlPl.Hp + 0.5f;	break;
			}
			EatCharNumber = 0;
		}

		switch(SortNumber){
			case 0:	ctrlTo.setActiveChList();		ctrlTo.SortNear();			break;
			case 1: ctrlWall.setActiveChList();		ctrlWall.SortNear();		break;
			case 2:	ctrlHouse.setActiveChList();	ctrlHouse.SortNear();		break;
			case 3:	ctrlMo.setActiveChList();		ctrlMo.SortNear();			break;
			case 4: ctrlHobit.setActiveChList();	ctrlHobit.SortNear();		break;
		}			

		SortNumber++;
		if(SortNumber > 8) SortNumber = 0;
			
			/*
		threadEn = new Thread(new ThreadStart(ThreadEnFrame));
		threadOther = new Thread(new ThreadStart(ThreadOtherFrame));
		threadEn.Start();	
		threadOther.Start();
			*/
			
		
        ctrlHobit.Frame();
        ctrlTo.Frame();
        ctrlWall.Frame();
		ctrlHouse.Frame();
        ctrlStg.Frame();
        ctrlEffect.Frame();
		ctrlMo.Frame();			
        ctrlCam.Frame();
        ctrlStg.Frame();
        ctrlEffect.Frame();
		ctrlPl.Frame();
		FrameEat();
		Scene2dTex.GetInstance().SetHp(CtrlPl.Hp);
		Scene2dTex.GetInstance().frame();
			
			
		if(countTime % (int)Data.SetupValue.TweetTime == 0){
			//SocketSample.GetInstance().sendData(StaticDataList.getRandom(1,5),StaticDataList.getRandom(0,10));
		}	
    }


    /// 全コントロールの描画
    public void Draw()
    {
		graphDev.Graphics.SetTexture(0, StaticDataList.textureUnified);	
			
        ctrlCam.Draw( graphDev );
		
		if(countTime <= (int)Data.SetupValue.EnemyAppearTime-50){
			ctrlStg.setBrightness(0.0f);
		}else if((int)Data.SetupValue.EnemyAppearTime-50 < countTime && countTime <= (int)Data.SetupValue.EnemyAppearTime){
			ctrlStg.setBrightness(1.0f - ((int)Data.SetupValue.EnemyAppearTime - countTime)/50.0f);
		}else if((int)Data.SetupValue.EnemyAppearTime < countTime && countTime <= (int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime-50){
			ctrlStg.setBrightness(1.0f);
		}else if((int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime-50 < countTime && countTime < (int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime){
			ctrlStg.setBrightness((((int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime)- countTime)/50.0f);				
		}else if((int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime < countTime && countTime < (int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime+200){
			ctrlStg.setBrightness(0.0f);	
		}else if((int)Data.SetupValue.EnemyAppearTime + (int)Data.SetupValue.EnemyAppearingTime+200 < countTime){
			countTime = 0;		
		}			
		ctrlStg.Draw( graphDev );
			
//		int allNumber = ctrlMo.GetActiveNum() + ctrlHobit.GetActiveNum() + ctrlHouse.GetActiveNum() + ctrlTo.GetActiveNum() + ctrlWall.GetActiveNum() + Scene2dTex.GetInstance().GetObjNumber();
		int allNumber = ctrlMo.GetActiveNum() + ctrlHobit.GetActiveNum() + CtrlHobit.speakCount + ctrlHouse.GetActiveNum() + ctrlTo.GetActiveNum() + ctrlWall.GetActiveNum() +1;
		if(allNumber > 4096)　allNumber = 4096;
		SetSpriteCount(allNumber) ;	
						
		if(eatingBoss == true){
			ctrlPl.Draw(graphDev);
		}
	
		ClearSpriteCounter();

		while(true){
			SetDrawFlag();
				
			if(EnemyDrawFlag == true){
				ctrlHobit.DrawIdx( graphDev , EnemyNumber);
				EnemyNumber++;
				EnemyDrawFlag = false;
			}
				
			if(TowerDrawFlag == true){
			   	ctrlTo.DrawIdx( graphDev , TowerNumber);
				TowerNumber++;
				TowerDrawFlag = false;

			}
				
			if(HouseDrawFlag == true){
		       	ctrlHouse.DrawIdx( graphDev , HouseNumber);
				HouseNumber++;
				HouseDrawFlag = false;
			}
				
			if(MonumentDrawFlag == true){
				ctrlMo.DrawIdx( graphDev , MonumentNumber);
				MonumentNumber++;
				MonumentDrawFlag = false;
			}
				
			if(WallDrawFlag == true){
		       	ctrlWall.DrawIdx( graphDev , WallNumber);
				WallNumber++;
				WallDrawFlag = false;
			}
				
				
				
			if((EnemyNumber == ctrlHobit.GetActiveNum() || ctrlHobit.GetActiveNum() == 0) && 
				(TowerNumber == ctrlTo.GetActiveNum() || ctrlTo.GetActiveNum() == 0) && 
				(HouseNumber == ctrlHouse.GetActiveNum() || ctrlHouse.GetActiveNum() == 0) && 
				(WallNumber == ctrlWall.GetActiveNum() || ctrlWall.GetActiveNum() == 0) &&
				(MonumentNumber == ctrlMo.GetActiveNum() || ctrlMo.GetActiveNum() == 0) ){

				EnemyNumber = 0;
				TowerNumber = 0;
				HouseNumber = 0;
				WallNumber = 0;
				MonumentNumber = 0;
				EnemyDrawFlag = false;
				TowerDrawFlag = false;
				HouseDrawFlag = false;			
				WallDrawFlag = false;
				MonumentDrawFlag = false;
				break;
			}
		}
		batch.Draw(graphDev) ;	
		
		ctrlHobit.DrawText(graphDev);
		
		Scene2dTex.GetInstance().Draw();
				
		for ( int i = 0 ; i < spriteCount ; i ++ ) {
			sprites[ i ].UpdatePosTex();
		}
		
			
		/*
		for(int i=0;i<spriteCount;i++){
			sprites[i].UpdateAll();
		}
		spriteCount = 8000;			
		*/
        /// 半透明の物は奥から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
		if(eatingBoss == false){
			ctrlPl.Draw(graphDev);
		}
		ctrlEffect.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortFar();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

			
    }
		

	private void ThreadEnFrame(){
        ctrlHobit.Frame();
	}
		
	private void ThreadOtherFrame(){
        ctrlTo.Frame();
        ctrlWall.Frame();
		ctrlHouse.Frame();
        ctrlStg.Frame();
        ctrlEffect.Frame();
		ctrlMo.Frame();
	}


    /// タイトル用Update
    public void FrameTitle()
    {
        ctrlCam.FrameTitle();
        ctrlStg.FrameTitle();
    }


    /// タイトル用の描画
    public void DrawTitle()
    {
        ctrlCam.Draw( graphDev );
			
/*			
        /// 不透明の物は手前から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlFix.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortNear();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );
*/
        ctrlStg.Draw( graphDev );
    }


    /// デバック用の描画
    public void DrawDebug()
    {
        ctrlCam.Draw( graphDev );

        /// 不透明の物は手前から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlHobit.DrawDebug( graphDev );
        ctrlPl.Draw( graphDev );

        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlStg.Draw( graphDev );

        /// 半透明の物は奥から描く
        GameCtrlDrawManager.GetInstance().EntryStart();
        ctrlPl.DrawAlpha( graphDev );
        ctrlHobit.DrawAlpha( graphDev );
        ctrlEffect.Draw( graphDev );
        GameCtrlDrawManager.GetInstance().SortFar();
        GameCtrlDrawManager.GetInstance().Draw( graphDev );

        ctrlEvent.Draw( graphDev );
    }


    /// リザルト用Update
    public void FrameResult()
    {
        ctrlPl.FrameResult();
        ctrlHobit.Frame();
        ctrlCam.FrameResult();
        ctrlStg.FrameResult();
        ctrlEffect.Frame();
        ctrlEvent.Frame();

    }


    /// 衝突対象をコンテナにセット
    public void SetCollisionActor( GameActorCollObjContainer container, Vector3 trgPos )
    {
        container.Clear();

		ctrlStg.SetCollisionActor( container, trgPos );
        ctrlHobit.SetCollisionActor( container, trgPos );
        ctrlTo.SetCollisionActor( container, trgPos );
        ctrlWall.SetCollisionActor( container, trgPos );
		ctrlMo.SetCollisionActor( container, trgPos );
		ctrlHouse.SetCollisionActor( container, trgPos );
    }

    /// 衝突対象をコンテナにセット
    public void SetCollisionActorEn( GameActorCollObjContainer container, Vector3 trgPos )
    {
        container.Clear();
		ctrlStg.SetCollisionActor( container, trgPos );
    }

    /// プレイヤーの干渉対象をコンテナにセット
    public void SetinterfereActorPl( GameActorContainer container )
    {
        container.Clear();
        ctrlHobit.SetinterfereActor( container );
        ctrlTo.SetinterfereActor( container );
        ctrlWall.SetinterfereActor( container );
        ctrlHouse.SetinterfereActor( container );
        ctrlMo.SetinterfereActor( container );
    }
		
	private void AddHouseFromMonument()
	{
		int disNS = (int)(CtrlStg.TowerAreaNorth - CtrlStg.TowerAreaSouth);
		int disEW = (int)(CtrlStg.TowerAreaEast - CtrlStg.TowerAreaWest);

		CtrlHouse.EntryAddHouse((int)(StaticDataList.getRandom((int)Data.Tex2dResId.HouseStart+2, (int)Data.Tex2dResId.HouseMax)),
					new Vector3(StaticDataList.getRandom((int)CtrlStg.TowerAreaSouth-disNS/2,(int)CtrlStg.TowerAreaNorth+disNS/2),
		            50.0f,
		            StaticDataList.getRandom((int)CtrlStg.TowerAreaWest-disEW/2,(int)CtrlStg.TowerAreaEast+disEW/2))
		            );
	}	

	private void AddEnemyFromMonument()
	{
//		int makeEnemy = 4+(int)Data.SetupValue.BonusNewMoNewEnemy*(ctrlMo.GetEntryNum()-1);
//		int tempRandNumber = StaticDataList.getRandom(0,makeEnemy);
		int tempRandNumber = 1;
		for(int i=0; i< tempRandNumber; i++){
			int moNumber = StaticDataList.getRandom(0,ctrlMo.GetEntryNum());
			ctrlHobit.EntryAddEnemy(StaticDataList.getRandom((int)Data.Tex2dResId.CharStart+1, (int)Data.Tex2dResId.NormalCharMax),new Vector3(ctrlMo.GetPos(0).X,34.0f,ctrlMo.GetPos(0).Z));
		}
	}

	private void SetDrawFlag(){
		if(ctrlHobit.Distance(EnemyNumber) < ctrlTo.Distance(TowerNumber)){
			if(ctrlTo.Distance(TowerNumber) < ctrlHouse.Distance(HouseNumber)){
				if(ctrlHouse.Distance(HouseNumber) < ctrlWall.Distance(WallNumber)){
					if(ctrlWall.Distance(WallNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlWall.Distance(WallNumber) >= ctrlMo.Distance(MonumentNumber)){							
						WallDrawFlag = true;
					}
				}else if(ctrlHouse.Distance(HouseNumber) >= ctrlWall.Distance(WallNumber)){
					if(ctrlHouse.Distance(HouseNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlHouse.Distance(HouseNumber) >= ctrlMo.Distance(MonumentNumber)){							
						HouseDrawFlag = true;
					}
				}
			}else if(ctrlTo.Distance(TowerNumber) >= ctrlHouse.Distance(HouseNumber)){
				if(ctrlTo.Distance(TowerNumber) < ctrlWall.Distance(WallNumber)){
					if(ctrlWall.Distance(WallNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlWall.Distance(WallNumber) >= ctrlMo.Distance(MonumentNumber)){							
						WallDrawFlag = true;
					}
				}else if(ctrlTo.Distance(TowerNumber) >= ctrlWall.Distance(WallNumber)){
					if(ctrlTo.Distance(TowerNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlTo.Distance(TowerNumber) >= ctrlMo.Distance(MonumentNumber)){							
						TowerDrawFlag = true;
					}
				}
			}
		}else if(ctrlHobit.Distance(EnemyNumber) >= ctrlTo.Distance(TowerNumber)){
			if(ctrlHobit.Distance(EnemyNumber) < ctrlHouse.Distance(HouseNumber)){
				if(ctrlHouse.Distance(HouseNumber) < ctrlWall.Distance(WallNumber)){
					if(ctrlWall.Distance(WallNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlWall.Distance(WallNumber) >= ctrlMo.Distance(MonumentNumber)){							
						WallDrawFlag = true;
					}
				}else if(ctrlHouse.Distance(HouseNumber) >= ctrlWall.Distance(WallNumber)){
					if(ctrlHouse.Distance(HouseNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlHouse.Distance(HouseNumber) >= ctrlMo.Distance(MonumentNumber)){							
						HouseDrawFlag = true;
					}
				}
			}else if(ctrlHobit.Distance(EnemyNumber) >= ctrlHouse.Distance(HouseNumber)){
				if(ctrlHobit.Distance(EnemyNumber) < ctrlWall.Distance(WallNumber)){
					if(ctrlWall.Distance(WallNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlWall.Distance(WallNumber) >= ctrlMo.Distance(MonumentNumber)){							
						WallDrawFlag = true;
					}
				}else if(ctrlHobit.Distance(EnemyNumber) >= ctrlWall.Distance(WallNumber)){
					if(ctrlHobit.Distance(EnemyNumber) < ctrlMo.Distance(MonumentNumber)){
						MonumentDrawFlag = true;
					}else if(ctrlHobit.Distance(EnemyNumber) >= ctrlMo.Distance(MonumentNumber)){							
						EnemyDrawFlag = true;
					}
				}
			}
		}
	} 
		
	private bool makeHouseAndEnTimer()
	{
		if(countTime % (int)Data.SetupValue.NewMonumentAppearTime == 0){
			if( CtrlHouse.GetEntryNum() < ctrlHobit.GetEntryNum() * (int)Data.SetupValue.BonusNewMoHouseLimit/10.0f*ctrlMo.GetEntryNum()){
				AddHouseFromMo = true;
			}
			if( ctrlHobit.GetEntryNum() < CtrlTo.GetEntryNum()*(int)Data.SetupValue.NewEnemyMultipleTower/10.0f * (int)Data.SetupValue.BonusNewMoEnemyLimit/10.0f*ctrlMo.GetEntryNum()){
				AddEnemyFromMo = true;
			}
			/*
			countTime = (int)Data.SetupValue.NewMonumentAppearTime + 
									StaticDataList.getRandom(-(int)(Data.SetupValue.NewMonumentAppearTime)/3,(int)(Data.SetupValue.NewMonumentAppearTime)/3);
									*/
		}
			
		return true;
	}
		
	private void battleStart(){
//		int number = ctrlHobit.GetEntryNum();
		for(int i=0; i<ZonbiNumber; i++){
	        ctrlHobit.EntryAddEnemy( (int)Data.Tex2dResId.Zonbi1,new Vector3(EnemyMoPos.X + StaticDataList.getRandom(-100,100)/10.0f, EnemyMoPos.Y, EnemyMoPos.Z + StaticDataList.getRandom(-100,100)/10.0f));
		}
//		ctrlHobit.EntryAddEnemy( (int)Data.Tex2dResId.Necromancer1,new Vector3(-70 + StaticDataList.getRandom(-100,100)/10.0f, 45.0f, 65 + StaticDataList.getRandom(-100,100)/10.0f));
	}
		
		
	public void InitSpriteBatch( int maxSpriteCount , DemoGame.GraphicsDevice graphDev )
	{
		batch = new SpriteBatch( graphDev.Graphics, maxSpriteCount ) ;
		material = new SpriteMaterial( new Texture2D( "/Application/res/data/2Dtex/unifiedTexture.png", true ) ) ;
		material.Texture.SetFilter( TextureFilterMode.Linear, TextureFilterMode.Linear,
									TextureFilterMode.Nearest ) ;

		sprites = new Sprite[ maxSpriteCount ] ;
		spriteCount = 0 ;
	}

	public void SetSpriteCount( int count )
	{
		count = Math.Min( Math.Max( count, 1 ), sprites.Length ) ;
		if ( count > spriteCount ) {
			for ( int i = spriteCount ; i < count ; i ++ ) {
				sprites[ i ] = new Sprite( batch, material, 0 ) ;
				InitSprite( sprites[ i ] ) ;
			}
		} else {
			for ( int i = count ; i < spriteCount ; i ++ ) {
				batch.RemoveSprite( sprites[ i ] ) ;
			}
		}
		spriteCount = count ;
	}

	public void SetSpriteSize( float size )
	{
		size = FMath.Clamp( size, 4.0f, 512.0f ) ;
		for ( int i = 0 ; i < spriteCount ; i ++ ) {
			sprites[ i ].Size.X = sprites[ i ].Size.Y = size ;
		}
		spriteSize = size ;
	}

	public void InitSprite( Sprite s )
	{
		s.Position.X = 0;
		s.Position.Y = 0;
		s.Direction.X = 0;
		s.Direction.Y = 0;
		s.Size = new Vector2( spriteSize, spriteSize ) ;
		s.Center = new Vector2( 0.5f, 0.5f ) ;
		s.UVOffset = new Vector2( 0.25f, 0.25f ) ;
		s.UVSize = new Vector2( 0.25f, 0.25f ) ;
		s.Color = new Rgba( 255, 255, 255, 255 ) ;

		s.UpdateAll() ;
	}
		
	public void SetSpriteData(Vector3 pos, Vector3 rot, Vector2 UVpos, Vector2 UVsize, Vector2 size)
	{
		if(spriteCounter > spriteCount) return;			
			
		sprites[spriteCounter].Position = pos;
		sprites[spriteCounter].Direction = rot;
		sprites[spriteCounter].UVOffset = UVpos;
		sprites[spriteCounter].UVSize = UVsize;
		sprites[spriteCounter].Size = size;
		sprites[spriteCounter].updataFlag = true;
		spriteCounter++;
	}
		
	public void ClearSpriteCounter(){
		spriteCounter = 0;			
	}
		
	private void FrameEat(){
			
		if(	(CtrlCam.camModeId == CtrlCamera.ModeId.NormalToCloseLook || CtrlCam.camModeId == CtrlCamera.ModeId.CloseLook) && EatingStart > 0){
			EatingStart--;
			if(ToothVector == true){
				Scene2dTex.GetInstance().SetUpperPos(-200-EatingStart*20);
				Scene2dTex.GetInstance().SetUnderPos(750+EatingStart*20);
			}else{
				Scene2dTex.GetInstance().SetUpperPos(750+EatingStart*20);
				Scene2dTex.GetInstance().SetUnderPos(-200-EatingStart*20);
			}
		}
			
		if(	( CtrlCam.camModeId == CtrlCamera.ModeId.Normal ) && EatingStart < 10){
			EatingStart++;
			if(ToothVector == true){
				Scene2dTex.GetInstance().SetUpperPos(-200-EatingStart*20);
				Scene2dTex.GetInstance().SetUnderPos(750+EatingStart*20);
			}else{
				Scene2dTex.GetInstance().SetUpperPos(750+EatingStart*20);
				Scene2dTex.GetInstance().SetUnderPos(-200-EatingStart*20);
			}
		}			
//		Scene2dTex.GetInstance().SetUpperPos(-400);
//		Scene2dTex.GetInstance().SetUnderPos(950);			
		if(	CtrlCam.camModeId == CtrlCamera.ModeId.CloseLook || 
			CtrlCam.camModeId == CtrlCamera.ModeId.NormalToCloseLook ||
			CtrlCam.camModeId == CtrlCamera.ModeId.CloseLookToNormal){
			if(EatingFlag == true || EatingNow == true){
				EatingCount++;
				EatingFlag = false;
				EatingNow = true;
				if(EatingCount < 5){
					if(ToothVector == true){
						Scene2dTex.GetInstance().SetUpperPos(-200+EatingCount*300/5);
						Scene2dTex.GetInstance().SetUnderPos(750-EatingCount*300/5);
					}else{
						Scene2dTex.GetInstance().SetUpperPos(750-EatingCount*300/5);
						Scene2dTex.GetInstance().SetUnderPos(-200+EatingCount*300/5);
					}
				}else if(5 <= EatingCount && EatingCount < 10){
					if(ToothVector == true){
						Scene2dTex.GetInstance().SetUpperPos(100-(EatingCount-5)*300/5);
						Scene2dTex.GetInstance().SetUnderPos(450+(EatingCount-5)*300/5);
					}else{
						Scene2dTex.GetInstance().SetUpperPos(450+(EatingCount-5)*300/5);
						Scene2dTex.GetInstance().SetUnderPos(100-(EatingCount-5)*300/5);
					}
				}
				if(EatingCount == 10){
					if(ToothVector == true){
						Scene2dTex.GetInstance().SetUpperPos(-200-EatingStart*20);
						Scene2dTex.GetInstance().SetUnderPos(750+EatingStart*20);
					}else{
						Scene2dTex.GetInstance().SetUpperPos(750+EatingStart*20);
						Scene2dTex.GetInstance().SetUnderPos(-200-EatingStart*20);
					}
					EatingCount = 0;
					EatingNow = false;
					EatingFlag = false;
				}					
			}
		}			
		
		if(eatingBoss == true){
			if(ToothVector == true){
				Scene2dTex.GetInstance().SetUpperPos(950);
				Scene2dTex.GetInstance().SetUnderPos(-400);
			}else{
				Scene2dTex.GetInstance().SetUpperPos(-400);
				Scene2dTex.GetInstance().SetUnderPos(950);
			}
		}					
			
	}
		
				
		
/// プロパティ
///---------------------------------------------------------------------------

    /// 
    public DemoGame.GraphicsDevice GraphDev
    {
        get {return graphDev;}
    }

    /// 
    public CtrlPlayer CtrlPl
    {
        get {return ctrlPl;}
    }
    public CtrlHobit CtrlHobit
    {
        get {return ctrlHobit;}
    }
    public CtrlTower CtrlTo
    {
        get {return ctrlTo;}
    }
    public CtrlWall CtrlWall
    {
        get {return ctrlWall;}
    }
    public CtrlCamera CtrlCam
    {
        get {return ctrlCam;}
    }
    public CtrlStage CtrlStg
    {
        get {return ctrlStg;}
    }
    public CtrlEffect CtrlEffect
    {
        get {return ctrlEffect;}
    }
    public CtrlEvent CtrlEvent
    {
        get {return ctrlEvent;}
    }
	public CtrlHouse CtrlHouse
	{
		get{return ctrlHouse;}			
	}
    public CtrlDestinationMark CtrlDestinationMark
    {
        get {return ctrlDesMark;}
    }		
    public CtrlMonument CtrlMo
    {
        get {return ctrlMo;}
    }		
		
}

} // namespace
