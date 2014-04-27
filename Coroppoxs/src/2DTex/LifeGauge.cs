using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using  Tutorial.Utility;

namespace AppRpg 
{
	public class LifeGuage
	{
		private UnifiedTextureInfo 			textureInfo;
		Scene2dTex	           				ctrlResMgr    = Scene2dTex.GetInstance();
//		GameCtrlManager            ctrlResMgr    = GameCtrlManager.GetInstance();
		
		private Vector2 Pos;
//		private Vector3 Pos;
//		private Vector3 rot;
		
		private Vector2 uvPos;
		private Vector2 uvSize;
		private Vector2 texSize;

		public void Init(){
			Data.ModelDataManager 	resMgr = Data.ModelDataManager.GetInstance();
			textureInfo = 		resMgr.SetTexture2((int)Data.Tex2dResId.Life);
			uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
			uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
			texSize = new Vector2(textureInfo.w,textureInfo.h)*1.55f;
			
			/*
			rot.X = 0.0f;
			rot.Y = 0.0f;
			rot.Z = 0.0f;
			*/
			Pos.X = 125;
			Pos.Y = 60;
		}
		
		public void Render(){
			ctrlResMgr.SetSpriteData(Pos,0,uvPos,uvSize,texSize);
			/*
			Pos = ctrlResMgr.CtrlCam.GetCamPos();
			float angleX = ctrlResMgr.CtrlCam.GetCamRotX()/180.0f*FMath.PI;
			float angleY = -ctrlResMgr.CtrlCam.GetCamRotY()/180.0f*FMath.PI - FMath.PI/2 ;
			Pos.X += 100.0f*FMath.Cos (angleY)*FMath.Cos (angleX);
			Pos.Y += 100.0f*FMath.Sin (angleX);
			Pos.Z += 100.0f*FMath.Sin (angleY)*FMath.Cos (angleX);
			rot.X = -ctrlResMgr.CtrlCam.GetCamRotY()/180*FMath.PI;
			rot.Y = -ctrlResMgr.CtrlCam.GetCamRotX()/180*FMath.PI;
			ctrlResMgr.SetSpriteData(Pos,rot,uvPos,uvSize,texSize);			
			*/
		}
		
		public void Term(){
		}
		
		public float upperY
		{
			set{this.Pos.Y =value;}			
		}
		
		
	}
}

