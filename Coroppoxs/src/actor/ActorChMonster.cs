/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;


namespace AppRpg {

///***************************************************************************
/// ACTOR : モンスターの操作
///***************************************************************************
public class ActorChMonster : ActorChBase
{
    private const int hpMax = 3;

    private ObjChMonster            objCh;
    private ObjSpeechballoon        objSpeech;
    private int                     deadCnt;
    public  bool                    ActiveFlg;
    public  int                     ActiveCnt;
    public  float                   ActiveDis;
	public  float					ActiveRot;
		
    public  int                     AiAttackCnt;
	public	int						AiMoveCount;
	public 	float					AiMoveRot;
	public 	int						TexId;
	public  bool					MovedFlag;
	public bool						FrameFlag;
	private int						eatingTime;
	GameCtrlManager           		ctrlResMgr    = GameCtrlManager.GetInstance();	

/// 継承メソッド
///---------------------------------------------------------------------------
	public ActorChMonster(int TexId){
		this.TexId = TexId;
	}
		
	
    /// 初期化
    protected override bool DoInit()
    {
        objCh = new ObjChMonster(TexId);
        objSpeech = new ObjSpeechballoon(TexId ,0);
        objCh.Init();
		if(TexId != 4){
	        objSpeech.Init();
		}
		MovedFlag = false;
		FrameFlag = true;

        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        if( objCh != null ){
            objCh.Term();
        }
        if( objSpeech != null ){
            objSpeech.Term();
        }

		objCh    = null;
        objSpeech    = null;
    }

    /// 開始
    protected override bool DoStart()
    {
        objCh.Start();
		if(TexId != 4){
			objSpeech.Start();
		}
	
        ActiveFlg    = false;
        ActiveCnt    = 0;
        ActiveDis    = 0;
        AiAttackCnt  = 0;
		AiMoveCount	 = (int)(Data.SetupValue.CharMoveChangeTime + (int)StaticDataList.getRandom(-10,10));
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        ActiveFlg    = false;
//        mvtHdl.End();
        objCh.End();
		if(TexId != 4){
			objSpeech.End();
		}
    }
		
	public void setdeadFlag(bool flag){
		objCh.deadflag = flag;		
	}

    /// フレーム処理
    protected override bool DoFrame()
    {
		if(statePlayTask == 0){
			if(objCh.deadflag == true){
				ChangeState( StateId.Dead );					
			}
			else if(objCh.eatFlag == true){
				ChangeState( StateId.Eat );
			}
		}
		
	
		switch( stateIsPlayId ){
			case StateId.Move:		statePlayMove();        break;	 
	        case StateId.Dead:      statePlayDead();        break;
			case StateId.Eat:		statePlayEat();			break;
        }
			
		if(stateIsPlayId != StateId.Eat){
	        unitCmnPlay.Frame();
			if( objCh.firstFlag == true){
			    unitCmnPlay.FrameGravity2d(ref objCh.firstFlag);        /// 敵は衝突判定を行わない
			}
				/// OBJの姿勢を更新
	        if( unitCmnPlay.IsUpdateMtx() ){
	            updateMatrix( unitCmnPlay.Mtx );
	        }				
		}
			
/*			
		if(unitCmnPlay.GetTouchGroundType() != 0){
			BasePos.Y += 3.0f;				
		}
			
		if(BasePos.Y < 10.0f) Enable = false;
*/	
		objCh.Frame();

		if(TexId != 4){	
	        objSpeech.Frame();
		}
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCh.Draw2( graphDev ,BasePos );
		if(TexId != 4){
			if(objSpeech.deadflag == false){
		        objSpeech.Draw2( graphDev ,BasePos );
			}
		}
        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objCh;
    }
    protected override int DoGetUseObjNum()
    {
        return 1;
    }

    /// 姿勢の更新
    protected override void DoSetMatrix( Matrix4 mtx )
    {
        unitCmnPlay.SetPlace( mtx );
        updateMatrix( mtx );
    }

