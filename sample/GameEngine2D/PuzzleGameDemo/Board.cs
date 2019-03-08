/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace PuzzleGameDemo
{
	public class Board : Node
	{
		public static float CrystalSize = 64.0f;
		public static int WaterLimit = 10000;

		public Vector2 CrystalsOffset;
		public Vector2i BoardSize;
		public SpriteList SpriteList;
		public Crystal SelectedCrystal;
		public List<Crystal> Crystals;
		public SpriteTile Background;
		public SpriteTile CrystalHighlight;
		public bool WasTouch;
		public bool IsTouch;
		public Vector2 TouchStart;
		public int BoardLockCount;
		public int BoardLockTimerCountdown;
		public int ComboCount;
		public ScissorNode ScissorNode;
		public int ScoreValue;
		public Label ScoreLabel;
		public int WaterValue;
		public SpriteTile WaterTileBackground;
		public SpriteTile WaterTile;
		public Vector2 WaterTileBaseScale;
		public Support.ParticleEffectsManager ParticleEffectsManager;
		public bool WaterMode;
		public Label WaterTouchLabel;
		public SpriteTile WaterCan;
		public SpriteTile WaterCanPanel;
		public int WaterNoTouchCounter;
		public Crystal HintCrystal;
		public Sequence HintSequence;
		public int HintDelayCounter;
		public Vector2 HintRestorePosition;
		public CrystalBlinker CrystalBlinker;
		public King King;

		public Board(int size_x, int size_y)
		{
			CrystalsOffset = new Vector2(428.0f, 16.0f);
			BoardSize = new Vector2i(size_x, size_y);
			Background = Support.TiledSpriteFromFile("/Application/assets/board.png", 1, 1);
			CrystalHighlight = Support.TiledSpriteFromFile("/Application/assets/crystal_highlight.png", 1, 1);
			SpriteList = Support.TiledSpriteListFromFile("/Application/assets/crystals.png", 5, 7);
			Crystals = new List<Crystal>();

			for (int y = 0; y < size_y; ++y)
			{
				for (int x = 0; x < size_x; ++x)
				{
					var crystal = new Crystal();
					Crystals.Add(crystal);
					SpriteList.AddChild(crystal.Sprite);
				}
			}

			ScissorNode = new ScissorNode((int)CrystalsOffset.X, (int)CrystalsOffset.Y, 512, 512);
			ScissorNode.AddChild(SpriteList);

			ScoreLabel = new Label() { Text = "0" };
			var font = new Font("/Application/assets/IndieFlower.ttf", 96, FontStyle.Regular);
			var map = new FontMap(font);
			ScoreLabel.FontMap = map;
			ScoreLabel.Position = new Vector2(150.0f, 434.0f);
			ScoreLabel.Color = Support.Color(0, 0, 0, 255);
			ScoreLabel.HeightScale = 1.0f;

			WaterTouchLabel = new Label() { Text = "Touch!" };
			font = new Font("/Application/assets/IndieFlower.ttf", 72, FontStyle.Bold);
			map = new FontMap(font);
			WaterTouchLabel.FontMap = map;
			WaterTouchLabel.Position = new Vector2(540.0f, 260.0f);
			WaterTouchLabel.Color = Support.Color(216, 216, 216, 255);
			WaterTouchLabel.HeightScale = 1.0f;
			WaterTouchLabel.Visible = false;

			WaterTile = Support.UnicolorSprite("Water", 8, 32, 192, 96);
			WaterTile.Position = new Vector2(293.0f, 49.0f);
			WaterTileBaseScale = new Vector2(220.0f / 960.0f, 360.0f / 544.0f) * 32.0f;
			WaterTile.Scale = WaterTileBaseScale;
			WaterTile.Quad.Centering(new Vector2(0.0f, 0.0f));

			WaterTileBackground = Support.TiledSpriteFromFile("/Application/assets/water_meter_bg.png", 1, 1);
			WaterTileBackground.Position = WaterTile.Position;
			WaterTile.Scale = WaterTileBaseScale;

			WaterCan = Support.TiledSpriteFromFile("/Application/assets/watering_can.png", 1, 1);
			WaterCan.Position = new Vector2(770.0f, 300.0f);
			WaterCan.Quad.Centering(new Vector2(0.5f, 0.5f));
			WaterCan.Visible = false;

			WaterCanPanel = Support.UnicolorSprite("WaterCanPanel", 32, 32, 32, 160);
			WaterCanPanel.Position = new Vector2(520.0f, 240.0f);
			WaterCanPanel.Scale = new Vector2(590.0f / 960.0f, 140.0f / 544.0f) * 32.0f;
			WaterCanPanel.Visible = false;

			King = new King();

			AddChild(WaterTileBackground);
			AddChild(WaterTile);
			AddChild(Background);
			AddChild(ScissorNode);
			AddChild(CrystalHighlight);

			AddChild(ScoreLabel);
			AddChild(WaterCanPanel);
			AddChild(WaterTouchLabel);
			AddChild(WaterCan);
			AddChild(King);

			ParticleEffectsManager = new Support.ParticleEffectsManager();
			AddChild(ParticleEffectsManager);

			CrystalBlinker = new CrystalBlinker();
		}

		public void Refresh(int level)
		{
			ScoreValue = 0;
			ScoreLabel.Text= "0";

			BoardLockCount = 0;
			WaterMode = false;
			WaterValue = 0;
			AddWater(WaterLimit / 2);

			King.DoBlinkSequence();

			SelectedCrystal = null;
			CrystalHighlight.Visible = false;

			// generate one row at a time
			// if crystal in row /below/
			for (int y = 0; y < BoardSize.Y; ++y)
			{
				for (int x = 0; x < BoardSize.X; ++x)
				{
					var crystal = Crystals[y * BoardSize.X + x];
					crystal.RandomizeType();

					// avoid some starting 3x blocks
					var left = GetCrystalAtTile(x - 1, y);
					var leftleft = GetCrystalAtTile(x - 2, y);
					var down = GetCrystalAtTile(x, y - 1);
					for (int i = 0; i < 8; ++i)
					{
						if (left != null && leftleft != null)
						{
							if (crystal.Type == left.Type && crystal.Type == leftleft.Type)
								crystal.RandomizeType();
						}

						if (down != null && crystal.Type == down.Type)
							crystal.RandomizeType();
					}

					crystal.Sprite.Quad.T = GetPositionAtTile(x, y);
					crystal.Sprite.Quad.S = new Vector2(crystal.Sprite.TextureInfo.TextureSizef.X / 5.0f, crystal.Sprite.TextureInfo.TextureSizef.Y / 7.0f);
					crystal.Visible = true;
				}
			}

			// perform a lock/unlock so that game over conditions/etc get correctly checked before first move
			LockBoard();
			UnlockBoard();

			King.DoBlinkSequence();
		}

		public Vector2 GetPositionAtTile(int x, int y)
		{
			return
				new Vector2(512.0f / (float)BoardSize.X * x, 512.0f / (float)BoardSize.Y * y) + 
				CrystalsOffset;
		}

		public Vector2i GetTileAtPosition(Vector2 position)
		{
			position -= CrystalsOffset;
			position /= CrystalSize;

			int x = (int)position.X;
			int y = (int)position.Y;

			x = System.Math.Max(0, System.Math.Min(BoardSize.X - 1, x));
			y = System.Math.Max(0, System.Math.Min(BoardSize.Y - 1, y));
			
			return new Vector2i(x, y);
		}

		public Crystal GetCrystalAtPosition(Vector2 position)
		{
			position -= CrystalsOffset;
			position /= CrystalSize;
			
			int x = (int)position.X;
			int y = (int)position.Y;

			return GetCrystalAtTile(x, y);
		}

		public Crystal GetCrystalAtTile(int x, int y)
		{
			if (x < 0 || x >= BoardSize.X)
			{
				return null;
			}

			if (y < 0 || y >= BoardSize.Y)
			{
				return null;
			}

			var crystal = Crystals[y * BoardSize.X + x];
			return crystal;
		}

		public void UpdateHint()
		{
			HintDelayCounter++;

			if (HintCrystal == null)
			{
				return;
			}

			if (HintDelayCounter <= 0)
			{
				return;
			}

			const int show_hint_time = 60 * 7;

			if (HintDelayCounter % show_hint_time != 0)
			{
				return;
			}

			if (HintSequence != null && HintSequence.IsRunning)
			{
				(HintSequence.Target as Crystal).Sprite.Quad.T = HintRestorePosition;
				HintSequence.Stop();
				HintSequence = null;
			}

			var movement = Vector2.UnitY * 4.0f;
			var src = HintCrystal.Sprite.Quad.T;
			var dst = HintCrystal.Sprite.Quad.T + movement;

			HintRestorePosition = src;

			var move_action_0 = new MoveTo(dst, 0.2f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.PowEaseIn(t, 2.0f),
			};

			var move_action_1 = new MoveTo(src, 0.3f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear(t),
			};

			var move_action_2 = new MoveTo(dst, 0.2f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.PowEaseIn(t, 2.0f),
			};

			var move_action_3 = new MoveTo(src, 0.3f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear(t),
			};

			var move_action_4 = new MoveTo(dst, 0.2f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.PowEaseIn(t, 2.0f),
			};

			var move_action_5 = new MoveTo(src, 0.3f) {
				Set = value => { HintCrystal.Sprite.Quad.T = value; },
				Get = () => HintCrystal.Sprite.Quad.T,
				Tween = (t) => Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Linear(t),
			};

			var restore_action_6 = new CallFunc(() => HintCrystal.Sprite.Quad.T = src);

			HintSequence = new Sequence();
			HintSequence.Add(move_action_0);
			HintSequence.Add(move_action_1);
			HintSequence.Add(move_action_2);
			HintSequence.Add(move_action_3);
			HintSequence.Add(move_action_4);
			HintSequence.Add(move_action_5);
			HintSequence.Add(restore_action_6);
			HintCrystal.RunAction(HintSequence);
		}

		public void LockBoard()
		{
			BoardLockCount += 1;
			HintDelayCounter = -100000000;
		}

		public Sce.PlayStation.HighLevel.GameEngine2D.ActionBase MakeGameOverDestroyCrystalAction(int x, int y)
		{
			var action = new CallFunc(() => {
				var crystal = GetCrystalAtTile(x, y);
				for (int i = 0; i < 7; ++i)
				{
					crystal.QueueFallAction();
					crystal.ShuffleSteps += 1;
				}

				crystal.Visible = false;
			});
			return action;
		}

		public Sce.PlayStation.HighLevel.GameEngine2D.ActionBase MakeExplodeCrystalAction(int x, int y)
		{
			var action = new CallFunc(() => {
				var crystal = GetCrystalAtTile(x, y);
				Vector2 position = crystal.Sprite.Quad.Center + Vector2.UnitX * -8.0f;
				int count = 4 + crystal.SubType * 3 + Game.Instance.Random.Next() % 4;
				float scale = 1.0f + crystal.SubType * 0.5f;
				Game.Instance.Board.ParticleEffectsManager.AddParticlesBurst(count, position, Vector2.UnitY * 1.5f, crystal.GetTypeColor(), 2.0f, scale);
				// NOTE: these don't work on spritelist items
				//crystal.Visible = false;
				//crystal.Sprite.Visible = false;
				crystal.Sprite.Quad.S = Vector2.Zero;
			});
			return action;
		}

		public void GameOver()
		{
			var destroy_crystals = new Sequence();

			var indices = new List<Vector2i>();
			for (int y = 0; y < BoardSize.Y; ++y)
			{
				for (int x = 0; x < BoardSize.X; ++x)
				{
					indices.Add(new Vector2i(x, y));
				}
			}

			for (int i = 0; i < indices.Count; ++i)
			{
				int ia = Game.Instance.Random.Next() % indices.Count;
				int ib = Game.Instance.Random.Next() % indices.Count;
				Vector2i tmp = indices[ia];
				indices[ia] = indices[ib];
				indices[ib] = tmp;
			}

			for (int i = 0; i < indices.Count; ++i)
			{
				var tile = indices[i];
				var destroy = MakeExplodeCrystalAction(tile.X, tile.Y);
				destroy_crystals.Add(destroy);

				if (i % 4 == 0){
					destroy_crystals.Add(new DelayTime() { Duration = 0.0001f });
				}
			}
			//this.RunAction(destroy_crystals);

			var sequence_end = new Sequence();
			Game.Instance.Board.LockBoard();
			//sequence_end.Add(new CallFunc(LockBoard));
			sequence_end.Add(new CallFunc(King.DoGameOverSequence));
			sequence_end.Add(new CallFunc(() => Support.SoundSystem.Instance.Play("gameover.wav")));
			sequence_end.Add(destroy_crystals);
			sequence_end.Add(new DelayTime() { Duration = 1.0f });
			sequence_end.Add(new CallFunc(UnlockBoard));
			sequence_end.Add(new CallFunc(() => Game.Instance.StartTitle()));
			this.RunAction(sequence_end);
		}

		public void UnlockBoard()
		{
			BoardLockCount -= 1;
			if (BoardLockCount == 0)
			{
				BoardLockTimerCountdown = 4;
			}
		}

		public void StopHintCrystal()
		{
			if (HintSequence != null)
			{
				HintSequence.Stop();
				HintSequence = null;
				HintCrystal.Sprite.Quad.T = HintRestorePosition;
			}

			HintRestorePosition = Vector2.Zero;
			HintDelayCounter = -100000000;
		}

		public void SwapCrystals(Crystal a, Crystal b)
		{
			const float time = 0.5f;

			a.MoveTo(b.Sprite.Quad.T, time);
			b.MoveTo(a.Sprite.Quad.T, time);

			var sequence_swap = new Sequence();
			Game.Instance.Board.LockBoard();
			sequence_swap.Add(new DelayTime() { Duration = time });
			sequence_swap.Add(new CallFunc(() => SwapCrystalIndices(a, b)));
			sequence_swap.Add(new CallFunc(UnlockBoard));
			this.RunAction(sequence_swap);
		}

		public void FailSwapCrystals(Crystal a, Crystal b)
		{
			const float time = 0.3f;

			var sequence_swap = new Sequence();
			Game.Instance.Board.LockBoard();
			sequence_swap.Add(new DelayTime() { Duration = time });
			sequence_swap.Add(new CallFunc(UnlockBoard));
			this.RunAction(sequence_swap);
		}

		public void SwapCrystalIndices(Crystal a, Crystal b)
		{
			int ai = Crystals.FindIndex(x => x == a);
			int bi = Crystals.FindIndex(x => x == b);

			Crystals[ai] = b;
			Crystals[bi] = a;
		}

		public int CountChainH(int x, int y)
		{
			Crystal current = GetCrystalAtTile(x, y);

			int chain = 0;
			for (int h = x; h < BoardSize.X; ++h)
			{
				Crystal next = GetCrystalAtTile(h, y);	
				if (next.Type != current.Type)
				{
					break;
				}
				chain++;
			}

			return chain;
		}

		public int CountChainV(int x, int y)
		{
			Crystal current = GetCrystalAtTile(x, y);

			int chain = 0;
			for (int v = y; v < BoardSize.Y; ++v)
			{
				Crystal next = GetCrystalAtTile(x, v);
				if (next.Type != current.Type)
				{
					break;
				}
				chain++;
			}

			return chain;
		}

		public struct ChainEvent
		{
			public Vector2i tile;
			public int count;
			public int power;
		};

		public Vector2i FindPotentialChain()
		{
			// search horizontal pairs
			for (int y = 0; y < BoardSize.Y; ++y)
			{
				for (int x = 0; x < BoardSize.X; ++x)
				{
					// check for pair
					Crystal current = GetCrystalAtTile(x, y);
					Crystal previous = GetCrystalAtTile(x - 1, y);

					if (previous == null)
					{
						continue;
					}

					if (current.Type != previous.Type)
					{
						continue;
					}

					// found pair, search surrounding crystals
					Crystal match_left_up = GetCrystalAtTile(x - 2, y + 1);
					Crystal match_left_left = GetCrystalAtTile(x - 3, y);
					Crystal match_left_down = GetCrystalAtTile(x - 2, y - 1);

					if (match_left_up != null && match_left_up.Type == previous.Type)
					{
						return new Vector2i(x - 2, y + 1);
					}

					if (match_left_left != null && match_left_left.Type == previous.Type)
					{
						return new Vector2i(x - 3, y);
					}

					if (match_left_down != null && match_left_down.Type == previous.Type)
					{
						return new Vector2i(x - 2, y - 1);
					}

					// found pair, search surrounding crystals
					Crystal match_right_up = GetCrystalAtTile(x + 1, y + 1);
					Crystal match_right_left = GetCrystalAtTile(x + 2, y);
					Crystal match_right_down = GetCrystalAtTile(x + 1, y - 1);

					if (match_right_up != null && match_right_up.Type == current.Type)
					{
						return new Vector2i(x + 1, y + 1);
					}

					if (match_right_left != null && match_right_left.Type == current.Type)
					{
						return new Vector2i(x + 2, y);
					}

					if (match_right_down != null && match_right_down.Type == current.Type)
					{
						return new Vector2i(x + 1, y - 1);
					}
				}
			}

			// search vertical pairs
			for (int x = 0; x < BoardSize.X; ++x)
			{
				for (int y = 0; y < BoardSize.Y; ++y)
				{
					// check for pair
					Crystal current = GetCrystalAtTile(x, y);
					Crystal previous = GetCrystalAtTile(x, y - 1);

					if (previous == null)
					{
						continue;
					}

					if (current.Type != previous.Type)
					{
						continue;
					}

					// found pair, search surrounding crystals
					Crystal match_down_left = GetCrystalAtTile(x - 1, y - 2);
					Crystal match_down_down = GetCrystalAtTile(x, y - 3);
					Crystal match_down_right = GetCrystalAtTile(x + 1, y - 2);

					if (match_down_left != null && match_down_left.Type == previous.Type)
					{
						return new Vector2i(x - 1, y - 2);
					}

					if (match_down_down != null && match_down_down.Type == previous.Type)
					{
						return new Vector2i(x, y - 3);
					}

					if (match_down_right != null && match_down_right.Type == previous.Type)
					{
						return new Vector2i(x + 1, y - 2);
					}

					// found pair, search surrounding crystals
					Crystal match_up_left = GetCrystalAtTile(x - 1, y + 1);
					Crystal match_up_up = GetCrystalAtTile(x, y + 2);
					Crystal match_up_right = GetCrystalAtTile(x + 1, y + 1);

					if (match_up_left != null && match_up_left.Type == current.Type)
					{
						return new Vector2i(x - 1, y + 1);
					}

					if (match_up_up != null && match_up_up.Type == current.Type)
					{
						return new Vector2i(x, y + 2);
					}

					if (match_up_right != null && match_up_right.Type == current.Type)
					{
						return new Vector2i(x + 1, y + 1);
					}
				}
			}

			return new Vector2i(-1, -1);
		}

		public bool CheckBoardForChains(bool trigger_clear)
		{
			var chain_tiles = new List<Vector2i>();
			var chain_events = new List<ChainEvent>();

			for (int x = 0; x < BoardSize.X; ++x)
			{
				for (int y = 0; y < BoardSize.Y; )
				{
					int chain = CountChainV(x, y);	

					if (chain >= 3)
					{
						int power = 0;
						for (int i = 0; i < chain; ++i)
						{
							var c = GetCrystalAtTile(x, y + i);
							power += c.SubType + 1;	
						}

						chain_events.Add(new ChainEvent() { tile = new Vector2i(x, y), count = chain, power = power });
						for (int i = 0; i < chain; ++i)
						{
							chain_tiles.Add(new Vector2i(x, y + i));
						}
					}

					y += chain;
				}
			}

			for (int y = 0; y < BoardSize.Y; ++y)
			{
				for (int x = 0; x < BoardSize.X; )
				{
					int chain = CountChainH(x, y);	
					
					if (chain >= 3)
					{
						int power = 0;
						for (int i = 0; i < chain; ++i)
						{
							var c = GetCrystalAtTile(x + i, y);
							power += c.SubType + 1;	
						}

						chain_events.Add(new ChainEvent() { tile = new Vector2i(x, y), count = chain, power = power });
						for (int i = 0; i < chain; ++i)
						{
							chain_tiles.Add(new Vector2i(x + i, y));
						}
					}
					x += chain;
				}
			}

			if (!trigger_clear)
			{
				return chain_events.Count > 0;
			}

			if (chain_events.Count == 0)
			{
				ComboCount = 0;
			}
			else
			{
				ComboCount += 1;

				// match1.wav to match5.wav
				int combo_sound_index = System.Math.Max(1, System.Math.Min(ComboCount, 5));
				Support.SoundSystem.Instance.Play(String.Format("match{0}.wav", combo_sound_index));
			}

			for (int i = 0; i < chain_events.Count; ++i)
			{
				Vector2i tile = chain_events[i].tile;
				Vector2 position = GetCrystalAtTile(tile.X, tile.Y).Sprite.Quad.T;

				int count = chain_events[i].count;
				int power = chain_events[i].power;

				int score = count * count * power * power * ComboCount;
				int round = score - score % 10;

				AddScore(position, round);
			}

			chain_tiles = chain_tiles.Distinct().ToList();

			var columns = new List<List<Vector2i>>();
			for (int x = 0; x < BoardSize.X; ++x)
			{
				columns.Add(new List<Vector2i>());
				for (int i = 0; i < chain_tiles.Count; ++i)
				{
					if (chain_tiles[i].X == x)
					{
						columns[x].Add(chain_tiles[i]);
					}
				}
			}

			for (int i = 0; i < columns.Count; ++i)
			{
				columns[i] = columns[i].OrderBy(item => item.Y).ToList();
			}

			// bubble-swap each chained crystal in a given column upwards to the top,
			// adding a fall-down action to each bubble that gets shuffled
			for (int i = 0; i < columns.Count; ++i)
			{
				List<Vector2i> column = columns[i];
				for (int j = 0; j < column.Count(); ++j)
				{
					var crystal = GetCrystalAtTile(column[j].X, column[j].Y);
					int x = column[j].X;
					for (int y = column[j].Y; y < BoardSize.Y - 1; ++y)
					{
						var cleared_block = GetCrystalAtTile(x, y);
						var existing_block = GetCrystalAtTile(x, y + 1);

						SwapCrystalIndices(cleared_block, existing_block);

						// only the old block needs to fall in the shuffles here
						existing_block.QueueFallAction();

						// push only the new block up one to shuffle it above the top of the board
						cleared_block.ShuffleSteps += 1;
					}

					// column indices need to swap to reflect change (except last tile)
					if (j < column.Count - 1)
					{
						Vector2i tmp = column[j];
						column[j] = column[j + 1];
						column[j + 1] = tmp;
					}

					// reposition up one more block to go off the edge and fall back into the board
					crystal.QueueFallAction();

					// needs to be moved up one more
					crystal.ShuffleSteps += 1;
				}
			}
			
			for (int y = 0; y < BoardSize.Y; ++y)
			{
				for (int x = 0; x < BoardSize.X; ++x)
				{
					GetCrystalAtTile(x, y).CheckFallActionQueue();
				}
			}
			return chain_events.Count > 0;
		}

		public void AddScore(Vector2 position, int amount)
		{
			ScoreValue += amount;
			ScoreLabel.Text = ScoreValue.ToString();

			AddWater(amount);
		}

		public void EnterWaterMode()
		{
			LockBoard();
			WaterMode = true;
			WaterNoTouchCounter = 60 * 2;
			King.DoWaterSequence();
			Support.SoundSystem.Instance.Play("watermode.wav");
		}

		public void ExitWaterMode()
		{
			WaterNoTouchCounter = 0;
			WaterValue = 0;
			WaterMode = false;
			WaterTouchLabel.Visible = false;
			WaterCanPanel.Visible = false;
			Support.SoundSystem.Instance.Stop("watermode.wav");
			UnlockBoard();
		}

		public void AddWater(int amount)
		{
			WaterValue += amount;
			WaterValue = System.Math.Min(WaterValue, WaterLimit);
			float ratio = (float)WaterValue / (float)WaterLimit;
			WaterTile.Scale = WaterTileBaseScale * new Vector2(1.0f, ratio);
		}

		public void ClearChain(List<Vector2i> chained, int x, int y, int dx, int dy)
		{
			int step_x = dx > 0 ? 1 : 0;
			int step_y = dy > 0 ? 1 : 0;

			dx = System.Math.Min(dx, 1);
			dy = System.Math.Min(dy, 1);

			for (; y < y + dy; ++y)
			{
				for (; x < x + dx; ++x)
				{
					chained.Add(new Vector2i(x, y));
				}
			}
		}

		public bool IsValidAdjacent(Crystal a, Crystal b)
		{
			if (a == null || b == null)
			{
				return false;
			}

			Vector2i ta = GetTileAtPosition(a.Sprite.Quad.T);
			Vector2i tb = GetTileAtPosition(b.Sprite.Quad.T);
			Vector2i t = tb - ta;
			int difference = System.Math.Abs(t.X) + System.Math.Abs(t.Y);

			return difference == 1;
		}
		
		delegate bool CheckMakeChain(int x, int y, int type);

		public bool WillSwapMakeChain(Crystal a, Crystal b)
		{
			if (a == null || b == null)
			{
				return false;
			}

			SwapCrystalIndices(a, b);

			Vector2i tile_a = GetTileAtPosition(a.Sprite.Quad.T);	
			Vector2i tile_b = GetTileAtPosition(b.Sprite.Quad.T);	
			
			CheckMakeChain xf = (int x, int y, int type) => {
				var x0 = GetCrystalAtTile(x - 2, y);
				var x1 = GetCrystalAtTile(x - 1, y);
				var x3 = GetCrystalAtTile(x + 1, y);
				var x4 = GetCrystalAtTile(x + 2, y);
				
				if (x0 != null && x0.Type == type && x1 != null && x1.Type == type)
				{
					return true;
				}

				if (x1 != null && x1.Type == type && x3 != null && x3.Type == type)
				{
					return true;
				}

				if (x3 != null && x3.Type == type && x4 != null && x4.Type == type)
				{
					return true;
				}

				return false;
			};
			
			CheckMakeChain yf = (int x, int y, int type) => {
				var y0 = GetCrystalAtTile(x, y - 2);
				var y1 = GetCrystalAtTile(x, y - 1);
				var y3 = GetCrystalAtTile(x, y + 1);
				var y4 = GetCrystalAtTile(x, y + 2);
				
				if (y0 != null && y0.Type == type && y1 != null && y1.Type == type)
				{
					return true;
				}

				if (y1 != null && y1.Type == type && y3 != null && y3.Type == type)
				{
					return true;
				}

				if (y3 != null && y3.Type == type && y4 != null && y4.Type == type)
				{
					return true;
				}

				return false;
			};
				
			bool a_x_chain = xf(tile_b.X, tile_b.Y, a.Type);
			bool a_y_chain = yf(tile_b.X, tile_b.Y, a.Type);
			bool b_x_chain = xf(tile_a.X, tile_a.Y, b.Type);
			bool b_y_chain = yf(tile_a.X, tile_a.Y, b.Type);

			SwapCrystalIndices(b, a);

			return a_x_chain || a_y_chain || b_x_chain || b_y_chain;
		}

		public void EmitWaterParticles()
		{
			Random r = Game.Instance.Random;

			float water_ratio = (float)WaterValue / (float)WaterLimit;
			float chance = water_ratio * 0.10f;

			if (WaterMode)
			{
				chance += 0.25f;
			}

			if (r.NextFloat() > chance)
			{
				return;
			}

			Vector2 base_position = WaterTile.LocalToWorld(WaterTile.Quad.T);
			Vector2 emit_position = base_position +
				new Vector2(
					r.NextFloat() * 72.0f,
					r.NextFloat() * 240.0f * water_ratio
				);


			ParticleEffectsManager.AddParticleWater(
				emit_position, Vector2.UnitY * 0.0f, Support.Color(192, 192, 220, 96), 1.0f
			);
		}

		public void Tick(float dt)
		{
			//Director.Instance.DebugFlags |= DebugFlags.Navigate; // press left alt + mouse to navigate in 2d space
			//Director.Instance.DebugFlags |= DebugFlags.DrawGrid;
			//Director.Instance.DebugFlags |= DebugFlags.DrawContentWorldBounds;
			//Director.Instance.DebugFlags |= DebugFlags.DrawContentLocalBounds;
			//Director.Instance.DebugFlags |= DebugFlags.DrawTransform;
			//Director.Instance.DebugFlags |= DebugFlags.Log;

			BoardLockTimerCountdown = System.Math.Max(0, BoardLockTimerCountdown - 1);
			if (BoardLockTimerCountdown == 1)
			{
				BoardLockTimerCountdown = 0;

				WaterCan.Visible = false;

				Vector2i potential = FindPotentialChain();
				var hint_crystal = GetCrystalAtTile(potential.X, potential.Y);

				// debug
				//if (Game.Instance.Random.Next() % 4 == 0) { hint_crystal = null; }

				if (hint_crystal != null)
				{
					HintCrystal = hint_crystal;
					HintDelayCounter = 0;
				}
				else
				{
					// Game over!
					GameOver();
				}

				bool has_chains = CheckBoardForChains(true);

				// only enter water mode if there are no more chains to do first
				if (!has_chains)
				{
					if (WaterValue >= WaterLimit)
					{
						EnterWaterMode();
					}
				}
			}

			UpdateHint();

			EmitWaterParticles();

			if (WaterMode)
			{
				Input2.TouchData touch = Input2.Touch00;

				WasTouch = false;
				IsTouch = false;

				AddWater(-15);

				if (touch.Down)
				{
					WaterNoTouchCounter = 0;
					WaterCan.Visible = false;
					WaterTouchLabel.Visible = false;
					WaterCanPanel.Visible = false;

					var normalized = touch.Pos;
					var world = Game.Instance.GameScene.Camera.NormalizedToWorld(normalized);
					var crystal = GetCrystalAtPosition(world);

					if (crystal != null)
					{
						AddWater(-20);

						Support.SoundSystem.Instance.PlayNoClobber("sprinklewater.wav");

						ParticleEffectsManager.AddParticlesBurst(
							7,
							world,
							Vector2.UnitY * 2.5f,
							Support.Color(48, 64, 192, 128),
							3.0f,
							2.0f
						);

						if (Game.Instance.Random.Next() % 100 < 25)
						{
							if (crystal.SubType < 2)
							{
								crystal.SetType(crystal.Type, crystal.SubType + 1);
								Support.SoundSystem.Instance.PlayNoClobber("veggie_upgrade.wav");
							}
						}

						if (Game.Instance.Random.Next() % 100 < 25)
						{
							Vector2i tile = new Vector2i(0, 0);
							switch (Game.Instance.Random.Next() % 8)
							{
								case 0: tile = new Vector2i(-1, -1); break;
								case 1: tile = new Vector2i(+0, -1); break;
								case 2: tile = new Vector2i(+1, -1); break;
								case 3: tile = new Vector2i(-1, +0); break;
								case 4: tile = new Vector2i(+1, +0); break;
								case 5: tile = new Vector2i(-1, +1); break;
								case 6: tile = new Vector2i(+0, +1); break;
								case 7: tile = new Vector2i(+1, +1); break;
							}

							var from = GetTileAtPosition(crystal.Sprite.Quad.T);
							var next = from + tile;
							var bonus = GetCrystalAtTile(next.X, next.Y);
							if (bonus != null)
							{
								if (bonus.SubType < 2)
								{
									bonus.SetType(bonus.Type, bonus.SubType + 1);
									Support.SoundSystem.Instance.PlayNoClobber("veggie_upgrade.wav");
								}
							}
						}
					}
				}
				else
				{
					WaterNoTouchCounter++;

					if (WaterNoTouchCounter > 60 * 2)
					{
						WaterCanPanel.Visible = true;
						WaterTouchLabel.Visible = true;
						WaterCan.Visible = true;
						WaterCan.Rotation = Vector2.UnitX.Rotate((WaterNoTouchCounter / 30) % 2 * -0.5f);
					}
				}

				if (WaterValue <= 0)
				{
					ExitWaterMode();
				}
			}

			UpdateInput(dt);
		}

		public bool IsBoardLocked()
		{
			return BoardLockCount > 0 || BoardLockTimerCountdown > 0;
		}

		public void UpdateInput(float dt)
		{
			Input2.TouchData touch = Input2.Touch00;

			WasTouch = IsTouch;
			IsTouch = touch.Down;

			if (IsBoardLocked() == false)
			{	
				var normalized = touch.Pos;
				var world = Game.Instance.GameScene.Camera.NormalizedToWorld(normalized);
				var crystal = GetCrystalAtPosition(world);
				var moved = TouchStart - world;
				var moved_distance = moved.SafeLength();
				
				Crystal a = null;
				Crystal b = null;

				if (crystal != null)
				{
					// used for hint
					//CrystalHint.Quad.T = crystal.Sprite.Quad.T;
				}

				// first touch frame
				if (IsTouch && !WasTouch)
				{
					if (SelectedCrystal == null)
					{
						if (crystal != null)
						{
							TouchStart = world;
							SelectedCrystal = crystal;
							CrystalHighlight.Quad.T = SelectedCrystal.Sprite.Quad.T;
							CrystalHighlight.Visible = true;
							Support.SoundSystem.Instance.Play("selectedcrystal.wav");
						}
					}
					else
					{
						if (SelectedCrystal == crystal || crystal == null)
						{
							SelectedCrystal = null;
							CrystalHighlight.Visible = false;
							Support.SoundSystem.Instance.Play("cannotmatch.wav");
						}
						else
						{
							if (IsValidAdjacent(SelectedCrystal, crystal))
							{
								a = SelectedCrystal;
								b = crystal;
							}
							else
							{
								Support.SoundSystem.Instance.Play("selectedcrystal.wav");
								TouchStart = world;
								SelectedCrystal = crystal;
								CrystalHighlight.Quad.T = SelectedCrystal.Sprite.Quad.T;
								CrystalHighlight.Visible = true;
							}
						}
					}
				}

				if (IsTouch && WasTouch)
				{
					if (SelectedCrystal != null)
					{
						if (moved_distance > CrystalSize * 0.25f)
						{
							int step_x = 0;
							int step_y = 0;
							
							if (moved.Abs().X > moved.Abs().Y)
								step_x = -System.Math.Sign(moved.X);
							else
								step_y = -System.Math.Sign(moved.Y);
								
							Vector2i tile = GetTileAtPosition(SelectedCrystal.Sprite.Quad.T);
							tile += new Vector2i(step_x, step_y);
							crystal = GetCrystalAtTile(tile.X, tile.Y);
							
							a = SelectedCrystal;
							b = crystal;
						}
					}
				}

				if (a != null && b != null && a != b)
				{
					// check adjacency rules
					if (IsValidAdjacent(a, b))
					{
						if (WillSwapMakeChain(a, b))
						{
							Support.SoundSystem.Instance.Play("swapcrystals.wav");
							StopHintCrystal();
							SwapCrystals(a, b);
							CrystalHighlight.Visible = false;
							SelectedCrystal = null;
						}
						else
						{
							Support.SoundSystem.Instance.Play("cannotmatch.wav");
							FailSwapCrystals(a, b);
							CrystalHighlight.Visible = false;
							SelectedCrystal = null;
						}
					}
				}
			}
		}
	}

}

