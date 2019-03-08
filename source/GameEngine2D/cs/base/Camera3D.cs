/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// Frustum object, used by Camera2D and Camera3D. 
	/// It only deals with perspective.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Camera2DとCamera3Dによって使用される視錐台 (Frustum) のオブジェクト。
	/// 透視変換に利用します。
	/// </summary>
	/// @endif
	public class Frustum
	{
		bool m_is_fovy;
		float m_fov;
		
		/// @if LANG_EN
		/// <summary>Frustum constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// @endif
		public Frustum()
		{
			FovY = Math.Deg2Rad( 53.0f );
		}
		
		/// @if LANG_EN
		/// <summary>The projection as a matrix.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>行列としてのプロジェクション。
		/// </summary>
		/// @endif
		public Matrix4 Matrix
		{
			get { return Matrix4.Perspective( FovY, Aspect, Znear, Zfar );}
		}

		/// @if LANG_EN
		/// <summary>Width/Height aspect ratio.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>幅/高さ のアスペクト比。
		/// </summary>
		/// @endif
		public float Aspect = 1.0f;

		/// @if LANG_EN
		/// <summary>
		/// Projection's near z value.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>プロジェクションの near Z値。
		/// </summary>
		/// @endif
		public float Znear = 0.1f;

		/// @if LANG_EN
		/// <summary>
		/// Projection's far z value.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>プロジェクションの far Z値。
		/// </summary>
		/// @endif
		public float Zfar = 1000.0f;

		/// @if LANG_EN
		/// <summary>
		/// Field of view along X axis. 
		/// If you set the field of view with this property, X becomes the main fov direction for this Frustum, 
		/// and FovY value's correctness will depend on Aspect's value. 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>X軸に沿ったビューのフィールド。
		/// このプロパティを使用してビューのフィールドを設定した場合、Xはこの錐台のメインの視野方向となり、FovY値の正しさは、アスペクトの値に依存します。
		/// </summary>
		/// @endif
		public float FovX 
		{
			set 
			{ 
				m_fov = value; 
				m_is_fovy = false;
			}  

			get
			{
				if ( !m_is_fovy )
					return m_fov;

				return 2.0f * FMath.Atan( FMath.Tan( m_fov * 0.5f ) * Aspect );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Field of view along Y axis. 
		/// If you set the field of view with this property, Y becomes the main fov direction for this Frustum, 
		/// and FovX value's correctness will depend on Aspect's value. 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Y軸に沿ったビューのフィールド。
		/// このプロパティを使用してビューのフィールドを設定した場合、Yはこの錐台のメインの視野方向となり、FovY値の正しさは、アスペクトの値に依存します。
		/// </summary>
		/// @endif
		public float FovY 
		{ 
			set 
			{ 
				m_fov = value; 
				m_is_fovy = true;
			} 

			get
			{
				if ( m_is_fovy )
					return m_fov;

				return 2.0f * FMath.Atan( FMath.Tan( m_fov * 0.5f ) / Aspect );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Given a point in normalized screen coordinates (bottom left (-1,1) and upper right (1,1)), 
		/// and a z value, return the corresponding 3D point in view space.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>正規化されたスクリーン座標（左下（-1,1）と右上（1,1））の位置とz値から、ビュー空間内の対応する3D位置を返します。
		/// </summary>
		/// @endif
		public Vector4 GetPoint( Vector2 screen_normalized_pos, float z )
		{
			float half_h = z * FMath.Tan( FovY * 0.5f );
			float half_w = Aspect * half_h;

			Vector4 ret = ( screen_normalized_pos * new Vector2( half_w, half_h ) ).Xy01;
			ret.Z = -z;

			return ret;
		}
	}

	/// @if LANG_EN
	/// <summary>The 3D camera here is quite primitive, as the library is mainly 2D.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>基本的な3Dカメラ。
	/// </summary>
	/// @endif
	public class Camera3D : ICamera
	{
		GraphicsContextAlpha GL;
		DrawHelpers m_draw_helpers;
		int m_push_depth; // check push/pop errors
		Bounds2 m_last_2d_bounds; // for SetFromCamera2D

		/// @if LANG_EN
		/// <summary>constructor.
		/// </summary>
		/// <param name="gl">Needed for its matrix stack.</param>
		/// <param name="draw_helpers">Needed only for debug draw (DebugDraw).</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="gl">行列スタックに必要な参照。</param>
		/// <param name="draw_helpers">デバッグ用描画に必要な参照。</param>
		/// @endif
		public Camera3D( GraphicsContextAlpha gl, DrawHelpers draw_helpers )
		{
			GL = gl;
			m_draw_helpers = draw_helpers;

			m_push_depth = 0;

			Frustum = new Frustum();
		}

		/// @if LANG_EN
		/// <summary>Eye positions.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>視点位置。
		/// </summary>
		/// @endif
		public Vector3 Eye;

		/// @if LANG_EN
		/// <summary>View center/target position.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ビューの中央/ターゲットの位置。
		/// </summary>
		/// @endif
		public Vector3 Center;

		/// @if LANG_EN
		/// <summary>Up vector.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>上方向のベクトル。
		/// </summary>
		/// @endif
		public Vector3 Up;

		/// @if LANG_EN
		/// <summary>The perspective.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>透視変換の錐台。
		/// </summary>
		/// @endif
		public Frustum Frustum;

		/// @if LANG_EN
		/// <summary>
		/// This model matrix is used by NormalizedToWorld/GetTouchPos so we can
		/// define a 2d plane to raytrace touch direction against.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このモデル行列はNormalizedToWorld/ GetTouchPosで使用されます。レイトレースのタッチ方向に対して、2D平面を定義します。
		/// </summary>
		/// @endif
		public Matrix4 TouchPlaneMatrix = Matrix4.Identity;

		/// @if LANG_EN
		/// <summary>
		/// Position the camera and set the persective so that it matches
		/// exactly the 2D ortho view (when all sprites are drawn
		/// on the Z=0 plane anyway, which is the default).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// カメラを配置し、2D並行投影ビューに正確にあうように透視変換をセットします。全てのスプライトが Z=0 の平面に描画されているとき、デフォルトになります。
		/// </summary>
		/// @endif
		public void SetFromCamera2D( Camera2D cam2d )
		{
			Vector2 y = cam2d.Y();
			float eye_distance = y.Length() / FMath.Tan( 0.5f * Frustum.FovY );	// distance to view plane
			Eye = cam2d.Center.Xy0 + eye_distance * Math._001;
			Center = cam2d.Center.Xy0;
			Up = y.Xy0.Normalize();

			m_last_2d_bounds = cam2d.CalcBounds();
		}

		/// @if LANG_EN
		/// <summary>
		/// Update the aspect ratio based on current viewport.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在のビューポートに基づき、アスペクト比を更新します。
		/// </summary>
		/// @endif
		public void SetAspectFromViewport()
		{
			Frustum.Aspect = GL.GetViewportf().Aspect;
		}

		/// @if LANG_EN
		/// <summary>
		/// Calculate the camera transform marix (positioning matrix), as a Matrix4.
		/// GetTransform().InverseOrthonormal() is what you push on the view matrix stack.
		/// Return an orthonormal matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Matrix4としてカメラ変換行列（位置行列）を計算します。
		/// GetTransform().InverseOrthonormal() はビュー行列スタックにプッシュするものです。
		/// 正規直交行列を返します。
		/// </summary>
		/// @endif
		public Matrix4 GetTransform()
		{
			return Math.LookAt( Eye, Center, Up );
		}

		/// @if LANG_EN
		/// <summary>
		/// Push all necessary matrices on the matrix stack.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>行列スタックに必要な全ての行列をプッシュします。
		/// </summary>
		/// @endif
		public void Push()
		{
			++m_push_depth;

			SetAspectFromViewport();

			GL.ProjectionMatrix.Push();
			GL.ProjectionMatrix.Set( Frustum.Matrix );

			GL.ViewMatrix.Push();
			GL.ViewMatrix.Set1( GetTransform().InverseOrthonormal() );

			GL.ModelMatrix.Push();
		}

		/// @if LANG_EN
		/// <summary>
		/// Pop all camera matrices from the matrix stack.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>行列スタックから全てのカメラ行列をポップします。
		/// </summary>
		/// @endif
		public void Pop()
		{
			Common.Assert( m_push_depth > 0 );
			--m_push_depth;

			GL.ModelMatrix.Pop();

			GL.ViewMatrix.Pop();

			GL.ProjectionMatrix.Pop();
		}

		/// @if LANG_EN
		/// <summary>
		/// Draw a canonical debug grid.
		/// Note that DebugDraw() doesn't call Push()/Pop() internally. It is your responsability to call it between this Camera's Push()/Pop().
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>標準的なデバッググリッドを描画します。
		/// DebugDraw() は内部的にPush()/Pop()を呼び出さないことに注意してください。カメラのPush()/Pop()は、開発者が呼び出してください。
		/// </summary>
		/// @endif
		public void DebugDraw( float step )
		{
//			m_draw_helpers.DrawDefaultGrid( CalcBounds(), step, Colors.Cyan * 0.5f );
			m_draw_helpers.DrawDefaultGrid( CalcBounds(), step );
		}

		// Struct returned by calc_view_ray.
		public struct Ray 
		{
			public Vector4 Start;
			public Vector4 Direction;
		}

		// Calculate a ray in camera space, given a point in normalized screen coordinates.
		Ray calc_view_ray( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			Matrix4 transform = GetTransform();
			Vector4 p = transform * Frustum.GetPoint( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos, Frustum.Znear );

			return new Ray() { Start = p, Direction = ( p - transform.ColumnW ).Normalize()}; // perspective
//			return new Ray() { Start = p, Direction = -transform.ColumnZ ) }; // ortho
		}

		/// @if LANG_EN
		/// <summary>
		/// Note that unlike Camera2D.NormalizedToWorld, Camera3D.NormalizedToWorld might not return a
		/// valid position, since it's a ray/plane intersection.
		/// 
		/// The return point is in 2d, in touch plane local coordinates. This function uses the
		/// TouchPlaneMatrix property to know which plane to intersect, so TouchPlaneMatrix must
		/// have been set beforehand (use Node.NormalizedToWorld does it for you).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Camera3D.NormalizedToWorldは有効な位置を返しません。なぜなら、それはray/planeの交差であるからです。 Camera2D.NormalizedToWorldとは違うことに、注意してください。
		/// 
		/// 返り値は、2D、タッチ平面のローカル座標内の位置座標として返されます。
		/// この関数は、どの平面と交差しているかを知るために、TouchPlaneMatrixプロパティを使います。そのため、TouchPlaneMatrixを前もって設定しておく必要があります。(設定にはNode.NormalizedToWorldを使用してください)
		/// </summary>
		/// @endif
		public Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos )
		{
			Ray ray = calc_view_ray( bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );

			Vector4 plane_base = TouchPlaneMatrix.ColumnW;
			Vector4 plane_normal = TouchPlaneMatrix.ColumnZ;

			float t = - ( ray.Start - plane_base ).Dot( plane_normal ) / ray.Direction.Dot( plane_normal );

			if ( t < 0.0f )
				return GameEngine2D.Base.Math._00;	// no hit

			return( TouchPlaneMatrix.InverseOrthonormal() * ( ray.Start + ray.Direction * t ) ).Xy;
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the 'nth' touch position in world coordinates.
		/// The 'prev' flag is for internal use only.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ワールド座標で引数 nthのタッチ位置を返します。'prev'フラグは内部的に使用します。
		/// </summary>
		/// @endif
		public Vector2 GetTouchPos( int nth = 0, bool prev = false )
		{
			return NormalizedToWorld( prev 
									  ? Input2.Touch.GetData(0)[nth].PreviousPos 
									  : Input2.Touch.GetData(0)[nth].Pos );
		}

		/// @if LANG_EN
		/// <summary>
		/// Return the most recent bounds set by SetFromCamera2D.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>SetFromCamera2Dによって設定された、直近の境界を返します。
		/// </summary>
		/// @endif
		public Bounds2 CalcBounds()
		{
			return m_last_2d_bounds;
		}

		/// @if LANG_EN
		/// <summary>
		/// Creates a 3D view that max the screen bounds in pixel size (in plane z=0).
		/// Exactly match the 3D frustum. Eye distance to z=0 plane is calculated based
		/// on current Frustum.FovY value.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ピクセルサイズ (平面 z =0) のスクリーン境界を最大にする、3Dビューを作成します。正確に3次元視錐台と一致します。視点からz=0平面への距離は、現在のFrustum.FovY値に基づいて計算されます。
		/// </summary>
		/// @endif
		public void SetViewFromViewport()
		{
			Camera2D tmp = new Camera2D( GL, m_draw_helpers );
			tmp.SetViewFromViewport();
			SetFromCamera2D( tmp );
		}

		/// @if LANG_EN
		/// <summary>
		/// Based on current viewport size, get the size of a "screen pixel" in world coordinates.
		/// Can be used to determine scale factor needed to draw sprites or fonts 1:1 for example.
		/// Uses the most recent Bounds2 set by SetFromCamera2D.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在のビューポートのサイズに基づいて、ワールド座標における'スクリーンピクセル'のサイズを取得します。
		/// 例えば、スプライトやフォントを1:1で描画するために必要なスケールファクターを決定するために使用することができます。
		/// SetFromCamera2Dによって設定された最新のBounds2を使用しています。
		/// </summary>
		/// @endif
		public float GetPixelSize()
		{
			Bounds2 bounds = GL.GetViewportf();
			return( m_last_2d_bounds.Size.X * 0.5f ) / ( bounds.Size.X * 0.5f );
		}

		/// @if LANG_EN
		/// <summary>
		/// Debug camera navigation.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>デバッグ用カメラのナビゲーション。
		/// </summary>
		/// @endif
		public void Navigate( int control )
		{
//			System.Console.WriteLine( Common.FrameCount + " Camera3D.Navigate is not implemented" );
		}

		/// @if LANG_EN
		/// <summary>
		/// Set the model plane matrix used in GetTouchPos and NormalizedToWorld.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary> GetTouchPos と NormalizedToWorldで使用される、モデル平面行列を設定します。
		/// </summary>
		/// @endif
		public void SetTouchPlaneMatrix( Matrix4 mat )  
		{
			TouchPlaneMatrix = mat;
		}
	}
}

