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
	/// The base class for all actions.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>全てのアクションの基底クラス。</summary>
	/// @endif
	public class ActionBase
	{
		bool m_running = false;
		Node m_target_node;

		/// @if LANG_EN
		/// <summary>A tag value that can be used for searching this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションを検索するために使用するタグ値。</summary>
		/// @endif
		public int Tag;

		/// @if LANG_EN
		/// <summary>IsRunning is true when the action is active.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションがアクティブの場合、IsRunning は true になります。</summary>
		/// @endif
		public bool IsRunning { get { return m_running;}}

		// not in Target set property for visibility
		internal void set_target( Node value )
		{
			Common.Assert( !IsRunning );
			m_target_node = value;
		}

		/// @if LANG_EN
		/// <summary>The node affected by this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションによって影響を受けるノード。</summary>
		/// @endif
		public Node Target { get { return m_target_node; } }

		/// @if LANG_EN
		/// <summary>Kick the action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションをキック (実行) します。</summary>
		/// @endif
		public virtual void Run() 
		{
			m_running = true;
		}

		/// @if LANG_EN
		/// <summary>Stop the action (some types of actions stop themselves).</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションを停止します (いくつかのアクションは自動で停止します)。</summary>
		/// @endif
		public virtual void Stop()
		{
			m_running = false;
		}

		/// @if LANG_EN
		/// <summary>The update function for this action, called every frame.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>毎フレーム呼び出される、このアクションのUpdate関数。</summary>
		/// @endif
		public virtual void Update( float dt )
		{
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// The base class for actions with a finite duration.
	/// This is an abstract class.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>有限の期間を持つアクションの基底クラス。
	/// 抽象クラス。
	/// </summary>
	/// @endif
	public abstract class ActionWithDuration : ActionBase
	{
		protected float m_elapsed = 0.0f;

		/// @if LANG_EN
		/// <summary>This action's duration in seconds.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>秒単位でのアクションの期間。
		/// </summary>
		/// @endif
		public float Duration = 0.0f;

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();
			m_elapsed = 0.0f;
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// A one shot action that calls a user function.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した関数を呼び出す、ワンショットアクション。</summary>
	/// @endif
	public class CallFunc : ActionBase
	{
		/// @if LANG_EN
		/// <summary>The function this action calls.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションが呼び出す関数。</summary>
		/// @endif
		public System.Action Func;

		/// @if LANG_EN
		/// <summary>CallFunc constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public CallFunc( System.Action func )  
		{
			Func = func;
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;

			if ( Func != null )
				Func();

			Stop();
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// A wait action, that ends after a user specified duration.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した期間ののち終了する、待機アクション。</summary>
	/// @endif
	public class DelayTime : ActionWithDuration
	{
		/// @if LANG_EN
		/// <summary>DelayTime constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public DelayTime()
		{
		}

		/// @if LANG_EN
		/// <summary>DelayTime constructor.</summary>
		/// <param name="duration">The time to wait, in seconds.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// <param name="duration">秒単位での待機時間。</param>		
		/// @endif
		public DelayTime( float duration )
		{
			Duration = duration;
		}

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;
			
			m_elapsed += dt;

			if ( Duration <= m_elapsed )
				Stop();
			
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// The base class for generic interpolating actions, you just pass set/get functions.
	/// This is an abstract class.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>汎用的な補間アクションの基底クラス。set/get 関数をセットします。
	/// 抽象クラス。
	/// </summary>
	/// @endif
	public abstract class ActionTweenGeneric<T> : ActionWithDuration
	{
		/// @if LANG_EN
		/// <summary>The target value.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>ターゲットの値。</summary>
		/// @endif
		public T TargetValue;

		/// @if LANG_EN
		/// <summary>If true, the target value is an offset.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>trueの場合、ターゲットの値はオフセットです。</summary>
		/// @endif
		public bool IsRelative = false;

		protected T m_start_value;

		/// @if LANG_EN
		/// <summary>
		/// The tween/interpolation function, basically a f(x) function where f(x) goes from
		/// 0 to 1 when x goes from 0 to 1.
		/// 
		/// Example of function you can set DTween to:
		/// 
		/// (t) => Math.Linear(t);
		/// 
		/// (t) => Math.ExpEaseIn(t,1.0f);
		/// 
		/// (t) => Math.ExpEaseOut(t,1.0f);
		/// 
		/// (t) => Math.PowEaseIn(t,4.0f);
		/// 
		/// (t) => Math.PowEaseOut(t,4.0f);
		/// 
		/// (t) => Math.PowEaseInOut(t,4.0f);
		/// 
		/// (t) => Math.BackEaseIn(t,1.0f);
		/// 
		/// (t) => Math.BackEaseOut(t,1.0f);
		/// 
		/// (t) => Math.BackEaseInOut(t,1.0f);
		/// 
		/// (t) => Math.PowerfulScurve(t,3.7f,3.7f);
		/// 
		/// (t) => Math.Impulse(t,10.0f);
		/// 
		/// (t) => FMath.Sin(t*Math.Pi*0.5f); // for sin curve tween
		/// 
		/// More about tweens can be found int he cocos2d documentaiton which itselfs points to:
		/// 
		/// http://www.robertpenner.com/easing/easing_demo.html
		/// 
		/// http://www.robertpenner.com/easing/penner_chapter7_tweening.pdf
		/// 
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// Tween/補間の関数。基本的に xが0から1になるとき、f(x)が0から1になる f(x)です。
		/// 
		/// DTween を設定可能な関数の例です。
		/// 
		/// (t) => Math.Linear(t);
		/// 
		/// (t) => Math.ExpEaseIn(t,1.0f);
		/// 
		/// (t) => Math.ExpEaseOut(t,1.0f);
		/// 
		/// (t) => Math.PowEaseIn(t,4.0f);
		/// 
		/// (t) => Math.PowEaseOut(t,4.0f);
		/// 
		/// (t) => Math.PowEaseInOut(t,4.0f);
		/// 
		/// (t) => Math.BackEaseIn(t,1.0f);
		/// 
		/// (t) => Math.BackEaseOut(t,1.0f);
		/// 
		/// (t) => Math.BackEaseInOut(t,1.0f);
		/// 
		/// (t) => Math.PowerfulScurve(t,3.7f,3.7f);
		/// 
		/// (t) => Math.Impulse(t,10.0f);
		/// 
		/// (t) => FMath.Sin(t*Math.Pi*0.5f); // for sin curve tween
		/// 
		/// tweenの詳細については、cocos2dのドキュメントを参考にしてください。
		/// 
		/// http://www.robertpenner.com/easing/easing_demo.html
		/// 
		/// http://www.robertpenner.com/easing/penner_chapter7_tweening.pdf
		/// 
		/// </summary>
		/// @endif
		public DTween Tween = (t) => Math.PowEaseOut(t,4.0f);

		/// @if LANG_EN
		/// <summary>
		/// Target value set function delegate.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>set関数のデリゲート。</summary>
		/// @endif
		public delegate void DSet( T value );

		/// @if LANG_EN
		/// <summary>
		/// Target value get function delegate.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>get関数のデリゲート。</summary>
		/// @endif
		public delegate T DGet();

		/// @if LANG_EN
		/// <summary>
		/// The function that sets the current value as we execute the tween.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>tweenを実行しているとき、現在の値をセットする関数。</summary>
		/// @endif
		public DSet Set;

		/// @if LANG_EN
		/// <summary>
		/// The function that gets the current value as we execute the tween.
		/// </summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>tweenを実行しているとき、現在の値を取得する関数。</summary>
		/// @endif
		public DGet Get;

		public abstract void lerp( float alpha );

		/// @if LANG_EN
		/// <summary>ActionTweenGeneric constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public ActionTweenGeneric() 
		{
		}

		/// @if LANG_EN
		/// <summary>ActionTweenGeneric constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public ActionTweenGeneric( DGet dget, DSet dset ) 
		{
			Get = dget;
			Set = dset;
		}

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();

			m_start_value = Get();
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			if ( !IsRunning )
				return;

			
			m_elapsed += dt;
			
			float t;
			
			if(Duration==0.0f)
				t = 1.0f;
			else
				t = FMath.Clamp( m_elapsed / Duration, 0.0f, 1.0f );
			
			
			float alpha = Tween( t );

			lerp( alpha );
			
			
			if(Duration==0.0f)
			{
				Stop();
			}
			else
			{
				if ( m_elapsed / Duration > 1.0f )
					Stop();
			}
		}
	}

	// we have to manually instanciate ActionTweenGeneric*

	/// @if LANG_EN
	/// <summary>
	/// ActionTweenGenericVector2 does a simple element wise blend from start value to target value.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ActionTweenGenericVector2 はスタート値からターゲット値に移行する、シンプルなwiseブレンドを行います。</summary>
	/// @endif
	public class ActionTweenGenericVector2 : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value + TargetValue * alpha );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// ActionTweenGenericVector4 does a simple element wise blend from start value to target value.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ActionTweenGenericVector4 はスタート値からターゲット値に移行する、シンプルなwiseブレンドを行います。</summary>
	/// @endif
	public class ActionTweenGenericVector4 : ActionTweenGeneric<Vector4>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value + TargetValue * alpha );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// ActionTweenGenericVector2Scale is similar to ActionTweenGenericVector2, 
	/// but acts on scale values (multiplicative).
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ActionTweenGenericVector2Scale は ActionTweenGenericVector2 に似ていますが、
	/// スケール値 (乗法) で実行します。
	/// </summary>
	/// @endif
	public class ActionTweenGenericVector2Scale : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.Lerp( m_start_value, TargetValue, alpha ) );
			else Set( Math.Lerp( m_start_value, m_start_value * TargetValue, alpha ) );
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// ActionTweenGenericVector2Rotation interpolates a unit vector 
	/// (a unit vector interpreted as a rotation).
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>ActionTweenGenericVector2Rotation は単位ベクトルを補間します。単位ベクトルは回転で補間します。</summary>
	/// @endif
	public class ActionTweenGenericVector2Rotation : ActionTweenGeneric<Vector2>
	{
		public override void lerp( float alpha )
		{
			if ( IsRelative == false ) Set( Math.LerpUnitVectors( m_start_value, TargetValue, alpha ) );
			else Set( m_start_value.Rotate( Math.Angle( TargetValue ) * alpha ) );                                              
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually adds an offset to the current position.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>オフセットの値を現在の位置に徐々に加算していくアクション。</summary>
	/// @endif
	public class MoveBy : ActionTweenGenericVector2
	{
		/// @if LANG_EN
		/// <summary>MoveBy constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public MoveBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Position;}; 
			Set = ( Vector2 value ) => { Target.Position = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually moves position to the specified target.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した目標に徐々に移動するアクション。</summary>
	/// @endif
	public class MoveTo : ActionTweenGenericVector2
	{
		/// @if LANG_EN
		/// <summary>MoveTo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public MoveTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Position;}; 
			Set = ( Vector2 value ) => { Target.Position = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually applies an extra scale to the current scale.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>現在のスケールに徐々に追加のスケールを追加していくアクション。</summary>
	/// @endif
	public class ScaleBy : ActionTweenGenericVector2Scale
	{
		/// @if LANG_EN
		/// <summary>ScaleBy constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public ScaleBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Scale;}; 
			Set = ( Vector2 value ) => { Target.Scale = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually sets the scale to the specified value.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した値へスケールを徐々にセットしていくアクション。</summary>
	/// @endif
	public class ScaleTo : ActionTweenGenericVector2Scale
	{
		/// @if LANG_EN
		/// <summary>ScaleTo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public ScaleTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Scale;}; 
			Set = ( Vector2 value ) => { Target.Scale = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually adds an offset to the current skew.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>現在のスキュー値に徐々にオフセット値を加算していくアクション。</summary>
	/// @endif
	public class SkewBy : ActionTweenGenericVector2
	{
		/// @if LANG_EN
		/// <summary>SkewBy constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public SkewBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Skew;}; 
			Set = ( Vector2 value ) => { Target.Skew = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually sets the skew to the specified value.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した値へ徐々にスキューをセットしていくアクション。</summary>
	/// @endif
	public class SkewTo : ActionTweenGenericVector2
	{
		/// @if LANG_EN
		/// <summary>SkewTo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public SkewTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Skew;}; 
			Set = ( Vector2 value ) => { Target.Skew = value;};
		}
	}
/*
	/// @if LANG_EN
	/// <summary></summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// </summary>
	/// @endif
	public class MovePivotBy : ActionTweenGenericVector2 
	{ 
		/// @if LANG_EN
		/// <summary></summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public MovePivotBy( Vector2 target, float duration ) 
		{ 
			TargetValue = target;
			Duration = duration;
			IsRelative = true; 
			Get = () => { return Target.Pivot; }; 
			Set = ( Vector2 value ) => { Target.Pivot = value; }; 
		} 
	}

	/// @if LANG_EN
	/// <summary></summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// </summary>
	/// @endif
	public class MovePivotTo : ActionTweenGenericVector2 
	{ 
		/// @if LANG_EN
		/// <summary></summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>
		/// </summary>
		/// @endif
		public MovePivotTo( Vector2 target, float duration ) 
		{ 
			TargetValue = target;
			Duration = duration;
			IsRelative = false; 
			Get = () => { return Target.Pivot; }; 
			Set = ( Vector2 value ) => { Target.Pivot = value; }; 
		} 
	}
*/
	/// @if LANG_EN
	/// <summary>An action that gradually adds an offset to the current rotation.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>現在の回転値に徐々にオフセット値を加算していくアクション。</summary>
	/// @endif
	public class RotateBy : ActionTweenGenericVector2Rotation
	{
		/// @if LANG_EN
		/// <summary>RotateBy constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public RotateBy( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return Target.Rotation;}; 
			Set = ( Vector2 value ) => { Target.Rotation = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually sets the rotation to the specified value.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した値へ徐々に回転値をセットしていくアクション。</summary>
	/// @endif
	public class RotateTo : ActionTweenGenericVector2Rotation
	{
		/// @if LANG_EN
		/// <summary>RotateTo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public RotateTo( Vector2 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return Target.Rotation;}; 
			Set = ( Vector2 value ) => { Target.Rotation = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually adds an offset to the color.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>徐々に色を加算していくアクション。</summary>
	/// @endif
	public class TintBy : ActionTweenGenericVector4
	{
		/// @if LANG_EN
		/// <summary>TintBy constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public TintBy( Vector4 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = true; 
			Get = () => { return((SpriteBase)Target).Color;}; 
			Set = ( Vector4 value ) => { ((SpriteBase)Target).Color = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>An action that gradually sets the color to the specified value.</summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>指定した値に徐々に色をセットしていくアクション。</summary>
	/// @endif
	public class TintTo : ActionTweenGenericVector4
	{
		/// @if LANG_EN
		/// <summary>TintTo constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public TintTo( Vector4 target, float duration )
		{ 
			TargetValue = target; 
			Duration = duration; 
			IsRelative = false; 
			Get = () => { return((SpriteBase)Target).Color;}; 
			Set = ( Vector4 value ) => { ((SpriteBase)Target).Color = value;};
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// An action that runs a sequence of other actions, in order.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>順番に、他の一連のアクションを実行するためのアクション。</summary>
	/// @endif
	public class Sequence : ActionBase
	{
		List< ActionBase > m_actions = new List<ActionBase>();
		int m_current = 0;

		/// @if LANG_EN
		/// <summary>Sequence constructor.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。</summary>
		/// @endif
		public Sequence()  
		{
		}

		/// @if LANG_EN
		/// <summary>Add an action this actions sequence.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>引数の action をアクションシーケンスに追加します。</summary>
		/// @endif
		public void Add( ActionBase action )
		{
			Common.Assert( !IsRunning );
			m_actions.Add( action );
		}

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();

			m_current = 0;

			if ( m_actions.Count == 0 )
			{
				Stop();
				return;
			}

			// start first action
			Target.RunAction( m_actions[ m_current ] );
		}

		/// @if LANG_EN
		/// <summary>Stop this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションを停止します。</summary>
		/// @endif
		public override void Stop()
		{
			base.Stop();

			foreach ( ActionBase action in m_actions )
			{
				if ( action != null )
					action.Stop();
			}
		}

		// maybe Sequence should just add/remove from the Action manager for consistency? when pausing etc.

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update 関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			// kalin: why would we be Update()'d if we are not running?
			if ( !IsRunning )
				return;

//			System.Console.WriteLine( "Sequence.Update m_current="+   m_current);

			if ( m_actions[ m_current ].IsRunning == false )
			{
				// go to action n+1, stop if this was the last

				if ( m_current == m_actions.Count - 1 )
				{
					Stop();
					return;
				}

				m_current++;
				Target.RunAction( m_actions[ m_current ] );
			}
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// An action that repeats an other action forever.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>他のアクションを永続的に繰り返すアクション。</summary>
	/// @endif
	public class RepeatForever : ActionBase
	{
		/// @if LANG_EN
		/// <summary>The action to repeat.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>繰り返しのアクション。</summary>
		/// @endif
		public ActionBase InnerAction;

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();

			if ( InnerAction == null )
			{
				Stop();
				return;
			}

			Target.RunAction( InnerAction );
		}

		/// @if LANG_EN
		/// <summary>Stop this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>アクションを停止します。</summary>
		/// @endif
		public override void Stop()
		{
			base.Stop();

			if ( InnerAction != null )
				InnerAction.Stop();
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update 関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			if ( !IsRunning || !InnerAction.IsRunning )
				Run();
		}
	}

	/// @if LANG_EN
	/// <summary>
	/// An action that repeats an action a finite number of times.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>アクションを回数指定で繰り返すアクション。</summary>
	/// @endif
	public class Repeat : ActionBase
	{
		/// @if LANG_EN
		/// <summary>The action to repeat.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>繰り返すアクション。</summary>
		/// @endif
		public ActionBase InnerAction;

		/// @if LANG_EN
		/// <summary>The number of times we want to repeat.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>繰り返す回数。</summary>
		/// @endif
		public int Times = 0;

		int m_count = 0;

		/// @if LANG_EN
		/// <summary>
		/// Repeat constructor.
		/// </summary>
		/// <param name="inner_action">The action to repeat.</param>
		/// <param name="times">The number of times the action must be repeated.</param>
		/// @endif
		/// @if LANG_JA
		/// <summary>コンストラクタ。
		/// </summary>
		/// <param name="inner_action">繰り返すアクション。</param>
		/// <param name="times">繰り返す回数。</param>
		/// @endif
		public Repeat( ActionBase inner_action, int times )  
		{
			InnerAction = inner_action;
			Times = times;
		}

		/// @if LANG_EN
		/// <summary>Kick this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションをキックします。</summary>
		/// @endif
		public override void Run()
		{
			base.Run();

			if ( InnerAction == null )
			{
				Stop();
				return;
			}

			if ( Times <= 0 ) {
				Stop();
				return;
			}

			m_count = 0;

			Target.RunAction( InnerAction );
		}

		/// @if LANG_EN
		/// <summary>Stop this action.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>このアクションを停止します。</summary>
		/// @endif
		public override void Stop()
		{
			base.Stop();

			if ( InnerAction != null )
				InnerAction.Stop();
		}

		/// @if LANG_EN
		/// <summary>The update function.</summary>
		/// @endif
		/// @if LANG_JA
		/// <summary>Update 関数。</summary>
		/// @endif
		public override void Update( float dt )
		{
			if ( Times <= m_count ) {
				Stop();
				return;
			}
			
			if ( ( !IsRunning || !InnerAction.IsRunning ) 
				 && m_count < Times )
			{
				Target.RunAction( InnerAction ); // re-kick InnerAction

				++m_count;
			}
		}
	}

} // namespace Sce.PlayStation.HighLevel.GameEngine2D

