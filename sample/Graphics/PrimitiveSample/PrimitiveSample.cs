/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Sample
{

/**
 * PrimitiveSample
 */
class PrimitiveSample
{
    static GraphicsContext graphics;
    static ShaderProgram program;
    static VertexBuffer vertices;

    static ModeInfo[] modeInfo = new ModeInfo[] {
        new ModeInfo(DrawMode.Points, "< Point List >"),
        new ModeInfo(DrawMode.Lines, "< Line List >"),
        new ModeInfo(DrawMode.LineStrip, "< Line Strip >"),
        new ModeInfo(DrawMode.Triangles, "< Triangle List >"),
        new ModeInfo(DrawMode.TriangleStrip, "< Triangle Strip >"),
        new ModeInfo(DrawMode.TriangleFan, "< Triangle Fan >")
    };
    static int modeIndex = 0;
    static bool loop = true;

    static void Main(string[] args)
    {
        Init();
        while (loop) {
            SystemEvents.CheckEvents();
            Update();
            Render();
        }
        Term();
    }

    static bool Init()
    {
        graphics = new GraphicsContext();

        SampleDraw.Init(graphics);

        program = new ShaderProgram("/Application/shaders/VertexColor.cgx");
        program.SetUniformBinding(0, "WorldViewProj");
        program.SetAttributeBinding(0, "a_Position");
        program.SetAttributeBinding(1, "a_Color0");

        vertices = createVertices(modeInfo[modeIndex].drawMode);
        return true;
    }

    static void Term()
    {
        SampleDraw.Term();
        program.Dispose();
        vertices.Dispose();
        graphics.Dispose();
    }

    static bool Update()
    {
        SampleDraw.Update();

        int nextModeIndex = modeIndex;

        List<TouchData> touchDataList = Touch.GetData(0);
        foreach (var touchData in touchDataList) {
            if (touchData.Status == TouchStatus.Down) {
                nextModeIndex = (modeIndex + 1) % modeInfo.Length;
            }
        }

        var gamePadData = GamePad.GetData(0);
        if ((gamePadData.ButtonsDown & GamePadButtons.Right) != 0) {
            nextModeIndex = (modeIndex + 1) % modeInfo.Length;
        } else if ((gamePadData.ButtonsDown & GamePadButtons.Left) != 0) {
            nextModeIndex = (modeIndex - 1) % modeInfo.Length;
            if (nextModeIndex < 0) nextModeIndex = modeInfo.Length - 1;
        }

        if (modeIndex != nextModeIndex) {
            modeIndex = nextModeIndex;

            vertices.Dispose();
            vertices = createVertices(modeInfo[modeIndex].drawMode);
        }
        return true;
    }

    static bool Render()
    {
        int dspWidth = graphics.Screen.Width;
        int dspHeight = graphics.Screen.Height;

        Matrix4 proj = Matrix4.Ortho(0, dspWidth, dspHeight, 0, 0.0f, 32768.0f);
        program.SetUniformValue(0, ref proj);

        graphics.SetViewport(0, 0, dspWidth, dspHeight);
        graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        graphics.Clear();

        graphics.SetShaderProgram(program);
        graphics.SetVertexBuffer(0, vertices);
        graphics.DrawArrays(modeInfo[modeIndex].drawMode, 0, vertices.VertexCount);

        SampleDraw.DrawText("Primitive Sample", 0xffffffff, 0, 0);
        SampleDraw.DrawText(modeInfo[modeIndex].desc, 0xffffffff, 0, 48);

        graphics.SwapBuffers();

        return true;
    }

    /// Create a VertexBuffer object per draw mode
    static VertexBuffer createVertices(DrawMode drawMode)
    {
        VertexBuffer vertices = null;

        switch (drawMode) {
        case DrawMode.Points:
            Random rand = new System.Random();

            vertices = new VertexBuffer(200, VertexFormat.Float3, VertexFormat.Float4);
            float[] vertexs = new float[vertices.VertexCount * 3];
            for (int i = 0; i < vertices.VertexCount; i++) {
                vertexs[i * 3 + 0] = (float)rand.Next(graphics.Screen.Width);
                vertexs[i * 3 + 1] = (float)rand.Next(graphics.Screen.Height);
                vertexs[i * 3 + 2] = 0.0f;
            }
            vertices.SetVertices(0, vertexs);
            break;
        case DrawMode.Lines:
            vertices = new VertexBuffer(6, VertexFormat.Float3, VertexFormat.Float4);
            vertices.SetVertices(0, new float[]{300, 200, 0,
                                                500, 200, 0,
                                                400, 250, 0,
                                                600, 250, 0,
                                                300, 300, 0,
                                                500, 300, 0});
            break;
        case DrawMode.LineStrip:
            vertices = new VertexBuffer(5, VertexFormat.Float3, VertexFormat.Float4);
            vertices.SetVertices(0, new float[]{200, 200, 0,
                                                300, 300, 0,
                                                400, 250, 0,
                                                500, 350, 0,
                                                600, 300, 0});
            break;
        case DrawMode.Triangles:
            vertices = new VertexBuffer(9, VertexFormat.Float3, VertexFormat.Float4);
            vertices.SetVertices(0, new float[]{200, 200, 0,
                                                250, 150, 0,
                                                300, 250, 0,
                                                400, 400, 0,
                                                450, 300, 0,
                                                500, 400, 0,
                                                600, 300, 0,
                                                650, 200, 0,
                                                700, 250, 0});
            break;
        case DrawMode.TriangleStrip:
            vertices = new VertexBuffer(7, VertexFormat.Float3, VertexFormat.Float4);
            vertices.SetVertices(0, new float[]{250, 300, 0,
                                                300, 200, 0,
                                                350, 300, 0,
                                                400, 200, 0,
                                                450, 300, 0,
                                                500, 200, 0,
                                                550, 300, 0});
            break;
        case DrawMode.TriangleFan:
            vertices = new VertexBuffer(5, VertexFormat.Float3, VertexFormat.Float4);
            vertices.SetVertices(0, new float[]{400, 300, 0,
                                                350, 200, 0,
                                                450, 200, 0,
                                                500, 250, 0,
                                                500, 350, 0});
            break;
        }

        if (vertices != null) {
            setVertexColor(vertices);
        }
        return vertices;
    }

    /// Set some vertex colors
    static void setVertexColor(VertexBuffer vertices)
    {
        float[] baseColors = {
            1.0f, 0.0f, 0.0f, 1.0f,
            0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 1.0f,
        };

        float[] colors = new float[vertices.VertexCount * 4];

        for (int i = 0; i < vertices.VertexCount; i++) {
            colors[i * 4 + 0] = baseColors[(i % 3) * 4 + 0];
            colors[i * 4 + 1] = baseColors[(i % 3) * 4 + 1];
            colors[i * 4 + 2] = baseColors[(i % 3) * 4 + 2];
            colors[i * 4 + 3] = baseColors[(i % 3) * 4 + 3];
        }

        vertices.SetVertices(1, colors);
    }

    /// Mode information class
    class ModeInfo
    {
        public DrawMode drawMode;
        public string desc;

        public ModeInfo(DrawMode drawMode, string desc)
        {
            this.drawMode = drawMode;
            this.desc = desc;
        }
    }
}

} // Sample
