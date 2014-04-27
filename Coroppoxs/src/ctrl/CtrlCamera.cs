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
/// カメラ制御
///***************************************************************************
public class CtrlCamera
{    ///-------------------------------------
    /// カメラモードID
    ///-------------------------------------
    public enum ModeId {
        Normal = 0,     /// 通常カメラ
		NormalToLookMyself,
		LookMyselfToNormal,
        LookMyself,            /// FPSカメラ		
		NormalToCloseLook,
		CloseLookToNormal,
		CloseLook
    };
		 
    private ActorCamBehind          actorBehind;
    private Matrix4                 worldMtx;
    private Vector3                 worldRot;
    private float                   worldDis;
    private int                     autoCamCnt;
    private int                     autoCamFrame;
    public  ModeId                  camModeId;
    private bool                    modeChangeFlg;
	private Vector3					preLook;
	private Vector3					changeRot;
	private float 				    moveDisX;
	private float 					lookrate;
	private int 					cammovespeed;
	private int 					changeCount;

    private float     		        camBehindY = 20.0f;


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorBehind = new ActorCamBehind();
        actorBehind.Init();

		worldMtx	= Matrix4.Identity;
        worldRot    = new Vector3( 0.0f, 0.0f, 0.0f );
		preLook = worldRot;
		moveDisX = 20;
		lookrate = 1.0f;
		cammovespeed = 1;
		changeCount = 0;
		return true;
    }

    /// 破棄
    public void Term()
    {
        actorBehind.Term();

        actorBehind        = null;
    }

    /// 開始
    public bool Start()
    {
        autoCamCnt      = 0;
        autoCamFrame    = 0;

        worldMtx    = Matrix4.Identity;
        worldRot    = new Vector3( 0.0f, 0.0f, 0.0f );
        worldDis    = 6.0f;
        modeChangeFlg = false;

        ResetCamMode();

        actorBehind.Start();

        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        GameObjProduct  trgObj     = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3         movePos    = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42, trgObj.Mtx.M43 );
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 5.0f, 1 );
        actorBehind.Frame();
        return true;
    }

    /// 終了
    public void End()
    {
        actorBehind.End();
    }


    /// フレーム処理
    public bool Frame()
    {
        switch( camModeId ){
	        case ModeId.Normal:     frameFieldEyecam();     break;        /// 通常カメラ
	        case ModeId.LookMyself: frameLookSelf();        break;        /// １人称カメラ
			case ModeId.CloseLook:	frameCloseLook();       break;        /// 通常カメラ(拡大)
			default: 				frameChangeCam();		break;
        }
        return true;
    }
		
	
		
    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        actorBehind.Draw( graphDev );

        return true;
    }

    /// 通常状態開始
    public void ResetCamMode()
    {
        worldDis    = 5.0f;
        SetCamMode( ModeId.Normal );
    }

    /// カメラモードのセット
    public void SetCamMode( ModeId id )
    {
        camModeId = id;
		if(id == ModeId.Normal){
//			camBehindY = 20.0f;
//			worldRot = preLook;
		}
		else if(id == ModeId.LookMyself){
//			camBehindY = 30.0f;			
		}
		else if(id == ModeId.CloseLook){
//			camBehindY = 20.0f;					
//			worldRot.X = -50.0f;
		}else{
			if(id == ModeId.NormalToCloseLook || id == ModeId.NormalToLookMyself){
				preLook = worldRot;
			}
			changeRot = worldRot;
		}
		modeChangeFlg = false;
    }


    /// カメラの世界に対するY軸回転のみの行列を返す
    public Matrix4 GetCamMatrixRotY()
    {
        ObjCamera camObj = actorBehind.Obj;

        float a_Cal     = (float)(3.141593f / 180.0);
        float angleY    = camObj.TrgRot.Y * a_Cal;
        return Matrix4.RotationY( angleY );
    }

    /// カメラのY軸回転をセット
    public void SetCamRotY( float rotY )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
        GameObjProduct  trgObj     = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3         movePos    = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42, trgObj.Mtx.M43 );
            
        worldRot.Y = rotY;
            
        /// カメラにセットする行列を登録
        /// 
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 4.0f, 1 );
        actorBehind.Frame();
        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );
    }
		
		
    public float GetCamRotX()
    {
        return actorBehind.Obj.TrgRot.X;
    }

    /// カメラのY軸回転値を返す
    public float GetCamRotY()
    {
        return actorBehind.Obj.TrgRot.Y;
    }

    public Vector3 GetCamRot()
    {
        return actorBehind.Obj.TrgRot;
    }


    /// カメラの取得
    public DemoGame.Camera GetCurrentCameraCore()
    {
        return actorBehind.Obj.CameraCore;
    }

    /// カメラの位置を取得
    public Vector3 GetCamPos()
    {
        return actorBehind.Obj.CamPos;
    }

    /// カメラの注視点の取得
    public Vector3 GetCamTrgPos()
    {
        return actorBehind.Obj.TrgPos;
    }

    /// 注視点との距離
    public bool CheckModeChange()
    {
        return modeChangeFlg;
    }


    /// フレーム処理（配置用）
    public bool FramePlace( Vector3 trgPos )
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.InputGamePad pad        = AppInput.GetInstance().Pad;

        if( (pad.Scan & DemoGame.InputGamePadState.L) != 0 ){
            worldDis -= 0.1f;
            if( worldDis < 1.0f ){
                worldDis = 1.0f;
            }
        }
        else if( (pad.Scan & DemoGame.InputGamePadState.R) != 0 ){
            worldDis += 0.1f;
        }
        else {
            worldRot.X += AppInput.GetInstance().CamRotX;
            worldRot.Y += AppInput.GetInstance().CamRotY;
        }

		if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, trgPos, worldRot, worldDis, 2 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }


    /// フレーム処理（タイトル用）
    public bool FrameTitle()
    {
        float[,] autoCamTrgPos = {
            { -105.5112f, 3.7772f, -84.7431f },
            { 58.3641f, 25.4343f, 61.3083f },
            { -24.0761f, 12.7641f, -115.3390f },
        };
        float[,] autoCamRot = {
            { 2.1f, -105.1f, 0.0f },
            { -12.1f, 128.9f, 0.0f },
            { -2.0f, -150.0f, 0.0f },
        };
        float[] autoCamDis = {
            17.1f, 19.0f, 10.3f
        };

        Vector3 pos = new Vector3( autoCamTrgPos[autoCamCnt,0], autoCamTrgPos[autoCamCnt,1], autoCamTrgPos[autoCamCnt,2] );
        Vector3 rot = new Vector3( autoCamRot[autoCamCnt,0], autoCamRot[autoCamCnt,1], autoCamRot[autoCamCnt,2] );
        float   dis = autoCamDis[autoCamCnt];
        rot.Y += (autoCamFrame/10.0f);

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, pos, rot, dis, 1 );
        actorBehind.Frame();

        autoCamFrame ++;
        if( autoCamFrame >= 120 ){
            autoCamFrame    = 0;
            autoCamCnt        = (autoCamCnt+1)%autoCamDis.Length;
        }
        return true;
    }



    /// フレーム処理（リザルト用）
    public bool FrameResult()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        if( worldRot.X > 30.0f ){
            worldRot.X = 30.0f;
        }
        else if( worldRot.X < 0.0f ){
            worldRot.X = 0.0f;
        }

        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );

        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, 5.0f, 4 );

        actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 通常カメラ
    private bool frameFieldEyecam()
    {
				
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
        }

        if( worldRot.X > 40.0f ){
            worldRot.X = 40.0f;
        }
        else if( worldRot.X < -40.0f ){
            worldRot.X = -40.0f;
        }
			
		worldDis = 6.0f;
		
        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, worldDis, 10 ,20 ,1);

        actorBehind.FrameLookSelf();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }


    /// FPSカメラ    
    private bool frameLookSelf()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, worldDis, 10 ,20 ,0.0f);

        actorBehind.FrameLookSelf();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }

    /// 通常カメラ
    private bool frameCloseLook()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

