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
	public bool							farFlag;
	public int 							countForHouse;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
    private ShapeCapsule                shapeCollforDis;

 //   private Common.ModelHandle          useMdlHdl;
	private VertexBuffer vertices;
		
	private UnifiedTextureInfo textureInfo;
	private ShaderProgram spriteShader;

	SetupModelDataList            dataList = new SetupModelDataList();

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChHouse(int TexId){
		this.TexId = TexId;
		this.TexId += 1;
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
		farFlag = false;
		countForHouse = (int)Data.SetupValue.NewHouseAppearTime;
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
		vertices.Dispose();
		spriteShader.Dispose();
    }

    /// 開始
    public override bool DoStart()
    {
//			"/Application/res/data/2D/" + dataList.TexFileNameList[mdlTexId,i]
//		Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();

		spriteShader = new ShaderProgram("/Application/shaders/Texture2.cgx");
		textureInfo =  resMgr.SetTexture2(TexId+(int)Data.Tex2dResId.HouseStart);
		float r = textureInfo.w;
		float b = textureInfo.h;		
// 		scale = 1/(float)texture.Height;
 		scale = 0.05f;
		vertices = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);

		vertices.SetVertices(0, new float[]{0, 0, 0,
                                            r, 0, 0,
	                                        r, b, 0,
	                                        0, b, 0});

		vertices.SetVertices(1, new float[]{textureInfo.u0, textureInfo.v0,
	                                        textureInfo.u1, textureInfo.v0,
	                                        textureInfo.u1, textureInfo.v1,
	                                        textureInfo.u0, textureInfo.v1});	
			
		texCenter.X = textureInfo.w/2;
		texCenter.Y = textureInfo.h/2;
			
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
//        useMdlHdl.UpdateAnim();
        return true;
    }

    /// 描画処理
    public override bool DoDraw2( DemoGame.GraphicsDevice graphDev ,Vector3 BasePos )
    {
			
 //       useMdlHdl.Render( graphDev, baseMtx * Matrix4.Scale(new Vector3(0.15f, 0.15f, 0.15f)) );
			
//        Matrix4 mtx = GetBoneMatrix( 2 );
//        Common.VectorUtil.Set( ref BodyPos, mtx.M41, mtx.M42, mtx.M43 );
//        boundingShape.SetMult( mtx );
   		
        var currentMatrix = CalcSpriteMatrix(graphDev.GetCurrentCamera().Pos ,BasePos);
        var worldViewProj = graphDev.GetCurrentCamera().Projection * graphDev.GetCurrentCamera().View * currentMatrix;

        spriteShader.SetUniformValue(0, ref worldViewProj);

		graphDev.Graphics.SetShaderProgram(spriteShader);
        graphDev.Graphics.SetVertexBuffer(0, vertices);
        graphDev.Graphics.SetTexture(0, StaticDataList.textureUnified);
	
//		graphDev.Graphics.Enable(EnableMode.Blend);
//        graphDev.Graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
        graphDev.Graphics.Disable(EnableMode.DepthTest);	
		graphDev.Graphics.Enable(EnableMode.CullFace);
		graphDev.Graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
        graphDev.Graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);

        return true;
    }

	private Matrix4 CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
		
//		myPosition.Z += 5.0f;
		myPosition.Y += texCenter.Y*scale;
	    var transMatrix = Matrix4.Translation(myPosition);
		var subPosition = cameraPosition - myPosition;
		var scaleMatrix = Matrix4.Scale(new Vector3(scale,scale,scale));
//		myRotation.Y = FMath.Atan(subPosition.X/subPosition.Z);
//      var rotMatrix = Matrix4.RotationXyz(new Vector3(0.0f,FMath.Atan(subPosition.X/subPosition.Z),0.0f));
		Common.VectorUtil.Set( ref BodyPos, myPosition.X, myPosition.Y, myPosition.Z );		
			
		Matrix4 rotMatrix;
		rotMatrix = Matrix4.RotationZ(FMath.PI);
		if(cameraPosition.Z < myPosition.Z){
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI);
		}else{ 
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z));
		}
			
		Matrix4 rotMatrixZ;
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

		Matrix4 centerMatrix;
		if(appearCount != 0){
		    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, texCenter.Y , 0.0f));			
			rotMatrixZ = Matrix4.RotationX(FMath.PI/2*-appearCount/(int)Data.SetupValue.AppearAndLeaveTime);
			rotMatrixZ *= Matrix4.Translation(new Vector3(0.0f, -texCenter.Y*2,0.0f)); 
		    return transMatrix * rotMatrix * scaleMatrix * centerMatrix * rotMatrixZ;
		}else{
		    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, -texCenter.Y , 0.0f));			
		    return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
		}
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
