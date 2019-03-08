/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace Physics2dDemo
{

///
/// Scoreクラス
/// 総スコアの計算
///
public class Score : Target
{
	private NumberLayout scoreLayout;
	private long totalScore = 0;
	private long prevScore = 0; //前回のスコア
		
	/// 優先度
    public override int Priority
    {
        get {return -5;}
    }
	
    /// コンストラクタ
	public Score(string name = null) : base(name)
	{
	}		

    /// 初期化
    public override bool Start(TargetManager targetManager)
    {
		var image = Resource2d.GetInstance().ImageLyt;
		
		scoreLayout = new NumberLayout(
            new SpriteAnimation[] {
                new SpriteAnimation(new Sprite(image,  4, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 33, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 62, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 91, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,120, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,149, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,178, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,207, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,236, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,265, 417, 29, 44, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff)
            },
            new int[]{434, 405, 376, 347},
            new int[]{15, 15, 15, 15});
			
		return true;
    }

    /// 解放
    public override bool End(TargetManager targetManager)
    {
		Release();
		return true;
    }
    /// 破棄
    private void Release()
    {			
		totalScore = 0;
		if (scoreLayout != null) {
            scoreLayout.Dispose();
            scoreLayout = null;
        }

		if(action != null)
		{
			action.Dispose();
			action = null;
		}
	}

	/// 更新
	///
	/// @param [out]
	///
    public override bool Update(TargetManager targetManager)
    {
		return true;
    }

	/// 描画
	///
	/// @param [out]
	///
    public override bool Render(TargetManager targetManager)
    {
		scoreLayout.Render(totalScore+prevScore);
		return true;
    }
		
    /// スコア
    public long TotalScore
    {
        get {return totalScore;}
    }

    /// 前回のスコア
    public long PrevScore
    {
        set {prevScore = value;}
        get {return prevScore;}
    }
    /// スコアの足しこみ
    public void AddScore(long score)
    {
		totalScore += score;
    }

	/// スコアのクリア
    public void ClearScore()
    {
		totalScore = 0;
    }
}	

} // Physics2dDemo
