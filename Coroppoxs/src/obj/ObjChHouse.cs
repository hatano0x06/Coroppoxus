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
public class ObjChHouse : GameObjProduct
{
    public  Vector3                     BodyPos;
    public  Vector2                     texCenter;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
    private ShapeCapsule                shapeCollforDis;
		
	private UnifiedTextureInfo 			textureInfo;

	SetupModelDataList            		dataList 	  = new SetupModelDataList();
	GameCtrlManager           			ctrlResMgr    = GameCtrlManager.GetInstance();
/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChHouse(int TexId){
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
        shapeCollforDis.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), (float)Data.SetupValue.TouchSize*4 );
 		scale = (int)Data.SetupValue.HouseScale/100.0f;
			
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
		Common.VectorUtil.Set( ref BodyPos, myPosition.X, myPosition.Y, myPosition.Z );		
		var subPosition = cameraPosition - myPosition;
			
		if(cameraPosition.Z < myPosition.Z){
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI;
		}else{ 
			rotation.X = -FMath.Atan(subPosition.X/subPosition.Z);
		}
			
		if(deadFlag == true || changeFlag == true || farFlag == true){
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
        case Data.ChTypeId.HouseA: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.HouseB: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.HouseC: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        }
        */
    }
		
	public int texId{
		set{this.TexId = value;}
		get{
			if( this.TexId != (int)Data.Tex2dResId.BossMonument ){
				return this.TexId-(int)Data.Tex2dResId.HouseStart;
			}else{
				return 0;						
			}
		}
	}	
	
	public override int GetTexId(){
		if( this.TexId != (int)Data.Tex2dResId.BossMonument ){
			return this.TexId-(int)Data.Tex2dResId.HouseStart;
		}else{
			return 0;						
		}			
	}
	
	public void changeTex(int Id){
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		textureInfo = 		resMgr.SetTexture2(TexId);
		
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
