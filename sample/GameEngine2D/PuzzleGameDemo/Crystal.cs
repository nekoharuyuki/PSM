/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PuzzleGameDemo
{
	public class Crystal : Node
	{
		public int Type;
		public int SubType;
		public SpriteTile Sprite;
		public int FallQueue;
		public int ShuffleSteps;

		// fall anim
		// disappear anim
		// swap swap anim
		
		public Crystal()
		{
			Sprite = Support.TiledSpriteFromFile("/Application/assets/crystals.png", 5, 7);
			Sprite.Pivot = new Vector2(Board.CrystalSize);
			SetType(0, 0);
		}

		public void RandomizeType()
		{
			var type = Game.Instance.Random.Next() % 7;
			var subtype = Game.Instance.Random.Next() % 3;
			SetType(type, subtype);
		}

		public void SetType(int type, int subtype)
		{
			Type = type;
			SubType = subtype;
			Sprite.TileIndex2D = new Vector2i(subtype, type);
		}

		public void MoveTo(Vector2 position, float duration)
		{
			var move_action = new MoveTo(position, duration) {
				Set = value => { Sprite.Quad.T = value; },
				Get = () => Sprite.Quad.T
			};

			Game.Instance.GameScene.RunAction(move_action);
		}

		public void FailMoveTo(Vector2 position, float duration)
		{
			var move_action = new MoveTo(position, duration) {
				Set = value => { Sprite.Quad.T = value; },
				Get = () => Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Impulse(t, 1.0f)
			};

			Game.Instance.GameScene.RunAction(move_action);
		}

		public void ScaleTo(float scale, float duration)
		{
			var scale_action = new ScaleTo(new Vector2(scale, scale), duration) {
				Set = value => { Sprite.Quad.S = value; },
				Get = () => Sprite.Quad.S
			};
			
			Game.Instance.GameScene.RunAction(scale_action);
		}

		public void QueueFallAction()
		{
			FallQueue += 1;
		}

		private void AddFallActionInternal()
		{
			const float SingleFallDuration = 0.15f;

			var duration = SingleFallDuration * FallQueue;
			var distance = -Board.CrystalSize * FallQueue;

			var sequence = new Sequence();
			var move_action = new MoveBy(new Vector2(0.0f, distance), duration) {
				Set = value => { Sprite.Quad.T = value; },
				Get = () => Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear(t)
			};

			Game.Instance.Board.LockBoard();
			sequence.Add(move_action);
			sequence.Add(new CallFunc(CheckFallActionQueue));
			sequence.Add(new CallFunc(Game.Instance.Board.UnlockBoard));
			sequence.Add(new CallFunc(() => Support.SoundSystem.Instance.Play("veggie_land.wav")));

			RunAction(sequence);

			FallQueue = 0;
		}

		public void CheckFallActionQueue()
		{
			if (ShuffleSteps > 0)
			{
				var sequence = new Sequence();
				var scale_action = new ScaleTo(new Vector2(Sprite.Quad.S.X, 0.0f), 0.0f) {
					Set = value => { Sprite.Quad.S = value; },
					Get = () => Sprite.Quad.S
				};
				var move_action = new MoveTo(Sprite.Quad.T + new Vector2(0.0f, Board.CrystalSize * ShuffleSteps), 0.0f) {
					Set = value => { Sprite.Quad.T = value; },
					Get = () => Sprite.Quad.T
				};

				Game.Instance.Board.LockBoard();
				int local_shuffle_steps = ShuffleSteps;
				sequence.Add(new CallFunc(() => {
					Sprite.Quad.T += new Vector2(0.0f, Board.CrystalSize * local_shuffle_steps);
					RandomizeType();
					Sprite.Quad.S = new Vector2(Board.CrystalSize);
					CheckFallActionQueue();
				}));

				sequence.Add(new CallFunc(Game.Instance.Board.UnlockBoard));

				Vector2 position = Sprite.Quad.Center + Vector2.UnitX * -8.0f;
				int count = 4 + SubType * 3 + Game.Instance.Random.Next() % 4;
				float scale = 1.0f + SubType * 0.5f;
				Game.Instance.Board.ParticleEffectsManager.AddParticlesBurst(count, position, Vector2.UnitY * 1.5f, GetTypeColor(), 2.0f, scale);

				this.RunAction(sequence);

				ShuffleSteps = 0;

				return;
			}

			if (FallQueue > 0)
			{
				AddFallActionInternal();
				return;
			}
		}

		public Vector4 GetTypeColor()
		{
			switch (Type)
			{
				case 0: return Support.Color(116, 218, 36, 255);
				case 1: return Support.Color(60, 109, 221, 255);
				case 2: return Support.Color(113, 23, 216, 255);
				case 3: return Support.Color(255, 41, 180, 255);
				case 4: return Support.Color(11, 39, 171, 255);
				case 5: return Support.Color(219, 189, 48, 255);
				case 6: return Support.Color(216, 23, 25, 255);
			}

			return Support.Color(255, 0, 0, 255);
		}
	}

}