    /// 境界ボリュームの取得
    protected override ShapeSphere DoGetBoundingShape()
    {
        return objCh.GetBoundSphere();
    }
		
		


/// アクタイベント
///---------------------------------------------------------------------------

     /// 対象の座標へ振り向く
    public override void SetEventTurnPos( Vector3 trgPos, int rot )
    {
        float trgRot = Common.MatrixUtil.GetPointRotY( BaseMtx, BasePos, trgPos );

        if( trgRot > (float)rot ){
            unitCmnPlay.SetTurn( (float)rot, 1 );
        }
        else if( trgRot < (float)-rot ){
            unitCmnPlay.SetTurn( (float)-rot, 1 );
        }
        else {
            unitCmnPlay.SetTurn( trgRot, 1 );
        }
    }




/// public メソッド
///---------------------------------------------------------------------------

    /// モデルの登録
    public void SetMdlHandle( Data.ChTypeId chTypeId )
    {
        this.chTypeId = chTypeId;
        objCh.SetMdlHandle( chTypeId );
    }

    /// 体の中心座標の取得
    public Vector3 GetBodyPos()
    {
        return objCh.BodyPos;
    }

    /// 体の半径取得
    public float GetBodyWidth()
    {
        float width = 1.0f;
        return width;
    }
		
	public void setAppearCountCh(int count){
		objCh.appearCount = count;
	}

	public void setAppearCountSp(int count){
		objSpeech.appearCount = count;
		objSpeech.deadflag = false;
		objSpeech.deadflag2 = false;
		objSpeech.timeCount = 0;
	}

	public void setDeadSp(){
		objSpeech.deadflag2 = true;
	}
	
	public bool deadFlagPl{
		get{return this.objCh.deadflag;}		
	}

	public bool deadFlagSp{
		get{return this.objSpeech.deadflag; }		
	}

/// private メソッド
///---------------------------------------------------------------------------

    /// 姿勢の更新
    private void updateMatrix( Matrix4 mtx )
    {
        BaseMtx        = mtx;
        Common.VectorUtil.Set( ref BasePos, mtx );

        objCh.SetMatrix( mtx );
    }

	public void setAppearCount(int count){
		objCh.appearCount = count;
		objCh.changeFlag = false;
	}
	
	public bool changeFlag{
		set{this.objCh.changeFlag = value;}
		get{return objCh.changeFlag;}
	}

	public bool eatFlag{
		get{return this.objCh.eatFlag;}		
	}
		
	public bool moveAngle
	{
		set{this.objCh.moveAngle = value;}
		get{return this.objCh.moveAngle;}
	}
		
	public bool deadFlag{
		set{this.deadFlag = value;}
		get{return this.objCh.deadflag;}
	}		
		
		
    /// 立ち
    private bool statePlayStand()
    {
		unitCmnPlay.SetStand2d();
        return true;
    }

    /// 歩く
    private bool statePlayMove()
    {
        /// 移動
        if( movePow != 0.0f ){
            unitCmnPlay.SetMove2d( moveVec, movePow );
        }

        /// 旋回
        if( moveTurn != 0.0f ){
            unitCmnPlay.SetRot( moveTurn );
        }

        movePow     = 0.0f;
        moveTurn    = 0.0f;
        
        return true;
    }

    /// 旋回
    private bool statePlayTurn()
    {
        float turnRot = 0.0f;
        switch(chTypeId){
        case Data.ChTypeId.MonsterA:        turnRot = 3.0f;            break;
        case Data.ChTypeId.MonsterB:        turnRot = 5.0f;            break;
        case Data.ChTypeId.MonsterC:        turnRot = 2.0f;            break;
        }

        setTurnRot( turnRot );

        if( moveTurn > turnRot*6 || moveTurn < -turnRot*6 ){
            if( chTypeId ==  Data.ChTypeId.MonsterC ){
//                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Turn, false );
            }
            else{
//                mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
            }
        }
        else{
//            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
        }

        return true;
    }


