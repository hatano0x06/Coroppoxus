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
public class CtrlSunMoon
{

    private List< ActorChSunMoon >    actorChList;
    private List< ActorChSunMoon >    activeList;
    public  float   	              EntryAreaDis = (float)Data.SetupValue.CharAppearArea;
    public  int                 	  EntryStayMax = (int)Data.SetupValue.CharAppearTime;
    public  const int                 EntryStayRot = (int)Data.SetupValue.CharAppearRot;
	public  const int                 EntryAreaDisLookSelfcam = 60;
	private const float       		  moveSpeed = 0.104f;
	GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();

/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorChList = new List< ActorChSunMoon >();
        if( actorChList == null ){
            return false;
        }

        activeList = new List< ActorChSunMoon >();
        if( activeList == null ){
            return false;
        }


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
        for( int i=0; i<actorChList.Count; i++ ){
			if(actorChList[i].ActiveFlg == true){

				GameActorCollManager    useCollMgr    = actorChList[i].GetMoveCollManager();
	            /// 他アクタからのイベントをチェック
	            ///-------------------------------------
	            if( actorChList[i].EventCntr.Num > 0 ){
	                ctrlResMgr.CtrlEvent.Play( actorChList[i], actorChList[i].EventCntr );
	            }
	
	            /// フレーム処理
	            ///-------------------------------------
				if(actorChList[i].GetStateId() != ActorChBase.StateId.Dead && actorChList[i].GetStateId() != ActorChBase.StateId.Eat){
	                frameMove( actorChList[i] );
				}
					
				if(actorChList[i].firstFlag == true){
						
					useCollMgr.SetMoveShape( GetUseActorBaseObj(i).GetMoveShape() ); 	
					ctrlResMgr.SetCollisionActorEn( useCollMgr.TrgContainer, actorChList[i].BasePos );
				}
	
				actorChList[i].Frame();
				
//				actorChList[i].ActiveDisPlace = Common.VectorUtil.DistanceXZ(actorChList[i].GetBodyPos(), CtrlRandom.getRandom.CtrlPl.GetPos());
					
	            /// 自身発生のイベントをチェック
	            ///-------------------------------------
	            if( actorChList[i].EventCntr.Num > 0 ){
	                ctrlResMgr.CtrlEvent.Play( actorChList[i], actorChList[i].EventCntr );
	            }
			}
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
		if(activeList.Count != 0){
			if(activeList[idx].ActiveFlg == true ){
	            activeList[idx].Draw( graphDev);
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
				if(actorChList[i].deadFlag == true  || actorChList[i].ActiveFlg == false){
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
			if(actorChList[i].deadFlag == true || actorChList[i].ActiveFlg == false){
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
			if(actorChList[i].deadFlag == true  || actorChList[i].ActiveFlg == false){
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

    /// 敵の登録
    public void EntryAddTower( int TexId, Vector3 pos)
    {
        ActorChSunMoon actorCh = new ActorChSunMoon(TexId);
        actorCh.Init();
        actorCh.Start();

        actorChList.Add( actorCh );

        SetPlace( (actorChList.Count-1), pos );
    }
		
	public void AddEffect(Vector3 Position){
		actorChList[actorChList.Count-1].AddEffectSplash(Position);			
	}

    /// 敵の登録削除
    public void DeleteEntryTower( int idx )
    {
        actorChList.RemoveAt( idx );
    }

    /// 敵の位置
    public Vector3 GetTowerPos( int idx )
    {
		return actorChList[idx].GetBodyPos();
    }
		
    /// 敵の位置
    public void killTower( int idx )
    {
		actorChList[idx].deadMark = true;
	}
		
	/// 敵の配置
    public void SetPlace( int idx, Vector3 pos )
    {
        Matrix4 mtx = Matrix4.RotationY( 0 );
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
    private bool frameMove( ActorChSunMoon actorCh )
    {

/*		Vector3 plPos = GameCtrlManager.GetInstance().CtrlPl.GetPos();
        float rot = Common.MatrixUtil.GetPointRotY( actorCh.BaseMtx, actorCh.BasePos, plPos );
        if( actorCh.ActiveDis < 10.0f && (rot > 10.0f || rot < -10.0f) ){
            actorCh.SetStateTurn( rot );
            return true;
        }
        */
//        actorCh.SetStateMove( new Vector3( rotMtx.M31, rotMtx.M32, rotMtx.M33 ), moveSpeed, 0.0f, false );
//        actorCh.SetStateMove( new Vector3( 0.0f, 0.0f, 1.0f ), moveSpeed, 0.0f, false );
	
        actorCh.SetStateStand();
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
            if( !actorChList[i].Enable ){
                actorChList[i].End();
                actorChList.RemoveAt(i);     // 要素の削除
                i --;
                continue ;
            }

			float rot = Common.VectorUtil.GetRotY( GameCtrlManager.GetInstance().CtrlCam.GetCamRot().Y, GameCtrlManager.GetInstance().CtrlCam.GetCamPos(), actorChList[i].BasePos);
            float dis = Common.VectorUtil.DistanceXZ( actorChList[i].BasePos, GameCtrlManager.GetInstance().CtrlCam.GetCamPos() );
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

}

} // namespace
