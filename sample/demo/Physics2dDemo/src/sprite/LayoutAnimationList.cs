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

///
/// LayoutAnimationListクラス
/// アニメーションするリスト
///
public class LayoutAnimationList : IDisposable
{
    private List<LayoutAnimation> animList = new List<LayoutAnimation>();

    /// 破棄
    public void Dispose()
    {
        animList.ForEach(anim => anim.Dispose());
    }

    /// アニメーションリストの追加
    /// 
    public void Add(LayoutAnimation anim)
    {
        animList.Add(anim);
    }

    /// 描画処理
    /// 
    public void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
        foreach (var anim in animList) {
            anim.Render(animTimeMillis, offsetX, offsetY);
        }
    }

    /// 再生終了時間の算出
    /// 
    public long EndTimeMillis()
    {
        long endTimeMillis = 0;

        foreach (var anim in animList) {
            if (endTimeMillis < anim.EndTimeMillis) {
                endTimeMillis = anim.EndTimeMillis;
            }
        }

        return endTimeMillis;
    }
		
	/// アニメーション再生時間を調節する
    public void ChangeEndTimeMillis(int par)
    {
        foreach (var anim in animList) {
            anim.ChangeAnimPlayTimeMillis(par);
        }
	}

	/// アニメーションの位置の変更
	/// @param x x座標(int)
	/// @param y y座標(int)
	public void ChangePosition(int x, int y)
	{
        foreach (var anim in animList) {
            anim.ChangePosition(x, y);
        }
	}

	/// アニメーションの角度の設定
	/// @param d (float)
	public void SetDegree(float d)
	{
        foreach (var anim in animList) {
            anim.SetDegree(d);
        }
	}

	/// アニメーションのCenterの設定
	/// @param x (float)
	/// @param y (float)
	public void SetCenter(float x, float y)
	{
        foreach (var anim in animList) {
            anim.SetCenter(x,y);
        }
	}
		
	/// アニメーションのScaleYの設定
	/// @param scale (float)
	public void SetScaleY(float scale)
	{
		foreach (var anim in animList) {
       		anim.SetScaleY(scale);
		}
	}
}

} // Physics2dDemo
