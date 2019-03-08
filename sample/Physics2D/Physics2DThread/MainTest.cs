/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

//
// 2D Physics Sample Main
//

// [Tips and notice]
//
// This sample show that there is a way to speed up the program by dividing 
// thread into main(including drawing) and simulation for multi-core cpu device.
// But for one-core cpu devices, this way may decrease the performnace a little.
//
// When you create a thread and make it run physics simulation, you should be careful
// to avoid confliction of physics data between simulation and main thread(including drawing).
//
// To draw objects, it is necessary to hold old status of physics object in seperate buffer and render those data,
// becaure rendering code can run in parallel together with simulation thread.
//
// While simulation is runnning, status of physics objects changes dynamically.
// Therefore, to make rendering and simulation run in parallel,
// rendering should use the buffer which contains old status of physics objects.
//

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
        static uint sceneNum = 0;
		
		static Vector2 eyePos = new Vector2(0.0f, 0.0f);
		
	    const uint totalScene = 2;
		
        #endregion

		static bool loop = true;
		
		// These values are used for thread control
		static bool simStart = false;
		static bool useThread = true;		
		static Thread simThread = null;
		static bool isThreadAlive = true;
		static object syncObject = new object();
		
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
			
			{
				// Simulation is not done yet
				simStart = false;
				
				// Simulation loop main thread entry point
				simThread = new Thread(new ThreadStart(SimulateThreadMain));
				
				// Simulation loop thread start
				simThread.Start ();
			}
			
			while (loop)
            {

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
							// Before changing myScene, 
							// it is necessary to avoid confliction with other thread
							lock(syncObject)
							{
                            	myScene.SceneBodies[clickIndex].velocity = new Vector2(0, 0);
                            	myScene.SceneBodies[clickIndex].angularVelocity = 0.0f;
                            	clickIndex = -1;
							}
                        }
						
						touchID = -1;
                        mouseMove = false;
                    }
                }

                {
                    GamePadData game_pad_data = GamePad.GetData(0);
                    if (game_pad_data.ButtonsDown != 0)
                    {
                        switch (game_pad_data.ButtonsDown)
                        {
                            case GamePadButtons.Left:
                            
                                Console.WriteLine("-> Previous Scene");
										
								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                                	myScene.ReleaseScene();	
								}
								
								GC.Collect();

                                sceneNum = (sceneNum + (totalScene - 1)) % totalScene;

      							// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                    	        	SwitchScene(sceneNum);
								}
                                break;

                            case GamePadButtons.Right:

                                Console.WriteLine("-> Next Scene");

								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                                	myScene.ReleaseScene();	
								}
							
                                GC.Collect();

                                sceneNum = (++sceneNum) % totalScene;

								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                    	        	SwitchScene(sceneNum);
								}

                                break;

                            case GamePadButtons.Triangle:

                                Console.WriteLine("<-> Reset Scene");

								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                                	myScene.ReleaseScene();	
								}
							
                                GC.Collect();

								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                    	        	SwitchScene(sceneNum);
								}

                                break;
							                           
							// This will change Threading <-> Non threading implementation
							case GamePadButtons.Square:	
								
								Console.WriteLine ("->  Threading was changed ");
								useThread = !useThread;
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
							
								// Before changing myScene, 
								// it is necessary to avoid confliction with other thread
								lock(syncObject)
								{
                                	myScene.KeyboardFunc(game_pad_data.ButtonsDown);
								}
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
		
				// In separate thread mode, this just changes the flag "simStart" to true
				if(useThread)	
				{
					// The previous simulation is not finished
					while(simStart != false)
					{
						Thread.Sleep(1);
					}
				
					// Copy necessary data for rendering before simulation starts
					if(sceneNum == 0)
						((TextureWithPrimitive)myScene).CopyPreviousFrame();
					else if(sceneNum == 1)
						((TextureWithStack)myScene).CopyPreviousFrame();
					else
						((TextureWithPrimitive)myScene).CopyPreviousFrame();
								
					if((myScene.sceneFrame % 20) == 0)
						Console.WriteLine("Separate Simulation Thread Mode");
					
					// Just raise this flag as true, it means that simulation is ready to start
					simStart = true;
					
				}
				// For one thread mode, you should call myScene.Simulate actually
				else
				{
					
					// Copy necessary data for rendering before simulation starts
					// This is not necessary for one thread mode originally.
					// But to share source code between one thread mode and seperate thread mode,
					// this sample re-use rendering code of seperate thread mode.
					if(sceneNum == 0)
						((TextureWithPrimitive)myScene).CopyPreviousFrame();
					else if(sceneNum == 1)
						((TextureWithStack)myScene).CopyPreviousFrame();
					else
						((TextureWithPrimitive)myScene).CopyPreviousFrame();
					
					if((myScene.sceneFrame % 20) == 0)
						Console.WriteLine("One Thread Mode");
					myScene.Simulate(clickIndex, clickPos, diffPos);
				}
					
				if(sceneNum == 0)
				{
					((TextureWithPrimitive)myScene).useThread = useThread;
					((TextureWithPrimitive)myScene).PerfCheck();	
				}
				else if(sceneNum == 1)
				{
					((TextureWithStack)myScene).useThread = useThread;
					((TextureWithStack)myScene).PerfCheck();	
				}
				else
				{
					((TextureWithPrimitive)myScene).useThread = useThread;
					((TextureWithPrimitive)myScene).PerfCheck();	
				}
				
				Draw();
			

                mouseMoveOld = mouseMove;		
				
            }
			
			if(isThreadAlive)
			{
				// simulation thread exit
				Console.WriteLine ("Trying to finish thread ...");
				isThreadAlive = false;
				simThread.Join ();
				useThread = false;
			}
			
            Term();
        }
		
		
		static void SwitchScene(uint num)
		{
			renderingScale = 10.0f;
			mouseMoveOld = false;
			mouseMove = false;
			clickIndex = -1;
			touchPoint = new Vector2(0.0f, 0.0f);
			eyePos = new Vector2(0.0f, 0.0f);

			renderingScale *= scaleResolution;
			
			switch (num)
			{
				case 0: myScene = new TextureWithPrimitive(); break;						
				case 1: myScene = new TextureWithStack(); break;				
				default: myScene = new TextureWithPrimitive(); break;
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
        
		static void SimulateThreadMain()
		{
			Console.WriteLine("Simulation Thread Start");
			
			// Are there any events which stop the program ???
			while(isThreadAlive)
			{
				// Previous simulation finished and it is ready to start ???
				if(simStart == true)
				{
					// Before changing myScene, 
					// it is necessary to avoid confliction with other thread					
					lock(syncObject)
					{
						myScene.Simulate(clickIndex, clickPos, diffPos);
						// To avoid doing several times simulation within one frame,
						// set this flag as false
						simStart = false;
					}
				}
				else
				{
					// Nothing to do now, therefore it sleeps for a while
					Thread.Sleep(1);
				}
			}
			
			Console.WriteLine("Simulation Thread End");
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
