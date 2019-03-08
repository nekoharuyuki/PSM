
using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace SirAwesome
{
	public class Player
        : PhysicsGameEntity
    {
        public const float GroundResponsiveness = 1.35f;
        public const float AirResponsiveness = 1.25f;
        public const float JumpPower = 11.0f;
        public const float IdleAnimationSpeedThreshold = 1.25f;

        public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile BodySprite { get; set; }
        public string CurrentAnimation { get; set; }
        public Dictionary<string, Support.AnimationAction> AnimationTable { get; set; }

        public float AttackTime { get; set; }
        
		public PlayerSwordAttack SwordAttack { get; set; }
		
        public float Beer { get; set; }
        public int Coins { get; set; }
        
		public bool DoneFirstLanding { get; set; }
		
		public int FootstepDelay { get; set; }
		
        public Player()
        {
            BodySprite = Support.TiledSpriteFromFile("/Application/assets/sir_awesome_frames.png", 4, 4);
            this.AddChild(BodySprite);
            
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite),
				radius = () => 14.0f,
			});
			
            CollisionDatas.Add(new EntityCollider.CollisionEntry() {
	            type = EntityCollider.CollisionEntityType.Player,
				owner = this,
				collider = BodySprite,
				center = () => GetCollisionCenter(BodySprite) + new Vector2(0.0f, -40.0f),
				radius = () => 8.0f,
			});
			
			const float SingleFrame = 1.0f / 60.0f;
			AnimationTable = new Dictionary<string, Support.AnimationAction>() {
				{ "Idle", new Support.AnimationAction(BodySprite, 4, 5, SingleFrame * 30, looping: true) },
				{ "Walk", new Support.AnimationAction(BodySprite, 0, 4, SingleFrame * 60, looping: true) },
				{ "Attack", new Support.AnimationAction(BodySprite, 5, 7, SingleFrame * 6) },
				{ "JumpAttack", new Support.AnimationAction(BodySprite, 11, 13, SingleFrame * 6) },
				{ "Jump", new Support.AnimationAction(BodySprite, 8, 10, 0.2f) },
				{ "Fall", new Support.AnimationAction(BodySprite, 9, 10, SingleFrame * 30) },
			};
				
			SetAnimation("Fall");
			
			AttackTime = -1.0f;
			
			Position = new Vector2(400.0f, 1500.0f);
	        AirFriction = new Vector2(0.70f, 0.98f);
	        GroundFriction = new Vector2(0.70f, 0.99f);
	        
			Beer = 4.0f;
			Coins = 0;
			Health = 5.0f;
        }

        public override void Tick(float dt)
        {
        	base.Tick(dt);
            
            // DEBUG
			//Game.Instance.ParticleEffects.AddParticlesBurst(4, GetCollisionCenter(BodySprite), Vector2.UnitY, Colors.Blue, 4.0f);
			
            if (InvincibleTime <= 0.0f)
			{
	            // ground control (to major tom)
	            if (AirborneTime == 0.0f)
	            {
	            	float axis = PlayerInput.LeftRightAxis();
	            	Velocity += Vector2.UnitX * GroundResponsiveness * axis;
	                    
	                if (PlayerInput.JumpButton())
					{
			            // cannot jump while attacking
		            	if (AttackTime <= 0.0f)
						{
							SetAnimation("Jump");
		                    Velocity += Vector2.UnitY * JumpPower;
		                    EmitJumpLandParticles(5, 3.0f);
							Support.SoundSystem.Instance.Play("player_jump.wav");
						}
					}
                    
	                // no movement if attacking from ground
					if (AttackTime > 0.0f)
						Velocity = new Vector2(0.0f, Velocity.Y);
	                
					if (System.Math.Abs(Velocity.X) > IdleAnimationSpeedThreshold)
					{
						if (CurrentAnimation == "Idle")
							SetAnimation("Walk");

						if (FootstepDelay <= 0)
						{
							Support.SoundSystem.Instance.PlayNoClobber("player_walk.wav");
							FootstepDelay = 10;
						}
						FootstepDelay -= 1;
					}
					else
					{
						if (CurrentAnimation == "Walk")
							SetAnimation("Idle");
					}
				}
	            // air control
	            else
	            {
	            	float axis = PlayerInput.LeftRightAxis();
	            	Velocity += Vector2.UnitX * AirResponsiveness * axis;
	
	                // double jump?
	                //if (AirborneFrames > N) ?
	            	//if (PlayerInput.JumpButton())
	                    //Velocity += Vector2.UnitY * JumpPower;
	            }
            }

            // Attacks
            if (AttackTime < 0.0f)
			{
				if (PlayerInput.AttackButton())
				{
					StartAttack();
				}
			}
			
			if (PlayerInput.SpecialButton())
			{
				if (Beer > 1.0f)
				{
					ThrowGlass();
				}
			}
                
			
			if (AttackTime > 0.0f)
			{
				AttackTime -= dt;
				
				if (AttackTime <= 0.0f)
				{
					StopAttack();
				}
			}

			// if on floor, stop fall
			if (AirborneTime <= 0.0f)
            {
				if (CurrentAnimation == "Fall")
				{
					DetermineAnimation();
					EmitJumpLandParticles(8, 3.0f);
					Support.SoundSystem.Instance.Play("player_land.wav");
					
					if (!DoneFirstLanding)
					{
						EmitJumpLandParticles(64, 5.0f);
						Game.Instance.UI.Shake = 1.75f;
						DoneFirstLanding = true;
					}
				}
					
				if (CurrentAnimation == "JumpAttack")
					StopAttack();
            }
            else
            {
				if (CurrentAnimation == "Jump")
				{
					if (Velocity.Y < 0.0f)
						SetAnimation("Fall");
				}
            }
            
			// touch attack
			CheckTouch();	

			// walk animation speed based on velocity
			AnimationTable["Walk"].SetSpeed(System.Math.Abs(Velocity.X));
			
			// 0.0f will just leave the flip state as-is
			if (InvincibleTime <= 0.0f)
			{
				if (Velocity.X < -0.1f)
					BodySprite.FlipU = true;
				if (Velocity.X > 0.1f)
					BodySprite.FlipU = false;
			}
			
			//if (CurrentAnimation == "Attack")
				//System.Console.WriteLine("frame: {0}, {1}", BodySprite.TileIndex2D.X, BodySprite.TileIndex2D.Y);
				
			Beer = FMath.Min(Beer + 0.15f * dt, 4.0f);
        }
        
		public void StartAttack()
		{
			//Game.Instance.ParticleEffects.AddParticle(GetCollisionCenter(BodySprite), Colors.White);
			//Game.Instance.ParticleEffects.AddParticlesBurst(1, GetCollisionCenter(BodySprite), Vector2.UnitY, Colors.Blue, 4.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemyZombie", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite), Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemySlime", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite), Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesTile("EnemySlime", 0, BodySprite.FlipU, GetCollisionCenter(BodySprite) + Vector2.UnitX * 150.0f, Vector2.UnitY, 1.0f);
			//Game.Instance.ParticleEffects.AddParticlesCone(16, GetCollisionCenter(BodySprite), Vector2.UnitY, Colors.White, 1.0f);
			//DropCoinsWithAChanceOfHeart(GetCollisionCenter(BodySprite) + Vector2.UnitY * 160.0f, 4);
			
			if (AirborneTime > 0.0f)
			{
				SetAnimation("JumpAttack");
				AttackTime = 0.225f;
			}
			else
			{
				SetAnimation("Attack");
				AttackTime = 0.125f;
			}
			
			SwordAttack = new PlayerSwordAttack();
			this.AddChild(SwordAttack);

			Support.SoundSystem.Instance.Play("player_sword_attack.wav");
		}
		
		public void ThrowGlass()
		{
			var glass = new PlayerGlassAttack() { Position = GetCollisionCenter(BodySprite) };
			glass.Velocity = new Vector2((BodySprite.FlipU ? -1.0f : 1.0f) * 8.0f, 8.0f);
			Game.Instance.World.AddChild(glass);
			Support.SoundSystem.Instance.Play("beer_throw.wav");
			Beer -= 1.0f;
		}
		
		public void StopAttack()
		{
			AttackTime = -1.0f;
			this.RemoveChild(SwordAttack,true);
			SwordAttack = null;
			DetermineAnimation();
		}
		
		public void CheckTouch()
		{
			if (Input2.Touch00.Press)
			{
				Vector2 position = Game.Instance.Scene.Camera.GetTouchPos();
				var touch_attack = new PlayerTouchAttack() { Position = position, };
				Game.Instance.World.AddChild(touch_attack);
			}
		}
        
		public void DetermineAnimation()
		{
			if (AttackTime > 0.0f)
			{
				if (AirborneTime > 0.0f)
					SetAnimation("Jump");
				else
					SetAnimation("Attack");
			}
			else	
			{
				if (AirborneTime > 0.0f)
				{
					if (Velocity.Y < 0.0f)
						SetAnimation("Jump");
					else
						SetAnimation("Fall");
				}
				else
				{
					if (Velocity.X != 0.0f)
						SetAnimation("Walk");
					else
						SetAnimation("Idle");
				}
			}
		}
        
		public void SetAnimation(string animation)
		{
			if (CurrentAnimation != null)
				BodySprite.StopAction(AnimationTable[CurrentAnimation]);
				
			CurrentAnimation = animation;
			BodySprite.RunAction(AnimationTable[animation]);
			AnimationTable[animation].Reset();
			
			//Console.WriteLine("SetAnimation(): {0}", animation);
		}
		
		public override void CollideFrom(GameEntity owner, Node collider)
		{
			base.CollideTo(owner, collider);
			
			Type type = owner.GetType();
			if (type == typeof(EnemySlime) ||
				type == typeof(EnemyRedSlime) || 
				type == typeof(EnemyZombie) ||
				type == typeof(EnemyBat))
			{
				Vector2 source = GetCollisionCenter(collider);
				Vector2 ofs = GetCollisionCenter(BodySprite) - source;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				Vector2 push = ofs.Normalize() * new Vector2(20.0f, 1.0f) + Vector2.UnitY * 4.0f;
				Velocity += push;
				(owner as PhysicsGameEntity).Velocity -= push * 0.1f;
				(owner as PhysicsGameEntity).InvincibleTime = 0.50f;
				InvincibleTime = 0.25f;
				
				if (type == typeof(EnemySlime))
				{
					TakeDamage(0.2f, source);
				}
				else if (type == typeof(EnemyRedSlime))
				{
					TakeDamage(0.4f, source);
					owner.TakeDamage(0.1f, GetCollisionCenter(BodySprite));
				}
				else if (type == typeof(EnemyZombie))
				{
					TakeDamage(0.75f, source);
				}
				else if (type == typeof(EnemyBat))
				{
					TakeDamage(0.35f, source);
				}
			}
			
			if (type == typeof(EnemyRedSlimeExplosion))
			{
				Vector2 source = GetCollisionCenter(collider);
				Vector2 ofs = GetCollisionCenter(BodySprite) - source;
				if (ofs.LengthSquared() <= 0.0f)
					ofs = Vector2.UnitY;
					
				float damage = 40.0f / ofs.Length();
				Vector2 push = ofs.Normalize() * new Vector2(30.0f * damage, 1.0f) + Vector2.UnitY * 8.0f * damage;
				Velocity += push;
				InvincibleTime = 0.15f;
				TakeDamage(damage, source);
			}
			
			if (type == typeof(Coin))
			{
				Coins += 1;
				(owner as Coin).Die(null, 0.0f);
				Support.SoundSystem.Instance.Play("coin_collect.wav");
			}
			
			if (type == typeof(Heart))
			{
				if ((owner as Heart).FrameCount > 20)
				{
					Health = FMath.Min(Health + 1.0f, 5.0f);
					(owner as Heart).Die(null, 0.0f);
					Support.SoundSystem.Instance.Play("heart_collect.wav");
				}
			}
		}
		
		public override void TakeDamage(float damage, Vector2? source)
		{
			SpawnDamageParticles(GetCollisionCenter(BodySprite), (Vector2)source, damage * 3.0f, Support.Color(178, 35, 50));
			
			Game.Instance.UI.Shake += damage;

			Support.SoundSystem.Instance.Play("player_take_damage.wav");
			
			base.TakeDamage(damage, source);
		}
		
		public override void Die(Vector2? source, float damage)
		{
			Vector2 center = GetCollisionCenter(BodySprite);
			SpawnDamageParticles(center, (Vector2)source, damage * 5.0f, Support.Color(174, 35, 50));
			SpawnDamageParticles(center, (Vector2)source, damage * 8.0f, Support.Color(96, 35, 50));
			SpawnDamageParticles(center, (Vector2)source, damage * 12.0f, Support.Color(255, 0, 8));
			
			PlayerDeadSword sword = new PlayerDeadSword();
			sword.Position = center + Vector2.UnitY * 12.0f;
			sword.Velocity = Vector2.UnitY * 12.0f;
			Game.Instance.World.AddChild(sword);
			
			var sequence = new Sequence();
			sequence.Add(new DelayTime() { Duration = 3.0f });
			sequence.Add(new CallFunc(Game.Instance.PlayerDied));
			Game.Instance.World.RunAction(sequence);

			Support.MusicSystem.Instance.Stop("game_game_music.mp3");
			Support.SoundSystem.Instance.Play("game_game_over.wav");
			
			base.Die(source, damage);
		}
		
		public void EmitJumpLandParticles(int count, float power)
		{
			Vector2 feet = GetCollisionCenter(BodySprite) + Vector2.UnitY * -64.0f;
			Vector4 color = feet.X < 168.0f ? Support.Color(129, 133, 110) : Support.Color(127, 89, 42);
			Game.Instance.ParticleEffects.AddParticlesBurst(count, feet, Vector2.UnitY, color, power, 0.75f);
		}
    };
}