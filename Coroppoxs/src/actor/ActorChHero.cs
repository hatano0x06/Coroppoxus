/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {

///***************************************************************************
/// ACTOR : 英雄の操作
///***************************************************************************
public class ActorChHero : ActorChBase
{
    private const float hpMax = 5.0f;

    private ObjChHero                objCh;
    private int                      moveCnt;
    private bool                     isMvtCancel;
    public  float        			 hpNow;
	public  bool					 eatFlag;
	public short					 poisionCount;		
	GameCtrlManager           		ctrlResMgr    = GameCtrlManager.GetInstance();		

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    protected override bool DoInit()
    {
        objCh = new ObjChHero();
        objCh.Init();
		eatFlag = false;
		poisionCount = 0;
        return true;
    }

    /// 破棄
    protected override void DoTerm()
    {
        if( objCh != null ){
            objCh.Term();
        }
        objCh        = null;
    }

    /// 開始
    protected override bool DoStart()
    {

        hpNow = hpMax;

        objCh.Start();
        return true;
    }

    /// 終了
    protected override void DoEnd()
    {
        objCh.End();
    }

    /// フレーム処理
    protected override bool DoFrame()
    {
        isMvtCancel = false;
			
        switch( stateIsPlayId ){
	        case StateId.Stand:     statePlayStand();       break;
	        case StateId.Move:      statePlayMove();        break;
	        case StateId.Battle:	statePlayAttack();      break;
	        case StateId.Damage:    statePlayDamage();      break;
	        case StateId.Dead:      statePlayDead();        break;
	        case StateId.Victory:   statePlayVictory();     break;
        }
			
        hpNow -= 0.001f;
	    if(poisionCount > 0){
			hpNow -= 0.001f;
			poisionCount--;
		}

			//			Console.WriteLine (hpNow);

       if( hpNow <= 0 ){
           hpNow = 0.001f;        /// 死亡のタイミングをずらす
           ChangeState( StateId.Dead );
       }
	
	   unitCmnPlay.Frame();
 
        if( AppDebug.GravityFlg ){
            unitCmnPlay.FrameGravity(false);
        }

        /// OBJの姿勢を更新
        if( unitCmnPlay.IsUpdateMtx() ){
            updateMatrix( unitCmnPlay.Mtx );
        }

        objCh.Frame();
        return true;
    }

    /// 描画処理
    protected override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
        objCh.Draw( graphDev );

        return true;
    }

    /// 使用しているOBJ
    protected override GameObjProduct DoGetUseObj( int index )
    {
        return objCh;
    }
    protected override int DoGetUseObjNum()
    {
        return 2;
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


/// public
///---------------------------------------------------------------------------

    /// 体の中心座標の取得
    public Vector3 GetBodyPos()
    {
        return objCh.BodyPos;
    }

    /// 動作キャンセル可能かのチェック
    public bool CheckMvtCancel()
    {
        return isMvtCancel;
    }

	public void AddEffectSplash(Vector3 Position){
		EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff05, Position );			
	}
		



/// アクタイベント
///---------------------------------------------------------------------------

    /// ダメージ
    public override void SetEventDamage( GameObjProduct trgObj, Data.AttackTypeId dmgId )
    {
        if( stateIsPlayId != StateId.Dead ){
            dmgTrgObj    = trgObj;

            hpNow --;

            if( hpNow <= 0 ){
                ChangeState( StateId.Dead );
            }
            else {
                ChangeState( StateId.Damage );
            }
        }
    }

    /// 動作キャンセル
    public override void SetEventMvtCancel()
    {
        isMvtCancel = true;
    }
		
	public float Hp{
		get{return hpNow;}
		set{this.hpNow = value;}
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
        /// 旋回
        if( moveTurn != 0.0f ){
            unitCmnPlay.SetRot( moveTurn );
        }
        moveCnt = 5;
        return true;
    }

    /// 歩く
    private bool statePlayMove()
    {
        if( moveCnt >= 12 ){
			if(movePow != 0.0f){
            	AppSound.GetInstance().PlaySe( AppSound.SeId.PlFoot );
			}
            moveCnt = 0;
        }
        moveCnt ++;

        /// 移動
        if( movePow != 0.0f ){
            unitCmnPlay.SetMoveActor( moveVec, movePow );
        }

        /// 旋回
        if( moveTurn != 0.0f ){
            unitCmnPlay.SetRot( moveTurn );
        }

        movePow     = 0.0f;
        moveTurn    = 0.0f;
        return true;
    }


    /// 攻撃
    private bool statePlayAttack()
    {
        return true;
    }



    /// ダメージ
    private bool statePlayDamage()
    {
        return true;
    }


    /// 死亡
    private bool statePlayDead()
    {
        switch(statePlayTask){

        /// 死亡セット
        case 0:
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            hpNow = 0;
            break;
        }

        return true;
    }


    /// 勝利
    private bool statePlayVictory()
    {
        switch(statePlayTask){

        /// ダメージセット
        case 0:
            statePlayTask ++;
            break;

        /// 終了待ち
        case 1:
            ChangeState( StateId.Stand );
            break;
        }

        return true;
    }


    /// ダメージ演出
    private void setDamageEff()
    {

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

        /// エフェクト表示
        Vector3 effPos = new Vector3( objCh.Mtx.M41+objCh.Mtx.M31*0.25f, objCh.Mtx.M42+0.9f, objCh.Mtx.M43+objCh.Mtx.M33*0.25f );
        EventCntr.Add( ActorEventId.Effect, (int)Data.EffTypeId.Eff12, effPos );
        AppSound.GetInstance().PlaySe( AppSound.SeId.PlDamage );
    }



}

} // namespace
