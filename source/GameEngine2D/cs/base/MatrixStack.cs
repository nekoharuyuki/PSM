/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// A simple, OpenGL like, transform stack,
	/// with some optimization hints for dealing
	/// with orthonormal matrices.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>OpenGLに似た、シンプルな変換行列のスタックです。直交行列を扱うための工夫がなされています。
	/// </summary>
	/// @endif
	public class MatrixStack
	{
		internal struct Entry
		{
			public Matrix4 m_value;	// this level's matrix
			public Matrix4 m_inverse; // this level's matrix's inverse
			public bool m_inverse_dirty; // if true, m_inverse needs to be recalculated
			public bool m_orthonormal; // flag that says if this matrix is orthonormal (if it is, inverse calculation can use a faster method)
		}

		Entry[] m_stack;

		uint m_index;
		uint m_capacity;
		uint m_tag;

		/// @if LANG_EN
		/// <summary>
		/// Tag is a number that gets incremented everytime the top matrix content changes.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Tag は一番上の行列の内容が変化したとき、インクリメントされる数値です。
		/// </summary>
		/// @endif
		public uint Tag
		{
			get { return m_tag;}
		}

		/// @if LANG_EN
		/// <summary>MatrixStack constructor.</summary>
		/// <param name="capacity">Maximum depth of the stack.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="capacity">スタックの最大の深さ。</param>		
		/// @endif
		public MatrixStack( uint capacity )
		{
			m_index = 0;
			m_capacity = capacity;

			m_stack = new Entry[capacity];

			m_stack[m_index].m_value = Matrix4.Identity;
			m_stack[m_index].m_inverse = Matrix4.Identity;
			m_stack[m_index].m_inverse_dirty = false;
			m_stack[m_index].m_orthonormal = true;

			m_tag = 0;
		}

		/// @if LANG_EN
		/// <summary>
		/// Size of current stack.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在のスタックのサイズ。
		/// </summary>
		/// @endif
		public uint Size
		{
			get { return m_index + 1; } 
		}

		/// @if LANG_EN
		/// <summary>
		/// Maximum stack size.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>最大スタックサイズ。
		/// </summary>
		/// @endif
		public uint Capacity
		{
			get { return m_capacity; } 
		}

		/// @if LANG_EN
		/// <summary>
		/// Push matrix pushes a copy of current top matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在トップにある行列をコピーして、スタックにプッシュします。
		/// </summary>
		/// @endif
		public void Push()
		{
			m_index++;

			Common.Assert( m_index < m_capacity );

			m_stack[m_index] = m_stack[m_index-1];

			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Pop the top matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列をポップします。
		/// </summary>
		/// @endif
		public void Pop()
		{
			Common.Assert( m_index > 0 );

			m_index--;

			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Right multiply top matrix by 'mat'.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数の 'mat' でトップの行列を右側から乗算します。
		/// </summary>
		/// @endif
		public void Mul( Matrix4 mat )
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * mat;
			m_stack[m_index].m_orthonormal = false;	// we don't know
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Right Multiply top matrix by an orthonormal matrix 'mat'. 
		/// If you know that your matrix is orthonormal, you can use 
		/// Mul1 instead of Mul. As long as you keep operating on 
		/// orthonormal matrices, inverse calculations will be faster.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>直交行列 'mat' でトップの行列を右側から乗算します。もし行列が正規直交行列であることがわかっていれば、Mul()のかわりに Mul1()を使うことができます。正規直交行列で演算し続ける限り、逆計算はより速くなります。
		/// </summary>
		/// @endif
		public void Mul1( Matrix4 mat )
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * mat;
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Set the current matrix (top of the matrix stack).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>行列スタックのトップに、現在の行列をセットします。
		/// </summary>
		/// @endif
		public void Set( Matrix4 mat )
		{
			m_stack[m_index].m_value = mat;
			m_stack[m_index].m_orthonormal = false;	// we don't know
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Set an orthonormal matrix. If you know that your matrix
		/// is orthonormal, you can use Set1 instead of Set. As long
		/// as you keep operating on orthonormal matrices, inverse
		/// calculations will be faster.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>正規直交行列を設定します。行列が正規直交であることが事前にわかっている場合、Set()の代わりにSet1()を使用することができます。正規直交行列で演算する限り、逆数計算はより速くなります。
		/// </summary>
		/// @endif
		public void Set1( Matrix4 mat )
		{
			m_stack[m_index].m_value = mat;
			m_stack[m_index].m_orthonormal = true;
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Get the current matrix (top of the matrix stack).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>現在、行列スタックのトップの行列を取得します。
		/// </summary>
		/// @endif
		public Matrix4 Get()
		{
			return m_stack[m_index].m_value;
		}

		/// @if LANG_EN
		/// <summary>
		/// Update (if necessary) and return the cached inverse matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>キャッシュされた逆行列を必要に応じて更新し、返します。
		/// </summary>
		/// @endif
		public Matrix4 GetInverse()
		{
			if ( m_stack[m_index].m_inverse_dirty )
			{
				if ( m_stack[m_index].m_orthonormal )
					m_stack[m_index].m_inverse = m_stack[m_index].m_value.InverseOrthonormal();
				else
					m_stack[m_index].m_inverse	= m_stack[m_index].m_value.Inverse();

				m_stack[m_index].m_inverse_dirty = false;
			}
			return m_stack[m_index].m_inverse;
		}

		/// @if LANG_EN
		/// <summary>
		/// Set the top matrix to identity.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列に単位行列をセットします。
		/// </summary>
		/// @endif
		public void SetIdentity()
		{
			m_stack[m_index].m_value = Matrix4.Identity;
			m_stack[m_index].m_inverse = Matrix4.Identity;
			m_stack[m_index].m_orthonormal = true;
			m_stack[m_index].m_inverse_dirty = false;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the top matrix around X axis.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列をX軸まわりで回転させます。
		/// </summary>
		/// @endif
		public void RotateX( float angle )  
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationX( angle );
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the top matrix around Y axis.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列をY軸まわりで回転させます。
		/// </summary>
		/// @endif
		public void RotateY( float angle )  
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationY( angle );
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the top matrix around Z axis.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列をZ軸まわりで回転させます。
		/// </summary>
		/// @endif
		public void RotateZ( float angle )  
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationZ( angle );
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Rotate the top matrix around a user given axis.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>開発者が指定した軸で、トップの行列を回転させます。
		/// </summary>
		/// @endif
		public void Rotate( Vector3 axis, float angle )  
		{
			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.RotationAxis( axis, angle );
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;
			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Scale the top matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列をスケールさせます。
		/// </summary>
		/// @endif
		public void Scale( Vector3 value )  
		{
			if ( value == Math._111 ) return;

			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.Scale( value );
			m_stack[m_index].m_orthonormal = false;
			m_stack[m_index].m_inverse_dirty = true;

			m_tag++;
		}

		/// @if LANG_EN
		/// <summary>
		/// Translate the top matrix.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>トップの行列を平行移動させます。
		/// </summary>
		/// @endif
		public void Translate( Vector3 value )  
		{
			if ( value == Math._000 ) return;

			m_stack[m_index].m_value = m_stack[m_index].m_value * Matrix4.Translation( value );
//			m_stack[m_index].m_orthonormal is unchanged
			m_stack[m_index].m_inverse_dirty = true;

			m_tag++;
		}
	}
}
