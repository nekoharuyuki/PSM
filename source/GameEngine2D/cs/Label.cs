/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// Draw text primitive. 2 types of font data supported:
	/// - One using SpriteRenderer's embedded debug font
	/// - The other using a FontMap object
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>テキストのプリミティブを描画します。2つのタイプのフォントデータをサポートしています: 
	/// - SpriteRenderer に埋め込まれたデバッグフォント。
	/// - FontMap オブジェクトを使ったもの。
	/// </summary>
	/// @endif
	public class Label : Node
	{
		/// @if LANG_EN
		/// <summary>The text to display.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>表示するテキスト。
		/// </summary>
		/// @endif
		public string Text = "";
		
		/// @if LANG_EN
		/// <summary>The text color.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>テキストの色。
		/// </summary>
		/// @endif
		public Vector4 Color = Colors.White;
		
		/// @if LANG_EN
		/// <summary>The text blend mode.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>テキストのブレンドモード。
		/// </summary>
		/// @endif
		public BlendMode BlendMode = BlendMode.Normal;

		/// @if LANG_EN
		/// <summary>A scale factor applied to the character's pixel height during rendering.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画中、文字のピクセル単位の高さに適用されるスケール係数。
		/// </summary>
		/// @endif
		public float HeightScale = 1.0f;
		
		/// @if LANG_EN
		/// <summary>The fontmap used to display this Label (the character set has to match).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>この Label を表示するのに使用されるフォントマップ(文字のセットに一致する必要があります)。
		/// </summary>
		/// @endif
		public FontMap FontMap;	

		/// @if LANG_EN
		/// <summary>
		/// User can set an external shader. 
		/// The Label class won't dispose of shaders set by user.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>開発者は、任意のシェーダーをセットすることができます。
		/// Label クラスは開発者がセットしたシェーダーを 破棄しないので注意してください。
		/// </summary>
		/// @endif
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultFontShader;

		/// @if LANG_EN
		/// <summary>The font character height in pixels.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>ピクセル単位でのフォントの文字の高さ。
		/// @endif
		float FontHeight
		{
			get
			{
				if ( FontMap == null ) return EmbeddedDebugFontData.CharSizei.Y;
				return FontMap.CharPixelHeight;
			}
		}

		/// @if LANG_EN
		/// <summary>The character height in world coordinates = FontHeight * HeightScale.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ワールド座標系 = FontHeight * HeightScale での文字の高さ。
		/// </summary>
		/// @endif
		public float CharWorldHeight
		{
			get { return FontHeight * HeightScale; }
		}

		/// @if LANG_EN
		/// <summary>Label constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public Label()
		{
		}

		/// @if LANG_EN
		/// <summary>Label constructor.</summary>
		/// <param name="text">The text to render.</param>
		/// <param name="fontmap">The font data used for rendering the text. If null, an embedded debug font will be used.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="text">描画するテキスト。</param>
		/// <param name="fontmap">テキストの描画に使用するフォントデータ。 null の場合、埋め込まれているデバッグフォントを使用します。
		/// @endif

		public Label( string text, FontMap fontmap = null )
		{
			Text = text;
			FontMap = fontmap;
		}

		/// @if LANG_EN
		/// <summary>The draw function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画関数。
		/// </summary>
		/// @endif
		public override void Draw()
		{
			base.Draw();

			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );

			if ( FontMap == null ) Director.Instance.SpriteRenderer.DrawTextDebug( Text, GameEngine2D.Base.Math._00, CharWorldHeight, true );
			else Director.Instance.SpriteRenderer.DrawTextWithFontMap( Text, GameEngine2D.Base.Math._00, CharWorldHeight, true, FontMap, Shader );
		}

		/// @if LANG_EN
		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ローカル空間での、テキストを囲む Bounds2 オブジェクトを返します。
		/// </summary>
		/// @endif
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// @if LANG_EN
		/// <summary>Return the Bounds2 object containing the text, in local space.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ローカル空間での、テキストを囲む Bounds2 オブジェクトを返します。
		/// </summary>
		/// @endif
		public Bounds2 GetlContentLocalBounds()
		{
			if ( FontMap == null ) return Director.Instance.SpriteRenderer.DrawTextDebug( Text, GameEngine2D.Base.Math._00, CharWorldHeight, false );
			else return Director.Instance.SpriteRenderer.DrawTextWithFontMap( Text, GameEngine2D.Base.Math._00, CharWorldHeight, false, FontMap, Shader );
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

