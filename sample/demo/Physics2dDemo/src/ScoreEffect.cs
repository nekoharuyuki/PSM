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
/// ScoreEffectクラス
/// 衝突時スコア表示
///
public class ScoreEffect : Target
{
	private SpriteAnimation[] anims;
	private SpriteAnimation[] anims2;//描画中上昇する２D
	private long animTimeMillis;
	private long addTimeMillis;
	private float  upY=-10;//anim2を上昇させる値

	/// 優先度
    public override int Priority
    {
        get {return -6;}
    }

	/// スコア種別
	public enum ScoreType
	{
	}

	public enum RepeatMode
	{
	    Constant,
	    Loop
	}
    /// コンストラクタ
	public ScoreEffect(string name = null) : base(name)
	{
	}
	
	/// 開始処理
	/// NumberLayoutの生成
	/// @param [out]
	///
	public override bool Start(TargetManager targetManager)
	{
		var image = Resource2d.GetInstance().ImageLyt;
		
		anims = new SpriteAnimation[] {
                new SpriteAnimation(new Sprite(image,   4, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image,  28, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image,  52, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image,  76, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 100, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 124, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 148, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 172, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 196, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00),
                new SpriteAnimation(new Sprite(image, 220, 465, 24, 35, 0, 0), 0, 1000, true, 0, 0, 0x88, 0, 0, 0x00)
            };
			
		anims2 = new SpriteAnimation[] {
                new SpriteAnimation(new Sprite(image,   4, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,  28, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,  52, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image,  76, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 100, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 124, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 148, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 172, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 196, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff),
                new SpriteAnimation(new Sprite(image, 220, 465, 24, 35, 0, 0), 0, 0, false, 0, 0, 0xff, 0, 0, 0xff)
            };
		
		score = 0;
		animTimeMillis = 0;
	    return true;
	}
	
	/// 終了処理
	///
	/// @param [out]
	///
	public override bool End(TargetManager targetManager)
	{
		Array.ForEach(anims, (anim) => anim.Dispose());
		Array.ForEach(anims2, (anim2) => anim2.Dispose());
        anims = null;
		anims2= null;
	    return true;
	}
	
	/// 更新処理
	///
	/// @param [out]
	///
	public override bool Update(TargetManager targetManager)
	{	
		//衝突してないのは表示しない
		if(!isCollision || (isCollision && animTimeMillis == anims[0].EndTimeMillis)){
			animTimeMillis = 0;
			isCollision = false;
			upY = -10;
			return true;
		}
		
		//スコアが変わったら時間初期化
		if(old_score != score){
			old_score = score;
			animTimeMillis = 0;
			upY = -10;
		}
					
		addTimeMillis = GameData.FrameTimeMillis;
		
        animTimeMillis += addTimeMillis;

    	long endTimeMillis = anims[0].EndTimeMillis;
		upY -= 2.0f;

   		if (animTimeMillis > endTimeMillis) {
        	animTimeMillis = endTimeMillis;
    	}
		
		return true;
	}
	
	/// 描画
	///
	/// @param [out]
	///
	public override bool Render(TargetManager targetManager)
	{
		//衝突してないのは表示しない
		if(!isCollision || score == 0)
			return true;
	
		int digit = 0;
        long calcNumber = Math.Abs(score);
        while (calcNumber > 0) {
            calcNumber /= 10;
            digit++;
        }

        calcNumber = Math.Abs(score);
		
		if(positionX-(854+GameData.WindowPosX) >= -1000 && positionX-(854+GameData.WindowPosX) < 1000)
		{
        	for (int i = 0; i < digit; i++) {
            	anims[calcNumber % 10].Render( animTimeMillis, positionX-24*i, positionY);
           		anims2[calcNumber % 10].Render( animTimeMillis, positionX-24*i, positionY+(int)upY);			
            	calcNumber /= 10;
			}
        }
		return true;
	}
    
}

} // Physics2dDemo
