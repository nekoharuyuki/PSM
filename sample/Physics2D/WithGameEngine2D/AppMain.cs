/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// Tips and notice
//
// One of bodies(last one) is update by GameEngine2D loop and this should
// be synchronized with the kinematic rigid body of Physics2D.
//
// Other bodies can be update by user clicking.
//
// All sprites of GameEngine2D should be synchronized with Physics2D each frame.
//

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.Core.Imaging;

// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;
using Physics2DSample;

namespace Physics2DWithGameEngine2D
{
	public class AppMain
	{
		static Scene scene = null;
		static SpriteUV[] sprite = null;
		static FontMap titleFontMap = null;
		static Label titleLabel = null;
		
		static PhysicsScene myScene;
		static ShaderProgram program = null;
		static Matrix4 worldMatrix;
		
		public static void Main (string[] args)
		{         

			GraphicsContext context = new GraphicsContext();
            ImageRect rectScreen = context.Screen.Rectangle;
                   
			uint sprites_capacity = 500;
            uint draw_helpers_capacity = 400;
			
            Director.Initialize( sprites_capacity, draw_helpers_capacity, context);
            Director.Instance.GL.Context.SetClearColor( Colors.Grey05 );

            // Definition of GameEngine2D scene
			scene = new Scene();
			sprite = new SpriteUV[100];
			

				
            // Set camera viewport
            scene.Camera.SetViewFromViewport();
			
			// Create Physics2D scene
			myScene = new StackMotionScene(ref scene, ref sprite, rectScreen.Width, rectScreen.Height);
			
			titleFontMap = new FontMap( new Font( FontAlias.System, 40, FontStyle.Bold ) );
			titleLabel = new Label(){FontMap = titleFontMap, Color = Colors.White };
			titleLabel.HeightScale = scene.Camera.GetPixelSize();
			titleLabel.Text = myScene.sceneName;
			titleLabel.Position = new Vector2(0, rectScreen.Height - 40);
			scene.AddChild(titleLabel);
			
			// This is necessary for Physics2D debug rendering
			program = new ShaderProgram("/Application/shaders/test.cgx");
            program.SetUniformBinding(0, "u_WorldMatrix");
            program.SetUniformBinding(1, "u_Color");

			// This object changed by GameEngine2D
			int targetIndex = myScene.numBody-1;
			SpriteUV spriteTarget = ((StackMotionScene)myScene).gd2dSprite[targetIndex];
			
			bool directionFlag = true;
			
			spriteTarget.Schedule ( (dt) => 
			{

				if(directionFlag) 
				{
				  spriteTarget.Position = new Vector2(spriteTarget.Position.X, spriteTarget.Position.Y + 0.2f*((StackMotionScene)myScene).renderingScale);
				  if(spriteTarget.Position.Y >= (scene.Camera.CalcBounds().Center.Y + 10.0f*((StackMotionScene)myScene).renderingScale)) directionFlag = false;
				}
				else
				{
				  spriteTarget.Position = new Vector2(spriteTarget.Position.X, spriteTarget.Position.Y - 0.2f* ((StackMotionScene)myScene).renderingScale);
				  if(spriteTarget.Position.Y <= (scene.Camera.CalcBounds().Center.Y - 10.0f*((StackMotionScene)myScene).renderingScale)) directionFlag = true;
				}
				
				// This target was updated by GameEngine2D and this information should be thrown to information of Physics2D
				// Please check Physics2DSample\TutorialScene\KinematicObjectScene.cs for kinematic object
				{
					PhysicsBody body = ((StackMotionScene)myScene).sceneBodies[targetIndex];
					body.SetBodyKinematic();
					body.sleep = false;
					body.SleepCount = 0;
					Vector2 oldPos = body.position;
					// Please notice that velocity also should be updated properly together with position
	   				body.position  = (spriteTarget.Position - scene.Camera.CalcBounds().Center) * 1.0f/((StackMotionScene)myScene).renderingScale;
					body.velocity = (body.position - oldPos)/(myScene.simDt);
					// if rotation also changes, rotation & angularVelocity also should be updated
				}	
				
			} );
			
  
            Director.Instance.RunWithScene( scene, true );

			// These values are used for checking click position
			Vector2 click_pos = new Vector2(0, 0);
			Vector2 diff_pos = new Vector2(0, 0);
			int click_index = -1;
			
            while(true)
            {
                SystemEvents.CheckEvents();
                         
				int pointX = 0;
				int pointY = 0;
				
                foreach(var touchData in Touch.GetData(0)) {
					
                    if (touchData.Status == TouchStatus.Down ||
                        touchData.Status == TouchStatus.Move )
					{
                        pointX = (int)((touchData.X + 0.5f) * rectScreen.Width);
                        pointY = (int)((touchData.Y + 0.5f) * rectScreen.Height);
						
						// Convert the click position to the position of Physics2D simulation scene
						click_pos = ((StackMotionScene)myScene).GetClickPos((float)((touchData.X + 0.5f) * rectScreen.Width), (float)(rectScreen.Height - ((touchData.Y + 0.5f) * rectScreen.Height)));
						
						// Check whether any rigid bodies exsist under the click position
						if(touchData.Status == TouchStatus.Down)
							click_index = myScene.CheckPicking(ref click_pos, ref diff_pos, false);
                    }
									
					if(touchData.Status == TouchStatus.Up)
					{
						// Cancel the previous touch point
						if(click_index != -1)
						{
							myScene.SceneBodies[click_index].velocity = new Vector2(0, 0);
                        	myScene.SceneBodies[click_index].angularVelocity = 0.0f;
                        	click_index = -1;	
						}
					}	
	
					break;
				
                }
				
				// GameEngine2D Update
				Director.Instance.Update();
				
				// Physics2D Simulation Update
				// The position of rigid body will be update by clicking
				myScene.Simulate(click_index, click_pos, diff_pos);
				
				// Render GameEngine2D Scene
                Director.Instance.Render();
				
				// Render Physics2D Debug Infomation
				{
					
					bool isClearFlag = false;
					
					// If you want to clear render screen
					if(isClearFlag)
					{
						context.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
						context.Clear();
					}
					
					// Change the shader for debug rendering of Physics2D	
					context.SetShaderProgram(program);
					
					float rendering_scale = ((StackMotionScene)myScene).renderingScale;
				
					int window_width = rectScreen.Width;
					int window_height = rectScreen.Height;
					
					Vector2 eye_pos = new Vector2(0, 0);
					
					worldMatrix = new Matrix4(
						rendering_scale / (window_width / 2.0f), 0.0f, 0.0f, 0.0f,
						0.0f, rendering_scale / (window_height / 2.0f), 0.0f, 0.0f,
						0.0f, 0.0f, 1.0f, 0.0f,
						-rendering_scale / (window_width / 2.0f) * eye_pos.X, -rendering_scale / (window_height / 2.0f) * eye_pos.Y, 0.0f, 1.0f
		     		);
		
					// In the case that you want to render Physics2D on the top of rendering (overwrite),
					// set the following value to true
					bool isRenderPhysics2D = false;
					
				     // Draw objects by Physics2D DrawAllBody
					if(isRenderPhysics2D)
					    myScene.DrawAllBody(ref context, ref program, worldMatrix, click_index);
		
					// Draw debug information
		      		myScene.DrawAdditionalInfo(ref context, ref program, worldMatrix);
				}
				
                Director.Instance.GL.Context.SwapBuffers();

                Director.Instance.PostSwap();
            }
		}

	}
}
