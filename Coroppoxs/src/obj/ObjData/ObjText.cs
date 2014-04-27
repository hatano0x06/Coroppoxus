using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using DemoGame;

namespace AppRpg
{
	public class ObjText : GameObjProduct
	{
	public int	 					timeCount;
	public float						angle;
	public float 						scaleX;
    private static ShaderProgram textureShaderProgram;
	private StringData 					tweet;
	private int						tweetNumber;
	/// 形状
		

	private Sprite sprite;
	private Font currentFont;

/// 継承メソッド
///---------------------------------------------------------------------------
	public ObjText(int texId){
		this.TexId = texId;
	}
			
    /// 初期化
    public override bool DoInit()
    {
		appearCount = 30;
		timeCount = 0;
		deadflag = false;
		tweet = new StringData();
		scale = (int)Data.SetupValue.TexScale/100.0f;
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
		sprite.Dispose();
    }

    /// 開始
    public override bool DoStart()
    {
		tweetNumber = StaticDataList.getRandom(0,9);
		currentFont = new Font( FontAlias.System, 20, FontStyle.Regular );
		sprite = new Sprite(tweet.tweetlist[TexId,tweetNumber], 0xffffffff, currentFont, 0,0);
		textureShaderProgram = createSimpleTextureShader();
		scaleX = tweet.tweetlist[TexId,tweetNumber].Length / 9.0f;

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
		
	public void changeText(){
		tweetNumber = (int)StaticDataList.getRandom(0,9);
		sprite = new Sprite(tweet.tweetlist[TexId,tweetNumber], 0xffffffff, currentFont, 0,0);
		scaleX *= tweet.tweetlist[TexId,tweetNumber].Length / 9.0f;
	}

	public void setangle(float angle){
		this.angle = angle;
	}

    /// 描画処理
    public override bool DoDraw2( DemoGame.GraphicsDevice graphDev ,Vector3 BasePos )
    {
        if(!sprite.Visible){
            return false;
        }
//			sprite.SetDrawRect(0,0,10,10);
		var currentMatrix = CalcSpriteMatrix(graphDev.GetCurrentCamera().Pos ,BasePos);
		var modelMatrix = sprite.CreateModelMatrix(0,0);
        var worldViewProj = graphDev.GetCurrentCamera().Projection * graphDev.GetCurrentCamera().View * currentMatrix * modelMatrix;

        textureShaderProgram.SetUniformValue(0, ref worldViewProj);
		
		float alphaRate = 1.0f;
        int alphaRateId = textureShaderProgram.FindUniform("AlphaRate");
        if (alphaRateId >= 0) {
            textureShaderProgram.SetUniformValue(alphaRateId, alphaRate);
        }

        graphDev.Graphics.SetShaderProgram(textureShaderProgram);
        graphDev.Graphics.SetVertexBuffer(0, sprite.Vertices);
        graphDev.Graphics.SetTexture(0, sprite.Texture);

		graphDev.Graphics.Enable(EnableMode.Blend);
        graphDev.Graphics.Disable(EnableMode.DepthTest);
        graphDev.Graphics.Disable(EnableMode.CullFace);
		graphDev.Graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

	    graphDev.Graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
        return true;
    }

	private Matrix4 CalcSpriteMatrix(Vector3 cameraPosition ,Vector3 myPosition)
	{
		myPosition.Y += 3.0f;

	    var transMatrix = Matrix4.Translation(myPosition);
		var subPosition = cameraPosition - myPosition;
		var scaleMatrix = Matrix4.Scale(new Vector3(scale,scale,scale));
		
		Matrix4 rotMatrix;

		rotMatrix = Matrix4.RotationZ(FMath.PI);
		if(cameraPosition.Z < myPosition.Z){
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z));
		}else{ 
			rotMatrix *= Matrix4.RotationY(-FMath.Atan(subPosition.X/subPosition.Z)+FMath.PI);
		}
		
		Matrix4 centerMatrix;
		if(appearCount != 0){
			Matrix4 rotMatrixZ;
		    centerMatrix = Matrix4.Translation(new Vector3(-sprite.CenterX,sprite.CenterY*4.4f, 0.0f));
			rotMatrixZ = Matrix4.RotationX(angle);
			rotMatrixZ *= Matrix4.Translation(new Vector3(0.0f, -sprite.CenterY*6.0f,0.0f)); 
		    return transMatrix * rotMatrix * scaleMatrix * centerMatrix * rotMatrixZ;
		}else{
		    centerMatrix = Matrix4.Translation(new Vector3(-sprite.CenterX,-sprite.CenterY*4.4f, 0.0f));
		    return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
		}
	}		

    private ShaderProgram createSimpleTextureShader()
    {
        var shaderProgram = new ShaderProgram("/Application/shaders/Texture.cgx");

        shaderProgram.SetAttributeBinding(0, "a_Position");
        shaderProgram.SetAttributeBinding(1, "a_TexCoord");

        shaderProgram.SetUniformBinding(0, "WorldViewProj");

        return shaderProgram;
    }			
    
/// public メソッド
///---------------------------------------------------------------------------


/// private メソッド
///---------------------------------------------------------------------------

	}
}

