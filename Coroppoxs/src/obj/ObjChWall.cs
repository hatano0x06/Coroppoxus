/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Tutorial.Utility;



namespace AppRpg {

///***************************************************************************
/// OBJ:モンスター
///***************************************************************************
public class ObjChWall : GameObjProduct
{
    public  Vector3                     BodyPos;
    public  Vector2                     texCenter;

	public Vector3	 					pos1;
	public Vector3	 					pos2;
	public float						distance;
	public float						angle;	

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
	private ShapeCapsule                shapeCollforDis;

	private UnifiedTextureInfo textureInfo;

	SetupModelDataList            		dataList = new SetupModelDataList();
	GameCtrlManager           			ctrlResMgr    = GameCtrlManager.GetInstance();
	

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChWall(int TexId, Vector3 pos1 ,Vector3 pos2){
		this.TexId = TexId;
		this.pos1 = pos1;
		this.pos2 = pos2;
	}
			
    /// 初期化
    public override bool DoInit()
    {
        shapeMove = null;

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, StaticDataList.getVectorZero(), 0.4f );

        shapeColl = new ShapeCapsule();
        shapeColl.Init(1);
        shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), (float)Data.SetupValue.TitanSize );
			
		shapeCollforDis = new ShapeCapsule();
        shapeCollforDis.Init(1);
        shapeCollforDis.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), (float)Data.SetupValue.TouchSize );
			
		glowCounter = 0;
					
		distance = Common.VectorUtil.DistanceXZ( pos1 ,pos2 );
		angle = -FMath.Atan((pos1.X-pos2.X)/(pos1.Z-pos2.Z)) +FMath.PI/2;

        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( shapeMove != null ){
            shapeMove.Term();
        }
        if( shapeColl != null ){
            shapeColl.Term();
        }
        if( shapeCollforDis != null ){
            shapeCollforDis.Term();
        }

        shapeMove = null;
        shapeColl = null;
		shapeCollforDis = null;
    }

    /// 開始
    public override bool DoStart()
    {
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();

		if(TexId != (int)Data.Tex2dResId.BossWall){
			changeTex((int)Data.Tex2dResId.Gareki);			
		}else{
			changeTex((int)Data.Tex2dResId.BossWall);			
		}
        DoUpdateMatrix();
        return true;
    }

    public  bool startforChange(int texnumber)
    {
		changeTex(TexId);			
        DoUpdateMatrix();

        return true;
    }		
		
    /// 終了
    public override void DoEnd()
    {

    }

    /// フレーム処理
    public override bool DoFrame()
    {
		if(TexId != (int)Data.Tex2dResId.BossWall){
			if(glowNumber == 2){
				glowCounter = (int)Data.SetupValue.ToNextStageCount*3;
				glowFinish = true;
			}
			if(glowCounter >= (int)Data.SetupValue.ToNextStageCount*2){
				if(glowCounter == (int)Data.SetupValue.ToNextStageCount*2 && glowNumber == 1){ 
					changeFlag = true;
					AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeMo, BodyPos );
					glowNumber++;
				}
				if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
					changeFlag = false;
					startforChange(TexId);
				}
			}else if(glowCounter >= (int)Data.SetupValue.ToNextStageCount){
				if(glowCounter == (int)Data.SetupValue.ToNextStageCount && glowNumber == 0){
					changeFlag = true;
					AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeTower1, BodyPos );
					glowNumber++;
				}
				if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
					changeFlag = false;
					startforChange((int)Data.Tex2dResId.GarekiWall);
				}
			}
		}
        return true;
    }

    /// 描画処理
    public override bool DoDraw2( DemoGame.GraphicsDevice graphDev ,Vector3 BasePos )
    {

        CalcSpriteMatrix(graphDev.GetCurrentCamera().Pos ,BasePos);
		ctrlResMgr.SetSpriteData(BasePos,rotation,uvPos,uvSize,texSize);

        return true;
    }

	private void CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
		var subPosition = cameraPosition - myPosition;
		Common.VectorUtil.Set( ref BodyPos, myPosition.X, myPosition.Y, myPosition.Z );		
			
		rotation.X = angle;
			
		if(deadFlag == true || changeFlag == true || farFlag == true){
			if(appearCount < (int)Data.SetupValue.AppearAndLeaveTime){		
				appearCount++;					
			}else{
				if(deadFlag == true){
					deadFlag = false;
					glowFinish = false;
					glowNumber = 0;
					glowCounter = 0;	
					startforChange((int)Data.Tex2dResId.MakingWall1);
				}
				appearCount = (int)Data.SetupValue.AppearAndLeaveTime;	
			}
		}else{
			if(appearCount > 0){		
				appearCount--;								
			}else{
				appearCount = 0;	
			}
		}

		rotation.Y = FMath.PI/2*appearCount/(int)Data.SetupValue.AppearAndLeaveTime;
	}
    
	/// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        shapeMove.SetMult( this.baseMtx );
        shapeColl.SetMult( this.baseMtx );
		shapeCollforDis.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }

    /// モデルハンドルを返す
    /*
    public override Common.ModelHandle GetModelHdl()
    {
//        return useMdlHdl;
    }*/

    /// 移動対象かどうか
    public override int CheckMoveTrgId()
    {
        return 3;
    }


/// public メソッド
///---------------------------------------------------------------------------
/*
    /// ボーンの姿勢を取得    
    public Matrix4 GetBoneMatrix( int boneId )
    {
//        return useMdlHdl.GetBoneMatrix( boneId );
    }
		 */
    /// モデルのセット
    public void SetMdlHandle( Data.ChTypeId chTypeId )
    {

	//	shapeColl.Set( 0, StaticDataList.getVectorZero(), 0.001f );			
		shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 20.0f );
/*        switch( chTypeId ){
        case Data.ChTypeId.WallA: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.WallB: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.WallC: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        }
        */
    }
		
	public int texId{
		get{
			if( this.TexId != (int)Data.Tex2dResId.BossWall ){
				return this.TexId-(int)Data.Tex2dResId.WallStart;
			}else{
				return 0;						
			}
		}
	}	
	
	public override int GetTexId(){
		if( this.TexId != (int)Data.Tex2dResId.BossWall ){
			return this.TexId-(int)Data.Tex2dResId.WallStart;
		}else{
			return 0;						
		}			
	}	

	public void changeTex(int Id){
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		textureInfo = 		resMgr.SetTexture2(Id);
		
		uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
		uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
		texSize = new Vector2(distance,textureInfo.h/10);	
//		texSize = new Vector2(distance*20/textureInfo.w,distance*10/textureInfo.h);						
//		scale = distance/(textureInfo.w/10);
	}
		
/// private メソッド
///---------------------------------------------------------------------------



/// 形状関連
///---------------------------------------------------------------------------

    /// 移動形状
    public override GameShapeProduct GetMoveShape()
    {
        return shapeMove;
    }
    public override int GetMoveShapeMax()
    {
        return 1;
    }

    /// 衝突形状
    public override GameShapeProduct GetCollisionShape( Data.CollTypeId type, int no )
    {	
		if(type == Data.CollTypeId.ChDestination){
			return shapeCollforDis;
		}
        return shapeColl;
    }
    public override int GetCollisionShapeMax( Data.CollTypeId type )
    {
        return 1;
    }
}

} // namespace
