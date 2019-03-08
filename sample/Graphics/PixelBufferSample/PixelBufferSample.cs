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

namespace Sample
{

/**
 * PixelBufferSample
 */
class PixelBufferSample
{
    static GraphicsContext graphics;
    static Stopwatch stopwatch;
    static FrameBuffer frameBuffer;
    static Texture2D renderTexture;
    static DepthBuffer depthBuffer;
    static ShaderProgram vcolorShader;
    static ShaderProgram textureShader;
    static VertexBuffer triangleVertices;
    static VertexBuffer cubeVertices;
    static Texture2D triangleTexture;

    const int offscreenWidth = 256;
    const int offscreenHeight = 256;

    static bool loop = true;

    static void Main(string[] args)
    {
        Init();
        while(loop){
            SystemEvents.CheckEvents();
            Update();
            Render();
        }
        Term();
    }

    static void Init()
    {
        graphics = new GraphicsContext();
        stopwatch = new Stopwatch();
        stopwatch.Start();

        SampleDraw.Init(graphics);

        // framebuffer

        frameBuffer = new FrameBuffer();
        renderTexture = new Texture2D(offscreenWidth, offscreenHeight, false, PixelFormat.Rgba, PixelBufferOption.Renderable);
        depthBuffer = new DepthBuffer(offscreenWidth, offscreenHeight, PixelFormat.Depth16);
        frameBuffer.SetColorTarget(renderTexture, 0);
        frameBuffer.SetDepthTarget(depthBuffer);

        // vcolor shader

        vcolorShader = new ShaderProgram("/Application/shaders/VertexColor.cgx");
        vcolorShader.SetUniformBinding(0, "WorldViewProj");
        vcolorShader.SetAttributeBinding(0, "a_Position");
        vcolorShader.SetAttributeBinding(1, "a_TexCoord");
        vcolorShader.SetAttributeBinding(2, "a_Color");

        // texture shader

        textureShader = new ShaderProgram("/Application/shaders/Texture.cgx");
        textureShader.SetUniformBinding(0, "WorldViewProj");
        textureShader.SetAttributeBinding(0, "a_Position");
        textureShader.SetAttributeBinding(1, "a_TexCoord");

        // vertex buffer

        triangleVertices = CreateTriangleVertices();
        cubeVertices = CreateCubeVertices();

        // texture

        triangleTexture = new Texture2D("/Application/test.png", false);

        // renderstate

        graphics.Enable(EnableMode.DepthTest);
        graphics.Enable(EnableMode.CullFace);
        graphics.SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);
        graphics.Enable(EnableMode.CullFace, false);
    }

    static void Term()
    {
        SampleDraw.Term();
        frameBuffer.Dispose();
        graphics.Dispose();
    }

    static void Update()
    {
        SampleDraw.Update();
    }

    static void Render()
    {
        float seconds = (float)stopwatch.ElapsedMilliseconds / 1000.0f;

        graphics.SetFrameBuffer(frameBuffer);
        RenderToOffScreen(seconds);

        graphics.SetFrameBuffer(graphics.Screen);
        RenderToScreen(seconds);

        graphics.Enable(EnableMode.CullFace, false);

        SampleDraw.DrawText("PixelBufferSample", 0xffffffff, 0, 0);

        graphics.SwapBuffers();
    }

    static void RenderToOffScreen(float seconds)
    {
        FrameBuffer currentBuffer = graphics.GetFrameBuffer() ;

        float aspect = currentBuffer.AspectRatio;
        float fovy = FMath.Radians(30.0f);
        Matrix4 proj = Matrix4.Perspective(fovy, aspect, 1.0f, 1000000.0f);
        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.5f, 3.0f), new Vector3(0.0f, 0.5f, 0.0f), Vector3.UnitY);
        Matrix4 world = Matrix4.RotationY(1.0f * seconds);
        Matrix4 worldViewProj = proj * view * world;
        vcolorShader.SetUniformValue(0, ref worldViewProj);

        graphics.SetViewport(0, 0, currentBuffer.Width, currentBuffer.Height);
        graphics.SetClearColor(0.0f, 0.0f, 1.0f, 1.0f);
        graphics.Clear();

        graphics.SetShaderProgram(vcolorShader);
        graphics.SetVertexBuffer(0, triangleVertices);
        graphics.SetTexture(0, triangleTexture);

        graphics.Enable(EnableMode.CullFace, false);
        graphics.DrawArrays(DrawMode.Triangles, 0, triangleVertices.VertexCount);
    }

    static void RenderToScreen(float seconds)
    {
        FrameBuffer currentBuffer = graphics.GetFrameBuffer() ;

        float aspect = currentBuffer.AspectRatio;
        float fovy = FMath.Radians(45.0f);
        Matrix4 proj = Matrix4.Perspective(fovy, aspect, 1.0f, 1000000.0f);
        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero, Vector3.UnitY);
        // Matrix4 world = Matrix4.RotationYxz(1.0f * seconds, 0.5f * seconds, 0.0f);
        Matrix4 world = Matrix4.RotationY(0.3f * seconds);
        Matrix4 worldViewProj = proj * view * world;
        textureShader.SetUniformValue(0, ref worldViewProj);

        graphics.SetViewport(0, 0, currentBuffer.Width, currentBuffer.Height);
        graphics.SetClearColor(0.0f, 0.5f, 1.0f, 1.0f);
        graphics.Clear();

        graphics.SetShaderProgram(textureShader);
        graphics.SetTexture(0, renderTexture);
        graphics.SetVertexBuffer(0, cubeVertices);

        graphics.Enable(EnableMode.CullFace, true);
        graphics.DrawArrays(DrawMode.Triangles, 0, cubeVertices.IndexCount);
    }

    static VertexBuffer CreateTriangleVertices()
    {
        VertexBuffer vertexbuffer = new VertexBuffer(3, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);

        float[] positions = {
            0.0f, 1.0f, 0.0f,
            -0.5f, 0.0f, 0.0f,
            0.5f, 0.0f, 0.0f,
        };
        float[] texcoords = {
            0.5f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
        };
        float[] colors = {
            1.0f, 0.0f, 0.0f, 1.0f,
            0.0f, 1.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 1.0f,
        };
        vertexbuffer.SetVertices(0, positions);
        vertexbuffer.SetVertices(1, texcoords);
        vertexbuffer.SetVertices(2, colors);

        return vertexbuffer;
    }

    static VertexBuffer CreateCubeVertices()
    {
        VertexBuffer vertexbuffer = new VertexBuffer(24, 36, VertexFormat.Float3, VertexFormat.Float2);

        float[] positions = {
            // top
            -0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,
            // bottom
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            // front
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            // back
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            // right
             0.5f,  0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
            // left
            -0.5f,  0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f
        };
        float[] texcoords = {   //  Note: v coordinate is flipped
            // top
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            // bottom
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            // front
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            // back
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            // right
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            // left
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 1.0f,
            1.0f, 0.0f
        };
        ushort[] indices = {
            // top
             0,  1,  2, 
             1,  3,  2, 
            // bottom
             4,  5,  6, 
             5,  7,  6, 
            // front
             8,  9, 10, 
             9, 11, 10, 
            // back
            12, 13, 14, 
            13, 15, 14, 
            // right
            16, 17, 18, 
            17, 19, 18, 
            // left
            20, 21, 22, 
            21, 23, 22
        };
        vertexbuffer.SetVertices(0, positions);
        vertexbuffer.SetVertices(1, texcoords);
        vertexbuffer.SetIndices(indices);

        return vertexbuffer;
    }
}

} // end ns Sample
