/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Text;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.UI;
using System.Diagnostics;

namespace GameUI
{
    public class GameLayer
    {
        GraphicsContext graphics;
        Stopwatch stopwatch;
        Model model;
        SoundPlayer sound;

        public void Init(GraphicsContext graphics)
        {
            this.graphics = graphics;

            stopwatch = new Stopwatch();
            stopwatch.Start();

            model = new Model();

            var soundObj = new Sound("/Application/assets/touch_sound.wav");
            sound = soundObj.CreatePlayer();
        }

        public void Update(List<TouchData> touchData)
        {
            if (Globals.GameState == GameState.Stop)
            {
                return;
            }

            foreach (TouchData item in touchData)
            {
                if (item.Skip) continue;

                if (item.Status == TouchStatus.Down)
                {
                    sound.Play();
                }
            }
        }

        long lastTicks = 0;
        float weave;
        float turn;

        public void Draw()
        {
            if (Globals.GameState == GameState.Stop)
            {
                return;
            }

            long ticks = stopwatch.ElapsedTicks;
            if (Globals.GameState == GameState.Play)
            {
                var seconds = (float)(ticks - lastTicks) / (float)Stopwatch.Frequency;
                turn += seconds * 0.1f * Globals.Speed;
                if (turn > Math.PI * 2) turn -= (float)Math.PI * 2;
                weave = (float)Math.Sin(turn * 10) * 0.2f;
            }
            lastTicks = ticks;

            Matrix4 world, worldInverse;
            Matrix4.RotationYxz(0.0f, turn, weave, out world);
            worldInverse = world.InverseOrthonormal();
            Vector3 lightDirection = worldInverse.TransformVector(new Vector3(-1.0f, 1.0f, 1.0f));

            graphics.Enable(EnableMode.Blend);
            graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
            graphics.Enable(EnableMode.CullFace);
            graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
            graphics.Enable(EnableMode.DepthTest);
            graphics.SetDepthFunc(DepthFuncMode.LEqual, true);

            Matrix4 worldViewProj;
            for (int i = 0; i < 50; i++)
            {
                world.M41 = (float)(i % 10) * 4.0f - 18.0f;
                world.M42 = (float)(i / 10) * 4.0f - 8.0f;
                Matrix4.Multiply(ref viewProj, ref world, out worldViewProj);
                model.Draw(graphics, ref worldViewProj, ref lightDirection);
            }
        }

        Matrix4 viewProj = new Matrix4(
            //  perspective ( 0.9273f, 854.0f / 480.0f, 1.0f, 1000000.0f ) ;
            //  lookat      ( 0.0f, 0.0f, 25.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f )
            1.124122f, 0.000000f, 0.000000f, 0.000000f,
            0.000000f, 2.000000f, 0.000000f, 0.000000f,
            0.000000f, 0.000000f, -1.000002f, -1.000000f,
            0.000000f, 0.000000f, 23.000050f, 25.000000f
        );

    }
}
