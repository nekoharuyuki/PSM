/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Diagnostics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using DemoGame;

namespace DefenseDemo
{

/**
 * Rendererクラス
 */
public static class Renderer
{
	/// グラフィックコンテキスト
	private static GraphicsDevice graphicsDevice;

	/// 初期化
	/**
	 * @param [in] graphicsDevice : グラフィックスデバイス
	 */
	public static bool Init(GraphicsDevice graphicsDevice)
	{
		Renderer.graphicsDevice = graphicsDevice;

		GetGraphicsContext().Enable(EnableMode.CullFace);
		GetGraphicsContext().SetCullFace(CullFaceMode.Back, CullFaceDirection.Ccw);

		GetGraphicsContext().Enable(EnableMode.Blend);
		GetGraphicsContext().SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);

		return true;
	}

	/// 解放
	/**
	 */
	public static void Term()
	{
	}

	/// グラフィックデバイスクラスの取得
	/**
	 */
	public static GraphicsDevice GetGraphicsDevice()
	{
		return graphicsDevice;
	}

	/// グラフィックコンテキストクラスの取得
	/**
	 */
	public static GraphicsContext GetGraphicsContext()
	{
		return graphicsDevice.Graphics;
	}
}

} // ShootingDemo
