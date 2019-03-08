/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.Framework;
using Tutorial.Utility;


namespace Sample
{
	public class GameManager : GameActor
	{
		Int32 cnt=0;
		
		SimpleSprite spriteTitle;
		SimpleSprite spritePressStart;
		SimpleSprite spriteGameOver;
		
		public GameManager(GameFrameworkSample gs, string name) : base (gs, name)
		{
			spriteTitle=new SimpleSprite(gs.Graphics, gs.textureTitle);
			spriteTitle.Position.X = gs.rectScreen.Width/2-gs.textureTitle.Width/2;
			spriteTitle.Position.Y = gs.rectScreen.Height/2-gs.textureTitle.Height;
			
			spritePressStart=new SimpleSprite(gs.Graphics, gs.texturePressStart);
			spritePressStart.Position.X = gs.rectScreen.Width/2-gs.texturePressStart.Width/2;
			spritePressStart.Position.Y = gs.rectScreen.Height/2+gs.texturePressStart.Height;

			spriteGameOver=new SimpleSprite(gs.Graphics, gs.textureGameOver);
			spriteGameOver.Position.X = gs.rectScreen.Width/2-gs.textureGameOver.Width/2;
			spriteGameOver.Position.Y = gs.rectScreen.Height/2-gs.textureGameOver.Height;
		}

		public override void Update()
		{
			gs.debugStringScore.Clear();
			gs.debugStringShip.Clear();			
			string str = String.Format("Score: {0:d8}", gs.Score);
			gs.debugStringScore.SetPosition(new Vector3( gs.rectScreen.Width/2-(str.Length*10/2), 2, 0));
			gs.debugStringScore.WriteLine(str);

			str = String.Format("Ships:{0}", gs.NumShips);
			gs.debugStringShip.SetPosition(new Vector3( gs.rectScreen.Width-(str.Length*10), 2, 0));
			gs.debugStringShip.WriteLine(str);
			
			switch(gs.Step)
			{
			case GameFrameworkSample.StepType.Opening:
				gs.debugString.WriteLine("Opening");
				
				if((gs.PadData.ButtonsDown & GamePadButtons.Start) != 0  )
				{
					Player player =(Player)gs.Root.Search("Player");
					player.Status = Actor.ActorStatus.Action;
					player.Initilize();
					
					gs.GameCounter=0;
					gs.NumShips=3;
					gs.Score=0;
					gs.soundPlayerButton.Play();
					gs.bgmPlayer.Play();
					gs.Step= GameFrameworkSample.StepType.GamePlay;
				}
				
				break;
			case GameFrameworkSample.StepType.GamePlay:
				gs.debugString.WriteLine("GamePlay");
				
				//@e Game over if player stock falls to zero.
				//@j 残機が0になったらゲームオーバーへ。
				if(gs.NumShips <= 0)
				{
					gs.Step= GameFrameworkSample.StepType.GameOver;
					gs.bgmPlayer.Stop();
					cnt=0;
				}
				
				++gs.GameCounter;
				
				break;
			case GameFrameworkSample.StepType.GameOver:
				gs.debugString.WriteLine("GameOver");
				
				if((gs.PadData.ButtonsDown & GamePadButtons.Start) != 0 || ++cnt > 300)
				{
					gs.Step= GameFrameworkSample.StepType.Opening;
					System.GC.Collect(2);
				}
				
				break;
			default:
				throw new Exception("default in StepType.");
				
			}
			
			base.Update();
		}
		
		public override void Render ()
		{
			gs.debugStringScore.Render();
			gs.debugStringShip.Render();
			
			switch(gs.Step)
			{
			case GameFrameworkSample.StepType.Opening:
				spriteTitle.Render();
				spritePressStart.Render();
				break;
			case GameFrameworkSample.StepType.GamePlay:
				
				break;
			case GameFrameworkSample.StepType.GameOver:
				spriteGameOver.Render();
				break;
			default:
				throw new Exception("default in StepType.");
			
			}
			
			base.Render ();
		}
	}
}

