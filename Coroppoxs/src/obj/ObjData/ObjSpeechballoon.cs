using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Tutorial.Utility;

namespace AppRpg
{
	public class ObjSpeechballoon : GameObjProduct
	{
    public  Vector2                     texCenter;
	public int	 					timeCount;
	public int	 					TexIdForText;
	public bool							deadflag2;

	public ObjText						ObjTex;

    /// 形状
		
	private VertexBuffer vertices;
	private UnifiedTextureInfo textureInfo;
	private ShaderProgram spriteShader;

	SetupModelDataList            dataList = new SetupModelDataList();

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjSpeechballoon(int ToTextId, int TexId){
		this.TexId = TexId;
		TexIdForText = ToTextId;
	}
			
    /// 初期化
    public override bool DoInit()
    {
		timeCount = 0;
		deadflag = true;
		deadflag2 = true;
		if(TexIdForText <3){
			scale = (int)Data.SetupValue.BallonScale/100.0f;	
			ObjTex = new ObjText(TexIdForText);
			ObjTex.Init();
		}else if(TexIdForText == 3){
			scale = (int)Data.SetupValue.GionScale/100.0f;	
		}
		return true;
    }

    /// 破棄
    public override void DoTerm()
    {
		vertices.Dispose();
		spriteShader.Dispose();
		if(TexIdForText <3){
			ObjTex.Term();
		}
    }

    /// 開始
    public override bool DoStart()
    {
		Data.ModelDataManager   	  resMgr = Data.ModelDataManager.GetInstance();
		spriteShader = new ShaderProgram("/Application/shaders/Texture2.cgx");
		if(TexIdForText <3){
			textureInfo = 		resMgr.SetTexture2(TexId+(int)Data.Tex2dResId.SpeechBalloon1);
		}else if(TexIdForText == 3){
			textureInfo = 		resMgr.SetTexture2((int)Data.Tex2dResId.SpeechBalloon3);
		}

		float r = textureInfo.w;
		float b = textureInfo.h;		
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
		if(TexIdForText <3){
			ObjTex.Start();
		}
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
		timeCount++;
		if(TexIdForText != 3){
			if(timeCount > (int)Data.SetupValue.BaloonAppearTime){
				deadflag2 = true;		
			}
		}else if(TexIdForText == 3){
			if(timeCount > (int)Data.SetupValue.GionAppearTime){
				deadflag2 = true;		
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
		graphDev.Graphics.SetCullFace(CullFaceMode.Front, CullFaceDirection.Ccw);	
        graphDev.Graphics.Disable(EnableMode.DepthTest);
	    graphDev.Graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
		if(TexIdForText < 3){
			if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime && deadflag2 == false){
				ObjTex.changeText();
			}
			ObjTex.Draw2(graphDev, BasePos);
		}
		return true;
    }

	private Matrix4 CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
		if(TexIdForText < 3){
			myPosition.Y += 3.0f;
		}else if(TexIdForText == 3){
			myPosition.Y += 6.0f;	
			myPosition.X += 2.0f;	
			myPosition.Z += 1.0f;	
		}
	    var transMatrix = Matrix4.Translation(myPosition);
		var subPosition = cameraPosition - myPosition;
		
		Matrix4 rotMatrix;

		rotMatrix = Matrix4.RotationZ(FMath.PI);
		if(cameraPosition.Z < myPosition.Z){
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z));
		}else{ 
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI);
		}
			
		Matrix4 rotMatrixZ;
		if(TexIdForText != 3){
			if(deadflag2 == true){
				appearCount++;
				if(appearCount == (int)Data.SetupValue.AppearAndLeaveTime){
					deadflag = true;
				}
			}else{
				if(appearCount > 0)		
					appearCount--;								
			}
		}else if(TexIdForText == 3){
			if(deadflag2 == true){
				appearCount++;
				if(appearCount == (int)Data.SetupValue.GionAppearSpeed){
					deadflag = true;
				}
			}else{
				if(appearCount > 0)		
					appearCount--;								
			}				
		}
			
		Matrix4 scaleMatrix = new Matrix4();
		if(TexIdForText <3){
			scaleMatrix = Matrix4.Scale(new Vector3(ObjTex.scaleX*scale,scale,scale));
		}else if(TexIdForText == 3 ){
			scaleMatrix = Matrix4.Scale(new Vector3(scale,scale,scale));				
		}
		
		Matrix4 centerMatrix;
		if(TexIdForText != 3){
			if(appearCount != 0){
			    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, texCenter.Y , 0.0f));			
				rotMatrixZ = Matrix4.RotationX(FMath.PI/2*-appearCount/(int)Data.SetupValue.AppearAndLeaveTime);
				if(TexIdForText <3){
					ObjTex.setangle(FMath.PI/2*-appearCount/(int)Data.SetupValue.AppearAndLeaveTime);
				}
				rotMatrixZ *= Matrix4.Translation(new Vector3(0.0f, -texCenter.Y*2,0.0f)); 
			    return transMatrix * rotMatrix * scaleMatrix * centerMatrix * rotMatrixZ;
			}else{
			    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, -texCenter.Y , 0.0f));			
				if(TexIdForText <3){
					ObjTex.setangle(0);
				}
			    return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
			}
		}else{
			if(appearCount != 0){
			    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, texCenter.Y , 0.0f));			
				rotMatrixZ = Matrix4.RotationX(FMath.PI/2*-appearCount/(int)Data.SetupValue.AppearAndLeaveTime);
				if(TexIdForText <3){
					ObjTex.setangle(FMath.PI/2*-appearCount/(int)Data.SetupValue.GionAppearSpeed);
				}
				rotMatrixZ *= Matrix4.Translation(new Vector3(0.0f, -texCenter.Y*2,0.0f)); 
			    return transMatrix * rotMatrix * scaleMatrix * centerMatrix * rotMatrixZ;
			}else{
			    centerMatrix = Matrix4.Translation(new Vector3(-texCenter.X, -texCenter.Y , 0.0f));			
				if(TexIdForText <3){
					ObjTex.setangle(0);
				}
			    return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
			}				
		}
			
			
	}		

			
    
/// public メソッド
///---------------------------------------------------------------------------


/// private メソッド
///---------------------------------------------------------------------------

	}
}

