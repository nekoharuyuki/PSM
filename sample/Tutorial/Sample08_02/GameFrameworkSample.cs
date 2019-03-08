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
		
		
		public Texture2D texturePlayer, textureStar, textureBullet, textureExplosion, textureEnemy, textureEnemyBullet, 
			textureTitle, texturePressStart, textureGameOver;
		
		Sound soundBullet;
		public SoundPlayer soundPlayerBullet;
		
		Sound soundExplosion;
		public SoundPlayer soundPlayerExplosion;
		
		Sound soundButton;
		public SoundPlayer soundPlayerButton;
		
		
		Bgm bgm;
		public BgmPlayer bgmPlayer;
		
		
		public Int32 GameCounter=0;
		public Int32 Score=0;
		public Int32 NumShips;
		
		public DebugString debugStringScore, debugStringShip;
		
		
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
			
			debugStringScore=new DebugString(graphics);
			debugStringScore.SetColor(new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
			
			debugStringShip=new DebugString(graphics);
			debugStringShip.SetColor(new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
			
			//@e Initialization of actor tree  
			//@j アクターツリーの初期化。
			Root = new Actor("root");
			
			
			textureTitle = new Texture2D("/Application/resources/SimpleShooting.png", false); 
			texturePressStart = new Texture2D("/Application/resources/PressStart.png", false); 
			textureGameOver = new Texture2D("/Application/resources/GameOver.png", false);
			
			//@e Initialization of game manager 
			//@j ゲームマネージャーの初期化。
			Actor gameManager = new GameManager(this, "gameManager");
			Root.AddChild(gameManager);
			
			//@e Initialization of Player 
			//@j プレイヤーの初期化。
			texturePlayer = new Texture2D("/Application/resources/Player.png", false);
			Player player = new Player(this, "Player", texturePlayer);
			player.Status = Actor.ActorStatus.Rest;
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
			
			
			//@e Initialization of fumes from explosion
			//@j 爆煙の初期化。
			textureExplosion = new Texture2D("/Application/resources/GraySmoke.png", false);
			Actor effectManager = new Actor("effectManager");
			Root.AddChild(effectManager);
			
			//@e Class to make appearance of enemies
			//@j 敵を出現させるクラス。
			Root.AddChild(new EnemyCommander(this, "enemyCommander"));
			
			//@e Collision detection
			//@j あたり判定。
			Root.AddChild(new CollisionCheck(this, "collisionCheck"));
			
			//@e Initialization of sound effect 
			//@j 効果音の初期化。
			soundBullet = new Sound("/Application/sound/Bullet.wav");
			soundPlayerBullet = soundBullet.CreatePlayer();
			
			soundExplosion = new Sound("/Application/sound/Explosion.wav");
			soundPlayerExplosion = soundExplosion.CreatePlayer();
			
			soundButton = new Sound("/Application/sound/SYS_SE_01.wav");
			soundPlayerButton = soundButton.CreatePlayer();
			
			//@e Initialization of BGM
			//@j BGMの初期化。
			bgm = new Bgm("/Application/sound/GameBgm.mp3");
			bgmPlayer = bgm.CreatePlayer();
			bgmPlayer.Loop = true;
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
		
		public override void Terminate()
		{
			texturePlayer.Dispose();
			textureStar.Dispose();
			textureBullet.Dispose();
			textureExplosion.Dispose();
			textureEnemy.Dispose();
			textureEnemyBullet.Dispose();
			textureTitle.Dispose();
			texturePressStart.Dispose();
			textureGameOver.Dispose();

			soundBullet.Dispose();
			soundPlayerBullet.Dispose();

			soundExplosion.Dispose();
			soundPlayerExplosion.Dispose();
			soundButton.Dispose();
			soundPlayerButton.Dispose();
			bgm.Dispose();
			bgmPlayer.Dispose();
			
			
			base.Terminate ();
		}
	}
}
