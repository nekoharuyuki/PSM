using System;
using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using  Tutorial.Utility;


/// <summary>POSITIONとUVをパック & index</summary>
public class PackedIndexedSprite
{

	// 頂点。
	protected struct Vertex
	{
		public Short2 Position; //4byte
		public Short2 UV;		//4byte
		public UByte4N Color;	//4byte
	}
	
	// スプライト。
	protected struct SpritePrim
	{
		public Vertex V0;
		public Vertex V1;
		public Vertex V2;
		public Vertex V3;
	}

	private SpritePrim[] m_spritePrimBuffer;
	
	
	private UByte4N _color = new UByte4N(1.0f,1.0f,1.0f,1.0f);
	
	//追加。
	protected GraphicsContext m_graphics;
	protected ShaderProgram m_ShaderProgram;
	protected VertexBuffer[] m_vertexBuffer = new VertexBuffer[2];
	
	protected readonly int  m_maxNumOfSprite=1000;

	protected int[] m_primCount = new int[2]{0,0};
	protected int m_index = 0;
	protected Matrix4 m_wvp;

	
	List<SpriteB> spriteList=new List<SpriteB>();
	Int32 cnt=0;
	
	
	
	public PackedIndexedSprite(GraphicsContext graphics, Int32 maxNumOfSprite=1000)
	{
		m_graphics = graphics;
		m_ShaderProgram = CreatePackedSpriteShader();
		m_maxNumOfSprite = maxNumOfSprite;

		int width = m_graphics.GetFrameBuffer().Width;
		int height = m_graphics.GetFrameBuffer().Height;		
		Matrix4 proj = Matrix4.Ortho(
			0, width,
			0, height,
			0, 32768.0f
			);
		
		Matrix4 view = Matrix4.LookAt(
				new Vector3(0, height, 0),
				new Vector3(0, height, 1),
				new Vector3(0, -1, 0));
		m_wvp = proj * view;
		
		
		// 頂点バッファを2つ作成。
		for(int i = 0; i < m_vertexBuffer.Length; ++i){
			m_vertexBuffer[i] = new VertexBuffer(
				4*m_maxNumOfSprite,
				6*m_maxNumOfSprite,
				VertexFormat.Short4,
				VertexFormat.UByte4N
				);
		}

		// index
		{
			int numIndices = 6 * m_maxNumOfSprite;
			ushort[] indices = new ushort[numIndices];
			for(int i = 0, wp = 0; i < m_maxNumOfSprite; ++i)
			{
				int vnum = i*4;

				indices[wp++] = (ushort)(0+vnum);
				indices[wp++] = (ushort)(1+vnum);
				indices[wp++] = (ushort)(2+vnum);

				indices[wp++] = (ushort)(1+vnum);
				indices[wp++] = (ushort)(3+vnum);
				indices[wp++] = (ushort)(2+vnum);
			}
			m_vertexBuffer[0].SetIndices(indices);
			m_vertexBuffer[1].SetIndices(indices);

		}

		m_spritePrimBuffer = new SpritePrim[m_maxNumOfSprite];
		
	}

	public void Clear()
	{
		spriteList.Clear();
		cnt=0;
	}
	
	public int GetNumOfSprite()
	{
		return cnt;	
	}
	
	public bool IsFull()
	{
		if(cnt>=m_maxNumOfSprite)
			return true;
		else 
			return false;
	}
	
	public void Add(SpriteB spriteB)
	{
		if(cnt >=  m_maxNumOfSprite)
			return;
		
		this.spriteList.Add(spriteB);
		++cnt;
	}
	

	public void Update()
	{
		// 頂点バッファ更新
		for(int i = 0; i < spriteList.Count; ++i){
			SetSpritePrimitive(out m_spritePrimBuffer[i], spriteList[i]);
		}

		m_primCount[m_index] = spriteList.Count;

		// 必要な分だけ転送。
		m_vertexBuffer[m_index].SetVertices(m_spritePrimBuffer,0,0,m_primCount[m_index]*4);
	}


	public void Render()
	{
		m_ShaderProgram.SetUniformValue(0,ref m_wvp);
			
		m_graphics.SetShaderProgram(m_ShaderProgram);
		
		m_graphics.SetVertexBuffer(0,m_vertexBuffer[m_index]);
		m_graphics.DrawArrays(
			DrawMode.Triangles,
			0,
			m_primCount[m_index]*6
			);
	}

