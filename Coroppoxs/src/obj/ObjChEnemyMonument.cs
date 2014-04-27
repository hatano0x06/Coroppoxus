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
public class ObjChEnemyMonument : GameObjProduct
{
    public  Vector3                     BodyPos;
	public bool							farFlag;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
    private ShapeCapsule                shapeCollforDis;
		
	private UnifiedTextureInfo 			textureInfo;

	SetupModelDataList            		dataList = new SetupModelDataList();
	GameCtrlManager           			ctrlResMgr    = GameCtrlManager.GetInstance();

/// 継承メソッド
///---------------------------------------------------------------------------
		
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
			
		farFlag = false;
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
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();

		textureInfo =  resMgr.SetTexture2((int)Data.Tex2dResId.Monument);
		uvPos = new Vector2(textureInfo.u0, textureInfo.v0);
		uvSize = new Vector2(textureInfo.u1-textureInfo.u0, textureInfo.v1-textureInfo.v0);
		texSize = new Vector2(textureInfo.w/10,textureInfo.h/10);
			
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
        return 4;
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
        case Data.ChTypeId.MonumentA: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.MonumentB: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.MonumentC: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        }
        */
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
