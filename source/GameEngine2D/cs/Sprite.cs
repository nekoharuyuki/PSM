/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// @if LANG_EN
	/// <summary>
	/// Base class for single sprite nodes.
	/// This is an abstract class.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>単一のスプライトノードの規定クラス。抽象クラスです。
	/// </summary>
	/// @endif
	public abstract class SpriteBase : Node
	{
		/// @if LANG_EN
		/// <summary>
		/// Sprite geometry in the node's local space. 
		/// A TRS defines an oriented rectangle.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ノードのローカル空間内のスプライト幾何情報。TRS は矩形の向きを定義します。</summary>
		/// @endif
		public TRS Quad = TRS.Quad0_1;

		/// @if LANG_EN
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>trueなら、スプライトのUVが水平方向に反転します。
		/// </summary>
		/// @endif
		public bool FlipU = false;

		/// @if LANG_EN
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>trueなら、スプライトのUVが垂直方向に反転します。
		/// </summary>
		/// @endif
		public bool FlipV = false;

		/// @if LANG_EN
		/// <summary>The sprite color.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの色。
		/// </summary>
		/// @endif
		public Vector4 Color = Colors.White;

		/// @if LANG_EN
		/// <summary>The blend mode.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ブレンドモード。
		/// </summary>
		/// @endif
		public BlendMode BlendMode = BlendMode.Normal;

		/// @if LANG_EN
		/// <summary>
		/// This is used only if the Sprite is drawn standalone (not in a SpriteList).
		/// If Sprite is used in a SpriteList, then the SpriteList's TextureInfo is used.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このプロパティは、スプライトが別個 ( つまり SpriteList でない) に描画されるときのみ、使用されます。
		/// スプライトが SpriteList で使用されている場合、後の SpriteList の TextureInfo が使用されます。
		/// </summary>
		/// @endif
		public TextureInfo TextureInfo;

		/// @if LANG_EN
		/// <summary>The shader.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>シェーダー。
		/// </summary>
		/// @endif
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// @if LANG_EN
		/// <summary>Return the dimensions of this sprite in pixels.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセル単位でこのスプライトのサイズを返します。
		/// </summary>
		/// @endif
		abstract public Vector2 CalcSizeInPixels();

		/// @if LANG_EN
		/// <summary>SpriteBase constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public SpriteBase()
		{
		}

		/// @if LANG_EN
		/// <summary>SpriteBase constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public SpriteBase( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// @if LANG_EN
		/// <summary>The draw function (expensive, standalone draw).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画関数 ( 処理が重い、別個の描画 )。
		/// </summary>
		/// @endif
		public override void Draw()
		{
			Common.Assert( TextureInfo != null, "Sprite's TextureInfo is null" );
			Common.Assert( Shader != null, "Sprite's Shader is null" );

//			base.Draw(); // AdHocDraw

			////Common.Profiler.Push("SpriteBase.Draw");

			////Common.Profiler.Push("SpriteBase.Draw prelude");
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			////Common.Profiler.Pop();
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, 1 );
			////Common.Profiler.Push("SpriteBase.internal_draw()");
			internal_draw();
			////Common.Profiler.Pop();
			////Common.Profiler.Push("SpriteBase.Draw end");
			Director.Instance.SpriteRenderer.EndSprites(); 
			////Common.Profiler.Pop();

			////Common.Profiler.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ローカルの境界は、このスプライトのクワッドを含む最小の Bounds2 です。回転がない場合、クワッドそれ自体 ( スプライトの矩形) と同じです。
		/// </summary>
		/// @endif
		public override bool GetlContentLocalBounds( ref Bounds2 bounds )
		{
			bounds = GetlContentLocalBounds();
			return true;
		}

		/// @if LANG_EN
		/// <summary>
		/// The content local bounds is the smallest Bounds2 containing this 
		/// sprite's Quad, and Quad itself (the sprite rectangle) if there is 
		/// no rotation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ローカルの境界は、このスプライトのクワッドを含む最小の Bounds2 です。回転がない場合、クワッドそれ自体 ( スプライトの矩形) と同じです。
		/// </summary>
		/// @endif
		public Bounds2 GetlContentLocalBounds()
		{
			return Quad.Bounds2();
		}

		/// @if LANG_EN
		/// <summary>
		/// Stretch sprite Quad so that it covers the entire screen. The scene
		/// needs to have been set/started, since it uses CurrentScene.Camera.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スクリーン全体を覆うようにスプライトのクワッドを延ばします。 CurrentScene.Camera を使用しているため、シーンを set/started している必要があります。
		/// </summary>
		/// @endif
		public void MakeFullScreen()
		{
			Quad = new TRS( Director.Instance.CurrentScene.Camera.CalcBounds() );
		}

		/// @if LANG_EN
		/// <summary>
		/// Translate sprite geometry so that center of the sprite becomes aligned 
		/// with the position of the Node.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの中心が Node の中心になるように、スプライトの幾何情報を変更します。
		/// </summary>
		/// @endif
		public void CenterSprite()
		{
			Quad.Centering( new Vector2( 0.5f, 0.5f ) );
//			Quad.Centering( TRS.Local.Center ); // same as above
		}

		/// @if LANG_EN
		/// <summary>
		/// Modify the center of the sprite geometry.
		/// </summary>
		/// <param name="new_center">
		/// The new center, specified in Node local coordinates.
		/// You can pass constants defined under TRS.Local for conveniency.
		/// </param>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの幾何情報の中心を変更します。
		/// </summary>
		/// @endif
		public void CenterSprite( Vector2 new_center )
		{
			Quad.Centering( new_center );
		}

		abstract internal void internal_draw();
		abstract internal void internal_draw_cpu_transform();
	}

	/// @if LANG_EN
	/// <summary>
	/// SpriteUV is a sprite for which you set uvs manually. Uvs are stored as a TRS object. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to 
	/// use then as children of SpriteList.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>SpriteUVは、手動で UV を設定するスプライトです。
	/// UV は TRS オブジェクトとして格納されます。
	/// SpriteUVを別個で描画すると、処理が重くなるので注意してください。可能な限り、SpriteList の子として利用してください。
	/// </summary>
	/// @endif
	public class SpriteUV 
	: SpriteBase
	{
		/// @if LANG_EN
		/// <summary>The UV is specified as a TRS, which lets you define any oriented rectangle in the UV domain.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>TRS として UV を指定します。UV領域内で矩形の向きを定義できます。
		/// </summary>
		/// @endif
		public TRS UV = TRS.Quad0_1;

		/// @if LANG_EN
		/// <summary>SpriteUV constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public SpriteUV()
		{
		}

		/// @if LANG_EN
		/// <summary>SpriteUV constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public SpriteUV( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// @if LANG_EN
		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// If the uv is Quad0_1 (the 0,1 unit quad), then this will return thr texture size in pixels.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>UVとテクスチャの寸法に基づいて、ピクセル単位で対応するサイズを返します。 
		/// 例えば、bob.Quad.S = bob.CalcSizeInPixels(). といった処理が行えます。
		/// UV が Quad0_1 ( 0,1 単位クワッド ) の場合、ピクセル単位のテクスチャサイズを返します。
		/// </summary>
		/// @endif
		override public Vector2 CalcSizeInPixels()
		{
			Common.Assert( TextureInfo != null );
			Common.Assert( TextureInfo.Texture != null );

			return new Vector2( UV.S.X * (float)TextureInfo.Texture.Width ,
								UV.S.Y * (float)TextureInfo.Texture.Height );
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, ref UV, ref trans );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// SpriteTile is a sprite for which you specify a tile index (1D or 2D) in a TextureInfo. 
	/// Note that the cost of using SpriteUV alone is heavy, try as much as you can to use 
	/// then as children of SpriteList.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>SpriteTile は、TextureInfo 内のタイルインデックス (1D または 2D) を指定する形式のスプライトです。
	/// 別個で描画すると、処理が重くなるので注意してください。可能な限り、SpriteList の子として利用してください。
	/// </summary>
	/// @endif
	public class SpriteTile : SpriteBase
	{
		/// @if LANG_EN
		/// <summary>
		/// TileIndex2D defines the UV that will be used for this sprite. 
		/// Tiles are indexed in 2 dimensions.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>TileIndex2D はこのスプライトに使われる UV を定義します。
		/// 2次元でタイルのインデックスを指定します。
		/// </summary>
		/// @endif
		public Vector2i TileIndex2D = new Vector2i(0,0);

		/// @if LANG_EN
		/// <summary>
		/// Instead of TileIndex2D you can also work with a flattened 1d index, for animation, etc.
		/// In that case the set/get calculation depend on TextureInfo, so TextureInfo must have 
		/// been set properly.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>TileIndex2D の代わりに、アニメーションの用途などで1次元のインデックスを使うこともできます。
		/// その場合には、set/get の計算がTextureInfoに依存してしまうので、TextureInfoが正しく設定されている必要があります。
		/// </summary>
		/// @endif
		public int TileIndex1D
		{
			set 
			{ 
				Common.Assert( TextureInfo != null );
				TileIndex2D = new Vector2i( value % TextureInfo.NumTiles.X, value / TextureInfo.NumTiles.X );
			}

			get 
			{ 
				Common.Assert( TextureInfo != null );
				return TileIndex2D.X + TileIndex2D.Y * TextureInfo.NumTiles.X; 
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// TileIndex2D はデフォルトで (0,0)にセットされます。
		/// </summary>
		/// @endif
		public SpriteTile()
		{
		}

		/// @if LANG_EN
		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// TileIndex2D はデフォルトで (0,0)にセットされます。
		/// </summary>
		/// @endif
		public SpriteTile( TextureInfo texture_info )
		: base( texture_info )
		{
		}

		/// @if LANG_EN
		/// <summary>
		/// SpriteTile constructor.
		/// TileIndex2D is set to (0,0) by default.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">2D tile index. (0,0) is the bottom left tile.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// TileIndex2D はデフォルトで (0,0)にセットされます。
		/// </summary>
		/// <param name="texture_info">タイルのテクスチャオブジェクト。</param>
		/// <param name="index">2次元のタイルインデックス。(0,0) は左下のタイルです。</param>
		/// @endif
		public SpriteTile( TextureInfo texture_info, Vector2i index )
		: base( texture_info )
		{
			TileIndex2D = index;
		}

		/// @if LANG_EN
		/// <summary>
		/// SpriteTile constructor.
		/// </summary>
		/// <param name="texture_info">The tiled texture object.</param>
		/// <param name="index">1D tile index. Flat indexing starts from bottom left tile, which is (0,0) in 2D.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="texture_info">タイルのテクスチャオブジェクト。</param>
		/// <param name="index">1次元のタイルインデックス。インデックスは左下のタイルから始まります(2Dでは (0,0)にあたる)。</param>
		/// @endif
		public SpriteTile( TextureInfo texture_info, int index )
		: base( texture_info )
		{
			TileIndex1D = index;
		}

		/// @if LANG_EN
		/// <summary>
		/// Based on the uv and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>UVとテクスチャのサイズに基づき、ピクセル単位で相当するサイズを返します。
		/// 例えば、bob.Quad.S = bob.CalcSizeInPixels()のように使います。
		/// </summary>
		/// @endif
		override public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}

		override internal void internal_draw()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D );
		}

		override internal void internal_draw_cpu_transform()
		{
			Director.Instance.SpriteRenderer.FlipU = FlipU;
			Director.Instance.SpriteRenderer.FlipV = FlipV;
			Matrix3 trans = GetTransform(); // warning: ignored local Camera and VertexZ
			Director.Instance.SpriteRenderer.AddSprite( ref Quad, TileIndex2D, ref trans );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// Draw sprites in batch to reduce the number of draw calls, state setup etc.
	/// 
	/// Just adding SpriteUV or SpriteTile objects as children of a SpriteList with AddChild()
	/// will enable batch rendering, with the limitation that the TextureInfo, BlendMode, 
	/// and Color property of the sprites will be ignored in favor of the parent SpriteList's 
	/// TextureInfo, BlendMode, and Color properties.
	/// 
	/// Important: some functions in SpriteUV and SpriteTile use their local TextureInfo
	/// instead of the parent's SpriteTile one, so you probably want to set both to be safe.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>描画呼び出しやステートのセットアップの呼び出し回数を削減するため、スプライトを一括して描画します。
	/// SpriteUV や SpriteTile オブジェクトを AddChild() で SpriteList の子として追加するだけで、一括描画を可能にします。
	/// ただし、スプライトの TextureInfo, BlendMode, Color のプロパティは無視されます。それらのプロパティは親の SpriteList に依存します。
	/// </summary>
	/// @endif
	public class SpriteList : Node
	{
		/// @if LANG_EN
		/// <summary>
		/// If EnableLocalTransform flag is true, the children sprite's local transform matrices get used,
		/// but vertices get partly transformed on the cpu. You can turn this behavior off to ignore the local 
		/// transform matrix to save a little bit of cpu processing (and rely on Sprite's Quad only 
		/// to position the sprite). In that case (EnableLocalTransform=false) the Position, Scale, Skew, Pivot 
		/// will be ignored.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>EnableLocalTransform フラグがtrueの場合、子のスプライトのローカル変換行列が使われます。
		/// その場合、頂点は CPU 上で変換します。
		/// 
		/// CPU処理の節約のため、ローカル変換行列を無視するために、この挙動を OFF にすることができます。
		/// その場合、スプライトの位置はクワッドに依存します。
		/// 
		/// EnableLocalTransform=false の場合、Position, Scale, Skew, Pivot は無視されます。
		/// </summary>
		/// @endif
		public bool EnableLocalTransform = true;
		
		/// @if LANG_EN
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用される色。
		/// </summary>
		/// @endif
		public Vector4 Color = Colors.White;
		
		/// @if LANG_EN
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用されるブレンドモード。
		/// </summary>
		/// @endif
		public BlendMode BlendMode = BlendMode.Normal;

		/// @if LANG_EN
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用される TextureInfo オブジェクト。
		/// </summary>
		/// @endif
		public TextureInfo TextureInfo; 
		
		/// @if LANG_EN
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用されるシェーダー。
		/// </summary>
		/// @endif
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// @if LANG_EN
		/// <summary>
		/// SpriteList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// TextureInfo はコンストラクタで必ず指定しなければなりません。
		/// </summary>
		/// @endif
		public SpriteList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		public override void DrawHierarchy()
		{
			if ( !Visible )
				return;

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawTransform ) != 0 )
				DebugDrawTransform();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PushTransform");
			PushTransform();
			////Common.Profiler.Pop();

			{
				Director.Instance.GL.SetBlendMode( BlendMode );
				Shader.SetColor( ref Color );
				Shader.SetUVTransform( ref Math.UV_TransformFlipV );
				Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Children.Count );
			}

			int index=0;
			for ( ; index < Children.Count; ++index )
			{
				if ( Children[index].Order >= 0 ) break;

				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			////Common.Profiler.Push("DrawHierarchy's PostDraw");
			Draw();
			////Common.Profiler.Pop();

			for ( ; index < Children.Count; ++index )
			{
				if ( !EnableLocalTransform ) ((SpriteBase)Children[index]).internal_draw();
				else ((SpriteBase)Children[index]).internal_draw_cpu_transform();
			}

			{
				Director.Instance.SpriteRenderer.EndSprites(); 
			}

//			#if DEBUG
			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawPivot ) != 0 )
				DebugDrawPivot();

			if ( ( Director.Instance.DebugFlags & DebugFlags.DrawContentLocalBounds ) != 0 )
				DebugDrawContentLocalBounds();
