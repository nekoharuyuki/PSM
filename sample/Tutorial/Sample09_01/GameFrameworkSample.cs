/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Audio;

using Sce.PlayStation.Framework;
using Tutorial.Utility;

using System.Xml.Linq;
using System.IO;

namespace Sample
{
	public class GameFrameworkSample: GameFramework
	{
		public ImageRect rectScreen;
		
		public Int32 appCounter=0;
		
		public Actor Root{ get; set;}
		
		bool m_pause=false;
		int forwardFrame = 0;
		
		
		public Texture2D textureUnified;
		
		public Dictionary<string, UnifiedTextureInfo> dicTextureInfo;
		
		//public SpriteBuffer spriteBuffer;
		public PackedIndexedSprite piSprite;
		
		public const int sizeofSprite=1024;
		
		
		public override void Initialize()
		{
			base.Initialize();
			
			rectScreen = graphics.GetViewport();

			//spriteBuffer=new SpriteBuffer(graphics, sizeofSprite);
			piSprite = new PackedIndexedSprite(graphics);
			
			//@e Process for unified texture.
			//@j 一体化テクスチャの処理。
			dicTextureInfo = UnifiedTexture.GetDictionaryTextureInfo("/Application/image/unified_texture.xml");
			textureUnified=new Texture2D("/Application/image/unified_texture.png", false);
			
			//@e Initialization of actor tree
			//@j アクターツリーの初期化。
			Root = new Actor("root");
			
			Root.AddChild(new Player(this, "Player"));
			
			Root.AddChild(new Map(this,"Map"));
			
			Root.AddChild(new Actor("bulletManager"));
		}
		
		
		public override void Update()
		{
			base.Update();
			
#if DEBUG
			debugString.WriteLine("Buttons="+PadData.Buttons);
			
			//@e Pause mode by press start button.
			//@j スタートボタンでポーズする。
			if((this.PadData.ButtonsDown & (GamePadButtons.Start)) != 0)
			{
				m_pause = m_pause ? false : true;
			}

			//@j コマ送り。ポーズ中、セレクトボタンが押されたら、1フレーム進める。
			if(m_pause==true && (this.PadData.ButtonsDown & (GamePadButtons.Select)) != 0)
			{
				forwardFrame =1;	
			}
#endif			
			
			if( forwardFrame > 0 || m_pause == false)
			{
				Root.Update();

				++appCounter;
				
				if( forwardFrame > 0)
					forwardFrame--;
			}
			else
			{
				//@j Pause.
				//@j ポーズ中
				debugString.WriteLine("Pause");
			}
		}
		
		
		public override void Render()
		{
			//graphics.SetClearColor(0.5f, 0.5f, 1.0f, 0.0f);
			graphics.SetClearColor(0.0f, 0.0f, 0.0f, 0.0f);
			graphics.Clear();
			
			
			//@e No depth test.
			//@j 深度テストなし。
			//graphics.Disable(EnableMode.DepthTest);
			
			//@e Depth test.
			//@j 不透明/深度テストあり。
			//graphics.Enable(EnableMode.DepthTest);
			
			//@e Semitransparent.
			//@j 半透明。
			//graphics.Disable(EnableMode.DepthTest);
			//graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			//@e Add-transparent.
			//@j 加算半透明
			graphics.Disable(EnableMode.DepthTest);
			graphics.SetBlendFunc( BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One ) ;
			
			graphics.SetTexture(0, this.textureUnified);
			
			//spriteBuffer.Clear();
			piSprite.Clear();
			
			Root.Render();
			
			//spriteBuffer.sortMode= SpriteBuffer.SortMode.NoSort;
			//spriteBuffer.Render();
			piSprite.Update();
			piSprite.Render();
			
			//debugString.WriteLine("Sprite Cnt="+spriteBuffer.GetNumOfSprite());
			debugString.WriteLine("Sprite Cnt="+piSprite.GetNumOfSprite());
			
			base.Render();
			
			Root.CheckStatus();
		}
		
		public override void Terminate ()
		{
			textureUnified.Dispose();
			
			base.Terminate ();
		}
	}
}
