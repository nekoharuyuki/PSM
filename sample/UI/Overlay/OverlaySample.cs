/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System ;
using System.Threading ;
using System.Diagnostics ;
using System.Collections.Generic ;
using Sce.PlayStation.Core ;
using Sce.PlayStation.Core.Graphics ;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;

namespace OverlaySample
{
    static class OverlaySample
    {

        static Stopwatch stopwatch = new Stopwatch () ;
        static GraphicsContext graphics = new GraphicsContext () ;
        static Model model ;
        static SoundPlayer sound;
        static UIOverlayScene scene;

        static int frameCount ;
        static int cpuTicks ;
        static int gpuTicks ;

        static void Main (string[] args)
        {
            Init ();
            while (true) {
                int start = (int)stopwatch.ElapsedTicks ;

                SystemEvents.CheckEvents ();

                List<TouchData> touchData = Touch.GetData (0);

                // Update UI
                UISystem.Update (touchData);
                // Update main model
                UpdateBackground (touchData);

                // Clear graphics
                graphics.SetViewport (0, 0, graphics.Screen.Width, graphics.Screen.Height);
                graphics.SetClearColor (scene.SettingDialog.SettingColorRgba);
                graphics.SetClearDepth (1.0f);
                graphics.Clear ();
                // Draw main model
                DrawBackground ();
                // Draw UI
                UISystem.Render ();

                cpuTicks += (int)stopwatch.ElapsedTicks - start ;
                graphics.SwapBuffers ();
                gpuTicks += (int)stopwatch.ElapsedTicks - start ;

                if ( ( ++ frameCount ) % 100 == 0 ) {
                    float freq = (float)Stopwatch.Frequency ;
                    float cpu = (float)cpuTicks * 60.0f / freq ;
                    float gpu = (float)gpuTicks * 60.0f / freq ;
                    float fps = freq * 100.0f / (float)gpuTicks ;
                    Console.Write( "CPU {0:f2}% / GPU {1:f2}% / {2:f2}fps\n", cpu, gpu, fps ) ;
                    cpuTicks = gpuTicks = 0 ;
                }
            }
        }

        static void Init ()
        {
            stopwatch.Start ();
            model = new Model ();
            
            var soundObj = new Sound("/Application/assets/touch_sound.wav");
            sound = soundObj.CreatePlayer();

            UISystem.Initialize (graphics);
            scene = new UIOverlayScene ();
            UISystem.SetScene (scene);
            Console.WriteLine ("Init finished.");
        }

        static void UpdateBackground (List<TouchData> touchData)
        {
            foreach (TouchData item in touchData) {
                if(item.Skip) continue;
                
                if(item.Status == TouchStatus.Down){
                    sound.Play();
                }
            }
        }

        static void DrawBackground ()
        {
            float seconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f ;
            float weave = (float)Math.Sin (seconds * 1.28f * scene.SettingDialog.Speed) * 0.2f;
            float turn = seconds * 0.1f * scene.SettingDialog.Speed;
            Matrix4 world = Matrix4.RotationYxz (0.0f, turn, weave);
            Matrix4 worldInverse = world.InverseOrthonormal ();
            Vector3 lightDirection = worldInverse.TransformVector (new Vector3 (-1.0f, 1.0f, 1.0f));

            graphics.Enable (EnableMode.Blend);
            graphics.SetBlendFunc (BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
            graphics.Enable (EnableMode.CullFace);
            graphics.SetCullFace (CullFaceMode.Back, CullFaceDirection.Ccw);
            graphics.Enable (EnableMode.DepthTest);
            graphics.SetDepthFunc (DepthFuncMode.LEqual, true);

            Matrix4 worldViewProj ;
            for (int i = 0; i < 50; i ++) {
                world.M41 = (float)(i % 10) * 4.0f - 18.0f;
                world.M42 = (float)(i / 10) * 4.0f - 8.0f;
                Matrix4.Multiply (ref viewProj, ref world, out worldViewProj);
                model.Draw (graphics, ref worldViewProj, ref lightDirection);
            }
        }

        static Matrix4 viewProj = new Matrix4 (
            //  perspective ( 0.9273f, 854.0f / 480.0f, 1.0f, 1000000.0f ) ;
            //  lookat      ( 0.0f, 0.0f, 25.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f )
            1.124122f, 0.000000f, 0.000000f, 0.000000f,
            0.000000f, 2.000000f, 0.000000f, 0.000000f,
            0.000000f, 0.000000f, -1.000002f, -1.000000f,
            0.000000f, 0.000000f, 23.000050f, 25.000000f
            ) ;
    }

} // namespace
