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
public class ObjChMonster : GameObjProduct
{
    public  Vector3                     BodyPos;
	public  bool						moveAngle;
	public	bool 						preDeadFlag;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
	private ShapeCapsule                shapeCollforDis;

		
	private UnifiedTextureInfo 			textureInfo;
	GameCtrlManager           			ctrlResMgr    = GameCtrlManager.GetInstance();

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChMonster(int TexId){
		this.TexId = TexId;			
	}
			
    /// 初期化
    public override bool DoInit()
    {
        shapeMove = null;

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, StaticDataList.getVectorZero(), 0.3f );

		shapeColl = new ShapeCapsule();
        shapeColl.Init(1);
        shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), (float)Data.SetupValue.TitanSize );
			
		shapeCollforDis = new ShapeCapsule();
        shapeCollforDis.Init(1);
        shapeCollforDis.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), (float)Data.SetupValue.TouchSize );
		scale = (int)Data.SetupValue.CharScale/100.0f;	
		
		moveAngle = false;
		preDeadFlag = false;
			
	

//			useMdlHdl = new Common.ModelHandle();
//        useMdlHdl.Init();
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
//        useMdlHdl = null;
    }

    /// 開始
    public override bool DoStart()
    {
//			"/Application/res/data/2D/" + dataList.TexFileNameList[mdlTexId,i]
//		Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
			
		changeTex(TexId);
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
		
		if(moveAngle == true){
			if(cameraPosition.Z < myPosition.Z){
				rotation.X = -FMath.Atan(subPosition.X/subPosition.Z);
			}else{ 
				rotation.X = -FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI;
			}
		}else{
			if(cameraPosition.Z < myPosition.Z){
				rotation.X = -FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI;
			}else{ 
				rotation.X = -FMath.Atan(subPosition.X/subPosition.Z);
			}
		}
			
		if(preDeadFlag == true || farFlag == true || changeFlag == true){
			if(appearCount < (int)Data.SetupValue.AppearAndLeaveTime){		
				appearCount++;					
			}else{
				appearCount = (int)Data.SetupValue.AppearAndLeaveTime;
				if(preDeadFlag == true){
					deadFlag = true;						
				}
				if(changeFlag == true){
					changeFlag = false;						
					changeTex(TexId);
				}
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
        return 1;
    }
		
	public void changeTex(int Id){
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		textureInfo = 		resMgr.SetTexture2(Id);
		
		uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
		uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
		texSize = new Vector2(textureInfo.w/10,textureInfo.h/10);						
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
    }
		
	public int texId{
		get{return this.TexId-(int)Data.Tex2dResId.CharStart;}
	}			
	
	public void changeId(int TexId){
		this.TexId = TexId;
		changeFlag = true;
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
		if(type == Data.CollTypeId.ChDestination || type == Data.CollTypeId.littleMove){
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
