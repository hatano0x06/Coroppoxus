using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using  Tutorial.Utility;

namespace AppRpg 
{
	public class Hp2dTex
	{
		private UnifiedTextureInfo 			textureInfo;
		Scene2dTex	           				ctrlResMgr    = Scene2dTex.GetInstance();
		private float rotate = 0;
		private float rotatespeed;
		private int Id;
		private Vector2 Pos;
		private float speed;
		private Vector2 uvPos;
		private Vector2 uvSize;
		private Vector2 texSize;
		public bool deadFlag;
		
		public Hp2dTex (int Id , float posX , float posY, float speed)
		{
			this.Id = Id;						
			Data.ModelDataManager 	resMgr = Data.ModelDataManager.GetInstance();
			if(Id == (int)Data.Tex2dResId.TowerStart){
				textureInfo = resMgr.SetTexture2((int)Data.Tex2dResId.Bosstower);			
			}else if(Id == (int)Data.Tex2dResId.HouseStart){
				textureInfo = resMgr.SetTexture2((int)Data.Tex2dResId.BossMonument);							
			}else if(Id == (int)Data.Tex2dResId.WallStart){
				textureInfo = resMgr.SetTexture2((int)Data.Tex2dResId.BossWall);						
			}else{
				textureInfo = 		resMgr.SetTexture2((int)Id);
			}
			
			uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
			uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
			texSize = new Vector2(textureInfo.w,textureInfo.h)*1.5f;
			
			//randomにする
			rotate = StaticDataList.getRandom(0,360);
			rotatespeed = StaticDataList.getRandom(3,6)/100.0f;
			Pos.X = posX;
			Pos.Y = posY;
			this.speed = speed;
			deadFlag = false;
		}
		
		public void frame(){
			Pos.X -= speed/1000.0f;
			rotate += rotatespeed;
			if(Pos.X < 40){
//				Pos.Y -= rotateHp*25.0f;
				Pos.Y = 60+(40 - Pos.X)/13.0f*30;
				texSize.X = (Pos.X - 27)/13.0f*textureInfo.w*1.5f;
				texSize.Y = (Pos.X - 27)/13.0f*textureInfo.h*1.5f;
				if(texSize.X < 0){
					texSize.X = 0;
					deadFlag = true;
				}else if(texSize.Y < 0){
					texSize.Y = 0;
					deadFlag = true;
				}
			}else{
				if(Pos.Y < 60){
					Pos.Y += 0.5f;		
				}else{
					Pos.Y = 60.0f;						
				}
			}			
		}
		
		public void Render(){
//			spriteHp.ScaleX = 1.7f*scale;
//			spriteHp.ScaleY = 1.7f*scale;
			ctrlResMgr.SetSpriteData(Pos,rotate,uvPos,uvSize,texSize);
		}
		
		public void Term(){
			
		}
	}
}