    /// 攻撃
    private bool statePlayAttack()
    {
        switch(statePlayTask){

        /// 攻撃セット
        case 0:
//            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.AttackLR, false );
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
/*            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
            }*/
            break;
        }

        return true;
    }


    /// ダメージ
    private bool statePlayDamage()
    {
        switch(statePlayTask){

        /// ダメージセット
        case 0:
//            mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Damage, false );

            /// 攻撃対象の方向へ向く
            if( dmgTrgObj != null ){
				Vector4 x = new Vector4(0,0,0,0);
				Vector4 y = new Vector4(0,0,0,0);
				Vector4 z = new Vector4(0,0,0,0);
				Vector4 w = new Vector4(0,0,0,0);
	            Matrix4 mtx = new Matrix4(x ,y, z, w);
                Vector3 vec = new Vector3( (dmgTrgObj.Mtx.M41 - objCh.Mtx.M41), 0.0f, (dmgTrgObj.Mtx.M43 - objCh.Mtx.M43) );
                Common.MatrixUtil.LookTrgVec( ref mtx, vec ); 
                Common.MatrixUtil.SetTranslate( ref mtx, BasePos );
                this.SetPlace( mtx );
            }

            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
//            if( mvtHdl.IsActive() == false ){
                switch(chTypeId){
                case Data.ChTypeId.MonsterA:        AiAttackCnt -= 10;      break;
                case Data.ChTypeId.MonsterB:        AiAttackCnt -= 5;       break;
                case Data.ChTypeId.MonsterC:        AiAttackCnt = 0;        break;
                }

                ChangeState( StateId.Stand );
//            }
            break;
        }

        return true;
    }



    /// 死亡
    private bool statePlayDead()
    {
        Vector3 effPos = StaticDataList.getVectorZero();

        switch(statePlayTask){

        /// 死亡セット
        case 0:
 //           mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Dead, false );

            /// 攻撃対象の方向へ向く
            if( dmgTrgObj != null ){
				Vector4 x = new Vector4(0,0,0,0);
				Vector4 y = new Vector4(0,0,0,0);
				Vector4 z = new Vector4(0,0,0,0);
				Vector4 w = new Vector4(0,0,0,0);
	            Matrix4 mtx = new Matrix4(x ,y, z, w);
                Vector3 vec;
                vec.X = dmgTrgObj.Mtx.M41 - objCh.Mtx.M41;
                vec.Y = 0.0f;
                vec.Z = dmgTrgObj.Mtx.M43 - objCh.Mtx.M43;
                Common.MatrixUtil.LookTrgVec( ref mtx, vec ); 
                Common.MatrixUtil.SetTranslate( ref mtx, BasePos );
                this.SetPlace( mtx );
            }
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
//            if( mvtHdl.IsActive() == false ){

                //Common.VectorUtil.Set( ref effPos, objCh.BodyPos.X, objCh.BodyPos.Y, objCh.BodyPos.Z );
/*
                switch(chTypeId){
                case Data.ChTypeId.MonsterA:        effPos.Y -= 0.6f;            break;
                case Data.ChTypeId.MonsterB:        effPos.Y -= 0.1f;            break;
                case Data.ChTypeId.MonsterC:        effPos.Y -= 0.1f;            break;
                }
*/

//                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, effPos );

                AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ObjBreak, BasePos );
                deadCnt = 0;
                statePlayTask ++;
            break;

        /// エフェクト余韻
        case 2:
			deadCnt++;
			if( deadCnt >= 15 ){
                Enable = false;
            }
            break;
        }

        return true;
	}

