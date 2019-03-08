/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
 

using System;


namespace AppRpg{  namespace Data {


///***************************************************************************
/// ゲーム基本処理の列挙
///***************************************************************************

/// キャラタイプID
public enum ChTypeId {
    Hero = 0,    /// 英雄
    MonsterA,    /// モンスターＡ
    MonsterB,    /// モンスターＢ
    MonsterC,    /// モンスターＣ
    Max          ///
}

/// 装備品タイプID
public enum EquipTypeId {
    Sword = 0,    /// 剣
    Max           ///
}

/// 弾タイプID
public enum BulletTypeId {
    FireBall = 0,    /// 火炎弾
    Max              ///
}

/// ステージ構成タイプID
public enum StageTypeId {
    Stage1 = 0,    /// ステージ
    Sky,           /// 空
    Max            ///
}

/// 草木タイプID
public enum PlantTypeId {
    Wood = 0,    /// 
    Wood2,
    Grass,
    Max         ///
}

/// エフェクトタイプID
public enum EffTypeId {
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
    Max         ///
}

/// 備品タイプID
public enum FixTypeId {
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


/// キャラクタの動作ID
public enum ChMvtResId {
    Stand = 0,    /// 待機
    Run,          /// 走る
    Turn,         /// 旋回
    AttackLR,     /// 右振り
    AttackRL,     /// 左振り
    AttackUD,     /// 振り下し
    AttackDU,     /// 振り上げ
    Damage,       /// ダメージ
    Dead,         /// 死亡
    JumpStart,    /// ジャンプ開始
    JumpLoop,     /// ジャンプ中
    JumpEnd,      /// ジャンプ終了
    Victory,      /// 勝利
    Max         ///
}


/// キャラクタの動作アクションID
public enum ChMvtActCmdId {
    Animation = 0,  /// アニメーション再生
    Attack,         /// 攻撃
    SePlay,         /// SE再生
    LookNearTrg,    /// 一番近くの相手に向く
    TurnNearTrg,    /// 一番近くの相手に振り向く
    EffPlay,        /// エフェクト再生
    SuperArm,       /// スーパーアーマー化
    MvtCancel,      /// 動作キャンセル
    Max             ///
}


/// 攻撃ID
public enum AttackTypeId {
    VerticalUD = 0,  /// 振り下し
    VerticalDU,      /// 振り上げ
    HorizontalLR,    /// 水平斬り左→右
    HorizontalRL,    /// 水平斬り右→左
    Magic,           /// 魔法攻撃
    Normal,          /// 通常攻撃
    Max              ///
}


/// 移動の衝突タイプID
public enum CollTypeId {
    ChMove = 0,      /// 通常移動
    BullMove,        /// 弾の移動
    ChDestination,   /// 目的地のチェック
    Max
}



}} // namespace
