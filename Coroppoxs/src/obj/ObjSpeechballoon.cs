using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Tutorial.Utility;

namespace AppRpg
{
	public class ObjSpeechballoon : GameObjProduct
	{
    public  Vector2                     texCenter;
	public int	 						timeCount;
	public int	 						TexIdForText;
	public bool							preDeadflag;

	public ObjText						ObjTex;

    /// 形状
	private UnifiedTextureInfo textureInfo;

	SetupModelDataList            dataList = new SetupModelDataList();
	GameCtrlManager            	　　ctrlResMgr    = GameCtrlManager.GetInstance();		
		

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjSpeechballoon(int ToTextId, int TexId){
		this.TexId = TexId;
		TexIdForText = ToTextId;
	}
			
    /// 初期化
    public override bool DoInit()
    {
		timeCount = 0;
		deadFlag = true;
		preDeadflag = true;
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
			scale = (int)Data.SetupValue.BallonScale/100.0f;	
			ObjTex = new ObjText(TexIdForText);
			ObjTex.Init();
		}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){
			scale = (int)Data.SetupValue.GionScale/100.0f;
			appearCount = (int)Data.SetupValue.GionAppearSpeed;
		}
		return true;
    }

    /// 破棄
    public override void DoTerm()
    {
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
//			ObjTex.Term();
		}
    }

    /// 開始
    public override bool DoStart()
    {
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
			textureInfo = 		resMgr.SetTexture2(TexId+(int)Data.Tex2dResId.SpeechBalloon1);
			ObjTex.Start();
		}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){
			textureInfo = 		resMgr.SetTexture2((int)Data.Tex2dResId.SpeechBalloon3);
		}
		uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
		uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
			texSize = new Vector2(textureInfo.w/10*ObjTex.scaleX,textureInfo.h/10);
		}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){
			texSize = new Vector2(textureInfo.w/10,textureInfo.h/10);
		}
			
        DoUpdateMatrix();
        return true;
    }

    /// 終了
    public override void DoEnd()
    {
//        useMdlHdl.End();
    }

    /// フレーム処理
    public override bool DoFrame()
    {
		if(appearCount == 0)　timeCount++;
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
			if(timeCount > (int)Data.SetupValue.BaloonAppearTime){
				preDeadflag = true;		
			}
		}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){
			if(timeCount > (int)Data.SetupValue.GionAppearTime){
				preDeadflag = true;		
			}
		}
        return true;
    }

    /// 描画処理
    public override bool DoDraw2( DemoGame.GraphicsDevice graphDev ,Vector3 BasePos )
    {
        CalcSpriteMatrix(graphDev.GetCurrentCamera().Pos ,BasePos);
		BasePos.Y += 3.0f;
		ctrlResMgr.SetSpriteData(BasePos,rotation,uvPos,uvSize,texSize);
		BasePos.Y -= 3.0f;
			
		return true;
    }
		
	public bool DrawText(DemoGame.GraphicsDevice graphDev ,Vector3 BasePos){
		if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
				/*
			if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && deadflag2 == false){
				ObjTex.changeText();
			}*/
			ObjTex.Draw2(graphDev, BasePos);
		}
			
		return true;
	}

	private void CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
			/*
		if(TexIdForText < 3){
			myPosition.Y += 3.0f;
		}else if(TexIdForText == 3){
			myPosition.Y += 6.0f;	
			myPosition.X += 2.0f;	
			myPosition.Z += 1.0f;	
		}
		*/
			
		var subPosition = cameraPosition - myPosition;		
		if(cameraPosition.Z < myPosition.Z){
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI;
		}else{ 
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z);
		}
			
		if(preDeadflag == true){
			appearCount++;
			if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
				if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && deadFlag == false){
					deadFlag = true;
					ctrlResMgr.CtrlHobit.speakCount--;		
				}
			}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){	
				if(appearCount == (int)Data.SetupValue.GionAppearSpeed && deadFlag == false){
					deadFlag = true;
					ctrlResMgr.CtrlHobit.speakCount--;
				}
			}				
		}else{
			if(appearCount > 0) appearCount--;								
		}
							
		if(appearCount != 0){		
			if(TexIdForText < (int)Data.Tex2dResId.NormalCharMax){
				rotation.Y = FMath.PI/2*appearCount/(int)Data.SetupValue.AppearAndLeaveTime;	
				ObjTex.setangle(FMath.PI/2*appearCount/(int)Data.SetupValue.AppearAndLeaveTime);
			}else if(TexIdForText == (int)Data.Tex2dResId.Zonbi1){
				rotation.Y = FMath.PI/2*appearCount/(int)Data.SetupValue.GionAppearSpeed;
			}
		}			
	}		

			
    
/// public メソッド
///---------------------------------------------------------------------------


/// private メソッド
///---------------------------------------------------------------------------

	}
}

