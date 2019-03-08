/* PlayStation(R)Mobile SDK 1.21.01
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


/// public メソッド
///---------------------------------------------------------------------------

    /// 英雄モデルデータの読み込み
    public bool SetHeroData()
    {
        Data.CharParamDataManager    resMgr    = Data.CharParamDataManager.GetInstance();
        Data.CharParamData           chParam   = resMgr.GetData( (int)Data.ChTypeId.Hero );

        int                    useActNum = 0;

/*
0  待機
1　走り
2　ジャンプ開始
3　ジャンプ中
4　ジャンプ終了
5　攻撃１：横切り
6　攻撃２：縦切り
7　ダメージ
8　死亡
9　－
*/

        chParam.Make( (int)Data.ChMvtResId.Max, ((int)Data.ChMvtResId.Max)*4 );


        /// 待機 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Stand, useActNum, 0 );

        /// 走り 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Run, useActNum, 1 );

        /// ジャンプ開始 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.JumpStart, useActNum, 2 );

        /// ジャンプ中 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.JumpLoop, useActNum, 3 );

        /// ジャンプ終了 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.JumpEnd, useActNum, 4 );

        /// ダメージ 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Damage, useActNum, 7 );

        /// 死亡 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Dead, useActNum, 8 );

        /// 攻撃 動作
        ///----------------------------------------------------
        useActNum = setPlAttackMvt( chParam, (int)Data.ChMvtResId.AttackLR, useActNum,  5, Data.AttackTypeId.HorizontalLR );
        useActNum = setPlAttackMvt( chParam, (int)Data.ChMvtResId.AttackRL, useActNum, 10, Data.AttackTypeId.HorizontalRL );

        /// 攻撃 動作
        ///----------------------------------------------------
        useActNum = setPlAttackMvt( chParam, (int)Data.ChMvtResId.AttackUD, useActNum,  6, Data.AttackTypeId.VerticalUD );
        useActNum = setPlAttackMvt( chParam, (int)Data.ChMvtResId.AttackDU, useActNum, 11, Data.AttackTypeId.VerticalDU );

        /// 勝利 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Victory, useActNum, 9 );

        return true;
    }


    /// 怪物データの読み込み
    public bool SetMonsterAData()
    {
        Data.CharParamDataManager   resMgr     = Data.CharParamDataManager.GetInstance();
        Data.CharParamData          chParam    = resMgr.GetData( (int)Data.ChTypeId.MonsterA );

        int                    useActNum = 0;

/*
0  待機
1　移動
2　攻撃
3　ダメージ
4　死亡
*/
            
        chParam.Make( (int)Data.ChMvtResId.Max, ((int)Data.ChMvtResId.Max)*2 );

        /// 待機 動作
        ///----------------------------------------------------
        useActNum = setEnAStandMvt( chParam, (int)Data.ChMvtResId.Stand, useActNum, 0 );

        /// 移動 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Run, useActNum, 1 );

        /// 旋回 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Turn, useActNum, 0 );

        /// 攻撃 動作
        ///----------------------------------------------------
        useActNum = setEnAAttackMvt( chParam, (int)Data.ChMvtResId.AttackLR, useActNum, 2, 0.0f );

        /// ダメージ 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Damage, useActNum, 3 );

        /// 死亡 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Dead, useActNum, 4 );

        return true;
    }



    /// 怪物データの読み込み
    public bool SetMonsterBData()
    {
        Data.CharParamDataManager    resMgr    = Data.CharParamDataManager.GetInstance();
        Data.CharParamData           chParam   = resMgr.GetData( (int)Data.ChTypeId.MonsterB );

        int                    useActNum = 0;
            
        chParam.Make( (int)Data.ChMvtResId.Max, ((int)Data.ChMvtResId.Max)*2 );

        /// 待機 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Stand, useActNum, 0 );

        /// 旋回 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Turn, useActNum, 0 );

        /// 攻撃 動作
        ///----------------------------------------------------
        useActNum = setEnBAttackMvt( chParam, (int)Data.ChMvtResId.AttackLR, useActNum, 2, 0.0f );

        /// ダメージ 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Damage, useActNum, 3 );

        /// 死亡 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Dead, useActNum, 4 );

        return true;
    }




    /// 怪物データの読み込み
    public bool SetMonsterCData()
    {
        Data.CharParamDataManager    resMgr    = Data.CharParamDataManager.GetInstance();
        Data.CharParamData            chParam    = resMgr.GetData( (int)Data.ChTypeId.MonsterC );

        int                    useActNum = 0;
            
        chParam.Make( (int)Data.ChMvtResId.Max, ((int)Data.ChMvtResId.Max)*2 );

        /// 待機 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Stand, useActNum, 0 );

        /// 旋回 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Turn, useActNum, 1 );

        /// 攻撃 動作
        ///----------------------------------------------------
        useActNum = setEnCAttackMvt( chParam, (int)Data.ChMvtResId.AttackLR, useActNum, 2, 0.0f );

        /// ダメージ 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Damage, useActNum, 3 );

        /// 死亡 動作
        ///----------------------------------------------------
        useActNum = setNormalMvt( chParam, (int)Data.ChMvtResId.Dead, useActNum, 4 );

        return true;
    }



