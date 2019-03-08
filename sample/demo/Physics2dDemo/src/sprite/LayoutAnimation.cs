/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

namespace Physics2dDemo
{

///
/// LayoutAnimationクラス
/// アニメーションのレイアウト
///
public abstract class LayoutAnimation : IDisposable
{
    protected long startTimeMillis;
    protected long playTimeMillis;
    protected long defaultPlayTimeMillis;//生成時の再生時間
    protected bool killEnd;
//    protected int interpolation;
    protected int offsetX1;
    protected int offsetY1;
    protected uint color1;
    protected int offsetX2;
    protected int offsetY2;
    protected uint color2;

    public long EndTimeMillis
    {
        get {return startTimeMillis + playTimeMillis;}
    }

    /// コンストラクタ
    /// 
    public LayoutAnimation(long startTimeMillis,
                           long playTimeMillis,
                           bool killEnd,
                           int offsetX1,
                           int offsetY1,
                           uint color1,
                           int offsetX2,
                           int offsetY2,
                           uint color2)
    {
        this.startTimeMillis = startTimeMillis;
        this.playTimeMillis = playTimeMillis;
        this.defaultPlayTimeMillis = playTimeMillis;
        this.killEnd = killEnd;
        this.offsetX1 = offsetX1;
        this.offsetY1 = offsetY1;
        this.color1 = color1;
        this.offsetX2 = offsetX2;
        this.offsetY2 = offsetY2;
        this.color2 = color2;
    }

    /// 破棄
    public virtual void Dispose()
    {
    }

    /// 描画処理
    public virtual void Render(long animTimeMillis, int offsetX = 0, int offsetY = 0)
    {
    }

    /// 指定時間の値を線形補間で取得(int型)
    protected static int getValue(int nFromValue, int nToValue, long nNowTime, long nMaxTime)
    {
        if (nNowTime <= 0) {
            return nFromValue;
        } else if (nNowTime >= nMaxTime) {
            return nToValue;
        } else {
            return (int)(nFromValue + (nToValue - nFromValue) * nNowTime / nMaxTime);
        }
    }

    /// 指定時間の値を線形補間で取得(long型)
    protected static long getValue(long nFromValue, long nToValue, long nNowTime, long nMaxTime)
    {
        if (nNowTime <= 0) {
            return nFromValue;
        } else if (nNowTime >= nMaxTime) {
            return nToValue;
        } else {
            return (long)(nFromValue + (nToValue - nFromValue) * nNowTime / nMaxTime);
        }
    }

    /// 再生時間の算出
    protected long calcAnimPlayTimeMillis(long animTimeMillis)
    {
        long animPlayTimeMillis = (animTimeMillis - startTimeMillis);

        if (animPlayTimeMillis >= 0) {
            if (playTimeMillis <= 0) {
                animPlayTimeMillis = 0;
            } else if (animPlayTimeMillis >= playTimeMillis) {
                if (killEnd) {
                    animPlayTimeMillis = -1;
                } else {
                    animPlayTimeMillis = playTimeMillis;
                }
            }
        }

        if (animPlayTimeMillis < 0 || animPlayTimeMillis > playTimeMillis) {
            animPlayTimeMillis = -1;
        }

        return animPlayTimeMillis;
    }
		
	/// 再生時間の変更
	/// 再生時間の間隔を常時変更するときに使用
	/// @param par 再生間隔の比率（％）
	/// @warning par 1~200 範囲外はデフォルトに戻す
	/// 
	/// 
	public long ChangeAnimPlayTimeMillis(int par)
	{
		if(par <= 0 || par > 200){//デフォルトに戻す
			this.playTimeMillis = defaultPlayTimeMillis;
		}else{
			this.playTimeMillis = (long)((defaultPlayTimeMillis * par) / 200);
		}
		return playTimeMillis;
	}

	/// アニメーションの位置の変更
	/// @param x x座標(int)
	/// @param y y座標(int)
	public void ChangePosition(int x, int y)
	{
        this.offsetX1 = x;
        this.offsetY1 = y;
        this.offsetX2 = x;
        this.offsetY2 = y;
	}

    /// 角度設定
    /// @param d 角度(float)
    public virtual void SetDegree(float d)
    {
    }

    /// Center設定
    /// @param d 角度(float)
    public virtual void SetCenter(float x, float y)
    {
    }

    /// ScaleY設定
    /// @param scale 拡大率(float)
	public virtual void SetScaleY(float scale)
	{
	}
}

} // Physics2dDemo
