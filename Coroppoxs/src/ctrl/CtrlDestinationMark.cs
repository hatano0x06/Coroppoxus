using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;

namespace AppRpg
{
	public class CtrlDestinationMark
	{

    private List< ActorDestinationMark >    actorChList;
    private List< ActorDestinationMark >    activeList;
	private const float       		  moveSpeed = 0.104f;
	private Random rand = new System.Random();
/// public メソッド
///---------------------------------------------------------------------------

    /// 初期化
    public bool Init()
    {
        actorChList = new List< ActorDestinationMark >();
        if( actorChList == null ){
            return false;
        }

        activeList = new List< ActorDestinationMark >();
        if( activeList == null ){
            return false;
        }

        return true;
    }

    /// 破棄
    public void Term()
    {
        if( activeList != null ){
            activeList.Clear();
        }
        if( actorChList != null ){
            for( int i=0; i<actorChList.Count; i++ ){
                actorChList[i].Term();
            }
            actorChList.Clear();
        }

		activeList       = null;
        actorChList      = null;
    }
		
	public void Clear()
	{
        if( activeList != null ){
            activeList.Clear();
        }
        if( actorChList != null ){
            for( int i=0; i<actorChList.Count; i++ ){
                actorChList[i].Term();
            }
            actorChList.Clear();
        }
	}

    /// 開始
    public bool Start()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Start();
        }

        return true;
    }

    /// 終了
    public void End()
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].End();
        }
        actorChList.Clear();
        activeList.Clear();
    }


    /// フレーム処理
    public bool Frame()
    {
        for( int i=0; i<actorChList.Count; i++ ){
			actorChList[i].Frame();
        }
        return true;
    }


    /// 描画処理
    public bool Draw( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Draw( graphDev);
        }
		Clear();
    
        return true;
    }

    /// 描画処理（デバック用）
    public bool DrawDebug( DemoGame.GraphicsDevice graphDev )
    {
        for( int i=0; i<actorChList.Count; i++ ){
            actorChList[i].Frame();
            actorChList[i].Draw( graphDev );
        }
    
        return true;
    }

    /// 敵の登録
    public void EntryAddDestinationMark(Vector3 pos)
    {
        ActorDestinationMark actorCh = new ActorDestinationMark();
        actorCh.Init();
        actorCh.Start();
        actorChList.Add( actorCh );

        SetPlace( (actorChList.Count-1), pos );
    }
		

    /// 敵の登録削除
    public void DeleteEntryTower( int idx )
    {
        actorChList.RemoveAt( idx );
    }

    /// 敵の配置
    public void SetPlace( int idx, Vector3 pos )
    {

        Matrix4 mtx = new Matrix4();
        Common.MatrixUtil.SetTranslate( ref mtx, pos );

        actorChList[idx].SetPlace( mtx );
    }

/// private メソッド
///---------------------------------------------------------------------------


	}
}

