/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// 2D Physics Sample Main
//


#define VERTICAL_MOTION

// When you don't want to look all samples, please undefine this
#define FULL_SAMPLES


using System;
using System.Threading;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Environment;

// Include 2D Physics Framework
using Sce.PlayStation.HighLevel.Physics2D;

// Include Graphics2D
using Sample;

using Physics2DSample;

enum SceneName
{
	PrimitiveScene,						
	CompoundScene,	
	JointScene,	
	PyramidStackScene,	
	SmallScaleScene,	
	AirHockeyScene,	
	HammerThrowerScene,	
	PinBallScene,	
	DifferentMassScene,						
	MassUnstableScene,
	LargeSmallScene,
	CreateConvexScene,
	SleepWakeupScene,
	CollisionFilterScene,
	GroupFilterScene,
	CollisionGroupScene,
	BallJointScene,
	SliderJointScene,	
	AnyDirSliderScene,
	FixedOrStaticScene,
	ChainJointScene,
	ChainSpringScene,	
	WheelMakeScene,
	QueryContactScene,
	CreateCompoundScene,
	ScenePropertyScene,
	NonCenterJointScene,
	TriggerObjectScene,
	CompoundBallanceScene,
	ChangeTensorScene,
	CreateRagdollScene,
	JointRotatingScene,
	SimulationAreaScene,
	LineChainScene,
	VehicleMakeScene,
	TextureWithScene,
	KinematicObjectScene,
	AnimationWithScene,
	SpriteBatchWithScene,
	RayCastScene,
	JointSoftbodyScene,
	NumOfSceneName
}

namespace MainSample
{

    static class MainSample
    {

        #region Fields
        static int windowWidth = 0; //854;
        static int windowHeight = 0; //480;

		static GraphicsContext graphics;

		static ShaderProgram program ;
        static VertexBuffer touchVertices;

        static Vector2 touchPoint = new Vector2(0.0f, 0.0f);
        
        static bool mouseMove = false;
        static bool mouseMoveOld = false;
        
        static int clickIndex = -1;
        static Vector2 clickPos;
        static Vector2 diffPos;

        static Matrix4 worldMatrix;
        static float renderingScale = 10.0f;

        static PhysicsScene myScene;
        static int sceneNum = 0;
		
		static Vector2 eyePos = new Vector2(0.0f, 0.0f);
		
        const int totalScene = (int)SceneName.NumOfSceneName;
		
        #endregion

		static bool loop = true;
		
		static bool manipulateMode = false;
		
		static int touchID = -1;
		
		static float scaleResolution = 1.0f;

