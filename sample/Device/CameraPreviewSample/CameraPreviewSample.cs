/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Device;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;

namespace Sample
{

/**
 * CameraPreviewSample
 */
public class CameraPreviewSample
{
	static readonly bool REDUCE_TILT = true;
		
	// for System
	static GraphicsContext sm_GraphicsContext = null;
	static bool sm_IsLoop = true;
		
	// for UserInterface
	static bool sm_IsRenderText = true;
	static String sm_PictureFilename = string.Empty;
	
	// for Camera
	static Camera sm_Camera = null;
	static int sm_CameraDeviceCount = 0;
	static int sm_CameraDeviceId = 0;
	static int sm_CameraPreviewSizeId = 0;
	static int sm_CameraTakePictureSizeId = 0;
		
	// for Sprite
	static SampleSprite sm_SampleSprite = null;
	static Texture2D sm_Texture2D = null;
		
	// Buffer for Sprite (Atmic)
	static object sm_LockObjectForBuffer = new object();
	static byte[] sm_Buffer = null;
	static byte[] Buffer
	{
		get
		{
			lock (sm_LockObjectForBuffer)
			{
				return sm_Buffer;
			}
		}
		set
		{
			lock (sm_LockObjectForBuffer)
			{
				sm_Buffer = value;
			}
		}
	}
		
	// Dirty Flag for Buffer (Atmic)
	static object sm_LockObjectForIsDirtyBuffer = new object();
	static bool sm_IsDirtyBuffer = false;
	static bool IsDirtyBuffer
	{
		get
		{
			lock (sm_LockObjectForIsDirtyBuffer)
			{
				return sm_IsDirtyBuffer;
			}
		}
		set
		{
			lock (sm_LockObjectForIsDirtyBuffer)
			{
				sm_IsDirtyBuffer = value;
			}
		}
	}
		
	// Frame Counter (Atmic)
	static object sm_LockObjectForFrameCount = new object();
	static long sm_FrameCount;
	static long FrameCount
	{
		get
		{
			lock (sm_LockObjectForFrameCount)
			{
				return sm_FrameCount;
			}
		}
		set
		{
			lock (sm_LockObjectForFrameCount)
			{
				sm_FrameCount = value;
			}
		}
	}
	
	// Performance Counter
	static FrameCounter sm_RenderFrameCounter = new FrameCounter();
	static FrameCounter sm_CallbackFrameCounter = new FrameCounter();
		
	////////////////////////////////////////////////////////////////
		
	static void Main(string[] args)
	{
		Init();

		while (sm_IsLoop)
		{
			SystemEvents.CheckEvents();
			Update();
			Render();
		}

		Term();
	}
		
	static void Init()
	{
		InitGraphicsContext();
		InitSampleDraw();
		InitCameraDevice();
		InitTexture2D();
		InitSampleSprite();
	}
	
	static void Term()
	{
		TermSampleSprite();
		TermTexture2D();
		TermCameraDevice();
		TermSampleDraw();	
		TermGraphicsContext();
	}
		
	static void Update()
	{
		if (0 >= sm_CameraDeviceCount) { return; }
			
		SampleDraw.Update();
		
		var gamePadData = GamePad.GetData(0);
		var pushedButtons = (gamePadData.Buttons & (~gamePadData.ButtonsPrev));
		foreach (GamePadButtons button in Enum.GetValues(typeof(GamePadButtons)))
		{
			if (0 == (pushedButtons & button)) { continue; }
			OnPushDownGamePadButton(button);
		}
			
		UpdateTexture2D();
	}
		
	static void Render()
	{
		if (null == sm_GraphicsContext) { return; }
		
		sm_RenderFrameCounter.UpdateFrame();
		
		RenderBackground();
		RenderSprite();
		RenderText();

		sm_GraphicsContext.SwapBuffers();
	}
		
	////////////////////////////////////////////////////////////////
		
	static void RenderBackground()
	{
		if (null == sm_GraphicsContext) { return; }
			
		sm_GraphicsContext.SetClearColor(0.0f, 0.0f, 0.0f, 1.0f);
		sm_GraphicsContext.Clear();
	}
		
