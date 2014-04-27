/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Tutorial.Utility;


namespace AppRpg {

///***************************************************************************
/// OBJ:モンスター
///***************************************************************************
public class ObjChSunMoon : GameObjProduct
{
    public  Vector3                     BodyPos;
    public  Vector2                     texCenter;
	
    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
    private ShapeCapsule                shapeCollforDis;

	private UnifiedTextureInfo textureInfo;

	SetupModelDataList            		dataList = new SetupModelDataList();
	GameCtrlManager           			ctrlResMgr    = GameCtrlManager.GetInstance();
		

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChSunMoon(int TexId){
		this.TexId = TexId;
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
		scale = (int)Data.SetupValue.TowerScale /100.0f;	
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
			
/*        if( useMdlHdl != null ){
            useMdlHdl.Term();
        }
*/
        shapeMove = null;
        shapeColl = null;
        shapeCollforDis = null;
    }

    /// 開始
    public override bool DoStart()
    {
//			"/Application/res/data/2D/" + dataList.TexFileNameList[mdlTexId,i]
//		Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
		if(TexId == (int)Data.Tex2dResId.Bosstower){
			changeTex(TexId);					
		}else{
			changeTex((int)Data.Tex2dResId.MakingWall1);								
		}
		DoUpdateMatrix();
        return true;
    }

    public  bool startforChange(int texnumber)
    {
		changeTex(texnumber);			
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
		if(TexId+(int)Data.Tex2dResId.TowerStart != (int)Data.Tex2dResId.Bosstower){
			if(glowNumber ==3){
				glowCounter = (int)Data.SetupValue.ToNextStageCount*3;
				glowFinish = true;
			}
			if(glowCounter >= (int)Data.SetupValue.ToNextStageCount*3){
				if(glowCounter == (int)Data.SetupValue.ToNextStageCount*3 && glowNumber == 2){
					changeFlag = true;
					AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeMo, BodyPos );
	
					glowNumber++;
				}else if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
					changeFlag = false;
					startforChange(TexId);
				}
			}else if(glowCounter >= (int)Data.SetupValue.ToNextStageCount*2){
				if(glowCounter == (int)Data.SetupValue.ToNextStageCount*2 && glowNumber == 1){
					changeFlag = true;
					AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeTower1, BodyPos );
					glowNumber++;
				}else if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
					changeFlag = false;
					startforChange((int)Data.Tex2dResId.MakingWall3);
				}
			}else if(glowCounter >= (int)Data.SetupValue.ToNextStageCount){
				if(glowCounter == (int)Data.SetupValue.ToNextStageCount && glowNumber == 0){
					changeFlag = true;
					AppSound.GetInstance().PlaySeCamDis( AppSound.SeId.MakeTower2, BodyPos );
					glowNumber++;
				}else if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
					changeFlag = false;
					startforChange((int)Data.Tex2dResId.MakingWall2);
				}
			}
		}
			
//        useMdlHdl.UpdateAnim();
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
			
		if(cameraPosition.Z < myPosition.Z){
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI;
		}else{ 
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z);
		}
			
		if(deadflag == true || changeFlag == true || farFlag == true){
			if(appearCount < (int)Data.SetupValue.AppearAndLeaveTime){		
				appearCount++;					
			}else{
				if(deadflag == true){	//最初の状態に戻してる
					deadflag = false;
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
        return 2;
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
		shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 20.0f );
        shapeCollforDis.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );

			/*        switch( chTypeId ){
        case Data.ChTypeId.TowerA: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.TowerB: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.TowerC: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        }
        */
    }

	public int texId{
		get{
			if( this.TexId != (int)Data.Tex2dResId.Bosstower ){
				return this.TexId-(int)Data.Tex2dResId.TowerStart;
			}else{
				return 0;						
			}
		}
	}	
	
	public override int GetTexId(){
		if( this.TexId != (int)Data.Tex2dResId.Bosstower ){
			return this.TexId-(int)Data.Tex2dResId.TowerStart;
		}else{
			return 0;						
		}			
	}
		
	public void changeTex(int Id){
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		textureInfo = 		resMgr.SetTexture2(Id);
		
		uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
		uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
		texSize = new Vector2(textureInfo.w/10,textureInfo.h/10);						
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