		static void Main( string[] args )
		{
			// Initialize Graphics Context
			graphics = new GraphicsContext();
			SampleDraw.Init(graphics);
			
            // Get the screen size
            windowWidth = graphics.Screen.Width;
            windowHeight = graphics.Screen.Height;
			
			scaleResolution = Math.Min(windowWidth/854.0f, windowHeight/480.0f);
			
            Init();

            while (loop)
            {
                SampleDraw.Update();

                SystemEvents.CheckEvents();

                // Get touch panel events
                {
                    List<TouchData> touch_data_list = Touch.GetData(0);

                    mouseMove = false;

                    if (touch_data_list.Count > 0)
                    {
                        foreach (TouchData data in touch_data_list)
                        {
							if((touchID != -1) && (touchID != data.ID))
								continue;
														
                            touchPoint = GetClickPos((float)data.X, (float)data.Y);
                            //Console.WriteLine("X, Y = " + touch_point.X + " " + touch_point.Y);
                            mouseMove = true;
							touchID = data.ID;
                            break;
                        }
                    }
                    else
                    {
                        if ((clickIndex != -1) && (clickIndex >=0) && (clickIndex < myScene.NumBody))
                        {
                            myScene.SceneBodies[clickIndex].velocity = new Vector2(0, 0);
                            myScene.SceneBodies[clickIndex].angularVelocity = 0.0f;
                            clickIndex = -1;
                        }
						
						touchID = -1;
                        mouseMove = false;
                    }
                }

                {
                    GamePadData game_pad_data = GamePad.GetData(0);
                    if (game_pad_data.ButtonsDown != 0)
                    {
                        switch (game_pad_data.ButtonsDown & ~GamePadButtons.Enter & ~GamePadButtons.Back)
                        {
                            case GamePadButtons.Left:
                            
                                Console.WriteLine("-> Previous Scene");

                                myScene.ReleaseScene();
                                GC.Collect();

                                sceneNum = (sceneNum + (totalScene - 1)) % totalScene;

                                // simulation setup
                                SwitchScene(sceneNum);

                                break;

                            case GamePadButtons.Right:

                                Console.WriteLine("-> Next Scene");

                                myScene.ReleaseScene();
                                GC.Collect();

                                sceneNum = (++sceneNum) % totalScene;

                                // simulation setup
                            	SwitchScene(sceneNum);

                                break;

                            case GamePadButtons.Triangle:

                                Console.WriteLine("<-> Reset Scene");

                                myScene.ReleaseScene();
                                GC.Collect();

                           		SwitchScene(sceneNum);

                                break;
							
							case GamePadButtons.Circle:
								Console.WriteLine("<-> Change Manipulate Mode");
								manipulateMode = !manipulateMode;
								if(manipulateMode)
									Console.WriteLine("Delete Mode");
								else
									Console.WriteLine("Move Mode");
								break;
							
                            case GamePadButtons.Up:
                                renderingScale += 1.0f;
                                Console.WriteLine("Up Button -----------------------------------");
                                break;

                            case GamePadButtons.Down:
                                renderingScale -= 1.0f;
                                if (renderingScale < 1.0f)
                                    renderingScale = 1.0f;
                                Console.WriteLine("Down Button -----------------------------------");
                                break;

                            default:
                                myScene.KeyboardFunc(game_pad_data.ButtonsDown);
                                break;

                        } 
                    }

                    if (game_pad_data.ButtonsUp != 0)
                    {
                        Console.WriteLine("GamePad : ButtonsUp = " + game_pad_data.ButtonsUp);
                    }
                }


                clickPos = touchPoint;
				
                // Handle mouse interaction
	            if (mouseMoveOld == false && mouseMove == true)
	            {
					if(clickIndex == -1)
						clickIndex = myScene.CheckPicking(ref clickPos, ref diffPos, false);
	            }
				
				// Scene which contains controls using fixed index of sceneBodies should not be allowed to delete objects
				// because it can cause the program crash if it does not exsists anymore 
				if((manipulateMode)&&(mouseMoveOld == false && mouseMove == true))
				{
					switch(sceneNum)
					{
						case (int)SceneName.CompoundScene:
						case (int)SceneName.JointScene:	
						case (int)SceneName.ChainJointScene:
						case (int)SceneName.ChainSpringScene:
						case (int)SceneName.PyramidStackScene:
						case (int)SceneName.CreateRagdollScene:
						
							clickIndex = myScene.CheckForcePicking(ref clickPos, ref diffPos, false);
							if((clickIndex >=0) && (clickIndex < myScene.numBody))
								myScene.DeleteBody(clickIndex);
							clickIndex = -1;
							break;
						default:
							// nothing to do
							// it is danger to delete objects because 
							// some of these scenes use fixed index to manipulate objects,
							// and those objects should not be removed by clicking
							break;
						
					}
					
				}
				
				myScene.Simulate(clickIndex, clickPos, diffPos);
				
				// Change eye position based on the scene only for VehicleMakeScene
				if(sceneNum == (int)SceneName.VehicleMakeScene)
				{
					eyePos = ((VehicleMakeScene)myScene).eyePos;
				}
				else if(sceneNum == (int)SceneName.SpriteBatchWithScene)
				{
					eyePos = ((SpriteBatchWithScene)myScene).eyePos;
					((SpriteBatchWithScene)myScene).clickPos = clickPos;
				}
					
                Draw();

               
                mouseMoveOld = mouseMove;

            }

            Term();
        }
		
		
		static void SwitchScene(int num)
		{
			renderingScale = 10.0f;
			mouseMoveOld = false;
			mouseMove = false;
			clickIndex = -1;
			touchPoint = new Vector2(0.0f, 0.0f);
			eyePos = new Vector2(0.0f, 0.0f);
			manipulateMode = false;
			
			renderingScale *= scaleResolution;
			
			switch (num)
			{
				case (int)SceneName.PrimitiveScene:
					myScene = new PrimitiveScene(); break;
				
				case (int)SceneName.CompoundScene:
					myScene = new CompoundScene(); break;
				
				case (int)SceneName.JointScene: 
					myScene = new JointScene(); break;
				
				case (int)SceneName.PyramidStackScene: 
					myScene = new PyramidStackScene(); break;
				
				case (int)SceneName.SmallScaleScene:
					myScene = new SmallScaleScene(); break;
				
				case (int)SceneName.AirHockeyScene:
					myScene = new AirHockeyScene(); break;
				
				case (int)SceneName.HammerThrowerScene:
					myScene = new HammerThrowerScene(); break;
				
				case (int)SceneName.PinBallScene: 
					myScene = new PinBallScene(); renderingScale = 3.0f; break;
				
				case (int)SceneName.DifferentMassScene:
					myScene = new DifferentMassScene(); break;							
				
				case (int)SceneName.MassUnstableScene:
					myScene = new MassUnstableScene(); break;
				
				case (int)SceneName.LargeSmallScene:
					myScene = new LargeSmallScene(); break;
				
				case (int)SceneName.CreateConvexScene:
					myScene = new CreateConvexScene(); break;
				
				case (int)SceneName.SleepWakeupScene: 
					myScene = new SleepWakeupScene(); break;
				
				case (int)SceneName.CollisionFilterScene:
					myScene = new CollisionFilterScene(); break;
				
				case (int)SceneName.GroupFilterScene:
					myScene = new GroupFilterScene(); break;
				
				case (int)SceneName.CollisionGroupScene:
					myScene = new CollisionGroupScene(); break;	
				
				case (int)SceneName.BallJointScene: 
					myScene = new BallJointScene(); break;
				
				case (int)SceneName.SliderJointScene: 
					myScene = new SliderJointScene(); break;			
				
				case (int)SceneName.AnyDirSliderScene:
					myScene = new AnyDirSliderScene(); break;	
				
				case (int)SceneName.FixedOrStaticScene:
					myScene = new FixedOrStaticScene(); break;	
				
				case (int)SceneName.ChainJointScene:
					myScene = new ChainJointScene(); break;
				
				case (int)SceneName.ChainSpringScene:
					myScene = new ChainSpringScene(); break;
				
				case (int)SceneName.WheelMakeScene:
					myScene = new WheelMakeScene(); break;
				
				case (int)SceneName.QueryContactScene:
					myScene = new QueryContactScene(); break;
				
				case (int)SceneName.CreateCompoundScene: 
					myScene = new CreateCompoundScene(); break;
				
				case (int)SceneName.ScenePropertyScene: 
					myScene = new ScenePropertyScene(); break;
				
				case (int)SceneName.NonCenterJointScene: 
					myScene = new NonCenterJointScene(); break;
				
				case (int)SceneName.TriggerObjectScene:
					myScene = new TriggerObjectScene(); break;
				
				case (int)SceneName.CompoundBallanceScene:
					myScene = new CompoundBallanceScene(); break;
				
				case (int)SceneName.ChangeTensorScene:
					myScene = new ChangeTensorScene(); break;
				
				case (int)SceneName.CreateRagdollScene:
					myScene = new CreateRagdollScene(); break;
				
				case (int)SceneName.JointRotatingScene:
					myScene = new JointRotatingScene(); break;
				
				case (int)SceneName.SimulationAreaScene:
					myScene = new SimulationAreaScene(); break;
				
				case (int)SceneName.LineChainScene: 
					myScene = new LineChainScene(); break;
				
				case (int)SceneName.VehicleMakeScene:
					myScene = new VehicleMakeScene(); break;
				
				case (int)SceneName.TextureWithScene: 
					myScene = new TextureWithScene(); break;
				
				case (int)SceneName.KinematicObjectScene:
					myScene = new KinematicObjectScene(); break;
				
				case (int)SceneName.AnimationWithScene:
					myScene = new AnimationWithScene(); break;
				
				case (int)SceneName.SpriteBatchWithScene:
					myScene = new SpriteBatchWithScene(); break;
				
				case (int)SceneName.RayCastScene:
					myScene = new RayCastScene(); break;
				
				case (int)SceneName.JointSoftbodyScene:
					myScene = new JointSoftbodyScene(); break;
				
				default:
					myScene = new PrimitiveScene(); break;
			}	
		}                       

