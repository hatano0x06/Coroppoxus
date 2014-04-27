/* PlayStation(R)Mobile SDK 1.11.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;

namespace AppRpg {


///***************************************************************************
/// キャラクターの動作データのセットアップ
///***************************************************************************
public class SetupChMvtData
{

    /// アニメーション再生だけを行う動作のセット
    private int setNormalMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo )
    {
        Data.MvtData        mvtRes;
        Data.MvtActData    actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        actRes.Make( 1 );

        mvtRes.AddParam( 0, 5 );

        /// アクションの登録
        mvtRes.SetParamAddActionRes( 0, useActNum );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );
        useActNum ++;

        return useActNum;
    }

}

} // namespace
