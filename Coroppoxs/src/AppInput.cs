/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;


namespace AppRpg {


///***************************************************************************
/// アプリ依存の入力処理
///***************************************************************************
public class AppInput
{

    ///---------------------------------------------------------------------------
    /// 入力イベントID
    ///---------------------------------------------------------------------------
    public enum EventId{
        Move       = (1 << 0),        /// プレイヤー移動
        CamRot     = (1 << 1),        /// カメラ移動
        Pause	   = (1 << 2),  /// フィールドタッチ
        DestinationMove   = (1 << 3),  /// フィールドタッチ
        DestinationCancel = (1 << 4),  /// フィールドタッチリリース
        buttonPress 	  = (1 << 5),  /// 移動先キャンセル
    };

    private struct cTapParam
    {
        public ImagePosition                 Pos;
        public int                           TouchId;
    }
    private struct cTouchParam
    {
        public ImagePosition                 Pos;
    }
		
    GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();			


    private static AppInput instance = new AppInput();

    private DemoGame.InputGamePad        useGPad;
    private DemoGame.InputTouch          useTouch;

    private int                          scrWidth;
    private int                          scrHeight;

    private EventId                      eventId;
    private float                        plRotY;
    private float                        camRotX;
    private float                        camRotY;

    private bool                         touchRelease;
    private int                          backTouchNum;

    /// シングルタッチ
    private bool                         singleTouchFlg;
    private bool                         singleFlicFlg;
    private int                          singleDTapCnt;
    public ImagePosition                 singleDTapBackPos;

    /// マルチタッチ
    private cTapParam                    singleTapParam;
    private cTouchParam                  singleTouchParam;
    private cTapParam[]                  multiTapParam;
    private float                        multiBackDis;
	private bool 						 ChangeCamFlag;
	private bool 						 ChangeCamFlag2;


    private bool                         plBehindFlg;

	

    /// インスタンスの取得
    public static AppInput GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init( DemoGame.InputGamePad pad, DemoGame.InputTouch touch, DemoGame.GraphicsDevice gDev )
    {
        useGPad        = pad;
        useTouch       = touch;
        plRotY         = 0.0f;
        camRotX        = 0.0f;
        camRotY        = 0.0f;
        eventId        = 0;
        touchRelease   = false;
        singleTouchFlg = false;
        singleDTapCnt  = 0;
        backTouchNum   = 0;
		plBehindFlg    = false;
		ChangeCamFlag  = false;
		ChangeCamFlag2 = false;

        multiTapParam   = new cTapParam[2];
        singleTapParam.Pos.X = 0;
        singleTapParam.Pos.Y = 0;
		singleTouchParam.Pos.X = 0;
		singleTouchParam.Pos.Y = 0;

        scrWidth     = gDev.DisplayWidth;
        scrHeight    = gDev.DisplayHeight;
        return true;
    }

    /// 破棄
    public void Term()
    {
        eventId     = 0;
        useGPad     = null;
        useTouch    = null;
        multiTapParam  = null;
    }

    /// フレーム処理
    public void Frame()
    {
        eventId = 0;

        camRotX = 0.0f;
        camRotY = 0.0f;
        touchRelease = false;

		//ポーズ判定
		checkPadPause();

        /// 移動判定
        checkPadPlMove();

        /// LR判定
        checkPadLR();
		checkPadcamMode();
			
        /// カメラの回転判定
        checkPadCamRot();
		motionCamRot();

        /// タップ判定
        ///-------------------------------------------------------
        if( backTouchNum == 0 ){
            if( useTouch.GetInputNum() > 0 ){
                touchRelease = true;
            }
        }


        /// ニュートラル状態
        ///-------------------------------------------------------
        if( useTouch.GetInputNum() == 0 ){

            if( singleTouchFlg ){

                /// フリックしていたら無効
                if( singleFlicFlg ){
                    singleDTapCnt  = 0;
                }

                /// 目的地セット
                else{
                    eventId |= EventId.DestinationCancel;
                    singleDTapCnt  = 30;
                }

				singleTouchFlg = false;
			}
        }

        /// シングルタッチ系の判定
        ///-------------------------------------------------------
        else if( useTouch.GetInputNum() == 1 ){

            /// タッチした直後のパラメータセット
            if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Down ){
                setSingleTapParam();
            }
            if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Move ){
                setSingleTouchParam();					
                eventId |= EventId.DestinationMove;
	        }


            /// 攻撃判定
/*            if( singleTouchFlg && !singleFlicFlg ){
                checkTouchAttack();
            }
            */
				
            if( singleTouchFlg){
//				checkSingleTouchCamRot();
			}
        }

