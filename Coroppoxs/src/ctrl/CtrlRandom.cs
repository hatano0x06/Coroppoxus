using System;

namespace AppRpg 
{
	public static class CtrlRandom
	{
		private static Random rand = new System.Random();
	
		public static int getRandom(int underNumber , int upperNumber){
			return rand.Next (underNumber,upperNumber);		
		}

		public static int getRandom(int upperNumber){
			return rand.Next (0,upperNumber);		
		}

	}
}