/// private メソッド
///---------------------------------------------------------------------------


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


    /// 英雄 攻撃動作のセット
    private int setPlAttackMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo, Data.AttackTypeId attackType )
    {
        Data.MvtData       mvtRes;
        Data.MvtActData    actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        mvtRes.AddParam( 0, 1 );

        actRes.Make( 6 );

        mvtRes.SetParamAddActionRes( 0, useActNum );
        useActNum ++;

        /// アニメーションの登録
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );

        /// 相手に向く
        actRes.AddParam( (int)Data.ChMvtActCmdId.LookNearTrg, 0.0f, 0.0f,    300, 0, 0, 0, 0 );

        /// エフェクト再生
        actRes.AddParam( (int)Data.ChMvtActCmdId.EffPlay, 0.0f, 0.0f,    (int)Data.EffTypeId.Eff01, 1, 0, 0, 0 );

        /// SE再生
        actRes.AddParam( (int)Data.ChMvtActCmdId.SePlay, 2.0f, 2.0f,    (int)AppSound.SeId.PlAtk, 0, 0, 0, 0 );

        /// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 5.0f, 5.0f,    (int)attackType, -60, 60, 300, 0 );

        /// 攻撃キャンセル受付期間
        actRes.AddParam( (int)Data.ChMvtActCmdId.MvtCancel, 2.0f, 300.0f,    0, 0, 0, 0, 0 );

        return useActNum;
    }


    /// 敵A 攻撃動作のセット
    private int setEnAAttackMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo, float atkFrame )
    {
        Data.MvtData        mvtRes;
        Data.MvtActData        actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        mvtRes.AddParam( 0, 1 );

        actRes.Make( 10 );

        mvtRes.SetParamAddActionRes( 0, useActNum );
        useActNum ++;


        /// アクションの登録
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );

        /// 相手に向く
        actRes.AddParam( (int)Data.ChMvtActCmdId.TurnNearTrg, 0.0f, 30.0f,    2000, 3, 0, 0, 0 );

        /// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 33.0f, 33.0f,    0, -45, 45, 250, 0 );

        /// スーパーアーマー化
        actRes.AddParam( (int)Data.ChMvtActCmdId.SuperArm, 0.0f, 35.0f,    0, 0, 0, 0, 0 );

        /// 羽ばたき音
        actRes.AddParam( (int)Data.ChMvtActCmdId.SePlay,   0.0f,   0.0f,    (int)AppSound.SeId.EnFly, 0, 0, 0, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.SePlay,  10.0f,  10.0f,    (int)AppSound.SeId.EnFly, 0, 0, 0, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.SePlay,  20.0f,  20.0f,    (int)AppSound.SeId.EnFly, 0, 0, 0, 0 );

		// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, atkFrame, atkFrame,    0, 0, 0, 0, 0 );

        return useActNum;
    }


    /// 敵B 攻撃動作のセット
    private int setEnBAttackMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo, float atkFrame )
    {
        Data.MvtData        mvtRes;
        Data.MvtActData        actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        mvtRes.AddParam( 0, 1 );

        actRes.Make( 10 );


        mvtRes.SetParamAddActionRes( 0, useActNum );
        useActNum ++;

        /// アクションの登録
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );

        /// 相手に向く
        actRes.AddParam( (int)Data.ChMvtActCmdId.TurnNearTrg, 0.0f, 37.0f,    2000, 10, 0, 0, 0 );

        /// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 38.0f, 38.0f,    0, -60, 60, 140, 0 );

        /// スーパーアーマー化
        actRes.AddParam( (int)Data.ChMvtActCmdId.SuperArm, 0.0f, 40.0f,    0, 0, 0, 0, 0 );

		/// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, atkFrame, atkFrame,    0, 0, 0, 0, 0 );

        return useActNum;
    }


    /// 敵C 攻撃動作のセット
    private int setEnCAttackMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo, float atkFrame )
    {
        Data.MvtData        mvtRes;
        Data.MvtActData        actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        mvtRes.AddParam( 0, 1 );

        actRes.Make( 10 );

        mvtRes.SetParamAddActionRes( 0, useActNum );
        useActNum ++;


        /// アクションの登録
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );

        /// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 26.0f, 26.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 30.0f, 30.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 34.0f, 34.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 38.0f, 38.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 42.0f, 42.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 46.0f, 46.0f,    0, -30, 30, 250, 0 );
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, 52.0f, 52.0f,    0, -30, 30, 250, 0 );

        /// スーパーアーマー化
        actRes.AddParam( (int)Data.ChMvtActCmdId.SuperArm, 0.0f, 28.0f,    0, 0, 0, 0, 0 );

		/// 攻撃
        actRes.AddParam( (int)Data.ChMvtActCmdId.Attack, atkFrame, atkFrame,    0, 0, 0, 0, 0 );

        return useActNum;
    }


    /// 敵A 待機動作のセット
    private int setEnAStandMvt( Data.CharParamData chParam, int mvtId, int useActNum, int animNo )
    {
        Data.MvtData        mvtRes;
        Data.MvtActData        actRes;
        mvtRes = chParam.GetMvt( mvtId );
        actRes = chParam.GetMvtAct( useActNum );

        mvtRes.Make( 1 );
        mvtRes.AddParam( 0, 1 );

        actRes.Make( 10 );

        mvtRes.SetParamAddActionRes( 0, useActNum );
        useActNum ++;

        /// アクションの登録
        actRes.AddParam( (int)Data.ChMvtActCmdId.Animation, 0.0f, 0.0f,    animNo, 0, 0, 0, 0 );

        /// 羽ばたきの音再生
        actRes.AddParam( (int)Data.ChMvtActCmdId.SePlay,  0.0f,  0.0f,    (int)AppSound.SeId.EnFly, 0, 0, 0, 0 );

        return useActNum;
    }
}

} // namespace
