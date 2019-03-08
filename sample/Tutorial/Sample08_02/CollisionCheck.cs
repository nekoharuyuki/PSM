/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Framework;


namespace Sample
{
	//@e Class to use collision detection.
	//@j あたり判定をおこなうクラス。
	public class CollisionCheck : GameActor
	{
		public CollisionCheck(GameFrameworkSample gs, string name) : base (gs, name){}

		public override void Update()
		{
			Player player =(Player)gs.Root.Search("Player");
			Actor bulletManager=gs.Root.Search("bulletManager");
			Actor enemyManager =gs.Root.Search("enemyManager");
			Actor bulletEnemyManager=gs.Root.Search("enemyBulletManager");
			Actor effectManager =gs.Root.Search("effectManager");
			
			//@e Collision detection for bullets and enemies
			//@j 弾と敵のあたり判定。
			foreach( Bullet bullet in  bulletManager.Children)
			{
				if(bullet.Status == Actor.ActorStatus.Action)
				{
					foreach( Enemy enemy in  enemyManager.Children)
					{
						if(enemy.Status ==  Actor.ActorStatus.Action &&  
						   Math.Abs(bullet.Sprite.Position.X -enemy.Sprite.Position.X ) < 30 && 
							Math.Abs(bullet.Sprite.Position.Y -enemy.Sprite.Position.Y ) < 30
								)
						{
							bullet.Status = Actor.ActorStatus.Dead;
							enemy.Status = Actor.ActorStatus.Dead;
							effectManager.AddChild(new Explosion(gs, "explosion", gs.textureExplosion, 
							    new Vector3(enemy.Sprite.Position.X, enemy.Sprite.Position.Y, 0.3f)));
							
							gs.Score += 100;
							gs.soundPlayerExplosion.Play();
						}
					}
				}
			}
			
			if(player.playerStatus== Player.PlayerStatus.Normal)
			{
				//@e Collision detection of player and enemy
				//@j 自機と敵のあたり判定。
				foreach( Enemy enemy in  enemyManager.Children)
				{
					if(enemy.Status ==  Actor.ActorStatus.Action &&  
					   Math.Abs(player.Sprite.Position.X -enemy.Sprite.Position.X ) < 42 && 
						Math.Abs(player.Sprite.Position.Y -enemy.Sprite.Position.Y ) < 42
							)
					{
						
						effectManager.AddChild(new Explosion(gs, "explosion", gs.textureExplosion, 
						    new Vector3(player.Sprite.Position.X, player.Sprite.Position.Y, 0.3f)));
						
						player.playerStatus = Player.PlayerStatus.Explosion;	
						
						gs.soundPlayerExplosion.Play();
						
						gs.NumShips--;
					}
				}
				
				
				//@e Collision detection for player's and enemy's bullet
				//@j 自機と敵弾のあたり判定。
				foreach( EnemyBullet enemyBullet in  bulletEnemyManager.Children)
				{
					if(enemyBullet.Status ==  Actor.ActorStatus.Action &&  
					   Math.Abs(player.Sprite.Position.X -enemyBullet.Sprite.Position.X ) < 26 && 
						Math.Abs(player.Sprite.Position.Y -enemyBullet.Sprite.Position.Y ) < 26
							)
					{
						enemyBullet.Status = Actor.ActorStatus.Dead;
						effectManager.AddChild(new Explosion(gs, "explosion", gs.textureExplosion, 
						    new Vector3(player.Sprite.Position.X, player.Sprite.Position.Y, 0.3f)));
						
						player.playerStatus = Player.PlayerStatus.Explosion;	
						gs.soundPlayerExplosion.Play();
						gs.NumShips--;
					}
				}
			}
			
			base.Update();
		}
	}
}