    /// 食べられる
    private bool statePlayEat()
    {
		if(objCh.TexId != 4){
			if(ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.CloseLook || ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.CloseLookToNormal || ctrlResMgr.CtrlCam.camModeId == CtrlCamera.ModeId.NormalToCloseLook){
		        switch(statePlayTask){
		
		        case 0:
		            statePlayTask ++;
		            break;
		
		        /// 終了待ち
		        case 1:
					EatAngle = new Vector3(ctrlResMgr.CtrlCam.GetCamPos().X,ctrlResMgr.CtrlCam.GetCamPos().Y-1.0f,ctrlResMgr.CtrlCam.GetCamPos().Z)-BasePos;
					ActiveDis = 0.001f;
		            statePlayTask ++;
					//食われる動作
		            break;
		
		        /// エフェクト余韻
		        case 2:
					BasePos += EatAngle.Normalize() * (int)(Data.SetupValue.EatingSpeed)/10.0f;
					float dis = Common.VectorUtil.DistanceY(BasePos, ctrlResMgr.CtrlCam.GetCamPos());
					if(dis < (int)Data.SetupValue.EatingDeadArea/10.0f){
			            statePlayTask ++;
					}
					break;
				case 3:
		//            EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, BasePos );
		            AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.Eat, BasePos );
					ctrlResMgr.EatingFlag = true;
					ctrlResMgr.EatCharNumber = objCh.TexId+1;
		            Enable = false;
		            
		            break;
		        }
			}else{
				ctrlResMgr.EatCharNumber = objCh.TexId+1;
	            Enable = false;
			}
		}else{
	        switch(statePlayTask){
	
	        case 0:
	            statePlayTask ++;
	            break;
	
	        /// 終了待ち
	        case 1:
				ctrlResMgr.changePlAni = true;
				ctrlResMgr.eatingBoss = true;
				ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.NormalToLookMyself);
				EatAngle = new Vector3(ctrlResMgr.CtrlCam.GetCamPos().X,ctrlResMgr.CtrlCam.GetCamPos().Y-1.0f,ctrlResMgr.CtrlCam.GetCamPos().Z)-BasePos;
				//BasePos.X = ctrlResMgr.CtrlPl.GetPos().X + FMath.Sin( ctrlResMgr.CtrlCam.GetCamRot().Y / 180 *FMath.PI)*5;
				//BasePos.Y = 2;
				//BasePos.Z = ctrlResMgr.CtrlPl.GetPos().Z + FMath.Cos( ctrlResMgr.CtrlCam.GetCamRot().Y / 180 *FMath.PI)*5;;	
				float angleY = ctrlResMgr.CtrlPl.GetRotY()-30;
				if(angleY < -360.0f){
					angleY += 360.0f;
				}else if(angleY > 360.0f){
					angleY -= 360.0f;																		
				}
					
				BasePos.X = ctrlResMgr.CtrlPl.GetPos().X + FMath.Sin ( angleY * FMath.PI / 180.0f ) *19;
				BasePos.Z = ctrlResMgr.CtrlPl.GetPos().Z + FMath.Cos ( angleY * FMath.PI / 180.0f ) *19;
				BasePos.Y = ctrlResMgr.CtrlPl.GetPos().Y;  
					
				eatingTime = 0;
				moveAngle = false;
	            statePlayTask ++;
				//食われる動作
	            break;
	
	        /// エフェクト余韻
	        case 2:
				float angleY2 = ctrlResMgr.CtrlPl.GetRotY()-30;
				if(angleY2 < -360.0f){
					angleY2 += 360.0f;
				}else if(angleY2 > 360.0f){
					angleY2 -= 360.0f;																		
				}
					
//				BasePos.X = ctrlResMgr.CtrlPl.GetPos().X + FMath.Sin ( angleY2 * FMath.PI / 180.0f ) * (19 - 12* FMath.Cos ( (1-eatingTime / 40.0f) * FMath.PI/2 ));
//				BasePos.Z = ctrlResMgr.CtrlPl.GetPos().Z + FMath.Cos ( angleY2 * FMath.PI / 180.0f ) * (19 - 12* FMath.Cos ( (1-eatingTime / 40.0f) * FMath.PI/2 ));
				BasePos.X = ctrlResMgr.CtrlPl.GetPos().X + FMath.Sin ( angleY2 * FMath.PI / 180.0f ) * 19;
				BasePos.Z = ctrlResMgr.CtrlPl.GetPos().Z + FMath.Cos ( angleY2 * FMath.PI / 180.0f ) * 19;
				BasePos.Y = ctrlResMgr.CtrlPl.GetPos().Y + FMath.Sin ( eatingTime / 80.0f * FMath.PI)* 13.0f;
				eatingTime++;

				if(eatingTime > 30){
					statePlayTask ++;
				}
				float dis = Common.VectorUtil.DistanceY(BasePos, ctrlResMgr.CtrlCam.GetCamPos());
					
					
				/*
				if(dis < (int)Data.SetupValue.EatingDeadArea){
		            statePlayTask ++;
				}*/
				break;
			case 3:
				float angleY3 = ctrlResMgr.CtrlPl.GetRotY()-30;
				if(angleY3 < -360.0f){
					angleY3 += 360.0f;
				}else if(angleY3 > 360.0f){
					angleY3 -= 360.0f;																		
				}
					
				BasePos.X = ctrlResMgr.CtrlPl.GetPos().X + FMath.Sin ( angleY3 * FMath.PI / 180.0f ) * 19 * (1-(eatingTime-30)/40.0f) + FMath.Sin ( ctrlResMgr.CtrlPl.GetRotY() * FMath.PI / 180.0f ) * 3 * (eatingTime-30)/40.0f;
				BasePos.Z = ctrlResMgr.CtrlPl.GetPos().Z + FMath.Cos ( angleY3 * FMath.PI / 180.0f ) * 19 * (1-(eatingTime-30)/40.0f) + FMath.Cos ( ctrlResMgr.CtrlPl.GetRotY() * FMath.PI / 180.0f ) * 3 * (eatingTime-30)/40.0f;
					
				if(eatingTime < 40)  BasePos.Y = ctrlResMgr.CtrlPl.GetPos().Y + FMath.Sin ( eatingTime / 80.0f * FMath.PI)* 13.0f;
				eatingTime++;
				if(eatingTime > 55){
					statePlayTask ++;
					eatingTime = 0;		
				}
					
	            break;
			case 4:
	//            EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, BasePos );
				ctrlResMgr.CtrlPl.Addeffect(BasePos);
				statePlayTask ++;
	            break;
			case 5:
				eatingTime++;
				if(eatingTime== 15) ctrlResMgr.CtrlPl.Addeffect(new Vector3(BasePos.X+1.0f,BasePos.Y+0.3f,BasePos.Z+1.0f));

				if(eatingTime > 30)statePlayTask++;	
				break;
			case 6:
	//            EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, BasePos );
				ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.LookMyselfToNormal);
				ctrlResMgr.changePlAni = true;
				ctrlResMgr.eatingBoss = false;
	            AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.Eat, BasePos );
				ctrlResMgr.CtrlPl.Addeffect(BasePos);
				ctrlResMgr.CtrlStg.MonumentSetFlag = true;
				ctrlResMgr.EatCharNumber = objCh.TexId+1;
				eatingTime++;
				if(eatingTime > 20)statePlayTask++;
//				ctrlResMgr.CtrlPl.Addeffect(BasePos);
	            Enable = false;
	            break;
	        }
		}
			

        return true;
	}

	
    /// 旋回値をセットする
    private void setTurnRot( float turnRot )
    {
        if( moveTurn > turnRot ){
            unitCmnPlay.SetTurn( turnRot, 1 );
        }
        else if( moveTurn < -turnRot ){
            unitCmnPlay.SetTurn( -turnRot, 1 );
        }
        else {
            unitCmnPlay.SetTurn( moveTurn, 1 );
        }
    }
}

} // namespace
