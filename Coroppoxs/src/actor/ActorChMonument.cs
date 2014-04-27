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
public class ActorChMonument : ActorChBase
{
    private const int hpMax = 3;

    private ObjChMonument	        objCh;
    private int                     deadCnt;
    public  bool                    ActiveFlg;
    public  int                     ActiveCnt;
    public  float                   ActiveDis;
	public  float					ActiveRot;
    private ObjEffect           	objShadow;
    private ObjEffect           	objShadowMax;
	public float 					ShadowSize;
	public float 					ShadowScale;
		
	GameCtrlManager           		ctrlResMgr    = GameCtrlManager.GetInstance();
/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objCh = new ObjChMonument();
        objCh.Init();
			
		objShadow = new ObjEffect();
		objShadow.Init();
		objShadowMax = new ObjEffect();
		objShadowMax.Init();
		ShadowSize = 18.0f;
		ShadowScale = 0;
			

        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        if( objCh != null ){
            objCh.Term();
        }
        objCh    = null;
        objShadow.Term();
			
    }

    /// 開始
    protected override bool DoStart()
    {
        objCh.Start();
			
        objShadow.SetMdlHandle( Data.EffTypeId.Eff10 );
        objShadow.Start();
        objShadow.SetScale( ShadowSize/10.0f*2.6f);	
        objShadowMax.SetMdlHandle( Data.EffTypeId.Eff10 );
        objShadowMax.Start();
		objShadowMax.SetScale( ShadowSize/10.0f*2.6f);	

        ActiveFlg    = false;
        ActiveDis    = 0;
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        ActiveFlg    = false;
        objShadow.End();
		objShadowMax.End();
        objCh.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
		ShadowScale += 0.01f;
		if(ShadowScale > 1.0f) ShadowScale = 0.0f;
		objShadow.SetScale( ShadowSize/10.0f*2.6f * ShadowScale);	
	
		if(statePlayTask == 0){
			if(objCh.deadFlag == true){
				ChangeState( StateId.Dead );					
			}
			else if(objCh.eatFlag == true){
				ChangeState( StateId.Eat );
			}
		}

		switch( stateIsPlayId ){
	        case StateId.Dead:      statePlayDead();        break;
			case StateId.Eat:		statePlayEat();			break;
        }
			
//        mvtHdl.Frame();
		if(objCh.firstFlag == true){	
	        unitCmnPlay.Frame();
		    unitCmnPlay.FrameGravity2d(ref objCh.firstFlag);        /// 敵は衝突判定を行わない
	
				/// OBJの姿勢を更新
	        if( unitCmnPlay.IsUpdateMtx() ){
	            updateMatrix( unitCmnPlay.Mtx );
	        }
		}

        objCh.Frame();
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCh.Draw2( graphDev ,BasePos );
        return true;
    }
		
    public bool DrawAlpha( DemoGame.GraphicsDevice graphDev )
    {
	 	Matrix4 mtx = BaseMtx;
	    mtx.M41 = GetBodyPos().X;
	    mtx.M43 = GetBodyPos().Z;
	
		objShadowMax.SetMatrix( mtx );
		objShadowMax.Draw( graphDev );
			
	    mtx.M42 += 0.01f;
		objShadow.SetMatrix( mtx );
		objShadow.Draw( graphDev );
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

	public void AddEffectSplash(Vector3 Position){
		EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, Position );			
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
		
	public bool deadFlag{
		get{return this.objCh.deadFlag;}		
	}


	public bool farFlag
	{
		set{this.objCh.farFlag = value;}
		get{return this.objCh.farFlag;}
	}

	public bool firstFlag
	{
		get{return this.objCh.firstFlag;}
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
		


    /// 立ち
    private bool statePlayStand()
    {
//        mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
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
 //               mvtHdl.SetPlayMvt( (int)Data.ChMvtResId.Stand, false );
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
//            if( mvtHdl.IsActive() == false ){
                ChangeState( StateId.Stand );
//            }
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
 //           if( mvtHdl.IsActive() == false ){

                ChangeState( StateId.Stand );
 //           }
            break;
        }

        return true;
    }



    /// 死亡
    private bool statePlayDead()
    {
        Vector3 effPos = new Vector3(0,0,0);

        switch(statePlayTask){
        /// 終了待ち
        case 0:
//            if( mvtHdl.IsActive() == false ){
                Common.VectorUtil.Set( ref effPos, objCh.BodyPos.X, objCh.BodyPos.Y, objCh.BodyPos.Z );

/*				switch(chTypeId){
                case Data.ChTypeId.MonsterA:        effPos.Y -= 0.6f;            break;
                case Data.ChTypeId.MonsterB:        effPos.Y -= 0.1f;            break;
                case Data.ChTypeId.MonsterC:        effPos.Y -= 0.1f;            break;
                }
*/
                EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, effPos );

                AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.ObjBreak, BasePos );

                deadCnt = 0;
                statePlayTask ++;
 //           }
            break;

        /// エフェクト余韻
        case 1:
            deadCnt ++;
            if( deadCnt >= 25 ){
                Enable = false;
            }
            break;
        }

        return true;
    }
		
    /// 食べられる
    private bool statePlayEat()
    {
        switch(statePlayTask){

        case 0:
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
			BasePos.Y += 0.3f;
			//食われる動作
            break;

        /// エフェクト余韻
        case 2:
            Enable = false;
            break;
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
