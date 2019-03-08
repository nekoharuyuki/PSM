/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using DemoGame;

namespace ShootingDemo
{

/// グループ
public enum GroupId
{
    Player,
    Enemy
}

/// 衝突レベル
public enum CollisionLevel
{
    None,
    PlayerUnit,
    EnemyUnit,
    Bullet
}

/// ライフステート
public enum MonoLifeState
{
    Normal,                // 正常
    Damage,                // ダメージ
    Explode                // 破壊
}


/**
 * Monoクラス
 */
public abstract class Mono
{
    protected Mono parent;
    protected List<Mono> childList = new List<Mono>();
    protected Matrix4 localMatrix = Matrix4.Identity;
    protected Vector3 previousPosition;
    protected Vector3 addRotation;
    protected MonoModel model = null;

    public string Name {get; protected set;}

    /// スコア
    public int Score {get; protected set;}

    /// 体力
    public int Hitpoint {get; set;}
    public int Attack {get; protected set;}
    public int Defense {get; protected set;}
    public GroupId GroupId {get; protected set;}

    /// 無敵時間
    public int UnrivaledTime {get; protected set;}

    public float CollisionRadius {get; set;}
    public float CollisionRadius2 {get; set;}

    /// 衝突レベル
    public CollisionLevel CollisionLevel {get; set;}
    public CollisionLevel PreCollisionLevel {get; set;}

    public MonoLifeState MonoLifeState {get; set;}

    public virtual float ZParam {
        get {return WorldPosition.X;}
    }

    /// コンストラクタ
    public Mono(string name = null, CollisionLevel collisionLevel = CollisionLevel.None)
    {
        Name = name;
        CollisionLevel = collisionLevel;
        UnrivaledTime = 0;
    }

    public Mono Parent
    {
        get {return parent;}
    }

    /// 親の登録
    public void SetParent(Mono value)
    {
        // 既に親が登録されている場合は親の子情報を削除
        if (parent != null) {
            parent.childList.Remove(this);
        }

        parent = value;

        // 親の子情報に己を追加
        if (parent != null && !parent.childList.Contains(this)) {
            parent.childList.Add(this);
        }
    }

    /// 子の削除
    public void RemoveChild()
    {
        if (parent != null) {
            parent.childList.Remove(this);
        }

        if (parent != null && !parent.childList.Contains(this)) {
            parent.childList.Add(this);
        }
    }

    /// 破棄
    public void Dispose()
    {
        if (parent != null) {
            parent.childList.Remove(this);
        }
        childList.Clear();
    }

    /// 開始処理
    public abstract bool Start(MonoManager monoManager);

    /// 終了処理
    public abstract bool End(MonoManager monoManager);

    /// 更新処理
    public virtual bool Update(MonoManager monoManager)
    {
        if (model != null) {
            model.Update(this);
        }

        return true;
    }

    /// 描画処理
    public virtual bool Render(MonoManager monoManager)
    {
        if (model != null) {
            if ( UnrivaledTime % 2 == 0 ) {
                model.Render(this);
            }
        }

        return true;
    }

    /// 攻撃呼び出し
    protected virtual bool CallAttack(MonoManager monoManager, Mono mono)
    {
        return true;
    }

    /// 防御呼び出し
    protected virtual bool CallDefense(MonoManager monoManager, Mono mono)
    {
        return true;
    }

    /// 衝突有効判定
    public virtual bool EnableCollision()
    {
        if (CollisionLevel == CollisionLevel.None ||
            MonoLifeState == MonoLifeState.Explode) {
            return false;
        }

        return true;
    }

    /// 正常時の更新処理
    protected virtual bool UpdateNormal(MonoManager monoManager)
    {
        /// 無敵時間
        if ( UnrivaledTime > 0 ) {
            if ( --UnrivaledTime == 0 ) {
                CollisionLevel = PreCollisionLevel;
            }
        }

        /// 衝突判定
        if (this.EnableCollision()) {
            try {
                monoManager.UpdateList.ForEach(mono => {
                    if (mono.EnableCollision() == false ||
                        mono == this ||
                        mono.CollisionLevel == this.CollisionLevel ||
                        mono.GroupId == this.GroupId) {
                        return;
                    }

                    if (IsCollision(mono, this.previousPosition, this.WorldPosition)) {
                        if (mono.CollisionLevel < this.CollisionLevel) {
                            this.CallAttack(monoManager, mono);
                            mono.CallDefense(monoManager, this);
                        } else if (mono.CollisionLevel > this.CollisionLevel) {
                            mono.CallAttack(monoManager, this);
                            this.CallDefense(monoManager, mono);
                        }
                    }
                });
            } catch (Exception) {
            }
        }

        return true;
    }

    /// ダメージ時の更新処理
    protected virtual bool UpdateDamage(MonoManager monoManager)
    {
        return true;
    }

    /// 破壊時の更新処理
    protected virtual bool UpdateExplode(MonoManager monoManager)
    {
        return true;
    }

    /// マネージャからの削除
    public bool Remove(MonoManager monoManager)
    {
        return Remove(monoManager, false);
    }

    /// マネージャからの削除
    public bool Remove(MonoManager monoManager, bool removeChildren)
    {
        monoManager.Remove(this);

        if (removeChildren) {
            childList.ForEach(mono => monoManager.Remove(mono));
        }

        return true;
    }

