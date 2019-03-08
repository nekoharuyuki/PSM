/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// Given a Font object and a text containing all the characters you intend to use, 
	/// FontMap creates a Texture2D object containg the characters and its corresponding 
	/// table of UVs. This data is used by GameEngine2D in various text rendering functions.
	///
	/// Examples:
	/// 
	///  new FontMap( new Font( "D:\\Blah\\YourFont.TTF", 32, FontStyle.Bold ), 512 );
	/// 
	///  new FontMap( new Font( FontAlias.System, 32, FontStyle.Bold ), 512 );
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// Fontオブジェクトと使用予定のすべての文字を含むテキストを指定して、フォントマップは、文字を格納したTexture2Dオブジェクトと対応するUVテーブルを作成します。このデータは、GameEngine2Dのさまざまなテキスト描画関数で使用されます。
	/// </summary>
	/// @endif
	public class FontMap : System.IDisposable
	{
		/// @if LANG_EN
		/// <summary>The UV data for a single character.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>単一文字のUVデータ。
		/// </summary>
		/// @endif
		public struct CharData
		{
			/// @if LANG_EN
			/// <summary>UV in FontMap's Texture.</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>フォントマップのテクスチャ内のUV。
			/// </summary>
			/// @endif
			public Bounds2 UV; 
			
			/// @if LANG_EN
			/// <summary>The pixel size for this character (depends on the font.)</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>この文字のピクセルサイズ (フォントに依存します)。
			/// </summary>
			/// @endif
			public Vector2 PixelSize; 
		};

		/// @if LANG_EN
		/// <summary>The font texture containing all the characters.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>全ての文字を含む、フォントテクスチャ。
		/// </summary>
		/// @endif
		public Texture2D Texture;

		/// @if LANG_EN
		/// <summary>Map characters to their corresponding CharData (UV and size data).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>文字に対応するCharData (UVとサイズデータ)。
		/// </summary>
		/// @endif
		public Dictionary< char, CharData > CharSet; 

		/// @if LANG_EN
		/// <summary>Character height in pixels - all characters have the same pixel height.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセル単位のモズの高さ。すべての文字は同じ高さです。
		/// </summary>
		/// @endif
		public float CharPixelHeight;

		/// @if LANG_EN
		/// <summary>The ascii character set as a string.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アスキー文字を文字列としてセットします。
		/// </summary>
		/// @endif
		static public string AsciiCharSet = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

		bool m_disposed = false;
		/// @if LANG_EN
		/// <summary>Return true if this object been disposed.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このオブジェクトが破棄されていれば、trueを返します。
		/// </summary>
		/// @endif
		public bool Disposed { get { return m_disposed; } }

		CharData[] m_ascii_char_data; // shortcut in CharSet, for readable ASCII characters
		bool[] m_ascii_char_data_valid;

		/// @if LANG_EN
		/// <summary>
		/// Create a FontMap for the ASCII char set.
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="fontmap_width">The internal width used by the texture. Height is adjusted automatically so that all characters fit.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>アスキー文字のフォントマップを作成します。
		/// </summary>
		/// <param name="font">文字を描画するための使用するフォント。フォントマップは、このFontオブジェクトを破棄することに注意してください。</param>
		/// <param name="fontmap_width">テクスチャで使用される内部の幅。高さはすべての文字が収まるように自動的に調整されます。</param>
		/// @endif
		public FontMap( Font font, int fontmap_width = 512 )
		{
			Initialize( font, AsciiCharSet, fontmap_width );
		}

		/// @if LANG_EN
		/// <summary>
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="charset">A string containing all the characters you will ever need when drawing text with this FontMap.</param>
		/// <param name="fontmap_width">The internal with used by the texture (height is adjusted automatically).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// <param name="font">文字を描画するための使用するフォント。フォントマップは、このFontオブジェクトを破棄することに注意してください。</param>
		/// <param name="charset">このFontMapでテキストを描画するときに必要な全ての文字を含む文字列。</param>
		/// <param name="fontmap_width">テクスチャによって使用される内部的な値 (高さは自動で調整されます)。</param>
		/// @endif
		public FontMap( Font font, string charset, int fontmap_width = 512 )
		{
			Initialize( font, charset, fontmap_width );
		}

		/// @if LANG_EN
		/// <summary>
		/// Dispose implementation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>破棄します。
		/// </summary>
		/// @endif
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				Common.DisposeAndNullify< Texture2D >( ref Texture );
				m_disposed = true;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="charset">A string containing all the characters you will ever need when drawing text with this FontMap.</param>
		/// <param name="fontmap_width">The internal with used by the texture (height is adjusted automatically).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// <param name="font">文字を描画するための使用するフォント。フォントマップは、このFontオブジェクトを破棄することに注意してください。</param>
		/// <param name="charset">このFontMapでテキストを描画するときに必要な全ての文字を含む文字列。</param>
		/// <param name="fontmap_width">テクスチャによって使用される内部的な値 (高さは自動で調整されます)。</param>
		/// @endif
		public void Initialize( Font font, string charset, int fontmap_width = 512 )
		{
			CharSet = new Dictionary< char, CharData >();

			CharPixelHeight = font.Metrics.Height;

			Image image = null;
			Vector2i totalsize = new Vector2i( 0, 0 );

			for ( int k=0; k < 2; ++k )
			{
				Vector2i turtle = new Vector2i( 0, 0 );	// turtle is in Sce.PlayStation.Core.Imaging.Font's coordinate system
				int max_height = 0;

				for ( int i=0; i < charset.Length; ++i )
				{
					if ( CharSet.ContainsKey( charset[i] ) )
						continue; // this character is already in the map

					Vector2i char_size = new Vector2i( font.GetTextWidth( charset[i].ToString(), 0, 1 ) 
													   , font.Metrics.Height );

					max_height = Common.Max( max_height, char_size.Y );

					if ( turtle.X + char_size.X > fontmap_width )
					{
						// hit the right side, go to next line
						turtle.X = 0;
						turtle.Y += max_height;	// Sce.PlayStation.Core.Imaging.Font's coordinate system: top is 0, so we += to move down
						max_height = 0;

						// make sure we are noit going to newline forever due to lack of fontmap_width
						Common.Assert( char_size.Y <= fontmap_width ); 
					}

					if ( k > 0 )
					{
						// that starts from top left
						image.DrawText( charset[i].ToString(), new ImageColor(255,255,255,255), font
										, new ImagePosition( turtle.X, turtle.Y ) );

						var uv = new Bounds2( turtle.Vector2() / totalsize.Vector2()
											  , ( turtle + char_size ).Vector2() / totalsize.Vector2() );

						// now fix the UV to be in GameEngine2D's UV coordinate system, where 0,0 is bottom left
						uv = uv.OutrageousYVCoordFlip().OutrageousYTopBottomSwap();

						CharSet.Add( charset[i], new CharData(){ UV = uv, PixelSize = char_size.Vector2()} );
					}

					turtle.X += char_size.X;

					if ( k == 0 )
					{
						totalsize.X = Common.Max( totalsize.X, turtle.X );
						totalsize.Y = Common.Max( totalsize.Y, turtle.Y + max_height );
					}
				}

				if ( k == 0 )
				{
//					System.Console.WriteLine( "FontMap.Initialize: totalsize " + totalsize );
					image = new Image( ImageMode.A, new ImageSize( totalsize.X, totalsize.Y ), new ImageColor(0,0,0,0) );

					CharSet.Clear(); // we want to go through the same add logic on second pass, so clear
				}
			}

			Texture = new Texture2D( image.Size.Width, image.Size.Height, false, PixelFormat.Luminance );
			Texture.SetPixels( 0, image.ToBuffer() );
//			image.Export("uh?","hey.png");
			image.Dispose();

			{
				// cache ascii entries so we can skip TryGetValue logic for those
				m_ascii_char_data = new CharData[ AsciiCharSet.Length ];
				m_ascii_char_data_valid = new bool[ AsciiCharSet.Length ];
				for ( int i=0; i < AsciiCharSet.Length; ++i )
				{
					CharData cdata;
					m_ascii_char_data_valid[i] = CharSet.TryGetValue( AsciiCharSet[i], out cdata );
					m_ascii_char_data[i] = cdata;
				}
			}

			// dispose of the font by default
			font.Dispose();
		}

		/// @if LANG_EN
		/// <summary>
		/// Try to get the CharData needed to draw the character 'c'.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数 'c' の文字を描画するのに必要な CharDataを取得します。
		/// </summary>
		/// @endif
		public bool TryGetCharData( char c, out CharData cdata ) 
		{
			{
				int index = (int)c-(int)' ';
				if ( index >= 0 && index < AsciiCharSet.Length )
				{
					cdata = m_ascii_char_data[index];
					return m_ascii_char_data_valid[index];
				}
			}

			if ( !CharSet.TryGetValue( c, out cdata ) )
			{
				System.Console.WriteLine( "The character [" + c + "] is not present in the FontMap you are trying to use. Please double check the input character set." );
				return false;
			}

			return true;
		}
	}
}

