/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{

/// @if LANG_EN
/// <summary>
/// A common interface for Camera2D and Camera3D.
/// </summary>
/// @endif
/// @if LANG_JA
/// <summary>Camera2D と Camera3D 用の共通インターフェース。
/// </summary>
/// @endif
public interface ICamera
{
	/// @if LANG_EN
	/// <summary>
	/// Read aspect ratio from viewport and update camera projection data accordingly.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ビューポートからアスペクト比を読み、その値からカメラプロジェクションのデータを更新します。
	/// </summary>
	/// @endif
	void SetAspectFromViewport();

	/// @if LANG_EN
	/// <summary>
	/// Push all matrices on the stack, and set Projection and View.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>スタック上の全ての行列をプッシュし、Projection と View を設定します。
	/// </summary>
	/// @endif
	void Push();

	/// @if LANG_EN
	/// <summary>
	/// Pop all matrices from the stack.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>スタックから全ての行列をポップします。
	/// </summary>
	/// @endif
	void Pop();
	
	/// @if LANG_EN
	/// <summary>
	/// Return the camera transform matrix (orthonormal positioning matrix), as a Matrix4.
	/// GetTransform().InverseOrthonormal() is what you push on the view matrix stack.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Matrix4としてカメラ変換行列（並行位置行列）を返します。
	/// GetTransform().InverseOrthonormal() はビュー行列スタックにプッシュするものです。
	/// </summary>
	/// @endif
	Matrix4 GetTransform();
	
	/// @if LANG_EN
	/// <summary>
	/// Draw a world grid and the world coordinate system, for debug.
	/// Note that DebugDraw() doesn't call Push()/Pop() internally. It is your responsability to call it between this Camera's Push()/Pop().
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>デバッグ用にワールドのグリッドと、ワールド座標系を描画します。
	/// DebugDraw() は内部的にPush()/Pop()を呼び出さないことに注意してください。カメラのPush()/Pop()は、開発者が呼び出してください。
	/// </summary>
	/// @endif
	void DebugDraw( float step );
	
	/// @if LANG_EN
	/// <summary>
	/// Process input for debug navigation.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>デバッグ用ナビゲーションのための入力を処理します。
	/// </summary>
	/// @endif
	void Navigate( int control );
	
	/// @if LANG_EN
	/// <summary>
	/// Set a camera view so that the bottom left of the screen matches world point (0,0) and 
	/// the top right of the screen matches world point (screen width, sreen height).
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>スクリーンの左下がワールドの point(0,0)、スクリーンの右上が point(スクリーンの幅, スクリーンの高さ) に一致するよう、カメラビューを設定します。
	/// </summary>
	/// @endif
	void SetViewFromViewport();
	
	/// @if LANG_EN
	/// <summary>
	/// Given a point in normalized screen coordinates (-1->1), return its corresponding world position.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>正規化されたスクリーンの座標系 (-1 -> 1)で与えられた点を、対応するワールドの位置で返します。
	/// </summary>
	/// @endif
	Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
	
	/// @if LANG_EN
	/// <summary>
	/// Return the 'nth' touch position in world coordinates.
	/// The 'prev' flag is for internal use only.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ワールド座標系での引数 'nth' のタッチ位置を返します。'prev' フラグは内部的に使用します。
	/// </summary>
	/// @endif
	Vector2 GetTouchPos( int nth = 0, bool prev = false );
	
	/// @if LANG_EN
	/// <summary>
	/// Calculate the world bounds currently visible on screen.
	/// This function is 2D only, somehow extended to 3D.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>スクリーン上で現在表示可能なワールドの境界を計算します。この関数は2Dのみ対応しています。(3Dに拡張予定です)。
	/// </summary>
	/// @endif
	Bounds2 CalcBounds();
	
	/// @if LANG_EN
	/// <summary>
	/// Based on current viewport size, get the size of a "screen pixel" in world coordinates.
	/// Can be used to determine scale factor needed to draw sprites 1:1 for example.
	/// 2D only, somehow extended to 3D.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// 現在のビューポートのサイズに基づいて、ワールド座標における"スクリーンピクセル"のサイズを取得します。
	/// 例えば、スプライトやフォントを1:1で描画するために必要なスケールファクターを決定するために使用することができます。
	/// この関数は2Dのみ対応しています。(3Dに拡張予定です)。
	/// </summary>
	/// @endif
	float GetPixelSize();
	
	/// @if LANG_EN
	/// <summary>
	/// The the orientation of the 3D plane that should be used by GetTouchPos(). 
	/// 3D only.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>GetTouchPos（）によって使用される、3次元平面の向きを設定します。3次元のみ対応しています。
	/// </summary>
	/// @endif
	void SetTouchPlaneMatrix( Matrix4 mat );
}

}

