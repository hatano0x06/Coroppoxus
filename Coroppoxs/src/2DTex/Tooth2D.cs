using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.UI;
using  Tutorial.Utility;

namespace AppRpg 
{
	public class Tooth2D
	{
	    private Texture2D                uppertoothimage;
	    private Texture2D                undertoothimage;
	    private Texture2D                LifeGauge;

		public  DemoGame.Sprite 			 sprituppertooth;
		public  DemoGame.Sprite 			 spritundertooth;
		public  DemoGame.Sprite 			 spritLifeGauge;
		float UnderY;
		float UpperY;		
		
		public void Init(){
			GameCtrlManager ctrlResMgr = GameCtrlManager.GetInstance();
	        DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;			
			uppertoothimage = new Texture2D("/Application/res/data/2Dtex/uppertooth.png", false );
			undertoothimage = new Texture2D("/Application/res/data/2Dtex/undertooth.png", false );	
			LifeGauge = new Texture2D("/Application/res/data/2Dtex/Life_Gauge.png", false );	

			sprituppertooth = new DemoGame.Sprite(uppertoothimage , 0.0f,0.0f,0.0f,8.0f);
			spritundertooth = new DemoGame.Sprite(undertoothimage , 0.0f,0.0f,0.0f,8.0f);
			spritLifeGauge = new DemoGame.Sprite(LifeGauge , 0.0f,0.0f,0.0f,1.5f);
		}
		
		public void Render(){
			//DemoGame.GraphicsDevice useGraphDev = ctrlResMgr.GraphDev;
			DemoGame.Graphics2D.DrawSprite(sprituppertooth,(int)uppertoothimage.Width*3,(int)UpperY+uppertoothimage.Height*8/2,1.0f);			
			DemoGame.Graphics2D.DrawSprite(spritundertooth,(int)undertoothimage.Width*3,(int)UnderY+undertoothimage.Height*8/2-40,1.0f);			
			DemoGame.Graphics2D.DrawSprite(spritLifeGauge,(int)60,(int)40,1.0f);			
		}
		
		public void Term(){
			uppertoothimage.Dispose();
			undertoothimage.Dispose();
			LifeGauge.Dispose();
			spritundertooth = null;
			sprituppertooth = null;
			spritLifeGauge = null;
		}
		
		public float underY
		{
			set{this.UnderY =value;}			
		}

		public float upperY
		{
			set{this.UpperY =value;}			
		}
	}
}