	private void SetSpritePrimitive(
		out SpritePrim spritePrim,
		SpriteB spriteB
		)
	{
		
		
		spritePrim.V0.Color = _color;
		spritePrim.V1.Color = _color;
		spritePrim.V2.Color = _color;
		spritePrim.V3.Color = _color;
	
		spritePrim.V0.UV = spriteB._uv[0];
		spritePrim.V1.UV = spriteB._uv[1];
		spritePrim.V2.UV = spriteB._uv[2];
		spritePrim.V3.UV = spriteB._uv[3];
		
		if(spriteB.Rotation == 0.0f)
		{
			spritePrim.V0.Position.X = (short)(spriteB.Position.X - spriteB.Width*spriteB.Center.X);//0.0f;	// x0
			spritePrim.V0.Position.Y = (short)(spriteB.Position.Y - spriteB.Height*spriteB.Center.Y);//0.0f;	// y0
			
			spritePrim.V1.Position.X = (short)(spriteB.Position.X - spriteB.Width*spriteB.Center.X);//0.0f;	// x1
			spritePrim.V1.Position.Y = (short)(spriteB.Position.Y + spriteB.Height*(1.0f-spriteB.Center.Y));//1.0f;	// y1
	
			spritePrim.V2.Position.X = (short)(spriteB.Position.X + spriteB.Width*(1.0f-spriteB.Center.X));//1.0f;	// x2
			spritePrim.V2.Position.Y = (short)(spriteB.Position.Y - spriteB.Height*spriteB.Center.Y);//0.0f;	// y2
			
			spritePrim.V3.Position.X = (short)(spriteB.Position.X + spriteB.Width*(1.0f-spriteB.Center.X));//1.0f;	// x3
			spritePrim.V3.Position.Y = (short)(spriteB.Position.Y + spriteB.Height*(1.0f-spriteB.Center.Y));//1.0f;	// y3
			
		}
		else
		{
			float x,y, rc, rs;

			rc=FMath.Cos(spriteB.Rotation);
			rs=FMath.Sin(spriteB.Rotation);
			
			x=-spriteB.Width*spriteB.Center.X;
			y=-spriteB.Height*spriteB.Center.Y;
			spritePrim.V0.Position.X=(short)(spriteB.Position.X + (float)(x*rc - y*rs));// x0
			spritePrim.V0.Position.Y=(short)(spriteB.Position.Y + (float)(x*rs + y*rc));// y0

			
			x=-spriteB.Width*spriteB.Center.X;
			y=spriteB.Height*(1.0f-spriteB.Center.Y);
			spritePrim.V1.Position.X=(short)(spriteB.Position.X + (float)(x*rc - y*rs));// x1
			spritePrim.V1.Position.Y=(short)(spriteB.Position.Y + (float)(x*rs + y*rc));// y1
			

			x=spriteB.Width*(1.0f-spriteB.Center.X);
			y=-spriteB.Height*spriteB.Center.Y;
			spritePrim.V2.Position.X=(short)(spriteB.Position.X + (float)(x*rc-y*rs));// x2
			spritePrim.V2.Position.Y=(short)(spriteB.Position.Y + (float)(x*rs+y*rc));// y2
			

			x=spriteB.Width*(1.0f-spriteB.Center.X);
			y=spriteB.Height*(1.0f-spriteB.Center.Y);
			spritePrim.V3.Position.X=(short)(spriteB.Position.X + (float)(x*rc-y*rs));// x3
			spritePrim.V3.Position.Y=(short)(spriteB.Position.Y + (float)(x*rs+y*rc));// y3
		}
	}
	
	//@e Initialization of shader program
	//@j シェーダープログラムの初期化。
	private static ShaderProgram CreatePackedSpriteShader()
	{
		string resourceName = "TutoLib.shaders.PackedSprite.cgx";
		
		Byte[] dataBuffer = Utility.ReadEmbeddedFile(resourceName);
		
		ShaderProgram shaderProgram = new ShaderProgram(dataBuffer);

		shaderProgram.SetAttributeBinding(0,"a_Position");
		shaderProgram.SetAttributeBinding(1,"a_Color");
		shaderProgram.SetUniformBinding(0,"u_WVP");
		
		return shaderProgram;
	}
	
}

