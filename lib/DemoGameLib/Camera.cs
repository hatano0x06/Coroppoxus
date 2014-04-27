/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;

namespace DemoGame{

public class Camera
{
	private const float pi = 3.141593f;

    //    private Matrix4 projMtx;
    //    private Matrix4 viewMtx;
 
	private Vector3 camPos;
	private Vector3 camUp;
	private Vector3 camLookVec;
	private Vector3 lookPosition;
	private Vector3 lookedPos;


	/// コンストラクタ 
    public Camera()
    {
	}


/// public メンバ
///---------------------------------------------------------------------------

	/// 射影行列の生成
    public void SetPerspective( int dspWidth, int dspHeight, float angle, float near, float far  )
    {
        float aspect = (float)dspWidth / (float)dspHeight;
        float fov = angle * (pi / 180.0f);
        this.Projection = Matrix4.Perspective( fov, aspect, near, far );
    }


	/// LookAtの指定からビュー行列の生成
	/**
	 * trgRotの回転値は１周を360.0fとする
	 */
    public void SetLookAt( Vector3 trgRot, Vector3 trgPos, float trgDis )
    {
		float a_Cal1, a_Cal2, a_Cal3;
		float a_Cal		= (float)(pi / 180.0);
		float angleX	= trgRot.X * a_Cal;
		float angleY	= trgRot.Y * a_Cal;
		float angleZ	= trgRot.Z * a_Cal;

		float a_sinx	= FMath.Sin( angleX );
		float a_cosx	= FMath.Cos( angleX );
		float a_siny	= FMath.Sin( angleY );
		float a_cosy	= FMath.Cos( angleY );
		float a_sinz	= FMath.Sin( angleZ );
		float a_cosz	= FMath.Cos( angleZ );
/*
		float a_sinx	= FMath.Sin( trgRot.X );
		float a_cosx	= FMath.Cos( trgRot.X );
		float a_siny	= FMath.Sin( trgRot.Y );
		float a_cosy	= FMath.Cos( trgRot.Y );
		float a_sinz	= FMath.Sin( trgRot.Z );
		float a_cosz	= FMath.Cos( trgRot.Z );
*/
		a_Cal1 = trgDis * a_cosx;
		a_Cal2 = trgDis * a_sinx;
		a_Cal3 = trgDis * a_cosx;

		camPos.X = trgPos.X + ( a_Cal1 * a_siny );
		camPos.Y = trgPos.Y + a_Cal2;
		camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

		camUp.X =  ( a_sinz * a_cosy );
		camUp.Y =  a_cosz;
		camUp.Z = -( a_sinz * a_siny );

		this.View = Matrix4.LookAt( camPos, trgPos, camUp );

        ViewProjection = Projection * View;

		camLookVec = trgPos - camPos;
		camLookVec = camLookVec.Normalize();
	}

    public void SetLookSelf( Vector3 trgRot, Vector3 trgPos, float trgDis, float trgDisX ,float lookselfRate )
    {
		float a_Cal1, a_Cal2, a_Cal3;
		float a_Cal		= (float)(pi / 180.0);
		float angleX	= trgRot.X * a_Cal;
		float angleY	= trgRot.Y * a_Cal;
		float angleZ	= trgRot.Z * a_Cal;

		float a_sinx	= FMath.Sin( angleX );
		float a_cosx	= FMath.Cos( angleX );
		float a_siny	= FMath.Sin( angleY );
		float a_cosy	= FMath.Cos( angleY );
		float a_sinz	= FMath.Sin( angleZ );
		float a_cosz	= FMath.Cos( angleZ );
/*
		float a_sinx	= FMath.Sin( trgRot.X );
		float a_cosx	= FMath.Cos( trgRot.X );
		float a_siny	= FMath.Sin( trgRot.Y );
		float a_cosy	= FMath.Cos( trgRot.Y );
		float a_sinz	= FMath.Sin( trgRot.Z );
		float a_cosz	= FMath.Cos( trgRot.Z );
*/
//		a_Cal1 = trgDis * a_cosx;
//		a_Cal2 = trgDis * a_sinx;
//		a_Cal3 = trgDis * a_cosx;
		
		if(lookselfRate == 1.0f){
			a_Cal1 = -trgDis;
			a_Cal2 = -trgDis * a_sinx;
			a_Cal3 = -trgDis;
				
			camPos.X = trgPos.X + ( a_Cal1 * a_siny );
			camPos.Y = trgPos.Y;
			camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

			lookPosition.X = camPos.X + (camPos.X - trgPos.X) * a_cosx;	
			lookPosition.Y = camPos.Y + trgDisX * a_sinx;
			lookPosition.Z = camPos.Z + (camPos.Z - trgPos.Z) * a_cosx;	
	
				
			lookedPos.X = trgPos.X + (camPos.X - trgPos.X) *3.5f;	
			lookedPos.Y = camPos.Y + trgDisX * a_sinx;
			lookedPos.Z = trgPos.Z + (camPos.Z - trgPos.Z) *3.5f;	
		}else if(lookselfRate == 0.0f){
			a_Cal1 = -trgDis * a_cosx;
			a_Cal2 = -trgDis * a_sinx;
			a_Cal3 = -trgDis * a_cosx;
				
			camPos.X = trgPos.X + ( a_Cal1 * a_siny );
			camPos.Y = trgPos.Y + a_Cal2;
			camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

			lookPosition.X = trgPos.X;	
			lookPosition.Y = trgPos.Y;
			lookPosition.Z = trgPos.Z;	
		}else{
			a_Cal1 = -trgDis * ((a_cosx-1)*(1-lookselfRate/2) +1);
			a_Cal2 = -trgDis * a_sinx;
			a_Cal3 = -trgDis * ((a_cosx-1)*(1-lookselfRate/2) +1);;
	
			camPos.X = trgPos.X + ( a_Cal1 * a_siny );
			camPos.Y = trgPos.Y + (a_Cal2 * (1-lookselfRate/2));
	//		camPos.Y = trgPos.Y + a_Cal2;
			camPos.Z = trgPos.Z + ( a_Cal3 * a_cosy );

			lookPosition.X = trgPos.X;
			lookPosition.Z = trgPos.Z;
			lookPosition.Y = trgPos.Y;
				/*
			lookPosition.X = trgPos.X * (1-lookselfRate) + lookedPos.X * lookselfRate;	
			lookPosition.Z = trgPos.Z * (1-lookselfRate) + lookedPos.Z * lookselfRate;	
			lookPosition.Y = trgPos.Y * (1-lookselfRate) + lookedPos.Y * lookselfRate;	
			*/
		}
			
		camUp.X =  ( a_sinz * a_cosy );
		camUp.Y =  a_cosz;
		camUp.Z = -( a_sinz * a_siny );
			
		this.View = Matrix4.LookAt(camPos, lookPosition, camUp );

        ViewProjection = Projection * View;

		camLookVec = camPos - lookPosition;
		camLookVec = camLookVec.Normalize();
	}

/// プロパティ
///---------------------------------------------------------------------------

    public Matrix4 Projection;
    public Matrix4 View;
    public Matrix4 ViewProjection;

    public Vector3 Pos
    {
        get{ return camPos; }
	}
    public Vector3 LookVec
    {
        get{ return camLookVec; }
	}

}
} // end ns DemoGame
