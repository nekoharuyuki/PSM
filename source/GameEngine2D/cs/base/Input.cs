/* PlayStation(R)Mobile SDK 1.21.01
 * Copyright (C) 2013 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// @if LANG_EN
	/// <summary>
	/// Wrap Sce.PlayStation.Core.Input so that input conditional expressions are less verbose.
	/// Some examples:
	/// 
	/// if ( Input2.GamePad.GetData(0).Left.Down )  // is game pad left button down
	/// 
	/// if ( Input2.GamePad0.Left.Down )            // same as above, but we use an alias 'GamePad0' for GamePad.GetData(0)
	/// 
	/// if ( Input2.GamePad0.Square.Release )       // has gamepad square button just been released
	/// 
	/// if ( Input2.Keyboard0.Left.Press )          // has keyboard left arrow button just been pressed
	/// 
	/// if ( Input2.Keyboard0.S.On )                // has keyboard S key been held down for more than one frame
	/// 
	/// Note that you don't have to use Input2. The GameEngine2D library uses it internally in a couple of places
	/// and it's used in samples too. It keeps the GetData( deviceIndex ) api but simply harmonizes the state queries for buttons.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>Sce.PlayStation.Core.Inputをラップしたクラス。
	/// 
	/// 例:
	/// 
	/// if ( Input2.GamePad.GetData(0).Left.Down )  // ゲームパッドの左ボタンが押されている。
	/// 
	/// if ( Input2.GamePad0.Left.Down )            // 上記と同じですが、GamePad.GetData(0)のエイリアス(別名) 'GamePad0' を使用しています。
	/// 
	/// if ( Input2.GamePad0.Square.Release )       // ゲームパッドの□ボタンが離された瞬間。
	/// 
	/// if ( Input2.Keyboard0.Left.Press )          // キーボードの左キーが押された瞬間。
	/// 
	/// if ( Input2.Keyboard0.S.On )                // キーボードのSキーが1フレーム以上押され続けている。
	/// 
	/// 
	/// </summary>
	/// @endif
	public static class Input2
	{
		/// @if LANG_EN
		/// <summary>
		/// Button data structure shared between various helper wrap of
		/// Input.GamePad, Input.Touch and the Keyboard.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ボタンのデータ構造体は Input.GamePad、Input.Touch、Keyboardなどのヘルパーラップで共有されます。
		/// </summary>
		/// @endif
		public struct ButtonState
		{
			internal byte m_data; 

			/// @if LANG_EN
			/// <summary>Return true if we are pushing the button this frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>このフレームでボタンが押されているなら、trueを返します。
			/// </summary>
			/// @endif
			public bool Down { get { return( m_data & 1 ) != 0;}}
			
			/// @if LANG_EN
			/// <summary>Return true if we are pushing the button this frame but weren't pushing in previous frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでボタンが押されておらず、このフレームでボタンが押されているなら、trueを返します。
			/// </summary>
			/// @endif
			public bool Press { get { return( m_data & 2 )!= 0;}}

			/// @if LANG_EN
			/// <summary>Return true if we are pushing the button this frame and were pushing in previous frame as well</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでもボタンが押されており、このフレームでボタンが押されている場合、trueを返します。
			/// </summary>
			/// @endif
			public bool On { get { return( m_data & 4 ) != 0;}}

			/// @if LANG_EN
			/// <summary>Return true if we are not pushing the button this frame but were pushing in previous frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでボタンを押しており、このフレームで押されていない場合、trueを返します。
			/// </summary>
			/// @endif
			public bool Release { get { return( m_data & 8 ) != 0;}}
//			public bool DownCount // todo: number of frames the key has been held down

			internal static ButtonState Default = new ButtonState() { m_data = 0};

			internal void frame_update( bool down )
			{
				byte data = (byte)( down ? 1 : 0 );	// down
				if ( !Down && down ) data |= 2;	// press
				if ( Down && down )	data |= 4; // on
				if ( Down && !down ) data |= 8;	// release
				m_data = data;
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Wrap Input.TouchData.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Input.TouchData をラップしたクラス。
		/// </summary>
		/// @endif
		public class TouchData
		{
			ButtonState m_state;
			Vector2 m_pos;
			Vector2 m_pos_prev;	// Sce.PlayStation.Core.Input bug workaround: in some cases, return the previous frame's position
			internal bool m_visited;
			internal int m_id; // api ID

			public TouchData() 
			{ 
				m_state = ButtonState.Default;
				m_pos = GameEngine2D.Base.Math._00;
				m_pos_prev = GameEngine2D.Base.Math._00;
				m_visited = false;
				m_id = -1;
			}

			/// @if LANG_EN
			/// <summary>
			/// For consistency with the graphics system, this function returns the touch position 
			/// in normalized screen coordinates:  bottom left (-1,1), upper right (1,1).
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>グラフィックスシステムとの一貫性を保つために、この関数は正規化された画面座標でのタッチ位置を返します。 : 左下（-1,1）、右上（1,1）。
			/// </summary>
			/// @endif
			public Vector2 Pos { get { return m_pos;}} 
			// work around a bug in Sce.PlayStation.Core.Input that resets position to 0,0 before we fully release the mouse...
			public Vector2 PreviousPos { get { return m_pos_prev;}}	

			/// @if LANG_EN
			/// <summary>Return true if we are touching this frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>このフレームでタッチしている場合、trueを返します。
			/// </summary>
			/// @endif
			public bool Down { get { return m_state.Down;}}

			/// @if LANG_EN
			/// <summary>Return true if we are touching this frame but weren't touching in previous frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでタッチしておらず、このフレームでタッチしている場合、trueを返します。
			/// </summary>
			/// @endif
			public bool Press { get { return m_state.Press;}}

			/// @if LANG_EN
			/// <summary>Return true if we are touching this frame and were touching in previous frame as well</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでタッチしており、このフレームでもタッチしている場合、trueを返します。
			/// </summary>
			/// @endif
			public bool On { get { return m_state.On;}}

			/// @if LANG_EN
			/// <summary>Return true if we are not touching this frame but were touching in previous frame</summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>前のフレームでタッチしており、このフレームでタッチしていない場合、trueを返します。
			/// </summary>
			/// @endif
			public bool Release { get { return m_state.Release;}}

			internal void frame_update( Vector2 pos, bool down )
			{
				m_state.frame_update( down );

				m_pos_prev = m_pos;
				m_pos = pos;

				if ( Press ) // this is a "new" touch so clear m_pos_prev as well
					m_pos_prev = m_pos;
			}
		}

		internal class TouchDataArray
		{
			internal TouchData[] m_touch_data;
			int m_device_index;
			bool m_external_control = false;
			List< Sce.PlayStation.Core.Input.TouchData > m_external_data;

			int Capacity {  get{ return 10; } }

			List<int> m_id_set;

			internal TouchDataArray( int device_index )
			{
				m_device_index = device_index;

				m_touch_data = new TouchData[Capacity]; // same as Touch.cs
				for ( int i=0; i < m_touch_data.Length; ++i )
					m_touch_data[i] = new TouchData();

				m_id_set = new List<int>();
			}

			TouchData get_touch_data( int id )
			{
				foreach ( TouchData td in m_touch_data )
				{
					if ( td.m_id == id )
						return td;
				}

				foreach ( TouchData td in m_touch_data )
				{
					if ( td.m_id == -1 )
					{
						td.m_id = id;
						return td;
					}
				}

				return null;
			}

			/// @if LANG_EN
			/// <summary>
			/// By default, Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2 simply gets the input data automatically, but 
			/// you can override this behaviour and manually set the data. If the data you set has the .Skip flag set 
			/// to true, all touch and button input will be ignored in Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2.
			/// Calling SetData once forever enables the manual/external control behavior.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>デフォルトでは、Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2は、単に自動的に入力データを取得しますが、この動作をオーバーライドして、データを手動で設定することができます。
			/// Skipフラグをtrueにセットすると、すべてのタッチ・ボタンの入力は Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2内で無視されるようになります。
			/// いったん SetData を呼び出すと、手動/外部コントローラのふるまいを永続的に可能にします。
			/// </summary>
			/// @endif
			public void SetData( List< Sce.PlayStation.Core.Input.TouchData > data )
			{
				m_external_control = true;
				m_external_data = data;
			}

			internal void frame_update()
			{
				List< Sce.PlayStation.Core.Input.TouchData > touch_data_list = m_external_data;
				if ( !m_external_control )
					touch_data_list = Sce.PlayStation.Core.Input.Touch.GetData( m_device_index );

				Common.Assert( touch_data_list != null );

				foreach ( TouchData td in m_touch_data )
					td.m_visited = false;

				m_id_set.Clear();
				foreach ( Sce.PlayStation.Core.Input.TouchData src_data in touch_data_list )
					m_id_set.Add( src_data.ID );

				foreach ( Sce.PlayStation.Core.Input.TouchData src_data in touch_data_list )
				{
					TouchData td = get_touch_data( src_data.ID );
					td.m_visited = true;

					Vector2 pos = td.Pos;
					bool down = false;

					if ( !src_data.Skip )
					{
						pos = new Vector2( src_data.X, -src_data.Y ) * 2.0f; // bottom left (0,0), top right (1,1)
						down = true;
					}

					td.frame_update( pos, down );
				}

				foreach ( TouchData td in m_touch_data )
				{
					if ( !td.m_visited )
					{
						td.m_id = -1;
						td.frame_update( td.Pos, false );
					}
				}
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Wrap Input.Touch.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Input.Touch をラップしたクラス。
		/// </summary>
		/// @endif
		static public class Touch
		{
			static uint m_last_frame_count = unchecked((uint)-1);
			static TouchDataArray s_touch_data0 = new TouchDataArray(0);


			/// <summary></summary>
			public static 
				int MaxTouch { get{ return s_touch_data0.m_touch_data.Length; } }


			/// <summary></summary>
			public static TouchData[] GetData( uint deviceIndex = 0 )
			{
				Common.Assert( deviceIndex == 0 );

				if ( m_last_frame_count != Common.FrameCount )
				{
					s_touch_data0.frame_update();
					m_last_frame_count = Common.FrameCount;
				}

				return s_touch_data0.m_touch_data;
			}

			public static void SetData( uint deviceIndex, List< Sce.PlayStation.Core.Input.TouchData > data )
			{
				Common.Assert( deviceIndex == 0 );
				s_touch_data0.SetData( data );
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Wrap Input.GamePadData.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Input.GamePadData をラップしたクラス。
		/// </summary>
		/// @endif
		public class GamePadData
		{
			int m_device_index;
			bool m_external_control = false;
			Sce.PlayStation.Core.Input.GamePadData m_external_data;


			/// <summary></summary>
			public ButtonState Left     = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Up       = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Right    = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Down     = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Square   = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Triangle = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Circle   = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Cross    = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Start    = ButtonState.Default;
			/// <summary></summary>
			public ButtonState Select   = ButtonState.Default;
			/// <summary></summary>
			public ButtonState L        = ButtonState.Default;
			/// <summary></summary>
			public ButtonState R        = ButtonState.Default;
			/// <summary></summary>
			public Vector2 Dpad = GameEngine2D.Base.Math._00;
			/// <summary></summary>
			public Vector2 AnalogLeft = new Vector2();
			/// <summary></summary>
			public Vector2 AnalogRight = new Vector2();

			internal GamePadData( int device_index )
			{
				m_device_index = device_index;
			}

			/// @if LANG_EN
			/// <summary>
			/// By default, Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2 simply gets the input data automatically, but 
			/// you can override this behaviour and manually set the data. If the data you set has the .Skip flag set 
			/// to true, all touch and button input will be ignored in Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2.
			/// Calling SetData once forever enables the manual/external control behavior.
			/// </summary>
			/// @endif
			/// @if LANG_JA
			/// <summary>デフォルトでは、Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2は、単に自動的に入力データを取得しますが、この動作をオーバーライドして、データを手動で設定することができます。
			/// Skipフラグをtrueにセットすると、すべてのタッチ・ボタンの入力は Sce.PlayStation.HighLevel.GameEngine2D.Base.Input2内で無視されるようになります。
			/// いったん SetData を呼び出すと、手動/外部コントローラのふるまいを永続的に可能にします。
			/// </summary>
			/// @endif
			public void SetData( Sce.PlayStation.Core.Input.GamePadData data )
			{
				m_external_control = true;
				m_external_data = data;
			}

			internal void frame_update()
			{
				Sce.PlayStation.Core.Input.GamePadData game_pad_data = m_external_data;
				if ( !m_external_control )
					game_pad_data = Sce.PlayStation.Core.Input.GamePad.GetData( m_device_index );

				bool skip_mask = true;
				if ( game_pad_data.Skip )
					skip_mask = false; // .Skip flag set on external data: ignore the data

				Left     . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Left     ) != 0 ) && skip_mask );
				Up       . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Up       ) != 0 ) && skip_mask );
				Right    . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Right    ) != 0 ) && skip_mask );
				Down     . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Down     ) != 0 ) && skip_mask );
				Square   . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Square   ) != 0 ) && skip_mask );
				Triangle . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Triangle ) != 0 ) && skip_mask );
				Circle   . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Circle   ) != 0 ) && skip_mask );
				Cross    . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Cross    ) != 0 ) && skip_mask );
				Start    . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Start    ) != 0 ) && skip_mask );
				Select   . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.Select   ) != 0 ) && skip_mask );
				L        . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.L        ) != 0 ) && skip_mask );
				R        . frame_update( ( ( game_pad_data.Buttons & Sce.PlayStation.Core.Input.GamePadButtons.R        ) != 0 ) && skip_mask );

				if ( skip_mask == true )
				{
					Dpad = GameEngine2D.Base.Math._00;

					if ( Left  . Down )	Dpad -= Math._10;
					if ( Up    . Down )	Dpad += Math._01;
					if ( Right . Down )	Dpad += Math._10;
					if ( Down  . Down )	Dpad -= Math._01;

					AnalogLeft.X = game_pad_data.AnalogLeftX;
					AnalogLeft.Y = game_pad_data.AnalogLeftY;

					AnalogRight.X = game_pad_data.AnalogRightX;
					AnalogRight.Y = game_pad_data.AnalogRightY;
				}
			}
		}

		/// @if LANG_EN
		/// <summary>
		/// Wrap Input.GamePad (button bits).
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Input.GamePad (button bits) をラップしたクラス。
		/// </summary>
		/// @endif
		static public class GamePad
		{
			static uint m_last_frame_count = unchecked((uint)-1);
			static GamePadData s_game_pad_data0 = new GamePadData(0);

			/// <summary></summary>
			public static GamePadData GetData( uint deviceIndex = 0 )
			{
				Common.Assert( deviceIndex == 0 );

				if ( m_last_frame_count != Common.FrameCount )
				{
					s_game_pad_data0.frame_update();
					m_last_frame_count = Common.FrameCount;
				}

				return s_game_pad_data0;
			}


			public static void SetData( uint deviceIndex, Sce.PlayStation.Core.Input.GamePadData data )
			{
				Common.Assert( deviceIndex == 0 );
				s_game_pad_data0.SetData( data );
			}
		}

		// some aliases so we don't have to write annoying long GetData statements everywhere all the time

		/// @if LANG_EN
		/// <summary>Alias for Touch.GetData(0)[0]</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Touch.GetData(0)[0] のエイリアス(別名)。
		/// </summary>
		/// @endif

		public static TouchData Touch00 { get { return Touch.GetData(0)[0];}}

		/// @if LANG_EN
		/// <summary>Alias for GamePad.GetData(0)</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>GamePad.GetData(0) のエイリアス。
		/// </summary>
		/// @endif
		public static GamePadData GamePad0 { get { return GamePad.GetData(0);}}
	}
}