	static void RenderSprite()
	{
		if (null == sm_GraphicsContext) { return; }
		if (null == sm_SampleSprite) { return; }
			
		SampleDraw.DrawSprite(sm_SampleSprite);
	}
		
	static void RenderText()
	{
		if (null == sm_GraphicsContext) { return; }
		if (!sm_IsRenderText) { return; }
			
		const uint colorText = 0xFFFFFFFF;
			
		if (0 >= sm_CameraDeviceCount)
		{
			var rectScreen = sm_GraphicsContext.Screen.Rectangle;
			var x = rectScreen.Width / 2 - 50;
			var y = rectScreen.Height / 2 - 10;
			SampleDraw.DrawText("No Camera", colorText, x, y);
				
			return;
		}
			
		{
			var text = "Display Framerate : " + sm_RenderFrameCounter.FrameRate.ToString("f") + "fps";
			SampleDraw.DrawText(text, colorText, 0, 0);
		}
			
		{
			var text = "Capture Framerate : " + sm_CallbackFrameCounter.FrameRate.ToString("f") + "fps";
			SampleDraw.DrawText(text, colorText, 0, 20);
		}
			
		{
			var text = "Frame Count: " + FrameCount;
			SampleDraw.DrawText(text, colorText, 0, 40);
		}
		
		{
			var cameraState = ((null != sm_Camera) ? sm_Camera.CameraState : CameraState.Closed);
			var text = "Camera State : " + cameraState;
			SampleDraw.DrawText(text, colorText, 0, 60);
		}
			
		{
			var filename = String.IsNullOrEmpty(sm_PictureFilename) ? "" : sm_PictureFilename;
			var text = "Taken Picture File : " + filename;
			SampleDraw.DrawText(text, colorText, 0, 80);
		}
			
		{
			var width = ((null != sm_Camera) ? sm_Camera.CurrentPreviewSize.Width : 0);
			var height = ((null != sm_Camera) ? sm_Camera.CurrentPreviewSize.Height : 0);
			var text = "Current Preview Size : " + width + " x " + height;
			SampleDraw.DrawText(text, colorText, 0, 100);
		}
			
		{
			var cameraTakePictureSize = GetCameraTakePictureSize();
			var width = (cameraTakePictureSize.HasValue ? cameraTakePictureSize.Value.Width : 0);
			var height = (cameraTakePictureSize.HasValue ? cameraTakePictureSize.Value.Height : 0);
			var text = "Current Picture Size : " + width + " x " + height;
			SampleDraw.DrawText(text, colorText, 0, 120);
		}
	}
		
	////////////////////////////////////////////////////////////////
		
	static void InitGraphicsContext()
	{
		if (null != sm_GraphicsContext) { return; }
			
		sm_GraphicsContext = new GraphicsContext();
	}
		
	static void TermGraphicsContext()
	{
		if (null == sm_GraphicsContext) { return; }
			
		sm_GraphicsContext.Dispose();
		sm_GraphicsContext = null;
	}
		
	////////////////////////////////////////////////////////////////
		
	static void InitSampleDraw()
	{
		if (null == sm_GraphicsContext) { return; }
			
		SampleDraw.Init(sm_GraphicsContext);
	}
		
	static void TermSampleDraw()
	{	
		SampleDraw.Term();
	}
	
	////////////////////////////////////////////////////////////////
		
	static void InitCameraDevice()
	{
		if (null != sm_Camera) { return; }
			
		sm_CameraDeviceCount = Camera.GetNumberOfCameras();
		if (0 >= sm_CameraDeviceCount) { return; }

		sm_Camera = new Camera(sm_CameraDeviceId);
		sm_Camera.FrameChanged +=
			(object sender, Camera.FrameChangedEventArgs e) =>
			{
				if (IsDirtyBuffer) { return; }
				
				Buffer =
					REDUCE_TILT ?
					e.FrameBuffer.Clone() as byte[] :
					e.FrameBuffer;
				
				IsDirtyBuffer = true;
				FrameCount = e.FrameCount;
				
				sm_CallbackFrameCounter.UpdateFrame();
			};
		sm_Camera.PictureStateChanged +=
			(object sender, Camera.PictureStateChangedEventArgs e) =>
			{
				if (PictureState.Finishied == e.PictureState)
				{
					sm_PictureFilename = sm_Camera.PictureFilename;
				}
			};
	}
	
