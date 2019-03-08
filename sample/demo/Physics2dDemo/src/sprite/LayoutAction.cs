/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

public enum RepeatMode
{
    Constant,
    Loop
}

///
/// LayoutActionクラス
/// LayoutAnimationListを追加し、
/// アニメーションの設定・変更・更新・描画を行なう
///
public class LayoutAction : IDisposable
{
    private string currentKey = null;
    private Dictionary<string, LayoutAnimationList> animTable = new Dictionary<string, LayoutAnimationList>();
    private long animTimeMillis;
    public int RenderOffsetX {get; set;}
    public int RenderOffsetY {get; set;}
    public RepeatMode RepeatMode {get; private set;}

    /// コンストラクタ
    ///
    public LayoutAction(int renderOffsetX = 0, int renderOffsetY = 0)
    {
        RenderOffsetX = renderOffsetX;
        RenderOffsetY = renderOffsetY;

        RepeatMode = RepeatMode.Constant;
    }

    /// 破棄
    /// 
    public void Dispose()
    {
        foreach (string key in animTable.Keys) {
            animTable[key].Dispose();
        }

        animTable.Clear();
    }

    /// アニメーションリストの追加
    /// 
    public void Add(string key, LayoutAnimationList animList)
    {
        if (Find(key) == null) {
            animTable[key] = animList;
        }
    }

    /// アニメーションリストの検索
    /// 
    public LayoutAnimationList Find(string key)
    {
        if (key != null && animTable.ContainsKey(key)) {
            return animTable[key];
        }

        return null;
    }

    /// カレントのアニメーションリストの取得
    /// 
    public LayoutAnimationList Current()
    {
        return Find(currentKey);
    }

    /// カレントのアニメーションリストのキーの取得
    /// 
    public string CurrentKey
    {
        get {return currentKey;}
    }

    /// アニメーションの切替
    /// 
    public void ChangeCurrent(string key, RepeatMode repeatMode = RepeatMode.Constant)
    {
        if (currentKey != key) {
            SetCurrent(key, repeatMode);
        }
    }

    /// アニメーションのセット
    /// 
    public void SetCurrent(string key, RepeatMode repeatMode = RepeatMode.Constant)
    {
        currentKey = key;
        animTimeMillis = 0;
        RepeatMode = repeatMode;
    }

    /// 更新処理
    /// 
    public void Update(long addTimeMillis)
    {
        var anim = Current();

        if (anim != null) {
            animTimeMillis += addTimeMillis;

            long endTimeMillis = anim.EndTimeMillis();

            if (animTimeMillis > endTimeMillis) {
                switch (RepeatMode) {
                case RepeatMode.Constant:
                    animTimeMillis = endTimeMillis;
                    break;
                case RepeatMode.Loop:
                    if (endTimeMillis > 0) {
                        animTimeMillis %= endTimeMillis;
                    }
                    break;
                }
            }
        }
    }

    /// 描画処理
    /// 
    public void Render()
    {
        var anim = Current();

        if (anim != null) {
            anim.Render(animTimeMillis, RenderOffsetX, RenderOffsetY);
        }
    }

    /// 再生終了確認
    /// 
    public bool IsPlayEnd()
    {
        var anim = Current();
        if (anim != null && animTimeMillis >= anim.EndTimeMillis()) {
            return true;
        }

        return false;
    }
	
	/// 再生時間の変更
	/// @param par 再生間隔の比率（％）
	public void ChangeActionPlayTimeMillis(int par)
	{
		foreach (string key in animTable.Keys) {
            Find(key).ChangeEndTimeMillis(par);
        }
	}
				
	/// アニメーションの位置の変更
	/// @param x x座標(int)
	/// @param y y座標(int)
	public void ChangeActionPosition(int x, int y)
	{
		foreach (string key in animTable.Keys) {
            Find(key).ChangePosition(x, y);
        }
	}

	/// アニメーションの角度の設定
	/// @param d (float)
	public void SetDegree(float d)
	{
        foreach (string key in animTable.Keys) {
            Find(key).SetDegree(d);
        }
	}

	/// アニメーションのCenterの設定
	/// @param x (float)
	/// @param y (float)
	public void SetCenter(string key, float x, float y)
	{
       	Find(key).SetCenter(x,y);
	}
		
	/// アニメーションのScaleYの設定
	/// @param scale (float)
	public void SetScaleY(string key, float scale)
	{
       	Find(key).SetScaleY(scale);
	}
}

} // Physics2dDemo
