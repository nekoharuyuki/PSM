/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using DemoGame;

namespace Physics2dDemo
{

/// Targetクラス
/// 基本的な物体のクラス
public abstract class Target
{
	protected  Target       parent;	
    protected  List<Target> childList = new List<Target>();
	public     LayoutAction action;
	
	public int    baseX;		// 初期位置X
	public int    baseY;		// 初期位置Y
	public int    positionX;	// 移動したときの位置X
	public int    positionY;	// 移動したときの位置Y
	public long   score = 0;
	public long   old_score = 0;
	public int    priority = 0;
	public bool   isCollision = false;
	public bool   isFirstCollision = false; //初回の衝突フラグ
	public bool   isFirstScore     = false; //初回の衝突フラグ
    public string Name {get; protected set;}
	public bool   isHide = false;
		
	/// 描画優先度
    public virtual int Priority {
			get{return priority;}
			set{priority = value;}
	}
		
	/// スコア
    public long Score {
			get{return score;}
			set{
				old_score = score;
				score = value;
			}
	}
	
    /// コンストラクタ
	public Target(string name = null)
	{
		Name = name;
	}

	/// コンストラクタ
	/// @param [in] name
	/// @param [in] img テクスチャ
	/// @param [in] x   切り出し座標x
	/// @param [in] y   切り出し座標y
	/// @param [in] w   幅
	/// @param [in] h   高さ
	/// @param [in] drawX 描画座標ｘ
	/// @param [in] drawY 描画座標ｙ
	/// @param [in] p     描画優先度
	///
	public Target(string name, Texture2D img, int x, int y, int w, int h, int drawX, int drawY, int p)
	{
	}
	
    /// 破棄
    public void Dispose()
    {			
		if(action != null)
		{
			action.Dispose();
			action = null;
		}

        if (parent != null) {
            parent.childList.Remove(this);
        }
        childList.Clear();
    }

	/// 開始処理
	///
	/// @param [out]
	public abstract bool Start(TargetManager targetMnager);
	
	/// 終了処理
	///
	/// @param [out]
	///
	public abstract bool End(TargetManager targetMnager);
	
	/// 更新処理
	///
	/// @param [out]
	///
	public virtual bool Update(TargetManager targetMnager)
	{			
		return true;
	}
	
	/// 描画
	///
	/// @param [out]
	///
	public virtual bool Render(TargetManager targetMnager)
	{
		return true;
	}

	/// 移動
	/// スクリーンがスクロールするときに主に使用する
	/// @param [in] x 移動先のX座標
	/// @param [in] y 移動先のY座標
	///
	public virtual void Move(int x, int y)
	{
		positionX = x;
		positionY = y;
	}

	/// 初期位置設定
	/// スクリーンがスクロールするときに主に使用する
	/// @param [in] x 移動先のX座標
	/// @param [in] y 移動先のY座標
	///
	public virtual void InitPos(int x, int y)
	{
		baseX = x;
		baseY = y;
		positionX = baseX;
		positionY = baseY;
	}
	
	/// 衝突したか
	///
	/// @param [in] isCol 衝突フラグ
	///
	public virtual void SetCollisionFlag(bool isCol)
	{
	}
		
	/// 衝突フラグ取得
	///
	/// @param [out]
	///
	public bool GetCollisionFlag()
	{
		return isCollision;
	}

	public Target Parent
    {
        get {return parent;}
    }

    /// 親の登録
    public void SetParent(Target value)
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
}

} // Physics2dDemo