        // Touch Pad Interaction
        // Convert Function
        //
        // (-0.5, 0.5) X (-0.5, 0.5) <=> (x * 20.0f / (854.0f / 2.0f)) X (y * 20.0f / 240.0f)
        //
        static Vector2 GetClickPos(float x, float y)
		{				
            return new Vector2(x * (windowWidth / 2.0f) / (0.5f * renderingScale) + eyePos.X,
                -y * (windowHeight / 2.0f) / (0.5f * renderingScale) +  eyePos.Y );
        }
		
        static bool Init()
        {	
            program = new ShaderProgram("/Application/shaders/test.cgx");
            program.SetUniformBinding(0, "u_WorldMatrix");
            program.SetUniformBinding(1, "u_Color");

            // simulation setup
            SwitchScene(sceneNum);

            touchVertices = new VertexBuffer(4, VertexFormat.Float3);
            MakeQuadPointList(touchVertices, 0.2f);

            return (program != null);
        }

        static void Term()
        {
			if(graphics != null) SampleDraw.Term();
			
            if(touchVertices != null) touchVertices.Dispose();
            if(program != null) program.Dispose();
            if(graphics != null) graphics.Dispose();
        }
        
        static void Draw()
        {
            graphics.SetViewport(0, 0, windowWidth, windowHeight);
            graphics.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            graphics.Clear();

            graphics.SetShaderProgram(program);

            worldMatrix = new Matrix4(
                     renderingScale / (windowWidth / 2.0f), 0.0f, 0.0f, 0.0f,
                     0.0f, renderingScale / (windowHeight / 2.0f), 0.0f, 0.0f,
                     0.0f, 0.0f, 1.0f, 0.0f,
                     -renderingScale / (windowWidth / 2.0f) * eyePos.X, -renderingScale / (windowHeight / 2.0f) * eyePos.Y, 0.0f, 1.0f
                 );

            // Draw objects
            myScene.DrawAllBody(ref graphics, ref program, worldMatrix, clickIndex);

            // Draw debug information
            myScene.DrawAdditionalInfo(ref graphics, ref program, worldMatrix);

            // Draw touch points
            {
                graphics.SetVertexBuffer(0, touchVertices);

                Matrix4 transMatrix = Matrix4.Translation(
                    new Vector3(touchPoint.X, touchPoint.Y, 0.0f));

                Matrix4 WorldMatrix = worldMatrix * transMatrix;
                program.SetUniformValue(0, ref WorldMatrix);

                Vector3 color = new Vector3(0.0f, 1.0f, 0.0f);
                program.SetUniformValue(1, ref color);

                graphics.DrawArrays(DrawMode.TriangleFan, 0, 4);
            }

            // Draw text message
            SampleDraw.DrawText(myScene.SceneName, 0xffffffff, 0, 0);

            graphics.SwapBuffers();
        }

        // Check touch points
        public static void MakeQuadPointList(VertexBuffer vertices, float scale)
        {

            float[] vertex = new float[]
            {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                1.0f, 1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f
            };

            for (int i = 0; i < 12; i++)
                vertex[i] = vertex[i] * scale;

            vertices.SetVertices(0, vertex);
        }

	}    
}
