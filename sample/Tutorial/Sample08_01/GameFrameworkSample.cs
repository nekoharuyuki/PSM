/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;

using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace Sample
{
	public class GameFrameworkSample: GameFramework
	{
		public ImageRect rectScreen;
		public Random rand = new Random(123);
		
		public Int32 appCounter=0;
		
		public Actor Root{ get; set;}
		
		
		public Texture2D texturePlayer, textureStar, textureBullet, textureEnemy, textureEnemyBullet;
		
		Sound soundBullet;
		public SoundPlayer soundPlayerBullet;
		
		
		Bgm bgm;
		public BgmPlayer bgmPlayer;
		
		
		public Int32 GameCounter=0;
		public Int32 Score=0;
		public Int32 NumShips;
		
		
		public enum StepType
		{
			Opening,
			GamePlay,
			GameOver,
		}
		
		public StepType Step;
		
		
		public override void Initialize()
		{
			base.Initialize();
			
			rectScreen = graphics.GetViewport();
			
			
			//@e Initialization of actor tree
			//@j アクターツリーの初期化。
			Root = new Actor("root");
			

			//@e Initialization of Player
			//@j プレイヤーの初期化。
			texturePlayer = new Texture2D("/Application/resources/Player.png", false);
			Player player = new Player(this, "Player", texturePlayer);
			Root.AddChild(player);
			
			
			//@e Initialization of bullet
			//@j 弾の初期化。
			textureBullet = new Texture2D("/Application/resources/Bullet.png", false);			
			Actor bulletManager = new Actor("bulletManager");
			Root.AddChild(bulletManager);

			
			//@e Initialization of enemy
			//@j 敵の初期化。
			textureEnemy = new Texture2D("/Application/resources/Enemy.png", false);			
			Actor enemyManager = new Actor("enemyManager");
			Root.AddChild(enemyManager);
			
			
			//@e Initialization of enemy's bullet
			//@j 敵の弾の初期化。
			textureEnemyBullet = new Texture2D("/Application/resources/EnemyBullet.png", false);
			Actor enemyBulletManager = new Actor("enemyBulletManager");
			Root.AddChild(enemyBulletManager);

			//@e Class to make appearance of enemies
			//@j 敵を出現させるクラス。
			Root.AddChild(new EnemyCommander(this, "enemyCommander"));
			
			
			//@e Initialization of star 
			//@j 星の初期化。
			textureStar = new Texture2D("/Application/resources/Star.png", false);			
			Actor starManager = new Actor("starManager");
			
			//@e Star color 
			//@j 星の色。
			Vector4[] starColors ={
				new Vector4( 0.5f, 0.5f, 1.0f, 1.0f),	// Blue
				new Vector4( 0.0f, 1.0f, 0.0f, 1.0f),	// Green
				new Vector4( 1.0f, 1.0f, 0.0f, 1.0f),	// Yellow
				new Vector4( 1.0f, 0.5f, 0.0f, 1.0f),	// Orange
				new Vector4( 1.0f, 0.0f, 0.0f, 1.0f),	// Red
			};
			
			Star star;
			for( int i=0; i< 20; ++i)
			{
				star= new Star(this, "star"+i, textureStar, 
					new Vector3((float)(rectScreen.Width * rand.NextDouble()),(float)(rectScreen.Height* rand.NextDouble()),0.6f),
					starColors[ i % starColors.Length],
					(float)(1.0f * (rand.NextDouble() + 0.5f)));
				
				starManager.AddChild(star);
			}
			Root.AddChild(starManager);
			
			
			//@e Initialization of sound effect 
			//@j 効果音の初期化。
			soundBullet = new Sound("/Application/sound/Bullet.wav");
			soundPlayerBullet = soundBullet.CreatePlayer();
			
			
			//@e Initialization of BGM
			//@j BGMの初期化。
			bgm = new Bgm("/Application/sound/GameBgm.mp3");
			bgmPlayer = bgm.CreatePlayer();
			bgmPlayer.Loop = true;
			bgmPlayer.Play();
		}
		
		
		public override void Update()
		{
			base.Update();
			
#if DEBUG
			debugString.WriteLine("counter "+appCounter);
			debugString.WriteLine("Buttons="+PadData.Buttons);
			debugString.WriteLine("Score="+Score);
#endif
			
			Root.Update();
			
			Root.CheckStatus();
			
			++appCounter;
		}
		
		
		public override void Render()
		{
			graphics.Clear();
			
			graphics.Enable(EnableMode.DepthTest);
			
			Root.Render();
			
			base.Render();
		}
		
		public override void Terminate ()
		{
			texturePlayer.Dispose();
			textureStar.Dispose();
			textureBullet.Dispose();
			textureEnemy.Dispose();
			textureEnemyBullet.Dispose();

			soundBullet.Dispose();
			soundPlayerBullet.Dispose();
			
			bgm.Dispose();
			bgmPlayer.Dispose();
			
			
			base.Terminate ();
		}
	}
}