        /// マルチタッチ系の判定
        ///-------------------------------------------------------
        else{
            /// マルチタッチした直後のパラメータセット
            if( backTouchNum != 2 ){
                setMultiTouchBackDis();
                setMultiTouchBackPos();
                singleTouchFlg   = false;
            }

            /// ズーム判定
//            if( !multiZoomModeFlg ){
//                checkTouchCamZoom();
//            }

            /// カメラの回転判定
            checkTouchCamRot();

            singleDTapCnt  = 0;
        }

        if( singleDTapCnt > 0 ){
            singleDTapCnt --;
        }
        backTouchNum  = useTouch.GetInputNum();

    }


    /// プレイヤーの向きの取得
    public float GetPlRotY()
    {
        return plRotY;
    }

    /// フリック入力逆転のセット
    public void SetPlBehind( bool flg )
    {
		plBehindFlg = flg;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouchUp()
    {
		if( useTouch.GetInputNum() == 1 ){
			if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Up ){
				return true;
			}
		}
		return false;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouching()
    {
		if( useTouch.GetInputNum() == 1 ){
			return true;
		}
		return false;
	}
    
    /// デバイスの入力状態の取得
    public bool CheckDeviceSingleTouchDown()
    {
		if( useTouch.GetInputNum() == 1 ){
			if( useTouch.GetInputState( 0 ) == DemoGame.InputTouchState.Down ){
				return true;
			}
		}
		return false;
	}
    
/// private メソッド
///---------------------------------------------------------------------------

    /// プレイヤーの移動チェック
    private void checkPadPlMove()
    {
        plRotY = 0.0f;
        eventId |= EventId.Move;


        /// ８方向の入力チェック

        if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 45.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 135.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 225.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 && (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 315.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Up) != 0 ){
            plRotY = 0.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Left) != 0 ){
            plRotY = 90.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Down) != 0 ){
            plRotY = 180.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Right) != 0 ){
            plRotY = 270.0f;
        }

			/// アナログパッド入力
        else 
        if( useGPad.AnalogLeftX != 0.0f || useGPad.AnalogLeftY != 0.0f ){
            plRotY = (float)( Math.Atan2( useGPad.AnalogLeftX, useGPad.AnalogLeftY ) / (3.141593f / 180.0) );
            plRotY += 180.0f;
            if( plRotY >= 360.0f ){
                plRotY -= 360.0f;
            }
        }

        else{
            eventId &= ~EventId.Move;
        }

    }

    private void checkPadCamRot()
    {
        /// パッド入力チェック
        ///----------------------------------------------------------

        /// ８方向の入力チェック
        if( (useGPad.Scan & DemoGame.InputGamePadState.Square) != 0 ){
            camRotY = 2.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Circle) != 0 ){
            camRotY = -2.0f;
        }
        if( (useGPad.Scan & DemoGame.InputGamePadState.Triangle) != 0 ){
            camRotX = 2.0f;
        }
        else if( (useGPad.Scan & DemoGame.InputGamePadState.Cross) != 0 ){
            camRotX = -2.0f;
        }

        /// アナログパッド入力
        else if( (Math.Abs(useGPad.AnalogRightX) != 0.0f) || (Math.Abs(useGPad.AnalogRightY) != 0.0f) ){
            camRotX = -1.25f * useGPad.AnalogRightY;
            camRotY = -1.25f * useGPad.AnalogRightX;
        }
    }
		
	private void motionCamRot(){
		if(ctrlResMgr.CtrlCam.camModeId != CtrlCamera.ModeId.CloseLook){
	
		    var motionData = Motion.GetData(0);
		    Vector3 vel = motionData.AngularVelocity;			
			if(vel.X *vel.X > 0.01)
			camRotX -= vel.X * 0.01f * 60.0f;
				
			if(vel.Y * vel.Y > 0.01)
				camRotY -= vel.Y * 0.1f * 60.0f;
		}
	}
		
    private void checkSingleTouchCamRot()
    {
        /// パッド入力チェック
        ///----------------------------------------------------------

        int moveX = 0;
        int moveY = 0;

        int j;

        for( j=0; j<useTouch.GetInputNum(); j++ ){

            if( singleTapParam.TouchId == useTouch.GetInputId( j ) ){
                moveX += (singleTapParam.Pos.X - useTouch.GetScrPosX( j ));
                moveY += (singleTapParam.Pos.Y - useTouch.GetScrPosY( j ));
                break;
            }
        }
        if( j >= useTouch.GetInputNum() ){
            return ;
        }


        camRotY = ((float)moveX / (float)scrWidth) * 10.0f;
        camRotX = ((float)moveY / (float)scrHeight) * -10.0f;
    }	
		
    private void checkPadLR()
    {
		if(ctrlResMgr.eatingBoss == false){
	        if( (useGPad.Scan & DemoGame.InputGamePadState.L) != 0 || (useGPad.Scan & DemoGame.InputGamePadState.R) != 0 ){
				eventId |= EventId.buttonPress;
				if(ChangeCamFlag == false) {
					ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.NormalToCloseLook);
					ChangeCamFlag = true;
				}
	        }else{
				eventId &= ~EventId.buttonPress;
				if(ChangeCamFlag == true) {
					ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.CloseLookToNormal);
					ChangeCamFlag = false;
				}
			}
		}
    }
	
    private void checkPadcamMode()
    {
	    if( (useGPad.Scan & DemoGame.InputGamePadState.Select) != 0 ){
			ctrlResMgr.CtrlStg.MonumentSetFlag = true;
			if(ChangeCamFlag2 == false) {
				ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.NormalToLookMyself);
				ChangeCamFlag2 = true;
			}
		}else{
			if(ChangeCamFlag2 == true) {
				ctrlResMgr.CtrlCam.SetCamMode(CtrlCamera.ModeId.LookMyselfToNormal);
				ChangeCamFlag2 = false;
			}
		}
    }

    private void checkPadPause()
    {
	    if( (useGPad.Trig & DemoGame.InputGamePadState.Start) != 0 ){
			eventId |= EventId.Pause;
		}else{
            eventId &= ~EventId.Pause;
		}
    }		
		
    /// カメラの回転チェック
    private void checkTouchCamRot()
    {
        int moveX = 0;
        int moveY = 0;

        for( int i=0; i<2; i++ ){
            int j;
            for( j=0; j<useTouch.GetInputNum(); j++ ){

                if( multiTapParam[i].TouchId == useTouch.GetInputId( j ) ){
                    moveX += (multiTapParam[i].Pos.X - useTouch.GetScrPosX( j ));
                    moveY += (multiTapParam[i].Pos.Y - useTouch.GetScrPosY( j ));
                    break;
                }
            }

            if( j >= useTouch.GetInputNum() ){
                setMultiTouchBackPos();
                return ;
            }
        }

        camRotY = ((float)moveX / (float)scrWidth) * 60.0f;
        camRotX = ((float)moveY / (float)scrHeight) * -60.0f;
        setMultiTouchBackPos();
    }


    /// シングルタッチした際のパラメータセット
    private void setSingleTapParam()
    {
        singleDTapBackPos    = singleTapParam.Pos;
        singleTapParam.Pos.X = useTouch.GetScrPosX( 0 );
        singleTapParam.Pos.Y = useTouch.GetScrPosY( 0 );
        singleTapParam.TouchId = useTouch.GetInputId( 0 );
        singleTouchFlg = true;
        singleFlicFlg  = false;
    }

	private void setSingleTouchParam()
    {
        singleTouchParam.Pos.X = useTouch.GetScrPosX( 0 );
        singleTouchParam.Pos.Y = useTouch.GetScrPosY( 0 );
    }

    /// マルチタッチした際のパラメータセット
    private void setMultiTouchBackDis()
    {
        ImagePosition calPos;
        calPos.X = useTouch.GetScrPosX( 0 ) - useTouch.GetScrPosX( 1 );
        calPos.Y = useTouch.GetScrPosY( 0 ) - useTouch.GetScrPosY( 1 );
        multiBackDis    = FMath.Sqrt( (float)(calPos.X * calPos.X + calPos.Y * calPos.Y) );
    }



    /// マルチタッチした際のパラメータセット
    private void setMultiTouchBackPos()
    {
        for( int i=0; i<2; i++ ){
            multiTapParam[i].Pos.X = useTouch.GetScrPosX( i );
            multiTapParam[i].Pos.Y = useTouch.GetScrPosY( i );
            multiTapParam[i].TouchId = useTouch.GetInputId( i );
        }
    }




