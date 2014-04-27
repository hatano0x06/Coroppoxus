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
/// シーン：ゲームタイトル
///***************************************************************************
public class SceneTitle : DemoGame.IScene
{

    private DemoGame.SceneManager        useSceneMgr;
    private int                          taskId;
    private EveStateId                   eventState;
	private bool 						 fadeFlag;
	private int 						 fadeCount;

    ///---------------------------------------------------------------------------
    /// 入力イベントID
    ///---------------------------------------------------------------------------
    public enum EveStateId{
        GameStart   = (1 << 0),        /// ゲームスタート
    };



/// public メソッド
///---------------------------------------------------------------------------

    /// シーンの初期化
    public bool Init( DemoGame.SceneManager sceneMgr )
    {
        taskId      = 0;
        useSceneMgr = sceneMgr;
		eventState	= 0;

        AppLyout.GetInstance().ClearSpriteAll();
//        AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Logo );

        GameCtrlManager.GetInstance().Start();

        /// 配置情報のセット
        SetupObjPlaceData.Load();

        AppDispEff.GetInstance().SetFadeIn( 0xffffff, 5, true );
			
		fadeFlag = false;
		fadeCount = 0;

        return true;
    }

    /// シーンの破棄
    public void Term()
    {
        GameCtrlManager.GetInstance().End();

        AppLyout.GetInstance().ClearSpriteAll();
        useSceneMgr = null;
    }

    /// シーンの継続切り替え時の再開処理
    public bool Restart()
    {
        taskId      = 0;
		eventState	= 0;

        AppLyout.GetInstance().ClearSpriteAll();
        AppLyout.GetInstance().SetSprite( AppLyout.SpriteId.Logo );

        return true;
    }

    /// シーンの継続切り替え時の停止処理
    public bool Pause()
    {
        return true;
    }

    /// サスペンド＆レジューム処理
    public void Suspend()
    {
    }
    public void Resume()
    {
    }

    /// フレーム処理
    public bool Update()
    {
        switch( taskId ){

        case 0:
            if( AppDispEff.GetInstance().NowEffId != AppDispEff.EffId.FadeIn ){
                taskId ++;
            }
            break;

        case 1:

			checkInputButtons();

			if( eventState != 0 ){
				if( (eventState & EveStateId.GameStart) != 0 ){
					AppDispEff.GetInstance().SetFadeOut( 0xffffff, 10, true );
					fadeFlag = true;
				}
  	            taskId ++;
   	        }
            break;

        case 2:
            if( AppDispEff.GetInstance().NowEffId != AppDispEff.EffId.FadeOut ){
				if( (eventState & EveStateId.GameStart) != 0 ){
	                useSceneMgr.Next( ( new SceneGameMain() ), false );
				}
			}
            break;
        }

       // GameCtrlManager.GetInstance().FrameTitle();

        return true;
    }

    /// 描画処理
    public bool Render()
    {
        GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();

        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;

        useGraphDev.Graphics.SetClearColor( 0.9f, 0.9f, 0.9f, 0.0f ) ;
        useGraphDev.Graphics.Clear() ;
        AppLyout.GetInstance().ClearSpriteAll();
		if(fadeFlag == true){
			fadeCount++;
			if(fadeCount > 20) fadeCount = 20;
		}
        AppLyout.GetInstance().RenderTitle(fadeCount);

        //AppDispEff.GetInstance().Draw( useGraphDev );

        useGraphDev.Graphics.SwapBuffers();

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 入力イベントのチェック
    private void checkInputButtons()
    {
		int devPosX = AppInput.GetInstance().DevicePosX;
		int devPosY = AppInput.GetInstance().DevicePosY;

		eventState	= 0;

		/// ゲームスタートチェック
     	if( AppInput.GetInstance().CheckDeviceSingleTouchDown() == true ){
			eventState = EveStateId.GameStart;
		}
    }
}

} // namespace