    /// 衝突チェック
    public virtual bool IsCollision(Mono dstMono, Vector3 previousPosition, Vector3 nextPosition)
    {
        Vector3 collisionPos = new Vector3(0,0,0);
        return CommonCollision.CheckSphereAndSphere(GetCapsule(previousPosition, nextPosition), dstMono.GetSphere(dstMono.CollisionRadius), ref collisionPos);
    }

    /// 衝突判定用のカプセルの生成
    public GeometryCapsule GetCapsule(Vector3 previousPosition, Vector3 nextPosition)
    {
        return new GeometryCapsule(previousPosition, nextPosition, CollisionRadius);
    }

    /// 衝突判定用の球の生成
    public GeometrySphere GetSphere(float collisionRadius)
    {
        return new GeometrySphere(WorldPosition, collisionRadius);
    }

    /// ローカル行列
    public Matrix4 LocalMatrix
    {
        get {return localMatrix;}
        set {localMatrix = value;}
    }

    /// ワールド行列
    public Matrix4 WorldMatrix
    {
        get {
            if (parent != null) {
                return localMatrix * parent.WorldMatrix;
            } else {
                return localMatrix;
            }
        }

        set {
            if (parent != null) {
                localMatrix = value * parent.WorldMatrix.Inverse();
            } else {
                localMatrix = value;
            }
        }
    }

    /// ローカルポジション
    public Vector3 LocalPosition
    {
        get {
            return new Vector3(localMatrix.M41, localMatrix.M42, localMatrix.M43);
        }
        set {
            localMatrix.M41 = value.X;
            localMatrix.M42 = value.Y;
            localMatrix.M43 = value.Z;
        }
    }

    /// ワールドポジション
    public Vector3 WorldPosition
    {
        get {
            var worldMatrix = WorldMatrix;
            return new Vector3(worldMatrix.M41, worldMatrix.M42, worldMatrix.M43);
        }
        set {
            var worldMatrix = WorldMatrix;

            worldMatrix.M41 = value.X;
            worldMatrix.M42 = value.Y;
            worldMatrix.M43 = value.Z;

            WorldMatrix = worldMatrix;
        }
    }

    /// 描画用の行列
    public Matrix4 DrawMatrix
    {
        get {
            Matrix4 m;
            Rotate(addRotation.X, addRotation.Y, addRotation.Z, out m);
            return WorldMatrix * m;
        }
    }

    /// 移動
    public static void Translate(float x, float y, float z, out Matrix4 m)
    {
        m = Matrix4.Translation(new Vector3(x, y, z));
    }

    /// 回転
    public static void Rotate(float x, float y, float z, out Matrix4 m)
    {
        m = Quaternion.RotationZyx(new Vector3(FMath.Radians(x),
                                               FMath.Radians(y),
                                               FMath.Radians(z))).ToMatrix4();
    }

    /// ローカル行列のセット
    public void SetLocalMatrix(ref Vector3 position, ref Vector3 rotation)
    {
        Rotate(rotation.X, rotation.Y, rotation.Z, out localMatrix);
        WorldPosition = position;
    }

    /// 移動
    public void Translate(float x, float y, float z)
    {
        Matrix4 rot;
        Translate(x, y, z, out rot);

        localMatrix = localMatrix * rot;
    }

    /// 回転
    public void Rotate(float x, float y, float z)
    {
        Matrix4 rot;
        Rotate(x, y, z, out rot);

        localMatrix = localMatrix * rot;
    }

    /// 角度の丸め込み
    private static float normalizeDegree(float degree)
    {
        if (degree < 0) {
            degree += 360.0f * (-((int)degree - 360) / 360);
        }

        degree %= 360;

        return degree;
    }

    /// 追加の回転のセット
    public void AddRotate(float x, float y, float z)
    {
        addRotation.X = normalizeDegree(addRotation.X + x);
        addRotation.Y = normalizeDegree(addRotation.Y + y);
        addRotation.Z = normalizeDegree(addRotation.Z + z);
    }

    /// アクションのセット
    public void SetAction(string key)
    {
        if (model != null) {
            model.SetAction(key);
        }
    }

    /// アクションの切り替え
    public void ChangeAction(string key)
    {
        if (model != null) {
            model.ChangeAction(key);
        }
    }

    /// アクションの終了確認
    public bool IsEndAction()
    {
        if (model != null) {
            return model.IsEndAction();
        }

        return true;
    }

    /// カレントのアクションの名前の取得
    public string CurrentActionName()
    {
        if (model != null) {
            return model.CurrentActionName();
        }

        return null;
    }

    /// 自機（通常弾）
    public void ShootBullet1(MonoManager monoManager)
    {
        NormalBullet bullet = new NormalBullet(this.WorldMatrix,GroupId);
        monoManager.Regist(bullet);

        /// 照り返し
        Stage stage = (Stage)monoManager.FindMono("Stage");

        if ( stage != null ) {
            stage.setLightStatus();
        }
    }

    /// 無敵時間設定
    public void SetUnrivaledTime(int time)
    {
        UnrivaledTime = time;

        PreCollisionLevel = CollisionLevel;
        CollisionLevel = CollisionLevel.None;
    }

    /// 数学：atan2
    public int MathAtan2( int posX, int posY )
    {
        return (int)FMath.Degrees(FMath.Atan2(-posY, -posX));
    }
}

} // ShootingDemo