/// プロパティ
///---------------------------------------------------------------------------

    /// イベントIDの取得
    public EventId Event
    {
        get{return eventId;}
    }
		
    public float CamRotX
    {
        get {return camRotX;}
    }
    public float CamRotY
    {
        get {return camRotY;}
    }
    public float PlRotY
    {
        get {return plRotY;}
    }
    public bool TouchRelease
    {
        get {return touchRelease;}
    }

    public int SingleScrTapPosX
    {
        get {return singleTapParam.Pos.X;}
    }
    public int SingleScrTapPosY
    {
        get {return singleTapParam.Pos.Y;}
    }

    public int SingleScrTouchPosX
    {
        get {return singleTouchParam.Pos.X;}
    }
    public int SingleScrTouchPosY
    {
        get {return singleTouchParam.Pos.Y;}
    }

    public int DeviceInputId
    {
        get {return useTouch.GetInputId( 0 );}
    }
    public int DevicePosX
    {
        get {return useTouch.GetScrPosX( 0 );}
    }
    public int DevicePosY
    {
        get {return useTouch.GetScrPosY( 0 );}
    }

    /// デバック用
    public DemoGame.InputGamePad Pad
    {
        get {return useGPad;}
    }
    public DemoGame.InputTouch Touch
    {
        get {return useTouch;}
    }



}

} // namespace
