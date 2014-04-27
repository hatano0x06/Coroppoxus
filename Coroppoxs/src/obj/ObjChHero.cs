/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.Model;


namespace AppRpg {

///***************************************************************************
/// OBJ:英雄
///***************************************************************************
public class ObjChHero : GameObjProduct
{
    private const float objWidth = 0.3f;

    public  Vector3                 BodyPos;

    /// 形状
    private ShapeSphere             shapeMove;

//	private BasicModel 				titanModel;
	private BasicProgram 			program ;
 

/// 継承メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public override bool DoInit()
    {
        shapeMove = null;

        shapeMove = new ShapeSphere();
        shapeMove.Init(1);
        shapeMove.Set( 0, StaticDataList.getVectorZero(), objWidth );
			
//		titanModel = new BasicModel("/Application/res/data/3D/char/titan.mdx", 0);
		program = new BasicProgram() ;
        return true;
    }

    /// 破棄
    public override void DoTerm()
    {
        if( shapeMove != null ){
            shapeMove.Term();
        }
        shapeMove = null;
//		titanModel.Dispose();
		program.Dispose();

    }

    /// 開始
    public override bool DoStart()
    {

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
    public override bool DoDraw( DemoGame.GraphicsDevice graphDev )
    {
		GameCtrlManager　ctrlResMgr    = GameCtrlManager.GetInstance();
		
        Matrix4 worldMatrix = baseMtx * Matrix4.Scale(new Vector3(0.85f, 0.85f, 0.85f));
        worldMatrix.M42 -= objWidth;
			
		Vector3 EmitColor = new Vector3( 1.0f, 1.0f, 1.0f ) ;			
		Vector3 litDirection = new Vector3( 1.0f, -1.0f, -1.0f ).Normalize() ;
		Vector3 litDirection2 = new Vector3( 0.0f, 1.0f, 0.0f ).Normalize() ;
		Vector3 litColor = new Vector3( 1.0f, 1.0f, 1.0f ) ;
		Vector3 litColor2 = new Vector3( 1.0f, 0.0f, 0.0f ) ;
		Vector3 litAmbient = new Vector3( 0.0f, 0.0f, 0.0f ) ;
		Vector3 fogColor = new Vector3( 0.0f, 0.5f, 1.0f ) ;
			
		BasicParameters parameters = program.Parameters ;
		
		parameters.Enable( BasicEnableMode.Lighting, true ) ;
		parameters.Enable( BasicEnableMode.Fog, false ) ;		
		parameters.SetProjectionMatrix( ref ctrlResMgr.CtrlCam.GetCurrentCameraCore().Projection ) ;
		parameters.SetViewMatrix( ref ctrlResMgr.CtrlCam.GetCurrentCameraCore().View ) ;
			
		parameters.SetLightCount( 1 ) ;
		parameters.SetLightDirection( 0, ref litDirection ) ;
		parameters.SetLightDiffuse( 0, ref litColor ) ;
		parameters.SetLightSpecular( 0, ref litColor ) ;
		parameters.SetLightAmbient( ref litAmbient ) ;
		parameters.SetMaterialEmission(ref EmitColor);
			
		graphDev.Graphics.Enable( EnableMode.Blend ) ;
		graphDev.Graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha ) ;
		graphDev.Graphics.Enable( EnableMode.CullFace ) ;
		graphDev.Graphics.SetCullFace( CullFaceMode.Back, CullFaceDirection.Ccw ) ;
		graphDev.Graphics.Enable( EnableMode.DepthTest ) ;
		graphDev.Graphics.SetDepthFunc( DepthFuncMode.LEqual, true ) ;
			
			/*
		titanModel.SetWorldMatrix( ref worldMatrix ) ;
		titanModel.Update();
		titanModel.Draw(graphDev.Graphics , program);
		*/
			
			
//        Matrix4 mtx = GetBoneMatrix( 1 );
//        Common.VectorUtil.Set( ref BodyPos, mtx.M41, mtx.M42, mtx.M43 );

        return true;
    }

    /// 姿勢の更新
    public override void DoUpdateMatrix()
    {
        shapeMove.SetMult( this.baseMtx );
        boundingShape.SetMult( this.baseMtx );
    }



/// public メソッド
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

}

} // namespace
