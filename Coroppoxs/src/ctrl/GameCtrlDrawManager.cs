/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;

namespace AppRpg {

///***************************************************************************
/// 衝突判定用OBJをまとめるコンテナ
///***************************************************************************
public class GameCtrlDrawParam
{
    public GameActorProduct     Actor;
    public float                Dis;
    public int                  LodLev;

    public void Clear()
    {
        Actor    = null;
    }
}


///***************************************************************************
/// シングルトン：描画対象のアクタを管理
///***************************************************************************
public class GameCtrlDrawManager
{
    private static GameCtrlDrawManager instance = new GameCtrlDrawManager();

    private List< GameCtrlDrawParam >    objParamList;

    private        ShapeFrustum          cullingShape;

    private        float[]               cullingDis;
    private        Vector3               camPos;
    


    /// コンストラクタ
    private GameCtrlDrawManager()
    {
    }

    /// インスタンスの取得
    public static GameCtrlDrawManager GetInstance()
    {
        return instance;
    }


/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        objParamList = new List< GameCtrlDrawParam >();
        if( objParamList == null ){
            return false;
        }

        cullingShape = new ShapeFrustum();
        cullingShape.Init(1);

        return true;
    }

    /// 破棄
    public void Term()
    {
        clear();

        if( objParamList != null ){
            for( int i=0; i<objParamList.Count; i++ ){
                objParamList[i].Clear();
                objParamList[i] = null;
            }
            objParamList.Clear();
        }

        if( cullingShape != null ){
            cullingShape.Term();
        }

        cullingShape     = null;
        objParamList     = null;
        cullingDis       = null;
    }

    /// 開始
    public void Start()
    {
        DemoGame.Camera camCore = GameCtrlManager.GetInstance().CtrlCam.GetCurrentCameraCore();
        cullingShape.Set( camCore.Projection.Inverse() );
    }

    /// 描画
    public void Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].Actor.Draw( graphDev );
        }
    }


    /// 登録開始
    public void EntryStart()
    {
        DemoGame.Camera camCore = GameCtrlManager.GetInstance().CtrlCam.GetCurrentCameraCore();
        Matrix4            mtx        = camCore.View.Inverse();

        Common.MatrixUtil.SetTranslate( ref mtx, camCore.Pos );
        cullingShape.SetMult( mtx );

        camPos = GameCtrlManager.GetInstance().CtrlCam.GetCamTrgPos();

        clear();
    }


    /// キャラクタの登録
    public void EntryCharacter( GameActorProduct actor, bool cullingFlg )
    {
        ShapeSphere bndSph = actor.GetBoundingShape();

        if( cullingFlg == false || cullingShape.CheckNearDis( bndSph.Sphre.Pos ) < bndSph.Sphre.R ){
            float dis = Common.VectorUtil.Distance( actor.BasePos, GameCtrlManager.GetInstance().CtrlCam.GetCamPos() );
            entryActor( actor, dis );
        }
    }

    /// エフェクトの登録
    public void EntryEffect( GameActorProduct actor, bool cullingFlg )
    {
        float dis = Common.VectorUtil.Distance( actor.BasePos, camPos );
        entryActor( actor, dis );
    }


    /// LODパラメータのセット
    public void SetLodParam( float disLv1, float disLv2, float disLv3, float disLv4 )
    {
        cullingDis[0] = disLv1;
        cullingDis[1] = disLv2;
        cullingDis[2] = disLv3;
        cullingDis[3] = disLv4;
    }
    public void SetLodParam( int lv, float val )
    {
        cullingDis[lv] = val;
    }
    public float GetLodParam( int lv )
    {
        return cullingDis[lv];
    }



/// private メソッド
///---------------------------------------------------------------------------

    /// 登録
    public void entryActor( GameActorProduct actor, float dis )
    {
        GameCtrlDrawParam    drawParam = new GameCtrlDrawParam();


        drawParam.Actor     = actor;
        drawParam.Dis       = dis;

        objParamList.Add( drawParam );
    }

    /// 登録のクリア
    public void clear()
    {

        for( int i=0; i<objParamList.Count; i++ ){
            objParamList[i].Clear();
        }
        objParamList.Clear();
    }


    /// 距離が遠い順にソート
    public void SortFar()
    {
        /// 描画対象のリスト
        objParamList.Sort( (x, y) => {
                if (x.Dis < y.Dis) {
                    return 1;
                }
                else if (x.Dis > y.Dis) {
                    return -1;
                }
                else {
                    return 0;
                }
            } );
    }


/// プロパティ
///---------------------------------------------------------------------------

    /// 登録数
    public int Num
    {
        get {return objParamList.Count;}
    }

}

} // namespace
