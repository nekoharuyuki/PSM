
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Tutorial.Utility
{

	public class UnifiedTextureInfo
	{
		/// <summary>UV coordinate. 0.0f~1.0f</summary>
		public float u0;
		public float v0;
		public float u1;
		public float v1;
		/// <summary>width of this texture.</summary>
		public float w;
		/// <summary>height of this texture.</summary>
		public float h;
		
		public UnifiedTextureInfo(float u0, float v0, float u1, float v1, float w, float h)
		{
			this.u0=u0;
			this.v0=v0;
			this.u1=u1;
			this.v1=v1;
			this.w=w;
			this.h=h;
		}
		
		
		/// <summary>このテクスチャ内にあるタイルのテクスチャ情報を、インデックス形式で取得する。</summary>
		/// <returns>引数のインデックス形式で指定したタイルのテクスチャ情報。</returns>
		/// <param name='indexX'>タイルのXインデックス。</param>
		/// <param name='indexY'>タイルのYインデックス。</param>
		/// <param name='numOfX'>タイル全体のX個数。</param>
		/// <param name='numOfY'>タイル全体のY個数。</param>
		public UnifiedTextureInfo GetTileTextureInfo(Int32 indexX, Int32 indexY, Int32 numOfX, Int32 numOfY)
		{
			
			if(indexX < 0 || indexY < 0)
			{
				throw new Exception("Index of argument is invalid.  Index must be greater than or equal to 0 . -  GetTileTextureInfo()");
			}
			
			if(numOfX <= 0 || numOfY <=0 )
			{
				throw new Exception("numOfXY of argument is invalid. numOfXY must be greater than 0. -  GetTileTextureInfo()");
			}
			
			if( numOfX <= indexX ||  numOfY <= indexY)
			{
				throw new Exception("Index of argument  is out of numOfXY.  -  GetTileTextureInfo()");
			}

			
			float tileW,tileH;
			tileW=this.w/numOfX;
			tileH=this.h/numOfY;
			
			float tileU0, tileV0, tileU1, tileV1;
				
			float tileUW = (this.u1-this.u0)/numOfX;
			float tileVH = (this.v1-this.v0)/numOfY;
			
			tileU0 = this.u0 + tileUW*indexX;
			tileV0 = this.v0 + tileVH*indexY;

			tileU1 = tileU0 + tileUW;
			tileV1 = tileV0 + tileVH;
			
			return new UnifiedTextureInfo(tileU0, tileV0, tileU1, tileV1, tileW, tileH);
		}
	}
	
	public class UnifiedTexture
	{
		/// <summary>
		/// Gets the dictionary texture info.
		/// </summary>
		/// <returns>
		/// The dictionary of texture info.
		/// </returns>
		/// <param name='xmlFilename'>
		/// Xml filename.
		/// </param>
		static public Dictionary<string, UnifiedTextureInfo>　GetDictionaryTextureInfo(string xmlFilename)
		{
			Dictionary<string, UnifiedTextureInfo> dicTextureInfo = new Dictionary<string, UnifiedTextureInfo>();
			
			try
			{
				XDocument doc = XDocument.Load(xmlFilename);
				
				
				XElement root= doc.Element("root");
				
				//@e Get the size of the whole texture.
				//@j テクスチャ全体の大きさを取得する。
				var elementInfo = root.Element("info");
				float textureWidth=float.Parse(elementInfo.Attribute("w").Value);
				float textureHeight=float.Parse(elementInfo.Attribute("h").Value);
				
				Console.WriteLine(textureWidth+","+textureHeight);
				
				float x,y,w,h;
				
				foreach( var element in root.Descendants("texture"))
				{
					Console.WriteLine("texture "+element.Attribute("filename").Value);
					
					x=float.Parse(element.Attribute("x").Value);
					y=float.Parse(element.Attribute("y").Value);
					w=float.Parse(element.Attribute("w").Value);
					h=float.Parse(element.Attribute("h").Value);

					// Convert to UV.
					dicTextureInfo.Add(element.Attribute("filename").Value, 
					    new UnifiedTextureInfo(
							x/textureWidth,	y/textureHeight,
							(x+w)/textureWidth,	(y+h)/textureHeight,
							w,	h
						)
					 );
				}
			}
			catch (Exception e)
			{
			    Console.Error.WriteLine(e.Message);
			    Environment.Exit(1);
			}
			
			return dicTextureInfo;
		}
	}

}