//			#endif

			////Common.Profiler.Push("DrawHierarchy's PopTransform");
			PopTransform();
			////Common.Profiler.Pop();
		}
	}

	/// @if LANG_EN
	/// <summary>Data struct used by RawSpriteTileList.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>RawSpriteTileList で使用される構造体。
	/// </summary>
	/// @endif
	public struct RawSpriteTile
	{
		/// @if LANG_EN
		/// <summary>Sprite geometry (position, rotation, scale define a rectangle).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>スプライトの幾何情報 (位置、回転、スケールで矩形を定義します)。
		/// </summary>
		/// @endif
		public TRS Quad;
		
		/// @if LANG_EN
		/// <summary>The tile index.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>タイルインデックス。
		/// </summary>
		/// @endif
		public Vector2i TileIndex2D;
		
		/// @if LANG_EN
		/// <summary>If true, the sprite UV are flipped horizontally.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>trueの場合、スプライトのUVは水平方向に反転します。
		/// </summary>
		/// @endif
		public bool FlipU;
		
		/// @if LANG_EN
		/// <summary>If true, the sprite UV are flipped vertically.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>trueの場合、スプライトのUVは垂直方向に反転します。
		/// </summary>
		/// @endif
		public bool FlipV;
//		float VertexZ;

		/// @if LANG_EN
		/// <summary>RawSpriteTile constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public RawSpriteTile( TRS positioning, Vector2i tile_index, bool flipu=false, bool flipv=false )
		{
			Quad = positioning;
			TileIndex2D = tile_index;
			FlipU = flipu;
			FlipV = flipv;
//			VertexZ = vertexz;
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// Draw sprites in batch to reduce number of draw calls, state setup etc.
	/// Unlike SpriteList, instead of holding a list of Node objects, this holds 
	/// a list of RawSpriteTile, which is more lightweight. In effect this is a 
	/// thin wrap of SpriteRenderer, usable as a Node.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>描画呼び出しや、ステートのセットアップの回数を減らすため、スプライトを一括して描画します。
	/// SpriteListとは違い、Node オブジェクトのリストを保持する代わりに、より軽いRawSpriteTileのリストを保持します。
	/// これは Node として利用可能な SpriteRenderer のラップです。
	/// </summary>
	/// @endif
	public class RawSpriteTileList : Node
	{
		/// @if LANG_EN
		/// <summary>The list of RawSpriteTile objects to render.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画可能な RawSpriteTile オブジェクトのリスト。
		/// </summary>
		/// @endif
		public List< RawSpriteTile > Sprites = new List< RawSpriteTile >();
		
		/// @if LANG_EN
		/// <summary>The color that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用される色。
		/// </summary>
		/// @endif
		public Vector4 Color = Colors.White;
		
		/// @if LANG_EN
		/// <summary>The blend mode that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用されるブレンドモード。
		/// </summary>
		/// @endif
		public BlendMode BlendMode = BlendMode.Normal;
		
		/// @if LANG_EN
		/// <summary>The TextureInfo object that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用される TextureInfoオブジェクト。
		/// </summary>
		/// @endif
		public TextureInfo TextureInfo; 
		
		/// @if LANG_EN
		/// <summary>The shader that will be used for all sprites in the Children list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Children リスト内の全てのスプライトで使用されるシェーダー。
		/// </summary>
		/// @endif
		public SpriteRenderer.ISpriteShader Shader = (SpriteRenderer.ISpriteShader)Director.Instance.SpriteRenderer.DefaultShader;

		/// @if LANG_EN
		/// <summary>
		/// RawSpriteTileList constructor.
		/// TextureInfo must be specified in constructor since there is no default for it.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// TextureInfo はコンストラクタで必ず指定しなければなりません。
		/// </summary>
		/// @endif
		public RawSpriteTileList( TextureInfo texture_info )
		{
			TextureInfo = texture_info;
		}

		/// @if LANG_EN
		/// <summary>The draw function, draws all sprites in Sprites list.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>描画関数。スプライトリスト内で全てのスプライトを描画します。
		/// </summary>
		/// @endif
		public override void Draw()
		{
			Director.Instance.GL.SetBlendMode( BlendMode );
			Shader.SetColor( ref Color );
			Shader.SetUVTransform( ref Math.UV_TransformFlipV );
			Director.Instance.SpriteRenderer.BeginSprites( TextureInfo, Shader, Sprites.Count );

//			System.Console.WriteLine( Sprites.Count );

			foreach ( RawSpriteTile sprite in Sprites )
			{
				Director.Instance.SpriteRenderer.FlipU = sprite.FlipU;
				Director.Instance.SpriteRenderer.FlipV = sprite.FlipV;
				TRS copy = sprite.Quad;
				Director.Instance.SpriteRenderer.AddSprite( ref copy, sprite.TileIndex2D );
			}

			Director.Instance.SpriteRenderer.EndSprites(); 
		}

		/// @if LANG_EN
		/// <summary>
		/// Based on the tile size and texture dimensions, return the corresponding size in pixels.
		/// For example you might want to do something like bob.Quad.S = bob.CalcSizeInPixels().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>タイルサイズとテクスチャのサイズに基づき、ピクセル単位で相当するサイズを返します。
		/// 例えば、bob.Quad.S = bob.CalcSizeInPixels()のように使います。
		/// @endif
		public Vector2 CalcSizeInPixels()
		{
			// in the tile case, all sprites have the same pixel size
			return TextureInfo.TileSizeInPixelsf;
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

