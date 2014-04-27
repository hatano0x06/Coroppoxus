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
	public bool							farFlag;

    /// 形状
    private ShapeSphere                 shapeMove;
    private ShapeCapsule                shapeColl;
	private ShapeCapsule                shapeCollforDis;

	private VertexBuffer vertices;
	private ShaderProgram spriteShader;
		
	private UnifiedTextureInfo textureInfo;

	SetupModelDataList            dataList = new SetupModelDataList();
	

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjChWall(int TexId, Vector3 pos1 ,Vector3 pos2){
		this.TexId = TexId;
		this.TexId += 1;
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
			
		appearCount = 30;
		glowCounter = 0;
		scale = (int)Data.SetupValue.WallScale /100.0f;	
			
		farFlag = false;
			
		distance = Common.VectorUtil.DistanceXZ( pos1 ,pos2 );
		angle = FMath.Atan((pos1.X-pos2.X)/(pos1.Z-pos2.Z)) +FMath.PI/2;
			
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
		vertices.Dispose();
		spriteShader.Dispose();
    }

    /// 開始
    public override bool DoStart()
    {
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();

		spriteShader = new ShaderProgram("/Application/shaders/Texture2.cgx");
		textureInfo = resMgr.SetTexture2((int)Data.Tex2dResId.Gareki);

		float r = textureInfo.w;
		float b = textureInfo.h;
		scale = 1/(float)textureInfo.h;
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

    public  bool startforChange(int texnumber)
    {

		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();

		spriteShader = new ShaderProgram("/Application/shaders/Texture2.cgx");
		textureInfo = 		resMgr.SetTexture2(texnumber);

		float r = textureInfo.w;
		float b = textureInfo.h;
//		scale = 1/(float)texture.Height;
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

    }

    /// フレーム処理
    public override bool DoFrame()
    {
		if(glowNumber == 2){
			glowCounter = (int)Data.SetupValue.ToNextStageCount*3;
			glowFinish = true;
		}
		if(glowCounter >= (int)Data.SetupValue.ToNextStageCount*2){
			if(glowCounter == (int)Data.SetupValue.ToNextStageCount*2 && glowNumber == 1){ 
				changeFlag = true;
				glowNumber++;
			}
			if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
				changeFlag = false;
				startforChange((int)Data.Tex2dResId.Wall);
			}
		}else if(glowCounter >= (int)Data.SetupValue.ToNextStageCount){
			if(glowCounter == (int)Data.SetupValue.ToNextStageCount && glowNumber == 0){
				changeFlag = true;
				glowNumber++;
			}
			if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && changeFlag == true){
				changeFlag = false;
				startforChange((int)Data.Tex2dResId.GarekiWall);
			}
		}
        return true;
    }

    /// 描画処理
    public override bool DoDraw2( DemoGame.GraphicsDevice graphDev ,Vector3 BasePos )
    {
        var currentMatrix = CalcSpriteMatrix(graphDev.GetCurrentCamera().Pos ,BasePos);
        var worldViewProj = graphDev.GetCurrentCamera().Projection * graphDev.GetCurrentCamera().View * currentMatrix;

        spriteShader.SetUniformValue(0, ref worldViewProj);
			
		graphDev.Graphics.SetShaderProgram(spriteShader);
        graphDev.Graphics.SetVertexBuffer(0, vertices);
        graphDev.Graphics.SetTexture(0, StaticDataList.textureUnified);

		graphDev.Graphics.Enable(EnableMode.CullFace);
		graphDev.Graphics.SetCullFace(CullFaceMode.None, CullFaceDirection.Ccw);
        graphDev.Graphics.Disable(EnableMode.DepthTest);
        graphDev.Graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);

        return true;
    }

	private Matrix4 CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
		myPosition.Y += texCenter.Y*scale;
	    var transMatrix = Matrix4.Translation(myPosition);
		var subPosition = cameraPosition - myPosition;
		var scaleMatrix = Matrix4.Scale(new Vector3(scale,scale,scale));
//		myRotation.Y = FMath.Atan(subPosition.X/subPosition.Z);
//      var rotMatrix = Matrix4.RotationXyz(new Vector3(0.0f,FMath.Atan(subPosition.X/subPosition.Z),0.0f));
		Common.VectorUtil.Set( ref BodyPos, myPosition.X, myPosition.Y, myPosition.Z );		
			
		Matrix4 rotMatrix;
		rotMatrix = Matrix4.RotationY(angle);
		rotMatrix *= Matrix4.RotationZ(FMath.PI);
			
		Matrix4 rotMatrixZ;
		if(deadflag == true || changeFlag == true || farFlag == true){
			if(appearCount < (int)Data.SetupValue.AppearAndLeaveTime){		
				appearCount++;					
			}else{
				if(deadflag == true){
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
        int chTypeIdx    = (int)chTypeId;
        int mdlResIdx    = (int)Data.ModelResId.Hero + chTypeIdx;
        int texResIdx    = (int)Data.ModelTexResId.Hero + chTypeIdx;

//        Data.ModelDataManager    resMgr = Data.ModelDataManager.GetInstance();//
//        useMdlHdl.Start( resMgr.GetModel( mdlResIdx ), resMgr.GetTextureContainer( texResIdx ), resMgr.GetShaderContainer( shaResIdx )    );
			
	//	shapeColl.Set( 0, StaticDataList.getVectorZero(), 0.001f );			
		shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 20.0f );
/*        switch( chTypeId ){
        case Data.ChTypeId.WallA: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
        case Data.ChTypeId.WallB: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 0.75f, 0.0f), 0.5f );    break;
        case Data.ChTypeId.WallC: shapeColl.Set( 0, StaticDataList.getVectorZero(), new Vector3(0.0f, 2.0f, 0.0f), 1.0f );     break;
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
