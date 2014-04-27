using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace AppRpg 
{
	public static class StaticDataList
	{
		private static Random rand = new System.Random();
		public static Texture2D textureUnified = new Texture2D("/Application/res/data/2Dtex/unifiedTexture.png", false);
		public static ShaderProgram spriteShader = new ShaderProgram("/Application/shaders/Texture.cgx");
		public static Vector3 VectorZero = new Vector3(0,0,0);

		public static int getRandom(int underNumber , int upperNumber){
			return rand.Next (underNumber,upperNumber);		
		}

		public static int getRandom(int upperNumber){
			return rand.Next (0,upperNumber);		
		}
		
		public static Vector3 getVectorZero(){
			return VectorZero;
		}
		
		public static int random100(){
			return rand.Next(100);
		}
		
		
	}
}

