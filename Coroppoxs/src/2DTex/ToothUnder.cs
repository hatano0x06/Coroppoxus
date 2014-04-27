using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using  Tutorial.Utility;

namespace AppRpg 
{
	public class ToothUnder
	{
		private UnifiedTextureInfo 			textureInfo;
		Scene2dTex	           				ctrlResMgr    = Scene2dTex.GetInstance();
		
		private Vector2 Pos;
		private Vector2 uvPos;
		private Vector2 uvSize;
		private Vector2 texSize;

		public void Init(){
			Data.ModelDataManager 	resMgr = Data.ModelDataManager.GetInstance();
			textureInfo = 		resMgr.SetTexture2((int)Data.Tex2dResId.Undertooth);
			uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
			uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
			texSize = new Vector2(textureInfo.w,textureInfo.h)*8.0f;
			Pos.X = 450;
			Pos.Y = -200;
		}
		
		public void Render(){
			ctrlResMgr.SetSpriteData(Pos,0,uvPos,uvSize,texSize);			
		}
		
		public void Term(){
		}
		
		public float posY
		{
			set{this.Pos.Y =value;}			
		}
	}
}