	static void TermCameraDevice()
	{
		if (null == sm_Camera) { return; }
		
		CloseCameraDevice();
			
		sm_Camera.Dispose();
		sm_Camera = null;
	}
		
	static void OpenCameraDevice()
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
			
		sm_Camera.Open(sm_CameraPreviewSizeId);

		InitTexture2D();
		InitSampleSprite();
	}
		
	static void CloseCameraDevice()
	{
		if (null == sm_Camera) { return; }
		
		StopCameraDevice();
		sm_Camera.Close();
	}
		
	static void StartCameraDevice()
	{	
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera)
		{
			InitCameraDevice();
		}
				
		if (CameraState.Closed == sm_Camera.CameraState)
		{
			OpenCameraDevice();
		}
		
		sm_Camera.Start();
	}
		
	static void StopCameraDevice()
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
		if (CameraState.TakingPicture == sm_Camera.CameraState) { return; }
			
		sm_Camera.Stop();
	}
		
	static void TakePictureCameraDevice()
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
		if (CameraState.TakingPicture == sm_Camera.CameraState) { return; }
			
		try
		{
			sm_Camera.TakePicture(sm_CameraTakePictureSizeId);
		}
		catch (Exception) { } // nothing to do
	}

	static void ToggleCameraPreviewSizeId(bool isIncrement)
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
		if (CameraState.TakingPicture == sm_Camera.CameraState) { return; }
			
		var cameraInfo = Camera.GetCameraInfo(sm_CameraDeviceId);
		sm_CameraPreviewSizeId =
			GetToggledNumber(
				sm_CameraPreviewSizeId,
				cameraInfo.SupportedPreviewSizes.Count,
				isIncrement);
	
		bool isRunning = (CameraState.Running == sm_Camera.CameraState);
		
		CloseCameraDevice();
		OpenCameraDevice();
					
		if (isRunning)
		{
			StartCameraDevice();
		}
	}
		
	static void ToggleCameraTakePictureSizeId(bool isIncrement)
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
		if (CameraState.TakingPicture == sm_Camera.CameraState) { return; }
			
		var cameraInfo = Camera.GetCameraInfo(sm_CameraDeviceId);
		sm_CameraTakePictureSizeId =
			GetToggledNumber(
				sm_CameraTakePictureSizeId,
				cameraInfo.SupportedPictureSizes.Count,
				isIncrement);
	}
		
	static void ToggleCameraDeviceId()
	{
		if (0 >= sm_CameraDeviceCount) { return; }
		if (null == sm_Camera) { return; }
		if (CameraState.TakingPicture == sm_Camera.CameraState) { return; }
	
		sm_CameraDeviceId = (sm_CameraDeviceId + 1) % Camera.GetNumberOfCameras();
		sm_CameraPreviewSizeId = 0;
		sm_CameraTakePictureSizeId = 0;
		
		bool isRunning = (sm_Camera.CameraState == CameraState.Running);
	
		TermCameraDevice();
		InitCameraDevice();
	
		if (isRunning)
		{
			OpenCameraDevice();
			StartCameraDevice();
		}
	}
		
	////////////////////////////////////////////////////////////////
		
	static void InitTexture2D()
	{
		if (null == sm_Camera) { return; }
			
		TermTexture2D();
			
		var width = sm_Camera.CurrentPreviewSize.Width;
		var height = sm_Camera.CurrentPreviewSize.Height;
		if (0 >= width || 0 >= height) { return; }
			
		var formatPreview = sm_Camera.CurrentPreviewImageFormat;
		var formatTexture = (CameraImageFormat.Rgba8888 == formatPreview) ? PixelFormat.Rgba : PixelFormat.Rgb565;
			
		sm_Texture2D =
			new Texture2D(
				width,
				height,
				false,
				formatTexture,
				PixelBufferOption.Renderable);
		
		// Rgba8888 : 4 bytes per pixel
		// Rgb565   : 2 bytes per pixel
		var bytesPerPixel = (CameraImageFormat.Rgba8888 == formatPreview) ? 4 : 2;
		var bytesCount = width * height * bytesPerPixel;
			
		Buffer = new byte[bytesCount];
		sm_Texture2D.SetPixels(0, Buffer);
	}
		
	static void TermTexture2D()
	{
		if (null == sm_Texture2D) { return; }
			
		sm_Texture2D.Dispose();
		sm_Texture2D = null;
	}
		
	static void UpdateTexture2D()
	{
		if (null == sm_Texture2D) { return; }
		if (null == Buffer) { return; }
		if (!IsDirtyBuffer) { return; }
	
		sm_Texture2D.SetPixels(0, Buffer);
			
		IsDirtyBuffer = false;
	}
		
	////////////////////////////////////////////////////////////////
		
	static void InitSampleSprite()
	{
		if (null == sm_Texture2D) { return; }
		if (0 >= sm_Texture2D.Width || 0 >= sm_Texture2D.Height) { return; }
			
		TermSampleSprite();
		
		var positionX = (SampleDraw.Width - sm_Texture2D.Width) / 2;
		var positionY = (SampleDraw.Height - sm_Texture2D.Height) / 2;
		var scale =
			Math.Min(
				(float)SampleDraw.Width / sm_Texture2D.Width,
				(float)SampleDraw.Height / sm_Texture2D.Height);
				
		sm_SampleSprite = new SampleSprite(sm_Texture2D, positionX, positionY, 0, scale);
	}
		
	static void TermSampleSprite()
	{
		if (null == sm_SampleSprite) { return; }
			
		sm_SampleSprite.Dispose();
		sm_SampleSprite = null;
	}
		
	////////////////////////////////////////////////////////////////
		
	static void OnPushDownGamePadButton(GamePadButtons button)
	{
		switch (button)
		{
			case GamePadButtons.Left:
			case GamePadButtons.Right:
				ToggleCameraPreviewSizeId(GamePadButtons.Left == button);
				break;
				
			case GamePadButtons.Up:
			case GamePadButtons.Down:
				ToggleCameraTakePictureSizeId(GamePadButtons.Up == button);
				break;
				
			case GamePadButtons.Square:
				sm_IsRenderText= !sm_IsRenderText;
				break;
				
			case GamePadButtons.Triangle:
				StopCameraDevice();
				break;
				
			case GamePadButtons.Circle:
				StartCameraDevice();
				break;
				
			case GamePadButtons.Cross:
				CloseCameraDevice();
				break;
				
			case GamePadButtons.L:
				ToggleCameraDeviceId();
				break;
				
			case GamePadButtons.R:
				TakePictureCameraDevice();
				break;
				
			default:
				break;
		}
	}
		
	static CameraSize? GetCameraTakePictureSize()
	{
		if (null == sm_Camera) { return null; }
			
		var cameraInfo = Camera.GetCameraInfo(sm_CameraDeviceId);
		return cameraInfo.SupportedPictureSizes[sm_CameraTakePictureSizeId];
	}
		
	static int GetToggledNumber(int num, int size, bool isIncrement)
	{
		return Math.Max((size + num + (isIncrement ? 1 : -1)) % size, 0);
	}
}
	
////////////////////////////////////////////////////////////////
	
public class FrameCounter
{
	private DateTime m_BaseTime = DateTime.Now;
	private ulong m_Count = 0;
	
	private object m_LockObjectForFrameRate = new object();
	private float m_FrameRate;
	public float FrameRate
	{
		get
		{
			lock (m_LockObjectForFrameRate)
			{
				return m_FrameRate;
			}
		}
		private set
		{
			lock (m_LockObjectForFrameRate)
			{
				m_FrameRate = value;
			}
		}
	}

	public void UpdateFrame()
	{
		++m_Count;
		var now = DateTime.Now;
		var passedTime = now.Ticks - m_BaseTime.Ticks;
		if (2000000 <= passedTime)
		{
			FrameRate = (float)m_Count * 10000000 / passedTime;
			m_BaseTime = now;
			m_Count = 0;
		}
	}
}

} // Sample