//        worldRot.X += AppInput.GetInstance().CamRotX;
        worldRot.Y += AppInput.GetInstance().CamRotY;

        if( worldRot.Y > 360.0f ){
            worldRot.Y -= 360.0f;
        }
        else if( worldRot.Y < -360.0f ){
            worldRot.Y += 360.0f;
		}
		/*	
        if( worldRot.X > -60.0f ){
            worldRot.X = -60.0f;
        }
        else if( worldRot.X < -80.0f ){
            worldRot.X = -80.0f;
        }			
		*/	
		worldDis = 20.0f;
	
        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
			
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, worldDis, 10 , 20 ,1);

        actorBehind.FrameLookSelf();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }
		
    /// 通常カメラ
    private bool frameChangeCam()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
		
		
		if(camModeId == ModeId.CloseLookToNormal || camModeId == ModeId.NormalToCloseLook){
			if(camModeId == ModeId.NormalToCloseLook){
				changeCount++;
				worldRot.X = (-60.0f*changeCount + changeRot.X*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			}else if(camModeId == ModeId.CloseLookToNormal){	
				changeCount--;
				worldRot.X = (changeRot.X*changeCount - 10.0f*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			}
				
			worldDis = (20.0f*changeCount + 6.0f*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
				
			if(changeCount > (int)Data.SetupValue.ChangeCamSpeed){
				SetCamMode(ModeId.CloseLook);				
			}else if(changeCount < 0){
				SetCamMode(ModeId.Normal);
			}
			cammovespeed = 5;			
			lookrate = 1.0f;
		}
		else if(camModeId == ModeId.LookMyselfToNormal || camModeId == ModeId.NormalToLookMyself){
			if( camModeId == ModeId.NormalToLookMyself){
				changeCount++;
				worldRot.X = (-45.0f*changeCount + changeRot.X*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;

				float lookY = preLook.Y + 25.0f;
				if(lookY > 360.0f) lookY -= 360.0f;					
				else if( lookY < -360.0f )lookY += 360.0f;
		     
				worldRot.Y = (lookY*changeCount +changeRot.Y*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			}else if(camModeId == ModeId.LookMyselfToNormal){	
				changeCount--;
				worldRot.X = (changeRot.X*changeCount + preLook.X*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
				worldRot.Y = (changeRot.Y*changeCount + preLook.Y*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			}				
				
			worldDis = (50.0f*changeCount + 6.0f*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			lookrate = (0.0f*changeCount + 1.0f*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			camBehindY = (15.0f*changeCount + 20.0f*((int)Data.SetupValue.ChangeCamSpeed-changeCount))/(int)Data.SetupValue.ChangeCamSpeed;
			cammovespeed = 20;

			if(changeCount > (int)Data.SetupValue.ChangeCamSpeed){
				SetCamMode(ModeId.LookMyself);				
			}else if(changeCount < 0){
				SetCamMode(ModeId.Normal);
			}
		}			
			
        GameObjProduct    trgObj = ctrlResMgr.CtrlPl.GetUseActorBaseObj();
        Vector3           movePos = new Vector3( trgObj.Mtx.M41, trgObj.Mtx.M42+camBehindY, trgObj.Mtx.M43 );
        /// カメラにセットする行列を登録
        actorBehind.SetMoveParam( worldMtx, movePos, worldRot, worldDis, cammovespeed , moveDisX ,lookrate);
        actorBehind.FrameLookSelf();
//			actorBehind.Frame();

        ctrlResMgr.GraphDev.SetCurrentCamera( actorBehind.Obj.CameraCore );

        return true;
    }
		

/// プロパティ
///---------------------------------------------------------------------------
}

} // namespace
