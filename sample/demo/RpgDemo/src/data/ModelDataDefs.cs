/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;


namespace AppRpg{  namespace Data {


///***************************************************************************
/// モデルデータのリソースID
///***************************************************************************

/// モデルのリソースID
public enum ModelResId {
    Hero = 0,      /// 英雄
    MonsterA,      /// 怪物A
    MonsterB,      /// 怪物B
    MonsterC,      /// 怪物C
    Stage,         /// ステージ
    Sky,           /// ステージ空
    Sword,         /// 剣
    Eff00,         /// エフェクト：剣オーラ
    Eff01,         /// エフェクト：剣の軌跡（縦切り）
    Eff02,         /// エフェクト：剣の軌跡（横切り）
    Eff03,         /// エフェクト：剣ヒット（縦切り）
    Eff04,         /// エフェクト：剣ヒット（横切り）
    Eff05,         /// エフェクト：敵消滅
    Eff06,         /// エフェクト：足跡
    Eff07,         /// エフェクト：魔法攻撃
    Eff08,         /// エフェクト：魔法ヒット
    Eff09,         /// エフェクト：オブジェクト破壊
    Eff10,         /// エフェクト：影
    Eff11,         /// エフェクト：草刈
    Eff12,         /// エフェクト：プレイヤーダメージ
    Eff13,         /// エフェクト：敵＆アイテム選択カーソル
    Eff14,         /// エフェクト：移動位置カーソル
    Fix00,         /// 木箱
    Fix01,         /// 木樽
    Fix02,         /// 木の柵
    Fix03,         /// 低い木
    Fix04,         /// 立札
    Fix05,         /// 石柱
    Fix06,         /// 家Type2
    Fix07,         /// 家Type3
    Fix08,         /// 家Type4
    Fix09,         /// 荷車
    Fix10,         /// 薪山
    Fix11,         /// 小舟
    Fix12,         /// アーチ
    Fix13,         /// 木Type0
    Fix14,         /// 木Type1
    Fix15,         /// 石柱Type0
    Fix16,         /// 石柱Type1
    Fix17,         /// 石柱Type2
    Fix18,         /// 家Type0
    Fix19,         /// 家Type1
    Fix01_l1,      /// LOD:木樽
    Fix02_l1,      /// LOD:木の柵
    Fix03_l1,      /// LOD:低い木
    Fix04_l1,      /// LOD:立札
    Fix05_l1,      /// LOD:石柱
    Fix06_l1,      /// LOD:家Type2
    Fix07_l1,      /// LOD:家Type3
    Fix08_l1,      /// LOD:家Type4
    Fix09_l1,      /// LOD:荷車
    Fix10_l1,      /// LOD:薪山
    Fix11_l1,      /// LOD:小舟
    Fix12_l1,      /// LOD:アーチ
    Fix13_l1,      /// LOD:木Type0
    Fix13_l2,      /// LOD:木Type0
    Fix14_l1,      /// LOD:木Type1
    Fix14_l2,      /// LOD:木Type1
    Fix15_l1,      /// LOD:石柱Type0
    Fix16_l1,      /// LOD:石柱Type1
    Fix17_l1,      /// LOD:石柱Type2
    Fix18_l1,      /// LOD:家Type0
    Fix19_l1,      /// LOD:家Type1
    Max            ///
}

/// テクスチャのリソースID
public enum ModelTexResId {
    Hero = 0,      /// 英雄
    MonsterA,      /// 怪物A
    MonsterB,      /// 怪物B
    MonsterC,      /// 怪物C
    Stage,         /// ステージ
    Sky,           /// ステージ空
    Sword,         /// 剣
    EffA,          /// エフェクト
    EffB,          /// エフェクト
    EffC,          /// エフェクト
    EffD,          /// エフェクト
    EffE,          /// エフェクト
    EffF,          /// エフェクト
    EffG,          /// エフェクト
    EffH,          /// エフェクト
    EffI,          /// エフェクト
    Fix00,         /// 木箱
    Fix01,         /// 木樽
    Fix02,         /// 木の柵
    Fix03,         /// 低い木
    Fix04,         /// 立札
    Fix05,         /// 石柱
    Fix06,         /// 家Type2
    Fix07,         /// 家Type3
    Fix08,         /// 家Type4
    Fix09,         /// 荷車
    Fix10,         /// 薪山
    Fix11,         /// 小舟
    Fix12,         /// アーチ
    Fix13,         /// 木Type0
    Fix14,         /// 木Type1
    Fix15,         /// 石柱Type0
    Fix16,         /// 石柱Type1
    Fix17,         /// 石柱Type2
    Fix18,         /// 家Type0
    Fix19,         /// 家Type1
    Max         ///
}

/// シェーダのリソースID
public enum ModelShaderReslId {
    Normal = 0,    /// 通常
    Max         ///
}


/// エフェクトのテクスチャの管理ID
public enum ModelEffTexId {
    EffA = 0,    ///
    EffB,        ///
    EffC,        ///
    EffD,        ///
    EffE,        ///
    EffF,        ///
    EffG,        ///
    EffH,        ///
    EffI,        ///
    Max         ///
}




}} // namespace
