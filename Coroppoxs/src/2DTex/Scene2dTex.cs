 

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using  Tutorial.Utility;

namespace AppRpg 
{
	public class Scene2dTex
	{
		private ToothUpper toothUpper;
		private ToothUnder toothUnder;
		private LifeGuage Life;
	    private Queue< Hp2dTex >    HpList;
		private float hpNumber;
		private static Scene2dTex instance = new Scene2dTex();
		
		public Sprite2DBatch batch ;
		public Sprite2DMaterial material ;
		public Sprite2D[] sprites ;
		public int spriteCount ;
		public float spriteSize = 24.0f ;
		public float spriteSpin = 1.0f ;
		public int spriteCounter = 0;
		public bool deadflag = false;
		
		
	    /// インスタンスの取得
	    public static Scene2dTex GetInstance()
	    {
	        return instance;
	    }

		public void init(GraphicsContext graphics){
			InitSpriteBatch( 30, graphics ) ;
			SetSpriteCount( 30 ) ;
			
			toothUpper = new ToothUpper();
			toothUpper.Init ();
			toothUnder = new ToothUnder();
			toothUnder.Init ();
			Life = new LifeGuage();
			Life.Init ();
	        HpList = new Queue<Hp2dTex>();
			HpList.Enqueue(new Hp2dTex(1 , 65  ,60 , (65-25)/1));
	        HpList.Enqueue(new Hp2dTex(1 , 100 ,60 , (100-25)/2));
	        HpList.Enqueue(new Hp2dTex(1 , 135 ,60 , (135-25)/3));
	        HpList.Enqueue(new Hp2dTex(1 , 170 ,60 , (170-25)/4));
	        HpList.Enqueue(new Hp2dTex(2 , 205 ,60 , (205-25)/5));
	        HpList.Enqueue(new Hp2dTex(2 , 220 ,30 , (220-25)/6));
		}
		
		public void frame(){
			foreach(var HpTexture in HpList){
				HpTexture.frame();						
			}
		}
		
		public void Draw(){
			ClearSpriteCounter();
			int SpriteNumber = HpList.Count+3;
			if(SpriteNumber > 30) SpriteNumber = 30;
			SetSpriteCount(SpriteNumber);
									
			toothUpper.Render();
			toothUnder.Render();
			Life.Render();
			//回転中心がずれているｐっぽい　修正
			foreach(var HpTexture in HpList){
				if(HpTexture.deadFlag == true){
					deadflag = true;
				}else{
					HpTexture.Render();
				}
			}
			
			if(deadflag == true) HpList.Dequeue();
			deadflag = false;

			for ( int i = 0 ; i < spriteCount ; i ++ ) {
				sprites[ i ].UpdatePosTex();
			}
			batch.Draw() ;
		}

		public void Term(){
			foreach(var HpTexture in HpList){
				HpTexture.Term();		
			}
			HpList = null;
		}

		public void SetUnderPos(int posY){
			toothUnder.posY = posY;
		}	

		public void SetUpperPos(int posY){
			toothUpper.posY = posY;
		}

		public void SetHp(float hpNumber){
			this.hpNumber = hpNumber;
		}

		public void AddHp(int Id){
			switch(Id){
				case (int)Data.Tex2dResId.Noumin1: 		HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.Senshi1: 		HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.Souryo1: 		HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+2.0f) ));	break;
				case (int)Data.Tex2dResId.Zonbi1: 		HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.Necromancer1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				
				case (int)Data.Tex2dResId.TowerStart:	HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiTower1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiTower2: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoTower1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoTower2: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;

				case (int)Data.Tex2dResId.NouminHouse1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.NouminHouse2: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.NouminHouse3: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.NouminHouse4: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.NouminHouse5: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiHouse1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiHouse2: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiHouse3: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiHouse4: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SenshiHouse5: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoHouse1: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoHouse2: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoHouse3: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoHouse4: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;
				case (int)Data.Tex2dResId.SouryoHouse5: HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+1.0f) ));	break;				

				case (int)Data.Tex2dResId.Gareki:		HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+0.5f) ));	break;	
				case (int)Data.Tex2dResId.GarekiWall:	HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+0.5f) ));	break;	
				case (int)Data.Tex2dResId.MakingWall1:	HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+0.5f) ));	break;	
				case (int)Data.Tex2dResId.MakingWall2:	HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+0.5f) ));	break;	
				case (int)Data.Tex2dResId.MakingWall3:	HpList.Enqueue(new Hp2dTex(Id , 220 ,30 ,(220-25)/(hpNumber+0.5f) ));	break;	
			
			}			
		}

		public void InitSpriteBatch( int maxSpriteCount , GraphicsContext graphics )
		{
			batch = new Sprite2DBatch( graphics, maxSpriteCount ) ;
			material = new Sprite2DMaterial( new Texture2D( "/Application/res/data/2Dtex/unifiedTexture.png", true ) ) ;
			material.Texture.SetFilter( TextureFilterMode.Linear, TextureFilterMode.Linear,
										TextureFilterMode.Nearest ) ;
	
			sprites = new Sprite2D[ maxSpriteCount ] ;
			spriteCount = 0 ;
		}
	
		public void SetSpriteCount( int count )
		{
			count = Math.Min( Math.Max( count, 1 ), sprites.Length ) ;
			if ( count > spriteCount ) {
				for ( int i = spriteCount ; i < count ; i ++ ) {
					sprites[ i ] = new Sprite2D( batch, material, 0 ) ;
					InitSprite( sprites[ i ] ) ;
				}
			} else {
				for ( int i = count ; i < spriteCount ; i ++ ) {
					batch.RemoveSprite( sprites[ i ] ) ;
				}
			}
			spriteCount = count ;
		}
	
		public void SetSpriteSize( float size )
		{
			size = FMath.Clamp( size, 4.0f, 512.0f ) ;
			for ( int i = 0 ; i < spriteCount ; i ++ ) {
				sprites[ i ].Size.X = sprites[ i ].Size.Y = size ;
			}
			spriteSize = size ;
		}
	
		public void InitSprite( Sprite2D s )
		{
			s.Position.X = -100 ;
			s.Position.Y = -100 ;
			s.Direction = 0;
			s.Size = new Vector2( spriteSize, spriteSize ) ;
			s.Center = new Vector2( 0.5f, 0.5f ) ;
			s.UVOffset = new Vector2( 1, 1 ) * 0.25f ;
			s.UVSize = new Vector2( 0.25f, 0.25f ) ;
			s.Color = new Rgba( 255, 255, 255, 255 ) ;
	
			s.UpdateAll() ;
		}
		
		public void SetSpriteData(Vector2 pos, float rot, Vector2 UVpos, Vector2 UVsize, Vector2 size)
		{
			if(spriteCounter > spriteCount) return;
				
			sprites[spriteCounter].updateFlag =true;
			sprites[spriteCounter].Position = pos;
			sprites[spriteCounter].Direction = rot;
			sprites[spriteCounter].UVOffset = UVpos;
			sprites[spriteCounter].UVSize = UVsize;
			sprites[spriteCounter].Size = size;
			spriteCounter++;
		}
		
		public void ClearSpriteCounter(){
			spriteCounter = 0;			
		}
		
		public int GetObjNumber(){
			return HpList.Count+3;
		}
		
	}
}

